using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public class AchievementEligibilityEvent : TimeCombatEvent
{
    public readonly long AchievementID;
    public readonly SingleActor Actor;
    public readonly bool Lost;

    internal AchievementEligibilityEvent(long time, long achievementID, SingleActor actor, bool lost) : base(time)
    {
        Actor = actor;
        AchievementID = achievementID;
        Lost = lost;
    }
}
