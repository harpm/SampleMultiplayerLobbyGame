using System.Text.Json;
using StackExchange.Redis;
using MultiPlayerLobbyGame.Contracts;

namespace MultiPlayerLobbyGame.Data;

public abstract class RedisRepository<T, TKey> : IRedisRepository<T, TKey>
    where TKey : IEquatable<TKey>, IComparable<TKey>
    where T : RedisModel<TKey>, new()
{
    protected readonly IConnectionMultiplexer _connectionMultiplexer;
    protected readonly IDatabase _database;

    protected abstract string _key { get; }

    protected RedisRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        this._connectionMultiplexer = connectionMultiplexer;
        this._database = connectionMultiplexer.GetDatabase();
    }

    public virtual async Task<T> GetById(TKey id)
    {
        if (id == null || id.Equals(default(TKey))) throw new ArgumentNullException(nameof(id));

        var rawItem = await _database.HashGetAsync(_key, id.ToString());

        if (string.IsNullOrWhiteSpace(rawItem)) throw new KeyNotFoundException(nameof(id));

        var item = JsonSerializer.Deserialize<T>(rawItem);

        if (item == null || item == default(T)) throw new KeyNotFoundException(nameof(id));

        return item;
    }

    public virtual async Task<IEnumerable<T>> GetAll()
    {
        IEnumerable<T> result;

        var rawItems = await _database.HashGetAllAsync(_key);
        if (rawItems.Any())
        {
            result = rawItems.Select(i => JsonSerializer.Deserialize<T>(i.Value));
        }
        else
        {
            result = new List<T>();
        }

        return result;
    }

    /// <summary>
    /// Insert a new Item into the database only if a same item doesn't already exists
    /// </summary>
    /// <param name="item"></param>
    /// <returns> True if item doesn't exist in database and False if already exists in databsae </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public virtual async Task<bool> Insert(T item)
    {
        bool result = false;

        if (item == null || item == default(T)) throw new ArgumentNullException(nameof(item));

        var itemInDatabase = await _database.HashGetAsync(_key, item.Id.ToString());

        if (!itemInDatabase.HasValue || itemInDatabase.IsNullOrEmpty)
        {
            var jsonItem = JsonSerializer.Serialize(item);

            if (string.IsNullOrWhiteSpace(jsonItem)) throw new ArgumentException(nameof(item));

            result = await _database.HashSetAsync(_key, item.Id.ToString(), jsonItem);
        }
        else
        {
            result = false;
        }

        return result;
    }

    /// <summary>
    /// Update an existing item in database with new value
    /// </summary>
    /// <param name="id"></param>
    /// <param name="item"></param>
    /// <returns> True if item already exists and False if does not exists in database </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public virtual async Task<bool> Update(TKey id, T item)
    {
        bool result = false;

        if (item == null || item == default(T)) throw new ArgumentNullException(nameof(item));
        if (id == null || id.Equals(default(TKey))) throw new ArgumentNullException(nameof(id));

        var itemInDatabase = await _database.HashGetAsync(_key, item.Id.ToString());

        if (itemInDatabase.HasValue || !itemInDatabase.IsNullOrEmpty)
        {
            var jsonItem = JsonSerializer.Serialize(item);

            if (string.IsNullOrWhiteSpace(jsonItem)) throw new ArgumentException(nameof(item));

            result = !await _database.HashSetAsync(_key, item.Id.ToString(), jsonItem);
        }
        else
        {
            result = false;
        }

        return result;
    }

    /// <summary>
    /// Removes an item if it already exists in database
    /// </summary>
    /// <param name="id"></param>
    /// <returns> True if item already exists and removed successfully and False if it doesn't exists </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<bool> Delete(TKey id)
    {
        bool result = false;

        if (id == null || id.Equals(default(TKey))) throw new ArgumentNullException(nameof(id));

        result = await _database.HashDeleteAsync(_key, id.ToString());

        return result;
    }

    // TODO: Implement in the future
    public Task<bool> Transaction(Func<IRedisRepository<T, TKey>> query)
    {
        throw new NotImplementedException();
    }
}
