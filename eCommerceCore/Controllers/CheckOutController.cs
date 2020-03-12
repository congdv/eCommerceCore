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
    public class CheckOutController : Controller
    {
        private readonly AppDbContext context;
        public CheckOutController(AppDbContext context) => this.context = context;

        //Cart Items Checkout
        // Post: api/Checkout
        async public Task<IActionResult> Post([FromBody] Cart data)
        {
            var resp = new Response { };
            try
            {
                //get the user
                var userId = HttpContext.Session.GetInt32("UserId");

                //check if userId is null
                if (userId == null)
                {
                    return BadRequest(new { success = false, message = "Login Failed" });
                }

                //check cart status existance
                var cartExist = await context.Carts
                                    .FirstOrDefaultAsync(c => c.UserId == userId && c.CartStatus == false);

                if (cartExist != null)
                {
                    //check input data
                    if (data.ShippingAddress != null && data.PaymentMethod != null)
                    {
                        cartExist.PaymentMethod = data.PaymentMethod;
                        cartExist.ShippingAddress = data.ShippingAddress;
                        cartExist.CartStatus = true;
                        await context.SaveChangesAsync();
                        resp.Success = true;
                        resp.Message = "Cart Checked Out successfully";
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Incorrect ShippingAddress/PaymentMethod" });
                    }
                }
                else
                {
                    return BadRequest(new { success = false, message = "Your Cart is Empty" });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "Invalid Request" });
            }
            return Ok(new { success = resp.Success, message = resp.Message });
        }
    }
}