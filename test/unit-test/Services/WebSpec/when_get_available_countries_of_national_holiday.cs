using Calendar.Service.Configuration;
using Calendar.Service.Services;
using Moq;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    public class when_get_available_countries_of_national_holiday 
    {
        [Fact]
        public void should_update_national_holiday()
        {
            MockAppSettings();
            const string mockCountriesResponse = @"{
                'meta': {
                    'code': 200,
                },
                'response': {
                    'url': 'https://calendarific.com/supported-countries',
                    'countries': [
                        {
                            'country_name': 'Afghanistan',
                            'iso-3166': 'AF',
                            'total_holidays': 24,
                            'supported_languages': 2,
                            'uuid': 'f0357a3f154bc2ffe2bff55055457068'
                        },
                        {
                            'country_name': 'Albania',
                            'iso-3166': 'AL',
                            'total_holidays': 33,
                            'supported_languages': 4,
                            'uuid': '97282b278e5d51866f8e57204e4820e5'
                        }
                    ]
                }
            }";

            var service = new NationalHolidayService
            {
                calendarificClientClient = CalendarificTestHelper.MockCalendarificClient(mockCountriesResponse)
            };

            var availableCountries = service.GetAvailableCountries();
            Assert.Equal(2, availableCountries.Count);
            Assert.Contains(availableCountries, c => c.Equals("AF"));
            Assert.Contains(availableCountries, c => c.Equals("AL"));
        }

        private static void MockAppSettings()
        {
            var appConfig = new Mock<IAppConfiguration>();
            appConfig.Setup(a => a.ApiManagementHostUrl).Returns("https://apimhost");
            appConfig.Setup(a => a.CalendarificApiKey).Returns("key");
            appConfig.Setup(a => a.CalendarificApimSubscriptionKey).Returns("key2");
            AppSettings.InitConfig(appConfig.Object);
        }
    }
}
