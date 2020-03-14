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

        // Get user profile
       [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            if (userID == null)
            {
                return BadRequest(new { success = false, message = "Invalid authentication" });
            }
            var user = context.Users.FirstOrDefault(u => u.Id == userID);
            var responseUser = new { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, ShippingAddress = user.ShippingAddress };
            return Ok(new { success = true, message = "User's Profile", data = responseUser });
        }
        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UpdateData data)
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            if (userID == null)
            {
                return BadRequest( new { success = false, message = "Invalid authentication" });
            }

            var user = context.Users.FirstOrDefault(u => u.Id == userID);

            // Requiring enter password when the user tries to update their profile
            if(string.IsNullOrEmpty(data.Password) || !PasswordHash.VerifyHashedPassword(user.Password, data.Password))
            {
                return BadRequest(new { success = false, message = "Invalid password" });
            }
            
            if (user != null)
            {
                user.FirstName = data.FirstName;
                user.LastName = data.LastName;
                user.ShippingAddress = data.ShippingAddress;
                user.Email = data.Email;
                if (user.FirstName != "" && user.LastName != "" && user.ShippingAddress != "" && user.Email != "")
                {
                    await context.SaveChangesAsync();
                    return Ok(new { sucess = true, message = "Successfully updated user" });
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
