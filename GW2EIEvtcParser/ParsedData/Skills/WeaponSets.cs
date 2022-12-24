using GW2EIGW2API.GW2API;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class WeaponSets
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

        internal WeaponSets()
        {
        }

        private void SetLand1MainHand(string wep)
        {
            LandMH1 = wep;
        }
        private void SetLand1OffHand(string wep)
        {
            LandOH1 = wep;
        }
        private void SetLand1TwoHand(string wep)
        {
            LandMH1 = wep;
            LandOH1 = TwoHand;
        }

        private void SetLand2MainHand(string wep)
        {
            LandMH2 = wep;
        }
        private void SetLand2OffHand(string wep)
        {
            LandOH2 = wep;
        }
        private void SetLand2TwoHand(string wep)
        {
            LandMH2 = wep;
            LandOH2 = TwoHand;
        }

        private void SetWater1MainHand(string wep)
        {
            WaterMH1 = wep;
        }
        private void SetWater1OffHand(string wep)
        {
            WaterOH1 = wep;
        }
        private void SetWater1TwoHand(string wep)
        {
            WaterMH1 = wep;
            WaterOH1 = TwoHand;
        }

        private void SetWater2MainHand(string wep)
        {
            WaterMH2 = wep;
        }
        private void SetWater2OffHand(string wep)
        {
            WaterOH2 = wep;
        }
        private void SetWater2TwoHand(string wep)
        {
            WaterMH2 = wep;
            WaterOH2 = TwoHand;
        }

        internal void SetWeapons(WeaponDescriptor weaponDescriptor, GW2APISkill ApiSkill, int swapped)
        {
            if (weaponDescriptor.WeaponSlot == WeaponDescriptor.Hand.Dual)
            {
                switch(swapped)
                {
                    case WeaponSetIDs.FirstLandSet:
                        SetLand1MainHand(ApiSkill.WeaponType);
                        SetLand1OffHand(ApiSkill.DualWield);
                        break;
                    case WeaponSetIDs.SecondLandSet:
                        SetLand2MainHand(ApiSkill.WeaponType);
                        SetLand2OffHand(ApiSkill.DualWield);
                        break;
                    case WeaponSetIDs.FirstWaterSet:
                        SetWater1MainHand(ApiSkill.WeaponType);
                        SetWater1OffHand(ApiSkill.DualWield);
                        break;
                    case WeaponSetIDs.SecondWaterSet:
                        SetWater2MainHand(ApiSkill.WeaponType);
                        SetWater2OffHand(ApiSkill.DualWield);
                        break;
                }
            }
            else if (weaponDescriptor.WeaponSlot == WeaponDescriptor.Hand.TwoHand)
            {
                switch (swapped)
                {
                    case WeaponSetIDs.FirstLandSet:
                        SetLand1TwoHand(ApiSkill.WeaponType);
                        break;
                    case WeaponSetIDs.SecondLandSet:
                        SetLand2TwoHand(ApiSkill.WeaponType);
                        break;
                    case WeaponSetIDs.FirstWaterSet:
                        SetWater1TwoHand(ApiSkill.WeaponType);
                        break;
                    case WeaponSetIDs.SecondWaterSet:
                        SetWater2TwoHand(ApiSkill.WeaponType);
                        break;
                }
            }
            else if (weaponDescriptor.WeaponSlot == WeaponDescriptor.Hand.MainHand)
            {
                switch (swapped)
                {
                    case WeaponSetIDs.FirstLandSet:
                        SetLand1MainHand(ApiSkill.WeaponType);
                        break;
                    case WeaponSetIDs.SecondLandSet:
                        SetLand2MainHand(ApiSkill.WeaponType);
                        break;
                    case WeaponSetIDs.FirstWaterSet:
                        SetWater1MainHand(ApiSkill.WeaponType);
                        break;
                    case WeaponSetIDs.SecondWaterSet:
                        SetWater2MainHand(ApiSkill.WeaponType);
                        break;
                }
            }
            else
            {
                switch (swapped)
                {
                    case WeaponSetIDs.FirstLandSet:
                        SetLand1OffHand(ApiSkill.WeaponType);
                        break;
                    case WeaponSetIDs.SecondLandSet:
                        SetLand2OffHand(ApiSkill.WeaponType);
                        break;
                    case WeaponSetIDs.FirstWaterSet:
                        SetWater1OffHand(ApiSkill.WeaponType);
                        break;
                    case WeaponSetIDs.SecondWaterSet:
                        SetWater2OffHand(ApiSkill.WeaponType);
                        break;
                }
            }
        }

        public string[] ToArray() => new string[] { LandMH1, LandOH1, LandMH2, LandOH2, WaterMH1, WaterOH1, WaterMH2, WaterOH2 };

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
}
