using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser
{
    public static class EffectGUIDs
    {
        // Generic
        // blue circles indicating radius of boons etc.
        public const string Generic240UnitRadius = "E7C50E0E148CBE44BB2770AF2D6750A4"; // e.g. speed of synergy, bypass coating
        public const string Generic360UnitRadius = "10873BDE22D87845AAF004B0A60FA546"; // e.g. crisis zone
        public const string Generic360UnitRadius2 = "0B3A5E8DDBB43447815547D96E7CA146"; // e.g. over shield, deathly haste, mechanical genius, barrier burst
        public const string Generic360Or600UnitRadius = "4C7A5E148F7FD642B34EE4996DDCBBAB"; // somehow both? e.g. chaos vortex, medical dispersion field, reconstruction enclosure, barrier engine
        public const string Generic600UnitRadius = "9C7C1B2379CCDD4990001A38030E4495"; // e.g. ranger spirits, protect me
        public const string Generic900UnitRadius = "EB9EBC2CB610B448BB00B7FBCB191F28"; // e.g. call of the wild
        public const string RuneOfNightmare = "149E616EB45B1E4982305B99A7952EA8";
        public const string StealthApply = "B44BAD999BEB2D4DB284745895B42BDD";
        public const string StealthReveal = "A37F8E2B550B254DA89F933BDF654B41"; // also used with e.g. infiltrators strike, infiltrators arrow, shadowstep, shadow return, infiltrators signet
        public const string WhiteMantlePortalInactive = "D43373FEFA19A54DA2A2B6BB7834A338";
        public const string WhiteMantlePortalActive = "388CF9246218A34DB2F8107E19FCA471";
        // Mesmer
        public const string MesmerThePrestigeDisappear1 = "48B69FBC3090E144BFC067D6C0208878";
        public const string MesmerThePrestigeDisappear2 = "5FA6527231BB8041AC783396142C6200"; // also used with elementalist cleansing fire
        public const string MesmerSignetOfMidnight = "02154B72900B5740A73CD0ADECED27BF";
        public const string MesmerFeedback = "D6C8F406E4DEE04AB16A215BE068E910";
        public const string MesmerTeleport = "C34E250B01FF534292EE6AB36D768337"; // used by blink, phase retreat, swap (illusionary leap)
        public const string MesmerPortalInactive = "F3CD4D9BFC8EAD45AAA1EA7A3AB148BF";
        public const string MesmerPortalActive = "3C346BE32EFB9E40BE39E379B061C803";
        public const string MesmerCryOfFrustration = "52F65A4D9970954BA849CB57A46A65A8";
        public const string MesmerDiversion = "916D8385083F144EBAA5BEEDE21FD47A";
        public const string MesmerDistortionOrMindWrack = "3D29ABD39CB5BD458C4D50A22FCC0E4B";
        public const string MesmerMantraOfResolveAndPowerCleanse = "593E668A006AB24D84999AED68F2E4C4";
        public const string MesmerMantraOfConcentrationAndPowerBreak = "5B488D552E316045AD99C4A98EEDDB1E";
        public const string MesmerPowerReturn = "F53E2CE3B06B934085D46FA59468477B";
        public const string ChronomancerSeizeTheMomentShatter = "4C7A5E148F7FD642B34EE4996DDCBBAB"; // This seems to happen everytime split second, rewinder, time sink or continuum split are cast under SeizeTheMoment
        public const string ChronomancerSplitSecond = "C035166E3E4C414ABE640F47797D9B4A"; // this is also triggered by the clones while being sourced to the chrono
        public const string ChronomancerRewinder = "DC1C8A043ADCD24B9458688A792B04BA";
        public const string ChronomancerTimeSink = "AB2E22E7EE74DA4C87DA777C62E475EA";
        public const string MirageCloak = "4C7A5E148F7FD642B34EE4996DDCBBAB";
        public const string MirageMirror = "1370CDF5F2061445A656A1D77C37A55C";
        public const string MirageJaunt = "3A5A38C26A1FFB438EAD734F3ED42E5E"; // may have collisions! not known which
        public const string VirtuosoBladeturnRequiem = "87B761200637AC48B71469F553BA6F60";
        public const string VirtuosoThousandCuts = "E4002B7AD7DF024394D0184B47A316E7";
        // public const string MirageJauntConflict1 = "B6557C336041B24FA7CC198B6EBDAD9A"; // used with e.g. jaunt & axes of symmetry

        // public const string MirageJauntConflict2 = "D7A05478BA0E164396EB90C037DCCF42"; // used with e.g. jaunt, axes of symmetry, illusionary ambush
        // public const string MesmerTrail = "73414BA39AFCF540A90CF91DE961CCEF"; // used with e.g. mirror images, phase retreat, illusionary ambush - likely the "trail" left behind
        // Necromancer
        public const string NecromancerNecroticTraversal = "47C48881C5AC214388F6253197A7F11A";
        public const string NecromancerUnholyBurst = "C4E8DD3234E0C647993857940ED79AC1"; // also used for spiteful spirit
        public const string NecromancerPlagueSignet = "E78ED095E97F1D4A8BEB901796449E2F"; // might be pov only?
        public const string ReaperSuffer = "6C8C388BCD26F04CA6618D2916B8D796";
        public const string ReaperYouAreAllWeaklings1 = "37242DF51D238A409E822E7A1936D7A6"; // 3 potential candidates, 4th effect has collisions
        public const string ReaperYouAreAllWeaklings2 = "FEE4F26C2866E34C9D75506A8ED94F5E";
        public const string ReaperYouAreAllWeaklings3 = "ED6A8440CB49B248A352B2073FAF1F5F";
        public const string ScourgeTrailOfAnguish = "1DAE3CAEF2228845867AAF419BF31E8C";
        public const string ScourgeShade = "78408C6DA08C2746BEABEB995187271A";
        public const string ScourgeShadeStrike = "C8B109540159AA429E83D0AA98EF3E90";
        public const string ScourgeSandSwellPortal = "086CF7823EB13047B2187E7933639703";
        public const string HarbingerCascadingCorruption = "EEDCAB61CD35E840909B03D398878B1C";
        public const string HarbingerDeathlyHaste = "9C06D9D9B0E22247A1752C426808CD80";
        public const string HarbingerDoomApproaches = "88C0010F0B7148469B88E2A1B4500DCC";
        // Elementalist
        public const string ElementalistArmorOfEarth1 = "D43DC34DEF81B746BC130F7A0393AAC7";
        public const string ElementalistArmorOfEarth2 = "D0C072102FAA6A4EA8A16CB73F3B96DD"; // happens at the same time as the other, could be relevant to check should collisions appear
        //public const string ElementalistCleansingFire = "5FA6527231BB8041AC783396142C6200"; // also used with mesmer the prestige, collides with some air traits
        public const string ElementalistSignetOfAir = "30A96C0E559DBD489FEE36DA96CC374A";
        //public const string ElementalistLightningFlash = "40818C8E9CC6EF4388C2821FCC26A9EC"; // Conflicts with certain field combos, thief teleport skills, guardian judges/merciful intervention
        public const string TempestFeelTheBurn = "C668B5DB6220D9448817B3E5F7DE6E46";
        public const string TempestEyeOfTheStorm1 = "52FEF389CF7D014BAA375EACF1826BB6";
        public const string TempestEyeOfTheStorm2 = "31FE88E9CCF82047895FD0EF19C9BBA0"; // happens at the same time as the other, could be relevant to check should collisions appear 
        public const string CatalystDeployFireJadeSphere = "AFC5D5C7DA63D64BAAD55F787205B64F";
        public const string CatalystDeployWaterJadeSphere = "6D7EB5747873484DAF29C01FA51FE175";
        public const string CatalystDeployAirJadeSphere = "A3C8A55C3E530140A7F99AAA1CBB4E09";
        public const string CatalystDeployEarthJadeSphere = "A674D3E7BC0C4342BC7A4EF0EE8FF8F0";
        // Warrior
        public const string WarriorSignetOfMight = "75EF160EAFC0394CACC436CF89819148";
        public const string WarriorSignetOfStamina = "1E720C4D42448D45BDCB6307869D3D66"; // not actually instant cast, just for reference
        public const string WarriorDolyakSignet = "D7F8FA5695F8714B99A51EE72EF6E178";
        // Revenant
        public const string RevenantTabletAutoHeal = "C715D15450E56E4998F9EB90B91C5668";
        public const string RevenantTabletVentarisWill = "D3FD740370D6B747B2DA4F8F065A0177";
        public const string RevenantProtectiveSolace = "63683ECFD27DA746BF0B16404D817978";
        public const string RevenantNaturalHarmony = "390487E4E5DFEA4C922AE3156A86D9DB";
        public const string RevenantNaturalHarmonyEnergyRelease = "E239BA17214B4943A4EC2D6B43F6175F";
        public const string RevenantPurifyingEssence = "D2B388E8DB721544A110979C3A384977";
        public const string RevenantEnergyExpulsion = "BE191381B1BC984A989D94D215DDEA1F";
        public const string RenegadeOrdersFromAbove = "F53F05F041957A47AD62B522FE030408";
        // Guardian
        public const string GuardianGenericFlames = "EA98C3533AA46E4A9B550929356B7277"; // used e.g. with judges intervention, signet of judgment
        public const string GuardianGenericTeleport = "61C193EBA6526143BE01B80FF7C52217"; // usd e.g. with judges intervention, merciful intervention
        public const string GuardianGenericTeleport2 = "5E1717FB11CE1D44B59B36B6AD83B9CC"; // delayed, when reaching target? used with e.g. judges intervention, symbol of blades
        public const string GuardianWallOfReflection = "70FABE08FFCFEE48A7160A4D479E3F8B";
        public const string GuardianShout = "122BA55CCDF2B643929F6C4A97226DC9"; // used with all shouts
        public const string GuardianSaveYourselves = "68F2C378E6C80548B5A3C89870C5DD86";
        public const string GuardianSmiteCondition = "8CBE6348BB8C9646B210AEE4BA9BCCA3"; // also lesser smite condition
        public const string GuardianContemplationOfPurity1 = "75D72E2DA47ECF47A6BD009B49B7C708";
        public const string GuardianContemplationOfPurity2 = "D0C072102FAA6A4EA8A16CB73F3B96DD"; // same as elementalist armor of earth
        public const string GuardianMercifulIntervention = "B45E7BD66E424A4CA695DE63DC13E93F"; // delayed, when reaching target?
        public const string GuardianSignetOfJudgement1 = "0AFA3936BD4D70458925660B54D47A90"; // happens twice?
        public const string GuardianSignetOfJudgement2 = "5EAC13DB0953EF4C9C5BCC10DB13C9C8";
        public const string FirebrandMantraOfLiberationCone = "86CC98C9D9D2B64689F8993AB02B09E5";
        public const string FirebrandMantraOfLiberationSymbol = "A8E0E4C48848424D85503B674015D247";
        public const string FirebrandMantraOfLoreCone = "C2B55AE44B295849A2983745203D19A1";
        public const string FirebrandMantraOfLoreSymbol = "3D01B04C5700904BA279E9F135A3FAB3";
        public const string FirebrandMantraOfPotenceCone = "FB70E37EB3915F4BAB6E06E328832D1D";
        public const string FirebrandMantraOfPotenceSymbol = "95B52793B838524AB237EB9FED7834BF";
        public const string FirebrandMantraOfSolaceCone = "D2C28FC5AB651746914FC595D1591623";
        public const string FirebrandMantraOfSolaceSymbol = "8F0C77784AFD7F40B27446617DC05CDC";
        public const string FirebrandMantraOfTruthCone = "C2F283E74AC9024DBB865BA0F98AF20B";
        public const string FirebrandMantraOfTruthSymbol = "E33EA0A63898CA469F864EDA1336FCD0";
        public const string FirebrandMantraOfFlameCone = "9C2F9434C5827943A7F175EFF245D39F";
        public const string FirebrandMantraOfFlameSymbol = "AF2B09AC1145AA4880B967C32A11E81C";
        public const string FirebrandTomeOfJusticeOpen = "D573910FDB59434ABF6E7433061995BD";
        public const string FirebrandTomeOfResolveOpen = "39C1BD24ADA04C4788A99C7B0FD9B53F";
        public const string FirebrandTomeOfCourageOpen = "9EE3EAFEF333BE44AD8A7D234A1C3899";
        public const string DragonhunterTrapEffect = "CCF55B3EAA4D514BBB8340E01B6A1DEC";
        public const string DragonhunterTestOfFaith = "D7006AC247BBE74BA54E912188EF6B12";
        public const string DragonhunterFragmentsOfFaith = "C84644DDAA59E542989FDB98CD69134C";
        // Engineer
        public const string EngineerHealingMist = "B02D3D0FF0A4FC47B23B1478D8E770AE"; // used with healing mist, soothing detonation
        public const string EngineerMagneticInversion = "F8BD502E5B0D9444AA6DC5B5918801EE";
        public const string ScrapperBulwarkGyro = "611D90C69ECF8142BEEE84139F333388";
        public const string ScrapperPurgeGyro = "0DBE4F7115EADC4889F1E00232B2398B";
        public const string ScrapperDefenseField = "9E2D190A92E2B5498A88722910A9DECD";
        public const string ScrapperBypassCoating = "D2307A69B227BE4B831C2AA1DAAE646A"; // player is owner
        public const string HolosmithFlashSpark = "418A090D719AB44AAF1C4AD1473068C4";
        public const string MechanistCrashDownImpact = "80E1A21E07C03A43A21E470B95075A5A"; // happens at spawn location, no owner, no target, ~800ms after spawn
        public const string MechanistMechEyeGlow = "CDF749672C01964BAEF64CCB3D431DEE"; // used with e.g. crash down (delayed), crisis zone
        public const string MechanistDischargeArray = "5AAD58AD0259604AADA18AFD3AE0DDFD"; // likely the white radius indicator
        public const string MechanistCrisisZone = "956450E1260FB94B8691BC1378086250";
        public const string MechanistShiftSignet1 = "E1C1DD7F866B4149A1BADD216C9AA69D"; // happens twice, without owner at destination, with owner at origin?
        public const string MechanistShiftSignet2 = "DB22850AE209B34BBD11372F56D42D43";
        public const string MechanistOverclockSignet = "734834E7EB7CD74EB129ACBCE5C64C1D";
        // Ranger
        public const string RangerLightningReflexes = "3CF1D1228CBC3740AA33EDA357EABED4";
        public const string RangerQuickeningZephyr = "B23157C515072E46B5514419B0F923B7";
        public const string RangerSignetOfRenewal = "EA9896A81DDF4843B18DBF6EE4F25E18";
        public const string RangerSignetOfTheHunt = "1A38CAE72C2F164BA3815441CA643A20";
        public const string DruidGlyphOfEquality = "9B8A1BE554450B4899B64F7579DF0A8C";
        public const string DruidGlyphOfEqualityCA = "74870558C43E4747955C573CAAC630A7";
        public const string DruidSeedOfLife = "19C4FA17A38E7E4780722799B48BF2BE"; // has owner
        public const string DruidSeedOfLifeBlossom = "666BCBD61F72E042B08EFE1C62555245"; // has owner, ~720ms delayed
        public const string SoulbeastEternalBond = "BF0A5B11A4076A4F98C6E1D655D507B1"; // has owner & target
        public const string UntamedMutateConditions = "D7DCD4ABF9E4A749950AF0175E02EA06";
        public const string UntamedUnnaturalTraversal = "8D36806A690A5442A983308EDCECB018";
        public const string UntamedVenomousOutburst = "60BE4692A455B140A05AD794BF4753F6";
        public const string UntamedRendingVines = "2C40B0741111444F98895A658A7F978F";
        public const string UntamedEnvelopingHaze = "F2B1B61970FC59418AC049BF3A07FFD4";
        // Thief
        public const string ThiefTeleportTrail = "03A8D8B8F81FE94FB52FFE5F74F31C9E"; // likely the trail, used with infiltrators arrow, shadow step, infiltrators signet, measured shot
        // public const string ThiefTeleport = "1DEF5F2ECCF6CA4683ECC2DAED54726C"; // used with e.g. shadow shot, shadow strike
        public const string ThiefShadowstep = "2C40AE26C91BEE468E245D0009B590F9";
        public const string ThiefInfiltratorsSignet1 = "23284B87C26C9A41A887F410F930E1A2";
        public const string ThiefInfiltratorsSignet2 = "2C89A39F7B88614ABED16D4B5A5BD2EB";
        // public const string ThiefInfiltratorsSignetCollision = "70CFE546FA6A9B4E93BCAAF1ED1CD326"; // collision with shadow shot, shadow strike
        public const string ThiefSignetOfAgility = "BB5488951B60B546BB1BD5626DAE83E1";
        public const string ThiefSignetOfShadows = "14A5982DB277744CB928A4935555F563";
        public const string ThiefPitfallAoE = "7325E9B0DD2E914F9837E5FCFC740A95";
        // public const string ThiefThousandNeedlesAoECollision = "2125A13079C1C5479C150926EB60A15D"; // collision with shadow flare & other
        public const string ThiefThousandNeedlesAoE1 = "9AF103E33FC235498190448A9496C98A"; // ~280ms delayed
        public const string ThiefThousandNeedlesAoE2 = "B8DC8C6736C8E0439295A9DBBADC6296"; // ~280ms delayed
        public const string ThiefSealAreaAoE = "92A7634C2C7F2746AFDA88E1AD9AE886";
        public const string ThiefShadowPortalArmedInactive = "97AF46D347914E4FBDB37BFEC91C4711"; // unarmed portal has no effect, is this pov only?
        public const string ThiefShadowPortalActiveEntrance = "8535B486C1BCD24A87B7AC895FB26BB0";
        public const string ThiefShadowPortalActiveExit = "97AF46D347914E4FBDB37BFEC91C4711";
        public const string DeadeyeMercy = "B59FCEFCF1D5D84B9FDB17F11E9B52E6";
        // Nightmare Fractal
        public const string SmallFluxBomb = "B9CB27D38747A94F817208835C41BB35";
        public const string ToxicSicknessIndicator = "3C98B00B9E795F4B8744E186EEEA7DF7";
        public const string ToxicSicknessPuke1 = "B7DFF8C2A8DABD4C9C7F1D4CFC31FC8C";
        public const string ToxicSicknessPuke2 = "E09CD66E417B59409401192201CE4B6E";
        public const string NightmareMiasmaIndicator = "41883B3BD532124DACF93F7C2584E63C";
        public const string ArkkShieldIndicator = "5B1B9D29D6242F47A82743330AE4225B";
        public const string NightmareHallucinationsSpawn = "0C284B1C201D1846B4D9F249AD01A5C6";
        public const string VileSpitSiax = "BC17A48E8DD2FF44864AA48A732BDC36";
        public const string CausticBarrageIndicator = "C910F1B11A21014AA99F24DBDFBF13FB";
        public const string CausticBarrageHitEffect = "CAF4E62C2C5CC04499657C2A6A78087B";
        public const string VolatileExpulsionIndicator = "F22E201EAF24DD42A43D297B2E83CC66";
        public const string CascadeOfTormentRing0 = "EFF32973C7921F41AA3FD65745E06506";
        public const string CascadeOfTormentRing1 = "D919AC7D1B2ABD438F809B3B9DCE9226";
        public const string CascadeOfTormentRing2 = "A5D958EDAD66D7469CA40059915843CC";
        public const string CascadeOfTormentRing3 = "55FC7E1387EA2241B6538CAAB6017497";
        public const string CascadeOfTormentRing4 = "8CFFD69B25B7E844856A7D06D11332D5";
        public const string CascadeOfTormentRing5 = "D427C86A0E120F4A860F4570B354396D";
        public const string EnsolyssMiasmaDoughnut100_66 = "16B9D11838F68A4C8E477ED62F956226";
        public const string EnsolyssMiasmaDoughnut66_15 = "3AE042F82A10B84DB7487B0C0F4D2AB1";
        public const string EnsolyssMiasmaDoughnut15_0 = "AB294EC140644E48BC739B8E303D2762";
        public const string EnsolyssNightmareAltarShockwave = "AA31A20BDC52324B945FD660D60429EB";
        // Shattered Observatory Fractal
        public const string SolarBolt = "FA097ABEFB8CEF4B89EB12825EEE1FB9";
        public const string KickGroundEffect = "47FE87414A88484AB05A84E1440F5FDD";
        public const string AoeIndicator130Radius = "8DDED161CE26964FA5952D821AD852F7";
        public const string MistBomb = "03FB41386DD2A54FA093795DF2870B7A";
        // Vale Guardian
        public const string ValeGuardianDistributedMagic = "43FD739499BB6040BBF9EEF37781B2CE";
        public const string ValeGuardianMagicSpike = "55364633145D264A934935C3F026B19F";
        // Cairn
        public const string CairnDisplacement = "7798B97ED6B6EB489F7E33DF9FE6BD99";
        public const string CairnDashGreen = "D2E6D55CC94F79418BB907F063CBDD81";
        // CA
        public const string CAArmSmash = "B1AAD873DB07E04E9D69627156CA8918";
        // Boneskinner
        public const string GraspAoeIndicator = "B9B32815D670DC4E8B8CF71E92A9FFD5"; // Orange aoe indicator
        public const string GraspClaws1 = "75B096EF78F3AB4CB1D05BAE9CA3235C"; // One is the claw, the other the red aoe indicator
        public const string GraspClaws2 = "4C290CBF719C0E448391E9415EF307A7";
        public const string CascadeBonesEffect = "3E370A8629BB134F83902A8F14B99CCE";
        public const string CascadeAoEIndicator1 = "4692619BBBFE6346B409C4A2B93B9BA6";
        public const string CascadeAoEIndicator2 = "8E8592D62B48834180C66FE806278C86";
        public const string CascadeAoEIndicator3 = "89CB4BCA7B012244B0864DFAD7E9F3AC";
        public const string CascadeAoEIndicator4 = "965355FD1C53F24085A9C422B8333780";
        public const string CascadeAoEIndicator5 = "F26A2240C0F1E24E81EAEFDE64EFA3BF";
        // Ankka
        public const string DeathsEmbrace = "4AC57C4159E0804D8DBEB6F0F39F5EF3";
        public const string DeathsHandOnPlayerCM = "9A64DC8F21EEC046BA1D4412863F2940";
        public const string DeathsHandByAnkkaCM = "651CA3631083EF4A81159989AB58F787";
        public const string DeathsHandByAnkkaNM = "805E3CE2A313584797C614082C44197D";
        // Harvest Temple
        public const string HarvestTemplePurificationLightnings = "ADDDB6E725094240845270262E59F2BD";
        public const string HarvestTemplePurificationVoidZones = "F5A9E487E2B3A64A83661D87DE1CAF1F";
        public const string HarvestTemplePurificationZones = "D5B07DF36991DD48B64AC403EFAA6F9F";
        public const string HarvestTemplePurificationFireBalls = "D49EB86EB17A0D4793768B19978C1B2C";
        public const string HarvestTemplePurificationBeeLaunch = "73FE43AEE78ADC4B9527DF683481984F";
        public const string HarvestTemplePurificationPoisonTrail = "CCBA0AD77B52774DA48EE37AED9108F4";
        public const string HarvestTemplePurificationWaterProjectiles = "F8F9628F58DA09438574D66424399151";
        public const string HarvestTempleJormagIceShards = "2B9395E6BDE51E4C90AD3A9CA78FBCE7";
        public const string HarvestTempleJormagFrostMeteorIceField = "40C38381C43B184A885960714F9388D5";
        public const string HarvestTemplePrimordusSmallJaw = "EDA1C033B296404BA403E106F6F258C0";
        public const string HarvestTemplePrimordusGeneralJawAttack = "160CBAE34F4A2941885EB3F3CD6BB0C3";
        public const string HarvestTemplePrimordusBigJaw = "4D8CA1836969BD4BBF345719576ACAAF";
        public const string HarvestTempleKralkatorrikBeamIndicator = "4ACBA11BFAC6B940BF6FD11CB332FFB8"; // This is the effect for the AoE indicator, the actual puddles are a different effect
        public const string HarvestTempleKralkatorrikBeamAoe = "8B55EBC6025EB3429D464EDA5710E419"; // This is the effect for the actual circular puddles
        public const string HarvestTempleMordremothPoisonRoarIndicator = "171A7BD24B5D0B4BA3770FF8A6A37EC0";
        public const string HarvestTempleMordremothPoisonRoarImpact = "E500544171F13643899C178EC3FB38A9";
        public const string HarvestTempleZhaitanPutridDelugeImpact = "FE8B96A200376B4BA75297FF2367C5C4";
        public const string HarvestTempleZhaitanPutridDelugeAoE = "82A8BC954DD69E4DBBF526EE1C6A3E74";
        public const string HarvestTempleSpreadNM = "F39933B190100B4C87E808Ef8E6C654A";
        public const string HarvestTempleSpreadCM = "BDF708225224C64183BA3CE2A609D37F";
        public const string HarvestTempleRedPuddleSelectNM = "0CD6F76C1BF9C049A2FCE4D86CB46475";
        public const string HarvestTempleRedPuddleNM = "60EE2CA1A95C514F8A325B654E0D9478";
        public const string HarvestTempleRedPuddleSelectCM = "61C1CD7E89346843B04FCE613EC487AA";
        public const string HarvestTempleRedPuddleCM = "FF0A7D32AD894E45993BE5ED748BF484";
        public const string HarvestTempleGreen = "72EE47DE4F63D3438E193578011FBCBF";
        public const string HarvestTempleFailedGreen = "F4F80E9AF2B6AF49AFE46D8CF797B604";
        public const string HarvestTempleOrbExplosion = "B329CFB6B354C148A537E114DC14CED6";
        public const string HarvestTempleVoidPool = "912F68E45158C14E9A30D6011B7B0C7F";
        public const string HarvestTempleSooWonClaw = "CB877C57D1423240BACDF8D6B52A440F";
        public const string HarvestTempleTormentOfTheVoidClawIndicator = "3F24896D3EF8D5459B399DAC8D0AD150"; // AoE indicator for bouncing orbs after Soo-Won's Claw Slap attack
        public const string HarvestTempleTormentOfTheVoidTailIndicator = "C1A523D71A841048897211B1020B8D95"; // AoE indicator for bouncing orbs after Soo-Won's Tail Slam attack
        public const string HarvestTempleTsunamiIndicator = "8B0EBA3241E1ED469DAC7AFD4E385FF2";
        public const string HarvestTempleTsunami1 = "8F96447526A09B4F8545CBEA1B0046D4"; // There are multiple effects when the Tsunami goes off
        public const string HarvestTempleTsunami2 = "C2CF236673BC0141B6EE5A918869728A"; // There are multiple effects when the Tsunami goes off
        public const string HarvestTempleTsunami3 = "E4700E828E058649B9B94F170DEF8659"; // There are multiple effects when the Tsunami goes off
        public const string HarvestTempleTailSlamIndicator = "49BD7FF8309E4047B4D17C83E660A461";
    }
}
