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

        public List<Point> getStates(int src_instid, String change, long start = 0, long end = 0)
        {
            List<Point> states = new List<Point>();
            List<CombatItem> li = (end - start) > 0 ? combat_list.Where(x => x.getTime() >= 0 && x.getTime() < end).ToList() : combat_list;
            foreach (CombatItem c in li)
            {
                if (c.getSrcInstid() == src_instid && c.isStateChange().getEnum() == change)
                {
                    states.Add(new Point((int)c.getTime(), (int)c.getDstAgent()));
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
        public int getBuffCount(int src_instid, int skill_id)
        {
            int count = 0;
            foreach (CombatItem c in combat_list)
            {
                if (c.getSrcInstid() == src_instid && c.getSkillID() == skill_id)
                {
                    if (c.isBuff() == 1 && c.isBuffremove().getID() == 0 )
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