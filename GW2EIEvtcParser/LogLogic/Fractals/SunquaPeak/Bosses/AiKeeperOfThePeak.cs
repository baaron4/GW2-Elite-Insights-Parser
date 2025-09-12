using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class AiKeeperOfThePeak : SunquaPeak
{
    internal const long Determined895DurationCheckForSuccess = int.MaxValue / 4;

    internal readonly MechanicGroup Mechanics = new MechanicGroup(
        [
            // General
            new PlayerDstHealthDamageHitMechanic(ElementalWhirl, new MechanicPlotlySetting(Symbols.Square,Colors.LightRed), "Ele.Whrl.", "Elemental Whirl","Elemental Whirl", 0),
            // Air
            new MechanicGroup(
                [
                    new PlayerDstHealthDamageHitMechanic([ElementalManipulationAir1, ElementalManipulationAir2, ElementalManipulationAir3], new MechanicPlotlySetting(Symbols.Square,Colors.Magenta), "Ar.Mnp.", "Elemental Manipulation (Air)","Elemental Manipulation (Air)", 0),
                    new PlayerDstHealthDamageHitMechanic([FulgorSphere1, FulgorSphere2], new MechanicPlotlySetting(Symbols.Circle,Colors.Magenta), "Flg.Sph.", "Fulgor Sphere","Fulgor Sphere", 0),
                    new PlayerDstHealthDamageHitMechanic(VolatileWind, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Magenta), "Vlt.Wnd.", "Volatile Wind","Volatile Wind", 0),
                    new MechanicGroup(
                        [
                            new PlayerDstHealthDamageHitMechanic(WindBurst, new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Magenta), "Wnd.Brst.", "Wind Burst","Wind Burst", 0)
                                .WithStabilitySubMechanic(
                                    new PlayerDstHealthDamageHitMechanic(WindBurst, new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Magenta), "L.Wnd.Burst", "Launched up by Wind Burst","Wind Burst Launch", 0),
                                    false
                                ),
                        ]
                    ),
                    new PlayerDstHealthDamageHitMechanic(CallOfStorms , new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Magenta), "Call.Strs", "Call of Storms","Call of Storms", 0),
                    new EnemyDstBuffApplyMechanic(WhirlwindShield, new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.Magenta),"W.Shield" , "Whirlwind Shield","Whirlwind Shield",0),
                ]
            ),
            // Fire
            new MechanicGroup(
                [

                    new PlayerDstHealthDamageHitMechanic([ElementalManipulationFire1, ElementalManipulationFire2, ElementalManipulationFire3], new MechanicPlotlySetting(Symbols.Square,Colors.Orange), "Fr.Mnp.", "Elemental Manipulation (Fire)","Elemental Manipulation (Fire)", 0),
                    new PlayerDstHealthDamageHitMechanic([RoilingFlames1, RoilingFlames2], new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Rlng.Flms.", "Roiling Flames","Roiling Flames", 0),
                    new PlayerDstHealthDamageHitMechanic(VolatileFire, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Orange), "Vlt.Fr.", "Volatile Fire","Volatile Fire", 0),
                    new MechanicGroup(
                        [
                            new EnemyCastStartMechanic(CallMeteorSummon, new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.Orange), "Smn.Meteor", "Summoned Meteor", "Summon Meteor", 0),
                            new PlayerDstHealthDamageMechanic(CallMeteorHit, new MechanicPlotlySetting(Symbols.Hexagram,Colors.Orange), "Mtr.H", "Hit by Meteor","Meteor Hit", 1000)
                            .UsingChecker((evt, log) => evt.HasDowned || evt.HasKilled),
                        ]
                    ),
                    new PlayerDstHealthDamageHitMechanic(FlameBurst, new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Orange), "Flm.Brst.", "Flame Burst","Flame Burst", 0),
                    new PlayerDstHealthDamageHitMechanic(FirestormAi, new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Orange), "Fr.Strm", "Firestorm","Firestorm", 0),
                ]
            ),
            // Water
            new MechanicGroup(
                [
                    new PlayerDstHealthDamageHitMechanic([ElementalManipulationWater1, ElementalManipulationWater2, ElementalManipulationWater3], new MechanicPlotlySetting(Symbols.Square,Colors.LightBlue), "Wtr.Mnp.", "Elemental Manipulation (Water)","Elemental Manipulation (Water)", 0),
                    new PlayerDstHealthDamageHitMechanic([TorrentialBolt1, TorrentialBolt2], new MechanicPlotlySetting(Symbols.Circle,Colors.LightBlue), "Tr.Blt.", "Torrential Bolt","Torrential Bolt", 0),
                    new PlayerDstHealthDamageHitMechanic(VolatileWater, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.LightBlue), "Vlt.Wtr.", "Volatile Water","Volatile Water", 0),
                    new PlayerDstHealthDamageHitMechanic(AquaticBurst, new MechanicPlotlySetting(Symbols.TriangleDown,Colors.LightBlue), "Aq.Brst.", "Aquatic Burst","Aquatic Burst", 0),
                    new EnemyDstBuffApplyMechanic(TidalBarrier, new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.LightBlue), "Tdl.Bar.", "Tidal Barrier", "Tidal Barrier", 0),
                    new MechanicGroup(
                        [
                            new PlayerDstBuffApplyMechanic(TidalBargain, new MechanicPlotlySetting(Symbols.StarOpen,Colors.LightBlue), "Tdl.Brgn.", "Downed by Tidal Bargain","Tidal Bargain", 0),
                            new PlayerDstBuffRemoveMechanic(TidalBargain, new MechanicPlotlySetting(Symbols.Star,Colors.LightBlue), "Tdl.Brgn.Dwn.","Downed by Tidal Bargain", "Tidal Bargain Downed",0)
                                .UsingChecker((evt, log) => evt.RemovedStacks == 10 && Math.Abs(evt.RemovedDuration - 90000) < 10 * ServerDelayConstant && log.CombatData.GetBuffDataByIDByDst(Downed, evt.To).Any(x => Math.Abs(x.Time - evt.Time) < 50 && x is BuffApplyEvent bae)),
                        ]
                    ),
                ]
            ),
            // Dark
            new MechanicGroup(
                [
                    new PlayerDstHealthDamageHitMechanic([EmpathicManipulationGuilt, EmpathicManipulation2, EmpathicManipulationSorrow, EmpathicManipulationFear, EmpathicManipulation5, EmpathicManipulation6, EmpathicManipulation7, EmpathicManipulation8, EmpathicManipulation9], new MechanicPlotlySetting(Symbols.Square,Colors.LightPurple), "Emp.Mnp.", "Empathic Manipulation","Empathic Manipulation", 0),
                    new PlayerDstHealthDamageHitMechanic([FocusedWrath, FocusedWrath2], new MechanicPlotlySetting(Symbols.Circle,Colors.LightPurple), "Fcsd.Wrth.", "Focused Wrath","Focused Wrath", 0),
                    new PlayerDstHealthDamageHitMechanic(NegativeBurst, new MechanicPlotlySetting(Symbols.DiamondWide,Colors.LightPurple), "N.Brst.", "Negative Burst","Negative Burst", 500),
                    new PlayerDstHealthDamageHitMechanic(Terrorstorm, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.LightPurple), "TrrStrm", "Terrorstorm","Terrorstorm", 0),
                    new MechanicGroup(
                        [
                            new PlayerDstBuffApplyMechanic(CrushingGuilt, new MechanicPlotlySetting(Symbols.StarOpen,Colors.LightPurple), "Crsh.Glt.", "Crushing Guilt","Crushing Guilt", 0),
                            new PlayerDstBuffRemoveMechanic(CrushingGuilt, new MechanicPlotlySetting(Symbols.Star,Colors.LightPurple), "Crsh.Glt.Dwn.","Downed by Crushing Guilt", "Crushing Guilt Down",0)
                                .UsingChecker((evt, log) => evt.RemovedStacks == 10 && Math.Abs(evt.RemovedDuration - 90000) < 10 * ServerDelayConstant && log.CombatData.GetBuffDataByIDByDst(Downed, evt.To).Any(x => Math.Abs(x.Time - evt.Time) < 50 && x is BuffApplyEvent bae)),
                        ]
                    ),
                    new PlayerDstBuffApplyMechanic([FixatedFear1, FixatedFear2, FixatedFear3, FixatedFear4], new MechanicPlotlySetting(Symbols.Bowtie, Colors.Purple), "Fear.Fix.A", "Fixated by Fear", "Fixated Application", 0),
                    new MechanicGroup(
                        [
                            new EnemyCastStartMechanic(EmpathicManipulationFear, new MechanicPlotlySetting(Symbols.TriangleUp,Colors.LightPurple), "Fear Mnp.", "Empathic Manipulation (Fear)", "Empathic Manipulation (Fear)", 0),
                            new EnemyCastEndMechanic(EmpathicManipulationFear, new MechanicPlotlySetting(Symbols.TriangleUpOpen,Colors.LightPurple), "IntFear.Mnp.", "Empathic Manipulation (Fear) Interrupt", "Empathic Manipulation (Fear) Interrupt", 0)
                                .UsingChecker((evt, log) => evt is AnimatedCastEvent ace && ace.IsInterrupted),
                            new EnemyCastStartMechanic(EmpathicManipulationSorrow, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.LightPurple), "Sor.Mnp.", "Empathic Manipulation (Sorrow)", "Empathic Manipulation (Sorrow)", 0),
                            new EnemyCastEndMechanic(EmpathicManipulationSorrow, new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.LightPurple), "IntSor.Mnp.", "Empathic Manipulation (Sorrow) Interrupt", "Empathic Manipulation (Sorrow) Interrupt", 0)
                                .UsingChecker((evt, log) => evt is AnimatedCastEvent ace && ace.IsInterrupted),
                            new EnemyCastStartMechanic(EmpathicManipulationGuilt, new MechanicPlotlySetting(Symbols.TriangleRight,Colors.LightPurple), "Glt.Mnp.", "Empathic Manipulation (Guilt)", "Empathic Manipulation (Guilt)", 0),
                            new EnemyCastEndMechanic(EmpathicManipulationGuilt, new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.LightPurple), "Int.Glt.Mnp.", "Empathic Manipulation (Guilt) Interrupt", "Empathic Manipulation (Guilt) Interrupt", 0)
                                .UsingChecker((evt, log) => evt is AnimatedCastEvent ace && ace.IsInterrupted),
                        ]
                    ),
                    new EnemyDstBuffApplyMechanic(CacophonousMind, new MechanicPlotlySetting(Symbols.Pentagon,Colors.LightPurple), "Ccphns.Mnd.", "Cacophonous Mind","Cacophonous Mind", 0),
                ]
            ),
        ]);
    public AiKeeperOfThePeak(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "ai";
        Icon = EncounterIconAi;
    }

    internal const long FullAiMask = 0x000001;
    internal const long ElementalAiMask = 0x000002;
    internal const long DarkAiMask = 0x000003;

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        if (HasDarkMode(agentData))
        {
            if (HasElementalMode(agentData))
            {
                LogID |= FullAiMask;
                Icon = EncounterIconAi;
                Extension = "ai";
                return "Ai, Keeper of the Peak";
            }
            LogID |= DarkAiMask;
            Icon = EncounterIconAiDark;
            Extension = "drkai";
            return "Dark Ai, Keeper of the Peak";
        }
        else
        {
            LogID |= ElementalAiMask;
            Icon = EncounterIconAiElemental;
            Extension = "elai";
            return "Elemental Ai, Keeper of the Peak";
        }
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (823, 1000),
                        (5411, -95, 8413, 3552));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayAi, crMap);
        return crMap;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.AiKeeperOfThePeak,
            TargetID.DarkAiKeeperOfThePeak,
            TargetID.CCSorrowDemon,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        var trashIDs = new List<TargetID>(9 + base.GetTrashMobsIDs().Count);
        trashIDs.AddRange(base.GetTrashMobsIDs());
        trashIDs.Add(TargetID.FearDemon);
        trashIDs.Add(TargetID.GuiltDemon);
        trashIDs.Add(TargetID.AiDoubtDemon);
        trashIDs.Add(TargetID.PlayerDoubtDemon);
        trashIDs.Add(TargetID.EnragedWaterSprite);
        // Transition sorrow demons
        trashIDs.Add(TargetID.TransitionSorrowDemon1);
        trashIDs.Add(TargetID.TransitionSorrowDemon2);
        trashIDs.Add(TargetID.TransitionSorrowDemon3);
        trashIDs.Add(TargetID.TransitionSorrowDemon4);

        return trashIDs;
    }

    private static bool HasDarkMode(AgentData agentData)
    {
        return agentData.GetNPCsByID(TargetID.DarkAiKeeperOfThePeak).Count > 0;
    }

    private static bool HasElementalMode(AgentData agentData)
    {
        return agentData.GetNPCsByID(TargetID.AiKeeperOfThePeak).Count > 0;
    }

    internal static void DetectAis(AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        foreach (var aiAgent in agentData.GetNPCsByID(TargetID.AiKeeperOfThePeak))
        {
            var aiCastEvents = combatData.Where(x => x.StartCasting() && x.SrcMatchesAgent(aiAgent));
            var china = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.Language && LanguageEvent.GetLanguage(x) == LanguageEvent.LanguageEnum.Chinese) != null;
            CombatItem? darkModePhaseEvent = aiCastEvents.FirstOrDefault(x => x.SkillID == AiDarkPhaseEvent);
            var hasDarkMode = combatData.Exists(x => (china ? x.SkillID == AiHasDarkModeCN_SurgeOfDarkness : x.SkillID == AiHasDarkMode_SurgeOfDarkness) && x.SrcMatchesAgent(aiAgent));
            var hasElementalMode = !hasDarkMode || darkModePhaseEvent != null;
            if (hasDarkMode)
            {
                if (hasElementalMode)
                {
                    long darkModeStart = aiCastEvents.FirstOrDefault(x => (china ? x.SkillID == AiDarkModeStartCN : x.SkillID == AiDarkModeStart) && x.Time >= darkModePhaseEvent!.Time)!.Time;
                    CombatItem? invul895Loss = combatData.FirstOrDefault(x => x.Time <= darkModeStart && x.SkillID == Determined895 && x.IsBuffRemove == BuffRemove.All && x.SrcMatchesAgent(aiAgent) && x.Value > Determined895DurationCheckForSuccess);
                    long elementalLastAwareTime = (invul895Loss != null ? invul895Loss.Time : darkModeStart);
                    AgentItem darkAiAgent = agentData.AddCustomNPCAgent(elementalLastAwareTime, aiAgent.LastAware, aiAgent.Name, aiAgent.Spec, TargetID.DarkAiKeeperOfThePeak, false, aiAgent.Toughness, aiAgent.Healing, aiAgent.Condition, aiAgent.Concentration, aiAgent.HitboxWidth, aiAgent.HitboxHeight);
                    AgentManipulationHelper.RedirectNPCEventsAndCopyPreviousStates(combatData, extensions, agentData, aiAgent, [aiAgent], darkAiAgent, false);
                    aiAgent.OverrideAwareTimes(aiAgent.FirstAware, elementalLastAwareTime);
                }
                else
                {
                    aiAgent.OverrideID(TargetID.DarkAiKeeperOfThePeak, agentData);
                }
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.AiKeeperOfThePeak, out var aiAgent))
        {
            throw new MissingKeyActorsException("Ai not found");
        }
        DetectAis(agentData, combatData, extensions);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        // Manually set HP and names
        SingleActor? eleAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak));
        SingleActor? darkAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DarkAiKeeperOfThePeak));
        darkAi?.OverrideName("Dark Ai, Keeper of the Peak");
        eleAi?.OverrideName("Elemental Ai, Keeper of the Peak");
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        // check invulnerability remove for new elemental ai
        var ai = agentData.GetNPCsByID(TargetID.AiKeeperOfThePeak).FirstOrDefault() ?? throw new MissingKeyActorsException("Ai not found");
        var invulnStart = GetLogOffsetByInvulnStart(logData, combatData, ai, Determined895);
        if (invulnStart > ai.FirstAware)
        {
            return invulnStart;
        }

        // first cast or fallback to regular offset (combat enter) for dark ai
        // old elemental ai will always end up with fallback due to idle time
        var start = base.GetLogOffset(evtcVersion, logData, agentData, combatData);
        var firstCast = combatData.Where(x => x.StartCasting() && x.SrcMatchesAgent(ai)).FirstOrDefault();
        if (firstCast != null)
        {
            return Math.Min(start, firstCast.Time);
        }
        return start;
    }

    internal override LogData.LogStartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {
        if (HasElementalMode(agentData))
        {
            if (TargetHPPercentUnderThreshold(TargetID.AiKeeperOfThePeak, logData.LogStart, combatData, Targets))
            {
                return LogData.LogStartStatus.Late;
            }
        }
        else if (HasDarkMode(agentData))
        {
            if (TargetHPPercentUnderThreshold(TargetID.DarkAiKeeperOfThePeak, logData.LogStart, combatData, Targets))
            {
                return LogData.LogStartStatus.Late;
            }
        }
        return LogData.LogStartStatus.Normal;
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.LogMode.CMNoName;
    }

    internal static List<PhaseData> ComputeElementalPhases(ParsedEvtcLog log, SingleActor elementalAi, PhaseData elementalPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<PhaseData>(3);
        // sub phases
        string[] eleNames = ["Air", "Fire", "Water"];
        var elementalSubPhases = GetPhasesByInvul(log, Determined762, elementalAi, false, true, elementalPhase.Start, elementalPhase.End).Take(3).ToList();
        for (int i = 0; i < elementalSubPhases.Count; i++)
        {
            PhaseData phase = elementalSubPhases[i];
            phase.Name = eleNames[i];
            phase.AddParentPhase(elementalPhase);
            phase.AddTarget(elementalAi, log);
            if (i > 0)
            {
                // try to use transition skill, fallback to determined loss
                // long skillId = _china ? 61388 : 61385;
                long skillID = 61187;
                var casts = elementalAi.GetCastEvents(log, phase.Start, phase.End);
                // use last cast since determined is fixed 5s and the transition out (ai flying up) can happen after loss
                CastEvent? castEvt = casts.LastOrDefault(x => x.SkillID == skillID);
                if (castEvt != null)
                {
                    phase.OverrideStart(castEvt.Time);
                }
                else
                {
                    phase.Name += " (Fallback)";
                }
            }
        }
        phases.AddRange(elementalSubPhases);
        return phases;
    }

    internal static List<PhaseData> ComputeDarkPhases(ParsedEvtcLog log, SingleActor darkAi, PhaseData darkPhase, bool china, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<PhaseData>(3);
        // sub phases
        long fearToSorrowSkillID = china ? EmpathicManipulationSorrowCN : EmpathicManipulationSorrow;
        var darkAiCasts = darkAi.GetCastEvents(log, darkPhase.Start, darkPhase.End);
        CastEvent? fearToSorrow = darkAiCasts.FirstOrDefault(x => x.SkillID == fearToSorrowSkillID);
        if (fearToSorrow != null)
        {
            var fearPhase = new SubPhasePhaseData(darkPhase.Start, fearToSorrow.Time, "Fear");
            fearPhase.AddTarget(darkAi, log);
            fearPhase.AddParentPhase(darkPhase);
            phases.Add(fearPhase);
            long sorrowToGuiltSkillID = china ? EmpathicManipulationGuiltCN : EmpathicManipulationGuilt;
            CastEvent? sorrowToGuilt = darkAiCasts.FirstOrDefault(x => x.SkillID == sorrowToGuiltSkillID);
            if (sorrowToGuilt != null)
            {
                var sorrowPhase = new SubPhasePhaseData(fearToSorrow.Time, sorrowToGuilt.Time, "Sorrow");
                sorrowPhase.AddTarget(darkAi, log);
                sorrowPhase.AddParentPhase(darkPhase);
                phases.Add(sorrowPhase);
                var guiltPhase = new SubPhasePhaseData(sorrowToGuilt.Time, darkPhase.End, "Guilt");
                guiltPhase.AddTarget(darkAi, log);
                guiltPhase.AddParentPhase(darkPhase);
                phases.Add(guiltPhase);
            }
            else
            {
                var sorrowPhase = new SubPhasePhaseData(fearToSorrow.Time, darkPhase.End, "Sorrow");
                sorrowPhase.AddTarget(darkAi, log);
                sorrowPhase.AddParentPhase(darkPhase);
                phases.Add(sorrowPhase);
            }
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor? elementalAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak));
        if (elementalAi == null && HasElementalMode(log.AgentData))
        {
            throw new MissingKeyActorsException("Elemental Ai not found");
        }
        SingleActor? darkAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.DarkAiKeeperOfThePeak));
        if (darkAi == null && HasDarkMode(log.AgentData))
        {
            throw new MissingKeyActorsException("Dark Ai not found");
        }
        if (elementalAi != null)
        {
            phases[0].AddTarget(elementalAi, log);
        }
        if (darkAi != null)
        {
            phases[0].AddTarget(darkAi, log);
        }
        if (elementalAi != null)
        {
            PhaseData elePhase = phases[0];
            if (darkAi != null)
            {
                BuffApplyEvent? invul895Gain = log.CombatData.GetBuffApplyDataByIDByDst(Determined895, elementalAi.AgentItem)
                    .OfType<BuffApplyEvent>()
                    .Where(x => x.AppliedDuration > Determined895DurationCheckForSuccess)
                    .FirstOrDefault();
                long eleStart = Math.Max(elementalAi.FirstAware, log.LogData.LogStart);
                long eleEnd = invul895Gain != null ? Math.Min(invul895Gain.Time + ServerDelayConstant, log.LogData.LogEnd) : log.LogData.LogEnd;
                elePhase = new SubPhasePhaseData(eleStart, eleEnd, "Elemental Phase");
                elePhase.AddTarget(elementalAi, log);
                phases.Add(elePhase);
                elePhase.AddParentPhase(phases[0]);
            }
            phases.AddRange(ComputeElementalPhases(log, elementalAi, elePhase, requirePhases));
        }
        if (darkAi != null)
        {
            var china = log.CombatData.GetLanguageEvent()?.Language == LanguageEvent.LanguageEnum.Chinese;
            PhaseData darkPhase = phases[0];
            if (elementalAi != null)
            {
                BuffApplyEvent? invul895Gain = log.CombatData.GetBuffDataByIDByDst(Determined895, darkAi.AgentItem)
                    .OfType<BuffApplyEvent>()
                    .Where(x => x.AppliedDuration > Determined895DurationCheckForSuccess)
                    .FirstOrDefault();
                long darkStart = Math.Max(darkAi.FirstAware, log.LogData.LogStart);
                long darkEnd = invul895Gain != null ? Math.Min(invul895Gain.Time + ServerDelayConstant, log.LogData.LogEnd) : log.LogData.LogEnd;
                darkPhase = new SubPhasePhaseData(darkStart, darkEnd, "Dark Phase");
                darkPhase.AddTarget(darkAi, log);
                phases.Add(darkPhase);
                darkPhase.AddParentPhase(phases[0]);
            }
            phases.AddRange(ComputeDarkPhases(log, darkAi, darkPhase, china,requirePhases));
        }
        return phases;
    }

    protected override IReadOnlyList<TargetID> GetSuccessCheckIDs()
    {
        return [TargetID.AiKeeperOfThePeak, TargetID.DarkAiKeeperOfThePeak];
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        int status = 0;
        if (HasElementalMode(agentData))
        {
            status |= 1;
        }
        if (HasDarkMode(agentData))
        {
            status |= 2;
        }
        switch (status)
        {
            case 1:
            case 2:
                var ai = Targets[0];
                BuffApplyEvent? invul895Gain = combatData.GetBuffApplyDataByIDByDst(Determined895, ai.AgentItem).OfType<BuffApplyEvent>().Where(x => x.AppliedDuration > Determined895DurationCheckForSuccess).FirstOrDefault();
                if (invul895Gain != null)
                {
                    logData.SetSuccess(true, invul895Gain.Time);
                }
                else
                {
                    logData.SetSuccess(false, ai.LastAware);
                }
                break;
            case 3:
                var darkAi = Targets.First(y => y.IsSpecies(TargetID.DarkAiKeeperOfThePeak));
                BuffApplyEvent? darkInvul895Gain = combatData.GetBuffApplyDataByIDByDst(Determined895, darkAi.AgentItem).OfType<BuffApplyEvent>().Where(x => x.AppliedDuration > Determined895DurationCheckForSuccess).FirstOrDefault();
                if (darkInvul895Gain != null)
                {
                    logData.SetSuccess(true, darkInvul895Gain.Time);
                } 
                else
                {
                    logData.SetSuccess(false, darkAi.LastAware);
                }
                break;
            case 0:
            default:
                throw new MissingKeyActorsException("Ai not found");
        }
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log, List<(Buff buff, int stack)> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);

        if (log.LogData.Success && HasDarkMode(log.AgentData) && HasElementalMode(log.AgentData))
        {
            if (log.CombatData.GetBuffData(AchievementEligibilityDancingWithDemons).Any())
            {
                int counter = 0;
                foreach (Player p in log.PlayerList)
                {
                    if (p.HasBuff(log, AchievementEligibilityDancingWithDemons, log.LogData.LogEnd - ServerDelayConstant))
                    {
                        counter++;
                    }
                }
                // The achievement requires 5 players alive with the buff, if the instance has only 4 players inside, you cannot get it.
                if (counter == 5)
                {
                    instanceBuffs.Add((log.Buffs.BuffsByIDs[AchievementEligibilityDancingWithDemons], 1));
                }
            }
            if (log.CombatData.GetBuffData(AchievementEligibilityEnergyDispersal).Any())
            {
                instanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityEnergyDispersal));
            }
        }
    }

    private static AgentItem? GetAiAgentAt(IReadOnlyList<SingleActor> targets, long time)
    {
        AgentItem? elementalAi = targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak))?.AgentItem;
        AgentItem? darkAi = targets.FirstOrDefault(x => x.IsSpecies(TargetID.DarkAiKeeperOfThePeak))?.AgentItem;
        if (elementalAi != null && elementalAi.InAwareTimes(time))
        {
            return elementalAi;
        }
        else if (darkAi != null && darkAi.InAwareTimes(time))
        {
            return darkAi;
        }
        return null;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);

        // tether between sprite and player
        var spriteFixations = GetBuffApplyRemoveSequence(log.CombatData, [FixatedEnragedWaterSprite], p, true, true);
        replay.Decorations.AddTether(spriteFixations, Colors.Purple, 0.5);

        // Tethering Players to Fears
        var fearFixations = GetBuffApplyRemoveSequence(log.CombatData, [FixatedFear1, FixatedFear2, FixatedFear3, FixatedFear4], p, true, true);
        replay.Decorations.AddTether(fearFixations, Colors.Magenta, 0.5);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputeNPCCombatReplayActors(target, log, replay);

        long growing;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.CCSorrowDemon:
                {
                    const long sorrowFullCastDuration = 11840;
                    const long sorrowHitDelay = 400;
                    const uint sorrowIndicatorSize = 2000;
                    var casts = target.GetAnimatedCastEvents(log).ToList();

                    foreach (CastEvent cast in casts)
                    {
                        switch (cast.SkillID)
                        {
                            // Overwhelming Sorrow - Explosion
                            case OverwhelmingSorrowWindup:
                                growing = cast.Time + sorrowFullCastDuration;
                                lifespan = (cast.Time, growing);
                                CastEvent? detonate = casts.FirstOrDefault(x => x.SkillID == OverwhelmingSorrowDetonate && x.Time > lifespan.start && x.Time < lifespan.end + ServerDelayConstant);
                                if (detonate != null)
                                {
                                    lifespan.end = detonate.Time;
                                    long hit = detonate.Time + sorrowHitDelay;
                                    replay.Decorations.Add(new CircleDecoration(sorrowIndicatorSize, (hit, hit + 300), Colors.Red, 0.15, new AgentConnector(target)));
                                }
                                else
                                {
                                    // attempt to find end while handling missing cast durations
                                    lifespan.end = cast.EndTime > ServerDelayConstant ? cast.EndTime : lifespan.end;
                                    DeadEvent? dead = log.CombatData.GetDeadEvents(target.AgentItem).FirstOrDefault();
                                    if (dead != null)
                                    {
                                        lifespan.end = Math.Min(lifespan.end, dead.Time);
                                    }
                                }
                                var circle = new CircleDecoration(sorrowIndicatorSize, lifespan, Colors.Orange, 0.15, new AgentConnector(target)).UsingFilled(false);
                                replay.Decorations.AddWithFilledWithGrowing(circle, true, growing);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                }
            case (int)TargetID.GuiltDemon:
                {
                    // tether between guilt and player/boss, buff applied TO guilt
                    var fixationBuffs = GetBuffApplyRemoveSequence(log.CombatData, [FixatedGuilt], target, true, true);
                    replay.Decorations.AddTether(fixationBuffs, Colors.DarkPurple, 0.5);
                    break;
                }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        // arrow indicators
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiArrowAttackIndicator, out var arrows))
        {
            const uint width = 80;
            const uint length = 360;
            foreach (EffectEvent effect in arrows)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 1800);
                var rotation = new AngleConnector(effect.Rotation.Z);
                GeographicalConnector position = new PositionConnector(effect.Position).WithOffset(new(0f, 0.5f * length, 0), true);
                environmentDecorations.Add(new RectangleDecoration(width, length, lifespan, Colors.Orange, 0.15, position).UsingRotationConnector(rotation));
            }
        }

        // orbs
        if (log.CombatData.TryGetEffectEventsByGUIDs([EffectGUIDs.AiAirOrbFloat, EffectGUIDs.AiFireOrbFloat, EffectGUIDs.AiWaterOrbFloat], out var orbs))
        {
            foreach (EffectEvent effect in orbs)
            {
                long start = effect.Time;
                long end = start + 5000;
                var position = new PositionConnector(effect.Position);
                environmentDecorations.Add(new CircleDecoration(180, (start, end), Colors.Red, 0.3, position).UsingFilled(false));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiDarkOrbFloat, out var darkOrbs))
        {
            foreach (EffectEvent effect in darkOrbs)
            {
                long start = effect.Time;
                long end = start + 5000;
                var position = new PositionConnector(effect.Position);
                environmentDecorations.Add(new CircleDecoration(180, (start, end), Colors.Purple, 0.3, position).UsingFilled(false));
            }
        }

        // spreads
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiSpreadCircle, out var spreadIndicators))
        {
            var spreadDetonateGUIDs = new[] { EffectGUIDs.AiAirDetonate, EffectGUIDs.AiFireDetonate, EffectGUIDs.AiWaterDetonate, EffectGUIDs.AiDarkDetonate };
            const float maxDist = 40f;
            const long duration = 5000;

            bool hasDetonates = log.CombatData.TryGetEffectEventsByGUIDs(spreadDetonateGUIDs, out var detonates);
            foreach (EffectEvent effect in spreadIndicators)
            {
                long start = effect.Time;
                long end = start + duration;
                var position = new AgentConnector(effect.Dst);
                environmentDecorations.Add(new CircleDecoration(600, (start, end), Colors.Orange, 0.3, position).UsingFilled(false));
                environmentDecorations.Add(new CircleDecoration(600, (start, end), Colors.Orange, 0.15, position).UsingGrowingEnd(end));
                if (hasDetonates)
                {
                    effect.Dst.TryGetCurrentPosition(log, end, out var endPos);
                    EffectEvent? detonate = detonates.FirstOrDefault(x => Math.Abs(x.Time - end) < ServerDelayConstant && (x.Position - endPos).XY().Length() < maxDist);
                    if (detonate != null)
                    {
                        environmentDecorations.Add(new CircleDecoration(600, (detonate.Time, detonate.Time + 300), Colors.Red, 0.15, new PositionConnector(detonate.Position)));
                    }
                }
            }
        }

        // frontals
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiConeIndicator, out var frontal))
        {
            foreach (EffectEvent effect in frontal)
            {
                long start = effect.Time;
                long end = start + 2000;
                var position = new PositionConnector(effect.Position);
                var rotation = new AgentFacingConnector(effect.Src);
                environmentDecorations.Add(new PieDecoration(240, 170f, (start, end), Colors.Orange, 0.15, position).UsingRotationConnector(rotation));
                environmentDecorations.Add(new PieDecoration(240, 170f, (end, end + 300), Colors.Red, 0.15, position).UsingRotationConnector(rotation));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiAoEAroundIndicator, out var around))
        {
            foreach (EffectEvent effect in around)
            {
                long start = effect.Time;
                long end = start + 2100;
                var position = new PositionConnector(effect.Position);
                environmentDecorations.Add(new CircleDecoration(300, (start, end), Colors.Orange, 0.15, position));
                environmentDecorations.Add(new CircleDecoration(300, (end, end + 300), Colors.Red, 0.15, position));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiRedPointblankIndicator, out var aroundRed))
        {
            foreach (EffectEvent effect in aroundRed)
            {
                long start = effect.Time;
                long end = start + 4000;
                var position = new PositionConnector(effect.Position);
                environmentDecorations.Add(new CircleDecoration(300, (start, end), Colors.Red, 0.15, position));
            }
        }

        // scaled circles
        if (log.CombatData.TryGetEffectEventsByGUIDs([EffectGUIDs.AiAirCircleDetonate, EffectGUIDs.AiFireCircleDetonate], out var circleDetonate))
        {
            AddScalingCircleDecorations(log, circleDetonate, 300, environmentDecorations);
        }
        // we need to filter water & dark detonates due to reuse
        var detonateReusedGUIDs = new[] { EffectGUIDs.AiWaterDetonate, EffectGUIDs.AiDarkCircleDetonate };
        if (log.CombatData.TryGetEffectEventsByGUIDs(detonateReusedGUIDs, out var circleDetonateReused))
        {
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiCircleAoEIndicator, out var indicators))
            {
                IEnumerable<EffectEvent> filteredCircles = circleDetonateReused.Where(detonate =>
                {
                    return indicators.Any(indicator =>
                    {
                        long timeDifference = detonate.Time - indicator.Time;
                        return timeDifference > 0 && timeDifference < 4000 && (detonate.Position - indicator.Position).LengthSquared() < 1.0f;
                    });
                });
                AddScalingCircleDecorations(log, filteredCircles, 300, environmentDecorations);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUIDs([EffectGUIDs.AiAirCirclePulsing, EffectGUIDs.AiFireCirclePulsing, EffectGUIDs.AiDarkCirclePulsing], out var circlePulsing))
        {
            AddScalingCircleDecorations(log, circlePulsing, 8000, environmentDecorations);
        }

        // air intermission lightning strikes
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiAirIntermissionRedCircleIndicator, out var intermissionReds))
        {
            foreach (EffectEvent effect in intermissionReds)
            {
                long start = effect.Time;
                long end = start + 1500;
                var position = new PositionConnector(effect.Position);
                environmentDecorations.Add(new CircleDecoration(300, (start, end), Colors.Orange, 0.15, position));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiAirLightningStrike, out var lightningStrikes))
        {
            foreach (EffectEvent effect in lightningStrikes)
            {
                long start = effect.Time;
                long end = start + 300;
                var position = new PositionConnector(effect.Position);
                environmentDecorations.Add(new CircleDecoration(300, (start, end), Colors.Red, 0.15, position));
            }
        }

        // fire intermission meteors
        AddMeteorDecorations(log, environmentDecorations);

        // water intermission greens
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiGreenCircleIndicator, out var greens))
        {
            foreach (EffectEvent effect in greens)
            {
                long start = effect.Time;
                long end = start + 6250;
                var position = new AgentConnector(effect.Dst);
                environmentDecorations.Add(new CircleDecoration(180, (start, end), Colors.DarkGreen, 0.3, position));
                environmentDecorations.Add(new CircleDecoration(180, (start, end), Colors.DarkGreen, 0.3, position).UsingGrowingEnd(end));
            }
        }

        // water tornados
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiWaterTornadoIndicator2, out var tornados))
        {
            const uint directionLength = 360;
            foreach (EffectEvent effect in tornados)
            {
                long start = effect.Time;
                long end = start + 1800;
                var position = new PositionConnector(effect.Position);
                GeographicalConnector offset = new PositionConnector(effect.Position).WithOffset(new(0f, 0.5f * directionLength, 0), true);
                var rotation = new AngleConnector(effect.Rotation.Z);
                environmentDecorations.Add(new CircleDecoration(180, (start, end), Colors.Orange, 0.15, position));
                environmentDecorations.Add(new RectangleDecoration(80, directionLength, (start, end), Colors.Orange, 0.15, offset).UsingRotationConnector(rotation));
            }
        }
    }

    private static void AddScalingCircleDecorations(ParsedEvtcLog log, IEnumerable<EffectEvent> effects, long damageDuration, CombatReplayDecorationContainer environmentDecorations)
    {
        foreach (EffectEvent effect in effects)
        {
            // TODO: determine duration from previous indicator at same location
            const long indicatorDuration = 1800;

            long start = effect.Time - indicatorDuration;
            long end = effect.Time;

            AgentItem? ai = GetAiAgentAt(log.LogData.Logic.Targets, effect.Time);
            if (ai != null)
            {
                ai.TryGetCurrentPosition(log, start, out var aiPos);
                float dist = (aiPos - effect.Position).XY().Length();

                // actual distances are 400, 670, 1080, 1630
                uint radius;
                if (dist > 1500f)
                {
                    radius = 320;
                }
                else if (dist > 900f)
                {
                    radius = 240;
                }
                else if (dist > 500f)
                {
                    radius = 160;
                }
                else
                {
                    radius = 100;
                }
                var position = new PositionConnector(effect.Position);
                environmentDecorations.Add(new CircleDecoration(radius, (start, end), Colors.Orange, 0.15, position));
                environmentDecorations.Add(new CircleDecoration(radius, (end, end + damageDuration), Colors.Red, 0.15, position));
            }
        }
    }

    // TODO: find proper sizes
    private const uint MeteorInnerSize = 90;
    private const uint MeteorCircleDist = 95;
    private const uint MeteorFullRadius = MeteorInnerSize + 7 * MeteorCircleDist;

    private static void AddMeteorDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiMeteorIndicatorGround, out var groundIndicators))
        {
            foreach (EffectEvent effect in groundIndicators)
            {
                (long, long) lifespan = (effect.Time, effect.Time + 6000);
                GeographicalConnector position = effect.IsAroundDst ? new AgentConnector(effect.Dst) : (GeographicalConnector)new PositionConnector(effect.Position);
                AddMeteorIndicatorDecoration(lifespan, position, Colors.Orange, environmentDecorations);
                environmentDecorations.Add(new CircleDecoration(MeteorFullRadius, lifespan, Colors.Orange, 0.15, position).UsingGrowingEnd(lifespan.Item2));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiMeteorImpact, out var impacts))
        {
            foreach (EffectEvent effect in impacts)
            {
                (long, long) lifespan = (effect.Time, effect.Time + 300);
                var position = new PositionConnector(effect.Position);
                AddMeteorIndicatorDecoration(lifespan, position, Colors.Red, environmentDecorations);
                environmentDecorations.Add(new CircleDecoration(MeteorFullRadius, lifespan, Colors.Red, 0.15, position));
            }
        }
    }

    private static void AddMeteorIndicatorDecoration((long, long) lifespan, GeographicalConnector connector, Color color, CombatReplayDecorationContainer environmentDecorations)
    {
        environmentDecorations.Add(new CircleDecoration(MeteorInnerSize, lifespan, color, 0.3, connector));
        for (uint i = 1; i <= 7; i++)
        {
            uint radius = MeteorInnerSize + i * MeteorCircleDist;
            environmentDecorations.Add(new CircleDecoration(radius, lifespan, color, 0.3, connector).UsingFilled(false));
        }
    }
}
