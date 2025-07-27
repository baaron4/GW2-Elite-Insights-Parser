using System.Text.Json.Serialization;

namespace GW2EIEvtcParser.EIData;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(NPCCombatReplayDescription))]
[JsonDerivedType(typeof(PlayerCombatReplayDescription))]
public abstract class SingleActorCombatReplayDescription : CombatReplayDescription
{
    public long Start { get; protected set; }
    public long End { get; protected set; }
    public readonly string Img;
    public readonly int ID;
    public readonly int ParentID;
    public readonly IReadOnlyList<float> Positions;
    public readonly IReadOnlyList<float> Angles;
    public IReadOnlyList<long>? Dead { get; private set; }
    public IReadOnlyList<long>? Down { get; private set; }
    public IReadOnlyList<long>? Dc { get; private set; }
    public readonly IReadOnlyList<long>? Hide;
    public IReadOnlyList<long>? BreakbarActive { get; private set; }

    public readonly long HitboxWidth;

    private static Types GetActorType(SingleActor actor, ParsedEvtcLog log)
    {
        if (actor.AgentItem.IsPlayer)
        {
            if (log.PlayerAgents.Contains(actor.AgentItem))
            {
                return Types.Player;
            }
            if (log.FightData.Logic.NonSquadFriendlyAgents.Contains(actor.AgentItem))
            {
                return Types.FriendlyPlayer;
            }
            return Types.TargetPlayer;
        }
        if (log.FightData.Logic.TargetAgents.Contains(actor.AgentItem))
        {
            return Types.Target;
        }
        if (log.FightData.Logic.NonSquadFriendlyAgents.Contains(actor.AgentItem) || actor.AgentItem.GetFinalMaster().Type == ParsedData.AgentItem.AgentType.Player)
        {
            return Types.Friendly;
        }
        return Types.Mob;
    }

    internal SingleActorCombatReplayDescription(SingleActor actor, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay)
    {
        Start = replay.TimeOffsets.start;
        End = replay.TimeOffsets.end;
        Img = actor.GetIcon(true);
        ID = actor.UniqueID;
        ParentID = actor.EnglobingAgentItem == actor.AgentItem ? -1 : actor.EnglobingAgentItem.UniqueID;
        Type = GetActorType(actor, log);
        HitboxWidth = actor.AgentItem.HitboxWidth;

        var positions = new List<float>(replay.PolledPositions.Count * 2);
        foreach (var pos in replay.PolledPositions)
        {
            (float x, float y) = map.GetMapCoordRounded(pos.XYZ.XY());
            positions.Add(x);
            positions.Add(y);
        }
        Positions = positions;
        
        var angles = new List<float>(replay.PolledRotations.Count);
        foreach (var facing in replay.PolledRotations)
        {
            angles.Add(-facing.XYZ.GetRoundedZRotationDeg());
        }
        Angles = angles;

        if (replay.Hidden.Count != 0)
        {
            var hide = new List<long>(replay.Hidden.Count * 2);
            foreach (Segment seg in replay.Hidden)
            {
                hide.Add(seg.Start);
                hide.Add(seg.End);
            }
            Hide = hide;
        }
        SetStatus(log, actor);
        SetBreakbarStatus(log, actor);
    }

    protected void SetStatus(ParsedEvtcLog log, SingleActor a)
    {
        var dead = new List<long>();
        Dead = dead;
        var down = new List<long>();
        Down = down;
        var dc = new List<long>();
        Dc = dc;
        (IReadOnlyList<Segment> deads, IReadOnlyList<Segment> downs, IReadOnlyList<Segment> dcs, _) = a.GetStatus(log);

        foreach (Segment seg in deads)
        {
            dead.Add(seg.Start);
            dead.Add(seg.End);
        }
        foreach (Segment seg in downs)
        {
            down.Add(seg.Start);
            down.Add(seg.End);
        }
        foreach (Segment seg in dcs)
        {
            dc.Add(seg.Start);
            dc.Add(seg.End);
        }
    }

    protected void SetBreakbarStatus(ParsedEvtcLog log, SingleActor a)
    {
        var active = new List<long>();
        BreakbarActive = active;
        (_, IReadOnlyList<Segment> actives, _, _) = a.GetBreakbarStatus(log);

        foreach (Segment seg in actives)
        {
            active.Add(seg.Start);
            active.Add(seg.End);
        }
    }

}
