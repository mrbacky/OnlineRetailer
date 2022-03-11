using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IRepository<Customer> repository;

        public CustomersController(IRepository<Customer> repos)
        {
            repository = repos;
        }

        // GET: orders
        [HttpGet]
        public async Task<IActionResult> Get()
        { 
                var result = await repository.GetAll();
                return Ok(result);
        }

        // GET orders/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // POST orders
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Customer customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }

            var newCustomer = await repository.Add(customer);

            return CreatedAtRoute("GetCustomer", new { id = newCustomer.Id }, newCustomer);
        }
        
        // PUT products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]Customer customer)
        {
            if (customer == null || customer.Id != id)
            {
                return BadRequest();
            }

            var modifiedCustomer = await repository.Get(id);

            if (modifiedCustomer == null)
            {
                return NotFound();
            }

            modifiedCustomer.Name = customer.Name;
            modifiedCustomer.Email = customer.Email;
            modifiedCustomer.Phone = customer.Phone;
            modifiedCustomer.BillingAddress = customer.BillingAddress;
            modifiedCustomer.ShippingAddress = customer.ShippingAddress;
            modifiedCustomer.CreditStanding = customer.CreditStanding;
            
            await repository.Edit(modifiedCustomer);
            return new NoContentResult();
        }

        [HttpDelete]

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await repository.Remove(id);
            return Ok();
        }
    }
}