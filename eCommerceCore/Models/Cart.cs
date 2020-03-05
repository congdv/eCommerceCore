using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceCore.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Boolean CartStatus { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime PurchasedDate { get; set; }
        public string PaymentMethod { get; set; }
        public virtual ICollection<CartDetails> CartDetails { get; set; }
    }
}
