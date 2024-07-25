namespace Calendar.Service.Configuration
{
    public interface IAppConfiguration
    {
        string DBConnectionString { get; set; }
        string ApplicationInsightsConnectionString { get; set; }
        string EnableTrackSqlFullQueryTextInApplicationInsights { get; set; }
        string CalendarificApiKey { get; set; }
        string CalendarificApimSubscriptionKey { get; set; }
        string HolidaysFilterType { get; set; }
        string ApiManagementHostUrl { get; set; }
    }
}