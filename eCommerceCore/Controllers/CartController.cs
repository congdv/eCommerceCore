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

                //get cartId of User's cart
                var cartId = await context.Carts
                                    .FirstOrDefaultAsync(b => b.CartStatus == false && b.UserId == userId);

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
                                   Pricing = p.Pricing,
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
            return Ok(new { success = resp.Success , data = resp.Data });
        }

        // POST: api/Cart
        [HttpPost]
        async public Task<IActionResult> Post([FromBody] ProductObject data)
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
                
                //get Current CartId
                var cartId = await context.Carts
                                    .FirstOrDefaultAsync(b => b.CartStatus == false && b.UserId == userId);

                if (cartId != null)
                {
                    //check for data
                    if (data.ProductId > 0 && data.Quantities > 0)
                    {
                        //check product existance in product table
                        var productExist = await context.Products
                                    .FirstOrDefaultAsync(p => p.Id == data.ProductId);
                        if (productExist != null)
                        {
                            CartDetails cartDetails = new CartDetails
                            {
                                ProductId = productExist.Id,
                                CartId = cartId.Id,
                                Quantities = data.Quantities,
                                CurrentPrice = productExist.Pricing
                            };
                            await context.CartsDetails.AddAsync(cartDetails);
                            context.SaveChanges();
                            resp.Success = true;
                            resp.Message = "Cart Found and Product Saved successfully";
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Product Does't Exist" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Incorrect ProductId/Quantities" });
                    }
                }
                else
                {
                    try
                    {
                        //check product existance in product table
                        var productExist = await context.Products
                                    .FirstOrDefaultAsync(p => p.Id == data.ProductId);

                        if (productExist != null)
                        {
                            //create new cartId for the User
                            var userCart = new Cart()
                            {
                                UserId = (int)userId,
                                CartStatus = false,
                                ShippingAddress = null,
                                PaymentMethod = null,
                                PurchasedDate = DateTime.Now
                            };
                            await context.Carts.AddAsync(userCart);
                            //add cartdetails with the created cartId as a foreign key
                            await context.CartsDetails.AddAsync(new CartDetails() { Cart = userCart, Quantities = data.Quantities, ProductId = productExist.Id, CurrentPrice = productExist.Pricing });
                            await context.SaveChangesAsync();

                            resp.Success = true;
                            resp.Message = "Cart created successfully";
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Product Does't Exist" });
                        }
                    }
                    catch (Exception exception)
                    {
                        return BadRequest(new { success = false, message = exception.Message });
                    }               
                }
            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
            return Ok(new { success = resp.Success , message = resp.Message });
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
        public string ImagePath { get; set; }
        public double Pricing { get; set; }
        public double ShippingCost { get; set; }
        public string Name { get; set; }
        public int Quantities { get; set; }
        public int ProductId { get; set; }
    }

    public class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public List<ProductObject> Data { get; set; }
    }
}
