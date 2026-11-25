using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using static GW2EIBuilders.JsonModels.JsonLogBuilder;
using static GW2EIJSON.JsonBuffVolumes;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities;

/// <summary>
/// Class representing buff on targets
/// </summary>
internal static class JsonBuffVolumesBuilder
{
    private static Dictionary<string, double>? ConvertKeys(IReadOnlyDictionary<SingleActor, double> toConvert)
    {
        var res = new Dictionary<string, double>(toConvert.Count);
        foreach (KeyValuePair<SingleActor, double> pair in toConvert)
        {
            if (pair.Value > 0)
            {
                res[pair.Key.Character] = pair.Value;
            }
        }
        return res;
    }

    public static JsonBuffVolumesData BuildJsonBuffVolumesData(BuffVolumeStatistics buffs, BuffVolumeByActorStatistics buffsDictionary)
    {
        var jsonBuffsUptimeData = new JsonBuffVolumesData
        {
            Incoming = buffs.Incoming,
            IncomingByExtension = buffs.IncomingByExtension,
            IncomingByUnknownExtension = buffs.IncomingByUnknownExtension,
            IncomingBy = ConvertKeys(buffsDictionary.IncomingBy),
            IncomingByExtensionBy = ConvertKeys(buffsDictionary.IncomingByExtensionBy),
        };
        return jsonBuffsUptimeData;
    }


    public static JsonBuffVolumes BuildJsonBuffVolumes(long buffID, ParsedEvtcLog log, List<JsonBuffVolumesData> buffVolumeData, UsedJsonData usedData, bool isMinion)
    {
        var jsonBuffVolumes = new JsonBuffVolumes
        {
            Id = buffID,
            BuffVolumeData = isMinion ? buffVolumeData.Select(x => x.Incoming > 0 || x.IncomingByExtension > 0 || x.IncomingByUnknownExtension > 0 ? x : null).ToList()! : buffVolumeData
        };
        usedData.buffs.TryAdd(buffID, log.Buffs.BuffsByIDs[buffID]);
        return jsonBuffVolumes;
    }
}
