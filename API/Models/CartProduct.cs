namespace API.Models
{
    public class CartProduct
    {
        public long Id { get; set; }
        public long CartId { get; set; }
        public required Cart Cart { get; set; }
        public long ProductId { get; set; }
        public required Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
