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
    internal class OldLionsCourt : EndOfDragonsStrike
    {
        public OldLionsCourt(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(new long [] { BoilingAetherRedBlueNM, BoilingAetherRedBlueCM }, "Boiling Aether (Vermilion & Indigo)", new MechanicPlotlySetting(Symbols.Circle, Colors.LightRed), "Red.VermIndi.H", "Hit by Boiling Aether (Vermilion & Indigo)", "Boiling Aether Hit (Vermilion & Indigo)", 0),
                new PlayerDstHitMechanic(new long [] { BoilingAetherGreenNM, BoilingAetherGreenCM }, "Boiling Aether (Arsenite)", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Red.Arse.H", "Hit by Boiling Aether (Arsenite)", "Boiling Aether Hit (Arsenite)", 0),
                new PlayerDstHitMechanic(new long [] { DualHorizon, DualHorizonCM }, "Dual Horizon", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightRed), "DualHrz.H", "Hit by Dual Horizon", "Dual Horizon Hit", 0),
                new PlayerDstHitMechanic(new long [] { TriBolt, TriBoltCM }, "Tri Bolt", new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "TriBolt.H", "Hit by Tri Bolt (Spread AoEs)", "Tri Bolt Hit", 150),
                new PlayerDstHitMechanic(new long [] { Tribocharge, TribochargeCM }, "Tribocharge", new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.LightOrange), "TriChg.H", "Hit by Tribocharge", "Tribocharge Hit", 150),
                new PlayerDstHitMechanic(new long [] { NoxiousVaporBlade, NoxiousVaporBladeCM }, "Noxious Vapor Blade", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Green), "BladeOut.H", "Hit by Noxious Vapor Blade (to player)", "Noxious Vapor Blade Hit", 150),
                new PlayerDstHitMechanic(new long [] { NoxiousReturn, NoxiousReturnCM }, "Noxious Return", new MechanicPlotlySetting(Symbols.CircleX, Colors.Green), "BladeBack.H", "Hit by Noxious Return (to Arsenite)", "Noxious Return Hit", 150),
                new PlayerDstHitMechanic(new long [] { BoilingAetherRedBlueNM, BoilingAetherRedBlueCM, BoilingAetherGreenNM, BoilingAetherGreenCM }, "Boiling Aether", new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.Red), "AethAver.Achiv", "Achievement Eligibility: Aether Aversion", "Achiv Aether Aversion", 150).UsingAchievementEligibility(true),
                new PlayerDstSkillMechanic(ExhaustPlume, "Exhaust Plume", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Red), "VermFall.H", "Hit by Exhaust Plume (Vermilion Fall)", "Exhaust Plume Hit (Vermilion)", 150).UsingChecker((de, log) => de.CreditedFrom.IsAnySpecies(new List<ArcDPSEnums.TargetID> { ArcDPSEnums.TargetID.PrototypeVermilion, ArcDPSEnums.TargetID.PrototypeVermilionCM })),
                new PlayerDstSkillMechanic(ExhaustPlume, "Exhaust Plume", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "ArseFall.H", "Hit by Exhaust Plume (Arsenite Fall)", "Exhaust Plume Hit (Arsenite)", 150).UsingChecker((de, log) => de.CreditedFrom.IsAnySpecies(new List<ArcDPSEnums.TargetID> { ArcDPSEnums.TargetID.PrototypeArsenite, ArcDPSEnums.TargetID.PrototypeArseniteCM })),
                new PlayerDstSkillMechanic(ExhaustPlume, "Exhaust Plume", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Blue), "IndiFall.H", "Hit by Exhaust Plume (Indigo Fall)", "Exhaust Plume Hit (Indigo)", 150).UsingChecker((de, log) => de.CreditedFrom.IsAnySpecies(new List<ArcDPSEnums.TargetID> { ArcDPSEnums.TargetID.PrototypeIndigo, ArcDPSEnums.TargetID.PrototypeIndigoCM })),
                new PlayerDstBuffApplyMechanic(Spaghettification, "Spaghettification", new MechanicPlotlySetting(Symbols.Bowtie, Colors.DarkRed), "Spgt.H", "Hit by Spaghettification", "Spaghettification Hit", 0),
                new PlayerDstBuffApplyMechanic(Dysapoptosis, "Dysapoptosis", new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.DarkRed), "Dysp.H", "Hit by Dysapoptosis", "Dysapoptosis Hit", 0),
                new PlayerDstBuffApplyMechanic(ThunderingUltimatum, "Thundering Ultimatum", new MechanicPlotlySetting(Symbols.Asterisk, Colors.DarkRed), "ThunUlti.H", "Hit by Thundering Ultimatum", "Thunderin gUltimatum Hit", 0),
                new PlayerDstBuffApplyMechanic(new long [] { TidalTorment, TidalTormentCM }, "Tidal Torment", new MechanicPlotlySetting(Symbols.Star, Colors.Red), "TidTorm.A", "Tidal Torment Applied", "Tidal Torment Applied", 0),
                new PlayerDstBuffApplyMechanic(new long [] { ErgoShear, ErgoShearCM }, "Ergo Shear", new MechanicPlotlySetting(Symbols.StarOpen, Colors.Red), "ErgShr.A", "Ergo Shear Applied", "Ergo Shear Applied", 0),
                new PlayerDstBuffApplyMechanic(FixatedOldLionsCourt, "Fixated (Vermilion)", new MechanicPlotlySetting(Symbols.Diamond, Colors.Red), "Fix.Verm.A", "Fixated Applied", "Fixated Applied", 0).UsingChecker((bae, log) => bae.CreditedBy.IsAnySpecies(new List<ArcDPSEnums.TargetID> { ArcDPSEnums.TargetID.PrototypeVermilion, ArcDPSEnums.TargetID.PrototypeVermilionCM })),
                new PlayerDstBuffApplyMechanic(FixatedOldLionsCourt, "Fixated (Arsenite)", new MechanicPlotlySetting(Symbols.Diamond, Colors.Green), "Fix.Arse.A", "Fixated Applied", "Fixated Applied", 0).UsingChecker((bae, log) => bae.CreditedBy.IsAnySpecies(new List<ArcDPSEnums.TargetID> { ArcDPSEnums.TargetID.PrototypeArsenite, ArcDPSEnums.TargetID.PrototypeArseniteCM })),
                new PlayerDstBuffApplyMechanic(FixatedOldLionsCourt, "Fixated (Indigo)", new MechanicPlotlySetting(Symbols.Diamond, Colors.Blue), "Fix.Indi.A", "Fixated Applied", "Fixated Applied", 0).UsingChecker((bae, log) => bae.CreditedBy.IsAnySpecies(new List<ArcDPSEnums.TargetID> { ArcDPSEnums.TargetID.PrototypeIndigo, ArcDPSEnums.TargetID.PrototypeIndigoCM })),
                new EnemyDstBuffApplyMechanic(EmpoweredWatchknightTriumverate, "Empowered", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Blue), "Empowered.A", "Knight gained Empowered", "Empowered Applied", 0),
                new EnemyDstBuffApplyMechanic(PowerTransfer, "Power Transfer", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Blue), "PwrTrns.A", "Knight gained Power Transfer", "Power Transfer Applied", 0),
                new EnemyDstBuffApplyMechanic(LeyWovenShielding, "Ley-Woven Shielding", new MechanicPlotlySetting(Symbols.Pentagon, Colors.Teal), "WovShld.A", "Knight gained Ley-Woven Shielding", "Ley-Woven Shielding Applied", 0),
                new EnemyDstBuffApplyMechanic(MalfunctioningLeyWovenShielding, "Malfunctioning Ley-Woven Shielding", new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.DarkTeal), "MalfWovShld.A", "Knight gained Malfunctioning Ley-Woven Shielding", "Malfunctioning Ley-Woven Shielding Applied", 0),
                new EnemyDstBuffApplyMechanic(Exposed31589, "Exposed (Knight)", new MechanicPlotlySetting(Symbols.HexagonOpen, Colors.Purple), "Expo.A", "Exposed Applied to Knight", "Exposed Applied to Knight", 0),
                new EnemyCastStartMechanic(new long [] { DualHorizon, DualHorizonCM }, "Dual Horizon", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Red), "DualHrz.C", "Casted Dual Horizon", "Dual Horizon Cast", 0),
                //new EnemyCastStartMechanic(new long [] { GravitationalWave, GravitationalWaveCM }, "Gravitational Wave", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Red), "GravWave.C", "Casted Gravitational Wave", "Gravitational Wave", 0), // TODO: Find effect event
                //new EnemyCastStartMechanic(new long [] { PerniciousVortex, PerniciousVortexCM }, "Pernicious Vortex", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Green), "PrnVrx.C", "Casted Pernicious Vortex", "Pernicious Vortex Cast", 0), // TODO: Find effect event
                //new EnemyCastStartMechanic(new long [] { CracklingWind, CracklingWindCM }, "Crackling Wind", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Blue), "CrckWind.C", "Casted Crackling Wind", "Cracking Wind Cast", 0), // TODO: Find effect event
            }
            );
            Icon = EncounterIconOldLionsCourt;
            Extension = "lioncourt";
            EncounterCategoryInformation.InSubCategoryOrder = 4;
            EncounterID |= 0x000005;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayOldLionsCourt,
                            (1008, 1008),
                            (-1420, 3010, 1580, 6010));
        }
        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.PrototypeVermilion,
                (int)ArcDPSEnums.TargetID.PrototypeIndigo,
                (int)ArcDPSEnums.TargetID.PrototypeArsenite,
                (int)ArcDPSEnums.TargetID.PrototypeVermilionCM,
                (int)ArcDPSEnums.TargetID.PrototypeIndigoCM,
                (int)ArcDPSEnums.TargetID.PrototypeArseniteCM,
            };
        }

        protected override List<int> GetSuccessCheckIDs()
        {
            return new List<int>
            {
            };
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                List<int> idsToCheck;
                if (fightData.IsCM)
                {
                    idsToCheck = new List<int>
                    {
                        (int)ArcDPSEnums.TargetID.PrototypeVermilionCM,
                        (int)ArcDPSEnums.TargetID.PrototypeIndigoCM,
                        (int)ArcDPSEnums.TargetID.PrototypeArseniteCM,
                    };
                } 
                else
                {
                    idsToCheck = new List<int>
                    {
                        (int)ArcDPSEnums.TargetID.PrototypeVermilion,
                        (int)ArcDPSEnums.TargetID.PrototypeIndigo,
                        (int)ArcDPSEnums.TargetID.PrototypeArsenite,
                    };
                }
                SetSuccessByDeath(Targets.Where(x => idsToCheck.Contains(x.ID)).ToList(), combatData, fightData, playerAgents, true);
            }
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Old Lion's Court";
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.PrototypeVermilion,
                (int)ArcDPSEnums.TargetID.PrototypeIndigo,
                (int)ArcDPSEnums.TargetID.PrototypeArsenite,
                (int)ArcDPSEnums.TargetID.PrototypeVermilionCM,
                (int)ArcDPSEnums.TargetID.PrototypeIndigoCM,
                (int)ArcDPSEnums.TargetID.PrototypeArseniteCM,
            };
        }

        private AbstractSingleActor Vermilion()
        {
            return Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.PrototypeVermilionCM)) ?? Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.PrototypeVermilion));
        }
        private AbstractSingleActor Indigo()
        {
            return Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.PrototypeIndigoCM)) ?? Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.PrototypeIndigo));
        }
        private AbstractSingleActor Arsenite()
        {
            return Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.PrototypeArseniteCM)) ?? Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.PrototypeArsenite));
        }

        private static List<PhaseData> GetSubPhases(AbstractSingleActor target, ParsedEvtcLog log, string phaseName)
        {
            DeadEvent dead = log.CombatData.GetDeadEvents(target.AgentItem).LastOrDefault();
            long end = log.FightData.FightEnd;
            long start = log.FightData.FightStart;
            if (dead != null && dead.Time < end)
            {
                end = dead.Time;
            } 
            List<PhaseData> subPhases = GetPhasesByInvul(log, new[] { LeyWovenShielding, MalfunctioningLeyWovenShielding }, target, false, true, start, end);
            string[] phaseNames;
            if (log.FightData.IsCM)
            {
                if (subPhases.Count > 3)
                {
                    return new List<PhaseData>();
                }
                phaseNames = new[]
                {
                    phaseName + " 100% - 60%",
                    phaseName + " 60% - 20%",
                    phaseName + " 20% - 0%",
                };
            }
            else
            {
                if (subPhases.Count > 4)
                {
                    return new List<PhaseData>();
                }
                phaseNames = new[]
                {
                    phaseName + " 100% - 80%",
                    phaseName + " 80% - 40%",
                    phaseName + " 40% - 10%",
                    phaseName + " 10% - 0%",
                };
            }
            for (int i = 0; i < subPhases.Count; i++)
            {
                subPhases[i].Name = phaseNames[i];
                subPhases[i].AddTarget(target);
            }
            return subPhases;
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = base.GetFightOffset(evtcVersion, fightData, agentData, combatData);
            AgentItem vermilion = agentData.GetNPCsByID(ArcDPSEnums.TargetID.PrototypeVermilionCM).FirstOrDefault() ?? agentData.GetNPCsByID(ArcDPSEnums.TargetID.PrototypeVermilion).FirstOrDefault();
            if (vermilion != null)
            {
                CombatItem breakbarStateActive = combatData.FirstOrDefault(x => x.SrcMatchesAgent(vermilion) && x.IsStateChange == ArcDPSEnums.StateChange.BreakbarState && x.Value == 0);
                if (breakbarStateActive != null)
                {
                    startToUse = breakbarStateActive.Time;
                }
            }
            return startToUse;
        }

        internal override List<AbstractBuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
        {
            List<AbstractBuffEvent> toAdd = base.SpecialBuffEventProcess(combatData, skillData);
            var shields = combatData.GetBuffData(LeyWovenShielding).GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<AbstractBuffEvent>> pair in shields)
            {
                // Missing Buff Initial
                if (pair.Value.FirstOrDefault() is AbstractBuffRemoveEvent)
                {
                    toAdd.Add(new BuffApplyEvent(pair.Key, pair.Key, pair.Key.FirstAware, int.MaxValue, skillData.Get(LeyWovenShielding), ArcDPSEnums.IFF.Friend, 1, true));
                }
            }
            return toAdd;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor vermilion = Vermilion();
            bool canComputePhases = vermilion != null && vermilion.HasBuff(log, LeyWovenShielding, 500); // check that vermilion is present and starts shielded, otherwise clearly incomplete log
            if (vermilion != null)
            {
                phases[0].AddTarget(vermilion);
                if (canComputePhases)
                {
                    phases.AddRange(GetSubPhases(vermilion, log, "Vermilion"));
                }
            }
            AbstractSingleActor indigo = Indigo();
            if (indigo != null)
            {
                phases[0].AddTarget(indigo);
                if (canComputePhases)
                {
                    phases.AddRange(GetSubPhases(indigo, log, "Indigo"));
                }
            }
            AbstractSingleActor arsenite = Arsenite();
            if (arsenite != null)
            {
                phases[0].AddTarget(arsenite);
                if (canComputePhases)
                {
                    phases.AddRange(GetSubPhases(arsenite, log, "Arsenite"));
                }
            }
            return phases;
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Vermilion() ?? Indigo() ?? Arsenite();
            if (target == null)
            {
                throw new MissingKeyActorsException("Main target not found");
            }
            return target.GetHealth(combatData) > 20e6 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);
            IReadOnlyList<AbstractBuffEvent> fearNotThisKnight = log.CombatData.GetBuffData(AchievementEligibilityFearNotThisKnight);
            
            if (fearNotThisKnight.Any() && log.FightData.Success)
            {
                foreach (Player p in log.PlayerList)
                {
                    if (p.HasBuff(log, AchievementEligibilityFearNotThisKnight, log.FightData.FightEnd - ServerDelayConstant))
                    {
                        InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityFearNotThisKnight], 1));
                        break;
                    }
                }
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // Fixation
            IEnumerable<AbstractBuffEvent> fixations = log.CombatData.GetBuffData(FixatedOldLionsCourt).Where(buff => buff.To == p.AgentItem);
            IEnumerable<AbstractBuffEvent> fixatedVermillion = fixations.Where(bae => bae.CreditedBy.IsAnySpecies(new List<ArcDPSEnums.TargetID> { ArcDPSEnums.TargetID.PrototypeVermilion, ArcDPSEnums.TargetID.PrototypeVermilionCM }));
            IEnumerable<AbstractBuffEvent> fixatedArsenite = fixations.Where(bae => bae.CreditedBy.IsAnySpecies(new List<ArcDPSEnums.TargetID> { ArcDPSEnums.TargetID.PrototypeArsenite, ArcDPSEnums.TargetID.PrototypeArseniteCM }));
            IEnumerable<AbstractBuffEvent> fixatedIndigo = fixations.Where(bae => bae.CreditedBy.IsAnySpecies(new List<ArcDPSEnums.TargetID> { ArcDPSEnums.TargetID.PrototypeIndigo, ArcDPSEnums.TargetID.PrototypeIndigoCM }));

            AddFixatedDecorations(p, log, replay, fixatedVermillion, ParserIcons.FixationRedOverhead);
            AddFixatedDecorations(p, log, replay, fixatedArsenite, ParserIcons.FixationGreenOverhead);
            AddFixatedDecorations(p, log, replay, fixatedIndigo, ParserIcons.FixationBlueOverhead);
        }

        /// <summary>
        /// Adds the Fixated decorations.
        /// </summary>
        /// <param name="player">Player for the decoration.</param>
        /// <param name="replay">Combat Replay.</param>
        /// <param name="fixations">The <see cref="AbstractBuffEvent"/> where the buff appears.</param>
        /// <param name="icon">The icon related to the respective buff.</param>
        private static void AddFixatedDecorations(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay, IEnumerable<AbstractBuffEvent> fixations, string icon)
        {
            IEnumerable<AbstractBuffEvent> applications = fixations.Where(x => x is BuffApplyEvent);
            IEnumerable<AbstractBuffEvent> removals = fixations.Where(x => x is BuffRemoveAllEvent);
            foreach (BuffApplyEvent bae in applications.Cast<BuffApplyEvent>())
            {
                long start = bae.Time;
                long end = removals.FirstOrDefault(x => x.Time > start) != null ? removals.FirstOrDefault(x => x.Time > start).Time : log.FightData.LogEnd;
                replay.Decorations.Add(new IconOverheadDecoration(icon, 20, 1, ((int)start, (int)end), new AgentConnector(player)));
            }
        }
    }
}
