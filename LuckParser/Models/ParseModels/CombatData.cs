using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class CombatData
    {
        // Fields
        private List<CombatItem> combat_list;

        // Constructors
        public CombatData()
        {
            this.combat_list = new List<CombatItem>();
        }

        // Public Methods
        public void addItem(CombatItem item)
        {
            combat_list.Add(item);
        }

        public List<Point> getStates(int src_instid, String change)
        {
            List<Point> states = new List<Point>();
            foreach (CombatItem c in combat_list)
            {
                if (c.getSrcInstid() == src_instid && c.isStateChange().getEnum() == change)
                {
                    states.Add(new Point(c.getTime(), (int)c.getDstAgent()));
                }
            }
            return states;
        }

        public int getSkillCount(int src_instid, int skill_id)
        {
            int count = 0;
            foreach (CombatItem c in combat_list)
            {
                if (c.getSrcInstid() == src_instid && c.getSkillID() == skill_id)
                {
                    if(c.isActivation().getID() == 1 || c.isActivation().getID() == 2)
                    count++;
                }
            }
            return count;
        }

        // Getters
        public List<CombatItem> getCombatList()
        {
            return combat_list;
        }
    }
}