using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Data;

public class CustomerRepository : IRepository<Customer>
{
    private readonly CustomerApiContext _db;

    public CustomerRepository(CustomerApiContext context)
    {
        _db = context;
    }

    public async Task<IEnumerable<Customer>> GetAll()
    {
        return await _db.Customers.ToListAsync();
    }


    public async Task<Customer?> Get(int id)
    {
        return await _db.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer> Add(Customer entity)
    {
        var newCustomer = await _db.Customers.AddAsync(entity);
        await _db.SaveChangesAsync();
        return newCustomer.Entity;
    }

    public async Task Edit(Customer entity)
    {
        _db.Entry(entity).State = EntityState.Modified;
        await _db.SaveChangesAsync();
    }

    public async Task Remove(Customer customer)
    {
        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();
    }
}