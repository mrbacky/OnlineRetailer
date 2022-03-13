using System;
using System.Collections.Generic;

namespace OrderApi.Models;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public DateTime? Date { get; set; }

    public IEnumerable<OrderItem> OrderItems { get; set; }

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
}