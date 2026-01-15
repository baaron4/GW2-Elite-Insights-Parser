using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.AchievementEligibilityIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class XunlaiJadeJunkyard : EndOfDragonsRaidEncounter
{
    public XunlaiJadeJunkyard(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(GraspingHorror, new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.LightOrange), "Hands.H", "Hit by Hands AoE", "Hands Hit", 150),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(DeathsEmbraceSkill, new MechanicPlotlySetting(Symbols.CircleCross, Colors.DarkRed), "AnkkaPull.H", "Hit by Death's Embrace (Ankka's Pull)", "Death's Embrace Hit", 150),
                    new EnemyCastStartMechanic(DeathsEmbraceSkill, new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Blue), "AnkkaPull.C", "Casted Death's Embrace", "Death's Embrace Cast", 150),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(DeathsHandInBetween, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Yellow), "Sctn.AoE.H", "Hit by in-between sections AoE", "Death's Hand Hit (transitions)", 150),
                new PlayerDstHealthDamageHitMechanic(DeathsHandDropped, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "Sprd.AoE.H", "Hit by placeable Death's Hand AoE", "Death's Hand Hit (placeable)", 150),
                new PlayerDstBuffApplyMechanic(DeathsHandSpreadBuff, new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Green), "Sprd.AoE.B", "Received Death's Hand Spread", "Death's Hand Spread", 150),
                new MechanicGroup([
                    new PlayerDstHealthDamageHitMechanic(ImminentDeathSkill, new MechanicPlotlySetting(Symbols.DiamondTall, Colors.Green), "Imm.Death.H", "Hit by Imminent Death", "Imminent Death Hit", 0),
                    new PlayerDstBuffApplyMechanic(ImminentDeathBuff, new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Green), "Imm.Death.B", "Placed Death's Hand AoE and gained Imminent Death Buff", "Imminent Death Buff", 150),
                ]),
            ]),
            // Extra adds
            new MechanicGroup([
                // Kraits
                new MechanicGroup([
                    new PlayerDstHealthDamageHitMechanic(WallOfFear, new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkRed), "Krait.H", "Hit by Krait AoE", "Krait Hit", 150),
                ]),
                // Quaggans
                new MechanicGroup([
                    new PlayerDstHealthDamageHitMechanic([WaveOfTormentNM, WaveOfTormentCM], new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Quaggan.H", "Hit by Quaggan Explosion", "Quaggan Hit", 150),
                ]),
                // Lich
                new MechanicGroup([
                    new PlayerDstHealthDamageHitMechanic(TerrifyingApparition, new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkRed), "Lich.H", "Hit by Lich AoE", "Lich Hit", 150),
                    new PlayerDstBuffApplyMechanic(AnkkaLichHallucinationFixation, new MechanicPlotlySetting(Symbols.Diamond, Colors.LightBlue), "Lich.H.F", "Fixated by Lich Hallucination", "Lich Fixation", 150),
                ]),
                new MechanicGroup([
                    new AchievementEligibilityMechanic(Ach_Clarity, new MechanicPlotlySetting(Symbols.DiamondTall, Colors.DarkBlue), "Clarity.Achiv.L", "Achievement Eligibility: Clarity Lost", "Achiv Clarity Lost", 0)
                            .UsingChecker((evt, log) => evt.Lost),
                    new AchievementEligibilityMechanic(Ach_Clarity, new MechanicPlotlySetting(Symbols.DiamondTall, Colors.Blue), "Clarity.Achiv.K", "Achievement Eligibility: Clarity Kept", "Achiv Clarity Kept", 0)
                            .UsingChecker((evt, log) => !evt.Lost)
                ]),
                // Reaches
                new MechanicGroup([
                    new PlayerDstHealthDamageHitMechanic([ZhaitansReachThrashXJJ1, ZhaitansReachThrashXJJ2], new MechanicPlotlySetting(Symbols.CircleOpen, Colors.DarkGreen), "ZhtRch.Pull", "Pulled by Zhaitan's Reach", "Zhaitan's Reach Pull", 150),
                    new PlayerDstHealthDamageHitMechanic([ZhaitansReachGroundSlam, ZhaitansReachGroundSlamXJJ], new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.DarkGreen), "ZhtRch.Knck", "Knocked by Zhaitan's Reach", "Zhaitan's Reach Knock", 150),
                ]),
                // Hallucinations
                new MechanicGroup([
                    new PlayerDstBuffApplyMechanic(Hallucinations, new MechanicPlotlySetting(Symbols.Square, Colors.LightBlue), "Hallu", "Received Hallucinations Debuff", "Hallucinations Debuff", 150),
                ]),
                // Hatred
                new MechanicGroup([
                    new PlayerDstBuffApplyMechanic(FixatedAnkkaKainengOverlook, new MechanicPlotlySetting(Symbols.Diamond, Colors.Purple), "Fxt.Hatred", "Fixated by Reanimated Hatred", "Fixated Hatred", 150),
                ]),
            ]),
            new EnemyCastStartMechanic(InevitabilityOfDeath, new MechanicPlotlySetting(Symbols.Octagon, Colors.LightRed), "Inev.Death.C", "Casted Inevitability of Death (Enrage)", "Inevitability of Death (Enrage)", 150),
            new EnemyDstBuffApplyMechanic(PowerOfTheVoid, new MechanicPlotlySetting(Symbols.Star, Colors.Yellow), "Pwrd.Up", "Ankka has powered up", "Ankka powered up", 150),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(DevouringVoid, new MechanicPlotlySetting(Symbols.DiamondWide, Colors.LightBlue), "DevVoid.B", "Received Devouring Void", "Devouring Void Applied", 150),
                new MechanicGroup([
                    new AchievementEligibilityMechanic(Ach_Undevoured, new MechanicPlotlySetting(Symbols.DiamondWide, Colors.DarkBlue), "Undev.Achiv.L", "Achievement Eligibility: Undevoured Lost", "Achiv Undevoured Lost", 0)
                            .UsingChecker((evt, log) => evt.Lost),
                    new AchievementEligibilityMechanic(Ach_Undevoured, new MechanicPlotlySetting(Symbols.DiamondWide, Colors.Blue), "Undev.Achiv.K", "Achievement Eligibility: Undevoured Kept", "Achiv Undevoured Kept", 0)
                            .UsingChecker((evt, log) => !evt.Lost)
                ]),
            ]),
        ])
        );
        Icon = EncounterIconXunlaiJadeJunkyard;
        Extension = "xunjadejunk";
        LogCategoryInformation.InSubCategoryOrder = 1;
        LogID |= 0x000002;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Xunlai Jade Junkyard";
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (1485, 1292),
                        (-7090, -2785, 3647, 6556));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayXunlaiJadeJunkyard, crMap);
        return crMap;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Ankka)) ?? throw new MissingKeyActorsException("Ankka not found");
        phases[0].AddTarget(ankka, log);
        if (!requirePhases)
        {
            return phases;
        }

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
            subPhases[i].AddParentPhase(phases[0]);
            subPhases[i].AddTarget(ankka, log);
        }
        phases.AddRange(subPhases);
        // DPS Phases
        List<PhaseData> dpsPhase = GetPhasesByInvul(log, Determined895, ankka, false, true);
        for (int i = 0; i < dpsPhase.Count; i++)
        {
            dpsPhase[i].Name = $"DPS Phase {i + 1}";
            dpsPhase[i].AddTarget(ankka, log);
            dpsPhase[i].AddParentPhases(subPhases);
            // We are not using the same buff between the two types of phases, the timings may slightly differ, this makes sure to put a dps phase within a fight phase
            var currentSubPhase = subPhases.FirstOrDefault(x => x.IntersectsWindow(dpsPhase[i].Start, dpsPhase[i].End));
            if (currentSubPhase != null)
            {
                dpsPhase[i].OverrideStart(Math.Max(dpsPhase[i].Start, currentSubPhase.Start));
                dpsPhase[i].OverrideEnd(Math.Min(dpsPhase[i].End, currentSubPhase.End));
            }
        }
        phases.AddRange(dpsPhase);
        // Necrotic Rituals
        List<PhaseData> rituals = GetPhasesByInvul(log, NecroticRitual, ankka, true, true);
        for (int i = 0; i < rituals.Count; i++)
        {
            if (i % 2 != 0)
            {
                rituals[i].Name = $"Necrotic Ritual {(i + 1) / 2}";
                rituals[i].AddTarget(ankka, log);
                rituals[i].AddParentPhases(subPhases);
            }
        }
        phases.AddRange(rituals);
        //
        return phases;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        base.CheckSuccess(combatData, agentData, logData, playerAgents);
        if (!logData.Success)
        {
            SingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Ankka)) ?? throw new MissingKeyActorsException("Ankka not found");
            var buffApplies = combatData.GetBuffApplyDataByIDByDst(Determined895, ankka.AgentItem).OfType<BuffApplyEvent>().Where(x => !x.Initial && x.AppliedDuration > int.MaxValue / 2 && x.Time >= logData.LogStart + 5000);
            if (buffApplies.Count() == 3)
            {
                logData.SetSuccess(true, buffApplies.Last().Time);
            }
        }
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.Ankka,
            TargetID.ReanimatedAntipathy,
            TargetID.ReanimatedSpite,
        ];
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>
        {
            {TargetID.Ankka, 0 },
            {TargetID.ReanimatedAntipathy, 1 },
            {TargetID.ReanimatedSpite, 1 },
        };
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.Ankka2,
            TargetID.ReanimatedMalice1,
            TargetID.ReanimatedMalice2,
            TargetID.ReanimatedHatred,
            TargetID.ZhaitansReach,
            TargetID.KraitsHallucination,
            TargetID.LichHallucination,
            TargetID.QuaggansHallucinationNM,
            TargetID.QuaggansHallucinationCM,
            TargetID.SanctuaryPrism,
        ];
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        SingleActor ankka = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Ankka)) ?? throw new MissingKeyActorsException("Ankka not found");
        MapIDEvent? map = combatData.GetMapIDEvents().FirstOrDefault();
        if (map != null && map.MapID == MapIDs.XunlaijadeJunkyardStory)
        {
            return LogData.Mode.Story;
        }
        return ankka.GetHealth(combatData) > 50e6 ? LogData.Mode.CM : LogData.Mode.Normal;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var sanctuaryPrism = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 14940 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 16);
        foreach (AgentItem sanctuary in sanctuaryPrism)
        {
            IEnumerable<CombatItem> items = combatData.Where(x => x.SrcMatchesAgent(sanctuary) && x.IsStateChange == StateChange.HealthUpdate && HealthUpdateEvent.GetHealthPercent(x) == 0);
            sanctuary.OverrideType(AgentItem.AgentType.NPC, agentData);
            sanctuary.OverrideID(TargetID.SanctuaryPrism, agentData);
            sanctuary.OverrideAwareTimes(logData.EvtcLogStart, items.Any() ? items.First().Time : logData.EvtcLogEnd);
        }
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);

        var encounterPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID);

        foreach (var encounterPhase in encounterPhases)
        {
            if (encounterPhase.Success && encounterPhase.IsCM && CustomCheckGazeIntoTheVoidEligibility(log, encounterPhase))
            {
                instanceBuffs.Add(new(log.Buffs.BuffsByIDs[AchievementEligibilityGazeIntoTheVoid], 1, encounterPhase));
            }
        }
    }

    private static bool CustomCheckGazeIntoTheVoidEligibility(ParsedEvtcLog log, EncounterPhaseData encounterPhase)
    {
        foreach (var ankka in encounterPhase.Targets.Where(x => x.Key.IsSpecies(TargetID.Ankka)))
        {
            IReadOnlyDictionary<long, BuffGraph> bgms = ankka.Key.GetBuffGraphs(log);
            if (bgms != null && bgms.TryGetValue(PowerOfTheVoid, out var bgm))
            {
                if (bgm.Values.Any(x => x.Value == 6))
                {
                    return true;
                }
            }
        }
        return false;
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Ankka:
            {
                var casts = target.GetAnimatedCastEvents(log).Where(x => x.SkillID == DeathsEmbraceSkill).ToList();
                castDuration = 10143;

                foreach (CastEvent cast in casts)
                {
                    long endTime = cast.Time + castDuration;

                    if (target.TryGetCurrentPosition(log, cast.Time, out var ankkaPosition))
                    {
                        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.DeathsEmbrace, out var deathsEmbraceEffects))
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

                            var effects = deathsEmbraceEffects.Where(x => x.Time >= cast.Time && x.Time <= cast.EndTime);
                            foreach (EffectEvent effectEvt in effects)
                            {
                                AddDeathEmbraceDecoration(replay, cast.Time, castDuration, radius, effectEvt.Time - cast.Time, effectEvt.Position);
                            }
                        }
                        else
                        {
                            // logs without effects
                            int delay = 1833 * 2;
                            // Zone 1
                            if (ankkaPosition.X > -6000 && ankkaPosition.X < -2500 && ankkaPosition.Y < 1000 && ankkaPosition.Y > -1000)
                            {
                                AddDeathEmbraceDecoration(replay, cast.Time, castDuration, 500, delay, new(-3941.78f, 66.76819f, -3611.2f)); // CENTER
                            }

                            // Zone 2
                            if (ankkaPosition.X > 0 && ankkaPosition.X < 4000)
                            {
                                AddDeathEmbraceDecoration(replay, cast.Time, castDuration, 340, delay, new(1663.69f, 1739.87f, -4639.695f)); // NW
                                AddDeathEmbraceDecoration(replay, cast.Time, castDuration, 340, delay, new(2563.689f, 1739.87f, -4664.611f)); // NE
                                AddDeathEmbraceDecoration(replay, cast.Time, castDuration, 340, delay, new(1663.69f, 839.8699f, -4640.633f)); // SW
                                AddDeathEmbraceDecoration(replay, cast.Time, castDuration, 340, delay, new(2563.689f, 839.8699f, -4636.368f)); // SE
                            }

                            // Zone 3
                            if (ankkaPosition.Y > 4000 && ankkaPosition.Y < 6000)
                            {
                                AddDeathEmbraceDecoration(replay, cast.Time, castDuration, 380, delay, new(-2547.61f, 5466.439f, -6257.504f)); // NW
                                AddDeathEmbraceDecoration(replay, cast.Time, castDuration, 380, delay, new(-1647.61f, 5466.439f, -6256.795f)); // NE
                                AddDeathEmbraceDecoration(replay, cast.Time, castDuration, 380, delay, new(-2547.61f, 4566.439f, -6256.799f)); // SW
                                AddDeathEmbraceDecoration(replay, cast.Time, castDuration, 380, delay, new(-1647.61f, 4566.439f, -6257.402f)); // SE
                            }
                        }
                    }
                }

                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DeathsHandByAnkkaRadius300, out var deathsHandOnPlayerNM))
                {
                    foreach (EffectEvent deathsHandEffect in deathsHandOnPlayerNM)
                    {
                        if (log.CombatData.GetBuffRemoveAllData(DeathsHandSpreadBuff).Any(x => Math.Abs(x.Time - deathsHandEffect.Time) < ServerDelayConstant))
                        {
                            AddDeathsHandDecoration(replay, deathsHandEffect.Position, deathsHandEffect.Time, 3000, 300, 13000);
                        }
                    }
                }

                var xjjPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID && x.IsCM).ToList();
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DeathsHandByAnkkaRadius380, out var deathsHandOnPlayerCMOrInBetween))
                {
                    foreach (EffectEvent deathsHandEffect in deathsHandOnPlayerCMOrInBetween)
                    {
                        if (!log.CombatData.GetBuffRemoveAllData(DeathsHandSpreadBuff).Any(x => Math.Abs(x.Time - deathsHandEffect.Time) < ServerDelayConstant))
                        {
                            // One also happens during death's embrace so we filter that one out
                            if (!casts.Any(x => x.Time <= deathsHandEffect.Time && x.Time + castDuration >= deathsHandEffect.Time))
                            {
                                AddDeathsHandDecoration(replay, deathsHandEffect.Position, deathsHandEffect.Time, 3000, 380, 1000);
                            }
                        }
                        else if (xjjPhases.Any(x => x.InInterval(deathsHandEffect.Time)))
                        {
                            AddDeathsHandDecoration(replay, deathsHandEffect.Position, deathsHandEffect.Time, 3000, 380, 33000);
                        }
                    }
                }

                // Power of the Void
                IEnumerable<Segment> potvSegments = target.GetBuffStatus(log, PowerOfTheVoid).Where(x => x.Value > 0);
                replay.Decorations.AddOverheadIcons(potvSegments, target, ParserIcons.PowerOfTheVoidOverhead);
            }
            break;

            case (int)TargetID.KraitsHallucination:
            {
                // Wall of Fear
                long firstMovementTime = target.FirstAware + 2550;
                uint kraitsRadius = 420;
                var agentConnector = new AgentConnector(target);
                replay.Decorations.Add(new CircleDecoration(kraitsRadius, (target.FirstAware, firstMovementTime), Colors.Orange, 0.2, agentConnector).UsingGrowingEnd(firstMovementTime));
                replay.Decorations.Add(new CircleDecoration(kraitsRadius, (firstMovementTime, target.LastAware), Colors.Red, 0.2, agentConnector));
            }
            break;

            case (int)TargetID.LichHallucination:
            {
                // Terrifying Apparition
                long awareTime = target.FirstAware + 1000;
                uint lichRadius = 280;
                var agentConnector = new AgentConnector(target);
                replay.Decorations.Add(new CircleDecoration(lichRadius, (target.FirstAware, awareTime), Colors.Orange, 0.2, agentConnector).UsingGrowingEnd(awareTime));
                replay.Decorations.Add(new CircleDecoration(lichRadius, (awareTime, target.LastAware), Colors.Red, 0.2, agentConnector));
            }
            break;

            case (int)TargetID.QuaggansHallucinationNM:
            {
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Wave of Torment - Circle explosion around Quaggan
                        case WaveOfTormentNM:
                            castDuration = 2800;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            replay.Decorations.AddWithGrowing(new CircleDecoration(300, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), lifespan.end);
                            break;
                        default:
                            break;
                    }
                }
            }
            break;

            case (int)TargetID.QuaggansHallucinationCM:
            {
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Wave of Torment - Circle explosion around Quaggan
                        case WaveOfTormentCM:
                            castDuration = 5600;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            replay.Decorations.AddWithGrowing(new CircleDecoration(450, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), lifespan.end);
                            break;
                        default:
                            break;
                    }
                }
            }
            break;

            case (int)TargetID.ZhaitansReach:
            {
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Thrash - Circle that pulls in
                        case ZhaitansReachThrashXJJ1:
                        case ZhaitansReachThrashXJJ2:
                            castDuration = 1900;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            replay.Decorations.AddWithGrowing(new DoughnutDecoration(300, 500, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), lifespan.end);
                            break;
                        // Ground Slam - AoE that knocks out
                        case ZhaitansReachGroundSlam:
                        case ZhaitansReachGroundSlamXJJ:
                            // 66534 -> Fast AoE -- 66397 -> Slow AoE
                            castDuration = cast.SkillID == ZhaitansReachGroundSlam ? 800 : 2500;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            replay.Decorations.AddWithGrowing(new CircleDecoration(400, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), lifespan.end);
                            break;
                        default:
                            break;
                    }
                }
            }
            break;

            case (int)TargetID.ReanimatedSpite:
                break;

            case (int)TargetID.SanctuaryPrism:
            {
                var xjjPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID && x.IsCM).ToList();
                var prismStart = log.LogData.EvtcLogStart;
                foreach (var xjjPhase in xjjPhases)
                {
                    replay.Hidden.Add(new(prismStart, xjjPhase.Start));
                    prismStart = xjjPhase.End;
                }
                replay.Hidden.Add(new(prismStart, log.LogData.EvtcLogEnd));
                break;
            }
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        var xjjPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID && x.IsCM).ToList();
        if (p.GetBuffGraphs(log).TryGetValue(DeathsHandSpreadBuff, out var value))
        {
            foreach (Segment segment in value.Values)
            {
                //TODO(Rennorb) @correctnes: there was a null check here, i have no clue why.
                if (!segment.IsEmpty() && segment.Value == 1)
                {
                    var isCM = xjjPhases.Any(x => x.IntersectsWindow(segment.Start, segment.End));
                    uint deathsHandRadius = (uint)(isCM ? 380 : 300);
                    int deathsHandDuration = isCM ? 33000 : 13000;
                    // AoE on player
                    replay.Decorations.AddWithGrowing(new CircleDecoration(deathsHandRadius, segment, Colors.Orange, 0.2, new AgentConnector(p)), segment.End);
                    // Logs without effects, we add the dropped AoE manually
                    if (!log.CombatData.HasEffectData)
                    {
                        if (p.TryGetCurrentPosition(log, segment.End, out var playerPosition))
                        {
                            AddDeathsHandDecoration(replay, playerPosition, segment.End, 3000, deathsHandRadius, deathsHandDuration);
                        }
                    }
                }
            }
        }
        // Tethering Players to Lich
        var lichTethers = GetBuffApplyRemoveSequence(log.CombatData, AnkkaLichHallucinationFixation, p, true, true);
        replay.Decorations.AddTether(lichTethers, Colors.Teal, 0.5);

        // Reanimated Hatred Fixation
        IEnumerable<Segment> hatredFixations = p.GetBuffStatus(log, FixatedAnkkaKainengOverlook).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(hatredFixations, p, ParserIcons.FixationPurpleOverhead);
        // Reanimated Hatred Tether to player - The buff is applied by Ankka to the player - The Reanimated Hatred spawns before the buff application
        replay.Decorations.AddTetherByThirdPartySrcBuff(log, p, FixatedAnkkaKainengOverlook, (int)TargetID.Ankka, (int)TargetID.ReanimatedHatred, Colors.Magenta, 0.5);
    }

    private static void AddDeathsHandDecoration(CombatReplay replay, Vector3 position, long start, int delay, uint radius, int duration)
    {
        (long start, long end) lifespan = (start, start + delay);
        (long start, long end) lifespanAoE = (lifespan.end, lifespan.start + duration);
        var positionConnector = new PositionConnector(position);
        // Growing AoE
        replay.Decorations.AddWithGrowing(new CircleDecoration(radius, lifespan, Colors.Orange, 0.2, positionConnector), lifespan.end);
        // Damaging AoE
        replay.Decorations.AddWithBorder(new CircleDecoration(radius, lifespanAoE, Colors.DarkGreen, 0.3, positionConnector), Colors.Red, 0.4);
    }

    private static void AddDeathEmbraceDecoration(CombatReplay replay, long start, long duration, uint radius, long delay, Vector3 position)
    {
        (long start, long end) lifespan = (start, start + delay);
        (long start, long end) lifespanAoE = (lifespan.end, lifespan.start + duration);
        var positionConnector = new PositionConnector(position);
        replay.Decorations.Add(new CircleDecoration(radius, lifespan, Colors.Orange, 0.2, positionConnector).UsingGrowingEnd(lifespan.end));
        replay.Decorations.Add(new CircleDecoration(radius, lifespanAoE, Colors.Red, 0.2, positionConnector));
    }

    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
        {
            var clarityEligibilityEvents = new List<AchievementEligibilityEvent>();
            var xjjPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID && x.IntersectsWindow(p.FirstAware, p.LastAware)).ToHashSet();
            List<HealthDamageEvent> damageData = [
                ..log.CombatData.GetDamageData(WallOfFear),
                ..log.CombatData.GetDamageData(WaveOfTormentNM),
                ..log.CombatData.GetDamageData(WaveOfTormentCM),
                ..log.CombatData.GetDamageData(TerrifyingApparition)
            ];
            damageData.SortByTime();
            foreach (var evt in damageData)
            {
                if (evt.HasHit && evt.To.Is(p.AgentItem) && p.InAwareTimes(evt.Time))
                {
                    InsertAchievementEligibityEventAndRemovePhase(xjjPhases, clarityEligibilityEvents, evt.Time, Ach_Clarity, p);
                }
            }
            AddSuccessBasedAchievementEligibityEvents(xjjPhases, clarityEligibilityEvents, Ach_Clarity, p);
            achievementEligibilityEvents.AddRange(clarityEligibilityEvents);
        }
        {
            var undevouredEligibilityEvents = new List<AchievementEligibilityEvent>();
            var xjjCMPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID && x.IsCM && x.IntersectsWindow(p.FirstAware, p.LastAware)).ToHashSet();
            var buffApplyData = log.CombatData.GetBuffApplyDataByIDByDst(DevouringVoid, p.AgentItem);
            foreach (var evt in buffApplyData)
            {
                InsertAchievementEligibityEventAndRemovePhase(xjjCMPhases, undevouredEligibilityEvents, evt.Time, Ach_Undevoured, p);
            }
            AddSuccessBasedAchievementEligibityEvents(xjjCMPhases, undevouredEligibilityEvents, Ach_Undevoured, p);
            achievementEligibilityEvents.AddRange(undevouredEligibilityEvents);
        }
    }
}
