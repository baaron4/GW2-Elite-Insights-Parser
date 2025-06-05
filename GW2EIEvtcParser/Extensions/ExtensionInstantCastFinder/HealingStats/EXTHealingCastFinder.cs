using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

internal class EXTHealingCastFinder : CheckedCastFinder<EXTHealingEvent>
{

    private readonly long _damageSkillID;
    public EXTHealingCastFinder(long skillID, long damageSkillID) : base(skillID)
    {
        UsingNotAccurate();
        UsingEnable((combatData) => combatData.HasEXTHealing);
        _damageSkillID = damageSkillID;
    }

    public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
    {
        var res = new List<InstantCastEvent>();
        var heals = combatData.EXTHealingCombatData.GetHealData(_damageSkillID).GroupBy(x => x.From);
        foreach (var group in heals)
        {
            var groupedHeals = group.ToList();
            long lastTime = int.MinValue;
            if (!HealingStatsExtensionHandler.SanitizeForSrc(groupedHeals))
            {
                continue;
            }
            foreach (EXTHealingEvent he in groupedHeals)
            {
                if (he.Time - lastTime < ICD)
                {
                    lastTime = he.Time;
                    continue;
                }
                if (CheckCondition(he, combatData, agentData, skillData))
                {
                    lastTime = he.Time;
                    res.Add(new InstantCastEvent(GetTime(he, he.From, combatData), skillData.Get(SkillID), he.From));
                }
            }
        }
        return res;
    }
}
