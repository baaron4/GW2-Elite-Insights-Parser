using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Gorseval : SpiritVale
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(SpectralImpact, new MechanicPlotlySetting(Symbols.Hexagram,Colors.Red), "Slam", "Spectral Impact (KB Slam)","Slam", 4000)
                .UsingBuffChecker(Stability, false),
            new PlayerDstBuffApplyMechanic(GhastlyPrison, new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Egg", "Ghastly Prison (Egged)","Egged", 500),
            new PlayerDstBuffApplyMechanic(SpectralDarkness, new MechanicPlotlySetting(Symbols.Circle,Colors.Blue), "Orb Debuff", "Spectral Darkness (Stood in Orb AoE)","Orb Debuff", 100),
            new EnemyDstBuffApplyMechanic(SpiritedFusion, new MechanicPlotlySetting(Symbols.Square,Colors.LightOrange), "Spirit Buff", "Spirited Fusion (Consumed a Spirit)","Ate Spirit", 0),
            new PlayerDstHealthDamageHitMechanic(SpiritKick, new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Magenta), "Kick", "Kicked by small add","Spirit Kick", 0)
                .UsingBuffChecker(Stability, false),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(Vulnerability, new MechanicPlotlySetting(Symbols.Circle,Colors.Black), "Black", "Hit by Black Goo","Black Goo",3000)
                    .UsingChecker( (ba,log) => ba.AppliedDuration == 10000),
                new EnemyCastStartMechanic(GhastlyRampage, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC", "Ghastly Rampage (Breakbar)","Breakbar", 0),
                new EnemyCastEndMechanic(GhastlyRampage, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC End", "Ghastly Rampage (Full duration)","CC ran out", 0)
                    .UsingChecker( (ce,log) => ce.ActualDuration > 21985),
                new EnemyCastEndMechanic(GhastlyRampage, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed", "Ghastly Rampage (Breakbar broken)","CCed", 0)
                    .UsingChecker((ce, log) => ce.ActualDuration <= 21985),
            ]),
        ]);
    public Gorseval(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "gors";
        Icon = EncounterIconGorseval;
        EncounterCategoryInformation.InSubCategoryOrder = 2;
        EncounterID |= 0x000002;
        ChestID = ChestID.GorsevalChest;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayGorseval,
                        (957, 1000),
                        (-603, -6754, 3751, -2206)/*,
                        (-15360, -36864, 15360, 39936),
                        (3456, 11012, 4736, 14212)*/);
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(HauntingAura, HauntingAura),
        ];
    }
    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Gorseval)) ?? throw new MissingKeyActorsException("Gorseval not found");
        phases[0].AddTarget(mainTarget, log);
        phases[0].AddTargets(Targets.Where(x => x.IsSpecies(TargetID.ChargedSoul)), log, PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return phases;
        }
        phases.AddRange(GetPhasesByInvul(log, ProtectiveShadow, mainTarget, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 1)
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(mainTarget, log);
            }
            else
            {
                phase.Name = "Split " + (i) / 2;
                var ids = new List<TargetID>
                {
                   TargetID.ChargedSoul
                };
                AddTargetsToPhaseAndFit(phase, ids, log);
            }
        }
        return phases;
    }

    // note: 2nd split spawn locations are further out
    static readonly List<(string, Vector2)> SoulLocations =
    [
        ("NE", new(2523.4495f, -3665.1294f)),
        ("NW", new(842.77686f, -3657.2395f)),
        ("SW", new(866.719f, -5306.719f)),
        ("SE", new(2470.5596f, -5194.389f)),
    ];

    protected override HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [
            (int)ChargedSoul
        ];
    }

    internal static void RenameChargedSouls(IReadOnlyList<SingleActor> targets, List<CombatItem> combatData)
    {
        var nameCount = new Dictionary<string, int> { { "NE", 1 }, { "NW", 1 }, { "SW", 1 }, { "SE", 1 } };
        foreach (SingleActor target in targets)
        {
            if (target.IsSpecies(TargetID.ChargedSoul))
            {
                // 2nd split souls spawn further out, check in larger radius
                string? suffix = AddNameSuffixBasedOnInitialPosition(target, combatData, SoulLocations, 300);
                if (suffix != null && nameCount.ContainsKey(suffix))
                {
                    // deduplicate name
                    target.OverrideName(target.Character + " " + (nameCount[suffix]++));
                }
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        RenameChargedSouls(Targets, combatData);
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.Gorseval,
            TargetID.ChargedSoul
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.EnragedSpirit,
            TargetID.AngeredSpirit
        ];
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        // Ghastly Prison - Eggs AoEs
        var eggs = p.GetBuffStatus(log, GhastlyPrison).Where(x => x.Value > 0);
        foreach (var seg in eggs)
        {
            replay.Decorations.Add(new CircleDecoration(180, seg, Colors.LightOrange, 0.2, new AgentConnector(p)));
        }

        // Spectral Darkness - Orbs Debuff Overhead
        var spectralDarknesses = p.GetBuffStatus(log, SpectralDarkness).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(spectralDarknesses, p, ParserIcons.SpectralDarknessOverhead);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        long growing;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Gorseval:
                const byte first = 1 << 0;
                const byte second = 1 << 1;
                const byte third = 1 << 2;
                const byte fourth = 1 << 3;
                const byte fifth = 1 << 4;
                const byte full = 1 << 5;

                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // World Eater - Oneshot
                        case GorsevalWorldEater:
                            castDuration = 10000;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, growing);
                            replay.Decorations.AddWithFilledWithGrowing(new CircleDecoration(600, lifespan, Colors.Orange, 0.5, new AgentConnector(target)).UsingFilled(false), true, growing);
                            break;
                        // Spectral Impact - Slam on the ground
                        case SpectralImpact:
                            castDuration = 1185;
                            lifespan = (cast.Time, Math.Min(cast.Time, cast.Time + castDuration));
                            uint radius = 320;
                            replay.Decorations.Add(new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new AgentConnector(target)));
                            replay.Decorations.Add(new CircleDecoration(radius, (lifespan.end, lifespan.end + 100), Colors.Red, 0.4, new AgentConnector(target)));
                            break;
                        // Ghastly Rampage - Doughnuts
                        case GhastlyRampage:
                            {
                                lifespan = (cast.Time, cast.EndTime);
                                if (!log.CombatData.HasBreakbarDamageData)
                                {
                                    replay.Decorations.Add(new CircleDecoration(180, lifespan, Colors.LightBlue, 0.3, new AgentConnector(target)));
                                }
                                if (!log.CombatData.HasEffectData)
                                {
                                    // or spawn -> 3 secs -> explosion -> 0.5 secs -> fade -> 0.5  secs-> next
                                    int ticks = (int)Math.Min(Math.Ceiling(cast.ActualDuration / 4000.0), 6);
                                    var curHP = target.GetCurrentHealthPercent(log, cast.Time);
                                    int phaseIndex = 1;
                                    if (curHP < 66)
                                    {
                                        phaseIndex++;
                                        if (curHP < 33)
                                        {
                                            phaseIndex++;
                                        }
                                    }
                                    Vector3 pos = new(1657.0142f, -4483.577f, -1908.5195f);
                                    List<byte> patterns;
                                    switch (phaseIndex)
                                    {
                                        case 1:
                                            patterns =
                                            [
                                                    second | third | fifth,
                                            second | third | fourth,
                                            first | fourth | fifth,
                                            first | second | fifth,
                                            first | third | fifth,
                                            full
                                            ];
                                            break;
                                        case 2:
                                            patterns =
                                            [
                                                    second | third | fourth,
                                            first | fourth | fifth,
                                            first | third | fourth,
                                            first | second | fifth,
                                            first | second | third,
                                            full
                                            ];
                                            break;
                                        case 3:
                                            patterns =
                                            [
                                                    first | fourth | fifth,
                                            first | second | fifth,
                                            second | third | fifth,
                                            third | fourth | fifth,
                                            third | fourth | fifth,
                                            full
                                            ];
                                            break;
                                        default:
                                            // no reason to stop parsing because of CR, worst case, no rampage
                                            patterns = [];
                                            ticks = 0;
                                            break;
                                            //throw new EIException("Gorseval cast rampage during a split phase");
                                    }
                                    lifespan.start += 2200;
                                    for (int i = 0; i < ticks; i++)
                                    {
                                        byte pattern = patterns[i];
                                        var connector = new PositionConnector(pos);
                                        //
                                        var nonFullDecorations = new List<FormDecoration>();
                                        long tickStartNonFull = lifespan.start + 4000 * i;
                                        long explosionNonFull = tickStartNonFull + 3000;
                                        long tickEndNonFull = tickStartNonFull + 3500;
                                        (long, long) lifespanRampageNonFull = (tickStartNonFull, tickEndNonFull);
                                        if ((pattern & first) > 0)
                                        {
                                            nonFullDecorations.Add(new CircleDecoration(360, lifespanRampageNonFull, Colors.DarkPurpleBlue, 0.25, connector));
                                        }
                                        if ((pattern & second) > 0)
                                        {
                                            nonFullDecorations.Add(new DoughnutDecoration(360, 720, lifespanRampageNonFull, Colors.DarkPurpleBlue, 0.25, connector));
                                        }
                                        if ((pattern & third) > 0)
                                        {
                                            nonFullDecorations.Add(new DoughnutDecoration(720, 1080, lifespanRampageNonFull, Colors.DarkPurpleBlue, 0.25, connector));
                                        }
                                        if ((pattern & fourth) > 0)
                                        {
                                            nonFullDecorations.Add(new DoughnutDecoration(1080, 1440, lifespanRampageNonFull, Colors.DarkPurpleBlue, 0.25, connector));
                                        }
                                        if ((pattern & fifth) > 0)
                                        {
                                            nonFullDecorations.Add(new DoughnutDecoration(1440, 1800, lifespanRampageNonFull, Colors.DarkPurpleBlue, 0.25, connector));
                                        }
                                        foreach (FormDecoration decoration in nonFullDecorations)
                                        {
                                            replay.Decorations.AddWithGrowing(decoration, explosionNonFull);
                                        }
                                        // Full a different timings
                                        if ((pattern & full) > 0)
                                        {
                                            (long, long) fullLifespanRampage = (tickStartNonFull - 1000, tickEndNonFull - 1000);
                                            long fullExplosion = explosionNonFull - 1000;
                                            replay.Decorations.AddWithGrowing(new CircleDecoration(1800, fullLifespanRampage, Colors.DarkPurpleBlue, 0.25, connector), fullExplosion);
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                // Protective Shadow - Invulnerability
                var protection = target.GetBuffStatus(log, ProtectiveShadow).Where(x => x.Value > 0);
                foreach (var seg in protection)
                {
                    replay.Decorations.Add(new CircleDecoration(300, seg, Colors.LightOrange, 0.5, new AgentConnector(target)));
                }
                //  Ghastly Rampage
                var breakbarUpdates = target.GetBreakbarPercentUpdates(log);
                var (breakbarNones, breakbarActives, breakbarImmunes, breakbarRecoverings) = target.GetBreakbarStatus(log);
                foreach (var segment in breakbarActives)
                {
                    replay.Decorations.AddActiveBreakbar(segment.TimeSpan, target, breakbarUpdates);
                }
                break;
            case (int)TargetID.ChargedSoul:
                lifespan = (replay.TimeOffsets.start, replay.TimeOffsets.end);
                replay.Decorations.Add(new CircleDecoration(220, lifespan, Colors.LightOrange, 0.5, new AgentConnector(target)).UsingFilled(false));
                break;
            default:
                break;
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        if (log.CombatData.TryGetEffectEventsByGUIDs([
            EffectGUIDs.GorsevalGhastlyRampageLayer0,
            EffectGUIDs.GorsevalGhastlyRampageLayer1,
            EffectGUIDs.GorsevalGhastlyRampageLayer4,
            EffectGUIDs.GorsevalGhastlyRampageLayer3,
            EffectGUIDs.GorsevalGhastlyRampageLayer2,
            ], out var ghastlyRamplages))
        {
            Vector3 pos = new(1657.0142f, -4483.577f, -1908.5195f);
            foreach (EffectEvent ghastlyRampage in ghastlyRamplages)
            {
                (long start, long end) lifespan = ghastlyRampage.ComputeLifespan(log, 3000);
                // for the explosion
                lifespan.end += 500;
                FormDecoration rampage;
                var contentGUID = ghastlyRampage.GUIDEvent.ContentGUID;
                if (contentGUID == EffectGUIDs.GorsevalGhastlyRampageLayer0)
                {
                    rampage = new CircleDecoration(360, lifespan, Colors.DarkPurpleBlue, 0.25, new PositionConnector(pos));
                } 
                else
                {
                    uint innerRadius, outerRadius;
                    if (contentGUID == EffectGUIDs.GorsevalGhastlyRampageLayer1)
                    {
                        innerRadius = 360;
                        outerRadius = 720;
                    } 
                    else if (contentGUID == EffectGUIDs.GorsevalGhastlyRampageLayer2)
                    {
                        innerRadius = 720;
                        outerRadius = 1080;
                    } 
                    else if (contentGUID == EffectGUIDs.GorsevalGhastlyRampageLayer3)
                    {
                        innerRadius = 1080;
                        outerRadius = 1440;
                    }
                    else
                    {
                        innerRadius = 1440;
                        outerRadius = 1800;
                    }
                    rampage = new DoughnutDecoration(innerRadius, outerRadius, lifespan, Colors.DarkPurpleBlue, 0.25, new PositionConnector(pos));
                }
                environmentDecorations.AddWithGrowing(rampage, lifespan.end - 500);
            }
        }

        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GorsevalGhastlyPrison, out var ghstlyPrison))
        {
            foreach (EffectEvent effect in ghstlyPrison)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 2000);
                var circle = new CircleDecoration(80, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }
    }
}
