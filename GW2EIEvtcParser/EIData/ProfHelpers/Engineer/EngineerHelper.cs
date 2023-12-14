using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;

namespace GW2EIEvtcParser.EIData
{
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
                    WeaponSwapEvent nextSwap = combatData.GetWeaponSwapData(swap.Caster).FirstOrDefault(x => x.Time > swap.Time + ServerDelayConstant);
                    long nextSwapTime = nextSwap != null ? nextSwap.Time : long.MaxValue;
                    var castIds = new HashSet<long>(combatData.GetAnimatedCastData(swap.Caster).Where(x => x.Time >= swap.Time + WeaponSwapDelayConstant && x.Time <= nextSwapTime).Select(x => x.SkillId));
                    return skill.ApiSkill.BundleSkills.Intersect(castIds).Any();
                });
                UsingNotAccurate(true);
            }
        }

        private static readonly HashSet<long> _engineerKit = new HashSet<long>
        {
            BombKit,
            ElixirGun,
            Flamethrower,
            GrenadeKit,
            MedKitSkill,
            ToolKit,
            EliteMortarKit,
        };

        public static bool IsEngineerKit(long id)
        {
            return _engineerKit.Contains(id);
        }

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffLossCastFinder(ExplosiveEntranceSkill, ExplosiveEntranceBuff)
                .WithBuilds(GW2Builds.February2020Balance).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
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
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            // Explosives
            new DamageLogDamageModifier("Glass Cannon", "5% if hp >=75%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Engineer, BuffImages.GlassCannon, (x, log) => (x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0), DamageModifierMode.All)
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.February2017Balance, GW2Builds.July2019Balance2),
            new DamageLogDamageModifier("Glass Cannon", "7% if hp >=75%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Engineer, BuffImages.GlassCannon, (x, log) => (x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0), DamageModifierMode.All)
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.July2019Balance2, GW2Builds.May2021Balance),
            new DamageLogDamageModifier("Glass Cannon", "10% if hp >=75%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, BuffImages.GlassCannon, (x, log) => (x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0), DamageModifierMode.All)
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.May2021Balance),
            new BuffOnFoeDamageModifier(Vulnerability, "Shaped Charge", "10% on vulnerable enemies", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, ByPresence, BuffImages.ExplosivePowder, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2019Balance),
            new BuffOnFoeDamageModifier(Vulnerability, "Shaped Charge", "0.5% per stack vuln", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Engineer, ByStack, BuffImages.ExplosivePowder, DamageModifierMode.All)
                .WithBuilds(GW2Builds.October2019Balance),
            new DamageLogDamageModifier("Big Boomer", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, BuffImages.BigBoomer, (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, DamageModifierMode.All ).UsingApproximate(true)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new DamageLogDamageModifier("Big Boomer", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, BuffImages.BigBoomer, (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, DamageModifierMode.sPvPWvW )
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.August2022Balance),
            new DamageLogDamageModifier("Big Boomer", "15% if target hp% lower than self hp%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Engineer, BuffImages.BigBoomer, (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, DamageModifierMode.PvE )
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.August2022Balance),
            // Firearms
            new BuffOnActorDamageModifier(ThermalVision, "Thermal Vision", "5% (4s) after burning foe", DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.All, Source.Engineer, ByPresence, BuffImages.SkilledMarksman, DamageModifierMode.All)
                .WithBuilds(GW2Builds.August2018Balance),
            new BuffOnActorDamageModifier(ThermalVision, "Thermal Vision", "10% (4s) after burning foe", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Engineer, ByPresence, BuffImages.SkilledMarksman, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2018Balance),
            new BuffOnFoeDamageModifier(NumberOfConditions, "Modified Ammunition", "2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Engineer, ByStack, BuffImages.ModifiedAmmunition, DamageModifierMode.All),
            // Tools
            new BuffOnActorDamageModifier(Vigor, "Excessive Energy", "10% under vigor", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, ByPresence, BuffImages.ExcessiveEnergy, DamageModifierMode.All),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(Protection, "Over Shield", "20% extra protection effectiveness", DamageSource.NoPets, (0.604/0.67 - 1) * 100, DamageType.Strike, DamageType.All, Source.Engineer, ByPresence, BuffImages.OverShield, DamageModifierMode.All), // We only compute the added effectiveness
            new BuffOnActorDamageModifier(IronBlooded, "Iron Blooded", "-2% per stack", DamageSource.NoPets, -2, DamageType.StrikeAndCondition, DamageType.All, Source.Engineer, ByStack, BuffImages.IronBlooded, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Static Shield", StaticShield, Source.Engineer, BuffClassification.Other, BuffImages.StaticShield),
            new Buff("Absorb", Absorb, Source.Engineer, BuffClassification.Other, BuffImages.Absorb),
            new Buff("A.E.D.", AED, Source.Engineer, BuffClassification.Other, BuffImages.AED),
            new Buff("Elixir S", ElixirSBuff, Source.Engineer, BuffClassification.Other, BuffImages.ElixirS),
            new Buff("Utility Goggles", UtilityGoggles, Source.Engineer, BuffClassification.Other, BuffImages.UtilityGoggles),
            new Buff("Slick Shoes", SlickShoesBuff, Source.Engineer, BuffClassification.Other, BuffImages.SlickShoes),
            new Buff("Gear Shield", GearShield, Source.Engineer, BuffClassification.Other, BuffImages.GearShield),
            new Buff("Iron Blooded", IronBlooded, Source.Engineer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.IronBlooded),
            new Buff("Streamlined Kits", StreamlinedKits, Source.Engineer, BuffClassification.Other, BuffImages.StreamlinedKits),
            new Buff("Kinetic Charge", KineticCharge, Source.Engineer, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.KineticBattery),
            new Buff("Pinpoint Distribution", PinpointDistribution, Source.Engineer, BuffClassification.Offensive, BuffImages.PinpointDistribution)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Thermal Vision", ThermalVision, Source.Engineer, BuffClassification.Other, BuffImages.SkilledMarksman),
            new Buff("Explosive Entrance", ExplosiveEntranceBuff, Source.Engineer, BuffClassification.Other, BuffImages.ExplosiveEntrance)
                .WithBuilds(GW2Builds.February2020Balance, GW2Builds.EndOfLife),
            new Buff("Explosive Temper", ExplosiveTemper, Source.Engineer, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.ExplosiveTemper)
                .WithBuilds(GW2Builds.February2020Balance, GW2Builds.EndOfLife),
            new Buff("Big Boomer", BigBoomer, Source.Engineer, BuffStackType.Queue, 3, BuffClassification.Other, BuffImages.BigBoomer),
            new Buff("Med Kit", MedKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.MedKit),
            new Buff("Med Kit Bonus", MedKitBonus, Source.Engineer, BuffClassification.Other,  BuffImages.MedKit),
            new Buff("Compounding Chemicals", CompoundingChemicals, Source.Engineer, BuffClassification.Other, BuffImages.CompoundingChemicals),
            /*
            new Buff("Grenade Kit", POV_GrenadeKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.GrenadeKit),
            new Buff("Bomb Kit", POV_BombKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.BombKit),
            new Buff("Elixir Gun", POV_ElixirGunKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.ElixirGun),
            new Buff("Flamethrower", POV_FlamethrowerKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.Flamethrower),
            new Buff("Tool Kit", POV_ToolKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.ToolKit),
            new Buff("Elite Mortar", POV_EliteMortarKitOpen, Source.Engineer, BuffClassification.Other, BuffImages.EliteMortarKit),
            */
        };

        public static void ProcessGadgets(IReadOnlyList<Player> players, CombatData combatData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));

            HashSet<AgentItem> flameTurrets = ProfHelper.GetOffensiveGadgetAgents(combatData, FireTurretDamage, playerAgents);

            HashSet<AgentItem> rifleTurrets = ProfHelper.GetOffensiveGadgetAgents(combatData, RifleTurretDamage, playerAgents);
            rifleTurrets.UnionWith(ProfHelper.GetOffensiveGadgetAgents(combatData, RifleTurretDamageUW, playerAgents));

            HashSet<AgentItem> netTurrets = ProfHelper.GetOffensiveGadgetAgents(combatData, NetTurretDamage, playerAgents);
            netTurrets.UnionWith(ProfHelper.GetOffensiveGadgetAgents(combatData, NetTurretDamageUW, playerAgents));

            HashSet<AgentItem> rocketTurrets = ProfHelper.GetOffensiveGadgetAgents(combatData, RocketTurretDamage, playerAgents);
            rocketTurrets.UnionWith(ProfHelper.GetOffensiveGadgetAgents(combatData, RocketTurretDamageUW, playerAgents));

            HashSet<AgentItem> thumperTurrets = ProfHelper.GetOffensiveGadgetAgents(combatData, ThumperTurret, playerAgents);
            thumperTurrets.UnionWith(ProfHelper.GetOffensiveGadgetAgents(combatData, ThumperTurretUW, playerAgents));
            // TODO: need ID here
            HashSet<AgentItem> harpoonTurrets = ProfHelper.GetOffensiveGadgetAgents(combatData, Unknown, playerAgents);

            HashSet<AgentItem> healingTurrets = ProfHelper.GetOffensiveGadgetAgents(combatData, TurretExplosion, playerAgents);
            healingTurrets.RemoveWhere(x => thumperTurrets.Contains(x) || rocketTurrets.Contains(x) || netTurrets.Contains(x) || rifleTurrets.Contains(x) || flameTurrets.Contains(x) || harpoonTurrets.Contains(x));

            var engineers = players.Where(x => x.BaseSpec == Spec.Engineer).ToList();
            // if only one engineer, could only be that one
            if (engineers.Count == 1)
            {
                Player engineer = engineers[0];
                ProfHelper.SetGadgetMaster(flameTurrets, engineer.AgentItem);
                ProfHelper.SetGadgetMaster(netTurrets, engineer.AgentItem);
                ProfHelper.SetGadgetMaster(rocketTurrets, engineer.AgentItem);
                ProfHelper.SetGadgetMaster(rifleTurrets, engineer.AgentItem);
                ProfHelper.SetGadgetMaster(thumperTurrets, engineer.AgentItem);
                ProfHelper.SetGadgetMaster(harpoonTurrets, engineer.AgentItem);
                ProfHelper.SetGadgetMaster(healingTurrets, engineer.AgentItem);
            }
            else if (engineers.Count > 1)
            {
                ProfHelper.AttachMasterToGadgetByCastData(combatData, flameTurrets, new List<long> { FlameTurretCast, SupplyCrate }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(combatData, rifleTurrets, new List<long> { RifleTurretCast }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(combatData, netTurrets, new List<long> { NetTurretCast, SupplyCrate, SupplyCrateUW }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(combatData, rocketTurrets, new List<long> { RocketTurretCast, RocketTurretCast2, SupplyCrateUW }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(combatData, thumperTurrets, new List<long> { ThumperTurretCast }, 1000);
                //AttachMasterToGadgetByCastData(castData, harpoonTurrets, new List<long> { 6093, 6183 }, 1000);
                //AttachMasterToGadgetByCastData(castData, healingTurrets, new List<long> { 5857, 5868 }, 1000);
            }
        }

        private static HashSet<int> Minions = new HashSet<int>();
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Engineer;

            // Thunderclap
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ScrapperThunderclap, out IReadOnlyList<EffectEvent> thunderclaps))
            {
                var skill = new SkillModeDescriptor(player, Spec.Engineer, Thunderclap, SkillModeCategory.ShowOnSelect);
                foreach (EffectEvent effect in thunderclaps)
                {
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 5000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectThunderclap, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
        }
    }
}
