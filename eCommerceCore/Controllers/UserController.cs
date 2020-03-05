using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /*// GET: api/User
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
        */
        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UpdateData data)
        {
            var resp = new UpdateResponse { Success = true };
            var userID = HttpContext.Session.GetInt32("UserId");
            if (userID == null)
            {
                throw new Exception("Invalid authentication");
            }
            else
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userID);
                if (user != null)
                {
                    user.FirstName = data.FirstName;
                    user.LastName = data.LastName;
                    user.Password = PasswordHash.HashPassword(data.Password);
                    user.ShippingAddress = data.ShippingAddress;
                    user.Email = data.Email;
                    if (user.FirstName != "" && user.FirstName != "" && user.Password != "" && user.ShippingAddress != "" && user.Email != "")
                    {
                        await context.SaveChangesAsync();
                        return Ok(new { sucess = true, message = "Data Succesfully Changed" });
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Data is Missing" });
                    }
                }
                else
                {
                    return BadRequest(new { success = false, message = "Wrong Inputs for Update data" });
                }
            }
        }

        /**
         * Change password API
         */
        [HttpPut("")]
        public async Task<IActionResult> Put([FromBody] PasswordData data)
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            if (userID == null)
            {
                return BadRequest(new { success = false, message = "Invalid Authentication" });
            }
            else
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userID);
                if (PasswordHash.VerifyHashedPassword(user.Password, data.OldPassword))
                {
                    user.Password = PasswordHash.HashPassword(data.NewPassword);
                    await context.SaveChangesAsync();
                }
                else
                {
                    return BadRequest(new { success = false, message = "Wrong Password" });
                }
                return Ok(new { sucess = true, message = "Succesfully change your password" });
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
    public class UpdateData
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShippingAddress { get; set; }
    }
    public class UpdateResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class PasswordData {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
