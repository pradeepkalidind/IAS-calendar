namespace Calendar.General.Dto
{
    public class DayDto : DtoBase
    {
        public string type;
        public string firstActivity;
        public double? firstActivityAllocation;
        public string secondActivity;
        public double? secondActivityAllocation;
        public string firstLocation;
        public int? firstLocationArrivalTime;
        public int? firstLocationDepartureTime;
        public string secondLocation;
        public int? secondLocationArrivalTime;
        public int? secondLocationDepartureTime;
        public string thirdLocation;
        public int? thirdLocationArrivalTime;
        public string note;
        public bool isCovid;
    }
}