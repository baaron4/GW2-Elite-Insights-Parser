using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class KainengOverlook : EndOfDragonsSingleBossRaid
{
    public KainengOverlook(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup(
        [  
            //Li
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([ DragonSlashWaveNM, DragonSlashWaveCM ], new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkRed), "Wave.H", "Hit by Wave", "Wave Hit", 150),
                new PlayerDstHealthDamageHitMechanic([ DragonSlashBurstNM, DragonSlashBurstCM ], new MechanicPlotlySetting(Symbols.TriangleUp, Colors.DarkRed), "Burst.H", "Hit by Burst", "Burst Hit", 150),
                new PlayerDstHealthDamageHitMechanic([ DragonSlashRushNM1, DragonSlashRushNM2, DragonSlashRush1CM, DragonSlashRush2CM ], new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkRed), "Rush.H", "Hit by Rush", "Rush Hit", 150),
                new PlayerDstHealthDamageHitMechanic([ DragonSlashWaveNM, DragonSlashWaveCM, DragonSlashRushNM1, DragonSlashRushNM2, DragonSlashRush1CM, DragonSlashRush2CM ], new MechanicPlotlySetting(Symbols.Diamond, Colors.Red), "TextReflx.Achiv", "Achievement Eligibility: A Test of Your Reflexes", "Achiv Test Reflexes", 150)
                    .UsingAchievementEligibility()
                    .UsingEnable((log) => log.LogData.IsCM),
            ]),
            new MechanicGroup([             
                // Mindblade
                new MechanicGroup([
                    new PlayerDstHealthDamageHitMechanic([ StormOfSwords1, StormOfSwords2, StormOfSwords3, StormOfSwords4, StormOfSwords5, StormOfSwords6, StormOfSwords7, StormOfSwords8, StormOfSwords9, StormOfSwords10 ], new MechanicPlotlySetting(Symbols.Circle, Colors.Pink), "Storm.H", "Hit by bladestorm", "Bladestorm Hit", 150),
                    new PlayerDstEffectMechanic(EffectGUIDs.KainengOverlookMindbladeRainOfBladesFirstOrangeAoEOnPlayer, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.LightPurple), "RainBlad.T", "Targeted by Rain of Blades", "Rain of Blades Target", 150),
                    new PlayerDstBuffApplyMechanic(FixatedAnkkaKainengOverlook, new MechanicPlotlySetting(Symbols.Circle, Colors.Purple), "Fixated.M", "Fixated by The Mindblade", "Fixated Mindblade", 150)
                        .UsingChecker((bae, log) => bae.CreditedBy.IsAnySpecies(new List<TargetID> { TargetID.TheMindblade, TargetID.TheMindbladeCM })),
                ]),
                // Enforcer
                new MechanicGroup([
                    new PlayerDstHealthDamageHitMechanic([ EnforcerRushingJusticeNM, EnforcerRushingJusticeCM ], new MechanicPlotlySetting(Symbols.Square, Colors.Orange), "Flames.S", "Stood in Flames", "Stood in Flames", 150),
                    new PlayerDstBuffApplyMechanic(FixatedAnkkaKainengOverlook, new MechanicPlotlySetting(Symbols.Circle, Colors.DarkPurple), "Fixated.E", "Fixated by The Enforcer", "Fixated Enforcer", 150)
                        .UsingChecker((bae, log) => bae.CreditedBy.IsAnySpecies(new List<TargetID> { TargetID.TheEnforcer, TargetID.TheEnforcerCM })),
                    new PlayerDstHealthDamageHitMechanic(BoomingCommandSkillNM, new MechanicPlotlySetting(Symbols.Circle, Colors.Red), "Red.O", "Red circle overlap", "Red Circle", 150),
                ]),
                // Ritualist
                new MechanicGroup([

                ]),
            ]),
            new MechanicGroup([          
                // Mech Rider
                new MechanicGroup([
                    new PlayerDstHealthDamageHitMechanic([ ExplosiveUppercutNM, ExplosiveUppercutCM ], new MechanicPlotlySetting(Symbols.TriangleNE, Colors.Pink), "ExpUpper.H", "Hit by Explosive Uppercut", "Explosive Uppercut Hit", 150),
                    new PlayerDstHealthDamageHitMechanic([ FallOfTheAxeSmallConeNM, FallOfTheAxeSmallConeCM ], new MechanicPlotlySetting(Symbols.TriangleRight, Colors.LightGrey), "FallAxe.S.H", "Hit by Mech Rider Small Cone", "Mech Rider Small Cone Hit", 150),
                    new PlayerDstHealthDamageHitMechanic([ FallOfTheAxeBigConeNM, FallOfTheAxeBigConeCM ], new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.LightGrey), "FallAxe.B.H", "Hit by Mech Rider Big Cone", "Mech Rider Small Big Hit", 150),
                    new PlayerDstHealthDamageHitMechanic([ ElectricRainNM, ElectricRainCM ], new MechanicPlotlySetting(Symbols.StarDiamond, Colors.LightOrange), "ElecRain.H", "Hit by Electric Rain (Set of 5 AoEs by Mech Rider)", "Electic Rain Hit", 150),
                    new PlayerDstHealthDamageHitMechanic(JadeBusterCannonMechRider, new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Orange), "Laser.H", "Hit by Big Laser", "Laser Hit", 150),
                ]),
                // Sniper
                new MechanicGroup([
                    new PlayerDstEffectMechanic(EffectGUIDs.KainengOverlookSniperRicochetBeamCM, new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Red), "Sniper.T", "Targeted by Sniper Ricochet", "Ricochet Target", 150),
                ]),
                new EnemyDstBuffApplyMechanic(EnhancedDestructiveAuraBuff, new MechanicPlotlySetting(Symbols.TriangleUpOpen, Colors.Purple), "DescAura", "Enhanced Destructive Aura", "Powered Up 2", 150),
                new EnemyDstBuffApplyMechanic(DestructiveAuraBuff, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Purple), "Pwrd.Up2", "Powered Up (Split 2)", "Powered Up 2", 150),
                new EnemyDstBuffApplyMechanic(LethalInspiration, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.DarkGreen), "Pwrd.Up1", "Powered Up (Split 1)", "Powered Up 1", 150),
                new PlayerDstNoHealthDamageMechanic([ EnhancedDestructiveAuraSkill1, EnhancedDestructiveAuraSkill2 ], new MechanicPlotlySetting(Symbols.DiamondWide, Colors.Purple), "MostResi.Achiv", "Achievement Eligibility: The Path of Most Resistance", "Achiv Most Resistance", 150)
                    .UsingAchievementEligibility()
                    .UsingEnable(x => x.LogData.IsCM),
            ]),
            new PlayerDstHealthDamageMechanic([ TargetedExpulsion, TargetedExpulsionCM ], new MechanicPlotlySetting(Symbols.Square, Colors.Purple), "Bomb.D", "Downed by Bomb", "Bomb Downed", 150).UsingChecker((ahde, log) => ahde.HasDowned),
            new PlayerDstBuffApplyMechanic([ TargetOrder1, TargetOrder2, TargetOrder3, TargetOrder4, TargetOrder5 ], new MechanicPlotlySetting(Symbols.Star, Colors.LightOrange), "Targ.Ord.A", "Received Target Order", "Target Order Application", 0),
            new MechanicGroup([
                new PlayerDstEffectMechanic(EffectGUIDs.KainengOverlookSharedDestructionGreen, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Green),  "Green", "Selected for Green", "Green", 150),
                new PlayerDstEffectMechanic(EffectGUIDs.KainengOverlookSharedDestructionGreenSuccess, new MechanicPlotlySetting(Symbols.Circle, Colors.Green),  "Green.Succ", "Successful Green", "Successful Green", 150),
                new PlayerDstEffectMechanic(EffectGUIDs.KainengOverlookSharedDestructionGreenFailure, new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.DarkGreen),  "Green.Fail", "Failed Green", "Failed Green", 150),
            ]),
        ])
        );
        Icon = EncounterIconKainengOverlook;
        Extension = "kaiover";
        LogCategoryInformation.InSubCategoryOrder = 2;
        LogID |= 0x000003;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (1803, 1918),
                        (-24798, -18014, -18164, -10932));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayKainengOverlook, crMap);
        return crMap;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.MinisterLi,
            TargetID.MinisterLiCM,
            TargetID.TheEnforcer,
            TargetID.TheMindblade,
            TargetID.TheMechRider,
            TargetID.TheRitualist,
            TargetID.TheSniper,
            TargetID.TheEnforcerCM,
            TargetID.TheMindbladeCM,
            TargetID.TheMechRiderCM,
            TargetID.TheRitualistCM,
            TargetID.TheSniperCM,
        ];
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.MinisterLi, 0 },
            {TargetID.MinisterLiCM, 0 },
            {TargetID.TheEnforcer, 1 },
            {TargetID.TheEnforcerCM, 1 },
            {TargetID.TheMindblade, 2 },
            {TargetID.TheMindbladeCM, 2 },
            {TargetID.TheRitualist, 3 },
            {TargetID.TheRitualistCM, 3 },
            {TargetID.TheSniper, 4 },
            {TargetID.TheSniperCM, 4 },
            {TargetID.TheMechRider, 5 },
            {TargetID.TheMechRiderCM, 5 },
        };
    }

    protected override IReadOnlyList<TargetID> GetSuccessCheckIDs()
    {
        return
        [
            TargetID.MinisterLi,
            TargetID.MinisterLiCM,
        ];
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Kaineng Overlook";
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.SpiritOfDestruction,
            TargetID.SpiritOfPain,
        ];
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(DestructiveAuraSkill, DestructiveAuraSkill),
            new DamageCastFinder(EnhancedDestructiveAuraSkill1, EnhancedDestructiveAuraSkill1),
            new DamageCastFinder(EnhancedDestructiveAuraSkill2, EnhancedDestructiveAuraSkill2),
        ];
    }

    private static void AddSplitPhase(List<PhaseData> phases, IReadOnlyList<SingleActor?> targets, SingleActor ministerLi, ParsedEvtcLog log, int phaseID)
    {
        if (targets.All(x => x != null))
        {
            EnterCombatEvent? cbtEnter = null;
            foreach (SingleActor? target in targets)
            {
                if (target != null)
                {
                    cbtEnter = log.CombatData.GetEnterCombatEvents(target.AgentItem).LastOrDefault();
                    if (cbtEnter != null)
                    {
                        break;
                    }
                }
            }
            if (cbtEnter != null)
            {
                BuffEvent? nextPhaseStartEvt = log.CombatData.GetBuffDataByIDByDst(Determined762, ministerLi.AgentItem).FirstOrDefault(x => x is BuffRemoveAllEvent && x.Time > cbtEnter.Time);
                long phaseEnd = nextPhaseStartEvt != null ? nextPhaseStartEvt.Time : log.LogData.LogEnd;
                var addPhase = new SubPhasePhaseData(cbtEnter.Time, phaseEnd, "Split Phase " + phaseID);
                addPhase.AddTargets(targets, log);
                addPhase.AddParentPhase(phases[0]);
                phases.Add(addPhase);
            }
        }
    }

    private SingleActor? GetMinisterLi(LogData logData)
    {
        return Targets.FirstOrDefault(x => x.IsSpecies(logData.IsCM ? TargetID.MinisterLiCM : TargetID.MinisterLi));
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor ministerLi = GetMinisterLi(log.LogData) ?? throw new MissingKeyActorsException("Minister Li not found");
        phases[0].AddTarget(ministerLi, log);
        //
        SingleActor? enforcer = Targets.LastOrDefault(x => x.IsSpecies(log.LogData.IsCM ? TargetID.TheEnforcerCM : TargetID.TheEnforcer));
        SingleActor? mindblade = Targets.LastOrDefault(x => x.IsSpecies(log.LogData.IsCM ? TargetID.TheMindbladeCM : TargetID.TheMindblade));
        SingleActor? mechRider = Targets.LastOrDefault(x => x.IsSpecies(log.LogData.IsCM ? TargetID.TheMechRiderCM : TargetID.TheMechRider));
        SingleActor? sniper = Targets.LastOrDefault(x => x.IsSpecies(log.LogData.IsCM ? TargetID.TheSniperCM : TargetID.TheSniper));
        SingleActor? ritualist = Targets.LastOrDefault(x => x.IsSpecies(log.LogData.IsCM ? TargetID.TheRitualistCM : TargetID.TheRitualist));
        //
        phases[0].AddTarget(enforcer, log, PhaseData.TargetPriority.Blocking);
        phases[0].AddTarget(mindblade, log, PhaseData.TargetPriority.Blocking);
        phases[0].AddTarget(mechRider, log, PhaseData.TargetPriority.Blocking);
        phases[0].AddTarget(sniper, log, PhaseData.TargetPriority.Blocking);
        phases[0].AddTarget(ritualist, log, PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return phases;
        }
        List<PhaseData> subPhases = GetPhasesByInvul(log, Determined762, ministerLi, false, true);
        for (int i = 0; i < subPhases.Count; i++)
        {
            subPhases[i].Name = "Phase " + (i + 1);
            subPhases[i].AddTarget(ministerLi, log);
            subPhases[i].AddParentPhase(phases[0]);
        }
        // when wiped during a split phase, Li's LastAware is well before fight end
        subPhases.RemoveAll(x => (x.End + x.Start) / 2 > ministerLi.LastAware + ServerDelayConstant);
        phases.AddRange(subPhases);
        AddSplitPhase(phases, [enforcer, mindblade, ritualist], ministerLi, log, 1);
        AddSplitPhase(phases, [mechRider, sniper], ministerLi, log, 2);
        return phases;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        SingleActor ministerLi = GetMinisterLi(logData) ?? throw new MissingKeyActorsException("Minister Li not found");
        var buffApplies = combatData.GetBuffApplyDataByIDByDst(Resurrection, ministerLi.AgentItem).OfType<BuffApplyEvent>();
        if (buffApplies.Any())
        {
            logData.SetSuccess(true, buffApplies.First().Time);
        } 
        else
        {
            logData.SetSuccess(false, ministerLi.LastAware);
        }
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        SingleActor? ministerLiCM = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.MinisterLiCM));
        return ministerLiCM != null ? LogData.LogMode.CM : LogData.LogMode.Normal;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);

        (long start, long end) lifespan;

        // Target Order
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder1).Where(x => x.Value > 0), p, ParserIcons.TargetOrder1Overhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder2).Where(x => x.Value > 0), p, ParserIcons.TargetOrder2Overhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder3).Where(x => x.Value > 0), p, ParserIcons.TargetOrder3Overhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder4).Where(x => x.Value > 0), p, ParserIcons.TargetOrder4Overhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder5).Where(x => x.Value > 0), p, ParserIcons.TargetOrder5Overhead);
        
        // Fixation
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, FixatedAnkkaKainengOverlook).Where(x => x.Value > 0), p, ParserIcons.FixationPurpleOverhead);
        var fixationEvents = GetBuffApplyRemoveSequence(log.CombatData, FixatedAnkkaKainengOverlook, p, true, true);
        replay.Decorations.AddTether(fixationEvents, Colors.Magenta, 0.5);

        // Shared Destruction (Green)
        int greenDuration = 6250;
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(p.AgentItem,
            [ EffectGUIDs.KainengOverlookSharedDestructionGreenSuccess, EffectGUIDs.KainengOverlookSharedDestructionGreenFailure ],
            out var greenEndEffectEvents))
        {
            foreach (EffectEvent effect in greenEndEffectEvents)
            {
                bool isSuccess = effect.GUIDEvent.ContentGUID == EffectGUIDs.KainengOverlookSharedDestructionGreenSuccess;
                AddSharedDestructionDecoration(p, replay, (effect.Time - greenDuration, effect.Time), isSuccess);
            }
        }
        else
        {
            greenEndEffectEvents = [];
        }
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(p.AgentItem, EffectGUIDs.KainengOverlookSharedDestructionGreen, out var greenApplyEffectEvents))
        {
            foreach (EffectEvent effect in greenApplyEffectEvents)
            {
                // Check if any green effect event happens within 200 ms from another successful or failed green.
                // If the green mechanic targets the same player twice at the same time (meaning only one green appears in game), the second effect gets queued up 6.5 seconds later.
                // This prevents the late green effect from appearing in the combat replay, since it doesn't exist in game.
                if (!greenEndEffectEvents.Any(x => Math.Abs(x.Time - effect.Time) < 200 || Math.Abs(x.Time - greenDuration - effect.Time) < 200))
                {
                    AddSharedDestructionDecoration(p, replay, effect.ComputeLifespan(log, greenDuration), true);
                }
            }
        }

        // TODO Check this again and rework it - not consistent enough
        // Sniper Ricochet Tether & AoE - CM
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.KainengOverlookSniperRicochetBeamCM, out var sniperBeamsCM))
        {
            foreach (EffectEvent effect in sniperBeamsCM)
            {
                // Check if any effect event exists before the current one within a 20 seconds time span
                // This is to fix the beam duration incorrectly logged
                // The first shot happens after 10 seconds, the following ones after 5 seconds
                int correctedDuration = sniperBeamsCM.Where(x => x.Time > effect.Time - 20000 && x.Time != effect.Time && x.Time < effect.Time).Any() ? 5000 : 10000;
                // Correct the life span for the circle decoration
                lifespan = (effect.Time, effect.Time + correctedDuration);

                // Tether Sniper to Player
                replay.Decorations.AddTetherByEffectGUID(log, effect, Colors.Yellow, 0.3, correctedDuration, true);

                // Circle around the player
                replay.Decorations.Add(new CircleDecoration(500, lifespan, Colors.Red, 0.2, new AgentConnector(p)).UsingFilled(false));
            }
        }

        // Targeted Expulsion - Orange spread AoEs CM
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.KainengOverlookTargetedExpulsion, out var spreads))
        {
            foreach (EffectEvent effect in spreads)
            {
                lifespan = effect.ComputeLifespan(log, 5000);
                replay.Decorations.AddWithGrowing(new CircleDecoration(230, lifespan, Colors.Orange, 0.2, new AgentConnector(p)), lifespan.end);
            }
        }

        // Rain Of Blades - Mindblade AoE on players - Orange circle (first)
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.KainengOverlookMindbladeRainOfBladesFirstOrangeAoEOnPlayer, out var mindbladeAoEOnPlayers))
        {
            foreach (EffectEvent effect in mindbladeAoEOnPlayers)
            {
                lifespan = effect.ComputeLifespan(log, 8000);
                replay.Decorations.AddWithGrowing(new CircleDecoration(240, lifespan, Colors.Orange, 0.2, new AgentConnector(p)), lifespan.end);
            }
        }

        // Rain Of Blades - Mindblade AoE on players - Orange circle (consecutives)
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.KainengOverlookMindbladeRainOfBladesConsecutiveOrangeAoEOnPlayer, out var mindbladeAoEOnPlayers4))
        {
            foreach (EffectEvent effect in mindbladeAoEOnPlayers4)
            {
                lifespan = effect.ComputeLifespan(log, 2000);
                replay.Decorations.AddWithGrowing(new CircleDecoration(240, lifespan, Colors.Orange, 0.2, new AgentConnector(p)), lifespan.end);
            }
        }

        // Heaven's Palm - AoE on players
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.KainengOverlookEnforcerHeavensPalmAoE, out var heavensPalm))
        {
            foreach (EffectEvent effect in heavensPalm)
            {
                lifespan = effect.ComputeLifespan(log, 5000);
                replay.Decorations.AddWithGrowing(new CircleDecoration(280, lifespan, Colors.Orange, 0.2, new AgentConnector(p)), lifespan.end);
            }
        }
    }

    private static void HideAfterDetermined(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var determined762Apply = log.CombatData.GetBuffApplyDataByIDByDst(Determined762, target.AgentItem).FirstOrDefault();
        if (determined762Apply != null)
        {
            replay.Trim(replay.TimeOffsets.start, determined762Apply.Time);
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.MinisterLi:
            case (int)TargetID.MinisterLiCM:
                // Dragon Slash-Wave
                // The effect is only usable in normal mode
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KainengOverlookDragonSlashWaveIndicator, out var waveEffect))
                {
                    foreach (EffectEvent effect in waveEffect)
                    {
                        int durationCone = 1000;
                        lifespan = effect.ComputeLifespan(log, durationCone);
                        AddDragonSlashWaveDecoration(log, target, replay, lifespan, durationCone);
                    }
                }
                else
                {
                    foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                    {
                        switch (cast.SkillID)
                        {
                            // Check for the normal mode skill for older logs
                            case DragonSlashWaveNM:
                            case DragonSlashWaveCM:
                                int durationCone = cast.SkillID == DragonSlashWaveNM ? 1000 : 500;
                                lifespan = (cast.Time, cast.Time + durationCone);
                                AddDragonSlashWaveDecoration(log, target, replay, lifespan, durationCone);
                                break;
                            default:
                                break;
                        }
                    }
                }
                break;
            case (int)TargetID.TheSniper:
            case (int)TargetID.TheSniperCM:
                HideAfterDetermined(target, log, replay);
                break;
            case (int)TargetID.TheMechRider:
            case (int)TargetID.TheMechRiderCM:
                long jadeBusterCannonWarning = 2800;
                var jadeBusterCannonOffset = new Vector3(0, -1400, 0);

                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Fall of the Axe - Small Cone
                        case FallOfTheAxeSmallConeNM:
                        case FallOfTheAxeSmallConeCM:
                            castDuration = 965;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            AddFallOfTheAxeDecoration(log, target, replay, lifespan, castDuration, 35);
                            break;
                        // Fall of the Axe - Big Cone
                        case FallOfTheAxeBigConeNM:
                        case FallOfTheAxeBigConeCM:
                            castDuration = 1030;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            AddFallOfTheAxeDecoration(log, target, replay, lifespan, castDuration, 75);
                            break;
                        // Jade Buster Cannon - Damage
                        case JadeBusterCannonMechRider:
                            castDuration = 10367;
                            lifespan = (cast.Time + jadeBusterCannonWarning, cast.Time + castDuration - jadeBusterCannonWarning);
                            var rotationConnector = new AgentFacingConnector(target, 90, AgentFacingConnector.RotationOffsetMode.AddToMaster);
                            var rectangle = (RectangleDecoration)new RectangleDecoration(375, 3000, lifespan, "rgba(30, 120, 40, 0.4)", new AgentConnector(target).WithOffset(jadeBusterCannonOffset, true)).UsingRotationConnector(rotationConnector);
                            replay.Decorations.AddWithBorder(rectangle, Colors.Red, 0.2);
                            break;
                        default:
                            break;
                    }
                }

                // Jade Buster Cannon - Warning
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KainengOverlookJadeBusterCannonWarning, out var warningRectangle))
                {
                    foreach (EffectEvent effect in warningRectangle)
                    {
                        lifespan = effect.ComputeLifespan(log, jadeBusterCannonWarning);
                        var rotationConnector = new AgentFacingConnector(target, 90, AgentFacingConnector.RotationOffsetMode.AddToMaster);
                        var rectangle = (RectangleDecoration)new RectangleDecoration(375, 3000, lifespan, Colors.Orange, 0.2, new AgentConnector(target).WithOffset(jadeBusterCannonOffset, true)).UsingRotationConnector(rotationConnector);
                        replay.Decorations.AddWithBorder(rectangle, Colors.Red, 0.2);
                    }
                }
                HideAfterDetermined(target, log, replay);
                break;
            case (int)TargetID.TheEnforcer:
            case (int)TargetID.TheEnforcerCM:
                // Blue tether from Enforcer to Mindblade when they're close to each other
                var enforcerInspiration = GetBuffApplyRemoveSequence(log.CombatData, LethalInspiration, target, true, true);
                replay.Decorations.AddTether(enforcerInspiration, Colors.Blue, 0.1);
                HideAfterDetermined(target, log, replay);
                break;
            case (int)TargetID.TheMindblade:
            case (int)TargetID.TheMindbladeCM:
                // Blue tether from Mindblade to Enforcer when they're close to each other
                var mindbladeInspiration = GetBuffApplyRemoveSequence(log.CombatData, LethalInspiration, target, true, true);
                replay.Decorations.AddTether(mindbladeInspiration, Colors.Blue, 0.1);
                HideAfterDetermined(target, log, replay);
                break;
            case (int)TargetID.TheRitualist:
            case (int)TargetID.TheRitualistCM:
                HideAfterDetermined(target, log, replay);
                break;
            case (int)TargetID.SpiritOfPain:
                // Volatile Expulsion - Orange AoE around the spirit
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.KainengOverlookVolatileExpulsionAoE, out var volatileExpulsion))
                {
                    foreach (EffectEvent effect in volatileExpulsion)
                    {
                        lifespan = effect.ComputeLifespan(log, 5500);
                        var circle = new CircleDecoration(380, lifespan, Colors.Orange, 0.2, new AgentConnector(target));
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }
                break;
            case (int)TargetID.SpiritOfDestruction:
                // Volatile Burst - Orange AoE around the spirit with safe zone in the center
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.KainengOverlookVolatileBurstAoE, out var volatileBurst))
                {
                    foreach (EffectEvent effect in volatileBurst)
                    {
                        lifespan = effect.ComputeLifespan(log, 5500);
                        var doughnut = new DoughnutDecoration(100, 500, lifespan, Colors.Orange, 0.2, new AgentConnector(target));
                        replay.Decorations.AddWithGrowing(doughnut, lifespan.end);
                    }
                }
                break;
            default:
                break;
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        (long start, long end) lifespan;

        // Dragon Slash Burst - Red AoE Puddles - CM
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KainengOverlookDragonSlashBurstRedAoE1, out var smolReds))
        {
            foreach (EffectEvent effect in smolReds)
            {
                lifespan = effect.ComputeLifespan(log, 20800);
                var connector = new PositionConnector(effect.Position);
                int damageDelay = 1610;
                long warningEnd = lifespan.start + damageDelay;
                var circle = new CircleDecoration(80, (lifespan.start, warningEnd), Colors.Red, 0.2, connector);
                environmentDecorations.AddWithGrowing(circle, warningEnd);
                environmentDecorations.Add(new CircleDecoration(80, (warningEnd, lifespan.end), Colors.Red, 0.4, connector));
            }
        }

        // Jade Mines
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KainengOverlookJadeMine1, out var mines))
        {
            foreach (EffectEvent effect in mines)
            {
                lifespan = effect.ComputeDynamicLifespan(log, effect.Duration);
                environmentDecorations.Add(new CircleDecoration(80, lifespan, Colors.Red, 0.4, new PositionConnector(effect.Position)));
            }
        }

        // Electric Rain - 5 AoEs in sequence up to 5
        // Jade Lob - Small deathly AoE
        // Small Orange AoEs
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KainengOverlookSmallOrangeAoE, out var electricRain))
        {
            foreach (EffectEvent effect in electricRain)
            {
                lifespan = effect.ComputeLifespan(log, 2400);
                var circle = new CircleDecoration(100, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }

        // Jade Lob - Small deathly AoE
        // Pulsing Green Effect
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KainengOverlookJadeLobPulsingGreen, out var jadeLob))
        {
            foreach (EffectEvent effect in jadeLob)
            {
                lifespan = effect.ComputeLifespan(log, 1500);
                var circle = new CircleDecoration(100, lifespan, Colors.SligthlyDarkGreen, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithBorder(circle, Colors.Red, 0.2);
            }
        }

        // Enforcer Orbs AoE
        // TODO - This doesn't work in normal mode
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KainengOverlookEnforcerOrbsAoE, out var enforcerOrbsAoEs))
        {
            foreach (EffectEvent effect in enforcerOrbsAoEs)
            {
                lifespan = effect.ComputeLifespan(log, 2708);
                var circle = new CircleDecoration(100, lifespan, Colors.SlightlyDarkBlue, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithBorder(circle, Colors.Red, 0.2);
            }
        }

        // Rain Of Blades - Mindblade Red AoEs dropped by players
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KainengOverlookMindbladeRainOfBladesRedAoECM, out var mindbladeReds))
        {
            foreach (EffectEvent effect in mindbladeReds)
            {
                lifespan = effect.ComputeLifespan(log, 5000);
                var connector = new PositionConnector(effect.Position);
                int damageDelay = 2000;
                long warningEnd = lifespan.start + damageDelay;
                var circle = new CircleDecoration(240, (lifespan.start, warningEnd), Colors.Red, 0.2, connector);
                environmentDecorations.AddWithGrowing(circle, warningEnd);
                environmentDecorations.Add(circle.GetBorderDecoration());
                environmentDecorations.Add(new CircleDecoration(240, (warningEnd, lifespan.end), Colors.Red, 0.4, connector));
            }
        }

        // Rushing Justice - Enforcer Flames
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KainengOverlookEnforcerRushingJusticeFlames, out var rushingJustice))
        {
            foreach (EffectEvent effect in rushingJustice)
            {
                lifespan = effect.ComputeDynamicLifespan(log, effect.Duration);
                environmentDecorations.Add(new RectangleDecoration(50, 145, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(new AngleConnector(effect.Rotation.Z)));
            }
        }

        // Spiritual Lightning AoEs - Ritualist
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KainengOverlookRitualistSpiritualLightningAoE, out var spiritualLightning))
        {
            foreach (EffectEvent effect in spiritualLightning)
            {
                lifespan = effect.ComputeLifespan(log, 2000);
                var circle = new CircleDecoration(90, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }

        // Storm of Swords - Indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KainengOverlookStormOfSwordsIndicator, out var stormOfSwords))
        {
            foreach (EffectEvent effect in stormOfSwords)
            {
                (long start, long end) lifespanIndicator = effect.ComputeLifespan(log, 3000);
                var connector = new PositionConnector(effect.Position);
                var indicatorCircle = new CircleDecoration(200, lifespanIndicator, Colors.Orange, 0.2, connector);
                environmentDecorations.AddWithGrowing(indicatorCircle, lifespanIndicator.end);
                var initialPosition = new ParametricPoint3D(effect.Position, lifespanIndicator.end);
                int velocity = 85; // Approximation
                int stormDuration = 15000; // Approximation - Attack disappears when off the edge of the platform
                (long, long) lifespanAnimation = (lifespanIndicator.end, lifespanIndicator.end + stormDuration);
                var finalPosition = new ParametricPoint3D(initialPosition.XYZ + (velocity * stormDuration / 1000.0f) * new Vector3((float)Math.Cos(effect.Orientation.Z - Math.PI / 2), (float)Math.Sin(effect.Orientation.Z - Math.PI / 2), 0), lifespanIndicator.end + stormDuration);
                var animatedCircle = new CircleDecoration(200, lifespanAnimation, Colors.DesaturatedPink, 0.2, new InterpolationConnector([initialPosition, finalPosition]));
                environmentDecorations.AddWithBorder(animatedCircle, Colors.Red, 0.2);
            }
        }

        var dragonSlash = log.CombatData.GetMissileEventsBySkillIDs([DragonSlashBurstNM, DragonSlashBurstCM]);
        environmentDecorations.AddNonHomingMissiles(log, dragonSlash, Colors.Orange, 0.3, 25);

        var ricochet = log.CombatData.GetMissileEventsBySkillIDs([JadeRicochetNM, JadeRicochetCM1, JadeRicochetCM2]);
        environmentDecorations.AddNonHomingMissiles(log, ricochet, Colors.Orange, 0.3, 20);

        var lasershot = log.CombatData.GetMissileEventsBySkillIDs([JadeLaserShotNM, JadeLaserShotCM]);
        environmentDecorations.AddNonHomingMissiles(log, lasershot, Colors.Red, 0.3, 20);

        var boomingCommand = log.CombatData.GetMissileEventsBySkillIDs([BoomingCommandDamageNM, BoomingCommandDamageCM]);
        environmentDecorations.AddNonHomingMissiles(log, boomingCommand, Colors.SlightlyDarkBlue, 0.2, 20);

        // TODO - Check if this works in normal mode - No log to test
        var jadeMine = log.CombatData.GetMissileEventsBySkillIDs([JadeMineProjectileFirstSetCM, JadeMineProjectileSecondSetCM, JadeMineProjectileThirdSetCM]);
        environmentDecorations.AddNonHomingMissiles(log, jadeMine, Colors.Red, 0.3, 80);
    }

    internal static void AddFallOfTheAxeDecoration(ParsedEvtcLog log, NPC target, CombatReplay replay, (long start, long end) lifespan, long duration, int angle)
    {
        if (target.TryGetCurrentFacingDirection(log, lifespan.start + 100, out var facingDirection, duration))
        {
            var pie = (PieDecoration)new PieDecoration(480, angle, lifespan, Colors.Orange, 0.2, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facingDirection));
            replay.Decorations.AddWithGrowing(pie, lifespan.end);
            replay.Decorations.Add(pie.GetBorderDecoration());
        }

    }

    private static void AddDragonSlashWaveDecoration(ParsedEvtcLog log, NPC target, CombatReplay replay, (long start, long end) lifespan, int duration)
    {
        if (target.TryGetCurrentFacingDirection(log, lifespan.start + 100, out var facingDirection, duration))
        {
            var pie = (PieDecoration)new PieDecoration(1200, 160, lifespan, Colors.Orange, 0.2, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facingDirection));
            replay.Decorations.AddWithGrowing(pie, lifespan.end);
        }

    }

    private static void AddSharedDestructionDecoration(PlayerActor p, CombatReplay replay, (long start, long end) lifespan, bool isSuccessful)
    {
        Color green = Colors.DarkGreen;
        Color color = isSuccessful ? green : Colors.DarkRed;
        var connector = new AgentConnector(p);
        replay.Decorations.Add(new CircleDecoration(180, lifespan, green, 0.4, connector).UsingGrowingEnd(lifespan.end));
        replay.Decorations.Add(new CircleDecoration(180, lifespan, color, 0.4, connector));
    }
}
