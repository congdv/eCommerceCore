using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceCore.Exceptions;
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
                List<ProductObject> products = new List<ProductObject>();
                if (cartId != null)
                {
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
                }
                return Ok(new { success = true, message = "All products of the current cart", data=products});
            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
        }

        // POST: api/Cart
        [HttpPost]
        async public Task<IActionResult> Post([FromBody] ProductObject data)
        {
            try
            {
                //get the user
                var userId = HttpContext.Session.GetInt32("UserId");

                //check if userId is null
                if (userId == null)
                {
                    throw new BadRequestException("Login Failed");
                }
                
                //get Current CartId
                Cart cartId = await context.Carts
                                    .FirstOrDefaultAsync(cart => cart.CartStatus == false && cart.UserId == userId);

                if (cartId != null)
                {
                    await AddProductToCurrentCart(data, cartId.Id);
                }
                else
                {
                    await AddProductToEmptyCart(data,(int) userId);
                }
            }
            catch (BadRequestException badRequestException)
            {
                return BadRequest(new { success = false, message = badRequestException.Message });
            }
            catch (NotFoundException)
            {
                return NotFound();
            } 

            return Ok(new { success = true, message = "Added new product to current cart successfully" });
        }

        async private Task AddProductToCurrentCart(ProductObject data, int cartId)
        {
            //check for data
            if (CheckUserData(data))
            {
                //check product existance in product table
                var productExist = await context.Products
                            .FirstOrDefaultAsync(p => p.Id == data.ProductId);
                if (productExist != null)
                {
                    // Update quantities if the product is already in current cart
                    if( IsProductInCart(productExist.Id, cartId))
                    {
                        await UpdateQuantities(cartId, productExist.Id, data.Quantities, true);
                    } else
                    {
                        await SaveCartDetailsOfProduct(new CartDetails
                        {
                            ProductId = productExist.Id,
                            CartId = cartId,
                            Quantities = data.Quantities,
                            CurrentPrice = productExist.Pricing
                        });
                    }
                    
                }
                else
                {
                    throw new BadRequestException("The product is not existed");
                }
            }
            else
            {
                throw new NotFoundException();
            }
        }

        private bool IsProductInCart(int productId, int cartId)
        {
            var product = context.CartsDetails.FirstOrDefault(cartDetail => cartDetail.ProductId == productId && cartDetail.CartId == cartId);
            return product != null;
        }

        async private Task AddProductToEmptyCart(ProductObject data, int userId)
        {
            //check product existance in product table
            var productExist = await context.Products
                        .FirstOrDefaultAsync(p => p.Id == data.ProductId);

            if (productExist != null)
            {
                //create new cartId for the User
                var userCart = new Cart()
                {
                    UserId = userId,
                    CartStatus = false,
                    ShippingAddress = null,
                    PaymentMethod = null,
                    PurchasedDate = DateTime.Now
                };
                await context.Carts.AddAsync(userCart);
                //add cartdetails with the created cartId as a foreign key
                await SaveCartDetailsOfProduct(new CartDetails() { Cart = userCart, Quantities = data.Quantities, ProductId = productExist.Id, CurrentPrice = productExist.Pricing });
            }
            else
            {
                throw new BadRequestException("The product is not existed");
            }
        }

        async private Task SaveCartDetailsOfProduct(CartDetails cartDetails)
        {
            await context.CartsDetails.AddAsync(cartDetails);
            context.SaveChanges();
        }

        private bool CheckUserData(ProductObject data)
        {
            if (data.ProductId > 0 && data.Quantities > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        async public Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var cartId = await context.Carts.FirstOrDefaultAsync(b => b.CartStatus == false && b.UserId == userId);
            if (userId == null)
            {
                return BadRequest(new { success = false, message = "Login Failed" });
            }
            else
            {
                if (cartId != null)
                {
                    var productExist = await context.CartsDetails.FirstOrDefaultAsync(cd => cd.ProductId == id && cd.CartId == cartId.Id);
                    if (productExist != null)
                    {
                       
                        context.CartsDetails.Remove(productExist);
                        await context.SaveChangesAsync();

                        return Ok(new { success = true, message = "The product is removed successfully" });
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
        }

        private bool IsCartEmpty(Cart cart)
        {
            var cr = context.CartsDetails.FirstOrDefaultAsync(cd => cd.CartId == cart.Id);
            return cr == null;
        }


        [HttpPut("")]
        async public Task<IActionResult> UpdateQuantitiesOfProduct([FromBody] UpdateQuantity data)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            try
            {
                //check if userId is null
                if (userId == null)
                {
                    throw new BadRequestException("Login Failed");
                }

                //get Current CartId
                Cart currentCart = await context.Carts
                                    .FirstOrDefaultAsync(cart => cart.CartStatus == false && cart.UserId == userId);
                if(currentCart != null)
                {
                    await UpdateQuantities(currentCart.Id, data.ProductId, data.Quantities);

                } else
                {
                    throw new NotFoundException("The cart is empty");
                }

            } catch (NotFoundException notFoundException)
            {
                return NotFound(new { success = false, message = notFoundException.Message }); ;
            } catch (BadRequestException badRequestException)
            {
                return BadRequest(new { success = false, message = badRequestException.Message });
            }
            
            return Ok(new { success=true, message="Successfully update quantities of product"});
        }

        async private Task UpdateQuantities(int cartId, int productId, int quantities, bool isAppend = false )
        {
            var toUpdateProduct = await context.CartsDetails.FirstOrDefaultAsync(product => product.CartId == cartId && product.ProductId == productId);
            if(toUpdateProduct != null)
            {
                if(isAppend)
                {
                    toUpdateProduct.Quantities += quantities;
                } else
                {
                    toUpdateProduct.Quantities = quantities;
                }
                await context.SaveChangesAsync();

            }
            else
            {
                throw new NotFoundException("The product is not in current cart");
            }
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

    public class UpdateQuantity
    {
        public int ProductId { get; set; }
        public int Quantities { get; set; }
    }
}
