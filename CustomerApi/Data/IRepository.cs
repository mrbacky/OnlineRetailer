using CustomerApi.Models;

namespace CustomerApi.Data;

public interface IRepository<T>
{
   Task<IEnumerable<T>> GetAll();
    Task<T?> Get(int id);
    Task<T> Add(T entity);
    Task Edit(T entity);
    Task Remove(T entity);
}