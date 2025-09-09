using System.Diagnostics.CodeAnalysis;

namespace GW2EIEvtcParser;

public class CachingCollectionCustom<Q, T>(ParsedEvtcLog log, Q nullValue, int initialTertiaryCapacity) 
    : AbstractCachingCollection<T>(log)
{
    private readonly int _initialSecondaryCapacity = 20;
    private readonly int _initialTertiaryCapacity = initialTertiaryCapacity;
    private readonly Q _nullValue = nullValue;

    internal readonly Dictionary<long, Dictionary<long, Dictionary<Q, T>>> _cache = new(20);

    public bool TryGetValue(long start, long end, Q? q, [NotNullWhen(true)] out T? value)
    {
        (start, end) = SanitizeTimes(start, end);
        q = q == null ? _nullValue : q;
        if (_cache.TryGetValue(start, out var subCache))
        {
            if (subCache.TryGetValue(end, out var subSubCache))
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

    public bool TryGetEnglobingValue(long start, long end, Q? q, [NotNullWhen(true)] out T? value)
    {
        (start, end) = SanitizeTimes(start, end);
        q = q == null ? _nullValue : q;
        var englobingStart = _cache.Keys.Where(x => x <= start).DefaultIfEmpty(0).Max();
        if (_cache.TryGetValue(englobingStart, out var subCache))
        {
            var englobingEnd = subCache.Keys.Where(x => x >= end).DefaultIfEmpty(0).Min();
            if (subCache.TryGetValue(englobingEnd, out var subSubCache))
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

        if (!_cache.TryGetValue(start, out var subCache))
        {
            _cache[start] = new Dictionary<long, Dictionary<Q, T>>(_initialSecondaryCapacity);
            subCache = _cache[start];
        }

        if (!subCache.TryGetValue(end, out var subSubCache))
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
