using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class AbstractExtensionHandler
    {
        public uint Signature { get; }
        public uint Revision { get; protected set; }
        public string Name { get; } = "Unknown";
        public string Version { get; protected set; } = "Unknown";

        internal AbstractExtensionHandler(uint sig, string name)
        {
            Signature = sig;
            Name = name;
        }

        internal abstract bool HasTime(CombatItem c);
        internal abstract bool SrcIsAgent(CombatItem c);
        internal abstract bool DstIsAgent(CombatItem c);

        internal abstract bool IsDamage(CombatItem c);

        internal abstract void InsertEIExtensionEvent(CombatItem c, AgentData agentData, SkillData skillData);

        internal abstract void AttachToCombatData(CombatData combatData, ParserController operation);

    }
}
