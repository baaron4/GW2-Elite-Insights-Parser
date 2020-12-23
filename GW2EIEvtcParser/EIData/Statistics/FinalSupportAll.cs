using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class FinalSupportAll : FinalSupport
    {
        //public long allHeal;
        public int Resurrects { get; internal set; }
        public long ResurrectTime { get; internal set; }

        private static long[] GetReses(ParsedEvtcLog log, AbstractSingleActor actor, long start, long end)
        {
            IReadOnlyList<AbstractCastEvent> cls = actor.GetCastEvents(log, start, end);
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

        internal FinalSupportAll(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor) : base(log, start, end, actor, null)
        {
            long[] resArray = GetReses(log, actor, start, end);
            Resurrects = (int)resArray[0];
            ResurrectTime = resArray[1];
        }

    }
}
