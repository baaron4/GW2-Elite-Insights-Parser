using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Instance : FightLogic
    {
        public bool StartedLate { get; private set; }
        public bool EndedBeforeExpectedEnd { get; private set; }
        private readonly List<FightLogic> _subLogics = new List<FightLogic>();
        public Instance(int id) : base(id)
        {
            Extension = "instance";
            Icon = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
            EncounterCategoryInformation.Category = FightCategory.UnknownEncounter;
            EncounterCategoryInformation.SubCategory = SubFightCategory.UnknownEncounter;
        }

        private void FillSubLogics(AgentData agentData)
        {
            var allTargetIDs = Enum.GetValues(typeof(ArcDPSEnums.TargetID)).Cast<int>().ToList();
            foreach (int targetID in allTargetIDs)
            {
                if (agentData.GetNPCsByID(targetID).Any())
                {
                    switch(targetID)
                    {
                        case (int)ArcDPSEnums.TargetID.AiKeeperOfThePeak:
                            //_subLogics.Add(new AiKeeperOfThePeak(targetID));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            FillSubLogics(agentData);
            foreach (FightLogic logic in _subLogics)
            {
                logic.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
                _targets.AddRange(logic.Targets);
                _trashMobs.AddRange(logic.TrashMobs);
                _nonPlayerFriendlies.AddRange(logic.NonPlayerFriendlies);
            }
            _targets.RemoveAll(x => x.ID == (int)ArcDPSEnums.TargetID.DummyTarget);
            Targetless = !_targets.Any();
            AgentItem dummyAgent = agentData.AddCustomNPCAgent(0, fightData.FightEnd, "Dummy Instance Target", ParserHelper.Spec.NPC, (int)ArcDPSEnums.TargetID.Instance, true);
            ComputeFightTargets(agentData, combatData, extensions);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            fightData.SetSuccess(true, fightData.FightEnd);
            InstanceStartEvent evt = combatData.GetInstanceStartEvent();
            if (evt == null)
            {
                StartedLate = true;
            }
            else
            {
                StartedLate = Math.Abs(evt.OffsetFromInstanceCreation - fightData.LogOffset) < 20000;
            }
        }
        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            MapIDEvent mapID = combatData.GetMapIDEvents().LastOrDefault();
            if (mapID == null)
            {
                return base.GetLogicName(combatData, agentData);
            }
            switch(mapID.MapID)
            {
                case 1384:
                    EncounterCategoryInformation.Category = FightCategory.Fractal;
                    EncounterCategoryInformation.SubCategory = SubFightCategory.SunquaPeak;
                    Extension = "snqpeak";
                    Icon = "https://i.imgur.com/3mlCdI9.png";
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
        internal override List<ErrorEvent> GetCustomWarningMessages(FightData fightData, int arcdpsVersion)
        {
            var res = new List<ErrorEvent>();
            foreach (FightLogic logic in _subLogics)
            {
                res.AddRange(logic.GetCustomWarningMessages(fightData, arcdpsVersion));
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
            if (!Targetless)
            {
                return new List<int>();
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
