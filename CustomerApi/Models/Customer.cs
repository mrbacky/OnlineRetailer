namespace CustomerApi.Models;

public class Customer
{
    public int Id { get; set; }
    public string RegistrationNumber { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone{ get; set; }
    public string BillingAddress { get; set; }
    public string ShippingAddress { get; set; }
    public int CreditStanding { get; set; }

    public Customer(int id, string registrationNumber, string name, string email, string phone, string billingAddress, string shippingAddress, int creditStanding)
    {
        Id = id;
        RegistrationNumber = registrationNumber;
        Name = name;
        Email = email;
        Phone = phone;
        BillingAddress = billingAddress;
        ShippingAddress = shippingAddress;
        CreditStanding = creditStanding;
    }

    public Customer(string registrationNumber, string name, string email, string phone, string billingAddress, string shippingAddress)
    {
        RegistrationNumber = registrationNumber;
        Name = name;
        Email = email;
        Phone = phone;
        BillingAddress = billingAddress;
        ShippingAddress = shippingAddress;
    }
}