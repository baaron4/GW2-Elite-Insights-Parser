using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffsTrackerSingle : BuffsTracker
    {
        private readonly long _id;

        public BuffsTrackerSingle(long id)
        {
            _id = id;
        }

        public override int GetStack(IReadOnlyDictionary<long, BuffsGraphModel> bgms, long time)
        {
            return bgms.TryGetValue(_id, out BuffsGraphModel bgm) ? bgm.GetStackCount(time) : 0;
        }

        public override bool Has(IReadOnlyDictionary<long, BuffsGraphModel> bgms)
        {
            return bgms.ContainsKey(_id);
        }
    }
}
