namespace WA.Pizza.Core.Abstractions
{
    public interface IBasketService<T>
    {
        Task AddAsync(int? userId);

        Task RemoveAsync(int? userId, int? basketId);

        Task<T> GetAsync(int? userId, int? basketId);

        Task<T> UpdateAsync(T entity);
    }
}
