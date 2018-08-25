using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private struct JSONLog
        {
            public struct Boss
            {
                public string name;
                public ushort id;
                public int totalHealth;
                public double finalHealth;
                public double healthPercentBurned;
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

        //Creating JSON---------------------------------------------------------------------------------
        public void CreateJSON()
        {
            JSONLog log = new JSONLog();

            double fightDuration = (_log.GetBossData().GetAwareDuration()) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fightDuration);
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

            log.boss.id = _log.GetBossData().GetID();
            log.boss.name = _log.GetBossData().GetName();
            log.boss.totalHealth = _log.GetBossData().GetHealth();
            int finalBossHealth = _log.GetBossData().GetHealthOverTime().Count > 0
                ? _log.GetBossData().GetHealthOverTime().Last().Y
                : 10000;
            log.boss.finalHealth = _log.GetBossData().GetHealth() * (100.0 - finalBossHealth * 0.01);
            log.boss.healthPercentBurned = 100.0 - finalBossHealth * 0.01;

            log = SetPlayers(log);
            log = SetPhases(log);

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            JsonWriter writer = new JsonTextWriter(_sw);
            writer.Formatting = Formatting.Indented;
            serializer.Serialize(writer, log);
        }

        private JSONLog SetPlayers(JSONLog log)
        {
            log.players = new ArrayList();

            foreach (var player in _log.GetPlayerList())
            {
                JSONLog.Player currentPlayer = new JSONLog.Player();

                currentPlayer.character = player.GetCharacter();
                currentPlayer.account = player.GetAccount();

                currentPlayer.condition = player.GetCondition();
                currentPlayer.concentration = player.GetConcentration();
                currentPlayer.healing = player.GetHealing();
                currentPlayer.toughness = player.GetToughness();
                currentPlayer.weapons = player.GetWeaponsArray(_log);
                currentPlayer.group = player.GetGroup();
                currentPlayer.profession = player.GetProf();

                log.players.Add(currentPlayer);
            }

            return log;
        }

        private JSONLog SetPhases(JSONLog log)
        {
            log.phases = new ArrayList();

            foreach (var phase in _statistics.Phases)
            {
                JSONLog.Phase currentPhase = new JSONLog.Phase();

                currentPhase.duration = phase.GetDuration();
                currentPhase.name = phase.GetName();

                log.phases.Add(currentPhase);
            }

            return log;
        }
    }
}