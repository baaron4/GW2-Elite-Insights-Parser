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

    private static readonly Vector3 GrandStrikeChestPosition = new(605.31f, -20400.5f, -15420.1f);
    private IEnumerable<SingleActor> FirstAwareSortedTargets = [];
    public HarvestTemple(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            // General
            new MechanicGroup([
                new PlayerDstEffectMechanic([EffectGUIDs.HarvestTempleTargetedExpulsionSpreadNM, EffectGUIDs.HarvestTempleTargetedExpulsionSpreadCM], "Spread Bait", new MechanicPlotlySetting(Symbols.Circle, Colors.Yellow), "Spread.B", "Baited spread mechanic", "Spread Bait", 150),
                new PlayerDstEffectMechanic([EffectGUIDs.HarvestTempleVoidPoolRedPuddleSelectionNM, EffectGUIDs.HarvestTempleVoidPoolRedPuddleSelectionCM], "Red Bait", new MechanicPlotlySetting(Symbols.Circle, Colors.Red), "Red.B", "Baited red puddle mechanic", "Red Bait", 150),
                new PlayerDstBuffApplyMechanic(InfluenceOfTheVoidBuff, "Influence of the Void", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkPurple), "Void.D", "Received Void debuff", "Void Debuff", 150),
                new PlayerDstHitMechanic(InfluenceOfTheVoidSkill, "Influence of the Void Hit", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.DarkPurple), "Void.H", "Hit by Void", "Void Hit", 150),
                new PlayerDstHitMechanic([VoidPoolNM, VoidPoolCM], "Void Pool", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkPurple), "Red.H", "Hit by Red Void Pool", "Void Pool", 150),
                new PlayerDstSkillMechanic([HarvestTempleTargetedExpulsionNM, HarvestTempleTargetedExpulsionCM], "Targeted Expulsion", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Orange), "Spread.H", "Hit by Targeted Expulsion (Spread)", "Targeted Expulsion (Spread)", 150)
                    .UsingChecker((@event, log) => @event.HasHit || @event.DoubleProcHit),
                new PlayerSrcAllHitsMechanic("Orb Push", new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightOrange), "Orb Push", "Orb was pushed by player", "Orb Push", 0)
                    .UsingChecker((de, log) => (de.To.IsSpecies(TargetID.PushableVoidAmalgamate) || de.To.IsSpecies(TargetID.KillableVoidAmalgamate)) && de is DirectHealthDamageEvent),
                new PlayerDstHitMechanic([Shockwave, TsunamiSlam1, TsunamiSlam2], "Shockwaves", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Yellow), "NopeRopes.Achiv", "Achievement Elibigility: Jumping the Nope Ropes", "Achiv Jumping Nope Ropes", 150)
                    .UsingAchievementEligibility(true),
                new PlayerDstHitMechanic([VoidExplosion, VoidExplosion2, VoidExplosion3], "Void Explosion", new MechanicPlotlySetting(Symbols.StarSquareOpenDot, Colors.Yellow), "VoidExp.H", "Hit by Void Explosion (Last Laugh)", "Void Explosion", 0),
                new PlayerDstHitMechanic(MagicDischarge, "Magic Discharge", new MechanicPlotlySetting(Symbols.Octagon, Colors.Grey), "MagicDisc.H", "Hit by Magic Discharge (Orb Explosion Wave)", "Magic Discharge", 0),
                new MechanicGroup([
                    new EnemySrcEffectMechanic(EffectGUIDs.HarvestTempleSuccessGreen, "Success Green", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkGreen), "S.Green", "Green Successful", "Success Green", 0),
                    new EnemySrcEffectMechanic(EffectGUIDs.HarvestTempleFailedGreen, "Failed Green", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "F.Green", "Green Failed", "Failed Green", 0),
                ]),
            ]),
            // Purification 1
            new MechanicGroup([
                new PlayerDstHitMechanic(LightningOfJormag, "Lightning of Jormag", new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.Ice), "Light.H", "Hit by Lightning of Jormag", "Lightning of Jormag", 0),
                new PlayerDstHitMechanic(FlamesOfPrimordus, "Flames of Primordus", new MechanicPlotlySetting(Symbols.StarTriangleDownOpen, Colors.Orange), "Flame.H", "Hit by Flames of Primordus", "Flames of Primordus", 0),
                new PlayerDstHitMechanic(Stormfall, "Stormfall", new MechanicPlotlySetting(Symbols.YUpOpen, Colors.Purple), "Storm.H", "Hit by Kralkatorrik's Stormfall", "Kralkatorrik's Stormfall", 0),
            ]),
            // Jormag
            new MechanicGroup([
                new PlayerDstHitMechanic([BreathOfJormag1, BreathOfJormag2, BreathOfJormag3], "Breath of Jormag", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Blue), "J.Breath.H", "Hit by Jormag Breath", "Jormag Breath", 150),
                new PlayerDstHitMechanic(GraspOfJormag, "Grasp of Jormag", new MechanicPlotlySetting(Symbols.StarOpen, Colors.DarkWhite), "J.Grasp.H", "Hit by Grasp of Jormag", "Grasp of Jormag", 0),
                new PlayerDstHitMechanic(FrostMeteor, "Frost Meteor", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Blue), "J.Meteor.H", "Hit by Jormag Meteor", "Jormag Meteor", 150),
            ]),
            // Primordus
            new MechanicGroup([
                new PlayerDstHitMechanic(LavaSlam, "Lava Slam", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Red), "Slam.H", "Hit by Primordus Slam", "Primordus Slam", 150),
                new PlayerDstHitMechanic(JawsOfDestruction, "Jaws of Destruction", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Jaws.H", "Hit by Primordus Jaws", "Primordus Jaws", 150),
            ]),
            // Kralkatorrik 
            new MechanicGroup([
                new PlayerDstHitMechanic(CrystalBarrage, "Crystal Barrage", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Purple), "Barrage.H", "Hit by Crystal Barrage", "Barrage", 150),
                new PlayerDstHitMechanic(BrandingBeam, "Branding Beam", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Purple), "Beam.H", "Hit by Kralkatorrik's Branding Beam", "Kralkatorrik Beam", 150),
                new PlayerDstHitMechanic(BrandedArtillery, "Branded Artillery", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Purple), "Artillery.H", "Hit by Brandbomber Artillery", "Brandbomber Artillery", 150),
                new PlayerDstHitMechanic(VoidPoolKralkatorrik, "Void Pool", new MechanicPlotlySetting(Symbols.Circle, Colors.Black), "K.Pool.H", "Hit by Kralkatorrik Void Pool", "Kralkatorrik Void Pool", 150),
            ]),
            // Purification 2
            new MechanicGroup([
                new PlayerDstHitMechanic(SwarmOfMordremoth_PoolOfUndeath, "Pool of Undeath", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Goop.H", "Hit by goop left by heart", "Heart Goop", 150),
                new PlayerDstHitMechanic(SwarmOfMordremoth, "Swarm of Mordremoth", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Red), "Bees.H", "Hit by bees from heart", "Heart Bees", 150),
                // Timecaster
                new MechanicGroup([
                    new PlayerDstHitMechanic(GravityCrushDamage, "Gravity Crush", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Black), "Grav.Cru.H", "Hit by Gravity Crush", "Gravity Crush", 0),
                    new PlayerDstHitMechanic(NightmareEpochDamage, "Nightmare Epoch", new MechanicPlotlySetting(Symbols.Hexagon, Colors.Pink), "NigEpoch.H", "Hit by Nightmare Epoch", "Nightmare Epoch", 0),
                ]),
            ]),
            // Mordremoth
            new MechanicGroup([
                new PlayerDstHitMechanic(Shockwave, "Shock Wave", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Green), "ShckWv.H", "Hit by Mordremoth Shockwave", "Mordremoth Shockwave", 150),
                new PlayerDstHitMechanic(Kick, "Kick", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "Kick.H", "Kicked by Void Skullpiercer", "Skullpiercer Kick", 150).UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
                new PlayerDstHitMechanic(PoisonRoar, "Poison Roar", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Green), "M.Poison.H", "Hit by Mordremoth Poison", "Mordremoth Poison", 150),
            ]),
            // Giants
            new MechanicGroup([
                new PlayerDstHitMechanic(DeathScream, "Death Scream", new MechanicPlotlySetting(Symbols.SquareOpen, Colors.Grey), "Scream.G.CC", "CC'd by Giant's Death Scream", "Death Scream", 0)
                    .UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
                new PlayerDstHitMechanic(RottingBile, "Rotting Bile", new MechanicPlotlySetting(Symbols.Square, Colors.GreenishYellow), "RotBile.H", "Hit by Giant's Rotting Bile", "Rotting Bile", 0),
                new PlayerDstHitMechanic(Stomp, "Stomp", new MechanicPlotlySetting(Symbols.StarSquare, Colors.Teal), "Stomp.CC", "CC'd by Giant's Stomp", "Stomp", 0)
                    .UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
            ]),
            // Zhaitan
            new MechanicGroup([
                new PlayerDstHitMechanic([ScreamOfZhaitanNM, ScreamOfZhaitanCM], "Scream of Zhaitan", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkGreen), "Scream.H", "Hit by Zhaitan Scream", "Zhaitan Scream", 150),
                new PlayerDstHitMechanic(PutridDeluge, "Putrid Deluge", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkGreen), "Z.Poison.H", "Hit by Zhaitan Poison", "Zhaitan Poison", 150),
                new PlayerDstHitMechanic(ZhaitanTailSlam, "Slam", new MechanicPlotlySetting(Symbols.Circle, Colors.Grey), "Slam.H", "Hit by Zhaitan's Tail Slam", "Zhaitan Slam", 0),
            ]),
            // Purification 3
            new MechanicGroup([
                new PlayerDstHitMechanic(SwarmOfMordremoth_CorruptedWaters, "Corrupted Waters", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.LightBlue), "Prjtile.H", "Hit by Corrupted Waters (Heart Projectile)", "Heart Projectile", 150),
                // Saltspray
                new MechanicGroup([
                    new PlayerDstHitMechanic(HydroBurst, "Hydro Burst", new MechanicPlotlySetting(Symbols.Circle, Colors.LightBlue), "Whrlpl.H", "Hit by Hydro Burst (Whirlpool)", "Hydro Burst (Whirlpool)", 150),
                    new PlayerDstHitMechanic(CallLightning, "Call Lightning", new MechanicPlotlySetting(Symbols.TriangleDownOpen, Colors.Purple), "CallLigh.H", "Hit by Call Lightning", "Call Lightning", 0),
                    new PlayerDstHitMechanic(FrozenFury, "Frozen Fury", new MechanicPlotlySetting(Symbols.TriangleRightOpen, Colors.Ice), "FrozFury.H", "Hit by Frozen Fury", "Frozen Fury", 0),
                    new PlayerDstHitMechanic(RollingFlame, "Rolling Flame", new MechanicPlotlySetting(Symbols.Circle, Colors.LightRed), "RollFlame.H", "Hit by Rolling Flame", "Rolling Flame", 0),
                    new PlayerDstHitMechanic([ShatterEarth, ShatterEarth2], "Shatter Earth", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Brown), "ShatEarth.H", "Hit by Shatter Earth", "Shatter Earth", 0),
                ]),
            ]),
            // Soo Won
            new MechanicGroup([
                new PlayerDstHitMechanic([TsunamiSlam1, TsunamiSlam2], "Tsunami Slam", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.LightBlue), "Tsunami.H", "Hit by Soo-Won Tsunami", "Soo-Won Tsunami", 150),
                new PlayerDstHitMechanic(ClawSlap, "Claw Slap", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.LightBlue), "Claw.H", "Hit by Soo-Won Claw", "Soo-Won Claw", 150),
                new PlayerDstHitMechanic(VoidPoolSooWon, "Void Pool", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkPink), "SW.Pool.H", "Hit by Soo-Won Void Pool", "Soo-Won Void Pool", 150),
                new PlayerDstHitMechanic(TailSlam, "Tail Slam", new MechanicPlotlySetting(Symbols.Square, Colors.LightBlue), "Tail.H", "Hit by Soo-Won Tail", "Soo-Won Tail", 150),
                new PlayerDstHitMechanic(TormentOfTheVoid, "Torment of the Void", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkMagenta), "Torment.H", "Hit by Torment of the Void (Bouncing Orbs)", "Torment of the Void", 150),
                // Obliterator
                new MechanicGroup([
                    new PlayerDstHitMechanic(VoidObliteratorFirebomb, "Firebomb", new MechanicPlotlySetting(Symbols.TriangleNW, Colors.DarkTeal), "Firebomb.H", "Hit by Firebomb", "Firebomb", 0),
                    new PlayerDstHitMechanic(VoidObliteratorWyvernBreathDamage, "Wyvern Breath", new MechanicPlotlySetting(Symbols.TriangleNEOpen, Colors.Magenta), "WyvBreath.H", "Hit by Wyvern Breath", "Wyvern Breath", 0),
                    new MechanicGroup([
                        new PlayerDstHitMechanic(VoidObliteratorCharge, "Charge", new MechanicPlotlySetting(Symbols.Diamond, Colors.Teal), "Charge.H", "Hit by Obliterator's Charge", "Charge Hit", 0),
                        new PlayerDstHitMechanic(VoidObliteratorCharge, "Charge", new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.Revenant), "Charge.CC", "CC'd by Obliterator's Charge", "Charge CC", 0)
                            .UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
                    ]),
                ]),
                // Goliath
                new MechanicGroup([
                    new PlayerDstHitMechanic(GlacialSlam, "Glacial Slam", new MechanicPlotlySetting(Symbols.CircleX, Colors.Ice), "GlaSlam.H", "Hit by Glacial Slam", "Glacial Slam Hit", 0),
                    new PlayerDstHitMechanic(GlacialSlam, "Glacial Slam", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Ice), "GlaSlam.CC", "CC'd by Glacial Slam", "Glacial Slam CC", 0)
                        .UsingChecker((ahde, log) => !ahde.To.HasBuff(log, Stability, ahde.Time - ServerDelayConstant)),
                ]),
            ]),
            // Purification 4
            new MechanicGroup([
                new PlayerDstHitMechanic(GraspOfTheVoid, "Grasp of the Void", new MechanicPlotlySetting(Symbols.Hexagram, Colors.Black), "GraspVoid.H", "Hit by Grasp of the Void (Final Orb Projectile)", "Grasp of the Void", 0),
            ]),
        ]));
        Icon = EncounterIconHarvestTemple;
        ChestID = ChestID.GrandStrikeChest;
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
        var subPhasesData = new List<(long start, long end, string name, NPC target, bool canBeSubPhase)>();
        var giants = new List<NPC>();
        foreach (NPC target in Targets)
        {
            long phaseEnd = Math.Min(target.LastAware, log.FightData.FightEnd);
            long phaseStart = Math.Max(target.FirstAware, log.FightData.FightStart);
            switch (target.ID)
            {
                case (int)TargetID.KillableVoidAmalgamate:
                    phases[0].AddTarget(target, PhaseData.TargetPriority.Blocking);
                    break;
                case (int)TargetID.VoidGiant:
                    giants.Add(target);
                    break;
                case (int)TargetID.VoidSaltsprayDragon:
                case (int)TargetID.VoidTimeCaster:
                case (int)TargetID.VoidObliterator:
                case (int)TargetID.VoidGoliath:
                    subPhasesData.Add((phaseStart, phaseEnd, target.Character, target, false));
                    break;
                case (int)TargetID.TheDragonVoidJormag:
                    phases[0].AddTarget(target);
                    subPhasesData.Add((phaseStart, phaseEnd, "Jormag", target, true));
                    break;
                case (int)TargetID.TheDragonVoidKralkatorrik:
                    phases[0].AddTarget(target);
                    subPhasesData.Add((phaseStart, phaseEnd, "Kralkatorrik", target, true));
                    break;
                case (int)TargetID.TheDragonVoidMordremoth:
                    phases[0].AddTarget(target);
                    subPhasesData.Add((phaseStart, phaseEnd, "Mordremoth", target, true));
                    break;
                case (int)TargetID.TheDragonVoidPrimordus:
                    phases[0].AddTarget(target);
                    subPhasesData.Add((phaseStart, phaseEnd, "Primordus", target, true));
                    break;
                case (int)TargetID.TheDragonVoidSooWon:
                    phases[0].AddTarget(target);
                    subPhasesData.Add((phaseStart, phaseEnd, "Soo-Won", target, true));
                    AttackTargetEvent? attackTargetEvent = log.CombatData.GetAttackTargetEvents(target.AgentItem).FirstOrDefault();
                    if (attackTargetEvent != null)
                    {
                        var targetables = log.CombatData.GetTargetableEvents(attackTargetEvent.AttackTarget).Where(x => x.Time >= target.FirstAware);
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
                            subPhasesData.Add((start, end, "Soo-Won " + (++id), target, true));
                        }
                    }
                    break;
                case (int)TargetID.TheDragonVoidZhaitan:
                    phases[0].AddTarget(target);
                    subPhasesData.Add((phaseStart, phaseEnd, "Zhaitan", target, true));
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
            foreach (NPC giant in giants)
            {
                start = Math.Min(start, giant.FirstAware);
                end = Math.Max(end, giant.LastAware);
            }
            var subPhase = new PhaseData(start, end, "Giants")
            {
                CanBeSubPhase = false
            };
            subPhase.AddTargets(giants);
            subPhase.OverrideEndTime(log);
            phases.Add(subPhase);
        }
        var subPhaseNonBlockings = Targets.Where(x => x.IsSpecies(TargetID.VoidGoliath) || x.IsSpecies(TargetID.VoidObliterator) || x.IsSpecies(TargetID.VoidGiant));
        foreach ((long start, long end, string name, NPC target, bool canBeSubPhase) in subPhasesData)
        {
            var subPhase = new PhaseData(start, end, name)
            {
                CanBeSubPhase = canBeSubPhase
            };
            subPhase.AddTarget(target);
            subPhase.OverrideEndTime(log);
            subPhase.AddTargets(subPhaseNonBlockings, PhaseData.TargetPriority.NonBlocking);
            phases.Add(subPhase);
        }
        int purificationID = 0;
        var purificationNonBlockings = Targets.Where(x => x.IsSpecies(TargetID.VoidTimeCaster) || x.IsSpecies(TargetID.VoidSaltsprayDragon));
        foreach (NPC voidAmal in Targets.Where(x => x.IsSpecies(TargetID.PushableVoidAmalgamate) || x.IsSpecies(TargetID.KillableVoidAmalgamate)))
        {
            var purificationPhase = new PhaseData(Math.Max(voidAmal.FirstAware, log.FightData.FightStart), voidAmal.LastAware, "Purification " + (++purificationID));
            purificationPhase.AddTarget(voidAmal);
            purificationPhase.OverrideEndTime(log);
            purificationPhase.AddTargets(purificationNonBlockings, PhaseData.TargetPriority.NonBlocking);
            phases.Add(purificationPhase);
        }
        return phases;
    }

    protected override List<TargetID> GetSuccessCheckIDs()
    {
        return [];
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

    protected override ReadOnlySpan<TargetID> GetTargetsIDs()
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

    protected override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.TheDragonVoidJormag, 0 },
            {TargetID.TheDragonVoidKralkatorrik, 0 },
            {TargetID.TheDragonVoidMordremoth, 0 },
            {TargetID.TheDragonVoidPrimordus, 0 },
            {TargetID.TheDragonVoidZhaitan, 0 },
            {TargetID.TheDragonVoidSooWon, 0 },
            {TargetID.PushableVoidAmalgamate, 1 },
            {TargetID.KillableVoidAmalgamate, 1 },
            {TargetID.VoidSaltsprayDragon, 1 },
            {TargetID.VoidObliterator, 1 },
            {TargetID.VoidGoliath, 1 },
            {TargetID.VoidTimeCaster, 1 },
            {TargetID.VoidGiant, 1},
        };
    }

    protected override ReadOnlySpan<TargetID> GetUniqueNPCIDs()
    {
        return [ ];
    }

    protected override List<TargetID> GetTrashMobsIDs()
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
        // no bouny chest detection, the reward is delayed
        SingleActor? soowon = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.TheDragonVoidSooWon));
        if (soowon != null)
        {
            AttackTargetEvent? attackTargetEvent = combatData.GetAttackTargetEvents(soowon.AgentItem).FirstOrDefault();
            if (attackTargetEvent == null)
            {
                return;
            }
            var targetables = combatData.GetTargetableEvents(attackTargetEvent.AttackTarget).Where(x => x.Time >= soowon.FirstAware);
            var targetOffs = targetables.Where(x => !x.Targetable).ToList();
            if (targetOffs.Count == 2)
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
                        fightData.SetSuccess(true, targetOffs[1].Time);
                    }
                }
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadget(ChestID, agentData, combatData, GrandStrikeChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 500 && agentItem.HitboxWidth == 2));
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
        var idsToUse = new List<TargetID> {
            TargetID.TheDragonVoidJormag,
            TargetID.TheDragonVoidPrimordus,
            TargetID.TheDragonVoidKralkatorrik,
            TargetID.TheDragonVoidMordremoth,
            TargetID.TheDragonVoidZhaitan,
            TargetID.TheDragonVoidSooWon,
        };
        var attackTargetEvents = combatData.Where(x => x.IsStateChange == StateChange.AttackTarget).Select(x => new AttackTargetEvent(x, agentData));
        var targetableEvents = combatData.Where(y => y.IsStateChange == StateChange.Targetable).Select(x => new TargetableEvent(x, agentData)).Where(x => x.Src.Type == AgentItem.AgentType.Gadget).GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
        attackTargetEvents = attackTargetEvents.Where(x =>
        {
            AgentItem atAgent = x.AttackTarget;
            if (targetableEvents.TryGetValue(atAgent, out var targetables))
            {
                return targetables.Any(y => y.Targetable);
            }
            return false;
        }).ToList();
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
                RedirectEventsAndCopyPreviousStates(combatData, extensions, agentData, dragonVoid, copyEventsFrom, extra, true,
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
        // Add missing agents
        for (int i = index; i < idsToUse.Count; i++)
        {
            agentData.AddCustomNPCAgent(fightData.FightStart + index, fightData.FightStart + index + 1, "Dragonvoid", Spec.NPC, idsToUse[i], false);
        }
        //
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        //
        int purificationID = 0;
        bool needRedirect = false;
        (HashSet<ulong> jormagDamagingAgents   , SingleActor? jormag   ) = ([], null);
        (HashSet<ulong> primordusDamagingAgents, SingleActor? primordus) = ([], null);
        (HashSet<ulong> kralkDamagingAgents    , SingleActor? kralk    ) = ([], null);
        (HashSet<ulong> mordDamagingAgents     , SingleActor? mord     ) = ([], null);
        (HashSet<ulong> zhaitanDamagingAgents  , SingleActor? zhaitan  ) = ([], null);
        (HashSet<ulong> soowonDamagingAgents   , SingleActor? soowon   ) = ([], null);
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
                        TsunamiSlam1,
                        TsunamiSlam2,
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
                        cbt.OverrideSrcAgent(jormag.AgentItem.Agent);
                    }
                    else if (primordus != null && primordusDamagingAgents.Any(x => cbt.SrcAgent == x && primordus.FirstAware <= cbt.Time && cbt.Time <= primordus.LastAware))
                    {
                        cbt.OverrideSrcAgent(primordus.AgentItem.Agent);
                    }
                    else if (kralk != null && kralkDamagingAgents.Any(x => cbt.SrcAgent == x && kralk.FirstAware <= cbt.Time && cbt.Time <= kralk.LastAware))
                    {
                        cbt.OverrideSrcAgent(kralk.AgentItem.Agent);
                    }
                    else if (mord != null && mordDamagingAgents.Any(x => cbt.SrcAgent == x && mord.FirstAware <= cbt.Time && cbt.Time <= mord.LastAware))
                    {
                        cbt.OverrideSrcAgent(mord.AgentItem.Agent);
                    }
                    else if (zhaitan != null && zhaitanDamagingAgents.Any(x => cbt.SrcAgent == x && zhaitan.FirstAware <= cbt.Time && cbt.Time <= zhaitan.LastAware))
                    {
                        cbt.OverrideSrcAgent(zhaitan.AgentItem.Agent);
                    }
                    else if (soowon != null && soowonDamagingAgents.Any(x => cbt.SrcAgent == x && soowon.FirstAware <= cbt.Time && cbt.Time <= soowon.LastAware))
                    {
                        cbt.OverrideSrcAgent(soowon.AgentItem.Agent);
                    }
                }
            }
        }
        FirstAwareSortedTargets = Targets.OrderBy(x => x.FirstAware);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleGreen, out var greenEffects))
        {
            AddBaseShareTheVoidDecoration(log, greenEffects);
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleSuccessGreen, out var successGreenEffects))
        {
            AddResultShareTheVoidDecoration(successGreenEffects, true);
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleFailedGreen, out var failedGreenEffects))
        {
            AddResultShareTheVoidDecoration(failedGreenEffects, false);
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidPoolRedPuddleAoECM, out var redPuddleEffectsCM))
        {
            AddPlacedVoidPoolDecoration(log, redPuddleEffectsCM, 400, 240000);
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidPoolRedPuddleAoENM, out var redPuddleEffectsNM))
        {
            AddPlacedVoidPoolDecoration(log, redPuddleEffectsNM, 300, 24000);
        }
        // Stormseer Ice Spike
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidStormseerIceSpikeIndicator, out var iceSpikes))
        {
            foreach (EffectEvent effect in iceSpikes)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 1750);
                var circle = new CircleDecoration(150, lifespan, Colors.LightBlue, 0.1, new PositionConnector(effect.Position));
                EnvironmentDecorations.Add(circle);
                EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.end));
            }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd); //TODO(Rennorb) @perf

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
                        int start = (int)purificationZoneEffect.Time;
                        int end = (int)log.FightData.FightEnd;
                        uint radius = 280;
                        if (voidShellRemovalOffset < voidShellRemovals.Count)
                        {
                            end = (int)voidShellRemovals[voidShellRemovalOffset++].Time;
                        }
                        replay.Decorations.Add(new CircleDecoration(radius, (start, end), Colors.White, 0.4, new PositionConnector(purificationZoneEffect.Position)));
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
                        (long start, long end) lifespan = (lightningEffect.Time - duration, lightningEffect.Time);
                        var circle = new CircleDecoration(200, lifespan, Colors.LightBlue, 0.2, new PositionConnector(lightningEffect.Position));
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }
                // Primordus - Flames of Primordus
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationFlamesOfPrimordus, out var flamesOfPrimordus))
                {
                    foreach (EffectEvent fireBallEffect in flamesOfPrimordus.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                    {
                        (long start, long end) lifespanIndicator = (fireBallEffect.Time - 2000, fireBallEffect.Time);
                        (long start, long end) lifespanFlame = fireBallEffect.ComputeLifespan(log, 2300);
                        var circle = new CircleDecoration(200, lifespanIndicator, Colors.LightOrange, 0.2, new PositionConnector(fireBallEffect.Position));
                        target.TryGetCurrentPosition(log, lifespanIndicator.start, out var primordusPos);
                        replay.Decorations.AddProjectile(primordusPos, fireBallEffect.Position, lifespanIndicator, Colors.Yellow, 0.2);
                        replay.Decorations.AddWithGrowing(circle, lifespanIndicator.end);
                        replay.Decorations.Add(new CircleDecoration(200, lifespanFlame, Colors.Red, 0.2, new PositionConnector(fireBallEffect.Position)));
                    }
                }
                // Kralkatorrik - Stormfall (Cracks)
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTemplePurificationStormfall, out var stormfallEffects))
                {
                    foreach (EffectEvent stormfallEffect in stormfallEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                    {
                        (long start, long end) lifespan = stormfallEffect.ComputeLifespan(log, 4560);
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
                        int lifespan = 15000;
                        var finalPosition = new ParametricPoint3D(beeLaunchEffect.Position + (velocity * lifespan / 1000.0f) * new Vector3((float)Math.Cos(beeLaunchEffect.Orientation.Z - Math.PI / 2), (float)Math.Sin(beeLaunchEffect.Orientation.Z - Math.PI / 2), 0), end + lifespan);
                        replay.Decorations.Add(new CircleDecoration(280, (end, end + lifespan), Colors.Red, 0.4, new InterpolationConnector([initialPosition, finalPosition])));
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
                // Magic Discharge - Orb Explosion
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleOrbExplosion, out var orbEffects))
                {
                    foreach (EffectEvent orbEffect in orbEffects.Where(x => x.Time >= target.FirstAware && x.Time <= target.LastAware))
                    {
                        int duration = 3000;
                        (long start, long end) lifespan = (orbEffect.Time, orbEffect.Time + duration);
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
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleJormagFrostMeteorIceField, out var meteorEffects))
                {
                    foreach (EffectEvent effect in meteorEffects)
                    {
                        int indicatorDuration = 1500;
                        int spreadDuration = 3000;
                        int lingerDuration = 9500;
                        int start = (int)effect.Time;
                        int fieldEnd = (int)Math.Min(start + lingerDuration, target.LastAware);
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
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 2000);
                        replay.Decorations.Add(new CircleDecoration(160, lifespan, Colors.LightOrange, 0.1, new PositionConnector(effect.Position)));
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
                    (long start, long end) lifespanBeam = (frostBeamMoveStartVelocity.Time, target.LastAware);
                    replay.Trim(lifespanBeam.start, lifespanBeam.end);
                    var beamAoE = new CircleDecoration(300, lifespanBeam, Colors.LightBlue, 0.1, new AgentConnector(target));
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
                        (long start, long end) lifespan = (effect.Time - duration, effect.Time);
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
                        (long start, long end) lifespanIndicator = (effect.Time, Math.Min(growingEnd, target.LastAware));
                        var indicator = new CircleDecoration(1450, lifespanIndicator, Colors.Orange, 0.2, jawsOfDestructionConnector);
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
                        (long start, long end) lifespan = (effect.Time, Math.Min(growingEnd, target.LastAware));
                        replay.Decorations.AddWithGrowing(new RectangleDecoration(700, 2900, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position)), growingEnd);
                    }
                }
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleKralkatorrikBeamAoe, out var kralkBeamAoeEffects))
                {
                    foreach (EffectEvent effect in kralkBeamAoeEffects)
                    {
                        // Effect duration logged is too long
                        int duration = 6300;
                        (long start, long end) lifespan = (effect.Time, Math.Min(effect.Time + duration, target.LastAware));
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
                                    (long start, long end) lifespan = (indicator.Time, indicator.Time + (impactEffect.Time - indicator.Time));
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
                        (long start, long end) lifespan = lastEffect.ComputeLifespanWithSecondaryEffectNoSrcCheck(log, EffectGUIDs.HarvestTempleVoidPoolOrbGettingReadyToBeDangerous);
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
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleMordremothPoisonRoarImpact, out var mordremothPoisonEffects))
                {
                    foreach (EffectEvent effect in mordremothPoisonEffects)
                    {
                        (long start, long end) lifespan = (effect.Time - 2000, effect.Time);
                        replay.Decorations.AddWithGrowing(new CircleDecoration(200, lifespan, Colors.MilitaryGreen, 0.2, new PositionConnector(effect.Position)), lifespan.end);
                    }
                }
                // Shockwaves
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleMordremothShockwave1, out var shockwaves))
                {
                    foreach (EffectEvent effect in shockwaves)
                    {
                        uint radius = 2000; // Assumed radius
                        (long start, long end) lifespan = (effect.Time, effect.Time + 1600); // Assumed duration, effect has 0
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
                        (long start, long end) lifespan = (effect.Time - 2000, effect.Time);
                        replay.Decorations.AddWithGrowing(new CircleDecoration(200, lifespan, Colors.LightMilitaryGreen, 0.2, new PositionConnector(effect.Position)), lifespan.end);
                    }
                }
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleZhaitanPutridDelugeAoE, out var putridDelugeAoEs))
                {
                    foreach (EffectEvent effect in putridDelugeAoEs)
                    {
                        int fieldDuration = 10000;
                        (long start, long end) lifespan = (effect.Time, effect.Time + fieldDuration);
                        if (lifespan.start < target.LastAware)
                        {
                            lifespan.end = Math.Min(target.LastAware, lifespan.end);
                        }
                        replay.Decorations.AddWithBorder(new CircleDecoration(200, lifespan, Colors.LightMilitaryGreen, 0.2, new PositionConnector(effect.Position)), Colors.Red, 0.4);
                    }
                }
                // Tail Slam
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleZhaitanTailSlamImpact, out var zhaitainTailSlam))
                {
                    foreach (EffectEvent effect in zhaitainTailSlam)
                    {
                        // We use effect time - 3000 because the AoE effect indicator isn't disambiguous, the impact is
                        (long start, long end) lifespan = (effect.Time - 3000, effect.Time);
                        var circle = new CircleDecoration(620, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }
                // Scream of Zhaitan
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleZhaitanScreamIndicator, out var screamOfZhaitan))
                {
                    foreach (EffectEvent effect in screamOfZhaitan)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 3000);
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
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 2300);
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
                            (long start, long end) lifespan = orb.ComputeLifespan(log, 2080);
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
                            replay.Decorations.AddProjectile(orb.Position, aoe.Position, lifespanAoE, Colors.Black, 0.5);
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
                            replay.Decorations.AddProjectile(startingAoE.Position, endingAoE.Position, lifespanAnimation, Colors.Black, 0.5);
                        }
                    }
                }

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
                                replay.Decorations.AddProjectile(tailSlamEffect.Position, orbAoeEffect.Position, lifespanAoE, Colors.Black, 0.5);
                            }
                        }
                    }
                }

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
                                replay.Decorations.AddProjectile(tsunamiSlamEffect.Position, orbAoeEffect.Position, lifespanAoE, Colors.Black, 0.5);
                            }
                        }
                    }
                }

                // Tsunami
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleTsunami1, out var tsunamiEffects))
                {
                    foreach (EffectEvent effect in tsunamiEffects)
                    {
                        // Expanding wave - radius and duration are estimates, can't seem to line up the decoration with actual hits
                        uint radius = 2000; // Assumed radius
                        (long, long) lifespan = (effect.Time, effect.Time + 4500); // Assumed duration
                        GeographicalConnector connector = new PositionConnector(effect.Position);
                        replay.Decorations.AddShockwave(connector, lifespan, Colors.Blue, 0.5, radius);
                    }
                }
                break;
            case (int)TargetID.VoidWarforged1:
            case (int)TargetID.VoidWarforged2:
                #if DEBUG_EFFECTS
                    CombatReplay.DebugEffects(target, log, replay, knownEffectsIDs, target.FirstAware, target.LastAware, true);
                #endif
                break;
            case (int)TargetID.ZhaitansReach:
                // Thrash - Circle that pulls in
                var thrash = casts.Where(x => x.SkillId == ZhaitansReachThrashHT1 || x.SkillId == ZhaitansReachThrashHT2);
                foreach (CastEvent c in thrash)
                {
                    int castTime = 1900;
                    int endTime = (int)c.Time + castTime;
                    replay.Decorations.AddWithGrowing(new DoughnutDecoration(260, 480, (c.Time, endTime), Colors.Orange, 0.2, new AgentConnector(target)), endTime);
                }
                // Ground Slam - AoE that knocks out
                var groundSlam = casts.Where(x => x.SkillId == ZhaitansReachGroundSlam || x.SkillId == ZhaitansReachGroundSlamHT);
                foreach (CastEvent c in groundSlam)
                {
                    int castTime = c.SkillId == ZhaitansReachGroundSlam ? 800 : 2500;
                    uint radius = 400;
                    long endTime = c.Time + castTime;
                    replay.Decorations.AddWithGrowing(new CircleDecoration(radius, (c.Time, endTime), Colors.Orange, 0.2, new AgentConnector(target)), endTime);
                }
                break;
            case (int)TargetID.VoidBrandbomber:
                // Branded Artillery
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidBrandbomberBrandedArtillery, out var brandedArtilleryAoEs))
                {
                    var brandedArtillery = casts.Where(x => x.SkillId == BrandedArtillery);
                    foreach (CastEvent c in brandedArtillery)
                    {
                        int castDuration = 2500;
                        EffectEvent? brandedArtilleryAoE = brandedArtilleryAoEs.FirstOrDefault(x => x.Time > c.Time && x.Time < c.Time + castDuration + 100);
                        if (brandedArtilleryAoE != null && target.TryGetCurrentPosition(log, c.Time, out var brandbomberPosition, 1000))
                        {
                            // Shooting animation
                            long animationDuration = brandedArtilleryAoE.Time - c.Time;
                            (long start, long end) lifespan = (c.Time, c.Time + (animationDuration));

                            // Landing indicator
                            uint radius = 240;
                            var positionConnector = new PositionConnector(brandedArtilleryAoE.Position);
                            var aoeCircle = new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, positionConnector);
                            replay.Decorations.AddWithGrowing(aoeCircle, lifespan.end);
                            // Projective decoration
                            replay.Decorations.AddProjectile(brandbomberPosition, brandedArtilleryAoE.Position, lifespan, Colors.DarkPurple);
                        }
                    }
                }
                break;
            case (int)TargetID.VoidTimeCaster:
                // Gravity Crush - Indicator
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidTimecasterGravityCrushIndicator, out var gravityCrushIndicators))
                {
                    foreach (EffectEvent effect in gravityCrushIndicators)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1600);
                        replay.Decorations.AddContrenticRings(0, 40, lifespan, effect.Position, Colors.Orange);
                    }
                }
                // Nightmare Epoch - AoEs
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidTimecasterNightmareEpoch, out var nightmareEpoch))
                {
                    foreach (EffectEvent effect in nightmareEpoch)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 10000);
                        var positionConnector = new PositionConnector(effect.Position);
                        var circle = new CircleDecoration(100, lifespan, Colors.Black, 0.2, positionConnector);
                        replay.Decorations.AddWithBorder(circle, Colors.LightRed, 0.4);
                    }
                }
                break;
            case (int)TargetID.GravityBall:
                // Setting the first aware to + 1600 due to the duration of the warning effect
                (long start, long end) lifespanBall = (target.FirstAware + 1600, target.LastAware);
                var perimeter = (CircleDecoration)new CircleDecoration(320, 300, lifespanBall, Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false);
                replay.Decorations.Add(perimeter);
                break;
            case (int)TargetID.VoidGiant:
                // Death Scream - Fear
                var deathScreams = casts.Where(x => x.SkillId == DeathScream);
                foreach (CastEvent c in deathScreams)
                {
                    uint radius = 500;
                    long castDuration = 1680;
                    long supposedEndCast = c.Time + castDuration;
                    long actualEndCast = ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration);
                    (long start, long end) lifespan = (c.Time, actualEndCast);
                    var agentConnector = new AgentConnector(c.Caster);
                    var circle = new CircleDecoration(radius, lifespan, Colors.Orange, 0.2, agentConnector);
                    replay.Decorations.AddWithGrowing(circle, supposedEndCast);
                }

                // Rotting Bile - Poison AoE Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidGiantRottingBileIndicator, out var bileIndicators))
                {
                    foreach (EffectEvent effect in bileIndicators)
                    {
                        uint radius = 250;
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1400);
                        var positionConnector = new PositionConnector(effect.Position);
                        var circle = new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, positionConnector);
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }

                // Rotting Bile - Poison AoE Damage
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidGiantRottingBileDamage, out var bileAoEs))
                {
                    foreach (EffectEvent effect in bileAoEs)
                    {
                        uint radius = 250;
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 10000);
                        var positionConnector = new PositionConnector(effect.Position);
                        var circle = new CircleDecoration(radius, lifespan, Colors.DarkGreen, 0.2, positionConnector);
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
                        (long start, long end) lifespan = (effect.Time - 2000, effect.Time);
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
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 3000);
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
                        uint radius = 710;
                        int angle = 60;
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1350);
                        var positionConnector = new PositionConnector(effect.Position);
                        var rotationConnector = new AngleConnector(effect.Rotation.Z + 90);
                        var cone = (PieDecoration)new PieDecoration(radius, angle, lifespan, Colors.LightOrange, 0.2, positionConnector).UsingRotationConnector(rotationConnector);
                        replay.Decorations.AddWithGrowing(cone, lifespan.end);
                    }
                }

                // Frozen Fury - Rectangle Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidSaltsprayDragonFrozenFuryRectangle, out var frozenFuryRectangles))
                {
                    foreach (EffectEvent effect in frozenFuryRectangles)
                    {
                        uint width = 200;
                        uint height = 800;
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1600);
                        var positionConnector = new PositionConnector(effect.Position);
                        var rotationConnector = new AngleConnector(effect.Rotation.Z + 90);
                        var rectangle = (RectangleDecoration)new RectangleDecoration(width, height, lifespan, Colors.LightOrange, 0.2, positionConnector).UsingRotationConnector(rotationConnector);
                        replay.Decorations.AddWithGrowing(rectangle, lifespan.end);
                    }
                }

                // Rolling Flames
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidSaltsprayDragonRollingFlames, out var rollingFlames))
                {
                    foreach (EffectEvent effect in rollingFlames)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1700);
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
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1550);
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
                // Abomination Swipe - Launch
                var abominationSwipes = casts.Where(x => x.SkillId == AbominationSwipe);
                foreach (CastEvent c in abominationSwipes)
                {
                    uint radius = 300;
                    int angle = 40;
                    long castDuration = 2368;
                    long supposedEndCast = c.Time + castDuration;
                    long actualEndCast = c.IsInterrupted && c.EndTime < c.Time + castDuration ? c.EndTime : supposedEndCast;
                    (long start, long end) lifespan = (c.Time, actualEndCast);
                    var agentConnector = new AgentConnector(c.Caster);
                    var cone = new PieDecoration(radius, angle, lifespan, Colors.Orange, 0.2, agentConnector);
                    replay.Decorations.AddWithGrowing(cone, supposedEndCast);
                }
                break;
            case (int)TargetID.VoidObliterator:
                // Charge - Indicator
                var charges = casts.Where(x => x.SkillId == VoidObliteratorChargeWindup);
                foreach (CastEvent c in charges)
                {
                    uint length = 2000;
                    uint width = target.HitboxWidth;
                    long castDuration = 1000;
                    long supposedEndCast = c.Time + castDuration;
                    long actualEndCast = ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration);
                    if (target.TryGetCurrentFacingDirection(log, c.Time + castDuration, out var facing))
                    {
                        (long start, long end) lifespan = (c.Time, actualEndCast);
                        var agentConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(length / 2, 0, 0), true);
                        var rotation = new AngleConnector(facing);
                        var rectangle = (RectangleDecoration)new RectangleDecoration(length, width, lifespan, Colors.Orange, 0.2, agentConnector).UsingRotationConnector(rotation);
                        replay.Decorations.AddWithGrowing(rectangle, supposedEndCast);
                    }
                }

                // Wyvern Breath - Indicator
                var wyvernBreaths = casts.Where(x => x.SkillId == VoidObliteratorWyvernBreathSkill);
                foreach (CastEvent c in wyvernBreaths)
                {
                    uint radius = 750;
                    int openingAngle = 60;
                    long castDuration = 3400;
                    long supposedEndCast = c.Time + castDuration;
                    long actualEndCast = ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration);
                    if (target.TryGetCurrentFacingDirection(log, c.Time + castDuration, out var facing))
                    {
                        (long start, long end) lifespan = (c.Time, actualEndCast);
                        var agentConnector = new AgentConnector(target);
                        var rotation = new AngleConnector(facing);
                        var warningCone = (PieDecoration)new PieDecoration(radius, openingAngle, lifespan, Colors.Orange, 0.2, agentConnector).UsingRotationConnector(rotation);
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

                // Wyvern Breath - Small fire AoEs
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidObliteratorWyvernBreathFire, out var wyvernBreahFires))
                {
                    foreach (EffectEvent effect in wyvernBreahFires)
                    {
                        uint radius = 80;
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 30000);
                        var positionConnector = new PositionConnector(effect.Position);
                        var circle = new CircleDecoration(radius, lifespan, Colors.Yellow, 0.2, positionConnector);
                        replay.Decorations.Add(circle);
                    }
                }

                // Claw Shockwave
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarvestTempleVoidObliteratorShockwave, out var clawShockwave))
                {
                    foreach (EffectEvent effect in clawShockwave)
                    {
                        uint radius = 1000; // Assumed radius
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 2500); // Assumed duration
                        var positionConnector = new PositionConnector(effect.Position);
                        GeographicalConnector connector = new PositionConnector(effect.Position);
                        replay.Decorations.AddShockwave(connector, lifespan, Colors.LightGrey, 0.6, radius);
                    }
                }

                // Firebomb
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidObliteratorFirebomb, out var firebombAoEs))
                {
                    var firebombs = casts.Where(x => x.SkillId == VoidObliteratorFirebomb);
                    foreach (CastEvent c in firebombs)
                    {
                        long castDuration = 1500;
                        EffectEvent? bombAoE = firebombAoEs.FirstOrDefault(x => x.Time > c.Time && x.Time < c.Time + castDuration);
                        if (bombAoE != null && target.TryGetCurrentPosition(log, c.Time, out var obliteratorPosition))
                        {
                            // Shooting animation
                            long animationDuration = bombAoE.Time - c.Time;
                            (long start, long end) lifespan = (c.Time, c.Time + animationDuration);
                            replay.Decorations.AddProjectile(obliteratorPosition, bombAoE.Position, lifespan, Colors.Red);

                            // Landed Firebomb
                            uint radius = 120;
                            (long start, long end) lifespanAoE = bombAoE.ComputeLifespan(log, 21000);
                            var positionConnector = new PositionConnector(bombAoE.Position);
                            var fireCircle = new CircleDecoration(radius, lifespanAoE, Colors.Yellow, 0.2, positionConnector);
                            replay.Decorations.Add(fireCircle);
                        }
                    }
                }
                break;
            case (int)TargetID.VoidGoliath:
                // Glacial Slam - Cast Indicator
                var glacialSlams = casts.Where(x => x.SkillId == GlacialSlam);
                foreach (CastEvent c in glacialSlams)
                {
                    uint radius = 600;
                    long castDuration = 1880;
                    long supposedEndCast = c.Time + castDuration;
                    long actualEndCast = ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration);
                    (long start, long end) lifespan = (c.Time, actualEndCast);
                    var agentConnector = new AgentConnector(c.Caster);
                    var circle = new CircleDecoration(radius, lifespan, Colors.Orange, 0.2, agentConnector);
                    replay.Decorations.AddWithGrowing(circle, supposedEndCast);
                }

                // Glacial Slam - AoE
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.HarvestTempleVoidGoliathGlacialSlam, out var glacialSlamsAoE))
                {
                    foreach (EffectEvent effect in glacialSlamsAoE)
                    {
                        uint radius = 600;
                        int duration = 5000;
                        (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
                        var positionConnector = new PositionConnector(effect.Position);
                        var circle = new CircleDecoration(radius, lifespan, Colors.Ice, 0.4, positionConnector);
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
    private void AddPlacedVoidPoolDecoration(ParsedEvtcLog log, IReadOnlyList<EffectEvent> redPuddleEffects, uint radius, int duration)
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
            EnvironmentDecorations.Add(new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingGrowingEnd(growing));
            EnvironmentDecorations.Add(new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)));
        }
    }

    /// <summary>
    /// Share the Void - Greens in CM, area effect.
    /// </summary>
    /// <param name="greenEffects">Effects List.</param>
    private void AddBaseShareTheVoidDecoration(ParsedEvtcLog log, IReadOnlyList<EffectEvent> greenEffects)
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
            EnvironmentDecorations.Add(new CircleDecoration(180, lifespan, Colors.DarkGreen, 0.4, new PositionConnector(green.Position)));
            EnvironmentDecorations.Add(new CircleDecoration(180, lifespan, Colors.DarkGreen, 0.4, new PositionConnector(green.Position)).UsingGrowingEnd(growing));
        }
    }

    /// <summary>
    /// Share the Void - Greens in CM, trigger effect.
    /// </summary>
    /// <param name="greenEffects">Effects List.</param>
    /// <param name="isSuccessful">Wether the mechanic was successful or not.</param>
    private void AddResultShareTheVoidDecoration(IReadOnlyList<EffectEvent> greenEffects, bool isSuccessful)
    {
        foreach (EffectEvent green in greenEffects)
        {
            (long start, long end) lifespan = (green.Time - 250, green.Time);
            Color color = isSuccessful ? Colors.DarkGreen : Colors.DarkRed;
            EnvironmentDecorations.Add(new CircleDecoration(180, lifespan, color, 0.6, new PositionConnector(green.Position)));
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
        if (combatData.GetEffectGUIDEvent(EffectGUIDs.HarvestTempleSuccessGreen) != null || agentData.GetNPCsByID(TargetID.VoidGoliath).Any() || combatData.GetBuffData(VoidEmpowerment).Any())
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
                InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityVoidwalker], 1));
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
