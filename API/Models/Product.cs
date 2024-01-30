namespace API.Models
{
    public class Product
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public byte[]? Image { get; set; } = null;
        public decimal Price { get; set; } = 0;
        public bool IsAvailable { get; set; } = false;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public long UserId { get; set; }
        public required User User { get; set; }
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}
