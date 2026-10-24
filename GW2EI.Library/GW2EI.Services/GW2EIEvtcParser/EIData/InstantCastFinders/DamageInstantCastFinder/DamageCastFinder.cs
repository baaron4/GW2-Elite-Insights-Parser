using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class DamageCastFinder : CheckedCastFinder<HealthDamageEvent>
{
    protected bool Target = false;
    private readonly long _damageSkillID;

    public DamageCastFinder(long skillID, long damageSkillID) : base(skillID)
    {
        UsingNotAccurate();
        _damageSkillID = damageSkillID;
    }

    public DamageCastFinder WithTarget()
    {
        Target = true;
        return this;
    }

    public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
    {
        var res = new List<InstantCastEvent>();
        var damages = combatData.GetDamageData(_damageSkillID).GroupBy(x => x.From);
        foreach (var group in damages)
        {
            long lastTime = int.MinValue;
            foreach (HealthDamageEvent de in group)
            {
                if (CheckCondition(de, combatData, agentData, skillData))
                {
                    if (de.Time - lastTime < ICD)
                    {
                        lastTime = de.Time;
                        continue;
                    }
                    lastTime = de.Time;
                    res.Add(new InstantCastEvent(GetTime(de, de.From, combatData), skillData.Get(SkillID), de.From, Target ? de.To : ParserHelper._unknownAgent));
                }
            }
        }
        return res;
    }
}
