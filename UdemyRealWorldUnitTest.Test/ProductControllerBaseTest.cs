using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.Web.Models;

//Ef Core InMemory - Base Class
namespace UdemyRealWorldUnitTest.Test
{
    public class ProductControllerBaseTest
    {
        protected DbContextOptions<UdemyRealWorldUnitTestContext> _contextOptions { get; private set; }

        public void SetContextOptions(DbContextOptions<UdemyRealWorldUnitTestContext> contextOptions) 
        {
            _contextOptions = contextOptions;
            Seed();
        }


        public void Seed()
        {
            using (UdemyRealWorldUnitTestContext context = new UdemyRealWorldUnitTestContext(_contextOptions))
            {
                context.Database.EnsureDeleted();// ayağa kalkarken sil.
                context.Database.EnsureCreated();//ayağa kalkarken oluştur.


                context.Categories.Add(new Category() { Name = "Kalemler"});
                context.Categories.Add(new Category() { Name = "Defterler"});
                context.SaveChanges();

                context.Products.Add(new Product() { CategoryId = 1, Name = "Kalem 10", Price=100, Stock=100, Color="Kırmızı"});
                context.Products.Add(new Product() { CategoryId = 1, Name = "Kalem 20", Price=100, Stock=100, Color="Mavi"});
                context.SaveChanges();


            }
        }


    }
}
