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

        public override int GetStack(Dictionary<long, BuffsGraphModel> bgms, long time)
        {
            if (bgms.TryGetValue(_id, out BuffsGraphModel bgm))
            {
                return bgm.GetStackCount(time);
            }
            return 0;
        }

        public override bool Has(Dictionary<long, BuffsGraphModel> bgms)
        {
            return bgms.ContainsKey(_id);
        }
    }
}
