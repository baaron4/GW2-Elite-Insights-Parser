using System.Numerics;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.ParsedData;

public class ProjectileEvent : StatusEvent
{
    public readonly Vector3 Origin = new(0, 0, 0);
    public readonly long SkillID;

    private readonly List<ProjectileLaunchEvent> _launchEvents = [];
    public IReadOnlyList<ProjectileLaunchEvent> LaunchEvents => _launchEvents;
    public ProjectileRemoveEvent? RemoveEvent { get; private set; }
    internal ProjectileEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        var originBytes = new ByteBuffer(stackalloc byte[3 * sizeof(float)]);
        // 2 
        originBytes.PushNative(evtcItem.DstAgent);
        // 1
        originBytes.PushNative(evtcItem.Value);
        unsafe
        {
            fixed (byte* ptr = originBytes.Span)
            {
                var originFloats = (float*)ptr;
                Origin = new(
                        originFloats[0],
                        originFloats[1],
                        originFloats[2]
                    );
            }
        }
        SkillID = evtcItem.BuffDmg;
        /*
            iff -> (float*)f[1] something to do with range, maybe you can find out
            shields/offcycle -> flags as per game. DE rifle 4 change one of them, thats all i know
        */
    }

    internal void AddLaunchEvent(ProjectileLaunchEvent launchEvent)
    {
        _launchEvents.Add(launchEvent);
        launchEvent.Projectile = this;
    }

    internal void SetRemoveEvent(ProjectileRemoveEvent removeEvent)
    {
        RemoveEvent = removeEvent;
        removeEvent.Projectile = this;
    }

    internal static uint GetTrackableID(CombatItem evtcItem)
    {
        return evtcItem.OverstackValue;
    }

}
