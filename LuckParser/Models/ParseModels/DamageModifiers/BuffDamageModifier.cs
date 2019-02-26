using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.Statistics;

namespace LuckParser.Models.ParseModels
{
    public class BuffDamageModifier : DamageModifier
    {

        public BuffDamageModifier(Boon buff, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer) : base(buff.ID, buff.Name, withPets, gainPerStack, srctype, compareType, src, buff.Link,gainComputer)
        {
        }

        public BuffDamageModifier(long id, string name, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url) : base(id, name, withPets, gainPerStack, srctype, compareType, src, url, gainComputer)
        {
        }

        public BuffDamageModifier(Boon buff, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, DamageLogChecker checker) : base(buff.ID, buff.Name, withPets, gainPerStack, srctype, compareType, src, buff.Link, gainComputer)
        {
            Checker = checker;
        }

        public BuffDamageModifier(long id, string name, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url, DamageLogChecker checker) : base(id, name, withPets, gainPerStack, srctype, compareType, src, url, gainComputer)
        {
            Checker = checker;
        }

        protected double ComputeGain(int stack, DamageLog dl)
        {
            if (Checker != null && !Checker(dl))
            {
                return 0.0;
            }
            return GainComputer.ComputeGain(GainPerStack, stack);
        }

        public override void ComputeDamageModifier(Dictionary<long, List<ExtraBoonData>> data, Dictionary<Target, Dictionary<long, List<ExtraBoonData>>> dataTarget, Player p, ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            Dictionary<long, BoonsGraphModel> bgms = p.GetBoonGraphs(log);
            BoonsGraphModel bgm;
            if (!bgms.ContainsKey(ID))
            {
                return;
            }
            bgm = bgms[ID];
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
                        List<DamageLog> effect = GetDamageLogs(p, log, target, phases[i]);
                        int damage = (int)effect.Sum(x => Math.Round(ComputeGain(bgm.GetStackCount(x.Time), x) * x.Damage));
                        extraDataList.Add(new ExtraBoonData(effect.Count, count, damage, totalDamage, GainComputer.Multiplier));
                    }
                    dict[ID] = extraDataList;
                }
            }
            data[ID] = new List<ExtraBoonData>();
            for (int i = 0; i < phases.Count; i++)
            {
                (int totalDamage, int count) = GetTotalDamageData(p, log, null, phases[i]);
                List<DamageLog> effect = GetDamageLogs(p, log, null, phases[i]);
                int damage = (int)effect.Sum(x => Math.Round(ComputeGain(bgm.GetStackCount(x.Time), x) * x.Damage));
                data[ID].Add(new ExtraBoonData(effect.Count, count, damage, totalDamage, GainComputer.Multiplier));
            }
        }
    }
}
