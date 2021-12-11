using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class ChronomancerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(29830, 30136, EIData.InstantCastFinder.DefaultICD), // Continuum Split
            new BuffLossCastFinder(30747, 30136, EIData.InstantCastFinder.DefaultICD), // Continuum Shift
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(26766, "Danger Time", "30% crit damage on slowed target", DamageSource.NoPets, 30.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", 86181, GW2Builds.December2018Balance, DamageModifierMode.All, ((x, log) => x.HasCrit)),
            new BuffDamageModifierTarget(26766, "Danger Time", "30% crit damage on slowed target", DamageSource.All, 30.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", GW2Builds.December2018Balance, GW2Builds.March2019Balance, DamageModifierMode.All, ((x, log) => x.HasCrit)),
            new BuffDamageModifierTarget(26766, "Danger Time", "10% crit damage on slowed target", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", GW2Builds.March2019Balance, GW2Builds.May2021Balance, DamageModifierMode.All, ((x, log) => x.HasCrit)),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Time Echo",29582, Source.Chronomancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8d/Deja_Vu.png"),
                new Buff("Time Anchored",30136, Source.Chronomancer, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/db/Continuum_Split.png"),
        };

        private static HashSet<long> NonCloneMinions = new HashSet<long>()
        {
            (int)MinionID.IllusionaryAvenger,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return NonCloneMinions.Contains(id);
        }

    }
}
