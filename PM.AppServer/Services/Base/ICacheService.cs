namespace PM.AppServer.Services.Base
{

public interface ICacheService<V>
{
    bool TryGetValue(string key, out V val);
    void Put(string key, V val);
}

}