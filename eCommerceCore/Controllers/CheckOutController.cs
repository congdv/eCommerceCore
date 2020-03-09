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
        async public Task<Response> Post([FromBody] Cart data)
        {
            var resp = new Response { };
            try
            {
                //get the user
                var userId = HttpContext.Session.GetInt32("UserId");

                //check if userId is null
                if (userId == null)
                {
                    throw new Exception("Login Failed");
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
                        resp.Message = "Cart Status Updated successfully";
                    }
                    else
                    {
                        throw new Exception("Data Missing");
                    }
                }
                else
                {
                    resp.Success = false;
                    resp.Message = "Your Cart is Empty";
                }
            }
            catch (Exception exception)
            {
                resp.Success = false;
                resp.Message = exception.Message;
            }
            return resp;
        }
    }
}