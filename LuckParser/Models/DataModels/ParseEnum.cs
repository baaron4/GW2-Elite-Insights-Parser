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
    }
}
