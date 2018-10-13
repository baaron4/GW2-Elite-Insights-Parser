using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public class BoonMap : Dictionary<long, List<BoonLog>>
    {
        // Constructors
        public BoonMap()
        {
        }
        public BoonMap(Boon boon)
        {
            this[boon.ID] = new List<BoonLog>();
        }

        public BoonMap(IEnumerable<Boon> boons)
        {
            foreach (Boon boon in boons)
            {
                this[boon.ID] = new List<BoonLog>();
            }
        }


        public void Add(IEnumerable<Boon> boons)
        {
            foreach (Boon boon in boons)
            {
                if (ContainsKey(boon.ID))
                {
                    continue;
                }
                this[boon.ID] = new List<BoonLog>();
            }
        }

        public void Add(Boon boon)
        {
            if (ContainsKey(boon.ID))
            {
                return;
            }
            this[boon.ID] = new List<BoonLog>();
        }

        private int CompareApplicationType(BoonLog x, BoonLog y)
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
                if (x.GetType() == typeof(BoonRemovalLog))
                {
                    if (y.GetType() == typeof(BoonRemovalLog))
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    if (y.GetType() == typeof(BoonRemovalLog))
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }


        public void Sort()
        {
            foreach (var pair in this)
            {
                pair.Value.Sort(CompareApplicationType);
            }
        }
        
    }

}