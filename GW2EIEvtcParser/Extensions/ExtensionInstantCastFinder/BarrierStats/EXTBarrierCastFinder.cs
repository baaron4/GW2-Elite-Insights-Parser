using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

internal class EXTBarrierCastFinder : CheckedCastFinder<EXTBarrierEvent>
{

    private readonly long _damageSkillID;
    public EXTBarrierCastFinder(long skillID, long damageSkillID) : base(skillID)
    {
        UsingNotAccurate(true);
        UsingEnable((combatData) => combatData.HasEXTBarrier);
        _damageSkillID = damageSkillID;
    }

    public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
    {
        var res = new List<InstantCastEvent>();
        var heals = combatData.EXTBarrierCombatData.GetBarrierData(_damageSkillID).GroupBy(x => x.From);
        foreach (var group in heals)
        {
            var groupedHeals = group.ToList();
            long lastTime = int.MinValue;
            if (!HealingStatsExtensionHandler.SanitizeForSrc(groupedHeals))
            {
                continue;
            }
            foreach (EXTBarrierEvent be in groupedHeals)
            {
                if (be.Time - lastTime < ICD)
                {
                    lastTime = be.Time;
                    continue;
                }
                if (CheckCondition(be, combatData, agentData, skillData))
                {
                    lastTime = be.Time;
                    res.Add(new InstantCastEvent(GetTime(be, be.From, combatData), skillData.Get(SkillID), be.From));
                }
            }
        }
        return res;
    }
}
