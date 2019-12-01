using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIParser.Parser.ParsedData
{
    public class AgentData
    {
        private readonly List<AgentItem> _allAgentsList;
        private Dictionary<ulong, AgentItem> _allAgentsByAgent;
        private Dictionary<ushort, List<AgentItem>> _allAgentsByInstID;
        private Dictionary<ushort, List<AgentItem>> _allNPCsByID;
        private Dictionary<ushort, List<AgentItem>> _allGadgetsByID;
        private Dictionary<AgentItem.AgentType, List<AgentItem>> _allAgentsByType;
        private Dictionary<string, List<AgentItem>> _allAgentsByName;
        public HashSet<ulong> AgentValues => new HashSet<ulong>(_allAgentsList.Select(x => x.Agent));
        public HashSet<ushort> InstIDValues => new HashSet<ushort>(_allAgentsList.Select(x => x.InstID));

        public AgentData(List<AgentItem> allAgentsList)
        {
            _allAgentsList = allAgentsList;
            Refresh();
        }

        public AgentItem AddCustomAgent(long start, long end, AgentItem.AgentType type, string name, string prof, ushort ID, uint toughness = 0, uint healing = 0, uint condition = 0, uint concentration = 0, uint hitboxWidth = 0, uint hitboxHeight = 0)
        {
            var rnd = new Random();
            ulong agentValue = 0;
            while (AgentValues.Contains(agentValue) || agentValue == 0)
            {
                agentValue = (ulong)rnd.Next(int.MaxValue / 2, int.MaxValue);
            }
            ushort instID = 0;
            while (InstIDValues.Contains(instID) || instID == 0)
            {
                instID = (ushort)rnd.Next(ushort.MaxValue / 2, ushort.MaxValue);
            }
            var agent = new AgentItem(agentValue, name, prof, ID, type, toughness, healing, condition, concentration, hitboxWidth, hitboxHeight)
            {
                InstID = instID,
                LastAware = end,
                FirstAware = start,
                Master = null
            };
            _allAgentsList.Add(agent);
            Refresh();
            return agent;
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
            return GeneralHelper.UnknownAgent;
        }

        public List<AgentItem> GetNPCsByID(ushort id)
        {
            if (id != 0)
            {
                if (_allNPCsByID.TryGetValue(id, out List<AgentItem> list))
                {
                    return list;
                }
            }
            return new List<AgentItem>();
        }

        public List<AgentItem> GetGadgetsByID(ushort id)
        {
            if (id != 0)
            {
                if (_allGadgetsByID.TryGetValue(id, out List<AgentItem> list))
                {
                    return list;
                }
            }
            return new List<AgentItem>();
        }

        public AgentItem GetAgentByInstID(ushort instid, long logTime)
        {
            if (instid != 0)
            {
                if (_allAgentsByInstID.TryGetValue(instid, out List<AgentItem> list))
                {
                    AgentItem a = list.FirstOrDefault(x => x.FirstAware <= logTime && x.LastAware >= logTime);
                    if (a != null)
                    {
                        return a;
                    }
                    return GeneralHelper.UnknownAgent;
                }
            }
            return GeneralHelper.UnknownAgent;
        }

        public void OverrideID(ushort ID, AgentItem agentItem)
        {
            _allAgentsList.RemoveAll(x => x.ID == ID);
            _allAgentsList.Add(agentItem);
            Refresh();
        }

        public void Refresh()
        {
            _allAgentsByAgent = _allAgentsList.GroupBy(x => x.Agent).ToDictionary(x => x.Key, x => x.ToList().First());
            _allNPCsByID = _allAgentsList.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.Where(y => y.Type == AgentItem.AgentType.NPC).ToList());
            _allGadgetsByID = _allAgentsList.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.Where(y => y.Type == AgentItem.AgentType.Gadget).ToList());
            _allAgentsByInstID = _allAgentsList.GroupBy(x => x.InstID).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByType = _allAgentsList.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByName = _allAgentsList.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList());
        }

        public List<AgentItem> GetAgentByType(AgentItem.AgentType type)
        {
            if (_allAgentsByType.TryGetValue(type, out List<AgentItem> list))
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
