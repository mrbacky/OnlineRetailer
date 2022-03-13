using System.Collections.Generic;

namespace OrderApi.Dtos;

public class CreateOrderDto
{
    public int CustomerId { get; set; }
    public IEnumerable<CreateOrderItemDto> OrderItems { get; set; }
}