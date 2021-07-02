using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.Extensions
{
    internal static class ExtensionHelper
    {
        public static AbstractExtensionHandler GetExtensionHandler(CombatItem c)
        {
            if (c.IsStateChange != ArcDPSEnums.StateChange.Extension && c.Pad != 0)
            {
                return null;
            }
            // place holder for sig
            switch (c.OverstackValue)
            {
                // TODO: based on sig, do a switch on rev
                default:
                    break;
            }
            return null;
        }
    }
}
