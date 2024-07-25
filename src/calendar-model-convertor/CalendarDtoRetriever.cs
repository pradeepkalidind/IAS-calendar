using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Client.Schema;
using Calendar.General.Dto;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Note = Calendar.Model.Compact.Note;

namespace Calendar.Model.Convertor
{
    public class CalendarDtoRetriever
    {
        private readonly ISessionWrapper session;

        public CalendarDtoRetriever(ISessionWrapper session)
        {
            this.session = session;
        }

        public CalendarRoot Retrieve(Dictionary<Guid, List<DateTime>> userDays)
        {
            var result = new CalendarRoot();
            var users = new Dictionary<Guid, UserCalendar>();
            foreach (var userDay in userDays)
            {
                var userCalendar = Retrieve(userDay.Key, userDay.Value);
                if (userCalendar.Days.Count > 0)
                {
                    users.Add(userDay.Key, userCalendar);
                }
            }
            users.ForEach(user => user.Value.SortDays());
            result.Users = users.Values.ToList();
            return result;
        }

        public UserCalendar Retrieve(Guid userId)
        {
            var monthActivities = session.Query<MonthActivity>().Where(ma => ma.UserId == userId).ToArray();
            var notes = session.Query<Note>().Where(note => note.UserId == userId).ToArray();
            var days = GetDays(monthActivities, notes, m => m.GetDays());
            return UserCalendar.Build(userId, days);
        }

        public UserCalendar Retrieve(Guid userId, DateTime from, DateTime to)
        {
            var daysCollector = new DaysCollector(userId, session);
            return Retrieve(userId, daysCollector, from, to, m => DaysCollector.BuildDayActivities(m, from, to));
        }

        public UserCalendar Retrieve(Guid userId, IEnumerable<DateTime> dates)
        {
            var from = dates.Min(d => d);
            var to = dates.Max(d => d);
            var daysCollector = new DaysCollector(userId, session);
            return Retrieve(userId, daysCollector, from, to, m => DaysCollector.BuildDayActivities(m, dates));
        }

        public ChangedDaysDto RetrieveDelta(Guid userId, DateTime from, DateTime to)
        {
            var daysCollector = new DaysCollector(userId, session);
            var notes = GetNotes(userId, @from, to).ToList();
            var monthActivities = daysCollector.GetMonthActivities(@from, to).ToList();

            var dates = monthActivities.SelectMany(m => 
                DaysCollector.BuildDayActivities(m, @from, to)
                .Where(d => !d.IsEmpty(() => MonthModelToDayDtoConvertor.GetNote(m,notes,d.Day)))
                .Select(d => new DateTime(m.Year, m.Month, d.Day)));
            
            return BuildChangedDaysDto(monthActivities, notes, dates);
        }

        public ChangedDaysDto RetrieveChangedDays(Guid userId, IEnumerable<DateTime> dates)
        {
            if (dates.Count() == 0)
            {
                return new ChangedDaysDto {updatedDays = new List<DayDto>(), deletedDays = new List<DeletedDayDto>()};
            }
            var from = dates.Min(d => d);
            var to = dates.Max(d => d);
            var daysCollector = new DaysCollector(userId, session);
            var notes = GetNotes(userId, from, to);
            var monthActivities = daysCollector.GetMonthActivities(from, to);
            return BuildChangedDaysDto(monthActivities, notes, dates);
        }

        private static ChangedDaysDto BuildChangedDaysDto(IEnumerable<MonthActivity> monthActivities, IEnumerable<Note> notes, IEnumerable<DateTime> dates)
        {
            var updatedDays = new List<DayDto>();
            var deletedDays = new List<DeletedDayDto>();

            monthActivities.ForEach(
                monthActivity =>
                    {
                        var dayActivities = DaysCollector.BuildDayActivities(monthActivity, dates);
                        GetByMonth(monthActivity, notes, MonthModelToDayDtoConvertor.ToJsonDto, dayActivities)
                            .ForEach(updatedDays.Add);
                        GetDeletedByMonth(monthActivity, notes, MonthModelToDayDtoConvertor.ToDeleteJsonDto, dayActivities)
                            .ForEach(deletedDays.Add);
                    });
            return new ChangedDaysDto { updatedDays = updatedDays.OrderBy(d => d.date).ToList(), deletedDays = deletedDays.OrderBy(d => d.date).ToList() };
        }

        private UserCalendar Retrieve(Guid userId, DaysCollector daysCollector, DateTime from, DateTime to,Func<MonthActivity, IEnumerable<DayActivity>> buildDayActivities)
        {
            var notes = GetNotes(userId, from, to);
            var monthActivities = daysCollector.GetMonthActivities(from, to);
            var days = GetDays(monthActivities, notes, buildDayActivities);
            return UserCalendar.Build(userId, days);
        } 
        
        private static IEnumerable<Day> GetDays(IEnumerable<MonthActivity> monthActivities,IEnumerable<Note> notes,Func<MonthActivity, IEnumerable<DayActivity>> buildDayActivities)
        {
            return monthActivities.SelectMany(m =>
                buildDayActivities(m)
                    .Where(d => !d.IsEmpty(() => MonthModelToDayDtoConvertor.GetNote(m,notes,d.Day)))
                    .Select<DayActivity, Day>(d => MonthModelToDayDtoConvertor.ToDay(d, m, notes)).ToArray<Day>());
        }

        private static IEnumerable<DayDto> GetByMonth(MonthActivity monthActivity, IEnumerable<Note> notes, Func<DayActivity, MonthActivity, IEnumerable<Note>, DayDto> toDay, IEnumerable<DayActivity> dayActivities)
        {
            return dayActivities.Where(d => !d.IsEmpty(() => MonthModelToDayDtoConvertor.GetNote(monthActivity, notes, d.Day))).Select(d => toDay(d, monthActivity, notes)).ToArray();
        }

        private static IEnumerable<DeletedDayDto> GetDeletedByMonth(MonthActivity monthActivity, IEnumerable<Note> notes, 
                                                           Func<DayActivity, MonthActivity, DeletedDayDto> toDeletedDay, IEnumerable<DayActivity> dayActivities)
        {
            return dayActivities
               .Where(d => d.IsEmpty(() => MonthModelToDayDtoConvertor.GetNote(monthActivity,notes,d.Day)))
               .Select(d => toDeletedDay(d, monthActivity)).ToArray();
        }

        private IEnumerable<Note> GetNotes(Guid userId, DateTime from, DateTime to)
        {
            return session.Query<Note>().Where(note => note.UserId == userId && note.Date >= from && note.Date <= to).ToArray();
        }
    }
}