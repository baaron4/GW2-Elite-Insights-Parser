using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class AbstractExtensionHandler
    {
        public uint Signature { get; }
        public uint Revision { get; }
        public string Name { get; }
        public string Version { get; }

        internal AbstractExtensionHandler(CombatItem c, string name)
        {
            Signature = c.OverstackValue;
            Revision = c.SkillID;
            Name = name;
        }

        internal abstract bool HasTime(CombatItem c);
        internal abstract bool SrcIsAgent(CombatItem c);
        internal abstract bool DstIsAgent(CombatItem c);

        internal abstract bool IsDamage(CombatItem c);

        internal abstract void InsertEIExtensionEvent(CombatItem c);

        internal abstract void AttachToCombatData(CombatData combatData, ParserController operation);

    }
}
