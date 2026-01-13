namespace GW2EIEvtcParser.ParserHelpers;

/// <summary>
/// List of game builds that brought changes used by Elite Insights.
/// </summary>
public static class GW2Builds
{
    public const ulong StartOfLife = ulong.MinValue;

    // https://wiki.guildwars2.com/wiki/Game_updates/2015-10-23
    public const ulong HoTRelease = 54485;

    // https://wiki.guildwars2.com/wiki/Game_updates/2016-11-21
    public const ulong November2016NightmareRelease = 69591;

    // https://wiki.guildwars2.com/wiki/Game_updates/2017-02-22
    public const ulong February2017Balance = 72781;

    // https://wiki.guildwars2.com/wiki/Game_updates/2017-05-16
    public const ulong May2017Balance = 76706;

    // https://wiki.guildwars2.com/wiki/Game_updates/2017-07-25
    public const ulong July2017ShatteredObservatoryRelease = 79873;

    // https://wiki.guildwars2.com/wiki/Game_updates/2017-08-08
    public const ulong August2017Balance = 80647;

    // https://wiki.guildwars2.com/wiki/Game_updates/2017-09-22
    public const ulong September2017PathOfFireRelease = 82356;

    // https://wiki.guildwars2.com/wiki/Game_updates/2017-12-12
    public const ulong December2017Balance = 84794;

    // https://wiki.guildwars2.com/wiki/Game_updates/2018-02-06
    public const ulong February2018Balance = 86181;

    // https://wiki.guildwars2.com/wiki/Game_updates/2018-05-08
    public const ulong May2018Balance = 88541;

    // https://wiki.guildwars2.com/wiki/Game_updates/2018-07-10
    public const ulong July2018Balance = 90455;

    // https://wiki.guildwars2.com/wiki/Game_updates/2018-08-28
    public const ulong August2018Balance = 92069;

    // https://wiki.guildwars2.com/wiki/Game_updates/2018-10-02
    public const ulong October2018Balance = 92715;

    // https://wiki.guildwars2.com/wiki/Game_updates/2018-11-13
    public const ulong November2018Rune = 93543;

    // https://wiki.guildwars2.com/wiki/Game_updates/2018-12-11
    public const ulong December2018Balance = 94051;

    // https://wiki.guildwars2.com/wiki/Game_updates/2019-03-05
    public const ulong March2019Balance = 95535;

    // https://wiki.guildwars2.com/wiki/Game_updates/2019-04-23
    public const ulong April2019Balance = 96406;

    // https://wiki.guildwars2.com/wiki/Game_updates/2019-06-11
    public const ulong June2019RaidRewards = 97235;

    // https://wiki.guildwars2.com/wiki/Game_updates/2019-07-16
    public const ulong July2019Balance = 97950;

    // https://wiki.guildwars2.com/wiki/Game_updates/2019-07-30
    public const ulong July2019Balance2 = 98248;

    // https://wiki.guildwars2.com/wiki/Game_updates/2019-10-01
    public const ulong October2019Balance = 99526;

    // https://wiki.guildwars2.com/wiki/Game_updates/2019-12-03
    public const ulong December2019Balance = 100690;

    // https://wiki.guildwars2.com/wiki/Game_updates/2020-02-25
    public const ulong February2020Balance = 102321;

    // https://wiki.guildwars2.com/wiki/Game_updates/2020-02-26
    public const ulong February2020Balance2 = 102389;

    // https://wiki.guildwars2.com/wiki/Game_updates/2020-03-17
    public const ulong March2020Balance = 102724;

    // https://wiki.guildwars2.com/wiki/Game_updates/2020-07-07
    public const ulong July2020Balance = 104844;

    // https://wiki.guildwars2.com/wiki/Game_updates/2020-09-15
    public const ulong September2020SunquaPeakRelease = 106277;

    // https://wiki.guildwars2.com/wiki/Game_updates/2021-05-11
    public const ulong May2021Balance = 115190;

    // https://wiki.guildwars2.com/wiki/Game_updates/2021-05-25
    public const ulong May2021BalanceHotFix = 115728;

    // https://wiki.guildwars2.com/wiki/Game_updates/2021-06-08
    public const ulong June2021Balance = 116210;

    // https://wiki.guildwars2.com/wiki/Game_updates/2021-08-17
    public const ulong EODBeta1 = 118697;

    // https://wiki.guildwars2.com/wiki/Game_updates/2021-09-21
    public const ulong EODBeta2 = 119939;

    // https://wiki.guildwars2.com/wiki/Game_updates/2021-10-26
    public const ulong EODBeta3 = 121168;

    // https://wiki.guildwars2.com/wiki/Game_updates/2021-11-30
    public const ulong EODBeta4 = 122479;

    // https://wiki.guildwars2.com/wiki/Game_updates/2022-02-28
    public const ulong EODRelease = 125589;

    // https://wiki.guildwars2.com/wiki/Game_updates/2022-03-15
    public const ulong March2022Balance = 126520;

    // https://wiki.guildwars2.com/wiki/Game_updates/2022-03-29
    public const ulong March2022Balance2 = 127285;

    // https://wiki.guildwars2.com/wiki/Game_updates/2022-05-10
    public const ulong May2022Balance = 128773;

    // https://wiki.guildwars2.com/wiki/Game_updates/2022-06-28
    public const ulong June2022Balance = 130910;

    // https://wiki.guildwars2.com/wiki/Game_updates/2022-06-30
    public const ulong June2022BalanceHotFix = 131084;

    // https://wiki.guildwars2.com/wiki/Game_updates/2022-07-19
    public const ulong July2022FractalInstabilitiesRework = 131720;

    // https://wiki.guildwars2.com/wiki/Game_updates/2022-08-02
    public const ulong August2022BalanceHotFix = 132359;

    // https://wiki.guildwars2.com/wiki/Game_updates/2022-08-23
    public const ulong August2022Balance = 133322;

    // https://wiki.guildwars2.com/wiki/Game_updates/2022-10-04
    public const ulong October2022Balance = 135242;

    // https://wiki.guildwars2.com/wiki/Game_updates/2022-10-18
    public const ulong October2022BalanceHotFix = 135930;

    // https://wiki.guildwars2.com/wiki/Game_updates/2022-11-29
    public const ulong November2022Balance = 137943;

    // https://wiki.guildwars2.com/wiki/Game_updates/2023-02-14
    public const ulong February2023Balance = 141374;

    // https://wiki.guildwars2.com/wiki/Game_updates/2023-05-02
    public const ulong May2023Balance = 145038;

    // https://wiki.guildwars2.com/wiki/Game_updates/2023-05-23
    public const ulong May2023BalanceHotFix = 146069;

    // https://wiki.guildwars2.com/wiki/Game_updates/2023-06-27
    public const ulong June2023BalanceAndSOTOBetaAndSilentSurfNM = 147734;

    // https://wiki.guildwars2.com/wiki/Game_updates/2023-07-18
    public const ulong July2023BalanceAndSilentSurfCM = 148697;

    // https://wiki.guildwars2.com/wiki/Game_updates/2023-08-22
    public const ulong SOTOReleaseAndBalance = 150431;

    // https://wiki.guildwars2.com/wiki/Game_updates/2023-09-26
    public const ulong September2023Balance = 151966;

    // https://wiki.guildwars2.com/wiki/Game_updates/2023-11-07
    public const ulong DagdaNMHPChangedAndCMRelease = 153978;

    // https://wiki.guildwars2.com/wiki/Game_updates/2023-11-28
    public const ulong November2023Balance = 154949;

    // https://wiki.guildwars2.com/wiki/Game_updates/2024-01-30
    public const ulong January2024Balance = 157732;

    // https://wiki.guildwars2.com/wiki/Game_updates/2024-02-27
    public const ulong February2024NewWeapons = 158837;

    // https://wiki.guildwars2.com/wiki/Game_updates/2024-02-28
    public const ulong February2024CerusCMHPFix = 158968;

    // https://wiki.guildwars2.com/wiki/Game_updates/2024-03-19
    public const ulong March2024BalanceAndCerusLegendary = 159951;

    // https://wiki.guildwars2.com/wiki/Game_updates/2024-05-21
    public const ulong May2024LonelyTowerFractalRelease = 163141;

    // https://wiki.guildwars2.com/wiki/Game_updates/2024-06-04
    public const ulong June2024LonelyTowerCMRelease = 163807;

    // https://wiki.guildwars2.com/wiki/Game_updates/2024-06-25
    public const ulong June2024Balance = 164824;

    // https://wiki.guildwars2.com/wiki/Game_updates/2024-08-20
    public const ulong August2024JWRelease = 167136;

    // https://wiki.guildwars2.com/wiki/Game_updates/2024-10-08
    public const ulong October2024Balance = 169300;

    // https://wiki.guildwars2.com/wiki/Game_updates/2024-11-19
    public const ulong November2024MountBalriorRelease = 171452;

    // https://wiki.guildwars2.com/wiki/Game_updates/2024-12-10
    public const ulong December2024MountBalriorNerfs = 172309;

    // https://wiki.guildwars2.com/wiki/Game_updates/2025-02-11
    public const ulong February2025Balance = 175086;

    // https://wiki.guildwars2.com/wiki/Game_updates/2025-03-11
    public const ulong March2025W8CMReleaseAndNewCoreRelics = 176750;

    // https://wiki.guildwars2.com/wiki/Game_updates/2025-04-15
    public const ulong April2025Balance = 178947;

    // https://wiki.guildwars2.com/wiki/Game_updates/2025-05-06
    public const ulong May2025BalanceHotFix = 180154;

    // https://wiki.guildwars2.com/wiki/Game_updates/2025-06-24
    public const ulong June2025Balance = 182824;

    // https://wiki.guildwars2.com/wiki/Game_updates/2025-07-01
    public const ulong July2025BalanceHotFix = 183275;

    // https://wiki.guildwars2.com/wiki/Game_updates/2025-08-19
    public const ulong August2025VoEBeta = 186019;

    // https://wiki.guildwars2.com/wiki/Game_updates/2025-10-28
    public const ulong OctoberVoERelease = 190000;
    // https://wiki.guildwars2.com/wiki/Game_updates/2025-11-18
    public const ulong November2025Balance = 191204;
    // https://wiki.guildwars2.com/wiki/Game_updates/2025-12-09
    public const ulong December2025Balance = 192224;
    //https://wiki.guildwars2.com/wiki/Game_updates/2026-01-13
    public const ulong January2026Balance = 193778;

    public const ulong EndOfLife = ulong.MaxValue;
}
