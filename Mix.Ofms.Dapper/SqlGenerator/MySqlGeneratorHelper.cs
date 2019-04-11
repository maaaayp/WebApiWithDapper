using MicroOrm.Dapper.Repositories.SqlGenerator;
using System.Collections.Generic;

namespace Mix.Ofms.Dapper.SqlGenerator
{
    public class MySqlGeneratorHelper<T> where T : class
    {
        static SqlProvider eSqlConnector = SqlProvider.MySQL;
        public SqlQuery GetInsert(T t)
        {
            var userSqlGenerator = new SqlGenerator<T>(eSqlConnector, true);
            var sqlQuery = userSqlGenerator.GetInsert(t);
            sqlQuery.SqlBuilder.Append(";SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS ID");
            return sqlQuery;
        }

        public SqlQuery GetBulkInsert(List<T> t)
        {
            var userSqlGenerator = new SqlGenerator<T>(eSqlConnector, true);
            var sqlQuery = userSqlGenerator.GetBulkInsert(t);
            return sqlQuery;
        }
    }
}
