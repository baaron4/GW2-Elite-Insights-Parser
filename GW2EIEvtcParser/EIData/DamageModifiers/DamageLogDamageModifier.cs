using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class DamageLogDamageModifier : DamageModifier
    {

        internal DamageLogDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageLogChecker checker, GainComputer gainComputer, ulong minBuild, ulong maxBuild, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, checker, minBuild, maxBuild, mode)
        {
        }

        internal DamageLogDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageLogChecker checker, GainComputer gainComputer, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, checker, ulong.MinValue, ulong.MaxValue, mode)
        {
        }

        internal override void ComputeDamageModifier(Dictionary<string, List<DamageModifierStat>> data, Dictionary<NPC, Dictionary<string, List<DamageModifierStat>>> dataTarget, Player p, ParsedEvtcLog log)
        {
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            double gain = GainComputer.ComputeGain(GainPerStack, 1);
            if (!p.GetHitDamageEvents(null, log, phases[0].Start, phases[0].End).Exists(x => DLChecker(x)))
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
                    foreach (PhaseData phase in phases)
                    {
                        int totalDamage = GetTotalDamage(p, log, target, phase.Start, phase.End);
                        List<AbstractHealthDamageEvent> typeHits = GetHitDamageLogs(p, log, target, phase.Start, phase.End);
                        var effect = typeHits.Where(x => DLChecker(x)).ToList();
                        extraDataList.Add(new DamageModifierStat(effect.Count, typeHits.Count, gain * effect.Sum(x => x.HealthDamage), totalDamage));
                    }
                    dict[Name] = extraDataList;
                }
            }
            data[Name] = new List<DamageModifierStat>();
            foreach (PhaseData phase in phases)
            {
                int totalDamage = GetTotalDamage(p, log, null, phase.Start, phase.End);
                List<AbstractHealthDamageEvent> typeHits = GetHitDamageLogs(p, log, null, phase.Start, phase.End);
                var effect = typeHits.Where(x => DLChecker(x)).ToList();
                data[Name].Add(new DamageModifierStat(effect.Count, typeHits.Count, gain * effect.Sum(x => x.HealthDamage), totalDamage));
            }
        }
    }
}
