using System;
using System.Collections.Generic;
using System.Threading;

namespace PM.AppServer.Models
{

public class SimpleCache<V>
{
    private readonly TimeSpan _cacheTtl;
    private DateTimeOffset _cacheUpdatedTime;

    private readonly ReaderWriterLockSlim _guard = new();
    private Dictionary<string, V> _cache;

    public SimpleCache(long cacheTtlMs)
    {
        _cacheTtl = TimeSpan.FromMilliseconds(cacheTtlMs);
        _cache = new Dictionary<string, V>();
    }

    public bool TryGetValue(string key, out V val)
    {
        if (DateTimeOffset.Now - _cacheUpdatedTime > _cacheTtl)
        {
            _guard.EnterWriteLock();
            try
            {
                _cache = new Dictionary<string, V>();
            }
            finally
            {
                _guard.ExitWriteLock();
            }

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

    public void Put(string key, V val)
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