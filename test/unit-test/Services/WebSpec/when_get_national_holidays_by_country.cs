using System.Linq;
using Calendar.Service.Services;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    public class when_get_national_holidays_by_country : CompactModelSpec
    {
        [Fact]
        public void should_get_national_holidays_by_country_and_sorted_by_date()
        {
            const string mockHolidaysResponse = @"{
                'meta': {
                    'code': 200,
                },
                'response': {
                    'holidays': [
                        {
                            'name': 'New Year\'s Day',
                            'date': {
                                'iso': '2019-01-01',
                                'datetime': {
                                    'year': 2019,
                                    'month': 1,
                                    'day': 1
                                } 
                            }
                        },
                        {
                            'name': 'Martin Luther King Jr. Day',
                            'date': {
                                'iso': '2019-01-21',
                                'datetime': {
                                    'year': 2019,
                                    'month': 1,
                                    'day': 21
                                } 
                            },
                        }
                    ]
                }
            }";


            var calendarificClient = CalendarificTestHelper.MockCalendarificClient(mockHolidaysResponse);

            var service = new NationalHolidayService
            {
                calendarificClientClient = calendarificClient
            };
            var availableCountries = service.GetNameOfNationalHolidaysByCountry("US", "en-US");

            Assert.Equal(2, availableCountries.Count);
            Assert.Equal("New Year's Day", availableCountries.First());
            Assert.Equal("Martin Luther King Jr. Day", availableCountries.Last());
        }

        [Fact]
        public void should_return_empty_holiday_list_when_no_country_matches()
        {
            const string mockHolidaysResponse =
                "{\"meta\":{\"code\":200},\"response\":{\"holidays\":[]}}";

            var calendarificClient = CalendarificTestHelper.MockCalendarificClient(mockHolidaysResponse);

            var service = new NationalHolidayService()
            {
                calendarificClientClient = calendarificClient
            };
            var availableCountries = service.GetNameOfNationalHolidaysByCountry("ES", "en-US");

            Assert.Equal(0, availableCountries.Count);
        }

        [Fact]
        public void should_return_empty_holiday_list_when_response_body_is_illegal_json_string()
        {
            const string mockHolidaysResponse =
                "{\"meta\":{\"code\":200},\"response\":{\"holidays\":[}}";

            var calendarificClient = CalendarificTestHelper.MockCalendarificClient(mockHolidaysResponse);

            var service = new NationalHolidayService()
            {
                calendarificClientClient = calendarificClient
            };
            var availableCountries = service.GetNameOfNationalHolidaysByCountry("ES", "en-US");

            Assert.Equal(0, availableCountries.Count);
        }

        [Fact]
        public void should_return_default_en_US_translation_when_no_culture_matches()
        {
            const string mockHolidaysResponse = @"{
                'meta': {
                    'code': 200,
                },
                'response': {
                    'holidays': [
                        {
                            'name': 'New Year\'s Day',
                            'date': {
                                'iso': '2019-01-01',
                                'datetime': {
                                    'year': 2019,
                                    'month': 1,
                                    'day': 1
                                } 
                            }
                        },
                        {
                            'name': 'Martin Luther King Jr. Day',
                            'date': {
                                'iso': '2019-01-21',
                                'datetime': {
                                    'year': 2019,
                                    'month': 1,
                                    'day': 21
                                } 
                            },
                        }
                    ]
                }
            }";


            var calendarificClient = CalendarificTestHelper.MockCalendarificClient(mockHolidaysResponse);

            var service = new NationalHolidayService
            {
                calendarificClientClient = calendarificClient
            };

            var availableCountries = service.GetNameOfNationalHolidaysByCountry("DE", "ja-JP");

            Assert.Equal(2, availableCountries.Count);
            Assert.Equal("New Year's Day", availableCountries[0]);
            Assert.Equal("Martin Luther King Jr. Day", availableCountries[1]);
        }

        [Fact(Skip = "language")]
        public void should_return_non_duplicated_holidays()
        {
            var service = new NationalHolidayService();
            var availableCountries = service.GetNameOfNationalHolidaysByCountry("DE", "en-US");

            Assert.Single(availableCountries);
            Assert.Equal("Spring Festival", availableCountries.ElementAt(0));
        }
    }
}
