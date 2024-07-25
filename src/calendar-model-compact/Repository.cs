using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Persistence;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Type;

namespace Calendar.Model.Compact
{
    public class Repository
    {
        private readonly ISessionWrapper session;

        public Repository(ISessionWrapper session)
        {
            this.session = session;
        }

        public UserActivityBase FindMostRecentBefore(long finishTimestamp)
        {
            var userActivity = FindMostRecentBefore<UserActivity>(finishTimestamp) as UserActivityBase;
            return userActivity ?? FindMostRecentBefore<UserActivityArchived>(finishTimestamp);
        }

        public IEnumerable<UserActivityBase> FindUserActivitiesInPeriod(long startTimestamp, long finishTimestamp)
        {
            var userActivities = FindUserActivitiesInPeriod<UserActivity>(startTimestamp, finishTimestamp);
            if (userActivities.Count() < 1)
            {
                return FindUserActivitiesInPeriod<UserActivityArchived>(startTimestamp, finishTimestamp).ToArray();
            }
            return userActivities.ToArray();
        }

        public IEnumerable<DateTime> FindUserActivitiesInPeriod(Guid userId,DateTime startTime,DateTime endTime )
        {
            return MonthSplitter.FindDaysInPeriod(startTime, endTime, userId, GetDaysByMonth);
        }

        private IEnumerable<DateTime> GetDaysByMonth(DateTime startTime, DateTime endTime, Guid userId)
        {
            var startTimestamp = startTime.Ticks;
            var finishTimestamp = endTime.Ticks;

            var days = FindUserActivitiesInPeriod<UserActivity>(startTimestamp, finishTimestamp)
                .Where(u => u.UserId == userId)
                .Select(u => u.Date).ToArray();

            var archivedDays = FindUserActivitiesInPeriod<UserActivityArchived>(startTimestamp, finishTimestamp)
                .Where(u => u.UserId == userId)
                .Select(u => u.Date).ToArray();

            return days.Concat(archivedDays);
        }

        private IQueryable<T> FindUserActivitiesInPeriod<T>(long startTimestamp, long finishTimestamp)
            where T : UserActivityBase
        {
            return session.Query<T>()
                .Where(f => f.Timestamp >= startTimestamp && f.Timestamp < finishTimestamp)
                .OrderByDescending(f => f.Timestamp);
        }

        private T FindMostRecentBefore<T>(long finishTimestamp) where T : UserActivityBase
        {
            return session.Query<T>().Where(f => f.Timestamp < finishTimestamp)
                .OrderByDescending(f => f.Timestamp).FirstOrDefault();
        }

        public static void SaveUserActivity(ISessionWrapper session, Guid userId, IEnumerable<DateTime> dates, string applicationId = null)
        {
            var timeStamp = DateTime.UtcNow.Ticks;
            dates.ToList().ForEach(d => session.Save(new UserActivity(d, userId, timeStamp, applicationId)));
        }

        public void SaveNote(Guid userId, DateTime date, Note note)
        {
            session.SaveOrUpdate(note);
            session.Save(UserActivity.Create(userId, date));
        }

        public void DeleteNote(Guid userId, DateTime noteDate)
        {
            session.Delete("from Note where Userid=? and date=?",
                new object[] { userId, noteDate },
                new IType[] { NHibernateUtil.Guid, NHibernateUtil.DateTime });
            session.Save(UserActivity.Create(userId, noteDate));
        }

        public bool NotHaveOldData(Guid userId)
        {
            var queryResult = session.CreateSQLQuery(string.Format("select count(id) c from [Days] where UserId='{0}'", userId))
                   .AddScalar("c", NHibernateUtil.Int32)
                   .UniqueResult();
            return Convert.ToInt32(queryResult) == 0;
        }

        public static List<List<T>> Split<T>(IEnumerable<T> collection, int maxLength)
        {
            if (maxLength < 1)
            {
                throw new ArgumentException("Invalid length.");
            }
            var result = new List<List<T>>();
            var list = collection.ToList();
            if (list.Count == 0) //optimization 1.
            {
                return result;
            }
            if (list.Count <= maxLength)  //optimization 2.
            {
                result.Add(list);
                return result;
            }
            for (var startIndex = 0; startIndex < list.Count; )
            {
                var remain = list.Count - startIndex;
                var count = remain > maxLength ? maxLength : remain;
                result.Add(list.GetRange(startIndex, count));
                startIndex += count;
            }
            return result;
        }
    }
}