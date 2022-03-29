using System;
using System.Collections.Generic;
using System.Threading;

namespace PM.AppServer.Models
{

public class SimpleCache<K, V>
{
    private readonly TimeSpan _cacheTtl;
    private DateTimeOffset _cacheUpdatedTime;

    private readonly ReaderWriterLockSlim _guard = new();
    private readonly Dictionary<K, V> _cache;

    public SimpleCache(TimeSpan cacheTtl)
    {
        _cacheTtl = cacheTtl;
        _cache = new Dictionary<K, V>();
    }

    public bool TryGetValue(K key, out V val)
    {
        if (DateTimeOffset.Now - _cacheUpdatedTime > _cacheTtl)
        {
            val = default;
            return false;
        }

        _guard.EnterReadLock();
        try
        {
            return _cache.TryGetValue(key, out val);
        }
        finally
        {
            _guard.ExitReadLock();
        }
    }

    public void Put(K key, V val)
    {
        _guard.EnterWriteLock();
        try
        {
            _cache[key] = val;
        }
        finally
        {
            _guard.ExitWriteLock();
            _cacheUpdatedTime = DateTimeOffset.Now;
        }
    }
}

}