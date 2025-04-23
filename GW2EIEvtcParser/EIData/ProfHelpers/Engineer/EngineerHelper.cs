using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class EngineerHelper
{
    private class EngineerKitFinder : WeaponSwapCastFinder
    {
        public EngineerKitFinder(long skillID) : base(skillID, WeaponSetIDs.KitSet)
        {
            UsingChecker((swap, combatData, agentData, skillData) =>
            {
                SkillItem skill = skillData.Get(skillID);
                if (skill.ApiSkill == null || skill.ApiSkill.BundleSkills == null)
                {
                    return false;
                }
                WeaponSwapEvent? nextSwap = combatData.GetWeaponSwapData(swap.Caster).FirstOrDefault(x => x.Time > swap.Time + ServerDelayConstant);
                long nextSwapTime = nextSwap != null ? nextSwap.Time : long.MaxValue;
                var castIds = new HashSet<long>(combatData.GetAnimatedCastData(swap.Caster).Where(x => x.Time >= swap.Time + WeaponSwapDelayConstant && x.Time <= nextSwapTime).Select(x => x.SkillId));
                return skill.ApiSkill.BundleSkills.Intersect(castIds).Any();
            });
            UsingNotAccurate(true);
        }
    }

    private static readonly HashSet<long> _engineerKit =
    [
        BombKit,
        ElixirGun,
        Flamethrower,
        GrenadeKit,
        MedKitSkill,
        ToolKit,
        EliteMortarKit,
    ];

    public static bool IsEngineerKit(long id)
    {
        return _engineerKit.Contains(id);
    }

    private static bool MineDetonationInstantCastChecker(EffectEvent effect, CombatData combatData, bool ifFound, ReadOnlySpan<GUID> effects)
    {
        // Find the DynamicEffectEnd of mine at the time of the explosion effects.
        if (combatData.TryGetEffectEventsBySrcWithGUIDs(effect.Src, effects, out var mineFields))
        {
            foreach (EffectEvent e in mineFields)
            {
                if (e.HasDynamicEndTime && Math.Abs(e.DynamicEndTime - effect.Time) < ServerDelayConstant)
                {
                    return ifFound;
                }
            }
        }
        return !ifFound;
    }

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffLossCastFinder(ExplosiveEntranceSkill, ExplosiveEntranceBuff)
            .WithBuilds(GW2Builds.February2020Balance)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new BuffGainCastFinder(ElixirSSkill, ElixirSBuff),
        new BuffGainCastFinder(SlickShoesSkill, SlickShoesBuff),
        new BuffGainCastFinder(IncendiaryAmmoSkill, IncendiaryAmmoBuff),
        new DamageCastFinder(OverchargedShot, OverchargedShot),
        new DamageCastFinder(MagneticInversion, MagneticInversion)
            .UsingDisableWithEffectData(),
        new EffectCastFinderByDst(MagneticInversion, EffectGUIDs.EngineerMagneticInversion)
            .UsingDstBaseSpecChecker(Spec.Engineer)
            .UsingChecker((effect, combatData, agentData, skillData) => combatData.HasLostBuff(Absorb, effect.Dst, effect.Time)),
        // Kits
        new EngineerKitFinder(BombKit),
        new EngineerKitFinder(ElixirGun),
        new EngineerKitFinder(Flamethrower),
        new EngineerKitFinder(GrenadeKit),
        new EngineerKitFinder(MedKitSkill),
        new EngineerKitFinder(ToolKit),
        new EngineerKitFinder(EliteMortarKit),
        new EffectCastFinderByDst(HealingMistOrSoothingDetonation, EffectGUIDs.EngineerHealingMist)
            .UsingDstBaseSpecChecker(Spec.Engineer),
        new EffectCastFinder(DetonateThrowMineOrMineField, EffectGUIDs.EngineerMineExplosion1)
            .UsingSecondaryEffectChecker(EffectGUIDs.EngineerMineExplosion2)
            .UsingChecker((effect, combatData, agentData, skillData) =>
            {
                // If Throw Mine and Mine Field are precasted out of combat, there won't be an DynamicEffectEnd event so we use the custom ID
                return MineDetonationInstantCastChecker(effect, combatData, false, [ EffectGUIDs.EngineerMineField, EffectGUIDs.EngineerThrowMineInactive1 ]);
            }),
        new EffectCastFinder(DetonateMineField, EffectGUIDs.EngineerMineExplosion1)
            .UsingSecondaryEffectChecker(EffectGUIDs.EngineerMineExplosion2)
            .UsingChecker((effect, combatData, agentData, skillData) =>
            {
                // Find the DynamicEffectEnd of Mine Field at the time of the explosion effects.
                return MineDetonationInstantCastChecker(effect, combatData, true, [ EffectGUIDs.EngineerMineField ]);
            }),
         new EffectCastFinder(DetonateThrowMine, EffectGUIDs.EngineerMineExplosion1)
            .UsingSecondaryEffectChecker(EffectGUIDs.EngineerMineExplosion2)
            .UsingChecker((effect, combatData, agentData, skillData) =>
            {
                // Find the DynamicEffectEnd of Throw Mine at the time of the explosion effects.
                return MineDetonationInstantCastChecker(effect, combatData, true, [ EffectGUIDs.EngineerThrowMineInactive1 ]);
            }),
         new DamageCastFinder(FocusedDevastation, FocusedDevastation)
            .UsingICD(1100), // Automatically procs on the target that has the Focused buff and is hit by Spear #5 Devastator, hits 6 times in 1 second.
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Explosives
        new DamageLogDamageModifier(Mod_GlassCannon, "Glass Cannon", "5% if hp >=75%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Engineer, TraitImages.GlassCannon, (x, log) => (x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0), DamageModifierMode.All)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.February2017Balance, GW2Builds.July2019Balance2),
        new DamageLogDamageModifier(Mod_GlassCannon, "Glass Cannon", "7% if hp >=75%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Engineer, TraitImages.GlassCannon, (x, log) => (x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0), DamageModifierMode.All)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.July2019Balance2, GW2Builds.May2021Balance),
        new DamageLogDamageModifier(Mod_GlassCannon, "Glass Cannon", "10% if hp >=75%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, TraitImages.GlassCannon, (x, log) => (x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0), DamageModifierMode.All)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.May2021Balance),
        new BuffOnFoeDamageModifier(Mod_ShapedCharge, Vulnerability, "Shaped Charge", "10% on vulnerable enemies", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, ByPresence, TraitImages.ExplosivePowder, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2019Balance),
        new BuffOnFoeDamageModifier(Mod_ShapedCharge, Vulnerability, "Shaped Charge", "0.5% per stack vuln", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Engineer, ByStack, TraitImages.ExplosivePowder, DamageModifierMode.All)
            .WithBuilds(GW2Builds.October2019Balance),
        new DamageLogDamageModifier(Mod_BigBoomer, "Big Boomer", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, TraitImages.BigBoomer, SelfHigherHPChecker, DamageModifierMode.All )
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
        new DamageLogDamageModifier(Mod_BigBoomer, "Big Boomer", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, TraitImages.BigBoomer, SelfHigherHPChecker, DamageModifierMode.sPvPWvW )
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.August2022Balance),
        new DamageLogDamageModifier(Mod_BigBoomer, "Big Boomer", "15% if target hp% lower than self hp%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Engineer, TraitImages.BigBoomer, SelfHigherHPChecker, DamageModifierMode.PvE )
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.August2022Balance),
        // Firearms
        new BuffOnActorDamageModifier(Mod_ThermalVision, ThermalVision, "Thermal Vision", "5% (4s) after burning foe", DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.All, Source.Engineer, ByPresence, TraitImages.ThermalVision, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2018Balance),
        new BuffOnActorDamageModifier(Mod_ThermalVision, ThermalVision, "Thermal Vision", "10% (4s) after burning foe", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Engineer, ByPresence, TraitImages.ThermalVision, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2018Balance),
        new BuffOnFoeDamageModifier(Mod_ModifiedAmmunition, NumberOfConditions, "Modified Ammunition", "2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Engineer, ByStack, TraitImages.ModifiedAmmunition, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2024JWRelease),
        new BuffOnFoeDamageModifier(Mod_ModifiedAmmunition, NumberOfConditions, "Modified Ammunition", "1.5% per condition on target", DamageSource.NoPets, 1.5, DamageType.Strike, DamageType.All, Source.Engineer, ByStack, TraitImages.ModifiedAmmunition, DamageModifierMode.WvW)
            .WithBuilds(GW2Builds.August2024JWRelease),
        new BuffOnFoeDamageModifier(Mod_ModifiedAmmunition, NumberOfConditions, "Modified Ammunition", "2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Engineer, ByStack, TraitImages.ModifiedAmmunition, DamageModifierMode.PvEsPvP)
            .WithBuilds(GW2Builds.August2024JWRelease),
        // Tools
        new BuffOnActorDamageModifier(Mod_ExcessiveEnergy, Vigor, "Excessive Energy", "10% under vigor", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, ByPresence, TraitImages.ExcessiveEnergy, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_OverShield, Protection, "Over Shield", "20% extra protection effectiveness", DamageSource.Incoming, (0.604/0.67 - 1) * 100, DamageType.Strike, DamageType.All, Source.Engineer, ByPresence, TraitImages.OverShield, DamageModifierMode.All), // We only compute the added effectiveness
        new BuffOnActorDamageModifier(Mod_IronBlooded, IronBlooded, "Iron Blooded", "-2% per stack", DamageSource.Incoming, -2, DamageType.StrikeAndCondition, DamageType.All, Source.Engineer, ByStack, TraitImages.IronBlooded, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Static Shield", StaticShield, Source.Engineer, BuffClassification.Other, SkillImages.StaticShield),
        new Buff("Absorb", Absorb, Source.Engineer, BuffClassification.Other, SkillImages.Absorb),
        new Buff("A.E.D.", AED, Source.Engineer, BuffClassification.Other, SkillImages.AED),
        new Buff("Elixir S", ElixirSBuff, Source.Engineer, BuffClassification.Other, SkillImages.ElixirS),
        new Buff("Utility Goggles", UtilityGoggles, Source.Engineer, BuffClassification.Other, SkillImages.UtilityGoggles),
        new Buff("Slick Shoes", SlickShoesBuff, Source.Engineer, BuffClassification.Other, SkillImages.SlickShoes),
        new Buff("Gear Shield", GearShield, Source.Engineer, BuffClassification.Other, SkillImages.GearShield),
        new Buff("Iron Blooded", IronBlooded, Source.Engineer, BuffStackType.Stacking, 25, BuffClassification.Other, TraitImages.IronBlooded),
        new Buff("Streamlined Kits", StreamlinedKits, Source.Engineer, BuffClassification.Other, TraitImages.StreamlinedKits),
        new Buff("Kinetic Charge", KineticCharge, Source.Engineer, BuffStackType.Stacking, 5, BuffClassification.Other, TraitImages.KineticBattery),
        new Buff("Pinpoint Distribution", PinpointDistribution, Source.Engineer, BuffClassification.Offensive, TraitImages.PinpointDistribution)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Thermal Vision", ThermalVision, Source.Engineer, BuffClassification.Other, TraitImages.ThermalVision),
        new Buff("Explosive Entrance", ExplosiveEntranceBuff, Source.Engineer, BuffClassification.Other, TraitImages.ExplosiveEntrance)
            .WithBuilds(GW2Builds.February2020Balance),
        new Buff("Explosive Temper", ExplosiveTemper, Source.Engineer, BuffStackType.Stacking, 10, BuffClassification.Other, TraitImages.ExplosiveTemper)
            .WithBuilds(GW2Builds.February2020Balance),
        new Buff("Big Boomer", BigBoomer, Source.Engineer, BuffStackType.Queue, 3, BuffClassification.Other, TraitImages.BigBoomer),
        new Buff("Med Kit", MedKitOpen, Source.Engineer, BuffClassification.Other, SkillImages.MedKit),
        new Buff("Med Kit Bonus", MedKitBonus, Source.Engineer, BuffClassification.Other,  SkillImages.MedKit),
        new Buff("Compounding Chemicals", CompoundingChemicals, Source.Engineer, BuffClassification.Other, TraitImages.CompoundingChemicals),
        /*
        new Buff("Grenade Kit", POV_GrenadeKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.GrenadeKit),
        new Buff("Bomb Kit", POV_BombKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.BombKit),
        new Buff("Elixir Gun", POV_ElixirGunKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.ElixirGun),
        new Buff("Flamethrower", POV_FlamethrowerKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.Flamethrower),
        new Buff("Tool Kit", POV_ToolKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.ToolKit),
        new Buff("Elite Mortar", POV_EliteMortarKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.EliteMortarKit),
        */
        // Spear
        new Buff("Conduit Surge", ConduitSurgeBuff, Source.Engineer, BuffClassification.Other, SkillImages.ConduitSurge),
        new Buff("Electric Artillery", ElectricArtilleryBuff, Source.Engineer, BuffClassification.Other, SkillImages.ElectricArtillery),
        new Buff("Focused", Focused, Source.Engineer, BuffStackType.Stacking, 99, BuffClassification.Other, SkillImages.Focused),
        new Buff("Lightning Rod Charges", LightningRodCharges, Source.Engineer, BuffStackType.Stacking, 12, BuffClassification.Other, SkillImages.LightningRod),
    ];

    public static void ProcessGadgets(IReadOnlyList<AgentItem> players, CombatData combatData)
    {
        var playerAgents = new HashSet<AgentItem>(players);

        HashSet<AgentItem> flameTurrets = GetOffensiveGadgetAgents(combatData, FlameTurretDamage, playerAgents);

        HashSet<AgentItem> rifleTurrets = GetOffensiveGadgetAgents(combatData, RifleTurretDamage, playerAgents);
        rifleTurrets.UnionWith(GetOffensiveGadgetAgents(combatData, RifleTurretDamageUW, playerAgents));

        HashSet<AgentItem> netTurrets = GetOffensiveGadgetAgents(combatData, NetTurretDamage, playerAgents);
        netTurrets.UnionWith(GetOffensiveGadgetAgents(combatData, NetTurretDamageUW, playerAgents));

        HashSet<AgentItem> rocketTurrets = GetOffensiveGadgetAgents(combatData, RocketTurretDamage, playerAgents);
        rocketTurrets.UnionWith(GetOffensiveGadgetAgents(combatData, RocketTurretDamageUW, playerAgents));

        HashSet<AgentItem> thumperTurrets = GetOffensiveGadgetAgents(combatData, ThumperTurret, playerAgents);
        thumperTurrets.UnionWith(GetOffensiveGadgetAgents(combatData, ThumperTurretUW, playerAgents));
        // TODO: need ID here
        HashSet<AgentItem> harpoonTurrets = GetOffensiveGadgetAgents(combatData, Unknown, playerAgents);

        HashSet<AgentItem> healingTurrets = GetOffensiveGadgetAgents(combatData, TurretExplosion, playerAgents);
        healingTurrets.RemoveWhere(x => thumperTurrets.Contains(x) || rocketTurrets.Contains(x) || netTurrets.Contains(x) || rifleTurrets.Contains(x) || flameTurrets.Contains(x) || harpoonTurrets.Contains(x));

        var engineers = players.Where(x => x.BaseSpec == Spec.Engineer);
        var engineersCount = engineers.Count();
        // if only one engineer, could only be that one
        if (engineersCount == 1)
        {
            AgentItem engineer = engineers.First();
            SetGadgetMaster(flameTurrets, engineer);
            SetGadgetMaster(netTurrets, engineer);
            SetGadgetMaster(rocketTurrets, engineer);
            SetGadgetMaster(rifleTurrets, engineer);
            SetGadgetMaster(thumperTurrets, engineer);
            SetGadgetMaster(harpoonTurrets, engineer);
            SetGadgetMaster(healingTurrets, engineer);
        }
        else if (engineersCount > 1)
        {
            AttachMasterToGadgetByCastData(combatData, flameTurrets, new List<long> { FlameTurretCast, SupplyCrate }, 1000);
            AttachMasterToGadgetByCastData(combatData, rifleTurrets, new List<long> { RifleTurretCast }, 1000);
            AttachMasterToGadgetByCastData(combatData, netTurrets, new List<long> { NetTurretCast, SupplyCrate, SupplyCrateUW }, 1000);
            AttachMasterToGadgetByCastData(combatData, rocketTurrets, new List<long> { RocketTurretCast, RocketTurretCast2, SupplyCrateUW }, 1000);
            AttachMasterToGadgetByCastData(combatData, thumperTurrets, new List<long> { ThumperTurretCast }, 1000);
            //AttachMasterToGadgetByCastData(castData, harpoonTurrets, new List<long> { 6093, 6183 }, 1000);
            //AttachMasterToGadgetByCastData(castData, healingTurrets, new List<long> { 5857, 5868 }, 1000);
        }
    }

    private static readonly HashSet<int> Minions = [];
    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Engineer;

        // Thunderclap
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ScrapperThunderclap, out var thunderclaps))
        {
            var skill = new SkillModeDescriptor(player, Spec.Engineer, Thunderclap, SkillModeCategory.ShowOnSelect | SkillModeCategory.CC);
            foreach (EffectEvent effect in thunderclaps)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectThunderclap, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
            }
        }

        // Throw Mine / Mine Field
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.EngineerMineExplosion1, out var mineDetonations))
        {
            var throwMine = new SkillModeDescriptor(player, Spec.Engineer, ThrowMine, SkillModeCategory.Strip | SkillModeCategory.CC);
            var mineField = new SkillModeDescriptor(player, Spec.Engineer, MineField);
            var detonate = new SkillModeDescriptor(player, Spec.Engineer, DetonateThrowMineOrMineField);
            var detonateThrowMine = new SkillModeDescriptor(player, Spec.Engineer, DetonateThrowMine, SkillModeCategory.Strip | SkillModeCategory.CC);
            var detonateMineField = new SkillModeDescriptor(player, Spec.Engineer, DetonateMineField);

            foreach (EffectEvent detonation in mineDetonations)
            {
                bool isThrowMine = false;
                bool isMineField = false;

                // Check if the detonation is sourced by Throw Mine
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.EngineerThrowMineInactive1, out var throwMines))
                {
                    foreach (EffectEvent mine in throwMines)
                    {
                        if (mine.HasDynamicEndTime && Math.Abs(mine.DynamicEndTime - detonation.Time) < ServerDelayConstant)
                        {
                            isThrowMine = true;
                            AddMineDecoration(log, replay, mine, color, throwMine, EffectImages.EffectThrowMine);
                            AddMineDetonationDecoration(log, replay, detonation, color, detonateThrowMine, 240);
                        }
                    }
                }

                // Check if the detonation is sourced by Mine Field
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.EngineerMineField, out var mineFields))
                {
                    // The mine field has 10 events, all using the same GUID, 5 with 0 duration and 5 with infinite duration, filter out half of them.
                    foreach (EffectEvent mine in mineFields.Where(x => x.Duration > 0))
                    {
                        if (mine.HasDynamicEndTime && Math.Abs(mine.DynamicEndTime - detonation.Time) < ServerDelayConstant)
                        {
                            isMineField = true;
                            AddMineDecoration(log, replay, mine, color, mineField, EffectImages.EffectMineField);
                            AddMineDetonationDecoration(log, replay, detonation, color, detonateMineField, 180);
                        }
                    }
                }

                // Throw Mine or Mine Field were placed before entering combat, log isn't aware of the source
                if (!isThrowMine && !isMineField)
                {
                    AddMineDetonationDecoration(log, replay, detonation, color, detonate, 180, true);
                }

            }
        }
    }

    /// <summary>
    /// Adds the decorations for <see cref="ThrowMine"/> or <see cref="MineField"/>.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="replay">The Combat Replay.</param>
    /// <param name="effect">The mine effect.</param>
    /// <param name="color">The specialization color.</param>
    /// <param name="skill">The source skill.</param>
    /// <param name="icon">The skill icon.</param>
    private static void AddMineDecoration(ParsedEvtcLog log, CombatReplay replay, EffectEvent effect, Color color, SkillModeDescriptor skill, string icon)
    {
        (long, long) lifespan = effect.ComputeDynamicLifespan(log, 60000);
        AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 120, icon);
    }

    /// <summary>
    /// Adds the decorations for <see cref="DetonateThrowMine"/> or <see cref="DetonateMineField"/>.<br></br>
    /// If the source of the detonation isn't known, use <see cref="DetonateThrowMineOrMineField"/>.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="replay">The Combat Replay.</param>
    /// <param name="effect">The detonation effect.</param>
    /// <param name="color">The specialization color.</param>
    /// <param name="skill">The source skill.</param>
    /// <param name="radius">The detonation radius.</param>
    /// <param name="unknownSource">Wether the source skill is known or not.</param>
    private static void AddMineDetonationDecoration(ParsedEvtcLog log, CombatReplay replay, EffectEvent effect, Color color, SkillModeDescriptor skill, uint radius, bool unknownSource = false)
    {
        (long, long) lifespan = effect.ComputeLifespan(log, 500);
        var connector = new PositionConnector(effect.Position);
        replay.Decorations.Add(new CircleDecoration(radius, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
        replay.Decorations.Add(new IconDecoration(EffectImages.EffectDetonateThrowMineOrMineField, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
        // If we do not know the source of the detonation, show both radiuses
        if (unknownSource)
        {
            replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
        }
    }

}
