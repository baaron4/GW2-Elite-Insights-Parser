using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class XunlaiJadeJunkyard : CanthaStrike
    {
        public XunlaiJadeJunkyard(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Extension = "xunjadejunk";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Ankka);
            if (ankka == null)
            {
                throw new MissingKeyActorsException("Ankka not found");
            }
            phases[0].AddTarget(ankka);
            if (!requirePhases)
            {
                return phases;
            }
            var subPhases = GetPhasesByInvul(log, 895, ankka, false, false);
            for (int i = 0; i < subPhases.Count; i++)
            {
                subPhases[i].Name = "Phase " + (i + 1);
                subPhases[i].AddTarget(ankka);
            }
            phases.AddRange(subPhases);
            //
            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Ankka);
                if (ankka == null)
                {
                    throw new MissingKeyActorsException("Ankka not found");
                }
                var buffApplies = combatData.GetBuffData(895).OfType<BuffApplyEvent>().Where(x => x.To == ankka.AgentItem && !x.Initial && x.AppliedDuration > int.MaxValue / 2).ToList();
                if (buffApplies.Count == 3)
                {
                    fightData.SetSuccess(true, buffApplies.LastOrDefault().Time);
                }
            }
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.Ankka,
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Ankka,
            };
        }

        protected override List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Ankka,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Ankka,
                ArcDPSEnums.TrashID.ReanimatedHatred,
                ArcDPSEnums.TrashID.ReanimatedMalice1,
                ArcDPSEnums.TrashID.ReanimatedMalice2,
                ArcDPSEnums.TrashID.ReanimatedHatred,
                ArcDPSEnums.TrashID.ZhaitansReach,
                ArcDPSEnums.TrashID.AnkkaHallucination1,
                ArcDPSEnums.TrashID.AnkkaHallucination2,
                ArcDPSEnums.TrashID.AnkkaHallucination3,
            };
        }

        /*protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/kLjZ7eU.png",
                            (905, 789),
                            (-1013, -1600, 2221, 1416));
        }*/

        /*internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(58736, 58736, InstantCastFinder.DefaultICD), // Unnatural Aura
            };
        }*/

        /*protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.VigilTactician,
                ArcDPSEnums.TrashID.VigilRecruit,
                ArcDPSEnums.TrashID.PrioryExplorer,
                ArcDPSEnums.TrashID.PrioryScholar,
                ArcDPSEnums.TrashID.AberrantWisp,
            };
        }*/
    }
}
