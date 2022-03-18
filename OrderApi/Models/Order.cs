using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OrderApi.Models;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public DateTime? Date { get; set; }

    public IEnumerable<OrderItem> OrderItems { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
}