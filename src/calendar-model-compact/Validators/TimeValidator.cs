using System.Collections.Generic;

namespace Calendar.Model.Compact.Validators
{
    internal class TimeValidator : IValidator
    {
        private readonly int? time;
        private readonly List<string> errorMessages;

        public TimeValidator(int? time, List<string> errorMessages)
        {
            this.time = time;
            this.errorMessages = errorMessages;
        }

        public bool Applicable()
        {
            return time != null;
        }

        public bool IsValid()
        {
            var isValid = time >= 0 && time < 24;
            if (!isValid)
            {
                errorMessages.Add(string.Format("time {0} should be in range [0, 23)", time));
            }
            return isValid;
        }
    }
}