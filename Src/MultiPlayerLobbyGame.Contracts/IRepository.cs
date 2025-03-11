
namespace MultiPlayerLobbyGame.Contracts;

public interface IRepository<T, TKey>
    where TKey : IEquatable<TKey>, IComparable<TKey>
    where T : class, new()
{
    Task<T> GetById(TKey id);
    Task<IEnumerable<T>> GetAll();
    Task<bool> Insert(T item);
    Task<bool> Update(TKey id, T item);
    Task<bool> Delete(TKey id);
}

public interface IRedisRepository<T, TKey> : IRepository<T, TKey>
    where TKey : IEquatable<TKey>, IComparable<TKey>
    where T : class, new()
{
    Task<bool> Transaction(Func<IRedisRepository<T, TKey>> query);
}