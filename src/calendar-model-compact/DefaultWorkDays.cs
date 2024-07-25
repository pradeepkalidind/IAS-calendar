using System;

namespace Calendar.Model.Compact
{
    public class DefaultWorkDays
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public byte Days { get; set; }
        public long Timestamp { get; set; }

        public DefaultWorkDays()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow.Ticks;
        }
    }
}