using System.Collections.Generic;

namespace OrderApi.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int ItemsInStock { get; set; }
    public int ItemsReserved { get; set; }

    public int AvailableToOrder => ItemsInStock - ItemsReserved;
    public IEnumerable<OrderItem> OrderItems { get; set; }
}