namespace CustomerApi.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string billingAddress { get; set; }
    public string shippingAddress { get; set; }
    public bool creditStanding { get; set; }
}