using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eCommerceCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CartController : ControllerBase
    {
        private readonly AppDbContext context;
        public CartController(AppDbContext context) => this.context = context; 

        // GET: api/Cart
        [HttpGet]
        async public Task<IEnumerable<ProductObject>> Get()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            //get cartId of User's cart
            var cartId = await context.Carts
                                .FirstOrDefaultAsync(b => b.CartStatus == true && b.UserId == userId);

            //get products for that User 
            await ( from c in context.Carts
                    join p in context.Products on c.Id equals p.Id
                    where c.Id == cartId.Id
                    select new ProductObject
                    {
                        Id = p.Id,
                        Description = p.Description,
                        //getImage
                        Pricing = p.Pricing,
                        ShippingCost = p.ShippingCost,
                        Name = p.ProductName
                    }).ToListAsync();
            //return as an Object
            return context.ProductObjects.ToList();
        }

        // GET: api/Cart/5
        [HttpGet("{id}", Name = "GetCart")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Cart
        [HttpPost]
        async public void Post([FromBody] ProductObject data)
        {
            var resp = new Response { };
            try
            {                
                //get the user
                var userId = HttpContext.Session.GetInt32("UserId");

                //get Current CartId
                var cartId = await context.Carts
                                    .FirstOrDefaultAsync(b => b.CartStatus == true && b.UserId == userId);

                if (cartId != null)
                {
                    var productId = context.ProductObjects.FirstOrDefault(p => p.Id == data.Id);
                    if (productId != null)
                    {
                        //check product existance in product table
                        var productExist = await context.Products
                                    .FirstOrDefaultAsync(p => p.Id == productId.Id);
                        if (productExist != null)
                        {
                            //fetch all the data from that row
                            //insert userid, cartid, productid, quantities into cartdetails
                            
                        }
                        else
                        {
                            resp.Success = false;
                            resp.Message = "Product Do Not Exist";
                        }
                    }
                    else
                    {
                        resp.Success = false;
                        resp.Message = "Product Not Found";
                    }
                }
                else
                {
                    //create new cartId for the User
                }
            }catch(Exception exception)
            {
                resp.Success = false;
                resp.Message = exception.Message;
            }
            
        }

        // PUT: api/Cart/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class ProductObject
    {
        public int Id { get; set; }
        public string Description { get; set; }
        //getImage
        public double Pricing { get; set; }
        public double ShippingCost { get; set; }
        public string Name { get; set; }
        public int Quantities { get; set; }
    }

    public class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
