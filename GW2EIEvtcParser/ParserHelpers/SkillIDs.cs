using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser
{
    /// <summary>
    /// Pool of skill ids used in the parser, custom or official.
    /// Naming convention: 
    /// no "id" inside the name
    /// should the skill name collide with its cast form and buff form, use XXX for the cast and XXXEffect for the buff
    /// </summary>
    public static class SkillIDs
    {

        /////////////// Custom IDS
        public const long Unknown = -1;
        public const long WeaponSwap = -2;
        public const long NumberOfBoons = -3;
        public const long NumberOfConditions = -4;
        public const long NoBuff = long.MinValue;
        // Weaver attunements
        public const long FireWater = -5;
        public const long FireAir = -6;
        public const long FireEarth = -7;
        public const long WaterFire = -8;
        public const long WaterAir = -9;
        public const long WaterEarth = -10;
        public const long AirFire = -11;
        public const long AirWater = -12;
        public const long AirEarth = -13;
        public const long EarthFire = -14;
        public const long EarthWater = -15;
        public const long EarthAir = -16;
        //
        public const long MirageCloakDodge = -17;
        public const long NumberOfActiveCombatMinions = -18;
        public const long NumberOfClones = -19;
        public const long Respawn = -20;
        public const long Death = -21;
        public const long Down = -22;
        public const long Despawn = -23;
        public const long Alive = -24;
        ////////////////
        internal const long ArcDPSDodge = 65001;
        internal const long ArcDPSGenericBreakbar = 65002;
        internal const long ArcDPSDodge20220307 = 23275;
        internal const long ArcDPSGenericBreakbar20220307 = 23276;
        ///
        public const long Protection = 717;
        public const long Regeneration = 718;
        public const long Swiftness = 719;
        public const long Fury = 725;
        public const long Vigor = 726;
        public const long Might = 740;
        public const long Aegis = 743;
        public const long Retaliation = 873;
        public const long Resolution = 873;
        public const long Resurrect = 1066;
        public const long Stability = 1122;
        public const long Bandage = 1175;
        public const long Quickness = 1187;
        public const long Resistance = 26980;
        public const long Alacrity = 30328;
        public const long FireMajor = 40926;
        public const long WaterDual = 41166;
        public const long AirMajor = 41692;
        public const long AirDual = 42264;
        public const long FireMinor = 42811;
        public const long AirMinor = 43229;
        public const long WaterMajor = 43236;
        public const long WaterMinor = 43370;
        public const long FireDual = 43470;
        public const long EarthMajor = 43740;
        public const long EarthMinor = 44822;
        public const long EarthDual = 44857;

    }

}
