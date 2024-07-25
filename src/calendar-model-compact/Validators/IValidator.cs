namespace Calendar.Model.Compact.Validators
{
    public interface IValidator
    {
        bool Applicable();
        bool IsValid();
    }
}