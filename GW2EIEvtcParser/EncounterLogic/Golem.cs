using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Golem : FightLogic
    {
        public Golem(int id) : base(id)
        {
            Mode = ParseMode.Benchmark;
            EncounterID |= EncounterIDs.EncounterMasks.GolemMask;
            EncounterID |= 0x000100;
            switch (ArcDPSEnums.GetTargetID(id))
            {
                case ArcDPSEnums.TargetID.MassiveGolem10M:
                    Extension = "MassiveGolem10M";
                    Icon = "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                    EncounterID |= 0x000001;
                    break;
                case ArcDPSEnums.TargetID.MassiveGolem4M:
                    Extension = "MassiveGolem4M";
                    Icon = "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                    EncounterID |= 0x000002;
                    break;
                case ArcDPSEnums.TargetID.MassiveGolem1M:
                    Extension = "MassiveGolem1M";
                    Icon = "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                    EncounterID |= 0x000003;
                    break;
                case ArcDPSEnums.TargetID.VitalGolem:
                    Extension = "VitalGolem";
                    Icon = "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                    EncounterID |= 0x000004;
                    break;
                case ArcDPSEnums.TargetID.AvgGolem:
                    Extension = "AvgGolem";
                    Icon = "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                    EncounterID |= 0x000005;
                    break;
                case ArcDPSEnums.TargetID.StdGolem:
                    Extension = "StdGolem";
                    Icon = "https://wiki.guildwars2.com/images/8/8f/Mini_Professor_Mew.png";
                    EncounterID |= 0x000006;
                    break;
                case ArcDPSEnums.TargetID.ConditionGolem:
                    Extension = "ToughGolem";
                    Icon = "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                    EncounterID |= 0x000007;
                    break;
                case ArcDPSEnums.TargetID.PowerGolem:
                    Extension = "ResGolem";
                    Icon = "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                    EncounterID |= 0x000008;
                    break;
                case ArcDPSEnums.TargetID.LGolem:
                    Extension = "LGolem";
                    Icon = "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                    EncounterID |= 0x000009;
                    break;
                case ArcDPSEnums.TargetID.MedGolem:
                    Extension = "MedGolem";
                    Icon = "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                    EncounterID |= 0x00000A;
                    break;
            }
            EncounterCategoryInformation.Category = FightCategory.Golem;
            EncounterCategoryInformation.SubCategory = SubFightCategory.Golem;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/gmnSuz7.png",
                            (895, 629),
                            (18115.12, -13978.016, 22590.12, -10833.016));
        }

        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            CombatItem pov = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.PointOfView);
            if (pov != null)
            {
                // to make sure that the logging starts when the PoV starts attacking (in case there is a slave with them)
                CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcAgent == pov.SrcAgent && x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat);
                if (enterCombat != null)
                {
                    return enterCombat.Time;
                }
            }
            return fightData.LogStart;
        }
        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new BuffGainCastFinder(MushroomKingsBlessing, 46970).UsingICD(500), // Mushroom King's Blessing`
            };
        }
        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            AgentItem target = agentData.GetNPCsByID(GenericTriggerID).FirstOrDefault();
            foreach (CombatItem c in combatData)
            {
                // redirect all attacks to the main golem
                if (c.DstAgent == 0 && c.DstInstid == 0 && c.IsDamage(extensions))
                {
                    c.OverrideDstAgent(target.Agent);
                }
            }
            ComputeFightTargets(agentData, combatData, extensions);
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
#if DEBUG
            ProfHelper.DEBUG_ComputeProfessionCombatReplayActors(p, log, replay);
#endif
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Golem not found");
            }
            phases[0].Name = "Final Number";
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            IReadOnlyList<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem);
            if (hpUpdates.Count > 0)
            {
                long fightDuration = log.FightData.FightEnd;
                var thresholds = new List<double> { 80, 60, 40, 20, 0 };
                string[] numberNames = new string[] { "First Number", "Second Number", "Third Number", "Fourth Number" };
                // Fifth number would the equivalent of full fight phase
                for (int j = 0; j < thresholds.Count - 1; j++)
                {
                    HealthUpdateEvent hpUpdate = hpUpdates.FirstOrDefault(x => x.HPPercent <= thresholds[j]);
                    if (hpUpdate != null)
                    {
                        var phase = new PhaseData(0, hpUpdate.Time, numberNames[j])
                        {
                            CanBeSubPhase = false
                        };
                        phase.AddTarget(mainTarget);
                        phases.Add(phase);
                    }
                }
                phases.AddRange(GetPhasesByHealthPercent(log, mainTarget, thresholds));
            }

            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Golem not found");
            }
            long fightEndLogTime = fightData.FightEnd;
            bool success = false;
            DeadEvent deadEvt = combatData.GetDeadEvents(mainTarget.AgentItem).LastOrDefault();
            if (deadEvt != null)
            {
                fightEndLogTime = deadEvt.Time;
                success = true;
            } 
            else
            {
                IReadOnlyList<HealthUpdateEvent> hpUpdates = combatData.GetHealthUpdateEvents(mainTarget.AgentItem);
                if (hpUpdates.Count > 0)
                {
                    AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(mainTarget.AgentItem).LastOrDefault(x => x.HealthDamage > 0);
                    success = hpUpdates.Last().HPPercent < 2.00;
                    if (success && lastDamageTaken != null)
                    {
                        fightEndLogTime = lastDamageTaken.Time;
                    }
                }
            }          
            fightData.SetSuccess(success, fightEndLogTime);
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                GenericTriggerID
            };
        }
    }
}
