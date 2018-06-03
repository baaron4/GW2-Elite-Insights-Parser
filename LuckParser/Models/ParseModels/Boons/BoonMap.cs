using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class BoonMap : Dictionary<int, List<BoonLog>>
    {
        // Constructors
        public BoonMap() : base()
        {
        }
        public BoonMap(Boon boon): base()
        {
            this[boon.getID()] = new List<BoonLog>();
        }

        public BoonMap(List<Boon> boons) : base()
        {
            foreach (Boon boon in boons)
            {
                this[boon.getID()] = new List<BoonLog>();
            }
        }


        public void add(List<Boon> boons)
        {
            foreach (Boon boon in boons)
            {
                if (ContainsKey(boon.getID()))
                {
                    continue;
                }
                this[boon.getID()] = new List<BoonLog>();
            }
        }

        public void add(Boon boon)
        {
            if (ContainsKey(boon.getID()))
            {
                return;
            }
            this[boon.getID()] = new List<BoonLog>();
        }
        
    }

}