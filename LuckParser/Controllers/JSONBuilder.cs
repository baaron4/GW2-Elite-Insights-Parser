using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using Newtonsoft.Json;

namespace LuckParser.Controllers
{
    class JSONBuilder
    {
        readonly SettingsContainer _settings;

        readonly ParsedLog _log;

        readonly Statistics _statistics;
        readonly StreamWriter _sw;

        public static void UpdateStatisticSwitches(StatisticsCalculator.Switches switches)
        {
            switches.CalculateBoons = true;
            switches.CalculateDPS = true;
            switches.CalculateConditions = true;
            switches.CalculateDefense = true;
            switches.CalculateStats = true;
            switches.CalculateSupport = true;
            switches.CalculateCombatReplay = true;
            switches.CalculateMechanics = true;
        }

        public JSONBuilder(StreamWriter sw, ParsedLog log, SettingsContainer settings, Statistics statistics)
        {
            _log = log;
            _sw = sw;
            _settings = settings;

            _statistics = statistics;
        }

        /*
         * Structs to get serialized into json
         */
        private struct JSONLog
        {
            public struct Boss
            {
                public string name;
                public ushort id;
                public int totalHealth;
                public double finalHealth;
                public double healthPercentBurned;
                public ArrayList dps;
            }

            public struct DPS
            {
                public int allDps;
                public int allDamage;
                public int allCondiDps;
                public int allCondiDamage;
                public int allPowerDps;
                public int allPowerDamage;
                public int bossDps;
                public int bossDamage;
                public int bossCondiDps;
                public int bossCondiDamage;
                public int bossPowerDps;
                public int bossPowerDamage;
                public int playerPowerDamage;
                public int playerBossPowerDamage;
            }

            public struct Player
            {
                public string character;
                public string account;
                public int condition;
                public int concentration;
                public int healing;
                public int toughness;
                public int group;
                public string profession;
                public string[] weapons;
                public ArrayList dps;
            }

            public struct Phase
            {
                public long duration;
                public string name;
            }

            public string eliteInsightsVersion;
            public string arcVersion;
            public string recordedBy;
            public string timeStart;
            public string timeEnd;
            public string duration;
            public bool success;
            public Boss boss;
            public ArrayList players;
            public ArrayList phases;
        }

        /*
         * Creating the JSON
         */
        public void CreateJSON()
        {
            var log = new JSONLog();

            double fightDuration = _log.GetBossData().GetAwareDuration() / 1000.0;
            var duration = TimeSpan.FromSeconds(fightDuration);
            string durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s";
            if (duration.ToString("hh") != "00")
            {
                durationString = duration.ToString("hh") + "h " + durationString;
            }

            log.eliteInsightsVersion = Application.ProductVersion;
            log.arcVersion = _log.GetLogData().GetBuildVersion();
            log.recordedBy = _log.GetLogData().GetPOV().Split(':')[0].TrimEnd('\u0000');
            log.timeStart = _log.GetLogData().GetLogStart();
            log.timeEnd = _log.GetLogData().GetLogEnd();
            log.duration = durationString;
            log.success = _log.GetLogData().GetBosskill();

            log = SetBoss(log);
            log = SetPlayers(log);
            log = SetPhases(log);

            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            var writer = new JsonTextWriter(_sw);
            writer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, log);
        }

        private JSONLog.DPS BuildDPS(Statistics.FinalDPS stats)
        {
            return new JSONLog.DPS
            {
                allDps = stats.AllDps,
                allDamage = stats.AllDamage,
                allCondiDps = stats.AllCondiDps,
                allCondiDamage = stats.AllCondiDamage,
                allPowerDps = stats.AllPowerDps,
                allPowerDamage = stats.AllPowerDamage,
                bossDps = stats.BossDps,
                bossDamage = stats.BossDamage,
                bossCondiDps = stats.BossCondiDps,
                bossCondiDamage = stats.BossCondiDamage,
                bossPowerDps = stats.BossPowerDps,
                bossPowerDamage = stats.BossPowerDamage,
                playerBossPowerDamage = stats.PlayerBossPowerDamage,
                playerPowerDamage = stats.PlayerPowerDamage
            };
        }

        private JSONLog SetBoss(JSONLog log)
        {
            log.boss.id = _log.GetBossData().GetID();
            log.boss.name = _log.GetBossData().GetName();
            log.boss.totalHealth = _log.GetBossData().GetHealth();
            int finalBossHealth = _log.GetBossData().GetHealthOverTime().Count > 0
                ? _log.GetBossData().GetHealthOverTime().Last().Y
                : 10000;
            log.boss.finalHealth = _log.GetBossData().GetHealth() * (100.0 - finalBossHealth * 0.01);
            log.boss.healthPercentBurned = 100.0 - finalBossHealth * 0.01;

            log.boss.dps = new ArrayList();

            for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
            {
                log.boss.dps.Add(BuildDPS(_statistics.BossDps[phaseIndex]));
            }

            return log;
        }

        private JSONLog SetPlayers(JSONLog log)
        {
            log.players = new ArrayList();

            foreach (var player in _log.GetPlayerList())
            {
                var currentPlayer = new JSONLog.Player
                {
                    character = player.GetCharacter(),
                    account = player.GetAccount(),
                    condition = player.GetCondition(),
                    concentration = player.GetConcentration(),
                    healing = player.GetHealing(),
                    toughness = player.GetToughness(),
                    weapons = player.GetWeaponsArray(_log),
                    group = player.GetGroup(),
                    profession = player.GetProf()
                };

                currentPlayer = SetDPS(currentPlayer, player);

                log.players.Add(currentPlayer);
            }

            return log;
        }

        private JSONLog.Player SetDPS(JSONLog.Player currentPlayer, Player player)
        {
            currentPlayer.dps = new ArrayList();

            for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
            {
                currentPlayer.dps.Add(BuildDPS(_statistics.Dps[player][phaseIndex]));
            }

            return currentPlayer;
        }

        private JSONLog SetPhases(JSONLog log)
        {
            log.phases = new ArrayList();

            foreach (var phase in _statistics.Phases)
            {
                log.phases.Add(new JSONLog.Phase
                {
                    duration = phase.GetDuration(),
                    name = phase.GetName()
                });
            }

            return log;
        }
    }
}