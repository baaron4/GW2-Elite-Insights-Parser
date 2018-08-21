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
            this[boon.GetID()] = new List<BoonLog>();
        }

        public BoonMap(List<Boon> boons)
        {
            foreach (Boon boon in boons)
            {
                this[boon.GetID()] = new List<BoonLog>();
            }
        }


        public void Add(List<Boon> boons)
        {
            foreach (Boon boon in boons)
            {
                if (ContainsKey(boon.GetID()))
                {
                    continue;
                }
                this[boon.GetID()] = new List<BoonLog>();
            }
        }

        public void Add(Boon boon)
        {
            if (ContainsKey(boon.GetID()))
            {
                return;
            }
            this[boon.GetID()] = new List<BoonLog>();
        }
        
    }

}