using LuckParser.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class AgentData
    {
        // Fields
        private List<AgentItem> player_agent_list = new List<AgentItem>();
        private List<AgentItem> NPC_agent_list = new List<AgentItem>();
        private List<AgentItem> gadget_agent_list = new List<AgentItem>();
        private List<AgentItem> all_agents_list = new List<AgentItem>();

        // Constructors
        public AgentData()
        {
        }

        // Public Methods
        public void AddItem(AgentItem item, string prof)
        {
            if (prof == "NPC")
            {
                NPC_agent_list.Add(item);
            }
            else if (prof == "GDG")
            {
                gadget_agent_list.Add(item);
            }
            else
            {
                player_agent_list.Add(item);
            }
            all_agents_list.Add(item);
        }

        // Getters
        public List<AgentItem> GetPlayerAgentList()
        {
            return player_agent_list;
        }

        public List<AgentItem> GetNPCAgentList()
        {
            return NPC_agent_list;
        }

        public List<AgentItem> GetGadgetAgentList()
        {
            return gadget_agent_list;
        }

        public List<AgentItem> GetAllAgentsList()
        {
            return all_agents_list;
        }
        public AgentItem GetAgent(ulong agent)
        {
            if (agent != 0)
            {
                AgentItem agtreturn = all_agents_list.FirstOrDefault(x => x.GetAgent() == agent);
                if (agtreturn != null)
                {
                    return agtreturn;
                }

            }

            return new AgentItem(0, "UNKOWN", "UNKNOWN",0,0,0,0);

        }
        public List<AgentItem> GetAgents(ushort id)
        {
            return all_agents_list.Where(x => x.GetID() == id).ToList();
        }
        public AgentItem GetAgentWInst(ushort instid)
        {
            return all_agents_list.FirstOrDefault(x => x.GetInstid() == instid);
        }

        public void Clean()
        {
            gadget_agent_list = gadget_agent_list.Where(x => x.GetInstid() != 0 && x.GetLastAware() - x.GetFirstAware() >= 0 && x.GetFirstAware() != 0 && x.GetLastAware() != long.MaxValue).ToList();
            NPC_agent_list = NPC_agent_list.Where(x => x.GetInstid() != 0 && x.GetLastAware() - x.GetFirstAware() >= 0 && x.GetFirstAware() != 0 && x.GetLastAware() != long.MaxValue).ToList();
            all_agents_list = all_agents_list.Where(x => x.GetInstid() != 0 && x.GetLastAware() - x.GetFirstAware() >= 0 && x.GetFirstAware() != 0 && x.GetLastAware() != long.MaxValue).ToList();
        }

        public void CleanInstid(ushort instid)
        {
            List<AgentItem> instances = NPC_agent_list.Where(x => x.GetInstid() == instid).ToList();
            long first_aware = long.MaxValue;
            long last_aware = 0;
            if (instances.Count == 0)
            {
                return;
            }
            string name = instances[0].GetName();
            string prof = instances[0].GetProf();
            ulong agent = instances[0].GetAgent();
            ushort inst = instances[0].GetInstid();
            AgentItem to_add = new AgentItem(agent, name, prof, instances[0].GetToughness(), instances[0].GetHealing(), instances[0].GetCondition(), instances[0].GetConcentration());
            foreach (AgentItem a in instances)
            {
                first_aware = Math.Min(first_aware, a.GetFirstAware());
                last_aware = Math.Max(last_aware, a.GetLastAware());
                all_agents_list.Remove(a);
                NPC_agent_list.Remove(a);
            }
            to_add.SetInstid(inst);
            to_add.SetFirstAware(first_aware);
            to_add.SetLastAware(last_aware);
            all_agents_list.Add(to_add);
            NPC_agent_list.Add(to_add);
        }
    }
}