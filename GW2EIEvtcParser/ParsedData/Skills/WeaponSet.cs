using GW2EIGW2API.GW2API;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class WeaponSet
{
    public const string Unknown = "Unknown";
    public const string TwoHand = "2Hand";

    private string LandMH1 = Unknown;
    private string LandOH1 = Unknown;
    private string LandMH2 = Unknown;
    private string LandOH2 = Unknown;
    public bool HasLandSwapped { get; internal set; } = false;
    private string WaterMH1 = Unknown;
    private string WaterOH1 = Unknown;
    private string WaterMH2 = Unknown;
    private string WaterOH2 = Unknown;
    public bool HasWaterSwapped { get; internal set; } = false;

    public long Start { get; private set; }
    public long End { get; private set; }

    internal WeaponSet(long start, long end)
    {
        Start = start;
        End = end;
    }

    internal void SetEnd(long end)
    {
        End = end;
    }

    private bool SetLand1MainHand(string wep, long time)
    {
        if (LandMH1 != Unknown && LandMH1 != wep)
        {
            return false;
        }
        LandMH1 = wep;
        End = time;
        return true;
    }
    private bool SetLand1OffHand(string wep, long time)
    {
        if (LandOH1 != Unknown && LandOH1 != wep)
        {
            return false;
        }
        LandOH1 = wep;
        End = time;
        return true;
    }
    private bool SetLand1TwoHand(string wep, long time)
    {
        if (LandMH1 != Unknown && LandMH1 != wep && LandOH1 != Unknown && LandOH1 != TwoHand)
        {
            return false;
        }
        LandMH1 = wep;
        LandOH1 = TwoHand;
        End = time;
        return true;
    }

    private bool SetLand2MainHand(string wep, long time)
    {
        if (LandMH2 != Unknown && LandMH2 != wep)
        {
            return false;
        }
        LandMH2 = wep;
        End = time;
        return true;
    }
    private bool SetLand2OffHand(string wep, long time)
    {
        if (LandOH2 != Unknown && LandOH2 != wep)
        {
            return false;
        }
        LandOH2 = wep;
        End = time;
        return true;
    }
    private bool SetLand2TwoHand(string wep, long time)
    {
        if (LandMH2 != Unknown && LandMH2 != wep && LandOH2 != Unknown && LandOH2 != TwoHand)
        {
            return false;
        }
        LandMH2 = wep;
        LandOH2 = TwoHand;
        End = time;
        return true;
    }

    private bool SetWater1MainHand(string wep, long time)
    {
        if (WaterMH1 != Unknown && WaterMH1 != wep)
        {
            return false;
        }
        WaterMH1 = wep;
        End = time;
        return true;
    }
    private bool SetWater1OffHand(string wep, long time)
    {
        if (WaterOH1 != Unknown && WaterOH1 != wep)
        {
            return false;
        }
        WaterOH1 = wep;
        End = time;
        return true;
    }
    private bool SetWater1TwoHand(string wep, long time)
    {
        if (WaterMH1 != Unknown && WaterMH1 != wep && WaterOH1 != Unknown && WaterOH1 != TwoHand)
        {
            return false;
        }
        WaterMH1 = wep;
        WaterOH1 = TwoHand;
        End = time;
        return true;
    }

    private bool SetWater2MainHand(string wep, long time)
    {
        if (WaterMH2 != Unknown && WaterMH2 != wep)
        {
            return false;
        }
        WaterMH2 = wep;
        End = time;
        return true;
    }
    private bool SetWater2OffHand(string wep, long time)
    {
        if (WaterOH2 != Unknown && WaterOH2 != wep)
        {
            return false;
        }
        WaterOH2 = wep;
        End = time;
        return true;
    }
    private bool SetWater2TwoHand(string wep, long time)
    {
        if (WaterMH2 != Unknown && WaterMH2 != wep && WaterOH2 != Unknown && WaterOH2 != TwoHand)
        {
            return false;
        }
        WaterMH2 = wep;
        WaterOH2 = TwoHand;
        End = time;
        return true;
    }

    internal bool SetWeapons(WeaponDescriptor weaponDescriptor, GW2APISkill ApiSkill, long time, int swapped)
    {
        if (weaponDescriptor.WeaponSlot == WeaponDescriptor.Hand.Dual)
        {
            switch (swapped)
            {
                case WeaponSetIDs.FirstLandSet:
                    return SetLand1MainHand(ApiSkill.WeaponType, time) && SetLand1OffHand(ApiSkill.DualWield, time);
                case WeaponSetIDs.SecondLandSet:
                    return SetLand2MainHand(ApiSkill.WeaponType, time) && SetLand2OffHand(ApiSkill.DualWield, time);
                case WeaponSetIDs.FirstWaterSet:
                    return SetWater1MainHand(ApiSkill.WeaponType, time) && SetWater1OffHand(ApiSkill.DualWield, time);
                case WeaponSetIDs.SecondWaterSet:
                    return SetWater2MainHand(ApiSkill.WeaponType, time) && SetWater2OffHand(ApiSkill.DualWield, time);
            }
        }
        else if (weaponDescriptor.WeaponSlot == WeaponDescriptor.Hand.TwoHand)
        {
            switch (swapped)
            {
                case WeaponSetIDs.FirstLandSet:
                    return SetLand1TwoHand(ApiSkill.WeaponType, time);
                case WeaponSetIDs.SecondLandSet:
                    return SetLand2TwoHand(ApiSkill.WeaponType, time);
                case WeaponSetIDs.FirstWaterSet:
                    return SetWater1TwoHand(ApiSkill.WeaponType, time);
                case WeaponSetIDs.SecondWaterSet:
                    return SetWater2TwoHand(ApiSkill.WeaponType, time);
            }
        }
        else if (weaponDescriptor.WeaponSlot == WeaponDescriptor.Hand.MainHand)
        {
            switch (swapped)
            {
                case WeaponSetIDs.FirstLandSet:
                    return SetLand1MainHand(ApiSkill.WeaponType, time);
                case WeaponSetIDs.SecondLandSet:
                    return SetLand2MainHand(ApiSkill.WeaponType, time);
                case WeaponSetIDs.FirstWaterSet:
                    return SetWater1MainHand(ApiSkill.WeaponType, time);
                case WeaponSetIDs.SecondWaterSet:
                    return SetWater2MainHand(ApiSkill.WeaponType, time);
            }
        }
        else
        {
            switch (swapped)
            {
                case WeaponSetIDs.FirstLandSet:
                    return SetLand1OffHand(ApiSkill.WeaponType, time);
                case WeaponSetIDs.SecondLandSet:
                    return SetLand2OffHand(ApiSkill.WeaponType, time);
                case WeaponSetIDs.FirstWaterSet:
                    return SetWater1OffHand(ApiSkill.WeaponType, time);
                case WeaponSetIDs.SecondWaterSet:
                    return SetWater2OffHand(ApiSkill.WeaponType, time);
            }
        }
        return true;
    }

    public (string[] Weapons, long[] Timeframe) ToArray() => ([LandMH1, LandOH1, LandMH2, LandOH2, WaterMH1, WaterOH1, WaterMH2, WaterOH2], [Start, End]);

    public (string MH, string OH) LandSet1 => (LandMH1, LandOH1);
    public bool IsLand1TwoHand => LandOH1 == TwoHand;
    public bool IsLand1Unknown => LandMH1 == Unknown && LandOH1 == Unknown;

    public (string MH, string OH) LandSet2 => (LandMH2, LandOH2);
    public bool IsLand2TwoHand => LandOH2 == TwoHand;
    public bool IsLand2Unknown => LandMH2 == Unknown && LandOH2 == Unknown;

    public (string MH, string OH) WaterSet1 => (WaterMH1, WaterOH1);
    public bool IsWater1TwoHand => WaterOH1 == TwoHand;
    public bool IsWater1Unknown => WaterMH1 == Unknown && WaterOH1 == Unknown;

    public (string MH, string OH) WaterSet2 => (WaterMH2, WaterOH2);
    public bool IsWater2TwoHand => WaterOH2 == TwoHand;
    public bool IsWater2Unknown => WaterMH2 == Unknown && WaterOH2 == Unknown;
}
