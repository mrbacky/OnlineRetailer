using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers
{
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
        public IEnumerable<Customer> Get()
        {
            return _repository.GetAll();
        }

        // GET orders/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public IActionResult Get(int id)
        {
            var item = _repository.Get(id);
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

            var newCustomer = _repository.Add(customer);

            return CreatedAtRoute("GetCustomer", new { id = newCustomer.Id }, newCustomer);
        }
        
        // PUT products/5
        [HttpPut()]
        public IActionResult Put([FromBody]CustomerPutBindingModel model)
        {
           
            var foundCustomer = _repository.Get(model.Id);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (foundCustomer == null)
            {
                return NotFound($"Customer with Id: [{model.Id}] was not found.");
            }

            var modifiedCustomer = foundCustomer;
            modifiedCustomer.Email = !string.IsNullOrEmpty(model.Email) ? model.Email : foundCustomer.Email;
            modifiedCustomer.Phone = !string.IsNullOrEmpty(model.Phone) ? model.Phone : foundCustomer.Phone;
            modifiedCustomer.BillingAddress = !string.IsNullOrEmpty(model.BillingAddress) ? model.BillingAddress : foundCustomer.BillingAddress;
            modifiedCustomer.ShippingAddress = !string.IsNullOrEmpty(model.ShippingAddress) ? model.ShippingAddress : foundCustomer.ShippingAddress;
            
            _repository.Edit(modifiedCustomer);
            return new ObjectResult(modifiedCustomer);

        }
    }
}