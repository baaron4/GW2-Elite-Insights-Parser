using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffLossCastFinder : BuffCastFinder
    {

        public delegate bool BuffLossCastChecker(BuffRemoveAllEvent evt, ParsedEvtcLog log);
        private BuffLossCastChecker _triggerCondition { get; set; }


        public BuffLossCastFinder(long skillID, long buffID) : base(skillID, buffID)
        {
        }

        internal BuffLossCastFinder UsingChecker(BuffLossCastChecker checker)
        {
            _triggerCondition = checker;
            return this;
        }

        public override List<InstantCastEvent> ComputeInstantCast(ParsedEvtcLog log)
        {
            var res = new List<InstantCastEvent>();
            var removals = log.CombatData.GetBuffData(BuffID).OfType<BuffRemoveAllEvent>().GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<BuffRemoveAllEvent>> pair in removals)
            {
                long lastTime = int.MinValue;
                foreach (BuffRemoveAllEvent brae in pair.Value)
                {
                    if (brae.Time - lastTime < ICD)
                    {
                        lastTime = brae.Time;
                        continue;
                    }
                    if (_triggerCondition != null)
                    {
                        if (_triggerCondition(brae, log))
                        {
                            lastTime = brae.Time;
                            res.Add(new InstantCastEvent(brae.Time, log.SkillData.Get(SkillID), brae.To));
                        }
                    }
                    else
                    {
                        lastTime = brae.Time;
                        res.Add(new InstantCastEvent(brae.Time, log.SkillData.Get(SkillID), brae.To));
                    }
                }
            }
            return res;
        }
    }
}
