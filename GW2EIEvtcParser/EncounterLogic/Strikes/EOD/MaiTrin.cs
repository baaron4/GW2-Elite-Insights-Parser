using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class MaiTrin : CanthaStrike
    {
        public MaiTrin(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Extension = "maitrin";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.MaiTrinStrike,
                (int)ArcDPSEnums.TargetID.EchoOfScarletBriar
            };
        }

        protected override List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.MaiTrinStrike,
                (int)ArcDPSEnums.TargetID.EchoOfScarletBriar
            };
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.MaiTrinStrike,
                (int)ArcDPSEnums.TargetID.EchoOfScarletBriar
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor maiTrin = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.MaiTrinStrike);
            if (maiTrin == null)
            {
                throw new MissingKeyActorsException("Mai Trin not found");
            }
            phases[0].AddTarget(maiTrin);
            AbstractSingleActor echoOfScarlet = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.EchoOfScarletBriar);
            if (echoOfScarlet != null)
            {
                phases[0].AddTarget(echoOfScarlet);
            }
            if (!requirePhases)
            {
                return phases;
            }
            if (log.CombatData.GetDamageTakenData(maiTrin.AgentItem).Any())
            {
                HealthUpdateEvent lastHPUpdate = log.CombatData.GetHealthUpdateEvents(maiTrin.AgentItem).LastOrDefault();
                var maiTrinEnd = lastHPUpdate.Time;
                var mainPhase = new PhaseData(0, maiTrinEnd, "Mai Trin");
                mainPhase.AddTarget(maiTrin);
                phases.Add(mainPhase);
                List<PhaseData> maiPhases = GetPhasesByInvul(log, 38793, maiTrin, false, true, maiTrinEnd);
                maiPhases.RemoveAll(x => x.DurationInMS < 2200);
                for (int i = 0; i < maiPhases.Count; i++)
                {
                    PhaseData subPhase = maiPhases[i];
                    subPhase.Name = "Mai Tring Phase " + (i + 1);
                    subPhase.AddTarget(maiTrin);
                }
                phases.AddRange(maiPhases);
            }
            if (echoOfScarlet != null)
            {
                long echoStart = echoOfScarlet.FirstAware;
                // Todo find a better start
                EnterCombatEvent echoCombat = log.CombatData.GetEnterCombatEvents(echoOfScarlet.AgentItem).FirstOrDefault();
                if (echoCombat != null)
                {
                    echoStart = echoCombat.Time;
                }
                var phase = new PhaseData(echoStart, log.FightData.FightEnd, "Echo of Scarlet Briar");
                phase.AddTarget(echoOfScarlet);
                phases.Add(phase);
            }
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // We remove extra Mai trins if present
            IReadOnlyList<AgentItem> maiTrins = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.MaiTrinStrike);
            if (maiTrins.Count > 1)
            {
                for (int i = 1; i < maiTrins.Count; i++)
                {
                    maiTrins[i].OverrideID(ArcDPSEnums.TargetID.DummyMaiTrinStrike);
                }
                agentData.Refresh();
            }
            ComputeFightTargets(agentData, combatData, extensions);
            AbstractSingleActor echoOfScarlet = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.EchoOfScarletBriar);
            if (echoOfScarlet != null)
            {
                var hpUpdates = combatData.Where(x => x.SrcMatchesAgent(echoOfScarlet.AgentItem) && x.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate).ToList();
                if (hpUpdates.Count > 1 && hpUpdates.LastOrDefault().DstAgent == 10000)
                {
                    hpUpdates.LastOrDefault().OverrideSrcAgent(ParserHelper._unknownAgent.Agent);
                }
            }
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
