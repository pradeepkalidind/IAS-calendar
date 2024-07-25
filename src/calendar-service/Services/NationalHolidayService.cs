using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Service.Clients;
using Calendar.Service.Models;

namespace Calendar.Service.Services
{
    public interface INationalHolidayService
    {
        IList<string> GetAvailableCountries();
        IList<string> GetNameOfNationalHolidaysByCountry(string country, string culture);
        IList<CalendarificHoliday> GetNationalHolidaysDetailByCountry(string country, string culture);
    }

    public class NationalHolidayService : INationalHolidayService
    {
        public ICalendarificClient calendarificClientClient;

        public NationalHolidayService()
        {
            calendarificClientClient = new CalendarificClient();
        }


        public IList<string> GetAvailableCountries()
        {
            return calendarificClientClient.GetAvailableCountries();
        }


        public IList<CalendarificHoliday> GetNationalHolidaysDetailByCountry(string country, string culture)
        {
            if (string.IsNullOrEmpty(country))
            {
                return new List<CalendarificHoliday>();
            }

            var holidays = new List<CalendarificHoliday>();
            var year = DateTime.Now.Year;

            for (var currentYear = year - 3; currentYear < year + 2; currentYear++)
            {
                var holidaysOfYear = calendarificClientClient.GetNationalHolidaysByCountry(country, culture, currentYear);
                holidays.AddRange(holidaysOfYear);
            }

            return holidays;
        }

        public IList<string> GetNameOfNationalHolidaysByCountry(string country, string culture)
        {
            var year = DateTime.Now.Year;
            var countries = calendarificClientClient.GetNationalHolidaysByCountry(country, culture, year);
            return countries.Select(x => x.name).Distinct().ToList();
        }
    }
}
