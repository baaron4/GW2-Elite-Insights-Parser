using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class AgentData
    {
        // Fields
        private readonly List<AgentItem> _playerAgentList = new List<AgentItem>();
        private readonly List<AgentItem> _npcAgentList = new List<AgentItem>();
        private readonly List<AgentItem> _gadgetAgentList = new List<AgentItem>();
        private readonly List<AgentItem> _allAgentList = new List<AgentItem>();

        // Public Methods
        public void AddItem(AgentItem item, string prof)
        {
            if (prof == "NPC")
            {
                _npcAgentList.Add(item);
            }
            else if (prof == "GDG")
            {
                _gadgetAgentList.Add(item);
            }
            else
            {
                _playerAgentList.Add(item);
            }
            _allAgentList.Add(item);
        }

        // Getters
        public List<AgentItem> GetPlayerAgentList()
        {
            return _playerAgentList;
        }

        public List<AgentItem> GetNPCAgentList()
        {
            return _npcAgentList;
        }

        public List<AgentItem> GetGadgetAgentList()
        {
            return _gadgetAgentList;
        }

        public List<AgentItem> GetAllAgentsList()
        {
            return _allAgentList;
        }
        public AgentItem GetAgent(ulong agent)
        {
            if (agent != 0)
            {
                AgentItem agtreturn = _allAgentList.FirstOrDefault(x => x.Agent == agent);
                if (agtreturn != null)
                {
                    return agtreturn;
                }

            }

            return new AgentItem(0, "UNKOWN", "UNKNOWN",0,0,0,0,0,0);

        }
        public List<AgentItem> GetAgents(ushort id)
        {
            return _allAgentList.Where(x => x.ID == id).ToList();
        }
        public AgentItem GetAgentWInst(ushort instid)
        {
            return _allAgentList.FirstOrDefault(x => x.InstID == instid);
        }

        public void Clean()
        {
            _gadgetAgentList.RemoveAll(x => !(x.InstID != 0 && x.LastAware - x.FirstAware >= 0 && x.FirstAware != 0 && x.LastAware != long.MaxValue));
            _npcAgentList.RemoveAll(x => !(x.InstID != 0 && x.LastAware - x.FirstAware >= 0 && x.FirstAware != 0 && x.LastAware != long.MaxValue));
            _allAgentList.RemoveAll(x => !(x.InstID != 0 && x.LastAware - x.FirstAware >= 0 && x.FirstAware != 0 && x.LastAware != long.MaxValue));
        }

        public void CleanInstid(ushort instid)
        {
            List<AgentItem> instances = _npcAgentList.Where(x => x.InstID == instid).ToList();
            long firstAware = long.MaxValue;
            long lastAware = 0;
            if (instances.Count == 0)
            {
                return;
            }
            AgentItem firstInstance = instances[0];
            string name = firstInstance.Name;
            string prof = firstInstance.Prof;
            ulong agent = firstInstance.Agent;
            ushort inst = firstInstance.InstID;
            AgentItem toAdd = new AgentItem(agent, name, prof, firstInstance.Toughness, firstInstance.Healing, firstInstance.Condition, firstInstance.Concentration, firstInstance.HitboxWidth, firstInstance.HitboxHeight);
            foreach (AgentItem a in instances)
            {
                firstAware = Math.Min(firstAware, a.FirstAware);
                lastAware = Math.Max(lastAware, a.LastAware);
                _allAgentList.Remove(a);
                _npcAgentList.Remove(a);
            }
            toAdd.InstID = inst;
            toAdd.FirstAware = firstAware;
            toAdd.LastAware = lastAware;
            _allAgentList.Add(toAdd);
            _npcAgentList.Add(toAdd);
        }
    }
}