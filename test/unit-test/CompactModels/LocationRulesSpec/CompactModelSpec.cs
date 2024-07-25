using System;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Calendar.Service.Configuration;
using Moq;

namespace Calendar.Tests.Unit.CompactModels.LocationRulesSpec
{
    public class CompactModelSpec: DbSpec, IDisposable
    {
        protected static ISessionWrapper Session;

        public CompactModelSpec()
        {
            Session = DbHelper.GetSession();
            MockAppSettings();
            ClearDB();
        }

        private static void MockAppSettings()
        {
            var appConfig = new Mock<IAppConfiguration>();
            appConfig.Setup(a => a.ApiManagementHostUrl).Returns("https://apimhost");
            appConfig.Setup(a => a.CalendarificApiKey).Returns("key");
            appConfig.Setup(a => a.CalendarificApimSubscriptionKey).Returns("key2");
            AppSettings.InitConfig(appConfig.Object);
        }

        private void ClearDB()
        {
            CleanTable(Session, typeof(MonthActivity),
                typeof(UserActivity),
                typeof(UserActivityArchived),
                typeof(Note),
                typeof(LocationPattern),
                typeof(NationalHolidaySetting),
                typeof(DefaultWorkDays),
                typeof(LastModifiedYearMonth),
                typeof(VisitedUser));
        }
        
        public static void CleanTable(ISessionWrapper sessionWrapper, params Type[] types)
        {
            types.ToList().ForEach(sessionWrapper.DeleteAll);
            sessionWrapper.Session.Flush();
        }

        public void Dispose()
        {
            ClearDB();
        }
    }
}
