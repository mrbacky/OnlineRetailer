using CustomerApi.Models;

namespace CustomerApi.Data;

public class DbInitializer: IDbInitializer
{
    // This method will create and seed the database.

    public void Initialize(CustomerApiContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Look for any Products
        if (context.Customers.Any())
        {
            return;   // DB has been seeded
        }

        List<Customer> customers = new List<Customer>
        {
            new Customer {Name = "Nemeth Armand", Email = "armandnemeth.work@gmail.com",Phone = "204113562", BillingAddress = "Stormgade 101",ShippingAddress = "Stormgade 101",CreditStanding = 0}
        };

        context.Customers.AddRange(customers);
        context.SaveChanges();
    }
}