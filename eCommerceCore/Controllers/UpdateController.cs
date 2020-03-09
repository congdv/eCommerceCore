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
    public class UpdateController : ControllerBase
    {
        private readonly AppDbContext context;

        public UpdateController(AppDbContext context) => this.context = context;

        // POST: api/Update
        //[HttpPost]
        /*public async Task<UpdateResponse> Post([FromBody] UpdateData data)
        {
            var resp = new UpdateResponse { Success = true };

            try
            {
                var userID = HttpContext.Session.GetInt32("UserId");

                if(userID == null)
                {
                    throw new Exception("Invalid authentication");
                }
                var user = context.Users.FirstOrDefault(u => u.Id == userID);
                if (user != null)
                {
                    var emailValidator = new EmailAddressAttribute();

                    if (emailValidator.IsValid(user.Email) &&
                        !string.IsNullOrEmpty(user.Password) &&
                        !string.IsNullOrEmpty(user.FirstName) &&
                        !string.IsNullOrEmpty(user.LastName) &&
                        !string.IsNullOrEmpty(user.ShippingAddress))
                    {
                        UpdateData updateData = new UpdateData
                        {
                            FirstName = data.FirstName,
                            Email = data.Email,
                            LastName = data.LastName,
                            Password = PasswordHash.HashPassword(data.Password),
                            ShippingAddress = data.ShippingAddress
                        };
                        resp.Success = true;
                        resp.Message = "Data updated Successfully";
                    }
                }
            }           
            catch (Exception exception)
            {
                resp.Message = exception.Message;
                resp.Success = false;
            }
            return resp;
        }
    }*/
    }
   /* public class UpdateData
    {
        public string UserName { get; }
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
    }*/
}
