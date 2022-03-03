using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class AetherbladeHideout : CanthaStrike
    {
        public AetherbladeHideout(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Icon = "https://i.imgur.com/UZmW8Sd.png";
            Extension = "aetherhide";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.MaiTrinStrike,
                (int)ArcDPSEnums.TargetID.MaiTrinStrikeDuringEcho,
                (int)ArcDPSEnums.TargetID.EchoOfScarletBriar,
                (int)ArcDPSEnums.TrashID.ScarletPhantomBreakbar,
                (int)ArcDPSEnums.TrashID.ScarletPhantomHP,
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

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.ScarletPhantom1,
                ArcDPSEnums.TrashID.ScarletPhantom2,
            };
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Aetherblade Hideout";
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.MaiTrinStrike,
                (int)ArcDPSEnums.TargetID.EchoOfScarletBriar
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.MaiTrinStrike:
                    HealthUpdateEvent lastHPUpdate = log.CombatData.GetHealthUpdateEvents(target.AgentItem).LastOrDefault();
                    var maiTrinEnd = lastHPUpdate.Time;
                    replay.Trim(replay.TimeOffsets.start, maiTrinEnd);
                    break;
                default:
                    break;
            }

        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AbstractSingleActor echoOfScarlet = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.EchoOfScarletBriar);
                if (echoOfScarlet != null)
                {
                    AbstractSingleActor maiTrin = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.MaiTrinStrike);
                    if (maiTrin == null)
                    {
                        throw new MissingKeyActorsException("Mai Trin not found");
                    }
                    var buffApply = combatData.GetBuffData(895).OfType<BuffApplyEvent>().Where(x => x.To == maiTrin.AgentItem).LastOrDefault();
                    if (buffApply != null && buffApply.Time > echoOfScarlet.FirstAware)
                    {
                        fightData.SetSuccess(true, buffApply.Time);
                    }
                }
            }
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
                long maiTrinEnd = lastHPUpdate.Time;
                long maiTrinStart = 0;
                var buffRemove = log.CombatData.GetBuffData(895).OfType<BuffRemoveAllEvent>().Where(x => x.To == maiTrin.AgentItem).FirstOrDefault();
                if (buffRemove != null)
                {
                    maiTrinStart = buffRemove.Time;
                }
                var mainPhase = new PhaseData(0, maiTrinEnd, "Mai Trin");
                mainPhase.AddTarget(maiTrin);
                phases.Add(mainPhase);
                List<PhaseData> maiPhases = GetPhasesByInvul(log, 38793, maiTrin, false, true, maiTrinStart, maiTrinEnd);
                for (int i = 0; i < maiPhases.Count; i++)
                {
                    PhaseData subPhase = maiPhases[i];
                    subPhase.Name = "Mai Trin Phase " + (i + 1);
                    subPhase.AddTarget(maiTrin);
                }
                phases.AddRange(maiPhases);
            }
            if (echoOfScarlet != null)
            {
                long echoStart = echoOfScarlet.FirstAware + 10000;
                var phase = new PhaseData(echoStart, log.FightData.FightEnd, "Echo of Scarlet Briar");
                phase.AddTarget(echoOfScarlet);
                phases.Add(phase);
                List<PhaseData> echoPhases = GetPhasesByInvul(log, 38793, echoOfScarlet, false, true, echoStart, log.FightData.FightEnd);
                for (int i = 0; i < echoPhases.Count; i++)
                {
                    PhaseData subPhase = echoPhases[i];
                    subPhase.Name = "Echo Phase " + (i + 1);
                    subPhase.AddTarget(echoOfScarlet);
                }
                phases.AddRange(echoPhases);
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
            foreach (NPC target in Targets)
            {
                switch (target.ID)
                {
                    case (int)ArcDPSEnums.TrashID.ScarletPhantomBreakbar:
                        target.OverrideName("Elite " + target.Character + " CC");
                        break;
                    case (int)ArcDPSEnums.TrashID.ScarletPhantomHP:
                        target.OverrideName("Elite " + target.Character + " HP");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
