namespace API.Models
{
    public class ProductResponse
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public byte[]? Image { get; set; } = null;
        public decimal Price { get; set; } = 0;
        public bool IsAvailable { get; set; } = false;
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public long UserId { get; set; }
    }
}
