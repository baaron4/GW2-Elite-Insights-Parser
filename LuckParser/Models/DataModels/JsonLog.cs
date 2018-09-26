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
                CondiDamage = new int[phaseCount];
                CondiDps = new int[phaseCount];
                Damage = new int[phaseCount];
                Dps = new int[phaseCount];
                PowerDamage = new int[phaseCount];
                PowerDps = new int[phaseCount];
                PlayerPowerDamage = new int[phaseCount];
            }

            public int[] Dps;
            public int[] Damage;
            public int[] CondiDps;
            public int[] CondiDamage;
            public int[] PowerDps;
            public int[] PowerDamage;

            // Player only
            public int[] PlayerPowerDamage;
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
            public Dictionary<string, JsonBossBoon> Conditions;
        }

        public class JsonStatsAll
        {
            public JsonStatsAll(int phaseCount)
            {
                AvgBoons = new double[phaseCount];
                CritablePowerLoopCount = new int[phaseCount];
                CriticalDmg = new int[phaseCount];
                CriticalRate = new int[phaseCount];
                Dcd = new double[phaseCount];
                Died = new double[phaseCount];
                DodgeCount = new int[phaseCount];
                DownCount = new int[phaseCount];
                FlankingRate = new int[phaseCount];
                GlanceRate = new int[phaseCount];
                Interrupts = new int[phaseCount];
                Invulned = new int[phaseCount];
                Missed = new int[phaseCount];
                MovingDamage = new int[phaseCount];
                MovingRate = new int[phaseCount];
                PowerLoopCount = new int[phaseCount];
                Saved = new int[phaseCount];
                ScholarDmg = new int[phaseCount];
                ScholarRate = new int[phaseCount];
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

            // Counts
            public int[] SwapCount;
            public int[] DownCount;
            public int[] DodgeCount;

            // Misc
            public double[] Died;
            public double[] Dcd;
        }

        public class JsonStatsBoss
        {
            public JsonStatsBoss(int phaseCount)
            {
                CritablePowerLoopCount = new int[phaseCount];
                CriticalDmg = new int[phaseCount];
                CriticalRate = new int[phaseCount];
                FlankingRate = new int[phaseCount];
                GlanceRate = new int[phaseCount];
                Interrupts = new int[phaseCount];
                Invulned = new int[phaseCount];
                Missed = new int[phaseCount];
                MovingDamage = new int[phaseCount];
                MovingRate = new int[phaseCount];
                PowerLoopCount = new int[phaseCount];
                ScholarDmg = new int[phaseCount];
                ScholarRate = new int[phaseCount];;
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
            public JsonDps DpsAll;
            public JsonDps DpsBoss;
            public JsonStatsAll StatsAll;
            public JsonStatsBoss StatsBoss;
            public JsonDefenses Defenses;
            public JsonSupport Support;
            public Dictionary<string, JsonBoonUptime> SelfBoons;
            public Dictionary<string, JsonBoonUptime> GroupBoons;
            public Dictionary<string, JsonBoonUptime> OffGroupBoons;
            public Dictionary<string, JsonBoonUptime> SquadBoons;
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
        public string[] UploadLinks;
    }
}