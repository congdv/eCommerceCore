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
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext context;

        public LoginController(AppDbContext context) => this.context = context;

        // POST: api/Login
        [HttpPost]
        public async Task<LoginResponse> Post([FromBody] LoginData data)
        {
            var resp = new LoginResponse { Success = true };

            try
            {
                var user = context.Users.FirstOrDefault(u => u.Email == data.Email);
                
                if(user != null)
                {
                    if(PasswordHash.VerifyHashedPassword(user.Password, data.Password))
                    {
                        await HttpContext.Session.LoadAsync();
                        HttpContext.Session.SetInt32("UserId", user.Id);
                        await HttpContext.Session.CommitAsync();

                        resp.Success = true;
                        resp.Message = "Successfully Login";
                    }
                }
            }catch (Exception exception)
            {
                resp.Success = false;
                resp.Message = exception.Message;
            }
            return resp; 
        }

    }

    public class LoginData
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
