using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models;
using Microsoft.IdentityModel.Tokens;

namespace API.Utils
{
    public class JwtAuthenticationService : IJwtAuthenticationService
    {
        public User? Authenticate(ApplicationContext context, string email, string password)
        {
            var user = context.Users
                .Where(user => user.Email.ToLower().Equals(email.ToLower()))
                .FirstOrDefault();

            if (user == null || !Password.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public string GenerateToken(string secret, List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
