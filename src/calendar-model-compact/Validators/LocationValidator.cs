using System.Collections.Generic;

namespace Calendar.Model.Compact.Validators
{
    internal class LocationValidator : IValidator
    {
        protected readonly string location;
        private readonly List<string> errorMessages;

        public LocationValidator(string location, List<string> errorMessages)
        {
            this.location = location;
            this.errorMessages = errorMessages;
        }

        public bool Applicable()
        {
            return !string.IsNullOrEmpty(location);
        }

        public bool IsValid()
        {
            var locationIsValid = false;
            var subString = location.Split('/');
            if (subString.Length == 3)
            {
                locationIsValid = subString[1] == "Country";
            }
            else if (subString.Length == 5)
            {
                locationIsValid = subString[1] == "Country" && subString[3] == "TaxUnit";
            }
            if (!locationIsValid)
            {
                errorMessages.Add(string.Format("Location \"{0}\" is invalid.", location));
            }
            return locationIsValid;
        }
    }
}