using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceCore.Library;
using eCommerceCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext context;

        public UserController(AppDbContext context) => this.context = context;
        // GET: api/User
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "GetUser")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/User
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        /**
         * Change password API
         */
        [HttpPut("")]
        public async Task<IActionResult> Put([FromBody] PasswordData data)
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            if(userID == null)
            {
                return BadRequest(new { success = false, message = "Invalid Authentication" });
            }
            var user = context.Users.FirstOrDefault(u => u.Id == userID);
            if(PasswordHash.VerifyHashedPassword(user.Password, data.OldPassword))
            {
                user.Password = PasswordHash.HashPassword(data.NewPassword);
                await context.SaveChangesAsync();
            } else
            {
                return BadRequest(new { success = false, message = "Wrong Password" });
            }
            

            return Ok( new { sucess = true, message = "Succesfully change your password"});
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
    public class PasswordData {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
