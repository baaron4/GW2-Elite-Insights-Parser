using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class HarvestTemple : EndOfDragonsStrike
{

    private IEnumerable<SingleActor> FirstAwareSortedTargets = [];
    public HarvestTemple(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            // General
            new MechanicGroup([
                new PlayerDstEffectMechanic([EffectGUIDs.HarvestTempleTargetedExpulsionSpreadNM, EffectGUIDs.HarvestTempleTargetedExpulsionSpreadCM], new MechanicPlotlySetting(Symbols.Circle, Colors.Yellow), "Spread.B", "Baited spread mechanic", "Spread Bait", 150),
                new PlayerDstEffectMechanic([EffectGUIDs.HarvestTempleVoidPoolRedPuddleSelectionNM, EffectGUIDs.HarvestTempleVoidPoolRedPuddleSelectionCM], new MechanicPlotlySetting(Symbols.Circle, Colors.Red), "Red.B", "Baited red puddle mechanic", "Red Bait", 150),
                new PlayerDstBuffApplyMechanic(InfluenceOfTheVoidBuff, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkPurple), "Void.D", "Received Void debuff", "Void Debuff", 150),
                new PlayerDstHealthDamageHitMechanic(InfluenceOfTheVoidSkill, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.DarkPurple), "Void.H", "Hit by Void", "Void Hit", 150),
                new PlayerDstHealthDamageHitMechanic([VoidPoolNM, VoidPoolCM], new MechanicPlotlySetting(Symbols.Circle, Colors.DarkPurple), "Red.H", "Hit by Red Void Pool", "Void Pool", 150),
                new PlayerDstHealthDamageMechanic([HarvestTempleTargetedExpulsionNM, HarvestTempleTargetedExpulsionCM], new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Orange), "Spread.H", "Hit by Targeted Expulsion (Spread)", "Targeted Expulsion (Spread)", 150)
                    .UsingChecker((@event, log) => @event.HasHit || @event.IsNotADamageEvent),
                new PlayerSrcAllHealthDamageHitsMechanic(new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightOrange), "Orb Push", "Orb was pushed by player", "Orb Push", 0)
                    .UsingChecker((de, log) => (de.To.IsSpecies(TargetID.PushableVoidAmalgamate) || de.To.IsSpecies(TargetID.KillableVoidAmalgamate)) && de is DirectHealthDamageEvent),
                new PlayerDstHealthDamageHitMechanic([Shockwave, TsunamiSlamOrb, TsunamiSlamTailSlamOrb], new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Yellow), "NopeRopes.Achiv", "Achievement Eligibility: Jumping the Nope Ropes", "Achiv Jumping Nope Ropes", 150)
                    .UsingAchievementEligibility(),
                new PlayerDstHealthDamageHitMechanic([VoidExplosion, VoidExplosion2, VoidExplosion3], new MechanicPlotlySetting(Symbols.StarSquareOpenDot, Colors.Yellow), "VoidExp.H", "Hit by Void Explosion (Last Laugh)", "Void Explosion", 0),
                new PlayerDstHealthDamageHitMechanic(MagicDischarge, new MechanicPlotlySetting(Symbols.Octagon, Colors.Grey), "MagicDisc.H", "Hit by Magic Discharge (Orb Explosion Wave)", "Magic Discharge", 0),
                new MechanicGroup([
                    new EnemySrcEffectMechanic(EffectGUIDs.HarvestTempleSuccessGreen, new MechanicPlotlySetting(Symbols.Circle, Colors.DarkGreen), "S.Green", "Green Successful", "Success Green", 0),
                    new EnemySrcEffectMechanic(EffectGUIDs.HarvestTempleFailedGreen, new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "F.Green", "Green Failed", "Failed Green", 0),
                ]),
            ]),
            // Purification 1
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(LightningOfJormag, new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.Ice), "Light.H", "Hit by Lightning of Jormag", "Lightning of Jormag", 0),
                new PlayerDstHealthDamageHitMechanic(FlamesOfPrimordus, new MechanicPlotlySetting(Symbols.StarTriangleDownOpen, Colors.Orange), "Flame.H", "Hit by Flames of Primordus", "Flames of Primordus", 0),
                new PlayerDstHealthDamageHitMechanic(Stormfall, new MechanicPlotlySetting(Symbols.YUpOpen, Colors.Purple), "Storm.H", "Hit by Kralkatorrik's Stormfall", "Kralkatorrik's Stormfall", 0),
            ]),
            // Jormag
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([BreathOfJormag1, BreathOfJormag2, BreathOfJormag3], new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Blue), "J.Breath.H", "Hit by Jormag Breath", "Jormag Breath", 150),
                new PlayerDstHealthDamageHitMechanic(GraspOfJormag, new MechanicPlotlySetting(Symbols.StarOpen, Colors.DarkWhite), "J.Grasp.H", "Hit by Grasp of Jormag", "Grasp of Jormag", 0),
                new PlayerDstHealthDamageHitMechanic(FrostMeteor, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Blue), "J.Meteor.H", "Hit by Jormag Meteor", "Jormag Meteor", 150),
            ]),
            // Primordus
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(LavaSlam, new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Red), "Slam.H", "Hit by Primordus Slam", "Primordus Slam", 150),
                new PlayerDstHealthDamageHitMechanic(JawsOfDestruction, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Jaws.H", "Hit by Primordus Jaws", "Primordus Jaws", 150),
            ]),
            // Kralkatorrik 
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(CrystalBarrage, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Purple), "Barrage.H", "Hit by Crystal Barrage", "Barrage", 150),
                new PlayerDstHealthDamageHitMechanic(BrandingBeam, new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Purple), "Beam.H", "Hit by Kralkatorrik's Branding Beam", "Kralkatorrik Beam", 150),
                new PlayerDstHealthDamageHitMechanic(BrandedArtillery, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Purple), "Artillery.H", "Hit by Brandbomber Artillery", "Brandbomber Artillery", 150),
                new PlayerDstHealthDamageHitMechanic(VoidPoolKralkatorrik, new MechanicPlotlySetting(Symbols.Circle, Colors.Black), "K.Pool.H", "Hit by Kralkatorrik Void Pool", "Kralkatorrik Void Pool", 150),
            ]),
            // Purification 2
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(SwarmOfMordremoth_PoolOfUndeath, new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Goop.H", "Hit by goop left by heart", "Heart Goop", 150),
                new PlayerDstHealthDamageHitMechanic(SwarmOfMordremoth, new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Red), "Bees.H", "Hit by bees from heart", "Heart Bees", 150),
                // Timecaster
                new MechanicGroup([
                    new PlayerDstHealthDamageHitMechanic(GravityCrushDamage, new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Black), "Grav.Cru.H", "Hit by Gravity Crush", "Gravity Crush", 0),
                    new PlayerDstHealthDamageHitMechanic(NightmareEpochDamage, new MechanicPlotlySetting(Symbols.Hexagon, Colors.Pink), "NigEpoch.H", "Hit by Nightmare Epoch", "Nightmare Epoch", 0),
                ]),
            ]),
            // Mordremoth
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(Shockwave, new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Green), "ShckWv.H", "Hit by Mordremoth Shockwave", "Mordremoth Shockwave", 150),
                new PlayerDstHealthDamageHitMechanic(Kick, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "Kick.H", "Kicked by Void Skullpiercer", "Skullpiercer Kick", 150).UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
                new PlayerDstHealthDamageHitMechanic(PoisonRoar, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Green), "M.Poison.H", "Hit by Mordremoth Poison", "Mordremoth Poison", 150),
            ]),
            // Giants
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(DeathScream, new MechanicPlotlySetting(Symbols.SquareOpen, Colors.Grey), "Scream.G.CC", "CC'd by Giant's Death Scream", "Death Scream", 0)
                    .UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
                new PlayerDstHealthDamageHitMechanic(RottingBile, new MechanicPlotlySetting(Symbols.Square, Colors.GreenishYellow), "RotBile.H", "Hit by Giant's Rotting Bile", "Rotting Bile", 0),
                new PlayerDstHealthDamageHitMechanic(Stomp, new MechanicPlotlySetting(Symbols.StarSquare, Colors.Teal), "Stomp.CC", "CC'd by Giant's Stomp", "Stomp", 0)
                    .UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
            ]),
            // Zhaitan
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([ScreamOfZhaitanNM, ScreamOfZhaitanCM], new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkGreen), "Scream.H", "Hit by Zhaitan Scream", "Zhaitan Scream", 150),
                new PlayerDstHealthDamageHitMechanic(PutridDeluge, new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkGreen), "Z.Poison.H", "Hit by Zhaitan Poison", "Zhaitan Poison", 150),
                new PlayerDstHealthDamageHitMechanic(ZhaitanTailSlam, new MechanicPlotlySetting(Symbols.Circle, Colors.Grey), "Slam.H", "Hit by Zhaitan's Tail Slam", "Zhaitan Slam", 0),
            ]),
            // Purification 3
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(SwarmOfMordremoth_CorruptedWaters, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.LightBlue), "Prjtile.H", "Hit by Corrupted Waters (Heart Projectile)", "Heart Projectile", 150),
                // Saltspray
                new MechanicGroup([
                    new PlayerDstHealthDamageHitMechanic(HydroBurst, new MechanicPlotlySetting(Symbols.Circle, Colors.LightBlue), "Whrlpl.H", "Hit by Hydro Burst (Whirlpool)", "Hydro Burst (Whirlpool)", 150),
                    new PlayerDstHealthDamageHitMechanic(CallLightning, new MechanicPlotlySetting(Symbols.TriangleDownOpen, Colors.Purple), "CallLigh.H", "Hit by Call Lightning", "Call Lightning", 0),
                    new PlayerDstHealthDamageHitMechanic(FrozenFury, new MechanicPlotlySetting(Symbols.TriangleRightOpen, Colors.Ice), "FrozFury.H", "Hit by Frozen Fury", "Frozen Fury", 0),
                    new PlayerDstHealthDamageHitMechanic(RollingFlame, new MechanicPlotlySetting(Symbols.Circle, Colors.LightRed), "RollFlame.H", "Hit by Rolling Flame", "Rolling Flame", 0),
                    new PlayerDstHealthDamageHitMechanic([ShatterEarth, ShatterEarth2], new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Brown), "ShatEarth.H", "Hit by Shatter Earth", "Shatter Earth", 0),
                ]),
            ]),
            // Soo Won
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([TsunamiSlamOrb, TsunamiSlamTailSlamOrb], new MechanicPlotlySetting(Symbols.TriangleRight, Colors.LightBlue), "Tsunami.H", "Hit by Soo-Won Tsunami", "Soo-Won Tsunami", 150),
                new PlayerDstHealthDamageHitMechanic(ClawSlap, new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.LightBlue), "Claw.H", "Hit by Soo-Won Claw", "Soo-Won Claw", 150),
                new PlayerDstHealthDamageHitMechanic(VoidPoolSooWon, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkPink), "SW.Pool.H", "Hit by Soo-Won Void Pool", "Soo-Won Void Pool", 150),
                new PlayerDstHealthDamageHitMechanic(TailSlam, new MechanicPlotlySetting(Symbols.Square, Colors.LightBlue), "Tail.H", "Hit by Soo-Won Tail", "Soo-Won Tail", 150),
                new PlayerDstHealthDamageHitMechanic(TormentOfTheVoid, new MechanicPlotlySetting(Symbols.Circle, Colors.DarkMagenta), "Torment.H", "Hit by Torment of the Void (Bouncing Orbs)", "Torment of the Void", 150),
                new PlayerDstHealthDamageHitMechanic(MagicHail, new MechanicPlotlySetting(Symbols.CircleX, Colors.Black), "MagHail.H", "Hit by Magic Hail", "Magic Hail Hit", 0),
                // Obliterator
                new MechanicGroup([
                    new PlayerDstHealthDamageHitMechanic(VoidObliteratorFirebomb, new MechanicPlotlySetting(Symbols.TriangleNW, Colors.DarkTeal), "Firebomb.H", "Hit by Firebomb", "Firebomb", 0),
                    new PlayerDstHealthDamageHitMechanic(VoidObliteratorWyvernBreathDamage, new MechanicPlotlySetting(Symbols.TriangleNEOpen, Colors.Magenta), "WyvBreath.H", "Hit by Wyvern Breath", "Wyvern Breath", 0),
                    new MechanicGroup([
                        new PlayerDstHealthDamageHitMechanic(VoidObliteratorCharge, new MechanicPlotlySetting(Symbols.Diamond, Colors.Teal), "Charge.H", "Hit by Obliterator's Charge", "Charge Hit", 0),
                        new PlayerDstHealthDamageHitMechanic(VoidObliteratorCharge, new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.Revenant), "Charge.CC", "CC'd by Obliterator's Charge", "Charge CC", 0)
                            .UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
                    ]),
                ]),
                // Goliath
                new MechanicGroup([
                    new PlayerDstHealthDamageHitMechanic(GlacialSlam, new MechanicPlotlySetting(Symbols.CircleX, Colors.Ice), "GlaSlam.H", "Hit by Glacial Slam", "Glacial Slam Hit", 0),
                    new PlayerDstHealthDamageHitMechanic(GlacialSlam, new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Ice), "GlaSlam.CC", "CC'd by Glacial Slam", "Glacial Slam CC", 0)
                        .UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
                ]),
            ]),
            // Purification 4
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(GraspOfTheVoid, new MechanicPlotlySetting(Symbols.Hexagram, Colors.Black), "GraspVoid.H", "Hit by Grasp of the Void (Final Orb Projectile)", "Grasp of the Void", 0),
            ]),
        ]));
        Icon = EncounterIconHarvestTemple;
        ChestID = ChestID.GrandStrikeChest;
        GenericFallBackMethod = FallBackMethod.None;
        Extension = "harvsttmpl";
        EncounterCategoryInformation.InSubCategoryOrder = 3;
        EncounterID |= 0x000004;
    }
    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayHarvestTemple,
                        (1024, 1024),
                        (-844, -21845, 2055, -18946)/*,
                        (-15360, -36864, 15360, 39936),
                        (3456, 11012, 4736, 14212)*/);
    }
    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var subPhasesData = new List<(long start, long end, string name, SingleActor target, string? subPhaseOf)>();
        var giants = new List<SingleActor>();
        foreach (SingleActor target in Targets)
        {
            long phaseEnd = Math.Min(target.LastAware, log.FightData.FightEnd);
            long phaseStart = Math.Max(target.FirstAware, log.FightData.FightStart);
            switch (target.ID)
            {
                case (int)TargetID.KillableVoidAmalgamate:
                    phases[0].AddTarget(target, log, PhaseData.TargetPriority.Blocking);
                    break;
                case (int)TargetID.VoidGiant:
                    giants.Add(target);
                    break;
                case (int)TargetID.VoidSaltsprayDragon:
                    subPhasesData.Add((phaseStart, phaseEnd, "Void Saltspray Dragon", target, null));
                    break;
                case (int)TargetID.VoidTimeCaster:
                    subPhasesData.Add((phaseStart, phaseEnd, "Void Time Caster", target, null));
                    break;
                case (int)TargetID.VoidObliterator:
                    subPhasesData.Add((phaseStart, phaseEnd, "Void Obliterator", target, null));
                    break;
                case (int)TargetID.VoidGoliath:
                    subPhasesData.Add((phaseStart, phaseEnd, "Void Goliath", target, null));
                    break;
                case (int)TargetID.TheDragonVoidJormag:
                    phases[0].AddTarget(target, log);
                    subPhasesData.Add((phaseStart, phaseEnd, "Jormag", target, "Full Fight"));
                    break;
                case (int)TargetID.TheDragonVoidKralkatorrik:
                    phases[0].AddTarget(target, log);
                    subPhasesData.Add((phaseStart, phaseEnd, "Kralkatorrik", target, "Full Fight"));
                    break;
                case (int)TargetID.TheDragonVoidMordremoth:
                    phases[0].AddTarget(target, log);
                    subPhasesData.Add((phaseStart, phaseEnd, "Mordremoth", target, "Full Fight"));
                    break;
                case (int)TargetID.TheDragonVoidPrimordus:
                    phases[0].AddTarget(target, log);
                    subPhasesData.Add((phaseStart, phaseEnd, "Primordus", target, "Full Fight"));
                    break;
                case (int)TargetID.TheDragonVoidSooWon:
                    phases[0].AddTarget(target, log);
                    subPhasesData.Add((phaseStart, phaseEnd, "Soo-Won", target, "Full Fight"));
                    AttackTargetEvent? attackTargetEvent = log.CombatData.GetAttackTargetEventsBySrc(target.AgentItem).Where(x => x.GetTargetableEvents(log).Any(y => y.Targetable && y.Time >= target.FirstAware)).FirstOrDefault();
                    if (attackTargetEvent != null)
                    {
                        var targetables = attackTargetEvent.GetTargetableEvents(log).Where(x => x.Time >= target.FirstAware);
                        var targetOns = targetables.Where(x => x.Targetable);
                        var targetOffs = targetables.Where(x => !x.Targetable);
                        int id = 0;
                        foreach (TargetableEvent targetOn in targetOns)
                        {
                            long start = targetOn.Time;
                            long end = log.FightData.FightEnd;
                            TargetableEvent? targetOff = targetOffs.FirstOrDefault(x => x.Time > start);
                            if (targetOff != null)
                            {
                                end = targetOff.Time;
                            }
                            subPhasesData.Add((start, end, "Soo-Won " + (++id), target, "Soo-Won"));
                        }
                    }
                    break;
                case (int)TargetID.TheDragonVoidZhaitan:
                    phases[0].AddTarget(target, log);
                    subPhasesData.Add((phaseStart, phaseEnd, "Zhaitan", target, "Full Fight"));
                    break;
            }
        }
        if (!requirePhases)
        {
            return phases;
        }

        if (giants.Count > 0)
        {
            long start = log.FightData.FightEnd;
            long end = log.FightData.FightStart;
            foreach (SingleActor giant in giants)
            {
                start = Math.Min(start, giant.FirstAware);
                end = Math.Max(end, giant.LastAware);
            }
            var subPhase = new PhaseData(start, end, "Giants");
            subPhase.AddTargets(giants, log);
            subPhase.OverrideEndTime(log);
            phases.Add(subPhase);
        }
        var subPhaseNonBlockings = Targets.Where(x => x.IsSpecies(TargetID.VoidGoliath) || x.IsSpecies(TargetID.VoidObliterator) || x.IsSpecies(TargetID.VoidGiant));
        foreach ((long start, long end, string name, SingleActor target, string? subPhaseOf) in subPhasesData)
        {
            var subPhase = new PhaseData(start, end, name);
            subPhase.AddTarget(target, log);
            subPhase.OverrideEndTime(log);
            subPhase.AddTargets(subPhaseNonBlockings, log, PhaseData.TargetPriority.NonBlocking);
            if (subPhaseOf != null)
            {
                subPhase.AddParentPhase(phases.FirstOrDefault(x => x.Name == subPhaseOf));
            }
            phases.Add(subPhase);
        }
        int purificationID = 0;
        var purificationNonBlockings = Targets.Where(x => x.IsSpecies(TargetID.VoidTimeCaster) || x.IsSpecies(TargetID.VoidSaltsprayDragon));
        foreach (SingleActor voidAmal in Targets.Where(x => x.IsSpecies(TargetID.PushableVoidAmalgamate) || x.IsSpecies(TargetID.KillableVoidAmalgamate)))
        {
            var purificationPhase = new PhaseData(Math.Max(voidAmal.FirstAware, log.FightData.FightStart), voidAmal.LastAware, "Purification " + (++purificationID));
            purificationPhase.AddTarget(voidAmal, log);
            purificationPhase.OverrideEndTime(log);
            purificationPhase.AddTargets(purificationNonBlockings, log, PhaseData.TargetPriority.NonBlocking);
            phases.Add(purificationPhase);
            if (voidAmal.IsSpecies(TargetID.PushableVoidAmalgamate))
            {
                purificationPhase.AddParentPhase(phases.FirstOrDefault(x => x.Name == "Full Fight"));
                if (purificationPhase.Targets.Keys.Any(x => x.IsSpecies(TargetID.VoidTimeCaster)))
                {
                    var voidTimeCasterPhase = phases.FirstOrDefault(x => x.Name == "Void Time Caster");
                    if (voidTimeCasterPhase != null)
                    {
                        voidTimeCasterPhase.OverrideStart(Math.Max(voidTimeCasterPhase.Start, purificationPhase.Start));
                        voidTimeCasterPhase.OverrideEnd(Math.Min(voidTimeCasterPhase.End, purificationPhase.End));
                        voidTimeCasterPhase.AddParentPhase(purificationPhase);
                    }
                }
                else if (purificationPhase.Targets.Keys.Any(x => x.IsSpecies(TargetID.VoidSaltsprayDragon)))
                {
                    var voidSaltsprayDragonPhase = phases.FirstOrDefault(x => x.Name == "Void Saltspray Dragon");
                    if (voidSaltsprayDragonPhase != null)
                    {
                        voidSaltsprayDragonPhase.OverrideStart(Math.Max(voidSaltsprayDragonPhase.Start, purificationPhase.Start));
                        voidSaltsprayDragonPhase.OverrideEnd(Math.Min(voidSaltsprayDragonPhase.End, purificationPhase.End));
                        voidSaltsprayDragonPhase.AddParentPhase(purificationPhase);
                    }
                }
            }
            else
            {
                var sooWonPhase = phases.FirstOrDefault(x => x.Name == "Soo-Won");
                purificationPhase.AddParentPhase(sooWonPhase);
                sooWonPhase?.AddTarget(voidAmal, log, PhaseData.TargetPriority.Blocking);
            }
        }
        return phases;
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        // To investigate
        return FightData.EncounterStartStatus.Normal;
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericFightOffset(fightData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            AgentItem firstAmalgamate = agentData.GetNPCsByID(TargetID.VoidAmalgamate).MinBy(x => x.FirstAware);
            if (firstAmalgamate != null)
            {
                startToUse = firstAmalgamate.FirstAware;
            }
        }
        return startToUse;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.TheDragonVoidJormag,
            TargetID.TheDragonVoidKralkatorrik,
            TargetID.TheDragonVoidMordremoth,
            TargetID.TheDragonVoidPrimordus,
            TargetID.TheDragonVoidSooWon,
            TargetID.TheDragonVoidZhaitan,
            TargetID.VoidSaltsprayDragon,
            TargetID.VoidObliterator,
            TargetID.VoidGoliath,
            TargetID.VoidTimeCaster,
            TargetID.PushableVoidAmalgamate,
            TargetID.KillableVoidAmalgamate,
            TargetID.VoidGiant
        ];
    }

    protected override IReadOnlyList<TargetID> GetSuccessCheckIDs()
    {
        return [];
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.TheDragonVoidJormag, 0 },
            {TargetID.TheDragonVoidPrimordus, 1 },
            {TargetID.TheDragonVoidKralkatorrik, 2},
            {TargetID.TheDragonVoidMordremoth, 3 },
            {TargetID.TheDragonVoidZhaitan, 4 },
            {TargetID.TheDragonVoidSooWon, 5 },
            {TargetID.PushableVoidAmalgamate, 6 },
            {TargetID.KillableVoidAmalgamate, 6 },
            {TargetID.VoidSaltsprayDragon, 6 },
            {TargetID.VoidObliterator, 6 },
            {TargetID.VoidGoliath, 6 },
            {TargetID.VoidTimeCaster, 6 },
            {TargetID.VoidGiant, 6},
        };
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.ZhaitansReach,
            TargetID.VoidAbomination,
            TargetID.VoidAmalgamate,
            TargetID.VoidBrandbomber,
            TargetID.VoidBurster,
            TargetID.VoidColdsteel,
            TargetID.VoidMelter,
            TargetID.VoidRotswarmer,
            TargetID.VoidSkullpiercer,
            TargetID.VoidStormseer,
            TargetID.VoidTangler,
            TargetID.VoidWarforged1,
            TargetID.VoidWarforged2,
            TargetID.DragonBodyVoidAmalgamate,
            TargetID.DragonEnergyOrb,
            TargetID.GravityBall,
            TargetID.JormagMovingFrostBeam,
            TargetID.JormagMovingFrostBeamNorth,
            TargetID.JormagMovingFrostBeamCenter,
        ];
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Harvest Temple";
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        NoBouncyChestGenericCheckSucess(combatData, agentData, fightData, playerAgents);
        if (!fightData.Success)
        {
            // no bouny chest detection, the reward is delayed
            SingleActor? soowon = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.TheDragonVoidSooWon));
            if (soowon != null)
            {
                var targetOffs = combatData.GetAttackTargetEventsBySrc(soowon.AgentItem).Select(x => x.GetTargetableEvents(combatData).Where(x => x.Time >= soowon.FirstAware && !x.Targetable)).FirstOrDefault(x => x.Any());
                if (targetOffs == null)
                {
                    return;
                }
                if (targetOffs.Count() == 2)
                {
                    HealthDamageEvent? lastDamageTaken = combatData.GetDamageTakenData(soowon.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                    if (lastDamageTaken != null)
                    {
                        bool isSuccess = false;
                        if (agentData.GetGadgetsByID(ChestID).Any())
                        {
                            isSuccess = true;
                        }
                        else
                        {
                            var determinedApplies = combatData.GetBuffData(Determined895).OfType<BuffApplyEvent>().Where(x => x.To.IsPlayer && Math.Abs(x.AppliedDuration - 10000) < ServerDelayConstant);
                            IReadOnlyList<AnimatedCastEvent> liftOffs = combatData.GetAnimatedCastData(HarvestTempleLiftOff);
                            foreach (AnimatedCastEvent liffOff in liftOffs)
                            {
                                isSuccess = true;
                                if (determinedApplies.Count(x => x.To == liffOff.Caster && liffOff.Time - x.Time + ServerDelayConstant > 0) != 1)
                                {
                                    isSuccess = false;
                                    break;
                                }
                            }
                        }
                        if (isSuccess)
                        {
                            fightData.SetSuccess(true, targetOffs.Last().Time);
                        }
                    }
                }
            }
        }
    }

    protected override HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [
            (int)TargetID.KillableVoidAmalgamate,
            (int)TargetID.PushableVoidAmalgamate,
        ];
    }

    internal override void HandleCriticalGadgets(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var idsToUse = new List<TargetID> {
            TargetID.TheDragonVoidJormag,
            TargetID.TheDragonVoidPrimordus,
            TargetID.TheDragonVoidKralkatorrik,
            TargetID.TheDragonVoidMordremoth,
            TargetID.TheDragonVoidZhaitan,
            TargetID.TheDragonVoidSooWon,
        };
        var attackTargetEvents = combatData.Where(x => x.IsStateChange == StateChange.AttackTarget).Select(x => new AttackTargetEvent(x, agentData));
        var targetableEvents = new Dictionary<AgentItem, IEnumerable<TargetableEvent>>();
        foreach (var attackTarget in attackTargetEvents)
        {
            targetableEvents[attackTarget.AttackTarget] = attackTarget.GetTargetableEvents(combatData, agentData);
        }
        attackTargetEvents = attackTargetEvents.Where(x =>
        {
            AgentItem atAgent = x.AttackTarget;
            if (targetableEvents.TryGetValue(atAgent, out var targetables))
            {
                return targetables.Any(y => y.Targetable);
            }
            return false;
        }).ToList();
        var attackTargetSortID = new Dictionary<AgentItem, long>();
        foreach (var attackTargetEvent in attackTargetEvents)
        {
            AgentItem atAgent = attackTargetEvent.AttackTarget;
            if (!targetableEvents.TryGetValue(atAgent, out var targetables))
            {
                attackTargetSortID[atAgent] = long.MaxValue;
                continue;
            }
            var targetOns = targetables.Where(x => x.Targetable);
            if (!targetOns.Any())
            {
                attackTargetSortID[atAgent] = long.MaxValue;
                continue;
            }
            attackTargetSortID[atAgent] = targetOns.Min(x => x.Time);
        }
        attackTargetEvents = attackTargetEvents.OrderBy(x => attackTargetSortID[x.AttackTarget]);
        int index = 0;
        var processedAttackTargets = new HashSet<AgentItem>();
        foreach (AttackTargetEvent attackTargetEvent in attackTargetEvents)
        {
            AgentItem atAgent = attackTargetEvent.AttackTarget;
            // We take attack events, filter out the first one, present at spawn, that is always a non targetable event
            // There are only two relevant attack targets, one represents the first five and the last one Soo Won
            if (processedAttackTargets.Contains(atAgent) || !targetableEvents.TryGetValue(atAgent, out var targetables))
            {
                continue;
            }
            AgentItem dragonVoid = attackTargetEvent.Src;
            var copyEventsFrom = new List<AgentItem>() { dragonVoid };
            processedAttackTargets.Add(atAgent);
            var targetOns = targetables.Where(x => x.Targetable);
            var targetOffs = targetables.Where(x => !x.Targetable);
            //
            foreach (TargetableEvent targetOn in targetOns)
            {
                // If Soo Won has been already created, we break
                if (index >= idsToUse.Count)
                {
                    break;
                }
                TargetID id = idsToUse[index++];
                long start = targetOn.Time;
                long end = dragonVoid.LastAware;
                TargetableEvent? targetOff = targetOffs.FirstOrDefault(x => x.Time > start);
                // Don't split Soo won into two
                if (targetOff != null && id != TargetID.TheDragonVoidSooWon)
                {
                    end = targetOff.Time;
                }
                double lastHPUpdate = 1e6;
                AgentItem extra = agentData.AddCustomNPCAgent(start, end, dragonVoid.Name, dragonVoid.Spec, id, false, dragonVoid.Toughness, dragonVoid.Healing, dragonVoid.Condition, dragonVoid.Concentration, atAgent.HitboxWidth > 0 ? atAgent.HitboxWidth : 800, atAgent.HitboxHeight);
                AgentManipulationHelper.RedirectNPCEventsAndCopyPreviousStates(combatData, extensions, agentData, dragonVoid, copyEventsFrom, extra, true,
                    (evt, from, to) =>
                    {
                        if (evt.IsStateChange == StateChange.HealthUpdate)
                        {
                            double healthPercent = HealthUpdateEvent.GetHealthPercent(evt);
                            // Avoid making the gadget go back to 100% hp on "death"
                            // Regenerating back to full HP
                            // use mid life check to allow hp going back up to 100% around first aware
                            if (healthPercent > lastHPUpdate && healthPercent > 99 && evt.Time > (to.LastAware + to.FirstAware) / 2)
                            {
                                return false;
                            }
                            // Remember last hp
                            lastHPUpdate = HealthUpdateEvent.GetHealthPercent(evt);
                        }
                        return true;
                    }
                );
                copyEventsFrom.Add(extra);
            }
        }
        // Add missing agents
        for (int i = index; i < idsToUse.Count; i++)
        {
            agentData.AddCustomNPCAgent(fightData.FightStart + index, fightData.FightStart + index + 1, "Dragonvoid", Spec.NPC, idsToUse[i], false);
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadget(ChestID, agentData, combatData, GrandStrikeChestHarvestTemplePosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 500 && agentItem.HitboxWidth == 2));
        var maxHPEvents = combatData
            .Where(x => x.IsStateChange == StateChange.MaxHealthUpdate)
            .Select(x => new MaxHealthUpdateEvent(x, agentData))
            .GroupBy(x => x.MaxHealth).ToDictionary(x => x.Key);
        //
        if (maxHPEvents.TryGetValue(491550, out var dragonOrbMaxHPs))
        {
            foreach (MaxHealthUpdateEvent dragonOrbMaxHP in dragonOrbMaxHPs)
            {
                AgentItem dragonOrb = dragonOrbMaxHP.Src;
                if (!dragonOrb.IsUnknown && combatData.Count(x => x.IsStateChange == StateChange.Velocity && x.SrcMatchesAgent(dragonOrb)) > 5)
                {
                    dragonOrb.OverrideName("Dragon Orb");
                    dragonOrb.OverrideID(TargetID.DragonEnergyOrb, agentData);
                }
            }
        }
        //
        IReadOnlyList<AgentItem> voidAmalgamates = agentData.GetNPCsByID(TargetID.VoidAmalgamate);
        foreach (AgentItem voidAmal in voidAmalgamates)
        {
            if (combatData.Where(x => x.SkillID == VoidShell && x.IsBuffApply() && x.SrcMatchesAgent(voidAmal)).Any())
            {
                voidAmal.OverrideID(TargetID.PushableVoidAmalgamate, agentData);
            }
        }
        AgentItem dragonBodyVoidAmalgamate = voidAmalgamates.MaxBy(x => x.LastAware - x.FirstAware);
        if (dragonBodyVoidAmalgamate != null)
        {
            dragonBodyVoidAmalgamate.OverrideID(TargetID.DragonBodyVoidAmalgamate, agentData);
        }
        // Gravity Ball - Timecaster gadget
        if (agentData.TryGetFirstAgentItem(TargetID.VoidTimeCaster, out var timecaster))
        {
            if (maxHPEvents.TryGetValue(14940, out var potentialGravityBallHPs))
            {
                var gravityBalls = potentialGravityBallHPs.Where(x => x.Src.Type == AgentItem.AgentType.Gadget && x.Src.HitboxHeight == 300 && x.Src.HitboxWidth == 100 && x.Src.Master == null && x.Src.FirstAware > timecaster.FirstAware && x.Src.FirstAware < timecaster.LastAware + 2000).Select(x => x.Src);
                var candidateVelocities = combatData.Where(x => x.IsStateChange == StateChange.Velocity && gravityBalls.Any(y => x.SrcMatchesAgent(y)));
                const int referenceLength = 200;
                var gravityBalls_ = gravityBalls.Where(x => candidateVelocities.Any(y => Math.Abs(MovementEvent.GetPointXY(y).Length() - referenceLength) < 10));
                foreach (AgentItem ball in gravityBalls_)
                {
                    ball.OverrideType(AgentItem.AgentType.NPC, agentData);
                    ball.OverrideID(TargetID.GravityBall, agentData);
                    ball.SetMaster(timecaster);
                }
            }
        }
        {
            if (agentData.TryGetFirstAgentItem(TargetID.TheDragonVoidJormag, out var jormagAgent))
            {
                var frostBeams = combatData.Where(evt => evt.SrcIsAgent() && agentData.GetAgent(evt.SrcAgent, evt.Time).IsNonIdentifiedSpecies())
                    .Select(evt => agentData.GetAgent(evt.SrcAgent, evt.Time))
                    .Distinct()
                    .Where(agent => agent.IsNPC && agent.FirstAware >= jormagAgent.FirstAware && agent.LastAware <= jormagAgent.LastAware && combatData.Count(evt => evt.SrcMatchesAgent(agent) && evt.IsStateChange == StateChange.Velocity && MovementEvent.GetPointXY(evt) != default) > 2);
                foreach (AgentItem frostBeam in frostBeams)
                {
                    frostBeam.OverrideID(TargetID.JormagMovingFrostBeam, agentData);
                    frostBeam.OverrideType(AgentItem.AgentType.NPC, agentData);
                    frostBeam.SetMaster(jormagAgent);
                }
                var knownFrostBeams = agentData.GetNPCsByID(TargetID.JormagMovingFrostBeamNorth).ToList();
                knownFrostBeams.AddRange(agentData.GetNPCsByID(TargetID.JormagMovingFrostBeamCenter));
                knownFrostBeams.ForEach(x => x.SetMaster(jormagAgent));
            }
        }
        //
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        //
        int purificationID = 0;
        bool needRedirect = false;
        (HashSet<ulong> jormagDamagingAgents, SingleActor? jormag) = ([], null);
        (HashSet<ulong> primordusDamagingAgents, SingleActor? primordus) = ([], null);
        (HashSet<ulong> kralkDamagingAgents, SingleActor? kralk) = ([], null);
        (HashSet<ulong> mordDamagingAgents, SingleActor? mord) = ([], null);
        (HashSet<ulong> zhaitanDamagingAgents, SingleActor? zhaitan) = ([], null);
        (HashSet<ulong> soowonDamagingAgents, SingleActor? soowon) = ([], null);
        foreach (SingleActor target in Targets)
        {
            switch (target.ID)
            {
                case (int)TargetID.TheDragonVoidJormag:
                    target.OverrideName("The JormagVoid");
                    jormag = target;
                    needRedirect = true;
                    var jormagAttacks = new HashSet<long>()
                    {
                        BreathOfJormag1,
                        BreathOfJormag2,
                        BreathOfJormag3,
                        GraspOfJormag,
                        FrostMeteor,
                    };
                    jormagDamagingAgents = new HashSet<ulong>(combatData.Where(x => x.IsDamage() && jormagAttacks.Contains(x.SkillID)).Select(x => x.SrcAgent));
                    break;
                case (int)TargetID.TheDragonVoidKralkatorrik:
                    target.OverrideName("The KralkatorrikVoid");
                    kralk = target;
                    needRedirect = true;
                    var kralkAttacks = new HashSet<long>()
                    {
                        BrandingBeam,
                        CrystalBarrage,
                        VoidPoolKralkatorrik
                    };
                    kralkDamagingAgents = new HashSet<ulong>(combatData.Where(x => x.IsDamage() && kralkAttacks.Contains(x.SkillID)).Select(x => x.SrcAgent));
                    break;
                case (int)TargetID.TheDragonVoidMordremoth:
                    target.OverrideName("The MordremothVoid");
                    mord = target;
                    needRedirect = true;
                    var mordAttacks = new HashSet<long>()
                    {
                        Shockwave,
                        PoisonRoar,
                    };
                    mordDamagingAgents = new HashSet<ulong>(combatData.Where(x => x.IsDamage() && mordAttacks.Contains(x.SkillID)).Select(x => x.SrcAgent));
                    break;
                case (int)TargetID.TheDragonVoidPrimordus:
                    target.OverrideName("The PrimordusVoid");
                    primordus = target;
                    needRedirect = true;
                    var primordusAttacks = new HashSet<long>()
                    {
                        LavaSlam,
                        JawsOfDestruction,
                    };
                    primordusDamagingAgents = new HashSet<ulong>(combatData.Where(x => x.IsDamage() && primordusAttacks.Contains(x.SkillID)).Select(x => x.SrcAgent));
                    break;
                case (int)TargetID.TheDragonVoidSooWon:
                    target.OverrideName("The SooWonVoid");
                    soowon = target;
                    needRedirect = true;
                    var soowonAttacks = new HashSet<long>()
                    {
                        TsunamiSlamOrb,
                        TsunamiSlamTailSlamOrb,
                        ClawSlap,
                        MagicHail,
                        VoidPurge,
                        VoidPoolSooWon,
                        TormentOfTheVoid,
                        TailSlam,
                    };
                    soowonDamagingAgents = new HashSet<ulong>(combatData.Where(x => x.IsDamage() && soowonAttacks.Contains(x.SkillID)).Select(x => x.SrcAgent));
                    break;
                case (int)TargetID.TheDragonVoidZhaitan:
                    target.OverrideName("The ZhaitanVoid");
                    zhaitan = target;
                    needRedirect = true;
                    var zhaiAttacks = new HashSet<long>()
                    {
                        ScreamOfZhaitanNM,
                        ScreamOfZhaitanCM,
                        ZhaitanTailSlam,
                        PutridDeluge
                    };
                    zhaitanDamagingAgents = new HashSet<ulong>(combatData.Where(x => x.IsDamage() && zhaiAttacks.Contains(x.SkillID)).Select(x => x.SrcAgent));
                    break;
                case (int)TargetID.PushableVoidAmalgamate:
                case (int)TargetID.KillableVoidAmalgamate:
                    target.OverrideName("Heart " + (++purificationID));
                    break;
            }
        }
        if (needRedirect)
        {
            foreach (CombatItem cbt in combatData)
            {
                if (cbt.IsDamage())
                {
                    // sanity check
                    if (agentData.GetAgent(cbt.SrcAgent, cbt.Time).GetFinalMaster().IsPlayer)
                    {
                        continue;
                    }
                    if (jormag != null && jormagDamagingAgents.Any(x => cbt.SrcAgent == x && jormag.FirstAware <= cbt.Time && cbt.Time <= jormag.LastAware))
                    {
                        cbt.OverrideSrcAgent(jormag.AgentItem);
                    }
                    else if (primordus != null && primordusDamagingAgents.Any(x => cbt.SrcAgent == x && primordus.FirstAware <= cbt.Time && cbt.Time <= primordus.LastAware))
                    {
                        cbt.OverrideSrcAgent(primordus.AgentItem);
                    }
                    else if (kralk != null && kralkDamagingAgents.Any(x => cbt.SrcAgent == x && kralk.FirstAware <= cbt.Time && cbt.Time <= kralk.LastAware))
                    {
                        cbt.OverrideSrcAgent(kralk.AgentItem);
                    }
                    else if (mord != null && mordDamagingAgents.Any(x => cbt.SrcAgent == x && mord.FirstAware <= cbt.Time && cbt.Time <= mord.LastAware))
                    {
                        cbt.OverrideSrcAgent(mord.AgentItem);
                    }
                    else if (zhaitan != null && zhaitanDamagingAgents.Any(x => cbt.SrcAgent == x && zhaitan.FirstAware <= cbt.Time && cbt.Time <= zhaitan.LastAware))
                    {
                        cbt.OverrideSrcAgent(zhaitan.AgentItem);
                    }
                    else if (soowon != null && soowonDamagingAgents.Any(x => cbt.SrcAgent == x && soowon.FirstAware <= cbt.Time && cbt.Time <= soowon.LastAware))
                    {
                        cbt.OverrideSrcAgent(soowon.AgentItem);
                    }
                }
            }
        }
        FirstAwareSortedTargets = Targets.OrderBy(x => x.FirstAware);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleGreen, out var greenEffects))
        {
            AddBaseShareTheVoidDecoration(log, greenEffects, environmentDecorations);
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSuccessGreen, out var successGreenEffects))
        {
            AddResultShareTheVoidDecoration(successGreenEffects, true, environmentDecorations);
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleFailedGreen, out var failedGreenEffects))
        {
            AddResultShareTheVoidDecoration(failedGreenEffects, false, environmentDecorations);
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidPoolRedPuddleAoECM, out var redPuddleEffectsCM))
        {
            AddPlacedVoidPoolDecoration(log, redPuddleEffectsCM, 400, 240000, environmentDecorations);
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidPoolRedPuddleAoENM, out var redPuddleEffectsNM))
        {
            AddPlacedVoidPoolDecoration(log, redPuddleEffectsNM, 300, 24000, environmentDecorations);
        }
        // Stormseer Ice Spike
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidStormseerIceSpikeIndicator, out var iceSpikes))
        {
            foreach (EffectEvent effect in iceSpikes)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 1750);
                var circle = new CircleDecoration(150, lifespan, Colors.LightBlue, 0.1, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.end));
            }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        long growing;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.PushableVoidAmalgamate:
                // Purification Zones
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationZones, out var purificationZoneEffects))
                {
                    var voidShells = log.CombatData.GetBuffDataByIDByDst(VoidShell, target.AgentItem);
                    var voidShellRemovals = voidShells.Where(x => x is BuffRemoveSingleEvent || x is BuffRemoveAllEvent).ToList();
                    int voidShellAppliesCount = voidShells.Where(x => x is BuffApplyEvent).Count();
                    int voidShellRemovalOffset = 0;
                    int purificationAdd = 0;
                    bool breakPurification = false;
                    foreach (EffectEvent purificationZoneEffect in purificationZoneEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                    {
                        lifespan = (purificationZoneEffect.Time, log.FightData.FightEnd);
                        uint radius = 280;
                        if (voidShellRemovalOffset < voidShellRemovals.Count)
                        {
                            lifespan.end = (int)voidShellRemovals[voidShellRemovalOffset++].Time;
                        }
                        replay.Decorations.Add(new CircleDecoration(radius, lifespan, Colors.White, 0.4, new PositionConnector(purificationZoneEffect.Position)));
                        purificationAdd++;
                        if (purificationAdd >= voidShellAppliesCount)
                        {
                            breakPurification = true;
                        }
                        if (breakPurification)
                        {
                            break;
                        }
                    }
                }
                // Jormag - Lightning of Jormag
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationLightningOfJormag, out var lightningOfJormagEffects))
                {
                    foreach (EffectEvent lightningEffect in lightningOfJormagEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                    {
                        int duration = 3000;
                        lifespan = (lightningEffect.Time - duration, lightningEffect.Time);
                        var circle = new CircleDecoration(200, lifespan, Colors.LightBlue, 0.2, new PositionConnector(lightningEffect.Position));
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }
                // Primordus - Flames of Primordus (Indicator)
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationFlamesOfPrimordusIndicator, out var flamesOfPrimordusIndicator))
                {
                    foreach (EffectEvent effect in flamesOfPrimordusIndicator.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                    {
                        // Durations between 2300 and 2400
                        lifespan = effect.Duration > 0 ? (effect.Time, effect.Time + effect.Duration) : (effect.Time, effect.Time + 2350);
                        var circle = new CircleDecoration(200, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                        if (!log.CombatData.HasMissileData && target.TryGetCurrentPosition(log, lifespan.start, out var primordusPos))
                        {
                            replay.Decorations.AddProjectile(primordusPos, effect.Position, lifespan, Colors.Yellow, 0.2);
                        }
                    }
                }
                // Primordus - Flames of Primordus (Flames)
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationFlamesOfPrimordus, out var flamesOfPrimordus))
                {
                    foreach (EffectEvent effect in flamesOfPrimordus.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                    {
                        (long start, long end) lifespanFlame = effect.ComputeLifespan(log, 2000); // Estimated
                        replay.Decorations.Add(new CircleDecoration(200, lifespanFlame, Colors.Red, 0.2, new PositionConnector(effect.Position)));
                    }

                    // Flame Orbs
                    var flameOrbs = log.CombatData.GetMissileEventsBySkillID(FlamesOfPrimordus);
                    replay.Decorations.AddNonHomingMissiles(log, flameOrbs, Colors.Red, 0.2, 50);
                }
                // Kralkatorrik - Stormfall (Cracks)
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationStormfall, out var stormfallEffects))
                {
                    foreach (EffectEvent stormfallEffect in stormfallEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                    {
                        lifespan = stormfallEffect.ComputeLifespan(log, 4560);
                        var connector = new PositionConnector(stormfallEffect.Position);
                        var rotationConnector = new AngleConnector(stormfallEffect.Rotation.Z);
                        var rectangle = (RectangleDecoration)new RectangleDecoration(90, 230, lifespan, Colors.DarkMagenta, 0.2, connector).UsingRotationConnector(rotationConnector);
                        replay.Decorations.AddWithGrowing(rectangle, lifespan.end);
                    }
                }
                // Mordremoth - Swarm of Mordremoth (Bees)
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationBeeLaunch, out var beeLaunchEffects))
                {
                    foreach (EffectEvent beeLaunchEffect in beeLaunchEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                    {
                        int start = (int)beeLaunchEffect.Time;
                        int end = start + 3000;
                        replay.Decorations.Add(new RectangleDecoration(380, 30, (start, end), Colors.Red, 0.4, new PositionConnector(beeLaunchEffect.Position).WithOffset(new(190, 0, 0), true)).UsingRotationConnector(new AngleConnector(beeLaunchEffect.Rotation.Z - 90)));
                        var circle = new CircleDecoration(280, (start, end), Colors.LightOrange, 0.2, new PositionConnector(beeLaunchEffect.Position));
                        replay.Decorations.AddWithGrowing(circle, end);
                        var initialPosition = new ParametricPoint3D(beeLaunchEffect.Position, end);
                        int velocity = 210;
                        int lifespanBees = 15000;
                        var finalPosition = new ParametricPoint3D(beeLaunchEffect.Position + (velocity * lifespanBees / 1000.0f) * new Vector3((float)Math.Cos(beeLaunchEffect.Orientation.Z - Math.PI / 2), (float)Math.Sin(beeLaunchEffect.Orientation.Z - Math.PI / 2), 0), end + lifespanBees);
                        replay.Decorations.Add(new CircleDecoration(280, (end, end + lifespanBees), Colors.Red, 0.4, new InterpolationConnector([initialPosition, finalPosition])));
                    }
                }
                // Zhaitan - Pool of Undeath
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationPoolOfUndeath, out var poolOfUndeathEffects))
                {
                    foreach (EffectEvent poolOfUndeathEffect in poolOfUndeathEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                    {
                        // The effect is larger than the damage area and the duration is shower than the logged one.
                        // The damage radius is roughly 180 to 200
                        int damageDuration = 18000;
                        uint radiusDamage = 190;
                        uint radiusEffect = 220;
                        (long start, long end) lifespanSpawning = (poolOfUndeathEffect.Time - 1000, poolOfUndeathEffect.Time);
                        (long start, long end) lifespanDamage = (poolOfUndeathEffect.Time, poolOfUndeathEffect.Time + damageDuration);
                        (long start, long end) lifespanDespawning = (poolOfUndeathEffect.Time + damageDuration, poolOfUndeathEffect.Time + damageDuration + 1000);
                        var position = new PositionConnector(poolOfUndeathEffect.Position);
                        var spawning = new CircleDecoration(radiusEffect, lifespanSpawning, Colors.GreyishGreen, 0.2, position);
                        var damage = new CircleDecoration(radiusDamage, lifespanDamage, Colors.GreyishGreen, 0.4, position);
                        var damageExtra = new DoughnutDecoration(radiusDamage, radiusEffect, lifespanDamage, Colors.GreyishGreen, 0.2, position);
                        var despawn = new CircleDecoration(radiusEffect, lifespanDespawning, Colors.GreyishGreen, 0.2, position);
                        replay.Decorations.AddWithGrowing(spawning, lifespanSpawning.end);
                        replay.Decorations.AddWithBorder(damage, Colors.Red, 0.4);
                        replay.Decorations.Add(damageExtra);
                        replay.Decorations.AddWithGrowing(despawn, lifespanDespawning.end, true);
                    }
                }
                // Soo Won - Corrupted Waters (Orbs)
                var corruptedWaters = log.CombatData.GetMissileEventsBySkillID(SwarmOfMordremoth_CorruptedWaters);
                replay.Decorations.AddNonHomingMissiles(log, corruptedWaters, Colors.LightBlue, 0.6, 10);
                // Magic Discharge - Orb Explosion
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleOrbExplosion, out var orbEffects))
                {
                    foreach (EffectEvent orbEffect in orbEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                    {
                        int duration = 3000;
                        lifespan = (orbEffect.Time, orbEffect.Time + duration);
                        // Radius is an estimate - orb exploding on edge doesn't quite cover the entirety of the arena
                        uint radius = 2700;
                        replay.Decorations.AddShockwave(new PositionConnector(orbEffect.Position), lifespan, Colors.White, 0.2, radius);
                    }
                }
                // Breakbar Active
                BreakbarStateEvent? breakbar = log.CombatData.GetBreakbarStateEvents(target.AgentItem).FirstOrDefault(x => x.State == BreakbarState.Active);
                if (breakbar != null)
                {
                    var breakbarStates = log.CombatData.GetBreakbarPercentEvents(target.AgentItem).Where(x => x.Time >= breakbar.Time);
                    replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, (breakbar.Time, target.LastAware), Colors.LightBlue, 0.6, Colors.Black, 0.2, breakbarStates.Select(x => (x.Time, x.BreakbarPercent)).ToList(), new AgentConnector(target))
                        .UsingInterpolationMethod(Connector.InterpolationMethod.Step)
                        .UsingRotationConnector(new AngleConnector(180)));
                }
                break;
            case (int)TargetID.TheDragonVoidJormag:
                // Frost Meteor
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleJormagFrostMeteorIceField, out var meteorEffects))
                {
                    foreach (EffectEvent effect in meteorEffects)
                    {
                        int indicatorDuration = 1500;
                        int spreadDuration = 3000;
                        int lingerDuration = 9500;
                        long start = effect.Time;
                        long fieldEnd = Math.Min(start + lingerDuration, target.LastAware);
                        // meteor impact
                        replay.Decorations.AddWithGrowing(new CircleDecoration(600, (start - indicatorDuration, start), Colors.LightOrange, 0.2, new PositionConnector(effect.Position)), start);
                        // ice field
                        replay.Decorations.AddWithGrowing(new CircleDecoration(1200, (start, fieldEnd), Colors.SkyBlue, 0.1, new PositionConnector(effect.Position)), start + spreadDuration);
                    }
                }
                // Grasp of Jormag - Indicators
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleJormagGraspOfJormagIndicator, out var graspOfJormag))
                {
                    foreach (EffectEvent effect in graspOfJormag)
                    {
                        lifespan = effect.ComputeLifespan(log, 2000);
                        replay.Decorations.Add(new CircleDecoration(160, lifespan, Colors.LightOrange, 0.1, new PositionConnector(effect.Position)));
                    }
                }
                // Grasp of Jormag - Moving AoE
                var graspOfJormagProjectiles = log.CombatData.GetMissileEventsBySkillID(GraspOfJormag);
                foreach (MissileEvent grasp in graspOfJormagProjectiles)
                {
                    lifespan = (grasp.Time, grasp.RemoveEvent?.Time ?? Math.Min(log.FightData.FightEnd, target.LastAware));
                    for (int i = 0; i < grasp.LaunchEvents.Count; i++)
                    {
                        MissileLaunchEvent? launch = grasp.LaunchEvents[i];
                        lifespan = (launch.Time, i != grasp.LaunchEvents.Count - 1 ? grasp.LaunchEvents[i + 1].Time : lifespan.end);
                        var connector = new InterpolationConnector([new ParametricPoint3D(launch.LaunchPosition, lifespan.start), launch.GetFinalPosition(lifespan)], Connector.InterpolationMethod.Linear);
                        var beamAoE = new CircleDecoration(160, lifespan, Colors.LightBlue, 0.1, connector);
                        replay.Decorations.AddWithBorder(beamAoE, Colors.Red, 0.5);
                    }
                }
                // Frost Beam - Non-NPC sets
                var breathOfJormag = log.CombatData.GetMissileEventsBySkillID(BreathOfJormag2);
                foreach (MissileEvent breath in breathOfJormag)
                {
                    // The breath is missing MissileRemove events, we compute the removal manually
                    long beamAgentSpawnTime = log.FightData.FightEnd;
                    var beamAgents = log.AgentData.GetNPCsByIDs([TargetID.JormagMovingFrostBeam, TargetID.JormagMovingFrostBeamNorth, TargetID.JormagMovingFrostBeamCenter]);
                    foreach (AgentItem agent in beamAgents)
                    {
                        // Find the closest velocity change event
                        VelocityEvent? frostBeamVelocity = log.CombatData.GetMovementData(agent).OfType<VelocityEvent>().FirstOrDefault(x => x.GetPoint3D().Length() > 0);
                        if (frostBeamVelocity != null && frostBeamVelocity.Time > breath.Time)
                        {
                            beamAgentSpawnTime = Math.Min(beamAgentSpawnTime, frostBeamVelocity.Time);
                        }
                    }
                    // Find the minimum value between LastAware, VelicityEvent and FightEnd
                    // If there isn't a VelocityEvent, it uses FightEnd, otherwise it's always LastAware
                    // The beams can spawn after Jormag has died, they last roughly 3 seconds, if they spawn just before Jormag dies, they get cancelled
                    var end = breath.Time >= target.LastAware ? Math.Min(target.LastAware + 3000, beamAgentSpawnTime) : Math.Min(target.LastAware, beamAgentSpawnTime);
                    lifespan = (breath.Time, end);

                    for (int i = 0; i < breath.LaunchEvents.Count; i++)
                    {
                        MissileLaunchEvent? launch = breath.LaunchEvents[i];
                        lifespan = (launch.Time, i != breath.LaunchEvents.Count - 1 ? breath.LaunchEvents[i + 1].Time : lifespan.end);
                        var connector = new InterpolationConnector([new ParametricPoint3D(launch.LaunchPosition, lifespan.start), launch.GetFinalPosition(lifespan)], Connector.InterpolationMethod.Linear);
                        var beamAoE = new CircleDecoration(300, lifespan, Colors.LightBlue, 0.1, connector);
                        replay.Decorations.AddWithBorder(beamAoE, Colors.Red, 0.5);
                    }
                }
                break;
            case (int)TargetID.JormagMovingFrostBeam:
            case (int)TargetID.JormagMovingFrostBeamNorth:
            case (int)TargetID.JormagMovingFrostBeamCenter:
                VelocityEvent? frostBeamMoveStartVelocity = log.CombatData.GetMovementData(target.AgentItem).OfType<VelocityEvent>().FirstOrDefault(x => x.GetPoint3D().Length() > 0);
                // Beams are immobile at spawn for around 3 seconds
                if (frostBeamMoveStartVelocity != null)
                {
                    lifespan = (frostBeamMoveStartVelocity.Time, target.LastAware);
                    replay.Trim(lifespan.start, lifespan.end);
                    var beamAoE = new CircleDecoration(300, lifespan, Colors.LightBlue, 0.1, new AgentConnector(target));
                    replay.Decorations.AddWithBorder(beamAoE, Colors.Red, 0.5);
                }
                else
                {
                    // Completely hide it
                    replay.Trim(0, 0);
                }
                break;
            case (int)TargetID.DragonEnergyOrb:
                (int dragonOrbStart, int dragonOrbEnd) = ((int)target.FirstAware, (int)target.LastAware);
                replay.Decorations.Add(new CircleDecoration(160, (dragonOrbStart, dragonOrbEnd), "rgba(200, 50, 0, 0.5)", new AgentConnector(target)).UsingFilled(false));
                break;
            case (int)TargetID.TheDragonVoidPrimordus:
                // Lava Slam - Chin Indicator
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePrimordusLavaSlamIndicator, out var lavaSlams))
                {
                    foreach (EffectEvent effect in lavaSlams)
                    {
                        // The indicator gets cancelled when phasing to Kralkatorrik.
                        int duration = 3500;
                        long growingEnd = effect.Time + duration;
                        lifespan = (effect.Time - duration, effect.Time);
                        lifespan.end = Math.Min(lifespan.end, target.LastAware);
                        replay.Decorations.AddWithGrowing(new CircleDecoration(580, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position)), growingEnd);
                    }
                }

                // We use the damage field for 2 reasons:
                // 1: When the instance is bugged from a previous wipe, the orange warning indicator is not present but the red damage field is always visible.
                // 2: The red damage field is not in the same position of the orange warning indicator, rending the indicator inaccurate.

                var jawsOfDestructionPosition = new Vector3(591.0542f, -21528.8555f, -15418.3f);
                var jawsOfDestructionConnector = new PositionConnector(jawsOfDestructionPosition);
                int jawsOfDestructionIndicatorDuration = 6950;

                // Jaws of Primordus - Orange Indicator
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePrimordusJawsOfDestructionIndicator, out var jawsOfDestructionIndicators))
                {
                    foreach (EffectEvent effect in jawsOfDestructionIndicators)
                    {
                        // The indicator doesn't get cancelled when phasing to Kralkatorrik but also doesn't do anything, we remove it when phasing.
                        long growingEnd = effect.Time + jawsOfDestructionIndicatorDuration;
                        lifespan = (effect.Time, Math.Min(growingEnd, target.LastAware));
                        var indicator = new CircleDecoration(1450, lifespan, Colors.Orange, 0.2, jawsOfDestructionConnector);
                        replay.Decorations.AddWithGrowing(indicator, growingEnd);
                    }
                }

                // Jaws of Primordus - Red Field
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePrimordusJawsOfDestructionDamage, out var jawsOfDestructionDamageAoEs))
                {
                    foreach (EffectEvent effect in jawsOfDestructionDamageAoEs)
                    {
                        // Adding the indicator in case it's bugged and it's invisible in game.
                        if (jawsOfDestructionIndicators == null || jawsOfDestructionIndicators.FirstOrDefault(x => x.Time > effect.Time - 10000 && x.Time < effect.Time) == null)
                        {
                            (long start, long end) lifespanIndicator = (effect.Time - jawsOfDestructionIndicatorDuration, effect.Time);
                            var indicator = new CircleDecoration(1450, lifespanIndicator, Colors.Orange, 0.2, jawsOfDestructionConnector);
                            replay.Decorations.AddWithGrowing(indicator, lifespanIndicator.end);
                        }
                        // The damage field gets cancelled when phasing to Kralkatorrik.
                        (long start, long end) lifespanDamage = effect.ComputeLifespan(log, 5000);
                        lifespanDamage.end = Math.Min(lifespanDamage.end, target.LastAware);
                        var damage = new CircleDecoration(1450, lifespanDamage, Colors.Red, 0.4, jawsOfDestructionConnector);
                        replay.Decorations.Add(damage);
                    }
                }
                break;
            case (int)TargetID.TheDragonVoidKralkatorrik:
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleKralkatorrikBeamIndicator, out var kralkBeamEffects))
                {
                    foreach (EffectEvent effect in kralkBeamEffects)
                    {
                        int indicatorDuration = 2000;
                        long growingEnd = effect.Time + indicatorDuration;
                        lifespan = (effect.Time, Math.Min(growingEnd, target.LastAware));
                        replay.Decorations.AddWithGrowing(new RectangleDecoration(700, 2900, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position)), growingEnd);
                    }
                }
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleKralkatorrikBeamAoe, out var kralkBeamAoeEffects))
                {
                    foreach (EffectEvent effect in kralkBeamAoeEffects)
                    {
                        // Effect duration logged is too long
                        int duration = 6300;
                        lifespan = (effect.Time, Math.Min(effect.Time + duration, target.LastAware));
                        replay.Decorations.Add(new CircleDecoration(350, lifespan, Colors.Black, 0.4, new PositionConnector(effect.Position)));
                    }
                }
                // Crystal Barrage
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleKralkatorrikCrystalBarrageImpact, out var crystalBarrage))
                {
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleScalableOrangeAoE, out var aoeIndicator))
                    {
                        foreach (EffectEvent impactEffect in crystalBarrage)
                        {
                            foreach (EffectEvent indicator in aoeIndicator.Where(x => Math.Abs(x.Time - impactEffect.Time) < 5000 && impactEffect.Time > x.Time))
                            {
                                if ((impactEffect.Position - indicator.Position).XY().Length() < 60)
                                {
                                    uint radius = 320;
                                    lifespan = (indicator.Time, indicator.Time + (impactEffect.Time - indicator.Time));
                                    var positionConnector = new PositionConnector(impactEffect.Position);
                                    var warning = new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, positionConnector);
                                    var impect = new CircleDecoration(radius, (lifespan.end, lifespan.end + 250), Colors.White, 0.3, positionConnector);
                                    replay.Decorations.AddWithGrowing(warning, lifespan.end);
                                    replay.Decorations.AddWithBorder(impect, Colors.DarkPurple, 0.5);
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            case (int)TargetID.DragonBodyVoidAmalgamate:
                break;
            case (int)TargetID.VoidAmalgamate:
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleInfluenceOfTheVoidPool, out var poolEffects))
                {
                    if (poolEffects.Count != 0)
                    {
                        // To be safe
                        poolEffects = poolEffects.OrderBy(x => x.Time).ToList();
                        double radius = 100.0;
                        double radiusIncrement = log.FightData.IsCM ? 35.0 : 35.0 / 2;
                        for (int i = 0; i < poolEffects.Count - 1; i++)
                        {
                            EffectEvent curEffect = poolEffects[i];
                            EffectEvent nextEffect = poolEffects[i + 1];
                            int start = (int)curEffect.Time;
                            int end = (int)nextEffect.Time;
                            replay.Decorations.AddWithBorder(new CircleDecoration((uint)radius, (start, end), "rgba(59, 0, 16, 0.2)", new PositionConnector(curEffect.Position)), Colors.Red, 0.5);
                            radius += radiusIncrement;
                        }
                        EffectEvent lastEffect = poolEffects.Last();
                        lifespan = lastEffect.ComputeLifespanWithSecondaryEffectNoSrcCheck(log, EffectGUIDs.HarvestTempleVoidPoolOrbGettingReadyToBeDangerous);
                        (long start, long end) lifespanPuriOrb = lastEffect.ComputeLifespanWithSecondaryEffectNoSrcCheck(log, EffectGUIDs.HarvestTemplePurificationOrbSpawns);
                        SingleActor? nextPurificationOrb = Targets.Where(x => x.IsSpecies(TargetID.PushableVoidAmalgamate) || x.IsSpecies(TargetID.KillableVoidAmalgamate)).FirstOrDefault(x => x.FirstAware > lastEffect.Time - ServerDelayConstant);
                        long nextPurifcationOrbStart = long.MaxValue;
                        if (nextPurificationOrb != null)
                        {
                            nextPurifcationOrbStart = nextPurificationOrb.FirstAware;
                        }
                        lifespan.end = Math.Min(nextPurifcationOrbStart, Math.Min(lifespan.end, lifespanPuriOrb.end));
                        // In case log ended before the event happens and we are on pre Effect51 events, we use the expected duration of the effect instead
                        if (lifespan.start == lifespan.end)
                        {
                            lifespan.end = lifespan.start + 4000;
                        }
                        replay.Decorations.AddWithBorder(new CircleDecoration((uint)radius, lifespan, "rgba(59, 0, 16, 0.2)", new PositionConnector(lastEffect.Position)), Colors.Red, 0.5);
                    }
                }
                break;
            case (int)TargetID.TheDragonVoidMordremoth:
                // Poison Roar - AoEs
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleMordremothPoisonRoarImpact, out var mordremothPoisonEffects))
                {
                    foreach (EffectEvent effect in mordremothPoisonEffects)
                    {
                        lifespan = (effect.Time - 2000, effect.Time);
                        replay.Decorations.AddWithGrowing(new CircleDecoration(200, lifespan, Colors.MilitaryGreen, 0.2, new PositionConnector(effect.Position)), lifespan.end);
                    }
                }
                // Poison Roar - Projectiles
                var poisonRoar = log.CombatData.GetMissileEventsBySkillID(PoisonRoar);
                replay.Decorations.AddNonHomingMissiles(log, poisonRoar, Colors.MilitaryGreen, 0.3, 25);
                // Shockwaves
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleMordremothShockwave1, out var shockwaves))
                {
                    foreach (EffectEvent effect in shockwaves)
                    {
                        uint radius = 2000; // Assumed radius
                        lifespan = (effect.Time, effect.Time + 1600); // Assumed duration, effect has 0
                        GeographicalConnector connector = new PositionConnector(effect.Position);
                        replay.Decorations.AddShockwave(connector, lifespan, Colors.Black, 0.6, radius);
                    }
                }
                break;
            case (int)TargetID.TheDragonVoidZhaitan:
                // Putrid Deluge
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleZhaitanPutridDelugeImpact, out var putridDelugeImpacts))
                {
                    foreach (EffectEvent effect in putridDelugeImpacts)
                    {
                        lifespan = (effect.Time - 2000, effect.Time);
                        replay.Decorations.AddWithGrowing(new CircleDecoration(200, lifespan, Colors.LightMilitaryGreen, 0.2, new PositionConnector(effect.Position)), lifespan.end);
                    }
                }
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleZhaitanPutridDelugeAoE, out var putridDelugeAoEs))
                {
                    foreach (EffectEvent effect in putridDelugeAoEs)
                    {
                        int fieldDuration = 10000;
                        lifespan = (effect.Time, effect.Time + fieldDuration);
                        if (lifespan.start < target.LastAware)
                        {
                            lifespan.end = Math.Min(target.LastAware, lifespan.end);
                        }
                        replay.Decorations.AddWithBorder(new CircleDecoration(200, lifespan, Colors.LightMilitaryGreen, 0.2, new PositionConnector(effect.Position)), Colors.Red, 0.4);
                    }
                }
                // Putrid Deluge - Projectiles
                var putrid = log.CombatData.GetMissileEventsBySkillID(PutridDeluge);
                replay.Decorations.AddNonHomingMissiles(log, putrid, Colors.LightMilitaryGreen, 0.3, 25);
                // Tail Slam
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleZhaitanTailSlamImpact, out var zhaitainTailSlam))
                {
                    foreach (EffectEvent effect in zhaitainTailSlam)
                    {
                        // We use effect time - 3000 because the AoE effect indicator isn't disambiguous, the impact is
                        lifespan = (effect.Time - 3000, effect.Time);
                        var circle = new CircleDecoration(620, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }
                // Scream of Zhaitan
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleZhaitanScreamIndicator, out var screamOfZhaitan))
                {
                    foreach (EffectEvent effect in screamOfZhaitan)
                    {
                        lifespan = effect.ComputeLifespan(log, 3000);
                        var circle = new CircleDecoration(1720, lifespan, Colors.LightRed, 0.1, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }
                break;
            case (int)TargetID.TheDragonVoidSooWon:
                // Claw Swipe
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSooWonClaw, out var sooWonClawEffects))
                {
                    var rotationConnector = new AngleConnector(99.6f);
                    foreach (EffectEvent effect in sooWonClawEffects)
                    {
                        lifespan = effect.ComputeLifespan(log, 2300);
                        var connector = new PositionConnector(effect.Position);
                        replay.Decorations.AddWithGrowing((PieDecoration)new PieDecoration(1060, 145, lifespan, Colors.Red, 0.4, connector).UsingRotationConnector(rotationConnector), lifespan.end);
                    }
                }

                // Claw Swipe - Bouncing Void Orbs
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSooWonVoidOrbs1, out var clawVoidOrbs))
                {
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleTormentOfTheVoidClawIndicator, out var clawVoidOrbsAoEs))
                    {
                        var aoeToAoeMatches = new List<(EffectEvent, EffectEvent, float)>();
                        var orbToAoeMatches = new List<(EffectEvent, EffectEvent, float)>();

                        if (clawVoidOrbs.Count > 0 && clawVoidOrbsAoEs.Count > 0)
                        {
                            // The aoe indicator can be used by other attacks before soo won - filtering out the effects which happen before a claw swipe
                            var filteredBouncingOrbsAoEs = clawVoidOrbsAoEs.Where(x => x.Time > clawVoidOrbs.First().Time);
                            orbToAoeMatches = MatchEffectToEffect(clawVoidOrbs, filteredBouncingOrbsAoEs);
                            aoeToAoeMatches = MatchEffectToEffect(filteredBouncingOrbsAoEs, filteredBouncingOrbsAoEs);
                        }

                        // Hard coded the orb positions and the durations for older logs
                        var positions = new List<ParametricPoint3D>()
                            {
                                new(1527.933f, -20447.47f, -15420.13f, 803),
                                new(74.92969f, -20728.86f, -15420.13f, 803),
                                new(-353.9098f, -21363.69f, -15420.13f, 803),
                                new(1873.578f, -20620.1f, -15420.13f, 803),
                                new(397.2551f, -20515.84f, -15420.13f, 803),
                                new(-181.2787f, -21018.05f, -15420.13f, 803),
                                new(763.7318f, -20393.5f, -15420.13f, 803),
                                new(1149.385f, -20370.18f, -15420.13f, 803),
                                new(1184.253f, -19876.46f, -15420.13f, 1133),
                                new(1689.397f, -19979.6f, -15420.13f, 1133),
                                new(-591.4208f, -20740.99f, -15420.13f, 1133),
                                new(-249.5301f, -20355.09f, -15420.13f, 1133),
                                new(180.5903f, -20070.83f, -15420.13f, 1133),
                                new(669.6267f, -19907.58f, -15420.13f, 1133),
                                new(-553.218f, -20005.25f, -15420.13f, 1133),
                                new(1216.888f, -19414.34f, -15420.13f, 1133),
                                new(-22.20401f, -19654.31f, -15420.13f, 1133),
                                new(581.5457f, -19452.76f, -15420.13f, 1133),
                                new(-224.9983f, -19237.79f, -15420.13f, 1133),
                                new(493.4647f, -18997.94f, -15420.13f, 1133),
                            };

                        // Orb indicator near the swipe cone
                        foreach (EffectEvent orb in clawVoidOrbs)
                        {
                            lifespan = orb.ComputeLifespan(log, 2080);
                            var circle = new CircleDecoration(25, lifespan, Colors.Black, 0.5, new PositionConnector(orb.Position));
                            replay.Decorations.Add(circle);
                        }

                        // Orb to AoE effects
                        foreach ((EffectEvent aoe, EffectEvent orb, float distance) in orbToAoeMatches)
                        {
                            (long start, long end) lifespanAoE = (aoe.Time, aoe.Time + aoe.Duration);
                            if (aoe.Duration == 0)
                            {
                                foreach (ParametricPoint3D point in positions)
                                {
                                    if ((aoe.Position - point.XYZ).XY().Length() < 0.5)
                                    {
                                        lifespanAoE.end = lifespanAoE.start + point.Time;
                                        break;
                                    }
                                }
                            }
                            // Add aoe
                            replay.Decorations.AddWithGrowing(new CircleDecoration(200, lifespanAoE, Colors.LightOrange, 0.2, new PositionConnector(aoe.Position)), lifespanAoE.end);
                            // Add projectile
                            if (!log.CombatData.HasMissileData)
                            {
                                replay.Decorations.AddProjectile(orb.Position, aoe.Position, lifespanAoE, Colors.Black, 0.5);
                            }
                        }

                        // AoE to AoE effects
                        foreach ((EffectEvent endingAoE, EffectEvent startingAoE, float distance) in aoeToAoeMatches)
                        {
                            long endingAoEDuration = endingAoE.Duration;
                            long startingAoEDuration = startingAoE.Duration;
                            if (endingAoEDuration == 0 || startingAoEDuration == 0)
                            {
                                foreach (ParametricPoint3D point in positions)
                                {
                                    if (endingAoEDuration != 0 && startingAoEDuration != 0)
                                    {
                                        break;
                                    }

                                    if (endingAoEDuration == 0 && (endingAoE.Position - point.XYZ).XY().Length() < 0.5)
                                    {
                                        endingAoEDuration = point.Time;
                                    }

                                    if (startingAoEDuration == 0 && (startingAoE.Position - point.XYZ).XY().Length() < 0.5)
                                    {
                                        startingAoEDuration = point.Time;
                                    }
                                }
                            }
                            // Add aoe
                            (long start, long end) lifespanAoE = (endingAoE.Time, endingAoE.Time + endingAoEDuration);
                            replay.Decorations.AddWithGrowing(new CircleDecoration(200, lifespanAoE, Colors.LightOrange, 0.2, new PositionConnector(endingAoE.Position)), lifespanAoE.end);
                            // Add projectile - Starts when the previous AoE ends because it's bouncing
                            (long start, long end) lifespanAnimation = (startingAoE.Time + startingAoEDuration, endingAoE.Time + endingAoEDuration);
                            if (!log.CombatData.HasMissileData)
                            {
                                replay.Decorations.AddProjectile(startingAoE.Position, endingAoE.Position, lifespanAnimation, Colors.Black, 0.5);
                            }
                        }
                    }
                }

                // Torment of the Void - Claw Swipe Orbs
                var tormentOrbs = log.CombatData.GetMissileEventsBySkillID(TormentOfTheVoid);
                replay.Decorations.AddNonHomingMissiles(log, tormentOrbs, Colors.Black, 0.5, 50);

                // Tail Slam
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleTailSlamIndicator, out var tailSlamEffects))
                {
                    // Generic Orange AoE - Used in multiple sections of the encounter
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleScalableOrangeAoE, out var genericOrangeAoE))
                    {
                        foreach (EffectEvent tailSlamEffect in tailSlamEffects)
                        {
                            // Filtering the effects
                            var filteredVoidOrbsAoEs = genericOrangeAoE.Where(x => Math.Abs(x.Time - tailSlamEffect.Time) < 2000 && x.Time < tailSlamEffect.Time + 10000);

                            // Tail Slam AoE
                            (long start, long end) lifespanTail = tailSlamEffect.ComputeLifespan(log, 1600);
                            replay.Decorations.AddWithGrowing(new RectangleDecoration(3000, 750, lifespanTail, Colors.Red, 0.2, new PositionConnector(tailSlamEffect.Position)), lifespanTail.end);

                            // Void Orbs AoEs
                            foreach (EffectEvent orbAoeEffect in filteredVoidOrbsAoEs)
                            {
                                (long start, long end) lifespanAoE = orbAoeEffect.ComputeLifespan(log, 1900);
                                replay.Decorations.AddWithGrowing(new CircleDecoration(200, lifespanAoE, Colors.LightOrange, 0.2, new PositionConnector(orbAoeEffect.Position)), lifespanAoE.end);
                                // Add projectile
                                if (!log.CombatData.HasMissileData)
                                {
                                    replay.Decorations.AddProjectile(tailSlamEffect.Position, orbAoeEffect.Position, lifespanAoE, Colors.Black, 0.5);
                                }
                            }
                        }
                    }
                }

                // Tail Slam Orbs
                var tailSlamOrbs = log.CombatData.GetMissileEventsBySkillID(TsunamiSlamTailSlamOrb);
                replay.Decorations.AddNonHomingMissiles(log, tailSlamOrbs, Colors.Black, 0.5, 50);

                // Tsunami Slam AoE indicator
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSooWonTsunamiSlamIndicator, out var tsunamiSlamIndicators))
                {
                    // Generic Orange AoE - Used in multiple sections of the encounter
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleScalableOrangeAoE, out var genericOrangeAoE))
                    {
                        foreach (EffectEvent tsunamiSlamEffect in tsunamiSlamIndicators)
                        {
                            // Filtering the effects
                            var filteredVoidOrbsAoEs = genericOrangeAoE.Where(x => Math.Abs(x.Time - tsunamiSlamEffect.Time) < 2000 && x.Time < tsunamiSlamEffect.Time + 10000);

                            (long start, long end) lifespanClawAoE = tsunamiSlamEffect.ComputeLifespan(log, 1600);
                            replay.Decorations.AddWithGrowing(new CircleDecoration(235, lifespanClawAoE, Colors.Red, 0.2, new PositionConnector(tsunamiSlamEffect.Position)), lifespanClawAoE.end);

                            // Void Orbs AoEs
                            foreach (EffectEvent orbAoeEffect in filteredVoidOrbsAoEs)
                            {
                                (long start, long end) lifespanAoE = orbAoeEffect.ComputeLifespan(log, 1900);
                                replay.Decorations.AddWithGrowing(new CircleDecoration(200, lifespanAoE, Colors.LightOrange, 0.2, new PositionConnector(orbAoeEffect.Position)), lifespanAoE.end);
                                // Add projectile
                                if (!log.CombatData.HasMissileData)
                                {
                                    replay.Decorations.AddProjectile(tsunamiSlamEffect.Position, orbAoeEffect.Position, lifespanAoE, Colors.Black, 0.5);
                                }
                            }
                        }
                    }
                }

                // Tsunami Slam Orbs
                var tsunamiOrbs = log.CombatData.GetMissileEventsBySkillID(TsunamiSlamOrb);
                replay.Decorations.AddNonHomingMissiles(log, tsunamiOrbs, Colors.Black, 0.5, 50);

                // Tsunami Wave
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleTsunami1, out var tsunamiEffects))
                {
                    foreach (EffectEvent effect in tsunamiEffects)
                    {
                        // Expanding wave - radius and duration are estimates, can't seem to line up the decoration with actual hits
                        uint radius = 2000; // Assumed radius
                        lifespan = (effect.Time, effect.Time + 4500); // Assumed duration
                        GeographicalConnector connector = new PositionConnector(effect.Position);
                        replay.Decorations.AddShockwave(connector, lifespan, Colors.Blue, 0.5, radius);
                    }
                }

                // Magic Hail
                var magicHail = log.CombatData.GetMissileEventsBySkillID(MagicHail);
                replay.Decorations.AddNonHomingMissiles(log, magicHail, Colors.Black, 0.5, 25);
                break;
            case (int)TargetID.KillableVoidAmalgamate:
                // Grasp of the Void
                var orbs = log.CombatData.GetMissileEventsBySkillID(GraspOfTheVoid);
                replay.Decorations.AddNonHomingMissiles(log, orbs, Colors.Black, 0.5, 40);
                break;
            case (int)TargetID.VoidWarforged1:
            case (int)TargetID.VoidWarforged2:
#if DEBUG_EFFECTS
                    CombatReplay.DebugEffects(target, log, replay, knownEffectsIDs, target.FirstAware, target.LastAware, true);
#endif
                break;
            case (int)TargetID.ZhaitansReach:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Thrash - Circle that pulls in
                        case ZhaitansReachThrashHT1:
                        case ZhaitansReachThrashHT2:
                            castDuration = 1900;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            replay.Decorations.AddWithGrowing(new DoughnutDecoration(260, 480, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), lifespan.end);
                            break;
                        // Ground Slam - AoE that knocks out
                        case ZhaitansReachGroundSlam:
                        case ZhaitansReachGroundSlamHT:
                            castDuration = cast.SkillID == ZhaitansReachGroundSlam ? 800 : 2500;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            replay.Decorations.AddWithGrowing(new CircleDecoration(400, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), lifespan.end);
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.VoidBrandbomber:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Branded Artillery
                        case BrandedArtillery:
                            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidBrandbomberBrandedArtillery, out var brandedArtilleryAoEs))
                            {
                                castDuration = 2500;
                                EffectEvent? brandedArtilleryAoE = brandedArtilleryAoEs.FirstOrDefault(x => x.Time > cast.Time && x.Time < cast.Time + castDuration + 100);
                                if (brandedArtilleryAoE != null && target.TryGetCurrentPosition(log, cast.Time, out var brandbomberPosition, 1000))
                                {
                                    // Shooting animation
                                    long animationDuration = brandedArtilleryAoE.Time - cast.Time;
                                    lifespan = (brandedArtilleryAoE.Time, brandedArtilleryAoE.Time + animationDuration);

                                    // Landing indicator
                                    var positionConnector = new PositionConnector(brandedArtilleryAoE.Position);
                                    var aoeCircle = new CircleDecoration(240, lifespan, Colors.LightOrange, 0.2, positionConnector);
                                    replay.Decorations.AddWithGrowing(aoeCircle, lifespan.end);
                                    // Projectile decoration
                                    if (!log.CombatData.HasMissileData)
                                    {
                                        replay.Decorations.AddProjectile(brandbomberPosition, brandedArtilleryAoE.Position, lifespan, Colors.DarkPurple);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                // Branded Artillery Orb
                var brandedArtilleryOrbs = log.CombatData.GetMissileEventsBySrcBySkillID(target.AgentItem, BrandedArtillery);
                replay.Decorations.AddNonHomingMissiles(log, brandedArtilleryOrbs, Colors.DarkPurple, 0.2, 50);
                break;
            case (int)TargetID.VoidTimeCaster:
                // Gravity Crush - Indicator
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidTimecasterGravityCrushIndicator, out var gravityCrushIndicators))
                {
                    foreach (EffectEvent effect in gravityCrushIndicators)
                    {
                        lifespan = effect.ComputeLifespan(log, 1600);
                        replay.Decorations.AddContrenticRings(0, 40, lifespan, effect.Position, Colors.Orange);
                    }
                }

                // Nightmare Epoch - AoEs
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidTimecasterNightmareEpoch, out var nightmareEpoch))
                {
                    foreach (EffectEvent effect in nightmareEpoch)
                    {
                        lifespan = effect.ComputeLifespan(log, 10000);
                        var positionConnector = new PositionConnector(effect.Position);
                        var circle = new CircleDecoration(100, lifespan, Colors.Black, 0.2, positionConnector);
                        replay.Decorations.AddWithBorder(circle, Colors.LightRed, 0.4);
                    }
                }

                // Nightmare Epoch - Projectiles
                var nightmareEpochProjectiles = log.CombatData.GetMissileEventsBySkillID(NightmareEpochDamage);
                replay.Decorations.AddNonHomingMissiles(log, nightmareEpochProjectiles, Colors.Black, 0.5, 10);
                break;
            case (int)TargetID.GravityBall:
                // Setting the first aware to + 1600 due to the duration of the warning effect
                (long start, long end) lifespanBall = (target.FirstAware + 1600, target.LastAware);
                var perimeter = (CircleDecoration)new CircleDecoration(320, 300, lifespanBall, Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false);
                replay.Decorations.Add(perimeter);
                break;
            case (int)TargetID.VoidGiant:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Death Scream - Fear
                        case DeathScream:
                            castDuration = 1680;
                            long supposedEndCast = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            var circle = new CircleDecoration(500, lifespan, Colors.Orange, 0.2, new AgentConnector(target));
                            replay.Decorations.AddWithGrowing(circle, supposedEndCast);
                            break;
                        default:
                            break;
                    }
                }

                // Rotting Bile - Poison AoE Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidGiantRottingBileIndicator, out var bileIndicators))
                {
                    foreach (EffectEvent effect in bileIndicators)
                    {
                        lifespan = effect.ComputeLifespan(log, 1400);
                        var positionConnector = new PositionConnector(effect.Position);
                        var circle = new CircleDecoration(250, lifespan, Colors.LightOrange, 0.2, positionConnector);
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }

                // Rotting Bile - Poison AoE Damage
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidGiantRottingBileDamage, out var bileAoEs))
                {
                    foreach (EffectEvent effect in bileAoEs)
                    {
                        lifespan = effect.ComputeLifespan(log, 10000);
                        var positionConnector = new PositionConnector(effect.Position);
                        var circle = new CircleDecoration(250, lifespan, Colors.DarkGreen, 0.2, positionConnector);
                        replay.Decorations.Add(circle);
                    }
                }
                break;
            case (int)TargetID.VoidSaltsprayDragon:
                // Call Lightning
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidSaltsprayDragonCallLightning, out var callLightnings))
                {
                    foreach (EffectEvent effect in callLightnings)
                    {
                        uint radius = 50;
                        lifespan = (effect.Time - 2000, effect.Time);
                        var positionConnector = new PositionConnector(effect.Position);
                        var circleIndicator = new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, positionConnector);
                        var lightningIndicator = new CircleDecoration(radius, (lifespan.end, lifespan.end + 250), Colors.LightPurple, 0.2, positionConnector);
                        replay.Decorations.AddWithGrowing(circleIndicator, lifespan.start);
                        replay.Decorations.Add(lightningIndicator);
                    }
                }

                // Hydro Burst - Whirlpools
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidSaltsprayDragonHydroBurstWhirlpools, out var hydroBurstWhirlpools))
                {
                    uint radius = 90;
                    int counter = 1;
                    foreach (EffectEvent effect in hydroBurstWhirlpools)
                    {
                        lifespan = effect.ComputeLifespan(log, 3000);
                        var positionConnector = new PositionConnector(effect.Position);
                        var circleIndicator = new CircleDecoration(radius, (lifespan.start - 2000, lifespan.start), Colors.LightOrange, 0.2, positionConnector);
                        var circleWhirlpool = new CircleDecoration(radius, lifespan, Colors.LightBlue, 0.2, positionConnector);
                        replay.Decorations.AddWithGrowing(circleIndicator, lifespan.start);
                        replay.Decorations.Add(circleWhirlpool);
                        // The whirlpools increase in size every set of 3, find if there is a next effect within 500ms.
                        EffectEvent? nextWhirlpool = hydroBurstWhirlpools.FirstOrDefault(x => Math.Abs(x.Time - effect.Time) < 500 && x.Time > effect.Time);
                        radius = counter % 3 == 0 ? radius + 10 : radius;
                        // if there isn't a next one, reset the radius to the starting value
                        if (nextWhirlpool == null)
                        {
                            radius = 90;
                        }
                        counter++;
                    }
                }

                // Frozen Fury - Cone Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidSaltsprayDragonFrozenFuryCone, out var frozenFuryCone))
                {
                    foreach (EffectEvent effect in frozenFuryCone)
                    {
                        lifespan = effect.ComputeLifespan(log, 1350);
                        var positionConnector = new PositionConnector(effect.Position);
                        var rotationConnector = new AngleConnector(effect.Rotation.Z + 90);
                        var cone = (PieDecoration)new PieDecoration(710, 60, lifespan, Colors.LightOrange, 0.2, positionConnector).UsingRotationConnector(rotationConnector);
                        replay.Decorations.AddWithGrowing(cone, lifespan.end);
                    }
                }

                // Frozen Fury - Rectangle Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidSaltsprayDragonFrozenFuryRectangle, out var frozenFuryRectangles))
                {
                    foreach (EffectEvent effect in frozenFuryRectangles)
                    {
                        lifespan = effect.ComputeLifespan(log, 1600);
                        var positionConnector = new PositionConnector(effect.Position);
                        var rotationConnector = new AngleConnector(effect.Rotation.Z + 90);
                        var rectangle = (RectangleDecoration)new RectangleDecoration(200, 800, lifespan, Colors.LightOrange, 0.2, positionConnector).UsingRotationConnector(rotationConnector);
                        replay.Decorations.AddWithGrowing(rectangle, lifespan.end);
                    }
                }

                // Rolling Flames
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidSaltsprayDragonRollingFlames, out var rollingFlames))
                {
                    foreach (EffectEvent effect in rollingFlames)
                    {
                        lifespan = effect.ComputeLifespan(log, 1700);
                        var positionConnector = new PositionConnector(effect.Position);
                        var circle = new CircleDecoration(300, lifespan, Colors.LightOrange, 0.2, positionConnector);
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }

                // Shatter Earth
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidSaltsprayDragonShatterEarth, out var shatterEarth))
                {
                    int counter = 0;
                    foreach (EffectEvent effect in shatterEarth)
                    {
                        lifespan = effect.ComputeLifespan(log, 1550);
                        var positionConnector = new PositionConnector(effect.Position);
                        uint innerRadius = 0;
                        uint outerRadius = 0;
                        switch (counter)
                        {
                            case 0:
                                innerRadius = 180;
                                outerRadius = 300;
                                var circle = new CircleDecoration(140, lifespan, Colors.LightOrange, 0.2, positionConnector);
                                replay.Decorations.AddWithGrowing(circle, lifespan.end);
                                break;
                            case 1:
                            case 2:
                            case 3:
                                innerRadius = 120;
                                outerRadius = 200;
                                break;
                            case 4:
                            case 5:
                            case 6:
                                innerRadius = 100;
                                outerRadius = 140;
                                break;
                            default:
                                break;
                        }
                        var doughnut = new DoughnutDecoration(innerRadius, outerRadius, lifespan, Colors.LightOrange, 0.2, positionConnector);
                        replay.Decorations.AddWithGrowing(doughnut, lifespan.end);
                        counter = (counter + 1) % 7;
                    }
                }
                break;
            case (int)TargetID.VoidAbomination:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Abomination Swipe - Launch
                        case AbominationSwipe:
                            castDuration = 2368;
                            long supposedEndCast = cast.Time + castDuration;
                            long actualEndCast = cast.IsInterrupted && cast.EndTime < cast.Time + castDuration ? cast.EndTime : supposedEndCast;
                            lifespan = (cast.Time, actualEndCast);
                            var cone = new PieDecoration(300, 40, lifespan, Colors.Orange, 0.2, new AgentConnector(target));
                            replay.Decorations.AddWithGrowing(cone, supposedEndCast);
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.VoidObliterator:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Charge - Indicator
                        case VoidObliteratorChargeWindup:
                            {
                                uint length = 2000;
                                uint width = target.HitboxWidth;
                                castDuration = 1000;
                                long supposedEndCast = cast.Time + castDuration;
                                long actualEndCast = ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration);
                                if (target.TryGetCurrentFacingDirection(log, cast.Time + castDuration, out var facing))
                                {
                                    lifespan = (cast.Time, actualEndCast);
                                    var agentConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(length / 2, 0, 0), true);
                                    var rectangle = (RectangleDecoration)new RectangleDecoration(length, width, lifespan, Colors.Orange, 0.2, agentConnector).UsingRotationConnector(new AngleConnector(facing));
                                    replay.Decorations.AddWithGrowing(rectangle, supposedEndCast);
                                }
                            }
                            break;
                        // Wyvern Breath - Indicator
                        case VoidObliteratorWyvernBreathSkill:
                            {
                                uint radius = 750;
                                int openingAngle = 60;
                                castDuration = 3400;
                                long supposedEndCast = cast.Time + castDuration;
                                long actualEndCast = ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration);
                                if (target.TryGetCurrentFacingDirection(log, cast.Time + castDuration, out var facing))
                                {
                                    lifespan = (cast.Time, actualEndCast);
                                    var agentConnector = new AgentConnector(target);
                                    var rotation = new AngleConnector(facing);
                                    var warningCone = (PieDecoration)new PieDecoration(radius, openingAngle, lifespan, Colors.Orange, 0.2, agentConnector).UsingRotationConnector(new AngleConnector(facing));
                                    replay.Decorations.AddWithGrowing(warningCone, supposedEndCast);
                                    // Manually adding a fire decoration for old logs
                                    if (!log.CombatData.HasEffectData)
                                    {
                                        int fireDuration = 30000;
                                        (long start, long end) lifespanFire = (lifespan.end, lifespan.end + fireDuration);
                                        var fireCone = (PieDecoration)new PieDecoration(radius, openingAngle, lifespanFire, Colors.Yellow, 0.2, agentConnector).UsingRotationConnector(rotation);
                                        replay.Decorations.Add(fireCone);
                                    }
                                }
                            }
                            break;
                        // Firebomb
                        case VoidObliteratorFirebomb:
                            {
                                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidObliteratorFirebomb, out var firebombAoEs))
                                {
                                    castDuration = 1500;
                                    EffectEvent? bombAoE = firebombAoEs.FirstOrDefault(x => x.Time > cast.Time && x.Time < cast.Time + castDuration);
                                    if (bombAoE != null && target.TryGetCurrentPosition(log, cast.Time, out var obliteratorPosition))
                                    {
                                        // Shooting animation
                                        if (!log.CombatData.HasMissileData)
                                        {
                                            long animationDuration = bombAoE.Time - cast.Time;
                                            lifespan = (cast.Time, cast.Time + animationDuration);
                                            replay.Decorations.AddProjectile(obliteratorPosition, bombAoE.Position, lifespan, Colors.Red);
                                        }

                                        // Landed Firebomb
                                        (long start, long end) lifespanAoE = bombAoE.ComputeLifespan(log, 21000);
                                        var fireCircle = new CircleDecoration(120, lifespanAoE, Colors.Yellow, 0.2, new PositionConnector(bombAoE.Position));
                                        replay.Decorations.Add(fireCircle);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                // Firebomb Missile
                var firebombMissile = log.CombatData.GetMissileEventsBySkillID(VoidObliteratorFirebomb);
                replay.Decorations.AddNonHomingMissiles(log, firebombMissile, Colors.Red, 0.5, 50);

                // Wyvern Breath - Small fire AoEs
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidObliteratorWyvernBreathFire, out var wyvernBreahFires))
                {
                    foreach (EffectEvent effect in wyvernBreahFires)
                    {
                        lifespan = effect.ComputeLifespan(log, 30000);
                        var circle = new CircleDecoration(80, lifespan, Colors.Yellow, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.Add(circle);
                    }
                }

                // Claw Shockwave
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidObliteratorShockwave, out var clawShockwave))
                {
                    foreach (EffectEvent effect in clawShockwave)
                    {
                        uint radius = 1000; // Assumed radius
                        lifespan = effect.ComputeLifespan(log, 2500); // Assumed duration
                        GeographicalConnector connector = new PositionConnector(effect.Position);
                        replay.Decorations.AddShockwave(connector, lifespan, Colors.LightGrey, 0.6, radius);
                    }
                }
                break;
            case (int)TargetID.VoidGoliath:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Glacial Slam - Cast Indicator
                        case GlacialSlam:
                            castDuration = 1880;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            var circle = new CircleDecoration(600, lifespan, Colors.Orange, 0.2, new AgentConnector(target));
                            replay.Decorations.AddWithGrowing(circle, growing);
                            break;
                        default:
                            break;
                    }
                }

                // Glacial Slam - AoE
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidGoliathGlacialSlam, out var glacialSlamsAoE))
                {
                    foreach (EffectEvent effect in glacialSlamsAoE)
                    {
                        lifespan = effect.ComputeLifespan(log, 5000);
                        var circle = new CircleDecoration(600, lifespan, Colors.Ice, 0.4, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithBorder(circle, Colors.Red, 0.5);
                    }
                }
                break;
            default:
                break;
        }
    }

    private SingleActor? FindActiveOrNextDragonVoid(long time)
    {
        var dragonVoidIDs = new List<int> {
            (int)TargetID.TheDragonVoidJormag,
            (int)TargetID.TheDragonVoidPrimordus,
            (int)TargetID.TheDragonVoidKralkatorrik,
            (int)TargetID.TheDragonVoidMordremoth,
            (int)TargetID.TheDragonVoidZhaitan,
            (int)TargetID.TheDragonVoidSooWon,
        };
        SingleActor? activeDragon = FirstAwareSortedTargets.FirstOrDefault(x => x.FirstAware <= time && x.LastAware >= time && dragonVoidIDs.Contains(x.ID));
        return activeDragon ?? FirstAwareSortedTargets.FirstOrDefault(x => x.FirstAware >= time);
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.HarvestTempleTargetedExpulsionSpreadCM, out var spreadEffectsCM))
        {
            AddSpreadSelectionDecoration(p, log, replay, spreadEffectsCM, 300);
        }
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.HarvestTempleTargetedExpulsionSpreadNM, out var spreadEffectsNM))
        {
            AddSpreadSelectionDecoration(p, log, replay, spreadEffectsNM, 240);
        }
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.HarvestTempleVoidPoolRedPuddleSelectionCM, out var redSelectedEffectsCM))
        {
            AddVoidPoolSelectionDecoration(p, log, replay, redSelectedEffectsCM, 400);
        }
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.HarvestTempleVoidPoolRedPuddleSelectionNM, out var redSelectedEffectsNM))
        {
            AddVoidPoolSelectionDecoration(p, log, replay, redSelectedEffectsNM, 300);
        }
    }

    /// <summary>
    /// Void Pools - Red Pool AoE on selected players.<br></br>
    /// Duration logged of 7936, overriding to 7000 for logs without Dynamic End Event.
    /// </summary>
    /// <remarks>As of EVTC20241030 Effects on players have Dynamic End Time.</remarks>
    /// <param name="p">Selected player.</param>
    /// <param name="log">The log.</param>
    /// <param name="replay">Combat Replay.</param>
    /// <param name="redSelectedEffects">Effects List.</param>
    /// <param name="radius">Radius of the AoE.</param>
    private void AddVoidPoolSelectionDecoration(PlayerActor p, ParsedEvtcLog log, CombatReplay replay, IReadOnlyList<EffectEvent> redSelectedEffects, uint radius)
    {
        foreach (EffectEvent effect in redSelectedEffects)
        {
            long duration = 7000;
            (long start, long end) lifespan = effect.HasDynamicEndTime ? effect.ComputeDynamicLifespan(log, 7936) : (effect.Time, effect.Time + duration);
            long growing = lifespan.start + duration;
            SingleActor? dragonVoid = FindActiveOrNextDragonVoid(effect.Time);
            if (dragonVoid == null)
            {
                continue;
            }
            lifespan.end = Math.Min((int)dragonVoid.LastAware, lifespan.end);
            replay.Decorations.AddWithGrowing(new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new AgentConnector(p)), growing);
        }
    }

    /// <summary>
    /// Targeted Expulsion - Spread AoE on players.
    /// </summary>
    /// <remarks>As of EVTC20241030 Effects on players have Dynamic End Time.</remarks>
    /// <param name="p">Selected player.</param>
    /// <param name="log">The log.</param>
    /// <param name="replay">Combat Replay.</param>
    /// <param name="spreadEffects">Effects List.</param>
    /// <param name="radius">Radius of the AoE.</param>
    /// <param name="duration">Duration of the AoE.</param>
    private void AddSpreadSelectionDecoration(PlayerActor p, ParsedEvtcLog log, CombatReplay replay, IReadOnlyList<EffectEvent> spreadEffects, uint radius)
    {
        foreach (EffectEvent effect in spreadEffects)
        {
            long duration = 5000;
            (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
            long growing = lifespan.start + duration;
            SingleActor? dragonVoid = FindActiveOrNextDragonVoid(effect.Time);
            if (dragonVoid == null)
            {
                continue;
            }
            lifespan.end = Math.Min(dragonVoid.LastAware, lifespan.end);
            replay.Decorations.AddWithGrowing(new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, new AgentConnector(p)), growing);
        }
    }

    /// <summary>
    /// Void Pools - Red Pool AoEs placed.
    /// </summary>
    /// <param name="redPuddleEffects">Effects List.</param>
    /// <param name="radius">Radius of the AoE.</param>
    /// <param name="duration">Duration of the AoE.</param>
    private void AddPlacedVoidPoolDecoration(ParsedEvtcLog log, IReadOnlyList<EffectEvent> redPuddleEffects, uint radius, int duration, CombatReplayDecorationContainer environmentDecorations)
    {
        foreach (EffectEvent effect in redPuddleEffects)
        {
            long inactiveDuration = 1500;
            (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
            long growing = lifespan.start + inactiveDuration;
            SingleActor? dragonVoid = FindActiveOrNextDragonVoid(effect.Time);
            if (dragonVoid == null)
            {
                continue;
            }
            lifespan.end = Math.Min((int)dragonVoid.LastAware, lifespan.end);
            environmentDecorations.Add(new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingGrowingEnd(growing));
            environmentDecorations.Add(new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)));
        }
    }

    /// <summary>
    /// Share the Void - Greens in CM, area effect.
    /// </summary>
    /// <param name="greenEffects">Effects List.</param>
    private void AddBaseShareTheVoidDecoration(ParsedEvtcLog log, IReadOnlyList<EffectEvent> greenEffects, CombatReplayDecorationContainer environmentDecorations)
    {
        foreach (EffectEvent green in greenEffects)
        {
            long duration = 6250;
            (long start, long end) lifespan = green.ComputeLifespan(log, duration);
            long growing = green.Time + duration;
            SingleActor? dragonVoid = FindActiveOrNextDragonVoid(green.Time);
            if (dragonVoid == null)
            {
                continue;
            }
            lifespan.end = Math.Min((int)dragonVoid.LastAware, lifespan.end);
            environmentDecorations.Add(new CircleDecoration(180, lifespan, Colors.DarkGreen, 0.4, new PositionConnector(green.Position)));
            environmentDecorations.Add(new CircleDecoration(180, lifespan, Colors.DarkGreen, 0.4, new PositionConnector(green.Position)).UsingGrowingEnd(growing));
        }
    }

    /// <summary>
    /// Share the Void - Greens in CM, trigger effect.
    /// </summary>
    /// <param name="greenEffects">Effects List.</param>
    /// <param name="isSuccessful">Wether the mechanic was successful or not.</param>
    private static void AddResultShareTheVoidDecoration(IReadOnlyList<EffectEvent> greenEffects, bool isSuccessful, CombatReplayDecorationContainer environmentDecorations)
    {
        foreach (EffectEvent green in greenEffects)
        {
            (long start, long end) lifespan = (green.Time - 250, green.Time);
            Color color = isSuccessful ? Colors.DarkGreen : Colors.DarkRed;
            environmentDecorations.Add(new CircleDecoration(180, lifespan, color, 0.6, new PositionConnector(green.Position)));
        }
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        var targetIDs = new HashSet<int>()
            {
                (int)TargetID.TheDragonVoidJormag,
                (int)TargetID.TheDragonVoidKralkatorrik,
                (int)TargetID.TheDragonVoidMordremoth,
                (int)TargetID.TheDragonVoidPrimordus,
                (int)TargetID.TheDragonVoidZhaitan,
            };
        if (Targets.Where(x => targetIDs.Contains(x.ID)).Any(x => x.GetHealth(combatData) > 16000000))
        {
            return FightData.EncounterMode.CM;
        }
        IReadOnlyList<AgentItem> voidMelters = agentData.GetNPCsByID(TargetID.VoidMelter);
        if (voidMelters.Count > 5)
        {
            long firstAware = voidMelters[0].FirstAware;
            if (voidMelters.Count(x => Math.Abs(x.FirstAware - firstAware) < ServerDelayConstant) > 5)
            {
                return FightData.EncounterMode.CM;
            }
        }
        // fallback for late logs
        if (combatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTempleSuccessGreen).IsValid || agentData.GetNPCsByID(TargetID.VoidGoliath).Any() || combatData.GetBuffData(VoidEmpowerment).Any())
        {
            return FightData.EncounterMode.CM;
        }
        return FightData.EncounterMode.Normal;
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        // Added a CM mode check because the eligibility had been bugged for some time and showed up in normal mode.
        if (log.FightData.Success && log.FightData.IsCM)
        {
            if (log.CombatData.GetBuffData(AchievementEligibilityVoidwalker).Any())
            {
                InstanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityVoidwalker));
            }
            else if (CustomCheckVoidwalkerEligibility(log)) // In case all 10 players already have voidwalker
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIDs[AchievementEligibilityVoidwalker], 1));
            }
        }
    }

    private static bool CustomCheckVoidwalkerEligibility(ParsedEvtcLog log)
    {
        IReadOnlyList<AgentItem> orbs = log.AgentData.GetNPCsByID((int)TargetID.PushableVoidAmalgamate);

        foreach (AgentItem orb in orbs)
        {
            IReadOnlyDictionary<long, BuffGraph> bgms = log.FindActor(orb).GetBuffGraphs(log);
            if (bgms != null && bgms.TryGetValue(VoidEmpowerment, out var bgm))
            {
                if (bgm.Values.Any(x => x.Value >= 3))
                {
                    return false;
                }
            }
        }
        return true;
    }

}
