using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class BuffDamageModifierTarget : BuffDamageModifier
    {
        private BuffsTracker _trackerSource { get; set; } = null;
        private GainComputer _gainComputerSource { get; set; } = null;
        internal BuffDamageModifierTarget(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode)
        {
        }

        internal BuffDamageModifierTarget(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode)
        {
        }

        internal BuffDamageModifierTarget UsingSourceActivatorByAbsence(long activatorID)
        {
            _trackerSource = new BuffsTrackerSingle(activatorID);
            _gainComputerSource = ByAbsence;
            return this;
        }

        internal BuffDamageModifierTarget UsingSourceActivatorByAbsence(long[] activatorIDs)
        {
            _trackerSource = new BuffsTrackerMulti(new List<long>(activatorIDs));
            _gainComputerSource = ByAbsence;
            return this;
        }

        internal BuffDamageModifierTarget UsingSourceActivatorByPresence(long activatorID)
        {
            _trackerSource = new BuffsTrackerSingle(activatorID);
            _gainComputerSource = ByPresence;
            return this;
        }

        internal BuffDamageModifierTarget UsingSourceActivatorByPresence(long[] activatorIDs)
        {
            _trackerSource = new BuffsTrackerMulti(new List<long>(activatorIDs));
            _gainComputerSource = ByPresence;
            return this;
        }

        protected bool IsSourceActivated(IReadOnlyDictionary<long, BuffsGraphModel> bgmsSource, AbstractHealthDamageEvent dl)
        {
            return _gainComputerSource == null || _gainComputerSource.ComputeGain(1.0, _trackerSource.GetStack(bgmsSource, dl.Time)) > 0.0;
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
                    if (IsSourceActivated(bgmsSource, evt))
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
