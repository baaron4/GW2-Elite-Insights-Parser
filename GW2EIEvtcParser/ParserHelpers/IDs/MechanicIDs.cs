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

    private const int SpiritValeMask = RaidWingMask | 0x00010000;
    private const int SalvationPassMask = RaidWingMask | 0x00020000;
    private const int StrongholdOfTheFaithfulMask = RaidWingMask | 0x00030000;
    private const int BastionOfThePenitentMask = RaidWingMask | 0x00040000;
    private const int HallOfChainsMask = RaidWingMask | 0x00050000;
    private const int MythwrightGambitMask = RaidWingMask | 0x00060000;
    private const int TheKeyOfAhdashimMask = RaidWingMask | 0x00070000;
    private const int MountBalriorMask = RaidWingMask | 0x00080000;

    private const int NightmareMask = FractalMask | 0x00010000;
    private const int ShatteredObservatoryMask = FractalMask | 0x00020000;
    private const int SunquaPeakMask = FractalMask | 0x00030000;
    private const int SilentSurfMask = FractalMask | 0x00040000;
    private const int LonelyTowerMask = FractalMask | 0x00050000;
    private const int KinfallMask = FractalMask | 0x00060000;

    private const int FestivalMask = RaidEncounterMask | 0x00010000;
    private const int IBSMask = RaidEncounterMask | 0x00020000;
    private const int EODMask = RaidEncounterMask | 0x00030000;
    private const int CoreMask = RaidEncounterMask | 0x00040000;
    private const int SotOMask = RaidEncounterMask | 0x00050000;
    private const int VoEMask = RaidEncounterMask | 0x00060000;

    private const int EternalBattlegroundsMask = WvWMask | 0x00010000;
    private const int GreenAlpineBorderlandsMask = WvWMask | 0x00020000;
    private const int BlueAlpineBorderlandsMask = WvWMask | 0x00030000;
    private const int RedDesertBorderlandsMask = WvWMask | 0x00040000;
    private const int ObsidianSanctumMask = WvWMask | 0x00050000;
    private const int EdgeOfTheMistsMask = WvWMask | 0x00060000;
    private const int ArmisticeBastionMask = WvWMask | 0x00070000;
    private const int GildedHollowMask = WvWMask | 0x00080000;
    private const int LostPrecipiceMask = WvWMask | 0x00090000;
    private const int WindsweptHavenMask = WvWMask | 0x000A0000;
    private const int IsleOfReflectionMask = WvWMask | 0x000B0000;

    private const int OuterNayosConvMask = ConvergenceMask | 0x00010000;
    private const int MountBalriorConvMask = ConvergenceMask | 0x00020000;


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

    #region FRACTALS
    #region KINFALL
    public const int Mech_DeathlyRime = KinfallMask | 1;
    public const int Mech_LifeFireApply = KinfallMask | 2;
    public const int Mech_LifeFireRemove = KinfallMask | 3;
    public const int Mech_VitreousSpiketHit = KinfallMask | 4;
    public const int Mech_FaillingIceHit = KinfallMask | 5;
    public const int Mech_FrozenTeethHit = KinfallMask | 6;
    public const int Mech_LoftedCryOfFlashHit = KinfallMask | 7;
    public const int Mech_TerrestrialCryOfFlashHit = KinfallMask | 8;
    public const int Mech_GorefrostTarget = KinfallMask | 9;
    public const int Mech_GorefrostHit = KinfallMask | 10;
    public const int Mech_FreezingFanHit = KinfallMask | 11;
    public const int Mech_LethalCoalescenceApply = KinfallMask | 12;
    public const int Mech_WintryOrbHit = KinfallMask | 13;
    public const int Mech_HailstormWhisperingShadowHit = KinfallMask | 14;
    public const int Mech_EmpoweredWhsiperingShadow = KinfallMask | 15;
    public const int Mech_ShatterstepLost = KinfallMask | 16;
    public const int Mech_ShatterstepKept = KinfallMask | 17;
    #endregion KINFALL
    #endregion FRACTALS

    #region CONVERGENCE
    public const int Mech_EssenceCollected = ConvergenceMask | 1;
    #endregion CONVERGENCE
}
