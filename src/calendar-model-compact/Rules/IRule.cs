namespace Calendar.Model.Compact.Rules
{
    public interface IRule
    {
        bool Applicable(DayActivity dayActivity);
        void Apply(DayActivity dayActivity);
    }
}