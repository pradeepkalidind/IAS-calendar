using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.General.Dto;
using Calendar.Model.Compact;
using Calendar.Model.Compact.Validators;
using Calendar.Model.Convertor.Validators;

namespace Calendar.Model.Convertor
{
    public class CalendarDayDtoToModelConvertor
    {
        public DayActivity GetDayActivity(CalendarDayDto dayDto)
        {
            return GetDayActivity(ConvertToDayDto(dayDto));
        }

        public DayActivity GetDayActivity(DayDto dayDto)
        {
            var locationPattern = new LocationPattern(dayDto.firstLocation, dayDto.firstLocationArrivalTime,
                dayDto.firstLocationDepartureTime,
                dayDto.secondLocation, dayDto.secondLocationArrivalTime, dayDto.secondLocationDepartureTime,
                dayDto.thirdLocation, dayDto.thirdLocationArrivalTime);

            var errorMessages = new List<string>();
            var isValid = IsValid(dayDto, errorMessages);

            var dayActivity = new DayActivity(GetActivityByLongName(dayDto.firstActivity, dayDto.firstActivityAllocation, dayDto.isCovid, dayDto.date),
                    GetActivityByLongName(dayDto.secondActivity, dayDto.secondActivityAllocation, dayDto.isCovid, dayDto.date),
                    locationPattern, DateTime.Parse(dayDto.date).Day, errorMessages)
                                  {
                                      EnterFrom = EnterFromConvertor.GetTypeFrom(dayDto.enterFrom)
                                  };

            if (!isValid || !dayActivity.IsValid())
            {
                throw new ArgumentException(String.Join(",", errorMessages));
            }
            var activityAllocationRule = new ActivityAllocationRule(dayActivity);

            if (dayDto.firstActivityAllocation == null)
            {
                dayActivity.First.FirstLocationAllocation = activityAllocationRule.DefaultFirstActivityAllocation;
            }
            if (dayDto.secondActivityAllocation == null)
            {
                dayActivity.Second.FirstLocationAllocation = activityAllocationRule.DefaultSecondActivityAllocation;
            }
            return dayActivity;
        }

        public bool IsValid(DayDto dayDto, List<string> errorMessages)
        {
            var validators = new IValidator[]
                                 {
                                     new ActivityValidator(dayDto.firstActivity, errorMessages),
                                     new ActivityValidator(dayDto.secondActivity, errorMessages),
                                     new AllocationValidator(dayDto.firstActivityAllocation, errorMessages),
                                     new AllocationValidator(dayDto.secondActivityAllocation, errorMessages)
                                 };
            return validators.Where(v => v.Applicable()).All(r => r.IsValid());
        }

        private static DayDto ConvertToDayDto(CalendarDayDto dayDto)
        {
            return new DayDto
                       {
                           date = dayDto.D,
                           firstActivity = ActivityTypeConvertor.GetActivityLongNameByShortName(dayDto.A),
                           secondActivity = ActivityTypeConvertor.GetActivityLongNameByShortName(dayDto.SA),
                           firstLocation = GetLocation(dayDto.FL),
                           firstLocationArrivalTime = dayDto.FLA,
                           firstLocationDepartureTime = dayDto.FLD,
                           secondLocation = GetLocation(dayDto.SL),
                           secondLocationArrivalTime = dayDto.SLA,
                           secondLocationDepartureTime = dayDto.SLD,
                           thirdLocation = GetLocation(dayDto.TL),
                           thirdLocationArrivalTime = dayDto.TLA,
                           note = dayDto.N,
                           isCovid = ActivityTypeConvertor.GetIsCovidByActivityShortName(dayDto.A)
                       };
        }


        private static Activity GetActivityByLongName(string activityAbbr,
            double? activityFirstLocationAllocation,
            bool isCovid,
            string dayDtoDate)
        {
            var alloc = activityFirstLocationAllocation.HasValue ? activityFirstLocationAllocation.Value * 100D : 0D;
            var activityType = ActivityTypeConvertor.GetActivityTypeByLongName(activityAbbr, isCovid);

            var isBefore401 = new DateTime(2019, 04, 01) > DateTime.Parse(dayDtoDate);
            
            if (isBefore401)
            {
                switch (activityType)
                {
                    case ActivityType.COVID19Work:
                        activityType = ActivityType.Work;
                        break;
                    case ActivityType.COVID19NonWork:
                        activityType = ActivityType.NonWork;
                        break;
                }
            }

            return new Activity(activityType, (byte) alloc);
        }

        private static string GetLocation(string locationAbbr)
        {
            if (string.IsNullOrWhiteSpace(locationAbbr))
            {
                return locationAbbr;
            }
            return "/Country/" + locationAbbr.Replace("/", "/TaxUnit/");
        }
    }
}