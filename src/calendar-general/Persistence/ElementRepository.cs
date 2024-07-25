using System;
using Calendar.Persistence;
using NHibernate.Criterion;

namespace Calendar.General.Persistence
{
    public class ElementRepository
    {
        private readonly ISessionWrapper session;

        public ElementRepository(ISessionWrapper session)
        {
            this.session = session;
        }

        public T UniqueResult<T>(Guid userId, DateTime dateTime) where T : class
        {
            var criteria = session.CreateCriteria<T>();
            criteria.Add(Restrictions.And(Restrictions.Eq("UserId", userId),Restrictions.Eq("Date",dateTime)));
            return criteria.UniqueResult<T>();
        }
    }
}