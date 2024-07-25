using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Calendar.Client.Schema;
using Newtonsoft.Json;

namespace PWC.IAS.Calendar.Client
{
    public class IasCalendarResource
    {
        private readonly HttpHelper httpHelper;

        public IasCalendarResource(HttpClient client, string userName, string password)
        {
            httpHelper = new HttpHelper(client, userName, password);
        }

        public async Task<string> RetrieveData(string relativeUrl, Guid token, string parameters, bool isStaticContent)
        {
            var resourceUrl = relativeUrl + "?token=" + token;
            return await RetrieveData(resourceUrl, parameters);
        }

        public async Task<string> Merge(string relativeUrl, string parameters, bool isStaticContent)
        {
            return await RetrieveData(relativeUrl, parameters);
        }

        public async Task<string> RetrieveData(string relativeUrl, Guid token, bool isStaticContent)
        {
            var resourceUrl = relativeUrl + "?token=" + token;
            var request = httpHelper.CreateRequestWithCredential(resourceUrl, HttpMethod.Post);
            var response = await httpHelper.SendRequest(request);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> RetrieveTravelYears(string uri, Guid userId)
        {
            var response = await RetrieveTravelYearsResponse(uri, userId);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> RetrieveTravelYearsResponse(string uri, Guid userId)
        {
            var request = httpHelper.CreateRequestWithCredential(uri, HttpMethod.Post);
            request.Content = new StringContent(JsonConvert.SerializeObject(new { token = userId }), Encoding.UTF8, "application/json");
            return await httpHelper.SendRequest(request);
        }

        public async Task<bool> IsCalendarComplete(Guid iasPlatformUserId, DateTime start, DateTime end)
        {
            const string urlTemplate = "{0}?userId={1}&startYear={2}&startMonth={3}&endYear={4}&endMonth={5}";
            var resourceUrl = string.Format(urlTemplate, "CalendarStatus/IsCompleted", iasPlatformUserId, start.Year, start.Month, end.Year, end.Month);
            var response = await httpHelper.SendRequest(resourceUrl, HttpMethod.Get);
            return (await response.Content.ReadAsStringAsync()).ToLower() == "true";
        }

        public async Task<int> GetCompleteDays(Guid iasPlatformUserId, DateTime periodStart, DateTime periodEnd)
        {
            const string urlTemplate = "{0}?userId={1}&startYear={2}&startMonth={3}&startDay={4}&endYear={5}&endMonth={6}&endDay={7}";
            var resourceUrl = string.Format(urlTemplate, "CalendarStatus/GetCompleteDays", iasPlatformUserId,
                periodStart.Year, periodStart.Month, periodStart.Day,
                periodEnd.Year, periodEnd.Month, periodEnd.Day);
            var response = await httpHelper.SendRequest(resourceUrl, HttpMethod.Get);
            var data = await response.Content.ReadAsStringAsync();

            return int.TryParse(data, out var result) ? result : 0;
        }

        public async Task<int[]> GetCompleteDaysByMonth(Guid iasPlatformUserId, DateTime periodStart, DateTime periodEnd)
        {
            const string urlTemplate = "{0}?userId={1}&startYear={2}&startMonth={3}&startDay={4}&endYear={5}&endMonth={6}&endDay={7}";
            var resourceUrl = string.Format(urlTemplate, "CalendarStatus/GetCompleteDaysByMonth", iasPlatformUserId,
                periodStart.Year, periodStart.Month, periodStart.Day,
                periodEnd.Year, periodEnd.Month, periodEnd.Day);
            var response = await httpHelper.SendRequest(resourceUrl, HttpMethod.Get);
            var data = await response.Content.ReadAsStringAsync();
            return data.Split(',').Select(int.Parse).ToArray();
        }

        public async Task<UserCalendar> UserCalendarAll(Guid iasPlatformUserId)
        {
            var calendarUrl = $"IASPlatformUser/{iasPlatformUserId}/Calendar";
            return await UserCalendarFromUrl<UserCalendar>(calendarUrl);
        }

        public async Task<CalendarRoot> UserCalendar(string userDayPairs)
        {
            var calendarUrl = $"Calendars?userDayPairs={userDayPairs}";
            return await UserCalendarRootFromUrl(calendarUrl);
        }

        public async Task<UserCalendar> UserCalendar(Guid iasPlatformUserId, string fromDate, string toDate)
        {
            var calendarUrl = $"IASPlatformUser/{iasPlatformUserId}/Calendar/{fromDate}/{toDate}";
            return await UserCalendarFromUrl<UserCalendar>(calendarUrl);
        }

        public async Task<UserCalendarWithDeleted> UserCalendar(Guid iasPlatformUserId, long changedStart, long changedEnd)
        {
            var calendarUrl = $"IASPlatformUser/{iasPlatformUserId}/CalendarByChange/{changedStart}/{changedEnd}";
            return await UserCalendarFromUrl<UserCalendarWithDeleted>(calendarUrl);
        }

        public async Task<string> GetIsVisitedUser(Guid userId)
        {
            var resourceUrl = $"User/{userId}/IsVisited";
            var response = await httpHelper.SendRequest(resourceUrl, HttpMethod.Get);
            return (await response.Content.ReadAsStringAsync()).ToLower();
        }

        public async Task MarkUserAsVisited(Guid userId)
        {
            var resourceUrl = $"User/{userId}/MarkAsVisited";
            var request = httpHelper.CreateRequestWithCredential(resourceUrl, HttpMethod.Post);
            await httpHelper.SendRequest(request);
        }

        public async Task<string> GetLastModifiedYearMonth(Guid userId)
        {
            var resourceUrl = $"User/{userId}/LastModifiedYearMonth";
            var response =await httpHelper.SendRequest(resourceUrl, HttpMethod.Get);
            return (await response.Content.ReadAsStringAsync()).ToLower();
        }

        public async Task<HttpResponseMessage> SetLastModifiedYearMonth(Guid userId, string year, string month)
        {
            var resourceUrl = $"User/{userId}/LastModifiedYearMonth";
            var parameters = $"year={year}&month={month}";
            var request = httpHelper.CreateRequestWithCredentialAndBody(resourceUrl, HttpMethod.Post, parameters);
            return await httpHelper.SendRequest(request);
        }

        private async Task<string> RetrieveData(string resourceUrl, string parameters)
        {
            var request = httpHelper.CreateRequestWithCredentialAndBody(resourceUrl, HttpMethod.Post, parameters);
            var response = await httpHelper.SendRequest(request);
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<T> UserCalendarFromUrl<T>(string calendarUrl)
        {
            var response = await httpHelper.SendRequest(calendarUrl, HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            var stringReader = new StringReader(content);
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stringReader);
        }

        private async Task<CalendarRoot> UserCalendarRootFromUrl(string calendarUrl)
        {
            var response = await httpHelper.SendRequest(calendarUrl, HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            var stringReader = new StringReader(content);
            var serializer = new XmlSerializer(typeof(CalendarRoot));
            return (CalendarRoot)serializer.Deserialize(stringReader);
        }

        public async Task<string> DaysRead(string userId, string fromDate, string toDate, string changedStartDate = null, string changedEndDate = null)
        {
            var resourceUrl = $"Days?userId={userId}&fromDate={fromDate}&toDate={toDate}";
            var response = await httpHelper.SendRequest(resourceUrl, HttpMethod.Get);
            return (await response.Content.ReadAsStringAsync()).ToLower();
        }

        public async Task<string> DaysSave(string userId, string @params)
        {
            var resourceUrl = $"User/{userId}/Days";
            var parameters = "params=" + @params;
            var request = httpHelper.CreateRequestWithCredentialAndBody(resourceUrl, HttpMethod.Post, parameters);
            var response = await httpHelper.SendRequest(request);
            return (await response.Content.ReadAsStringAsync()).ToLower();
        }

        public async Task<SyndicationFeed> CalendarChangedFeed()
        {
            const string feedUrl = "CalendarChanged.atom";
            return await FeedFromUrl(feedUrl);
        }

        public async Task<SyndicationFeed> CalendarChangedFeed(int year, int month, int day, int hour, int minute)
        {
            var feedUrl = $"{year}/{month}/{day}/{hour}/{minute}/CalendarChanged.atom";
            return await FeedFromUrl(feedUrl);
        }

        private async Task<SyndicationFeed> FeedFromUrl(string feedUrl)
        {
            var response = await httpHelper.SendRequest(feedUrl, HttpMethod.Get);
            var feedContent = await response.Content.ReadAsStringAsync();
            var xmlReader = XmlReader.Create(new StringReader(feedContent));
            var feed = SyndicationFeed.Load(xmlReader);
            return feed;
        }
    }
}
