using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class AgentData
    {
        private readonly List<AgentItem> _allAgentsList;
        private Dictionary<ulong, AgentItem> _allAgentsByAgent;
        private Dictionary<ushort, List<AgentItem>> _allAgentsByInstID;
        private Dictionary<ushort, List<AgentItem>> _allAgentsByID;
        private Dictionary<AgentItem.AgentType, List<AgentItem>> _allAgentsByType;

        public AgentData(List<AgentItem> allAgentsList)
        {
            _allAgentsList = allAgentsList;
            _allAgentsByAgent = allAgentsList.ToDictionary(a => a.Agent);
            _allAgentsByID = allAgentsList.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByInstID = allAgentsList.GroupBy(x => x.InstID).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByType = allAgentsList.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
        }

        public AgentItem GetAgent(ulong agentAddress)
        {
            if (agentAddress != 0)
            {
                if (_allAgentsByAgent.TryGetValue(agentAddress, out AgentItem a))
                {
                    return a;
                }
            }

            return new AgentItem(0, "UNKNOWN");
        }

        public List<AgentItem> GetAgentsByID(ushort id)
        {
            if (id != 0)
            {
                if (_allAgentsByID.TryGetValue(id, out var list))
                {
                    return list;
                }
            }

            return new List<AgentItem>();
        }

        public List<AgentItem> GetAgentByInstID(ushort instid)
        {
            if (instid != 0)
            {
                if (_allAgentsByInstID.TryGetValue(instid, out var list))
                {
                    return list;
                }
            }

            return new List<AgentItem>();
        }

        public AgentItem GetAgentByInstID(ushort instid, long time)
        {
            if (instid != 0)
            {
                if (_allAgentsByInstID.TryGetValue(instid, out var list))
                {
                    return list.FirstOrDefault(x => x.FirstAware <= time && x.LastAware >= time);
                }
            }
            return new AgentItem(0, "UNKNOWN");
        }
        
        public void OverrideID(ushort ID, ushort instid, AgentItem agentItem)
        {
            _allAgentsList.RemoveAll(x => x.ID == ID && x.InstID == instid);
            _allAgentsList.Add(agentItem);
            _allAgentsByAgent = _allAgentsList.ToDictionary(a => a.Agent);
            _allAgentsByID = _allAgentsList.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByInstID = _allAgentsList.GroupBy(x => x.InstID).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByType = _allAgentsList.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
        }

        public List<AgentItem> GetAgentByType(AgentItem.AgentType type)
        {
            if (_allAgentsByType.TryGetValue(type, out var list))
            {
                return list;
            }
            else
            {
                return new List<AgentItem>();
            }
        }
    }
}