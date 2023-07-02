using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class TempestHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            //new DamageCastFinder(30662, 30662, 10000), // "Feel the Burn!" - shockwave, fire aura indiscernable from the focus skill
            new EffectCastFinder(FeelTheBurn, EffectGUIDs.TempestFeelTheBurn).UsingSrcSpecChecker(Spec.Tempest),
            new EffectCastFinder(EyeOfTheStormShout, EffectGUIDs.TempestEyeOfTheStorm1).UsingSrcSpecChecker(Spec.Tempest),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(HarmoniousConduit, "Harmonious Conduit", "10% (4s) after overload", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Tempest, ByPresence, BuffImages.HarmoniousConduit, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife ,GW2Builds.October2019Balance),
            new BuffDamageModifier(TranscendentTempest, "Transcendent Tempest", "7% after overload", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Tempest, ByPresence, BuffImages.TranscendentTempest, DamageModifierMode.All).WithBuilds(GW2Builds.October2019Balance, GW2Builds.August2022Balance),
            new BuffDamageModifier(TranscendentTempest, "Transcendent Tempest", "7% after overload", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Tempest, ByPresence, BuffImages.TranscendentTempest, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.August2022Balance),
            new BuffDamageModifier(TranscendentTempest, "Transcendent Tempest", "15% after overload", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Tempest, ByPresence, BuffImages.TranscendentTempest, DamageModifierMode.PvE).WithBuilds(GW2Builds.August2022Balance),
            new BuffDamageModifier(TempestuousAria, "Tempestuous Aria", "10% after giving aura", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Tempest, ByPresence, BuffImages.TempestuousAria, DamageModifierMode.PvE).WithBuilds(GW2Builds.June2023Balance),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Rebound", Rebound, Source.Tempest, BuffClassification.Defensive, BuffImages.Rebound),
            new Buff("Harmonious Conduit", HarmoniousConduit, Source.Tempest, BuffClassification.Other, BuffImages.HarmoniousConduit).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2019Balance),
            new Buff("Transcendent Tempest", TranscendentTempest, Source.Tempest, BuffClassification.Other, BuffImages.TranscendentTempest).WithBuilds(GW2Builds.October2019Balance, GW2Builds.EndOfLife),
            new Buff("Static Charge", StaticCharge, Source.Tempest, BuffClassification.Offensive, BuffImages.OverloadAir),
            new Buff("Heat Sync", HeatSync, Source.Tempest, BuffClassification.Support, BuffImages.HeatSync),
            new Buff("Tempestuous Aria", TempestuousAria, Source.Tempest, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.TempestuousAria).WithBuilds(GW2Builds.June2023Balance),
        };

    }
}
