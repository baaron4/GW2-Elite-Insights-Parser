using System.Collections.Generic;

namespace LuckParser.EIData
{
    public class BuffsTrackerSingle : BuffsTracker
    {
        private readonly long _id;

        public BuffsTrackerSingle(long id)
        {
            _id = id;
        }

        public override int GetStack(Dictionary<long, BoonsGraphModel> bgms, long time)
        {
            if (bgms.TryGetValue(_id, out var bgm))
            {
                return bgm.GetStackCount(time);
            }
            return 0;
        }

        public override bool Has(Dictionary<long, BoonsGraphModel> bgms)
        {
            return bgms.ContainsKey(_id);
        }
    }
}
