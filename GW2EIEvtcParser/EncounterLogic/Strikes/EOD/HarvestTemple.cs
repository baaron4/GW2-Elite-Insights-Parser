using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class HarvestTemple : CanthaStrike
    {
        public HarvestTemple(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Extension = "harvsttmpl";
            EncounterCategoryInformation.InSubCategoryOrder = 3;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor dragonVoid = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.TheDragonVoidJormag);
            if (dragonVoid == null)
            {
                throw new MissingKeyActorsException("The Dragon Void not found");
            }
            phases[0].AddTarget(dragonVoid);
            if (!requirePhases)
            {
                return phases;
            }
            var attackTargets = log.CombatData.GetAttackTargetEvents(dragonVoid.AgentItem);
            return phases;
        }

        protected override List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.TheDragonVoidJormag,
                (int)ArcDPSEnums.TargetID.TheDragonVoidKralkatorik,
                (int)ArcDPSEnums.TargetID.TheDragonVoidMordremoth,
                (int)ArcDPSEnums.TargetID.TheDragonVoidPrimordious,
                (int)ArcDPSEnums.TargetID.TheDragonVoidSonWoo,
                (int)ArcDPSEnums.TargetID.TheDragonVoidZhaitan,
                (int)ArcDPSEnums.TrashID.VoidSaltsprayDragon,
                (int)ArcDPSEnums.TrashID.VoidTimeCaster,
            };
        }
        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.ZhaitansReach,
                ArcDPSEnums.TrashID.VoidAbomination,
                ArcDPSEnums.TrashID.VoidAmalgamate1,
                ArcDPSEnums.TrashID.VoidAmalgamate2,
                ArcDPSEnums.TrashID.VoidBrandbomber,
                ArcDPSEnums.TrashID.VoidBurster,
                ArcDPSEnums.TrashID.VoidColdsteel,
                ArcDPSEnums.TrashID.VoidGiant,
                ArcDPSEnums.TrashID.VoidMelter,
                ArcDPSEnums.TrashID.VoidRotswarmer,
                ArcDPSEnums.TrashID.VoidSaltsprayDragon,
                ArcDPSEnums.TrashID.VoidSkullpiercer,
                ArcDPSEnums.TrashID.VoidStormseer,
                ArcDPSEnums.TrashID.VoidTangler,
                ArcDPSEnums.TrashID.VoidTimeCaster,
                ArcDPSEnums.TrashID.VoidWarforged1,
                ArcDPSEnums.TrashID.VoidWarforged2,
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // We remove extra Mai trins if present
            var attackTargetEvents = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.AttackTarget).ToList();
            var linkedTargets = new HashSet<AgentItem>();
            foreach (CombatItem ate in attackTargetEvents)
            {
                linkedTargets.Add(agentData.GetAgent(ate.DstAgent, ate.Time));
            }
            if (linkedTargets.Any())
            {
                foreach (AgentItem a in linkedTargets)
                {
                    var maxHPUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && x.SrcMatchesAgent(a));
                    a.OverrideType(AgentItem.AgentType.NPC);
                }
                agentData.Refresh();
            }
            ComputeFightTargets(agentData, combatData, extensions);
        }
    }
}
