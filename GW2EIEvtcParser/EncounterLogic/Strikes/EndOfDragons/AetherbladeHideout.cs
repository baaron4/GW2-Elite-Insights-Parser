using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class AetherbladeHideout : EndOfDragonsStrike
    {
        public AetherbladeHideout(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(NightmareFusilladeMain, "Nightmare Fusillade", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkRed), "Cone", "Hit by Cone attack", "Cone", 150),
                new PlayerDstHitMechanic(NightmareFusilladeSide, "Nightmare Fusillade Side", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkRed), "Cone.S", "Hit by Side Cone attack", "Side Cone", 150),
                new PlayerDstHitMechanic(new long[] {TormentingWave, TormentingWaveCM }, "Tormenting Wave", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Shck.Wv", "Hit by Shockwave attack", "Shockwave", 150),
                new PlayerDstHitMechanic(new long[] {LeyBreach, LeyBreachCM }, "Ley Breach", new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "Puddle", "Stood in Puddle", "Puddle", 150),
                new PlayerDstHitMechanic(KaleidoscopicChaos, "Kaleidoscopic Chaos", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Orange), "Circle.H", "Hit by Yellow Circle", "Yellow Circle Hit", 150),
                new PlayerDstBuffApplyMechanic(new long[] {SharedDestructionMaiTrin, SharedDestructionMaiTrinCM }, "Shared Destruction", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Green", "Selected for Green", "Green", 150),
                new PlayerDstBuffApplyMechanic(PhotonSaturation, "Photon Saturation", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "Green.D", "Received Green debuff", "Green Debuff", 150),
                new PlayerDstSkillMechanic(FocusedDestruction, "Focused Destruction", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Green.Dwn", "Downed by Green", "Green Downed", 150).UsingChecker((evt, log) => evt.HasDowned),
                new PlayerDstBuffApplyMechanic(MagneticBomb, "Magnetic Bomb", new MechanicPlotlySetting(Symbols.Circle, Colors.Magenta), "Bomb", "Selected for Bomb", "Bomb", 150),
            }
            );
            Icon = EncounterIconAetherbladeHideout;
            Extension = "aetherhide";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayAetherbladeHideout,
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
                (int)ArcDPSEnums.TrashID.ScarletPhantomHPCM,
            };
        }

        protected override List<int> GetSuccessCheckIDs()
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
                ArcDPSEnums.TrashID.ScarletPhantomNormalBeam,
                ArcDPSEnums.TrashID.ScarletPhantomConeWaveNM,
                ArcDPSEnums.TrashID.ScarletPhantomDeathBeamCM,
                ArcDPSEnums.TrashID.ScarletPhantomDeathBeamCM2,
                ArcDPSEnums.TrashID.MaiTrinStrikeDuringEcho,
                ArcDPSEnums.TrashID.FerrousBomb,
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

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new BuffLossCastFinder(ReverseThePolaritySAK, MaiTrinCMBeamsTargetGreen),
                new BuffLossCastFinder(ReverseThePolaritySAK, MaiTrinCMBeamsTargetBlue),
                new BuffLossCastFinder(ReverseThePolaritySAK, MaiTrinCMBeamsTargetRed),
            };
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(player, log, replay);
            // Bomb Selection
            var bombs = player.GetBuffStatus(log, new long[] { MaiTrinCMBeamsTargetBlue, MaiTrinCMBeamsTargetGreen, MaiTrinCMBeamsTargetRed }, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            replay.AddOverheadIcons(bombs, player, ParserIcons.BombOverhead);
        }

        private AbstractSingleActor GetEchoOfScarletBriar(FightData fightData)
        {
            return Targets.FirstOrDefault(x => x.IsSpecies(fightData.IsCM ? (int)ArcDPSEnums.TargetID.EchoOfScarletBriarCM : (int)ArcDPSEnums.TargetID.EchoOfScarletBriarNM));
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                AbstractSingleActor echoOfScarlet = GetEchoOfScarletBriar(fightData);
                if (echoOfScarlet != null)
                {
                    AbstractSingleActor maiTrin = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.MaiTrinStrike));
                    if (maiTrin == null)
                    {
                        throw new MissingKeyActorsException("Mai Trin not found");
                    }
                    BuffApplyEvent buffApply = combatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == maiTrin.AgentItem).LastOrDefault();
                    if (buffApply != null && buffApply.Time > echoOfScarlet.FirstAware)
                    {
                        fightData.SetSuccess(true, buffApply.Time);
                    }
                }
            }
        }

        private IEnumerable<AbstractSingleActor> GetHPScarletPhantoms(PhaseData phase)
        {
            return Targets.Where(x => (x.IsSpecies(ArcDPSEnums.TrashID.ScarletPhantomHP) || x.IsSpecies(ArcDPSEnums.TrashID.ScarletPhantomHPCM)) && (phase.InInterval(x.FirstAware) || phase.InInterval(x.LastAware)));
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor maiTrin = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.MaiTrinStrike));
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
                BuffRemoveAllEvent buffRemove = log.CombatData.GetBuffData(Determined895).OfType<BuffRemoveAllEvent>().Where(x => x.To == maiTrin.AgentItem && x.Time > maiTrinStart).FirstOrDefault();
                if (buffRemove != null)
                {
                    maiTrinStart = buffRemove.Time;
                }
                var mainPhase = new PhaseData(0, maiTrinEnd, "Mai Trin");
                mainPhase.AddTarget(maiTrin);
                phases.Add(mainPhase);
                List<PhaseData> maiPhases = GetPhasesByInvul(log, Untargetable, maiTrin, true, true, maiTrinStart, maiTrinEnd);
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
                List<PhaseData> echoPhases = GetPhasesByInvul(log, Untargetable, echoOfScarlet, true, true, echoStart, log.FightData.FightEnd);
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

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // Ferrous Bombs
            var bombs = combatData.Where(x => x.DstAgent == 89640 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget).ToList();
            foreach (AgentItem bomb in bombs)
            {
                bomb.OverrideType(AgentItem.AgentType.NPC);
                bomb.OverrideID(ArcDPSEnums.TrashID.FerrousBomb);
            }
            agentData.Refresh();
            // We remove extra Mai trins if present
            IReadOnlyList<AgentItem> maiTrins = agentData.GetNPCsByID(ArcDPSEnums.TargetID.MaiTrinStrike);
            if (maiTrins.Count > 1)
            {
                for (int i = 1; i < maiTrins.Count; i++)
                {
                    maiTrins[i].OverrideID(ArcDPSEnums.TargetID.DummyMaiTrinStrike);
                }
                agentData.Refresh();
            }
            if (agentData.GetNPCsByID(ArcDPSEnums.TargetID.EchoOfScarletBriarNM).Count + agentData.GetNPCsByID(ArcDPSEnums.TargetID.EchoOfScarletBriarCM).Count == 0)
            {
                agentData.AddCustomNPCAgent(int.MaxValue, int.MaxValue, "Echo of Scarlet Briar", Spec.NPC, ArcDPSEnums.TargetID.EchoOfScarletBriarNM, false);
                agentData.AddCustomNPCAgent(int.MaxValue, int.MaxValue, "Echo of Scarlet Briar", Spec.NPC, ArcDPSEnums.TargetID.EchoOfScarletBriarCM, false);
            }
            ComputeFightTargets(agentData, combatData, extensions);
            var echoesOfScarlet = Targets.Where(x => x.IsSpecies(ArcDPSEnums.TargetID.EchoOfScarletBriarNM) || x.IsSpecies(ArcDPSEnums.TargetID.EchoOfScarletBriarCM)).ToList();
            foreach (AbstractSingleActor echoOfScarlet in echoesOfScarlet)
            {
                var hpUpdates = combatData.Where(x => x.SrcMatchesAgent(echoOfScarlet.AgentItem) && x.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate).ToList();
                if (hpUpdates.Count > 1 && hpUpdates.LastOrDefault().DstAgent == 10000)
                {
                    hpUpdates.LastOrDefault().OverrideSrcAgent(_unknownAgent.Agent);
                }
            }
            int curPhantom = 1;
            int curCC = 1;
            foreach (NPC target in Targets)
            {
                switch (target.ID)
                {
                    case (int)ArcDPSEnums.TrashID.ScarletPhantomBreakbar:
                        target.OverrideName("Elite " + target.Character + " CC " + (curCC++));
                        break;
                    case (int)ArcDPSEnums.TrashID.ScarletPhantomHP:
                    case (int)ArcDPSEnums.TrashID.ScarletPhantomHPCM:
                        target.OverrideName("Elite " + target.Character + " HP " + (curPhantom++));
                        break;
                    default:
                        break;
                }
            }
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor maiTrin = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.MaiTrinStrike));
            if (maiTrin == null)
            {
                throw new MissingKeyActorsException("Mai Trin not found");
            }
            return maiTrin.GetHealth(combatData) > 8e6 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);
            
            if (log.FightData.Success)
            {
                if (log.CombatData.GetBuffData(AchievementEligibilityTriangulation).Any())
                {
                    InstanceBuffs.AddRange(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityTriangulation));
                }
                else if (CustomCheckTriangulationEligibility(log))
                {
                    InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityTriangulation], 1));
                }
            }
        }

        private static bool CustomCheckTriangulationEligibility(ParsedEvtcLog log)
        {
            IReadOnlyList<long> beamsBuffs = new List<long>() { MaiTrinCMBeamsTargetBlue, MaiTrinCMBeamsTargetGreen, MaiTrinCMBeamsTargetRed };
            var beamsSegments = new List<Segment>();
            var bombInvulnSegments = new List<Segment>();

            foreach (AgentItem player in log.PlayerAgents)
            {
                IReadOnlyDictionary<long, BuffsGraphModel> bgms = log.FindActor(player).GetBuffGraphs(log);
                foreach (long buff in beamsBuffs)
                {
                    beamsSegments = GetBuffSegments(bgms, buff, beamsSegments).OrderBy(x => x.Start).ToList();
                }
            }

            foreach (AgentItem agent in log.AgentData.GetNPCsByID(ArcDPSEnums.TrashID.FerrousBomb))
            {
                IReadOnlyDictionary<long, BuffsGraphModel> bgms = log.FindActor(agent).GetBuffGraphs(log);
                bombInvulnSegments = GetBuffSegments(bgms, FailSafeActivated, bombInvulnSegments).OrderBy(x => x.Start).ToList();
            }

            int counter = 0;

            // For each segment where a bomb is invulnerable, check if it has started between the assignment and loss of a beam effect on a player (through buff)
            // If the counter is == 8, it means every possible combination check has been met and it's eligible for the achievement.
            // The combinations are 2 players buffs for each bomb invulnerability buff, so 2 x 4 total.
            foreach (Segment invuln in bombInvulnSegments)
            {
                foreach (Segment s in  beamsSegments)
                {
                    if (s.Start < invuln.Start && invuln.Start < s.End)
                    {
                        counter++;
                    }
                }
            }

            return counter == 8;
        }

        private static List<Segment> GetBuffSegments(IReadOnlyDictionary<long, BuffsGraphModel> bgms, long buff, List<Segment> segments)
        {
            if (bgms != null && bgms.TryGetValue(buff, out BuffsGraphModel bgm))
            {
                foreach (Segment s in bgm.BuffChart)
                {
                    if (s.Value == 1)
                    {
                        segments.Add(s);
                    }
                }
            }
            return segments;
        }
    }
}
