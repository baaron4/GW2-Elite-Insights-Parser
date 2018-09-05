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

        public BoonMap(List<Boon> boons)
        {
            foreach (Boon boon in boons)
            {
                this[boon.ID] = new List<BoonLog>();
            }
        }


        public void Add(List<Boon> boons)
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
        
    }

}