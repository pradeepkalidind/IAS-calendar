using System;

namespace Calendar.Model.Compact
{
    public class LastModifiedYearMonth
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int? LastModifiedMonth { get; set; }
        public int? LastModifiedYear { get; set; }

        public LastModifiedYearMonth()
        {
            Id = Guid.NewGuid();
        }
        public LastModifiedYearMonth(Guid userId):this()
        {
            UserId = userId;
        }
    }
}