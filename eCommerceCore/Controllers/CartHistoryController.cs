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
    [Route("api/history")]
    [ApiController]
    public class CartHistoryController : Controller
    {
        private readonly AppDbContext context;
        public CartHistoryController(AppDbContext context) => this.context = context;

        // GET: api/Cart
        [HttpGet]
        async public Task<IActionResult> Get()
        {
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
                List<Cart> carts = await context.Carts.Where(cart => cart.CartStatus == true && cart.UserId == userId).ToListAsync();
                List<ResponseCart> responseCarts = carts.Select(cart => new ResponseCart(cart)).ToList();

                foreach (ResponseCart cart in responseCarts)
                {
                    cart.Products = await GetAllProductsFromCart(cart.Id);
                }
                return Ok(new { success = true, message = "Purchasing History", data = responseCarts });
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "Invalid Request" });
            }
        }

        private List<ResponseCart> ConvertCartToResponseCart(List<Cart> carts)
        {
            List<ResponseCart> responseCarts = new List<ResponseCart>();

            foreach(Cart cart in carts)
            {
                responseCarts.Add(new ResponseCart(cart));
            }

            carts.ForEach(cart => new ResponseCart(cart));
            return responseCarts;
        }

        async private  Task<List<ResponseProduct>> GetAllProductsFromCart(int cartId)
        {
            List<ResponseProduct> products = await (
                from cartDetail in context.CartsDetails
                join product in context.Products on cartDetail.ProductId equals product.Id
                where cartDetail.CartId == cartId
                select new ResponseProduct
                {
                    Id = product.Id,
                    Description = product.Description,
                    ImagePath = product.ImagePath,
                    //get current price from CartDetails
                    Pricing = cartDetail.CurrentPrice,
                    ShippingCost = product.ShippingCost,
                    Name = product.ProductName,
                    ProductId = product.Id,
                    Quantities = cartDetail.Quantities
                }).ToListAsync();
                
            return products;
        }
    }


    class ResponseCart
    {
        private Cart cart;
        public ResponseCart(Cart cart)
        {
            this.cart = cart;
        }

        public int Id
        {
            get => this.cart.Id;
            set => this.cart.Id = value;
        }

        public string ShippingAddres
        {
            get => this.cart.ShippingAddress;
            set => this.ShippingAddres = value;
        }

        public DateTime PurchaseDate
        {
            get => this.cart.PurchasedDate;
            set => this.cart.PurchasedDate = value;
        }
        public string PaymentMethod
        {
            get => this.cart.PaymentMethod;
            set => this.cart.PaymentMethod = value;
        }

        public List<ResponseProduct> Products
        {
            get; set;
        }
    }

    class ResponseProduct
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
}