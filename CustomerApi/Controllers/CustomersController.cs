using CustomerApi.Data;
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


    [HttpGet("{id}")]
    public IActionResult GetCustomerById(int id)
    {
        var customer = _repository.Get(id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    // POST orders
    [HttpPost]
    public async Task<IActionResult> CreateCustomerAsync([FromBody] Customer customer)
    {
        if (customer == null) return BadRequest();

        var newCustomer = await _repository.Add(customer);
        var created = await _repository.Get(newCustomer.Id);
        return Ok(created);
    }

    // PUT products/5
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] CustomerPutBindingModel model)
    {
        var foundCustomer = await _repository.Get(model.Id);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (foundCustomer == null) return NotFound($"Customer with Id: [{model.Id}] was not found.");

        var modifiedCustomer = foundCustomer;
        modifiedCustomer.Email = !string.IsNullOrEmpty(model.Email) ? model.Email : foundCustomer.Email;
        modifiedCustomer.Phone = !string.IsNullOrEmpty(model.Phone) ? model.Phone : foundCustomer.Phone;
        modifiedCustomer.BillingAddress = !string.IsNullOrEmpty(model.BillingAddress)
            ? model.BillingAddress
            : foundCustomer.BillingAddress;
        modifiedCustomer.ShippingAddress = !string.IsNullOrEmpty(model.ShippingAddress)
            ? model.ShippingAddress
            : foundCustomer.ShippingAddress;

        await _repository.Edit(modifiedCustomer);
        return new ObjectResult(modifiedCustomer);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        if (id == null) return NotFound();

        await _repository.Remove(id);
        return Ok();
    }
}