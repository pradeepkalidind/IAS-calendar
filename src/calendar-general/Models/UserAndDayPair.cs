using System;

namespace Calendar.General.Models
{
    public class UserAndDayPair
    {
        public Guid UserId { get; set; }
        public DateTime Day { get; set; }
    }
}