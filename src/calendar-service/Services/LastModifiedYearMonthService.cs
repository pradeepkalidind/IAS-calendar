using System;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Persistence;

namespace Calendar.Service.Services
{
    public class LastModifiedYearMonthService
    {
        private readonly ISessionWrapper session;

        public LastModifiedYearMonthService(ISessionWrapper session)
        {
            this.session = session;
        }

        public LastModifiedYearMonth GetByUser(Guid userId)
        {
            return session.Query<LastModifiedYearMonth>().FirstOrDefault(s => s.UserId == userId);
        }

        public void SetLastModifiedMonth(Guid userId, int year, int month)
        {
            var existingVisitStatus = session.Query<LastModifiedYearMonth>().FirstOrDefault(s => s.UserId == userId);
            if (existingVisitStatus == null)
            {
                session.Save(new LastModifiedYearMonth(userId)
                {
                    LastModifiedMonth = month,
                    LastModifiedYear = year
                });
            }
            else
            {
                existingVisitStatus.LastModifiedMonth = month;
                existingVisitStatus.LastModifiedYear = year;
                session.Update(existingVisitStatus);
            }
        }
    }
}