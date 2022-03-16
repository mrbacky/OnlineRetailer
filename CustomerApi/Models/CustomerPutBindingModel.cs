using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Models;

public class CustomerPutBindingModel
{
    [Required]
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? BillingAddress { get; set; }
    public string? ShippingAddress { get; set; }
}


