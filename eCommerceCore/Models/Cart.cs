using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceCore.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public Boolean CartStatus { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime PurchasedDate { get; set; }
        public string PaymentMethod { get; set; }
        public virtual ICollection<CartDetails> CartDetails { get; set; }
    }
}
