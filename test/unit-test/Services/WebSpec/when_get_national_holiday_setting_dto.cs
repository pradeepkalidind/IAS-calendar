using System;
using Calendar.Model.Compact;
using Calendar.Service.Services;
using Calendar.Tests.Unit.CompactModels.LocationRulesSpec;
using Xunit;

namespace Calendar.Tests.Unit.Services.WebSpec
{
    public class when_get_national_holiday_setting_dto : CompactModelSpec
    {
        private static Guid kaylaId;

        public when_get_national_holiday_setting_dto()
        {
            kaylaId = Guid.NewGuid();
            var nationalHolidaySettingsForKayla = new NationalHolidaySetting(kaylaId, "CN");
            Session.Save(nationalHolidaySettingsForKayla);
        }

        [Fact]
        public void should_get_national_holiday_for_kayla()
        {
            const string mockHolidaysResponse = @"{
                'meta': {
                    'code': 200,
                },
                'response': {
                    'holidays': [
                        {
                            'name': 'de-DE New Year\'s Day',
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
                            'name': 'de-DE Martin Luther King Jr. Day',
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

            var nationalHolidayService = new NationalHolidayService()
            {
                calendarificClientClient = calendarificClient
            };

            var service = new CalendarSettingsService(Session)
            {
                holidayService = nationalHolidayService
            };
            var dto = service.GetNationalHolidaySettingDto(kaylaId, "de-DE");

            Assert.Equal("CN", dto.Country);
            Assert.Equal(10, dto.Holidays.Length);
            Assert.Equal("2019-01-01", dto.Holidays[0].Date);
            Assert.Equal("de-DE New Year's Day", dto.Holidays[0].I18n);
            Assert.Equal("2019-01-21", dto.Holidays[1].Date);
            Assert.Equal("de-DE Martin Luther King Jr. Day", dto.Holidays[1].I18n);
        }


        [Fact]
        public void should_get_right_national_holiday_for_kayla_when_iso_have_specific_date_format()
        {

            const string mockHolidaysResponse = @"{
                'meta': {
                    'code': 200,
                },
                'response': {
                    'holidays': [
                        {
                            'name': 'de-DE New Year\'s Day',
                            'date': {
                                'iso': '2019-01-01T05:58:32+08:00',
                                'datetime': {
                                    'year': 2019,
                                    'month': 1,
                                    'day': 1
                                } 
                            }
                        },
                        {
                            'name': 'de-DE Martin Luther King Jr. Day',
                            'date': {
                                'iso': '2019-01-21',
                                'datetime': {
                                    'year': 2019,
                                    'month': 1,
                                    'day': 21
                                } 
                            },
                        },
                        {
                            'name': 'de-DE Martin Luther King Jr. Day Worry',
                            'date': {
                                'iso': '2019-01-22'
                            },
                        }
                    ]
                }
            }";

            var calendarificClient = CalendarificTestHelper.MockCalendarificClient(mockHolidaysResponse);

            var nationalHolidayService = new NationalHolidayService()
            {
                calendarificClientClient = calendarificClient
            };

            var service = new CalendarSettingsService(Session)
            {
                holidayService = nationalHolidayService
            };
            var dto = service.GetNationalHolidaySettingDto(kaylaId, "de-DE");

            Assert.Equal("CN", dto.Country);
            Assert.Equal(10, dto.Holidays.Length);
            Assert.Equal("2019-01-01", dto.Holidays[0].Date);
            Assert.Equal("de-DE New Year's Day", dto.Holidays[0].I18n);
            Assert.Equal("2019-01-21", dto.Holidays[1].Date);
            Assert.Equal("de-DE Martin Luther King Jr. Day", dto.Holidays[1].I18n);
        }

        [Fact(Skip = "need calendarific language")]
        public void should_get_en_US_translation_if_translation_for_given_culture_is_empty()
        {
            const string mockHolidaysResponse = @"{
                'meta': {
                    'code': 200,
                },
                'response': {
                    'holidays': [
                        {
                            'name': 'de-DE New Year\'s Day',
                            'date': {
                                'iso': '2019-01-01T05:58:32+08:00',
                                'datetime': {
                                    'year': 2019,
                                    'month': 1,
                                    'day': 1
                                } 
                            }
                        },
                        {
                            'name': 'de-DE Martin Luther King Jr. Day',
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

            var nationalHolidayService = new NationalHolidayService()
            {
                calendarificClientClient = calendarificClient
            };

            var service = new CalendarSettingsService(Session)
            {
                holidayService = nationalHolidayService
            };
            var dto = service.GetNationalHolidaySettingDto(kaylaId, "fr-FR");

            Assert.Equal("CN", dto.Country);
            Assert.Single(dto.Holidays);
            Assert.Equal("2012-01-27", dto.Holidays[0].Date);
            Assert.Equal("Spring Festival", dto.Holidays[0].I18n);
        }

        [Fact(Skip = "need calendarific language")]
        public void should_get_en_US_translation_if_given_culture_is_not_exist()
        {
            const string mockHolidaysResponse = @"{
                'meta': {
                    'code': 200,
                },
                'response': {
                    'holidays': [
                        {
                            'name': 'de-DE New Year\'s Day',
                            'date': {
                                'iso': '2019-01-01T05:58:32+08:00',
                                'datetime': {
                                    'year': 2019,
                                    'month': 1,
                                    'day': 1
                                } 
                            }
                        },
                        {
                            'name': 'de-DE Martin Luther King Jr. Day',
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

            var nationalHolidayService = new NationalHolidayService
            {
                calendarificClientClient = calendarificClient
            };
            var service = new CalendarSettingsService(Session)
            {
                holidayService = nationalHolidayService
            };
            var dto = service.GetNationalHolidaySettingDto(kaylaId, "en-CA");

            Assert.Equal("CN", dto.Country);
            Assert.Equal(5, dto.Holidays.Length);
            Assert.Equal("2012-01-27", dto.Holidays[0].Date);
            Assert.Equal("Spring Festival", dto.Holidays[0].I18n);
        }
    }
}
