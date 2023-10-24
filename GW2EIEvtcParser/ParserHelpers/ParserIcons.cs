using System.Collections.Generic;
using static System.Net.WebRequestMethods;
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
        private const string TrashHostilePeerlessQadimPylon = "https://i.imgur.com/b33vAEQ.png";
        private const string TrashEntropicDistortion = "https://i.imgur.com/MIpP5pK.png";
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
        private const string TrashVoidAmalgamate = "https://i.imgur.com/BuKbosz.png";
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

        // Portal icons
        internal const string PortalMesmerEntre = "https://i.imgur.com/TILHQ1u.png";
        internal const string PortalMesmerExeunt = "https://i.imgur.com/7gsObHV.png";
        internal const string PortalShadowPortalPrepare = "https://i.imgur.com/oDpQcea.png";
        internal const string PortalShadowPortalOpen = "https://i.imgur.com/P7YI6am.png";
        internal const string PortalSandswell = "https://i.imgur.com/15kzkcj.png";
        internal const string PortalWhiteMantleItem = "https://i.imgur.com/FcO9n7c.png";
        internal const string PortalWhiteMantleSkill = "https://i.imgur.com/qf99SF9.png";

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
        internal const string EffectGeyser = "https://i.imgur.com/4h9HaFh.png";
        internal const string EffectDeployJadeSphereFire = "https://i.imgur.com/qdkoYap.png";
        internal const string EffectDeployJadeSphereWater = "https://i.imgur.com/xy0SAxK.png";
        internal const string EffectDeployJadeSphereAir = "https://i.imgur.com/SvUQmyj.png";
        internal const string EffectDeployJadeSphereEarth = "https://i.imgur.com/KG8ysd9.png";

        // Overhead icons
        // - Fixations
        internal const string FixationBlueOverhead = "https://i.imgur.com/EUoDTln.png";
        internal const string FixationGreenOverhead = "https://i.imgur.com/cDmJWrY.png";
        internal const string FixationPurpleOverhead = "https://i.imgur.com/UImUF0H.png";
        internal const string FixationRedOverhead = "https://i.imgur.com/wIYRfY6.png";
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
            { ArcDPSEnums.TargetID.WorldVersusWorld, TargetWorldVersusWorld },
            { ArcDPSEnums.TargetID.Mordremoth, TargetMordremoth },
            { ArcDPSEnums.TargetID.ValeGuardian, TargetValeGuardian },
            { ArcDPSEnums.TargetID.Gorseval, TargetGorseval },
            { ArcDPSEnums.TargetID.Sabetha, TargetSabetha },
            { ArcDPSEnums.TargetID.Slothasor, TargetSlothasor },
            { ArcDPSEnums.TargetID.Berg, TargetBerg },
            { ArcDPSEnums.TargetID.Narella, TargetNarella },
            { ArcDPSEnums.TargetID.Zane, TargetZane},
            { ArcDPSEnums.TargetID.Matthias, TargetMatthias },
            { ArcDPSEnums.TargetID.McLeodTheSilent, TargetMcLeodTheSilent },
            { ArcDPSEnums.TargetID.KeepConstruct, TargetKeepConstruct },
            { ArcDPSEnums.TargetID.Xera, TargetXera },
            { ArcDPSEnums.TargetID.Cairn, TargetCairn },
            { ArcDPSEnums.TargetID.MursaatOverseer, TargetMursaatOverseer },
            { ArcDPSEnums.TargetID.Samarog, TargetSamarog },
            { ArcDPSEnums.TargetID.Deimos, TargetDeimos },
            { ArcDPSEnums.TargetID.SoullessHorror, TargetDesmina },
            { ArcDPSEnums.TargetID.Desmina, TargetDesmina },
            { ArcDPSEnums.TargetID.BrokenKing, TargetBrokenKing },
            { ArcDPSEnums.TargetID.EaterOfSouls, TargetEaterOfSouls },
            { ArcDPSEnums.TargetID.EyeOfFate, TargetEyes },
            { ArcDPSEnums.TargetID.EyeOfJudgement, TargetEyes },
            { ArcDPSEnums.TargetID.Dhuum, TargetDhuum },
            { ArcDPSEnums.TargetID.ConjuredAmalgamate, TargetConjuredAmalgamate },
            { ArcDPSEnums.TargetID.CALeftArm, TargetCALeftArm },
            { ArcDPSEnums.TargetID.CARightArm, TargetCARightArm },
            { ArcDPSEnums.TargetID.Kenut, TargetKenut },
            { ArcDPSEnums.TargetID.Nikare, TargetNikare },
            { ArcDPSEnums.TargetID.Qadim, TargetQadim },
            { ArcDPSEnums.TargetID.Freezie, TargetFreezie },
            { ArcDPSEnums.TargetID.Adina, TargetAdina },
            { ArcDPSEnums.TargetID.Sabir, TargetSabir },
            { ArcDPSEnums.TargetID.PeerlessQadim, TargetPeerlessQadim },
            { ArcDPSEnums.TargetID.IcebroodConstruct, TargetIcebroodConstruct },
            { ArcDPSEnums.TargetID.IcebroodConstructFraenir, TargetIcebroodConstruct },
            { ArcDPSEnums.TargetID.ClawOfTheFallen, TargetClawOfTheFallen },
            { ArcDPSEnums.TargetID.VoiceOfTheFallen, TargetVoiceOfTheFallen },
            { ArcDPSEnums.TargetID.VoiceAndClaw, TargetVoiceAndClaw },
            { ArcDPSEnums.TargetID.FraenirOfJormag, TargetFraenirOfJormag },
            { ArcDPSEnums.TargetID.Boneskinner, TargetBoneskinner },
            { ArcDPSEnums.TargetID.WhisperOfJormag, TargetWhisperOfJormag },
            { ArcDPSEnums.TargetID.VariniaStormsounder, TargetVariniaStormsounder },
            { ArcDPSEnums.TargetID.MAMA, TargetMAMA },
            { ArcDPSEnums.TargetID.Siax, TargetSiax},
            { ArcDPSEnums.TargetID.Ensolyss, TargetEnsolyss },
            { ArcDPSEnums.TargetID.Skorvald, TargetSkorvald },
            { ArcDPSEnums.TargetID.Artsariiv, TargetArtsariiv },
            { ArcDPSEnums.TargetID.Arkk, TargetArkk },
            { ArcDPSEnums.TargetID.AiKeeperOfThePeak, TargetAiKeeperOfThePeak },
            { ArcDPSEnums.TargetID.AiKeeperOfThePeak2, TargetAiKeeperOfThePeak2 },
            { ArcDPSEnums.TargetID.LGolem, TargetVitalGolem },
            { ArcDPSEnums.TargetID.VitalGolem, TargetVitalGolem },
            { ArcDPSEnums.TargetID.PowerGolem, TargetPowerGolem },
            { ArcDPSEnums.TargetID.ConditionGolem, TargetPowerGolem },
            { ArcDPSEnums.TargetID.MedGolem, TargetPowerGolem },
            { ArcDPSEnums.TargetID.AvgGolem, TargetPowerGolem },
            { ArcDPSEnums.TargetID.StdGolem, TargetStdGolem },
            { ArcDPSEnums.TargetID.MassiveGolem10M, TargetMassiveGolem },
            { ArcDPSEnums.TargetID.MassiveGolem4M, TargetMassiveGolem },
            { ArcDPSEnums.TargetID.MassiveGolem1M, TargetMassiveGolem },
            { ArcDPSEnums.TargetID.MaiTrinFract, TargetMaiTrin },
            { ArcDPSEnums.TargetID.MaiTrinStrike, TargetMaiTrin },
            { ArcDPSEnums.TargetID.EchoOfScarletBriarNM, TargetEchoOfScarletBriar },
            { ArcDPSEnums.TargetID.EchoOfScarletBriarCM, TargetEchoOfScarletBriar },
            { ArcDPSEnums.TargetID.Ankka, TargetAnkka },
            { ArcDPSEnums.TargetID.MinisterLi, TargetMinisterLi },
            { ArcDPSEnums.TargetID.MinisterLiCM, TargetMinisterLi },
            { ArcDPSEnums.TargetID.TheDragonVoidJormag, TargetTheDragonVoidJormag },
            { ArcDPSEnums.TargetID.TheDragonVoidKralkatorrik, TargetTheDragonVoidKralkatorrik },
            { ArcDPSEnums.TargetID.TheDragonVoidMordremoth, TargetTheDragonVoidMordremoth },
            { ArcDPSEnums.TargetID.TheDragonVoidPrimordus, TargetTheDragonVoidPrimordus },
            { ArcDPSEnums.TargetID.TheDragonVoidSooWon, TargetTheDragonVoidSooWon },
            { ArcDPSEnums.TargetID.TheDragonVoidZhaitan, TargetTheDragonVoidZhaitan },
            { ArcDPSEnums.TargetID.SooWonOW, TargetSooWonOW },
            { ArcDPSEnums.TargetID.PrototypeVermilion, TargetPrototypeVermilion },
            { ArcDPSEnums.TargetID.PrototypeVermilionCM, TargetPrototypeVermilion },
            { ArcDPSEnums.TargetID.PrototypeArsenite, TargetPrototypeArsenite },
            { ArcDPSEnums.TargetID.PrototypeArseniteCM, TargetPrototypeArsenite },
            { ArcDPSEnums.TargetID.PrototypeIndigo, TargetPrototypeIndigo },
            { ArcDPSEnums.TargetID.PrototypeIndigoCM, TargetPrototypeIndigo },
            { ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM, TargetKanaxai },
            { ArcDPSEnums.TargetID.Dagda, TargetDagda },
            { ArcDPSEnums.TargetID.Cerus, TargetCerus },
        };

        /// <summary>
        /// Dictionary matching a <see cref="ArcDPSEnums.TrashID"/> to their icon.
        /// </summary>
        internal static IReadOnlyDictionary<ArcDPSEnums.TrashID, string> TrashNPCIcons { get; private set; } = new Dictionary<ArcDPSEnums.TrashID, string>()
        {
            { ArcDPSEnums.TrashID.Canach, TrashCanach },
            { ArcDPSEnums.TrashID.Braham, TrashBraham },
            { ArcDPSEnums.TrashID.Caithe, TrashCaithe },
            { ArcDPSEnums.TrashID.BlightedRytlock, TrashBlightedRytlock },
            //{ ArcDPSEnums.TrashID.BlightedCanach, TrashBlightedCanach },
            { ArcDPSEnums.TrashID.BlightedBraham, TrashBlightedBraham },
            { ArcDPSEnums.TrashID.BlightedMarjory, TrashBlightedMarjory },
            { ArcDPSEnums.TrashID.BlightedCaithe, TrashBlightedCaithe },
            { ArcDPSEnums.TrashID.BlightedForgal, TrashBlightedForgal },
            { ArcDPSEnums.TrashID.BlightedSieran, TrashBlightedSieran },
            //{ ArcDPSEnums.TrashID.BlightedTybalt, TrashBlightedTybalt },
            //{ ArcDPSEnums.TrashID.BlightedPaleTree, TrashBlightedPaleTree },
            //{ ArcDPSEnums.TrashID.BlightedTrahearne, TrashBlightedTrahearne },
            //{ ArcDPSEnums.TrashID.BlightedEir, TrashBlightedEir },
            { ArcDPSEnums.TrashID.Spirit, TrashSpiritDemonSoul },
            { ArcDPSEnums.TrashID.Spirit2, TrashSpiritDemonSoul },
            { ArcDPSEnums.TrashID.ChargedSoul, TrashSpiritDemonSoul },
            { ArcDPSEnums.TrashID.HollowedBomber, TrashSpiritDemonSoul },
            { ArcDPSEnums.TrashID.Saul, TrashSaul },
            { ArcDPSEnums.TrashID.ShackledPrisoner, TrashShackledPrisoner },
            { ArcDPSEnums.TrashID.DemonicBond, TrashDemonicBond },
            { ArcDPSEnums.TrashID.GamblerClones, TrashGamblerClones },
            { ArcDPSEnums.TrashID.BloodstoneFragment, TrashChargedBloodstoneFragment },
            { ArcDPSEnums.TrashID.ChargedBloodstone, TrashChargedBloodstoneFragment },
            { ArcDPSEnums.TrashID.BloodstoneShardMainFight, TrashGamblerReal },
            { ArcDPSEnums.TrashID.BloodstoneShardRift, TrashGamblerReal },
            { ArcDPSEnums.TrashID.BloodstoneShardButton, TrashGamblerReal },
            { ArcDPSEnums.TrashID.GamblerReal, TrashGamblerReal },
            { ArcDPSEnums.TrashID.Pride, TrashPride },
            { ArcDPSEnums.TrashID.OilSlick, TrashOil },
            { ArcDPSEnums.TrashID.Oil, TrashOil },
            { ArcDPSEnums.TrashID.Tear, TrashTear },
            { ArcDPSEnums.TrashID.Gambler, TrashGamblerDrunkarThief },
            { ArcDPSEnums.TrashID.Drunkard, TrashGamblerDrunkarThief },
            { ArcDPSEnums.TrashID.Thief, TrashGamblerDrunkarThief },
            { ArcDPSEnums.TrashID.TormentedDead, TrashTormentedDeadMessenger },
            { ArcDPSEnums.TrashID.Messenger, TrashTormentedDeadMessenger },
            { ArcDPSEnums.TrashID.Enforcer, TrashEnforcer },
            { ArcDPSEnums.TrashID.Echo, TrashEcho },
            { ArcDPSEnums.TrashID.KeepConstructCore, TrashKeepConstructCoreExquisiteConjunction },
            { ArcDPSEnums.TrashID.ExquisiteConjunction, TrashKeepConstructCoreExquisiteConjunction },
            { ArcDPSEnums.TrashID.Jessica, TrashKeepConstructGhosts },
            { ArcDPSEnums.TrashID.Olson, TrashKeepConstructGhosts },
            { ArcDPSEnums.TrashID.Engul, TrashKeepConstructGhosts },
            { ArcDPSEnums.TrashID.Faerla, TrashKeepConstructGhosts },
            { ArcDPSEnums.TrashID.Caulle, TrashKeepConstructGhosts },
            { ArcDPSEnums.TrashID.Henley, TrashKeepConstructGhosts },
            { ArcDPSEnums.TrashID.Galletta, TrashKeepConstructGhosts },
            { ArcDPSEnums.TrashID.Ianim, TrashKeepConstructGhosts },
            { ArcDPSEnums.TrashID.InsidiousProjection, TrashInsidiousProjection },
            { ArcDPSEnums.TrashID.EnergyOrb, TrashEnergyOrb },
            { ArcDPSEnums.TrashID.UnstableLeyRift, TrashUnstableLeyRift },
            { ArcDPSEnums.TrashID.RadiantPhantasm, TrashRadiantPhantasm },
            { ArcDPSEnums.TrashID.CrimsonPhantasm, TrashCrimsonPhantasm },
            { ArcDPSEnums.TrashID.Storm, TrashStorm },
            { ArcDPSEnums.TrashID.IcePatch, TrashIcePatch },
            { ArcDPSEnums.TrashID.BanditSaboteur, TrashBanditSaboteur },
            { ArcDPSEnums.TrashID.NarellaTornado, TrashTornado },
            { ArcDPSEnums.TrashID.Tornado, TrashTornado },
            { ArcDPSEnums.TrashID.Jade, TrashJade },
            { ArcDPSEnums.TrashID.AngryZommoros, TrashAngryChillZommoros },
            { ArcDPSEnums.TrashID.ChillZommoros, TrashAngryChillZommoros },
            { ArcDPSEnums.TrashID.AncientInvokedHydra, TrashAncientInvokedHydra },
            { ArcDPSEnums.TrashID.IcebornHydra, TrashIcebornHydra },
            { ArcDPSEnums.TrashID.IceElemental, TrashIceElemental },
            { ArcDPSEnums.TrashID.WyvernMatriarch, TrashWyvernMatriarch },
            { ArcDPSEnums.TrashID.WyvernPatriarch, TrashWyvernPatriarch },
            { ArcDPSEnums.TrashID.ApocalypseBringer, TrashApocalypseBringer },
            { ArcDPSEnums.TrashID.ConjuredGreatsword, TrashConjuredGreatsword },
            { ArcDPSEnums.TrashID.ConjuredPlayerSword, TrashConjuredPlayerSword },
            { ArcDPSEnums.TrashID.ConjuredShield, TrashConjuredShield },
            { ArcDPSEnums.TrashID.GreaterMagmaElemental1, TrashGreaterMagmaElemental },
            { ArcDPSEnums.TrashID.GreaterMagmaElemental2, TrashGreaterMagmaElemental },
            { ArcDPSEnums.TrashID.LavaElemental1, TrashLavaElemental },
            { ArcDPSEnums.TrashID.LavaElemental2, TrashLavaElemental },
            { ArcDPSEnums.TrashID.PyreGuardian, TrashPyreGuardianKillerTornado },
            { ArcDPSEnums.TrashID.SmallKillerTornado, TrashPyreGuardianKillerTornado},
            { ArcDPSEnums.TrashID.BigKillerTornado, TrashPyreGuardianKillerTornado },
            { ArcDPSEnums.TrashID.PoisonMushroom, TrashPoisonMushroom },
            { ArcDPSEnums.TrashID.SpearAggressionRevulsion, TrashSpearAggressionRevulsion },
            { ArcDPSEnums.TrashID.QadimLamp, TrashQadimLamp },
            { ArcDPSEnums.TrashID.PyreGuardianRetal, TrashPyreGuardianRetal },
            { ArcDPSEnums.TrashID.PyreGuardianResolution, TrashPyreGuardianResolution },
            { ArcDPSEnums.TrashID.PyreGuardianStab, TrashPyreGuardianStab },
            { ArcDPSEnums.TrashID.PyreGuardianProtect, TrashPyreGuardianProtect },
            { ArcDPSEnums.TrashID.ReaperOfFlesh, TrashReaperofFlesh },
            { ArcDPSEnums.TrashID.Kernan, TrashKernan },
            { ArcDPSEnums.TrashID.Knuckles, TrashKnuckles },
            { ArcDPSEnums.TrashID.Karde, TrashKarde },
            { ArcDPSEnums.TrashID.Rigom, TrashRigom },
            { ArcDPSEnums.TrashID.Guldhem, TrashGuldhem },
            { ArcDPSEnums.TrashID.Scythe, TrashScythe },
            { ArcDPSEnums.TrashID.SmotheringShadow, TrashSmotheringShadow },
            { ArcDPSEnums.TrashID.MazeMinotaur, TrashMazeMinotaur },
            { ArcDPSEnums.TrashID.VoidSaltsprayDragon, TrashVoidSaltsprayDragon },
            { ArcDPSEnums.TrashID.BanditBombardier, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.EchoOfTheUnclean, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.SurgingSoul, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.Enervator, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.WhisperEcho, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.DoppelgangerElementalist, TrashDoppelgangerElementalist },
            { ArcDPSEnums.TrashID.DoppelgangerElementalist2, TrashDoppelgangerElementalist },
            { ArcDPSEnums.TrashID.DoppelgangerEngineer, TrashDoppelgangerEngineer },
            { ArcDPSEnums.TrashID.DoppelgangerEngineer2, TrashDoppelgangerEngineer },
            { ArcDPSEnums.TrashID.DoppelgangerGuardian, TrashDoppelgangerGuardian },
            { ArcDPSEnums.TrashID.DoppelgangerGuardian2, TrashDoppelgangerGuardian },
            { ArcDPSEnums.TrashID.DoppelgangerMesmer, TrashDoppelgangerMesmer },
            { ArcDPSEnums.TrashID.DoppelgangerMesmer2, TrashDoppelgangerMesmer },
            { ArcDPSEnums.TrashID.DoppelgangerNecromancer, TrashDoppelgangerNecromancer },
            { ArcDPSEnums.TrashID.DoppelgangerNecromancer2, TrashDoppelgangerNecromancer },
            { ArcDPSEnums.TrashID.DoppelgangerRanger, TrashDoppelgangerRanger },
            { ArcDPSEnums.TrashID.DoppelgangerRanger2, TrashDoppelgangerRanger },
            { ArcDPSEnums.TrashID.DoppelgangerRevenant, TrashDoppelgangerRevenant },
            { ArcDPSEnums.TrashID.DoppelgangerRevenant2, TrashDoppelgangerRevenant },
            { ArcDPSEnums.TrashID.DoppelgangerThief, TrashDoppelgangerThief },
            { ArcDPSEnums.TrashID.DoppelgangerThief2, TrashDoppelgangerThief },
            { ArcDPSEnums.TrashID.DoppelgangerWarrior, TrashDoppelgangerWarrior },
            { ArcDPSEnums.TrashID.DoppelgangerWarrior2, TrashDoppelgangerWarrior },
            { ArcDPSEnums.TrashID.CharrTank, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.PropagandaBallon, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.EnragedWaterSprite, TrashEnragedWaterSprite },
            { ArcDPSEnums.TrashID.FearDemon, TrashFear },
            { ArcDPSEnums.TrashID.GuiltDemon, TrashGuilt },
            { ArcDPSEnums.TrashID.AiDoubtDemon, TrashAiDoubt },
            { ArcDPSEnums.TrashID.PlayerDoubtDemon, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.TransitionSorrowDemon1, TrashTransitionSorrow },
            { ArcDPSEnums.TrashID.TransitionSorrowDemon2, TrashTransitionSorrow },
            { ArcDPSEnums.TrashID.TransitionSorrowDemon3, TrashTransitionSorrow },
            { ArcDPSEnums.TrashID.TransitionSorrowDemon4, TrashTransitionSorrow },
            { ArcDPSEnums.TrashID.CCSorrowDemon, TrashCCSorrow },
            { ArcDPSEnums.TrashID.ScarletPhantomHP, TrashScarletPhantom },
            { ArcDPSEnums.TrashID.ScarletPhantomHPCM, TrashScarletPhantom },
            { ArcDPSEnums.TrashID.ScarletPhantomDeathBeamCM, TrashScarletPhantom },
            { ArcDPSEnums.TrashID.ScarletPhantomDeathBeamCM2, TrashScarletPhantom },
            { ArcDPSEnums.TrashID.ScarletPhantomBreakbar, TrashScarletPhantom },
            { ArcDPSEnums.TrashID.ScarletPhantomNormalBeam, TrashScarletPhantom },
            { ArcDPSEnums.TrashID.ScarletPhantomConeWaveNM, TrashScarletPhantom },
            { ArcDPSEnums.TrashID.FerrousBomb, TrashFerrousBomb },
            { ArcDPSEnums.TrashID.KraitsHallucination, TrashKraitHallucination },
            { ArcDPSEnums.TrashID.LichHallucination, TrashLichHallucination },
            { ArcDPSEnums.TrashID.QuaggansHallucinationNM, TrashQuagganHallucination },
            { ArcDPSEnums.TrashID.QuaggansHallucinationCM, TrashQuagganHallucination },
            { ArcDPSEnums.TrashID.ReanimatedAntipathy, TrashReanimatedAntipathy },
            { ArcDPSEnums.TrashID.ReanimatedMalice1, TrashReanimatedMalice },
            { ArcDPSEnums.TrashID.ReanimatedMalice2, TrashReanimatedMalice },
            { ArcDPSEnums.TrashID.ReanimatedHatred, TrashReanimatedHatred },
            { ArcDPSEnums.TrashID.ReanimatedSpite, TrashReanimatedSpite },
            { ArcDPSEnums.TrashID.SanctuaryPrism, TrashSanctuaryPrism },
            { ArcDPSEnums.TrashID.VoidBrandstalker, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.SpiritOfDestruction, TrashSpiritOfDestructionOrPain },
            { ArcDPSEnums.TrashID.SpiritOfPain, TrashSpiritOfDestructionOrPain },
            { ArcDPSEnums.TrashID.DragonEnergyOrb, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.HandOfErosion, TrashHandOfErosionEruption },
            { ArcDPSEnums.TrashID.HandOfEruption, TrashHandOfErosionEruption },
            { ArcDPSEnums.TrashID.VoltaicWisp, TrashVoltaicWisp },
            { ArcDPSEnums.TrashID.ParalyzingWisp, TrashParalyzingWisp },
            { ArcDPSEnums.TrashID.HostilePeerlessQadimPylon, TrashHostilePeerlessQadimPylon },
            { ArcDPSEnums.TrashID.EntropicDistortion, TrashEntropicDistortion },
            { ArcDPSEnums.TrashID.SmallJumpyTornado, TrashSmallJumpyTornado },
            { ArcDPSEnums.TrashID.OrbSpider, TrashOrbSpider },
            { ArcDPSEnums.TrashID.Seekers, TrashSeekers },
            { ArcDPSEnums.TrashID.BlueGuardian, TrashBlueGuardian },
            { ArcDPSEnums.TrashID.GreenGuardian, TrashGreenGuardian },
            { ArcDPSEnums.TrashID.RedGuardian, TrashRedGuardian },
            { ArcDPSEnums.TrashID.UnderworldReaper, TrashUnderworldReaper },
            { ArcDPSEnums.TrashID.VeteranTorturedWarg, TrashVeteranTorturedWarg },
            { ArcDPSEnums.TrashID.GreenSpirit1, TrashGenericFriendlyTarget },
            { ArcDPSEnums.TrashID.GreenSpirit2, TrashGenericFriendlyTarget },
            { ArcDPSEnums.TrashID.BanditSapper, TrashGenericFriendlyTarget },
            { ArcDPSEnums.TrashID.ProjectionArkk, TrashGenericFriendlyTarget },
            { ArcDPSEnums.TrashID.PrioryExplorer, TrashGenericFriendlyTarget },
            { ArcDPSEnums.TrashID.PrioryScholar, TrashGenericFriendlyTarget },
            { ArcDPSEnums.TrashID.VigilRecruit, TrashGenericFriendlyTarget },
            { ArcDPSEnums.TrashID.VigilTactician, TrashGenericFriendlyTarget },
            { ArcDPSEnums.TrashID.Prisoner1, TrashGenericFriendlyTarget },
            { ArcDPSEnums.TrashID.Prisoner2, TrashGenericFriendlyTarget },
            { ArcDPSEnums.TrashID.FriendlyPeerlessQadimPylon, TrashGenericFriendlyTarget },
            { ArcDPSEnums.TrashID.Mine, TrashMine },
            { ArcDPSEnums.TrashID.FleshWurm, TrashFleshWurm },
            { ArcDPSEnums.TrashID.Hands, TrashHands },
            { ArcDPSEnums.TrashID.TemporalAnomaly, TrashTemporalAnomaly },
            { ArcDPSEnums.TrashID.TemporalAnomaly2, TrashTemporalAnomaly },
            { ArcDPSEnums.TrashID.DOC, TrashDOC_BLIGHT_PLINK_CHOP },
            { ArcDPSEnums.TrashID.BLIGHT, TrashDOC_BLIGHT_PLINK_CHOP },
            { ArcDPSEnums.TrashID.PLINK, TrashDOC_BLIGHT_PLINK_CHOP },
            { ArcDPSEnums.TrashID.CHOP, TrashDOC_BLIGHT_PLINK_CHOP },
            { ArcDPSEnums.TrashID.Archdiviner, TrashArchdiviner },
            { ArcDPSEnums.TrashID.EliteBrazenGladiator, TrashEliteBrazenGladiator },
            { ArcDPSEnums.TrashID.Fanatic, TrashFanatic },
            { ArcDPSEnums.TrashID.FreeziesFrozenHeart, TrashFreeziesFrozenHeart },
            { ArcDPSEnums.TrashID.IceSpiker, TrashIceSpiker },
            { ArcDPSEnums.TrashID.IceStormer, TrashIceElemental },
            { ArcDPSEnums.TrashID.IcyProtector, TrashIcyProtector },
            { ArcDPSEnums.TrashID.RiverOfSouls, TrashRiverOfSouls },
            { ArcDPSEnums.TrashID.WargBloodhound, TrashWargBloodhound },
            { ArcDPSEnums.TrashID.CrimsonMcLeod, TrashCrimsonMcLeod },
            { ArcDPSEnums.TrashID.RadiantMcLeod, TrashRadiantMcLeod },
            { ArcDPSEnums.TrashID.MushroomCharger, TrashMushroomCharger },
            { ArcDPSEnums.TrashID.MushroomKing, TrashMushroomKing },
            { ArcDPSEnums.TrashID.MushroomSpikeThrower, TrashMushroomSpikeThrower },
            { ArcDPSEnums.TrashID.WhiteMantleBattleCleric1, TrashWhiteMantleCleric },
            { ArcDPSEnums.TrashID.WhiteMantleBattleCleric2, TrashWhiteMantleCleric },
            { ArcDPSEnums.TrashID.WhiteMantleBattleKnight1, TrashWhiteMantleKnight },
            { ArcDPSEnums.TrashID.WhiteMantleBattleKnight2, TrashWhiteMantleKnight },
            { ArcDPSEnums.TrashID.WhiteMantleBattleMage1, TrashWhiteMantleMage },
            { ArcDPSEnums.TrashID.WhiteMantleBattleMage2, TrashWhiteMantleMage },
            { ArcDPSEnums.TrashID.WhiteMantleBattleMage1Escort, TrashWhiteMantleMage },
            { ArcDPSEnums.TrashID.WhiteMantleBattleMage2Escort, TrashWhiteMantleMage },
            { ArcDPSEnums.TrashID.WhiteMantleBattleSeeker1, TrashWhiteMantleSeeker },
            { ArcDPSEnums.TrashID.WhiteMantleBattleSeeker2, TrashWhiteMantleSeeker },
            { ArcDPSEnums.TrashID.WhiteMantleBattleCultist1, GenericEnemyIcon },
            { ArcDPSEnums.TrashID.WhiteMantleBattleCultist2, GenericEnemyIcon },
            { ArcDPSEnums.TrashID.DhuumDesmina, TrashDhuumDesmina },
            { ArcDPSEnums.TrashID.Glenna, TrashGlenna },
            { ArcDPSEnums.TrashID.VoidStormseer, TrashVoidStormseer },
            { ArcDPSEnums.TrashID.VoidStormseer2, TrashVoidStormseer },
            { ArcDPSEnums.TrashID.VoidStormseer3, TrashVoidStormseer },
            { ArcDPSEnums.TrashID.VoidWarforged1, TrashVoidWarforged },
            { ArcDPSEnums.TrashID.VoidWarforged2, TrashVoidWarforged },
            { ArcDPSEnums.TrashID.VoidRotswarmer, TrashVoidRotswarmer },
            { ArcDPSEnums.TrashID.VoidMelter, TrashVoidMelter },
            { ArcDPSEnums.TrashID.VoidMelter1, TrashVoidMelter },
            { ArcDPSEnums.TrashID.VoidMelter2, TrashVoidMelter },
            { ArcDPSEnums.TrashID.VoidGiant, TrashVoidGiant },
            { ArcDPSEnums.TrashID.VoidGiant2, TrashVoidGiant },
            { ArcDPSEnums.TrashID.ZhaitansReach, TrashZhaitansReach },
            { ArcDPSEnums.TrashID.VoidAbomination, TrashVoidAbomination },
            { ArcDPSEnums.TrashID.VoidAbomination2, TrashVoidAbomination },
            { ArcDPSEnums.TrashID.VoidColdsteel, TrashVoidColdsteel },
            { ArcDPSEnums.TrashID.VoidColdsteel2, TrashVoidColdsteel },
            { ArcDPSEnums.TrashID.VoidColdsteel3, TrashVoidColdsteel },
            { ArcDPSEnums.TrashID.VoidTangler, TrashVoidTangler },
            { ArcDPSEnums.TrashID.VoidTangler2, TrashVoidTangler },
            { ArcDPSEnums.TrashID.VoidObliterator, TrashVoidObliterator },
            { ArcDPSEnums.TrashID.VoidObliterator2, TrashVoidObliterator },
            { ArcDPSEnums.TrashID.VoidGoliath, TrashVoidGoliath },
            { ArcDPSEnums.TrashID.VoidBrandbomber, TrashVoidBrandbomber },
            { ArcDPSEnums.TrashID.VoidSkullpiercer, TrashVoidSkullpiercer },
            { ArcDPSEnums.TrashID.VoidTimeCaster, TrashVoidTimeCaster },
            { ArcDPSEnums.TrashID.VoidTimeCaster2, TrashVoidTimeCaster },
            { ArcDPSEnums.TrashID.VoidThornheart1, TrashVoidThornheart },
            { ArcDPSEnums.TrashID.VoidThornheart2, TrashVoidThornheart },
            { ArcDPSEnums.TrashID.VoidBrandfang1, TrashVoidBrandfang },
            { ArcDPSEnums.TrashID.VoidBrandfang2, TrashVoidBrandfang },
            { ArcDPSEnums.TrashID.VoidBrandbeast, TrashVoidBrandbeast },
            { ArcDPSEnums.TrashID.VoidBrandscale1, TrashVoidBrandscale },
            { ArcDPSEnums.TrashID.VoidBrandscale2, TrashVoidBrandscale },
            { ArcDPSEnums.TrashID.VoidFrostwing, TrashVoidFrostwing },
            //{ ArcDPSEnums.TrashID.CastleFountain, TrashCastleFountain },
            { ArcDPSEnums.TrashID.HauntingStatue, TrashHauntingStatue },
            { ArcDPSEnums.TrashID.GreenKnight, TrashRGBKnight },
            { ArcDPSEnums.TrashID.RedKnight, TrashRGBKnight },
            { ArcDPSEnums.TrashID.BlueKnight, TrashRGBKnight },
            { ArcDPSEnums.TrashID.CloneArtsariiv, TrashCloneArtsariiv },
            { ArcDPSEnums.TrashID.MaiTrinStrikeDuringEcho, TrashMaiTrinStrikeDuringEcho },
            { ArcDPSEnums.TrashID.SooWonTail, TrashSooWonTail },
            { ArcDPSEnums.TrashID.TheEnforcer, TrashTheEnforcer },
            { ArcDPSEnums.TrashID.TheEnforcerCM, TrashTheEnforcer },
            { ArcDPSEnums.TrashID.TheMechRider, TrashTheMechRider },
            { ArcDPSEnums.TrashID.TheMechRiderCM, TrashTheMechRider },
            { ArcDPSEnums.TrashID.TheMindblade, TrashTheMindblade },
            { ArcDPSEnums.TrashID.TheMindbladeCM, TrashTheMindblade },
            { ArcDPSEnums.TrashID.TheRitualist, TrashTheRitualist },
            { ArcDPSEnums.TrashID.TheRitualistCM, TrashTheRitualist },
            { ArcDPSEnums.TrashID.TheSniper, TrashTheSniper },
            { ArcDPSEnums.TrashID.TheSniperCM, TrashTheSniper },
            { ArcDPSEnums.TrashID.PushableVoidAmalgamate, TrashVoidAmalgamate },
            { ArcDPSEnums.TrashID.KillableVoidAmalgamate, TrashVoidAmalgamate },
            { ArcDPSEnums.TrashID.FluxAnomaly1, TrashFluxAnomaly },
            { ArcDPSEnums.TrashID.FluxAnomaly2, TrashFluxAnomaly },
            { ArcDPSEnums.TrashID.FluxAnomaly3, TrashFluxAnomaly },
            { ArcDPSEnums.TrashID.FluxAnomaly4, TrashFluxAnomaly },
            { ArcDPSEnums.TrashID.FluxAnomalyCM1, TrashFluxAnomaly },
            { ArcDPSEnums.TrashID.FluxAnomalyCM2, TrashFluxAnomaly },
            { ArcDPSEnums.TrashID.FluxAnomalyCM3, TrashFluxAnomaly },
            { ArcDPSEnums.TrashID.FluxAnomalyCM4, TrashFluxAnomaly },
            { ArcDPSEnums.TrashID.Torch, TrashTorch },
            { ArcDPSEnums.TrashID.AberrantWisp, TrashAberrantWisp },
            { ArcDPSEnums.TrashID.BoundIcebroodElemental, TrashBoundIcebroodElemental },
            { ArcDPSEnums.TrashID.IcebroodElemental, TrashIcebroodElemental },
            { ArcDPSEnums.TrashID.FractalAvenger, TrashFractalAvenger },
            { ArcDPSEnums.TrashID.FractalVindicator, TrashFractalVindicator },
            { ArcDPSEnums.TrashID.AspectOfDeath, TrashAspects },
            { ArcDPSEnums.TrashID.AspectOfExposure, TrashAspects },
            { ArcDPSEnums.TrashID.AspectOfLethargy, TrashAspects },
            { ArcDPSEnums.TrashID.AspectOfTorment, TrashAspects },
            { ArcDPSEnums.TrashID.AspectOfFear, TrashAspects },
            { ArcDPSEnums.TrashID.ChampionRabbit, TrashChampionRabbit },
            { ArcDPSEnums.TrashID.TheMossman, TrashTheMossman },
            { ArcDPSEnums.TrashID.InspectorEllenKiel, TrashInspectorEllenKiel },
            { ArcDPSEnums.TrashID.JadeMawTentacle, TrashJadeMawTentacle },
            { ArcDPSEnums.TrashID.AwakenedAbomination, TrashAwakenedAbomination },
            { ArcDPSEnums.TrashID.EmbodimentOfDespair, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.EmbodimentOfEnvy, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.EmbodimentOfGluttony, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.EmbodimentOfMalice, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.EmbodimentOfRage, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.EmbodimentOfRegret, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.TheTormented1, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.TheTormented2, TrashGenericRedEnemySkull },
            { ArcDPSEnums.TrashID.TheTormented3, TrashGenericRedEnemySkull },
        };

        /// <summary>
        /// Dictionary matching a <see cref="ArcDPSEnums.MinionID"/> to their icon.
        /// </summary>
        internal static IReadOnlyDictionary<ArcDPSEnums.MinionID, string> MinionNPCIcons { get; private set; } = new Dictionary<ArcDPSEnums.MinionID, string>()
        {
            { ArcDPSEnums.MinionID.HoundOfBalthazar, MinionHoundOfBalthazar },
            { ArcDPSEnums.MinionID.SnowWurm, MinionCallWurm },
            { ArcDPSEnums.MinionID.DruidSpirit, MinionDruidSpirit },
            { ArcDPSEnums.MinionID.SylvanHound, MinionSylvanHound },
            { ArcDPSEnums.MinionID.IronLegionSoldier, MinionWarband },
            { ArcDPSEnums.MinionID.IronLegionMarksman, MinionWarband },
            { ArcDPSEnums.MinionID.BloodLegionSoldier, MinionWarband },
            { ArcDPSEnums.MinionID.BloodLegionMarksman, MinionWarband },
            { ArcDPSEnums.MinionID.AshLegionSoldier, MinionWarband },
            { ArcDPSEnums.MinionID.AshLegionMarksman, MinionWarband },
            { ArcDPSEnums.MinionID.STAD007, MinionDSeries },
            { ArcDPSEnums.MinionID.STA7012, Minion7Series },
            { ArcDPSEnums.MinionID.MistfireWolf, MinionMistfireWolf },
            { ArcDPSEnums.MinionID.RuneJaggedHorror, MinionRuneJaggedHorror },
            { ArcDPSEnums.MinionID.RuneRockDog, MinionRuneRockDog },
            { ArcDPSEnums.MinionID.RuneMarkIGolem, MinionRuneMarkIGolem },
            { ArcDPSEnums.MinionID.RuneTropicalBird, MinionTropicalBird },
            { ArcDPSEnums.MinionID.Ember, MinionEmber },
            { ArcDPSEnums.MinionID.HawkeyeGriffon, MinionHawkeyeGriffon },
            { ArcDPSEnums.MinionID.SousChef, MinionSousChef },
            { ArcDPSEnums.MinionID.SunspearParagonSupport, MinionSunspreadParagon },
            { ArcDPSEnums.MinionID.RavenSpiritShadow, MinionRavenSpiritShadow },
            { ArcDPSEnums.MinionID.CloneSpear, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneGreatsword, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneStaff, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneTrident, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneSword, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneSwordPistol, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneSwordTorch, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneSwordFocus, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneSwordSword, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneSwordShield, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneIllusionaryLeap, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneIllusionaryLeapFocus, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneIllusionaryLeapShield, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneIllusionaryLeapSword, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneIllusionaryLeapPistol, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneIllusionaryLeapTorch, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneScepter, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneScepterTorch, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneScepterShield, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneScepterPistol, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneScepterFocus, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneScepterSword, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneAxe, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneAxeTorch, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneAxePistol, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneAxeSword, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneAxeFocus, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneAxeShield, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneDagger, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneDaggerFocus, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneDaggerPistol, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneDaggerShield, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneDaggerSword, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneDaggerTorch, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneDownstate, MinionMesmerClone },
            { ArcDPSEnums.MinionID.CloneUnknown, MinionMesmerClone },
            { ArcDPSEnums.MinionID.IllusionarySwordsman, MinionIllusionarySwordsman },
            { ArcDPSEnums.MinionID.IllusionaryBerserker, MinionIllusionaryBerserker },
            { ArcDPSEnums.MinionID.IllusionaryDisenchanter, MinionIllusionaryDisenchanter },
            { ArcDPSEnums.MinionID.IllusionaryRogue, MinionIllusionaryRogue },
            { ArcDPSEnums.MinionID.IllusionaryDefender, MinionIllusionaryDefender },
            { ArcDPSEnums.MinionID.IllusionaryMage, MinionIllusionaryMage },
            { ArcDPSEnums.MinionID.IllusionaryDuelist, MinionIllusionaryDuelist },
            { ArcDPSEnums.MinionID.IllusionaryWarden, MinionIllusionaryWarden },
            { ArcDPSEnums.MinionID.IllusionaryWarlock, MinionIllusionaryWarlock },
            { ArcDPSEnums.MinionID.IllusionaryAvenger, MinionIllusionaryAvenger },
            { ArcDPSEnums.MinionID.IllusionaryWhaler, MinionIllusionaryWhaler },
            { ArcDPSEnums.MinionID.IllusionaryMariner, MinionIllusionaryMariner },
            { ArcDPSEnums.MinionID.JadeMech, MinionJadeMech },
            { ArcDPSEnums.MinionID.EraBreakrazor, MinionEraBreakrazor },
            { ArcDPSEnums.MinionID.KusDarkrazor, MinionKusDarkrazor },
            { ArcDPSEnums.MinionID.ViskIcerazor, MinionViskIcerazor },
            { ArcDPSEnums.MinionID.JasRazorclaw, MinionJasRazorclaw },
            { ArcDPSEnums.MinionID.OfelaSoulcleave, MinionOfelaSoulcleave },
            { ArcDPSEnums.MinionID.VentariTablet, MinionVentariTablet },
            { ArcDPSEnums.MinionID.FrostSpirit, MinionFrostSpirit },
            { ArcDPSEnums.MinionID.SunSpirit, MinionSunSpirit },
            { ArcDPSEnums.MinionID.StoneSpirit, MinionStoneSpirit },
            { ArcDPSEnums.MinionID.StormSpirit, MinionStormSpirit },
            { ArcDPSEnums.MinionID.WaterSpirit, MinionWaterSpirit },
            { ArcDPSEnums.MinionID.SpiritOfNatureRenewal, MinionSpiritOfNatureRenewal },
            { ArcDPSEnums.MinionID.JuvenileAlpineWolf, MinionJuvenileAlpineWolf },
            { ArcDPSEnums.MinionID.JuvenileArctodus, MinionJuvenileArctodus },
            { ArcDPSEnums.MinionID.JuvenileArmorFish, MinionJuvenileArmorFish },
            { ArcDPSEnums.MinionID.JuvenileBlackBear, MinionJuvenileBlackBear },
            { ArcDPSEnums.MinionID.JuvenileBlackMoa, MinionJuvenileBlackMoa },
            { ArcDPSEnums.MinionID.JuvenileBlackWidowSpider, MinionJuvenileBlackWidowSpider },
            { ArcDPSEnums.MinionID.JuvenileBlueJellyfish, MinionJuvenileBlueRainbowJellyfish },
            { ArcDPSEnums.MinionID.JuvenileBlueMoa, MinionJuvenileBlueMoa },
            { ArcDPSEnums.MinionID.JuvenileBoar, MinionJuvenileBoar },
            { ArcDPSEnums.MinionID.JuvenileBristleback, MinionJuvenileBristleback },
            { ArcDPSEnums.MinionID.JuvenileBrownBear, MinionJuvenileBrownBear },
            { ArcDPSEnums.MinionID.JuvenileCarrionDevourer, MinionJuvenileCarrionDevourer },
            { ArcDPSEnums.MinionID.JuvenileCaveSpider, MinionJuvenileCaveSpider },
            { ArcDPSEnums.MinionID.JuvenileCheetah, MinionJuvenileCheetah },
            { ArcDPSEnums.MinionID.JuvenileEagle, MinionJuvenileEagle },
            { ArcDPSEnums.MinionID.JuvenileEletricWywern, MinionJuvenileEletricWywern },
            { ArcDPSEnums.MinionID.JuvenileFangedIboga, MinionJuvenileFangedIboga },
            { ArcDPSEnums.MinionID.JuvenileFernHound, MinionJuvenileFernHound },
            { ArcDPSEnums.MinionID.JuvenileFireWywern, MinionJuvenileFireWywern },
            { ArcDPSEnums.MinionID.JuvenileForestSpider, MinionJuvenileForestSpider },
            { ArcDPSEnums.MinionID.JuvenileHawk, MinionJuvenileHawk },
            { ArcDPSEnums.MinionID.JuvenileIceDrake, MinionJuvenileIceDrake },
            { ArcDPSEnums.MinionID.JuvenileJacaranda, MinionJuvenileJacaranda },
            { ArcDPSEnums.MinionID.JuvenileJaguar, MinionJuvenileJaguar },
            { ArcDPSEnums.MinionID.JuvenileJungleSpider, MinionJuvenileJungleSpider },
            { ArcDPSEnums.MinionID.JuvenileJungleStalker, MinionJuvenileJungleStalker },
            { ArcDPSEnums.MinionID.JuvenileKrytanDrakehound, MinionJuvenileKrytanDrakehound },
            { ArcDPSEnums.MinionID.JuvenileLashtailDevourer, MinionJuvenileLashtailDevourer },
            { ArcDPSEnums.MinionID.JuvenileLynx, MinionJuvenileLynx },
            { ArcDPSEnums.MinionID.JuvenileMarshDrake, MinionJuvenileMarshDrake },
            { ArcDPSEnums.MinionID.JuvenileMurellow, MinionJuvenileMurellow },
            { ArcDPSEnums.MinionID.JuvenileOwl, MinionJuvenileOwl },
            { ArcDPSEnums.MinionID.JuvenilePhoenix, MinionJuvenilePhoenix },
            { ArcDPSEnums.MinionID.JuvenilePig, MinionJuvenilePig },
            { ArcDPSEnums.MinionID.JuvenilePinkMoa, MinionJuvenilePinkMoa },
            { ArcDPSEnums.MinionID.JuvenilePolarBear, MinionJuvenilePolarBear },
            { ArcDPSEnums.MinionID.JuvenileRainbowJellyfish, MinionJuvenileBlueRainbowJellyfish },
            { ArcDPSEnums.MinionID.JuvenileRaven, MinionJuvenileRaven },
            { ArcDPSEnums.MinionID.JuvenileRedJellyfish, MinionJuvenileRedJellyfish },
            { ArcDPSEnums.MinionID.JuvenileRedMoa, MinionJuvenileRedMoa },
            { ArcDPSEnums.MinionID.JuvenileReefDrake, MinionJuvenileReefDrake },
            { ArcDPSEnums.MinionID.JuvenileRiverDrake, MinionJuvenileRiverDrake },
            { ArcDPSEnums.MinionID.JuvenileRockGazelle, MinionJuvenileRockGazelle },
            { ArcDPSEnums.MinionID.JuvenileSalamanderDrake, MinionJuvenileSalamanderDraker },
            { ArcDPSEnums.MinionID.JuvenileSandLion, MinionJuvenileSandLion },
            { ArcDPSEnums.MinionID.JuvenileShark, MinionJuvenileShark },
            { ArcDPSEnums.MinionID.JuvenileSiamoth, MinionJuvenileSiamoth },
            { ArcDPSEnums.MinionID.JuvenileSiegeTurtle, MinionJuvenileSiegeTurtle },
            { ArcDPSEnums.MinionID.JuvenileSmokescale, MinionJuvenileSmokescale },
            { ArcDPSEnums.MinionID.JuvenileSnowLeopard, MinionJuvenileSnowLeopard },
            { ArcDPSEnums.MinionID.JuvenileTiger, MinionJuvenileTiger },
            { ArcDPSEnums.MinionID.JuvenileWallow, MinionJuvenileWallow },
            { ArcDPSEnums.MinionID.JuvenileWarthog, MinionJuvenileWarthog },
            { ArcDPSEnums.MinionID.JuvenileWhiptailDevourer, MinionJuvenileWhiptailDevourer },
            { ArcDPSEnums.MinionID.JuvenileWhiteMoa, MinionJuvenileWhiteMoa },
            { ArcDPSEnums.MinionID.JuvenileWhiteRaven, MinionJuvenileWhiteRaven },
            { ArcDPSEnums.MinionID.JuvenileWhiteTiger, MinionJuvenileWhiteTiger },
            { ArcDPSEnums.MinionID.JuvenileWolf, MinionJuvenileWolf },
            { ArcDPSEnums.MinionID.JuvenileHyena, MinionJuvenileHyena },
            { ArcDPSEnums.MinionID.JuvenileAetherHunter, MinionJuvenileAetherHunter },
            { ArcDPSEnums.MinionID.BloodFiend, MinionBloodFiend },
            { ArcDPSEnums.MinionID.BoneFiend, MinionBoneFiend },
            { ArcDPSEnums.MinionID.FleshGolem, MinionFleshGolem },
            { ArcDPSEnums.MinionID.ShadowFiend, MinionShadowFiend },
            { ArcDPSEnums.MinionID.FleshWurm, MinionFleshWurm },
            { ArcDPSEnums.MinionID.UnstableHorror, MinionUnstableHorror },
            { ArcDPSEnums.MinionID.ShamblingHorror, MinionShamblingHorror },
            { ArcDPSEnums.MinionID.Thief1, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief2, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief3, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief4, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief5, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief6, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief7, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief8, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief9, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief10, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief11, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief12, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief13, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief14, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief15, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief16, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief17, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief18, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief19, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief20, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief21, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Thief22, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Daredevil1, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Daredevil2, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Daredevil3, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Daredevil4, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Daredevil5, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Daredevil6, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Daredevil7, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Daredevil8, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Daredevil9, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Daredevil10, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Deadeye1, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Deadeye2, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Deadeye3, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Deadeye4, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Deadeye5, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Deadeye6, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Deadeye7, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Deadeye8, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Deadeye9, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Deadeye10, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Specter1, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Specter2, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Specter3, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Specter4, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Specter5, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Specter6, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Specter7, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Specter8, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Specter9, MinionThievesGuild },
            { ArcDPSEnums.MinionID.Specter10, MinionThievesGuild },
            { ArcDPSEnums.MinionID.BowOfTruth, MinionBowOfTruth },
            { ArcDPSEnums.MinionID.HammerOfWisdom, MinionHammerOfWisdom },
            { ArcDPSEnums.MinionID.ShieldOfTheAvenger, MinionShieldOfTheAvenger },
            { ArcDPSEnums.MinionID.SwordOfJustice, MinionSwordOfJustice },
            { ArcDPSEnums.MinionID.LesserAirElemental, MinionLesserAirElemental },
            { ArcDPSEnums.MinionID.LesserEarthElemental, MinionLesserEarthElemental },
            { ArcDPSEnums.MinionID.LesserFireElemental, MinionLesserFireElemental },
            { ArcDPSEnums.MinionID.LesserIceElemental, MinionLesserIceElemental },
            { ArcDPSEnums.MinionID.AirElemental, MinionAirElemental },
            { ArcDPSEnums.MinionID.EarthElemental, MinionEarthElemental },
            { ArcDPSEnums.MinionID.FireElemental, MinionFireElemental },
            { ArcDPSEnums.MinionID.IceElemental, MinionIceElemental },
            { ArcDPSEnums.MinionID.BlastGyro, MinionBlastGyro },
            { ArcDPSEnums.MinionID.BulwarkGyro, MinionBulwarkGyro },
            { ArcDPSEnums.MinionID.FunctionGyro, MinionFunctionGyro },
            { ArcDPSEnums.MinionID.MedicGyro, MinionMedicGyro },
            { ArcDPSEnums.MinionID.PurgeGyro, MinionPurgeGyro },
            { ArcDPSEnums.MinionID.ShredderGyro, MinionShredderGyro },
            { ArcDPSEnums.MinionID.SneakGyro, MinionSneakGyro },
        };
    }
}
