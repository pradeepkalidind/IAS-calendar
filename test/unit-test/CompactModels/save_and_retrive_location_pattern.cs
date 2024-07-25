using System;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels
{
    [XUnitCases]
    public class save_and_retrive_location_pattern : DbSpec
    {
        private static ISessionWrapper session;
        private static readonly LocationPattern LocationPattern = new LocationPattern("BJ");

        private static void SaveLocationPattern(LocationPattern lp)
        {
            using (var session1 = DbConfigurationFactory.Get().GetSession())
            {
                using (var transaction = session1.BeginTransaction())
                {
                    session1.Save(lp);
                    transaction.Commit();
                }
            }
        }

        private static void cleanup_db()
        {
            Cleaner.CleanTable(DbHelper.GetSession(), typeof(LocationPattern));
        }

        private static LocationPattern firstLocationPattern = LocationPattern.Empty();

        [Fact]
        public void test_save_and_retrive_location_pattern()
        {
            cleanup_db();
            Given("context", () =>
            {
                session = DbHelper.GetSession();
                SaveLocationPattern(firstLocationPattern);
                SaveLocationPattern(LocationPattern);
            });
            Then("should_retrive", () =>
            {
                var hash = LocationPattern.Hash;
                var locationPatterns = session.Query<LocationPattern>().Where(l => l.Hash == hash);
                Assert.True(locationPatterns.Any());
                Assert.Equal(hash, locationPatterns.First().Hash);
                Console.Out.WriteLine(firstLocationPattern.Id);
                Console.Out.WriteLine(LocationPattern.Id);
            });
        }
    }
}