using Calendar.Persistence;

namespace Calendar.General.Persistence
{
    public static class DbInitializerFactory
    {
        private static IDbConfiguration instance;

        public static void CreateSqlServer(string connectionString)
        {
            instance = new SqlServerDbInitializer(connectionString);
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