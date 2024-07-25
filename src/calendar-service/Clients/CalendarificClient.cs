using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using Calendar.Service.Configuration;
using Calendar.Service.Models;
using Calendar.Service.Utils;
using Newtonsoft.Json;
using NLog;

namespace Calendar.Service.Clients;

public interface ICalendarificClient
{
    List<string> GetAvailableCountries();
    IEnumerable<CalendarificHoliday> GetNationalHolidaysByCountry(string country, string culture, long year);
}

public class CalendarificClient : ICalendarificClient
{
    private readonly IHttpClientHandler httpClient;
    private readonly string apiKey;
    private readonly string holidaysFilterType;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public CalendarificClient(IHttpClientHandler clientHandler = null)
    {
        apiKey = AppSettings.Config.CalendarificApiKey;
        holidaysFilterType = AppSettings.Config.HolidaysFilterType;
        httpClient = clientHandler ?? new HttpClientHandler(AppSettings.Config.ApiManagementHostUrl);
    }

    public List<string> GetAvailableCountries()
    {
        return Get($"countries?api_key={apiKey}",
            r => r.countries,
            new { countries = default(List<CalendarificCountry>) })
            .Select(c => c.IsoName)
            .ToList();
    }

    public IEnumerable<CalendarificHoliday> GetNationalHolidaysByCountry(string country, string culture, long year)
    {
        return Get($"holidays?api_key={apiKey}&country={country}&year={year}&type={holidaysFilterType}",
            r => r.holidays,
            new { holidays = default(List<CalendarificHoliday>) }, $"holiday-{country}-{year}");
    }

    private List<T> Get<T, TR>(string url, Func<TR, List<T>> map, TR responseObjSchema, string cacheKey = default)
    {
        HttpRequestMessage requestMessage = new(HttpMethod.Get, url);
        requestMessage.Headers.Add("Ocp-Apim-Subscription-Key", AppSettings.Config.CalendarificApimSubscriptionKey);

        var response = httpClient.SendAsync(requestMessage).Result;
        if (response.Content.Headers.ContentType?.MediaType == MediaTypeNames.Application.Json)
        {
            try
            {
                var result = Deserialize(response, new
                {
                    meta = new {
                        code = 0,
                        error_type = default(string),
                        error_detail = default(string)
                    },
                    response = responseObjSchema
                });
                if (response.IsSuccessStatusCode && result.meta.code == (int)HttpStatusCode.OK)
                {
                    var model = map(result.response);
                    return model;
                }

                if (result.meta.code != (int)HttpStatusCode.OK)
                {
                    Logger.Error($"error-code: {result.meta.code}, type: {result.meta.error_type}, detail: {result.meta.error_detail}");
                }
            }
            catch (Exception)
            {
                Logger.Error($"GetNationalHolidaysByCountry: {response.Content.ReadAsStringAsync().Result}");
                return new List<T>();
            }
        }
        else
        {
            Logger.Error($"statusCode: {response.StatusCode}, type: unexpected media type, " +
                         $"mediaType: {response.Content.Headers.ContentType?.MediaType}, body: {response.Content.ReadAsStringAsync().Result}");
        }
        return new List<T>();
    }

    private T Deserialize<T>(HttpResponseMessage response, T _ = default)
    {
        return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
    }
}