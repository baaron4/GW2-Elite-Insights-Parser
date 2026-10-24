using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class MissileCastFinder : CheckedCastFinder<MissileEvent>
{
    protected bool Minions = false;
    protected bool Target = false;
    private readonly long _missileSkillID;
    public MissileCastFinder(long skillID, long missileSkillID) : base(skillID)
    {
        _missileSkillID = missileSkillID;
    }

    public MissileCastFinder WithMinions()
    {
        Minions = true;
        return this;
    }

    public MissileCastFinder WithTarget()
    {
        Target = true;
        return this;
    }

    protected AgentItem GetAgent(MissileEvent missileEvent)
    {
        return Minions ? missileEvent.Src.GetFinalMaster() : missileEvent.Src;
    }

    public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
    {
        var res = new List<InstantCastEvent>();
        var missiles = combatData.GetMissileEventsBySkillID(_missileSkillID).GroupBy(GetAgent);
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
                    var caster = group.Key;
                    var instant = new InstantCastEvent(GetTime(missileEvent, missileEvent.Src, combatData), skillData.Get(SkillID), caster, Target ? missileEvent.GetInitialTarget() : ParserHelper._unknownAgent);
                    res.Add(instant);
                }
            }
        }
        return res;
    }
}
