using System.Configuration;
using PwC.Configuration;

namespace Calendar.Service.Configuration
{
    public class AppConfiguration : IAppConfiguration
    {
        public string DBConnectionString { get; set; }

        public string ApplicationInsightsConnectionString { get; set; }

        public string EnableTrackSqlFullQueryTextInApplicationInsights { get; set; }

        public string CalendarificApiKey { get; set; }

        public string CalendarificApimSubscriptionKey { get; set; }

        public string HolidaysFilterType { get; set; }

        public string ApiManagementHostUrl { get; set; }

        public IAppConfiguration Get()
        {
            return new AppConfiguration
            {
                DBConnectionString = ConfigurationHelper.Configuration["DBConnectionString"],
                ApplicationInsightsConnectionString = ConfigurationHelper.Configuration["ApplicationInsightsConnectionString"],
                EnableTrackSqlFullQueryTextInApplicationInsights = ConfigurationHelper.Configuration["EnableTrackSqlFullQueryTextInApplicationInsights"],
                CalendarificApiKey = ConfigurationHelper.Configuration["CalendarificApiKey"],
                CalendarificApimSubscriptionKey = ConfigurationHelper.Configuration["CalendarificApimSubscriptionKey"],
                HolidaysFilterType = ConfigurationHelper.Configuration["HolidaysFilterType"],
                ApiManagementHostUrl = ConfigurationHelper.Configuration["ApiManagementHostUrl"],
            }; 
        }
    }
}