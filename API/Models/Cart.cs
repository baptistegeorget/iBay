namespace API.Models
{
    public class Cart
    {
        public long Id { get; set; }
        public bool IsValidate { get; set; } = false;
        public long UserId { get; set; }
        public required User User { get; set; }
        public ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
    }
}
