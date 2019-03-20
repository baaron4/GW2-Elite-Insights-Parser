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

        public BuffDamageModifier(Boon buff, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer) : base(buff.Name, tooltip, damageSource, gainPerStack, srctype, compareType, src, buff.Link, gainComputer)
        {
            BuffsChecker = new BuffsTrackerSingle(buff);
        }

        public BuffDamageModifier(Boon buff, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, buff.Link, gainComputer)
        {
            BuffsChecker = new BuffsTrackerSingle(buff);
        }

        public BuffDamageModifier(Boon buff, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, url, gainComputer)
        {
            BuffsChecker = new BuffsTrackerSingle(buff);
        }

        public BuffDamageModifier(BuffsTrackerMulti buffsChecker, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string url) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, url, ByPresence)
        {
            BuffsChecker = buffsChecker;
        }

        public BuffDamageModifier(Boon buff, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, DamageLogChecker dlChecker) : base(buff.Name, tooltip, damageSource, gainPerStack, srctype, compareType, src, buff.Link, gainComputer, dlChecker)
        {
            BuffsChecker = new BuffsTrackerSingle(buff);
        }

        public BuffDamageModifier(Boon buff, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, GainComputer gainComputer, string url, DamageLogChecker dlChecker) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, url, gainComputer, dlChecker)
        {
            BuffsChecker = new BuffsTrackerSingle(buff);
        }

        public BuffDamageModifier(BuffsTrackerMulti buffsChecker, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string url, DamageLogChecker dlChecker) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, url, ByPresence, dlChecker)
        {
            BuffsChecker = buffsChecker;
        }

        protected double ComputeGain(int stack, DamageLog dl)
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
            if (!BuffsChecker.Has(bgms))
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
                        List<double> damages = typeHits.Select(x => ComputeGain(BuffsChecker.GetStack(bgms, x.Time), x)).Where(x => x != -1.0).ToList();
                        extraDataList.Add(new DamageModifierData(damages.Count, typeHits.Count, damages.Sum(), totalDamage));
                    }
                    dict[Name] = extraDataList;
                }
            }
            data[Name] = new List<DamageModifierData>();
            for (int i = 0; i < phases.Count; i++)
            {
                int totalDamage = GetTotalDamage(p, log, null, i);
                List<DamageLog> typeHits = GetDamageLogs(p, log, null, phases[i]);
                List<double> damages = typeHits.Select(x => ComputeGain(BuffsChecker.GetStack(bgms, x.Time), x)).Where(x => x != -1.0).ToList();
                data[Name].Add(new DamageModifierData(damages.Count, typeHits.Count, damages.Sum(), totalDamage));
            }
        }
    }
}
