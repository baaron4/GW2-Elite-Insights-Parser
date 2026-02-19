using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class AchievementEligibilityMechanic : IDBasedMechanic<AchievementEligibilityEvent>
{
     public AchievementEligibilityMechanic(long achievementID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base([achievementID], plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        IsEnemyMechanic = false;
    }
    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long achievementID in MechanicIDs)
        {
            foreach (var player in log.PlayerList)
            {
                foreach (var evt in player.GetAchievementEligibilityEvents(log))
                {
                    if (evt.AchievementID == achievementID && Keep(evt, log))
                    {
                        InsertMechanic(log, mechanicLogs, evt.Time, player, 1);
                    }
                }
            }
        }
    }

}
