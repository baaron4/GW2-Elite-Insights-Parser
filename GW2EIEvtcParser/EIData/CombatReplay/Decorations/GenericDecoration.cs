using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecoration
    {

        internal abstract class GenericDecorationMetadata
        {

            public abstract string GetSignature();

            internal abstract GenericDecoration GetDecorationFromVariable(VariableGenericDecoration variable);

        }
        internal abstract class VariableGenericDecoration
        {
            public (int start, int end) Lifespan { get; }

            protected VariableGenericDecoration((long start, long end) lifespan)
            {
                Lifespan = ((int)lifespan.start, (int)lifespan.end);
            }
        }

        internal GenericDecorationMetadata DecorationMetadata { get; }
        internal VariableGenericDecoration VariableDecoration { get; }

        public (int start, int end) Lifespan => VariableDecoration.Lifespan;
        internal GenericDecoration(GenericDecorationMetadata metaData, VariableGenericDecoration variable)
        {
            DecorationMetadata = metaData;
            VariableDecoration = variable;
        }
        protected GenericDecoration()
        {
        }
        //

        public abstract GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs);

    }
}
