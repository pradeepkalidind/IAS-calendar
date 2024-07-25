using System;

namespace Calendar.Model.Compact
{
    public class NationalHolidaySetting
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Country { get; set; }

        public NationalHolidaySetting()
        {
            Id = Guid.NewGuid();
        }

        public NationalHolidaySetting(Guid userId,string country):this()
        {
            UserId = userId;
            Country = country;
        }
    }
}