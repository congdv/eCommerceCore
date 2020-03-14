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
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext context;

        public RegisterController(AppDbContext context) => this.context = context;

        // POST: api/Register
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            try
            {
                if(!context.Users.Any(u => u.Email == user.Email || u.Username == user.Username))
                {
                    var emailValidator = new EmailAddressAttribute();
                    
                    if (emailValidator.IsValid(user.Email) &&
                        !string.IsNullOrEmpty(user.Password) &&
                        !string.IsNullOrEmpty(user.Username))
                    {
                        if(user.Password.Length < 8 )
                        {
                            throw new Exception("The length of password should be at least 8 characters");
                        }
                        user.Password = PasswordHash.HashPassword(user.Password);
                        await context.Users.AddAsync(user);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("Email, Password, and Username are required");
                    }
                }
                else
                {
                    throw new Exception("Email/Username is already exists");
                }
            }catch (Exception exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }

            return Ok(new { success= true, message= "Successfully Registered" });
        }

    }
}
