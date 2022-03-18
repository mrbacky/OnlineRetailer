using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    [HttpGet]
    public IEnumerable<Order> GetAllOrders()
    {
        return repository.GetAll();
    }

    [HttpGet("{id}")]
    public IActionResult GetOrderById(int id)
    {
        var item = repository.Get(id);
        if (item == null) return NotFound();
        return new ObjectResult(item);
    }

    // GET orders/5
    [HttpGet("customer/{id}")]
    public IActionResult GetCustomerOrders(int id)
    {
        var orders = repository.GetAll();
        if (orders == null) return NotFound();

        var customerOrders = orders.Where(order => order.CustomerId == id);
        return Ok(customerOrders);
    }

    // POST orders
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order createOrder)
    {
        if (createOrder == null) return BadRequest();

        // fetching customer
        var customerService = new RestClient("http://localhost:6000/customers");
        var customerRequest = new RestRequest(createOrder.CustomerId.ToString());
        var customerResponse = await customerService.GetAsync<Customer>(customerRequest);
        // customerResponse.Wait();
        var foundCustomer = customerResponse;

        // customer null check
        if (foundCustomer == null)
            return NotFound($"Customer with Id: {createOrder.CustomerId} does not exist.");

        // Customer orders check
        if (foundCustomer.CreditStanding < 0)
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
                var isItemAvailable = matchedOrderItem.Quantity <= prod.AvailableToOrder;
                if (isItemAvailable)
                    prod.ItemsReserved += matchedOrderItem.Quantity;
                else if (!isItemAvailable)
                    return NotFound(
                        $"There is not enough items of product: ID: {prod.Id}, Product Name: {prod.Name}. " +
                        $"Available items for order: {prod.AvailableToOrder}");
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
            return Ok(created);
        }

        // If the order could not be created, "return no content".
        return NoContent();
    }
}