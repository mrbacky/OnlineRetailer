using CustomerApi.Models;

namespace CustomerApi.Data;

public class DbInitializer : IDbInitializer
{
    // This method will create and seed the database.

    public void Initialize(CustomerApiContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Look for any Products
        if (context.Customers.Any()) return; // DB has been seeded

        var customers = new List<Customer>
        {
            new()
            {
                RegistrationNumber = "DK-1111", Name = "Apple", Email = "contact@apple.com", Phone = "204113562",
                BillingAddress = "Stormgade 101", ShippingAddress = "Stormgade 101"
            },
            new()
            {
                RegistrationNumber = "DK-5555", Name = "Netflix", Email = "contact@netflix.com", Phone = "4234243",
                BillingAddress = "Stormgade 22", ShippingAddress = "Stormgade 22"
            },
            new()
            {
                RegistrationNumber = "DK-2222", Name = "Amazon", Email = "contact@amazon.com", Phone = "2342534",
                BillingAddress = "Stormgade 29", ShippingAddress = "Stormgade 29"
            }
        };
        Console.WriteLine("*****>>> Customers: " + customers);
        context.Customers.AddRange(customers);
        context.SaveChanges();
    }
}