using System;
using System.Linq;
using Calendar.General.Persistence;
using Calendar.Persistence;

namespace Calendar.Tests.Unit
{
    public class PersistenceSpec : DbSpec
    {
        protected ISessionWrapper session;
        public PersistenceSpec()
        {
            session = DbInitializerFactory.Get().GetSession();
            Feed.ModelTypes.Get().ForEach(Cleaner.CleanTable);
        }
    }

    internal class Cleaner
    {
        public static void CleanTable(Type type)
        {
            var session = DbInitializerFactory.Get().GetSession();
            session.DeleteAll(type);
        }

        public static void CleanTable(ISessionWrapper session,params Type[] types)
        {
            types.ToList().ForEach(session.DeleteAll);
        }
    }
}
