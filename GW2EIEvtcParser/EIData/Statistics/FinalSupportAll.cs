using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class FinalSupportAll : FinalSupport
    {
        //public long allHeal;
        public int Resurrects { get; internal set; }
        public long ResurrectTime { get; internal set; }

        private static long[] GetReses(ParsedEvtcLog log, AbstractSingleActor actor, long start, long end)
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

        internal FinalSupportAll(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor actor) : base(log, phase, actor, null)
        {
            long[] resArray = GetReses(log, actor, phase.Start, phase.End);
            Resurrects = (int)resArray[0];
            ResurrectTime = resArray[1];
        }

    }
}
