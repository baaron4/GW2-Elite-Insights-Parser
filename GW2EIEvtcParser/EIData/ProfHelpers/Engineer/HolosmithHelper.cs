using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class HolosmithHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(EnterPhotonForge, PhotonForge)
                .UsingBeforeWeaponSwap(true),
            new BuffLossCastFinder(ExitPhotonForge, PhotonForge)
                .UsingBeforeWeaponSwap(true),
            new BuffGainCastFinder(OverheatSkill, OverheatBuff)
                .UsingBeforeWeaponSwap(true),
            new BuffGainCastFinder(SpectrumShieldSkill, SpectrumShieldBuff),
            new DamageCastFinder(ThermalReleaseValve, ThermalReleaseValve)
                .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EffectCastFinderByDst(FlashSpark, EffectGUIDs.HolosmithFlashSpark)
                .UsingDstSpecChecker(Spec.Holosmith),
            new EffectCastFinderByDst(BladeBurstOrParticleAccelerator, EffectGUIDs.HolosmitBladeBurstParticleAccelerator1)
                .UsingDstSpecChecker(Spec.Holosmith)
                .UsingSecondaryEffectChecker(EffectGUIDs.HolosmitBladeBurstParticleAccelerator2),
        };

        private static readonly HashSet<long> _photonForgeCast = new HashSet<long>
        {
            EnterPhotonForge, ExitPhotonForge
        };

        public static bool IsPhotonForgeTransform(long id)
        {
            return _photonForgeCast.Contains(id);
        }

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(LasersEdge, "Laser's Edge", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Holosmith, ByPresence, BuffImages.LasersEdge, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Cooling Vapor", CoolingVapor, Source.Holosmith, BuffClassification.Other, BuffImages.CoolantBlast),
            new Buff("Photon Wall Deployed", PhotonWallDeployed, Source.Holosmith, BuffClassification.Other, BuffImages.PhotonWall),
            new Buff("Spectrum Shield", SpectrumShieldBuff, Source.Holosmith, BuffClassification.Other, BuffImages.SpectrumShield),
            new Buff("Photon Forge", PhotonForge, Source.Holosmith, BuffClassification.Other, BuffImages.EngagePhotonForge),
            new Buff("Laser's Edge", LasersEdge, Source.Holosmith, BuffClassification.Other, BuffImages.LasersEdge)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
            new Buff("Afterburner", Afterburner, Source.Holosmith, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.SolarFocusingLens),
            new Buff("Heat Therapy", HeatTherapy, Source.Holosmith, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.HeatTherapy),
            new Buff("Overheat", OverheatBuff, Source.Holosmith, BuffClassification.Other, BuffImages.Overheat),

            // heat buffs only present in forge
            new Buff("0-50 Heat (Photon Forge)", PhotonForgeAbove0Heat, Source.Holosmith, BuffClassification.Other, BuffImages.Heat1),
            new Buff("50-100 Heat (Photon Forge)", PhotonForgeAbove50Heat, Source.Holosmith, BuffClassification.Other, BuffImages.Heat2),
            new Buff("100-150 Heat (Photon Forge)", PhotonForgeAbove100Heat, Source.Holosmith, BuffClassification.Other, BuffImages.Heat3),
        };
    }
}
