using System;
using System.Collections.Generic;
using System.Linq;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class AgentData
    {
#if DEBUG
        public readonly List<AgentItem> _allAgentsList;
#else
        private readonly List<AgentItem> _allAgentsList;
#endif
        private Dictionary<ulong, List<AgentItem>> _allAgentsByAgent;
        private Dictionary<ushort, List<AgentItem>> _allAgentsByInstID;
        private Dictionary<int, List<AgentItem>> _allNPCsByID;
        private Dictionary<int, List<AgentItem>> _allGadgetsByID;
        private Dictionary<AgentItem.AgentType, List<AgentItem>> _allAgentsByType;
#if DEBUG
        private Dictionary<string, List<AgentItem>> _allAgentsByName;
#endif
        public IReadOnlyCollection<ulong> AgentValues => new HashSet<ulong>(_allAgentsList.Select(x => x.Agent));
        public IReadOnlyCollection<ushort> InstIDValues => new HashSet<ushort>(_allAgentsList.Select(x => x.InstID));


        private readonly GW2EIGW2API.GW2APIController _apiController;

        internal AgentData(GW2EIGW2API.GW2APIController apiController, List<AgentItem> allAgentsList)
        {
            _apiController = apiController;
            _allAgentsList = allAgentsList;
            Refresh();
        }
        internal string GetSpec(uint prof, uint elite)
        {
            return _apiController.GetSpec(prof, elite);
        }

        internal AgentItem AddCustomNPCAgent(long start, long end, string name, ParserHelper.Spec spec, int ID, bool isFake, ushort toughness = 0, ushort healing = 0, ushort condition = 0, ushort concentration = 0, uint hitboxWidth = 0, uint hitboxHeight = 0)
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
            var agent = new AgentItem(agentValue, name, spec, ID, instID, toughness, healing, condition, concentration, hitboxWidth, hitboxHeight, start, end, isFake);
            _allAgentsList.Add(agent);
            Refresh();
            return agent;
        }

        internal AgentItem AddCustomNPCAgent(long start, long end, string name, ParserHelper.Spec spec, ArcDPSEnums.TrashID ID, bool isFake, ushort toughness = 0, ushort healing = 0, ushort condition = 0, ushort concentration = 0, uint hitboxWidth = 0, uint hitboxHeight = 0)
        {
            return AddCustomNPCAgent(start, end, name, spec, (int)ID, isFake, toughness, healing, condition, concentration, hitboxWidth, hitboxHeight);
        }

        internal AgentItem AddCustomNPCAgent(long start, long end, string name, ParserHelper.Spec spec, ArcDPSEnums.TargetID ID, bool isFake, ushort toughness = 0, ushort healing = 0, ushort condition = 0, ushort concentration = 0, uint hitboxWidth = 0, uint hitboxHeight = 0)
        {
            return AddCustomNPCAgent(start, end, name, spec, (int)ID, isFake, toughness, healing, condition, concentration, hitboxWidth, hitboxHeight);
        }

        public AgentItem GetAgent(ulong agentAddress, long time)
        {
            if (agentAddress != 0)
            {
                if (_allAgentsByAgent.TryGetValue(agentAddress, out List<AgentItem> agents))
                {
                    foreach (AgentItem a in agents)
                    {
                        if (a.InAwareTimes(time))
                        {
                            return a;
                        }
                    }
                }
            }
            return ParserHelper._unknownAgent;
        }

        public IReadOnlyList<AgentItem> GetNPCsByID(int id)
        {
            if (_allNPCsByID.TryGetValue(id, out List<AgentItem> list))
            {
                return list;
            }
            return new List<AgentItem>();
        }
        public IReadOnlyList<AgentItem> GetNPCsByIDAndAgent(int id, ulong agent)
        {
            if (agent == 0)
            {
                return GetNPCsByID(id);
            }
            return GetNPCsByID(id).Where(x => x.Agent == agent).ToList();
        }

        public IReadOnlyList<AgentItem> GetNPCsByID(ArcDPSEnums.TrashID id)
        {
            return GetNPCsByID((int)id);
        }
        public IReadOnlyList<AgentItem> GetNPCsByIDAndAgent(ArcDPSEnums.TrashID id, ulong agent)
        {
            return GetNPCsByIDAndAgent((int)id, agent);
        }

        public IReadOnlyList<AgentItem> GetNPCsByID(ArcDPSEnums.TargetID id)
        {
            return GetNPCsByID((int)id);
        }
        public IReadOnlyList<AgentItem> GetNPCsByIDAndAgent(ArcDPSEnums.TargetID id, ulong agent)
        {
            return GetNPCsByIDAndAgent((int)id, agent);
        }

        public IReadOnlyList<AgentItem> GetNPCsByID(ArcDPSEnums.MinionID id)
        {
            return GetNPCsByID((int)id);
        }
        public IReadOnlyList<AgentItem> GetNPCsByIDAndAgent(ArcDPSEnums.MinionID id, ulong agent)
        {
            return GetNPCsByIDAndAgent((int)id, agent);
        }


        public IReadOnlyList<AgentItem> GetGadgetsByID(int id)
        {
            if (_allGadgetsByID.TryGetValue(id, out List<AgentItem> list))
            {
                return list;
            }
            return new List<AgentItem>();
        }

        public IReadOnlyList<AgentItem> GetGadgetsByID(ArcDPSEnums.TrashID id)
        {
            return GetGadgetsByID((int)id);
        }

        public IReadOnlyList<AgentItem> GetGadgetsByID(ArcDPSEnums.TargetID id)
        {
            return GetGadgetsByID((int)id);
        }
        public IReadOnlyList<AgentItem> GetGadgetsByID(ArcDPSEnums.MinionID id)
        {
            return GetGadgetsByID((int)id);
        }

        public IReadOnlyList<AgentItem> GetGadgetsByID(ArcDPSEnums.ChestID id)
        {
            return GetGadgetsByID((int)id);
        }

        public AgentItem GetAgentByInstID(ushort instid, long time)
        {
            if (instid != 0)
            {
                if (_allAgentsByInstID.TryGetValue(instid, out List<AgentItem> agents))
                {
                    foreach (AgentItem a in agents)
                    {
                        if (a.InAwareTimes(time))
                        {
                            return a;
                        }
                    }
                }
            }
            return ParserHelper._unknownAgent;
        }

        public bool HasSpawnedMinion(MinionID minion, AgentItem master, long time, long epsilon = ParserHelper.ServerDelayConstant)
        {
            return GetNPCsByID(minion)
                .Any(agent => agent.GetFinalMaster() == master && Math.Abs(agent.FirstAware - time) < epsilon);
        }

        internal void ReplaceAgentsFromID(AgentItem agentItem)
        {
            if (agentItem.ID == 0)
            {
                return;
            }
            _allAgentsList.RemoveAll(x => x.ID == agentItem.ID);
            _allAgentsList.Add(agentItem);
            Refresh();
        }

        internal void RemoveAllFrom(HashSet<AgentItem> agents)
        {
            if (agents.Count == 0)
            {
                return;
            }
            _allAgentsList.RemoveAll(x => agents.Contains(x));

            Refresh();
        }

        internal void Refresh()
        {
            _allAgentsByAgent = _allAgentsList.GroupBy(x => x.Agent).ToDictionary(x => x.Key, x => x.ToList());
            _allNPCsByID = _allAgentsList.Where(x => x.Type == AgentItem.AgentType.NPC).GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
            _allGadgetsByID = _allAgentsList.Where(x => x.Type == AgentItem.AgentType.Gadget).GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByInstID = _allAgentsList.GroupBy(x => x.InstID).ToDictionary(x => x.Key, x => x.ToList());
            _allAgentsByType = _allAgentsList.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
#if DEBUG
            _allAgentsByName = _allAgentsList.Where(x => !x.Name.Contains("UNKNOWN")).GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList());
#endif
        }

        public IReadOnlyList<AgentItem> GetAgentByType(AgentItem.AgentType type)
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

        internal void SwapMasters(HashSet<AgentItem> froms, AgentItem to)
        {
            foreach (AgentItem a in GetAgentByType(AgentItem.AgentType.NPC))
            {
                if (a.Master != null && froms.Contains(a.Master))
                {
                    a.SetMaster(to);
                }
            }
            foreach (AgentItem a in GetAgentByType(AgentItem.AgentType.Gadget))
            {
                if (a.Master != null && froms.Contains(a.Master))
                {
                    a.SetMaster(to);
                }
            }
        }

        internal void SwapMasters(AgentItem from, AgentItem to)
        {
            SwapMasters(new HashSet<AgentItem> { from }, to);
        }

        /// <summary>
        /// Tries to retrieve the first <see cref="AgentItem"/> corresponding to the provided <see cref="TargetID"/>.
        /// </summary>
        /// <param name="targetID">The ID of the target to search for.</param>
        /// <param name="agentItem">The <see cref="AgentItem"/> found, if any.</param>
        /// <returns><see langword="true"/> if an <see cref="AgentItem"/> was found for the given  <see cref="TargetID"/>; otherwise,  <see langword="false"/>.</returns>
        public bool TryGetFirstAgentItem(TargetID targetID, out AgentItem agentItem)
        {
            return TryGetFirstAgentItem((int)targetID, out agentItem);
        }

        /// <summary>
        /// Tries to retrieve the first <see cref="AgentItem"/> corresponding to the provided <see cref="TrashID"/>.
        /// </summary>
        /// <param name="trashID">The ID of the trash to search for.</param>
        /// <param name="agentItem">The <see cref="AgentItem"/> found, if any.</param>
        /// <returns><see langword="true"/> if an <see cref="AgentItem"/> was found for the given <see cref="TrashID"/>; otherwise,  <see langword="false"/>.</returns>
        public bool TryGetFirstAgentItem(TrashID trashID, out AgentItem agentItem)
        {
            return TryGetFirstAgentItem((int)trashID, out agentItem);
        }

        /// <summary>
        /// Tries to retrieve the first <see cref="AgentItem"/> corresponding to the provided <see cref="MinionID"/>.
        /// </summary>
        /// <param name="minionID">The ID of the minion to search for.</param>
        /// <param name="agentItem">The <see cref="AgentItem"/> found, if any.</param>
        /// <returns><see langword="true"/> if an <see cref="AgentItem"/> was found for the given <see cref="MinionID"/>; otherwise,  <see langword="false"/>.</returns>
        public bool TryGetFirstAgentItem(MinionID minionID, out AgentItem agentItem)
        {
            return TryGetFirstAgentItem((int)minionID, out agentItem);
        }

        /// <summary>
        /// Tries to retrieve the first <see cref="AgentItem"/> corresponding to the provided <see cref="ChestID"/>.
        /// </summary>
        /// <param name="chestID">The ID of the chest to search for.</param>
        /// <param name="agentItem">The <see cref="AgentItem"/> found, if any.</param>
        /// <returns><see langword="true"/> if an <see cref="AgentItem"/> was found for the given <see cref="ChestID"/>; otherwise,  <see langword="false"/>.</returns>
        public bool TryGetFirstAgentItem(ChestID chestID, out AgentItem agentItem)
        {
            return TryGetFirstAgentItem((int)chestID, out agentItem);
        }

        /// <summary>
        /// Tries to retrieve the first <see cref="AgentItem"/> corresponding to the provided <paramref name="agentId"/>.<br></br>
        /// </summary>
        /// <param name="agentId">The ID of the agent to search for.</param>
        /// <param name="agentItem">The <see cref="AgentItem"/> found, if any.</param>
        /// <returns><see langword="true"/> if an <see cref="AgentItem"/> was found for the given <paramref name="agentId"/>; otherwise, <see langword="false"/>.</returns>
        public bool TryGetFirstAgentItem(int agentId, out AgentItem agentItem)
        {
            agentItem = GetNPCsByID(agentId).FirstOrDefault();
            if (agentItem != null)
            {
                return true;
            }
            return false;
        }
    }
}
