using System.Linq;
using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Data;

public class CustomerRepository: IRepository<Customer>
{
    private readonly CustomerApiContext db;
    
    public CustomerRepository(CustomerApiContext context)
    {
        db = context;
    }
    
    public IEnumerable<Customer> GetAll()
    {
        return db.Customers.ToList();
    }

    public Customer Get(int id)
    {
        return db.Customers.FirstOrDefault(o => o.CustomerId == id);
    }

    public Customer Add(Customer entity)
    {
        var newCustomer = db.Customers.Add(entity).Entity;
        db.SaveChanges();
        return newCustomer;
    }

    public void Edit(Customer entity)
    {
        db.Entry(entity).State = EntityState.Modified;
        db.SaveChanges();
    }

    public void Remove(int id)
    {
        var customer = db.Customers.FirstOrDefault(p => p.CustomerId == id);
        db.Customers.Remove(customer);
        db.SaveChanges();
    }
}