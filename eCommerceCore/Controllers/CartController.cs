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
        async public Task<Response> Get()
        {
            var resp = new Response { };
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                //get cartId of User's cart
                var cartId = await context.Carts
                                    .FirstOrDefaultAsync(b => b.CartStatus == true && b.UserId == userId);

                //get products for that User 
                await (from c in context.Carts
                       join p in context.Products on c.Id equals p.Id
                       where c.Id == cartId.Id
                       select new ProductObject
                       {
                           Id = p.Id,
                           Description = p.Description,
                           ImagePath = p.ImagePath,
                           Pricing = p.Pricing,
                           ShippingCost = p.ShippingCost,
                           Name = p.ProductName
                       }).ToListAsync();

                resp.Data = null;
            }
            catch (Exception exception)
            {
                resp.Success = false;
                resp.Message = exception.Message;
            }
            return resp;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Cart
        [HttpPost]
        async public Task<Response> Post([FromBody] ProductObject data)
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
                
                //get Current CartId
                var cartId = await context.Carts
                                    .FirstOrDefaultAsync(b => b.CartStatus == false && b.UserId == userId);

                if (cartId != null)
                {
                    //check product existance in product table
                    var productExist = await context.Products
                                .FirstOrDefaultAsync(p => p.Id == data.ProductId);
                    if (productExist != null)
                    {
                        CartDetails cartDetails = new CartDetails
                        {
                            //id = null,
                            ProductId = productExist.Id,
                            CartId = cartId.Id,
                            Quantities = data.Quantities
                            //price
                        };
                        await context.CartsDetails.AddAsync(cartDetails);
                        context.SaveChanges();
                        resp.Success = true;
                        resp.Message = "Cart Found and Product Saved successfully";
                    }
                    else
                    {
                        resp.Success = false;
                        resp.Message = "Product Does't Exist";
                    }
                }
                else
                {
                    //List<CartDetails> cartDetails = new List<CartDetails>();
                    //cartDetails.Add(new CartDetails { ProductId = 1, CurrentPrice = 20, Quantities = 10 });
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
                    await context.CartsDetails.AddAsync(new CartDetails() { Cart = userCart, Quantities = data.Quantities, ProductId = 1 });
                    await context.SaveChangesAsync();
                    resp.Success = true;
                    resp.Message = "Cart created successfully";


                    //context.CartsDetails.Add(new CartDetails { CurrentPrice = 20, Quantities = 10});
                    //context.SaveChanges();

                   
                }
            }
            catch (Exception exception)
            {
                resp.Success = false;
                resp.Message = exception.Message;
            }
            return resp;
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
