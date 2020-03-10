using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerceCore.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext context;
        public CommentController(AppDbContext context) => this.context = context;

        //GET : api/comment
        [HttpGet("{id}", Name = "GetComment")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            try
            {
                List<Comment> productComment = context.Comments.ToList();
                if (productComment == null)
                {
                    return BadRequest(new { success = false, message = "Comments not found" });
                }
                return Ok(productComment);
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "Comment fetch problem" });
            }
        }

        // POST: api/comment
        [HttpPost]
        public async Task<IActionResult> CommentPost([FromBody] CommentData data)
        {
            var resp = new CommentResponse { Success = true };
            var userID = HttpContext.Session.GetInt32("UserId");

            if (userID == null)
            {
                throw new Exception("Invalid comment authentication");
            }
            //get cart id
            var cartId = await context.Carts.FirstOrDefaultAsync(b => b.CartStatus == true && b.UserId == userID);
            if (cartId == null)
            {
                return BadRequest(new { success = false, message = "User is not purchased the product" });
            }
            Comment comment = new Comment
            {
                UserId = data.UserId,
                ProductId = data.ProductId,
                Comments = data.Comments,
                Rating = data.Rating
            };
            context.Comments.Add(comment);
            await context.SaveChangesAsync();
            return Ok(new { sucess = true, message = "Comment Added Sucessfully" });
        }
    }
    public class CommentData
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Comments { get; set; }
        public float Rating { get; set; }
    }
    public class CommentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}