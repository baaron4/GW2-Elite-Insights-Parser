using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class WvWFight : FightLogic
    {
        private readonly string _defaultName;
        private readonly bool _detailed;
        public WvWFight(int triggerID, bool detailed) : base(triggerID)
        {
            Mode = ParseMode.WvW;
            Icon = "https://wiki.guildwars2.com/images/3/35/WvW_Rank_up.png";
            _detailed = detailed;
            Extension = _detailed ? "detailed_wvw" : "wvw";
            _defaultName = _detailed ? "Detailed WvW" : "World vs World";
            EncounterCategoryInformation.Category = FightCategory.WvW;
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>();
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.WorldVersusWorld);
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
                phases.Add(new PhaseData(phases[0].Start, phases[0].End)
                {
                    Name = "Detailed Full Fight",
                    CanBeSubPhase = false
                });
                phases[1].AddTargets(Targets);
                if (phases[1].Targets.Any())
                {
                    phases[1].RemoveTarget(mainTarget);
                }
                phases[0].Dummy = true;
            }
            return phases;
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
                    return new CombatReplayMap("https://i.imgur.com/t0khtQd.png", (954, 1000), (-36864, -36864, 36864, 36864)/*, (-36864, -36864, 36864, 36864), (8958, 12798, 12030, 15870)*/);
                // Green Alpine
                case 95:
                    return new CombatReplayMap("https://i.imgur.com/nVu2ivF.png", (697, 1000), (-30720, -43008, 30720, 43008)/*, (-30720, -43008, 30720, 43008), (5630, 11518, 8190, 15102)*/);
                // Blue Alpine
                case 96:
                    return new CombatReplayMap("https://i.imgur.com/nVu2ivF.png", (697, 1000), (-30720, -43008, 30720, 43008)/*, (-30720, -43008, 30720, 43008), (12798, 10878, 15358, 14462)*/);
                // Red Desert
                case 1099:
                    return new CombatReplayMap("https://i.imgur.com/R5p9fqw.png", (1000, 1000), (-36864, -36864, 36864, 36864)/*, (-36864, -36864, 36864, 36864), (9214, 8958, 12286, 12030)*/);
            }
            return base.GetCombatMapInternal(log);
        }
        internal override string GetLogicName(ParsedEvtcLog log)
        {
            MapIDEvent mapID = log.CombatData.GetMapIDEvents().LastOrDefault();
            if (mapID == null)
            {
                return _defaultName;
            }
            switch (mapID.MapID)
            {
                case 38:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.EternalBattlegrounds;
                    return _defaultName + " - Eternal Battlegrounds";
                case 95:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.GreenAlpineBorderlands;
                    return _defaultName + " - Green Alpine Borderlands";
                case 96:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.BlueAlpineBorderlands;
                    return _defaultName + " - Blue Alpine Borderlands";
                case 1099:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.RedDesertBorderlands;
                    return _defaultName + " - Red Desert Borderlands";
                case 899:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.ObsidianSanctum;
                    return _defaultName + " - Obsidian Sanctum";
                case 968:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.EdgeOfTheMists;
                    return _defaultName + " - Edge of the Mists";
                case 1315:
                    EncounterCategoryInformation.SubCategory = SubFightCategory.ArmisticeBastion;
                    return _defaultName + " - Armistice Bastion";
            }
            return _defaultName;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            fightData.SetSuccess(true, fightData.FightEnd);
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, List<AbstractSingleActor> friendlies)
        {
            AgentItem dummyAgent = agentData.AddCustomAgent(fightData.FightStart, fightData.FightEnd, AgentItem.AgentType.NPC, _detailed ? "Dummy WvW Agent" : "Enemy Players", "", (int)ArcDPSEnums.TargetID.WorldVersusWorld, true);
            ComputeFightTargets(agentData, combatData);

            IReadOnlyList<AgentItem> aList = agentData.GetAgentByType(AgentItem.AgentType.NonSquadPlayer);
            if (_detailed)
            {
                var set = new HashSet<string>();
                foreach (AgentItem a in aList)
                {
                    List<AbstractSingleActor> actorListToFill = a.IsNotInSquadFriendlyPlayer ? friendlies : _targets;
                    var nonSquadPlayer = new PlayerNonSquad(a);
                    if (!set.Contains(nonSquadPlayer.Character))
                    {
                        actorListToFill.Add(nonSquadPlayer);
                        set.Add(nonSquadPlayer.Character);
                    }
                    else
                    {
                        // we merge
                        AbstractSingleActor mainNPC = actorListToFill.FirstOrDefault(x => x.Character == nonSquadPlayer.Character);
                        foreach (CombatItem c in combatData)
                        {
                            if (c.IsStateChange.SrcIsAgent() && c.SrcAgent == nonSquadPlayer.Agent)
                            {
                                c.OverrideSrcAgent(mainNPC.Agent);
                            }
                            if (c.IsStateChange.DstIsAgent() && c.DstAgent == nonSquadPlayer.Agent)
                            {
                                c.OverrideDstAgent(mainNPC.Agent);
                            }
                        }
                        agentData.SwapMasters(nonSquadPlayer.AgentItem, mainNPC.AgentItem);
                        mainNPC.AgentItem.OverrideAwareTimes(Math.Min(nonSquadPlayer.FirstAware, mainNPC.FirstAware), Math.Max(nonSquadPlayer.LastAware, mainNPC.LastAware));
                    }
                }
            }
            else
            {
                var enemyPlayerDicts = aList.GroupBy(x => x.Agent).ToDictionary(x => x.Key, x => x.ToList().First());
                foreach (CombatItem c in combatData)
                {
                    if (c.IsStateChange == ArcDPSEnums.StateChange.None &&
                        c.IsActivation == ArcDPSEnums.Activation.None &&
                        c.IsBuffRemove == ArcDPSEnums.BuffRemove.None &&
                        ((c.IsBuff != 0 && c.Value == 0) || (c.IsBuff == 0)))
                    {
                        if (enemyPlayerDicts.TryGetValue(c.SrcAgent, out AgentItem src))
                        {
                            c.OverrideSrcAgent(dummyAgent.Agent);
                        }
                        if (enemyPlayerDicts.TryGetValue(c.DstAgent, out AgentItem dst))
                        {
                            c.OverrideDstAgent(dummyAgent.Agent);
                        }
                    }
                }
            }
        }
    }
}
