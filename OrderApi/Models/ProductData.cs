using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models;

public class ProductData
{
    public int ProductId { get; set; }
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}