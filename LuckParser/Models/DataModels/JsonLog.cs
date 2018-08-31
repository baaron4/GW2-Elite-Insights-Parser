using System;
using System.Collections.Generic;
using LuckParser.Models.ParseModels;

namespace LuckParser.Models.DataModels
{
    class JsonLog
    {
        public class JsonDps
        {
            public JsonDps(int phaseCount)
            {
                AllCondiDamage = new int[phaseCount];
                AllCondiDps = new int[phaseCount];
                AllDamage = new int[phaseCount];
                AllDps = new int[phaseCount];
                AllPowerDamage = new int[phaseCount];
                AllPowerDps = new int[phaseCount];
                BossCondiDamage = new int[phaseCount];
                BossCondiDps = new int[phaseCount];
                BossDamage = new int[phaseCount];
                BossDps = new int[phaseCount];
                BossPowerDamage = new int[phaseCount];
                BossPowerDps = new int[phaseCount];
                PlayerBossPowerDamage = new int[phaseCount];
                PlayerPowerDamage = new int[phaseCount];
            }

            public int[] AllDps;
            public int[] AllDamage;
            public int[] AllCondiDps;
            public int[] AllCondiDamage;
            public int[] AllPowerDps;
            public int[] AllPowerDamage;

            // Boss
            public int[] BossDps;
            public int[] BossDamage;
            public int[] BossCondiDps;
            public int[] BossCondiDamage;
            public int[] BossPowerDps;
            public int[] BossPowerDamage;

            // Player only
            public int[] PlayerPowerDamage;
            public int[] PlayerBossPowerDamage;
        }

        public class JsonDefenses
        {
            public JsonDefenses(int phaseCount)
            {
                BlockedCount = new int[phaseCount];
                DamageBarrier = new int[phaseCount];
                DamageInvulned = new int[phaseCount];
                DamageTaken = new long[phaseCount];
                EvadedCount = new int[phaseCount];
                InvulnedCount = new int[phaseCount];
            }

            public long[] DamageTaken;
            public int[] BlockedCount;
            public int[] EvadedCount;
            public int[] InvulnedCount;
            public int[] DamageInvulned;
            public int[] DamageBarrier;
        }

        public class JsonBoonUptime
        {
            public JsonBoonUptime(int phaseCount)
            {
                Generation = new double[phaseCount];
                Overstack = new double[phaseCount];
                Uptime = new double[phaseCount];
            }

            public double[] Uptime;
            public double[] Generation;
            public double[] Overstack;
        }

        public class JsonSupport
        {
            public JsonSupport(int phaseCount)
            {
                CondiCleanse = new int[phaseCount];
                CondiCleanseTime = new float[phaseCount];
                ResurrectTime = new float[phaseCount];
                Resurrects = new int[phaseCount];
            }

            public int[] Resurrects;
            public float[] ResurrectTime;
            public int[] CondiCleanse;
            public float[] CondiCleanseTime;
        }

        public struct JsonBoss
        {
            public string Name;
            public ushort Id;
            public int TotalHealth;
            public double FinalHealth;
            public double HealthPercentBurned;
            public JsonDps Dps;
            public Dictionary<long, JsonBossBoon> Conditions;
        }

        public class JsonStats
        {
            public JsonStats(int phaseCount)
            {
                AvgBoons = new double[phaseCount];
                CritablePowerLoopCount = new int[phaseCount];
                CritablePowerLoopCountBoss = new int[phaseCount];
                CriticalDmg = new int[phaseCount];
                CriticalDmgBoss = new int[phaseCount];
                CriticalRate = new int[phaseCount];
                CriticalRateBoss = new int[phaseCount];
                Dcd = new double[phaseCount];
                Died = new double[phaseCount];
                DodgeCount = new int[phaseCount];
                DownCount = new int[phaseCount];
                FlankingRate = new int[phaseCount];
                FlankingRateBoss = new int[phaseCount];
                GlanceRate = new int[phaseCount];
                GlanceRateBoss = new int[phaseCount];
                Interrupts = new int[phaseCount];
                InterruptsBoss = new int[phaseCount];
                Invulned = new int[phaseCount];
                InvulnedBoss = new int[phaseCount];
                Missed = new int[phaseCount];
                MissedBoss = new int[phaseCount];
                MovingDamage = new int[phaseCount];
                MovingDamageBoss = new int[phaseCount];
                MovingRate = new int[phaseCount];
                MovingRateBoss = new int[phaseCount];
                PowerLoopCount = new int[phaseCount];
                PowerLoopCountBoss = new int[phaseCount];
                Saved = new int[phaseCount];
                ScholarDmg = new int[phaseCount];
                ScholarDmgBoss = new int[phaseCount];
                ScholarRate = new int[phaseCount];
                ScholarRateBoss = new int[phaseCount];
                StackDist = new double[phaseCount];
                SwapCount = new int[phaseCount];
                TimeSaved = new double[phaseCount];
                TimeWasted = new double[phaseCount];
                Wasted = new int[phaseCount];
            }

            // Rates
            public int[] PowerLoopCount;
            public int[] CritablePowerLoopCount;
            public int[] CriticalRate;
            public int[] CriticalDmg;
            public int[] ScholarRate;
            public int[] ScholarDmg;
            public int[] MovingRate;
            public int[] MovingDamage;
            public int[] FlankingRate;
            public int[] GlanceRate;
            public int[] Missed;
            public int[] Interrupts;
            public int[] Invulned;
            public int[] Wasted;
            public double[] TimeWasted;
            public int[] Saved;
            public double[] TimeSaved;
            public double[] AvgBoons;
            public double[] StackDist;

            //Boss only Rates
            public int[] PowerLoopCountBoss;
            public int[] CritablePowerLoopCountBoss;
            public int[] CriticalRateBoss;
            public int[] CriticalDmgBoss;
            public int[] ScholarRateBoss;
            public int[] ScholarDmgBoss;
            public int[] MovingRateBoss;
            public int[] MovingDamageBoss;
            public int[] FlankingRateBoss;
            public int[] GlanceRateBoss;
            public int[] MissedBoss;
            public int[] InterruptsBoss;
            public int[] InvulnedBoss;

            // Counts
            public int[] SwapCount;
            public int[] DownCount;
            public int[] DodgeCount;

            // Misc
            public double[] Died;
            public double[] Dcd;
        }

        public struct JsonPlayer
        {
            public string Character;
            public string Account;
            public int Condition;
            public int Concentration;
            public int Healing;
            public int Toughness;
            public int Group;
            public string Profession;
            public string[] Weapons;
            public JsonDps Dps;
            public JsonStats Stats;
            public JsonDefenses Defenses;
            public JsonSupport Support;
            public Dictionary<long, JsonBoonUptime> SelfBoons;
            public Dictionary<long, JsonBoonUptime> GroupBoons;
            public Dictionary<long, JsonBoonUptime> OffGroupBoons;
            public Dictionary<long, JsonBoonUptime> SquadBoons;
        }

        public struct JsonPhase
        {
            public long Duration;
            public string Name;
        }

        public class JsonBossBoon
        {
            public JsonBossBoon(int phaseCount)
            {
                Uptime = new double[phaseCount];
                Generated = new Dictionary<string, double>[phaseCount];
                Overstacked = new Dictionary<string, double>[phaseCount];
            }

            public double[] Uptime;
            public Dictionary<string, double>[] Generated;
            public Dictionary<string, double>[] Overstacked;
        }

        public struct JsonMechanic
        {
            public long Time;
            public string Player;
            public string Description;
            public long Skill;
        }

        public string EliteInsightsVersion;
        public string ArcVersion;
        public string RecordedBy;
        public string TimeStart;
        public string TimeEnd;
        public string Duration;
        public bool Success;
        public JsonBoss Boss;
        public List<JsonPlayer> Players;
        public List<JsonPhase> Phases;
        public JsonMechanic[] Mechanics;
        public List<Point3D> StackCenterPositions;
    }
}