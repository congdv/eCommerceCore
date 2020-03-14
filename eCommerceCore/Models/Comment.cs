using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceCore.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; }
        public float Rating { get; set; }

        public virtual ICollection<ImagePath> ImagePaths { get; set; }
    }
}
