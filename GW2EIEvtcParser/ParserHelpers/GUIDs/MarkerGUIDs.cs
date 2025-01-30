namespace GW2EIEvtcParser;

public static class MarkerGUIDs
{
    // Commander Tag
    public static readonly GUID    RedCommanderTag = new("4242F370667CE54EB3BF22BE8D06F986");
    public static readonly GUID OrangeCommanderTag = new("E57AAE9EE7FC5D458B0CF16BE4B096BF");
    public static readonly GUID YellowCommanderTag = new("AF9442A290C6214596E0B339EB3BDE92");
    public static readonly GUID  GreenCommanderTag = new("74AD480E531F4740A407879976C8CA91");
    public static readonly GUID   CyanCommanderTag = new("96F4AB5CDEC5294388375C7A03AB7614");
    public static readonly GUID   BlueCommanderTag = new("AE714FC5E4EA464C8961CD78E86F9291");
    public static readonly GUID PurpleCommanderTag = new("1993FADB6FB70E4383A223A54D311F7D");
    public static readonly GUID   PinkCommanderTag = new("E911D8C0EF2FDF4D8D252E5FB1283C62");
    public static readonly GUID  WhiteCommanderTag = new("A59678CDFB5732439D7FCBF58D8BCEC3");
    // Catmander Tag
    public static readonly GUID    RedCatmanderTag = new("CA76AB023593B0448F692FE29DF03D17");
    public static readonly GUID OrangeCatmanderTag = new("9FDF03E9BA09A2458C1EDDA4D81BC34D");
    public static readonly GUID YellowCatmanderTag = new("6BCE90E99016B448969EB317784A8334");
    public static readonly GUID  GreenCatmanderTag = new("2CA226E07262C743BA193ACF6F9D0AF6");
    public static readonly GUID   CyanCatmanderTag = new("A8072D65CE35924BABBAC831B12019D7");
    public static readonly GUID   BlueCatmanderTag = new("9B94F0FD616E7F4AA58EFDC8C59FB689");
    public static readonly GUID PurpleCatmanderTag = new("7224A4AF710E4243BFE032629E17CA6E");
    public static readonly GUID   PinkCatmanderTag = new("4387BE6146D43246AA7B333168EA58EA");
    public static readonly GUID  WhiteCatmanderTag = new("A0B0EC076BC83B40A293C1CDEC4A7DE7");
    // Overhead Squad Markers
    public static readonly GUID    ArrowOverhead = new("C3A56F1E045E3848B07CBAC5BBDD2C32");
    public static readonly GUID   CircleOverhead = new("73C880AE431C9F4D8A5972ACF7066F4E");
    public static readonly GUID    HeartOverhead = new("185008E2437B184D8FDAD647DD972D9F");
    public static readonly GUID   SquareOverhead = new("6E5997457B3F6A45B984C613806FA72A");
    public static readonly GUID     StarOverhead = new("5140125657C6084D94226C8EC0216649");
    public static readonly GUID    SwirlOverhead = new("EBBE113AE2E53F4E96F3E92FB1353ECE");
    public static readonly GUID TriangleOverhead = new("46EBC4397F8A3740B900333B591F6183");
    public static readonly GUID        XOverhead = new("8BDCF5C47F8A8340A251F102AF3B5905");

    //TODO(Rennorb) @perf: Potential to use sorted lists instead for unconditional O(log(n)), instaed of computing hashes.

    /// <summary>
    /// HashSet containing the types of Commander Tag GUIDs.
    /// </summary>
    public static readonly IReadOnlyCollection<GUID> CommanderTagMarkersHexGUIDs = new HashSet<GUID>()
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
    public static readonly IReadOnlyCollection<GUID> SquadOverheadMarkersHexGUIDs = new HashSet<GUID>()
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

    // Other
    internal static readonly GUID UraTitanspawnGeyserMarker = new("2818A92C67388940B74D627979000D39");
}
