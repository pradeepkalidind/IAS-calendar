using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Calendar.Model.Compact
{
    public class MonthActivity
    {
        protected MonthActivity()
        {
            Id = Guid.NewGuid();
            ActivityContent = new byte[64];
            FirstLocationActivityAllocationContent = new byte[64];
            LocationPatternContent = new byte[256];
            DayType = new byte[32];
            DataSource = new byte[32];
            Init();
        }

        private void Init()
        {
            for (var i = 0; i < 32; i++)
            {
                LocationPattern.Empty().WriteTo(i, LocationPatternContent);
            }
        }

        public Guid Id { get; protected set; }

        public MonthActivity(Guid userId, int year, int month) : this()
        {
            UserId = userId;
            Year = year;
            Month = month;
        }

        public Guid UserId { get; protected set; }
        public int Year { get; protected set; }
        public int Month { get; protected set; }
        public bool NoTravelConfirmation { get; protected set; }
        private byte[] ActivityContent { get; set; }
        private byte[] FirstLocationActivityAllocationContent { get; set; }
        private byte[] LocationPatternContent { get; set; }
        private byte[] DayType { get; set; }
        private byte[] DataSource { get; set; }

        public DayActivity[] GetDays()
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(Year, Month)).Select(CreateDay).ToArray();
        }
        
        public DayActivity[] GetDays(IEnumerable<int> days)
        {
            return days.Select(CreateDay).ToArray();
        }
        
        public void Update(IEnumerable<DayActivity> dayActivities)
        {
            dayActivities.ToList().ForEach(Update);     
        }

        public void Update(DayActivity dayActivity)
        {
            var index = dayActivity.Day - 1;

            var offset = index*2;
            SetActivity(offset, dayActivity.First);
            SetActivity(offset + 1, dayActivity.Second);
            SetDayType(index, dayActivity);
            SetEnterFrom(index, dayActivity);
            dayActivity.LocationPattern.WriteTo(index, LocationPatternContent);
        }

        private void SetEnterFrom(int index, DayActivity dayActivity)
        {
            DataSource[index] = (byte) dayActivity.EnterFrom;
        }

        private void SetDayType(int index, DayActivity dayActivity)
        {
            DayType[index] = (byte)dayActivity.DayType;
        }

        private void SetActivity(int offset, Activity activity)
        {
            ActivityContent[offset] = (byte) activity.Type;
            FirstLocationActivityAllocationContent[offset] = activity.FirstLocationAllocation;
        }

        private DayActivity CreateDay(int day)
        {
            var index = day-1;

            var offset = index*2;
            var first = CreateActivity(offset);
            var second = CreateActivity(offset + 1);
            var dayActivity = new DayActivity(first, second, LocationPattern.From(index, LocationPatternContent), day, new List<string>())
                                  {
                                      DayType = (DayType) DayType[index],
                                      EnterFrom = (EnterFromType)DataSource[index]
                                  };
            return  dayActivity;
        }

        private Activity CreateActivity(int offset)
        {
            var type = (ActivityType) ActivityContent[offset];
            var alloc = FirstLocationActivityAllocationContent[offset];
            return new Activity(type, alloc);
        }

        public DayActivity GetDay(int day)
        {
            return CreateDay(day);
        }

        public void setNoTravelConfirmation(bool hasNoTravelConfirmation)
        {
            NoTravelConfirmation = hasNoTravelConfirmation;
        }
    }
}