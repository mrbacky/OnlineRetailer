using CustomerApi.Data;
using CustomerApi.Dtos;
using CustomerApi.Enums;
using CustomerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IRepository<Customer> _repository;

    public CustomersController(IRepository<Customer> repos)
    {
        _repository = repos;
    }

    // GET: orders
    [HttpGet]
    public async Task<IActionResult> GetAllCustomers()
    {
        var customers = await _repository.GetAll();
        return Ok(customers);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCustomerById(int id)
    {
        var customer = await _repository.Get(id);
        if (customer == null) return NotFound();
        
        return Ok(customer);
    }

    // POST orders
    [HttpPost]
    public async Task<IActionResult> CreateCustomerAsync([FromBody] NewCustomerDto newCustomer)
    {
        var customer = new Customer(
            registrationNumber: newCustomer.RegistrationNumber,
            name: newCustomer.Name,
            email: newCustomer.Email,
            phone: newCustomer.Phone,
            billingAddress: newCustomer.BillingAddress,
            shippingAddress: newCustomer.ShippingAddress);
        
        var created = await _repository.Add(customer);
        return Ok(created);
    }

    [HttpPut("{customerId:int}/Credit")]
    public async Task<IActionResult> UserCreatedOrder(int customerId, [FromBody] CreditAction action)
    {
        var customer = await _repository.Get(customerId);
        if (customer is null)
            return BadRequest($"Customer with ID: {customerId} does not exist");

        switch (action)
        {
            case CreditAction.DecreaseCredit:
                customer.CreditStanding -= 1;
                break;
            case CreditAction.IncreaseCredit:
                customer.CreditStanding += 1;
                break;
        }
        await _repository.Edit(customer);

        return NoContent();
    }

    // PUT products/5
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] CustomerPutBindingModel model)
    {
        var customer = await _repository.Get(model.Id);
        if (customer == null) 
            return NotFound($"Customer with Id: [{model.Id}] was not found.");

        var modifiedCustomer = customer;
        modifiedCustomer.Email = !string.IsNullOrEmpty(model.Email) 
            ? model.Email 
            : customer.Email;
        modifiedCustomer.Phone = !string.IsNullOrEmpty(model.Phone) 
            ? model.Phone 
            : customer.Phone;
        modifiedCustomer.BillingAddress = !string.IsNullOrEmpty(model.BillingAddress)
            ? model.BillingAddress
            : customer.BillingAddress;
        modifiedCustomer.ShippingAddress = !string.IsNullOrEmpty(model.ShippingAddress)
            ? model.ShippingAddress
            : customer.ShippingAddress;

        await _repository.Edit(modifiedCustomer);
        
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _repository.Get(id);
        if (customer is null) 
            return NotFound();

        await _repository.Remove(customer);
        
        return Ok();
    }
}