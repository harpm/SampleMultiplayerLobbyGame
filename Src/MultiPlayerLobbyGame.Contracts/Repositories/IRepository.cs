namespace MultiPlayerLobbyGame.Contracts.Repositories;

public interface IRepository<T, TKey>
    where TKey : IEquatable<TKey>, IComparable<TKey>
    where T : class, new()
{
    T GetById(TKey id);
    Task<T> GetByIdAsync(TKey id);
    IEnumerable<T> GetAll();
    Task<IEnumerable<T>> GetAllAsync();
    bool Insert(T item);
    Task<bool> InsertAsync(T item);
    bool Update(TKey id, T item);
    Task<bool> UpdateAsync(TKey id, T item);
    bool Delete(TKey id);
    Task<bool> DeleteAsync(TKey id);
}

public interface IRedisRepository<T, TKey> : IRepository<T, TKey>
    where TKey : IEquatable<TKey>, IComparable<TKey>
    where T : class, new()
{
    bool Transaction(Func<bool> query);
    Task<bool> TransactionAsync(Func<Task<bool>> query);
}