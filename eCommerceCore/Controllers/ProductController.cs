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
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext context;
        public ProductController(AppDbContext context) => this.context = context;

        // GET: api/product
        [HttpGet]
        public ActionResult<List<Product>> Get()
        {
            try
            {
                List<Product> product = context.Products.ToList();
                return product;
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "Invalid Request" });
            }
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id.GetType() == typeof(System.String))
            {
                return BadRequest(new { success = false, message = "Invalid Request" });
            }

            try
            {
                int productId = id;
                Product product = await context.Products.FirstOrDefaultAsync(prod => prod.Id == id);
                
                if (product == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(new { success=true, message="Detail Product", data=product });
                }

            }
            catch (Exception exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
        }

        
        // POST: api/Product/
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {

            try
            {
                if (!string.IsNullOrEmpty(product.Description) &&
                    !string.IsNullOrEmpty(product.ImagePath) &&
                    !string.IsNullOrEmpty(product.ProductName) &&
                    product.Pricing != 0)
                {
                    await context.Products.AddAsync(product);
                    await context.SaveChangesAsync();
                } else
                {
                    throw new Exception("All product specs not provided");
                }
            }

            catch (Exception exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }

            return Ok(new { success = true, message = "Successfully added new product" });

        }

    }

}