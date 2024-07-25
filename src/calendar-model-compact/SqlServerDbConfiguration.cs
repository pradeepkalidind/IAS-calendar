using FluentNHibernate.Cfg.Db;

namespace Calendar.Model.Compact
{
    public sealed class SqlServerDbConfiguration: DbConfiguration
    {
        public SqlServerDbConfiguration(string connectionString) : base(
            MsSqlConfiguration.MsSql2008.ConnectionString(connectionString)
                .Raw("connection.release_mode", "on_close"))
        {
        }
    }
}