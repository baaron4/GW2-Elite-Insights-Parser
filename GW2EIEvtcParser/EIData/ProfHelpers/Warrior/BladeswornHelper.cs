using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
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
            new BuffLossCastFinder(GunsaberSheath, GunsaberMode).WithBuilds(GW2Builds.EODBeta2).UsingBeforeWeaponSwap(true),
            new BuffGainCastFinder(Gunsaber, GunsaberMode).WithBuilds(GW2Builds.EODBeta2).UsingBeforeWeaponSwap(true),
            new DamageCastFinder(UnseenSword, UnseenSword).WithBuilds(GW2Builds.EODBeta2).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new BuffGainCastFinder(FlowStabilizer, PositiveFlow)
                .UsingChecker((bae, combatData, agentData, skillData) =>
                {
                    return 2 == CombatData.FindRelatedEvents(combatData.GetBuffData(PositiveFlow).OfType<BuffApplyEvent>(), bae.Time).Count(apply => apply.By == bae.To && apply.To == bae.To);
                }),
            new EffectCastFinder(DragonspikeMineSkill, EffectGUIDs.BladeswornDragonspikeMine).UsingSrcSpecChecker(Spec.Bladesworn),
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
            new BuffDamageModifier(FierceAsFire, "Fierce as Fire", "1%", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Bladesworn, ByStack, BuffImages.FierceAsFire, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Gunsaber Mode", GunsaberMode, Source.Bladesworn, BuffClassification.Other, BuffImages.UnsheatheGunsaber),
            new Buff("Dragon Trigger", DragonTrigger, Source.Bladesworn, BuffClassification.Other, BuffImages.DragonTrigger),
            new Buff("Positive Flow", PositiveFlow, Source.Bladesworn, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.AttributeBonus),
            new Buff("Fierce as Fire", FierceAsFire, Source.Bladesworn, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.FierceAsFire),
            new Buff("Stim State", StimState, Source.Bladesworn, BuffClassification.Other, BuffImages.CombatStimulant),
            new Buff("Guns and Glory", GunsAndGlory, Source.Bladesworn, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.GunsAndGlory),
            new Buff("Tactical Reload", TacticalReload, Source.Bladesworn, BuffClassification.Other, BuffImages.TacticalReload),
            new Buff("Overcharged Cartridges", OverchargedCartridgesBuff, Source.Bladesworn, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.OverchargedCartridges).WithBuilds(GW2Builds.June2022Balance, GW2Builds.EndOfLife),
        };


    }
}
