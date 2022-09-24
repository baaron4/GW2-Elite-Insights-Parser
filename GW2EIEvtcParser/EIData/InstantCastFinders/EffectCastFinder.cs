using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class EffectCastFinder : InstantCastFinder
    {
        public delegate bool EffectCastChecker(EffectEvent evt, ParsedEvtcLog log);
        private EffectCastChecker _triggerCondition { get; set; }

        private readonly string _effectGUID;

        protected virtual Dictionary<AgentItem, List<EffectEvent>> GetEffectEventDict(EffectGUIDEvent effectGUIDEvent, CombatData combatData)
        {
            return combatData.GetEffectEvents(effectGUIDEvent.ContentID).GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
        }

        protected virtual AgentItem GetAgent(EffectEvent effectEvent)
        {
            return effectEvent.Src;
        }

        public EffectCastFinder(long skillID, string effectGUID) : base(skillID)
        {
            UsingNotAccurate(true); // TODO: confirm if culling is server side logic
            _effectGUID = effectGUID;
        }

        internal EffectCastFinder UsingChecker(EffectCastChecker checker)
        {
            _triggerCondition = checker;
            return this;
        }

        public override List<InstantCastEvent> ComputeInstantCast(ParsedEvtcLog log)
        {
            var res = new List<InstantCastEvent>();
            EffectGUIDEvent effectGUIDEvent = log.CombatData.GetEffectGUIDEvent(_effectGUID);
            if (effectGUIDEvent != null)
            {
                Dictionary<AgentItem, List<EffectEvent>> effects = GetEffectEventDict(effectGUIDEvent, log.CombatData);
                foreach (KeyValuePair<AgentItem, List<EffectEvent>> pair in effects)
                {
                    long lastTime = int.MinValue;
                    foreach (EffectEvent effectEvent in pair.Value)
                    {
                        if (effectEvent.Time - lastTime < ICD)
                        {
                            lastTime = effectEvent.Time;
                            continue;
                        }
                        if (_triggerCondition != null)
                        {
                            if (_triggerCondition(effectEvent, log))
                            {
                                lastTime = effectEvent.Time;
                                res.Add(new InstantCastEvent(effectEvent.Time, log.SkillData.Get(SkillID), GetAgent(effectEvent)));
                            }
                        }
                        else
                        {
                            lastTime = effectEvent.Time;
                            res.Add(new InstantCastEvent(effectEvent.Time, log.SkillData.Get(SkillID), GetAgent(effectEvent)));
                        }
                    }
                }
            }
            return res;
        }
    }
}
