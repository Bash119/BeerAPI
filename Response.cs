namespace BeerProjectAPI.Models
{
    public class Response
    {
        public int Id { get; set; }
        public int QuoteId { get; set; }
        public string Message { get; set; }
        public decimal TotalPrice { get; set; } 
    }
}
