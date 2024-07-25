namespace Calendar.Authentication
{
    public interface IUserRepository
    {
        IUser Authenticate(string userName, string password, out string errorMessage);
    }
}