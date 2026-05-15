using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class WvWTeamsEvent : MetaDataEvent
{
    public readonly uint RedShardID;
    public readonly uint BlueShardID;
    public readonly uint GreenShardID;

    public readonly uint RedTeamID;
    public readonly uint BlueTeamID;
    public readonly uint GreenTeamID;
    internal WvWTeamsEvent(CombatItem evtcItem) : base(evtcItem)
    {
        var wvwTeamBytes = new ByteBuffer(stackalloc byte[6 * sizeof(uint)]);
        // 2
        wvwTeamBytes.PushNative(evtcItem.SrcAgent);
        // 2
        wvwTeamBytes.PushNative(evtcItem.DstAgent);
        // 1
        wvwTeamBytes.PushNative(evtcItem.Value);
        // 1
        wvwTeamBytes.PushNative(evtcItem.BuffDmg);

        unsafe
        {
            fixed (byte* ptr = wvwTeamBytes.Span)
            {
                var wvwTeamUints = (uint*)ptr;
                RedShardID = wvwTeamUints[0];
                BlueShardID = wvwTeamUints[1];
                GreenShardID = wvwTeamUints[2];
                RedTeamID = wvwTeamUints[3];
                BlueTeamID = wvwTeamUints[4];
                GreenTeamID = wvwTeamUints[5];
            }
        }
    }

}
