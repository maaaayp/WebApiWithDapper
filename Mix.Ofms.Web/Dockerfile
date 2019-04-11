FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 56312
EXPOSE 44386

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Mix.Ofms.Web/Mix.Ofms.Web.csproj", "Mix.Ofms.Web/"]
RUN dotnet restore "Mix.Ofms.Web/Mix.Ofms.Web.csproj"
COPY . .
WORKDIR "/src/Mix.Ofms.Web"
RUN dotnet build "Mix.Ofms.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Mix.Ofms.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Mix.Ofms.Web.dll"]