using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class RenegadeHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(LegendaryRenegadeStanceSkill, LegendaryRenegadeStanceEffect), // Legendary Renegade Stance
            new DamageCastFinder(CallOfTheRenegade, CallOfTheRenegade), // Call of the Renegade
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(KallasFervor, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png", DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifier(ImprovedKallasFervor, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png", DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifier(KallasFervor, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
            new BuffDamageModifier(ImprovedKallasFervor, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Legendary Renegade Stance",LegendaryRenegadeStanceEffect, Source.Renegade, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/19/Legendary_Renegade_Stance.png"),
                new Buff("Breakrazor's Bastion",BreakrazorsBastion, Source.Renegade, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/a/a7/Breakrazor%27s_Bastion.png"),
                new Buff("Razorclaw's Rage",RazorclawsRage, Source.Renegade, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/7/73/Razorclaw%27s_Rage.png"),
                new Buff("Soulcleave's Summit",SoulcleavesSummit, Source.Renegade, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/7/78/Soulcleave%27s_Summit.png"),
                new Buff("Kalla's Fervor",KallasFervor, Source.Renegade, BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
                new Buff("Improved Kalla's Fervor",ImprovedKallasFervor, Source.Renegade, BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
        };

        private static HashSet<long> Minions = new HashSet<long>()
        {
            (int)MinionID.JasRazorclaw,
            (int)MinionID.ViskIcerazor,
            (int)MinionID.KusDarkrazor,
            (int)MinionID.EraBreakrazor,
            (int)MinionID.OfelaSoulcleave,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }
    }
}
