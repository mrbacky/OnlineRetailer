﻿using CustomerApi.Models;

namespace CustomerApi.Data;

public class DbInitializer : IDbInitializer
{
    // This method will create and seed the database.

    public void Initialize(CustomerApiContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Look for any Products
        if (context.Customers.Any())
        {
            return; // DB has been seeded
        }

        List<Customer> customers = new List<Customer>
        {
            new Customer
            {
                RegistrationNumber = "DK-1111", Name = "Apple", Email = "contact@apple.com", Phone = "204113562",
                BillingAddress = "Stormgade 101", ShippingAddress = "Stormgade 101"
            },
            new Customer
            {
                RegistrationNumber = "DK-5555", Name = "Netflix", Email = "contact@netflix.com", Phone = "204113562",
                BillingAddress = "Stormgade 22", ShippingAddress = "Stormgade 22"
            }
        };

        context.Customers.AddRange(customers);
        context.SaveChanges();
    }
}