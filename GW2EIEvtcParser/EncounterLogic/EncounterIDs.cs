namespace GW2EIEvtcParser.EncounterLogic
{
    internal static class EncounterIDs
    {

        public static long Unknown = 0x0;

        public static class EncounterMasks
        {
            public static long Unsupported = 0x010000;
            public static long RaidMask = 0x020000;
            public static long FractalMask = 0x030000;
            public static long StrikeMask = 0x040000;
            public static long OpenWorldMask = 0x050000;
            public static long StoryInstanceMask = 0x060000;
            public static long WvWMask = 0x070000;
            public static long GolemMask = 0x080000;
        }

        public static class RaidWingMasks
        {
            public static long SpiritValeMask = 0x000100;
            public static long SalvationPassMask = 0x000200;
            public static long StrongholdOfTheFaithfulMask = 0x000300;
            public static long BastionOfThePenitentMask = 0x000400;
            public static long HallOfChainsMask = 0x000500;
            public static long MythwrightGambitMask = 0x000600;
            public static long TheKeyOfAhdashimMask = 0x000700;
        }

        public static class FractalMasks
        {
            public static long NightmareMask = 0x000100;
            public static long ShatteredObservatoryMask = 0x000200;
            public static long SunquaPeakMask = 0x000300;
        }

        public static class StrikeMasks
        {
            public static long FestivalMask = 0x000100;
            public static long IBSMask = 0x000200;
            public static long EODMask = 0x000300;
        }

        public static class WvWMasks
        {
            public static long EternalBattlegroundsMask = 0x000100;
            public static long GreenAlpineBorderlandsMask = 0x000200;
            public static long BlueAlpineBorderlandsMask = 0x000300;
            public static long RedDesertBorderlandsMask = 0x000400;
            public static long ObsidianSanctumMask = 0x000500;
            public static long EdgeOfTheMistsMask = 0x000600;
            public static long ArmisticeBastionMask = 0x000700;
        }

    }
}
