namespace BeerProjectAPI.Models
{
    public class Quote
    {
        public int QuoteId { get; set; }
        public int ClientId { get; set; }

        public int WholesalerId { get; set; }

        public List<QuoteDetails> QDList { get; set; }
    }
}
