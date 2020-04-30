using System;

namespace GW2EIParser.Parser
{
    public static class ParseEnum
    {
        // Activation
        public enum Activation : byte
        {
            None = 0,
            Normal = 1,
            Quickness = 2,
            CancelFire = 3,
            CancelCancel = 4,
            Reset = 5,

            Unknown
        };

        public static Activation GetActivation(byte bt)
        {
            return bt < (byte)Activation.Unknown
                ? (Activation)bt
                : Activation.Unknown;
        }

        // Buff remove
        public enum BuffRemove : byte
        {
            None = 0,
            All = 1,
            Single = 2,
            Manual = 3
        };

        public static BuffRemove GetBuffRemove(byte bt)
        {
            return bt <= 3
                ? (BuffRemove)bt
                : BuffRemove.None;
        }

        // Result

        public enum PhysicalResult : byte
        {
            Normal = 0,
            Crit = 1,
            Glance = 2,
            Block = 3,
            Evade = 4,
            Interrupt = 5,
            Absorb = 6,
            Blind = 7,
            KillingBlow = 8,
            Downed = 9,

            Unknown
        };

        public static PhysicalResult GetPhysicalResult(byte bt)
        {
            return bt < (byte)PhysicalResult.Unknown
                ? (PhysicalResult)bt
                : PhysicalResult.Unknown;
        }

        public enum ConditionResult : byte
        {
            ExpectedToHit = 0,
            InvulByBuff = 1,
            InvulByPlayerSkill1 = 2,
            InvulByPlayerSkill2 = 3,
            InvulByPlayerSkill3 = 4,

            Unknown
        };
        public static ConditionResult GetConditionResult(byte bt)
        {
            return bt < (byte)ConditionResult.Unknown
                ? (ConditionResult)bt
                : ConditionResult.Unknown;
        }

        // State Change    
        public enum StateChange : byte
        {
            None = 0,
            EnterCombat = 1,
            ExitCombat = 2,
            ChangeUp = 3,
            ChangeDead = 4,
            ChangeDown = 5,
            Spawn = 6,
            Despawn = 7,
            HealthUpdate = 8,
            LogStart = 9,
            LogEnd = 10,
            WeaponSwap = 11,
            MaxHealthUpdate = 12,
            PointOfView = 13,
            Language = 14,
            GWBuild = 15,
            ShardId = 16,
            Reward = 17,
            BuffInitial = 18,
            Position = 19,
            Velocity = 20,
            Rotation = 21,
            TeamChange = 22,
            AttackTarget = 23,
            Targetable = 24,
            MapID = 25,
            ReplInfo = 26,
            StackActive = 27,
            StackReset = 28,
            Guild = 29,
            BuffInfo = 30,
            BuffFormula = 31,
            SkillInfo = 32,
            SkillTiming = 33,
            Unknown
        };

        public static StateChange GetStateChange(byte bt)
        {
            return bt < (byte)StateChange.Unknown
                ? (StateChange)bt
                : StateChange.Unknown;
        }

        public enum Attribute : byte
        {
            None = 0,
            Power = 1,
            Precision = 2,
            Toughness = 3,
            Vitality = 4,
            Ferocity = 5,
            Healing = 6,
            Condition = 7,
            Concentration = 8,
            Expertise = 9,
            Armor = 10,
            Agony = 11,
            StatInc = 12,
            FaltInc = 13,
            PhysInc = 14,
            CondInc = 15,
            Physrec = 16,
            CondRec = 17,
            AttackSpeed = 18
        }

        // Friend of for

        public enum IFF : byte
        {
            Friend = 0,
            Foe = 1,

            Unknown
        };

        // Custom ids
        private const int _twilightCastle = -1;
        private const int _handOfErosion = -2;
        private const int _handOfEruption = -3;
        private const int _pyreGuardianProtect = -4;
        private const int _pyreGuardianStab = -5;
        private const int _pyreGuardianRetal = -6;
        private const int _qadimLamp = -7;


        //

        public static IFF GetIFF(byte bt)
        {
            return bt < (byte)IFF.Unknown
                ? (IFF)bt
                : IFF.Unknown;
        }

        public enum TrashIDS : int
        {
            // VG
            Seekers = 15426,
            RedGuardian = 15433,
            BlueGuardian = 15431,
            GreenGuardian = 15420,
            // Gorse
            ChargedSoul = 15434,
            EnragedSpirit = 16024,
            AngeredSpirit = 16005,
            // Sab
            Kernan = 15372,
            Knuckles = 15404,
            Karde = 15430,
            BanditSapper = 15423,
            BanditThug = 15397,
            BanditArsonist = 15421,
            // Slothasor
            Slubling1 = 16064,
            Slubling2 = 16071,
            Slubling3 = 16077,
            Slubling4 = 16104,
            // Trio
            BanditSaboteur = 16117,
            Warg = 7481,
            CagedWarg = 16129,
            BanditAssassin = 16067,
            BanditSapperTrio = 16074,
            BanditDeathsayer = 16076,
            BanditBrawler = 16066,
            BanditBattlemage = 16093,
            BanditCleric = 16101,
            BanditBombardier = 16138,
            BanditSniper = 16065,
            NarellaTornado = 16092,
            OilSlick = 16096,
            Prisoner1 = 16056,
            Prisoner2 = 16103,
            // Matthias
            Spirit = 16105,
            Spirit2 = 16114,
            IcePatch = 16139,
            Storm = 16108,
            Tornado = 16068,
            // KC
            Olson = 16244,
            Engul = 16274,
            Faerla = 16264,
            Caulle = 16282,
            Henley = 16236,
            Jessica = 16278,
            Galletta = 16228,
            Ianim = 16248,
            Core = 16261,
            GreenPhantasm = 16237,
            InsidiousProjection = 16227,
            UnstableLeyRift = 16277,
            RadiantPhantasm = 16259,
            CrimsonPhantasm = 16257,
            RetrieverProjection = 16249,
            // Twisted Castle
            HauntingStatue = 16247,
            //CastleFountain = 32951,
            // Xera
            ChargedBloodstone = 8267,
            BloodstoneFragment = 40724,
            XerasPhantasm = 16225,
            WhiteMantleSeeker1 = 16238,
            WhiteMantleSeeker2 = 16283,
            WhiteMantleKnight1 = 16251,
            WhiteMantleKnight2 = 16287,
            WhiteMantleBattleMage1 = 16221,
            WhiteMantleBattleMage2 = 16226,
            ExquisiteConjunction = 16232,
            //BloodStone Shard (Gadget)     = 13864,
            // MO
            Jade = 17181,
            // Samarog
            Guldhem = 17208,
            Rigom = 17124,
            // Deimos
            Saul = 17126,
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
            // SH
            TormentedDead = 19422,
            SurgingSoul = 19474,
            Scythe = 19396,
            FleshWurm = 19464,
            // River
            Enervator = 19863,
            HollowedBomber = 19399,
            RiverOfSouls = 19829,
            SpiritHorde1 = 19461,
            SpiritHorde2 = 19400,
            SpiritHorde3 = 19692,
            // Statues of Darkness
            LightThieves = 19658,
            MazeMinotaur = 19402,
            // Statue of Death
            OrbSpider = 19801,
            GreenSpirit1 = 19587,
            GreenSpirit2 = 19571,
            // Skeletons are the same as Spirit hordes
            // Dhuum
            Messenger = 19807,
            Echo = 19628,
            Enforcer = 19681,
            Deathling = 19759,
            UnderworldReaper = 19831,
            DhuumDesmina = 19481,
            // CA
            ConjuredGreatsword = 21255,
            ConjuredShield = 21170,
            // Qadim
            LavaElemental1 = 21236,
            LavaElemental2 = 21078,
            IcebornHydra = 21163,
            GreaterMagmaElemental1 = 21150,
            GreaterMagmaElemental2 = 21223,
            FireElemental = 21221,
            FireImp = 21100,
            PyreGuardian = 21050,
            PyreGuardianRetal = _pyreGuardianRetal,
            PyreGuardianProtect = _pyreGuardianProtect,
            PyreGuardianStab = _pyreGuardianStab,
            ReaperofFlesh = 21218,
            DestroyerTroll = 20944,
            IceElemental = 21049,
            AncientInvokedHydra = 21285,
            ApocalypseBringer = 21073,
            WyvernMatriarch = 20997,
            WyvernPatriarch = 21183,
            QadimLamp = _qadimLamp,
            Zommoros = 20961, //21118 is probably the start and end NPC, not the one during the battle
            // Adina
            HandOfErosion = _handOfErosion,
            HandOfEruption = _handOfEruption,
            // Sabir
            ParalyzingWisp = 21955,
            VoltaicWisp = 21975,
            SmallJumpyTornado = 21961,
            SmallKillerTornado = 21957,
            BigKillerTornado = 21987,
            // Peerless Qadim
            Pylon1 = 21996,
            Pylon2 = 21962,
            EntropicDistortion = 21973,
            EnergyOrb = 21946,
            // Fraenir
            IcebroodElemental = 22576,
            // Boneskinner
            PrioryExplorer = 22561,
            PrioryScholar = 22448,
            VigilRecruit = 22389,
            VigilTactician = 22420,
            AberrantWisp = 22538,
            // Whisper of Jormal
            WhisperEcho = 22628,
            // to complete
            DoppelgangerNecro = 22713,
            DoppelgangerWarrior= 22640,
            DoppelgangerGuardian1= 22635,
            DoppelgangerGuardian2 = 22608,
            DoppelgangerThief1 = 22656,
            DoppelgangerThief2 = 22612,
            DoppelgangerRevenant = 22610,
            // Freezie
            FreeziesFrozenHeart = 21328,
            // Fractals
            FractalVindicator = 19684,
            FractalAvenger = 15960,
            // MAMA
            GreenKnight = 16906,
            RedKnight = 16974,
            BlueKnight = 16899,
            TwistedHorror = 17009,
            // Siax
            Hallucination = 17002,
            EchoOfTheUnclean = 17068,
            // Ensolyss
            NightmareHallucination1 = 16912, // (exploding after jump and charging in last phase)
            NightmareHallucination2 = 17033, // (small adds, last phase)
            // Skorvald
            FluxAnomaly4 = 17673,
            FluxAnomaly3 = 17851,
            FluxAnomaly2 = 17770,
            FluxAnomaly1 = 17599,
            SolarBloom = 17732,
            // Artsariiv
            TemporalAnomaly = 17870,
            Spark = 17630,
            Artsariiv1 = 17811, // tiny adds
            Artsariiv2 = 17694, // small adds
            Artsariiv3 = 17937, // big adds
            // Arkk
            TemporalAnomaly2 = 17720,
            Archdiviner = 17893,
            Fanatic = 11282,
            SolarBloom2 = 17732,
            BrazenGladiator = 17730,
            BLIGHT = 16437,
            PLINK = 16325,
            DOC = 16657,
            CHOP = 16552,
            ProjectionArkk = 17613,
            //
            Unknown
        };
        public static TrashIDS GetTrashIDS(int id)
        {
            return Enum.IsDefined(typeof(TrashIDS), id) ? (TrashIDS)id : TrashIDS.Unknown;
        }

        public enum TargetIDS : int
        {
            WorldVersusWorld = 1,
            TwistedCastle = _twilightCastle,
            // Raid
            ValeGuardian = 15438,
            Gorseval = 15429,
            Sabetha = 15375,
            Slothasor = 16123,
            Berg = 16088,
            Zane = 16137,
            Narella = 16125,
            Matthias = 16115,
            Escort = 16253, // McLeod the Silent
            KeepConstruct = 16235,
            Xera = 16246,
            Cairn = 17194,
            MursaatOverseer = 17172,
            Samarog = 17188,
            Deimos = 17154,
            SoullessHorror = 19767,
            Desmina = 19828,
            BrokenKing = 19691,
            SoulEater = 19536,
            EyeOfJudgement = 19651,
            EyeOfFate = 19844,
            Dhuum = 19450,
            ConjuredAmalgamate = 43974, // Gadget
            CARightArm = 10142, // Gadget
            CALeftArm = 37464, // Gadget
            Nikare = 21105,
            Kenut = 21089,
            Qadim = 20934,
            Freezie = 21333,
            Adina = 22006,
            Sabir = 21964,
            PeerlessQadim = 22000,
            // Strike Missions
            IcebroodConstruct = 22154,
            VoiceOfTheFallen = 22343,
            ClawOfTheFallen = 22481,
            VoiceAndClaw = 22315,
            FraenirOfJormag = 22492,
            IcebroodConstructFraenir = 22436,
            Boneskinner = 22521,
            WhisperOfJormag = 22711,
            // Fract
            MAMA = 17021,
            Siax = 17028,
            Ensolyss = 16948,
            Skorvald = 17632,
            Artsariiv = 17949,
            Arkk = 17759,
            // Golems
            MassiveGolem = 16202,
            AvgGolem = 16177,
            LGolem = 19676,
            MedGolem = 19645,
            StdGolem = 16199,
            //
            Unknown
        };
        public static TargetIDS GetTargetIDS(int id)
        {
            return Enum.IsDefined(typeof(TargetIDS), id) ? (TargetIDS)id : TargetIDS.Unknown;
        }

    }

    internal static class PhysicalResultExtensions
    {
        public static bool IsHit(this ParseEnum.PhysicalResult result)
        {
            return result == ParseEnum.PhysicalResult.Normal || result == ParseEnum.PhysicalResult.Crit || result == ParseEnum.PhysicalResult.Glance || result == ParseEnum.PhysicalResult.KillingBlow; //Downed and Interrupt omitted for now due to double procing mechanics || result == ParseEnum.Result.Downed || result == ParseEnum.Result.Interrupt; 
        }
    }

    internal static class ConditionResultExtensions
    {
        public static bool IsHit(this ParseEnum.ConditionResult result)
        {
            return result == ParseEnum.ConditionResult.ExpectedToHit;
        }
    }

    internal static class SpanwExtensions
    {
        public static bool IsSpawn(this ParseEnum.StateChange state)
        {
            return state == ParseEnum.StateChange.None || state == ParseEnum.StateChange.Position || state == ParseEnum.StateChange.Velocity || state == ParseEnum.StateChange.Rotation || state == ParseEnum.StateChange.MaxHealthUpdate || state == ParseEnum.StateChange.Spawn || state == ParseEnum.StateChange.TeamChange;
        }
    }

    internal static class StateChangeAgentExtensions
    {
        public static bool SrcIsAgent(this ParseEnum.StateChange state)
        {
            return state == ParseEnum.StateChange.None || state == ParseEnum.StateChange.EnterCombat
                || state == ParseEnum.StateChange.ExitCombat || state == ParseEnum.StateChange.ChangeUp
                || state == ParseEnum.StateChange.ChangeDead || state == ParseEnum.StateChange.ChangeDown
                || state == ParseEnum.StateChange.Spawn || state == ParseEnum.StateChange.Despawn
                || state == ParseEnum.StateChange.HealthUpdate || state == ParseEnum.StateChange.WeaponSwap
                || state == ParseEnum.StateChange.MaxHealthUpdate || state == ParseEnum.StateChange.PointOfView
                || state == ParseEnum.StateChange.BuffInitial || state == ParseEnum.StateChange.Position
                || state == ParseEnum.StateChange.Velocity || state == ParseEnum.StateChange.Rotation
                || state == ParseEnum.StateChange.TeamChange || state == ParseEnum.StateChange.AttackTarget
                || state == ParseEnum.StateChange.Targetable || state == ParseEnum.StateChange.StackActive
                || state == ParseEnum.StateChange.StackReset;
        }

        public static bool DstIsAgent(this ParseEnum.StateChange state)
        {
            return state == ParseEnum.StateChange.None || state == ParseEnum.StateChange.AttackTarget;
        }

        public static bool HasTime(this ParseEnum.StateChange state)
        {
            return state == ParseEnum.StateChange.None || state == ParseEnum.StateChange.EnterCombat
                || state == ParseEnum.StateChange.ExitCombat || state == ParseEnum.StateChange.ChangeUp
                || state == ParseEnum.StateChange.ChangeDead || state == ParseEnum.StateChange.ChangeDown
                || state == ParseEnum.StateChange.Spawn || state == ParseEnum.StateChange.Despawn
                || state == ParseEnum.StateChange.HealthUpdate || state == ParseEnum.StateChange.WeaponSwap
                || state == ParseEnum.StateChange.MaxHealthUpdate || state == ParseEnum.StateChange.BuffInitial
                || state == ParseEnum.StateChange.Position || state == ParseEnum.StateChange.Velocity
                || state == ParseEnum.StateChange.Rotation || state == ParseEnum.StateChange.TeamChange
                || state == ParseEnum.StateChange.AttackTarget || state == ParseEnum.StateChange.Targetable
                || state == ParseEnum.StateChange.StackActive || state == ParseEnum.StateChange.StackReset
                || state == ParseEnum.StateChange.Reward;
        }
    }

    internal static class ActivationExtensions
    {
        public static bool StartCasting(this ParseEnum.Activation activation)
        {
            return activation == ParseEnum.Activation.Normal || activation == ParseEnum.Activation.Quickness;
        }

        public static bool NoInterruptEndCasting(this ParseEnum.Activation activation)
        {
            return activation == ParseEnum.Activation.CancelFire || activation == ParseEnum.Activation.Reset;
        }
    }
}
