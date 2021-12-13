using System.Collections.Generic;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal static class ScourgeHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            // Trail of Anguish ? Unique effect?
            new EXTBarrierCastFinder(44663, 44663, EIData.InstantCastFinder.DefaultICD), // Desert Shroud
            new EXTBarrierCastFinder(43448, 43448, EIData.InstantCastFinder.DefaultICD), // Sand Cascade
            // Sandstorm Shroud ? The detonation part is problematic
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Sadistic Searing",43626, ParserHelper.Source.Scourge, BuffNature.GraphOnly, "https://wiki.guildwars2.com/images/d/dd/Sadistic_Searing.png"),
                new Buff("Path Uses",43410, ParserHelper.Source.Scourge, BuffStackType.Stacking, 25, BuffNature.GraphOnly, "https://wiki.guildwars2.com/images/2/20/Sand_Swell.png"),
        };
    }
}
