using System.Linq;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Xunit;

namespace Calendar.Tests.Unit.CompactModels
{
    [XUnitCases]
    public class location_pattern_map : DbSpec
    {
        private readonly ISessionWrapper session;

        public location_pattern_map()
        {
            session = DbHelper.GetSession();
        }

        private void cleanup_db()
        {
            Cleaner.CleanTable(session, typeof(LocationPattern));
        }

        [Fact]
        public void test_location_pattern_map()
        {
            cleanup_db();
            Then("should_save_if_not_exist", () =>
            {
                LocationPatternMap.Init();
                var empty = LocationPattern.Empty();
                new LocationPatternMap().AddToMap(empty);
                var hash = empty.Hash;
                Assert.Equal(1, session.Query<LocationPattern>().Count(l => l.Hash == hash));
            });
            Then("should_get_if_not_exist", () =>
            {
                LocationPatternMap.Init();
                var empty = LocationPattern.Empty();
                var hash = empty.Hash;
                var locationPatternMap = new LocationPatternMap();
                Assert.NotNull(locationPatternMap.GetFromMap(hash));
            });
            Then("should_save_new_if_exist_hash_but_location_pattern_not_equal", () =>
            {
                LocationPatternMap.Init();
                var us = new LocationPattern("US");
                new LocationPatternMap().AddToMap(us);
                var empty = LocationPattern.Empty();
                empty.Hash = us.GetHashCode();
                new LocationPatternMap().AddToMap(empty);
                var hash = us.Hash;
                Assert.Equal(2, session.Query<LocationPattern>().Count(l => l.Hash == hash));
            });
        }
    }
}