namespace BeerProjectAPI.Models
{
    public class QuoteDetails
    {
        public int Id { get; set; }
        public int QuoteId { get; set; }    

        public int BeerId { get; set; }
        public int Quantity { get; set; }   
    }
}
