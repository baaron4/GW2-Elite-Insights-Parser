using System.Collections.Generic;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffOnFoeDamageModifier : BuffOnActorDamageModifier
    {
        private BuffsTracker _trackerSource { get; set; } = null;
        private GainComputer _gainComputerSource { get; set; } = null;
        internal BuffOnFoeDamageModifier(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode)
        {
        }

        internal BuffOnFoeDamageModifier(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode)
        {
        }

        internal BuffOnFoeDamageModifier UsingActorCheckerByAbsence(long activatorID)
        {
            _trackerSource = new BuffsTrackerSingle(activatorID);
            _gainComputerSource = ByAbsence;
            return this;
        }

        internal BuffOnFoeDamageModifier UsingActorCheckerByAbsence(long[] activatorIDs)
        {
            _trackerSource = new BuffsTrackerMulti(new List<long>(activatorIDs));
            _gainComputerSource = ByAbsence;
            return this;
        }

        internal BuffOnFoeDamageModifier UsingActorCheckerByPresence(long activatorID)
        {
            _trackerSource = new BuffsTrackerSingle(activatorID);
            _gainComputerSource = ByPresence;
            return this;
        }

        internal BuffOnFoeDamageModifier UsingActorCheckerByPresence(long[] activatorIDs)
        {
            _trackerSource = new BuffsTrackerMulti(new List<long>(activatorIDs));
            _gainComputerSource = ByPresence;
            return this;
        }

        protected bool CheckActor(IReadOnlyDictionary<long, BuffsGraphModel> bgmsSource, long time)
        {
            return _gainComputerSource == null || _gainComputerSource.ComputeGain(1.0, _trackerSource.GetStack(bgmsSource, time)) > 0.0;
        }

        internal override bool Keep(FightLogic.ParseMode mode, EvtcParserSettings parserSettings)
        {
            // Remove target  based damage mods from PvP contexts
            if (mode == FightLogic.ParseMode.WvW || mode == FightLogic.ParseMode.sPvP)
            {
                return false;
            }
            return base.Keep(mode, parserSettings);
        }
        internal override List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedEvtcLog log, DamageModifier damageModifier)
        {
            IReadOnlyDictionary<long, BuffsGraphModel> bgmsSource = actor.GetBuffGraphs(log);
            if (_trackerSource != null)
            {
                if (!_trackerSource.Has(bgmsSource) && _gainComputerSource != ByAbsence)
                {
                    return new List<DamageModifierEvent>();
                }
            }
            var res = new List<DamageModifierEvent>();
            IReadOnlyList<AbstractHealthDamageEvent> typeHits = damageModifier.GetHitDamageEvents(actor, log, null, log.FightData.FightStart, log.FightData.FightEnd);
            foreach (AbstractHealthDamageEvent evt in typeHits)
            {
                AbstractSingleActor target = log.FindActor(damageModifier.GetFoe(evt));
                IReadOnlyDictionary<long, BuffsGraphModel> bgms = target.GetBuffGraphs(log);
                if (CheckActor(bgmsSource, evt.Time) && ComputeGain(bgms, evt, log, out double gain) && CheckCondition(evt, log))
                {
                    res.Add(new DamageModifierEvent(evt, damageModifier, gain * evt.HealthDamage));
                }
            }
            return res;
        }
    }
}
