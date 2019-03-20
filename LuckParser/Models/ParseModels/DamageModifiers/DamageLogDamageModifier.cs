using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.Statistics;

namespace LuckParser.Models.ParseModels
{
    public class DamageLogDamageModifier : DamageModifier
    {

        public DamageLogDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string url, DamageLogChecker checker, GainComputer gainComputer) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, url, gainComputer)
        {
            DLChecker = checker;
        }

        public DamageLogDamageModifier(Boon boon, string tooltip, long id, DamageSource damageSource, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer) : base(boon.Name, tooltip, damageSource, double.PositiveInfinity, srctype, compareType, src, boon.Link, gainComputer)
        {
            DLChecker = (dl => dl.SkillId == id);
        }

        public override void ComputeDamageModifier(Dictionary<string, List<DamageModifierData>> data, Dictionary<Target, Dictionary<string, List<DamageModifierData>>> dataTarget, Player p, ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            double gain = double.IsPositiveInfinity(GainPerStack) ? 1.0 : GainComputer.ComputeGain(GainPerStack, 1);
            if (!p.GetDamageLogs(null, log, phases[0]).Exists(x => DLChecker(x)))
            {
                return;
            }
            foreach (Target target in log.FightData.Logic.Targets)
            {
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
                        List<DamageLog> typeHits = GetDamageLogs(p, log, target, phases[i]);
                        List<DamageLog> effect = typeHits.Where(x => DLChecker(x)).ToList();
                        extraDataList.Add(new DamageModifierData(effect.Count, typeHits.Count, gain * effect.Sum(x => x.Damage), totalDamage));
                    }
                    dict[Name] = extraDataList;
                }
            }
            data[Name] = new List<DamageModifierData>();
            for (int i = 0; i < phases.Count; i++)
            {
                int totalDamage = GetTotalDamage(p, log, null, i);
                List<DamageLog> typeHits = GetDamageLogs(p, log, null, phases[i]);
                List<DamageLog> effect = typeHits.Where(x => DLChecker(x)).ToList();
                data[Name].Add(new DamageModifierData(effect.Count, typeHits.Count, gain * effect.Sum(x => x.Damage), totalDamage));
            }
        }
    }
}
