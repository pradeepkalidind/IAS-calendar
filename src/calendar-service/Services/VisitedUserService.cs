using System;
using System.Linq;
using Calendar.Model.Compact;
using Calendar.Persistence;

namespace Calendar.Service.Services
{
    public class VisitedUserService
    {
        private readonly ISessionWrapper session;

        public VisitedUserService(ISessionWrapper session)
        {
            this.session = session;
        }

        public bool IsVisitedUser(Guid userId)
        {
            return session.Query<VisitedUser>().Any(s => s.UserId == userId);
        }

        public void MarkAsVisited(Guid userId)
        {
            var existingVisitStatus = session.Query<VisitedUser>().FirstOrDefault(s => s.UserId == userId);
            if (existingVisitStatus == null)
            {
                session.Save(new VisitedUser(userId));
            }
        }
    }
}