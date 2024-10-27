using System;

namespace GW2EIEvtcParser.EIData;

public class DamageModifierStat
{
    public readonly int HitCount;
    public readonly int TotalHitCount;
    public readonly double DamageGain;
    public readonly int TotalDamage;

    public DamageModifierStat(int hitCount, int totalHitCount, double damageGain, int totalDamage)
    {
        HitCount = hitCount;
        TotalHitCount = totalHitCount;
        DamageGain = Math.Round(damageGain, ParserHelper.DamageModGainDigit);
        TotalDamage = totalDamage;
    }
}
