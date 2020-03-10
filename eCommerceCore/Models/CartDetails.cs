using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceCore.Models
{
    public class CartDetails
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Quantities { get; set; }
        public int CurrentPrice { get; set; }
        public int CartId { get; set; }
    }
}
