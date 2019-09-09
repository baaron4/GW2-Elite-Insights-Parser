using System.Collections.Generic;
using System.Linq;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData.CombatEvents;
using static LuckParser.Models.Statistics;

namespace LuckParser.EIData
{
    public class BuffDamageModifier : DamageModifier
    {

        protected BuffsTracker Tracker { get; }

        public BuffDamageModifier(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string icon, DamageLogChecker dlChecker = null) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, dlChecker, ulong.MinValue, ulong.MaxValue)
        {
            Tracker = new BuffsTrackerSingle(id);
        }

        public BuffDamageModifier(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string icon, ulong minBuild, ulong maxBuild, DamageLogChecker dlChecker = null) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, dlChecker, minBuild, maxBuild)
        {
            Tracker = new BuffsTrackerSingle(id);
        }

        public BuffDamageModifier(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string icon, DamageLogChecker dlChecker = null) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, dlChecker, ulong.MinValue, ulong.MaxValue)
        {
            Tracker = new BuffsTrackerMulti(new List<long>(ids));
        }

        public BuffDamageModifier(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string icon, ulong minBuild, ulong maxBuild, DamageLogChecker dlChecker = null) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, dlChecker, minBuild, maxBuild)
        {
            Tracker = new BuffsTrackerMulti(new List<long>(ids));
        }

        protected double ComputeGain(int stack, AbstractDamageEvent dl)
        {
            if (DLChecker != null && !DLChecker(dl))
            {
                return -1.0;
            }
            double gain = GainComputer.ComputeGain(GainPerStack, stack);
            return gain > 0.0 ? gain * dl.Damage : -1.0;
        }

        public override void ComputeDamageModifier(Dictionary<string, List<DamageModifierData>> data, Dictionary<Target, Dictionary<string, List<DamageModifierData>>> dataTarget, Player p, ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            Dictionary<long, BoonsGraphModel> bgms = p.GetBoonGraphs(log);
            if (!Tracker.Has(bgms) && GainComputer != ByAbsence)
            {
                return;
            }
            foreach (Target target in log.FightData.Logic.Targets)
            {
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
                        List<AbstractDamageEvent> typeHits = GetDamageLogs(p, log, target, phases[i]);
                        var damages = typeHits.Select(x => ComputeGain(Tracker.GetStack(bgms, x.Time), x)).Where(x => x != -1.0).ToList();
                        extraDataList.Add(new DamageModifierData(damages.Count, typeHits.Count, damages.Sum(), totalDamage));
                    }
                    dict[Name] = extraDataList;
                }
            }
            data[Name] = new List<DamageModifierData>();
            for (int i = 0; i < phases.Count; i++)
            {
                int totalDamage = GetTotalDamage(p, log, null, i);
                List<AbstractDamageEvent> typeHits = GetDamageLogs(p, log, null, phases[i]);
                var damages = typeHits.Select(x => ComputeGain(Tracker.GetStack(bgms, x.Time), x)).Where(x => x != -1.0).ToList();
                data[Name].Add(new DamageModifierData(damages.Count, typeHits.Count, damages.Sum(), totalDamage));
            }
        }
    }
}
