using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Instance : FightLogic
    {
        public bool StartedLate { get; private set; }
        public bool EndedBeforeExpectedEnd { get; private set; }
        private readonly List<FightLogic> _subLogics = new List<FightLogic>();
        private readonly List<int> _targetIDs = new List<int>();
        public Instance(int id) : base(id)
        {
            Extension = "instance";
            ParseMode = ParseModeEnum.FullInstance;
            SkillMode = SkillModeEnum.PvE;
            Icon = InstanceIconGeneric;
            EncounterCategoryInformation.Category = FightCategory.UnknownEncounter;
            EncounterCategoryInformation.SubCategory = SubFightCategory.UnknownEncounter;
        }

        private void FillSubLogics(AgentData agentData)
        {
            var allTargetIDs = Enum.GetValues(typeof(ArcDPSEnums.TargetID)).Cast<int>().ToList();
            var blackList = new HashSet<int>()
            {
                (int) ArcDPSEnums.TargetID.Artsariiv,
                (int) ArcDPSEnums.TargetID.Deimos,
                (int) ArcDPSEnums.TargetID.ConjuredAmalgamate,
                (int) ArcDPSEnums.TargetID.CALeftArm_CHINA,
                (int) ArcDPSEnums.TargetID.CARightArm_CHINA,
                (int) ArcDPSEnums.TargetID.ConjuredAmalgamate_CHINA,
            };
            foreach (int targetID in allTargetIDs)
            {
                if (agentData.GetNPCsByID(targetID).Any())
                {
                    if (blackList.Contains(targetID))
                    {
                        continue;
                    }
                    _targetIDs.Add(targetID);
                    /*switch (targetID)
                    {
                        case (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak:
                            //_subLogics.Add(new AiKeeperOfThePeak(targetID));
                            break;
                        default:
                            break;
                    }*/
                }
            }
        }

        internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return GetGenericFightOffset(fightData);
        }

        internal override void UpdatePlayersSpecAndGroup(IReadOnlyList<Player> players, CombatData combatData, FightData fightData)
        {
            // Nothing to do
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            if (_targetIDs.Count == 0)
            {
                return base.GetPhases(log, requirePhases);
            }
            List<PhaseData> phases = GetInitialPhase(log);
            phases[0].AddTargets(Targets);
            int phaseCount = 0;
            foreach (NPC target in Targets)
            {
                var phase = new PhaseData(Math.Max(log.FightData.FightStart, target.FirstAware), Math.Min(target.LastAware, log.FightData.FightEnd), "Phase " + (++phaseCount));
                phase.AddTarget(target);
                phases.Add(phase);
            }
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            FillSubLogics(agentData);
            foreach (FightLogic logic in _subLogics)
            {
                logic.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
                _targets.AddRange(logic.Targets);
                _trashMobs.AddRange(logic.TrashMobs);
                _nonPlayerFriendlies.AddRange(logic.NonPlayerFriendlies);
            }
            _targets.RemoveAll(x => x.IsSpecies(ArcDPSEnums.TargetID.DummyTarget));
            AgentItem dummyAgent = agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Dummy Instance Target", ParserHelper.Spec.NPC, ArcDPSEnums.TargetID.Instance, true);
            ComputeFightTargets(agentData, combatData, extensions);
            _targets.RemoveAll(x => x.LastAware - x.FirstAware < ParserHelper.MinimumInCombatDuration);
            TargetAgents = new HashSet<AgentItem>(_targets.Select(x => x.AgentItem));
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            fightData.SetSuccess(true, fightData.FightEnd);
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            InstanceStartEvent evt = combatData.GetInstanceStartEvent();
            if (evt == null)
            {
                return FightData.EncounterStartStatus.Normal;
            }
            else
            {
                return evt.TimeOffsetFromInstanceCreation > 1000 ? FightData.EncounterStartStatus.Late : FightData.EncounterStartStatus.Normal;
            }
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            MapIDEvent mapID = combatData.GetMapIDEvents().LastOrDefault();
            if (mapID == null)
            {
                return base.GetLogicName(combatData, agentData);
            }
            switch (mapID.MapID)
            {
                // Raids
                case 1062:
                    EncounterCategoryInformation.Category = FightCategory.Raid;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.SpiritVale;
                    Icon = InstanceIconSpiritVale;
                    Extension = "sprtvale";
                    return "Spirit Vale";
                case 1149:
                    EncounterCategoryInformation.Category = FightCategory.Raid;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.SalvationPass;
                    Icon = InstanceIconSalvationPass;
                    Extension = "salvpass";
                    return "Salvation Pass";
                case 1156:
                    EncounterCategoryInformation.Category = FightCategory.Raid;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.StrongholdOfTheFaithful;
                    Icon = InstanceIconStrongholdOfTheFaithful;
                    Extension = "strgldfaith";
                    return "Stronghold Of The Faithful";
                case 1188:
                    EncounterCategoryInformation.Category = FightCategory.Raid;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.BastionOfThePenitent;
                    Icon = InstanceIconBastionOfThePenitent;
                    Extension = "bstpen";
                    return "Bastion Of The Penitent";
                case 1264:
                    EncounterCategoryInformation.Category = FightCategory.Raid;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.HallOfChains;
                    Icon = InstanceIconHallOfChains;
                    Extension = "hallchains";
                    return "Hall Of Chains";
                case 1303:
                    EncounterCategoryInformation.Category = FightCategory.Raid;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.MythwrightGambit;
                    Icon = InstanceIconMythwrightGambit;
                    Extension = "mythgamb";
                    return "Mythwright Gambit";
                case 1323:
                    EncounterCategoryInformation.Category = FightCategory.Raid;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.TheKeyOfAhdashim;
                    Icon = InstanceIconTheKeyOfAhdashim;
                    Extension = "keyadash";
                    return "The Key Of Ahdashim";
                // Fractals
                case 960:
                    EncounterCategoryInformation.Category = FightCategory.Fractal;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.CaptainMaiTrinBossFractal;
                    Icon = InstanceIconCaptainMaiTrin;
                    Extension = "captnmai";
                    return "Captain Mai Trin Boss Fractal";
                case 1177:
                    EncounterCategoryInformation.Category = FightCategory.Fractal;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.Nightmare;
                    Icon = InstanceIconNightmare;
                    Extension = "nightmare";
                    return "Nightmare";
                case 1205:
                    EncounterCategoryInformation.Category = FightCategory.Fractal;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.ShatteredObservatory;
                    Icon = InstanceIconShatteredObservatory;
                    Extension = "shatrdobs";
                    return "Shattered Observatory";
                case 1290:
                    EncounterCategoryInformation.Category = FightCategory.Fractal;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.Deepstone;
                    Icon = InstanceIconDeepstone;
                    Extension = "deepstone";
                    return "Deepstone";
                case 1384:
                    EncounterCategoryInformation.Category = FightCategory.Fractal;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.SunquaPeak;
                    Icon = InstanceIconSunquaPeak;
                    Extension = "snqpeak";
                    return "Sunqua Peak";
            }
            return base.GetLogicName(combatData, agentData);
        }
        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            var res = new List<InstantCastFinder>();
            foreach (FightLogic logic in _subLogics)
            {
                res.AddRange(logic.GetInstantCastFinders());
            }
            return res;
        }
        internal override List<ErrorEvent> GetCustomWarningMessages(FightData fightData, EvtcVersionEvent evtcVersion)
        {
            var res = new List<ErrorEvent>();
            foreach (FightLogic logic in _subLogics)
            {
                res.AddRange(logic.GetCustomWarningMessages(fightData, evtcVersion));
            }
            return res;
        }
        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            foreach (FightLogic logic in _subLogics)
            {
                logic.ComputePlayerCombatReplayActors(p, log, replay);
            }
        }
        internal override List<AbstractBuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            foreach (FightLogic logic in _subLogics)
            {
                res.AddRange(logic.SpecialBuffEventProcess(combatData, skillData));
            }
            return res;
        }
        internal override List<AbstractCastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
        {
            var res = new List<AbstractCastEvent>();
            foreach (FightLogic logic in _subLogics)
            {
                res.AddRange(logic.SpecialCastEventProcess(combatData, skillData));
            }
            return res;
        }
        internal override List<AbstractHealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
        {
            var res = new List<AbstractHealthDamageEvent>();
            foreach (FightLogic logic in _subLogics)
            {
                res.AddRange(logic.SpecialDamageEventProcess(combatData, skillData));
            }
            return res;
        }
        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            foreach (FightLogic logic in _subLogics)
            {
                logic.ComputeNPCCombatReplayActors(target, log, replay);
            }
        }
        protected override List<int> GetTargetsIDs()
        {
            if (_targetIDs.Count != 0)
            {
                return _targetIDs;
            }
            return new List<int>()
            {
                GenericTriggerID
            };
        }
        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>();
        }
        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>();
        }
        protected override List<int> GetFriendlyNPCIDs()
        {
            return new List<int>();
        }
    }
}
