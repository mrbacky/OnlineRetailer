using System;
using System.Collections.Generic;
using System.Linq;
using CustomerApi.Models;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Models;
using RestSharp;

namespace OrderApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IRepository<Order> repository;

    public OrdersController(IRepository<Order> repos)
    {
        repository = repos;
    }

    // GET: orders
    [HttpGet]
    public IEnumerable<Order> Get()
    {
        return repository.GetAll();
    }

    // GET orders/5
    [HttpGet("{id}", Name = "GetOrder")]
    public IActionResult Get(int id)
    {
        var item = repository.Get(id);
        if (item == null) return NotFound();
        return new ObjectResult(item);
    }

    // POST orders
    [HttpPost]
    public IActionResult Post([FromBody] Order createOrder)
    {
        if (createOrder == null) return BadRequest();

        // fetching customer
        var customerService = new RestClient("http://localhost:6000/customers");
        var customerRequest = new RestRequest(createOrder.CustomerId.ToString());
        var customerResponse = customerService.GetAsync<Customer>(customerRequest);
        customerResponse.Wait();
        var foundCustomer = customerResponse.Result;

        // customer null check
        if (foundCustomer == null || foundCustomer.Id == 0)
            return NotFound($"Customer with Id: {createOrder.CustomerId} does not exist.");

        // Customer orders check
        if (foundCustomer.hasOutstandingBills)
            return BadRequest("Order declined. You have already an order you need to pay for.");

        // Get products from order
        var productIds = createOrder.OrderItems.Select(oi => oi.ProductId).ToArray();
        var productService = new RestClient("http://localhost:8000/products/inRange");
        var productRequest = new RestRequest().AddJsonBody(productIds);
        var productResponse = productService.GetAsync<List<Product>>(productRequest);
        productResponse.Wait();
        var orderedProducts = productResponse.Result;

        // Checking items availability and updating ordered products
        if (orderedProducts is not null)
            foreach (var prod in orderedProducts)
            {
                var matchedOrderItem = createOrder.OrderItems.First(oi => oi.ProductId == prod.Id);
                var isItemAvailable = matchedOrderItem.Quantity <= prod.ItemsInStock - prod.ItemsReserved;
                if (isItemAvailable)
                    prod.ItemsReserved += matchedOrderItem.Quantity;
                else if (!isItemAvailable)
                    return NotFound(
                        $"There is not enough items of product: ID: {prod.Id}, Product Name: {prod.Name}. " +
                        $"Available items in stock: {prod.ItemsInStock}");
            }

        var productsToUpdate = orderedProducts!;
        // Update products in product service
        productService = new RestClient("http://localhost:8000/products/updateCollection");
        var updateProductRequest = new RestRequest().AddJsonBody(productsToUpdate);
        var updateProductsResponse = productService.PutAsync(updateProductRequest);
        updateProductsResponse.Wait();


        if (updateProductsResponse.IsCompletedSuccessfully)
        {
            var newOrder = new Order
            {
                Date = DateTime.Now,
                CustomerId = createOrder.CustomerId,
                OrderItems = createOrder.OrderItems,
                OrderStatus = OrderStatus.Accepted
            };
            var created = repository.Add(newOrder);
            return CreatedAtRoute("GetOrder",
                new {id = created.Id}, created);
        }

        // If the order could not be created, "return no content".
        return NoContent();
    }
}