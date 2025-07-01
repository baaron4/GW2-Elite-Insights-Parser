using System.Numerics;

namespace GW2EIEvtcParser;

/// <summary>
/// NPC and Gadget IDs.<br></br>
/// Gadgets are stabilized with custom IDs.
/// </summary>
public static class SpeciesIDs
{
    public enum TargetID : int
    {
        // Raids
        Glenna = 15014,
        // - Wing 1
        // - Vale Guardian
        ValeGuardian = 15438,
        Seekers = 15426,
        RedGuardian = 15433,
        BlueGuardian = 15431,
        GreenGuardian = 15420,
        // - Spirit Race
        EtherealBarrier = SpeciesIDs.EtherealBarrier,
        _EtherealBarrier1 = SpeciesIDs.EtherealBarrier1,
        _EtherealBarrier2 = SpeciesIDs.EtherealBarrier2,
        _EtherealBarrier3 = SpeciesIDs.EtherealBarrier3,
        _EtherealBarrier4 = SpeciesIDs.EtherealBarrier4,
        EtherealBarrierGadget = 47188, // Gadget
        WallOfGhosts = 15415,
        AngeredSpiritSR = 15389,
        AngeredSpiritSR2 = 15409,
        DerangedSpiritSR = 15390,
        DerangedSpiritSR2 = 15425,
        EnragedSpiritSR = 15414,
        // - Gorseval
        Gorseval = 15429,
        ChargedSoul = 15434,
        EnragedSpirit = 16024,
        AngeredSpirit = 16005,
        // - Sabetha
        Sabetha = 15375,
        Kernan = 15372,
        Knuckles = 15404,
        Karde = 15430,
        BanditSapper = 15423,
        BanditThug = 15397,
        BanditArsonist = 15421,
        Cannon = SpeciesIDs.Cannon,
        HeavyBomb = SpeciesIDs.HeavyBomb,
        // - Wing 2
        // - Slothasor
        Slothasor = 16123,
        Slubling1 = 16064,
        Slubling2 = 16071,
        Slubling3 = 16077,
        Slubling4 = 16104,
        PoisonMushroom = SpeciesIDs.PoisonMushroom,
        // - Bandit Trio
        Berg = 16088,
        Zane = 16137,
        Narella = 16125,
        Cage = SpeciesIDs.Cage,
        Bombs = SpeciesIDs.Bombs,
        BanditSaboteur = 16117,
        Warg = 7481,
        VeteranTorturedWarg = 16129,
        BanditAssassin = 16067,
        BanditAssassin2 = 16113,
        BanditSapperTrio = 16074,
        BanditDeathsayer = 16076,
        BanditDeathsayer2 = 16080,
        BanditBrawler = 16066,
        BanditBrawler2 = 16119,
        BanditBattlemage = 16093,
        BanditBattlemage2 = 16100,
        BanditCleric = 16101,
        BanditCleric2 = 16060,
        BanditBombardier = 16138,
        BanditSniper = 16065,
        NarellaTornado = 16092,
        OilSlick = 16096,
        InsectSwarm = 16120,
        Prisoner1 = 16056,
        Prisoner2 = 16103,
        // - Matthias
        Matthias = 16115,
        Spirit = 16105,
        Spirit2 = 16114,
        IcePatch = 16139,
        Storm = 16108,
        Tornado = 16068,
        MatthiasSacrificeCrystal = MatthiasSacrifice,
        // - Wing 3
        // - Escort
        McLeodTheSilent = 16253,
        MushroomSpikeThrower = 16219,
        MushroomKing = 16255,
        MushroomCharger = 16224,
        WhiteMantleBattleMage1Escort = 16229,
        WhiteMantleBattleMage2Escort = 16240,
        WhiteMantleBattleCultist1 = 16265,
        WhiteMantleBattleCultist2 = 16281,
        WhiteMantleBattleKnight1 = 16242,
        WhiteMantleBattleKnight2 = 16220,
        WhiteMantleBattleCleric1 = 16272,
        WhiteMantleBattleCleric2 = 16266,
        WhiteMantleBattleSeeker1 = 16288,
        WhiteMantleBattleSeeker2 = 16256,
        WargBloodhound = 16222,
        RadiantMcLeod = 16234,
        CrimsonMcLeod = 16241,
        Mine = SpeciesIDs.Mine,
        // - Keep Construct
        KeepConstruct = 16235,
        Olson = 16244,
        Engul = 16274,
        Faerla = 16264,
        Caulle = 16282,
        Henley = 16236,
        Jessica = 16278,
        Galletta = 16228,
        Ianim = 16248,
        KeepConstructCore = 16261,
        GreenPhantasm = 16237,
        InsidiousProjection = 16227,
        UnstableLeyRift = 16277,
        RadiantPhantasm = 16259,
        CrimsonPhantasm = 16257,
        RetrieverProjection = 16249,
        // - Twisted Castle
        //CastleFountain = 32951,
        HauntingStatue = 16247,
        // - Xera
        Xera = 16246,
        Xera2 = 16286,
        BloodstoneShardMainFight = SpeciesIDs.BloodstoneShardMainFight,
        BloodstoneShardRift = SpeciesIDs.BloodstoneShardRift,
        BloodstoneShardButton = SpeciesIDs.BloodstoneShardButton,
        ChargedBloodstone = SpeciesIDs.ChargedBloodstone,
        BloodstoneFragment = SpeciesIDs.BloodstoneFragment,
        XerasPhantasm = 16225,
        WhiteMantleSeeker1 = 16238,
        WhiteMantleSeeker2 = 16283,
        WhiteMantleKnight1 = 16251,
        WhiteMantleKnight2 = 16287,
        WhiteMantleBattleMage1 = 16221,
        WhiteMantleBattleMage2 = 16226,
        ExquisiteConjunction = 16232,
        FakeXera = 16289,
        // - Wing 4
        // - Cairn
        Cairn = 17194,
        // - Mursaat Overseer
        MursaatOverseer = 17172,
        Jade = 17181,
        // - Samarog
        Samarog = 17188,
        Guldhem = 17208,
        Rigom = 17124,
        SpearAggressionRevulsion = SpeciesIDs.SpearAggressionRevulsion,
        // - Deimos
        Deimos = 17154,
        DeimosAttackTarget = SpeciesIDs.DeimosAttackTarget,
        DeimosBodyStruct = SpeciesIDs.DeimosBodyStruct,
        DeimosArmStruct = SpeciesIDs.DeimosArmStruct,
        Saul = 17126,
        ShackledPrisoner = SpeciesIDs.ShackledPrisoner,
        DemonicBond = SpeciesIDs.DemonicBond,
        DemonicBondAttackTarget = SpeciesIDs.DemonicBondAttackTarget,
        Thief = 17206,
        Gambler = 17335,
        GamblerClones = 17161,
        GamblerReal = 17355,
        Drunkard = 17163,
        Oil = 17332,
        Tear = 17303,
        Greed = 17213,
        Pride = 17233,
        Hands = 17221,
        // - Wing 5
        // - Soulless Horror
        SoullessHorror = 19767,
        TormentedDead = 19422,
        SurgingSoul = 19474,
        Scythe = 19396,
        FleshWurm = 19464,
        // - River of Souls
        Desmina = 19828,
        Enervator = 19863,
        HollowedBomber = 19399,
        RiverOfSouls = 19829,
        SpiritHorde1 = 19461,
        SpiritHorde2 = 19400,
        SpiritHorde3 = 19692,
        // - Statues of Ice
        BrokenKing = 19691,
        // - Statue of Death
        EaterOfSouls = 19536,
        OrbSpider = 19801,
        GreenSpirit1 = 19587,
        GreenSpirit2 = 19571,
        AscalonianPeasant1 = 19810,
        AscalonianPeasant2 = 19758,
        // - Statues of Darkness
        EyeOfJudgement = 19651,
        EyeOfFate = 19844,
        LightThieves = 19658,
        MazeMinotaur = 19402,
        // - Dhuum (Skeletons are the same as Spirit Hordes)
        Dhuum = 19450,
        Messenger = 19807,
        Echo = 19628,
        Enforcer = 19681,
        Deathling = 19759,
        UnderworldReaper = 19831,
        DhuumDesmina = 19481,
        YourSoul = SpeciesIDs.YourSoul,
        // - Wing 6
        // - Conjured Amalgamate
        ConjuredAmalgamate = 43974, // Gadget
        CARightArm = 10142, // Gadget
        CALeftArm = 37464, // Gadget
        ConjuredAmalgamate_CHINA = 44885, // Gadget
        CARightArm_CHINA = 11053, // Gadget
        CALeftArm_CHINA = 38375, // Gadget
        CARightArmAttackTarget = SpeciesIDs.CARightArmAttackTarget,
        CALeftArmAttackTarget = SpeciesIDs.CALeftArmAttackTarget,
        CABodyAttackTarget = SpeciesIDs.CABodyAttackTarget,
        ConjuredGreatsword = 21255,
        ConjuredShield = 21170,
        ConjuredPlayerSword = CASword,
        // - Twin Largos
        Nikare = 21105,
        Kenut = 21089,
        // - Qadim
        Qadim = 20934,
        LavaElemental1 = 21236,
        LavaElemental2 = 21078,
        IcebornHydra = 21163,
        GreaterMagmaElemental1 = 21150,
        GreaterMagmaElemental2 = 21223,
        FireElemental = 21221,
        FireImp = 21100,
        PyreGuardian = 21050,
        PyreGuardianRetal = SpeciesIDs.PyreGuardianRetal,
        PyreGuardianResolution = SpeciesIDs.PyreGuardianResolution,
        PyreGuardianProtect = SpeciesIDs.PyreGuardianProtect,
        PyreGuardianStab = SpeciesIDs.PyreGuardianStab,
        ReaperOfFlesh = 21218,
        DestroyerTroll = 20944,
        IceElemental = 21049,
        AncientInvokedHydra = 21285,
        ApocalypseBringer = 21073,
        WyvernMatriarch = 20997,
        WyvernPatriarch = 21183,
        QadimLamp = SpeciesIDs.QadimLamp,
        QadimPlatform = SpeciesIDs.QadimPlatform,
        AngryZommoros = 20961,
        ChillZommoros = 21118,
        AssaultCube = 21092,
        AwakenedSoldier = 21244,
        Basilisk = 21140,
        BlackMoa = 20980,
        BrandedCharr = 21083,
        BrandedDevourer = 21053,
        ChakDrone = 21064,
        CrazedKarkaHatchling = 21040,
        FireImpLamp = 21173,
        GhostlyPirateFighter = 21257,
        GiantBrawler = 21288,
        GiantHunter = 20972,
        GoldOoze = 21264,
        GrawlBascher = 21145,
        GrawlTrapper = 21290,
        GuildInitiateModusSceleris = 21161,
        IcebroodAtrocity = 16504,
        IcebroodKodan = 20975,
        IcebroodQuaggan = 21196,
        Jotun = 21054,
        JungleWurm = 21147,
        Karka = 21192,
        MinotaurBull = 20969,
        ModnirrBerserker = 20951,
        MoltenDisaggregator = 21010,
        MoltenProtector = 21037,
        MoltenReverberant = 20956,
        MordremVinetooth = 20940,
        Murellow = 21032,
        NightmareCourtier = 21261,
        OgreHunter = 21116,
        PirareSkrittSentry = 21189,
        PolarBear = 20968,
        Rabbit = 1085,
        ReefSkelk = 21024,
        RisenKraitDamoss = 21070,
        RottingAncientOakheart = 21252,
        RottingDestroyer = 21182,
        ShadowSkelk = 20966,
        SpiritOfExcess = 21095,
        TamedWarg = 18184,
        TarElemental = 21019,
        WindRider = 21164,
        // - Wing 7
        // - Adina
        Adina = 22006,
        HandOfErosion = SpeciesIDs.HandOfErosion,
        HandOfEruption = SpeciesIDs.HandOfEruption,
        // - Sabir
        Sabir = 21964,
        ParalyzingWisp = 21955,
        VoltaicWisp = 21975,
        SmallJumpyTornado = 21961,
        SmallKillerTornado = 21957,
        BigKillerTornado = 21987,
        SabirPlatform = 21998,
        SabirMainPlateform = SpeciesIDs.SabirMainPlateform,
        SabirSquarePlateform = SpeciesIDs.SabirSquarePlateform,
        SabirRectanglePlateform = SpeciesIDs.SabirRectanglePlateform,
        SabirBigRectanglePlateform = SpeciesIDs.SabirBigRectanglePlateform,
        // - Peerless Qadim
        PeerlessQadim = 22000,
        PeerlessQadimPylon = 21996,
        PeerlessQadimAuraPylon = 21962,
        EntropicDistortion = 21973,
        EnergyOrb = 21946,
        Brandstorm = 21978,
        GiantQadimThePeerless = 21953,
        DummyPeerlessQadim = 22005,
        // - Wing 8
        VeteranVentshot = 26799,
        EliteVentshot = 26766,
        ChampionVentshot = 26795,
        // - Greer
        Greer = 26725,
        Reeg = 26742,
        Gree = 26771,
        Ereg = 26859,
        EmpoweringBeast = 26776,
        ProtoGreerling = 26862,
        // - Decima
        Decima = 26774,
        DecimaCM = 26867,
        EnlightenedConduit = 26709,
        EnlightenedConduitGadget = SpeciesIDs.EnlightenedConduitGadget,
        BigEnlightenedConduitGadget = SpeciesIDs.BigEnlightenedConduitGadget,
        GreenOrb1Player = 26798,
        GreenOrb1PlayerCM = 26884,
        GreenOrb2Players = 26783,
        GreenOrb2PlayersCM = 26813,
        GreenOrb3Players = 26727,
        GreenOrb3PlayersCM = 26871,
        DecimaBeamEnd = 26793,
        DecimaBeamEndCM = 26845,
        DecimaBeamStart = 26708,
        DecimaBeamStartCM = 26858,
        EnlightenedConduitCM = 26826,
        TranscendentBoulder = 26856,
        // - Ura
        Ura = 26712,
        EliteFumaroller = 26797,
        ChampionFumaroller = 26744,
        ToxicGeyser = SpeciesIDs.ToxicGeyser,
        SulfuricGeyser = SpeciesIDs.SulfuricGeyser,
        TitanspawnGeyser = 26741,
        TitanspawnGeyserGadget = SpeciesIDs.TitanspawnGeyserGadget,
        UraGadget_BloodstoneShard = SpeciesIDs.UraGadget_BloodstoneShard,
        LegendaryVentshot = 26824,
        // Strike Missions
        // - Festival
        // - Freezie
        Freezie = 21333,
        FreeziesFrozenHeart = 21328,
        IceStormer = 21325,
        IceSpiker = 21337,
        IcyProtector = 21326,
        SnowPile = SpeciesIDs.SnowPile,
        // - Icebrood Saga
        // - Icebrood
        IcebroodConstruct = 22154,
        // - Voice and Claw
        VoiceOfTheFallen = 22343,
        ClawOfTheFallen = 22481,
        VoiceAndClaw = 22315,
        // - Fraenir
        FraenirOfJormag = 22492,
        IcebroodElemental = 22576,
        BoundIcebroodElemental = SpeciesIDs.BoundIcebroodElemental,
        IcebroodConstructFraenir = 22436,
        // - Boneskinner
        Boneskinner = 22521,
        PrioryExplorer = 22561,
        PrioryScholar = 22448,
        VigilRecruit = 22389,
        VigilTactician = 22420,
        AberrantWisp = 22538,
        Torch = SpeciesIDs.Torch,
        // - Whisper of Jormag
        WhisperOfJormag = 22711,
        WhisperEcho = 22628,
        DoppelgangerElementalist = 22627,
        DoppelgangerElementalist2 = 22691,
        DoppelgangerEngineer = 22625,
        DoppelgangerEngineer2 = 22699,
        DoppelgangerGuardian = 22608,
        DoppelgangerGuardian2 = 22635,
        DoppelgangerMesmer = 22683,
        DoppelgangerMesmer2 = 22721,
        DoppelgangerNecromancer = 22672,
        DoppelgangerNecromancer2 = 22713,
        DoppelgangerRanger = 22667,
        DoppelgangerRanger2 = 22678,
        DoppelgangerRevenant = 22610,
        DoppelgangerRevenant2 = 22615,
        DoppelgangerThief = 22612,
        DoppelgangerThief2 = 22656,
        DoppelgangerWarrior = 22640,
        DoppelgangerWarrior2 = 22717,
        // - Cold War
        VariniaStormsounder = 22836,
        PropagandaBallon = 23093,
        DominionBladestorm = 23102,
        DominionStalker = 22882,
        DominionSpy1 = 22833,
        DominionSpy2 = 22856,
        DominionAxeFiend = 22938,
        DominionEffigy = 22897,
        FrostLegionCrusher = 23005,
        FrostLegionMusketeer = 22870,
        BloodLegionBlademaster = 22993,
        CharrTank = 22953,
        SonsOfSvanirHighShaman = 22283,
        // - End of Dragons
        // - Aetherblade Hideout
        MaiTrinStrike = 24033,
        MaiTrinStrikeFake = SpeciesIDs.MaiTrinStrikeFake,
        EchoOfScarletBriarNM = 24768,
        EchoOfScarletBriarCM = 25247,
        MaiTrinStrikeDuringEcho = 23826,
        ScarletPhantom = 24404,
        ScarletPhantomBreakbar = 23656,
        ScarletPhantomHP = 24431,
        ScarletPhantomHPCM = 25262,
        ScarletPhantomBeamNM = 24396,
        ScarletPhantomDeathBeamCM = 25284,
        ScarletPhantomDeathBeamCM2 = 25287,
        FerrousBomb = SpeciesIDs.FerrousBomb,
        // - Xunlai Jade Junkyard
        Ankka = 23957,
        Ankka2 = 24634,
        KraitsHallucination = 24258,
        LichHallucination = 24158,
        QuaggansHallucinationNM = 24969,
        QuaggansHallucinationCM = 25289,
        ReanimatedMalice1 = 24976,
        ReanimatedMalice2 = 24171,
        ReanimatedSpite = 24348,
        ReanimatedHatred = 23673,
        ReanimatedAntipathy = 24827,
        ZhaitansReach = 23839,
        SanctuaryPrism = SpeciesIDs.SanctuaryPrism,
        // - Kaineng Overlook
        MinisterLi = 24485,
        MinisterLiCM = 24266,
        TheSniper = 23612,
        TheSniperCM = 25259,
        TheMechRider = 24660,
        TheMechRiderCM = 25271,
        TheEnforcer = 24261,
        TheEnforcerCM = 25236,
        TheRitualist = 23618,
        TheRitualistCM = 25242,
        TheMindblade = 24254,
        TheMindbladeCM = 25280,
        SpiritOfPain = 23793,
        SpiritOfDestruction = 23961,
        // - Harvest Temple - Void Amalgamate
        GadgetTheDragonVoid1 = 43488, // Gadget
        GadgetTheDragonVoid2 = 1378, // Gadget
        TheDragonVoidZhaitan = SpeciesIDs.TheDragonVoidZhaitan,
        TheDragonVoidJormag = SpeciesIDs.TheDragonVoidJormag,
        TheDragonVoidKralkatorrik = SpeciesIDs.TheDragonVoidKralkatorrik,
        TheDragonVoidSooWon = SpeciesIDs.TheDragonVoidSooWon,
        TheDragonVoidPrimordus = SpeciesIDs.TheDragonVoidPrimordus,
        TheDragonVoidMordremoth = SpeciesIDs.TheDragonVoidMordremoth,
        PushableVoidAmalgamate = SpeciesIDs.PushableVoidAmalgamate,
        VoidAmalgamate = 24375,
        KillableVoidAmalgamate = 23956,
        DragonBodyVoidAmalgamate = SpeciesIDs.DragonBodyVoidAmalgamate,
        VoidTangler = 25138,
        VoidColdsteel = 23945,
        VoidAbomination = 23936,
        VoidSaltsprayDragon = 23846,
        VoidObliterator = 23995,
        VoidRotswarmer = 24590,
        VoidGiant = 24450,
        VoidSkullpiercer = 25177,
        VoidTimeCaster = 25025,
        VoidBrandbomber = 24783,
        VoidBurster = 24464,
        VoidWarforged1 = 24129,
        VoidWarforged2 = 24855,
        VoidStormseer = 24677,
        VoidMelter = 24223,
        VoidGoliath = 24761,
        DragonEnergyOrb = DragonOrb,
        GravityBall = SpeciesIDs.GravityBall,
        JormagMovingFrostBeamCenter = 23747,
        JormagMovingFrostBeamNorth = 24541,
        JormagMovingFrostBeam = SpeciesIDs.JormagMovingFrostBeam,
        // - Old Lion's Court
        PrototypeVermilion = 25413,
        PrototypeArsenite = 25415,
        PrototypeIndigo = 25419,
        PrototypeVermilionCM = 25414,
        PrototypeArseniteCM = 25416,
        PrototypeIndigoCM = 25423,
        Tribocharge = 25424,
        // Secrets of the Obscure
        // - Cosmic Observatory
        Dagda = 25705,
        TheTormented = 26016,
        VeteranTheTormented = 25829,
        EliteTheTormented = 26000,
        ChampionTheTormented = 25623,
        TormentedPhantom = 25604,
        SoulFeast = 26069,
        Zojja = 26011,
        // - Temple of Febe
        Cerus = 25989,
        EmbodimentOfGluttony = 25677,
        EmbodimentOfRage = 25686,
        EmbodimentOfDespair = 26034,
        EmbodimentOfRegret = 26049,
        EmbodimentOfEnvy = 25967,
        EmbodimentOfMalice = 25700,
        MaliciousShadow = 25747,
        MaliciousShadowCM = 25645,
        PermanentEmbodimentOfGluttony = SpeciesIDs.PermanentEmbodimentOfGluttony,
        PermanentEmbodimentOfRage = SpeciesIDs.PermanentEmbodimentOfRage,
        PermanentEmbodimentOfDespair = SpeciesIDs.PermanentEmbodimentOfDespair,
        PermanentEmbodimentOfRegret = SpeciesIDs.PermanentEmbodimentOfRegret,
        PermanentEmbodimentOfEnvy = SpeciesIDs.PermanentEmbodimentOfEnvy,
        PermanentEmbodimentOfMalice = SpeciesIDs.PermanentEmbodimentOfMalice,
        // Fractals
        FractalVindicator = 19684,
        FractalAvenger = 15960,
        JadeMawTentacle = 16721,
        InspectorEllenKiel = 21566,
        ChampionRabbit = 11329,
        AwakenedAbomination = 21634,
        TheMossman = 11277,
        MaiTrinFract = 19697,
        ShadowMinotaur = 20682,
        BroodQueen = 20742,
        TheVoice = 20497,
        // - MAMA
        MAMA = 17021,
        //Arkk2 = 16902,
        GreenKnight = 16906,
        RedKnight = 16974,
        BlueKnight = 16899,
        TwistedHorror = 17009,
        // - Siax
        Siax = 17028,
        VolatileHallucinationSiax = 17002,
        EchoOfTheUnclean = 17068,
        NightmareHallucinationSiax = 16911,
        // - Ensolyss
        Ensolyss = 16948,
        NightmareHallucination1 = 16912, // (exploding after jump and charging in last phase)
        NightmareHallucination2 = 17033, // (small adds, last phase)
        NightmareAltar = 35791,
        // - Skorvald
        Skorvald = 17632,
        UnknownAnomaly = SpeciesIDs.UnknownAnomaly,
        FluxAnomaly1 = 17578,
        FluxAnomaly2 = 17929,
        FluxAnomaly3 = 17695,
        FluxAnomaly4 = 17651,
        FluxAnomalyCM1 = 17599,
        FluxAnomalyCM2 = 17770,
        FluxAnomalyCM3 = 17851,
        FluxAnomalyCM4 = 17673,
        SolarBloom = 17732,
        // - Artsariiv
        Artsariiv = 17949,
        TemporalAnomalyArtsariiv = 17870,
        Spark = 17630,
        SmallArtsariiv = 17811, // tiny adds
        MediumArtsariiv = 17694, // small adds
        BigArtsariiv = 17937, // big adds
        CloneArtsariiv = SubArtsariiv, // clone adds
        // - Arkk
        Arkk = 17759,
        TemporalAnomalyArkk = 17720,
        Archdiviner = 17893,
        FanaticDagger1 = 11281,
        FanaticDagger2 = 11282,
        FanaticBow = 11288,
        EliteBrazenGladiator = 17730,
        BLIGHT = 16437,
        PLINK = 16325,
        DOC = 16657,
        CHOP = 16552,
        ProjectionArkk = 17613,
        // - Ai
        AiKeeperOfThePeak = 23254,
        AiKeeperOfThePeak2 = SpeciesIDs.AiKeeperOfThePeak2,
        EnragedWaterSprite = 23270,
        TransitionSorrowDemon1 = 23265,
        TransitionSorrowDemon2 = 23242,
        TransitionSorrowDemon3 = 23279,
        TransitionSorrowDemon4 = 23245,
        CCSorrowDemon = 23256,
        AiDoubtDemon = 23268,
        PlayerDoubtDemon = 23246,
        FearDemon = 23264,
        GuiltDemon = 23252,
        // - Kanaxai
        KanaxaiScytheOfHouseAurkusNM = 25572,
        KanaxaiScytheOfHouseAurkusCM = 25577,
        AspectOfTorment = 25556,
        AspectOfLethargy = 25561,
        AspectOfExposure = 25562,
        AspectOfDeath = 25580,
        AspectOfFear = 25563,
        LuxonMonkSpirit = 25571,
        CaptainThess1 = 25554,
        CaptainThess2 = 25557,
        // - Eparch
        CerusLonelyTower = 26257,
        DeimosLonelyTower = 26226,
        EparchLonelyTower = 26231,
        IncarnationOfCruelty = 26270,
        IncarnationOfJudgement = 26260,
        AvatarOfSpite = 26268,
        TheTormentedLonelyTower = 26193,
        TheCravenLonelyTower = 26193,
        KryptisRift = SpeciesIDs.KryptisRift,
        // - Kinfall
        WhisperingShadow = 27010,
        // Golems
        MassiveGolem10M = 16169,
        MassiveGolem4M = 16202,
        MassiveGolem1M = 16178,
        VitalGolem = 16198,
        AvgGolem = 16177,
        StdGolem = 16199,
        LGolem = 19676,
        MedGolem = 19645,
        ConditionGolem = 16174,
        PowerGolem = 16176,
        // Open World
        // Soo Won OW
        SooWonOW = 35552, // Gadget
        SooWonTail = 51756,
        VoidGiant2 = 24310,
        VoidTimeCaster2 = 24586,
        VoidBrandstalker = 24951,
        VoidColdsteel2 = 23791,
        VoidObliterator2 = 24947,
        VoidAbomination2 = 23886,
        VoidBomber = 24714,
        VoidBrandbeast = 23917,
        VoidBrandcharger1 = 24936,
        VoidBrandcharger2 = 24039,
        VoidBrandfang1 = 24912,
        VoidBrandfang2 = 24772,
        VoidBrandscale1 = 24053,
        VoidBrandscale2 = 24426,
        VoidColdsteel3 = 24063,
        VoidCorpseknitter1 = 24756,
        VoidCorpseknitter2 = 24607,
        VoidDespoiler1 = 23874,
        VoidDespoiler2 = 25179,
        VoidFiend1 = 23707,
        VoidFiend2 = 24737,
        VoidFoulmaw = 24766,
        VoidFrostwing = 24780,
        VoidGlacier1 = 23753,
        VoidGlacier2 = 24235,
        VoidInfested1 = 24390,
        VoidInfested2 = 24997,
        VoidMelter1 = 24497,
        VoidMelter2 = 24807,
        VoidRimewolf1 = 24698,
        VoidRimewolf2 = 23798,
        VoidRotspinner1 = 25057,
        VoidStorm = 24007,
        VoidStormseer2 = 24419,
        VoidStormseer3 = 23962,
        VoidTangler2 = 23567,
        VoidThornheart1 = 24406,
        VoidThornheart2 = 23688,
        VoidWorm = 23701,
        // Story
        // - Mordremoth
        Mordremoth = 15884,
        SmotheringShadow = 15640,
        Canach = 15501,
        Braham = 15778,
        Caithe = 15565,
        BlightedRytlock = 15999,
        //BlightedCanach = 15999,
        BlightedBraham = 15553,
        BlightedMarjory = 15572,
        BlightedCaithe = 15916,
        BlightedForgal = 15597,
        BlightedSieran = 15979,
        //BlightedTybalt = 15597,
        //BlightedPaleTree = 15597,
        //BlightedTrahearne = 15597,
        //BlightedEir = 15597,
        // General
        WorldVersusWorld = 1,
        Instance = 2,
        DummyTarget = SpeciesIDs.DummyTarget,
        Environment = SpeciesIDs.Environment,
        Unknown = int.MaxValue,
        // Convergences
        // - Outer Nayos
        DemonKnight = 26142,
        Sorrow = 26126,
        Dreadwing = 26106,
        HellSister = 26146,
        UmbrielHalberdOfHouseAurkus = 26681,
        ZojjaNayos = 25874,
        ZojjasAstralProjection = 26112,
        // - Mount Balrior
        GreerTheBlightbringerConv = 26889,
        GreeTheBingerConv = 26881,
        ReegTheBlighterConv = 26842,
        DecimaTheStormsingerConv = 26720,
        UraTheSteamshriekerConv = 27017,
    };

    public enum MinionID : int
    {
        // Racial Summons
        HoundOfBalthazar = 6394,
        SnowWurm = 6445,
        DruidSpirit = 6475,
        SylvanHound = 6476,
        IronLegionSoldier = 6509,
        IronLegionMarksman = 6510,
        BloodLegionSoldier = 10106,
        BloodLegionMarksman = 10107,
        AshLegionSoldier = 10108,
        AshLegionMarksman = 10109,
        STAD007 = 10145,
        STA7012 = 10146,
        // GW2 Digital Deluxe
        MistfireWolf = 9801,
        // Rune / Relic Summons
        JaggedHorror = 21314,
        RockDog = 8836,
        MarkIGolem = 8837,
        TropicalBird = 8838,
        // Consumables with summons
        Ember = 1454,
        HawkeyeGriffon = 5614,
        SousChef = 10076,
        SunspearParagonSupport = 19643,
        RavenSpiritShadow = 22309,
        // Mesmer Phantasmas
        IllusionarySwordsman = 6487,
        IllusionaryBerserker = 6535,
        IllusionaryDisenchanter = 6621,
        IllusionaryRogue = 9444,
        IllusionaryDefender = 9445,
        IllusionaryMage = 5750,
        IllusionaryDuelist = 5758,
        IllusionaryWarlock = 6449,
        IllusionaryWarden = 7981,
        IllusionaryMariner = 9052,
        IllusionaryWhaler = 9057,
        IllusionaryAvenger = 15188,
        IllusionarySharpShooter = 26152,
        IllusionaryLancer = 26271,
        // Mesmer Clones
        // - Single Weapon
        CloneSword = 8108,
        CloneScepter = 8109,
        CloneAxe = 18894,
        CloneGreatsword = 8110,
        CloneStaff = 8111,
        CloneTrident = 9058,
        CloneSpear = 6479,
        CloneDownstate = 10542,
        CloneDagger = 25569,
        CloneRifle = 26153,
        CloneUnknown = 8107, // Possibly -> https://wiki.guildwars2.com/wiki/Clone_(Snowball_Mayhem)
        // - Sword + Offhand
        CloneSwordTorch = 15090,
        CloneSwordFocus = 15114,
        CloneSwordSword = 15233,
        CloneSwordShield = 15199,
        CloneSwordPistol = 15181,
        // - Sword 3 + Offhand
        CloneIllusionaryLeap = 8106,
        CloneIllusionaryLeapFocus = 15084,
        CloneIllusionaryLeapShield = 15131,
        CloneIllusionaryLeapSword = 15117,
        CloneIllusionaryLeapPistol = 15003,
        CloneIllusionaryLeapTorch = 15032,
        // - Scepter + Offhand
        CloneScepterTorch = 15044,
        CloneScepterShield = 15156,
        CloneScepterPistol = 15196,
        CloneScepterFocus = 15240,
        CloneScepterSword = 15249,
        // - Axe + Offhand
        CloneAxeTorch = 18922,
        CloneAxePistol = 18939,
        CloneAxeSword = 19134,
        CloneAxeFocus = 19257,
        CloneAxeShield = 25576,
        // - Dagger + Offhand
        CloneDaggerShield = 25570,
        CloneDaggerPistol = 25573,
        CloneDaggerFocus = 25575,
        CloneDaggerTorch = 25578,
        CloneDaggerSword = 25582,
        // Necromancer Minions
        BloodFiend = 1104,
        BoneFiend = 1458,
        FleshGolem = 1792,
        ShadowFiend = 5673,
        FleshWurm = 6002,
        BoneMinion = 1192,
        UnstableHorror = 18802,
        ShamblingHorror = 15314,
        // Ranger Spirits
        StoneSpirit = 6370,
        SunSpirit = 6330,
        FrostSpirit = 6369,
        StormSpirit = 6371,
        WaterSpirit = 12778,
        SpiritOfNatureRenewal = 6649,
        // Ranger Pets
        JuvenileJungleStalker = 3827,
        JuvenileKrytanDrakehound = 4425,
        JuvenileBrownBear = 4426,
        JuvenileCarrionDevourer = 5581,
        JuvenileSalamanderDrake = 5582,
        JuvenileAlpineWolf = 6043,
        JuvenileSnowLeopard = 6044,
        JuvenileRaven = 6045,
        JuvenileJaguar = 6849,
        JuvenileMarshDrake = 6850,
        JuvenileBlueMoa = 6883,
        JuvenilePinkMoa = 6884,
        JuvenileRedMoa = 6885,
        JuvenileWhiteMoa = 6886,
        JuvenileBlackMoa = 6887,
        JuvenileRiverDrake = 6888,
        JuvenileIceDrake = 6889,
        JuvenileMurellow = 6898,
        JuvenileShark = 6968,
        JuvenileFernHound = 7336,
        JuvenilePolarBear = 7926,
        JuvenileBlackBear = 7927,
        JuvenileArctodus = 7928,
        JuvenileLynx = 7932,
        JuvenileWhiptailDevourer = 7948,
        JuvenileLashtailDevourer = 7949,
        JuvenileWolf = 7975,
        JuvenileHyena = 7976,
        JuvenileOwl = 8002,
        JuvenileEagle = 8003,
        JuvenileWhiteRaven = 8004,
        JuvenileCaveSpider = 8005,
        JuvenileJungleSpider = 8006,
        JuvenileForestSpider = 8007,
        JuvenileBlackWidowSpider = 8008,
        JuvenileBoar = 8013,
        JuvenileWarthog = 8014,
        JuvenileSiamoth = 8015,
        JuvenilePig = 8016,
        JuvenileArmorFish = 8035,
        JuvenileBlueJellyfish = 8041,
        JuvenileRedJellyfish = 8042,
        JuvenileRainbowJellyfish = 9458,
        JuvenileHawk = 10022,
        JuvenileReefDrake = 11491,
        JuvenileTiger = 15380,
        JuvenileFireWywern = 15399,
        JuvenileSmokescale = 15402,
        JuvenileBristleback = 15418,
        JuvenileEletricWywern = 15436,
        JuvenileJacaranda = 18119,
        JuvenileFangedIboga = 18688,
        JuvenileCheetah = 19005,
        JuvenileRockGazelle = 19104,
        JuvenileSandLion = 19166,
        JuvenileWallow = 24203,
        JuvenileWhiteTiger = 24298,
        JuvenileSiegeTurtle = 24796,
        JuvenilePhoenix = 25131,
        JuvenileAetherHunter = 25652,
        JuvenileSkyChakStriker = 26147,
        JuvenileSpinegazer = 26220,
        JuvenileWarclaw = 26628,
        JuvenileJanthiriBee = 26851,
        // Guardian Weapon Summons
        BowOfTruth = 6383,
        HammerOfWisdom = 5791,
        ShieldOfTheAvenger = 6382,
        SwordOfJustice = 6381,
        // Thief
        ThiefDaggerHuman = 7580,
        ThiefPistolHuman = 7581,
        ThiefUnknown1 = 10090,
        ThiefUnknown2 = 10091,
        ThiefDaggerAsura = 10092,
        ThiefPistolAsura = 10093,
        ThiefPistolCharr = 10094,
        ThiefDaggerCharr = 10095,
        ThiefPistolNorn = 10098,
        ThiefDaggerNorn = 10099,
        ThiefPistolSylvari = 10102,
        ThiefDaggerSylvari = 10103,
        ThiefSwordAsura1 = 18049,
        ThiefSwordNorn1 = 18419,
        ThiefSwordAsura2 = 18492,
        ThiefSwordCharr1 = 18853,
        ThiefSwordSylvari1 = 18871,
        ThiefSwordCharr2 = 18947,
        ThiefSwordNorn2 = 19069,
        ThiefSwordHuman1 = 19087,
        ThiefSwordHuman2 = 19244,
        ThiefSwordSylvari2 = 19258,
        DaredevilSylvari1 = 17970,
        DaredevilAsura1 = 18161,
        DaredevilHuman1 = 18369,
        DaredevilAsura2 = 18420,
        DaredevilNorn1 = 18502,
        DaredevilNorn2 = 18600,
        DaredevilCharr1 = 18723,
        DaredevilSylvari2 = 18742,
        DaredevilHuman2 = 19197,
        DaredevilCharr2 = 19242,
        DeadeyeSylvari1 = 18023,
        DeadeyeHuman1 = 18053,
        DeadeyeCharr1 = 18224,
        DeadeyeSylvari2 = 18249,
        DeadeyeAsura1 = 18264,
        DeadeyeNorn1 = 18565,
        DeadeyeNorn2 = 18710,
        DeadeyeHuman2 = 18812,
        DeadeyeCharr2 = 18870,
        DeadeyeAsura2 = 18902,
        SpecterAsura1 = 25210,
        SpecterHuman1 = 25211,
        SpecterAsura2 = 25212,
        SpecterSylvari1 = 25220,
        SpecterHuman2 = 25221,
        SpecterNorn1 = 25223,
        SpecterCharr1 = 25227,
        SpecterSylvari2 = 25231,
        SpecterCharr2 = 25232,
        SpecterNorn2 = 25234,
        // Elementalist Summons
        LesserAirElemental = 8711,
        LesserEarthElemental = 8712,
        LesserFireElemental = 8713,
        LesserIceElemental = 8714,
        AirElemental = 6522,
        EarthElemental = 6523,
        FireElemental = 6524,
        IceElemental = 6525,
        // Scrapper Gyros
        SneakGyro = 15012,
        ShredderGyro = 15046,
        BulwarkGyro = 15134,
        PurgeGyro = 15135,
        MedicGyro = 15208,
        BlastGyro = 15330,
        FunctionGyro = 15336,
        // Revenant Summons
        ViskIcerazor = 18524,
        KusDarkrazor = 18594,
        JasRazorclaw = 18791,
        EraBreakrazor = 18806,
        OfelaSoulcleave = 19002,
        VentariTablet = SpeciesIDs.VentariTablet,
        // Mechanist
        JadeMech = 23549,
        // General
        Unknown,
    }

    internal static readonly Vector3 GuardianChestPosition = new( -4770.21f, -20629.2f, -2401.27f );
    internal static readonly Vector3 GorsevalChestPosition = new(1626.19f, -4467.53f, -1904.65f);
    internal static readonly Vector3 SabethaChestPosition = new(-4903.99f, 3431.57f, -2461.83f);
    internal static readonly Vector3 SlothasorChestPosition = new(7690.45f, -496.617f, -25.7641f);
    internal static readonly Vector3 ChestOfPrisonCampPosition = new(-903.703f, -9450.76f, -126.277008f);
    internal static readonly Vector3 MatthiasChestPosition = new(-5896.35f, 5454.3f, -5182.15f);
    internal static readonly Vector3 SiegeChestPosition = new(-3815.47f, 16688.5f, -5322.35f);
    internal static readonly Vector3 KeepConstructChestPosition = new (-3860.69f, 9389.32f, -5907.59f);
    internal static readonly Vector3 XeraChestPosition = new(-2840.79f, -7552.79f, -9594.75f);
    internal static readonly Vector3 CairnChestPosition = new(14025.2f, 2211.56f, -1348.89f);
    internal static readonly Vector3 RecreationRoomChestPosition = new(2647.21f, 3985.09f, -4187.83f);
    internal static readonly Vector3 SamarogChestPosition = new(-5900.66f, 3007.57f, -5096.52f);
    internal static readonly Vector3 SaulsTreasureChestPosition = new(-8445.49f, 3033.19f, -4011.12f);
    internal static readonly Vector3 ChestOfDesminaPosition = new(-9349.45f, 258.757f, -807.954f);
    internal static readonly Vector3 ChestOfSoulsPosition = new(7906.54f, 2147.48f, -5746.19f);
    internal static readonly Vector3 GrenthChestPosition = new(7825.54f, 177.448f, -5750.63f);
    internal static readonly Vector3 DhuumChestPosition = new(16877.2f, 644.802f, -6223.94f);
    internal static readonly Vector3 CAChestPosition = new(-4594f, -13004f, -2063.04f);
    internal static readonly Vector3 TwinLargosChestPosition = new(15262.2f, 6799.07f, -1497.76f);
    internal static readonly Vector3 QadimsChestPosition = new(-8291.92f, 12078.5f, -5008.73f);
    internal static readonly Vector3 AdinasChestPosition = new(14908.8f, -1478.69f, -303.638f);
    internal static readonly Vector3 SabirsChestPosition = new(-12095.1f, 2407.99f, -6542.6f);
    internal static readonly Vector3 QadimThePeerlessChestPosition = new(1636.04f, 10079.2f, -660.289f);
    internal static readonly Vector3 GrandStrikeChestHarvestTemplePosition = new(605.31f, -20400.5f, -15420.1f);

    public enum ChestID : int
    {
        GuardianChest = SpeciesIDs.GuardianChest,
        GorsevalChest = SpeciesIDs.GorsevalChest,
        SabethaChest = SpeciesIDs.SabethaChest,
        SlothasorChest = SpeciesIDs.SlothasorChest,
        ChestOfPrisonCamp = SpeciesIDs.ChestOfPrisonCamp,
        MatthiasChest = SpeciesIDs.MatthiasChest,
        SiegeChest = SpeciesIDs.SiegeChest,
        KeepConstructChest = SpeciesIDs.KeepConstructChest,
        XeraChest = SpeciesIDs.XeraChest,
        CairnChest = SpeciesIDs.CairnChest,
        RecreationRoomChest = SpeciesIDs.RecreationRoomChest,
        SamarogChest = SpeciesIDs.SamarogChest,
        SaulsTreasureChest = SpeciesIDs.SaulsTreasureChest,
        ChestOfDesmina = SpeciesIDs.ChestOfDesmina,
        ChestOfSouls = SpeciesIDs.ChestOfSouls,
        GrenthChest = SpeciesIDs.GrenthChest,
        DhuumChest = SpeciesIDs.DhuumChest,
        CAChest = SpeciesIDs.CAChest,
        TwinLargosChest = SpeciesIDs.TwinLargosChest,
        QadimsChest = SpeciesIDs.QadimsChest,
        AdinasChest = SpeciesIDs.AdinasChest,
        SabirsChest = SpeciesIDs.SabirsChest,
        QadimThePeerlessChest = SpeciesIDs.QadimThePeerlessChest,
        GrandStrikeChest = SpeciesIDs.GrandStrikeChest,

        None = int.MaxValue,
    };

    public static TargetID GetTargetID(int id)
    {
        return Enum.IsDefined(typeof(TargetID), id) ? (TargetID)id : TargetID.Unknown;
    }

    public static MinionID GetMinionID(int id)
    {
        return Enum.IsDefined(typeof(MinionID), id) ? (MinionID)id : MinionID.Unknown;
    }

    public static ChestID GetChestID(int id)
    {
        return Enum.IsDefined(typeof(ChestID), id) ? (ChestID)id : ChestID.None;
    }

    #region Custom IDs
    private const int DummyTarget = -1;
    private const int HandOfErosion = -2;
    private const int HandOfEruption = -3;
    private const int PyreGuardianProtect = -4;
    private const int PyreGuardianStab = -5;
    private const int PyreGuardianRetal = -6;
    private const int QadimLamp = -7;
    private const int AiKeeperOfThePeak2 = -8;
    private const int MatthiasSacrifice = -9;
    private const int BloodstoneFragment = -10;
    private const int BloodstoneShardMainFight = -11;
    private const int ChargedBloodstone = -12;
    private const int PyreGuardianResolution = -13;
    private const int CASword = -14;
    private const int SubArtsariiv = -15;
    private const int Cannon = -16;
    private const int TheDragonVoidZhaitan = -17;
    private const int TheDragonVoidSooWon = -18;
    private const int TheDragonVoidKralkatorrik = -19;
    private const int TheDragonVoidMordremoth = -20;
    private const int TheDragonVoidJormag = -21;
    private const int TheDragonVoidPrimordus = -22;
    private const int PushableVoidAmalgamate = -23;
    private const int DragonBodyVoidAmalgamate = -24;
    private const int VentariTablet = -25;
    private const int PoisonMushroom = -26;
    private const int SpearAggressionRevulsion = -27;
    private const int DragonOrb = -28;
    private const int ChestOfSouls = -29;
    private const int ShackledPrisoner = -30;
    private const int DemonicBond = -31;
    private const int BloodstoneShardRift = -32;
    private const int BloodstoneShardButton = -33;
    private const int SiegeChest = -34;
    private const int Mine = -35;
    private const int Environment = -36;
    private const int FerrousBomb = -37;
    private const int SanctuaryPrism = -38;
    private const int Torch = -39;
    private const int BoundIcebroodElemental = -40;
    private const int CAChest = -41;
    private const int ChestOfDesmina = -42;
    private const int UnknownAnomaly = -43;
    private const int ChestOfPrisonCamp = -44;
    private const int SnowPile = -45;
    private const int Cage = -46;
    private const int Bombs = -47;
    private const int YourSoul = -48;
    private const int QadimPlatform = -49;
    private const int GravityBall = -50;
    private const int JormagMovingFrostBeam = -51;
    private const int GrandStrikeChest = -52;
    private const int PermanentEmbodimentOfGluttony = -53;
    private const int PermanentEmbodimentOfRage = -54;
    private const int PermanentEmbodimentOfDespair = -55;
    private const int PermanentEmbodimentOfRegret = -56;
    private const int PermanentEmbodimentOfEnvy = -57;
    private const int PermanentEmbodimentOfMalice = -58;
    private const int KryptisRift = -59;
    private const int HeavyBomb = -60;
    private const int EtherealBarrier = -61;
    private const int ToxicGeyser = -62;
    private const int SulfuricGeyser = -63;
    private const int MaiTrinStrikeFake = -64;
    private const int UraGadget_BloodstoneShard = -65;
    private const int EnlightenedConduitGadget = -66;
    private const int BigEnlightenedConduitGadget = -67;
    private const int EtherealBarrier1 = -68;
    private const int EtherealBarrier2 = -69;
    private const int EtherealBarrier3 = -70;
    private const int EtherealBarrier4 = -71;
    private const int GuardianChest = -72;
    private const int GorsevalChest = -73;
    private const int SabethaChest = -74;
    private const int SlothasorChest = -75;
    private const int MatthiasChest = -76;
    private const int KeepConstructChest = -77;
    private const int XeraChest = -78;
    private const int CairnChest = -79;
    private const int RecreationRoomChest = -80;
    private const int SamarogChest = -81;
    private const int SaulsTreasureChest = -82;
    private const int GrenthChest = -83;
    private const int DhuumChest = -84;
    private const int TwinLargosChest = -85;
    private const int QadimsChest = -86;
    private const int CALeftArmAttackTarget = -87;
    private const int CARightArmAttackTarget = -88;
    private const int CABodyAttackTarget = -89;
    private const int DemonicBondAttackTarget = -90;
    private const int DeimosAttackTarget = -90;
    private const int DeimosBodyStruct = -91;
    private const int DeimosArmStruct = -92;
    private const int TitanspawnGeyserGadget = -93;
    private const int AdinasChest = -94;
    private const int SabirsChest = -95;
    private const int SabirMainPlateform = -96;
    private const int SabirSquarePlateform = -97;
    private const int SabirRectanglePlateform = -98;
    private const int SabirBigRectanglePlateform = -99;
    private const int QadimThePeerlessChest = -100;

    public const int IgnoredSpecies = int.MinValue;
    public const int NonIdentifiedSpecies = 0;
    #endregion
}
