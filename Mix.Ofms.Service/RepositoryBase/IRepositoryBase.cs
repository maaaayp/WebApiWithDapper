using System.Threading.Tasks;

namespace Mix.Ofms.Service.RepositoryBase
{
    public interface IRepositoryBase<T>
    {
        Task<object> InsertAndGetIdAsync(T t);
    }
}
