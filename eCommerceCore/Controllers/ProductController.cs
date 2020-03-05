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

        //List<Product> productList = new List<Product>();


        // GET: api/product
        [HttpGet]
        public ActionResult<List<Product>> Get()
        {
            List<Product> products = context.Products.ToList();
            return products;
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public ActionResult<Product> Get(int id)
        {
            int productId =  id;
            id.GetType();
            var prod = context.Products.FindAsync(from p in
                                                        context.Products
                                                        where p.Id == productId
                                                       select p);
            return (Product)prod;
        }

        /*
        // GET: api/Product
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Product/5
        //[HttpGet("{id}", Name = "GetProducts")]
        //public string Get(int id)
        //{
        //    return "value";
        //}



            // POST: api/Product
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}
