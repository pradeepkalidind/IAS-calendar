using System;
using System.Threading;
using Calendar.Model.Compact;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels
{
    [XUnitCases]
    public class concurrent_location_pattern_save : CompactModelSpec
    {
        private static int errorCount = 0;

        private static void AddToMap(object state)
        {
            LocationPatternMap.Init();
            try
            {
                new LocationPatternMap().AddToMap(LocationPattern.Empty());
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);
                errorCount++;
            }

        }
        [Fact(Skip = "will fix this later")]
        public void test_concurrent_location_pattern_save()
        {
            Given("Init", () =>
            {
                LocationPatternMap.Init();
            });
            Then("should", () =>
            {
                const int maxThreads = 11;
                ThreadPool.SetMaxThreads(maxThreads, maxThreads);

                for (var i = 0; i < 100; i++)
                {
                    ThreadPool.QueueUserWorkItem(AddToMap, null);
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

                Assert.Equal(0, errorCount);
            });
        }
    }
}