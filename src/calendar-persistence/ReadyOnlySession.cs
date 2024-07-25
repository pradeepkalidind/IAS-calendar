using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.Type;

namespace Calendar.Persistence
{
    public class ReadyOnlySession : ISessionWrapper
    {
        private readonly ISessionWrapper session;

        public ReadyOnlySession(ISessionWrapper session)
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
            
        }

        public void SaveOrUpdate(object obj)
        {
        }

        public void Update(object obj)
        {
        }

        public ITransactionCommitter BeginTransaction()
        {
            return new ReadOnlyCommitter(session.BeginTransaction());
        }

        public ITransactionCommitter BeginTransaction(IsolationLevel isolationLevel)
        {
            return new ReadOnlyCommitter(session.BeginTransaction(isolationLevel));
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
        }

        public void Clear()
        {
            session.Clear();
        }

        public T Get<T>(object id)
        {
            return session.Get<T>(id);
        }

        public ISession Session { get { return session.Session; } }

        public int Delete(string query, object[] values, IType[] types)
        {
            return 0;
        }

        public int Delete(string query)
        {
            return 0;
        }
    }
}