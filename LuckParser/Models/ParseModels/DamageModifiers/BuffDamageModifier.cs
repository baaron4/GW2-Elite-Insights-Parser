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
        private readonly bool _targetOnly;
        public BuffDamageModifier(Boon buff, bool multiplier, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, bool targetOnly) : base(buff.ID, buff.Name, multiplier, gainPerStack, srctype, compareType, src, buff.Link)
        {
            _targetOnly = targetOnly;
        }

        private double ComputeGain(int stack)
        {
            return Multiplier ? (GainPerStack * stack / (100 * stack * GainPerStack)) / 100.0 : (stack > 0 ? 1 : 0);
        }

        public override void ComputeDamageModifier(Dictionary<long, List<ExtraBoonData>> data, Dictionary<Target, Dictionary<long, List<ExtraBoonData>>> dataTarget, Player p, ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            Dictionary<long, BoonsGraphModel> bgms = p.GetBoonGraphs(log);
            BoonsGraphModel bgm;
            if (!_targetOnly && !bgms.ContainsKey(ID))
            {
                return;
            }
            bgm = bgms[ID];
            foreach (Target target in log.FightData.Logic.Targets)
            {
                if (_targetOnly)
                {
                    bgms = target.GetBoonGraphs(log);
                    if (!bgms.ContainsKey(ID))
                    {
                        continue;
                    }
                    bgm = bgms[ID];
                }
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
                        int damage = (int)effect.Sum(x => Math.Round(ComputeGain(bgm.GetStackCount(x.Time)) * x.Damage));
                        extraDataList.Add(new ExtraBoonData(effect.Count, count, damage, totalDamage, Multiplier));
                    }
                    dict[ID] = extraDataList;
                }
            }
            if (_targetOnly)
            {
                return;
            }
            data[ID] = new List<ExtraBoonData>();
            for (int i = 0; i < phases.Count; i++)
            {
                (int totalDamage, int count) = GetTotalDamageData(p, log, null, phases[i]);
                List<DamageLog> effect = GetDamageLogs(p, log, null, phases[i]);
                int damage = (int)effect.Sum(x => Math.Round(ComputeGain(bgm.GetStackCount(x.Time)) * x.Damage));
                data[ID].Add(new ExtraBoonData(effect.Count, count, damage, totalDamage, Multiplier));
            }
        }
    }
}
