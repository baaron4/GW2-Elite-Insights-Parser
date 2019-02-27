using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.Statistics;

namespace LuckParser.Models.ParseModels
{
    public class BuffDamageModifierTarget : BuffDamageModifier
    {

        public BuffDamageModifierTarget(Boon buff, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer) : base(buff, damageSource, gainPerStack, srctype, compareType, src, gainComputer)
        {
        }

        public BuffDamageModifierTarget(Boon buff, string name, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url) : base(buff, name, damageSource, gainPerStack, srctype, compareType, src, gainComputer, url)
        {
        }

        public BuffDamageModifierTarget(BuffsTrackerMulti buffsChecker, string name, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string url) : base(buffsChecker, name, damageSource, gainPerStack, srctype, compareType, src, url)
        {
        }

        public BuffDamageModifierTarget(Boon buff, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, DamageLogChecker dlChecker) : base(buff, damageSource, gainPerStack, srctype, compareType, src, gainComputer, dlChecker)
        {
        }

        public BuffDamageModifierTarget(Boon buff, string name, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url, DamageLogChecker dlChecker) : base(buff, name, damageSource, gainPerStack, srctype, compareType, src, gainComputer, url, dlChecker)
        {
        }

        public BuffDamageModifierTarget(BuffsTrackerMulti buffsChecker, string name, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string url, DamageLogChecker dlChecker) : base(buffsChecker, name, damageSource, gainPerStack, srctype, compareType, src, url, dlChecker)
        {
        }

        public override void ComputeDamageModifier(Dictionary<string, List<ExtraBoonData>> data, Dictionary<Target, Dictionary<string, List<ExtraBoonData>>> dataTarget, Player p, ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            foreach (Target target in log.FightData.Logic.Targets)
            {
                Dictionary<long, BoonsGraphModel> bgms = target.GetBoonGraphs(log);
                if (!BuffsChecker.Has(bgms))
                {
                    continue;
                }
                if (!dataTarget.TryGetValue(target, out var extra))
                {
                    dataTarget[target] = new Dictionary<string, List<ExtraBoonData>>();
                }
                Dictionary<string, List<ExtraBoonData>> dict = dataTarget[target];
                if (!dict.TryGetValue(Name, out var list))
                {
                    List<ExtraBoonData> extraDataList = new List<ExtraBoonData>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        (int totalDamage, int count) = GetTotalDamageData(p, log, target, phases[i]);
                        List<DamageLog> effect = GetDamageLogs(p, log, target, phases[i]);
                        int damage = (int)effect.Sum(x => Math.Round(ComputeGain(BuffsChecker.GetStack(bgms, x.Time), x) * x.Damage));
                        extraDataList.Add(new ExtraBoonData(effect.Count, count, damage, totalDamage, GainComputer.Multiplier));
                    }
                    dict[Name] = extraDataList;
                }
            }
        }
    }
}
