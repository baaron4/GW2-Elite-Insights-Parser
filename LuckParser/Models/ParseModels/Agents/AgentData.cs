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
        public void addItem(AgentItem item, string prof)
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
        public List<AgentItem> getPlayerAgentList()
        {
            return player_agent_list;
        }

        public List<AgentItem> getNPCAgentList()
        {
            return NPC_agent_list;
        }

        public List<AgentItem> getGadgetAgentList()
        {
            return gadget_agent_list;
        }

        public List<AgentItem> getAllAgentsList()
        {
            return all_agents_list;
        }
        public AgentItem GetAgent(ulong agent)
        {
            if (agent != 0)
            {
                AgentItem agtreturn = all_agents_list.FirstOrDefault(x => x.getAgent() == agent);
                if (agtreturn != null)
                {
                    return agtreturn;
                }

            }

            return new AgentItem(0, "UNKOWN", "UNKNOWN");

        }
        public List<AgentItem> GetAgents(ushort id)
        {
            return all_agents_list.Where(x => x.getID() == id).ToList();
        }
        public AgentItem GetAgentWInst(ushort instid)
        {
            return all_agents_list.FirstOrDefault(x => x.getInstid() == instid);
        }

        public void clean()
        {
            gadget_agent_list = gadget_agent_list.Where(x => x.getInstid() != 0 && x.getLastAware() - x.getFirstAware() >= 0 && x.getFirstAware() != 0 && x.getLastAware() != long.MaxValue).ToList();
            NPC_agent_list = NPC_agent_list.Where(x => x.getInstid() != 0 && x.getLastAware() - x.getFirstAware() >= 0 && x.getFirstAware() != 0 && x.getLastAware() != long.MaxValue).ToList();
            all_agents_list = all_agents_list.Where(x => x.getInstid() != 0 && x.getLastAware() - x.getFirstAware() >= 0 && x.getFirstAware() != 0 && x.getLastAware() != long.MaxValue).ToList();
        }

        public void cleanInstid(ushort instid)
        {
            List<AgentItem> instances = NPC_agent_list.Where(x => x.getInstid() == instid).ToList();
            long first_aware = long.MaxValue;
            long last_aware = 0;
            if (instances.Count == 0)
            {
                return;
            }
            string name = instances[0].getName();
            string prof = instances[0].getProf();
            ulong agent = instances[0].getAgent();
            ushort inst = instances[0].getInstid();
            AgentItem to_add = new AgentItem(agent, name, prof);
            foreach (AgentItem a in instances)
            {
                first_aware = Math.Min(first_aware, a.getFirstAware());
                last_aware = Math.Max(last_aware, a.getLastAware());
                all_agents_list.Remove(a);
                NPC_agent_list.Remove(a);
            }
            to_add.setInstid(inst);
            to_add.setFirstAware(first_aware);
            to_add.setLastAware(last_aware);
            all_agents_list.Add(to_add);
            NPC_agent_list.Add(to_add);
        }
    }
}