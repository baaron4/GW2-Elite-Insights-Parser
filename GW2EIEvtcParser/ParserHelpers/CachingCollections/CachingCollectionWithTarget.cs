using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser
{
    internal class CachingCollectionWithTarget<T> : AbstractCachingCollection<T>
    {
        private static readonly NPC _nullActor = new NPC(new AgentItem());

        private readonly Dictionary<long, Dictionary<long, Dictionary<AbstractActor, T>>> _cache = new Dictionary<long, Dictionary<long, Dictionary<AbstractActor, T>>>();

        public CachingCollectionWithTarget(ParsedEvtcLog log) : base(log)
        {
        }

        public bool TryGetValue(long start, long end, AbstractActor actor, out T value)
        {
            (start, end) = SanitizeTimes(start, end);
            actor = actor ?? _nullActor;
            if (_cache.TryGetValue(start, out Dictionary<long, Dictionary<AbstractActor, T>> subCache))
            {
                if (subCache.TryGetValue(end, out Dictionary<AbstractActor, T> subSubCache))
                {
                    if (subSubCache.TryGetValue(actor, out value))
                    {
                        return true;
                    }
                }
            }
            value = default;
            return false;
        }

        public void Set(long start, long end, AbstractActor actor, T value)
        {
            (start, end) = SanitizeTimes(start, end);
            actor = actor ?? _nullActor;

            if (!_cache.TryGetValue(start, out Dictionary<long, Dictionary<AbstractActor, T>> subCache))
            {
                _cache[start] = new Dictionary<long, Dictionary<AbstractActor, T>>();
                subCache = _cache[start];
            }

            if (!subCache.TryGetValue(end, out Dictionary<AbstractActor, T> subSubCache))
            {
                subCache[end] = new Dictionary<AbstractActor, T>();
                subSubCache = subCache[end];
            }
            subSubCache[actor] = value;
        }

        public bool HasKeys(long start, long end, AbstractActor actor)
        {
            return TryGetValue(start, end, actor, out _);
        }

        public override void Clear()
        {
            _cache.Clear();
        }

    }
}
