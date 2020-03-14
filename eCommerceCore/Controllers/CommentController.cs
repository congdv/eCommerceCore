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
                var product = context.Products.FirstOrDefault(prod => prod.Id == id);
                if (product!=null)
                {
                    //LINQ query here for specific product comment
                    //List<Comment> comments = context.Comments.ToList();
                    var comments = (from comment in context.Comments
                                join user in context.Users on comment.UserId equals user.Id
                                where comment.ProductId == id
                                select new
                                {
                                    Id = comment.Id,
                                    UserName = user.Username,
                                    ProductId = id,
                                    Comment = comment.Content,
                                    ImagePaths = comment.ImagePaths.Select(path => path.Path),
                                    Rating = comment.Rating
                                }).ToList();
                    return Ok( new { success = true, message = "All comments", data = comments });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Comments are not availble for this product" });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "Comment could not be fetched at this time" });
            }
        }

        // POST: api/comment
        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] CommentData data)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            
            if (userId == null)
            {
                return BadRequest(new { success = false, message = "User should login first" });
            }

            //get cart id
            List<int> verifyPurchase = (from cart in context.Carts
                                        join cartDetail in context.CartsDetails on cart.Id equals cartDetail.CartId
                                          where cart.UserId == userId
                                          && cart.CartStatus == true
                                          && cartDetail.ProductId == data.ProductId
                                        select cartDetail.ProductId).Distinct().ToList();
 
            if (verifyPurchase.Count() > 0)
            {
                Comment comment = new Comment
                {
                    UserId = (int)userId,
                    ProductId = data.ProductId,
                    Content = data.Comment,
                    Rating = data.Rating
                };
                context.Comments.Add(comment);
                await context.SaveChangesAsync();
                if(data.ImagePaths != null)
                {
                    foreach (string path in data.ImagePaths)
                    {
                        ImagePath imagePath = new ImagePath()
                        {
                            Path = path,
                            Comment = comment
                        };
                        context.ImagePaths.Add(imagePath);
                    }
                    await context.SaveChangesAsync();
                }
                
                return Ok(new { sucess = true, message = "Comment added sucessfully" });
            }
            else
            {
                return BadRequest(new { success = false, message = "User is not authorized to comment about this product" });
            }
        }
    }
    public class CommentData
    {
        public int ProductId { get; set; }
        public string Comment { get; set; }
        public List<string> ImagePaths { get; set; }
        public float Rating { get; set; }
    }
    public class CommentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}