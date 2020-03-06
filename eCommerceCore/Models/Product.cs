using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceCore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public double Pricing { get; set; }
        public string ProductName { get; set; }
        public double ShippingCost { get; set; }

        public static explicit operator Product(Task<Product> v)
        {
            throw new NotImplementedException();
        }
    }
}
