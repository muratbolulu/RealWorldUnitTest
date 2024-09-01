using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.Web.Controllers;
using UdemyRealWorldUnitTest.Web.Helpers;
using UdemyRealWorldUnitTest.Web.Models;
using UdemyRealWorldUnitTest.Web.Repository;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{
    public class ProductApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        //ProductApiController test edileceğinden dolayı bir nesne örneği oluşturmak için kullanılır.
        private readonly ProductsAPIController _productsController;

        private List<Product> _products;
        private readonly Helper _helper;

        public ProductApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _productsController = new ProductsAPIController(_mockRepo.Object);
            _helper= new Helper();
            _products = new List<Product>() //fake products
            {
                new Product() {Id=1,Name="kalem",Color="Red",Price=100,Stock=15 },
                new Product() {Id=2,Name="kalem2",Color="Red2",Price=200,Stock=25 }
            };
        }


        //business metod test etme örneği
        //(dotnet test diyerek power shell üzerinden test de edilebilir.)
        [Theory]
        [InlineData(4,5,9)]
        public void Add_SampleValues_ReturnTotal(int a, int b, int total)
        {
            var result = _helper.Add(a,b);

            Assert.Equal(total, result);
        }

        //[Fact]
        public async void GetProduct_ActionExecutes_ReturnOkResultWithProducts()
        {
            //sahte repo kuruldu.
            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(_products);
            //products çekildi.
            var result = await _productsController.GetProducts();
            //result dönüş tipi test edilir.(Ok, NoContent, BadRequest .. )
            var okResult = Assert.IsType<OkObjectResult>(result);
            //result içerisindeki ürün liste şeklinde bir product mıdır kontrolü
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            //Gelen data sayısı karşılaştırıldı.
            Assert.Equal<int>(2, returnProducts.ToList().Count);
        }

        //[Theory]
        [InlineData(0)]
        public async void GetProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(repo=>repo.GetById(productId)).ReturnsAsync(product);

            var result = _productsController.GetProduct(productId);

            //NotFoundObjectResult controller return içerisinde obje olursa dönülür.
            //NotFoundResult controller return içerisinde data olmadığından kullanıldı.
            Assert.IsType<NotFoundResult>(result);
        }

        //[Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetProduct_InValid_ReturnOkResult(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            //fake product repository den geliyormuş gibi gönderilir.
            _mockRepo.Setup(repo=>repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productsController.GetProduct(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(productId, returnProduct.Id);
            Assert.Equal(product.Name,returnProduct.Name);
        }


        //[Theory]
        [InlineData(7)]
        public void PutProduct_IdIsNotEqual_ReturnBadRequestResult(int productId)
        {
            var product = _products.First();

            var result = _productsController.PutProduct(productId, product);

            //badRequestResult üzerinden status code da kontrol edilebilir.
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        //[Theory]
        [InlineData(1)]
        public void PutProduct_ActionExecutes_ReturnNoContent(int productId)
        {
            var product = _products.First();

            _mockRepo.Setup(repo=>repo.Update(product));

            var result = _productsController.PutProduct(productId, product);

            //En az bir kere bu metodun çalıştığını kontrol eder.
            _mockRepo.Verify(x => x.Update(product), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }

        //[Fact]
        public async void PostProduct_ActionExecutes_ReturnCreateAtAction()
        {

            var product = _products.First();

            _mockRepo.Setup(repo => repo.Create(product)).Returns(Task.CompletedTask);

            var result = await _productsController.PostProduct(product);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

            _mockRepo.Verify(x=>x.Create(product), Times.Once);

            Assert.Equal("GetProduct", createdAtActionResult.ActionName);
        }

        //[Theory]
        [InlineData(1)]
        public async void DeleteProduct_IdInValid_ReturnNotFound(int productId) 
        {
            Product product = null;
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var resultNotFound = await _productsController.DeleteProduct(productId);

            //dönen obje (ActionResult) olduğundan Result alınır. objeyi göstermek lazım olduğundan.
            //IActionResult interface ini dönseydi Result a gerek olmazdı.
            Assert.IsType<NotFoundResult>(resultNotFound.Result);
        }

        //[Theory]
        [InlineData(1)]
        public async void DeleteProduct_ActionExecutes_ReturnNoContent(int productId)
        {
            Product product = _products.First();

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            _mockRepo.Setup(repo => repo.Delete(product));

            var noContentResult = await _productsController.DeleteProduct(productId);

            _mockRepo.Verify(x=>x.Delete(product),Times.Once);

            Assert.IsType<NoContentResult>(noContentResult.Result);

        }
    }
}
