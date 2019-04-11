using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Mix.Ofms.Dapper;
using Mix.Ofms.Entity.JWT;
using Mix.Ofms.Service;
using Mix.Ofms.Service.RepositoryBase;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Mix.Ofms.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var urls = "http://localhost:39287/";
            services.AddCors(options =>
              options.AddPolicy("MyDomain",
          builder => builder.WithOrigins(urls).AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin().AllowCredentials()));

            //读取配置文件
            var audienceConfig = Configuration.GetSection("JwtSetting");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],//发行人
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,

            };

            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            //这个集合模拟用户权限表,可从数据库中查询出来
            var permission = new List<Permission> {
                              new Permission {  Url="/", Name="admin"},
                              new Permission {  Url="/api/values", Name="admin"},
                              new Permission {  Url="/", Name="system"},
                              new Permission {  Url="/api/values1", Name="system"}
                          };
            //如果第三个参数，是ClaimTypes.Role，上面集合的每个元素的Name为角色名称，如果ClaimTypes.Name，即上面集合的每个元素的Name为用户名
            var permissionRequirement = new PermissionRequirement(
                "/api/denied", permission,
                ClaimTypes.Role,
                audienceConfig["Issuer"],
                audienceConfig["Audience"],
                signingCredentials,
                expiration: TimeSpan.FromDays(5)
                );

            services.AddAuthorization(options =>
            {

                options.AddPolicy("Permission",
                          policy => policy.Requirements.Add(permissionRequirement));

            }).AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                //不使用https
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = tokenValidationParameters;
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.Request.Path.Value.ToString() == "/api/logout")
                        {
                            var token = ((context as TokenValidatedContext).SecurityToken as JwtSecurityToken).RawData;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            var connecStr = Configuration.GetConnectionString("default");
            //注入授权Handler
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IUnitOfWork, UnitOfWork>((s) => new UnitOfWork(connecStr));
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddTransient<IUserRepository, UserRepository>()
                ;
            services.AddSingleton(permissionRequirement);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Main API v1.0", Version = "v1.0" });

                // Swagger 2.+ support
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);

            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("localhost");
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseStaticFiles();
            app.UseDefaultFiles("/swagger/index.html");

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");


            });

            app.UseMvc();

        }
    }
}
