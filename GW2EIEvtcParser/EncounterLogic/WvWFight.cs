using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class WvWFight : FightLogic
    {
        private readonly string _defaultName;
        private readonly bool _detailed;
        public WvWFight(int triggerID, bool detailed) : base(triggerID)
        {
            Mode = ParseMode.WvW;
            Icon = EncounterIconWvW;
            _detailed = detailed;
            Extension = _detailed ? "detailed_wvw" : "wvw";
            _defaultName = _detailed ? "Detailed WvW" : "World vs World";
            EncounterCategoryInformation.Category = FightCategory.WvW;
            EncounterID |= EncounterIDs.EncounterMasks.WvWMask;
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>();
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.WorldVersusWorld));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            if (_detailed)
            {
                var detailedPhase = new PhaseData(phases[0].Start, phases[0].End)
                {
                    Name = "Detailed Full Fight",
                    CanBeSubPhase = false
                };
                detailedPhase.AddTargets(Targets);
                if (detailedPhase.Targets.Any())
                {
                    detailedPhase.RemoveTarget(mainTarget);
                }
                if (detailedPhase.Targets.Any())
                {
                    phases[0] = detailedPhase;
                }
            }
            return phases;
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return GetGenericFightOffset(fightData);
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            MapIDEvent mapID = log.CombatData.GetMapIDEvents().LastOrDefault();
            if (mapID == null)
            {
                return base.GetCombatMapInternal(log);
            }
            switch (mapID.MapID)
            {
                // EB
                case 38:
                    return new CombatReplayMap(CombatReplayEternalBattlegrounds, (954, 1000), (-36864, -36864, 36864, 36864)/*, (-36864, -36864, 36864, 36864), (8958, 12798, 12030, 15870)*/);
                // Green Alpine
                case 95:
                    return new CombatReplayMap(CombatReplayAlpineBorderlands, (697, 1000), (-30720, -43008, 30720, 43008)/*, (-30720, -43008, 30720, 43008), (5630, 11518, 8190, 15102)*/);
                // Blue Alpine
                case 96:
                    return new CombatReplayMap(CombatReplayAlpineBorderlands, (697, 1000), (-30720, -43008, 30720, 43008)/*, (-30720, -43008, 30720, 43008), (12798, 10878, 15358, 14462)*/);
                // Red Desert
                case 1099:
                    return new CombatReplayMap(CombatReplayDesertBorderlands, (1000, 1000), (-36864, -36864, 36864, 36864)/*, (-36864, -36864, 36864, 36864), (9214, 8958, 12286, 12030)*/);
                case 968:
                    return new CombatReplayMap(CombatReplayEdgeOfTheMists, (3556, 3646), (-36864, -36864, 36864, 36864)/*, (-36864, -36864, 36864, 36864), (9214, 8958, 12286, 12030)*/);
            }
            return base.GetCombatMapInternal(log);
        }
        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            MapIDEvent mapID = combatData.GetMapIDEvents().LastOrDefault();
            if (mapID == null)
            {
                return _defaultName;
            }
            switch (mapID.MapID)
            {
                case 38:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.EternalBattlegrounds;
                    EncounterID |= EncounterIDs.WvWMasks.EternalBattlegroundsMask;
                    Icon = InstanceIconEternalBattlegrounds;
                    return _defaultName + " - Eternal Battlegrounds";
                case 95:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.GreenAlpineBorderlands;
                    EncounterID |= EncounterIDs.WvWMasks.GreenAlpineBorderlandsMask;
                    Icon = InstanceIconGreenBorderlands;
                    return _defaultName + " - Green Alpine Borderlands";
                case 96:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.BlueAlpineBorderlands;
                    EncounterID |= EncounterIDs.WvWMasks.BlueAlpineBorderlandsMask;
                    Icon = InstanceIconBlueBorderlands;
                    return _defaultName + " - Blue Alpine Borderlands";
                case 1099:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.RedDesertBorderlands;
                    EncounterID |= EncounterIDs.WvWMasks.RedDesertBorderlandsMask;
                    Icon = InstanceIconRedBorderlands;
                    return _defaultName + " - Red Desert Borderlands";
                case 899:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.ObsidianSanctum;
                    EncounterID |= EncounterIDs.WvWMasks.ObsidianSanctumMask;
                    Icon = InstanceIconEternalBattlegrounds;
                    return _defaultName + " - Obsidian Sanctum";
                case 968:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.EdgeOfTheMists;
                    EncounterID |= EncounterIDs.WvWMasks.EdgeOfTheMistsMask;
                    return _defaultName + " - Edge of the Mists";
                case 1315:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.ArmisticeBastion;
                    EncounterID |= EncounterIDs.WvWMasks.ArmisticeBastionMask;
                    Icon = InstanceIconEternalBattlegrounds;
                    return _defaultName + " - Armistice Bastion";
            }
            return _defaultName;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            fightData.SetSuccess(true, fightData.FightEnd);
        }

        private void SolveWvWPlayers(AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            IReadOnlyList<AgentItem> aList = agentData.GetAgentByType(AgentItem.AgentType.NonSquadPlayer);
            var set = new HashSet<string>();
            var toRemove = new HashSet<AgentItem>();
            var garbageList = new List<AbstractSingleActor>();
            var teamChangeDict = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.TeamChange).GroupBy(x => x.SrcAgent).ToDictionary(x => x.Key, x => x.ToList());
            //
            IReadOnlyList<AgentItem> squadPlayers = agentData.GetAgentByType(AgentItem.AgentType.Player);
            ulong greenTeam = ulong.MaxValue;
            var greenTeams = new List<ulong>();
            foreach (AgentItem a in squadPlayers)
            {
                if (teamChangeDict.TryGetValue(a.Agent, out List<CombatItem> teamChangeList))
                {
                    greenTeams.AddRange(teamChangeList.Where(x => x.SrcMatchesAgent(a)).Select(x => x.DstAgent));
                }
            }
            if (greenTeams.Any())
            {
                greenTeam = greenTeams.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key).First();
            }
            var playersToMerge = new Dictionary<PlayerNonSquad, AbstractSingleActor>();
            var agentsToPlayersToMerge = new Dictionary<ulong, PlayerNonSquad>();
            //
            foreach (AgentItem a in aList)
            {
                if (teamChangeDict.TryGetValue(a.Agent, out List<CombatItem> teamChangeList))
                {
                    a.OverrideIsNotInSquadFriendlyPlayer(teamChangeList.Where(x => x.SrcMatchesAgent(a)).Select(x => x.DstAgent).Any(x => x == greenTeam));
                }
                List<AbstractSingleActor> actorListToFill = a.IsNotInSquadFriendlyPlayer ? _nonPlayerFriendlies : _detailed ? _targets : garbageList;
                var nonSquadPlayer = new PlayerNonSquad(a);
                if (!set.Contains(nonSquadPlayer.Character))
                {
                    actorListToFill.Add(nonSquadPlayer);
                    set.Add(nonSquadPlayer.Character);
                }
                else
                {
                    // we merge
                    AbstractSingleActor mainPlayer = actorListToFill.FirstOrDefault(x => x.Character == nonSquadPlayer.Character);
                    playersToMerge[nonSquadPlayer] = mainPlayer;
                    agentsToPlayersToMerge[nonSquadPlayer.AgentItem.Agent] = nonSquadPlayer;
                }
            }
            if (playersToMerge.Any())
            {
                foreach (CombatItem c in combatData)
                {
                    if (agentsToPlayersToMerge.TryGetValue(c.SrcAgent, out PlayerNonSquad nonSquadPlayer) && c.SrcMatchesAgent(nonSquadPlayer.AgentItem, extensions))
                    {
                        AbstractSingleActor mainPlayer = playersToMerge[nonSquadPlayer];
                        c.OverrideSrcAgent(mainPlayer.AgentItem.Agent);
                    }
                    if (agentsToPlayersToMerge.TryGetValue(c.DstAgent, out nonSquadPlayer) && c.DstMatchesAgent(nonSquadPlayer.AgentItem, extensions))
                    {
                        AbstractSingleActor mainPlayer = playersToMerge[nonSquadPlayer];
                        c.OverrideDstAgent(mainPlayer.AgentItem.Agent);
                    }
                }
                foreach (KeyValuePair<PlayerNonSquad, AbstractSingleActor> pair in playersToMerge)
                {
                    PlayerNonSquad nonSquadPlayer = pair.Key;
                    AbstractSingleActor mainPlayer = pair.Value;
                    agentData.SwapMasters(nonSquadPlayer.AgentItem, mainPlayer.AgentItem);
                    mainPlayer.AgentItem.OverrideAwareTimes(Math.Min(nonSquadPlayer.FirstAware, mainPlayer.FirstAware), Math.Max(nonSquadPlayer.LastAware, mainPlayer.LastAware));
                    toRemove.Add(nonSquadPlayer.AgentItem);
                }
            }
            agentData.RemoveAllFrom(toRemove);
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            AgentItem dummyAgent = agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, _detailed ? "Dummy WvW Agent" : "Enemy Players", ParserHelper.Spec.NPC, ArcDPSEnums.TargetID.WorldVersusWorld, true);

            SolveWvWPlayers(agentData, combatData, extensions);
            if (!_detailed)
            {
                var friendlyAgents = new HashSet<AgentItem>(NonPlayerFriendlies.Select(x => x.AgentItem));
                var aList = agentData.GetAgentByType(AgentItem.AgentType.NonSquadPlayer).Where(x => !friendlyAgents.Contains(x)).ToList();
                var enemyPlayerDicts = aList.GroupBy(x => x.Agent).ToDictionary(x => x.Key, x => x.ToList());
                foreach (CombatItem c in combatData)
                {
                    if (c.IsDamage(extensions))
                    {
                        if (enemyPlayerDicts.TryGetValue(c.SrcAgent, out List<AgentItem> srcs))
                        {
                            foreach (AgentItem src in srcs)
                            {
                                if (c.SrcMatchesAgent(src, extensions))
                                {
                                    c.OverrideSrcAgent(dummyAgent.Agent);
                                    break;
                                }
                            }
                        }
                        if (enemyPlayerDicts.TryGetValue(c.DstAgent, out List<AgentItem> dsts))
                        {
                            foreach (AgentItem dst in dsts)
                            {
                                if (c.DstMatchesAgent(dst, extensions))
                                {
                                    c.OverrideDstAgent(dummyAgent.Agent);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            ComputeFightTargets(agentData, combatData, extensions);
        }
    }
}
