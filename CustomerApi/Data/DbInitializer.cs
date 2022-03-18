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
            new(
                registrationNumber: "DK-1111", 
                name: "Apple", 
                email: "contact@apple.com", 
                phone: "204113562",
                billingAddress: "Stormgade 101",
                shippingAddress: "Stormgade 101"),
            new(
                registrationNumber: "DK-5555", 
                name: "Netflix", 
                email: "contact@netflix.com", 
                phone: "4234243",
                billingAddress: "Stormgade 22",
                shippingAddress: "Stormgade 22"),            
            new(
                registrationNumber: "DK-2222", 
                name: "Amazon", 
                email: "contact@amazon.com", 
                phone: "2342534",
                billingAddress: "Stormgade 29",
                shippingAddress: "Stormgade 29")
        };
        Console.WriteLine("*****>>> Customers: " + customers);
        context.Customers.AddRange(customers);
        context.SaveChanges();
    }
}