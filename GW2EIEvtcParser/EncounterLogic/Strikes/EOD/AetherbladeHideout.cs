using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class AetherbladeHideout : EODStrike
    {
        public AetherbladeHideout(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new HitOnPlayerMechanic(NightmareFusilladeMain, "Nightmare Fusillade", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkRed), "Cone", "Hit by Cone attack", "Cone", 150),
                new HitOnPlayerMechanic(NightmareFusilladeSide, "Nightmare Fusillade Side", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkRed), "Cone.S", "Hit by Side Cone attack", "Side Cone", 150),
                new HitOnPlayerMechanic(new long[] {TormentingWave, TormentingWaveCM }, "Tormenting Wave", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Shck.Wv", "Hit by Shockwave attack", "Shockwave", 150),
                new HitOnPlayerMechanic(new long[] {LeyBreach, LeyBreachCM }, "Ley Breach", new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "Puddle", "Stood in Puddle", "Puddle", 150),
                new HitOnPlayerMechanic(KaleidoscopicChaos, "Kaleidoscopic Chaos", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Orange), "Circle.H", "Hit by Yellow Circle", "Yellow Circle Hit", 150),
                new PlayerBuffApplyMechanic(ExposedEODStrike, "Exposed", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Red), "Exposed", "Received Exposed stack", "Exposed", 150),
                new PlayerBuffApplyMechanic(new long[] {SharedDestructionMaiTrin, SharedDestructionMaiTrinCM }, "Shared Destruction", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Green", "Selected for Green", "Green", 150),
                new PlayerBuffApplyMechanic(PhotonSaturation, "Photon Saturation", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "Green.D", "Received Green debuff", "Green Debuff", 150),
                new SkillOnPlayerMechanic(FocusedDestruction, "Focused Destruction", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Green.Dwn", "Downed by Green", "Green Downed", 150, (evt, log) => evt.HasDowned),
                new PlayerBuffApplyMechanic(MagneticBomb, "Magnetic Bomb", new MechanicPlotlySetting(Symbols.Circle, Colors.Magenta), "Bomb", "Selected for Bomb", "Bomb", 150),
            }
            );
            Icon = "https://i.imgur.com/UZmW8Sd.png";
            Extension = "aetherhide";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/Qhnhrvp.png",
                            (838, 639),
                            (1165, 540, 4194, 2850)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }
        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.MaiTrinStrike,
                (int)ArcDPSEnums.TargetID.EchoOfScarletBriarNM,
                (int)ArcDPSEnums.TargetID.EchoOfScarletBriarCM,
                (int)ArcDPSEnums.TrashID.ScarletPhantomBreakbar,
                (int)ArcDPSEnums.TrashID.ScarletPhantomHP,
                (int)ArcDPSEnums.TrashID.ScarletPhantomHP2,
            };
        }

        protected override List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.MaiTrinStrike,
                (int)ArcDPSEnums.TargetID.EchoOfScarletBriarNM,
                (int)ArcDPSEnums.TargetID.EchoOfScarletBriarCM,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.ScarletPhantom1,
                ArcDPSEnums.TrashID.ScarletPhantom2,
                ArcDPSEnums.TrashID.MaiTrinStrikeDuringEcho,
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
                (int)ArcDPSEnums.TargetID.EchoOfScarletBriarNM,
                (int)ArcDPSEnums.TargetID.EchoOfScarletBriarCM,
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.MaiTrinStrike:
                    HealthUpdateEvent lastHPUpdate = log.CombatData.GetHealthUpdateEvents(target.AgentItem).LastOrDefault();
                    long maiTrinEnd = lastHPUpdate.Time;
                    replay.Trim(replay.TimeOffsets.start, maiTrinEnd);
                    break;
                default:
                    break;
            }

        }

        private AbstractSingleActor GetEchoOfScarletBriar(FightData fightData)
        {
            return Targets.FirstOrDefault(x => x.ID == (fightData.IsCM ? (int)ArcDPSEnums.TargetID.EchoOfScarletBriarCM : (int)ArcDPSEnums.TargetID.EchoOfScarletBriarNM));
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AbstractSingleActor echoOfScarlet = GetEchoOfScarletBriar(fightData);
                if (echoOfScarlet != null)
                {
                    AbstractSingleActor maiTrin = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.MaiTrinStrike);
                    if (maiTrin == null)
                    {
                        throw new MissingKeyActorsException("Mai Trin not found");
                    }
                    BuffApplyEvent buffApply = combatData.GetBuffData(SkillIDs.Determined895).OfType<BuffApplyEvent>().Where(x => x.To == maiTrin.AgentItem).LastOrDefault();
                    if (buffApply != null && buffApply.Time > echoOfScarlet.FirstAware)
                    {
                        fightData.SetSuccess(true, buffApply.Time);
                    }
                }
            }
        }

        private IEnumerable<AbstractSingleActor> GetHPScarletPhantoms(PhaseData phase)
        {
            return Targets.Where(x => (x.ID == (int)ArcDPSEnums.TrashID.ScarletPhantomHP || x.ID == (int)ArcDPSEnums.TrashID.ScarletPhantomHP2) && (phase.InInterval(x.FirstAware) || phase.InInterval(x.LastAware)));
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
            AbstractSingleActor echoOfScarlet = GetEchoOfScarletBriar(log.FightData);
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
                BuffRemoveAllEvent buffRemove = log.CombatData.GetBuffData(SkillIDs.Determined895).OfType<BuffRemoveAllEvent>().Where(x => x.To == maiTrin.AgentItem).FirstOrDefault();
                if (buffRemove != null)
                {
                    maiTrinStart = buffRemove.Time;
                }
                var mainPhase = new PhaseData(0, maiTrinEnd, "Mai Trin");
                mainPhase.AddTarget(maiTrin);
                phases.Add(mainPhase);
                List<PhaseData> maiPhases = GetPhasesByInvul(log, 38793, maiTrin, true, true, maiTrinStart, maiTrinEnd);
                for (int i = 0; i < maiPhases.Count; i++)
                {
                    PhaseData subPhase = maiPhases[i];
                    if ((i % 2) == 0)
                    {
                        subPhase.Name = "Mai Trin Phase " + ((i / 2) + 1);
                        subPhase.AddTarget(maiTrin);
                    } 
                    else
                    {
                        subPhase.Name = "Mai Trin Split Phase " + ((i / 2) + 1);
                        subPhase.AddTargets(GetHPScarletPhantoms(subPhase));
                    }
                }
                phases.AddRange(maiPhases);
            }
            if (echoOfScarlet != null)
            {
                long echoStart = echoOfScarlet.FirstAware + 10000;
                var phase = new PhaseData(echoStart, log.FightData.FightEnd, "Echo of Scarlet Briar");
                phase.AddTarget(echoOfScarlet);
                phases.Add(phase);
                List<PhaseData> echoPhases = GetPhasesByInvul(log, 38793, echoOfScarlet, true, true, echoStart, log.FightData.FightEnd);
                for (int i = 0; i < echoPhases.Count; i++)
                {
                    PhaseData subPhase = echoPhases[i];
                    if ((i % 2) == 0)
                    {
                        subPhase.Name = "Echo Phase " + ((i / 2) + 1);
                        subPhase.AddTarget(echoOfScarlet);
                    }
                    else
                    {
                        subPhase.Name = "Echo Split Phase " + ((i / 2) + 1);
                        subPhase.AddTargets(GetHPScarletPhantoms(subPhase));
                    }
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
            if (agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.EchoOfScarletBriarNM).Count + agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.EchoOfScarletBriarCM).Count == 0)
            {
                agentData.AddCustomNPCAgent(long.MaxValue, long.MaxValue, "Echo of Scarlet Briar", Spec.NPC, (int)ArcDPSEnums.TargetID.EchoOfScarletBriarNM, false);
                agentData.AddCustomNPCAgent(long.MaxValue, long.MaxValue, "Echo of Scarlet Briar", Spec.NPC, (int)ArcDPSEnums.TargetID.EchoOfScarletBriarCM, false);
            }
            ComputeFightTargets(agentData, combatData, extensions);
            var echoesOfScarlet = Targets.Where(x => x.ID == (int)ArcDPSEnums.TargetID.EchoOfScarletBriarNM || x.ID == (int)ArcDPSEnums.TargetID.EchoOfScarletBriarCM).ToList();
            foreach (AbstractSingleActor echoOfScarlet in echoesOfScarlet)
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
                    case (int)ArcDPSEnums.TrashID.ScarletPhantomHP2:
                        target.OverrideName("Elite " + target.Character + " HP");
                        break;
                    default:
                        break;
                }
            }
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor maiTrin = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.MaiTrinStrike);
            if (maiTrin == null)
            {
                throw new MissingKeyActorsException("Mai Trin not found");
            }
            return maiTrin.GetHealth(combatData) > 8e6 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }
    }
}
