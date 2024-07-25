using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.General.Dto;
using Calendar.Model.Compact;
using Calendar.Model.Compact.Rules;
using Calendar.Persistence;

namespace Calendar.Model.Convertor
{
    public class MonthActivityCollection
    {
        private readonly Guid userId;

        private readonly ISessionWrapper session;

        private IList<MonthActivity> monthActivities;

        private readonly CalendarDayDtoToModelConvertor calendarDayDtoToModelConvertor;

        private readonly IEnumerable<IRule> rules;

        public MonthActivityCollection(Guid userId, ISessionWrapper session)
        {
            rules = InitRules();
            this.userId = userId;
            this.session = session;
            monthActivities = new List<MonthActivity>();
            calendarDayDtoToModelConvertor = new CalendarDayDtoToModelConvertor();
        }

        private static IEnumerable<IRule> InitRules()
        {
            return new List<IRule>
                       {
                           new WhenFirstLocationIsBlank(),
                           new WhenSecondLocationIsBlank(),
                           new WhenThirdLocationIsBlank(),
                           new WhenFirstActivityIsBlank()
                       };
        }

        public MonthActivityCollection AddDayActivities(IEnumerable<CalendarDayDto> dayDtos)
        {
            InitMonthActivities(dayDtos);
            dayDtos.ForEach(AddDayActivity);
            return this;
        }

        public MonthActivityCollection AddDayActivities(IEnumerable<DayDto> dayDtos,IEnumerable<DeletedDayDto> deletedDayDtos)
        {
            var yearMonthsOfDeleted = deletedDayDtos.Select(d=>new YearMonth(d.date)).Distinct();
            var yearMonths = dayDtos.Select(d => new YearMonth(d.date)).Distinct();
            var allYearMonths = yearMonths.Concat(yearMonthsOfDeleted).Distinct();
            monthActivities = allYearMonths.Select(GetMonthActivity).ToList();
            dayDtos.ForEach(AddDayActivity);
            deletedDayDtos.ForEach(AddEmptyDayActivity);
            return this;
        }

        private void InitMonthActivities(IEnumerable<CalendarDayDto> dayDtos)
        {
            var yearMonthes = dayDtos.Select(d=>new YearMonth(d.D)).Distinct();
            monthActivities = yearMonthes.Select(GetMonthActivity).ToList();
        }

        private MonthActivity GetMonthActivity(YearMonth ym)
        {
            var monthActivity = session.Query<MonthActivity>().Where(m => m.UserId == userId && m.Year == ym.Year && m.Month == ym.Month).FirstOrDefault();
            return monthActivity ?? new MonthActivity(userId, ym.Year, ym.Month);
        }

        public IEnumerable<MonthActivity> MonthActivities
        {
            get { return monthActivities; }    
        }

        private void AddDayActivity(CalendarDayDto dayDto)
        {
            var dayActivity = calendarDayDtoToModelConvertor.GetDayActivity(dayDto);
            AddDayActivity(dayActivity, dayDto.D);
        }

        private void AddDayActivity(DayDto dayDto)
        {
            var dayActivity = calendarDayDtoToModelConvertor.GetDayActivity(dayDto);
            AddDayActivity(dayActivity, dayDto.date);
        }

        private void AddEmptyDayActivity(DeletedDayDto dayDto)
        {
            var date = DateTime.Parse(dayDto.date);
            var dayActivity = DayActivity.Empty(date.Day);
            dayActivity.EnterFrom = EnterFromConvertor.GetTypeFrom(dayDto.enterFrom);
            FindMonthActivity(date).Update(dayActivity);
        }

        private void AddDayActivity(DayActivity dayActivity, string dateString)
        {
            rules.ForEach(r => r.Apply(dayActivity));
            var date = DateTime.Parse(dateString);
            FindMonthActivity(date).Update(dayActivity);
        }

        private MonthActivity FindMonthActivity(DateTime date)
        {
            return monthActivities.Where(m => m.Year == date.Year && m.Month == date.Month).FirstOrDefault();
        }

        private class YearMonth
        {
            public int Year { get; private set; }
            public int Month { get; private set; }

            public YearMonth(string d)
            {
                var date = DateTime.Parse(d);
                Year = date.Year;
                Month = date.Month;
            }

            private bool Equals(YearMonth other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.Year == Year && other.Month == Month;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (YearMonth)) return false;
                return Equals((YearMonth) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Year*397) ^ Month;
                }
            }
        }
    }
}