using GW2EIGW2API.GW2API;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

internal class WeaponDescriptor
{
    private static readonly HashSet<string> _underwaterWeapons = [
        "Trident",
        "Speargun",
        "Spear",
    ];

    private static readonly HashSet<string> _hybridWeapons = [
        "Spear"
    ];

    private static readonly HashSet<string> _twoHandedLandWeapons = [
        "Greatsword",
        "Staff",
        "Rifle",
        "Longbow",
        "Shortbow",
        "Hammer",
    ];

    internal static HashSet<string> AllowedWeaponTypes = [
        .._underwaterWeapons,

        .._twoHandedLandWeapons,

        "Axe",
        "Dagger",
        "Mace",
        "Pistol",
        "Scepter",
        "Sword",
        "Focus",
        "Shield",
        "Torch",
        "Warhorn",
    ];

    private static readonly HashSet<string> _mainWeaponSlots = [
        "Weapon_1",
        "Weapon_2",
        "Weapon_3",
    ];

    private static readonly HashSet<string> _weaponSlots = [
        .._mainWeaponSlots,
        "Weapon_4",
        "Weapon_5",
    ];

    public enum Hand { MainHand, TwoHand, OffHand, Dual }

    public enum WeaponEstimateResult { NotApplicable, Updated, NeedNewSet }

    public readonly bool IsLand;
    public readonly Hand WeaponSlot;

    public WeaponDescriptor(GW2APISkill apiSkill)
    {
        if (_underwaterWeapons.Contains(apiSkill.WeaponType))
        {
            IsLand = false;
            WeaponSlot = Hand.TwoHand;
            // Use flags to know if land or water
            if (_hybridWeapons.Contains(apiSkill.WeaponType) && apiSkill.Flags != null && apiSkill.Flags.Contains("NoUnderwater"))
            {
                IsLand = true;
            }
        }
        else
        {
            IsLand = true;
            if (apiSkill.DualWield != null && apiSkill.DualWield != "None" && apiSkill.DualWield != "Nothing")
            {
                WeaponSlot = Hand.Dual;
            }
            else if (_twoHandedLandWeapons.Contains(apiSkill.WeaponType))
            {
                WeaponSlot = Hand.TwoHand;
            }
            else
            {
                WeaponSlot = _mainWeaponSlots.Contains(apiSkill.Slot) ? Hand.MainHand : Hand.OffHand;
            }
        }
    }

    public static bool IsWeaponSlot(string slot)
    {
        return _weaponSlots.Contains(slot);
    }

    internal int FindFirstWeaponSet(IReadOnlyList<WeaponSwapEvent> swaps)
    {
        int swapped = WeaponSetIDs.NoSet;
        int firstSwap = swaps.Count > 0 ? swaps[0].SwappedTo : WeaponSetIDs.NoSet;
        if (IsLand)
        {
            // if the first swap is not a land set that means the next time we get to a land set was the first set to begin with
            if (!WeaponSetIDs.IsLandSet(firstSwap))
            {
                swapped = swaps.Any(x => WeaponSetIDs.IsLandSet(x.SwappedTo)) ? swaps.First(x => WeaponSetIDs.IsLandSet(x.SwappedTo)).SwappedTo : WeaponSetIDs.FirstLandSet;
            }
            else
            {
                swapped = firstSwap == WeaponSetIDs.FirstLandSet ? WeaponSetIDs.SecondLandSet : WeaponSetIDs.FirstLandSet;
            }
        }
        else
        {
            // if the first swap is not a water set that means the next time we get to a water set was the first set to begin with
            if (!WeaponSetIDs.IsWaterSet(firstSwap))
            {
                swapped = swaps.Any(x => WeaponSetIDs.IsWaterSet(x.SwappedTo)) ? swaps.First(x => WeaponSetIDs.IsWaterSet(x.SwappedTo)).SwappedTo : WeaponSetIDs.FirstWaterSet;
            }
            else
            {
                swapped = firstSwap == WeaponSetIDs.FirstWaterSet ? WeaponSetIDs.SecondWaterSet : WeaponSetIDs.FirstWaterSet;
            }
        }
        return swapped;
    }
}
