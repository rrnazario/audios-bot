using AudiosBot.Domain.Interfaces;
using AudiosBot.Infra.Constants;

namespace AudiosBot.Domain.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public bool IsValidUser(string username, string password)
         => AdminConstants.Users.TryGetValue(username, out var pass) && password.Equals(pass);
    }
}
