using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceCore.Controllers
{
    [Route("api/Product")]
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
        [HttpGet("{id}", Name = "GetProduct")]
        public ActionResult<Product> Get(int id)
        {
            try
            {
                int productId = id;
                var product = context.Products.FirstOrDefault(prod => prod.Id == id);

                if (id.GetType() == typeof(System.String))
                {
                    return BadRequest(new { success = false, message = "Invalid Request" });
                }
                else if (product == null)
                {
                    return Ok(new { success = false, message = "Product Not Found!" });
                }
                else
                {
                    return (Product)product;
                }
                
            }
            catch(Exception)
            {
                return BadRequest(new { success = false, message = "Invalid Request" });
            }
        }
    }
}
