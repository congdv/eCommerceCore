using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceCore.Exceptions;
using eCommerceCore.Library;
using eCommerceCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext context;

        public LoginController(AppDbContext context) => this.context = context;

        // POST: api/Login
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User data)
        {
            try
            {
                var user = context.Users.FirstOrDefault(u => u.Username == data.Username);
                
                if(user != null)
                {
                    if(PasswordHash.VerifyHashedPassword(user.Password, data.Password))
                    {
                        await HttpContext.Session.LoadAsync();
                        HttpContext.Session.SetInt32("UserId", user.Id);
                        await HttpContext.Session.CommitAsync();
                    } else
                    {
                        throw new BadRequestException("Invalid Username/Password");
                    }
                } else
                {
                    throw new BadRequestException("Invalid Username/Password");
                }
            }catch (BadRequestException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
            return Ok( new { succes =  true, message = "Successfully logged in" }); 
        }

    }
}
