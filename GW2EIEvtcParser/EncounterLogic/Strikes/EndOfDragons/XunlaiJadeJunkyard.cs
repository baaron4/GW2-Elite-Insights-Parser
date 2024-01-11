using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class XunlaiJadeJunkyard : EndOfDragonsStrike
    {
        public XunlaiJadeJunkyard(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(GraspingHorror, "GraspingHorror", new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.LightOrange), "Hands.H", "Hit by Hands AoE", "Hands Hit", 150),
                new PlayerDstHitMechanic(DeathsEmbraceSkill, "Death's Embrace", new MechanicPlotlySetting(Symbols.CircleCross, Colors.DarkRed), "AnkkaPull.H", "Hit by Death's Embrace (Ankka's Pull)", "Death's Embrace Hit", 150),
                new PlayerDstHitMechanic(DeathsHandInBetween, "Death's Hand", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Yellow), "Sctn.AoE.H", "Hit by in-between sections AoE", "Death's Hand Hit (transitions)", 150),
                new PlayerDstHitMechanic(DeathsHandDropped, "Death's Hand", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "Sprd.AoE.H", "Hit by placeable Death's Hand AoE", "Death's Hand Hit (placeable)", 150),
                new PlayerDstHitMechanic(WallOfFear, "Wall of Fear", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkRed), "Krait.H", "Hit by Krait AoE", "Krait Hit", 150),
                new PlayerDstHitMechanic(new long[] { WaveOfTormentNM, WaveOfTormentCM }, "Wave of Torment", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Quaggan.H", "Hit by Quaggan Explosion", "Quaggan Hit", 150),
                new PlayerDstHitMechanic(TerrifyingApparition, "Terrifying Apparition", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkRed), "Lich.H", "Hit by Lich AoE", "Lich Hit", 150),
                new PlayerDstHitMechanic(new long[] { WallOfFear, WaveOfTormentNM, WaveOfTormentCM, TerrifyingApparition }, "Clarity", new MechanicPlotlySetting(Symbols.DiamondTall, Colors.Blue), "Clarity.Achiv", "Achievement Eligibility: Clarity", "Achiv Clarity", 150).UsingAchievementEligibility(true),
                new PlayerDstBuffApplyMechanic(AnkkaLichHallucinationFixation, "Lich Fixation", new MechanicPlotlySetting(Symbols.Diamond, Colors.LightBlue), "Lich.H.F", "Fixated by Lich Hallucination", "Lich Fixation", 150),
                new PlayerDstHitMechanic(new long[] { ZhaitansReachThrashXJJ1, ZhaitansReachThrashXJJ2 }, "Thrash", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.DarkGreen), "ZhtRch.Pull", "Pulled by Zhaitan's Reach", "Zhaitan's Reach Pull", 150),
                new PlayerDstHitMechanic(new long[] { ZhaitansReachGroundSlam, ZhaitansReachGroundSlamXJJ }, "Ground Slam", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.DarkGreen), "ZhtRch.Knck", "Knocked by Zhaitan's Reach", "Zhaitan's Reach Knock", 150),
                new PlayerDstHitMechanic(ImminentDeathSkill, "Imminent Death", new MechanicPlotlySetting(Symbols.DiamondTall, Colors.Green), "Imm.Death.H", "Hit by Imminent Death", "Imminent Death Hit", 0),
                new EnemyCastStartMechanic(InevitabilityOfDeath, "Inevitability of Death", new MechanicPlotlySetting(Symbols.Octagon, Colors.LightRed), "Inev.Death.C", "Casted Inevitability of Death (Enrage)", "Inevitability of Death (Enrage)", 150),
                new EnemyCastStartMechanic(DeathsEmbraceSkill, "Death's Embrace", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Blue), "AnkkaPull.C", "Casted Death's Embrace", "Death's Embrace Cast", 150),
                new EnemyDstBuffApplyMechanic(PowerOfTheVoid, "Power of the Void", new MechanicPlotlySetting(Symbols.Star, Colors.Yellow), "Pwrd.Up", "Ankka has powered up", "Ankka powered up", 150),
                new PlayerDstBuffApplyMechanic(ImminentDeathBuff, "Imminent Death", new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Green), "Imm.Death.B", "Placed Death's Hand AoE and gained Imminent Death Buff", "Imminent Death Buff", 150),
                new PlayerDstBuffApplyMechanic(FixatedAnkkaKainengOverlook, "Fixated", new MechanicPlotlySetting(Symbols.Diamond, Colors.Purple), "Fxt.Hatred", "Fixated by Reanimated Hatred", "Fixated Hatred", 150),
                new PlayerDstBuffApplyMechanic(Hallucinations, "Hallucinations", new MechanicPlotlySetting(Symbols.Square, Colors.LightBlue), "Hallu", "Received Hallucinations Debuff", "Hallucinations Debuff", 150),
                new PlayerDstBuffApplyMechanic(DeathsHandSpreadBuff, "Death's Hand Spread", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Green), "Sprd.AoE.B", "Received Death's Hand Spread", "Death's Hand Spread", 150),
                new PlayerDstBuffApplyMechanic(DevouringVoid, "Devouring Void", new MechanicPlotlySetting(Symbols.DiamondWide, Colors.LightBlue), "DevVoid.B", "Received Devouring Void", "Devouring Void Applied", 150),
                new PlayerDstBuffApplyMechanic(DevouringVoid, "Undevoured", new MechanicPlotlySetting(Symbols.DiamondWide, Colors.Blue), "Undev.Achiv", "Achievement Eligibility: Undevoured", "Achiv Undevoured", 150).UsingAchievementEligibility(true).UsingEnable(x => x.FightData.IsCM),
            }
            );
            Icon = EncounterIconXunlaiJadeJunkyard;
            Extension = "xunjadejunk";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Xunlai Jade Junkyard";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayXunlaiJadeJunkyard,
                            (1485, 1292),
                            (-7090, -2785, 3647, 6556)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Ankka));
            if (ankka == null)
            {
                throw new MissingKeyActorsException("Ankka not found");
            }
            phases[0].AddTarget(ankka);
            if (!requirePhases)
            {
                return phases;
            }

            // DPS Phases
            List<PhaseData> dpsPhase = GetPhasesByInvul(log, Determined895, ankka, false, true);
            for (int i = 0; i < dpsPhase.Count; i++)
            {
                dpsPhase[i].Name = $"DPS Phase {i + 1}";
                dpsPhase[i].AddTarget(ankka);
            }
            phases.AddRange(dpsPhase);

            // Necrotic Rituals
            List<PhaseData> rituals = GetPhasesByInvul(log, NecroticRitual, ankka, true, true);
            for (int i = 0; i < rituals.Count; i++)
            {
                if (i % 2 != 0)
                {
                    rituals[i].Name = $"Necrotic Ritual {(i + 1) / 2}";
                    rituals[i].AddTarget(ankka);
                }
            }
            phases.AddRange(rituals);

            // Health and Transition Phases
            List<PhaseData> subPhases = GetPhasesByInvul(log, AnkkaPlateformChanging, ankka, true, true);
            for (int i = 0; i < subPhases.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        subPhases[i].Name = "Phase 100-75%";
                        break;
                    case 1:
                        subPhases[i].Name = "Transition 1";
                        break;
                    case 2:
                        subPhases[i].Name = "Phase 75-40%";
                        break;
                    case 3:
                        subPhases[i].Name = "Transition 2";
                        break;
                    case 4:
                        subPhases[i].Name = "Phase 40-0%";
                        break;
                    default:
                        break;
                }
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
                AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Ankka));
                if (ankka == null)
                {
                    throw new MissingKeyActorsException("Ankka not found");
                }
                var buffApplies = combatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To == ankka.AgentItem && !x.Initial && x.AppliedDuration > int.MaxValue / 2 && x.Time >= fightData.FightStart + 5000).ToList();
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
                (int)ArcDPSEnums.TrashID.ReanimatedAntipathy,
                (int)ArcDPSEnums.TrashID.ReanimatedSpite,
            };
        }

        protected override Dictionary<int, int> GetTargetsSortIDs()
        {
            return new Dictionary<int, int>()
            {
                {(int)ArcDPSEnums.TargetID.Ankka, 0 },
                {(int)ArcDPSEnums.TrashID.ReanimatedAntipathy, 1 },
                {(int)ArcDPSEnums.TrashID.ReanimatedSpite, 1 },
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Ankka,
                ArcDPSEnums.TrashID.ReanimatedMalice1,
                ArcDPSEnums.TrashID.ReanimatedMalice2,
                ArcDPSEnums.TrashID.ReanimatedHatred,
                ArcDPSEnums.TrashID.ZhaitansReach,
                ArcDPSEnums.TrashID.KraitsHallucination,
                ArcDPSEnums.TrashID.LichHallucination,
                ArcDPSEnums.TrashID.QuaggansHallucinationNM,
                ArcDPSEnums.TrashID.QuaggansHallucinationCM,
                ArcDPSEnums.TrashID.SanctuaryPrism,
            };
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Ankka));
            if (ankka == null)
            {
                throw new MissingKeyActorsException("Ankka not found");
            }
            MapIDEvent map = combatData.GetMapIDEvents().FirstOrDefault();
            if (map != null && map.MapID == 1434)
            {
                return FightData.EncounterMode.Story;
            }
            return ankka.GetHealth(combatData) > 50e6 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var sanctuaryPrism = combatData.Where(x => x.DstAgent == 14940 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 16).ToList();
            foreach (AgentItem sanctuary in sanctuaryPrism)
            {
                IEnumerable<CombatItem> items = combatData.Where(x => x.SrcMatchesAgent(sanctuary) && x.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate && x.DstAgent == 0);
                sanctuary.OverrideType(AgentItem.AgentType.NPC);
                sanctuary.OverrideID(ArcDPSEnums.TrashID.SanctuaryPrism);
                sanctuary.OverrideAwareTimes(fightData.LogStart, items.Any() ? items.FirstOrDefault().Time : fightData.LogEnd);
            }
            agentData.Refresh();
            ComputeFightTargets(agentData, combatData, extensions);
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);

            if (log.FightData.Success && log.FightData.IsCM && CustomCheckGazeIntoTheVoidEligibility(log))
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityGazeIntoTheVoid], 1));
            }
        }

        private static bool CustomCheckGazeIntoTheVoidEligibility(ParsedEvtcLog log)
        {
            IReadOnlyList<AgentItem> agents = log.AgentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Ankka);

            foreach (AgentItem agent in agents)
            {
                IReadOnlyDictionary<long, BuffsGraphModel> bgms = log.FindActor(agent).GetBuffGraphs(log);
                if (bgms != null && bgms.TryGetValue(PowerOfTheVoid, out BuffsGraphModel bgm))
                {
                    if (bgm.BuffChart.Any(x => x.Value == 6)) { return true; }
                }
            }
            return false;
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Ankka:
                    var deathsEmbraces = casts.Where(x => x.SkillId == DeathsEmbraceSkill).ToList();
                    int deathsEmbraceCastDuration = 10143;
                    foreach (AbstractCastEvent deathEmbrace in deathsEmbraces)
                    {
                        int endTime = (int)deathEmbrace.Time + deathsEmbraceCastDuration;

                        Point3D ankkaPosition = target.GetCurrentPosition(log, deathEmbrace.Time);
                        if (ankkaPosition == null) { continue; }

                        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.DeathsEmbrace, out IReadOnlyList<EffectEvent> deathsEmbraceEffects))
                        {
                            uint radius = 500; // Zone 1
                            // Zone 2
                            if (ankkaPosition.X > 0 && ankkaPosition.X < 4000)
                            {
                                radius = 340;
                            }
                            // Zone 3
                            if (ankkaPosition.Y > 4000 && ankkaPosition.Y < 6000)
                            {
                                radius = 380;
                            }
                            var effects = deathsEmbraceEffects.Where(x => x.Time >= deathEmbrace.Time && x.Time <= deathEmbrace.EndTime).ToList();
                            foreach (EffectEvent effectEvt in effects)
                            {
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, radius, (int)(effectEvt.Time - deathEmbrace.Time), effectEvt.Position);
                            }
                        }
                        else
                        {
                            // logs without effects
                            int delay = 1833 * 2;
                            // Zone 1
                            if (ankkaPosition.X > -6000 && ankkaPosition.X < -2500 && ankkaPosition.Y < 1000 && ankkaPosition.Y > -1000)
                            {
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 500, delay, new Point3D(-3941.78f, 66.76819f, -3611.2f)); // CENTER
                            }
                            // Zone 2
                            if (ankkaPosition.X > 0 && ankkaPosition.X < 4000)
                            {
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 340, delay, new Point3D(1663.69f, 1739.87f, -4639.695f)); // NW
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 340, delay, new Point3D(2563.689f, 1739.87f, -4664.611f)); // NE
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 340, delay, new Point3D(1663.69f, 839.8699f, -4640.633f)); // SW
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 340, delay, new Point3D(2563.689f, 839.8699f, -4636.368f)); // SE
                            }
                            // Zone 3
                            if (ankkaPosition.Y > 4000 && ankkaPosition.Y < 6000)
                            {
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 380, delay, new Point3D(-2547.61f, 5466.439f, -6257.504f)); // NW
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 380, delay, new Point3D(-1647.61f, 5466.439f, -6256.795f)); // NE
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 380, delay, new Point3D(-2547.61f, 4566.439f, -6256.799f)); // SW
                                AddDeathEmbraceDecoration(replay, (int)deathEmbrace.Time, deathsEmbraceCastDuration, 380, delay, new Point3D(-1647.61f, 4566.439f, -6257.402f)); // SE
                            }
                        }
                    }
                    //
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DeathsHandByAnkkaRadius300, out IReadOnlyList<EffectEvent> deathsHandOnPlayerNM))
                    {
                        foreach (EffectEvent deathsHandEffect in deathsHandOnPlayerNM)
                        {
                            if (log.CombatData.GetBuffRemoveAllData(DeathsHandSpreadBuff).Any(x => Math.Abs(x.Time - deathsHandEffect.Time) < ServerDelayConstant))
                            {
                                AddDeathsHandDecoration(replay, deathsHandEffect.Position, (int)deathsHandEffect.Time, 3000, 300, 13000);
                            }
                        }
                    }
                    //
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DeathsHandByAnkkaRadius380, out IReadOnlyList<EffectEvent> deathsHandOnPlayerCMOrInBetween))
                    {
                        foreach (EffectEvent deathsHandEffect in deathsHandOnPlayerCMOrInBetween)
                        {
                            if (!log.CombatData.GetBuffRemoveAllData(DeathsHandSpreadBuff).Any(x => Math.Abs(x.Time - deathsHandEffect.Time) < ServerDelayConstant))
                            {
                                // One also happens during death's embrace so we filter that one out
                                if (!deathsEmbraces.Any(x => x.Time <= deathsHandEffect.Time && x.Time + deathsEmbraceCastDuration >= deathsHandEffect.Time))
                                {
                                    AddDeathsHandDecoration(replay, deathsHandEffect.Position, (int)deathsHandEffect.Time, 3000, 380, 1000);
                                }
                            } 
                            else if (log.FightData.IsCM)
                            {
                                AddDeathsHandDecoration(replay, deathsHandEffect.Position, (int)deathsHandEffect.Time, 3000, 380, 33000);
                            }
                        }
                    }

                    // Power of the Void
                    IEnumerable<Segment> potvSegments = target.GetBuffStatus(log, PowerOfTheVoid, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
                    replay.AddOverheadIcons(potvSegments, target, ParserIcons.PowerOfTheVoidOverhead);
                    break;
                case (int)ArcDPSEnums.TrashID.KraitsHallucination:
                    // Wall of Fear
                    long firstMovementTime = target.FirstAware + 2550;
                    uint kraitsRadius = 420;

                    replay.Decorations.Add(new CircleDecoration(kraitsRadius, (target.FirstAware, firstMovementTime), Colors.Orange, 0.2, new AgentConnector(target)).UsingGrowingEnd(firstMovementTime));
                    replay.Decorations.Add(new CircleDecoration(kraitsRadius, (firstMovementTime, target.LastAware), Colors.Red, 0.2, new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.LichHallucination:
                    // Terrifying Apparition
                    long awareTime = target.FirstAware + 1000;
                    uint lichRadius = 280;

                    replay.Decorations.Add(new CircleDecoration(lichRadius, (target.FirstAware, awareTime), Colors.Orange, 0.2, new AgentConnector(target)).UsingGrowingEnd(awareTime));
                    replay.Decorations.Add(new CircleDecoration(lichRadius, (awareTime, target.LastAware), Colors.Red, 0.2, new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.QuaggansHallucinationNM:
                    var waveOfTormentNM = casts.Where(x => x.SkillId == WaveOfTormentNM).ToList();
                    foreach (AbstractCastEvent c in waveOfTormentNM)
                    {
                        int castTime = 2800;
                        uint radius = 300;
                        int endTime = (int)c.Time + castTime;
                        replay.AddDecorationWithGrowing(new CircleDecoration(radius, (c.Time, endTime), Colors.Orange, 0.2, new AgentConnector(target)), endTime);
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.QuaggansHallucinationCM:
                    var waveOfTormentCM = casts.Where(x => x.SkillId == WaveOfTormentCM).ToList();
                    foreach (AbstractCastEvent c in waveOfTormentCM)
                    {
                        int castTime = 5600;
                        uint radius = 450;
                        int endTime = (int)c.Time + castTime;
                        replay.AddDecorationWithGrowing(new CircleDecoration(radius, (c.Time, endTime), Colors.Orange, 0.2, new AgentConnector(target)), endTime);
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.ZhaitansReach:
                    // Thrash - Circle that pulls in
                    var thrash = casts.Where(x => x.SkillId == ZhaitansReachThrashXJJ1 || x.SkillId == ZhaitansReachThrashXJJ2).ToList();
                    foreach (AbstractCastEvent c in thrash)
                    {
                        int castTime = 1900;
                        int endTime = (int)c.Time + castTime;
                        replay.AddDecorationWithGrowing(new DoughnutDecoration(300, 500, (c.Time, endTime), Colors.Orange, 0.2, new AgentConnector(target)), endTime);
                    }
                    // Ground Slam - AoE that knocks out
                    var groundSlam = casts.Where(x => x.SkillId == ZhaitansReachGroundSlam || x.SkillId == ZhaitansReachGroundSlamXJJ).ToList();
                    foreach (AbstractCastEvent c in groundSlam)
                    {
                        int castTime = 0;
                        uint radius = 400;
                        int endTime = 0;
                        // 66534 -> Fast AoE -- 66397 -> Slow AoE
                        if (c.SkillId == ZhaitansReachGroundSlam) { castTime = 800; } else if (c.SkillId == ZhaitansReachGroundSlamXJJ) { castTime = 2500; }
                        endTime = (int)c.Time + castTime;
                        replay.AddDecorationWithGrowing(new CircleDecoration(radius, (c.Time, endTime), Colors.Orange, 0.2, new AgentConnector(target)), endTime);
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.ReanimatedSpite:
                    break;
                case (int)ArcDPSEnums.TrashID.SanctuaryPrism:
                    if (!log.FightData.IsCM)
                    {
                        replay.Trim(log.FightData.LogStart, log.FightData.LogStart);
                    }
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            if (p.GetBuffGraphs(log).TryGetValue(DeathsHandSpreadBuff, out BuffsGraphModel value))
            {
                foreach (Segment segment in value.BuffChart)
                {
                    if (segment != null && segment.Start > 0 && segment.Value == 1)
                    {
                        uint deathsHandRadius = (uint)(log.FightData.IsCM ? 380 : 300);
                        int deathsHandDuration = log.FightData.IsCM ? 33000 : 13000;
                        // AoE on player
                        replay.AddDecorationWithGrowing(new CircleDecoration(deathsHandRadius, segment, Colors.Orange, 0.2, new AgentConnector(p)), segment.End);
                        // Logs without effects, we add the dropped AoE manually
                        if (!log.CombatData.HasEffectData)
                        {
                            ParametricPoint3D playerPosition = p.GetCombatReplayPolledPositions(log).Where(x => x.Time <= (int)segment.End).LastOrDefault();
                            if (playerPosition != null)
                            {
                                AddDeathsHandDecoration(replay, playerPosition, (int)segment.End, 3000, deathsHandRadius, deathsHandDuration);
                            }
                        }
                    }
                }
            }
            // Tethering Players to Lich
            List<AbstractBuffEvent> lichTethers = GetFilteredList(log.CombatData, AnkkaLichHallucinationFixation, p, true, true);
            replay.AddTether(lichTethers, Colors.Teal, 0.5);

            // Reanimated Hatred Fixation
            IEnumerable<Segment> hatredFixations = p.GetBuffStatus(log, FixatedAnkkaKainengOverlook, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            replay.AddOverheadIcons(hatredFixations, p, ParserIcons.FixationPurpleOverhead);
            // Reanimated Hatred Tether to player - The buff is applied by Ankka to the player - The Reanimated Hatred spawns before the buff application
            replay.AddTetherByThirdPartySrcBuff(log, p, FixatedAnkkaKainengOverlook, (int)ArcDPSEnums.TargetID.Ankka, (int)ArcDPSEnums.TrashID.ReanimatedHatred, Colors.Magenta, 0.5);
        }

        private static void AddDeathsHandDecoration(CombatReplay replay, Point3D position, int start, int delay, uint radius, int duration)
        {
            int deathHandGrowStart = start;
            int deathHandGrowEnd = deathHandGrowStart + delay;
            // Growing AoE
            replay.AddDecorationWithGrowing(new CircleDecoration(radius, (deathHandGrowStart, deathHandGrowEnd), Colors.Orange, 0.2, new PositionConnector(position)), deathHandGrowEnd);
            // Damaging AoE
            int AoEStart = deathHandGrowEnd;
            int AoEEnd = AoEStart + duration;
            replay.AddDecorationWithBorder(new CircleDecoration(radius, (AoEStart, AoEEnd), "rgba(0, 100, 0, 0.3)", new PositionConnector(position)), Colors.Red, 0.4);
        }

        private static void AddDeathEmbraceDecoration(CombatReplay replay, int startCast, int durationCast, uint radius, int delay, Point3D position)
        {
            int endTime = startCast + durationCast;
            var connector = new PositionConnector(position);
            replay.Decorations.Add(new CircleDecoration(radius, (startCast, startCast + delay), Colors.Orange, 0.2, connector).UsingGrowingEnd(startCast + delay));
            replay.Decorations.Add(new CircleDecoration(radius, (startCast + delay, endTime), Colors.Red, 0.2, connector));
        }
    }
}
