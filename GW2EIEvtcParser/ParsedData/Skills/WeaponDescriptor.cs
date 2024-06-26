using System.Collections.Generic;
using System.Linq;
using GW2EIGW2API.GW2API;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    internal class WeaponDescriptor
    {
        public enum Hand { MainHand, TwoHand, OffHand, Dual }

        public bool IsLand { get; }
        public Hand WeaponSlot { get; }

        public WeaponDescriptor(GW2APISkill apiSkill)
        {
            if (apiSkill.WeaponType == "Trident" || apiSkill.WeaponType == "Speargun" || apiSkill.WeaponType == "Spear")
            {
                IsLand = false;
                WeaponSlot = Hand.TwoHand;
                // Placeholder, very possible we may need some sort of id list for non weapon swapping classes/people
                if (apiSkill.WeaponType == "Spear" && apiSkill.Id > 70000)
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
                else if (apiSkill.WeaponType == "Greatsword" || apiSkill.WeaponType == "Staff" || apiSkill.WeaponType == "Rifle" || apiSkill.WeaponType == "Longbow" || apiSkill.WeaponType == "Shortbow" || apiSkill.WeaponType == "Hammer")
                {
                    WeaponSlot = Hand.TwoHand;
                }
                else
                {
                    WeaponSlot = apiSkill.Slot == "Weapon_1" || apiSkill.Slot == "Weapon_2" || apiSkill.Slot == "Weapon_3" ? Hand.MainHand : Hand.OffHand;
                }
            }
        }

        public static bool IsWeaponSlot(string slot)
        {
            return slot == "Weapon_1" || slot == "Weapon_2" || slot == "Weapon_3" || slot == "Weapon_4" || slot == "Weapon_5";
        }

        internal int FindFirstWeaponSet(IReadOnlyList<(int to, int from)> swaps)
        {
            if (swaps.Count > 0 && swaps[0].from >= 0)
            {
                return swaps[0].from;
            }
            int swapped = WeaponSetIDs.NoSet;
            int firstSwap = swaps.Count > 0 ? swaps[0].to : WeaponSetIDs.NoSet;
            if (IsLand)
            {
                // if the first swap is not a land set that means the next time we get to a land set was the first set to begin with
                if (firstSwap != WeaponSetIDs.FirstLandSet && firstSwap != WeaponSetIDs.SecondLandSet)
                {
                    swapped = swaps.Any(x => x.to == WeaponSetIDs.FirstLandSet || x.to == WeaponSetIDs.SecondLandSet) ? swaps.First(x => x.to == WeaponSetIDs.FirstLandSet || x.to == WeaponSetIDs.SecondLandSet).to : WeaponSetIDs.FirstLandSet;
                }
                else
                {
                    swapped = firstSwap == WeaponSetIDs.FirstLandSet ? WeaponSetIDs.SecondLandSet : WeaponSetIDs.FirstLandSet;
                }
            }
            else
            {
                // if the first swap is not a water set that means the next time we get to a water set was the first set to begin with
                if (firstSwap != WeaponSetIDs.FirstWaterSet && firstSwap != WeaponSetIDs.SecondWaterSet)
                {
                    swapped = swaps.Any(x => x.to == WeaponSetIDs.FirstWaterSet || x.to == WeaponSetIDs.SecondWaterSet) ? swaps.First(x => x.to == WeaponSetIDs.FirstWaterSet || x.to == WeaponSetIDs.SecondWaterSet).to : WeaponSetIDs.FirstWaterSet;
                }
                else
                {
                    swapped = firstSwap == WeaponSetIDs.FirstWaterSet ? WeaponSetIDs.SecondWaterSet : WeaponSetIDs.FirstWaterSet;
                }
            }
            return swapped;
        }
    }
}
