using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using LuckParser.Controllers;
using LuckParser.Models.ParseModels;

namespace LuckParser.Models.DataModels
{
    class JsonLog
    {
        public struct JsonDps
        {
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

        public struct JsonDefenses
        {
            public long[] DamageTaken;
            public int[] BlockedCount;
            public int[] EvadedCount;
            public int[] InvulnedCount;
            public int[] DamageInvulned;
            public int[] DamageBarrier;
        }

        public struct JsonBoonUptime
        {
            public double[] Uptime;
            public double[] Generation;
            public double[] Overstack;
        }

        public struct JsonSupport
        {
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
            public List<Point> HealthOverTime;
            public JsonDps Dps;
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
            public Statistics.FinalStats[] Stats;
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

        public string EliteInsightsVersion;
        public string ArcVersion;
        public string RecordedBy;
        public string TimeStart;
        public string TimeEnd;
        public string Duration;
        public bool Success;
        public JsonBoss Boss;
        public ArrayList Players;
        public ArrayList Phases;
        public List<Point3D> StackCenterPositions;
    }
}
