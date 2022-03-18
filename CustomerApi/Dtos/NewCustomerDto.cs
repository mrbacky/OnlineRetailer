using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Dtos;

public class NewCustomerDto
{
    [Required]
    public string RegistrationNumber { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Phone{ get; set; }
    [Required]
    public string BillingAddress { get; set; }
    [Required]
    public string ShippingAddress { get; set; }
    
    public NewCustomerDto(string registrationNumber, string name, string email, string phone, string billingAddress, string shippingAddress)
    {
        RegistrationNumber = registrationNumber;
        Name = name;
        Email = email;
        Phone = phone;
        BillingAddress = billingAddress;
        ShippingAddress = shippingAddress;
    }
}