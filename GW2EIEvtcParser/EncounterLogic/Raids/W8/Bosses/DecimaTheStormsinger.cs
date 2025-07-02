using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class DecimaTheStormsinger : MountBalrior
{
    private bool isCM => GenericTriggerID == (int)TargetID.DecimaCM;

    private SingleActor Decima => Targets.FirstOrDefault(x => isCM ? x.IsSpecies(TargetID.DecimaCM) : x.IsSpecies(TargetID.Decima)) ?? throw new MissingKeyActorsException("Decima not found");

    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new MechanicGroup([
                new PlayerDstHitMechanic([ChorusOfThunderDamage, ChorusOfThunderCM], new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "ChorThun.H", "Hit by Chorus of Thunder (Spreads AoE / Conduit AoE)", "Chorus of Thunder Hit", 0),
                new PlayerDstEffectMechanic(EffectGUIDs.DecimaChorusOfThunderAoE, new MechanicPlotlySetting(Symbols.Circle, Colors.LightGrey), "ChorThun.T", "Targeted by Chorus of Thunder (Spreads)", "Chorus of Thunder Target", 0),
            ]),
            new PlayerDstHitMechanic([DiscordantThunderCM], new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "DiscThun.H", "Hit by Discordant Thunder", "Discordant Thunder Hit", 0),
            new PlayerDstHitMechanic(HarmoniousThunder, new MechanicPlotlySetting(Symbols.Circle, Colors.Yellow), "HarmThun.H", "Hit by Harmonious Thunder", "Harmonious Thunder Hit", 0),
            new MechanicGroup([
                new PlayerDstHitMechanic([SeismicCrashDamage, SeismicCrashDamageCM, SeismicCrashDamageCM2], new MechanicPlotlySetting(Symbols.Hourglass, Colors.White), "SeisCrash.H", "Hit by Seismic Crash (Concentric Rings)", "Seismic Crash Hit", 0),
                new PlayerDstHitMechanic([SeismicCrashDamage, SeismicCrashDamageCM, SeismicCrashDamageCM2], new MechanicPlotlySetting(Symbols.Hourglass, Colors.DarkWhite), "SeisCrash.CC", "CC by Seismic Crash (Concentric Rings)", "Seismic Crash CC", 0)
                    .UsingChecker((hde, log) => !hde.To.HasBuff(log, Stability, hde.Time, ServerDelayConstant)),
                new PlayerDstSkillMechanic(SeismicCrashHitboxDamage, new MechanicPlotlySetting(Symbols.CircleCross, Colors.LightRed), "SeisCrash.Dwn", "Downed by Seismic Crash (Hitbox)", "Seismic Crash Downed", 0)
                    .UsingChecker((hde, log) => hde.To.IsDowned(log, hde.Time)).WithBuilds(GW2Builds.December2024MountBalriorNerfs),
                new PlayerDstSkillMechanic(SeismicCrashHitboxDamage, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Red), "SeisCrash.D", "Seismic Crash Death (Hitbox)", "Seismic Crash Death", 0)
                    .UsingChecker((hde, log) => hde.To.IsDead(log, hde.Time)), // If a player is already in downstate they get killed in NM, not logged in CM
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic([SeismicReposition, SeismicReposition2, SeismicReposition3], new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.White), "SeisRepos.H", "Hit by Seismic Reposition (Concentric Rings Leap)", "Seismic Reposition Hit", 0),
                new PlayerDstHitMechanic([SeismicReposition, SeismicReposition2, SeismicReposition3], new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.DarkWhite), "SeisRepos.CC", "CC by Seismic Reposition (Concentric Rings Leap)", "Seismic Reposition CC", 0)
                    .UsingChecker((hde, log) => !hde.To.HasBuff(log, Stability, hde.Time, ServerDelayConstant)),
            ]),
            new MechanicGroup([

                new PlayerDstHitMechanic([Earthrend, EarthrendCM, EarthrendCM2], new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Blue), "Earthrend.H", "Hit by Earthrend (Outer Doughnut)", "Earthrend Hit", 0),
                new PlayerDstHitMechanic([Earthrend, EarthrendCM, EarthrendCM2], new MechanicPlotlySetting(Symbols.CircleOpen, Colors.DarkBlue), "Earthrend.CC", "CC by Earthrend (Outer Doughnut)", "Earthrend CC", 0)
                    .UsingChecker((hde, log) => !hde.To.HasBuff(log, Stability, hde.Time, ServerDelayConstant)),
                new PlayerDstSkillMechanic(Earthrend, new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.LightRed), "Earthrend.Dwn", "Downed by Earthrend (Hitbox)", "Earthrend Downed", 0)
                    .UsingChecker((hde, log) => hde.To.IsDowned(log, hde.Time))
                    .WithBuilds(GW2Builds.December2024MountBalriorNerfs),
                new PlayerDstSkillMechanic([Earthrend], new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.Red), "Earthrend.D", "Earthrend Death (Hitbox)", "Earthrend Death", 0)
                    .UsingChecker((hde, log) => hde.To.IsDead(log, hde.Time)), // If a player is already in downstate they get killed in NM, not logged in CM
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(Fluxlance, new MechanicPlotlySetting(Symbols.StarSquare, Colors.LightOrange), "Fluxlance.H", "Hit by Fluxlance (Single Orange Arrow)", "Fluxlance Hit", 0),
                new PlayerDstHitMechanic([FluxlanceFusillade, FluxlanceFusilladeCM], new MechanicPlotlySetting(Symbols.StarDiamond, Colors.LightOrange), "FluxFusi.H", "Hit by Fluxlance Fusillade (Sequential Orange Arrows)", "Fluxlance Fusillade Hit", 0),
                new PlayerDstHitMechanic([FluxlanceSalvo1, FluxlanceSalvoCM1, FluxlanceSalvo2, FluxlanceSalvoCM2, FluxlanceSalvo3, FluxlanceSalvoCM3, FluxlanceSalvo4, FluxlanceSalvoCM4, FluxlanceSalvo5, FluxlanceSalvoCM5], new MechanicPlotlySetting(Symbols.StarDiamondOpen, Colors.LightOrange), "FluxSalvo.H", "Hit by Fluxlance Salvo (Simultaneous Orange Arrows)", "Fluxlance Salvo Hit", 0),
                new PlayerDstHitMechanic([Fluxlance, FluxlanceFusillade, FluxlanceFusilladeCM, FluxlanceSalvo1, FluxlanceSalvoCM1, FluxlanceSalvo2, FluxlanceSalvoCM2, FluxlanceSalvo3, FluxlanceSalvoCM3, FluxlanceSalvo4, FluxlanceSalvoCM4, FluxlanceSalvo5, FluxlanceSalvoCM5], new MechanicPlotlySetting(Symbols.DiamondWide, Colors.DarkMagenta), "FluxInc.H", "Hit by Fluxlance with Harmonic Sensitivity", "Fluxlance with Harmonic Sensitivity Hit", 0)
                    .UsingChecker((hde, log) => hde.To.HasBuff(log, HarmonicSensitivity, hde.Time, ServerDelayConstant)),
                new PlayerDstBuffApplyMechanic(FluxlanceTargetBuff1, new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.Orange), "Fluxlance.T", "Targeted by Fluxlance", "Fluxlance Target", 0),
                new PlayerDstBuffApplyMechanic(FluxlanceRedArrowTargetBuff, new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.Red), "FluxRed.T", "Targeted by Fluxlance (Red Arrow)", "Fluxlance (Red Arrow)", 0),
                new PlayerDstBuffApplyMechanic([TargetOrder1JW, TargetOrder2JW, TargetOrder3JW, TargetOrder4JW, TargetOrder5JW], new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.LightOrange), "FluxOrder.T", "Targeted by Fluxlance (Target Order)", "Fluxlance Target (Sequential)", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic([SparkingAuraTier1, SparkingAuraTier1CM], new MechanicPlotlySetting(Symbols.CircleX, Colors.Green), "SparkAura1.H", "Sparking Aura (Absorbed Tier 1 Green Damage)", "Absorbed Tier 1 Green", 0),
                new PlayerDstHitMechanic([SparkingAuraTier2, SparkingAuraTier2CM], new MechanicPlotlySetting(Symbols.CircleX, Colors.LightMilitaryGreen), "SparkAura2.H", "Sparking Aura (Absorbed Tier 2 Green Damage)", "Absorbed Tier 2 Green", 0),
                new PlayerDstHitMechanic([SparkingAuraTier3, SparkingAuraTier3CM], new MechanicPlotlySetting(Symbols.CircleX, Colors.DarkGreen), "SparkAura3.H", "Sparking Aura (Absorbed Tier 3 Green Damage)", "Absorbed Tier 3 Green", 0),
                new PlayerDstHitMechanic([SparkingAuraTier1, SparkingAuraTier1CM, SparkingAuraTier2, SparkingAuraTier2CM, SparkingAuraTier3, SparkingAuraTier3CM], new MechanicPlotlySetting(Symbols.CircleX, Colors.MilitaryGreen), "SparkAuraInc.H", "Hit by Sparking Aura with Galvanic Sensitivity", "Sparking Aura with Galvanic Sensitivity Hit", 0)
                    .UsingChecker((hde, log) => hde.To.HasBuff(log, GalvanicSensitivity, hde.Time, ServerDelayConstant)),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic([FulgentFence, FulgentFenceCM], new MechanicPlotlySetting(Symbols.Octagon, Colors.Purple), "FulFence.H", "Hit by Fulgent Fence (Barriers between Conduits)", "Fulgence Fence Hit", 0),
                new PlayerDstHitMechanic([FulgentAuraTier1, FulgentAuraTier1CM, FulgentAuraTier2, FulgentAuraTier2CM, FulgentAuraTier3, FulgentAuraTier3CM], new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Purple), "FulAura.H", "Hit by Fulgent Aura (Conduit AoE)", "Fulgent Aura Hit", 0),
                new PlayerDstHitMechanic([ReverberatingImpact, ReverberatingImpactCM], new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightBlue), "RevImpact.H", "Hit by Reverberating Impact (Hit a Conduit)", "Reverberating Impact Hit", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(Earthfall, new MechanicPlotlySetting(Symbols.YUp, Colors.LightPink), "Earthfall.H", "Hit by Earthfall (Transcendent Boudlers Jump)", "Earthfall Hit", 0),
                new PlayerDstHitMechanic(Sparkwave, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.LightOrange), "Sparkwave.H", "Hit by Sparkwave (Transcendent Boulders Cone)", "Sparkwave Hit", 0),
                new PlayerDstHitMechanic(ChargedGround, new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.CobaltBlue), "CharGrnd.H", "Hit by Charged Ground (Transcendent Boulders AoEs)", "Charged Ground Hit", 0),
            ]),
            new PlayerDstHitMechanic([FulgentFenceCM, FluxlanceFusilladeCM, FluxlanceSalvoCM1, FluxlanceSalvoCM2, FluxlanceSalvoCM3, FluxlanceSalvoCM4, FluxlanceSalvoCM5, ChorusOfThunderCM, DiscordantThunderCM, HarmoniousThunder], new MechanicPlotlySetting(Symbols.Pentagon, Colors.Lime), "BugDance.Achiv", "Achievement Eligibility: This Bug Can Dance", "Achiv: This Bug Can Dance", 0).UsingChecker((adhe, log) =>
            {
                // If you are dead, lose the achievement
                if (adhe.To.IsDead(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    return true;
                }

                // If you get hit by Fulgent Fence, lose the achievement
                if (adhe.SkillID == FulgentFenceCM && adhe.HasHit)
                {
                    return true;
                }

                var damageTaken = log.CombatData.GetDamageTakenData(adhe.To);
                bool hasExposed = log.CombatData.GetBuffData(ExposedPlayer).Any(x => x is BuffApplyEvent && x.To == adhe.To && Math.Abs(x.Time - adhe.Time) < ServerDelayConstant);

                // If you get hit by your own Fluxlance only during the current sequence, keep the achievement
                // If you get hit by 2 Fluxlance in the current sequence, lose the achievement
                long[] fluxlanceIDs = [FluxlanceFusilladeCM, FluxlanceSalvoCM1, FluxlanceSalvoCM2, FluxlanceSalvoCM3, FluxlanceSalvoCM4, FluxlanceSalvoCM5];
                var fluxlanceTimes = damageTaken.Where(x => (fluxlanceIDs.Contains(x.SkillID)) && x.HasHit).Select(x => x.Time).OrderBy(x => x);
                foreach (long fluxlanceTime in fluxlanceTimes)
                {
                    // Fluxlance sequence lasts about 5 seconds, giving it 7 as margin
                    if (Math.Abs(fluxlanceTime - adhe.Time) < 7000 && fluxlanceIDs.Contains(adhe.SkillID) && hasExposed)
                    {
                        return true;
                    }
                }

                // If you get hit by your own thunder, keep the achievement
                // If you get hit by a thunder on another player or on a conduit, lose the achievement
                long[] thunderIDs = [ChorusOfThunderCM, DiscordantThunderCM, HarmoniousThunder];
                var thunderTimes = damageTaken.Where(x => (thunderIDs.Contains(x.SkillID)) && x.HasHit).Select(x => x.Time).OrderBy(x => x);
                foreach (long thunderTime in thunderTimes)
                {
                    if (Math.Abs(thunderTime - adhe.Time) < ServerDelayConstant && thunderIDs.Contains(adhe.SkillID) && hasExposed)
                    {
                        return true;
                    }
                }

                return false;
            })
                .UsingEnable(log => log.FightData.IsCM)
                .UsingAchievementEligibility(),
            new EnemyDstBuffApplyMechanic(ChargeDecima, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.DarkMagenta), "Charge", "Charge Stacks", "Charge Stack", 0),
            new EnemyDstBuffApplyMechanic(Exposed31589, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.LightPurple), "Exposed", "Got Exposed (Broke Breakbar)", "Exposed", 0),
        ]);

    public DecimaTheStormsinger(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "decima";
        Icon = EncounterIconDecima;
        ChestID = ChestID.DecimasChest;
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000002;
    }
    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayDecimaTheStormsinger,
                        (1602, 1602),
                        (-13068, 10300, -7141, 16227));
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Decima,
            TargetID.DecimaCM,
            TargetID.TranscendentBoulder,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.GreenOrb1Player,
            TargetID.GreenOrb1PlayerCM,
            TargetID.GreenOrb2Players,
            TargetID.GreenOrb2PlayersCM,
            TargetID.GreenOrb3Players,
            TargetID.GreenOrb3PlayersCM,
            TargetID.EnlightenedConduitCM,
            TargetID.EnlightenedConduit,
            TargetID.EnlightenedConduitGadget,
            TargetID.BigEnlightenedConduitGadget,
            TargetID.DecimaBeamStart,
            TargetID.DecimaBeamStartCM,
            TargetID.DecimaBeamEnd,
            TargetID.DecimaBeamEndCM,
        ];
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(ThrummingPresenceBuff, ThrummingPresenceDamage),
            new DamageCastFinder(ThrummingPresenceBuffCM, ThrummingPresenceDamageCM),
        ];
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericFightOffset(fightData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            var decima = agentData.GetNPCsByID(isCM ? TargetID.DecimaCM : TargetID.Decima).FirstOrDefault() ?? throw new MissingKeyActorsException("Decima not found");
            var determined = combatData.Where(x => (x.IsBuffApply() || x.IsBuffRemoval()) && x.SkillID == Determined762);
            var determinedLost = determined.Where(x => x.IsBuffRemoval() && x.DstMatchesAgent(decima)).FirstOrDefault();
            var determinedApply = determined.Where(x => x.IsBuffApply() && x.SrcMatchesAgent(decima)).FirstOrDefault();
            var enterCombatTime = GetEnterCombatTime(fightData, agentData, combatData, logStartNPCUpdate.Time, GenericTriggerID, logStartNPCUpdate.DstAgent);
            if (determinedLost != null && enterCombatTime >= determinedLost.Time)
            {
                return determinedLost.Time;
            } 
            else if (determinedApply != null)
            {
                return decima.LastAware;
            }
            return decima.FirstAware;
        }
        return startToUse;
    }

    internal static void FindConduits(AgentData agentData, List<CombatItem> combatData)
    {
        var maxHPEventsAgents = combatData
            .Where(x => x.IsStateChange == StateChange.MaxHealthUpdate && MaxHealthUpdateEvent.GetMaxHealth(x) == 15276)
            .Select(x => agentData.GetAgent(x.SrcAgent, x.Time));
        var conduitsGadgets = maxHPEventsAgents
            .Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 100 && x.HitboxHeight == 200)
            .Distinct();
        var effects = combatData.Where(x => x.IsEffect && agentData.GetAgent(x.SrcAgent, x.Time).IsSpecies(TargetID.EnlightenedConduitCM));
        foreach (var conduitGadget in conduitsGadgets)
        {
            conduitGadget.OverrideID(TargetID.EnlightenedConduitGadget, agentData);
            conduitGadget.OverrideType(AgentItem.AgentType.NPC, agentData);
            var effectByConduitOnGadget = effects
                .Where(x => x.DstMatchesAgent(conduitGadget)).FirstOrDefault();
            if (effectByConduitOnGadget != null)
            {
                conduitGadget.SetMaster(agentData.GetAgent(effectByConduitOnGadget.SrcAgent, effectByConduitOnGadget.Time));
            }
        }
        var bigConduitsGadgets = maxHPEventsAgents
            .Where(x => x.Type == AgentItem.AgentType.Gadget)
            .Distinct();

        foreach (var conduitGadget in conduitsGadgets)
        {
            conduitGadget.OverrideID(TargetID.BigEnlightenedConduitGadget, agentData);
            conduitGadget.OverrideType(AgentItem.AgentType.NPC, agentData);
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindConduits(agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
    }

    private static PhaseData GetBoulderPhase(ParsedEvtcLog log, IEnumerable<SingleActor> boulders, string name, SingleActor decima)
    {
        long start = long.MaxValue;
        long end = long.MinValue;
        foreach (SingleActor boulder in boulders) {
            start = Math.Min(boulder.FirstAware, start);
            var deadEvent = log.CombatData.GetDeadEvents(boulder.AgentItem).FirstOrDefault();
            if (deadEvent != null)
            {
                end = Math.Max(deadEvent.Time, end);
            } 
            else
            {
                end = Math.Max(boulder.LastAware, end);
            }
        }
        var phase = new PhaseData(start, end, name);
        phase.AddTargets(boulders, log);
        phase.AddTarget(decima, log, PhaseData.TargetPriority.Blocking);
        return phase;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor decima = Decima;
        phases[0].AddTarget(decima, log);
        if (!requirePhases)
        {
            return phases;
        }
        // Invul check
        phases.AddRange(GetPhasesByInvul(log, isCM ? NovaShieldCM : NovaShield, decima, true, true));
        List<PhaseData> mainPhases = new List<PhaseData>(3);
        var currentMainPhase = 1;
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 0)
            {
                phase.Name = "Split " + (currentMainPhase++);
                if (isCM && i < phases.Count - 1)
                {
                    var nextMainPhase = phases[i + 1];
                    var fractureArmorStatus = decima.GetBuffStatus(log, FracturedArmorCM, nextMainPhase.Start + ServerDelayConstant);
                    if (fractureArmorStatus.Value > 0)
                    {
                        phase.OverrideEnd(fractureArmorStatus.End);
                        nextMainPhase.OverrideStart(fractureArmorStatus.End);
                    }
                }
                // Decima gets nova shield during enrage, not a phase
                if (decima.GetBuffStatus(log, ChargeDecima, phase.Start + ServerDelayConstant).Value < 10)
                {
                    phase.AddTarget(decima, log);
                }
            }
            else if (i % 2 == 1)
            {
                mainPhases.Add(phase);
                phase.Name = "Phase " + (currentMainPhase);
                phase.AddTarget(decima, log);
            }
        }
        // Final phases + Boulder phases
        if (isCM)
        {
            var finalSeismicJumpEvent = log.CombatData.GetBuffData(SeismicRepositionInvul).FirstOrDefault(x => x is BuffApplyEvent && x.To == decima.AgentItem);
            if (finalSeismicJumpEvent != null)
            {
                var preFinalPhase = new PhaseData(phases[^1].Start, finalSeismicJumpEvent.Time, "40% - 10%");
                preFinalPhase.AddParentPhases(mainPhases);
                preFinalPhase.AddTarget(Decima, log);
                phases.Add(preFinalPhase);
                var finalPhaseStartEvent = log.CombatData.GetBuffRemoveAllData(SeismicRepositionInvul).FirstOrDefault(x => x.To == decima.AgentItem);
                if (finalPhaseStartEvent != null)
                {
                    var finalPhase = new PhaseData(finalPhaseStartEvent.Time, log.FightData.FightEnd, "10% - 0%");
                    finalPhase.AddParentPhases(mainPhases);
                    finalPhase.AddTarget(Decima, log);
                    phases.Add(finalPhase);
                }
            }
            var boulders = Targets.Where(x => x.IsSpecies(TargetID.TranscendentBoulder)).OrderBy(x => x.FirstAware);
            phases[0].AddTargets(boulders, log, PhaseData.TargetPriority.Blocking);
            var firstBoulders = boulders.Take(new Range(0, 2));
            if (firstBoulders.Any())
            {
                phases.Add(GetBoulderPhase(log, firstBoulders, "Boulders 1", decima).WithParentPhases(mainPhases));
                var secondBoulders = boulders.Take(new Range(2, 4));
                if (secondBoulders.Any())
                {
                    phases.Add(GetBoulderPhase(log, secondBoulders, "Boulders 2", decima).WithParentPhases(mainPhases));
                }
            }
        }
        return phases;
    }


    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        (long start, long end) lifespan = (replay.TimeOffsets.start, replay.TimeOffsets.end);

        switch (target.ID)
        {
            case (int)TargetID.Decima:
            case (int)TargetID.DecimaCM:
                var casts = target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).ToList();

                // Thrumming Presence - Red Ring around Decima
                var thrummingSegments = target.GetBuffStatus(log, ThrummingPresenceBuffCM, log.FightData.FightStart, log.FightData.FightEnd)
                    .Where(x => x.Value > 0)
                    .Concat(target.GetBuffStatus(log, ThrummingPresenceBuff, log.FightData.FightStart, log.FightData.FightEnd)
                        .Where(x => x.Value > 0)
                    );
                foreach (var segment in thrummingSegments)
                {
                    replay.Decorations.Add(new CircleDecoration(700, segment.TimeSpan, Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false));
                }

                // Add the Charge indicator on top right of the replay
                var chargeSegments = target.GetBuffStatus(log, ChargeDecima, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                foreach (Segment segment in chargeSegments)
                {
                    replay.Decorations.Add(new TextDecoration(segment.TimeSpan, "Decima Charge(s) " + segment.Value + " out of 10", 15, Colors.Red, 1.0, new ScreenSpaceConnector(new Vector2(600, 60))));
                }

                // Mainshock - Pizza Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(target.AgentItem, [EffectGUIDs.DecimaMainshockIndicator, EffectGUIDs.DecimaMainshockIndicatorCM], out var mainshockSlices))
                {
                    foreach (EffectEvent effect in mainshockSlices)
                    {
                        long duration = 2300;
                        long growing = effect.Time + duration;
                        lifespan = effect.ComputeLifespan(log, duration);
                        var rotation = new AngleConnector(effect.Rotation.Z + 90);
                        var slice = (PieDecoration)new PieDecoration(1200, 32, lifespan, Colors.LightOrange, 0.4, new PositionConnector(effect.Position)).UsingRotationConnector(rotation);
                        replay.Decorations.AddWithBorder(slice, Colors.LightOrange, 0.6);
                    }
                }

                // For some reason the effects all start at the same time
                // We sequence them using the skill cast
                var foreshock = casts.Where(x => x.SkillID == Foreshock || x.SkillID == ForeshockCM1 || x.SkillID == ForeshockCM2 || x.SkillID == ForeshockCM3 || x.SkillID == ForeshockCM4);
                foreach (var cast in foreshock)
                {
                    (long start, long end) = (cast.Time, cast.Time + cast.ActualDuration + 3000); // 3s padding as safety
                    long nextStartTime = 0;

                    // Decima's Left Side
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(target.AgentItem, [EffectGUIDs.DecimaForeshockLeft, EffectGUIDs.DecimaForeshockLeftCM], out var foreshockLeft))
                    {
                        foreach (EffectEvent effect in foreshockLeft.Where(x => x.Time >= start && x.Time < end))
                        {
                            lifespan = effect.ComputeLifespan(log, 1967);
                            nextStartTime = lifespan.end;
                            var rotation = new AngleConnector(effect.Rotation.Z + 90);
                            var leftHalf = (PieDecoration)new PieDecoration(1185, 180, lifespan, Colors.LightOrange, 0.4, new PositionConnector(effect.Position)).UsingRotationConnector(rotation);
                            replay.Decorations.AddWithBorder(leftHalf, Colors.LightOrange, 0.6);
                        }
                    }

                    // Decima's Right Side
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(target.AgentItem, [EffectGUIDs.DecimaForeshockRight, EffectGUIDs.DecimaForeshockRightCM], out var foreshockRight))
                    {
                        foreach (EffectEvent effect in foreshockRight.Where(x => x.Time >= start && x.Time < end))
                        {
                            lifespan = effect.ComputeLifespan(log, 3000);
                            lifespan.start = nextStartTime - 700; // Trying to match in game timings
                            nextStartTime = lifespan.end;
                            var rotation = new AngleConnector(effect.Rotation.Z + 90);
                            var rightHalf = (PieDecoration)new PieDecoration(1185, 180, lifespan, Colors.LightOrange, 0.4, new PositionConnector(effect.Position)).UsingRotationConnector(rotation);
                            replay.Decorations.AddWithBorder(rightHalf, Colors.LightOrange, 0.6);
                        }
                    }

                    // Decima's Frontal
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(target.AgentItem, [EffectGUIDs.DecimaForeshockFrontal, EffectGUIDs.DecimaForeshockFrontalCM], out var foreshockFrontal))
                    {
                        foreach (EffectEvent effect in foreshockFrontal.Where(x => x.Time >= start && x.Time < end))
                        {
                            lifespan = effect.ComputeLifespan(log, 5100);
                            lifespan.start = nextStartTime;
                            var frontalCircle = new CircleDecoration(600, lifespan, Colors.LightOrange, 0.4, new PositionConnector(effect.Position));
                            replay.Decorations.AddWithBorder(frontalCircle, Colors.LightOrange, 0.6);
                        }
                    }
                }

                // Earthrend - Outer Sliced Doughnut - 8 Slices
                if (log.CombatData.TryGetGroupedEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaEarthrendDoughnutSlice, out var earthrend))
                {
                    // Since we don't have a decoration shaped like this, we regroup the 8 effects and use Decima position as the center for a doughnut sliced by lines.
                    foreach (List<EffectEvent> group in earthrend)
                    {
                        uint inner = 1200;
                        uint outer = 3000;
                        int lineAngle = 45;
                        var offset = new Vector3(0, inner + (outer - inner) / 2, 0);
                        lifespan = group[0].ComputeLifespan(log, 2800);

                        if (target.TryGetCurrentFacingDirection(log, group[0].Time, out Vector3 facing, 100))
                        {
                            for (int i = 0; i < 360; i += lineAngle)
                            {
                                var rotation = facing.GetRoundedZRotationDeg() + i;
                                var line = new RectangleDecoration(10, outer - inner, lifespan, Colors.LightOrange, 0.6, new AgentConnector(target).WithOffset(offset, true)).UsingRotationConnector(new AngleConnector(rotation));
                                replay.Decorations.Add(line);
                            }
                        }

                        var doughnut = new DoughnutDecoration(inner, outer, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target));
                        replay.Decorations.AddWithBorder(doughnut, Colors.LightOrange, 0.6);
                    }
                }

                // Seismic Crash - Jump with rings
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaSeismicCrashRings, out var seismicCrash))
                {
                    foreach (var effect in seismicCrash)
                    {
                        lifespan = effect.ComputeLifespan(log, 3000);
                        replay.Decorations.AddContrenticRings(300, 140, lifespan, effect.Position, Colors.LightOrange, 0.30f, 6, false);
                    }
                }

                // Jump Death Zone
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaJumpAoEUnderneath, out var deathZone))
                {
                    foreach (var effect in deathZone)
                    {
                        // Logged effect has 2 durations depending on attack - 3000 and 2500
                        lifespan = effect.ComputeLifespan(log, effect.Duration);
                        var zone = new CircleDecoration(300, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(zone, effect.Time + effect.Duration);
                    }
                }

                // Aftershock - Moving AoEs - 4 Cascades
                if (log.CombatData.TryGetGroupedEffectEventsBySrcWithGUIDs(target.AgentItem, [EffectGUIDs.DecimaAftershockAoECM, EffectGUIDs.DecimaAftershockAoE], out var aftershock, 12000))
                {
                    // All the AoEs take roughly 11-12 seconds to appear
                    // There are 10 AoEs of radius 200, then 10 of 240, 10 of 280 and 10 of 320. When they bounce back to Decima they restart at 200 radius.
                    uint radius = 200;
                    float distance = 0;
                    EffectEvent first = aftershock.First().First();
                    long groupStartTime = first.Time;

                    // Because the x9th and the x0th can happen at the same timestamp, we need to check the distance of the from Decima.
                    // A simple increase every 10 can happen to increase the x9th instead of the following x0th.
                    if (target.TryGetCurrentPosition(log, first.Time, out Vector3 decimaPosition))
                    {
                        foreach (var group in aftershock)
                        {
                            foreach (var effect in group)
                            {
                                distance = (effect.Position - decimaPosition).XY().Length();
                                if (distance > 1074 && distance < 1076 || distance > 1759 && distance < 1761)
                                {
                                    radius = 200;
                                }
                                if (distance > 1324 && distance < 1326 || distance > 1528 && distance < 1530)
                                {
                                    radius = 240;
                                }
                                if (distance > 1574 && distance < 1576 || distance > 1297 && distance < 1299)
                                {
                                    radius = 280;
                                }
                                if (distance > 1824 && distance < 1826 || distance > 1066 && distance < 1068)
                                {
                                    radius = 320;
                                }
                                lifespan = effect.ComputeLifespan(log, 1500);
                                var zone = (CircleDecoration)new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingFilled(false);
                                replay.Decorations.Add(zone);
                            }
                        }
                    }
                }

                // Flux Nova - Breakbar
                var breakbarUpdates = target.GetBreakbarPercentUpdates(log);
                var (breakbarNones, breakbarActives, breakbarImmunes, breakbarRecoverings) = target.GetBreakbarStatus(log);
                foreach (var segment in breakbarActives)
                {
                    replay.Decorations.AddActiveBreakbar(segment.TimeSpan, target, breakbarUpdates);
                }
                break;
            case (int)TargetID.GreenOrb1Player:
            case (int)TargetID.GreenOrb1PlayerCM:
                // Green Circle
                replay.Decorations.Add(new CircleDecoration(90, lifespan, Colors.DarkGreen, 0.2, new AgentConnector(target)));

                // Overhead Number
                replay.Decorations.AddOverheadIcon(lifespan, target, ParserIcons.GreenMarkerSize1Overhead);

                // Hp Bar
                var hpUpdatesOrb1 = target.GetHealthUpdates(log);
                replay.Decorations.Add(
                    new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMinorSizeInPixel, lifespan, Colors.SligthlyDarkGreen, 0.8, Colors.Black, 0.6, hpUpdatesOrb1.Select(x => (x.Start, x.Value)).ToList(), new AgentConnector(target))
                    .UsingInterpolationMethod(Connector.InterpolationMethod.Step)
                    .UsingRotationConnector(new AngleConnector(180))
                );
                break;
            case (int)TargetID.GreenOrb2Players:
            case (int)TargetID.GreenOrb2PlayersCM:
                // Green Circle
                replay.Decorations.Add(new CircleDecoration(185, lifespan, Colors.DarkGreen, 0.2, new AgentConnector(target)));

                // Overhead Number
                replay.Decorations.AddOverheadIcon(lifespan, target, ParserIcons.GreenMarkerSize2Overhead);

                // Hp Bar
                var hpUpdatesOrb2 = target.GetHealthUpdates(log);
                replay.Decorations.Add(
                    new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMinorSizeInPixel, lifespan, Colors.SligthlyDarkGreen, 0.8, Colors.Black, 0.6, hpUpdatesOrb2.Select(x => (x.Start, x.Value)).ToList(), new AgentConnector(target))
                    .UsingInterpolationMethod(Connector.InterpolationMethod.Step)
                    .UsingRotationConnector(new AngleConnector(180))
                );
                break;
            case (int)TargetID.GreenOrb3Players:
            case (int)TargetID.GreenOrb3PlayersCM:
                // Green Circle
                replay.Decorations.Add(new CircleDecoration(285, lifespan, Colors.DarkGreen, 0.2, new AgentConnector(target)));

                // Overhead Number
                replay.Decorations.AddOverheadIcon(lifespan, target, ParserIcons.GreenMarkerSize3Overhead);

                // Hp Bar
                var hpUpdatesOrb3 = target.GetHealthUpdates(log);
                replay.Decorations.Add(
                    new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMinorSizeInPixel, lifespan, Colors.SligthlyDarkGreen, 0.8, Colors.Black, 0.6, hpUpdatesOrb3.Select(x => (x.Start, x.Value)).ToList(), new AgentConnector(target))
                    .UsingInterpolationMethod(Connector.InterpolationMethod.Step)
                    .UsingRotationConnector(new AngleConnector(180))
                );
                break;
            case (int)TargetID.EnlightenedConduit:
                AddThunderAoE(target, log, replay);
                AddEnlightenedConduitDecorations(log, target, replay, FluxlanceTargetBuff1, DecimaConduitWallWarningBuffCM, DecimaConduitWallBuff);
                    break;
            case (int)TargetID.EnlightenedConduitCM:
                AddEnlightenedConduitDecorations(log, target, replay, FluxlanceTargetBuffCM1, DecimaConduitWallWarningBuff, DecimaConduitWallBuffCM);
                break;
            case (int)TargetID.EnlightenedConduitGadget:
                var gadgetConnectorAgent = target.AgentItem.GetFinalMaster();
                var gadgetEffectConnector = new AgentConnector(gadgetConnectorAgent);
                List<long> chargeTierBuffs = [EnlightenedConduitGadgetChargeTier1Buff, EnlightenedConduitGadgetChargeTier2Buff, EnlightenedConduitGadgetChargeTier3Buff];
                List<uint> chargeRadius = [100, 200, 400];
                List<string> chargeIcons = [ParserIcons.TargetOrder1Overhead, ParserIcons.TargetOrder2Overhead, ParserIcons.TargetOrder3Overhead];
                if (target.AgentItem.Master != null)
                {
                    chargeTierBuffs = [EnlightenedConduitGadgetChargeTier1BuffCM, EnlightenedConduitGadgetChargeTier2BuffCM, EnlightenedConduitGadgetChargeTier3BuffCM];
                    // Chorus of Thunder / Discordant Thunder - Orange AoE
                    AddThunderAoE(target, log, replay);
                }
                // Fulgent Aura - Tier Charges
                for (int i = 0; i <  chargeTierBuffs.Count; i++)
                {
                    var tier = target.GetBuffStatus(log, chargeTierBuffs[i], log.FightData.FightStart, log.FightData.FightEnd);
                    foreach (var segment in tier.Where(x => x.Value > 0))
                    {
                        replay.Decorations.AddWithBorder(new CircleDecoration(chargeRadius[i], segment.TimeSpan, Colors.DarkPurple, 0.4, gadgetEffectConnector), Colors.Red, 0.4);
                        replay.Decorations.AddOverheadIcon(segment.TimeSpan, gadgetConnectorAgent, chargeIcons[i]);
                    }
                }
                break;
            case (int)TargetID.DecimaBeamStart:
                const uint beamLength = 3900;
                const uint orangeBeamWidth = 80;
                const uint redBeamWidth = 160;
                var orangeBeams = GetFilteredList(log.CombatData, DecimaBeamTargeting, target.AgentItem, true, true);
                AddBeamWarning(log, target, replay, DecimaBeamLoading, orangeBeamWidth, beamLength, orangeBeams.OfType<BuffApplyEvent>(), Colors.LightOrange);
                AddBeam(log, replay, orangeBeamWidth, orangeBeams, Colors.LightOrange);

                var redBeams = GetFilteredList(log.CombatData, DecimaRedBeamTargeting, target.AgentItem, true, true);
                AddBeamWarning(log, target, replay, DecimaRedBeamLoading, redBeamWidth, beamLength, redBeams.OfType<BuffApplyEvent>(), Colors.Red);
                AddBeam(log, replay, redBeamWidth, redBeams, Colors.Red);
                break;
            case (int)TargetID.DecimaBeamStartCM:
                const uint beamLengthCM = 3900;
                const uint orangeBeamWidthCM = 80;
                const uint redBeamWidthCM = 160;
                var orangeBeamsCM = GetFilteredList(log.CombatData, DecimaBeamTargetingCM, target.AgentItem, true, true);
                AddBeamWarning(log, target, replay, DecimaBeamLoadingCM1, orangeBeamWidthCM, beamLengthCM, orangeBeamsCM.OfType<BuffApplyEvent>(), Colors.LightOrange);
                AddBeamWarning(log, target, replay, DecimaBeamLoadingCM2, orangeBeamWidthCM, beamLengthCM, orangeBeamsCM.OfType<BuffApplyEvent>(), Colors.LightOrange);
                AddBeam(log, replay, orangeBeamWidthCM, orangeBeamsCM, Colors.LightOrange);

                var redBeamsCM = GetFilteredList(log.CombatData, DecimaRedBeamTargetingCM, target.AgentItem, true, true);
                AddBeamWarning(log, target, replay, DecimaRedBeamLoadingCM1, redBeamWidthCM, beamLengthCM, redBeamsCM.OfType<BuffApplyEvent>(), Colors.Red);
                AddBeamWarning(log, target, replay, DecimaRedBeamLoadingCM2, redBeamWidthCM, beamLengthCM, redBeamsCM.OfType<BuffApplyEvent>(), Colors.Red);
                AddBeam(log, replay, redBeamWidthCM, redBeamsCM, Colors.Red);
                break;
            case (int)TargetID.TranscendentBoulder:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillID)
                    {
                        // Sparking Reverberation - Breakbar
                        case SparkingReverberation:
                            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaSparkingReverberation, out var effects))
                            {
                                foreach (var effect in effects.Where(x => x.Time > cast.Time && x.Time < cast.Time + 2000)) // 2000ms margin
                                {
                                    uint radius = 800;
                                    long warningDuration = effect.Time - cast.Time;
                                    (long start, long end) lifespanWarning = (cast.Time, effect.Time);
                                    (long start, long end) lifespanDamage = effect.ComputeDynamicLifespan(log, 30000);
                                    lifespanWarning.end = ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, warningDuration); // Cast can be interrupted

                                    var warningIndicator = new CircleDecoration(radius, lifespanWarning, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                                    replay.Decorations.AddWithGrowing(warningIndicator, effect.Time);

                                    var damageField = new CircleDecoration(radius, lifespanDamage, Colors.LightBlue, 0.1, new PositionConnector(effect.Position));
                                    replay.Decorations.Add(damageField);
                                }
                            }
                            break;
                        // Sparkwave - Cone
                        case Sparkwave:
                            long castDuration = 1800;
                            (long start, long end) lifespanSparkwave = (cast.Time, Math.Min(cast.EndTime, cast.Time + castDuration));
                            if (target.TryGetCurrentFacingDirection(log, cast.Time + castDuration, out var facing))
                            {
                                var cone = (PieDecoration)new PieDecoration(6000, 120, lifespanSparkwave, Colors.LightOrange, 0.2, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facing));
                                replay.Decorations.AddWithGrowing(cone, cast.Time + castDuration);
                            }
                            break;
                        default:
                            break;
                    }
                }

                // Charged Ground - AoEs from Sparkwave
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaChargedGroundBorder, out var chargedGround))
                {
                    foreach (var effect in chargedGround)
                    {
                        (long start, long end) lifespanChargedGround = (effect.Time, effect.Time + effect.Duration);
                        var circle = new CircleDecoration(400, lifespanChargedGround, Colors.CobaltBlue, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithBorder(circle, Colors.Red, 0.2);
                    }
                }

                // Charged Ground - AoEs from Sparkwave - Max Charge
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaChargedGroundMax, out var chargedGroundMax))
                {
                    foreach (var effect in chargedGroundMax)
                    {
                        (long start, long end) lifespanChargedGround = effect.ComputeLifespan(log, 600000);
                        var circle = new CircleDecoration(400, lifespanChargedGround, Colors.Red, 0.4, new PositionConnector(effect.Position));
                        replay.Decorations.Add(circle);
                    }
                }
                break;
            default:
                break;
        }
    }

    private static void AddEnlightenedConduitDecorations(ParsedEvtcLog log, SingleActor target, CombatReplay replay, long fluxLanceTargetBuffID, long wallWarningBuffID, long wallBuffID)
    {

        // Focused Fluxlance - Green Arrow from Decima to the Conduit
        var greenArrow = GetFilteredList(log.CombatData, fluxLanceTargetBuffID, target, true, true).Where(x => x is BuffApplyEvent);
        foreach (var apply in greenArrow)
        {
            replay.Decorations.Add(new LineDecoration((apply.Time, apply.Time + 5500), Colors.DarkGreen, 0.2, new AgentConnector(apply.To), new AgentConnector(apply.By)).WithThickess(80, true));
            replay.Decorations.Add(new LineDecoration((apply.Time + 5500, apply.Time + 6500), Colors.DarkGreen, 0.5, new AgentConnector(apply.To), new AgentConnector(apply.By)).WithThickess(80, true));
        }

        // Warning indicator of walls spawning between Conduits.
        var wallsWarnings = GetFilteredList(log.CombatData, wallWarningBuffID, target, true, true);
        replay.Decorations.AddTether(wallsWarnings, Colors.Red, 0.2, 30, true);

        // Walls connecting Conduits to each other.
        var walls = GetFilteredList(log.CombatData, wallBuffID, target, true, true);
        replay.Decorations.AddTether(walls, Colors.Purple, 0.4, 60, true);
    }

    private static void AddBeam(ParsedEvtcLog log, CombatReplay replay, uint beamWidth, IEnumerable<BuffEvent> beams, Color color)
    {
        int tetherStart = 0;
        AgentItem src = _unknownAgent;
        AgentItem dst = _unknownAgent;
        foreach (BuffEvent tether in beams)
        {
            if (tether is BuffApplyEvent)
            {
                tetherStart = (int)tether.Time;
                src = tether.By;
                dst = tether.To;
            }
            else if (tether is BuffRemoveAllEvent)
            {
                int tetherEnd = (int)tether.Time;
                if (!src.IsUnknown && !dst.IsUnknown)
                {
                    if (src.TryGetCurrentInterpolatedPosition(log, tetherStart, out var posSrc))
                    {
                        // Get the position before movement happened
                        if (dst.TryGetCurrentInterpolatedPosition(log, tetherStart - 500, out var posDst))
                        {
                            replay.Decorations.Add(new LineDecoration((tetherStart, tetherEnd), color, 0.5, new PositionConnector(posSrc), new PositionConnector(posDst)).WithThickess(beamWidth, true));
                        }
                        src = _unknownAgent;
                        dst = _unknownAgent;
                    }
                }
            }
        }
    }

    private static void AddBeamWarning(ParsedEvtcLog log, SingleActor target, CombatReplay replay, long buffID, uint beamWidth, uint beamLength, IEnumerable<BuffApplyEvent> beamFireds, Color color)
    {
        var beamWarnings = target.AgentItem.GetBuffStatus(log, buffID, log.FightData.FightStart, log.FightData.FightEnd);
        foreach (var beamWarning in beamWarnings)
        {
            if (beamWarning.Value > 0)
            {
                long start = beamWarning.Start;
                long end = beamFireds.FirstOrDefault(x => x.Time >= start)?.Time ?? beamWarning.End;
                // We ignore the movement of the agent, it moves closer to target before firing
                if (target.TryGetCurrentInterpolatedPosition(log, start, out var posDst))
                {
                    var connector = new PositionConnector(posDst).WithOffset(new(beamLength / 2, 0, 0), true);
                    var rotationConnector = new AgentFacingConnector(target);
                    replay.Decorations.Add(new RectangleDecoration(beamLength, beamWidth, (start, end), color, 0.2, connector).UsingRotationConnector(rotationConnector));
                }
            }
        }
    }


    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);

        // Target Overhead
        // In phase 2 you get the Fluxlance Target Buff but also Target Order, in game only Target Order is displayed overhead, so we filter those out.
        var p2Targets = player.GetBuffStatus(log, [TargetOrder1JW, TargetOrder2JW, TargetOrder3JW, TargetOrder4JW, TargetOrder5JW], log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        var allTargets = player.GetBuffStatus(log, FluxlanceTargetBuff1, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        var filtered = allTargets.Where(x => !p2Targets.Any(y => Math.Abs(x.Start - y.Start) < ServerDelayConstant));
        foreach (var segment in filtered)
        {
            replay.Decorations.AddOverheadIcon(segment, player, ParserIcons.TargetOverhead);
        }

        // Target Order Overhead
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder1JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder1Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder2JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder2Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder3JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder3Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder4JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder4Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder5JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder5Overhead);

        // Chorus of Thunder / Discordant Thunder - Orange AoE
        AddThunderAoE(player, log, replay);
    }

    /// <summary>
    /// Chorus of Thunder / Discordant Thunder - Orange spread AoE on players or on Conduits.
    /// </summary>
    private static void AddThunderAoE(SingleActor actor, ParsedEvtcLog log, CombatReplay replay)
    {
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(actor.AgentItem, EffectGUIDs.DecimaChorusOfThunderAoE, out var thunders))
        {
            AgentItem dst = (actor.AgentItem.Master != null ? actor.AgentItem.Master : actor.AgentItem);
            foreach (var effect in thunders)
            {
                long duration = 5000;
                long growing = effect.Time + duration;
                (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
                replay.Decorations.AddWithGrowing(new CircleDecoration(285, lifespan, Colors.LightOrange, 0.2, new AgentConnector(dst)), growing);
            }
        }
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return isCM ? FightData.EncounterMode.CMNoName : FightData.EncounterMode.Normal;
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success && isCM)
        {
            if (Decima != null && !Decima.GetBuffStatus(log, ChargeDecima, log.FightData.FightStart, log.FightData.FightEnd).Any(x => x.Value > 0))
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIDs[AchievementEligibilityCalmBeforeTheStorm], 1));
            }
        }
    }
}
