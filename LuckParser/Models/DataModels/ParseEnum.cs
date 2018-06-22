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
    }
}
