using Calendar.Persistence;

namespace Calendar.Model.Compact
{
    public static class DbConfigurationFactory
    {
        private static IDbConfiguration instance;

        public static void CreateSqlServer(string connectionString)
        {
            instance = new SqlServerDbConfiguration(connectionString);
        }

        public static void CreateWith(IDbConfiguration configuration)
        {
            instance = configuration;
        }
        
        public static IDbConfiguration Get()
        {
            return instance;
        }
    }
}