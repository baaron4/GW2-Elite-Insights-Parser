using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser
{
    public static class MarkerGUIDs
    {
        // Commander Tag
        public const string RedCommanderTag = "4242F370667CE54EB3BF22BE8D06F986";
        public const string OrangeCommanderTag = "E57AAE9EE7FC5D458B0CF16BE4B096BF";
        public const string YellowCommanderTag = "AF9442A290C6214596E0B339EB3BDE92";
        public const string GreenCommanderTag = "74AD480E531F4740A407879976C8CA91";
        public const string CyanCommanderTag = "96F4AB5CDEC5294388375C7A03AB7614";
        public const string BlueCommanderTag = "AE714FC5E4EA464C8961CD78E86F9291";
        public const string PurpleCommanderTag = "1993FADB6FB70E4383A223A54D311F7D";
        public const string PinkCommanderTag = "E911D8C0EF2FDF4D8D252E5FB1283C62";
        public const string WhiteCommanderTag = "A59678CDFB5732439D7FCBF58D8BCEC3";
        // Catmander Tag
        public const string RedCatmanderTag = "CA76AB023593B0448F692FE29DF03D17";
        public const string OrangeCatmanderTag = "9FDF03E9BA09A2458C1EDDA4D81BC34D";
        public const string YellowCatmanderTag = "6BCE90E99016B448969EB317784A8334";
        public const string GreenCatmanderTag = "2CA226E07262C743BA193ACF6F9D0AF6";
        public const string CyanCatmanderTag = "A8072D65CE35924BABBAC831B12019D7";
        public const string BlueCatmanderTag = "9B94F0FD616E7F4AA58EFDC8C59FB689";
        public const string PurpleCatmanderTag = "7224A4AF710E4243BFE032629E17CA6E";
        public const string PinkCatmanderTag = "4387BE6146D43246AA7B333168EA58EA";
        public const string WhiteCatmanderTag = "A0B0EC076BC83B40A293C1CDEC4A7DE7";
        // Overhead Squad Markers
        public const string ArrowOverhead = "C3A56F1E045E3848B07CBAC5BBDD2C32";
        public const string CircleOverhead = "73C880AE431C9F4D8A5972ACF7066F4E";
        public const string HeartOverhead = "185008E2437B184D8FDAD647DD972D9F";
        public const string SquareOverhead = "6E5997457B3F6A45B984C613806FA72A";
        public const string StarOverhead = "5140125657C6084D94226C8EC0216649";
        public const string SwirlOverhead = "EBBE113AE2E53F4E96F3E92FB1353ECE";
        public const string TriangleOverhead = "46EBC4397F8A3740B900333B591F6183";
        public const string XOverhead = "8BDCF5C47F8A8340A251F102AF3B5905";

        /// <summary>
        /// HashSet containing the types of Commander Tag GUIDs.
        /// </summary>
        public static IReadOnlyCollection<string> CommanderTagMarkersGUIDs { get; set; } = new HashSet<string>()
        {
            RedCommanderTag,
            OrangeCommanderTag,
            YellowCommanderTag,
            GreenCommanderTag,
            CyanCommanderTag,
            BlueCommanderTag,
            PurpleCommanderTag,
            PinkCommanderTag,
            WhiteCommanderTag,
            RedCatmanderTag,
            OrangeCatmanderTag,
            YellowCatmanderTag,
            GreenCatmanderTag,
            CyanCatmanderTag,
            BlueCatmanderTag,
            PurpleCatmanderTag,
            PinkCatmanderTag,
            WhiteCatmanderTag,
        };

        /// <summary>
        /// HashSet containing the types of Squad Marker GUIDs.
        /// </summary>
        public static IReadOnlyCollection<string> SquadOverheadMarkersGUIDs { get; set; } = new HashSet<string>()
        {
            ArrowOverhead,
            CircleOverhead,
            HeartOverhead,
            SquareOverhead,
            StarOverhead,
            SwirlOverhead,
            TriangleOverhead,
            XOverhead
        };

        /// <summary>
        /// Matches the Squad Marker GUIDs to the relative icons.
        /// </summary>
        public static IReadOnlyDictionary<string, string> SquadMarkerToIcon { get; set; } = new Dictionary<string, string>()
        {
            { ArrowOverhead, ParserIcons.ArrowSquadMarkerOverhead },
            { CircleOverhead, ParserIcons.CircleSquadMarkerOverhead },
            { HeartOverhead, ParserIcons.HeartSquadMarkerOverhead },
            { SquareOverhead, ParserIcons.SquareSquadMarkerOverhead },
            { StarOverhead, ParserIcons.StarSquadMarkerOverhead },
            { SwirlOverhead, ParserIcons.SwirlSquadMarkerOverhead },
            { TriangleOverhead, ParserIcons.TriangleSquadMarkerOverhead },
            { XOverhead, ParserIcons.XSquadMarkerOverhead },
        };

        /// <summary>
        /// Matches the Commander/Catmander Tag GUIDs to the relative icons.
        /// </summary>
        public static IReadOnlyDictionary<string, string> CommanderTagToIcon { get; set; } = new Dictionary<string, string>()
        {
            { RedCommanderTag, ParserIcons.RedCommanderTagOverhead },
            { OrangeCommanderTag, ParserIcons.OrangeCommanderTagOverhead },
            { YellowCommanderTag, ParserIcons.YellowCommanderTagOverhead },
            { GreenCommanderTag, ParserIcons.GreenCommanderTagOverhead },
            { CyanCommanderTag, ParserIcons.CyanCommanderTagOverhead },
            { BlueCommanderTag, ParserIcons.BlueCommanderTagOverhead },
            { PurpleCommanderTag, ParserIcons.PurpleCommanderTagOverhead },
            { PinkCommanderTag, ParserIcons.PinkCommanderTagOverhead },
            { WhiteCommanderTag, ParserIcons.WhiteCommanderTagOverhead },
            { RedCatmanderTag, ParserIcons.RedCatmanderTagOverhead },
            { OrangeCatmanderTag, ParserIcons.OrangeCatmanderTagOverhead },
            { YellowCatmanderTag, ParserIcons.YellowCatmanderTagOverhead },
            { GreenCatmanderTag, ParserIcons.GreenCatmanderTagOverhead },
            { CyanCatmanderTag, ParserIcons.CyanCatmanderTagOverhead },
            { BlueCatmanderTag, ParserIcons.BlueCatmanderTagOverhead },
            { PurpleCatmanderTag, ParserIcons.PurpleCatmanderTagOverhead },
            { PinkCatmanderTag, ParserIcons.PinkCatmanderTagOverhead },
            { WhiteCatmanderTag, ParserIcons.WhiteCatmanderTagOverhead },
        };
    }

}
