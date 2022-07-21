using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class BuffDamageModifierTarget : BuffDamageModifier
    {
        private BuffsTracker _trackerPlayer { get; set; } = null;
        private GainComputer _gainComputerPlayer { get; set; } = null;

        protected double ComputeGainPlayer(int stack, AbstractHealthDamageEvent dl, ParsedEvtcLog log)
        {
            if (DLChecker != null && !DLChecker(dl, log))
            {
                return -1.0;
            }
            double gain = _gainComputerPlayer.ComputeGain(1.0, stack);
            return gain > 0.0 ? 1.0 : -1.0;
        }

        internal BuffDamageModifierTarget(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode)
        {
        }

        internal BuffDamageModifierTarget(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode)
        {
        }

        internal BuffDamageModifierTarget UsingPlayerTracking(long playerId, GainComputer gainComputerPlayer)
        {
            _trackerPlayer = new BuffsTrackerSingle(playerId);
            _gainComputerPlayer = gainComputerPlayer;
            return this;
        }

        internal BuffDamageModifierTarget UsingPlayerTracking(long[] playerIds, GainComputer gainComputerPlayer)
        {
            _trackerPlayer = new BuffsTrackerMulti(new List<long>(playerIds));
            _gainComputerPlayer = gainComputerPlayer;
            return this;
        }

        internal override List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedEvtcLog log)
        {
            Dictionary<long, BuffsGraphModel> bgmsP = actor.GetBuffGraphs(log);
            if (_trackerPlayer != null)
            {
                if (!_trackerPlayer.Has(bgmsP) && _gainComputerPlayer != ByAbsence)
                {
                    return new List<DamageModifierEvent>();
                }
            }
            var res = new List<DamageModifierEvent>();
            IReadOnlyList<AbstractHealthDamageEvent> typeHits = GetHitDamageEvents(actor, log, null, 0, log.FightData.FightEnd);
            if (_trackerPlayer != null)
            {
                foreach (AbstractHealthDamageEvent evt in typeHits)
                {
                    AbstractSingleActor target = log.FindActor(evt.To);
                    Dictionary<long, BuffsGraphModel> bgms = target.GetBuffGraphs(log);
                    double gain = ComputeGainPlayer(_trackerPlayer.GetStack(bgmsP, evt.Time), evt, log) < 0.0 ? -1.0 : ComputeGain(Tracker.GetStack(bgms, evt.Time), evt, log);
                    res.Add(new DamageModifierEvent(evt, this, gain));
                }
            }
            else
            {
                foreach (AbstractHealthDamageEvent evt in typeHits)
                {
                    AbstractSingleActor target = log.FindActor(evt.To);
                    Dictionary<long, BuffsGraphModel> bgms = target.GetBuffGraphs(log);
                    res.Add(new DamageModifierEvent(evt, this, ComputeGain(Tracker.GetStack(bgms, evt.Time), evt, log)));
                }
            }
            res.RemoveAll(x => x.DamageGain == -1.0);
            return res;
        }
    }
}
