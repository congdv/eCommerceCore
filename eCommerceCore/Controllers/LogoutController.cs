using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        // GET: api/Logout
        [HttpGet]
        public IActionResult Get()
        {
            HttpContext.Session.Remove("UserId");
            return Ok(new { success = true, message="Successfully Logged Out" }); 
        }

       
    }
}
