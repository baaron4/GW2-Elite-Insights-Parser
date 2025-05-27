using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class MissileCastFinder : CheckedCastFinder<MissileEvent>
{
    private readonly long _missileSkillID;
    public MissileCastFinder(long skillID, long missileSkillID) : base(skillID)
    {
        _missileSkillID = missileSkillID;
    }

    public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
    {
        var res = new List<InstantCastEvent>();
        var missiles = combatData.GetMissileEventsBySkillID(_missileSkillID).GroupBy(x => x.Src);
        foreach (var group in missiles)
        {
            long lastTime = int.MinValue;
            foreach (MissileEvent missileEvent in group)
            {
                if (CheckCondition(missileEvent, combatData, agentData, skillData))
                {
                    if (missileEvent.Time - lastTime < ICD)
                    {
                        lastTime = missileEvent.Time;
                        continue;
                    }
                    lastTime = missileEvent.Time;
                    res.Add(new InstantCastEvent(GetTime(missileEvent, missileEvent.Src, combatData), skillData.Get(SkillID), missileEvent.Src));
                }
            }
        }
        return res;
    }
}
