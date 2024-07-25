namespace Calendar.Service.Requests
{
    public class SaveWorkWeeksDefaultRequest
    {
        public string Token { get; set; }
        public int[] WorkWeekDefaults { get; set; }
    }
}