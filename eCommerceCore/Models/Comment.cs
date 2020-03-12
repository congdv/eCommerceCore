using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceCore.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Comments { get; set; }
        public string CommentImagePath { get; set; }
        public float Rating { get; set; }
    }
}
