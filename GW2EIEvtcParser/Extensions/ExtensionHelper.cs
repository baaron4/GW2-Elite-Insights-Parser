using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.Extensions
{
    public static class ExtensionHelper
    {
        // TODO add supported sigs as public consts
        internal static AbstractExtensionHandler GetExtensionHandler(CombatItem c)
        {
            if (!c.IsExtension && c.Pad != 0)
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
