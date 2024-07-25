namespace Calendar.Model.Compact
{
    public class EnterFromConvertor
    {
        public static EnterFromType GetTypeFrom(string enterFrom)
        {
            return EnterFromType.Mobile.ToString().Equals(enterFrom)
                ? EnterFromType.Mobile : EnterFromType.Empty;
        }
    }
}