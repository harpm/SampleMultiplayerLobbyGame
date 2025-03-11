
namespace MultiPlayerLobbyGame.Contracts;

public class RedisModel<TKey>
    where TKey : IEquatable<TKey>, IComparable<TKey>
{
    public virtual TKey Id { get; set; }
}
