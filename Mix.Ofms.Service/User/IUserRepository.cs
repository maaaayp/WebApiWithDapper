using Mix.Ofms.Entity.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Mix.Ofms.Service
{
    public interface IUserRepository
    {
        Task<List<User>> GetList();
    }
}
