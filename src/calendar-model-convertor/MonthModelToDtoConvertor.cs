using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Calendar.General.Dto;
using Calendar.Model.Compact;

namespace Calendar.Model.Convertor
{
    public class MonthModelToDtoConvertor
    {
        private readonly IEnumerable<MonthActivity> monthActivities;
        private readonly Note[] notes;

        public MonthModelToDtoConvertor(IEnumerable<MonthActivity> monthActivities, Note[] notes)
        {
            this.monthActivities = monthActivities;
            this.notes = notes;
        }

        public IEnumerable<CalendarMonthDto> GetYearDto(int year )
        {
            return Enumerable.Range(1, 12)
                .Select(month => FindOrCreateMonthActivity(month, year))
                .Select(GetMonthDto);
        }

        public Dictionary<string, double> GetTravelDaysInCountries(int year)
        {
            var allYearDays = monthActivities.Where(m => m.Year == year)
                .SelectMany(m => m.GetDays()).Where(d => d.HasLocationAndActivity())
                .SelectMany(ToDayLocation);
            var countryDict = new Dictionary<string, double>();
            foreach (var d in allYearDays)
            {
                if(countryDict.ContainsKey(d.Key))
                {
                    countryDict[d.Key] += d.Value;
                }
                else
                {
                    countryDict[d.Key] = d.Value;
                }
            }

            return countryDict;
        } 

        private MonthActivity FindOrCreateMonthActivity(int month, int year)
        {
            return monthActivities.FirstOrDefault(m => m.Month == month && m.Year == year) ?? new MonthActivity(Guid.Empty, year, month);
        }

        private CalendarMonthDto GetMonthDto(MonthActivity monthActivity)
        {
            var dayActivities = monthActivity.GetDays().Where(d => !d.IsEmpty(() => GetNote(monthActivity,d.Day)));
            var dayDicts = dayActivities.Select(d => ToDayDict(d, monthActivity)).ToArray();
            var noTravel = monthActivity.NoTravelConfirmation ? 1 : 0;
            return new CalendarMonthDto { m = monthActivity.Month, y = monthActivity.Year, nt = noTravel, d = dayDicts };
        }

        private Dictionary<string, string> ToDayDict(DayActivity dayActivity,MonthActivity monthActivity)
        {
            var note = GetNote(monthActivity, dayActivity.Day);
            return ToDayDict(dayActivity, note);
        }

        private Note GetNote(MonthActivity monthActivity, int day)
        {
            var date = new DateTime(monthActivity.Year, monthActivity.Month, day);
            return GetNote(notes, date);
        }

        private static Dictionary<string, string> ToDayDict(DayActivity dayActivity, Note note)
        {
            var allocationRule = new ActivityAllocationRule(dayActivity);
            var dayDic = new Dictionary<string, string>();
            dayDic["D"] = dayActivity.Day.ToString();
            if (!dayActivity.IsFirstActivityEmpty()) dayDic["A"] = ActivityTypeConvertor.GetActivityAbbr(dayActivity.First.Type);
            if (!dayActivity.IsSecondActivityEmpty()) dayDic["SA"] = ActivityTypeConvertor.GetActivityAbbr(dayActivity.Second.Type);
            if (!allocationRule.IsFirstActivityFirstLocationAllocationEmpty()) dayDic["AA"] = allocationRule.GetFirstActivityFirstLocationAllocation().ToString();
            if (!allocationRule.IsSecondActivityFirstLocationAllocationEmpty()) dayDic["SAA"] = allocationRule.GetSecondActivityFirstLocationAllocation().ToString();
            if (!string.IsNullOrEmpty(dayActivity.LocationPattern.FirstLocation)) dayDic["FL"] = GetLocationAbbr(dayActivity.LocationPattern.FirstLocation);
            if (dayActivity.LocationPattern.FirstLocationDepartureTime.HasValue) dayDic["FLD"] = dayActivity.LocationPattern.FirstLocationDepartureTime.ToString();
            if (dayActivity.LocationPattern.FirstLocationArrivalTime.HasValue) dayDic["FLA"] = dayActivity.LocationPattern.FirstLocationArrivalTime.ToString();
            if (!string.IsNullOrEmpty(dayActivity.LocationPattern.SecondLocation)) dayDic["SL"] = GetLocationAbbr(dayActivity.LocationPattern.SecondLocation);
            if (dayActivity.LocationPattern.SecondLocationDepartureTime.HasValue) dayDic["SLD"] = dayActivity.LocationPattern.SecondLocationDepartureTime.ToString();
            if (dayActivity.LocationPattern.SecondLocationArrivalTime.HasValue) dayDic["SLA"] = dayActivity.LocationPattern.SecondLocationArrivalTime.ToString();
            if (!string.IsNullOrEmpty(dayActivity.LocationPattern.ThirdLocation)) dayDic["TL"] = GetLocationAbbr(dayActivity.LocationPattern.ThirdLocation);
            if (dayActivity.LocationPattern.ThirdLocationArrivalTime.HasValue) dayDic["TLA"] = dayActivity.LocationPattern.ThirdLocationArrivalTime.ToString();
            if (note != null) dayDic["N"] = note.Content;
            return dayDic;
        }
        
        private static Dictionary<string, double> ToDayLocation(DayActivity dayActivity)
        {
            var dayDict = new Dictionary<string, double>();
            var secondLocation = dayActivity.LocationPattern.SecondLocation;
            var thirdLocation = dayActivity.LocationPattern.ThirdLocation;
            if (!string.IsNullOrEmpty(thirdLocation))
            {
                dayDict[GetCountryAbbr(thirdLocation)] = 0.5;
                if (dayDict.ContainsKey(GetCountryAbbr(secondLocation)))
                {
                    dayDict[GetCountryAbbr(secondLocation)] += 0.5;
                    return dayDict;
                }
                dayDict[GetCountryAbbr(secondLocation)] = 0.5;
                return dayDict;
            }

            var firstLocation = dayActivity.LocationPattern.FirstLocation;
            if (!string.IsNullOrEmpty(secondLocation))
            {
                dayDict[GetCountryAbbr(firstLocation)] = 0.5;
                if (dayDict.ContainsKey(GetCountryAbbr(secondLocation)))
                {
                    dayDict[GetCountryAbbr(secondLocation)] += 0.5;
                    return dayDict;
                }
                dayDict[GetCountryAbbr(secondLocation)] = 0.5;
                return dayDict;
            }

            dayDict[GetCountryAbbr(firstLocation)] = 1;
            return dayDict;
        }

        private static Note GetNote(IEnumerable<Note> notes, DateTime date)
        {
            return notes.FirstOrDefault(n => n.Date.Equals(date));
        }

        private static string GetCountryAbbr(string location)
        {
            var locationArray = location.Trim().Split('/').ToArray();
            return locationArray[2];
        }

        private static string GetLocationAbbr(string location)
        {
            return location.Replace("TaxUnit/", "").Replace("/Country/", "");
        }
    }
}