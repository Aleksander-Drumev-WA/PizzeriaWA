namespace WA.Pizza.Core.Abstractions
{
    public interface IBaseService<T>
    {
        Task AddAsync(T entity);

        Task RemoveAsync(int Id);

        Task<T> GetAsync(int Id);

        Task<T> UpdateAsync(T entity);

        Task<IEnumerable<T>> GetAllAsync();
    }
}
