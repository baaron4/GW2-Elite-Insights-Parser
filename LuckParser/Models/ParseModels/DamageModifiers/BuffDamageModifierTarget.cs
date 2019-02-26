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

        public BuffDamageModifierTarget(Boon buff, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer) : base(buff, withPets, gainPerStack, srctype, compareType, src, gainComputer)
        {
        }

        public BuffDamageModifierTarget(long id, string name, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url) : base(id, name, withPets, gainPerStack, srctype, compareType, src, gainComputer, url)
        {
        }

        public BuffDamageModifierTarget(Boon buff, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, DamageLogChecker checker) : base(buff, withPets, gainPerStack, srctype, compareType, src, gainComputer, checker)
        {
        }

        public BuffDamageModifierTarget(long id, string name, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url, DamageLogChecker checker) : base(id, name, withPets, gainPerStack, srctype, compareType, src, gainComputer, url, checker)
        {
        }

        public override void ComputeDamageModifier(Dictionary<string, List<ExtraBoonData>> data, Dictionary<Target, Dictionary<string, List<ExtraBoonData>>> dataTarget, Player p, ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            Dictionary<long, BoonsGraphModel> bgms = p.GetBoonGraphs(log);
            BoonsGraphModel bgm;
            foreach (Target target in log.FightData.Logic.Targets)
            {
                bgms = target.GetBoonGraphs(log);
                if (!bgms.ContainsKey(ID))
                {
                    continue;
                }
                bgm = bgms[ID];
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
                        int damage = (int)effect.Sum(x => Math.Round(ComputeGain(bgm.GetStackCount(x.Time), x) * x.Damage));
                        extraDataList.Add(new ExtraBoonData(effect.Count, count, damage, totalDamage, GainComputer.Multiplier));
                    }
                    dict[Name] = extraDataList;
                }
            }
        }
    }
}
