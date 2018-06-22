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
        public enum Activation { None, Normal, Quickness, CancelFire, CancelCancel, Reset, Unknown };

        public static Activation getActivation(byte bt)
        {
            switch (bt)
            {
                case 0:
                    return Activation.None;
                case 1:
                    return Activation.Normal;
                case 2:
                    return Activation.Quickness;
                case 3:
                    return Activation.CancelFire;
                case 4:
                    return Activation.CancelCancel;
                case 5:
                    return Activation.Reset;
                default:
                    return Activation.Unknown;
            }
        }

        public static bool casting(Activation status)
        {
            return status == Activation.Normal || status == Activation.Quickness;
        }

        // Buff remove
        public enum BuffRemove { None, All, Single, Manual};

        public static BuffRemove getBuffRemove(byte bt)
        {
            switch (bt)
            {
                case 1:
                    return BuffRemove.All;
                case 2:
                    return BuffRemove.Single;
                case 3:
                    return BuffRemove.Manual;
                default:
                    return BuffRemove.None;
            }
        }

        // Result
        
        public enum Result { Normal, Crit, Glance, Block , Evade, Interrupt, Absorb, Blind, KillingBlow, Unknown };

        public static Result getResult(byte bt)
        {
            switch (bt)
            {
                case 0:
                    return Result.Normal;
                case 1:
                    return Result.Crit;
                case 2:
                    return Result.Glance;
                case 3:
                    return Result.Block;
                case 4:
                    return Result.Evade;
                case 5:
                    return Result.Interrupt;
                case 6:
                    return Result.Absorb;
                case 7:
                    return Result.Blind;
                case 8:
                    return Result.KillingBlow;
                default:
                    return Result.Unknown;
            }
        }

        public static bool hit(Result status)
        {
            return status == Result.Normal || status == Result.Crit || status == Result.Glance;
        }

        // State Change    
        public enum StateChange { Normal, EnterCombat,ExitCombat,ChangeUp,ChangeDead,ChangeDown,
                                Spawn,Despawn,HealthUpdate,LogStart,LogEnd,WeaponSwap,MaxHealthUpdate,
                                PointOfView,CBTSLanguage, GWBuild, ShardId, Reward, BuffInitial, Unknown};

        public static StateChange getStateChange(byte bt)
        {
            switch (bt)
            {
                case 0:
                    return StateChange.Normal;
                case 1:
                    return StateChange.EnterCombat;
                case 2:
                    return StateChange.ExitCombat;
                case 3:
                    return StateChange.ChangeUp;
                case 4:
                    return StateChange.ChangeDead;
                case 5:
                    return StateChange.ChangeDown;
                case 6:
                    return StateChange.Spawn;
                case 7:
                    return StateChange.Despawn;
                case 8:
                    return StateChange.HealthUpdate;
                case 9:
                    return StateChange.LogStart;
                case 10:
                    return StateChange.LogEnd;
                case 11:
                    return StateChange.WeaponSwap;
                case 12:
                    return StateChange.MaxHealthUpdate;
                case 13:
                    return StateChange.PointOfView;
                case 14:
                    return StateChange.CBTSLanguage;
                case 15:
                    return StateChange.GWBuild;
                case 16:
                    return StateChange.ShardId;
                case 17:
                    return StateChange.Reward;
                case 18:
                    return StateChange.BuffInitial;
                default:
                    return StateChange.Unknown;
            }
        }

    }
}
