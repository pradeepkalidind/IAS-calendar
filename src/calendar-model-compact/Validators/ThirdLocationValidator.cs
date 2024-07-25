using System.Collections.Generic;

namespace Calendar.Model.Compact.Validators
{
    internal class ThirdLocationValidator : IValidator
    {
        private readonly LocationPattern locationPattern;
        private readonly List<string> errorMessages;

        public ThirdLocationValidator(LocationPattern locationPattern, List<string> errorMessages)
        {
            this.locationPattern = locationPattern;
            this.errorMessages = errorMessages;
        }

        public bool Applicable()
        {
            return !string.IsNullOrEmpty(locationPattern.ThirdLocation) && !LocationPattern.Intransit.Equals(locationPattern.ThirdLocation);
        }

        public bool IsValid()
        {
            var locationIsValid = locationPattern.SecondLocationDepartureTime != null && locationPattern.ThirdLocationArrivalTime != null ;
            if (!locationIsValid)
            {
                errorMessages.Add("Second travel should has departure location/time and arrival location/time.");
            }
            return locationIsValid;
        }
    }
}