using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceCore.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            foreach (var rel in mb.Model.GetEntityTypes().SelectMany(f => f.GetForeignKeys()))
                rel.DeleteBehavior = DeleteBehavior.Restrict;

            foreach (var prop in mb.Model.GetEntityTypes()
                                        .SelectMany(t => t.GetProperties())
                                        .Where(q => q.ClrType == typeof(decimal)))
                prop.Relational().ColumnType = "decimal(18,2)";

            base.OnModelCreating(mb);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Cart> Carts { get; set; }
        
        public DbSet<Product>  Products { get; set; }
    }
}
