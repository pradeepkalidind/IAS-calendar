using System;
using System.Linq;
using System.Threading.Tasks;
using Calendar.API.Test.Common;
using Calendar.General.Dto;
using Calendar.General.Persistence;
using Calendar.Model.Compact;
using Calendar.Models.Feed;
using Calendar.Persistence;
using Newtonsoft.Json;
using NHibernate;
using Xunit;

namespace Calendar.API.Test.Route
{
    public class AllUsersUsingNewUIFacts : ApiFactBase
    {
        private readonly Guid userId = new Guid("32e11ce2-eb60-4eb8-a24b-d8f76352f71f");
        private const string NOTE_DATE = "2012-04-01";

        [Fact]
        public async Task should_use_new_ui()
        {
            await SaveDaysByDay(
                "{\"updatedDays\":[{\"firstActivity\":\"/ActivityType/Work\",\"date\":\"2012/01/02\"}],\"deletedDays\":[]}");
            Assert.True(SaveToNewTable());
            await SaveDays("[{\"D\":\"2012/01/01\",\"A\":\"NW\",\"SA\":\"NW\",\"FL\":\"US\",\"N\":\"note\"}]");
            Assert.True(SaveToNewTable());
            const string expectedYear =
                "{\"year\":[{\"y\":2012,\"m\":1,\"d\":[{\"D\":\"1\",\"A\":\"NW\",\"SA\":\"NW\",\"AA\":\"1\",\"SAA\":\"1\",\"FL\":\"US\",\"N\":\"note\"}],\"nt\":0},{\"y\":2012,\"m\":2,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":3,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":4,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":5,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":6,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":7,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":8,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":9,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":10,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":11,\"d\":[],\"nt\":0},{\"y\":2012,\"m\":12,\"d\":[],\"nt\":0}]}";
            Assert.Equal(expectedYear, await GetYear("2012"));
            Assert.Equal("/activitytype/nonwork standard ", await DaysRead());
            Assert.Equal(1, await GetCompleteDays(2012));
            Assert.Equal(1, await GetCompleteDaysByMonth(2012));
            Assert.False(await IsCalendarComplete(2012));

            Assert.Equal(
                "{\"Days\":[{\"Date\":\"2012-01-01\",\"Location\":\"/Country/US\",\"ArrivalTime\":null,\"DepartureTime\":null,\"IsCovid\":\"FALSE\",\"Activities\":[{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"},{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"}],\"Note\":{\"Content\":\"note\"},\"Travels\":[]}],\"IasPlatformUser\":\"32e11ce2-eb60-4eb8-a24b-d8f76352f71f\"}",
                await UserCalendarAll());
            Assert.Equal(
                "{\"Days\":[{\"Date\":\"2012-01-01\",\"Location\":\"/Country/US\",\"ArrivalTime\":null,\"DepartureTime\":null,\"IsCovid\":\"FALSE\",\"Activities\":[{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"},{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"}],\"Note\":{\"Content\":\"note\"},\"Travels\":[]}],\"IasPlatformUser\":\"32e11ce2-eb60-4eb8-a24b-d8f76352f71f\"}",
                await UserCalendar());
            Assert.Equal(
                "{\"Days\":[{\"Date\":\"2012-01-01\",\"Location\":\"/Country/US\",\"ArrivalTime\":null,\"DepartureTime\":null,\"IsCovid\":\"FALSE\",\"Activities\":[{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"},{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"}],\"Note\":{\"Content\":\"note\"},\"Travels\":[]}],\"DeletedDays\":[],\"IasPlatformUser\":\"32e11ce2-eb60-4eb8-a24b-d8f76352f71f\"}",
                await UserCalendarByDates());
            Assert.Equal(
                "{\"Users\":[{\"Days\":[{\"Date\":\"2012-01-01\",\"Location\":\"/Country/US\",\"ArrivalTime\":null,\"DepartureTime\":null,\"IsCovid\":\"FALSE\",\"Activities\":[{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"},{\"Type\":\"/ActivityType/NonWork\",\"FirstAllocation\":1.0,\"SecondAllocation\":0.0,\"FirstAllocationAttr\":\"1\",\"SecondAllocationAttr\":\"0\"}],\"Note\":{\"Content\":\"note\"},\"Travels\":[]}],\"IasPlatformUser\":\"32e11ce2-eb60-4eb8-a24b-d8f76352f71f\"}]}",
                await UserCalendarByUserDayPairs());
        }

        [Fact]
        public async Task should_note()
        {
            await SaveNote("note");
            Assert.True(SaveNoteToNewTable());
            Assert.Equal("[{\"content\":\"note\",\"date\":\"2012-04-01\"}]",await GetNote());
            Assert.Equal(1,await GetNoteCount());
            
           await DeleteNote();
            
            Assert.Equal("[]",await GetNote());
            Assert.Equal(0,await GetNoteCount());
        }

        private async Task SaveNote(string content)
        {
            Clear();
            await new NoteApi().SaveNote(content, GetClient(""), NOTE_DATE, userId);
        }

        private async Task<string> GetNote()
        {
            return await NoteApi.GetNote(GetClient(""), userId);
        }

        private async Task DeleteNote()
        {
            await NoteApi.DeleteNote(GetClient(""), NOTE_DATE, userId);
        }

        private async Task<int> GetNoteCount()
        {
            var response = await GetClient("").RetrieveData("CalendarNote/Count", userId, false);
            return int.Parse(response);
        }

        private async Task<string> UserCalendarAll()
        {
            var response = await GetClient("").UserCalendarAll(userId);
            var userCalendarAll = JsonConvert.SerializeObject(response);
            return userCalendarAll;
        }

        private async Task<string> UserCalendar()
        {
            var response =await GetClient("").UserCalendar(userId, "2012-01-01", "2012-01-01");
            return JsonConvert.SerializeObject(response);
        }

        private async Task<string> UserCalendarByDates()
        {
            var response = await GetClient("").UserCalendar(userId, DateTime.UtcNow.AddHours(-1).Ticks, DateTime.UtcNow.Ticks);
            return JsonConvert.SerializeObject(response);
        }

        private async Task<string> UserCalendarByUserDayPairs()
        {
            var userDayPairs = new[]
            {
                new { UserId = userId, Day = "2012-01-01" },
                new { UserId = Guid.NewGuid(), Day = "2012-01-02" }
            };
            var response = await GetClient("").UserCalendar(JsonConvert.SerializeObject(userDayPairs));
            return JsonConvert.SerializeObject(response);
        }

        private async Task<int> GetCompleteDays(int year)
        {
            return await GetClient("").GetCompleteDays(userId, new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }

        private async Task<int> GetCompleteDaysByMonth(int year)
        {
            var response = await GetClient("")
                .GetCompleteDaysByMonth(
                    userId,
                    new DateTime(year, 1, 1),
                    new DateTime(year, 1, 31));
            return response.FirstOrDefault();
        }

        private async Task<bool> IsCalendarComplete(int year)
        {
            return await GetClient("").IsCalendarComplete(userId, new DateTime(year, 1, 1), new DateTime(year, 1, 1));
        }

        private async Task<string> DaysRead()
        {
            var daysRead =await GetClient("").DaysRead(userId.ToString(), "2012-01-01", "2012-01-02");
            var dto =  JsonConvert.DeserializeObject<ChangedDaysDto>(daysRead);
            var dayDto = dto?.updatedDays.First();
            return $"{dayDto?.firstActivity} {dayDto?.type} {dayDto?.enterFrom}";
        }

        private async Task SaveDays(string daysBody)
        {
            Clear();
            var daysBodyParam = "daysBody=" + daysBody;
            await GetClient("").RetrieveData("Calendar/SaveDays", userId, daysBodyParam, false);
        }

        private static void Clear()
        {
            DeleteAll(DbConfigurationFactory.Get().GetSession(), typeof(MonthActivity));
            DeleteAll(DbConfigurationFactory.Get().GetSession(), typeof(Note));
            DeleteAll(DbConfigurationFactory.Get().GetSession(), typeof(UserActivity));
            DeleteAll(DbInitializerFactory.Get().GetSession(), typeof(FeedEntry));
        }

        private static void DeleteAll(ISessionWrapper session, Type type)
        {
            session.Delete($"from {type.Name}");
        }

        private async Task SaveDaysByDay(string @params)
        {
            await GetClient("").DaysSave(userId.ToString(), @params);
        }

        private static int GetByNewApi()
        {
            var query = DbConfigurationFactory.Get().GetSession()
                .CreateSQLQuery("Select Count(*) as c from MonthActivity")
                .AddScalar("c", NHibernateUtil.Int32);
            return Convert.ToInt32(query.UniqueResult());
        }

        private bool SaveNoteToNewTable()
        {
            return GetNoteByNewApi() > 0;
        }

        private async Task<string> GetYear(string year)
        {
            var yearParam = "year=" + year;
            return await GetClient("").RetrieveData("Calendar/GetYear", userId, yearParam, false);
        }

        private static bool SaveToNewTable()
        {
            return GetByNewApi() > 0;
        }

        private static int GetNoteByNewApi()
        {
            var query = DbConfigurationFactory.Get().GetSession()
                .CreateSQLQuery("Select Count(*) as c from Note")
                .AddScalar("c", NHibernateUtil.Int32);
            return Convert.ToInt32(query.UniqueResult());
        }
    }
}