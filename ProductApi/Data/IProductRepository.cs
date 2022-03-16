using System.Collections.Generic;
using ProductApi.Models;

namespace ProductApi.Data;

public interface IProductRepository: IRepository<Product>
{
    IEnumerable<Product> GetInRange(IEnumerable<int> productIds);
    void ReserveProducts(IEnumerable<ProductData> productData);
    void SellProducts(IEnumerable<ProductData> productData);
    void DeleteReservationOnProducts(IEnumerable<ProductData> productData);
}