using System;
using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.Type;

namespace Calendar.Persistence
{
    public class LazyLoadedSession : ISessionWrapper
    {
        private readonly Func<ISessionWrapper> getSession;
        private ISessionWrapper sessionWrapper;

        public LazyLoadedSession(Func<ISessionWrapper> getSession)
        {
            this.getSession = getSession;
        }

        private ISessionWrapper session
        {
            get
            {
                if (sessionWrapper == null)
                {
                    sessionWrapper = getSession();
                }
                return sessionWrapper;
            }
        }

        public void SetSessionWrapper(Func<ISessionWrapper,ISessionWrapper> getSessionWrapper)
        {
            sessionWrapper = getSessionWrapper(session);
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
        }

        public void SaveOrUpdate(object obj)
        {
            session.SaveOrUpdate(obj);
        }

        public void Update(object obj)
        {
            session.Update(obj);
        }

        public ITransactionCommitter BeginTransaction()
        {
            return session.BeginTransaction();
        }

        public ITransactionCommitter BeginTransaction(IsolationLevel isolationLevel)
        {
            return session.BeginTransaction(isolationLevel);
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
        }

        public int Delete(string query)
        {
            return session.Delete(query);
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
            return session.Delete(query,values,types);
        }
    }
}