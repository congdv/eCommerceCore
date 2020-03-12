﻿using System;
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
                var userId = HttpContext.Session.GetInt32("UserId");
                if (product!=null)
                {
                    //LINQ query here for specific product comment
                    //List<Comment> comments = context.Comments.ToList();
                    List<Comment> comments;
                    comments = (from c in context.Comments
                                join u in context.Users on c.UserId equals userId
                                where c.ProductId == id
                                select new Comment
                                {
                                    Id = c.Id,
                                    UserId = c.UserId,
                                    ProductId = id,
                                    Comments = c.Comments,
                                    CommentImagePath = c.CommentImagePath,
                                    Rating = c.Rating
                                }).ToList();
                    return Ok(comments);
                }
                else
                {
                    return BadRequest(new { success = false, message = "Product was not found" });
                }
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
            var userId = HttpContext.Session.GetInt32("UserId");
            //get cart id
            var cartId = await context.Carts.FirstOrDefaultAsync(b => b.CartStatus == true && b.UserId == userId);
            
            if (cartId == null)
            {
                return BadRequest(new { success = false, message = "User did not purchased the product" });
            }
            var productExist =  (from c in context.Carts
                                      join cd in context.CartsDetails on c.Id equals cartId.Id
                                      where c.UserId == userId 
                                      select cd.ProductId).Distinct().ToList();

            if (productExist != null)
            {
                Comment comment = new Comment
                {
                    UserId = (int)userId,
                    ProductId = data.ProductId,
                    Comments = data.Comments,
                    CommentImagePath = data.CommentImagePath,
                    Rating = data.Rating
                };
                context.Comments.Add(comment);
                await context.SaveChangesAsync();
                return Ok(new { sucess = true, message = "Comment Added Sucessfully" });
            }

            else
            {
                return BadRequest(new { success = false, message = "Product not associated to User" });
            }
        }
    }
    public class CommentData
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Comments { get; set; }
        public string CommentImagePath { get; set; }
        public float Rating { get; set; }
    }
    public class CommentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}