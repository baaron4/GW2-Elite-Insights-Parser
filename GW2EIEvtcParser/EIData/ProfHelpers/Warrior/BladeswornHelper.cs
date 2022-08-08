using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class BladeswornHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffLossCastFinder(GunsaberSheath, GunsaberMode).WithBuilds(GW2Builds.EODBeta2), // Gunsaber sheath
            new BuffGainCastFinder(Gunsaber, GunsaberMode).WithBuilds(GW2Builds.EODBeta2), // Gunsaber         
            new DamageCastFinder(UnseenSword, UnseenSword).WithBuilds(GW2Builds.EODBeta2), // Unseen Sword
        };

        private static readonly HashSet<long> _gunsaberForm = new HashSet<long>
        {
            GunsaberSheath, Gunsaber,
        };

        public static bool IsGunsaberForm(long id)
        {
            return _gunsaberForm.Contains(id);
        }

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(FierceAsFire, "Fierce as Fire", "1%", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Bladesworn, ByStack, "https://wiki.guildwars2.com/images/8/8e/Fierce_as_Fire.png", DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Gunsaber Mode", GunsaberMode, Source.Bladesworn, BuffClassification.Other,"https://wiki.guildwars2.com/images/f/f0/Unsheathe_Gunsaber.png"),
            new Buff("Dragon Trigger", DragonTrigger, Source.Bladesworn, BuffClassification.Other,"https://wiki.guildwars2.com/images/b/b1/Dragon_Trigger.png"),
            new Buff("Positive Flow", PositiveFlow, Source.Bladesworn, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/f9/Attribute_bonus.png"),
            new Buff("Fierce as Fire", FierceAsFire, Source.Bladesworn, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/8e/Fierce_as_Fire.png"),
            new Buff("Stim State", StimState, Source.Bladesworn, BuffClassification.Other,"https://wiki.guildwars2.com/images/a/ad/Combat_Stimulant.png"),
            new Buff("Guns and Glory", GunsAndGlory, Source.Bladesworn, BuffStackType.Queue, 9, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/72/Guns_and_Glory.png"),
            new Buff("Tactical Reload", TacticalReload, Source.Bladesworn, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/47/Tactical_Reload.png"),
            new Buff("Overcharged Cartridges", OverchargedCartridges, Source.Bladesworn, BuffStackType.Stacking, 25, BuffClassification.Other,"https://wiki.guildwars2.com/images/0/0a/Overcharged_Cartridges.png"),
        };


    }
}
