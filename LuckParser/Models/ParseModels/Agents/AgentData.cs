using LuckParser.Controllers;
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
        public void addItem(Agent agent, AgentItem item,string buildVersion,GW2APIController apiController)
        {
            if (agent.getProf(buildVersion, apiController) == "NPC")
            {
                NPC_agent_list.Add(item);
            }
            else if (agent.getProf(buildVersion, apiController) == "GDG")
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
        public AgentItem GetAgent(long agent) {
            if (agent != 0)
            {
                AgentItem agtreturn = all_agents_list.FirstOrDefault(x => x.getAgent() == agent);
                if (agtreturn != null)
                {
                    return agtreturn;
                }
                
            }
            
            return new AgentItem(0,"UNKOWN","UNKNOWN");
            
        }
        public AgentItem GetAgentWInst(ushort instid)
        {
            return all_agents_list.FirstOrDefault(x => x.getInstid() == instid);
        }
    }
}