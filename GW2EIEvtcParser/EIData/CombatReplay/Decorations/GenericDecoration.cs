using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecoration
    {

        public abstract class ConstantGenericDecoration
        {

            public abstract string GetID();
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

        protected GenericDecoration()
        {
        }
        //

        public abstract GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs);

    }
}
