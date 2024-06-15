using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecoration
    {

        public abstract class ConstantGenericDecoration
        {

            public abstract string GetSignature();

            internal abstract GenericDecoration GetDecorationFromVariable(VariableGenericDecoration variable);

        }
        public abstract class VariableGenericDecoration
        {
            public (int start, int end) Lifespan { get; }

            protected VariableGenericDecoration((long start, long end) lifespan)
            {
                Lifespan = ((int)lifespan.start, (int)lifespan.end);
            }
        }

        public ConstantGenericDecoration ConstantDecoration { get; protected set; }
        public VariableGenericDecoration VariableDecoration { get; protected set; }

        public (int start, int end) Lifespan => VariableDecoration.Lifespan;
        internal GenericDecoration(ConstantGenericDecoration constant, VariableGenericDecoration variable)
        {
            ConstantDecoration = constant;
            VariableDecoration = variable;
        }
        protected GenericDecoration()
        {
        }
        //

        public abstract GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs);

    }
}
