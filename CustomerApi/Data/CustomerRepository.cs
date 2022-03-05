using System.Linq;
using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Data;

public class CustomerRepository: IRepository<Customer>
{
    private readonly CustomerApiContext _db;

    public CustomerRepository(CustomerApiContext context)
    {
        _db = context;
    }
    
    public IEnumerable<Customer> GetAll()
    {
        return _db.Customers.ToList();
    }

  

    Customer IRepository<Customer>.Get(int id)
    {
        return _db.Customers.FirstOrDefault(o => o.Id == id);
    }

    public Customer Add(Customer entity)
    {
        var newCustomer = _db.Customers.Add(entity).Entity;
        _db.SaveChanges();
        return newCustomer;
    }

    public void Edit(Customer entity)
    {
        _db.Entry(entity).State = EntityState.Modified;
        _db.SaveChanges();
    }

    void IRepository<Customer>.Remove(int id)
    {
        var customer = _db.Customers.FirstOrDefault(c => c.Id == id);
        _db.Customers.Remove(customer);
        _db.SaveChanges();
    }
}