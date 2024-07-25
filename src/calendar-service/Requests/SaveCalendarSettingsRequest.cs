namespace Calendar.Service.Requests
{
    public class SaveCalendarSettingsRequest
    {
        public string Country { get; set; }
        public int[] WorkWeekDefaults { get; set; }
    }
}