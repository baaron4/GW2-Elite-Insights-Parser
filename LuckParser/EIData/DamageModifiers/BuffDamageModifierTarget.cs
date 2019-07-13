using LuckParser.Parser;
using LuckParser.Parser.ParsedData.CombatEvents;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.Statistics;

namespace LuckParser.EIData
{
    public class BuffDamageModifierTarget : BuffDamageModifier
    {

        public BuffDamageModifierTarget(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url, DamageLogChecker dlChecker = null) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, url, dlChecker)
        {
        }

        public BuffDamageModifierTarget(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url, ulong minBuild, ulong maxBuild, DamageLogChecker dlChecker = null) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, url, minBuild, maxBuild, dlChecker)
        {
        }

        public BuffDamageModifierTarget(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url, DamageLogChecker dlChecker = null) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, url, dlChecker)
        {
        }

        public BuffDamageModifierTarget(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url, ulong minBuild, ulong maxBuild, DamageLogChecker dlChecker = null) : base(ids, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, gainComputer, url, minBuild, maxBuild, dlChecker)
        {
        }


        public override void ComputeDamageModifier(Dictionary<string, List<DamageModifierData>> data, Dictionary<Target, Dictionary<string, List<DamageModifierData>>> dataTarget, Player p, ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            foreach (Target target in log.FightData.Logic.Targets)
            {
                Dictionary<long, BoonsGraphModel> bgms = target.GetBoonGraphs(log);
                if (!Tracker.Has(bgms))
                {
                    continue;
                }
                if (!dataTarget.TryGetValue(target, out var extra))
                {
                    dataTarget[target] = new Dictionary<string, List<DamageModifierData>>();
                }
                Dictionary<string, List<DamageModifierData>> dict = dataTarget[target];
                if (!dict.TryGetValue(Name, out var list))
                {
                    List<DamageModifierData> extraDataList = new List<DamageModifierData>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        int totalDamage = GetTotalDamage(p, log, target, i);
                        List<AbstractDamageEvent> typedHits = GetDamageLogs(p, log, target, phases[i]);
                        List<double> damages = typedHits.Select(x => ComputeGain(Tracker.GetStack(bgms, x.Time), x)).Where(x => x != -1.0).ToList();
                        extraDataList.Add(new DamageModifierData(damages.Count, typedHits.Count, damages.Sum(), totalDamage));
                    }
                    dict[Name] = extraDataList;
                }
            }
        }
    }
}
