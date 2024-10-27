using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GW2EIEvtcParser;

public class CachingCollectionCustom<Q, T>(ParsedEvtcLog log, Q nullValue, int initialPrimaryCapacity, int initialSecondaryCapacity, int initialTertiaryCapacity) 
    : AbstractCachingCollection<T>(log)
{
    private readonly int _initialSecondaryCapacity = initialSecondaryCapacity;
    private readonly int _initialTertiaryCapacity = initialTertiaryCapacity;
    private readonly Q _nullValue = nullValue;

    internal readonly Dictionary<long, Dictionary<long, Dictionary<Q, T>>> _cache = new(initialPrimaryCapacity);

    public bool TryGetValue(long start, long end, Q? q, [NotNullWhen(true)] out T? value)
    {
        (start, end) = SanitizeTimes(start, end);
        q = q == null ? _nullValue : q;
        if (_cache.TryGetValue(start, out Dictionary<long, Dictionary<Q, T>> subCache))
        {
            if (subCache.TryGetValue(end, out Dictionary<Q, T> subSubCache))
            {
                if (subSubCache.TryGetValue(q, out value!))
                {
                    return true;
                }
            }
        }
        value = default;
        return false;
    }

    public void Set(long start, long end, Q? q, T value)
    {
        (start, end) = SanitizeTimes(start, end);
        q = q == null ? _nullValue : q;

        if (!_cache.TryGetValue(start, out Dictionary<long, Dictionary<Q, T>> subCache))
        {
            _cache[start] = new Dictionary<long, Dictionary<Q, T>>(_initialSecondaryCapacity);
            subCache = _cache[start];
        }

        if (!subCache.TryGetValue(end, out Dictionary<Q, T> subSubCache))
        {
            subCache[end] = new Dictionary<Q, T>(_initialTertiaryCapacity);
            subSubCache = subCache[end];
        }
        subSubCache[q] = value;
    }

    public bool HasKeys(long start, long end, Q q)
    {
        return TryGetValue(start, end, q, out _);
    }

    public T? Get(long start, long end, Q q)
    {
        if (TryGetValue(start, end, q, out T? value))
        {
            return value;
        }
        return default;
    }

    public override void Clear()
    {
        _cache.Clear();
    }

}
