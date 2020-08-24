using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using System.Linq;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal class RangerHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(12573,12573,EIData.InstantCastFinder.DefaultICD), // Hunter's Shot
            new DamageCastFinder(12507,12507,EIData.InstantCastFinder.DefaultICD), // Crippling Shot
            new BuffGiveCastFinder(33902,12633,EIData.InstantCastFinder.DefaultICD), // "Sic 'Em!"
            new BuffGiveCastFinder(56923,12633,EIData.InstantCastFinder.DefaultICD), // "Sic 'Em!" PvP
            new BuffGainCastFinder(12500,12543,EIData.InstantCastFinder.DefaultICD, (evt, combatData) => Math.Abs(evt.AppliedDuration - 6000) < ParserHelper.ServerDelayConstant), // Signet of Stone
            new BuffGainCastFinder(42470,12543,EIData.InstantCastFinder.DefaultICD, (evt, combatData) => Math.Abs(evt.AppliedDuration - 5000) < ParserHelper.ServerDelayConstant), // Lesser Signet of Stone
            new BuffGainCastFinder(12537,12536,EIData.InstantCastFinder.DefaultICD), // Sharpening Stone
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Counterattack",14509, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c1/Counterattack.png"),
                //signets
                new Buff("Signet of Renewal",41147, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/11/Signet_of_Renewal.png"),
                new Buff("Signet of Stone (Passive)",12627, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Signet_of_Stone.png"),
                new Buff("Signet of the Hunt (Passive)",12626, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/df/Signet_of_the_Hunt.png"),
                new Buff("Signet of the Wild",12518, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/23/Signet_of_the_Wild.png"),
                new Buff("Signet of the Wild (Pet)",12636, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/23/Signet_of_the_Wild.png"),
                new Buff("Signet of Stone (Active)",12543, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Signet_of_Stone.png"),
                new Buff("Signet of the Hunt (Active)",12541, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/df/Signet_of_the_Hunt.png"),
                //spirits
                // new Boon("Water Spirit (old)", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png"),
                new Buff("Frost Spirit", 12544, ParserHelper.Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png", 0, 88541),
                new Buff("Sun Spirit", 12540, ParserHelper.Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png", 0, 88541),
                new Buff("Stone Spirit", 12547, ParserHelper.Source.Ranger, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png", 0, 88541),
                //new Boon("Storm Spirit (old)", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png"),
                //reworked
                new Buff("Water Spirit", 50386, ParserHelper.Source.Ranger, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Frost Spirit", 50421, ParserHelper.Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Sun Spirit", 50413, ParserHelper.Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Stone Spirit", 50415, ParserHelper.Source.Ranger, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png", 88541, ulong.MaxValue),
                new Buff("Storm Spirit", 50381, ParserHelper.Source.Ranger, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png", 88541, ulong.MaxValue),
                //skills
                new Buff("Attack of Opportunity",12574, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/47/Moment_of_Clarity.png"),
                new Buff("Call of the Wild",36781, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8d/Call_of_the_Wild.png",0 , 97950),
                new Buff("Call of the Wild",36781, ParserHelper.Source.Ranger, BuffStackType.Stacking, 3, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8d/Call_of_the_Wild.png",97950 , 102321),
                new Buff("Strength of the Pack!",12554, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4b/%22Strength_of_the_Pack%21%22.png"),
                new Buff("Sic 'Em!",33902, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png"),
                new Buff("Sic 'Em! (PvP)",56923, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png"),
                new Buff("Sharpening Stones",12536, ParserHelper.Source.Ranger, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/af/Sharpening_Stone.png"),
                new Buff("Sharpen Spines",43266, ParserHelper.Source.Ranger, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/95/Sharpen_Spines.png"),
                //traits
                new Buff("Spotter", 14055, ParserHelper.Source.Ranger, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/b/b0/Spotter.png"),
                new Buff("Opening Strike",13988, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/44/Opening_Strike_%28effect%29.png"),
                new Buff("Quick Draw",29703, ParserHelper.Source.Ranger, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/39/Quick_Draw.png"),
                new Buff("Light on your Feet",30673, ParserHelper.Source.Ranger, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/22/Light_on_your_Feet.png"),
        };


        public static void AttachMasterToRangerGadgets(List<Player> players, Dictionary<long, List<AbstractDamageEvent>> damageData, Dictionary<long, List<AbstractCastEvent>> castData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            // entangle works fine already
            HashSet<AgentItem> jacarandaEmbraces = ProfHelper.GetOffensiveGadgetAgents(damageData, 1286, playerAgents);
            HashSet<AgentItem> blackHoles = ProfHelper.GetOffensiveGadgetAgents(damageData, 31436, playerAgents);
            var rangers = players.Where(x => x.Prof == "Ranger" || x.Prof == "Soulbeast" || x.Prof == "Druid").ToList();
            // if only one ranger, could only be that one
            if (rangers.Count == 1)
            {
                Player ranger = rangers[0];
                ProfHelper.SetGadgetMaster(jacarandaEmbraces, ranger.AgentItem);
                ProfHelper.SetGadgetMaster(blackHoles, ranger.AgentItem);
            }
            else if (rangers.Count > 1)
            {
                ProfHelper.AttachMasterToGadgetByCastData(castData, jacarandaEmbraces, new List<long> { 44980 }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(castData, blackHoles, new List<long> { 31503 }, 1000);
            }
        }

    }
}
