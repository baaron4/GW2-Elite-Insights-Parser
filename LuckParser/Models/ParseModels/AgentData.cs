using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public void addItem(Agent agent, AgentItem item,string buildVersion)
        {
            if (agent.getProf(buildVersion) == "NPC")
            {
                NPC_agent_list.Add(item);
            }
            else if (agent.getProf(buildVersion) == "GDG")
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

    }
}