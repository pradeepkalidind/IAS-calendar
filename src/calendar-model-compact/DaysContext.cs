using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Persistence;
using NHibernate;
using NHibernate.Linq;

namespace Calendar.Model.Compact
{
    public class DaysContext
    {
        private readonly Guid userId;
        private readonly IEnumerable<DateTime> dates;
        private readonly IEnumerable<Note> notes;
        private readonly IEnumerable<MonthActivity> monthActivities;
        private Action<ISessionWrapper> extend;

        public DaysContext(Guid userId, IEnumerable<DateTime> dates, IEnumerable<Note> notes, IEnumerable<MonthActivity> monthActivities)
        {
            this.userId = userId;
            this.dates = dates;
            this.notes = notes;
            this.monthActivities = monthActivities;
        }

        public Guid UserId
        {
            get { return userId; }
        }

        public IEnumerable<DateTime> Dates
        {
            get { return dates; }
        }

        public IEnumerable<Note> Notes
        {
            get { return notes; }
        }

        public IEnumerable<MonthActivity> MonthActivities
        {
            get { return monthActivities; }
        }

        public Action<ISessionWrapper> Extend
        {
            get { return extend; }
        }

        public void Save(ISessionWrapper session)
        {
            var existedNotes = GetExistedNotes(UserId, Dates, session);

            using (var transaction = session.BeginTransaction())
            {
                existedNotes.ToList().ForEach(session.Delete);
                
                Notes.ToList().ForEach(session.Save);
                MonthActivities.ToList().ForEach(session.Save);
                if (Extend != null)
                {
                    Extend(session);    
                }
                transaction.Commit();
            }
        }

        private static IEnumerable<Note> GetExistedNotes(Guid userId, IEnumerable<DateTime> dates, ISessionWrapper session)
        {
            if (dates.Count() == 0)
            {
                return new Note[0];
            }
            var start = dates.Min(d => d);
            var end = dates.Max(d => d);
            var existedNotesInPeriod = session.Query<Note>().Where(n => n.UserId == userId && n.Date >= start && n.Date <= end).ToArray();
            return existedNotesInPeriod.Where(n => dates.Contains(n.Date));
        }

        public DaysContext ExtendAction(Action<ISessionWrapper> action)
        {
            extend = action;
            return this;
        }
    }
}