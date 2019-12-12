using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class FinalSupportAll : FinalSupport
    {
        //public long allHeal;
        public long Resurrects { get; set; }
        public double ResurrectTime { get; set; }

        private static long[] GetReses(ParsedLog log, AbstractSingleActor actor, long start, long end)
        {
            List<AbstractCastEvent> cls = actor.GetCastLogs(log, start, end);
            long[] reses = { 0, 0 };
            foreach (AbstractCastEvent cl in cls)
            {
                if (cl.SkillId == SkillItem.ResurrectId)
                {
                    reses[0]++;
                    reses[1] += cl.ActualDuration;
                }
            }
            return reses;
        }

        public FinalSupportAll(ParsedLog log, PhaseData phase, AbstractSingleActor actor) : base(log, phase, actor)
        {
            long[] resArray = GetReses(log, actor, phase.Start, phase.End);
            Resurrects = resArray[0];
            ResurrectTime = resArray[1];
        }

    }
}
