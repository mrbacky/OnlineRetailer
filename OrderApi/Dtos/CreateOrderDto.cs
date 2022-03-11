using System.Collections.Generic;

namespace OrderApi.Dtos;

public class CreateOrderDto
{
    public int CustomerId { get; set; }
    public List<CreateOrderItemDto> OrderItems { get; set; }
}