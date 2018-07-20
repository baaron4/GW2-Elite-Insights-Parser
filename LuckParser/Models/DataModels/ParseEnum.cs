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
    }

    static class ResultExtensions
    {
        public static bool IsHit(this ParseEnum.Result result)
        {
            return result == ParseEnum.Result.Normal || result == ParseEnum.Result.Crit || result == ParseEnum.Result.Glance;
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
