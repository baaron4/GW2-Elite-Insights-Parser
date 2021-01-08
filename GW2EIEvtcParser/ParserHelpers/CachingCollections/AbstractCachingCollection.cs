using System;

namespace GW2EIEvtcParser
{
    public abstract class AbstractCachingCollection<T>
    {

        private readonly long _start;
        private readonly long _end;

        public AbstractCachingCollection(ParsedEvtcLog log)
        {
            _start = log.FightData.LogStart;
            _end = log.FightData.LogEnd;
        }

        protected (long, long) SanitizeTimes(long start, long end)
        {
            long newStart = Math.Max(start, _start);
            long newEnd = Math.Max(newStart, Math.Min(end, _end));
            return (newStart, newEnd);
        }

        public abstract void Clear();

    }
}
