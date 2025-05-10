using System.Numerics;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.ParsedData;

public class MissileEvent : StatusEvent
{
    public readonly Vector3 Origin;
    public readonly SkillItem Skill;
    public long SkillID => Skill.ID;
    public readonly long MissileID;
    public GUID MissileGUID => GUIDEvent.ContentGUID;
    public readonly MissileGUIDEvent GUIDEvent = MissileGUIDEvent.DummyMissileGUID;

    private readonly List<MissileLaunchEvent> _launchEvents = [];
    public IReadOnlyList<MissileLaunchEvent> LaunchEvents => _launchEvents;
    public MissileRemoveEvent? RemoveEvent { get; private set; }
    internal MissileEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, IReadOnlyDictionary<long, MissileGUIDEvent> missileGUIDs) : base(evtcItem, agentData)
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
        Skill = skillData.Get(evtcItem.BuffDmg);
        /*
            iff -> (float*)f[1] something to do with range, maybe you can find out
            shields/offcycle -> flags as per game. DE rifle 4 change one of them, thats all i know
        */
        MissileID = evtcItem.SkillID;
        if (missileGUIDs.TryGetValue(MissileID, out var missileGUID))
        {
            GUIDEvent = missileGUID;
        }
    }

    internal void AddLaunchEvent(MissileLaunchEvent launchEvent)
    {
        _launchEvents.Add(launchEvent);
        launchEvent.Missile = this;
    }

    internal void SetRemoveEvent(MissileRemoveEvent removeEvent)
    {
        RemoveEvent = removeEvent;
        removeEvent.Missile = this;
    }

}
