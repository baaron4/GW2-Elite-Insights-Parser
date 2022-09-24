using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    internal class EXTBarrierCastFinder : InstantCastFinder
    {
        public delegate bool BarrierCastChecker(EXTAbstractBarrierEvent evt, ParsedEvtcLog log);
        private BarrierCastChecker _triggerCondition { get; set; }

        private readonly long _damageSkillID;
        public EXTBarrierCastFinder(long skillID, long damageSkillID) : base(skillID)
        {
            UsingNotAccurate(true);
            _damageSkillID = damageSkillID;
        }
        internal EXTBarrierCastFinder UsingChecker(BarrierCastChecker checker)
        {
            _triggerCondition = checker;
            return this;
        }

        public override List<InstantCastEvent> ComputeInstantCast(ParsedEvtcLog log)
        {
            var res = new List<InstantCastEvent>();
            if (!log.CombatData.HasEXTBarrier)
            {
                return res;
            }
            var heals = log.CombatData.EXTBarrierCombatData.GetBarrierData(_damageSkillID).GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<EXTAbstractBarrierEvent>> pair in heals)
            {
                long lastTime = int.MinValue;
                if (!HealingStatsExtensionHandler.SanitizeForSrc(pair.Value))
                {
                    continue;
                }
                foreach (EXTAbstractBarrierEvent de in pair.Value)
                {
                    if (de.Time - lastTime < ICD)
                    {
                        lastTime = de.Time;
                        continue;
                    }
                    if (_triggerCondition != null)
                    {
                        if (_triggerCondition(de, log))
                        {
                            lastTime = de.Time;
                            res.Add(new InstantCastEvent(de.Time, log.SkillData.Get(SkillID), de.From));
                        }
                    }
                    else
                    {
                        lastTime = de.Time;
                        res.Add(new InstantCastEvent(de.Time, log.SkillData.Get(SkillID), de.From));
                    }
                }
            }
            return res;
        }
    }
}
