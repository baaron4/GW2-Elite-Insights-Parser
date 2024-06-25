using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class WvWFight : FightLogic
    {
        private readonly string _defaultName;
        private readonly bool _detailed;
        private bool _foundSkillMode { get; set; }
        private bool _isGuildHall { get; set; }
        public WvWFight(int triggerID, bool detailed) : base(triggerID)
        {
            ParseMode = ParseModeEnum.WvW;
            SkillMode = SkillModeEnum.WvW;
            Icon = EncounterIconWvW;
            _detailed = detailed;
            Extension = _detailed ? "detailed_wvw" : "wvw";
            _defaultName = _detailed ? "Detailed WvW" : "World vs World";
            EncounterCategoryInformation.Category = FightCategory.WvW;
            EncounterID |= EncounterIDs.EncounterMasks.WvWMask;
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDamageMechanic("Killing Blows to enemy Players", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Blue), "Kllng.Blw.Player", "Killing Blows inflicted by Squad Players to enemy Players", "Killing Blows to enemy Players", 0, (log, a) => {
                    if (a.Type != AgentItem.AgentType.Player)
                    {
                        return new List<AbstractHealthDamageEvent>();
                    }
                    return log.FindActor(a).GetDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd);
                }).UsingChecker((x, log) => x.HasKilled && (x.To.Type == AgentItem.AgentType.NonSquadPlayer || x.To.IsSpecies(ArcDPSEnums.TargetID.WorldVersusWorld))),
                new EnemyDamageMechanic("Killing Blows received by enemies", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Red), "Kllng.Blw.Enemy", "Killing Blows inflicted enemy Players by Squad Players", "Killing Blows received by enemies", 0, (log, a) => {
                    return log.FindActor(a).GetDamageTakenEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd);
                }).UsingChecker((x, log) => x.HasKilled && x.CreditedFrom.Type == AgentItem.AgentType.Player),
            });
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>();
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.WorldVersusWorld)) ?? throw new MissingKeyActorsException("Main target of the fight not found");
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

        internal override void UpdatePlayersSpecAndGroup(IReadOnlyList<Player> players, CombatData combatData, FightData fightData)
        {
            foreach (Player p in players)
            {
                // We get the first enter combat for the player, we ignore it however if there was an exit combat before it as that means the player was already in combat at log start
                var enterCombat = combatData.GetEnterCombatEvents(p.AgentItem).FirstOrDefault();
                if (enterCombat != null && enterCombat.Spec != ParserHelper.Spec.Unknown && !combatData.GetExitCombatEvents(p.AgentItem).Any(x => x.Time < enterCombat.Time))
                {
                    p.AgentItem.OverrideSpec(enterCombat.Spec);
                    p.OverrideGroup(enterCombat.Subgroup);
                }
            }
        }

        internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
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
                    return new CombatReplayMap(CombatReplayEternalBattlegrounds, (954, 1000), (-36864, -36864, 36864, 36864));
                // Green Alpine
                case 95:
                    return new CombatReplayMap(CombatReplayAlpineBorderlands, (697, 1000), (-30720, -43008, 30720, 43008));
                // Blue Alpine
                case 96:
                    return new CombatReplayMap(CombatReplayAlpineBorderlands, (697, 1000), (-30720, -43008, 30720, 43008));
                // Red Desert
                case 1099:
                    return new CombatReplayMap(CombatReplayDesertBorderlands, (1000, 1000), (-36864, -36864, 36864, 36864));
                case 968:
                    return new CombatReplayMap(CombatReplayEdgeOfTheMists, (3556, 3646), (-36864, -36864, 36864, 36864));
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
                case 1068:
                case 1101:
                case 1107:
                case 1108:
                case 1121:
                    _isGuildHall = true;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.GuildHall;
                    EncounterID |= EncounterIDs.WvWMasks.GildedHollowMask;
                    Extension = _detailed ? "detailed_gh" : "gh";
                    if (!_foundSkillMode)
                    {
                        SkillMode = SkillModeEnum.PvE;
                    }
                    //Icon = InstanceIconEternalBattlegrounds;
                    return (_detailed ? "Detailed " : "") + "Gilded Hollow";
                case 1069:
                case 1071:
                case 1076:
                case 1104:
                case 1124:
                    _isGuildHall = true;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.GuildHall;
                    EncounterID |= EncounterIDs.WvWMasks.LostPrecipiceMask;
                    Extension = _detailed ? "detailed_gh" : "gh";
                    if (!_foundSkillMode)
                    {
                        SkillMode = SkillModeEnum.PvE;
                    }
                    //Icon = InstanceIconEternalBattlegrounds;
                    return (_detailed ? "Detailed " : "") + "Lost Precipice";
                case 1214:
                case 1215:
                case 1224:
                case 1232:
                case 1243:
                    _isGuildHall = true;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.GuildHall;
                    EncounterID |= EncounterIDs.WvWMasks.WindsweptHavenMask;
                    Extension = _detailed ? "detailed_gh" : "gh";
                    if (!_foundSkillMode)
                    {
                        SkillMode = SkillModeEnum.PvE;
                    }
                    //Icon = InstanceIconEternalBattlegrounds;
                    return (_detailed ? "Detailed " : "") + "Windswept Haven";
                case 1419:
                case 1426:
                case 1435:
                case 1444:
                case 1462:
                    _isGuildHall = true;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.GuildHall;
                    EncounterID |= EncounterIDs.WvWMasks.IsleOfReflectionMask;
                    Extension = _detailed ? "detailed_gh" : "gh";
                    if (!_foundSkillMode)
                    {
                        SkillMode = SkillModeEnum.PvE;
                    }
                    //Icon = InstanceIconEternalBattlegrounds;
                    return (_detailed ? "Detailed " : "") + "Isle of Reflection";
            }
            return _defaultName;
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);
            if (_isGuildHall)
            {
                var modes = new List<AbstractBuffEvent>(log.CombatData.GetBuffData(GuildHallPvEMode));
                modes.AddRange(log.CombatData.GetBuffData(GuildHallsPvPMode));
                modes.AddRange(log.CombatData.GetBuffData(GuildHallWvWMode));
                var usedModes = modes.OrderBy(x => x.Time).Select(x => x.BuffID).Distinct().ToList();
                foreach (long buffID in usedModes)
                {
                    InstanceBuffs.Add((log.Buffs.BuffsByIds[buffID], 1));
                }
                // When buff is missing on a player, they are in PvE mode
                if (!usedModes.Contains(GuildHallPvEMode) && log.PlayerList.Any(x => !modes.Any(y => y.To == x.AgentItem)))
                {
                    InstanceBuffs.Add((log.Buffs.BuffsByIds[GuildHallPvEMode], 1));
                }
            }
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            fightData.SetSuccess(true, fightData.FightEnd);
        }

        internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            AgentItem dummyAgent = agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, _detailed ? "Dummy PvP Agent" : "Enemy Players", ParserHelper.Spec.NPC, ArcDPSEnums.TargetID.WorldVersusWorld, true);
            // Handle non squad players
            IReadOnlyList<AgentItem> aList = agentData.GetAgentByType(AgentItem.AgentType.NonSquadPlayer);
            //
            var garbageList = new List<AbstractSingleActor>();
            var auxTargets = new List<AbstractSingleActor>();
            var auxFriendlies = new List<AbstractSingleActor>();
            foreach (AgentItem a in aList)
            {
                var nonSquadPlayer = new PlayerNonSquad(a);
                List<AbstractSingleActor> actorListToFill = nonSquadPlayer.IsFriendlyPlayer ? auxFriendlies : _detailed ? auxTargets : garbageList;
                actorListToFill.Add(nonSquadPlayer);
            }
            //
            if (!_detailed)
            {
                var friendlyAgents = new HashSet<AgentItem>(NonPlayerFriendlies.Select(x => x.AgentItem));
                var enemyPlayerList = aList.Where(x => !friendlyAgents.Contains(x)).ToList();
                var enemyPlayerDicts = enemyPlayerList.GroupBy(x => x.Agent).ToDictionary(x => x.Key, x => x.ToList());
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
            CombatItem modeEvent = combatData.FirstOrDefault(x => (x.IsBuffApply() || x.IsBuffRemoval()) && (x.SkillID == GuildHallPvEMode || x.SkillID == GuildHallsPvPMode || x.SkillID == GuildHallWvWMode));
            if (modeEvent != null)
            {
                _foundSkillMode = true;
                switch ((long)modeEvent.SkillID)
                {
                    case GuildHallPvEMode:
                        SkillMode = SkillModeEnum.PvE;
                        break;
                    case GuildHallsPvPMode:
                        SkillMode = SkillModeEnum.sPvP;
                        break;
                    case GuildHallWvWMode:
                        SkillMode = SkillModeEnum.WvW;
                        break;
                }
            }
            ComputeFightTargets(agentData, combatData, extensions);
            auxFriendlies = auxFriendlies.OrderBy(x => x.Character).ToList();
            _nonPlayerFriendlies.AddRange(auxFriendlies);
            auxTargets = auxTargets.OrderBy(x => x.Character).ToList();
            _targets.AddRange(auxTargets);
        }
    }
}
