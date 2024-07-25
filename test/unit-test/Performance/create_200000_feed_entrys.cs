using System;
using System.Threading;
using Calendar.General.Persistence;
using Calendar.Models.Feed;
using Calendar.Persistence;
using Xunit;

namespace Calendar.Tests.Unit.Performance
{
    [XUnitCases]
    public class create_200000_feed_entrys : DbSpec
    {
        private readonly ISessionWrapper session;

        public create_200000_feed_entrys()
        {
            session = DbInitializerFactory.Get().GetSession();
        }

        private void CreateFeedData(object input)
        {
            const int UserCount = 100;
            const int DayCount = 365;
            for (int j = 0; j < UserCount; j++)
            {
                var userId = Guid.NewGuid();
                for (var i = 0; i < DayCount; i++)
                {
                    session
                        .Save(new FeedEntry(userId, DateTime.Today.AddDays(-i), DateTime.UtcNow.AddHours(-2).AddMinutes(-i).Ticks));
                }
            }

        }

        private static void DoUntilFinish(int maxThreads)
        {
            while (true)
            {
                int placeHolder;
                int availThreads;
                ThreadPool.GetAvailableThreads(out availThreads, out placeHolder);
                if (maxThreads == availThreads)
                {
                    break;
                }

                Thread.Sleep(5);
            }
        }

        [Fact(Skip = "For performance test")]
        public void test_create_200000_feed_entrys()
        {
            Given("context", () =>
            {
                const int maxThreads = 10;
                ThreadPool.SetMaxThreads(maxThreads, maxThreads);
                for (var i = 0; i < maxThreads; i++)
                {
                    ThreadPool.QueueUserWorkItem(CreateFeedData, null);
                }

                DoUntilFinish(maxThreads);
            });
            Then("should_succeed", () => { });
        }
    }
}