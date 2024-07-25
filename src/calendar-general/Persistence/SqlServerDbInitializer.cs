using FluentNHibernate.Cfg.Db;

namespace Calendar.General.Persistence
{
    public sealed class SqlServerDbInitializer: DbInitializer
    {
        public SqlServerDbInitializer(string connectionString) : base(
            MsSqlConfiguration.MsSql2008.ConnectionString(connectionString)
                .Raw("connection.release_mode", "on_close"))
        {
        }
    }
}