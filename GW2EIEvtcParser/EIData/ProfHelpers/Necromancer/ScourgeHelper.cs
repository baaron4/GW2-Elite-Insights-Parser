using System.Collections.Generic;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ScourgeHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            // Trail of Anguish ? Unique effect?
            new EffectCastFinder(TrailOfAnguish, EffectGUIDs.ScourgeTrailOfAnguish).UsingChecker((evt, log) => evt.Src.Spec == ParserHelper.Spec.Scourge).UsingICD(6100),
            new EXTBarrierCastFinder(DesertShroud, DesertShroud), // Desert Shroud
            new EXTBarrierCastFinder(SandCascadeSkill, SandCascadeBarrier), // Sand Cascade
            // Sandstorm Shroud ? The detonation part is problematic
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Sadistic Searing",SadisticSearing, ParserHelper.Source.Scourge, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/dd/Sadistic_Searing.png"),
                new Buff("Path Uses",PathUses, ParserHelper.Source.Scourge, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/20/Sand_Swell.png"),
        };
    }
}
