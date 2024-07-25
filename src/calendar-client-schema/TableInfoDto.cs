namespace Calendar.Client.Schema
{
    public class TableInfoDto
    {
        public CalendarInfoDto All { get; set; }
        public CalendarInfoDto NonPii { get; set; }
    }

    public class CalendarInfoDto
    {
        public UserCalendar CalendarInfo { get; set; }
    }
}