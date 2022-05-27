using System.Collections.Generic;
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
            new BuffGainCastFinder(OverheatSkill, OverheatEffect, EIData.InstantCastFinder.DefaultICD), // Overheat
            new BuffGainCastFinder(EnterPhotonForge, PhotonForge, EIData.InstantCastFinder.DefaultICD), // Photon Forge
            new BuffLossCastFinder(ExitPhotonForge, PhotonForge, EIData.InstantCastFinder.DefaultICD), // Deactivate Photon Forge - red or blue irrevelant
            new BuffGainCastFinder(SpectrumShieldSkill, SpectrumShieldEffect, EIData.InstantCastFinder.DefaultICD), // Spectrum Shield
            new DamageCastFinder(ThermalReleaseValve, ThermalReleaseValve, EIData.InstantCastFinder.DefaultICD), // Thermal Release Valve
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
            new BuffDamageModifier(LasersEdge, "Laser's Edge", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Holosmith, ByPresence, "https://wiki.guildwars2.com/images/5/5d/Laser%27s_Edge.png", 0, GW2Builds.July2019Balance, DamageModifierMode.PvE).UsingApproximate(true),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Cooling Vapor",CoolingVapor, Source.Holosmith, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b1/Coolant_Blast.png"),
                new Buff("Photon Wall Deployed",PhotonWallDeployed, Source.Holosmith, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/ea/Photon_Wall.png"),
                new Buff("Spectrum Shield",SpectrumShieldEffect, Source.Holosmith, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/29/Spectrum_Shield.png"),
                new Buff("Photon Forge",PhotonForge, Source.Holosmith, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/dd/Engage_Photon_Forge.png"),
                new Buff("Laser's Edge",LasersEdge, Source.Holosmith, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5d/Laser%27s_Edge.png",0 , GW2Builds.July2019Balance),
                new Buff("Afterburner",Afterburner, Source.Holosmith, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/51/Solar_Focusing_Lens.png"),
                new Buff("Heat Therapy",HeatTherapy, Source.Holosmith, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/34/Heat_Therapy.png"),
                new Buff("Overheat", OverheatEffect, Source.Holosmith, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/4b/Overheat.png"),          

        };
    }
}
