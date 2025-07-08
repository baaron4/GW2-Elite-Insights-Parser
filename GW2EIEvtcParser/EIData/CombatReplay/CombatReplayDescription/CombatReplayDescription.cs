namespace GW2EIEvtcParser.EIData;

public abstract class CombatReplayDescription
{
    public Types Type { get; protected set; }

    /// <summary>
    /// Define the type of the decoration. Must match ordering of the object in animator.js
    /// </summary>
    public enum Types : byte
    {
        ActorOrientation = 0,
        BackgroundIcon = 1,
        Circle = 2,
        Doughnut = 3,
        Friendly = 4,
        FriendlyPlayer = 5,
        Icon = 6,
        IconOverhead = 7,
        Line = 8,
        Mob = 9,
        MovingPlatform = 10,
        Pie = 11,
        Player = 12,
        ProgressBar = 13,
        ProgressBarOverhead = 14,
        Rectangle = 15,
        SquadMarker = 16,
        SquadMarkerOverhead = 17,
        Target = 18,
        TargetPlayer = 19,
        Text = 20,
        Polygon = 21
    }

    protected CombatReplayDescription()
    {
    }
}
