using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductApiContext _db;

        public ProductRepository(ProductApiContext context)
        {
            _db = context;
        }

        Product IRepository<Product>.Add(Product entity)
        {
            var newProduct = _db.Products.Add(entity).Entity;
            _db.SaveChanges();
            return newProduct;
        }

        void IRepository<Product>.Edit(Product entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            _db.SaveChanges();
        }

        Product IRepository<Product>.Get(int id)
        {
            return _db.Products.FirstOrDefault(p => p.Id == id);
        }

        IEnumerable<Product> IRepository<Product>.GetAll()
        {
            return _db.Products.ToList();
        }
        
        public IEnumerable<Product> GetInRange(IEnumerable<int> productIds)
        {
            var products = (
                from product in _db.Products 
                where productIds.Contains(product.Id) 
                select product).ToList();
            
            return products;
        }

        public void ReserveProducts(IEnumerable<ProductData> productData)
        {
            foreach (var p in productData)
            {
                var product = ((IRepository<Product>) this).Get(p.ProductId);
                product.ItemsReserved += p.Quantity;
                product.ItemsInStock -= p.Quantity;
                _db.Entry(product).State = EntityState.Modified;
            }
            _db.SaveChanges();
        }

        public void SellProducts(IEnumerable<ProductData> productData)
        {
            foreach (var p in productData)
            {
                var product = ((IRepository<Product>) this).Get(p.ProductId);
                product.ItemsReserved -= p.Quantity;
                _db.Entry(product).State = EntityState.Modified;
            }
            _db.SaveChanges();
        }

        public void DeleteReservationOnProducts(IEnumerable<ProductData> productData)
        {
            foreach (var p in productData)
            {
                var product = ((IRepository<Product>) this).Get(p.ProductId);
                product.ItemsReserved -= p.Quantity;
                product.ItemsInStock += p.Quantity;
                _db.Entry(product).State = EntityState.Modified;
            }
            _db.SaveChanges();
        }

        void IRepository<Product>.Remove(int id)
        {
            var product = _db.Products.FirstOrDefault(p => p.Id == id);
            if (product is null)
                return;
            _db.Products.Remove(product);
            _db.SaveChanges();
        }
    }
}
