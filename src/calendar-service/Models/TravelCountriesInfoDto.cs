using System.Collections.Generic;

namespace Calendar.Service.Models
{
    public class TravelCountriesInfoDto
    {
        public int year { get; set; }
        public Dictionary<string, double> countries { get; set; }
    }
}