using static API.Models.User;

namespace API.Models
{
    public class UserRequest
    {
        public required string Email { get; set; }
        public required string Pseudo { get; set; }
        public required string Password { get; set; }
        public Roles Role { get; set; } = Roles.User;
    }
}
