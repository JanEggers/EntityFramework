using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class Product
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
    }
}
