namespace GW2EIEvtcParser.EncounterLogic
{
    internal static class EncounterIDs
    {

        public const long Unknown = 0x0;

        public static class EncounterMasks
        {
            public const long Unsupported = 0x010000;
            public const long RaidMask = 0x020000;
            public const long FractalMask = 0x030000;
            public const long StrikeMask = 0x040000;
            public const long OpenWorldMask = 0x050000;
            public const long StoryInstanceMask = 0x060000;
            public const long WvWMask = 0x070000;
            public const long GolemMask = 0x080000;
        }

        public static class RaidWingMasks
        {
            public const long SpiritValeMask = 0x000100;
            public const long SalvationPassMask = 0x000200;
            public const long StrongholdOfTheFaithfulMask = 0x000300;
            public const long BastionOfThePenitentMask = 0x000400;
            public const long HallOfChainsMask = 0x000500;
            public const long MythwrightGambitMask = 0x000600;
            public const long TheKeyOfAhdashimMask = 0x000700;
        }

        public static class FractalMasks
        {
            public const long NightmareMask = 0x000100;
            public const long ShatteredObservatoryMask = 0x000200;
            public const long SunquaPeakMask = 0x000300;
            public const long SilentSurfMask = 0x000400;
            public const long LonelyTowerMask = 0x000500;
        }

        public static class StrikeMasks
        {
            public const long FestivalMask = 0x000100;
            public const long IBSMask = 0x000200;
            public const long EODMask = 0x000300;
            public const long CoreMask = 0x000400;
            public const long SotOMask = 0x000500;
        }

        public static class WvWMasks
        {
            public const long EternalBattlegroundsMask = 0x000100;
            public const long GreenAlpineBorderlandsMask = 0x000200;
            public const long BlueAlpineBorderlandsMask = 0x000300;
            public const long RedDesertBorderlandsMask = 0x000400;
            public const long ObsidianSanctumMask = 0x000500;
            public const long EdgeOfTheMistsMask = 0x000600;
            public const long ArmisticeBastionMask = 0x000700;
            public const long GildedHollowMask = 0x000800;
            public const long LostPrecipiceMask = 0x000900;
            public const long WindsweptHavenMask = 0x000A00;
            public const long IsleOfReflectionMask = 0x000B00;
        }

    }
}
