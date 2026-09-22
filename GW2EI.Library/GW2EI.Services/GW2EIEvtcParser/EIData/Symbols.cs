namespace GW2EIEvtcParser.EIData;

internal class Symbol(string symbol)
{
    public readonly string Str = symbol;
}

internal static class Symbols
{
    // Don't work
    //public static Symbol Asterisk = new Symbol("asterisk");
    //public static Symbol AsteriskOpen = new Symbol("asterisk-open");
    public static Symbol Bowtie = new("bowtie");
    public static Symbol BowtieOpen = new("bowtie-open");
    public static Symbol X = new("x");
    public static Symbol XThinOpen = new("x-thin-open");
    public static Symbol Y = new("y");
    public static Symbol YUp = new("y-up");
    public static Symbol YUpOpen = new("y-up-open");
    public static Symbol YDown = new("y-down");
    public static Symbol YDownOpen = new("y-down-open");
    public static Symbol Circle = new("circle");
    public static Symbol CircleOpen = new("circle-open");
    public static Symbol CircleOpenDot = new("circle-open-dot");
    public static Symbol CircleX = new("circle-x");
    public static Symbol CircleXOpen = new("circle-x-open");
    public static Symbol CircleCross = new("circle-cross");
    public static Symbol CircleCrossOpen = new("circle-cross-open");
    public static Symbol Square = new("square");
    public static Symbol SquareOpen = new("square-open");
    public static Symbol Cross = new("cross");
    public static Symbol CrossOpen = new("cross-open");
    public static Symbol TriangleLeft = new("triangle-left");
    public static Symbol TriangleLeftOpen = new("triangle-left-open");
    public static Symbol TriangleRight = new("triangle-right");
    public static Symbol TriangleRightOpen = new("triangle-right-open");
    public static Symbol TriangleUp = new("triangle-up");
    public static Symbol TriangleUpOpen = new("triangle-up-open");
    public static Symbol TriangleDown = new("triangle-down");
    public static Symbol TriangleDownOpen = new("triangle-down-open");
    public static Symbol TriangleSW = new("triangle-sw");
    public static Symbol TriangleSWOpen = new("triangle-sw-open");
    public static Symbol TriangleNW = new("triangle-nw");
    public static Symbol TriangleNWOpen = new("triangle-nw-open");
    public static Symbol TriangleSE = new("triangle-se");
    public static Symbol TriangleSEOpen = new("triangle-se-open");
    public static Symbol TriangleNE = new("triangle-ne");
    public static Symbol TriangleNEOpen = new("triangle-ne-open");
    public static Symbol DiamondTall = new("diamond-tall");
    public static Symbol DiamondWide = new("diamond-wide");
    public static Symbol DiamondWideOpen = new("diamond-wide-open");
    public static Symbol Diamond = new("diamond");
    public static Symbol DiamondOpen = new("diamond-open");
    public static Symbol Star = new("star");
    public static Symbol StarOpen = new("star-open");
    public static Symbol StarDiamond = new("star-diamond");
    public static Symbol StarDiamondOpen = new("star-diamond-open");
    public static Symbol StarTriangleUp = new("star-triangle-up");
    public static Symbol StarTriangleUpOpen = new("star-triangle-up-open");
    public static Symbol StarTriangleDown = new("star-triangle-down");
    public static Symbol StarTriangleDownOpen = new("star-triangle-down-open");
    public static Symbol StarSquare = new("star-square");
    public static Symbol StarSquareOpen = new("star-square-open");
    public static Symbol StarSquareOpenDot = new("star-square-open-dot");
    public static Symbol Pentagon = new("pentagon");
    public static Symbol PentagonOpen = new("pentagon-open");
    public static Symbol Hexagon = new("hexagon");
    public static Symbol HexagonOpen = new("hexagon-open");
    public static Symbol Hexagram = new("hexagram");
    public static Symbol HexagramOpen = new("hexagram-open");
    public static Symbol Hourglass = new("hourglass");
    public static Symbol HourglassOpen = new("hourglass-open");
    public static Symbol Octagon = new("octagon");
}
