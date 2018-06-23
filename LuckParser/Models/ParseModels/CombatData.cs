using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public List<CombatItem> getStates(int src_instid, ParseEnum.StateChange change, long start, long end)
        {
            List<CombatItem> states = new List<CombatItem>();
            foreach (CombatItem c in combat_list.Where(x => x.getTime() >= start && x.getTime() <= end))
            {
                if (c.getSrcInstid() == src_instid && c.isStateChange() == change)
                {
                    states.Add(c);
                }
            }
            return states;
        }

        public int getSkillCount(int src_instid, int skill_id, long start, long end)
        {
            int count = 0;
            foreach (CombatItem c in combat_list.Where(x => x.getTime() >= start && x.getTime() <= end))
            {
                if (c.getSrcInstid() == src_instid && c.getSkillID() == skill_id)
                {
                    if (DataModels.ParseEnum.casting(c.isActivation()))
                        count++;
                }
            }
            return count;
        }
        public int getBuffCount(int src_instid, int skill_id, long start, long end)
        {
            int count = 0;
            foreach (CombatItem c in combat_list.Where(x => x.getTime() >= start && x.getTime() <= end))
            {
                if (c.getSrcInstid() == src_instid && c.getSkillID() == skill_id)
                {
                    if (c.isBuff() == 1 && c.isBuffremove() == ParseEnum.BuffRemove.None)
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