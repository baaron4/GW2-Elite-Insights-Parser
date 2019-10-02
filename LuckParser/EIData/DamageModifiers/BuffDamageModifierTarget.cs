using System.Collections.Generic;
using System.Linq;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData.CombatEvents;
using static LuckParser.Models.Statistics;

namespace LuckParser.EIData
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

        public BuffDamageModifierTarget(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string icon, DamageLogChecker dlChecker = null) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, dlChecker)
        {
        }

        public BuffDamageModifierTarget(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string icon, ulong minBuild, ulong maxBuild, DamageLogChecker dlChecker = null) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, dlChecker)
        {
        }

        public BuffDamageModifierTarget(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string icon, DamageLogChecker dlChecker = null) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, dlChecker)
        {
        }

        public BuffDamageModifierTarget(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string icon, ulong minBuild, ulong maxBuild, DamageLogChecker dlChecker = null) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, dlChecker)
        {
        }

        public BuffDamageModifierTarget(long id, long playerId, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, GainComputer gainComputerPlayer, string icon, DamageLogChecker dlChecker = null) : this(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, dlChecker)
        {
            _trackerPlayer = new BuffsTrackerSingle(playerId);
            _gainComputerPlayer = gainComputerPlayer;
        }

        public BuffDamageModifierTarget(long id, long[] playerIds, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, GainComputer gainComputerPlayer, string icon, ulong minBuild, ulong maxBuild, DamageLogChecker dlChecker = null) : this(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, dlChecker)
        {
            _trackerPlayer = new BuffsTrackerMulti(new List<long>(playerIds));
            _gainComputerPlayer = gainComputerPlayer;
        }

        public BuffDamageModifierTarget(long[] ids, long playerId, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, GainComputer gainComputerPlayer, string icon, DamageLogChecker dlChecker = null) : this(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, dlChecker)
        {
            _trackerPlayer = new BuffsTrackerSingle(playerId);
            _gainComputerPlayer = gainComputerPlayer;
        }

        public BuffDamageModifierTarget(long[] ids, long[] playerIds, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, GainComputer gainComputerPlayer, string icon, ulong minBuild, ulong maxBuild, DamageLogChecker dlChecker = null) : this(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, icon, minBuild, maxBuild, dlChecker)
        {
            _trackerPlayer = new BuffsTrackerMulti(new List<long>(playerIds));
            _gainComputerPlayer = gainComputerPlayer;
        }


        public override void ComputeDamageModifier(Dictionary<string, List<DamageModifierData>> data, Dictionary<Target, Dictionary<string, List<DamageModifierData>>> dataTarget, Player p, ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            Dictionary<long, BuffsGraphModel> bgmsP = p.GetBoonGraphs(log);
            if (_trackerPlayer != null)
            {
                if (!_trackerPlayer.Has(bgmsP) && _gainComputerPlayer != ByAbsence)
                {
                    return;
                }
            }
            foreach (Target target in log.FightData.Logic.Targets)
            {
                Dictionary<long, BuffsGraphModel> bgms = target.GetBoonGraphs(log);
                if (!Tracker.Has(bgms) && GainComputer != ByAbsence)
                {
                    continue;
                }
                if (!dataTarget.TryGetValue(target, out Dictionary<string, List<DamageModifierData>> extra))
                {
                    dataTarget[target] = new Dictionary<string, List<DamageModifierData>>();
                }
                Dictionary<string, List<DamageModifierData>> dict = dataTarget[target];
                if (!dict.TryGetValue(Name, out List<DamageModifierData> list))
                {
                    var extraDataList = new List<DamageModifierData>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        int totalDamage = GetTotalDamage(p, log, target, i);
                        List<AbstractDamageEvent> typedHits = GetDamageLogs(p, log, target, phases[i]);
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
                        extraDataList.Add(new DamageModifierData(damages.Count, typedHits.Count, damages.Sum(), totalDamage));
                    }
                    dict[Name] = extraDataList;
                }
            }
        }
    }
}
