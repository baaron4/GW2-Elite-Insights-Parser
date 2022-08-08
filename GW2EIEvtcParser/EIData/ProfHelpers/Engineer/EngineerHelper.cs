using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class EngineerHelper
    {
        private class EngineerKitFinder : WeaponSwapCastFinder
        {
            public EngineerKitFinder(long skillID) : base(skillID, WeaponSetIDs.KitSet)
            {
                UsingChecker((swap, combatData, skillData) =>
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
                NotAccurate = true;
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
            new BuffLossCastFinder(ExplosiveEntranceSkill, ExplosiveEntranceEffect).WithBuilds(GW2Builds.February2020Balance), // Explosive Entrance
            new BuffGainCastFinder(ElixirSSkill, ElixirSEffect), // Elixir S
            new DamageCastFinder(OverchargedShot,OverchargedShot), // Overcharged Shot
            // Kits
            new EngineerKitFinder(BombKit), // Bomb Kit
            new EngineerKitFinder(ElixirGun), // Elixir Gun
            new EngineerKitFinder(Flamethrower), // Flamethrower
            new EngineerKitFinder(GrenadeKit), // Grenade Kit
            new EngineerKitFinder(MedKitSkill), // Med Kit
            new EngineerKitFinder(ToolKit), // Tool Kit
            new EngineerKitFinder(EliteMortarKit), // Elite Mortar Kit
            new EffectCastFinderByDst(HealingMist, EffectGUIDs.EngineerHealingMist).UsingChecker((evt, log) => evt.Dst.BaseSpec == Spec.Engineer && evt.Dst.Spec != Spec.Mechanist),
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Explosives
            new DamageLogDamageModifier("Glass Cannon", "5% if self >75%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Engineer,"https://wiki.guildwars2.com/images/6/6e/Glass_Cannon.png", (x, log) => (x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0), ByPresence, DamageModifierMode.All).UsingApproximate(true).WithBuilds(72781, 98248),
            new DamageLogDamageModifier("Glass Cannon", "7% if self >75%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Engineer,"https://wiki.guildwars2.com/images/6/6e/Glass_Cannon.png", (x, log) => (x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0), ByPresence, DamageModifierMode.All).UsingApproximate(true).WithBuilds(98248, GW2Builds.May2021Balance),
            new DamageLogDamageModifier("Glass Cannon", "10% if self >75%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer,"https://wiki.guildwars2.com/images/6/6e/Glass_Cannon.png", (x, log) => (x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0), ByPresence, DamageModifierMode.All).UsingApproximate(true).WithBuilds(GW2Builds.May2021Balance),
            new BuffDamageModifierTarget(Vulnerability, "Shaped Charge", "10% on vulnerable enemies", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, ByPresence, "https://wiki.guildwars2.com/images/f/f3/Explosive_Powder.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2019Balance),
            new BuffDamageModifierTarget(Vulnerability, "Shaped Charge", "0.5% per stack vuln", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Engineer, ByStack, "https://wiki.guildwars2.com/images/f/f3/Explosive_Powder.png", DamageModifierMode.All).WithBuilds(GW2Builds.October2019Balance),
            new DamageLogDamageModifier("Big Boomer", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, "https://wiki.guildwars2.com/images/8/83/Big_Boomer.png", (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, ByPresence, DamageModifierMode.All ).UsingApproximate(true),
            // Firearms
            new BuffDamageModifier(ThermalVision, "Thermal Vision", "5% (4s) after burning foe", DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.All, Source.Engineer, ByPresence, "https://wiki.guildwars2.com/images/8/8a/Skilled_Marksman.png", DamageModifierMode.All).WithBuilds(GW2Builds.August2018Balance),
            new BuffDamageModifier(ThermalVision, "Thermal Vision", "10% (4s) after burning foe", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Engineer, ByPresence, "https://wiki.guildwars2.com/images/8/8a/Skilled_Marksman.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2018Balance),
            new BuffDamageModifierTarget(NumberOfConditions, "Modified Ammunition", "2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Engineer, ByStack, "https://wiki.guildwars2.com/images/9/94/Modified_Ammunition.png", DamageModifierMode.All),
            // Tools
            new BuffDamageModifier(Vigor, "Excessive Energy", "10% under vigor", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Engineer, ByPresence, "https://wiki.guildwars2.com/images/1/1f/Excessive_Energy.png", DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Static Shield",StaticShield, Source.Engineer, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/90/Static_Shield.png"),
                new Buff("Absorb",Absorb, Source.Engineer, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/8a/Absorb.png"),
                new Buff("A.E.D.",AED, Source.Engineer, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e6/A.E.D..png"),
                new Buff("Elixir S",ElixirSEffect, Source.Engineer, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d8/Elixir_S.png"),
                new Buff("Utility Goggles",UtilityGoggles, Source.Engineer, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/02/Utility_Goggles.png"),
                new Buff("Slick Shoes",SlickShoes, Source.Engineer, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/3d/Slick_Shoes.png"),
                new Buff("Gear Shield",GearShield, Source.Engineer, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/ca/Gear_Shield.png"),
                new Buff("Iron Blooded",IronBlooded, Source.Engineer, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/1e/Iron_Blooded.png"),
                new Buff("Streamlined Kits",StreamlinedKits, Source.Engineer, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/cb/Streamlined_Kits.png"),
                new Buff("Kinetic Charge",KineticCharge, Source.Engineer, BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e0/Kinetic_Battery.png"),
                new Buff("Pinpoint Distribution", PinpointDistribution, Source.Engineer, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/b/bf/Pinpoint_Distribution.png", GW2Builds.StartOfLife, GW2Builds.June2022Balance),
                new Buff("Thermal Vision", ThermalVision, Source.Engineer, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/8a/Skilled_Marksman.png"),
                new Buff("Explosive Entrance",ExplosiveEntranceEffect, Source.Engineer, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/33/Explosive_Entrance.png", GW2Builds.February2020Balance, GW2Builds.EndOfLife),
                new Buff("Explosive Temper",ExplosiveTemper, Source.Engineer, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c1/Explosive_Temper.png", GW2Builds.February2020Balance, GW2Builds.EndOfLife),
                new Buff("Big Boomer",BigBoomer, Source.Engineer, BuffStackType.Queue, 3, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/83/Big_Boomer.png"),
                new Buff("Med Kit",MedKitEffect, Source.Engineer, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/14/Med_Kit.png"),
                new Buff("Med Kit Bonus",MedKitBonus, Source.Engineer, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/14/Med_Kit.png"),

        };

        public static void AttachMasterToEngineerTurrets(IReadOnlyList<Player> players, CombatData combatData)
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

        private static HashSet<long> Minions = new HashSet<long>();
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }

    }
}
