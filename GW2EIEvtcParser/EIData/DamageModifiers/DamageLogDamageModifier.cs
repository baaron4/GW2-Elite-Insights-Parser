using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class DamageLogDamageModifier : DamageModifier
    {

        public DamageLogDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageLogChecker checker, GainComputer gainComputer, ulong minBuild, ulong maxBuild, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, checker, minBuild, maxBuild, mode)
        {
        }

        public DamageLogDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageLogChecker checker, GainComputer gainComputer, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, checker, ulong.MinValue, ulong.MaxValue, mode)
        {
        }

        public override void ComputeDamageModifier(Dictionary<string, List<DamageModifierStat>> data, Dictionary<NPC, Dictionary<string, List<DamageModifierStat>>> dataTarget, Player p, ParsedEvtcLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            double gain = GainComputer.ComputeGain(GainPerStack, 1);
            if (!p.GetDamageLogs(null, log, phases[0]).Exists(x => DLChecker(x)))
            {
                return;
            }
            foreach (NPC target in log.FightData.Logic.Targets)
            {
                if (!dataTarget.TryGetValue(target, out Dictionary<string, List<DamageModifierStat>> extra))
                {
                    dataTarget[target] = new Dictionary<string, List<DamageModifierStat>>();
                }
                Dictionary<string, List<DamageModifierStat>> dict = dataTarget[target];
                if (!dict.TryGetValue(Name, out List<DamageModifierStat> list))
                {
                    var extraDataList = new List<DamageModifierStat>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        int totalDamage = GetTotalDamage(p, log, target, i);
                        List<AbstractDamageEvent> typeHits = GetDamageLogs(p, log, target, phases[i]);
                        var effect = typeHits.Where(x => DLChecker(x)).ToList();
                        extraDataList.Add(new DamageModifierStat(effect.Count, typeHits.Count, gain * effect.Sum(x => x.Damage), totalDamage));
                    }
                    dict[Name] = extraDataList;
                }
            }
            data[Name] = new List<DamageModifierStat>();
            for (int i = 0; i < phases.Count; i++)
            {
                int totalDamage = GetTotalDamage(p, log, null, i);
                List<AbstractDamageEvent> typeHits = GetDamageLogs(p, log, null, phases[i]);
                var effect = typeHits.Where(x => DLChecker(x)).ToList();
                data[Name].Add(new DamageModifierStat(effect.Count, typeHits.Count, gain * effect.Sum(x => x.Damage), totalDamage));
            }
        }
    }
}
