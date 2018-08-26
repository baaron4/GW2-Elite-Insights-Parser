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
                AgentItem agtreturn = _allAgentList.FirstOrDefault(x => x.GetAgent() == agent);
                if (agtreturn != null)
                {
                    return agtreturn;
                }

            }

            return new AgentItem(0, "UNKOWN", "UNKNOWN",0,0,0,0);

        }
        public List<AgentItem> GetAgents(ushort id)
        {
            return _allAgentList.Where(x => x.GetID() == id).ToList();
        }
        public AgentItem GetAgentWInst(ushort instid)
        {
            return _allAgentList.FirstOrDefault(x => x.GetInstid() == instid);
        }

        public void Clean()
        {
            _gadgetAgentList.RemoveAll(x => !(x.GetInstid() != 0 && x.GetLastAware() - x.GetFirstAware() >= 0 && x.GetFirstAware() != 0 && x.GetLastAware() != long.MaxValue));
            _npcAgentList.RemoveAll(x => !(x.GetInstid() != 0 && x.GetLastAware() - x.GetFirstAware() >= 0 && x.GetFirstAware() != 0 && x.GetLastAware() != long.MaxValue));
            _allAgentList.RemoveAll(x => !(x.GetInstid() != 0 && x.GetLastAware() - x.GetFirstAware() >= 0 && x.GetFirstAware() != 0 && x.GetLastAware() != long.MaxValue));
        }

        public void CleanInstid(ushort instid)
        {
            List<AgentItem> instances = _npcAgentList.Where(x => x.GetInstid() == instid).ToList();
            long firstAware = long.MaxValue;
            long lastAware = 0;
            if (instances.Count == 0)
            {
                return;
            }
            string name = instances[0].GetName();
            string prof = instances[0].GetProf();
            ulong agent = instances[0].GetAgent();
            ushort inst = instances[0].GetInstid();
            AgentItem toAdd = new AgentItem(agent, name, prof, instances[0].GetToughness(), instances[0].GetHealing(), instances[0].GetCondition(), instances[0].GetConcentration());
            foreach (AgentItem a in instances)
            {
                firstAware = Math.Min(firstAware, a.GetFirstAware());
                lastAware = Math.Max(lastAware, a.GetLastAware());
                _allAgentList.Remove(a);
                _npcAgentList.Remove(a);
            }
            toAdd.SetInstid(inst);
            toAdd.SetFirstAware(firstAware);
            toAdd.SetLastAware(lastAware);
            _allAgentList.Add(toAdd);
            _npcAgentList.Add(toAdd);
        }
    }
}