using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var services = new ServiceCollection()
                .AddDbContext<TestDbContext>(o => o.UseCsvDatabase("lalaa"))
                .BuildServiceProvider())
            {
                var ctx = services.GetRequiredService<TestDbContext>();
                
                var products = ctx.Products.ToList();
                
                ctx.Products.Add(new Product() { Name = "xxx" });

                ctx.SaveChanges();

                products = ctx.Products.ToList();
            }
        }
    }
}
