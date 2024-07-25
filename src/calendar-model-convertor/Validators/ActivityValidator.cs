using System.Collections.Generic;
using Calendar.Model.Compact;
using Calendar.Model.Compact.Validators;

namespace Calendar.Model.Convertor.Validators
{
    internal class ActivityValidator : IValidator
    {
        private readonly List<string> errorMessages;
        protected readonly string activity;

        public ActivityValidator(string activity, List<string> errorMessages)
        {
            this.errorMessages = errorMessages;
            this.activity = activity;
        }

        public bool Applicable()
        {
            return !string.IsNullOrEmpty(activity);
        }

        public bool IsValid()
        {
            var exist = ActivityTypeConvertor.Exist(activity);
            if (!exist)
            {
                errorMessages.Add(string.Format("Activity Type \"{0}\" is invalid", activity));
            }
            return exist;
        }
    }
}