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

        var orderItem1 = new OrderItem
        {
            ProductId = 1,
            OrderId = 1,
            Quantity = 3
        };

        var orderItem2 = new OrderItem
        {
            ProductId = 2,
            OrderId = 1,
            Quantity = 9
        };


        var orders = new List<Order>
        {
            new()
            {
                Date = DateTime.Now,
                OrderItems = new List<OrderItem> {orderItem1, orderItem2}
            }
        };

        context.Orders.AddRange(orders);
        context.SaveChanges();
    }
}