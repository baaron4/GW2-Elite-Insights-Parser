using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class AiKeeperOfThePeak : SunquaPeak
{
    private bool _hasDarkMode = false;
    private bool _hasElementalMode = false;
    private bool _china = false;

    private const long Determined895Duration = int.MaxValue / 4;
    public AiKeeperOfThePeak(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup(
        [
            // General
            new PlayerDstHitMechanic(ElementalWhirl, new MechanicPlotlySetting(Symbols.Square,Colors.LightRed), "Ele.Whrl.", "Elemental Whirl","Elemental Whirl", 0),
            // Air
            new MechanicGroup(
                [
                    new PlayerDstHitMechanic([ElementalManipulationAir1, ElementalManipulationAir2, ElementalManipulationAir3], new MechanicPlotlySetting(Symbols.Square,Colors.Magenta), "Ar.Mnp.", "Elemental Manipulation (Air)","Elemental Manipulation (Air)", 0),
                    new PlayerDstHitMechanic([FulgorSphere1, FulgorSphere2], new MechanicPlotlySetting(Symbols.Circle,Colors.Magenta), "Flg.Sph.", "Fulgor Sphere","Fulgor Sphere", 0),
                    new PlayerDstHitMechanic(VolatileWind, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Magenta), "Vlt.Wnd.", "Volatile Wind","Volatile Wind", 0),
                    new MechanicGroup(
                        [
                            new PlayerDstHitMechanic(WindBurst, new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Magenta), "Wnd.Brst.", "Wind Burst","Wind Burst", 0),
                            new PlayerDstHitMechanic(WindBurst, new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Magenta), "L.Wnd.Burst", "Launched up by Wind Burst","Wind Burst Launch", 0)
                            .UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
                        ]
                    ),
                    new PlayerDstHitMechanic(CallOfStorms , new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Magenta), "Call.Strs", "Call of Storms","Call of Storms", 0),
                    new EnemyDstBuffApplyMechanic(WhirlwindShield, new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.Magenta),"W.Shield" , "Whirlwind Shield","Whirlwind Shield",0),
                ]
            ),
            // Fire
            new MechanicGroup(
                [

                    new PlayerDstHitMechanic([ElementalManipulationFire1, ElementalManipulationFire2, ElementalManipulationFire3], new MechanicPlotlySetting(Symbols.Square,Colors.Orange), "Fr.Mnp.", "Elemental Manipulation (Fire)","Elemental Manipulation (Fire)", 0),
                    new PlayerDstHitMechanic([RoilingFlames1, RoilingFlames2], new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Rlng.Flms.", "Roiling Flames","Roiling Flames", 0),
                    new PlayerDstHitMechanic(VolatileFire, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Orange), "Vlt.Fr.", "Volatile Fire","Volatile Fire", 0),
                    new MechanicGroup(
                        [
                            new EnemyCastStartMechanic(CallMeteorSummon, new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.Orange), "Smn.Meteor", "Summoned Meteor", "Summon Meteor", 0),
                            new PlayerDstSkillMechanic(CallMeteorHit, new MechanicPlotlySetting(Symbols.Hexagram,Colors.Orange), "Mtr.H", "Hit by Meteor","Meteor Hit", 1000)
                            .UsingChecker((evt, log) => evt.HasDowned || evt.HasKilled),
                        ]
                    ),
                    new PlayerDstHitMechanic(FlameBurst, new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Orange), "Flm.Brst.", "Flame Burst","Flame Burst", 0),
                    new PlayerDstHitMechanic(FirestormAi, new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Orange), "Fr.Strm", "Firestorm","Firestorm", 0),
                ]
            ),
            // Water
            new MechanicGroup(
                [
                    new PlayerDstHitMechanic([ElementalManipulationWater1, ElementalManipulationWater2, ElementalManipulationWater3], new MechanicPlotlySetting(Symbols.Square,Colors.LightBlue), "Wtr.Mnp.", "Elemental Manipulation (Water)","Elemental Manipulation (Water)", 0),
                    new PlayerDstHitMechanic([TorrentialBolt1, TorrentialBolt2], new MechanicPlotlySetting(Symbols.Circle,Colors.LightBlue), "Tr.Blt.", "Torrential Bolt","Torrential Bolt", 0),
                    new PlayerDstHitMechanic(VolatileWater, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.LightBlue), "Vlt.Wtr.", "Volatile Water","Volatile Water", 0),
                    new PlayerDstHitMechanic(AquaticBurst, new MechanicPlotlySetting(Symbols.TriangleDown,Colors.LightBlue), "Aq.Brst.", "Aquatic Burst","Aquatic Burst", 0),
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
                    new PlayerDstHitMechanic([EmpathicManipulationGuilt, EmpathicManipulation2, EmpathicManipulationSorrow, EmpathicManipulationFear, EmpathicManipulation5, EmpathicManipulation6, EmpathicManipulation7, EmpathicManipulation8, EmpathicManipulation9], new MechanicPlotlySetting(Symbols.Square,Colors.LightPurple), "Emp.Mnp.", "Empathic Manipulation","Empathic Manipulation", 0),
                    new PlayerDstHitMechanic([FocusedWrath, FocusedWrath2], new MechanicPlotlySetting(Symbols.Circle,Colors.LightPurple), "Fcsd.Wrth.", "Focused Wrath","Focused Wrath", 0),
                    new PlayerDstHitMechanic(NegativeBurst, new MechanicPlotlySetting(Symbols.DiamondWide,Colors.LightPurple), "N.Brst.", "Negative Burst","Negative Burst", 500),
                    new PlayerDstHitMechanic(Terrorstorm, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.LightPurple), "TrrStrm", "Terrorstorm","Terrorstorm", 0),
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
        ]));
        Extension = "ai";
        Icon = EncounterIconAi;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        if (_hasDarkMode && _hasElementalMode)
        {
            EncounterID |= 0x000001;
            return "Ai, Keeper of the Peak";
        }
        else if (_hasDarkMode)
        {
            EncounterID |= 0x000003;
            return "Dark Ai, Keeper of the Peak";
        }
        else
        {
            EncounterID |= 0x000002;
            return "Elemental Ai, Keeper of the Peak";
        }
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayAi,
                        (823, 1000),
                        (5411, -95, 8413, 3552));
    }

    /*internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
    {
        var res = new List<AbstractBuffEvent>();
        // Tidal Bargain, Cacophonous Mind and Crushing Guilt adjust
        AdjustTimeRefreshBuff(buffsByDst, buffsById, 61512);
        AdjustTimeRefreshBuff(buffsByDst, buffsById, 61208);
        AdjustTimeRefreshBuff(buffsByDst, buffsById, 61435);
        return res;
    }*/

    protected override ReadOnlySpan<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.AiKeeperOfThePeak,
            TargetID.AiKeeperOfThePeak2,
            TargetID.CCSorrowDemon,
        ];
    }

    protected override List<TargetID> GetTrashMobsIDs()
    {
        var trashIDs = base.GetTrashMobsIDs();
        trashIDs.ReserveAdditional(9);
        
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

    protected override ReadOnlySpan<TargetID> GetUniqueNPCIDs()
    {
        return
        [
            TargetID.AiKeeperOfThePeak,
            TargetID.AiKeeperOfThePeak2,
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.AiKeeperOfThePeak, out var aiAgent))
        {
            throw new MissingKeyActorsException("Ai not found");
        }
        var aiCastEvents = combatData.Where(x => x.StartCasting() && x.SrcMatchesAgent(aiAgent));
        _china = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.Language && LanguageEvent.GetLanguage(x) == LanguageEvent.LanguageEnum.Chinese) != null;
        CombatItem? darkModePhaseEvent = aiCastEvents.FirstOrDefault(x => x.SkillID == AiDarkPhaseEvent);
        _hasDarkMode = combatData.Exists(x => (_china ? x.SkillID == AiHasDarkModeCN_SurgeOfDarkness : x.SkillID == AiHasDarkMode_SurgeOfDarkness) && x.SrcMatchesAgent(aiAgent));
        _hasElementalMode = !_hasDarkMode || darkModePhaseEvent != null;
        if (_hasDarkMode)
        {
            if (_hasElementalMode)
            {
                long darkModeStart = aiCastEvents.FirstOrDefault(x => (_china ? x.SkillID == AiDarkModeStartCN : x.SkillID == AiDarkModeStart) && x.Time >= darkModePhaseEvent!.Time)!.Time;
                CombatItem? invul895Loss = combatData.FirstOrDefault(x => x.Time <= darkModeStart && x.SkillID == Determined895 && x.IsBuffRemove == BuffRemove.All && x.SrcMatchesAgent(aiAgent) && x.Value > Determined895Duration);
                long elementalLastAwareTime = (invul895Loss != null ? invul895Loss.Time : darkModeStart);
                AgentItem darkAiAgent = agentData.AddCustomNPCAgent(elementalLastAwareTime, aiAgent.LastAware, aiAgent.Name, aiAgent.Spec, TargetID.AiKeeperOfThePeak2, false, aiAgent.Toughness, aiAgent.Healing, aiAgent.Condition, aiAgent.Concentration, aiAgent.HitboxWidth, aiAgent.HitboxHeight);
                RedirectEventsAndCopyPreviousStates(combatData, extensions, agentData, aiAgent, [aiAgent], darkAiAgent, false);
                aiAgent.OverrideAwareTimes(aiAgent.FirstAware, elementalLastAwareTime);
            }
            else
            {
                Extension = "drkai";
                aiAgent.OverrideID(TargetID.AiKeeperOfThePeak2, agentData);
            }
        }
        else
        {
            Extension = "elai";
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        // Manually set HP and names
        SingleActor? eleAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak));
        SingleActor? darkAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak2));
        darkAi?.OverrideName("Dark Ai");
        eleAi?.OverrideName("Elemental Ai");
        if (_hasDarkMode)
        {
            int sorrowCount = 0;
            foreach (SingleActor target in Targets)
            {
                if (target.IsSpecies(TargetID.CCSorrowDemon))
                {
                    target.OverrideName(target.Character + " " + (++sorrowCount));
                }
            }
        }
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        if (_hasElementalMode)
        {
            if (TargetHPPercentUnderThreshold(TargetID.AiKeeperOfThePeak, fightData.FightStart, combatData, Targets))
            {
                return FightData.EncounterStartStatus.Late;
            }
        }
        else if (_hasDarkMode)
        {
            if (TargetHPPercentUnderThreshold(TargetID.AiKeeperOfThePeak2, fightData.FightStart, combatData, Targets))
            {
                return FightData.EncounterStartStatus.Late;
            }
        }
        return FightData.EncounterStartStatus.Normal;
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.CMNoName;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor? elementalAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak));
        if (elementalAi == null)
        {
            if (_hasElementalMode)
            {
                throw new MissingKeyActorsException("Ai not found");
            }
        }
        else
        {
            phases[0].AddTarget(elementalAi);
        }
        SingleActor? darkAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak2));
        if (darkAi == null)
        {
            if (_hasDarkMode)
            {
                throw new MissingKeyActorsException("Ai not found");
            }
        }
        else
        {
            phases[0].AddTarget(darkAi);
        }
        if (_hasElementalMode)
        {
            BuffApplyEvent? invul895Gain = log.CombatData.GetBuffDataByIDByDst(Determined895, elementalAi!.AgentItem).OfType<BuffApplyEvent>().Where(x => x.AppliedDuration > Determined895Duration).FirstOrDefault();
            long eleStart = Math.Max(elementalAi.FirstAware, log.FightData.FightStart);
            long eleEnd = invul895Gain != null ? invul895Gain.Time : log.FightData.FightEnd;
            var elePhase = phases[0];
            if (_hasDarkMode)
            {
                elePhase = new PhaseData(eleStart, eleEnd, "Elemental Phase");
                elePhase.AddTarget(elementalAi);
                phases.Add(elePhase);
                elePhase.AddParentPhase(phases[0]);
            }
            if (requirePhases)
            {
                // sub phases
                string[] eleNames = ["Air", "Fire", "Water"];
                var elementalPhases = GetPhasesByInvul(log, Determined762, elementalAi, false, true, log.FightData.FightStart, Math.Min(elementalAi.LastAware, log.FightData.FightEnd)).Take(3).ToList();
                for (int i = 0; i < elementalPhases.Count; i++)
                {
                    PhaseData phase = elementalPhases[i];
                    phase.Name = eleNames[i];
                    phase.AddParentPhase(elePhase);
                    phase.AddTarget(elementalAi);
                    if (i > 0)
                    {
                        // try to use transition skill, fallback to determined loss
                        // long skillId = _china ? 61388 : 61385;
                        long skillID = 61187;
                        var casts = elementalAi.GetCastEvents(log, phase.Start, phase.End);
                        // use last cast since determined is fixed 5s and the transition out (ai flying up) can happen after loss
                        CastEvent? castEvt = casts.LastOrDefault(x => x.SkillId == skillID);
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
                phases.AddRange(elementalPhases);
            }
        }
        if (_hasDarkMode)
        {
            BuffApplyEvent? invul895Gain = log.CombatData.GetBuffDataByIDByDst(Determined895, darkAi!.AgentItem).OfType<BuffApplyEvent>().Where(x => x.AppliedDuration > Determined895Duration).FirstOrDefault();
            long darkStart = Math.Max(darkAi.FirstAware, log.FightData.FightStart);
            long darkEnd = invul895Gain != null ? invul895Gain.Time : log.FightData.FightEnd;
            var darkPhase = phases[0];
            if (_hasElementalMode)
            {
                darkPhase = new PhaseData(darkStart, darkEnd, "Dark Phase");
                darkPhase.AddTarget(darkAi);
                phases.Add(darkPhase);
                darkPhase.AddParentPhase(phases[0]);
            }
            if (requirePhases)
            {
                // sub phases
                long fearToSorrowSkillID = _china ? EmpathicManipulationSorrowCN : EmpathicManipulationSorrow;
                CastEvent? fearToSorrow = darkAi.GetCastEvents(log, darkStart, darkEnd).FirstOrDefault(x => x.SkillId == fearToSorrowSkillID);
                if (fearToSorrow != null)
                {
                    var fearPhase = new PhaseData(darkStart, fearToSorrow.Time, "Fear");
                    fearPhase.AddTarget(darkAi);
                    fearPhase.AddParentPhase(darkPhase);
                    phases.Add(fearPhase);
                    long sorrowToGuiltSkillID = _china ? EmpathicManipulationGuiltCN : EmpathicManipulationGuilt;
                    CastEvent? sorrowToGuilt = darkAi.GetCastEvents(log, darkStart, darkEnd).FirstOrDefault(x => x.SkillId == sorrowToGuiltSkillID);
                    if (sorrowToGuilt != null)
                    {
                        var sorrowPhase = new PhaseData(fearToSorrow.Time, sorrowToGuilt.Time, "Sorrow");
                        sorrowPhase.AddTarget(darkAi);
                        sorrowPhase.AddParentPhase(darkPhase);
                        phases.Add(sorrowPhase);
                        var guiltPhase = new PhaseData(sorrowToGuilt.Time, darkEnd, "Guilt");
                        guiltPhase.AddTarget(darkAi);
                        guiltPhase.AddParentPhase(darkPhase);
                        phases.Add(guiltPhase);
                    }
                    else
                    {
                        var sorrowPhase = new PhaseData(fearToSorrow.Time, darkEnd, "Sorrow");
                        sorrowPhase.AddTarget(darkAi);
                        sorrowPhase.AddParentPhase(darkPhase);
                        phases.Add(sorrowPhase);
                    }
                }
            }
        }
        return phases;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        int status = 0;
        if (_hasElementalMode)
        {
            status |= 1;
        }
        if (_hasDarkMode)
        {
            status |= 2;
        }
        switch (status)
        {
            case 1:
            case 2:
                BuffApplyEvent? invul895Gain = combatData.GetBuffDataByIDByDst(Determined895, Targets[0].AgentItem).OfType<BuffApplyEvent>().Where(x => x.AppliedDuration > Determined895Duration).FirstOrDefault();
                if (invul895Gain != null)
                {
                    fightData.SetSuccess(true, invul895Gain.Time);
                }
                break;
            case 3:
                BuffApplyEvent? darkInvul895Gain = combatData.GetBuffDataByIDByDst(Determined895, Targets.First(y => y.IsSpecies(TargetID.AiKeeperOfThePeak2)).AgentItem).OfType<BuffApplyEvent>().Where(x => x.AppliedDuration > Determined895Duration).FirstOrDefault();
                if (darkInvul895Gain != null)
                {
                    fightData.SetSuccess(true, darkInvul895Gain.Time);
                }
                break;
            case 0:
            default:
                throw new MissingKeyActorsException("Ai not found");
        }
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success && _hasDarkMode && _hasElementalMode)
        {
            if (log.CombatData.GetBuffData(AchievementEligibilityDancingWithDemons).Any())
            {
                int counter = 0;
                foreach (Player p in log.PlayerList)
                {
                    if (p.HasBuff(log, AchievementEligibilityDancingWithDemons, log.FightData.FightEnd - ServerDelayConstant))
                    {
                        counter++;
                    }
                }
                // The achievement requires 5 players alive with the buff, if the instance has only 4 players inside, you cannot get it.
                if (counter == 5)
                {
                    InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityDancingWithDemons], 1));
                }
            }
            if (log.CombatData.GetBuffData(AchievementEligibilityEnergyDispersal).Any())
            {
                InstanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityEnergyDispersal));
            }
        }
    }

    private AgentItem? GetAiAgentAt(long time)
    {
        AgentItem? elementalAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak))?.AgentItem;
        AgentItem? darkAi = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.AiKeeperOfThePeak2))?.AgentItem;
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
        var spriteFixations = GetFilteredList(log.CombatData, [ FixatedEnragedWaterSprite ], p, true, true);
        replay.Decorations.AddTether(spriteFixations, Colors.Purple, 0.5);

        // Tethering Players to Fears
        var fearFixations = GetFilteredList(log.CombatData, [ FixatedFear1, FixatedFear2, FixatedFear3, FixatedFear4 ], p, true, true);
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
                    var casts = target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).ToList();

                    foreach (CastEvent cast in casts)
                    {
                        switch (cast.SkillId)
                        {
                            // Overwhelming Sorrow - Explosion
                            case OverwhelmingSorrowWindup:
                                growing = cast.Time + sorrowFullCastDuration;
                                lifespan = (cast.Time, growing);
                                CastEvent? detonate = casts.FirstOrDefault(x => x.SkillId == OverwhelmingSorrowDetonate && x.Time > lifespan.start && x.Time < lifespan.end + ServerDelayConstant);
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
                    var fixationBuffs = GetFilteredList(log.CombatData, [ FixatedGuilt ], target, true, true);
                    replay.Decorations.AddTether(fixationBuffs, Colors.DarkPurple, 0.5);
                    break;
                }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
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
                EnvironmentDecorations.Add(new RectangleDecoration(width, length, lifespan, Colors.Orange, 0.15, position).UsingRotationConnector(rotation));
            }
        }

        // orbs
        if (log.CombatData.TryGetEffectEventsByGUIDs([ EffectGUIDs.AiAirOrbFloat, EffectGUIDs.AiFireOrbFloat, EffectGUIDs.AiWaterOrbFloat ], out var orbs))
        {
            foreach (EffectEvent effect in orbs)
            {
                long start = effect.Time;
                long end = start + 5000;
                var position = new PositionConnector(effect.Position);
                EnvironmentDecorations.Add(new CircleDecoration(180, (start, end), Colors.Red, 0.3, position).UsingFilled(false));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiDarkOrbFloat, out var darkOrbs))
        {
            foreach (EffectEvent effect in darkOrbs)
            {
                long start = effect.Time;
                long end = start + 5000;
                var position = new PositionConnector(effect.Position);
                EnvironmentDecorations.Add(new CircleDecoration(180, (start, end), Colors.Purple, 0.3, position).UsingFilled(false));
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
                EnvironmentDecorations.Add(new CircleDecoration(600, (start, end), Colors.Orange, 0.3, position).UsingFilled(false));
                EnvironmentDecorations.Add(new CircleDecoration(600, (start, end), Colors.Orange, 0.15, position).UsingGrowingEnd(end));
                if (hasDetonates)
                {
                    effect.Dst.TryGetCurrentPosition(log, end, out var endPos);
                    EffectEvent? detonate = detonates.FirstOrDefault(x => Math.Abs(x.Time - end) < ServerDelayConstant && (x.Position - endPos).XY().Length() < maxDist);
                    if (detonate != null)
                    {
                        EnvironmentDecorations.Add(new CircleDecoration(600, (detonate.Time, detonate.Time + 300), Colors.Red, 0.15, new PositionConnector(detonate.Position)));
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
                EnvironmentDecorations.Add(new PieDecoration(240, 170f, (start, end), Colors.Orange, 0.15, position).UsingRotationConnector(rotation));
                EnvironmentDecorations.Add(new PieDecoration(240, 170f, (end, end + 300), Colors.Red, 0.15, position).UsingRotationConnector(rotation));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiAoEAroundIndicator, out var around))
        {
            foreach (EffectEvent effect in around)
            {
                long start = effect.Time;
                long end = start + 2100;
                var position = new PositionConnector(effect.Position);
                EnvironmentDecorations.Add(new CircleDecoration(300, (start, end), Colors.Orange, 0.15, position));
                EnvironmentDecorations.Add(new CircleDecoration(300, (end, end + 300), Colors.Red, 0.15, position));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiRedPointblankIndicator, out var aroundRed))
        {
            foreach (EffectEvent effect in aroundRed)
            {
                long start = effect.Time;
                long end = start + 4000;
                var position = new PositionConnector(effect.Position);
                EnvironmentDecorations.Add(new CircleDecoration(300, (start, end), Colors.Red, 0.15, position));
            }
        }

        // scaled circles
        if (log.CombatData.TryGetEffectEventsByGUIDs([EffectGUIDs.AiAirCircleDetonate, EffectGUIDs.AiFireCircleDetonate], out var circleDetonate))
        {
            AddScalingCircleDecorations(log, circleDetonate, 300);
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
                AddScalingCircleDecorations(log, filteredCircles, 300);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUIDs([ EffectGUIDs.AiAirCirclePulsing, EffectGUIDs.AiFireCirclePulsing, EffectGUIDs.AiDarkCirclePulsing ], out var circlePulsing))
        {
            AddScalingCircleDecorations(log, circlePulsing, 8000);
        }

        // air intermission lightning strikes
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiAirIntermissionRedCircleIndicator, out var intermissionReds))
        {
            foreach (EffectEvent effect in intermissionReds)
            {
                long start = effect.Time;
                long end = start + 1500;
                var position = new PositionConnector(effect.Position);
                EnvironmentDecorations.Add(new CircleDecoration(300, (start, end), Colors.Orange, 0.15, position));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiAirLightningStrike, out var lightningStrikes))
        {
            foreach (EffectEvent effect in lightningStrikes)
            {
                long start = effect.Time;
                long end = start + 300;
                var position = new PositionConnector(effect.Position);
                EnvironmentDecorations.Add(new CircleDecoration(300, (start, end), Colors.Red, 0.15, position));
            }
        }

        // fire intermission meteors
        AddMeteorDecorations(log);

        // water intermission greens
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiGreenCircleIndicator, out var greens))
        {
            foreach (EffectEvent effect in greens)
            {
                long start = effect.Time;
                long end = start + 6250;
                var position = new AgentConnector(effect.Dst);
                EnvironmentDecorations.Add(new CircleDecoration(180, (start, end), Colors.DarkGreen, 0.3, position));
                EnvironmentDecorations.Add(new CircleDecoration(180, (start, end), Colors.DarkGreen, 0.3, position).UsingGrowingEnd(end));
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
                EnvironmentDecorations.Add(new CircleDecoration(180, (start, end), Colors.Orange, 0.15, position));
                EnvironmentDecorations.Add(new RectangleDecoration(80, directionLength, (start, end), Colors.Orange, 0.15, offset).UsingRotationConnector(rotation));
            }
        }
    }

    private void AddScalingCircleDecorations(ParsedEvtcLog log, IEnumerable<EffectEvent> effects, long damageDuration)
    {
        foreach (EffectEvent effect in effects)
        {
            // TODO: determine duration from previous indicator at same location
            const long indicatorDuration = 1800;

            long start = effect.Time - indicatorDuration;
            long end = effect.Time;

            AgentItem? ai = GetAiAgentAt(effect.Time);
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
                EnvironmentDecorations.Add(new CircleDecoration(radius, (start, end), Colors.Orange, 0.15, position));
                EnvironmentDecorations.Add(new CircleDecoration(radius, (end, end + damageDuration), Colors.Red, 0.15, position));
            }
        }
    }

    // TODO: find proper sizes
    private const uint MeteorInnerSize = 90;
    private const uint MeteorCircleDist = 95;
    private const uint MeteorFullRadius = MeteorInnerSize + 7 * MeteorCircleDist;

    private void AddMeteorDecorations(ParsedEvtcLog log)
    {
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiMeteorIndicatorGround, out var groundIndicators))
        {
            foreach (EffectEvent effect in groundIndicators)
            {
                (long, long) lifespan = (effect.Time, effect.Time + 6000);
                GeographicalConnector position = effect.IsAroundDst ? new AgentConnector(effect.Dst) : (GeographicalConnector)new PositionConnector(effect.Position);
                AddMeteorIndicatorDecoration(lifespan, position, Colors.Orange);
                EnvironmentDecorations.Add(new CircleDecoration(MeteorFullRadius, lifespan, Colors.Orange, 0.15, position).UsingGrowingEnd(lifespan.Item2));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AiMeteorImpact, out var impacts))
        {
            foreach (EffectEvent effect in impacts)
            {
                (long, long) lifespan = (effect.Time, effect.Time + 300);
                var position = new PositionConnector(effect.Position);
                AddMeteorIndicatorDecoration(lifespan, position, Colors.Red);
                EnvironmentDecorations.Add(new CircleDecoration(MeteorFullRadius, lifespan, Colors.Red, 0.15, position));
            }
        }
    }

    private void AddMeteorIndicatorDecoration((long, long) lifespan, GeographicalConnector connector, Color color)
    {
        EnvironmentDecorations.Add(new CircleDecoration(MeteorInnerSize, lifespan, color, 0.3, connector));
        for (uint i = 1; i <= 7; i++)
        {
            uint radius = MeteorInnerSize + i * MeteorCircleDist;
            EnvironmentDecorations.Add(new CircleDecoration(radius, lifespan, color, 0.3, connector).UsingFilled(false));
        }
    }
}
