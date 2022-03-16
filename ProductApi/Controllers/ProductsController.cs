using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.Controllers;

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
    [HttpGet("{id}", Name = "GetProduct")]
    public IActionResult GetById(int id)
    {
        var item = _productRepository.Get(id);
        if (item == null) return NotFound();
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
    public IActionResult Post([FromBody] Product product)
    {
        if (product == null) return BadRequest();

        var newProduct = _productRepository.Add(product);

        return CreatedAtRoute("GetProduct", new {id = newProduct.Id}, newProduct);
    }

    // PUT products/5
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] Product product)
    {
        if (product == null || product.Id != id) return BadRequest();

        var modifiedProduct = _productRepository.Get(id);

        if (modifiedProduct == null) return NotFound();

        modifiedProduct.Name = product.Name;
        modifiedProduct.Price = product.Price;
        modifiedProduct.ItemsInStock = product.ItemsInStock;
        modifiedProduct.ItemsReserved = product.ItemsReserved;

        _productRepository.Edit(modifiedProduct);
        return NoContent();
    }

    // Updates list of products
    [HttpPut("updateCollection")]
    public IActionResult Put([FromBody] List<Product> products)
    {
        if (products == null) return BadRequest();
        foreach (var product in products) _productRepository.Edit(product);
        return NoContent();
    }

        [HttpPost("Reserve")]
        public IActionResult ReserveProducts([FromBody] IEnumerable<ProductData> productData)
        {
            foreach (var prod in productData)
            {
                var product = _productRepository.Get(prod.ProductId);
                if (product is null)
                {
                    return BadRequest($"Product with ID: {prod.ProductId} does not exist");
                }
                if (product.AvailableToOrder < prod.Quantity)
                {
                    return BadRequest(
                        $"Not enough items available to make this reservation. " +
                        $"Item ID: {product.Id}, Name: {product.Name}, " +
                        $"Available: {product.AvailableToOrder}, Requested: {prod.Quantity}");
                }
                
                product.ItemsReserved += prod.Quantity;
                _productRepository.Edit(product);
            }
            //_productRepository.ReserveProducts(productData);
            return NoContent();
        }
        
        [HttpPost("Sell")]
        public IActionResult SellProducts([FromBody] IEnumerable<ProductData> productData)
        {
            foreach (var prod in productData)
            {
                var product = _productRepository.Get(prod.ProductId);
                if (product is null)
                {
                    return BadRequest($"Product with ID: {prod.ProductId} does not exist");
                }

                product.ItemsReserved -= prod.Quantity;
                product.ItemsInStock -= prod.Quantity;
                
                _productRepository.Edit(product);
            }
            //_productRepository.SellProducts(productData);
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
            foreach (var prod in productData)
            {
                var product = _productRepository.Get(prod.ProductId);
                if (product is null)
                {
                    return BadRequest($"Product with ID: {prod.ProductId} does not exist");
                }

                product.ItemsReserved -= prod.Quantity;
                _productRepository.Edit(product);
            }
            //_productRepository.DeleteReservationOnProducts(productData);
            return NoContent();
        }
    }
}