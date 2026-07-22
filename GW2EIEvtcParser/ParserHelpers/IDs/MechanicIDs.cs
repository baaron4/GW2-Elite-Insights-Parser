namespace GW2EIEvtcParser;

/// <summary>
/// Pool of mechanic IDs used in the parser, always custom.
/// <para>Naming convention: </para>
/// <list type="bullet">
/// <item>No "id" inside the name.</item>
/// <item>Value must be strictly positive</item>
/// <item>Prefix the name with "Mech_"</item>
/// <item>A masking system is in place, based on categories (Raid, Fractal, ...)</item>
/// </list>
/// </summary>
public static class MechanicIDs
{
    private const int CommonMask = 0x01000000;
    private const int RaidWingMask = 0x02000000;
    private const int FractalMask = 0x03000000;
    private const int RaidEncounterMask = 0x04000000;
    private const int OpenWorldMask = 0x05000000;
    private const int StoryInstanceMask = 0x06000000;
    private const int WvWMask = 0x07000000;
    private const int GolemMask = 0x08000000;
    private const int ConvergenceMask = 0x09000000;

    private const int SpiritValeMask = 0x00010000;
    private const int SalvationPassMask = 0x00020000;
    private const int StrongholdOfTheFaithfulMask = 0x00030000;
    private const int BastionOfThePenitentMask = 0x00040000;
    private const int HallOfChainsMask = 0x00050000;
    private const int MythwrightGambitMask = 0x00060000;
    private const int TheKeyOfAhdashimMask = 0x00070000;
    private const int MountBalriorMask = 0x00080000;

    private const int NightmareMask = 0x00010000;
    private const int ShatteredObservatoryMask = 0x00020000;
    private const int SunquaPeakMask = 0x00030000;
    private const int SilentSurfMask = 0x00040000;
    private const int LonelyTowerMask = 0x00050000;
    private const int KinfallMask = 0x00060000;

    private const int FestivalMask = 0x00010000;
    private const int IBSMask = 0x00020000;
    private const int EODMask = 0x00030000;
    private const int CoreMask = 0x00040000;
    private const int SotOMask = 0x00050000;
    private const int VoEMask = 0x00060000;

    private const int EternalBattlegroundsMask = 0x00010000;
    private const int GreenAlpineBorderlandsMask = 0x00020000;
    private const int BlueAlpineBorderlandsMask = 0x00030000;
    private const int RedDesertBorderlandsMask = 0x00040000;
    private const int ObsidianSanctumMask = 0x00050000;
    private const int EdgeOfTheMistsMask = 0x00060000;
    private const int ArmisticeBastionMask = 0x00070000;
    private const int GildedHollowMask = 0x00080000;
    private const int LostPrecipiceMask = 0x00090000;
    private const int WindsweptHavenMask = 0x000A0000;
    private const int IsleOfReflectionMask = 0x000B0000;

    private const int OuterNayosConvMask = 0x00010000;
    private const int MountBalriorConvMask = 0x00020000;


    #region COMMONS
    public const int Mech_PlayerDead = CommonMask | 1;
    public const int Mech_PlayerDowned = CommonMask | 2;
    public const int Mech_PlayerUp = CommonMask | 3;
    public const int Mech_PlayerRes = CommonMask | 4;
    public const int Mech_PlayerDC = CommonMask | 5;
    public const int Mech_PlayerSpawn = CommonMask | 6;
    public const int Mech_PlayerKD = CommonMask | 7;
    public const int Mech_PlayerKBP = CommonMask | 8;
    public const int Mech_PlayerFloat = CommonMask | 9;
    public const int Mech_PlayerLaunch = CommonMask | 10;
    public const int Mech_PlayerLockOut = CommonMask | 11;
    public const int Mech_PlayerFloatSinkWater = CommonMask | 12;
    #endregion COMMONS

    #region CONVERGENCE
    public const int Mech_EssenceCollected = ConvergenceMask | 1;
    #endregion CONVERGENCE
}
