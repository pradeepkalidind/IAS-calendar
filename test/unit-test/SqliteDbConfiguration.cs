using System.Data.Common;
using Calendar.Model.Compact;
using Calendar.Persistence;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;

namespace Calendar.Tests.Unit
{
    public sealed class SqliteDbConfiguration : DbConfiguration
    {
        private DbConnection connection;

        public SqliteDbConfiguration() : base(SQLiteConfiguration.Standard
            .InMemory()
            .ConnectionString(
                "Data Source=:memory:;Version=3;New=True;DateTimeFormatString=yyyy-MM-dd HH:mm:ss.FFFFFFF;BinaryGUID=False"))
        {
            connection = ((ISessionFactoryImplementor) sessionFactory).ConnectionProvider.GetConnection();
            new SchemaExport(configuration).Create(false, true, connection);
        }

        public override ISessionWrapper GetSession()
        {
            lock (typeof(SqliteDbConfiguration))
            {
                return new PersistSession(sessionFactory
                    .WithOptions()
                    .Connection(connection)
                    .FlushMode(FlushMode.Commit)
                    .OpenSession());
            }
        }

        public override ISession GetNHibernateSession()
        {
            lock (typeof(SqliteDbConfiguration))
            {
                return sessionFactory
                    .WithOptions()
                    .Connection(connection)
                    .FlushMode(FlushMode.Commit)
                    .OpenSession();
            }
        }
    }
}