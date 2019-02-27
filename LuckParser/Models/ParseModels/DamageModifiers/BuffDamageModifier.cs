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
        protected BuffsTracker BuffsChecker;

        public BuffDamageModifier(Boon buff, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer) : base(buff.Name, withPets, gainPerStack, srctype, compareType, src, buff.Link, gainComputer)
        {
            BuffsChecker = new BuffsTrackerSingle(buff);
        }

        public BuffDamageModifier(Boon buff, string name, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url) : base(name, withPets, gainPerStack, srctype, compareType, src, url, gainComputer)
        {
            BuffsChecker = new BuffsTrackerSingle(buff);
        }

        public BuffDamageModifier(BuffsTrackerMulti buffsChecker, string name, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string url) : base(name, withPets, gainPerStack, srctype, compareType, src, url, ByPresence)
        {
            BuffsChecker = buffsChecker;
        }

        public BuffDamageModifier(Boon buff, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, DamageLogChecker dlChecker) : base(buff.Name, withPets, gainPerStack, srctype, compareType, src, buff.Link, gainComputer, dlChecker)
        {
            BuffsChecker = new BuffsTrackerSingle(buff);
        }

        public BuffDamageModifier(Boon buff, string name, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url, DamageLogChecker dlChecker) : base(name, withPets, gainPerStack, srctype, compareType, src, url, gainComputer, dlChecker)
        {
            BuffsChecker = new BuffsTrackerSingle(buff);
        }

        public BuffDamageModifier(BuffsTrackerMulti buffsChecker, string name, bool withPets, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string url, DamageLogChecker dlChecker) : base(name, withPets, gainPerStack, srctype, compareType, src, url, ByPresence, dlChecker)
        {
            BuffsChecker = buffsChecker;
        }

        protected double ComputeGain(int stack, DamageLog dl)
        {
            if (DLChecker != null && !DLChecker(dl))
            {
                return 0.0;
            }
            return GainComputer.ComputeGain(GainPerStack, stack);
        }

        public override void ComputeDamageModifier(Dictionary<string, List<ExtraBoonData>> data, Dictionary<Target, Dictionary<string, List<ExtraBoonData>>> dataTarget, Player p, ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            Dictionary<long, BoonsGraphModel> bgms = p.GetBoonGraphs(log);
            if (!BuffsChecker.Has(bgms))
            {
                return;
            }
            foreach (Target target in log.FightData.Logic.Targets)
            {
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
            data[Name] = new List<ExtraBoonData>();
            for (int i = 0; i < phases.Count; i++)
            {
                (int totalDamage, int count) = GetTotalDamageData(p, log, null, phases[i]);
                List<DamageLog> effect = GetDamageLogs(p, log, null, phases[i]);
                int damage = (int)effect.Sum(x => Math.Round(ComputeGain(BuffsChecker.GetStack(bgms, x.Time), x) * x.Damage));
                data[Name].Add(new ExtraBoonData(effect.Count, count, damage, totalDamage, GainComputer.Multiplier));
            }
        }
    }
}
