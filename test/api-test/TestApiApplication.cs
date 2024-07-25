using Calendar.Service.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Calendar.API.Test;

internal class TestApiApplication : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        AppSettings.InitConfig(new AppConfiguration
        {
            DBConnectionString = "Data Source=(local);Initial Catalog=calendar;Integrated Security=True;Connection Timeout=60;",
            CalendarificApiKey = "CalendarificApiKey",
            CalendarificApimSubscriptionKey = "CalendarificApimSubscriptionKey",
            ApiManagementHostUrl = "https://ApiManagementHostUrl"
        });
        return base.CreateHost(builder);
    }
}