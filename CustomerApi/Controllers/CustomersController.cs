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
        public IEnumerable<Customer> Get()
        {
            return repository.GetAll();
        }

        // GET orders/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public IActionResult Get(int id)
        {
            var item = repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST orders
        [HttpPost]
        public IActionResult Post([FromBody]Customer customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }

            var newCustomer = repository.Add(customer);

            return CreatedAtRoute("GetCustomer", new { id = newCustomer.Id }, newCustomer);
        }
        
        // PUT products/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Customer customer)
        {
            if (customer == null || customer.Id != id)
            {
                return BadRequest();
            }

            var modifiedCustomer = repository.Get(id);

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
            
            repository.Edit(modifiedCustomer);
            return new NoContentResult();
        }
    }
}