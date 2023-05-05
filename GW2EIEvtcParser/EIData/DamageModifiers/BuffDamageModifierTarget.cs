using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class BuffDamageModifierTarget : BuffDamageModifier
    {
        private BuffsTracker _trackerSource { get; set; } = null;
        private GainComputer _gainComputerSource { get; set; } = null;

        protected bool IsSourceActivated(IReadOnlyDictionary<long, BuffsGraphModel> bgmsSource, AbstractHealthDamageEvent dl, ParsedEvtcLog log)
        {
            return _gainComputerSource == null || _gainComputerSource.ComputeGain(1.0, _trackerSource.GetStack(bgmsSource, dl.Time)) > 0.0;
        }

        internal BuffDamageModifierTarget(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode)
        {
        }

        internal BuffDamageModifierTarget(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode)
        {
        }

        internal BuffDamageModifierTarget UsingSourceActivator(long activatorID, GainComputer gainComputerSource)
        {
            _trackerSource = new BuffsTrackerSingle(activatorID);
            _gainComputerSource = gainComputerSource;
            return this;
        }

        internal BuffDamageModifierTarget UsingSourceActivator(long[] activatorIDs, GainComputer gainComputerSource)
        {
            _trackerSource = new BuffsTrackerMulti(new List<long>(activatorIDs));
            _gainComputerSource = gainComputerSource;
            return this;
        }

        internal override List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedEvtcLog log)
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
            IReadOnlyList<AbstractHealthDamageEvent> typeHits = GetHitDamageEvents(actor, log, null, log.FightData.FightStart, log.FightData.FightEnd);
            foreach (AbstractHealthDamageEvent evt in typeHits)
            {
                if (CheckCondition(evt, log))
                {
                    AbstractSingleActor target = log.FindActor(evt.To);
                    IReadOnlyDictionary<long, BuffsGraphModel> bgms = target.GetBuffGraphs(log);
                    if (IsSourceActivated(bgmsSource, evt, log))
                    {
                        res.Add(new DamageModifierEvent(evt, this, ComputeGain(bgms, evt, log)));
                    }
                }
            }
            res.RemoveAll(x => x.DamageGain == -1.0);
            return res;
        }
    }
}
