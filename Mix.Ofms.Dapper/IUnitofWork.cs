using System;
using System.Data;

namespace Mix.Ofms.Dapper
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection Connection { get; set; }
        IDbTransaction Transaction { get; set; }

        

        void Commit();

        void RoolBack();


        void BeginTranscation();
    }
}
