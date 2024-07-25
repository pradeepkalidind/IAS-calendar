using System;

namespace Calendar.Model.Compact
{
    public class VisitedUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public VisitedUser()
        {
            Id = Guid.NewGuid();
        }
        public VisitedUser(Guid userId):this()
        {
            UserId = userId;
        }
    }
}