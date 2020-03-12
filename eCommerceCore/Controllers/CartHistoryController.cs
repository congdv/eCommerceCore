using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerceCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartHistoryController : Controller
    {
        private readonly AppDbContext context;
        public CartHistoryController(AppDbContext context) => this.context = context;

        // GET: api/Cart
        [HttpGet]
        async public Task<IActionResult> Get()
        {
            var resp = new Response { };
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                //check if userId is null
                if (userId == null)
                {
                    return BadRequest(new { success = false, message = "Login Failed" });
                }

                //get cartId of User's cart with cart status true
                var cartId = await context.Carts
                                    .FirstOrDefaultAsync(b => b.CartStatus == true && b.UserId == userId);

                if (cartId != null)
                {
                    List<ProductObject> products = new List<ProductObject>();
                    //get products for that User 
                    products = (from c in context.Carts
                                join cd in context.CartsDetails on c.Id equals cd.CartId
                                join p in context.Products on cd.ProductId equals p.Id
                                where c.Id == cartId.Id
                                select new ProductObject
                                {
                                    Id = p.Id,
                                    Description = p.Description,
                                    ImagePath = p.ImagePath,
                                    //get current price from CartDetails
                                    Pricing = cd.CurrentPrice,
                                    ShippingCost = p.ShippingCost,
                                    Name = p.ProductName,
                                    ProductId = p.Id,
                                    Quantities = cd.Quantities
                                }).ToList();
                    resp.Data = products;
                    resp.Success = true;
                }
                else
                {
                    return BadRequest(new { success = false, message = "Cart Not Found" });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "Invalid Request" });
            }
            return Ok(new { success = resp.Success, data = resp.Data});
        }
    }
}