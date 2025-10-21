namespace GW2EIJSON;

/// <summary>
/// Class corresponding to a weapon set
/// </summary>
public class JsonWeaponSet
{
    /// <summary>
    /// Weapons of the actor \n
    /// 0-1 are the first land set, 1-2 are the second land set \n
    /// 3-4 are the first aquatic set, 5-6 are the second aquatic set \n
    /// When unknown, 'Unknown' value will appear \n
    /// If 2 handed weapon even indices will have "2Hand" as value \n
    /// </summary>
    public IReadOnlyList<string>? Weapons;
    /// <summary>
    /// Time window during which that specific weapon set is in use.
    /// </summary>
    public IReadOnlyList<long>? Timeframe;
}
