using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal class BerserkerHelper : WarriorHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> BerserkerInstantCastFinders = new List<InstantCastFinder>()
        {
            new DamageCastFinder(31289, 31289, 500, 97950, ulong.MaxValue), // King of Fires
        };

        internal static readonly List<Buff> BerserkerBuffs = new List<Buff>
        {
                new Buff("Berserk",29502, ParserHelper.Source.Berserker, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/4/44/Berserk.png"),
                new Buff("Flames of War", 31708, ParserHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/6f/Flames_of_War_%28warrior_skill%29.png"),
                new Buff("Blood Reckoning", 29466 , ParserHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d6/Blood_Reckoning.png"),
                new Buff("Rock Guard", 34256 , ParserHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c7/Shattering_Blow.png"),
                new Buff("Feel No Pain (Savage Instinct)",55030, ParserHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4d/Savage_Instinct.png", 96406, ulong.MaxValue),
                new Buff("Always Angry",34099, ParserHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Always_Angry.png", 0 , 96406),
        };


    }
}
