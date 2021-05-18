using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class SingleActorDamageHelper : AbstractSingleActorHelper
    {

        // damage list
        private CachingCollectionWithTarget<List<AbstractHealthDamageEvent>> _hitSelfDamageEventsPerPhasePerTarget;
        private CachingCollectionWithTarget<List<AbstractHealthDamageEvent>> _powerHitSelfDamageEventsPerPhasePerTarget;
        private CachingCollectionWithTarget<List<AbstractHealthDamageEvent>> _conditionHitSelfDamageEventsPerPhasePerTarget;

        public SingleActorDamageHelper(AbstractSingleActor actor) : base(actor)
        {
        }

        internal IReadOnlyList<AbstractHealthDamageEvent> GetJustActorHitDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_hitSelfDamageEventsPerPhasePerTarget == null)
            {
                _hitSelfDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
            }
            if (!_hitSelfDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = Actor.GetHitDamageEvents(target, log, start, end).Where(x => x.From == AgentItem).ToList();
                _hitSelfDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        internal IReadOnlyList<AbstractHealthDamageEvent> GetJustActorConditionHitDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_conditionHitSelfDamageEventsPerPhasePerTarget == null)
            {
                _conditionHitSelfDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
            }
            if (!_conditionHitSelfDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = Actor.GetJustActorHitDamageEvents(target, log, start, end).Where(x => x.ConditionDamageBased(log)).ToList();
                _conditionHitSelfDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        internal IReadOnlyList<AbstractHealthDamageEvent> GetJustActorPowerHitDamageEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_powerHitSelfDamageEventsPerPhasePerTarget == null)
            {
                _powerHitSelfDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<AbstractHealthDamageEvent>>(log);
            }
            if (!_powerHitSelfDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = Actor.GetJustActorHitDamageEvents(target, log, start, end).Where(x => !x.ConditionDamageBased(log)).ToList();
                _powerHitSelfDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }
    }
}
