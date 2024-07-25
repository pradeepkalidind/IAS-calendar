using System;
using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.Type;

namespace Calendar.Persistence
{
    public interface ISessionWrapper : IDisposable
    {
        IQueryable<T> Query<T>();
        void Save(object obj);
        void SaveOrUpdate(object obj);
        void Update(object obj);
        ITransactionCommitter BeginTransaction();
        ITransactionCommitter BeginTransaction(IsolationLevel isolationLevel);
        ISQLQuery CreateSQLQuery(string queryString);
        ICriteria CreateCriteria<T>() where T : class;
        void Delete(object obj);
        int Delete(string query, object[] values, IType[] types);
        int Delete(string query);
        void Clear();
        T Get<T>(object id);
        ISession Session { get; }
    }

    public interface ITransactionCommitter : IDisposable
    {
        void Commit();
    }

    public class ReadOnlyCommitter :ITransactionCommitter
    {
        private readonly ITransactionCommitter transaction;

        public ReadOnlyCommitter(ITransactionCommitter transaction)
        {
            this.transaction = transaction;
        }

        public void Dispose()
        {
            transaction.Dispose();
        }

        public void Commit()
        {
        }
    }
    public class TransactionCommitter : ITransactionCommitter
    {
        private readonly ITransaction transaction;

        public TransactionCommitter(ITransaction transaction)
        {
            this.transaction = transaction;
        }

        public void Dispose()
        {
            transaction.Dispose();
        }

        public void Commit()
        {
            transaction.Commit();
        }
    }
}