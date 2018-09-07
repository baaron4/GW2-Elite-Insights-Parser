using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class AgentData
    {
        public List<AgentItem> PlayerAgentList { get; } = new List<AgentItem>();
        public List<AgentItem> NPCAgentList { get; } = new List<AgentItem>();
        public List<AgentItem> GadgetAgentList { get; } = new List<AgentItem>();
        public List<AgentItem> AllAgentsList { get; } = new List<AgentItem>();

        public void AddItem(AgentItem item, string prof)
        {
            if (prof == "NPC")
            {
                NPCAgentList.Add(item);
            }
            else if (prof == "GDG")
            {
                GadgetAgentList.Add(item);
            }
            else
            {
                PlayerAgentList.Add(item);
            }
            AllAgentsList.Add(item);
        }

        public AgentItem GetAgent(ulong agentAddress)
        {
            if (agentAddress != 0)
            {
                AgentItem agent = AllAgentsList.FirstOrDefault(x => x.Agent == agentAddress);
                if (agent != null)
                {
                    return agent;
                }
            }

            return new AgentItem(0, "UNKNOWN", "UNKNOWN", 0, 0, 0, 0, 0, 0);
        }

        public List<AgentItem> GetAgents(ushort id)
        {
            return AllAgentsList.Where(x => x.ID == id).ToList();
        }

        public AgentItem GetAgentWInst(ushort instid)
        {
            return AllAgentsList.FirstOrDefault(x => x.InstID == instid);
        }

        public void Clean()
        {
            GadgetAgentList.RemoveAll(x => !(x.InstID != 0 && x.LastAware - x.FirstAware >= 0 && x.FirstAware != 0 && x.LastAware != long.MaxValue));
            NPCAgentList.RemoveAll(x => !(x.InstID != 0 && x.LastAware - x.FirstAware >= 0 && x.FirstAware != 0 && x.LastAware != long.MaxValue));
            AllAgentsList.RemoveAll(x => !(x.InstID != 0 && x.LastAware - x.FirstAware >= 0 && x.FirstAware != 0 && x.LastAware != long.MaxValue));
        }

        public void CleanInstid(ushort instid)
        {
            List<AgentItem> instances = NPCAgentList.Where(x => x.InstID == instid).ToList();
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
                AllAgentsList.Remove(a);
                NPCAgentList.Remove(a);
            }
            toAdd.InstID = inst;
            toAdd.FirstAware = firstAware;
            toAdd.LastAware = lastAware;
            AllAgentsList.Add(toAdd);
            NPCAgentList.Add(toAdd);
        }
    }
}