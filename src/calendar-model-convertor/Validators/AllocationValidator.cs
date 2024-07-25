using System.Collections.Generic;
using Calendar.Model.Compact.Validators;

namespace Calendar.Model.Convertor.Validators
{
    internal class AllocationValidator : IValidator
    {
        private readonly decimal? allocation;
        private readonly List<string> errorMessages;

        public AllocationValidator(double? allocation, List<string> errorMessages)
        {
            this.allocation = allocation == null ? (decimal?)null : new decimal(allocation.Value);
            this.errorMessages = errorMessages;
        }

        public bool Applicable()
        {
            return allocation != null;
        }

        public bool IsValid()
        {
            var isValid = allocation >= 0 && allocation <= 1;
            if (!isValid)
            {
                errorMessages.Add(string.Format("allocation {0} should be in range [0, 1]", allocation));
            }
            return isValid;
        }
    }
}