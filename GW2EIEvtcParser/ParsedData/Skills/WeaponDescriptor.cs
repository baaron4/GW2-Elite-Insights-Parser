using GW2EIUtils.GW2API;

namespace GW2EIEvtcParser.ParsedData
{
    public class WeaponDescriptor
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
    }
}
