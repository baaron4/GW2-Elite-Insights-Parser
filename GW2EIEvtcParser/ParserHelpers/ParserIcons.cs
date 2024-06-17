using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParserHelpers
{
    internal static class ParserIcons
    {
        /// <summary>
        /// Default icon in case of unknown profession.
        /// </summary>
        public const string UnknownProfessionIcon = "https://i.imgur.com/UbvyFSt.png";

        /// <summary>
        /// Default icon in case of unknown NPC.
        /// </summary>
        public const string UnknownNPCIcon = "https://i.imgur.com/nSYuby8.png";

        /// <summary>
        /// Generic enemy icon.
        /// </summary>
        public const string GenericEnemyIcon = "https://i.imgur.com/ZnFcOIA.png";

        /// <summary>
        /// Generic blue arrow pointing upwards.
        /// </summary>
        public const string GenericBlueArrowUp = "https://i.imgur.com/0EnjyQX.png";

        /// <summary>
        /// Generic Green arrow pointing upwards.
        /// </summary>
        public const string GenericGreenArrowUp = "https://i.imgur.com/Nlu3u04.png";

        /// <summary>
        /// Generic Red arrow pointing upwards.
        /// </summary>
        public const string GenericRedArrowUp = "https://i.imgur.com/hCrbYA6.png";

        /// <summary>
        /// Generic Purple arrow pointing upwards.
        /// </summary>
        public const string GenericPurpleArrowUp = "https://i.imgur.com/DBq6i0R.png";

        #region Specialization
        // High Resolution Icons 200px
        private const string HighResUntamed = "https://wiki.guildwars2.com/images/3/33/Untamed_tango_icon_200px.png";
        private const string HighResSoulbeast = "https://wiki.guildwars2.com/images/f/f6/Soulbeast_tango_icon_200px.png";
        private const string HighResDruid = "https://wiki.guildwars2.com/images/6/6d/Druid_tango_icon_200px.png";
        private const string HighResRanger = "https://wiki.guildwars2.com/images/5/51/Ranger_tango_icon_200px.png";
        private const string HighResMechanist = "https://wiki.guildwars2.com/images/8/8a/Mechanist_tango_icon_200px.png";
        private const string HighResHolosmith = "https://wiki.guildwars2.com/images/a/ae/Holosmith_tango_icon_200px.png";
        private const string HighResScrapper = "https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png";
        private const string HighResEngineer = "https://wiki.guildwars2.com/images/2/2f/Engineer_tango_icon_200px.png";
        private const string HighResSpecter = "https://wiki.guildwars2.com/images/e/eb/Specter_tango_icon_200px.png";
        private const string HighResDeadeye = "https://wiki.guildwars2.com/images/b/b0/Deadeye_tango_icon_200px.png";
        private const string HighResDaredevil = "https://wiki.guildwars2.com/images/c/ca/Daredevil_tango_icon_200px.png";
        private const string HighResThief = "https://wiki.guildwars2.com/images/1/19/Thief_tango_icon_200px.png";
        private const string HighResCatalyst = "https://wiki.guildwars2.com/images/9/92/Catalyst_tango_icon_200px.png";
        private const string HighResWeaver = "https://wiki.guildwars2.com/images/3/31/Weaver_tango_icon_200px.png";
        private const string HighResTempest = "https://wiki.guildwars2.com/images/9/90/Tempest_tango_icon_200px.png";
        private const string HighResElementalist = "https://wiki.guildwars2.com/images/a/a0/Elementalist_tango_icon_200px.png";
        private const string HighResVirtuoso = "https://wiki.guildwars2.com/images/c/cd/Virtuoso_tango_icon_200px.png";
        private const string HighResMirage = "https://wiki.guildwars2.com/images/a/a9/Mirage_tango_icon_200px.png";
        private const string HighResChronomancer = "https://wiki.guildwars2.com/images/8/8b/Chronomancer_tango_icon_200px.png";
        private const string HighResMesmer = "https://wiki.guildwars2.com/images/7/73/Mesmer_tango_icon_200px.png";
        private const string HighResHarbinger = "https://wiki.guildwars2.com/images/b/b3/Harbinger_tango_icon_200px.png";
        private const string HighResScourge = "https://wiki.guildwars2.com/images/8/8a/Scourge_tango_icon_200px.png";
        private const string HighResReaper = "https://wiki.guildwars2.com/images/9/95/Reaper_tango_icon_200px.png";
        private const string HighResNecromancer = "https://wiki.guildwars2.com/images/c/cd/Necromancer_tango_icon_200px.png";
        private const string HighResBladesworn = "https://wiki.guildwars2.com/images/c/c1/Bladesworn_tango_icon_200px.png";
        private const string HighResSpellbreaker = "https://wiki.guildwars2.com/images/7/78/Spellbreaker_tango_icon_200px.png";
        private const string HighResBerserker = "https://wiki.guildwars2.com/images/8/80/Berserker_tango_icon_200px.png";
        private const string HighResWarrior = "https://wiki.guildwars2.com/images/d/db/Warrior_tango_icon_200px.png";
        private const string HighResWillbender = "https://wiki.guildwars2.com/images/5/57/Willbender_tango_icon_200px.png";
        private const string HighResFirebrand = "https://wiki.guildwars2.com/images/7/73/Firebrand_tango_icon_200px.png";
        private const string HighResDragonhunter = "https://wiki.guildwars2.com/images/1/1f/Dragonhunter_tango_icon_200px.png";
        private const string HighResGuardian = "https://wiki.guildwars2.com/images/6/6c/Guardian_tango_icon_200px.png";
        private const string HighResVindicator = "https://wiki.guildwars2.com/images/f/f0/Vindicator_tango_icon_200px.png";
        private const string HighResRenegade = "https://wiki.guildwars2.com/images/b/bc/Renegade_tango_icon_200px.png";
        private const string HighResHerald = "https://wiki.guildwars2.com/images/c/c7/Herald_tango_icon_200px.png";
        private const string HighResRevenant = "https://wiki.guildwars2.com/images/a/a8/Revenant_tango_icon_200px.png";

        // Base Resolution Icons 20px
        private const string BaseResUntamed = "https://wiki.guildwars2.com/images/thumb/3/33/Untamed_tango_icon_200px.png/20px-Untamed_tango_icon_200px.png";
        private const string BaseResSoulbeast = "https://wiki.guildwars2.com/images/7/7c/Soulbeast_tango_icon_20px.png";
        private const string BaseResDruid = "https://wiki.guildwars2.com/images/d/d2/Druid_tango_icon_20px.png";
        private const string BaseResRanger = "https://wiki.guildwars2.com/images/4/43/Ranger_tango_icon_20px.png";
        private const string BaseResMechanist = "https://wiki.guildwars2.com/images/thumb/8/8a/Mechanist_tango_icon_200px.png/20px-Mechanist_tango_icon_200px.png";
        private const string BaseResHolosmith = "https://wiki.guildwars2.com/images/2/28/Holosmith_tango_icon_20px.png";
        private const string BaseResScrapper = "https://wiki.guildwars2.com/images/b/be/Scrapper_tango_icon_20px.png";
        private const string BaseResEngineer = "https://wiki.guildwars2.com/images/2/27/Engineer_tango_icon_20px.png";
        private const string BaseResSpecter = "https://wiki.guildwars2.com/images/5/5c/Specter_tango_icon_20px.png";
        private const string BaseResDeadeye = "https://wiki.guildwars2.com/images/c/c9/Deadeye_tango_icon_20px.png";
        private const string BaseResDaredevil = "https://wiki.guildwars2.com/images/e/e1/Daredevil_tango_icon_20px.png";
        private const string BaseResThief = "https://wiki.guildwars2.com/images/7/7a/Thief_tango_icon_20px.png";
        private const string BaseResCatalyst = "https://wiki.guildwars2.com/images/d/d5/Catalyst_tango_icon_20px.png";
        private const string BaseResWeaver = "https://wiki.guildwars2.com/images/f/fc/Weaver_tango_icon_20px.png";
        private const string BaseResTempest = "https://wiki.guildwars2.com/images/4/4a/Tempest_tango_icon_20px.png";
        private const string BaseResElementalist = "https://wiki.guildwars2.com/images/a/aa/Elementalist_tango_icon_20px.png";
        private const string BaseResVirtuoso = "https://wiki.guildwars2.com/images/6/62/Virtuoso_tango_icon_20px.png";
        private const string BaseResMirage = "https://wiki.guildwars2.com/images/d/df/Mirage_tango_icon_20px.png";
        private const string BaseResChronomancer = "https://wiki.guildwars2.com/images/f/f4/Chronomancer_tango_icon_20px.png";
        private const string BaseResMesmer = "https://wiki.guildwars2.com/images/6/60/Mesmer_tango_icon_20px.png";
        private const string BaseResHarbinger = "https://wiki.guildwars2.com/images/7/7f/Harbinger_tango_icon_20px.png";
        private const string BaseResScourge = "https://wiki.guildwars2.com/images/0/06/Scourge_tango_icon_20px.png";
        private const string BaseResReaper = "https://wiki.guildwars2.com/images/1/11/Reaper_tango_icon_20px.png";
        private const string BaseResNecromancer = "https://wiki.guildwars2.com/images/4/43/Necromancer_tango_icon_20px.png";
        private const string BaseResBladesworn = "https://wiki.guildwars2.com/images/thumb/c/c1/Bladesworn_tango_icon_200px.png/20px-Bladesworn_tango_icon_200px.png";
        private const string BaseResSpellbreaker = "https://wiki.guildwars2.com/images/e/ed/Spellbreaker_tango_icon_20px.png";
        private const string BaseResBerserker = "https://wiki.guildwars2.com/images/d/da/Berserker_tango_icon_20px.png";
        private const string BaseResWarrior = "https://wiki.guildwars2.com/images/4/43/Warrior_tango_icon_20px.png";
        private const string BaseResWillbender = "https://wiki.guildwars2.com/images/3/3a/Willbender_tango_icon_20px.png";
        private const string BaseResFirebrand = "https://wiki.guildwars2.com/images/0/02/Firebrand_tango_icon_20px.png";
        private const string BaseResDragonhunter = "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png";
        private const string BaseResGuardian = "https://wiki.guildwars2.com/images/8/8c/Guardian_tango_icon_20px.png";
        private const string BaseResVindicator = "https://wiki.guildwars2.com/images/5/5a/Vindicator_tango_icon_20px.png";
        private const string BaseResRenegade = "https://wiki.guildwars2.com/images/7/7c/Renegade_tango_icon_20px.png";
        private const string BaseResHerald = "https://wiki.guildwars2.com/images/6/67/Herald_tango_icon_20px.png";
        private const string BaseResRevenant = "https://wiki.guildwars2.com/images/b/b5/Revenant_tango_icon_20px.png";
        #endregion

        #region Target
        // Target NPC Icons
        private const string TargetWorldVersusWorld = "https://wiki.guildwars2.com/images/d/db/PvP_Server_Browser_%28map_icon%29.png";
        private const string TargetMordremoth = "https://i.imgur.com/xcQ3AFW.png";
        private const string TargetValeGuardian = "https://i.imgur.com/MIpP5pK.png";
        private const string TargetGorseval = "https://i.imgur.com/5hmMq12.png";
        private const string TargetSabetha = "https://i.imgur.com/UqbFp9S.png";
        private const string TargetSlothasor = "https://i.imgur.com/h1xH3ER.png";
        private const string TargetBerg = "https://i.imgur.com/tLMXqL7.png";
        private const string TargetNarella = "https://i.imgur.com/FwMCoR0.png";
        private const string TargetZane = "https://i.imgur.com/tkPWMST.png";
        private const string TargetMatthias = "https://i.imgur.com/3uMMmTS.png";
        private const string TargetMcLeodTheSilent = "https://i.imgur.com/jGEmZn5.png";
        private const string TargetKeepConstruct = "https://i.imgur.com/Kq0kL07.png";
        private const string TargetXera = "https://i.imgur.com/lYwJEyV.png";
        private const string TargetCairn = "https://i.imgur.com/gQY37Tf.png";
        private const string TargetMursaatOverseer = "https://i.imgur.com/5LNiw4Y.png";
        private const string TargetSamarog = "https://i.imgur.com/MPQhKfM.png";
        private const string TargetDeimos = "https://i.imgur.com/mWfxBaO.png";
        private const string TargetDesmina = "https://i.imgur.com/jAiRplg.png";
        private const string TargetBrokenKing = "https://i.imgur.com/FNgUmvL.png";
        private const string TargetEaterOfSouls = "https://i.imgur.com/Sd6Az8M.png";
        private const string TargetEyes = "https://i.imgur.com/kAgdoa5.png";
        private const string TargetDhuum = "https://i.imgur.com/RKaDon5.png";
        private const string TargetConjuredAmalgamate = "https://i.imgur.com/C23rYTl.png";
        private const string TargetCALeftArm = "https://i.imgur.com/qrkQvEY.png";
        private const string TargetCARightArm = "https://i.imgur.com/MVwjtH7.png";
        private const string TargetKenut = "https://i.imgur.com/6yq45Cc.png";
        private const string TargetNikare = "https://i.imgur.com/TLykcrJ.png";
        private const string TargetQadim = "https://i.imgur.com/IfoHTHT.png";
        private const string TargetFreezie = "https://i.imgur.com/98uyYXk.png";
        private const string TargetAdina = "https://i.imgur.com/or3m1yb.png";
        private const string TargetSabir = "https://i.imgur.com/Q4WUXqw.png";
        private const string TargetPeerlessQadim = "https://i.imgur.com/47uePpb.png";
        private const string TargetIcebroodConstruct = "https://i.imgur.com/dpaZFa5.png";
        private const string TargetClawOfTheFallen = "https://i.imgur.com/HF85QpV.png";
        private const string TargetVoiceOfTheFallen = "https://i.imgur.com/BdTGXMU.png";
        private const string TargetVoiceAndClaw = "https://i.imgur.com/V1rJBnq.png";
        private const string TargetFraenirOfJormag = "https://i.imgur.com/MxudnKp.png";
        private const string TargetBoneskinner = "https://i.imgur.com/7HPdKDQ.png";
        private const string TargetWhisperOfJormag = "https://i.imgur.com/lu9ZLVq.png";
        private const string TargetVariniaStormsounder = "https://i.imgur.com/2o8TtiM.png";
        private const string TargetMAMA = "https://i.imgur.com/1h7HOII.png";
        private const string TargetSiax = "https://i.imgur.com/5C60cQb.png";
        private const string TargetEnsolyss = "https://i.imgur.com/GUTNuyP.png";
        private const string TargetSkorvald = "https://i.imgur.com/IOPAHRE.png";
        private const string TargetArtsariiv = "https://i.imgur.com/HwZ2g1n.png";
        private const string TargetArkk = "https://i.imgur.com/u6vv8cW.png";
        private const string TargetAiKeeperOfThePeak = "https://i.imgur.com/eCXjoAS.png";
        private const string TargetAiKeeperOfThePeak2 = "https://i.imgur.com/I8nwhAw.png";
        private const string TargetVitalGolem = "https://i.imgur.com/Te4dz9y.png";
        private const string TargetPowerGolem = "https://i.imgur.com/S6eQaSf.png";
        private const string TargetStdGolem = "https://i.imgur.com/TCSo8TI.png";
        private const string TargetMassiveGolem = "https://i.imgur.com/LRlXv1t.png";
        private const string TargetMaiTrin = "https://i.imgur.com/GjHgAtX.png";
        private const string TargetEchoOfScarletBriar = "https://i.imgur.com/O9CzW46.png";
        private const string TargetAnkka = "https://i.imgur.com/3OQwlpP.png";
        private const string TargetMinisterLi = "https://i.imgur.com/2nPBLcp.png";
        private const string TargetTheDragonVoidJormag = "https://i.imgur.com/UqHxOqi.png";
        private const string TargetTheDragonVoidKralkatorrik = "https://i.imgur.com/x9id5iH.png";
        private const string TargetTheDragonVoidMordremoth = "https://i.imgur.com/6gec61w.png";
        private const string TargetTheDragonVoidPrimordus = "https://i.imgur.com/O77QoPM.png";
        private const string TargetTheDragonVoidSooWon = "https://i.imgur.com/NHs4OFG.png";
        private const string TargetTheDragonVoidZhaitan = "https://i.imgur.com/9dpoFqR.png";
        private const string TargetSooWonOW = "https://i.imgur.com/lcZGgBC.png";
        private const string TargetPrototypeVermilion = "https://i.imgur.com/Lwz1Ypr.png";
        private const string TargetPrototypeArsenite = "https://i.imgur.com/tSD5Phl.png";
        private const string TargetPrototypeIndigo = "https://i.imgur.com/DRdLQSr.png";
        private const string TargetKanaxai = "https://i.imgur.com/AxKZDs8.png";
        private const string TargetDagda = "https://i.imgur.com/k6rQdML.png";
        private const string TargetCerus = "https://i.imgur.com/wDdN8O3.png";
        private const string TargetEparch = "https://i.imgur.com/3NnhvUT.png";
        #endregion

        #region Trash
        // Trash NPC Icons
        private const string TrashCanach = "https://i.imgur.com/UuxqFko.png";
        private const string TrashBraham = "https://i.imgur.com/IZwGr7N.png";
        private const string TrashCaithe = "https://i.imgur.com/gxBNudo.png";
        private const string TrashBlightedRytlock = "https://i.imgur.com/iDvXEZj.png";
        //private const string TrashBlightedCanach = "https://i.imgur.com/ObZJXxd.png";
        private const string TrashBlightedBraham = "https://i.imgur.com/wcLwsIg.png";
        private const string TrashBlightedMarjory = "https://i.imgur.com/SqKNAzN.png";
        private const string TrashBlightedCaithe = "https://i.imgur.com/hruf4mI.png";
        private const string TrashBlightedForgal = "https://i.imgur.com/LIioL0V.png";
        private const string TrashBlightedSieran = "https://i.imgur.com/8EaXVPP.png";
        //private const string TrashBlightedTybalt = "https://i.imgur.com/TgHJKB3.png";
        //private const string TrashBlightedPaleTree = "https://i.imgur.com/l6ADzRj.png";
        //private const string TrashBlightedTrahearne = "https://i.imgur.com/MIH8rLB.png";
        //private const string TrashBlightedEir = "https://i.imgur.com/aAIFLgG.png";
        private const string TrashSpiritDemonSoul = "https://i.imgur.com/sHmksvO.png";
        private const string TrashSaul = "https://i.imgur.com/ck2IsoS.png";
        private const string TrashShackledPrisoner = "https://i.imgur.com/kxlMw7l.png";
        private const string TrashDemonicBond = "https://i.imgur.com/E4Jqwdn.png";
        private const string TrashGamblerClones = "https://i.imgur.com/zMsBWEx.png";
        private const string TrashChargedBloodstoneFragment = "https://i.imgur.com/PZ2VNAN.png";
        private const string TrashGamblerReal = "https://i.imgur.com/J6oMITN.png";
        private const string TrashPride = "https://i.imgur.com/ePTXx23.png";
        private const string TrashOil = "https://i.imgur.com/R26VgEr.png";
        private const string TrashTear = "https://i.imgur.com/N9seps0.png";
        private const string TrashFluxAnomaly = "https://i.imgur.com/JZUYCHt.png";
        private const string TrashGamblerDrunkarThief = "https://i.imgur.com/vINeVU6.png";
        private const string TrashTormentedDeadMessenger = "https://i.imgur.com/1J2BTFg.png";
        private const string TrashEnforcer = "https://i.imgur.com/elHjamF.png";
        private const string TrashEcho = "https://i.imgur.com/kcN9ECn.png";
        private const string TrashKeepConstructCoreExquisiteConjunction = "https://i.imgur.com/yI34iqw.png";
        private const string TrashKeepConstructGhosts = "https://i.imgur.com/qeYT1Bf.png";
        private const string TrashInsidiousProjection = "https://i.imgur.com/9EdItBS.png";
        private const string TrashEnergyOrb = "https://i.postimg.cc/NMNvyts0/Power-Ball.png";
        private const string TrashUnstableLeyRift = "https://i.imgur.com/YXM3igs.png";
        private const string TrashRadiantPhantasm = "https://i.imgur.com/O5VWLyY.png";
        private const string TrashCrimsonPhantasm = "https://i.imgur.com/zP7Bvb4.png";
        private const string TrashStorm = "https://i.imgur.com/9XtNPdw.png";
        private const string TrashIcePatch = "https://i.imgur.com/yxKJ5Yc.png";
        private const string TrashBanditSaboteur = "https://i.imgur.com/jUKMEbD.png";
        private const string TrashTornado = "https://i.imgur.com/e10lZMa.png";
        private const string TrashJade = "https://i.imgur.com/ivtzbSP.png";
        private const string TrashAngryChillZommoros = "https://i.imgur.com/BxbsRCI.png";
        private const string TrashAncientInvokedHydra = "https://i.imgur.com/YABLiBz.png";
        private const string TrashIcebornHydra = "https://i.imgur.com/LoYMBRU.png";
        private const string TrashIceElemental = "https://i.imgur.com/pEkBeNp.png";
        private const string TrashWyvernMatriarch = "https://i.imgur.com/kLKLSfv.png";
        private const string TrashWyvernPatriarch = "https://i.imgur.com/vjjNSpI.png";
        private const string TrashApocalypseBringer = "https://i.imgur.com/0LGKCn2.png";
        private const string TrashConjuredGreatsword = "https://i.imgur.com/vHka0QN.png";
        private const string TrashConjuredPlayerSword = "https://wiki.guildwars2.com/images/0/07/Crimson_Antique_Blade.png";
        private const string TrashConjuredShield = "https://i.imgur.com/wUiI19S.png";
        private const string TrashGreaterMagmaElemental = "https://i.imgur.com/sr146T6.png";
        private const string TrashLavaElemental = "https://i.imgur.com/mydwiYy.png";
        private const string TrashPyreGuardianKillerTornado = "https://i.imgur.com/6zNPTUw.png";
        private const string TrashPoisonMushroom = "https://i.imgur.com/AaxHiot.png";
        private const string TrashSpearAggressionRevulsion = "https://i.imgur.com/KUpmqdA.png";
        private const string TrashQadimLamp = "https://i.imgur.com/89Kjv0N.png";
        private const string TrashPyreGuardianRetal = "https://i.imgur.com/WC6LRkO.png";
        private const string TrashPyreGuardianResolution = "https://i.imgur.com/26rY9IM.png";
        private const string TrashPyreGuardianStab = "https://i.imgur.com/ISa0urR.png";
        private const string TrashPyreGuardianProtect = "https://i.imgur.com/jLW7rpV.png";
        private const string TrashReaperofFlesh = "https://i.imgur.com/Notctbt.png";
        private const string TrashKernan = "https://i.imgur.com/WABRQya.png";
        private const string TrashKnuckles = "https://i.imgur.com/m1y8nJE.png";
        private const string TrashKarde = "https://i.imgur.com/3UGyosm.png";
        private const string TrashRigom = "https://i.imgur.com/REcGMBe.png";
        private const string TrashGuldhem = "https://i.imgur.com/xa7Fefn.png";
        private const string TrashScythe = "https://i.imgur.com/INCGLIK.png";
        private const string TrashSmotheringShadow = "https://i.imgur.com/iOtx7l1.png";
        private const string TrashMazeMinotaur = "https://i.imgur.com/EMR1lQG.png";
        private const string TrashVoidSaltsprayDragon = "https://i.imgur.com/KuC1xF1.png";
        private const string TrashGenericRedEnemySkull = "https://i.imgur.com/k79t7ZA.png"; // GENERIC ICON
        private const string TrashHandOfErosionEruption = "https://i.imgur.com/reGQHhr.png";
        private const string TrashVoltaicWisp = "https://i.imgur.com/C1mvNGZ.png";
        private const string TrashParalyzingWisp = "https://i.imgur.com/YBl8Pqo.png";
        private const string TrashPeerlessQadimPylon = "https://i.imgur.com/Db5Pi3b.png";
        private const string TrashPeerlessQadimAuraPylon = "https://i.imgur.com/ojjQQlR.png";
        private const string TrashEntropicDistortion = "https://i.imgur.com/MIpP5pK.png";
        private const string TrashGiantQadimThePeerless = "https://i.imgur.com/qRhJSgR.png";
        private const string TrashSmallJumpyTornado = "https://i.imgur.com/WBJNgp7.png";
        private const string TrashOrbSpider = "https://i.imgur.com/FB5VM9X.png";
        private const string TrashSeekers = "https://i.imgur.com/FrPoluz.png";
        private const string TrashBlueGuardian = "https://i.imgur.com/6CefnkP.png";
        private const string TrashGreenGuardian = "https://i.imgur.com/nauDVYP.png";
        private const string TrashRedGuardian = "https://i.imgur.com/73Uj4lG.png";
        private const string TrashUnderworldReaper = "https://i.imgur.com/Tq6SYVe.png";
        private const string TrashVeteranTorturedWarg = "https://i.imgur.com/NklqOp3.png";
        private const string TrashGenericFriendlyTarget = "https://i.imgur.com/0koP4xB.png"; // GENERIC ICON
        private const string TrashMine = "https://i.imgur.com/A9AxMHG.png";
        private const string TrashFleshWurm = "https://i.imgur.com/o3vX9Zc.png";
        private const string TrashHands = "https://i.imgur.com/8JRPEoo.png";
        private const string TrashTemporalAnomaly = "https://i.imgur.com/MIpP5pK.png";
        private const string TrashDOC_BLIGHT_PLINK_CHOP = "https://i.imgur.com/Te4dz9y.png";
        private const string TrashArchdiviner = "https://i.imgur.com/cuLLR43.png";
        private const string TrashEliteBrazenGladiator = "https://i.imgur.com/PrQdM0K.png";
        private const string TrashFanatic = "https://i.imgur.com/niPMlFb.png";
        private const string TrashFreeziesFrozenHeart = "https://i.imgur.com/OP8aYhI.png";
        private const string TrashRiverOfSouls = "https://i.imgur.com/4pXEnaX.png";
        private const string TrashWargBloodhound = "https://i.imgur.com/AATY8BJ.png";
        private const string TrashCrimsonMcLeod = "https://i.imgur.com/dLNMI85.png";
        private const string TrashRadiantMcLeod = "https://i.imgur.com/ZlPTU4a.png";
        private const string TrashDhuumDesmina = "https://i.imgur.com/jAiRplg.png";
        private const string TrashGlenna = "https://i.imgur.com/qOPm38P.png";
        private const string TrashVoidStormseer = "https://i.imgur.com/ZullvP1.png";
        private const string TrashVoidWarforged = "https://i.imgur.com/gea0hIt.png";
        private const string TrashVoidRotswarmer = "https://i.imgur.com/uzevKld.png";
        private const string TrashVoidMelter = "https://i.imgur.com/k6jyCMc.png";
        private const string TrashVoidGiant = "https://i.imgur.com/PnaeYp4.png";
        private const string TrashZhaitansReach = "https://i.imgur.com/h1sNdhU.png";
        private const string TrashVoidAbomination = "https://i.imgur.com/h4ONU1u.png";
        private const string TrashVoidColdsteel = "https://i.imgur.com/iEofFua.png";
        private const string TrashVoidTangler = "https://i.imgur.com/qqUgKGE.png";
        private const string TrashVoidObliterator = "https://i.imgur.com/5DzJct1.png";
        private const string TrashVoidGoliath = "https://i.imgur.com/Yz62GKB.png";
        private const string TrashVoidBrandbomber = "https://i.imgur.com/s8e1QhP.png";
        private const string TrashVoidSkullpiercer = "https://i.imgur.com/7HLTnsp.png";
        private const string TrashVoidTimeCaster = "https://i.imgur.com/AKWe7od.png";
        private const string TrashVoidThornheart = "https://i.imgur.com/rts8zEg.png";
        private const string TrashVoidBrandfang = "https://i.imgur.com/UUuIS9u.png";
        private const string TrashVoidBrandbeast = "https://i.imgur.com/bnuIjnn.png";
        private const string TrashVoidBrandscale = "https://i.imgur.com/RlcKaWe.png";
        private const string TrashVoidFrostwing = "https://i.imgur.com/KbTHpBb.png";
        private const string TrashCastleFountain = "https://i.imgur.com/xV0OPWL.png";
        private const string TrashHauntingStatue = "https://i.imgur.com/7IQDyuK.png";
        private const string TrashRGBKnight = "https://i.imgur.com/lpBm4d6.png";
        private const string TrashCloneArtsariiv = "https://i.imgur.com/8I6ectk.png";
        private const string TrashMaiTrinStrikeDuringEcho = "https://i.imgur.com/GjHgAtX.png";
        private const string TrashSooWonTail = "https://i.imgur.com/O8VEP57.png";
        private const string TrashTheEnforcer = "https://i.imgur.com/GNQCYda.png";
        private const string TrashTheMechRider = "https://i.imgur.com/JSsBc6a.png";
        private const string TrashTheMindblade = "https://i.imgur.com/KyMgGQD.png";
        private const string TrashTheRitualist = "https://i.imgur.com/gG5p3Hz.png";
        private const string TrashTheSniper = "https://i.imgur.com/RWIjUoe.png";
        private const string TrashPushableVoidAmalgamate = "https://i.imgur.com/BuKbosz.png";
        private const string TrashKillableVoidAmalgamate = "https://i.imgur.com/fSFEiPm.png";
        private const string TrashKraitHallucination = "https://i.imgur.com/JtISLvT.png";
        private const string TrashQuagganHallucination = "https://i.imgur.com/8dHLEfj.png";
        private const string TrashLichHallucination = "https://i.imgur.com/IXKqCYT.png";
        private const string TrashReanimatedHatred = "https://i.imgur.com/ZjjzCq8.png";
        private const string TrashReanimatedMalice = "https://i.imgur.com/uzevKld.png";
        private const string TrashReanimatedSpite = "https://i.imgur.com/MXRFYAQ.png";
        private const string TrashReanimatedAntipathy = "https://i.imgur.com/qyLVfai.png";
        private const string TrashSanctuaryPrism = "https://i.imgur.com/u2P02yO.png";
        private const string TrashFerrousBomb = "https://i.imgur.com/jobyMFv.png";
        private const string TrashMushroomCharger = "https://i.imgur.com/hfk0rwt.png";
        private const string TrashMushroomKing = "https://i.imgur.com/WP66Ohq.png";
        private const string TrashMushroomSpikeThrower = "https://i.imgur.com/XA5nNzW.png";
        private const string TrashWhiteMantleCleric = "https://i.imgur.com/FXtlUZl.png";
        private const string TrashWhiteMantleMage = "https://i.imgur.com/OPZ4IcT.png";
        private const string TrashWhiteMantleKnight = "https://i.imgur.com/1ZVuehM.png";
        private const string TrashWhiteMantleSeeker = "https://i.imgur.com/FucZcVD.png";
        private const string TrashIcyProtector = "https://i.imgur.com/v1f7urt.png";
        private const string TrashIceSpiker = "https://i.imgur.com/RRJzjMf.png";
        private const string TrashDoppelgangerElementalist = "https://i.imgur.com/wyj7HUd.png";
        private const string TrashDoppelgangerEngineer = "https://i.imgur.com/Le9N3M4.png";
        private const string TrashDoppelgangerGuardian = "https://i.imgur.com/rTVjO4J.png";
        private const string TrashDoppelgangerMesmer = "https://i.imgur.com/OJ8msZY.png";
        private const string TrashDoppelgangerNecromancer = "https://i.imgur.com/6n9U29m.png";
        private const string TrashDoppelgangerRanger = "https://i.imgur.com/SmwqCst.png";
        private const string TrashDoppelgangerRevenant = "https://i.imgur.com/MGBaaIc.png";
        private const string TrashDoppelgangerThief = "https://i.imgur.com/up4YjAX.png";
        private const string TrashDoppelgangerWarrior = "https://i.imgur.com/zqygNLJ.png";
        private const string TrashScarletPhantom = "https://i.imgur.com/Dbv2Ztj.png";
        private const string TrashTorch = "https://i.imgur.com/vZXmiEJ.png";
        private const string TrashAberrantWisp = "https://i.imgur.com/m3cKqUB.png";
        private const string TrashBoundIcebroodElemental = "https://i.imgur.com/Jm0wCup.png";
        private const string TrashIcebroodElemental = "https://i.imgur.com/36L8Si4.png";
        private const string TrashFractalAvenger = "https://i.imgur.com/LC2E4rg.png";
        private const string TrashFractalVindicator = "https://i.imgur.com/1EgFMTg.png";
        private const string TrashCCSorrow = "https://i.imgur.com/jvNPtMJ.png";
        private const string TrashFear = "https://i.imgur.com/Eq3xHIm.png";
        private const string TrashAiDoubt = "https://i.imgur.com/IKmpccC.png";
        private const string TrashGuilt = "https://i.imgur.com/ko7137g.png";
        private const string TrashTransitionSorrow = "https://i.imgur.com/2rrUxX7.png";
        private const string TrashEnragedWaterSprite = "https://i.imgur.com/TQ5vOrg.png";
        //private const string TrashBoulders = "https://i.imgur.com/OAFzpxC.png"; // For future usage - Sunqua Peak boulders
        private const string TrashAspects = "https://i.imgur.com/mwqNEQo.png";
        private const string TrashChampionRabbit = "https://i.imgur.com/czFbODR.png";
        private const string TrashAwakenedAbomination = "https://i.imgur.com/NayYuGw.png";
        private const string TrashTheMossman = "https://i.imgur.com/jwFEGFA.png";
        private const string TrashJadeMawTentacle = "https://i.imgur.com/SfB9gJG.png";
        private const string TrashInspectorEllenKiel = "https://i.imgur.com/TyYmSnn.png";
        private const string TrashSpiritOfDestructionOrPain = "https://i.imgur.com/mKJ8X8k.png";
        private const string TrashTheTormented = "https://i.imgur.com/kEytVae.png";
        private const string TrashSoulFeast = "https://i.imgur.com/iNp5Rnx.png";
        private const string TrashSnowPile = "https://i.imgur.com/uku1klD.png";
        private const string TrashCage = "https://i.imgur.com/W9Z0roU.png";
        private const string TrashBombs = "https://i.imgur.com/fV8psEZ.png";
        private const string TrashGravityBall = "https://i.imgur.com/7zm0q8A.png";
        private const string TrashCerusDespair = "https://i.imgur.com/A5ORL5I.png";
        private const string TrashCerusEnvy = "https://i.imgur.com/pncZoZZ.png";
        private const string TrashCerusGluttony = "https://i.imgur.com/Hsturw6.png";
        private const string TrashCerusMalice = "https://i.imgur.com/13Z3dNF.png";
        private const string TrashCerusRage = "https://i.imgur.com/L0v8Goe.png";
        private const string TrashCerusRegret = "https://i.imgur.com/jZdx74j.png";
        private const string TrashAvatarOfSpite = "https://i.imgur.com/FwIaQi8.png";
        private const string TrashIncarnationOfJudgment = "https://i.imgur.com/XgR5rrk.png";
        private const string TrashIncarnationOfCruelty = "https://i.imgur.com/QVXWHny.png";
        #endregion

        #region Minion
        // Minion NPC Icons
        private const string MinionHoundOfBalthazar = "https://i.imgur.com/FFSYrzL.png";
        private const string MinionCallWurm = "https://i.imgur.com/sXA9P8O.png";
        private const string MinionDruidSpirit = "https://i.imgur.com/JacYzfo.png";
        private const string MinionSylvanHound = "https://i.imgur.com/djuA9vW.png";
        private const string MinionWarband = "https://i.imgur.com/WOU77Qb.png";
        private const string MinionDSeries = "https://i.imgur.com/hLGtfGX.png";
        private const string Minion7Series = "https://i.imgur.com/9KLoDPh.png";
        private const string MinionMistfireWolf = "https://i.imgur.com/61qeJrW.png";
        private const string MinionRuneJaggedHorror = "https://i.imgur.com/opMTn10.png";
        private const string MinionRuneRockDog = "https://i.imgur.com/gWZUfrB.png";
        private const string MinionRuneMarkIGolem = "https://i.imgur.com/0ePg7eN.png";
        private const string MinionTropicalBird = "https://i.imgur.com/zBePi2S.png";
        private const string MinionMesmerClone = "https://i.imgur.com/5Hknsa6.png";
        private const string MinionIllusionarySwordsman = "https://i.imgur.com/ReUwrAL.png";
        private const string MinionIllusionaryBerserker = "https://i.imgur.com/VNcYhXZ.png";
        private const string MinionIllusionaryDisenchanter = "https://i.imgur.com/Jbg96sq.png";
        private const string MinionIllusionaryRogue = "https://i.imgur.com/3v4pj2C.png";
        private const string MinionIllusionaryDefender = "https://i.imgur.com/jXp8Q9M.png";
        private const string MinionIllusionaryMage = "https://i.imgur.com/xIGA5Xj.png";
        private const string MinionIllusionaryDuelist = "https://i.imgur.com/ZY54uOt.png";
        private const string MinionIllusionaryWarden = "https://i.imgur.com/dId5lC2.png";
        private const string MinionIllusionaryWarlock = "https://i.imgur.com/ZRCcbBM.png";
        private const string MinionIllusionaryAvenger = "https://i.imgur.com/SmEAtBo.png";
        private const string MinionIllusionaryWhaler = "https://i.imgur.com/vVqpOvR.png";
        private const string MinionIllusionaryMariner = "https://i.imgur.com/2oSj7rI.png";
        private const string MinionJadeMech = "https://i.imgur.com/54evTaq.png";
        private const string MinionEraBreakrazor = "https://i.imgur.com/2X3G3Fl.png";
        private const string MinionKusDarkrazor = "https://i.imgur.com/rJq4Ngh.png";
        private const string MinionViskIcerazor = "https://i.imgur.com/SlTx8R5.png";
        private const string MinionJasRazorclaw = "https://i.imgur.com/SkSsLmw.png";
        private const string MinionOfelaSoulcleave = "https://i.imgur.com/xFsl0gj.png";
        private const string MinionVentariTablet = "https://i.imgur.com/nRoYMep.png";
        private const string MinionFrostSpirit = "https://i.imgur.com/dfbRWGh.png";
        private const string MinionSunSpirit = "https://i.imgur.com/HtCusPF.png";
        private const string MinionStoneSpirit = "https://i.imgur.com/4r6Ytj5.png";
        private const string MinionStormSpirit = "https://i.imgur.com/jXmencD.png";
        private const string MinionWaterSpirit = "https://i.imgur.com/ALMLVSZ.png";
        private const string MinionSpiritOfNatureRenewal = "https://i.imgur.com/sGMfD5j.png";
        private const string MinionJuvenileAlpineWolf = "https://i.imgur.com/6NJ4PJx.png";
        private const string MinionJuvenileArctodus = "https://i.imgur.com/of68C0V.png";
        private const string MinionJuvenileArmorFish = "https://i.imgur.com/s6jE8ex.png";
        private const string MinionJuvenileBlackBear = "https://i.imgur.com/VAza7ac.png";
        private const string MinionJuvenileBlackMoa = "https://i.imgur.com/l47XZUw.png";
        private const string MinionJuvenileBlackWidowSpider = "https://i.imgur.com/dNRN5Cd.png";
        private const string MinionJuvenileBlueMoa = "https://i.imgur.com/8lC1l7N.png";
        private const string MinionJuvenileBoar = "https://i.imgur.com/l9ZDJoG.png";
        private const string MinionJuvenileBristleback = "https://i.imgur.com/rLFL4JL.png";
        private const string MinionJuvenileBrownBear = "https://i.imgur.com/tTR3z9V.png";
        private const string MinionJuvenileCarrionDevourer = "https://i.imgur.com/vkANBFV.png";
        private const string MinionJuvenileCaveSpider = "https://i.imgur.com/cQJPEEg.png";
        private const string MinionJuvenileCheetah = "https://i.imgur.com/IosaqHc.png";
        private const string MinionJuvenileEagle = "https://i.imgur.com/WuOl5qh.png";
        private const string MinionJuvenileEletricWywern = "https://i.imgur.com/RsSNDV3.png";
        private const string MinionJuvenileFangedIboga = "https://i.imgur.com/cRE9fwE.png";
        private const string MinionJuvenileFernHound = "https://i.imgur.com/j1c43bj.png";
        private const string MinionJuvenileFireWywern = "https://i.imgur.com/WjODNiP.png";
        private const string MinionJuvenileForestSpider = "https://i.imgur.com/Xu5kRnv.png";
        private const string MinionJuvenileHawk = "https://i.imgur.com/Vjd2MqK.png";
        private const string MinionJuvenileIceDrake = "https://i.imgur.com/SlbkLrD.png";
        private const string MinionJuvenileJacaranda = "https://i.imgur.com/IrmdDqo.png";
        private const string MinionJuvenileJaguar = "https://i.imgur.com/QFxmbby.png";
        private const string MinionJuvenileJungleSpider = "https://i.imgur.com/4zNZcn8.png";
        private const string MinionJuvenileJungleStalker = "https://i.imgur.com/jM51zQ0.png";
        private const string MinionJuvenileKrytanDrakehound = "https://i.imgur.com/KZZ0YPw.png";
        private const string MinionJuvenileLashtailDevourer = "https://i.imgur.com/CnRTFH8.png";
        private const string MinionJuvenileLynx = "https://i.imgur.com/fCjd2Qz.png";
        private const string MinionJuvenileMarshDrake = "https://i.imgur.com/5EuKe2F.png";
        private const string MinionJuvenileMurellow = "https://i.imgur.com/3yp4riI.png";
        private const string MinionJuvenileOwl = "https://i.imgur.com/dh3thfS.png";
        private const string MinionJuvenilePhoenix = "https://i.imgur.com/ujZYaDr.png";
        private const string MinionJuvenilePig = "https://i.imgur.com/kjj0810.png";
        private const string MinionJuvenilePinkMoa = "https://i.imgur.com/AdzQreO.png";
        private const string MinionJuvenilePolarBear = "https://i.imgur.com/n72K30t.png";
        private const string MinionJuvenileBlueRainbowJellyfish = "https://i.imgur.com/kS5xGdi.png";
        private const string MinionJuvenileRaven = "https://i.imgur.com/1V5khFm.png";
        private const string MinionJuvenileRedJellyfish = "https://i.imgur.com/BlQij3o.png";
        private const string MinionJuvenileRedMoa = "https://i.imgur.com/N97LXIO.png";
        private const string MinionJuvenileReefDrake = "https://i.imgur.com/qbqd5Or.png";
        private const string MinionJuvenileRiverDrake = "https://i.imgur.com/K56jP8H.png";
        private const string MinionJuvenileRockGazelle = "https://i.imgur.com/XV1ySBt.png";
        private const string MinionJuvenileSalamanderDraker = "https://i.imgur.com/2EK2OBE.png";
        private const string MinionJuvenileSandLion = "https://i.imgur.com/XDkpvp9.png";
        private const string MinionJuvenileShark = "https://i.imgur.com/vZ9jIE9.png";
        private const string MinionJuvenileSiamoth = "https://i.imgur.com/TkEoQTE.png";
        private const string MinionJuvenileSiegeTurtle = "https://i.imgur.com/A0HC52S.png";
        private const string MinionJuvenileSmokescale = "https://i.imgur.com/30k6BmC.png";
        private const string MinionJuvenileSnowLeopard = "https://i.imgur.com/hRtnkfe.png";
        private const string MinionJuvenileTiger = "https://i.imgur.com/vALPpMJ.png";
        private const string MinionJuvenileWallow = "https://i.imgur.com/ipVdqSV.png";
        private const string MinionJuvenileWarthog = "https://i.imgur.com/MPPsWBH.png";
        private const string MinionJuvenileWhiptailDevourer = "https://i.imgur.com/hqWNZkD.png";
        private const string MinionJuvenileWhiteMoa = "https://i.imgur.com/Pw8T8hn.png";
        private const string MinionJuvenileWhiteRaven = "https://i.imgur.com/QZDITTj.png";
        private const string MinionJuvenileWhiteTiger = "https://i.imgur.com/B6wtfhQ.png";
        private const string MinionJuvenileWolf = "https://i.imgur.com/GQLiFky.png";
        private const string MinionJuvenileHyena = "https://i.imgur.com/XtYVyhH.png";
        private const string MinionJuvenileAetherHunter = "https://i.imgur.com/qMVHHki.png";
        private const string MinionSkyChakStriker = "https://i.imgur.com/bfmZsdK.png";
        private const string MinionSpinegazer = "https://i.imgur.com/xCcnGrr.png"; // TODO update this image
        private const string MinionBloodFiend = "https://i.imgur.com/PrOpULe.png";
        private const string MinionBoneFiend = "https://i.imgur.com/BEntBIt.png";
        private const string MinionFleshGolem = "https://i.imgur.com/JkYUNug.png";
        private const string MinionShadowFiend = "https://i.imgur.com/Undu5EU.png";
        private const string MinionFleshWurm = "https://i.imgur.com/Bc1VfLm.png";
        private const string MinionUnstableHorror = "https://i.imgur.com/zHPC8BX.png";
        private const string MinionShamblingHorror = "https://i.imgur.com/eeE34so.png";
        private const string MinionThievesGuild = "https://i.imgur.com/6YkM5zY.png";
        private const string MinionBowOfTruth = "https://i.imgur.com/i9uCT6p.png";
        private const string MinionHammerOfWisdom = "https://i.imgur.com/XXrlAma.png";
        private const string MinionShieldOfTheAvenger = "https://i.imgur.com/86a9LQ3.png";
        private const string MinionSwordOfJustice = "https://i.imgur.com/BKJz3br.png";
        private const string MinionLesserAirElemental = "https://i.imgur.com/T8Qb5j7.png";
        private const string MinionLesserEarthElemental = "https://i.imgur.com/E4M9YXp.png";
        private const string MinionLesserFireElemental = "https://i.imgur.com/X2QIo3j.png";
        private const string MinionLesserIceElemental = "https://i.imgur.com/ldzBrLt.png";
        private const string MinionAirElemental = "https://i.imgur.com/2cjYTg3.png";
        private const string MinionEarthElemental = "https://i.imgur.com/OrueMCk.png";
        private const string MinionFireElemental = "https://i.imgur.com/7Qhev66.png";
        private const string MinionIceElemental = "https://i.imgur.com/BTf2r0D.png";
        private const string MinionSneakGyro = "https://i.imgur.com/GlU3E6S.png";
        private const string MinionShredderGyro = "https://i.imgur.com/QwFweU2.png";
        private const string MinionPurgeGyro = "https://i.imgur.com/8EBXeai.png";
        private const string MinionBulwarkGyro = "https://i.imgur.com/Huzn2eD.png";
        private const string MinionBlastGyro = "https://i.imgur.com/12Wslnn.png";
        private const string MinionMedicGyro = "https://i.imgur.com/NiuVIte.png";
        private const string MinionFunctionGyro = "https://i.imgur.com/Gbumc3j.png";
        private const string MinionEmber = "https://i.imgur.com/a4hYNRA.png";
        private const string MinionHawkeyeGriffon = "https://i.imgur.com/t9PH5H8.png";
        private const string MinionSousChef = "https://i.imgur.com/dcBeQep.png";
        private const string MinionSunspreadParagon = "https://i.imgur.com/QXbFenF.png";
        private const string MinionRavenSpiritShadow = "https://i.imgur.com/dbLzIiY.png";
        #endregion

        #region Skill
        // Portal icons
        internal const string PortalMesmerEntre = "https://i.imgur.com/TILHQ1u.png";
        internal const string PortalMesmerExeunt = "https://i.imgur.com/7gsObHV.png";
        internal const string PortalShadowPortalPrepare = "https://i.imgur.com/oDpQcea.png";
        internal const string PortalShadowPortalOpen = "https://i.imgur.com/P7YI6am.png";
        internal const string PortalSandswell = "https://i.imgur.com/15kzkcj.png";
        internal const string PortalWhiteMantleItem = "https://i.imgur.com/FcO9n7c.png";
        internal const string PortalWhiteMantleSkill = "https://i.imgur.com/qf99SF9.png";
        internal const string PortalDimensionalAperture = "https://i.imgur.com/kzbkWov.png";

        // Skill effect decoration icons
        internal const string EffectCorrosivePoisonCloud = "https://i.imgur.com/tzUFm6G.png";
        internal const string EffectThunderclap = "https://i.imgur.com/uaTRWqN.png";
        internal const string EffectFunctionGyro = "https://i.imgur.com/HFg6kXZ.png";
        internal const string EffectDefenseField = "https://i.imgur.com/AwZ1O8O.png";
        internal const string EffectFeedback = "https://i.imgur.com/S5mXVxy.png";
        internal const string EffectBarrage = "https://i.imgur.com/8XZHYwx.png";
        internal const string EffectBonfire = "https://i.imgur.com/khgHSD5.png";
        internal const string EffectFlameTrap = "https://i.imgur.com/Z0hDXFW.png";
        internal const string EffectFrostTrap = "https://i.imgur.com/gW8yg4G.png";
        internal const string EffectSpikeTrap = "https://i.imgur.com/sHgsGxe.png";
        internal const string EffectVipersNest = "https://i.imgur.com/M7imyZL.png";
        internal const string EffectHunkerDown = "https://i.imgur.com/KkFFoJa.png";
        internal const string EffectInspiringReinforcement = "https://i.imgur.com/OItlXSb.png";
        internal const string EffectLineOfWarding = "https://i.imgur.com/XSix0GV.png";
        internal const string EffectShade = "https://i.imgur.com/tIBkK5d.png";
        internal const string EffectNullField = "https://i.imgur.com/wwaavul.png";
        internal const string EffectPlaguelands = "https://i.imgur.com/FsENhPT.png";
        internal const string EffectRainOfSwords = "https://i.imgur.com/BITK6dS.png";
        internal const string EffectRingOfWarding = "https://i.imgur.com/TK6tL8x.png";
        internal const string EffectSanctuary = "https://i.imgur.com/odR5akI.png";
        internal const string EffectSealArea = "https://i.imgur.com/OXGdr7m.png";
        internal const string EffectShadowRefuge = "https://i.imgur.com/ghU8kz0.png";
        internal const string EffectShieldOfTheAvenger = "https://i.imgur.com/O6cTek5.png";
        internal const string EffectSublimeConversion = "https://i.imgur.com/RhNsDFz.png";
        internal const string EffectThousandCuts = "https://i.imgur.com/5Fll0GH.png";
        internal const string EffectValiantBulwark = "https://i.imgur.com/cQCtl85.png";
        internal const string EffectVeil = "https://i.imgur.com/k8sVoEk.png";
        internal const string EffectWallOfReflection = "https://i.imgur.com/dc81qhc.png";
        internal const string EffectWellOfBlood = "https://i.imgur.com/j9JBLJO.png";
        internal const string EffectWellOfCorruption = "https://i.imgur.com/agcWjKA.png";
        internal const string EffectWellOfDarkness = "https://i.imgur.com/hfaRt5e.png";
        internal const string EffectWellOfSuffering = "https://i.imgur.com/PPnNujO.png";
        internal const string EffectWindsOfDisenchantment = "https://i.imgur.com/IAW0hFN.png";
        internal const string EffectProtectiveSolace = "https://i.imgur.com/uBO48eB.png";
        internal const string EffectWellOfGloom = "https://i.imgur.com/k4RSbxD.png";
        internal const string EffectWellOfBounty = "https://i.imgur.com/ZgKMlgA.png";
        internal const string EffectWellOfTears = "https://i.imgur.com/pbCS8tn.png";
        internal const string EffectWellOfSilence = "https://i.imgur.com/ZQxshJM.png";
        internal const string EffectWellOfSorrow = "https://i.imgur.com/B2gQzkY.png";
        internal const string EffectShadowfall = "https://i.imgur.com/SfRb7bC.png";
        internal const string EffectWellOfEternity = "https://i.imgur.com/TPisxaE.png";
        internal const string EffectWellOfAction = "https://i.imgur.com/YnwzegV.png";
        internal const string EffectWellOfCalamity = "https://i.imgur.com/ZrYTFnd.png";
        internal const string EffectWellOfPrecognition = "https://i.imgur.com/ECcw4jT.png";
        internal const string EffectWellOfSenility = "https://i.imgur.com/TyNZ8EN.png";
        internal const string EffectGravityWell = "https://i.imgur.com/wtdZPRo.png";
        internal const string EffectVitalDraw = "https://i.imgur.com/v0FW8mn.png";
        internal const string EffectMarkOfBlood = "https://i.imgur.com/RXSaEJE.png";
        internal const string EffectChillblains = "https://i.imgur.com/ViOzLNe.png";
        internal const string EffectMarkOfBloodOrChillblains = "https://i.imgur.com/dAPDUFm.png";
        internal const string EffectPutridMark = "https://i.imgur.com/wJ3V2Fm.png";
        internal const string EffectReapersMark = "https://i.imgur.com/85VAsHR.png";
        internal const string EffectMeteorShower = "https://i.imgur.com/ENXoZja.png";
        internal const string EffectStaticField = "https://i.imgur.com/XixW4Ae.png";
        internal const string EffectOverloadFire = "https://i.imgur.com/K34TWxW.png";
        internal const string EffectOverloadAir = "https://i.imgur.com/VwMTUTD.png";
        internal const string EffectUpdraft = "https://i.imgur.com/LQSmmJ1.png";
        internal const string EffectFirestormGlyphOrFieryGreatsword = "https://i.imgur.com/BpsVQir.png";
        internal const string EffectFirestormFieryGreatsword = "https://i.imgur.com/YuYYuGW.png";
        internal const string EffectFirestormGlyph = "https://i.imgur.com/677TYqA.png";
        internal const string EffectGeyser = "https://i.imgur.com/4h9HaFh.png";
        internal const string EffectDeployJadeSphereFire = "https://i.imgur.com/qdkoYap.png";
        internal const string EffectDeployJadeSphereWater = "https://i.imgur.com/xy0SAxK.png";
        internal const string EffectDeployJadeSphereAir = "https://i.imgur.com/SvUQmyj.png";
        internal const string EffectDeployJadeSphereEarth = "https://i.imgur.com/KG8ysd9.png";
        internal const string EffectGlyphOfTheStars = "https://i.imgur.com/hDrlcEU.png";
        internal const string EffectGlyphOfTheStarsCA = "https://i.imgur.com/hHOcANm.png";
        internal const string EffectSignetOfUndeath = "https://i.imgur.com/hIdzJ0J.png";
        internal const string EffectIllusionOfLife = "https://i.imgur.com/5FCB6T4.png";
        internal const string EffectSignetOfMercy = "https://i.imgur.com/UV4R1mo.png";
        internal const string EffectSpectralRing = "https://i.imgur.com/bMpFia8.png";
        internal const string EffectUnstableBladestorm = "https://i.imgur.com/J8LVXGm.png";
        internal const string EffectCoalescenceOfRuin = "https://i.imgur.com/EFLZ9s9.png";
        internal const string EffectDropTheHammer = "https://i.imgur.com/Dvg5jKT.png";
        internal const string EffectEternitysRequiem = "https://i.imgur.com/weWJdsh.png";
        internal const string EffectCitadelBombardment = "https://i.imgur.com/IUZceLh.png";
        internal const string EffectCitadelBombardmentPortal = "https://i.imgur.com/sYUZpkj.png";
        internal const string EffectThrowMine = "https://i.imgur.com/q9Z4xdI.png";
        internal const string EffectMineField = "https://i.imgur.com/Miy0H2G.png";
        internal const string EffectDetonateThrowMineOrMineField = "https://i.imgur.com/0Jbtf2q.png";
        internal const string EffectHuntersWard = "https://i.imgur.com/Di7OvVr.png";
        internal const string EffectSymbolOfEnergy = "https://i.imgur.com/tPaAF8S.png";
        internal const string EffectSymbolOfVengeance = "https://i.imgur.com/1niDRW8.png";
        internal const string EffectSymbolOfResolution = "https://i.imgur.com/Vy3948S.png";
        internal const string EffectSymbolOfPunishment = "https://i.imgur.com/cSoxJvd.png";
        internal const string EffectSymbolOfBlades = "https://i.imgur.com/4vyfoTs.png";
        internal const string EffectStalwartStand = "https://i.imgur.com/uBAyxRu.png";
        internal const string EffectShiningRiver = "https://i.imgur.com/5ahjMoe.png";
        internal const string EffectScorchedAftermath = "https://i.imgur.com/XvxyVC7.png";
        #endregion

        #region Marker
        // Overhead icons
        // - Fixations
        internal const string FixationBlueOverhead = "https://i.imgur.com/EUoDTln.png";
        internal const string FixationGreenOverhead = "https://i.imgur.com/cDmJWrY.png";
        internal const string FixationPurpleOverhead = "https://i.imgur.com/UImUF0H.png";
        internal const string FixationRedOverhead = "https://i.imgur.com/wIYRfY6.png";
        internal const string FixationYellowOverhead = "https://i.imgur.com/FAPQufJ.png";
        // - Squad Markers
        internal const string ArrowSquadMarkerOverhead = "https://wiki.guildwars2.com/images/e/ef/Commander_arrow_marker.png";
        internal const string CircleSquadMarkerOverhead = "https://wiki.guildwars2.com/images/6/60/Commander_circle_marker.png";
        internal const string HeartSquadMarkerOverhead = "https://wiki.guildwars2.com/images/3/32/Commander_heart_marker.png";
        internal const string SwirlSquadMarkerOverhead = "https://wiki.guildwars2.com/images/0/00/Commander_spiral_marker.png";
        internal const string SquareSquadMarkerOverhead = "https://wiki.guildwars2.com/images/0/01/Commander_square_marker.png";
        internal const string StarSquadMarkerOverhead = "https://wiki.guildwars2.com/images/3/31/Commander_star_marker.png";
        internal const string TriangleSquadMarkerOverhead = "https://wiki.guildwars2.com/images/e/ea/Commander_triangle_marker.png";
        internal const string XSquadMarkerOverhead = "https://wiki.guildwars2.com/images/b/b9/Commander_x_marker.png";
        // - Commander Tags
        internal const string BlueCommanderTagOverhead = "https://wiki.guildwars2.com/images/5/54/Commander_tag_%28blue%29.png";
        internal const string CyanCommanderTagOverhead = "https://wiki.guildwars2.com/images/2/2b/Commander_tag_%28cyan%29.png";
        internal const string GreenCommanderTagOverhead = "https://wiki.guildwars2.com/images/5/5e/Commander_tag_%28green%29.png";
        internal const string PinkCommanderTagOverhead = "https://wiki.guildwars2.com/images/b/ba/Commander_tag_%28magenta%29.png";
        internal const string OrangeCommanderTagOverhead = "https://wiki.guildwars2.com/images/1/1c/Commander_tag_%28orange%29.png";
        internal const string PurpleCommanderTagOverhead = "https://wiki.guildwars2.com/images/c/cb/Commander_tag_%28purple%29.png";
        internal const string RedCommanderTagOverhead = "https://wiki.guildwars2.com/images/4/40/Commander_tag_%28red%29.png";
        internal const string WhiteCommanderTagOverhead = "https://wiki.guildwars2.com/images/f/f1/Commander_tag_%28white%29.png";
        internal const string YellowCommanderTagOverhead = "https://wiki.guildwars2.com/images/c/cb/Commander_tag_%28yellow%29.png";
        // - Catmander Tags
        internal const string BlueCatmanderTagOverhead = "https://wiki.guildwars2.com/images/1/16/Catmander_tag_%28blue%29.png";
        internal const string CyanCatmanderTagOverhead = "https://wiki.guildwars2.com/images/9/9d/Catmander_tag_%28cyan%29.png";
        internal const string GreenCatmanderTagOverhead = "https://wiki.guildwars2.com/images/f/fd/Catmander_tag_%28green%29.png";
        internal const string PinkCatmanderTagOverhead = "https://wiki.guildwars2.com/images/9/94/Catmander_tag_%28magenta%29.png";
        internal const string OrangeCatmanderTagOverhead = "https://wiki.guildwars2.com/images/2/2a/Catmander_tag_%28orange%29.png";
        internal const string PurpleCatmanderTagOverhead = "https://wiki.guildwars2.com/images/f/ff/Catmander_tag_%28purple%29.png";
        internal const string RedCatmanderTagOverhead = "https://wiki.guildwars2.com/images/5/59/Catmander_tag_%28red%29.png";
        internal const string WhiteCatmanderTagOverhead = "https://wiki.guildwars2.com/images/5/51/Catmander_tag_%28white%29.png";
        internal const string YellowCatmanderTagOverhead = "https://wiki.guildwars2.com/images/a/a3/Catmander_tag_%28yellow%29.png";
        // - Miscellaneous
        internal const string RedArrowOverhead = "https://wiki.guildwars2.com/images/3/33/Generic_red_arrow_down.png";
        internal const string EnragedOverhead = "https://wiki.guildwars2.com/images/thumb/8/8e/Enraged_%28overhead_icon%29.png/120px-Enraged_%28overhead_icon%29.png";
        internal const string BombOverhead = "https://wiki.guildwars2.com/images/d/da/Bomb_%28overhead_icon%29.png";
        internal const string VolatilePoisonOverhead = "https://wiki.guildwars2.com/images/d/db/Volatile_Poison_IG.png";
        internal const string UnbalancedOverhead = "https://wiki.guildwars2.com/images/0/0e/Unbalanced_%28overhead_icon%29.png";
        internal const string RadiantAttunementOverhead = "https://wiki.guildwars2.com/images/c/ce/Radiant_Attunement_overhead_icon.png";
        internal const string CrimsonAttunementOverhead = "https://wiki.guildwars2.com/images/1/13/Crimson_Attunement_overhead_icon.png";
        internal const string TearInstabilityOverhead = "https://wiki.guildwars2.com/images/7/7d/Tear_Instability_%28overhead_icon%29.png";
        internal const string TidalPoolOverhead = "https://wiki.guildwars2.com/images/e/e7/Tidal_Pool_%28overhead_icon%29.png";
        internal const string SkullOverhead = "https://wiki.guildwars2.com/images/f/f6/Jade_Maw_agony_attack.png";
        internal const string PowerOfTheVoidOverhead = "https://wiki.guildwars2.com/images/f/f4/Power_of_the_Void_%28overhead_icon%29.png";
        internal const string SpectralDarknessOverhead = "https://wiki.guildwars2.com/images/f/f6/Spectral_Darkness_%28overhead_icon%29.png";
        internal const string CorruptionOverhead = "https://wiki.guildwars2.com/images/2/2d/Disarm_Poison_Gas_Mine.png";
        internal const string ConjuredShieldEmptyOverhead = "https://wiki.guildwars2.com/images/4/41/Conjured_Shield_%28overhead_icon%29.png";
        internal const string GreatswordPowerEmptyOverhead = "https://wiki.guildwars2.com/images/c/c1/Greatsword_Power_%28overhead_icon%29.png";
        internal const string EyeOverhead = "https://i.imgur.com/OBfLywE.png";
        internal const string CallTarget = "https://wiki.guildwars2.com/images/e/e9/Call_Target.png";
        internal const string TargetOverhead = "https://wiki.guildwars2.com/images/9/96/Targeted_%28Vishen_Steelshot%29.png";
        // - Bomb Timer
        internal const string BombTimerEmptyOverhead = "https://wiki.guildwars2.com/images/a/a0/Timer_empty_%28overhead_icon%29.png";
        internal const string BombTimerFullOverhead = "https://wiki.guildwars2.com/images/d/d9/Timer_full_%28overhead_icon%29.png";
        // - Madness
        internal const string MadnessSilverOverhead = "https://wiki.guildwars2.com/images/a/a8/Madness_%28overhead_icon_silver%29.png";
        internal const string MadnessGoldOverhead = "https://wiki.guildwars2.com/images/c/ce/Madness_%28overhead_icon_gold%29.png";
        internal const string MadnessRedOverhead = "https://wiki.guildwars2.com/images/f/f4/Madness_%28overhead_icon_red%29.png";
        // - Derangement
        internal const string DerangementSilverOverhead = "https://wiki.guildwars2.com/images/4/44/Derangement_%28overhead_icon_white%29.png";
        internal const string DerangementGoldOverhead = "https://wiki.guildwars2.com/images/c/cb/Derangement_%28overhead_icon_yellow%29.png";
        internal const string DerangementRedOverhead = "https://wiki.guildwars2.com/images/a/a1/Derangement_%28overhead_icon_red%29.png";
        // - Sensors
        internal const string SensorBlueOverhead = "https://wiki.guildwars2.com/images/a/aa/Sensor_Tracking_icon_%28blue%29.png";
        internal const string SensorGreenOverhead = "https://wiki.guildwars2.com/images/1/18/Sensor_Tracking_icon_%28green%29.png";
        internal const string SensorRedOverhead = "https://wiki.guildwars2.com/images/3/38/Sensor_Tracking_icon_%28red%29.png";
        // - Target Orders
        internal const string TargetOrder1Overhead = "https://wiki.guildwars2.com/images/6/6f/Target_Order-1_%28overhead_icon%29.png";
        internal const string TargetOrder2Overhead = "https://wiki.guildwars2.com/images/8/87/Target_Order-2_%28overhead_icon%29.png";
        internal const string TargetOrder3Overhead = "https://wiki.guildwars2.com/images/d/d8/Target_Order-3_%28overhead_icon%29.png";
        internal const string TargetOrder4Overhead = "https://wiki.guildwars2.com/images/c/c6/Target_Order-4_%28overhead_icon%29.png";
        internal const string TargetOrder5Overhead = "https://wiki.guildwars2.com/images/4/47/Target_Order-5_%28overhead_icon%29.png";
        #endregion
        // NPC / Gadgets Icons not private
        internal const string DhuumPlayerSoul = "https://i.imgur.com/rAyuxqS.png";

        // - Other
        internal const string QadimPlatform = "https://i.imgur.com/DbXr5Fo.png";

        internal const string NoImage = "";

        /// <summary>
        /// Dictionary matching a <see cref="Spec"/> to their high resolution profession icon.
        /// </summary>
        internal static IReadOnlyDictionary<Spec, string> HighResProfIcons { get; private set; } = new Dictionary<Spec, string>()
        {
            { Spec.Untamed, HighResUntamed },
            { Spec.Soulbeast, HighResSoulbeast },
            { Spec.Druid, HighResDruid },
            { Spec.Ranger, HighResRanger },
            { Spec.Mechanist, HighResMechanist },
            { Spec.Holosmith, HighResHolosmith },
            { Spec.Scrapper, HighResScrapper },
            { Spec.Engineer, HighResEngineer },
            { Spec.Specter, HighResSpecter },
            { Spec.Deadeye, HighResDeadeye },
            { Spec.Daredevil, HighResDaredevil },
            { Spec.Thief, HighResThief },
            { Spec.Catalyst, HighResCatalyst },
            { Spec.Weaver, HighResWeaver },
            { Spec.Tempest, HighResTempest },
            { Spec.Elementalist, HighResElementalist },
            { Spec.Virtuoso, HighResVirtuoso },
            { Spec.Mirage, HighResMirage },
            { Spec.Chronomancer, HighResChronomancer },
            { Spec.Mesmer, HighResMesmer },
            { Spec.Harbinger, HighResHarbinger },
            { Spec.Scourge, HighResScourge },
            { Spec.Reaper, HighResReaper },
            { Spec.Necromancer, HighResNecromancer },
            { Spec.Bladesworn, HighResBladesworn },
            { Spec.Spellbreaker, HighResSpellbreaker },
            { Spec.Berserker, HighResBerserker },
            { Spec.Warrior, HighResWarrior },
            { Spec.Willbender, HighResWillbender },
            { Spec.Firebrand, HighResFirebrand },
            { Spec.Dragonhunter, HighResDragonhunter },
            { Spec.Guardian, HighResGuardian },
            { Spec.Vindicator, HighResVindicator },
            { Spec.Renegade, HighResRenegade },
            { Spec.Herald, HighResHerald },
            { Spec.Revenant, HighResRevenant },
        };

        /// <summary>
        /// Dictionary matching a <see cref="Spec"/> to their base resolution profession icon.
        /// </summary>
        internal static IReadOnlyDictionary<Spec, string> BaseResProfIcons { get; private set; } = new Dictionary<Spec, string>()
        {
            { Spec.Untamed, BaseResUntamed },
            { Spec.Soulbeast, BaseResSoulbeast },
            { Spec.Druid, BaseResDruid },
            { Spec.Ranger, BaseResRanger },
            { Spec.Mechanist, BaseResMechanist },
            { Spec.Holosmith, BaseResHolosmith },
            { Spec.Scrapper, BaseResScrapper },
            { Spec.Engineer, BaseResEngineer },
            { Spec.Specter, BaseResSpecter },
            { Spec.Deadeye, BaseResDeadeye },
            { Spec.Daredevil, BaseResDaredevil },
            { Spec.Thief, BaseResThief },
            { Spec.Catalyst, BaseResCatalyst },
            { Spec.Weaver, BaseResWeaver },
            { Spec.Tempest, BaseResTempest },
            { Spec.Elementalist, BaseResElementalist },
            { Spec.Virtuoso, BaseResVirtuoso },
            { Spec.Mirage, BaseResMirage },
            { Spec.Chronomancer, BaseResChronomancer },
            { Spec.Mesmer, BaseResMesmer },
            { Spec.Harbinger, BaseResHarbinger },
            { Spec.Scourge, BaseResScourge },
            { Spec.Reaper, BaseResReaper },
            { Spec.Necromancer, BaseResNecromancer },
            { Spec.Bladesworn, BaseResBladesworn },
            { Spec.Spellbreaker, BaseResSpellbreaker },
            { Spec.Berserker, BaseResBerserker },
            { Spec.Warrior, BaseResWarrior },
            { Spec.Willbender, BaseResWillbender },
            { Spec.Firebrand, BaseResFirebrand },
            { Spec.Dragonhunter, BaseResDragonhunter },
            { Spec.Guardian, BaseResGuardian },
            { Spec.Vindicator, BaseResVindicator },
            { Spec.Renegade, BaseResRenegade },
            { Spec.Herald, BaseResHerald },
            { Spec.Revenant, BaseResRevenant },
        };

        /// <summary>
        /// Dictionary matching a <see cref="ArcDPSEnums.TargetID"/> to their icon.
        /// </summary>
        internal static IReadOnlyDictionary<ArcDPSEnums.TargetID, string> TargetNPCIcons { get; private set; } = new Dictionary<ArcDPSEnums.TargetID, string>()
        {
            { TargetID.WorldVersusWorld, TargetWorldVersusWorld },
            { TargetID.Mordremoth, TargetMordremoth },
            { TargetID.ValeGuardian, TargetValeGuardian },
            { TargetID.Gorseval, TargetGorseval },
            { TargetID.Sabetha, TargetSabetha },
            { TargetID.Slothasor, TargetSlothasor },
            { TargetID.Berg, TargetBerg },
            { TargetID.Narella, TargetNarella },
            { TargetID.Zane, TargetZane},
            { TargetID.Matthias, TargetMatthias },
            { TargetID.McLeodTheSilent, TargetMcLeodTheSilent },
            { TargetID.KeepConstruct, TargetKeepConstruct },
            { TargetID.Xera, TargetXera },
            { TargetID.Cairn, TargetCairn },
            { TargetID.MursaatOverseer, TargetMursaatOverseer },
            { TargetID.Samarog, TargetSamarog },
            { TargetID.Deimos, TargetDeimos },
            { TargetID.SoullessHorror, TargetDesmina },
            { TargetID.Desmina, TargetDesmina },
            { TargetID.BrokenKing, TargetBrokenKing },
            { TargetID.EaterOfSouls, TargetEaterOfSouls },
            { TargetID.EyeOfFate, TargetEyes },
            { TargetID.EyeOfJudgement, TargetEyes },
            { TargetID.Dhuum, TargetDhuum },
            { TargetID.ConjuredAmalgamate, TargetConjuredAmalgamate },
            { TargetID.CALeftArm, TargetCALeftArm },
            { TargetID.CARightArm, TargetCARightArm },
            { TargetID.Kenut, TargetKenut },
            { TargetID.Nikare, TargetNikare },
            { TargetID.Qadim, TargetQadim },
            { TargetID.Freezie, TargetFreezie },
            { TargetID.Adina, TargetAdina },
            { TargetID.Sabir, TargetSabir },
            { TargetID.PeerlessQadim, TargetPeerlessQadim },
            { TargetID.IcebroodConstruct, TargetIcebroodConstruct },
            { TargetID.IcebroodConstructFraenir, TargetIcebroodConstruct },
            { TargetID.ClawOfTheFallen, TargetClawOfTheFallen },
            { TargetID.VoiceOfTheFallen, TargetVoiceOfTheFallen },
            { TargetID.VoiceAndClaw, TargetVoiceAndClaw },
            { TargetID.FraenirOfJormag, TargetFraenirOfJormag },
            { TargetID.Boneskinner, TargetBoneskinner },
            { TargetID.WhisperOfJormag, TargetWhisperOfJormag },
            { TargetID.VariniaStormsounder, TargetVariniaStormsounder },
            { TargetID.MAMA, TargetMAMA },
            { TargetID.Siax, TargetSiax},
            { TargetID.Ensolyss, TargetEnsolyss },
            { TargetID.Skorvald, TargetSkorvald },
            { TargetID.Artsariiv, TargetArtsariiv },
            { TargetID.Arkk, TargetArkk },
            { TargetID.AiKeeperOfThePeak, TargetAiKeeperOfThePeak },
            { TargetID.AiKeeperOfThePeak2, TargetAiKeeperOfThePeak2 },
            { TargetID.LGolem, TargetVitalGolem },
            { TargetID.VitalGolem, TargetVitalGolem },
            { TargetID.PowerGolem, TargetPowerGolem },
            { TargetID.ConditionGolem, TargetPowerGolem },
            { TargetID.MedGolem, TargetPowerGolem },
            { TargetID.AvgGolem, TargetPowerGolem },
            { TargetID.StdGolem, TargetStdGolem },
            { TargetID.MassiveGolem10M, TargetMassiveGolem },
            { TargetID.MassiveGolem4M, TargetMassiveGolem },
            { TargetID.MassiveGolem1M, TargetMassiveGolem },
            { TargetID.MaiTrinFract, TargetMaiTrin },
            { TargetID.MaiTrinStrike, TargetMaiTrin },
            { TargetID.EchoOfScarletBriarNM, TargetEchoOfScarletBriar },
            { TargetID.EchoOfScarletBriarCM, TargetEchoOfScarletBriar },
            { TargetID.Ankka, TargetAnkka },
            { TargetID.MinisterLi, TargetMinisterLi },
            { TargetID.MinisterLiCM, TargetMinisterLi },
            { TargetID.TheDragonVoidJormag, TargetTheDragonVoidJormag },
            { TargetID.TheDragonVoidKralkatorrik, TargetTheDragonVoidKralkatorrik },
            { TargetID.TheDragonVoidMordremoth, TargetTheDragonVoidMordremoth },
            { TargetID.TheDragonVoidPrimordus, TargetTheDragonVoidPrimordus },
            { TargetID.TheDragonVoidSooWon, TargetTheDragonVoidSooWon },
            { TargetID.TheDragonVoidZhaitan, TargetTheDragonVoidZhaitan },
            { TargetID.SooWonOW, TargetSooWonOW },
            { TargetID.PrototypeVermilion, TargetPrototypeVermilion },
            { TargetID.PrototypeVermilionCM, TargetPrototypeVermilion },
            { TargetID.PrototypeArsenite, TargetPrototypeArsenite },
            { TargetID.PrototypeArseniteCM, TargetPrototypeArsenite },
            { TargetID.PrototypeIndigo, TargetPrototypeIndigo },
            { TargetID.PrototypeIndigoCM, TargetPrototypeIndigo },
            { TargetID.KanaxaiScytheOfHouseAurkusCM, TargetKanaxai },
            { TargetID.Dagda, TargetDagda },
            { TargetID.Cerus, TargetCerus },
            { TargetID.CerusLonelyTower, TargetCerus },
            { TargetID.DeimosLonelyTower, TargetDeimos },
            { TargetID.EparchLonelyTower, TargetEparch },
        };

        /// <summary>
        /// Dictionary matching a <see cref="ArcDPSEnums.TrashID"/> to their icon.
        /// </summary>
        internal static IReadOnlyDictionary<ArcDPSEnums.TrashID, string> TrashNPCIcons { get; private set; } = new Dictionary<ArcDPSEnums.TrashID, string>()
        {
            { TrashID.Canach, TrashCanach },
            { TrashID.Braham, TrashBraham },
            { TrashID.Caithe, TrashCaithe },
            { TrashID.BlightedRytlock, TrashBlightedRytlock },
            //{ ArcDPSEnums.TrashID.BlightedCanach, TrashBlightedCanach },
            { TrashID.BlightedBraham, TrashBlightedBraham },
            { TrashID.BlightedMarjory, TrashBlightedMarjory },
            { TrashID.BlightedCaithe, TrashBlightedCaithe },
            { TrashID.BlightedForgal, TrashBlightedForgal },
            { TrashID.BlightedSieran, TrashBlightedSieran },
            //{ ArcDPSEnums.TrashID.BlightedTybalt, TrashBlightedTybalt },
            //{ ArcDPSEnums.TrashID.BlightedPaleTree, TrashBlightedPaleTree },
            //{ ArcDPSEnums.TrashID.BlightedTrahearne, TrashBlightedTrahearne },
            //{ ArcDPSEnums.TrashID.BlightedEir, TrashBlightedEir },
            { TrashID.Spirit, TrashSpiritDemonSoul },
            { TrashID.Spirit2, TrashSpiritDemonSoul },
            { TrashID.ChargedSoul, TrashSpiritDemonSoul },
            { TrashID.HollowedBomber, TrashSpiritDemonSoul },
            { TrashID.Saul, TrashSaul },
            { TrashID.ShackledPrisoner, TrashShackledPrisoner },
            { TrashID.DemonicBond, TrashDemonicBond },
            { TrashID.GamblerClones, TrashGamblerClones },
            { TrashID.BloodstoneFragment, TrashChargedBloodstoneFragment },
            { TrashID.ChargedBloodstone, TrashChargedBloodstoneFragment },
            { TrashID.BloodstoneShardMainFight, TrashGamblerReal },
            { TrashID.BloodstoneShardRift, TrashGamblerReal },
            { TrashID.BloodstoneShardButton, TrashGamblerReal },
            { TrashID.GamblerReal, TrashGamblerReal },
            { TrashID.Pride, TrashPride },
            { TrashID.OilSlick, TrashOil },
            { TrashID.Oil, TrashOil },
            { TrashID.Tear, TrashTear },
            { TrashID.Gambler, TrashGamblerDrunkarThief },
            { TrashID.Drunkard, TrashGamblerDrunkarThief },
            { TrashID.Thief, TrashGamblerDrunkarThief },
            { TrashID.TormentedDead, TrashTormentedDeadMessenger },
            { TrashID.Messenger, TrashTormentedDeadMessenger },
            { TrashID.Enforcer, TrashEnforcer },
            { TrashID.Echo, TrashEcho },
            { TrashID.YourSoul, DhuumPlayerSoul },
            { TrashID.KeepConstructCore, TrashKeepConstructCoreExquisiteConjunction },
            { TrashID.ExquisiteConjunction, TrashKeepConstructCoreExquisiteConjunction },
            { TrashID.Jessica, TrashKeepConstructGhosts },
            { TrashID.Olson, TrashKeepConstructGhosts },
            { TrashID.Engul, TrashKeepConstructGhosts },
            { TrashID.Faerla, TrashKeepConstructGhosts },
            { TrashID.Caulle, TrashKeepConstructGhosts },
            { TrashID.Henley, TrashKeepConstructGhosts },
            { TrashID.Galletta, TrashKeepConstructGhosts },
            { TrashID.Ianim, TrashKeepConstructGhosts },
            { TrashID.InsidiousProjection, TrashInsidiousProjection },
            { TrashID.EnergyOrb, TrashEnergyOrb },
            { TrashID.UnstableLeyRift, TrashUnstableLeyRift },
            { TrashID.RadiantPhantasm, TrashRadiantPhantasm },
            { TrashID.CrimsonPhantasm, TrashCrimsonPhantasm },
            { TrashID.Storm, TrashStorm },
            { TrashID.IcePatch, TrashIcePatch },
            { TrashID.BanditSaboteur, TrashBanditSaboteur },
            { TrashID.NarellaTornado, TrashTornado },
            { TrashID.Cage, TrashCage },
            { TrashID.Bombs, TrashBombs },
            { TrashID.Tornado, TrashTornado },
            { TrashID.Jade, TrashJade },
            { TrashID.AngryZommoros, TrashAngryChillZommoros },
            { TrashID.ChillZommoros, TrashAngryChillZommoros },
            { TrashID.AncientInvokedHydra, TrashAncientInvokedHydra },
            { TrashID.IcebornHydra, TrashIcebornHydra },
            { TrashID.IceElemental, TrashIceElemental },
            { TrashID.WyvernMatriarch, TrashWyvernMatriarch },
            { TrashID.WyvernPatriarch, TrashWyvernPatriarch },
            { TrashID.ApocalypseBringer, TrashApocalypseBringer },
            { TrashID.ConjuredGreatsword, TrashConjuredGreatsword },
            { TrashID.ConjuredPlayerSword, TrashConjuredPlayerSword },
            { TrashID.ConjuredShield, TrashConjuredShield },
            { TrashID.GreaterMagmaElemental1, TrashGreaterMagmaElemental },
            { TrashID.GreaterMagmaElemental2, TrashGreaterMagmaElemental },
            { TrashID.LavaElemental1, TrashLavaElemental },
            { TrashID.LavaElemental2, TrashLavaElemental },
            { TrashID.QadimPlatform, NoImage},
            { TrashID.PyreGuardian, TrashPyreGuardianKillerTornado },
            { TrashID.SmallKillerTornado, TrashPyreGuardianKillerTornado},
            { TrashID.BigKillerTornado, TrashPyreGuardianKillerTornado },
            { TrashID.PoisonMushroom, TrashPoisonMushroom },
            { TrashID.SpearAggressionRevulsion, TrashSpearAggressionRevulsion },
            { TrashID.QadimLamp, TrashQadimLamp },
            { TrashID.PyreGuardianRetal, TrashPyreGuardianRetal },
            { TrashID.PyreGuardianResolution, TrashPyreGuardianResolution },
            { TrashID.PyreGuardianStab, TrashPyreGuardianStab },
            { TrashID.PyreGuardianProtect, TrashPyreGuardianProtect },
            { TrashID.SabirPlatform, NoImage},
            { TrashID.ReaperOfFlesh, TrashReaperofFlesh },
            { TrashID.Kernan, TrashKernan },
            { TrashID.Knuckles, TrashKnuckles },
            { TrashID.Karde, TrashKarde },
            { TrashID.Rigom, TrashRigom },
            { TrashID.Guldhem, TrashGuldhem },
            { TrashID.Scythe, TrashScythe },
            { TrashID.SmotheringShadow, TrashSmotheringShadow },
            { TrashID.MazeMinotaur, TrashMazeMinotaur },
            { TrashID.VoidSaltsprayDragon, TrashVoidSaltsprayDragon },
            { TrashID.BanditBombardier, TrashGenericRedEnemySkull },
            { TrashID.EchoOfTheUnclean, TrashGenericRedEnemySkull },
            { TrashID.SurgingSoul, TrashGenericRedEnemySkull },
            { TrashID.Enervator, TrashGenericRedEnemySkull },
            { TrashID.WhisperEcho, TrashGenericRedEnemySkull },
            { TrashID.DoppelgangerElementalist, TrashDoppelgangerElementalist },
            { TrashID.DoppelgangerElementalist2, TrashDoppelgangerElementalist },
            { TrashID.DoppelgangerEngineer, TrashDoppelgangerEngineer },
            { TrashID.DoppelgangerEngineer2, TrashDoppelgangerEngineer },
            { TrashID.DoppelgangerGuardian, TrashDoppelgangerGuardian },
            { TrashID.DoppelgangerGuardian2, TrashDoppelgangerGuardian },
            { TrashID.DoppelgangerMesmer, TrashDoppelgangerMesmer },
            { TrashID.DoppelgangerMesmer2, TrashDoppelgangerMesmer },
            { TrashID.DoppelgangerNecromancer, TrashDoppelgangerNecromancer },
            { TrashID.DoppelgangerNecromancer2, TrashDoppelgangerNecromancer },
            { TrashID.DoppelgangerRanger, TrashDoppelgangerRanger },
            { TrashID.DoppelgangerRanger2, TrashDoppelgangerRanger },
            { TrashID.DoppelgangerRevenant, TrashDoppelgangerRevenant },
            { TrashID.DoppelgangerRevenant2, TrashDoppelgangerRevenant },
            { TrashID.DoppelgangerThief, TrashDoppelgangerThief },
            { TrashID.DoppelgangerThief2, TrashDoppelgangerThief },
            { TrashID.DoppelgangerWarrior, TrashDoppelgangerWarrior },
            { TrashID.DoppelgangerWarrior2, TrashDoppelgangerWarrior },
            { TrashID.CharrTank, TrashGenericRedEnemySkull },
            { TrashID.PropagandaBallon, TrashGenericRedEnemySkull },
            { TrashID.EnragedWaterSprite, TrashEnragedWaterSprite },
            { TrashID.FearDemon, TrashFear },
            { TrashID.GuiltDemon, TrashGuilt },
            { TrashID.AiDoubtDemon, TrashAiDoubt },
            { TrashID.PlayerDoubtDemon, TrashGenericRedEnemySkull },
            { TrashID.TransitionSorrowDemon1, TrashTransitionSorrow },
            { TrashID.TransitionSorrowDemon2, TrashTransitionSorrow },
            { TrashID.TransitionSorrowDemon3, TrashTransitionSorrow },
            { TrashID.TransitionSorrowDemon4, TrashTransitionSorrow },
            { TrashID.CCSorrowDemon, TrashCCSorrow },
            { TrashID.ScarletPhantomHP, TrashScarletPhantom },
            { TrashID.ScarletPhantomHPCM, TrashScarletPhantom },
            { TrashID.ScarletPhantomDeathBeamCM, TrashScarletPhantom },
            { TrashID.ScarletPhantomDeathBeamCM2, TrashScarletPhantom },
            { TrashID.ScarletPhantomBreakbar, TrashScarletPhantom },
            { TrashID.ScarletPhantomNormalBeam, TrashScarletPhantom },
            { TrashID.ScarletPhantomConeWaveNM, TrashScarletPhantom },
            { TrashID.FerrousBomb, TrashFerrousBomb },
            { TrashID.KraitsHallucination, TrashKraitHallucination },
            { TrashID.LichHallucination, TrashLichHallucination },
            { TrashID.QuaggansHallucinationNM, TrashQuagganHallucination },
            { TrashID.QuaggansHallucinationCM, TrashQuagganHallucination },
            { TrashID.ReanimatedAntipathy, TrashReanimatedAntipathy },
            { TrashID.ReanimatedMalice1, TrashReanimatedMalice },
            { TrashID.ReanimatedMalice2, TrashReanimatedMalice },
            { TrashID.ReanimatedHatred, TrashReanimatedHatred },
            { TrashID.ReanimatedSpite, TrashReanimatedSpite },
            { TrashID.SanctuaryPrism, TrashSanctuaryPrism },
            { TrashID.VoidBrandstalker, TrashGenericRedEnemySkull },
            { TrashID.SpiritOfDestruction, TrashSpiritOfDestructionOrPain },
            { TrashID.SpiritOfPain, TrashSpiritOfDestructionOrPain },
            { TrashID.DragonEnergyOrb, TrashGenericRedEnemySkull },
            { TrashID.HandOfErosion, TrashHandOfErosionEruption },
            { TrashID.HandOfEruption, TrashHandOfErosionEruption },
            { TrashID.VoltaicWisp, TrashVoltaicWisp },
            { TrashID.ParalyzingWisp, TrashParalyzingWisp },
            { TrashID.PeerlessQadimPylon, TrashPeerlessQadimPylon },
            //{ ArcDPSEnums.TrashID.PeerlessQadimAuraPylon, TrashPeerlessQadimAuraPylon },
            { TrashID.EntropicDistortion, TrashEntropicDistortion },
            //{ ArcDPSEnums.TrashID.Brandstorm, GenericEnemyIcon },
            { TrashID.GiantQadimThePeerless, TrashGiantQadimThePeerless },
            //{ ArcDPSEnums.TrashID.DummyPeerlessQadim, GenericEnemyIcon },
            { TrashID.SmallJumpyTornado, TrashSmallJumpyTornado },
            { TrashID.OrbSpider, TrashOrbSpider },
            { TrashID.Seekers, TrashSeekers },
            { TrashID.BlueGuardian, TrashBlueGuardian },
            { TrashID.GreenGuardian, TrashGreenGuardian },
            { TrashID.RedGuardian, TrashRedGuardian },
            { TrashID.UnderworldReaper, TrashUnderworldReaper },
            { TrashID.VeteranTorturedWarg, TrashVeteranTorturedWarg },
            { TrashID.GreenSpirit1, TrashGenericFriendlyTarget },
            { TrashID.GreenSpirit2, TrashGenericFriendlyTarget },
            { TrashID.BanditSapper, TrashGenericFriendlyTarget },
            { TrashID.ProjectionArkk, TrashGenericFriendlyTarget },
            { TrashID.PrioryExplorer, TrashGenericFriendlyTarget },
            { TrashID.PrioryScholar, TrashGenericFriendlyTarget },
            { TrashID.VigilRecruit, TrashGenericFriendlyTarget },
            { TrashID.VigilTactician, TrashGenericFriendlyTarget },
            { TrashID.Prisoner1, TrashGenericFriendlyTarget },
            { TrashID.Prisoner2, TrashGenericFriendlyTarget },
            { TrashID.Mine, TrashMine },
            { TrashID.FleshWurm, TrashFleshWurm },
            { TrashID.Hands, TrashHands },
            { TrashID.TemporalAnomalyArtsariiv, TrashTemporalAnomaly },
            { TrashID.TemporalAnomalyArkk, TrashTemporalAnomaly },
            { TrashID.DOC, TrashDOC_BLIGHT_PLINK_CHOP },
            { TrashID.BLIGHT, TrashDOC_BLIGHT_PLINK_CHOP },
            { TrashID.PLINK, TrashDOC_BLIGHT_PLINK_CHOP },
            { TrashID.CHOP, TrashDOC_BLIGHT_PLINK_CHOP },
            { TrashID.Archdiviner, TrashArchdiviner },
            { TrashID.EliteBrazenGladiator, TrashEliteBrazenGladiator },
            { TrashID.FanaticDagger1, TrashFanatic },
            { TrashID.FanaticDagger2, TrashFanatic },
            { TrashID.FanaticBow, TrashFanatic },
            { TrashID.FreeziesFrozenHeart, TrashFreeziesFrozenHeart },
            { TrashID.IceSpiker, TrashIceSpiker },
            { TrashID.IceStormer, TrashIceElemental },
            { TrashID.IcyProtector, TrashIcyProtector },
            { TrashID.SnowPile, TrashSnowPile },
            { TrashID.RiverOfSouls, TrashRiverOfSouls },
            { TrashID.WargBloodhound, TrashWargBloodhound },
            { TrashID.CrimsonMcLeod, TrashCrimsonMcLeod },
            { TrashID.RadiantMcLeod, TrashRadiantMcLeod },
            { TrashID.MushroomCharger, TrashMushroomCharger },
            { TrashID.MushroomKing, TrashMushroomKing },
            { TrashID.MushroomSpikeThrower, TrashMushroomSpikeThrower },
            { TrashID.WhiteMantleBattleCleric1, TrashWhiteMantleCleric },
            { TrashID.WhiteMantleBattleCleric2, TrashWhiteMantleCleric },
            { TrashID.WhiteMantleBattleKnight1, TrashWhiteMantleKnight },
            { TrashID.WhiteMantleBattleKnight2, TrashWhiteMantleKnight },
            { TrashID.WhiteMantleBattleMage1, TrashWhiteMantleMage },
            { TrashID.WhiteMantleBattleMage2, TrashWhiteMantleMage },
            { TrashID.WhiteMantleBattleMage1Escort, TrashWhiteMantleMage },
            { TrashID.WhiteMantleBattleMage2Escort, TrashWhiteMantleMage },
            { TrashID.WhiteMantleBattleSeeker1, TrashWhiteMantleSeeker },
            { TrashID.WhiteMantleBattleSeeker2, TrashWhiteMantleSeeker },
            { TrashID.WhiteMantleBattleCultist1, GenericEnemyIcon },
            { TrashID.WhiteMantleBattleCultist2, GenericEnemyIcon },
            { TrashID.DhuumDesmina, TrashDhuumDesmina },
            { TrashID.Glenna, TrashGlenna },
            { TrashID.VoidStormseer, TrashVoidStormseer },
            { TrashID.VoidStormseer2, TrashVoidStormseer },
            { TrashID.VoidStormseer3, TrashVoidStormseer },
            { TrashID.VoidWarforged1, TrashVoidWarforged },
            { TrashID.VoidWarforged2, TrashVoidWarforged },
            { TrashID.VoidRotswarmer, TrashVoidRotswarmer },
            { TrashID.VoidMelter, TrashVoidMelter },
            { TrashID.VoidMelter1, TrashVoidMelter },
            { TrashID.VoidMelter2, TrashVoidMelter },
            { TrashID.VoidGiant, TrashVoidGiant },
            { TrashID.VoidGiant2, TrashVoidGiant },
            { TrashID.ZhaitansReach, TrashZhaitansReach },
            { TrashID.VoidAbomination, TrashVoidAbomination },
            { TrashID.VoidAbomination2, TrashVoidAbomination },
            { TrashID.VoidColdsteel, TrashVoidColdsteel },
            { TrashID.VoidColdsteel2, TrashVoidColdsteel },
            { TrashID.VoidColdsteel3, TrashVoidColdsteel },
            { TrashID.VoidTangler, TrashVoidTangler },
            { TrashID.VoidTangler2, TrashVoidTangler },
            { TrashID.VoidObliterator, TrashVoidObliterator },
            { TrashID.VoidObliterator2, TrashVoidObliterator },
            { TrashID.VoidGoliath, TrashVoidGoliath },
            { TrashID.VoidBrandbomber, TrashVoidBrandbomber },
            { TrashID.VoidSkullpiercer, TrashVoidSkullpiercer },
            { TrashID.VoidTimeCaster, TrashVoidTimeCaster },
            { TrashID.VoidTimeCaster2, TrashVoidTimeCaster },
            { TrashID.GravityBall, TrashGravityBall },
            { TrashID.JormagMovingFrostBeam, TrashGenericRedEnemySkull },
            { TrashID.JormagMovingFrostBeamNorth, TrashGenericRedEnemySkull },
            { TrashID.JormagMovingFrostBeamCenter, TrashGenericRedEnemySkull },
            { TrashID.VoidThornheart1, TrashVoidThornheart },
            { TrashID.VoidThornheart2, TrashVoidThornheart },
            { TrashID.VoidBrandfang1, TrashVoidBrandfang },
            { TrashID.VoidBrandfang2, TrashVoidBrandfang },
            { TrashID.VoidBrandbeast, TrashVoidBrandbeast },
            { TrashID.VoidBrandscale1, TrashVoidBrandscale },
            { TrashID.VoidBrandscale2, TrashVoidBrandscale },
            { TrashID.VoidFrostwing, TrashVoidFrostwing },
            //{ ArcDPSEnums.TrashID.CastleFountain, TrashCastleFountain },
            { TrashID.HauntingStatue, TrashHauntingStatue },
            { TrashID.GreenKnight, TrashRGBKnight },
            { TrashID.RedKnight, TrashRGBKnight },
            { TrashID.BlueKnight, TrashRGBKnight },
            { TrashID.CloneArtsariiv, TrashCloneArtsariiv },
            { TrashID.MaiTrinStrikeDuringEcho, TrashMaiTrinStrikeDuringEcho },
            { TrashID.SooWonTail, TrashSooWonTail },
            { TrashID.TheEnforcer, TrashTheEnforcer },
            { TrashID.TheEnforcerCM, TrashTheEnforcer },
            { TrashID.TheMechRider, TrashTheMechRider },
            { TrashID.TheMechRiderCM, TrashTheMechRider },
            { TrashID.TheMindblade, TrashTheMindblade },
            { TrashID.TheMindbladeCM, TrashTheMindblade },
            { TrashID.TheRitualist, TrashTheRitualist },
            { TrashID.TheRitualistCM, TrashTheRitualist },
            { TrashID.TheSniper, TrashTheSniper },
            { TrashID.TheSniperCM, TrashTheSniper },
            { TrashID.PushableVoidAmalgamate, TrashPushableVoidAmalgamate },
            { TrashID.KillableVoidAmalgamate, TrashKillableVoidAmalgamate },
            { TrashID.FluxAnomaly1, TrashFluxAnomaly },
            { TrashID.FluxAnomaly2, TrashFluxAnomaly },
            { TrashID.FluxAnomaly3, TrashFluxAnomaly },
            { TrashID.FluxAnomaly4, TrashFluxAnomaly },
            { TrashID.FluxAnomalyCM1, TrashFluxAnomaly },
            { TrashID.FluxAnomalyCM2, TrashFluxAnomaly },
            { TrashID.FluxAnomalyCM3, TrashFluxAnomaly },
            { TrashID.FluxAnomalyCM4, TrashFluxAnomaly },
            { TrashID.Torch, TrashTorch },
            { TrashID.AberrantWisp, TrashAberrantWisp },
            { TrashID.BoundIcebroodElemental, TrashBoundIcebroodElemental },
            { TrashID.IcebroodElemental, TrashIcebroodElemental },
            { TrashID.FractalAvenger, TrashFractalAvenger },
            { TrashID.FractalVindicator, TrashFractalVindicator },
            { TrashID.AspectOfDeath, TrashAspects },
            { TrashID.AspectOfExposure, TrashAspects },
            { TrashID.AspectOfLethargy, TrashAspects },
            { TrashID.AspectOfTorment, TrashAspects },
            { TrashID.AspectOfFear, TrashAspects },
            { TrashID.ChampionRabbit, TrashChampionRabbit },
            { TrashID.TheMossman, TrashTheMossman },
            { TrashID.InspectorEllenKiel, TrashInspectorEllenKiel },
            { TrashID.JadeMawTentacle, TrashJadeMawTentacle },
            { TrashID.AwakenedAbomination, TrashAwakenedAbomination },
            { TrashID.EmbodimentOfDespair, TrashCerusDespair },
            { TrashID.EmbodimentOfEnvy, TrashCerusEnvy },
            { TrashID.EmbodimentOfGluttony, TrashCerusGluttony },
            { TrashID.EmbodimentOfMalice, TrashCerusMalice },
            { TrashID.EmbodimentOfRage, TrashCerusRage },
            { TrashID.EmbodimentOfRegret, TrashCerusRegret },
            { TrashID.PermanentEmbodimentOfDespair, TrashCerusDespair },
            { TrashID.PermanentEmbodimentOfEnvy, TrashCerusEnvy },
            { TrashID.PermanentEmbodimentOfGluttony, TrashCerusGluttony },
            { TrashID.PermanentEmbodimentOfMalice, TrashCerusMalice },
            { TrashID.PermanentEmbodimentOfRage, TrashCerusRage },
            { TrashID.PermanentEmbodimentOfRegret, TrashCerusRegret },
            { TrashID.TheTormented, TrashTheTormented },
            { TrashID.VeteranTheTormented, TrashTheTormented },
            { TrashID.EliteTheTormented, TrashTheTormented },
            { TrashID.ChampionTheTormented, TrashTheTormented },
            { TrashID.SoulFeast, TrashSoulFeast },
            { TrashID.AvatarOfSpite, TrashAvatarOfSpite },
            { TrashID.IncarnationOfJudgement, TrashIncarnationOfJudgment },
            { TrashID.IncarnationOfCruelty, TrashIncarnationOfCruelty },
            { TrashID.KryptisRift, TrashGenericRedEnemySkull },
        };

        /// <summary>
        /// Dictionary matching a <see cref="ArcDPSEnums.MinionID"/> to their icon.
        /// </summary>
        internal static IReadOnlyDictionary<ArcDPSEnums.MinionID, string> MinionNPCIcons { get; private set; } = new Dictionary<ArcDPSEnums.MinionID, string>()
        {
            { MinionID.HoundOfBalthazar, MinionHoundOfBalthazar },
            { MinionID.SnowWurm, MinionCallWurm },
            { MinionID.DruidSpirit, MinionDruidSpirit },
            { MinionID.SylvanHound, MinionSylvanHound },
            { MinionID.IronLegionSoldier, MinionWarband },
            { MinionID.IronLegionMarksman, MinionWarband },
            { MinionID.BloodLegionSoldier, MinionWarband },
            { MinionID.BloodLegionMarksman, MinionWarband },
            { MinionID.AshLegionSoldier, MinionWarband },
            { MinionID.AshLegionMarksman, MinionWarband },
            { MinionID.STAD007, MinionDSeries },
            { MinionID.STA7012, Minion7Series },
            { MinionID.MistfireWolf, MinionMistfireWolf },
            { MinionID.RuneJaggedHorror, MinionRuneJaggedHorror },
            { MinionID.RuneRockDog, MinionRuneRockDog },
            { MinionID.RuneMarkIGolem, MinionRuneMarkIGolem },
            { MinionID.RuneTropicalBird, MinionTropicalBird },
            { MinionID.Ember, MinionEmber },
            { MinionID.HawkeyeGriffon, MinionHawkeyeGriffon },
            { MinionID.SousChef, MinionSousChef },
            { MinionID.SunspearParagonSupport, MinionSunspreadParagon },
            { MinionID.RavenSpiritShadow, MinionRavenSpiritShadow },
            { MinionID.CloneSpear, MinionMesmerClone },
            { MinionID.CloneGreatsword, MinionMesmerClone },
            { MinionID.CloneStaff, MinionMesmerClone },
            { MinionID.CloneTrident, MinionMesmerClone },
            { MinionID.CloneSword, MinionMesmerClone },
            { MinionID.CloneSwordPistol, MinionMesmerClone },
            { MinionID.CloneSwordTorch, MinionMesmerClone },
            { MinionID.CloneSwordFocus, MinionMesmerClone },
            { MinionID.CloneSwordSword, MinionMesmerClone },
            { MinionID.CloneSwordShield, MinionMesmerClone },
            { MinionID.CloneIllusionaryLeap, MinionMesmerClone },
            { MinionID.CloneIllusionaryLeapFocus, MinionMesmerClone },
            { MinionID.CloneIllusionaryLeapShield, MinionMesmerClone },
            { MinionID.CloneIllusionaryLeapSword, MinionMesmerClone },
            { MinionID.CloneIllusionaryLeapPistol, MinionMesmerClone },
            { MinionID.CloneIllusionaryLeapTorch, MinionMesmerClone },
            { MinionID.CloneScepter, MinionMesmerClone },
            { MinionID.CloneScepterTorch, MinionMesmerClone },
            { MinionID.CloneScepterShield, MinionMesmerClone },
            { MinionID.CloneScepterPistol, MinionMesmerClone },
            { MinionID.CloneScepterFocus, MinionMesmerClone },
            { MinionID.CloneScepterSword, MinionMesmerClone },
            { MinionID.CloneAxe, MinionMesmerClone },
            { MinionID.CloneAxeTorch, MinionMesmerClone },
            { MinionID.CloneAxePistol, MinionMesmerClone },
            { MinionID.CloneAxeSword, MinionMesmerClone },
            { MinionID.CloneAxeFocus, MinionMesmerClone },
            { MinionID.CloneAxeShield, MinionMesmerClone },
            { MinionID.CloneDagger, MinionMesmerClone },
            { MinionID.CloneDaggerFocus, MinionMesmerClone },
            { MinionID.CloneDaggerPistol, MinionMesmerClone },
            { MinionID.CloneDaggerShield, MinionMesmerClone },
            { MinionID.CloneDaggerSword, MinionMesmerClone },
            { MinionID.CloneDaggerTorch, MinionMesmerClone },
            { MinionID.CloneDownstate, MinionMesmerClone },
            { MinionID.CloneUnknown, MinionMesmerClone },
            { MinionID.IllusionarySwordsman, MinionIllusionarySwordsman },
            { MinionID.IllusionaryBerserker, MinionIllusionaryBerserker },
            { MinionID.IllusionaryDisenchanter, MinionIllusionaryDisenchanter },
            { MinionID.IllusionaryRogue, MinionIllusionaryRogue },
            { MinionID.IllusionaryDefender, MinionIllusionaryDefender },
            { MinionID.IllusionaryMage, MinionIllusionaryMage },
            { MinionID.IllusionaryDuelist, MinionIllusionaryDuelist },
            { MinionID.IllusionaryWarden, MinionIllusionaryWarden },
            { MinionID.IllusionaryWarlock, MinionIllusionaryWarlock },
            { MinionID.IllusionaryAvenger, MinionIllusionaryAvenger },
            { MinionID.IllusionaryWhaler, MinionIllusionaryWhaler },
            { MinionID.IllusionaryMariner, MinionIllusionaryMariner },
            { MinionID.JadeMech, MinionJadeMech },
            { MinionID.EraBreakrazor, MinionEraBreakrazor },
            { MinionID.KusDarkrazor, MinionKusDarkrazor },
            { MinionID.ViskIcerazor, MinionViskIcerazor },
            { MinionID.JasRazorclaw, MinionJasRazorclaw },
            { MinionID.OfelaSoulcleave, MinionOfelaSoulcleave },
            { MinionID.VentariTablet, MinionVentariTablet },
            { MinionID.FrostSpirit, MinionFrostSpirit },
            { MinionID.SunSpirit, MinionSunSpirit },
            { MinionID.StoneSpirit, MinionStoneSpirit },
            { MinionID.StormSpirit, MinionStormSpirit },
            { MinionID.WaterSpirit, MinionWaterSpirit },
            { MinionID.SpiritOfNatureRenewal, MinionSpiritOfNatureRenewal },
            { MinionID.JuvenileAlpineWolf, MinionJuvenileAlpineWolf },
            { MinionID.JuvenileArctodus, MinionJuvenileArctodus },
            { MinionID.JuvenileArmorFish, MinionJuvenileArmorFish },
            { MinionID.JuvenileBlackBear, MinionJuvenileBlackBear },
            { MinionID.JuvenileBlackMoa, MinionJuvenileBlackMoa },
            { MinionID.JuvenileBlackWidowSpider, MinionJuvenileBlackWidowSpider },
            { MinionID.JuvenileBlueJellyfish, MinionJuvenileBlueRainbowJellyfish },
            { MinionID.JuvenileBlueMoa, MinionJuvenileBlueMoa },
            { MinionID.JuvenileBoar, MinionJuvenileBoar },
            { MinionID.JuvenileBristleback, MinionJuvenileBristleback },
            { MinionID.JuvenileBrownBear, MinionJuvenileBrownBear },
            { MinionID.JuvenileCarrionDevourer, MinionJuvenileCarrionDevourer },
            { MinionID.JuvenileCaveSpider, MinionJuvenileCaveSpider },
            { MinionID.JuvenileCheetah, MinionJuvenileCheetah },
            { MinionID.JuvenileEagle, MinionJuvenileEagle },
            { MinionID.JuvenileEletricWywern, MinionJuvenileEletricWywern },
            { MinionID.JuvenileFangedIboga, MinionJuvenileFangedIboga },
            { MinionID.JuvenileFernHound, MinionJuvenileFernHound },
            { MinionID.JuvenileFireWywern, MinionJuvenileFireWywern },
            { MinionID.JuvenileForestSpider, MinionJuvenileForestSpider },
            { MinionID.JuvenileHawk, MinionJuvenileHawk },
            { MinionID.JuvenileIceDrake, MinionJuvenileIceDrake },
            { MinionID.JuvenileJacaranda, MinionJuvenileJacaranda },
            { MinionID.JuvenileJaguar, MinionJuvenileJaguar },
            { MinionID.JuvenileJungleSpider, MinionJuvenileJungleSpider },
            { MinionID.JuvenileJungleStalker, MinionJuvenileJungleStalker },
            { MinionID.JuvenileKrytanDrakehound, MinionJuvenileKrytanDrakehound },
            { MinionID.JuvenileLashtailDevourer, MinionJuvenileLashtailDevourer },
            { MinionID.JuvenileLynx, MinionJuvenileLynx },
            { MinionID.JuvenileMarshDrake, MinionJuvenileMarshDrake },
            { MinionID.JuvenileMurellow, MinionJuvenileMurellow },
            { MinionID.JuvenileOwl, MinionJuvenileOwl },
            { MinionID.JuvenilePhoenix, MinionJuvenilePhoenix },
            { MinionID.JuvenilePig, MinionJuvenilePig },
            { MinionID.JuvenilePinkMoa, MinionJuvenilePinkMoa },
            { MinionID.JuvenilePolarBear, MinionJuvenilePolarBear },
            { MinionID.JuvenileRainbowJellyfish, MinionJuvenileBlueRainbowJellyfish },
            { MinionID.JuvenileRaven, MinionJuvenileRaven },
            { MinionID.JuvenileRedJellyfish, MinionJuvenileRedJellyfish },
            { MinionID.JuvenileRedMoa, MinionJuvenileRedMoa },
            { MinionID.JuvenileReefDrake, MinionJuvenileReefDrake },
            { MinionID.JuvenileRiverDrake, MinionJuvenileRiverDrake },
            { MinionID.JuvenileRockGazelle, MinionJuvenileRockGazelle },
            { MinionID.JuvenileSalamanderDrake, MinionJuvenileSalamanderDraker },
            { MinionID.JuvenileSandLion, MinionJuvenileSandLion },
            { MinionID.JuvenileShark, MinionJuvenileShark },
            { MinionID.JuvenileSiamoth, MinionJuvenileSiamoth },
            { MinionID.JuvenileSiegeTurtle, MinionJuvenileSiegeTurtle },
            { MinionID.JuvenileSmokescale, MinionJuvenileSmokescale },
            { MinionID.JuvenileSnowLeopard, MinionJuvenileSnowLeopard },
            { MinionID.JuvenileTiger, MinionJuvenileTiger },
            { MinionID.JuvenileWallow, MinionJuvenileWallow },
            { MinionID.JuvenileWarthog, MinionJuvenileWarthog },
            { MinionID.JuvenileWhiptailDevourer, MinionJuvenileWhiptailDevourer },
            { MinionID.JuvenileWhiteMoa, MinionJuvenileWhiteMoa },
            { MinionID.JuvenileWhiteRaven, MinionJuvenileWhiteRaven },
            { MinionID.JuvenileWhiteTiger, MinionJuvenileWhiteTiger },
            { MinionID.JuvenileWolf, MinionJuvenileWolf },
            { MinionID.JuvenileHyena, MinionJuvenileHyena },
            { MinionID.JuvenileAetherHunter, MinionJuvenileAetherHunter },
            { MinionID.JuvenileSkyChakStriker, MinionSkyChakStriker },
            { MinionID.JuvenileSpinegazer, MinionSpinegazer },
            { MinionID.BloodFiend, MinionBloodFiend },
            { MinionID.BoneFiend, MinionBoneFiend },
            { MinionID.FleshGolem, MinionFleshGolem },
            { MinionID.ShadowFiend, MinionShadowFiend },
            { MinionID.FleshWurm, MinionFleshWurm },
            { MinionID.UnstableHorror, MinionUnstableHorror },
            { MinionID.ShamblingHorror, MinionShamblingHorror },
            { MinionID.Thief1, MinionThievesGuild },
            { MinionID.Thief2, MinionThievesGuild },
            { MinionID.Thief3, MinionThievesGuild },
            { MinionID.Thief4, MinionThievesGuild },
            { MinionID.Thief5, MinionThievesGuild },
            { MinionID.Thief6, MinionThievesGuild },
            { MinionID.Thief7, MinionThievesGuild },
            { MinionID.Thief8, MinionThievesGuild },
            { MinionID.Thief9, MinionThievesGuild },
            { MinionID.Thief10, MinionThievesGuild },
            { MinionID.Thief11, MinionThievesGuild },
            { MinionID.Thief12, MinionThievesGuild },
            { MinionID.Thief13, MinionThievesGuild },
            { MinionID.Thief14, MinionThievesGuild },
            { MinionID.Thief15, MinionThievesGuild },
            { MinionID.Thief16, MinionThievesGuild },
            { MinionID.Thief17, MinionThievesGuild },
            { MinionID.Thief18, MinionThievesGuild },
            { MinionID.Thief19, MinionThievesGuild },
            { MinionID.Thief20, MinionThievesGuild },
            { MinionID.Thief21, MinionThievesGuild },
            { MinionID.Thief22, MinionThievesGuild },
            { MinionID.Daredevil1, MinionThievesGuild },
            { MinionID.Daredevil2, MinionThievesGuild },
            { MinionID.Daredevil3, MinionThievesGuild },
            { MinionID.Daredevil4, MinionThievesGuild },
            { MinionID.Daredevil5, MinionThievesGuild },
            { MinionID.Daredevil6, MinionThievesGuild },
            { MinionID.Daredevil7, MinionThievesGuild },
            { MinionID.Daredevil8, MinionThievesGuild },
            { MinionID.Daredevil9, MinionThievesGuild },
            { MinionID.Daredevil10, MinionThievesGuild },
            { MinionID.Deadeye1, MinionThievesGuild },
            { MinionID.Deadeye2, MinionThievesGuild },
            { MinionID.Deadeye3, MinionThievesGuild },
            { MinionID.Deadeye4, MinionThievesGuild },
            { MinionID.Deadeye5, MinionThievesGuild },
            { MinionID.Deadeye6, MinionThievesGuild },
            { MinionID.Deadeye7, MinionThievesGuild },
            { MinionID.Deadeye8, MinionThievesGuild },
            { MinionID.Deadeye9, MinionThievesGuild },
            { MinionID.Deadeye10, MinionThievesGuild },
            { MinionID.Specter1, MinionThievesGuild },
            { MinionID.Specter2, MinionThievesGuild },
            { MinionID.Specter3, MinionThievesGuild },
            { MinionID.Specter4, MinionThievesGuild },
            { MinionID.Specter5, MinionThievesGuild },
            { MinionID.Specter6, MinionThievesGuild },
            { MinionID.Specter7, MinionThievesGuild },
            { MinionID.Specter8, MinionThievesGuild },
            { MinionID.Specter9, MinionThievesGuild },
            { MinionID.Specter10, MinionThievesGuild },
            { MinionID.BowOfTruth, MinionBowOfTruth },
            { MinionID.HammerOfWisdom, MinionHammerOfWisdom },
            { MinionID.ShieldOfTheAvenger, MinionShieldOfTheAvenger },
            { MinionID.SwordOfJustice, MinionSwordOfJustice },
            { MinionID.LesserAirElemental, MinionLesserAirElemental },
            { MinionID.LesserEarthElemental, MinionLesserEarthElemental },
            { MinionID.LesserFireElemental, MinionLesserFireElemental },
            { MinionID.LesserIceElemental, MinionLesserIceElemental },
            { MinionID.AirElemental, MinionAirElemental },
            { MinionID.EarthElemental, MinionEarthElemental },
            { MinionID.FireElemental, MinionFireElemental },
            { MinionID.IceElemental, MinionIceElemental },
            { MinionID.BlastGyro, MinionBlastGyro },
            { MinionID.BulwarkGyro, MinionBulwarkGyro },
            { MinionID.FunctionGyro, MinionFunctionGyro },
            { MinionID.MedicGyro, MinionMedicGyro },
            { MinionID.PurgeGyro, MinionPurgeGyro },
            { MinionID.ShredderGyro, MinionShredderGyro },
            { MinionID.SneakGyro, MinionSneakGyro },
        };



        /// <summary>
        /// Matches the Squad Marker GUIDs to the relative icons.
        /// </summary>
        public static IReadOnlyDictionary<string, string> SquadMarkerGUIDsToIcon { get; set; } = new Dictionary<string, string>()
        {
            { MarkerGUIDs.ArrowOverhead, ArrowSquadMarkerOverhead },
            { MarkerGUIDs.CircleOverhead, CircleSquadMarkerOverhead },
            { MarkerGUIDs.HeartOverhead, HeartSquadMarkerOverhead },
            { MarkerGUIDs.SquareOverhead, SquareSquadMarkerOverhead },
            { MarkerGUIDs.StarOverhead, StarSquadMarkerOverhead },
            { MarkerGUIDs.SwirlOverhead, SwirlSquadMarkerOverhead },
            { MarkerGUIDs.TriangleOverhead, TriangleSquadMarkerOverhead },
            { MarkerGUIDs.XOverhead, XSquadMarkerOverhead },
        };


        /// <summary>
        /// Matches the Squad Marker Index to the relative icons.
        /// </summary>
        public static IReadOnlyDictionary<SquadMarkerIndex, string> SquadMarkerIndexToIcon { get; set; } = new Dictionary<SquadMarkerIndex, string>()
        {
            { SquadMarkerIndex.Arrow, ArrowSquadMarkerOverhead },
            { SquadMarkerIndex.Circle, CircleSquadMarkerOverhead },
            { SquadMarkerIndex.Heart, HeartSquadMarkerOverhead },
            { SquadMarkerIndex.Square, SquareSquadMarkerOverhead },
            { SquadMarkerIndex.Star, StarSquadMarkerOverhead },
            { SquadMarkerIndex.Swirl, SwirlSquadMarkerOverhead },
            { SquadMarkerIndex.Triangle, TriangleSquadMarkerOverhead },
            { SquadMarkerIndex.X, XSquadMarkerOverhead },
        };

        /// <summary>
        /// Matches the Commander/Catmander Tag GUIDs to the relative icons.
        /// </summary>
        public static IReadOnlyDictionary<string, string> CommanderTagToIcon { get; set; } = new Dictionary<string, string>()
        {
            { MarkerGUIDs.RedCommanderTag, RedCommanderTagOverhead },
            { MarkerGUIDs.OrangeCommanderTag, OrangeCommanderTagOverhead },
            { MarkerGUIDs.YellowCommanderTag, YellowCommanderTagOverhead },
            { MarkerGUIDs.GreenCommanderTag, GreenCommanderTagOverhead },
            { MarkerGUIDs.CyanCommanderTag, CyanCommanderTagOverhead },
            { MarkerGUIDs.BlueCommanderTag, BlueCommanderTagOverhead },
            { MarkerGUIDs.PurpleCommanderTag, PurpleCommanderTagOverhead },
            { MarkerGUIDs.PinkCommanderTag, PinkCommanderTagOverhead },
            { MarkerGUIDs.WhiteCommanderTag, WhiteCommanderTagOverhead },
            { MarkerGUIDs.RedCatmanderTag, RedCatmanderTagOverhead },
            { MarkerGUIDs.OrangeCatmanderTag, OrangeCatmanderTagOverhead },
            { MarkerGUIDs.YellowCatmanderTag, YellowCatmanderTagOverhead },
            { MarkerGUIDs.GreenCatmanderTag, GreenCatmanderTagOverhead },
            { MarkerGUIDs.CyanCatmanderTag, CyanCatmanderTagOverhead },
            { MarkerGUIDs.BlueCatmanderTag, BlueCatmanderTagOverhead },
            { MarkerGUIDs.PurpleCatmanderTag, PurpleCatmanderTagOverhead },
            { MarkerGUIDs.PinkCatmanderTag, PinkCatmanderTagOverhead },
            { MarkerGUIDs.WhiteCatmanderTag, WhiteCatmanderTagOverhead },
        };
    }
}
