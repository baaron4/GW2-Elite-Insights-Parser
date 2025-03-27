using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerDstNoSkillMechanic : PlayerSkillMechanic
{

    public PlayerDstNoSkillMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public PlayerDstNoSkillMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }
    protected override AgentItem GetAgentItem(HealthDamageEvent ahde)
    {
        return ahde.To;
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        var allPlayers = new HashSet<SingleActor>(log.PlayerList);
        var regroupedSkillDst = new Dictionary<long, HashSet<SingleActor>>();
        foreach (long skillID in MechanicIDs)
        {
            long lastTime = int.MinValue;
            foreach (HealthDamageEvent ahde in log.CombatData.GetDamageData(skillID))
            {
                if (!TryGetActor(log, GetCreditedAgentItem(ahde), regroupedMobs, out var amp) || !Keep(ahde, log))
                {
                    continue;
                }
                long time = ahde.Time;
                if (Math.Abs(ahde.Time - lastTime) < ParserHelper.ServerDelayConstant)
                {
                    time = lastTime;
                }
                if (regroupedSkillDst.TryGetValue(time, out var actorSet))
                {
                    actorSet.Add(amp);
                }
                else
                {
                    regroupedSkillDst[time] = [amp];
                }

                lastTime = ahde.Time;
            }
        }
        var regroupedNeverSkillDst = new Dictionary<long, HashSet<SingleActor>>();
        foreach (KeyValuePair<long, HashSet<SingleActor>> pair in regroupedSkillDst)
        {
            regroupedNeverSkillDst[pair.Key] = [];
            foreach (SingleActor p in allPlayers.Except(pair.Value))
            {
                if (!regroupedSkillDst.Any(x => x.Value != pair.Value && x.Value.Contains(p)))
                {
                    regroupedNeverSkillDst[pair.Key].Add(p);
                }
            }
        }
        foreach (KeyValuePair<long, HashSet<SingleActor>> pair in regroupedNeverSkillDst)
        {
            foreach (SingleActor p in pair.Value)
            {
                InsertMechanic(log, mechanicLogs, pair.Key, p);
            }
        }
    }
}
