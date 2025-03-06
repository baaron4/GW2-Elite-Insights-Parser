using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using static GW2EIJSON.JsonDeathRecap;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonPlayerUtilities;

/// <summary>
/// Class corresponding to a death recap
/// </summary>
internal static class JsonDeathRecapBuilder
{
    private static JsonDeathRecapDamageItem BuildJsonDeathRecapDamageItem(DeathRecap.DeathRecapDamageItem item)
    {
        var jsonDeathRecapDamageItem = new JsonDeathRecapDamageItem
        {
            Id = item.ID,
            IndirectDamage = item.IndirectDamage,
            Src = item.SourceAgent,
            Damage = item.Damage,
            Time = item.Time
        };
        return jsonDeathRecapDamageItem;
    }

    public static JsonDeathRecap BuildJsonDeathRecap(DeathRecap recap)
    {
        var jsonDeathRecap = new JsonDeathRecap
        {
            DeathTime = recap.DeathTime,
            ToDown = recap.ToDown?.Select(x => BuildJsonDeathRecapDamageItem(x)).ToList(),
            ToKill = recap.ToKill?.Select(x => BuildJsonDeathRecapDamageItem(x)).ToList()
        };
        return jsonDeathRecap;
    }

}
