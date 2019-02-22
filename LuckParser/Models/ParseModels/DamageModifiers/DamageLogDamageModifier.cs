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
        public delegate bool DamageLogChecker(DamageLog dl);
        private readonly DamageLogChecker _checker;

        public DamageLogDamageModifier(long id, string name, bool multiplier, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string url, DamageLogChecker checker) : base(id, name, multiplier, gainPerStack, srctype, compareType, src, url)
        {
            _checker = checker;
        }
        
        public DamageLogDamageModifier(long id, string name, bool multiplier, double gainPerStack, string url, DamageLogChecker checker) : base(id, name, multiplier, gainPerStack, DamageType.Power, DamageType.Power, ModifierSource.All, url)
        {
            _checker = checker;
        }

        public override void ComputeDamageModifier(Dictionary<long, List<ExtraBoonData>> data, Dictionary<Target, Dictionary<long, List<ExtraBoonData>>> dataTarget, Player p, ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            double gain = (GainPerStack / (100 + GainPerStack)) / 100.0;
            foreach (Target target in log.FightData.Logic.Targets)
            {
                if (!dataTarget.TryGetValue(target, out var extra))
                {
                    dataTarget[target] = new Dictionary<long, List<ExtraBoonData>>();
                }
                Dictionary<long, List<ExtraBoonData>> dict = dataTarget[target];
                if (!dict.TryGetValue(ID, out var list))
                {
                    List<ExtraBoonData> extraDataList = new List<ExtraBoonData>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        (int totalDamage, int count) = GetTotalDamageData(p, log, target, phases[i]);
                        List<DamageLog> effect = GetDamageLogs(p, log, target, phases[i]).Where(x => _checker(x)).ToList();
                        int damage = (int)Math.Round(gain * effect.Sum(x => x.Damage));
                        extraDataList.Add(new ExtraBoonData(effect.Count, count, damage, totalDamage, true));
                    }
                    dict[ID] = extraDataList;
                }
            }
            data[ID] = new List<ExtraBoonData>();
            for (int i = 0; i < phases.Count; i++)
            {
                (int totalDamage, int count) = GetTotalDamageData(p, log, null, phases[i]);
                List<DamageLog> effect = GetDamageLogs(p, log, null, phases[i]).Where(x => _checker(x)).ToList();
                int damage = (int)Math.Round(gain * effect.Sum(x => x.Damage));
                data[ID].Add(new ExtraBoonData(effect.Count, count, damage, totalDamage, true));
            }
        }
    }
}
