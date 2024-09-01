using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyRealWorldUnitTest.Web.Models;
using UdemyRealWorldUnitTest.Web.Repository;

namespace UdemyRealWorldUnitTest.Web.Controllers
{
    //GET //api/productsapi
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsAPIController : ControllerBase
    {
        private readonly IRepository<Product> _productsRepository;

        public ProductsAPIController(IRepository<Product> productsRepository)
        {
            _productsRepository = productsRepository;
        }


        [HttpGet("{a}/{b}")]
        public IActionResult Add(int a, int b)
        {
            //custom bir metodun 
            return Ok(new Helpers.Helper().Add(2, 5));
        }

        // GET: api/ProductsAPI
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productsRepository.GetAllAsync();

            return Ok(products);
        }

        // GET: api/ProductsAPI/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productsRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/ProductsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _productsRepository.Update(product);

            return NoContent();
        }

        // POST: api/ProductsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            await _productsRepository.Create(product);

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/ProductsAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _productsRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            _productsRepository.Delete(product);
            return NoContent();
        }

        private bool ProductExists(int id)
        {
            Product product = _productsRepository.GetById(id).Result;

            if (product == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
