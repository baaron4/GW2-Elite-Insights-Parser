using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    internal abstract class AbstractExtensionHandler
    {
        public uint Sig { get; }

        protected AbstractExtensionHandler(uint sig)
        {
            Sig = sig;
        }

        public abstract bool HasTime(CombatItem c);
        public abstract bool SrcIsAgent(CombatItem c);
        public abstract bool DstIsAgent(CombatItem c);

        public abstract bool IsDamage(CombatItem c);

        public abstract void InsertEIExtensionEvent(CombatData combatData, CombatItem c);

    }
}
