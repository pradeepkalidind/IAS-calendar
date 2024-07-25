using Microsoft.AspNetCore.Authorization;

namespace Calendar.Service.Filters
{
    public class UserBasicAuthAttribute : AuthorizeAttribute 
    {
    }
}