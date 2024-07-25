using System;
using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Type;

namespace Calendar.Persistence
{
    public class PersistSession : ISessionWrapper
    {
        private readonly ISession session;

        public PersistSession(ISession session)
        {
            this.session = session;
        }
        
        public void Dispose()
        {
            session.Dispose();
        }

        public IQueryable<T> Query<T>()
        {
            return session.Query<T>();
        }

        public void Save(object obj)
        {
            session.Save(obj);
            session.Flush();
        }

        public void SaveOrUpdate(object obj)
        {
            session.SaveOrUpdate(obj);
            session.Flush();
        }

        public void Update(object obj)
        {
            session.Update(obj);
            session.Flush();
        }

        public ITransactionCommitter BeginTransaction()
        {
            return new TransactionCommitter(session.BeginTransaction());
        }

        public ITransactionCommitter BeginTransaction(IsolationLevel isolationLevel)
        {
            return new TransactionCommitter(session.BeginTransaction(isolationLevel));
        }

        public ISQLQuery CreateSQLQuery(string queryString)
        {
            return session.CreateSQLQuery(queryString);
        }

        public ICriteria CreateCriteria<T>() where T : class
        {
            return session.CreateCriteria<T>();
        }

        public void Delete(object obj)
        {
            session.Delete(obj);
            session.Flush();
        }

        public int Delete(string query)
        {
            var count = session.Delete(query);
            session.Flush();
            return count;
        }

        public void Clear()
        {
            session.Clear();
        }

        public T Get<T>(object id)
        {
            return session.Get<T>(id);
        }

        public ISession Session { get { return session; } }

        public int Delete(string query, object[] values, IType[] types)
        {
            var count = session.Delete(query, values, types);
            session.Flush();
            return count;
        }
    }
}