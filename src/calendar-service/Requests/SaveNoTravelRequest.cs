namespace Calendar.Service.Requests
{
    public class SaveNoTravelRequest
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public bool HasNoTravelConfirmation { get; set; }
    }
}