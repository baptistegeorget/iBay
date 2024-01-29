namespace API.Models
{
    public class ProductRequest
    {
        public required string Name { get; set; }
        public byte[]? Image { get; set; } = null;
        public decimal Price { get; set; } = 0;
        public bool IsAvailable { get; set; } = false;
    }
}
