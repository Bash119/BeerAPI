using Microsoft.AspNetCore.Mvc;
using BeerProjectAPI.Context;
using BeerProjectAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BeerProjectAPI.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class BeerController : ControllerBase
    {
        private BeerDbContext _beerContext;

        public BeerController(BeerDbContext beerContext)
        {
            _beerContext = beerContext;
        }

        [HttpGet]
        public IEnumerable<Beer> GetAllBeers()
        {
            return _beerContext.Beers;
        }

        // GET api/<BeerController>/5
        [HttpGet("{id}")]
        public IEnumerable<Beer> GetBeersListByBreweryId(int id)
        {
            return _beerContext.Beers.Where(s => s.BreweryId == id);
        }

        // POST api/<BeerController>
        [HttpPost]
        public void AddNewBeer([FromBody] Beer value)
        {
            _beerContext.Beers.Add(value);
            _beerContext.SaveChanges();
        }

        [HttpPost]
        public void AddSaleBeerToWholesaler([FromBody] WholesalerBeer value)
        {
            _beerContext.WholesalerBeers.Add(value);
            _beerContext.SaveChanges();
        }
        
        [HttpPost]
        public void UpdateRemainingBeerAmount(int beerid,int salerid,int qty)
        {
            if (qty > 0)
            {
                var obj = _beerContext.WholesalerBeers.FirstOrDefault(s => s.BeerId == beerid && s.WholesalerId== salerid);
                if (obj != null) obj.Quantity= qty;
                
            }

        }


        // PUT api/<BeerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BeerController>/5
        [HttpDelete("{id}")]
        public void DeleteBeer(int id)
        {
            var beer = _beerContext.Beers.FirstOrDefault(s => s.BeerId == id);
            if (beer != null)
            {
                _beerContext.Beers.Remove(beer);
                _beerContext.SaveChanges();
            }
        }

        [HttpPost]
        public Response OrderQuote([FromBody] Quote quote)
        {
            Response response = new Response();
            if (String.IsNullOrEmpty(quote.WholesalerId.ToString()))
            {
                response.QuoteId = quote.QuoteId;
                response.Message = "The beer must be sold by the wholesaler";
            }
            else 
            {
                int count = 0;
            foreach(var item in quote.QDList)
            {
                count += item.Quantity;
            }
            if (count <= 0) {

                response.QuoteId = quote.QuoteId;
                response.Message = "The order cannot be empty";
            }
            else
            {  
                var obj = _beerContext.Wholesalers.First(s => s.WholesalerId == quote.WholesalerId);
                if (obj != null)
                {
                    var query = quote.QDList.GroupBy(x => x)
                                            .Where(g => g.Count() > 1)
                                            .Select(y => y.Key)
                                             .ToList();
                    if(query.Count > 0 )
                    {
                        response.QuoteId = quote.QuoteId;
                        response.Message = "There can't be any duplicate in the order";
                    }
                    else
                    {
                       if(!CheckBeerQtyPerWholesaler(quote))
                        {
                            response.QuoteId = quote.QuoteId;
                            response.Message = "The number of beers cannot be greater than the wholesaler's stock ";
                        }
                        else 
                        {
                                int co=quote.QDList.Count();
                                if(co >20)
                                {
                                    response.TotalPrice =TotalPriceCalculated(quote) * Convert.ToDecimal(0.2);
                                    response.QuoteId = quote.QuoteId;
                                    response.Message = "You have got 20% discount,Thank you for your purchase ";
                                }
                                else if(co >10)
                                {
                                    response.TotalPrice = TotalPriceCalculated(quote) * Convert.ToDecimal(0.1);
                                    response.QuoteId = quote.QuoteId;
                                    response.Message = "You have got 10% discount ,Thank you for your purchase ";

                                }
                                else 
                                {
                                    response.TotalPrice = TotalPriceCalculated(quote);
                                    response.QuoteId = quote.QuoteId;
                                    response.Message = "Thank you for your purchase ";

                                }


                            }
                    }
                }
                else
                {
                    response.QuoteId = quote.QuoteId;
                    response.Message = "The wholesaler must exist";
                }

            }

            
            }
                        return response;
        }

        private bool CheckBeerQtyPerWholesaler(Quote quote)
        {
         foreach(var item in quote.QDList)
            {
                int cwb = _beerContext.WholesalerBeers.Where(s => s.WholesalerId == quote.WholesalerId && s.BeerId == item.BeerId).Count();
                if(cwb < item.Quantity) return false;
            }
         return true;
        }

        private decimal TotalPriceCalculated(Quote quote)
        {
            decimal total = 0;
            foreach(var item in quote.QDList)
            {
                WholesalerBeer obj = _beerContext.WholesalerBeers.Where(s => s.WholesalerId == quote.WholesalerId && s.BeerId == item.BeerId).First();
                Beer beer = _beerContext.Beers.Where(s => s.BeerId == item.BeerId).First();
                total += obj.Quantity * beer.Price;
            }
            return total;
        }
    }
}
