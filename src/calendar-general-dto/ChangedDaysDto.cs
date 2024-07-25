using System.Collections.Generic;

namespace Calendar.General.Dto
{
    public class ChangedDaysDto
    {
        public IEnumerable<DayDto> updatedDays;
        public IEnumerable<DeletedDayDto> deletedDays;
        public string enterFrom;
    }
}