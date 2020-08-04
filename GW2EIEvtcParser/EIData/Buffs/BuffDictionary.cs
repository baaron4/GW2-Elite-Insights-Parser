using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class BuffDictionary : Dictionary<long, List<AbstractBuffEvent>>
    {
        // Constructors
        public BuffDictionary()
        {
        }
        public BuffDictionary(Buff buff)
        {
            this[buff.ID] = new List<AbstractBuffEvent>();
        }

        public BuffDictionary(IEnumerable<Buff> buffs)
        {
            foreach (Buff boon in buffs)
            {
                this[boon.ID] = new List<AbstractBuffEvent>();
            }
        }


        public void Add(IEnumerable<Buff> buffs)
        {
            foreach (Buff buff in buffs)
            {
                if (ContainsKey(buff.ID))
                {
                    continue;
                }
                this[buff.ID] = new List<AbstractBuffEvent>();
            }
        }

        public void Add(Buff buff)
        {
            if (ContainsKey(buff.ID))
            {
                return;
            }
            this[buff.ID] = new List<AbstractBuffEvent>();
        }

        private int CompareApplicationType(AbstractBuffEvent x, AbstractBuffEvent y)
        {
            if (x.Time < y.Time)
            {
                return -1;
            }
            else if (x.Time > y.Time)
            {
                return 1;
            }
            else
            {
                return x.CompareTo(y);
            }
        }


        public void Sort()
        {
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in this)
            {
                pair.Value.Sort(CompareApplicationType);
            }
        }

    }

}
