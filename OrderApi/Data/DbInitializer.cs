using System;
using System.Collections.Generic;
using System.Linq;
using OrderApi.Models;

namespace OrderApi.Data;

public class DbInitializer : IDbInitializer
{
    // This method will create and seed the database.
    public void Initialize(OrderApiContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Look for any Products
        if (context.Orders.Any()) return; // DB has been seeded

        var orders = new List<Order>
        {
            new()
            {
                CustomerId = 1,
                Date = DateTime.Now,
                OrderItems = new List<OrderItem>
                {
                    new()
                    {
                        ProductId = 2,
                        OrderId = 1,
                        Quantity = 5
                    },
                    new()
                    {
                        ProductId = 4,
                        OrderId = 1,
                        Quantity = 2
                    }
                }
            },
            new()
            {
                CustomerId = 2,
                Date = DateTime.Now,
                OrderItems = new List<OrderItem>
                {
                    new()
                    {
                        ProductId = 1,
                        OrderId = 2,
                        Quantity = 2
                    },
                    new()
                    {
                        ProductId = 2,
                        OrderId = 2,
                        Quantity = 3
                    }
                }
            }
        };

        context.Orders.AddRange(orders);
        context.SaveChanges();
    }
}