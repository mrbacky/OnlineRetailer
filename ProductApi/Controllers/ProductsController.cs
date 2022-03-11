using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productProductRepository)
        {
            _productRepository = productProductRepository;
        }

        // GET products
        [HttpGet]
        public IEnumerable<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        // GET products/5
        [HttpGet("{id}", Name="GetProduct")]
        public IActionResult GetById(int id)
        {
            var item = _productRepository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpGet("InRange")]
        public IActionResult GetInRange([FromBody] IEnumerable<int> productIds)
        {
            var products = _productRepository.GetInRange(productIds);
            return Ok(products);
        }

        // POST products
        [HttpPost]
        public IActionResult Post([FromBody]Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            var newProduct = _productRepository.Add(product);

            return CreatedAtRoute("GetProduct", new { id = newProduct.Id }, newProduct);
        }

        // PUT products/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Product product)
        {
            if (product == null || product.Id != id)
            {
                return BadRequest();
            }

            var modifiedProduct = _productRepository.Get(id);

            if (modifiedProduct == null)
            {
                return NotFound();
            }

            modifiedProduct.Name = product.Name;
            modifiedProduct.Price = product.Price;
            modifiedProduct.ItemsInStock = product.ItemsInStock;
            modifiedProduct.ItemsReserved = product.ItemsReserved;

            _productRepository.Edit(modifiedProduct);
            return NoContent();
        }

        [HttpPost("Reserve")]
        public IActionResult ReserveProducts([FromBody] IEnumerable<ProductData> productData)
        {
            _productRepository.ReserveProducts(productData);
            return NoContent();
        }
        
        [HttpPost("Sell")]
        public IActionResult SellProducts([FromBody] IEnumerable<ProductData> productData)
        {
            _productRepository.SellProducts(productData);
            return NoContent();
        }

        // DELETE products/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_productRepository.Get(id) == null)
            {
                return NotFound();
            }

            _productRepository.Remove(id);
            return NoContent();
        }
        
        [HttpDelete("Reserve")]
        public IActionResult DeleteReservationOnProducts([FromBody] IEnumerable<ProductData> productData)
        {
            _productRepository.DeleteReservationOnProducts(productData);
            return NoContent();
        }
    }
}
