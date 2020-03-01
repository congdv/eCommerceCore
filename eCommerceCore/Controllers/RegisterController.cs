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
        public async Task<RegisterResponse> Post([FromBody] User user)
        {
            var resp = new RegisterResponse { Success = false };

            try
            {
                if(!context.Users.Any(u => u.Email == user.Email))
                {
                    var emailValidator = new EmailAddressAttribute();

                    if(emailValidator.IsValid(user.Email) &&
                        !string.IsNullOrEmpty(user.Password) &&
                        !string.IsNullOrEmpty(user.Username))
                    {
                        user.Password = PasswordHash.HashPassword(user.Password);
                        await context.Users.AddAsync(user);
                        await context.SaveChangesAsync();
                        resp.Success = true;
                        resp.Message = "Successfully Registered";
                    }
                    else
                    {
                        resp.Message = "String cannot be empty";
                    }
                }
                else
                {
                    resp.Message = "An Account with this email already exists";
                }
            }catch
            {
                resp.Message = "An internal error occured. Please try again.";
            }

            return resp;
        }

        // PUT: api/Register/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
