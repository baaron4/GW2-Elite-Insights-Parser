using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.DataModels
{
    public class ParseEnum
    {
        // Activation
        public enum Activation : byte
        {
            None         = 0,
            Normal       = 1,
            Quickness    = 2,
            CancelFire   = 3,
            CancelCancel = 4,
            Reset        = 5,

            Unknown
        };

        public static Activation getActivation(byte bt)
        {
            return bt < (byte)Activation.Unknown
                ? (Activation)bt
                : Activation.Unknown;
        }

        // Buff remove
        public enum BuffRemove : byte
        {
            None   = 0,
            All    = 1,
            Single = 2,
            Manual = 3,
        };

        public static BuffRemove getBuffRemove(byte bt)
        {
            return bt <= 3
                ? (BuffRemove)bt
                : BuffRemove.None;
        }

        // Result
        
        public enum Result : byte
        {
            Normal      = 0,
            Crit        = 1,
            Glance      = 2,
            Block       = 3,
            Evade       = 4,
            Interrupt   = 5,
            Absorb      = 6,
            Blind       = 7,
            KillingBlow = 8,
            Downed      = 9,

            Unknown
        };

        public static Result getResult(byte bt)
        {
            return bt < (byte)Result.Unknown
                ? (Result)bt
                : Result.Unknown;
        }

        // State Change    
        public enum StateChange : byte
        {
            Normal          =  0,
            EnterCombat     =  1,
            ExitCombat      =  2,
            ChangeUp        =  3,
            ChangeDead      =  4,
            ChangeDown      =  5,
            Spawn           =  6,
            Despawn         =  7,
            HealthUpdate    =  8,
            LogStart        =  9,
            LogEnd          = 10,
            WeaponSwap      = 11,
            MaxHealthUpdate = 12,
            PointOfView     = 13,
            CBTSLanguage    = 14,
            GWBuild         = 15,
            ShardId         = 16,
            Reward          = 17,
            BuffInitial     = 18,
            Position        = 19,
            Velocity        = 20,
            Rotation        = 21,
            Unknown
        };

        public static StateChange getStateChange(byte bt)
        {
            return bt < (byte)StateChange.Unknown
                ? (StateChange)bt
                : StateChange.Unknown;
        }

        // Friend of for

        public enum IFF : byte
        {
            Friend  = 0,
            Foe     = 1,

            Unknown
        };

        public static IFF getIFF(byte bt)
        {
            return bt < (byte)IFF.Unknown
                ? (IFF)bt
                : IFF.Unknown;
        }

        public enum ThrashIDS : ushort
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
            // Xera
            XerasPhantasm = 16225,
            WhiteMantleSeeker1 = 16238,
            WhiteMantleSeeker2 = 16283,
            WhiteMantleKnight = 16251,
            WhiteMantleBattleMage = 16221,
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
            // Dhuum
            Messenger = 19807,
            Echo = 19628,
            Enforcer = 19681,
            Deathling = 19759,
            UnderworldReaper = 19831,
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
            DOC =16657,
            CHOP =16552,
            //
            Unknown
        };
        public static ThrashIDS getThrashIDS(ushort id)
        {
            return Enum.IsDefined(typeof(ThrashIDS), id) ? (ThrashIDS)id : ThrashIDS.Unknown;
        }

        public enum BossIDS : ushort
        {
            ValeGuardian    = 15438,
            Gorseval        = 15429,
            Sabetha         = 15375,
            Slothasor       = 16123,
            Matthias        = 16115,
            KeepConstruct   = 16235,
            Xera            = 16246,
            Cairn           = 17194,
            MursaatOverseer = 17172,
            Samarog         = 17188,
            Deimos          = 17154,
            SoullessHorror  = 19767,
            Dhuum           = 19450,
            MAMA            = 17021,
            Siax            = 17028,
            Ensolyss        = 16948,
            Skorvald        = 17632,
            Artsariiv       = 17949,
            Arkk            = 17759,
            Golem1          = 16202,
            Golem2          = 16177,
            Golem3          = 19676,
            Golem4          = 19645,
            Golem5          = 16199,
            //
            Unknown
        };
        public static BossIDS getBossIDS(ushort id)
        {
            return Enum.IsDefined(typeof(BossIDS), id) ? (BossIDS)id : BossIDS.Unknown;
        }

    }

    static class ResultExtensions
    {
        public static bool IsHit(this ParseEnum.Result result)
        {
            return result == ParseEnum.Result.Normal || result == ParseEnum.Result.Crit || result == ParseEnum.Result.Glance;
        }
    }

    static class SpanwExtensions
    {
        public static bool IsSpawn(this ParseEnum.StateChange state)
        {
            return state == ParseEnum.StateChange.Normal || state == ParseEnum.StateChange.Position || state == ParseEnum.StateChange.Velocity || state == ParseEnum.StateChange.Rotation || state == ParseEnum.StateChange.MaxHealthUpdate || state == ParseEnum.StateChange.Spawn;
        }

        public static bool IsDead(this ParseEnum.StateChange state)
        {
            return state == ParseEnum.StateChange.ChangeDead;
        }

        public static bool IsDown(this ParseEnum.StateChange state)
        {
            return state == ParseEnum.StateChange.ChangeDown;
        }

        public static bool IsUp(this ParseEnum.StateChange state)
        {
            return state == ParseEnum.StateChange.ChangeUp;
        }

        public static bool IsDespawn(this ParseEnum.StateChange state)
        {
            return state == ParseEnum.StateChange.Despawn;
        }
    }

    static class ActivationExtensions
    {
        public static bool IsCasting(this ParseEnum.Activation activation)
        {
            return activation == ParseEnum.Activation.Normal || activation == ParseEnum.Activation.Quickness;
        }
    }
}
