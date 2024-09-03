using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UdemyRealWorldUnitTest.Web.Controllers;
using UdemyRealWorldUnitTest.Web.Models;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{
    public class ProductControllerTestWithSQLite : ProductControllerBaseTest
    {
        public ProductControllerTestWithSQLite()
        {
            var connection = new SqliteConnection("DataSource=:memory:");

            SetContextOptions(new DbContextOptionsBuilder<UdemyRealWorldUnitTestContext>()
                                .UseSqlite(connection).Options);
        }


        //[Fact]
        public async Task Create_ModelValidProduct_ReturnsRedirectToActionWithSaveProduct()
        {
            var newProduct = new Product { Name = "Kalem 30", Price = 200, Stock = 100, Color = "Yeşil" };

            //eklenecek data için.
            using (var context = new UdemyRealWorldUnitTestContext(_contextOptions))
            {
                var category = context.Categories.First();

                newProduct.CategoryId = category.Id;

                var controller = new ProductsController(context);

                var result = await controller.Create(newProduct);

                var redirect = Assert.IsType<RedirectToActionResult>(result);

                Assert.Equal("Index", redirect.ActionName);
            }

            //eklenen datayı getirmek için.
            using (var context = new UdemyRealWorldUnitTestContext(_contextOptions))
            {
                var product = context.Products.FirstOrDefault(x => x.Name == newProduct.Name);
                Assert.Equal(newProduct.Name, product.Name);
            }
        }

        //[Theory]
        [InlineData(1)]
        public async Task DeleteCategory_ExistCategoryId_DeletedAllProducts(int categoryId)
        {
            using (var context = new UdemyRealWorldUnitTestContext(_contextOptions))
            {
                var category = await context.Categories.FindAsync(categoryId);
                Assert.NotNull(category);

                context.Categories.Remove(category);
                await context.SaveChangesAsync();
            }


            using (var context = new UdemyRealWorldUnitTestContext(_contextOptions))
            {
                var products = await context.Products.Where(x => x.CategoryId == categoryId).ToListAsync();

                Assert.Empty(products);

            }
        }
    }
}
