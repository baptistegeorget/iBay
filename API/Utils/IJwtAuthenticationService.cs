using System.Security.Claims;
using API.Models;

namespace API.Utils
{
    public interface IJwtAuthenticationService
    {
        User? Authenticate(ApplicationContext context, string email, string password);
        string GenerateToken(string secret, List<Claim> claims);
    }
}
