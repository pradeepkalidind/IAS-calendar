using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Calendar.Model.Compact;
using NHibernate;
using PWC.IAS.Calendar.Client;
using Xunit;

namespace Calendar.Tests.Unit.Performance
{
    [XUnitCases]
    public class save_and_retrieve_1000_user_by_day_for_new_api : DbSpec
    {
        private const int USER_COUNT = 1000;
        private const int YEAR = 2012;
        private const int MONTH = 1;

        private static void RetrieveByUser(IasCalendarResource resource, Guid userId)
        {
            var yearParam = "year=" + YEAR;
            resource.RetrieveData("Calendar/GetYear", userId, yearParam, false);
        }

        private static void SaveByUser(IasCalendarResource resource)
        {
            var daysInMonth = DateTime.DaysInMonth(YEAR, MONTH);
            var userId = Guid.NewGuid();
            SaveByDay(daysInMonth, resource, userId);
        }

        private static void SaveByMonth(int daysInMonth, IasCalendarResource resource, Guid userId)
        {
            var list = new List<string>();
            for (var dayIndex = 0; dayIndex < daysInMonth; dayIndex++)
            {
                var dayBody =
                    string.Format("{{\"D\":\"2012/01/{0}\",\"A\":\"W\",\"SA\":\"W\",\"FL\":\"BJ\",\"FLA\":1,\"FLD\":2,\"SL\":\"NY\",\"SLA\":3,\"SLD\":4}}",
                        (dayIndex + 1).ToString("00"));
                list.Add(dayBody);
            }

            var daysBodyParam = "daysBody=[" + string.Join(",", list) + "]";
            resource.RetrieveData("Calendar/SaveDays", userId, daysBodyParam, false);
        }

        private static void SaveByDay(int daysInMonth, IasCalendarResource resource, Guid userId)
        {
            for (var dayIndex = 0; dayIndex < daysInMonth; dayIndex++)
            {
                var dayBody =
                    string.Format("[{{\"D\":\"2012/01/{0}\",\"A\":\"W\",\"SA\":\"W\",\"FL\":\"BJ\",\"FLA\":1,\"FLD\":2,\"SL\":\"NY\",\"SLA\":3,\"SLD\":4}}]",
                        (dayIndex + 1).ToString("00"));
                var daysBodyParam = "daysBody=" + dayBody;
                resource.RetrieveData("Calendar/SaveDays", userId, daysBodyParam, false);
            }
        }

        private static IasCalendarResource CreateResource(string area)
        {
            return new IasCalendarResource(new HttpClient { BaseAddress = new Uri("http://localhost/calendar-service" + "/" + area) }, "mytaxes", "mytaxes123");
        }

        [Fact(Skip = "For performance tes")]
        public void test_save_and_retrieve_1000_user_by_day_for_new_api()
        {
            Then("save", () =>
            {
                var resource = CreateResource("new");
                var userIds = DbConfigurationFactory.Get().GetSession().CreateSQLQuery("select distinct userid from [MonthActivity]")
                    .AddScalar("userid", NHibernateUtil.Guid)
                    .List().Cast<Guid>().ToArray();
                var start = DateTime.Now;

                for (var userIndex = 0; userIndex < USER_COUNT; userIndex++)
                {
                    SaveByUser(resource);
                }

                // for (var userIndex = 0; userIndex < userIds.Length; userIndex++)
                // {
                //     var userId = userIds[userIndex];
                //     RetrieveByUser(resource, userId);
                // }

                Console.Out.WriteLine(DateTime.Now.Subtract(start).ToString());
            });
        }
    }
}