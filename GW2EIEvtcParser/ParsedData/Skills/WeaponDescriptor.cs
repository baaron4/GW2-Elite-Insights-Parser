using System.Collections.Generic;
using System.Linq;
using GW2EIGW2API.GW2API;

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

        internal int FindWeaponSlot(List<int> swaps)
        {
            int swapped = -1;
            int firstSwap = swaps.Count > 0 ? swaps[0] : -1;
            if (IsLand)
            {
                // if the first swap is not a land set that means the next time we get to a land set was the first set to begin with
                if (firstSwap != ParserHelper.WeaponSetIDs.FirstLandSet && firstSwap != ParserHelper.WeaponSetIDs.SecondLandSet)
                {
                    swapped = swaps.Exists(x => x == ParserHelper.WeaponSetIDs.FirstLandSet || x == ParserHelper.WeaponSetIDs.SecondLandSet) ? swaps.First(x => x == ParserHelper.WeaponSetIDs.FirstLandSet || x == ParserHelper.WeaponSetIDs.SecondLandSet) : ParserHelper.WeaponSetIDs.FirstLandSet;
                }
                else
                {
                    swapped = firstSwap == ParserHelper.WeaponSetIDs.FirstLandSet ? ParserHelper.WeaponSetIDs.SecondLandSet : ParserHelper.WeaponSetIDs.FirstLandSet;
                }
            }
            else
            {
                // if the first swap is not a water set that means the next time we get to a water set was the first set to begin with
                if (firstSwap != ParserHelper.WeaponSetIDs.FirstWaterSet && firstSwap != ParserHelper.WeaponSetIDs.SecondWaterSet)
                {
                    swapped = swaps.Exists(x => x == ParserHelper.WeaponSetIDs.FirstWaterSet || x == ParserHelper.WeaponSetIDs.SecondWaterSet) ? swaps.First(x => x == ParserHelper.WeaponSetIDs.FirstWaterSet || x == ParserHelper.WeaponSetIDs.SecondWaterSet) : ParserHelper.WeaponSetIDs.FirstWaterSet;
                }
                else
                {
                    swapped = firstSwap == ParserHelper.WeaponSetIDs.FirstWaterSet ? ParserHelper.WeaponSetIDs.SecondWaterSet : ParserHelper.WeaponSetIDs.FirstWaterSet;
                }
            }
            return swapped;
        }
    }
}
