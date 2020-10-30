using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class BuffDamageModifierTarget : BuffDamageModifier
    {
        private readonly BuffsTracker _trackerPlayer = null;
        private readonly GainComputer _gainComputerPlayer = null;

        protected double ComputeGainPlayer(int stack, AbstractDamageEvent dl)
        {
            if (DLChecker != null && !DLChecker(dl))
            {
                return -1.0;
            }
            double gain = _gainComputerPlayer.ComputeGain(1.0, stack);
            return gain > 0.0 ? 1.0 : -1.0;
        }

        internal BuffDamageModifierTarget(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode, dlChecker)
        {
        }

        internal BuffDamageModifierTarget(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, ulong minBuild, ulong maxBuild, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, mode, dlChecker)
        {
        }

        internal BuffDamageModifierTarget(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode, dlChecker)
        {
        }

        internal BuffDamageModifierTarget(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, ulong minBuild, ulong maxBuild, DamageModifierMode mode, DamageLogChecker dlChecker = null) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, mode, dlChecker)
        {
        }

        internal BuffDamageModifierTarget(long id, long playerId, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, GainComputer gainComputerPlayer, string icon, DamageModifierMode mode, DamageLogChecker dlChecker = null) : this(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode, dlChecker)
        {
            _trackerPlayer = new BuffsTrackerSingle(playerId);
            _gainComputerPlayer = gainComputerPlayer;
        }

        internal BuffDamageModifierTarget(long id, long[] playerIds, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, GainComputer gainComputerPlayer, string icon, ulong minBuild, ulong maxBuild, DamageModifierMode mode, DamageLogChecker dlChecker = null) : this(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, mode, dlChecker)
        {
            _trackerPlayer = new BuffsTrackerMulti(new List<long>(playerIds));
            _gainComputerPlayer = gainComputerPlayer;
        }

        internal BuffDamageModifierTarget(long[] ids, long playerId, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, GainComputer gainComputerPlayer, string icon, DamageModifierMode mode, DamageLogChecker dlChecker = null) : this(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, mode, dlChecker)
        {
            _trackerPlayer = new BuffsTrackerSingle(playerId);
            _gainComputerPlayer = gainComputerPlayer;
        }

        internal BuffDamageModifierTarget(long[] ids, long[] playerIds, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, GainComputer gainComputerPlayer, string icon, ulong minBuild, ulong maxBuild, DamageModifierMode mode, DamageLogChecker dlChecker = null) : this(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, mode, dlChecker)
        {
            _trackerPlayer = new BuffsTrackerMulti(new List<long>(playerIds));
            _gainComputerPlayer = gainComputerPlayer;
        }


        internal override void ComputeDamageModifier(Dictionary<string, List<DamageModifierStat>> data, Dictionary<NPC, Dictionary<string, List<DamageModifierStat>>> dataTarget, Player p, ParsedEvtcLog log)
        {
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            Dictionary<long, BuffsGraphModel> bgmsP = p.GetBuffGraphs(log);
            if (_trackerPlayer != null)
            {
                if (!_trackerPlayer.Has(bgmsP) && _gainComputerPlayer != ByAbsence)
                {
                    return;
                }
            }
            foreach (NPC target in log.FightData.Logic.Targets)
            {
                Dictionary<long, BuffsGraphModel> bgms = target.GetBuffGraphs(log);
                if (!Tracker.Has(bgms) && GainComputer != ByAbsence)
                {
                    continue;
                }
                if (!dataTarget.TryGetValue(target, out Dictionary<string, List<DamageModifierStat>> extra))
                {
                    dataTarget[target] = new Dictionary<string, List<DamageModifierStat>>();
                }
                Dictionary<string, List<DamageModifierStat>> dict = dataTarget[target];
                if (!dict.TryGetValue(Name, out List<DamageModifierStat> list))
                {
                    var extraDataList = new List<DamageModifierStat>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        int totalDamage = GetTotalDamage(p, log, target, i);
                        IReadOnlyList<AbstractDamageEvent> typedHits = GetHitDamageLogs(p, log, target, phases[i]);
                        List<double> damages;
                        if (_trackerPlayer != null)
                        {
                            damages = typedHits.Select(x =>
                            {

                                if (ComputeGainPlayer(_trackerPlayer.GetStack(bgmsP, x.Time), x) < 0.0)
                                {
                                    return -1.0;
                                }
                                return ComputeGain(Tracker.GetStack(bgms, x.Time), x);
                            }).Where(x => x != -1.0).ToList();
                        }
                        else
                        {
                            damages = typedHits.Select(x =>
                            {
                                return ComputeGain(Tracker.GetStack(bgms, x.Time), x);
                            }).Where(x => x != -1.0).ToList();
                        }
                        extraDataList.Add(new DamageModifierStat(damages.Count, typedHits.Count, damages.Sum(), totalDamage));
                    }
                    dict[Name] = extraDataList;
                }
            }
        }
    }
}
