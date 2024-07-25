namespace Calendar.Model.Compact.Rules
{
    public abstract class WhenStringPropetyIsBlank : IRule
    {
        public abstract void DoApply(DayActivity dayActivity);

        protected bool Applicable(string property)
        {
            return string.IsNullOrEmpty(property);
        }

        public abstract bool Applicable(DayActivity dayActivity);

        public void Apply(DayActivity dayActivity)
        {
            if (!Applicable(dayActivity)) return;
            DoApply(dayActivity);
        }
    }
}