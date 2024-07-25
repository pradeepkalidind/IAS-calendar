using System;
using Calendar.General.Persistence;
using Calendar.Model.Compact;

namespace Calendar.Tests.Unit
{
    public class DbSpec
    {
        protected DbSpec()
        {
            DbInitializerFactory.CreateWith(new SqliteDbInitializer());
            DbConfigurationFactory.CreateWith(new SqliteDbConfiguration());
            LocationPatternMap.Init();
        }

        protected static void Then(string name, Action assertion)
        {
            assertion();
        }

        protected static void Given(string name, Action establish)
        {
            establish();
        }

        protected static void When(string name, Action action)
        {
            action();
        }
    }
}