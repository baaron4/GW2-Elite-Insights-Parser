namespace GW2EIEvtcParser;

public static class MarkerGUIDs
{
    // Commander Tag
    public static readonly Guid RedCommanderTag = new("4242F370667CE54EB3BF22BE8D06F986");
    public static readonly Guid OrangeCommanderTag = new("E57AAE9EE7FC5D458B0CF16BE4B096BF");
    public static readonly Guid YellowCommanderTag = new("AF9442A290C6214596E0B339EB3BDE92");
    public static readonly Guid GreenCommanderTag = new("74AD480E531F4740A407879976C8CA91");
    public static readonly Guid CyanCommanderTag = new("96F4AB5CDEC5294388375C7A03AB7614");
    public static readonly Guid BlueCommanderTag = new("AE714FC5E4EA464C8961CD78E86F9291");
    public static readonly Guid PurpleCommanderTag = new("1993FADB6FB70E4383A223A54D311F7D");
    public static readonly Guid PinkCommanderTag = new("E911D8C0EF2FDF4D8D252E5FB1283C62");
    public static readonly Guid WhiteCommanderTag = new("A59678CDFB5732439D7FCBF58D8BCEC3");
    // Catmander Tag
    public static readonly Guid RedCatmanderTag = new("CA76AB023593B0448F692FE29DF03D17");
    public static readonly Guid OrangeCatmanderTag = new("9FDF03E9BA09A2458C1EDDA4D81BC34D");
    public static readonly Guid YellowCatmanderTag = new("6BCE90E99016B448969EB317784A8334");
    public static readonly Guid GreenCatmanderTag = new("2CA226E07262C743BA193ACF6F9D0AF6");
    public static readonly Guid CyanCatmanderTag = new("A8072D65CE35924BABBAC831B12019D7");
    public static readonly Guid BlueCatmanderTag = new("9B94F0FD616E7F4AA58EFDC8C59FB689");
    public static readonly Guid PurpleCatmanderTag = new("7224A4AF710E4243BFE032629E17CA6E");
    public static readonly Guid PinkCatmanderTag = new("4387BE6146D43246AA7B333168EA58EA");
    public static readonly Guid WhiteCatmanderTag = new("A0B0EC076BC83B40A293C1CDEC4A7DE7");
    // Overhead Squad Markers
    public static readonly Guid ArrowOverhead = new("C3A56F1E045E3848B07CBAC5BBDD2C32");
    public static readonly Guid CircleOverhead = new("73C880AE431C9F4D8A5972ACF7066F4E");
    public static readonly Guid HeartOverhead = new("185008E2437B184D8FDAD647DD972D9F");
    public static readonly Guid SquareOverhead = new("6E5997457B3F6A45B984C613806FA72A");
    public static readonly Guid StarOverhead = new("5140125657C6084D94226C8EC0216649");
    public static readonly Guid SwirlOverhead = new("EBBE113AE2E53F4E96F3E92FB1353ECE");
    public static readonly Guid TriangleOverhead = new("46EBC4397F8A3740B900333B591F6183");
    public static readonly Guid XOverhead = new("8BDCF5C47F8A8340A251F102AF3B5905");

    //TODO_PERF(Rennorb): Potential to use sorted lists instead for unconditional O(log(n)), instaed of computing hashes.

    /// <summary>
    /// HashSet containing the types of Commander Tag GUIDs.
    /// </summary>
    public static readonly IReadOnlyCollection<Guid> CommanderTagMarkersHexGUIDs = new HashSet<Guid>()
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
    public static readonly IReadOnlyCollection<Guid> SquadOverheadMarkersHexGUIDs = new HashSet<Guid>()
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
    // Artsariiv
    internal static readonly Guid ArtsariivTripleLaserEyeMarker = new("7701D3BA0508E3418D250F22FE407CDA");
    // Sabetha
    internal static readonly Guid SabethaCannonRedCrossSwordsMarker = new("09C2D6DFE68B004D928B1C76EF77D47E");
    // Bandit Trio
    internal static readonly Guid BanditTrioCageMarker = new("65476B6DABCA3B4E98ABC274989CB6F8");
    // Ura
    internal static readonly Guid UraTitanspawnGeyserMarker = new("2818A92C67388940B74D627979000D39");
    internal static readonly Guid UraBloodstoneShardMarker = new("27BBF2F636CEE64683A0AF5BEB7A9E54");
    internal static readonly Guid QadimLampMarker = new("0D2F04BB061CC5478C3126CBF9061F06");
}
