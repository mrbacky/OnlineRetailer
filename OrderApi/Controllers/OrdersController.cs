using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Models;
using ProductApi.Models;
using RestSharp;

namespace OrderApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IRepository<Order> _repository;
    private readonly string _customerBaseUrl;
    private readonly string _productBaseUrl;

    public OrdersController(IRepository<Order> repos)
    {
        _repository = repos;
        _customerBaseUrl = Environment.GetEnvironmentVariable("CustomerBaseUrl");
        _productBaseUrl = Environment.GetEnvironmentVariable("ProductBaseUrl");
    }

    [HttpGet]
    public IEnumerable<Order> GetAllOrders()
    {
        return _repository.GetAll();
    }

    [HttpGet("{id}")]
    public IActionResult GetOrderById(int id)
    {
        var order = _repository.Get(id);
        if (order == null) return NotFound();

        var prodIds = order.OrderItems.Select(x => x.ProductId);
        var orderedProducts = GetOrderedProducts(prodIds);

        foreach (var oi in order.OrderItems)
        {
            var foundProduct = orderedProducts.FirstOrDefault(p => p.Id == oi.ProductId);
            if (foundProduct is not null)
                if (oi.ProductId == foundProduct.Id)
                    oi.Product = foundProduct;
        }

        return Ok(order);
    }

    // GET orders/5
    [HttpGet("customer/{id}")]
    public IActionResult GetCustomerOrders(int id)
    {
        var orders = _repository.GetAll();
        var customerOrders = orders.Where(o => o.CustomerId == id);
        var customerDetailOrders = new List<Order>();
        foreach (var o in customerOrders)
        {
            var order = _repository.Get(o.Id);
            if (order == null) return NotFound();

            var prodIds = order.OrderItems.Select(x => x.ProductId);
            var orderedProducts = GetOrderedProducts(prodIds);

            foreach (var oi in order.OrderItems)
            {
                var foundProduct = orderedProducts.FirstOrDefault(p => p.Id == oi.ProductId);
                if (foundProduct is not null)
                    if (oi.ProductId == foundProduct.Id)
                        oi.Product = foundProduct;
            }

            customerDetailOrders.Add(order);
        }


        return Ok(customerDetailOrders);
    }

    // POST orders
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order createOrder)
    {
        if (createOrder == null) return BadRequest();

        // fetching customer
        var customerService = new RestClient($"{_customerBaseUrl}/customers");
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
        var prodIds = createOrder.OrderItems.Select(x => x.ProductId);
        var orderedProducts = GetOrderedProducts(prodIds);
        var reserveItems = new List<ProductData>();

        // Checking items availability and updating ordered products
        if (orderedProducts is not null)
            foreach (var prod in orderedProducts)
            {
                var matchedOrderItem = createOrder.OrderItems.FirstOrDefault(oi => oi.ProductId == prod.Id);
                if (matchedOrderItem is null)
                    return BadRequest($"Product with ID: {prod.Id} does not exist.");

                var isItemAvailable = matchedOrderItem.Quantity <= prod.AvailableToOrder;
                if (!isItemAvailable)
                    return NotFound(
                        $"There is not enough items of product: ID: {prod.Id}, Product Name: {prod.Name}. " +
                        $"Available items for order: {prod.AvailableToOrder}");
                reserveItems.Add(new ProductData {ProductId = prod.Id, Quantity = matchedOrderItem.Quantity});
            }

        // Send order to product service
        var productService = new RestClient($"{_productBaseUrl}/products/Reserve");
        var updateProductRequest = new RestRequest().AddJsonBody(reserveItems);
        var updateProductsResponse = productService.PostAsync(updateProductRequest);
        updateProductsResponse.Wait();

        // Update customer credit
        var customerService2 = new RestClient($"{_customerBaseUrl}/Customers/{foundCustomer.Id}/Credit");
        var customerRequest2 = new RestRequest().AddJsonBody("DecreaseCredit");
        var customerResponse2 = customerService2.DeleteAsync(customerRequest2);
        customerResponse2.Wait();


        if (updateProductsResponse.IsCompletedSuccessfully)
        {
            var newOrder = new Order
            {
                Date = DateTime.Now,
                CustomerId = createOrder.CustomerId,
                OrderItems = createOrder.OrderItems,
                OrderStatus = OrderStatus.Accepted
            };
            var created = _repository.Add(newOrder);


            return Ok(new {orderId = created.Id});
        }

        return BadRequest($"Response from product service: {updateProductsResponse.Exception}");
    }

    [HttpPut("{id:int}/Cancel")]
    public IActionResult CancelOrder(int id)
    {
        var order = _repository.Get(id);
        if (order is null) return NotFound("Order not found");

        order.OrderStatus = OrderStatus.Canceled;
        _repository.Edit(order);

        var productData = new List<ProductData>();
        foreach (var orderItem in order.OrderItems)
        {
            var prodData = new ProductData
            {
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity
            };
            productData.Add(prodData);
        }

        // remove reservations
        var productService = new RestClient($"{_productBaseUrl}/products/Reserve");
        var productRequest = new RestRequest().AddJsonBody(productData);
        var productResponse = productService.DeleteAsync(productRequest);
        productResponse.Wait();

        // Update customer credit
        var customerService = new RestClient($"{_customerBaseUrl}/Customers/{order.CustomerId}/Credit");
        var customerRequest = new RestRequest().AddJsonBody("IncreaseCredit");
        var customerResponse = customerService.DeleteAsync(customerRequest);
        customerResponse.Wait();

        return NoContent();
    }

    [HttpPut("{id:int}/Ship")]
    public IActionResult ShipOrder(int id)
    {
        var order = _repository.Get(id);
        if (order is null) return NotFound("Order not found");

        order.OrderStatus = OrderStatus.Shipped;
        _repository.Edit(order);

        var productData = new List<ProductData>();
        foreach (var orderItem in order.OrderItems)
        {
            var prodData = new ProductData
            {
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity
            };
            productData.Add(prodData);
        }

        // remove reservations
        var productService = new RestClient($"{_productBaseUrl}/products/Sell");
        var productRequest = new RestRequest().AddJsonBody(productData);
        var productResponse = productService.PostAsync(productRequest);
        productResponse.Wait();

        // Update customer credit
        var customerService = new RestClient($"{_customerBaseUrl}/Customers/{order.CustomerId}/Credit");
        var customerRequest = new RestRequest().AddJsonBody("IncreaseCredit");
        var customerResponse = customerService.DeleteAsync(customerRequest);
        customerResponse.Wait();

        return NoContent();
    }

    private List<Product> GetOrderedProducts(IEnumerable<int> productIds)
    {
        var productService = new RestClient($"{_productBaseUrl}/products/inRange");
        var productRequest = new RestRequest().AddJsonBody(productIds);
        var productResponse = productService.GetAsync<List<Product>>(productRequest);
        productResponse.Wait();
        var orderedProducts = productResponse.Result;
        return orderedProducts;
    }
}