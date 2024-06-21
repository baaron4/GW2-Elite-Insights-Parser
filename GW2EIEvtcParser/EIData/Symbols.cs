namespace GW2EIEvtcParser.EIData
{
    internal class Symbol
    {
        public string Str { get; }

        public Symbol(string symbol)
        {
            Str = symbol;
        }
    }

    internal static class Symbols
    {
        // Don't work
        //public static Symbol Asterisk = new Symbol("asterisk");
        //public static Symbol AsteriskOpen = new Symbol("asterisk-open");
        public static Symbol Bowtie = new Symbol("bowtie");
        public static Symbol BowtieOpen = new Symbol("bowtie-open");
        public static Symbol X = new Symbol("x");
        public static Symbol XThinOpen = new Symbol("x-thin-open");
        public static Symbol Y = new Symbol("y");
        public static Symbol YUp = new Symbol("y-up");
        public static Symbol YUpOpen = new Symbol("y-up-open");
        public static Symbol YDown = new Symbol("y-down");
        public static Symbol YDownOpen = new Symbol("y-down-open");
        public static Symbol Circle = new Symbol("circle");
        public static Symbol CircleOpen = new Symbol("circle-open");
        public static Symbol CircleOpenDot = new Symbol("circle-open-dot");
        public static Symbol CircleX = new Symbol("circle-x");
        public static Symbol CircleXOpen = new Symbol("circle-x-open");
        public static Symbol CircleCross = new Symbol("circle-cross");
        public static Symbol CircleCrossOpen = new Symbol("circle-cross-open");
        public static Symbol Square = new Symbol("square");
        public static Symbol SquareOpen = new Symbol("square-open");
        public static Symbol Cross = new Symbol("cross");
        public static Symbol CrossOpen = new Symbol("cross-open");
        public static Symbol TriangleLeft = new Symbol("triangle-left");
        public static Symbol TriangleLeftOpen = new Symbol("triangle-left-open");
        public static Symbol TriangleRight = new Symbol("triangle-right");
        public static Symbol TriangleRightOpen = new Symbol("triangle-right-open");
        public static Symbol TriangleUp = new Symbol("triangle-up");
        public static Symbol TriangleUpOpen = new Symbol("triangle-up-open");
        public static Symbol TriangleDown = new Symbol("triangle-down");
        public static Symbol TriangleDownOpen = new Symbol("triangle-down-open");
        public static Symbol TriangleSW = new Symbol("triangle-sw");
        public static Symbol TriangleSWOpen = new Symbol("triangle-sw-open");
        public static Symbol TriangleNW = new Symbol("triangle-nw");
        public static Symbol TriangleNWOpen = new Symbol("triangle-nw-open");
        public static Symbol TriangleSE = new Symbol("triangle-se");
        public static Symbol TriangleSEOpen = new Symbol("triangle-se-open");
        public static Symbol TriangleNE = new Symbol("triangle-ne");
        public static Symbol TriangleNEOpen = new Symbol("triangle-ne-open");
        public static Symbol DiamondTall = new Symbol("diamond-tall");
        public static Symbol DiamondWide = new Symbol("diamond-wide");
        public static Symbol DiamondWideOpen = new Symbol("diamond-wide-open");
        public static Symbol Diamond = new Symbol("diamond");
        public static Symbol DiamondOpen = new Symbol("diamond-open");
        public static Symbol Star = new Symbol("star");
        public static Symbol StarOpen = new Symbol("star-open");
        public static Symbol StarDiamond = new Symbol("star-diamond");
        public static Symbol StarDiamondOpen = new Symbol("star-diamond-open");
        public static Symbol StarTriangleUp = new Symbol("star-triangle-up");
        public static Symbol StarTriangleUpOpen = new Symbol("star-triangle-up-open");
        public static Symbol StarTriangleDown = new Symbol("star-triangle-down");
        public static Symbol StarTriangleDownOpen = new Symbol("star-triangle-down-open");
        public static Symbol StarSquare = new Symbol("star-square");
        public static Symbol StarSquareOpen = new Symbol("star-square-open");
        public static Symbol StarSquareOpenDot = new Symbol("star-square-open-dot");
        public static Symbol Pentagon = new Symbol("pentagon");
        public static Symbol PentagonOpen = new Symbol("pentagon-open");
        public static Symbol Hexagon = new Symbol("hexagon");
        public static Symbol HexagonOpen = new Symbol("hexagon-open");
        public static Symbol Hexagram = new Symbol("hexagram");
        public static Symbol HexagramOpen = new Symbol("hexagram-open");
        public static Symbol Hourglass = new Symbol("hourglass");
        public static Symbol HourglassOpen = new Symbol("hourglass-open");
        public static Symbol Octagon = new Symbol("octagon");
    }
}
