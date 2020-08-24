using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal class DruidHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(31869,31508,EIData.InstantCastFinder.DefaultICD), // Celestial Avatar
            new BuffLossCastFinder(31411,31508,EIData.InstantCastFinder.DefaultICD), // Release Celestial Avatar
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Celestial Avatar", 31508, ParserHelper.Source.Druid, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/59/Celestial_Avatar.png"),
                new Buff("Ancestral Grace", 31584, ParserHelper.Source.Druid, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4b/Ancestral_Grace.png"),
                new Buff("Glyph of Empowerment", 31803, ParserHelper.Source.Druid, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/d/d7/Glyph_of_the_Stars.png", 0 , 96406),
                new Buff("Glyph of Unity", 31385, ParserHelper.Source.Druid, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b1/Glyph_of_Unity.png"),
                new Buff("Glyph of Unity (CA)", 31556, ParserHelper.Source.Druid, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/4/4c/Glyph_of_Unity_%28Celestial_Avatar%29.png"),
                new Buff("Glyph of the Stars", 55048, ParserHelper.Source.Druid, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/d/d7/Glyph_of_the_Stars.png", 96406, ulong.MaxValue),
                new Buff("Glyph of the Stars (CA)", 55049, ParserHelper.Source.Druid, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/d/d7/Glyph_of_the_Stars.png", 96406, ulong.MaxValue),
                new Buff("Natural Mender",30449, ParserHelper.Source.Druid, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e9/Natural_Mender.png"),
                new Buff("Lingering Light",32248, ParserHelper.Source.Druid, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5d/Lingering_Light.png"),
        };

    }
}
