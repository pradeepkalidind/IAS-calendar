using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PWC.IAS.Calendar.Client
{
    public class IasCalendarSettingsResource
    {
        private readonly HttpHelper httpHelper;

        public IasCalendarSettingsResource(HttpClient client, string userName, string password)
        {
            httpHelper = new HttpHelper(client, userName, password);
        }

        public async Task<string> GetCalendarSettings(Guid userId)
        {
            const string resourceUrl = "CalendarSettings/GetCalendarSettings";
            var tokenParam = "token=" + userId;
            var request = httpHelper.CreateRequestWithCredentialAndBody(resourceUrl, HttpMethod.Post, tokenParam);
            var response = await httpHelper.SendRequest(request);
            return (await response.Content.ReadAsStringAsync()).ToLower();
        }

        public async Task<HttpResponseMessage> SaveWorkWeekDefaults(Guid userId, string @params)
        {
            const string resourceUrl = "CalendarSettings/SaveWorkWeekDefaults";
            var tokenParam = "token=" + userId;
            var daysBodyParam = "workWeekDefaults=" + @params;
            var request = httpHelper.CreateRequestWithCredentialAndBody(resourceUrl, HttpMethod.Post, tokenParam + "&" + daysBodyParam);
            return await httpHelper.SendRequest(request);
        }

        public async Task<HttpResponseMessage> SaveDefaultWorkingDays(Guid iasPlatformUserId, string @params)
        {
            var resourceUrl = $"User/{iasPlatformUserId}/DefaultWorkDays";
            var parameters = "params=" + @params;
            var request = httpHelper.CreateRequestWithCredentialAndBody(resourceUrl, HttpMethod.Post, parameters);
            return await httpHelper.SendRequest(request);
        }
    }
}