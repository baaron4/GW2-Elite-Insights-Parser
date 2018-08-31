using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public void CreateJSON()
        {
            var log = new JsonLog();

            SetGeneral(log);
            SetBoss(log);
            SetPlayers(log);
            SetPhases(log);
            SetMechanics(log);

            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            var writer = new JsonTextWriter(_sw);
            writer.Formatting = _settings.IndentJSON ? Formatting.Indented : Formatting.None;
            serializer.Serialize(writer, log);
        }

        private void SetGeneral(JsonLog log)
        {
            double fightDuration = _log.GetBossData().GetAwareDuration() / 1000.0;
            var duration = TimeSpan.FromSeconds(fightDuration);
            string durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s";
            if (duration.ToString("hh") != "00")
            {
                durationString = duration.ToString("hh") + "h " + durationString;
            }

            log.EliteInsightsVersion = Application.ProductVersion;
            log.ArcVersion = _log.GetLogData().GetBuildVersion();
            log.RecordedBy = _log.GetLogData().GetPOV().Split(':')[0].TrimEnd('\u0000');
            log.TimeStart = _log.GetLogData().GetLogStart();
            log.TimeEnd = _log.GetLogData().GetLogEnd();
            log.Duration = durationString;
            log.Success = _log.GetLogData().GetBosskill();
        }

        private void SetMechanics(JsonLog log)
        {
            MechanicData mechanicData = _log.GetMechanicData();
            var mechanicLogs = new List<MechanicLog>();
            foreach (var mLog in mechanicData.Values)
            {
                mechanicLogs.AddRange(mLog);
            }
            mechanicLogs = mechanicLogs.OrderBy(x => x.GetTime()).ToList();
            if (mechanicLogs.Any())
            {
                log.Mechanics = new JsonLog.JsonMechanic[mechanicLogs.Count];
                for (int i = 0; i < mechanicLogs.Count; i++)
                {
                    log.Mechanics[i] = new JsonLog.JsonMechanic
                    {
                        Time = mechanicLogs[i].GetTime(),
                        Player = mechanicLogs[i].GetPlayer().GetCharacter(),
                        Description = mechanicLogs[i].GetDescription(),
                        Skill = mechanicLogs[i].GetSkill()
                    };
                }
            }
        }

        private void SetBoss(JsonLog log)
        {
            log.Boss.Id = _log.GetBossData().GetID();
            log.Boss.Name = _log.GetBossData().GetName();
            log.Boss.TotalHealth = _log.GetBossData().GetHealth();
            int finalBossHealth = _log.GetBossData().GetHealthOverTime().Count > 0
                ? _log.GetBossData().GetHealthOverTime().Last().Y
                : 10000;
            log.Boss.FinalHealth = _log.GetBossData().GetHealth() * (100.0 - finalBossHealth * 0.01);
            log.Boss.HealthPercentBurned = 100.0 - finalBossHealth * 0.01;

            log.Boss.Dps = BuildDPS(_statistics.BossDps);
            log.Boss.Conditions = BuildBossBoons(_statistics.BossConditions);
        }

        private void SetPlayers(JsonLog log)
        {
            log.Players = new List<JsonLog.JsonPlayer>();

            foreach (var player in _log.GetPlayerList())
            {
                log.Players.Add(new JsonLog.JsonPlayer
                {
                    Character = player.GetCharacter(),
                    Account = player.GetAccount(),
                    Condition = player.GetCondition(),
                    Concentration = player.GetConcentration(),
                    Healing = player.GetHealing(),
                    Toughness = player.GetToughness(),
                    Weapons = player.GetWeaponsArray(_log).Where(w => w != null).ToArray(),
                    Group = player.GetGroup(),
                    Profession = player.GetProf(),
                    Dps = BuildDPS(_statistics.Dps[player]),
                    Stats = BuildStats(_statistics.Stats[player]),
                    Defenses = BuildDefenses(_statistics.Defenses[player]),
                    Support = BuildSupport(_statistics.Support[player]),
                    SelfBoons = BuildBoonUptime(_statistics.SelfBoons[player]),
                    GroupBoons = BuildBoonUptime(_statistics.GroupBoons[player]),
                    OffGroupBoons = BuildBoonUptime(_statistics.OffGroupBoons[player]),
                    SquadBoons = BuildBoonUptime(_statistics.SquadBoons[player])
                });
            }
        }

        private void SetPhases(JsonLog log)
        {
            log.Phases = new List<JsonLog.JsonPhase>();

            foreach (var phase in _statistics.Phases)
            {
                log.Phases.Add(new JsonLog.JsonPhase
                {
                    Duration = phase.GetDuration(),
                    Name = phase.GetName()
                });
            }
        }

        // Statistics to Json Converters ////////////////////////////////////////////////////

        private bool ContainsBossBoon(long boon, Dictionary<long, Statistics.FinalBossBoon>[] statBoons)
        {
            for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
            {
                if (statBoons[phaseIndex][boon].Uptime > 0) return true;
                if (statBoons[phaseIndex][boon].Generated.Any(x => x.Value > 0)) return true;
                if (statBoons[phaseIndex][boon].Overstacked.Any(x => x.Value > 0)) return true;
            }

            return false;
        }

        private void MakePhaseBossBoon(JsonLog.JsonBossBoon boon, int phase, Statistics.FinalBossBoon value)
        {
            boon.Uptime[phase] = value.Uptime;
            boon.Generated[phase] = boon.Generated[phase] ?? new Dictionary<string, double>();
            boon.Overstacked[phase] = boon.Overstacked[phase] ?? new Dictionary<string, double>();

            foreach (var playerBoon in value.Generated.Where(x => x.Value > 0))
            {
                boon.Generated[phase][playerBoon.Key.GetCharacter()] = playerBoon.Value;
            }

            foreach (var playerBoon in value.Overstacked.Where(x => x.Value > 0))
            {
                boon.Overstacked[phase][playerBoon.Key.GetCharacter()] = playerBoon.Value;
            }
        }

        private Dictionary<long, JsonLog.JsonBossBoon> BuildBossBoons(Dictionary<long, Statistics.FinalBossBoon>[] statBoons)
        {
            int phases = _statistics.Phases.Count;
            var boons = new Dictionary<long, JsonLog.JsonBossBoon>();

            var boonsFound = new List<long>();
            var boonsNotFound = new List<long>();

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                foreach (var boon in statBoons[phaseIndex])
                {
                    if (boonsFound.Contains(boon.Key))
                    {
                        MakePhaseBossBoon(boons[boon.Key], phaseIndex, boon.Value);
                    }
                    else if (!boonsNotFound.Contains(boon.Key))
                    {
                        if (ContainsBossBoon(boon.Key, statBoons))
                        {
                            boonsFound.Add(boon.Key);

                            boons[boon.Key] = new JsonLog.JsonBossBoon(phases);
                            MakePhaseBossBoon(boons[boon.Key], phaseIndex, boon.Value);
                        }
                        else
                        {
                            boonsNotFound.Add(boon.Key);
                        }
                    }
                }
            }

            return boons;
        }

        private JsonLog.JsonDps BuildDPS(Statistics.FinalDPS[] statDps)
        {
            var dps = new JsonLog.JsonDps(_statistics.Phases.Count);

            MoveArrayLevel(dps, _statistics.Phases.Count, statDps);
            RemoveZeroArrays(dps);

            return dps;
        }

        private bool ContainsBoon(long boon, Dictionary<long, Statistics.FinalBoonUptime>[] statUptimes)
        {
            int phases = _statistics.Phases.Count;
            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                if (statUptimes[phaseIndex][boon].Uptime > 0) return true;
                if (statUptimes[phaseIndex][boon].Generation > 0) return true;
                if (statUptimes[phaseIndex][boon].Overstack > 0) return true;
            }

            return false;
        }

        private void MakePhaseBoon(JsonLog.JsonBoonUptime boon, int phase, Statistics.FinalBoonUptime value)
        {
            boon.Overstack[phase] = value.Overstack;
            boon.Generation[phase] = value.Generation;
            boon.Uptime[phase] = value.Uptime;
        }

        private Dictionary<long, JsonLog.JsonBoonUptime> BuildBoonUptime(Dictionary<long, Statistics.FinalBoonUptime>[] statUptimes)
        {
            var uptimes = new Dictionary<long, JsonLog.JsonBoonUptime>();
            int phases = _statistics.Phases.Count;

            var boonsFound = new List<long>();
            var boonsNotFound = new List<long>();

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                foreach (var boon in statUptimes[phaseIndex])
                {
                    if (boonsFound.Contains(boon.Key))
                    {
                        MakePhaseBoon(uptimes[boon.Key], phaseIndex, boon.Value);
                    }
                    else if (!boonsNotFound.Contains(boon.Key))
                    {
                        if (ContainsBoon(boon.Key, statUptimes))
                        {
                            boonsFound.Add(boon.Key);

                            uptimes[boon.Key] = new JsonLog.JsonBoonUptime(phases);
                            MakePhaseBoon(uptimes[boon.Key], phaseIndex, boon.Value);
                        }
                        else
                        {
                            boonsNotFound.Add(boon.Key);
                        }
                    }
                }
            }

            if (!uptimes.Any()) return null;

            foreach (var boon in uptimes)
            {
                RemoveZeroArrays(boon.Value);
            }

            return uptimes;
        }

        private JsonLog.JsonSupport BuildSupport(Statistics.FinalSupport[] statSupport)
        {
            var support = new JsonLog.JsonSupport(_statistics.Phases.Count);

            MoveArrayLevel(support, _statistics.Phases.Count, statSupport);
            RemoveZeroArrays(support);

            return support;
        }

        private JsonLog.JsonDefenses BuildDefenses(Statistics.FinalDefenses[] statDefense)
        {
            var defense = new JsonLog.JsonDefenses(_statistics.Phases.Count);

            MoveArrayLevel(defense, _statistics.Phases.Count, statDefense);
            RemoveZeroArrays(defense);

            return defense;
        }

        private JsonLog.JsonStats BuildStats(Statistics.FinalStats[] statStat)
        {
            var stats = new JsonLog.JsonStats(_statistics.Phases.Count);

            MoveArrayLevel(stats, _statistics.Phases.Count, statStat);
            RemoveZeroArrays(stats);

            return stats;
        }

        // Null all arrays only consisting of zeros(or below!) by using reflection
        private void RemoveZeroArrays(object inObject)
        {
            FieldInfo[] fields = inObject.GetType().GetFields();
            foreach (var field in fields)
            {
                if (!field.FieldType.IsArray) continue;
                var entry = ((IEnumerable)field.GetValue(inObject)).Cast<object>().ToArray();
                if (!entry.Any(e => Convert.ToDouble(e) > 0)) field.SetValue(inObject, null);
            }
        }

        private void MoveArrayLevel(object lowerObject, int count, object[] upperObject)
        {
            for (int i = 0; i < count; i++)
            {
                FieldInfo[] fields = lowerObject.GetType().GetFields();
                foreach (var field in fields)
                {
                    if (!field.FieldType.IsArray) continue;
                    var entry = (Array) field.GetValue(lowerObject);
                    entry.SetValue(upperObject[i].GetType().GetField(field.Name).GetValue(upperObject[i]), i);
                }
            }
        }
    }
}