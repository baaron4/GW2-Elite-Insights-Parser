using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class ShardEvent : MetaDataEvent
{
    public readonly ulong ShardID;
    public readonly ulong UpperShardID;
    public readonly uint UserWorldID0;
    public readonly uint UserWorldID1;

    public readonly RegionEnum Region = RegionEnum.Unknown;


    private static RegionEnum GetRegion(ulong shardID)
    {
        if (shardID < 2000 && shardID > 1000)
        {
            return RegionEnum.NA;
        } 
        else if (shardID > 2000 && shardID < 3000)
        {
            return RegionEnum.EU;
        }
        else if (shardID > 7000 && shardID < 8000)
        {
            return RegionEnum.CN;
        }
        return RegionEnum.Unknown;
    }
    internal ShardEvent(CombatItem evtcItem, MapIDEvent? mapEvent, GW2APIController apiController) : base(evtcItem)
    {
        ShardID = evtcItem.SrcAgent;
        UpperShardID = evtcItem.DstAgent;
        UserWorldID0 = (uint)evtcItem.Value;
        UserWorldID1 = (uint)evtcItem.BuffDmg;
        if (UserWorldID0 > 0)
        {
            Region = GetRegion(UserWorldID0);
        } 
        else if (mapEvent != null)
        {
            var mapAPI = apiController.GetAPIMap(mapEvent.MapID);
            if (mapAPI != null && mapAPI.Type == "Instance")
            {
                Region = GetRegion(ShardID);
            }
        }
    }

    /// <summary>
    /// Can be used when it is certain that we are in an instance map
    /// </summary>
    /// <param name="evtcItem"></param>
    /// <returns></returns>
    internal static RegionEnum GetRegionFromInstanceMap(CombatItem evtcItem)
    {
        if (evtcItem.Value > 0)
        {
            return GetRegion((uint)evtcItem.Value);
        }
        return GetRegion(evtcItem.SrcAgent);
    }
}
