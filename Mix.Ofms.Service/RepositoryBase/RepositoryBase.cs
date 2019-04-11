using Dapper;
using Mix.Ofms.Dapper;
using Mix.Ofms.Dapper.SqlGenerator;
using System.Threading.Tasks;

namespace Mix.Ofms.Service.RepositoryBase
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        IUnitOfWork _unitOfWork;

        public RepositoryBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public async Task<object> InsertAndGetIdAsync(T t)
        {
            var sql = new MySqlGeneratorHelper<T>().GetInsert(t);
            var id = await _unitOfWork.Connection.ExecuteScalarAsync(sql.SqlBuilder.ToString(), sql.Param, _unitOfWork.Transaction);
            return id;
        }
    }
}
