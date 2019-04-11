using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Mix.Ofms.Dapper
{
    public class UnitOfWork : IUnitOfWork
    {


        private bool _disposed;

        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }

        public UnitOfWork(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
            Connection.Open();
            Transaction = Connection.BeginTransaction();
        }





        public void Commit()
        {
            try
            {
                Transaction.Commit();
            }
            catch
            {
                Transaction.Rollback();
                throw;
            }
            finally
            {
                Transaction.Dispose();
                Transaction = Connection.BeginTransaction();

            }
        }



        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (Transaction != null)
                    {
                        Transaction.Dispose();
                        Transaction = null;
                    }
                    if (Transaction != null)
                    {
                        Transaction.Dispose();
                        Transaction = null;
                    }
                }
                _disposed = true;
            }
        }

        public void RoolBack()
        {
            try
            {
                Transaction.Rollback();
            }
            catch
            {

                throw;
            }
        }

        public void BeginTranscation()
        {
            Transaction = Connection.BeginTransaction();
        }

        ~UnitOfWork()
        {
            dispose(false);
        }
    }
}
