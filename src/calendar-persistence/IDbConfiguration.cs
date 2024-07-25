using NHibernate;

namespace Calendar.Persistence
{
    public interface IDbConfiguration
    {
        ISessionWrapper GetSession();
        ISession GetNHibernateSession();
        IStatelessSession GetStatelessSession();
    }
}