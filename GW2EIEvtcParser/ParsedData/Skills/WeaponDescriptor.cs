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
                if (firstSwap != SkillItem.FirstLandSet && firstSwap != SkillItem.SecondLandSet)
                {
                    swapped = swaps.Exists(x => x == SkillItem.FirstLandSet || x == SkillItem.SecondLandSet) ? swaps.First(x => x == SkillItem.FirstLandSet || x == SkillItem.SecondLandSet) : SkillItem.FirstLandSet;
                }
                else
                {
                    swapped = firstSwap == SkillItem.FirstLandSet ? SkillItem.SecondLandSet : SkillItem.FirstLandSet;
                }
            }
            else
            {
                // if the first swap is not a water set that means the next time we get to a water set was the first set to begin with
                if (firstSwap != SkillItem.FirstWaterSet && firstSwap != SkillItem.SecondWaterSet)
                {
                    swapped = swaps.Exists(x => x == SkillItem.FirstWaterSet || x == SkillItem.SecondWaterSet) ? swaps.First(x => x == SkillItem.FirstWaterSet || x == SkillItem.SecondWaterSet) : SkillItem.FirstWaterSet;
                }
                else
                {
                    swapped = firstSwap == SkillItem.FirstWaterSet ? SkillItem.SecondWaterSet : SkillItem.FirstWaterSet;
                }
            }
            return swapped;
        }
    }
}
