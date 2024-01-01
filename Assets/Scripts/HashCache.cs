// Class grabbed from LiteNetLib source code
// Maybe can be some problems after lib updating
public static class HashCache<T>
{
    public static readonly ulong Id;

    //FNV-1 64 bit hash
    static HashCache()
    {
        ulong hash = 14695981039346656037UL; //offset
        string typeName = typeof(T).ToString();
        for (var i = 0; i < typeName.Length; i++)
        {
            hash ^= typeName[i];
            hash *= 1099511628211UL; //prime
        }
        Id = hash;
    }
}