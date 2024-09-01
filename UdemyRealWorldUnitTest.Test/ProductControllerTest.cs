using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyRealWorldUnitTest.Web.Controllers;
using UdemyRealWorldUnitTest.Web.Models;
using UdemyRealWorldUnitTest.Web.Repository;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _productsController;

        private List<Product> _products;

        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _productsController = new ProductsController(_mockRepo.Object);

            _products = new List<Product>() //fake products
            {
                new Product() {Id=1,Name="kalem",Color="Red",Price=100,Stock=15 },
                new Product() {Id=2,Name="kalem2",Color="Red2",Price=200,Stock=25 }
            };
        }

        //[Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _productsController.Index(); //result == view result.
            Assert.IsType<ViewResult>(result);
        }

        //[Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_products);

            var result = await _productsController.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(2, productList.Count());
        }

        //[Fact]
        public async void Details_IdNull_ReturnRedirectToIndexAction()
        {
            var result = await _productsController.Details(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        //[Fact]
        public async void Details_IdInValid_ReturnNotFound()
        {
            Product product = null;
            //id sini 0 verip null bir product oluşturup return ettik.
            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);

            var result = await _productsController.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        //[Theory]
        [InlineData(1)]
        public async void Details_IdValid_ReturnProduct(int productId)
        {
            Product product = _products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productsController.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            //gelen product bize gelen product mi kontrolü
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        //[Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _productsController.Create();
            Assert.IsType<ViewResult>(result);
        }

        //[Fact]
        public async void CreatePOST_InValidModelState_ReturnView()
        {
            _productsController.ModelState.AddModelError("Name", "Name alanı gereklidir.");

            var result = await _productsController.Create(_products.First());

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        //[Fact]
        public async void CreatePOST_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _productsController.Create(_products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        //[Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;

            //It.IsAny ile herhangi bir nesne geleceğini söyleriz. Gelen nesneyi newProduct nesnesine aktarırız.
            _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);
            //ilk product ürününü verir.
            var result = await _productsController.Create(_products.First());
            //Create metodunun en az 1 kez çalışıp çalışmadığını doğrular.
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
            //ilk ürünün id'si ile eklenen ürünün id'si aynı mıdır kontrolüdür.
            Assert.Equal(_products.First().Id, newProduct.Id);
        }

        //[Fact]
        public async void CreatePOST_InValidModelState_NeverCreateMethodExecute()
        {
            _productsController.ModelState.AddModelError("Name", "model state için hata oluştur");

            var result = await _productsController.Create(_products.First());

            //Create metodu hiç çalışmazsa testten geçer. doğrulanır. (Times.Never)
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never);
        }

        //[Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _productsController.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        //[Theory]
        [InlineData(3)] //Id = 3 _product nesnemizde olmadığından.
        public async void Edit_IdInVaid_ReturnNotFound(int productId)
        {
            Product product = null;

            //GetById metodu çalışınca sahte bir product döner, yukarıda verdiğimiz gibi.
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _productsController.Edit(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }


        //[Theory]
        [InlineData(2)] //Id = 2 _product nesnemizde olduğundan.
        public async void Edit_ActionExecutes_ReturnProduct(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            //GetById üzerinden bir mock oluşturuldu.
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            //edit metodu çalıştırıldı.
            var result = await _productsController.Edit(productId);
            //sonucun tipinin viewResult olması beklendi.
            //tip kontrolü yapar.
            var viewResult = Assert.IsType<ViewResult>(result);
            //IsAssignableFrom referans verilip verilmediğini kontrol eder. miras alınabilme durumu da kontrol eder.
            //IsAssignableFrom da verilen class'in object nesnesinden miras alıp almadığını kontrol eder.
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        //[Theory]
        [InlineData(1)]
        public void EditPOST_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = _productsController.Edit(2, _products.First(x => x.Id == productId));

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        //[Theory]
        [InlineData(1)]
        public void EditPOST_InValidModelState_ReturnView(int productId)
        {
            _productsController.ModelState.AddModelError("Name", "name gelmedi..");

            var result = _productsController.Edit(productId, _products.First(x => x.Id == productId));

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        //[Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {

            var result = _productsController.Edit(productId, _products.First(x => x.Id == productId));

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            // hangi sayfaya yönlendirdiğini görürüz.
            Assert.Equal("Index", redirect.ActionName);
        }

        //update mock'lama ile kontrol edilir.
        //[Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_ReturnUpdateMethodExecute(int productId)
        {
            //ürün bulunur.
            var product = _products.First(x => x.Id == productId);
            //ürüne ait bir repo datası gösterilir.
            _mockRepo.Setup(repo => repo.Update(product));
            //action çalıştırılır
            _productsController.Edit(productId, product);
            //repo'ya ilgili ürün gönderildiğini yakalar. (update işlemi olduğunu gösterir.)
            _mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
        }

        //[Fact]
        public async void  Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _productsController.Delete(null);
            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        //[Theory]
        [InlineData(0)] //id 0 olan yok.
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;

            //mock'lama ile repodan bu ürün çekilmek istenildi.(return edilen data Id=0 olduğundan null döndü.)
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            //ürün action ile silinmek iştenildi.
            var result = await _productsController.Delete(productId);

            //silinecek ürün bulunamadı
            Assert.IsType<NotFoundResult>(result);
        }

        //[Theory]
        [InlineData(1)]
        public async void Delete_ActionExecute_ReturnProduct(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _productsController.Delete(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            //atama yapılması ile model testi yapılır. 
            //Dönen modelin testidir.
            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }

        //[Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecute_ReturnRedirectToAction(int productId)
        {
            var result = await _productsController.DeleteConfirmed(productId);

            Assert.IsType<RedirectToActionResult>(result);

        }

        //Delete Metodunun çalışması kontrol edilir.
        //[Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecute_DeleteMethodExecute(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            // delete motedou entity sildiğinden entity gönderildi.
            _mockRepo.Setup(repo => repo.Delete(product));

            await _productsController.DeleteConfirmed(productId);

            //en az bir kere çalıştığını görebilmek için.
            _mockRepo.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
        }

    }
}
