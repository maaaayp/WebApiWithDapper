using Dapper;
using Mix.Ofms.Dapper;
using Mix.Ofms.Entity.Identity;
using Mix.Ofms.Service.RepositoryBase;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Mix.Ofms.Service
{
    public class UserRepository : IUserRepository
    {

        IUnitOfWork _unitOfWork;
        IRepositoryBase<User> _repositoryBaseUser;
        IRepositoryBase<UserSetting> _repositoryBaseUserSetting;
        public UserRepository(
            IUnitOfWork unitOfWork,
            IRepositoryBase<User> repositoryBaseUser,
            IRepositoryBase<UserSetting> repositoryBaseUserSetting
            )
        {
            _unitOfWork = unitOfWork;
            _repositoryBaseUser = repositoryBaseUser;
            _repositoryBaseUserSetting = repositoryBaseUserSetting;
        }


        public async Task<List<User>> GetList()
        {

            var userId = await _repositoryBaseUser.InsertAndGetIdAsync(new User
            {
                Pwd = "test",
                NickName = "test",
                Role = 0,
                Phone = "13818605348"
            });
            var setting = await _repositoryBaseUserSetting.InsertAndGetIdAsync(new UserSetting
            {
                UserId = (long)userId,
                OpenAunt = true
            });
            var list = await _unitOfWork.Connection.QueryAsync<User>("SELECT * FROM Users;", null, _unitOfWork.Transaction);

            _unitOfWork.RoolBack();
            list = await _unitOfWork.Connection.QueryAsync<User>("SELECT * FROM Users;", null, _unitOfWork.Transaction);
            return list.AsList();
        }
    }
}
