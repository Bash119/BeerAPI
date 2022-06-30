namespace BeerProjectAPI.Models
{
    public class WholesalerBeer
    {
        public int Id { get; set; }
        public int WholesalerId { get; set; }

        public int BeerId { get; set; }

        public int Quantity { get; set; }   
    }
}
