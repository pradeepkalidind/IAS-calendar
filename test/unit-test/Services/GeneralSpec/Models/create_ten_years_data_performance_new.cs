using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Calendar.Tests.Unit.CompactModels;
using Xunit;

namespace Calendar.Tests.Unit.Services.GeneralSpec.Models
{
    [XUnitCases]
    public class create_ten_years_data_performance_new : DbSpec
    {
        private static void SaveToDB(Guid userId, int year,int month, ISessionWrapper getSession)
        {
            var monthActivity = new MonthActivity(userId,year,month);
            for (var i = 0; i < DateTime.DaysInMonth(year,month); i++)
            {
                var dayActivity = new DayActivityBuilder().Day(i + 1)
                    .FirstActivityType(ActivityType.Work)
                    .SecondActivityType(ActivityType.NonWork)
                    .Location(l => l.FirstLocation("/Country/CN")
                        .FirstLocationArrivalTime(1)
                        .FirstLocationDepartureTime(2)
                        .SecondLocation("/Country/US")
                        .SecondLocationArrivalTime(3)
                        .SecondLocationDepartureTime(4)
                        .ThirdLocation("/Country/CA").ThirdLocationArrivalTime(5)).Build();
                monthActivity.Update(dayActivity);
            }
            getSession.Save(monthActivity);
        }

        private static void CreateYearsData()
        {
            var maxThreads = 20;
            ThreadPool.SetMaxThreads(maxThreads, maxThreads);
            var allUserIds = GetAllUserIds();
            foreach (var userId in allUserIds)
            {
//                CreateYearsDataForUser(userId);
                ThreadPool.QueueUserWorkItem(CreateYearsDataForUser, userId);
            }

            var placeHolder = 0;
            var availThreads = 0;

            while (true)
            {
                ThreadPool.GetAvailableThreads(out availThreads, out placeHolder);
                if (maxThreads == availThreads)
                {
                    break;
                }
                Thread.Sleep(5);
            }
        }

        private static void CreateYearsDataForUser(object userId)
        {
            const int year = 5;
            var session1 = DbHelper.GetSession();
            using (var transaction = session1.BeginTransaction())
            {
                for (int i = 0; i < 12 * year; i++)
                {
                    var addMonths = DateTime.Today.AddMonths(-i);
                    SaveToDB((Guid)userId, addMonths.Year, addMonths.Month, session1);
                }
                transaction.Commit();
            }
            session1.Dispose();
        }

        private static List<Guid> GetAllUserIds()
        {
            var guids = new List<Guid>();
//            using (var sqlConnection = new SqlConnection("Data Source=(local);Initial Catalog=iasaccount;User=ias;Password=pwc123#;"))
//            {
//                var sqlCommand = new SqlCommand("select PlatformUserId from Credential ", sqlConnection);
//                sqlConnection.Open();
//                var sqlDataReader = sqlCommand.ExecuteReader();
//                while (sqlDataReader.Read())
//                {
//                    guids.Add(sqlDataReader.GetGuid(0));
//                }
//            }
//            return guids;
            return Enumerable.Range(1, 1000).Select(i => Guid.NewGuid()).ToList();
        }

        private static int takeTime;
        [Fact(Skip = "run on demand")]
        public void test_create_ten_years_data_performance_new()
        {
            When("of", () =>
            {
                var startTime = DateTime.Now;
                CreateYearsData();

                takeTime = (int)(DateTime.Now - startTime).TotalSeconds;
            });
            Then("should_finish_on_time", () =>
            {
                var start = DateTime.Now;
                Console.WriteLine(takeTime);
            //                                               takeTime.ShouldBeLessThan(30);
            });
        }
    }
}
