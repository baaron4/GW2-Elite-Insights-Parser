using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffLossCastFinder : InstantCastFinder
    {

        public delegate bool BuffLossCastChecker(BuffRemoveAllEvent evt, CombatData combatData);
        private readonly BuffLossCastChecker _triggerCondition;


        private readonly long _buffID;
        public BuffLossCastFinder(long skillID, long buffID, long icd, BuffLossCastChecker checker = null) : base(skillID, icd)
        {
            _triggerCondition = checker;
            _buffID = buffID;
        }

        public BuffLossCastFinder(long skillID, long buffID, long icd, ulong minBuild, ulong maxBuild, BuffLossCastChecker checker = null) : base(skillID, icd, minBuild, maxBuild)
        {
            _triggerCondition = checker;
            _buffID = buffID;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            var removals = combatData.GetBuffData(_buffID).OfType<BuffRemoveAllEvent>().GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<BuffRemoveAllEvent>> pair in removals)
            {
                long lastTime = long.MinValue;
                foreach (BuffRemoveAllEvent brae in pair.Value)
                {
                    if (brae.Time - lastTime < ICD)
                    {
                        lastTime = brae.Time;
                        continue;
                    }
                    if (_triggerCondition != null)
                    {
                        if (_triggerCondition(brae, combatData))
                        {
                            lastTime = brae.Time;
                            res.Add(new InstantCastEvent(brae.Time, skillData.Get(SkillID), brae.To));
                        }
                    }
                    else
                    {
                        lastTime = brae.Time;
                        res.Add(new InstantCastEvent(brae.Time, skillData.Get(SkillID), brae.To));
                    }
                }
            }
            return res;
        }
    }
}
