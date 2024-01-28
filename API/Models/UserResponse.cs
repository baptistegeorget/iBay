using static API.Models.User;

namespace API.Models
{
    public class UserResponse
    {
        public long Id { get; set; }
        public required string Email { get; set; }
        public required string Pseudo { get; set; }
        public Roles Role { get; set; }
    }
}
