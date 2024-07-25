using Calendar.Authentication;

namespace Calendar.Tests.Unit.Authentications
{
    public class FakeUserRepository : IUserRepository
    {
        private readonly string correctUserName;
        private readonly string correctPassword;
        private readonly string errorMessage;

        public FakeUserRepository(string correctUserName, string correctPassword, string errorMessage)
        {
            this.correctUserName = correctUserName;
            this.errorMessage = errorMessage;
            this.correctPassword = correctPassword;
        }

        public string CorrectUserName
        {
            get { return correctUserName; }
        }

        public string CorrectPassword
        {
            get { return correctPassword; }
        }

        public IUser Authenticate(string userName, string password, out string error)
        {
            error = errorMessage;
            return CorrectUserName.Equals(userName) && CorrectPassword.Equals(password)
                       ? new FakeUser(CorrectUserName, new[] { "Role" })
                       : null;
        }
    }
}