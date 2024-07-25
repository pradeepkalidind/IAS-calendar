namespace Calendar.Authentication
{
    public interface IUser
    {
        string Identity { get; }
        string[] Roles { get; }
    }
}