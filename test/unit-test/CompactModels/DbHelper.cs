using Calendar.Model.Compact;
using Calendar.Persistence;

namespace Calendar.Tests.Unit.CompactModels
{
    public class DbHelper
    {
        public static ISessionWrapper GetSession()
        {
            return DbConfigurationFactory.Get().GetSession();
        }
    }
}