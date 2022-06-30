namespace BeerProjectAPI.Models
{
    public class Beer
    {
        public int BeerId { get; set; }
        public string BeerName { get; set; }
        public string AlcoholContent { get; set; }

        public decimal Price { get; set; }

        public int BreweryId { get; set; }

    }
}
