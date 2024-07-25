using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Client.Schema;
using Calendar.General.Dto;
using Calendar.Model.Compact;
using Activity = Calendar.Client.Schema.Activity;
using Note = Calendar.Client.Schema.Note;

namespace Calendar.Model.Convertor
{
    public class MonthModelToDayDtoConvertor
    {
        public const string DateFormat = "yyyy-MM-dd";
        public const string TimeFormat = "{0:00}00";
        public static Day ToDay(DayActivity dayActivity, MonthActivity monthActivity,
                                IEnumerable<Model.Compact.Note> notes)
        {
            var note = GetNote(monthActivity, notes, dayActivity.Day);
            return ToDayDto(dayActivity, note, monthActivity.Year, monthActivity.Month);
        }

        public static Model.Compact.Note GetNote(MonthActivity monthActivity, IEnumerable<Model.Compact.Note> notes, int day)
        {
            var date = new DateTime(monthActivity.Year, monthActivity.Month, day);
            return notes.FirstOrDefault(n => n.Date.Equals(date));
        }

        public static DayDto ToJsonDto(DayActivity dayActivity, MonthActivity monthActivity,
                                       IEnumerable<Model.Compact.Note> notes)
        {
            var note = GetNote(monthActivity, notes, dayActivity.Day);
            return ToJsonDto(dayActivity, note, monthActivity.Year, monthActivity.Month);
        }

        public static DeletedDayDto ToDeleteJsonDto(DayActivity dayActivity, MonthActivity monthActivity)
        {
            return ToJsonDto(dayActivity, monthActivity.Year, monthActivity.Month);
        }

        private static Day ToDayDto(DayActivity dayActivity, Model.Compact.Note note, int year, int month)
        {
            return new Day
                       {
                           Note = ExtractNote(note),
                           Location = ExtractFirstLocation(dayActivity),
                           ArrivalTime = ExtractLocationArrivalTime(dayActivity),
                           DepartureTime = ExtractLocationDepartureTime(dayActivity),
                           IsCovid = ExtractIsCovidImpacted(dayActivity),
                           Date = ExtractDate(dayActivity, year, month),
                           Activities = ExtractActivities(dayActivity),
                           Travels = ExtractTravels(dayActivity, year, month)
                       };
        }

        private static DayDto ToJsonDto(DayActivity dayActivity, Model.Compact.Note note, int year, int month)
        {
            return new DayDto
                       {
                           date = ExtractDate(dayActivity, year, month),
                           type = dayActivity.DayType.ToString(),
                           firstActivity = ActivityTypeConvertor.GetActivityLongName(dayActivity.First.Type),
                           firstActivityAllocation = ExtractFirstActivityAllocation(dayActivity),
                           secondActivity = ActivityTypeConvertor.GetActivityLongName(dayActivity.Second.Type),
                           secondActivityAllocation = ExtractSecondActivityAllocation(dayActivity),
                           firstLocation = ExtractFirstLocation(dayActivity),
                           firstLocationDepartureTime = dayActivity.LocationPattern.FirstLocationDepartureTime,
                           firstLocationArrivalTime = dayActivity.LocationPattern.FirstLocationArrivalTime,
                           secondLocation = dayActivity.LocationPattern.SecondLocation,
                           secondLocationDepartureTime = dayActivity.LocationPattern.SecondLocationDepartureTime,
                           secondLocationArrivalTime = dayActivity.LocationPattern.SecondLocationArrivalTime,
                           thirdLocation = dayActivity.LocationPattern.ThirdLocation,
                           thirdLocationArrivalTime = dayActivity.LocationPattern.ThirdLocationArrivalTime,
                           enterFrom = dayActivity.EnterFrom.Equals(EnterFromType.Empty)?string.Empty:dayActivity.EnterFrom.ToString(),
                           note = note == null ? string.Empty : note.Content,
                           isCovid = ActivityTypeConvertor.GetIsCovidByActivityType(dayActivity.First.Type)
                       };
        }

        public static DeletedDayDto ToJsonDto(DayActivity dayActivity, int year, int month)
        {
            return new DeletedDayDto { date = new DateTime(year, month, dayActivity.Day).ToString(DateFormat) };
        }

        private static List<Travel> ExtractTravels(DayActivity dayActivity, int year, int month)
        {
            var firstTravel = ExtractFirstTravel(dayActivity, year, month);
            var secondTravel = ExtractSecondTravel(dayActivity, year, month);
            return firstTravel == null
                ? null
                : secondTravel == null
                    ? new List<Travel> { firstTravel }
                    : new List<Travel> { firstTravel, secondTravel };
        }

        private static Travel ExtractSecondTravel(DayActivity dayActivity, int year, int month)
        {
            return string.IsNullOrEmpty(dayActivity.LocationPattern.SecondLocation) ||
                string.IsNullOrEmpty(dayActivity.LocationPattern.ThirdLocation)
                ? null
                : new Travel
                      {
                          Departure = new Departure
                                          {
                                              Date = ExtractDate(dayActivity, year, month),
                                              Location = dayActivity.LocationPattern.SecondLocation,
                                              Time =
                                                  string.Format(TimeFormat,
                                                      dayActivity.LocationPattern.SecondLocationDepartureTime)
                                          },
                          Arrival = new Arrival
                                        {
                                            Date = ExtractDate(dayActivity, year, month),
                                            Location = dayActivity.LocationPattern.ThirdLocation,
                                            Time =
                                                string.Format(TimeFormat,
                                                    dayActivity.LocationPattern.ThirdLocationArrivalTime)
                                        }
                      };
        }

        private static Travel ExtractFirstTravel(DayActivity dayActivity, int year, int month)
        {
            return string.IsNullOrEmpty(dayActivity.LocationPattern.FirstLocation) ||
                string.IsNullOrEmpty(dayActivity.LocationPattern.SecondLocation)
                ? null
                : new Travel
                      {
                          Departure = new Departure
                                          {
                                              Date = ExtractDate(dayActivity, year, month),
                                              Location = dayActivity.LocationPattern.FirstLocation,
                                              Time =
                                                  string.Format(TimeFormat,
                                                      dayActivity.LocationPattern.FirstLocationDepartureTime)
                                          },
                          Arrival = new Arrival
                                        {
                                            Date = ExtractDate(dayActivity, year, month),
                                            Location = dayActivity.LocationPattern.SecondLocation,
                                            Time =
                                                string.Format(TimeFormat,
                                                    dayActivity.LocationPattern.SecondLocationArrivalTime)
                                        }
                      };
        }

        private static List<Activity> ExtractActivities(DayActivity dayActivity)
        {
            var firstActivity = ExtractFirstActivity(dayActivity);
            var secondActivity = ExtractSecondActivity(dayActivity);
            if (firstActivity == null && secondActivity == null)
                return null;
            var activities = new List<Activity>
                                 {
                                     firstActivity ?? new Activity
                                                          {
                                                              FirstAllocation = null,
                                                              SecondAllocation = null,
                                                              Type =
                                                                  ActivityTypeConvertor.GetActivityLongName(
                                                                      ActivityType.Empty)
                                                          }
                                 };
            if (secondActivity != null)
            {
                activities.Add(secondActivity);
            }
            return activities;
        }

        private static Activity ExtractSecondActivity(DayActivity dayActivity)
        {
            var allocationRule = new ActivityAllocationRule(dayActivity);
            return allocationRule.IsSecondActivityFirstLocationAllocationEmpty()
                ? null
                : new Activity
                      {
                          FirstAllocation = allocationRule.GetSecondActivityFirstLocationAllocation(),
                          SecondAllocation = allocationRule.GetSecondActivitySecondLocationAllocation(),
                          Type = ActivityTypeConvertor.GetActivityLongName(dayActivity.Second.Type)
                      };
        }

        private static double? ExtractSecondActivityAllocation(DayActivity dayActivity)
        {
            var allocationRule = new ActivityAllocationRule(dayActivity);
            return allocationRule.IsSecondActivityFirstLocationAllocationEmpty()
                ? (double?)null
                : allocationRule.GetSecondActivityFirstLocationAllocation();
        }

        private static Activity ExtractFirstActivity(DayActivity dayActivity)
        {
            var allocationRule = new ActivityAllocationRule(dayActivity);
            return allocationRule.IsFirstActivityFirstLocationAllocationEmpty()
                ? null
                : new Activity
                      {
                          FirstAllocation = allocationRule.GetFirstActivityFirstLocationAllocation(),
                          SecondAllocation = allocationRule.GetFirstActivitySecondLocationAllocation(),
                          Type = ActivityTypeConvertor.GetActivityLongName(dayActivity.First.Type)
                      };
        }

        private static double? ExtractFirstActivityAllocation(DayActivity dayActivity)
        {
            var allocationRule = new ActivityAllocationRule(dayActivity);
            return allocationRule.IsFirstActivityFirstLocationAllocationEmpty()
                ? (double?)null
                : allocationRule.GetFirstActivityFirstLocationAllocation();
        }

        private static string ExtractDate(DayActivity dayActivity, int year, int month)
        {
            return new DateTime(year, month, dayActivity.Day).ToString(DateFormat);
        }

        private static string ExtractLocationDepartureTime(DayActivity dayActivity)
        {
            return dayActivity.LocationPattern.FirstLocationDepartureTime == null
                ? null
                : string.Format(TimeFormat, dayActivity.LocationPattern.FirstLocationDepartureTime);
        }

        private static string ExtractLocationArrivalTime(DayActivity dayActivity)
        {
            return dayActivity.LocationPattern.FirstLocationArrivalTime == null
                ? null
                : string.Format(TimeFormat, dayActivity.LocationPattern.FirstLocationArrivalTime);
        }

        private static string ExtractFirstLocation(DayActivity dayActivity)
        {
            return dayActivity.LocationPattern.FirstLocation;
        }

        private static Note ExtractNote(Model.Compact.Note note)
        {
            return note == null ? null : new Note { Content = note.Content };
        }

        private static string ExtractIsCovidImpacted(DayActivity dayActivity)
        {
            return ActivityTypeConvertor.GetIsCovidByActivityType(dayActivity.First.Type) ? "TRUE" : "FALSE";
        }
    }
}