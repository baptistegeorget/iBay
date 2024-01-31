namespace API.Models
{
    public class CartResponse
    {
        public long Id { get; set; }
        public bool IsValidate { get; set; } = false;
        public ICollection<long> ProductsId { get; set; } = new List<long>();
        public IDictionary<long, int> Quantities { get; set; } = new Dictionary<long, int>();
    }
}
