namespace OrderApi.Models;

public class OrderItem
{
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; }

    public int Quantity { get; set; }

    public double SubTotalPrice { get; set; }
}