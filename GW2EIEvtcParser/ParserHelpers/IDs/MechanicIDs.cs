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

    private const int FestivalMask = RaidEncounterMask | 0x00010000;
    private const int IBSMask = RaidEncounterMask | 0x00020000;
    private const int EODMask = RaidEncounterMask | 0x00030000;
    private const int CoreMask = RaidEncounterMask | 0x00040000;
    private const int SotOMask = RaidEncounterMask | 0x00050000;
    private const int VoEMask = RaidEncounterMask | 0x00060000;


    #region COMMONS
    private static int _commonCount = 0;
    public static readonly int Mech_PlayerDead = CommonMask | ++_commonCount;
    public static readonly int Mech_PlayerDowned = CommonMask | ++_commonCount;
    public static readonly int Mech_PlayerUp = CommonMask | ++_commonCount;
    public static readonly int Mech_PlayerRes = CommonMask | ++_commonCount;
    public static readonly int Mech_PlayerDC = CommonMask | ++_commonCount;
    public static readonly int Mech_PlayerSpawn = CommonMask | ++_commonCount;
    public static readonly int Mech_PlayerKD = CommonMask | ++_commonCount;
    public static readonly int Mech_PlayerKBP = CommonMask | ++_commonCount;
    public static readonly int Mech_PlayerFloat = CommonMask | ++_commonCount;
    public static readonly int Mech_PlayerLaunch = CommonMask | ++_commonCount;
    public static readonly int Mech_PlayerLockOut = CommonMask | ++_commonCount;
    public static readonly int Mech_PlayerFloatSinkWater = CommonMask | ++_commonCount;
    #endregion COMMONS

    #region FRACTALS
    private const int NightmareMask = FractalMask | 0x00010000;
    private const int ShatteredObservatoryMask = FractalMask | 0x00020000;
    private const int SunquaPeakMask = FractalMask | 0x00030000;
    private const int SilentSurfMask = FractalMask | 0x00040000;
    private const int LonelyTowerMask = FractalMask | 0x00050000;
    private const int KinfallMask = FractalMask | 0x00060000;

    private static int _fractalCount = 0;
    public static readonly int Mech_FluxBombBuff = FractalMask | ++_fractalCount;
    public static readonly int Mech_FluxBombHit = FractalMask | ++_fractalCount;
    public static readonly int Mech_FractalVindicator = FractalMask | ++_fractalCount;
    public static readonly int Mech_ToxicSicknessReceived = FractalMask | ++_fractalCount;
    public static readonly int Mech_ToxicSicknessApplied = FractalMask | ++_fractalCount;
    public static readonly int Mech_ToxicSicknessHitOther = FractalMask | ++_fractalCount;
    public static readonly int Mech_ToxicSicknessHitByOther = FractalMask | ++_fractalCount;
    #region KINFALL
    private static int _kinfallCount = 0;
    public static readonly int Mech_DeathlyRime = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_LifeFireApply = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_LifeFireRemove = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_VitreousSpiketHit = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_FaillingIceHit = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_FrozenTeethHit = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_LoftedCryOfFlashHit = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_TerrestrialCryOfFlashHit = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_GorefrostTarget = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_GorefrostHit = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_FreezingFanHit = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_LethalCoalescenceApply = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_WintryOrbHit = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_HailstormWhisperingShadowHit = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_EmpoweredWhsiperingShadow = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_ShatterstepLost = KinfallMask | ++_kinfallCount;
    public static readonly int Mech_ShatterstepKept = KinfallMask | ++_kinfallCount;
    #endregion KINFALL
    #region NIGHTMARE
    private static int _nightmareCount = 0;
    public static readonly int Mech_CascadeOfTorment = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_BlastWave = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_TantrumMAMA = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_LeapMAMA = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_ToxicShoot = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_KnightJump = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_SweepingStrikes = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_MiasmaMAMA = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_GrenadeBarrage = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_BulletsMAMA = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_Extraction = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_HomingGrenades = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_KnightsGaze = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_NightmareDevastationMAMA = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_VileSpit = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_TailLashSiax = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_HallucinationSpawnedSiax = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_CausticExplosionSiaxCastStart = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_CausticExplosionSiaxCastEnd = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_CausticExplosionSiaxBreakbarStart = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_CausticExplosionSiaxBreakbarEnd = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_HallucinationSiaxFixated = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_LungeNightmare = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_UpswingEnsolyss = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_UpswingHallucinationNightmare = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_MiasmaEnsolyss = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_CausticExplosionEnsolyssCastStart = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_CausticExplosionEnsolyssCastEndFail = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_CausticExplosionEnsolyssCastEndSuccess = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_CausticExplosionEnsolyssHit = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_NightmareDevastationEnsolyss = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_TailLashEnsolyss = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_RampageEnsolyss = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_CausticGraspEnsolyss = NightmareMask | ++_nightmareCount;
    public static readonly int Mech_TormentingBlastEnsolyss = NightmareMask | ++_nightmareCount;
    #endregion NIGHTMARE
    #region SHATTERED OBSERVATORY
    private static int _shatteredCount = 0;
    public static readonly int Mech_FixatedBloom = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_HitByEye = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_CorporealReassingment = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_CombustionRush = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_PunishingKick = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_CranialCascade = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_RadiantFury = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_FocusedAnger = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_HorizonStrikeSkorvald = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_CrimsonDawn = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_SolarCyclone = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_SkorvaldsIre = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_BloomExplode = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_SpiralStrike = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_WaveOfMutilation = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_VaultArtsariiv = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_SlamArtsariiv = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_TeleportLunge = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_AstralSurge = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_RedMarble = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_SparkSpawn = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_HorizonStrikeArkk = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_HorizonStrikeArkkNormal = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_SolarFury = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_SolarDischarge = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_SolarStomp = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_DiffractiveEdge = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_FocusedRage = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_StarburstCascade = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_OverheadSmashArkk = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_ExplodeArkk = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_CosmicMeteor = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_ArkkBreakbarStart = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_ArkkBreakbarFail = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_ArkkBreakbarSuccess = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_OverheadSmashArkkArchDiviner = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_RollingChaos = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_CosmicStreaks = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_WhirlingDevastation = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_PullArkkGladiatorStart = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_PullArkkGladiatorFail = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_PullArkkGladiatorSuccess = ShatteredObservatoryMask | ++_shatteredCount;
    public static readonly int Mech_SpinningCut = ShatteredObservatoryMask | ++_shatteredCount;
    #endregion SHATTERED OBSERVATORY
    #region SILENT SURF
    private static int _silentSurfCount = 0;
    public static readonly int Mech_RendingStorm = SilentSurfMask | ++_silentSurfCount;
    public static readonly int Mech_RendingStormTarget = SilentSurfMask | ++_silentSurfCount;
    public static readonly int Mech_Harrowshot = SilentSurfMask | ++_silentSurfCount;
    public static readonly int Mech_ExtremeVulnApply = SilentSurfMask | ++_silentSurfCount;
    public static readonly int Mech_DreadVisageDeath = SilentSurfMask | ++_silentSurfCount;
    public static readonly int Mech_FrighteningSpeedDeath = SilentSurfMask | ++_silentSurfCount;
    public static readonly int Mech_KanaxaiExposedPlayer = SilentSurfMask | ++_silentSurfCount;
    public static readonly int Mech_KanaxaiFear = SilentSurfMask | ++_silentSurfCount;
    public static readonly int Mech_Phantasmagoria = SilentSurfMask | ++_silentSurfCount;
    public static readonly int Mech_KanaxaiExposed = SilentSurfMask | ++_silentSurfCount;
    #endregion SILENT SURF
    #region LONELY TOWER
    private static int _lonelyTowerCount = 0;
    public static readonly int Mech_DespairAttunement = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_EnvyAttunement = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_GluttonyAttunement = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_MaliceAttunement = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_RageAttunement = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_RegretAttunement = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_DespairEmpowerment = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_EnvyEmpowerment = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_GluttonyEmpowerment = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_MaliceEmpowerment = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_RageEmpowerment = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_RegretEmpowerment = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_RainOfDespair = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_WaveOfEnvy = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_Inhale = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_SpikeOfMalice = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_RageFissure = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_Consumed = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_CruelDetonation = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_WallOfTalons = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_PoolOfDraining = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_UnliddedEye = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_EyeOfJudgment = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_EparchBreakbar = LonelyTowerMask | ++_lonelyTowerCount;
    public static readonly int Mech_EparchRegret = LonelyTowerMask | ++_lonelyTowerCount;
    #endregion LONELY TOWER
    #region SUNQUA PEAK
    private static int _sunquaPeakCount = 0;
    public static readonly int Mech_ElementalWhirl = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_ElementalManipulationAir = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_FulgorSphere = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_VolatileWind = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_WindBurst = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_WindBurstNoStab = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_CallOfStorms = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_WhirlwindShield = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_ElementalManipulationFire = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_RoilingFlames = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_VolatileFire = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_CallMeteorSummon = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_CallMeteorHit = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_FlameBurst = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_AiFirestorm = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_ElementalManipulationWater = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_TorrentialBolt = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_VolatileWater = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_AquaticBurst = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_TidalBarrier = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_TidalBargain = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_TidalBargainDown = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_EmpathicManipulation = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_FocusedWrath = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_NegativeBurst = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_Terrorstorm = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_CrushingGuilt = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_CrushingGuiltDown = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_FixatedByFear = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_EmpathicManipulationFear = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_EmpathicManipulationFearInterrupt = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_EmpathicManipulationSorrow = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_EmpathicManipulationSorrowInterrupt = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_EmpathicManipulationGuilt = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_EmpathicManipulationGuiltInterrupt = SunquaPeakMask | ++_sunquaPeakCount;
    public static readonly int Mech_CacophonousMind = SunquaPeakMask | ++_sunquaPeakCount;
    #endregion SUNQUA PEAK
    #endregion FRACTALS

    #region CONVERGENCE
    private const int OuterNayosConvMask = ConvergenceMask | 0x00010000;
    private const int MountBalriorConvMask = ConvergenceMask | 0x00020000;

    public static readonly int Mech_EssenceCollected = ConvergenceMask | 1;
    #endregion CONVERGENCE

    #region OPEN WORLD
    #region SOOWON
    private const int SooWonMask = OpenWorldMask | 0x00010000;

    private static int _sooWonCount = 0;
    public static readonly int Mech_SooWonSlam = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonAcidPool = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonClawSlap = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonTailSlap = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonBite = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonWaveHalf = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonWaveFull = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonWisp = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonGreenFailed = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonBubble = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonWhirlpool = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonTailSpawn = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonTailKilled = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonTailDespawn = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonSideSwap = SooWonMask | ++_sooWonCount;
    public static readonly int Mech_SooWonCC = SooWonMask | ++_sooWonCount;
    #endregion SOOWON
    #endregion OPEN WORLD

    #region WVW
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

    private static int _wvwCount = 0;
    public static readonly int Mech_KillingBlowPlayer = WvWMask | ++_wvwCount;
    public static readonly int Mech_KillingBlowEnemy = WvWMask | ++_wvwCount;
    #endregion WVW
}
