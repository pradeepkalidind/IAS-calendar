using System.Collections.Generic;

namespace Calendar.Model.Compact.Validators
{
    internal class SecondLocationValidator : IValidator
    {
        private readonly LocationPattern locationPattern;
        private readonly List<string> errorMessages;

        public SecondLocationValidator(LocationPattern locationPattern, List<string> errorMessages)
        {
            this.locationPattern = locationPattern;
            this.errorMessages = errorMessages;
        }

        public bool Applicable()
        {
            return !string.IsNullOrEmpty(locationPattern.SecondLocation) && !LocationPattern.Intransit.Equals(locationPattern.SecondLocation);
        }

        public bool IsValid()
        {
            var locationIsValid = locationPattern.FirstLocation != null && locationPattern.FirstLocationDepartureTime != null && locationPattern.SecondLocationArrivalTime != null;
            if (!locationIsValid)
            {
                errorMessages.Add("First travel should has departure location/time and arrival location/time.");
            }
            return locationIsValid;
        }
    }
}