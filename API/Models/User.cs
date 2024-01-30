namespace API.Models
{
    public class User
    {
        public long Id { get; set; }
        public required string Email { get; set; }
        public required string Pseudo { get; set; }
        public required string PasswordHash { get; set; }
        public Roles Role { get; set; } = Roles.User;
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Product> Products { get; set; } = new List<Product>();

        public enum Roles
        {
            User,
            Vendor
        }
    }
}
