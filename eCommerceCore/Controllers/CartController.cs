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
        [HttpGet()]
        async public Task<string> Get()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            //get cartId of User's cart
            var cartId = await context.Carts
                                .FirstOrDefaultAsync(b => b.CartStatus == true && b.UserId == userId);

            //get productId for that User
            //var query = from id in Cart
            //            join CartDetailsId in CartDetails on CartDetailsId equals cartId
            //            select new { ProductId = CartDetails.ProductId };

            return "no";
        }

       
        // POST: api/Cart
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
}
