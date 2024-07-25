using System;

namespace Calendar.Model.Compact
{
    public abstract class UserActivityBase
    {
        protected UserActivityBase()
        {
            Id = Guid.NewGuid();
        }

        protected UserActivityBase(DateTime date, Guid userId, long timestamp):this()
        {
            Date = date;
            UserId = userId;
            Timestamp = timestamp;
        }

        public virtual Guid Id { get; protected set; }
        public virtual DateTime Date { get; protected set; }
        public virtual Guid UserId { get; protected set; }
        public virtual long Timestamp { get; protected set; }
        public virtual string ApplicationId { get; protected set; }

        public static UserActivity Create(Guid userId, DateTime date)
        {
            return new UserActivity(date,userId,DateTime.UtcNow.Ticks);
        }
    }

    public class UserActivity : UserActivityBase
    {
        protected UserActivity()
            : base()
        {
        }

        public UserActivity(DateTime date, Guid userId, long timestamp)
            : base(date, userId, timestamp)
        {
        }

        public UserActivity(DateTime date, Guid userId, long timestamp, string applicationId)
            : this(date, userId, timestamp)
        {
            ApplicationId = applicationId;
        }
    }
    
    public class UserActivityArchived : UserActivityBase
    {
        protected UserActivityArchived()
            : base()
        {
        }

        public UserActivityArchived(DateTime date, Guid userId, long timestamp)
            : base(date, userId, timestamp)
        {
        }
    }
}