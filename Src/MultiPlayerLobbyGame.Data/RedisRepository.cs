using System.Text.Json;
using StackExchange.Redis;
using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Contracts.Repositories;

namespace MultiPlayerLobbyGame.Data;

public abstract class RedisRepositoryBase<T, TKey> : IRedisRepository<T, TKey>
    where TKey : IEquatable<TKey>, IComparable<TKey>
    where T : RedisModel<TKey>, new()
{
    protected readonly IConnectionMultiplexer _connectionMultiplexer;
    protected readonly IDatabase _database;

    protected abstract string _key { get; }

    protected RedisRepositoryBase(IConnectionMultiplexer connectionMultiplexer)
    {
        this._connectionMultiplexer = connectionMultiplexer;
        this._database = connectionMultiplexer.GetDatabase();
    }

    public virtual T GetById(TKey id)
    {
        if (id == null || id.Equals(default(TKey))) throw new ArgumentNullException(nameof(id));

        var rawItem = _database.HashGet(_key, id.ToString());

        if (string.IsNullOrWhiteSpace(rawItem)) throw new KeyNotFoundException(nameof(id));

        var item = JsonSerializer.Deserialize<T>(rawItem);

        if (item == null || item == default(T)) throw new KeyNotFoundException(nameof(id));

        return item;
    }

    public virtual async Task<T> GetByIdAsync(TKey id)
    {
        if (id == null || id.Equals(default(TKey))) throw new ArgumentNullException(nameof(id));

        var rawItem = await _database.HashGetAsync(_key, id.ToString());

        if (string.IsNullOrWhiteSpace(rawItem)) throw new KeyNotFoundException(nameof(id));

        var item = JsonSerializer.Deserialize<T>(rawItem);

        if (item == null || item == default(T)) throw new KeyNotFoundException(nameof(id));

        return item;
    }

    public virtual IEnumerable<T> GetAll()
    {
        IEnumerable<T> result;

        var rawItems = _database.HashGetAll(_key);
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

    public virtual async Task<IEnumerable<T>> GetAllAsync()
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
    public virtual bool Insert(T item)
    {
        bool result = false;

        if (item == null || item == default(T)) throw new ArgumentNullException(nameof(item));

        var itemInDatabase = _database.HashGet(_key, item.Id.ToString());

        if (!itemInDatabase.HasValue || itemInDatabase.IsNullOrEmpty)
        {
            var jsonItem = JsonSerializer.Serialize(item);

            if (string.IsNullOrWhiteSpace(jsonItem)) throw new ArgumentException(nameof(item));

            result = _database.HashSet(_key, item.Id.ToString(), jsonItem);
        }
        else
        {
            result = false;
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
    public virtual async Task<bool> InsertAsync(T item)
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
    public virtual bool Update(TKey id, T item)
    {
        bool result = false;

        if (item == null || item == default(T)) throw new ArgumentNullException(nameof(item));
        if (id == null || id.Equals(default(TKey))) throw new ArgumentNullException(nameof(id));

        var itemInDatabase = _database.HashGet(_key, item.Id.ToString());

        if (itemInDatabase.HasValue || !itemInDatabase.IsNullOrEmpty)
        {
            var jsonItem = JsonSerializer.Serialize(item);

            if (string.IsNullOrWhiteSpace(jsonItem)) throw new ArgumentException(nameof(item));

            result = !_database.HashSet(_key, item.Id.ToString(), jsonItem);
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
    public virtual async Task<bool> UpdateAsync(TKey id, T item)
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
    public bool Delete(TKey id)
    {
        bool result = false;

        if (id == null || id.Equals(default(TKey))) throw new ArgumentNullException(nameof(id));

        result = _database.HashDelete(_key, id.ToString());

        return result;
    }

    /// <summary>
    /// Removes an item if it already exists in database
    /// </summary>
    /// <param name="id"></param>
    /// <returns> True if item already exists and removed successfully and False if it doesn't exists </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<bool> DeleteAsync(TKey id)
    {
        bool result = false;

        if (id == null || id.Equals(default(TKey))) throw new ArgumentNullException(nameof(id));

        result = _database.HashDelete(_key, id.ToString());

        return result;
    }

    /// <summary>
    /// Execute a transaction thread safe and 
    /// </summary>
    /// <param name="query"></param>
    /// <returns> True if the transaction committed successfully and false if transaction aborted </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Transaction(Func<bool> query)
    {
        bool result = false;

        if (query == null) throw new ArgumentNullException(nameof(query));

        if (_database.StringGet($"{_key}_LOCK") == "UNLOCKED")
        {
            _database.StringSet($"{_key}_LOCK", "LOCKED");
            _database.Execute($"WATCH {_key}_LOCK");
            _database.Execute("MULTI");

            try
            {
                query.Invoke();
                // TODO: consider exec result
                _database.Execute("EXEC");
                result = true;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _database.StringSet($"{_key}_LOCK", "UNLOCKED");
            }
        }

        return result;
    }

    public async Task<bool> TransactionAsync(Func<Task<bool>> query)
    {
        bool result = false;

        if (query == null) throw new ArgumentNullException(nameof(query));

        var lockValue = await _database.StringGetAsync($"{_key}_LOCK");

        if (lockValue == "UNLOCKED")
        {

            _database.StringSet($"{_key}_LOCK", "LOCKED");
            _database.Execute($"WATCH {_key}_LOCK");
            _database.Execute("MULTI");

            try
            {
                await query.Invoke();
                // TODO: consider exec result
                _database.Execute("EXEC");
                result = true;
            }
            catch (Exception ex)
            {

            }
        }

        return result;
    }
}
