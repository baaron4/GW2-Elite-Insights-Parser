using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using LuckParser.Models;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using Newtonsoft.Json;
using static LuckParser.Models.DataModels.JsonLog;

namespace LuckParser.Controllers
{
    class JSONBuilder
    {
        readonly SettingsContainer _settings;

        readonly ParsedLog _log;

        readonly Statistics _statistics;
        readonly StreamWriter _sw;

        private readonly String[] _uploadLink;

        private Dictionary<long, string> _boonDict;

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

        public JSONBuilder(StreamWriter sw, ParsedLog log, SettingsContainer settings, Statistics statistics, string[] UploadString)
        {
            _log = log;
            _sw = sw;
            _settings = settings;

            _statistics = statistics;

            _boonDict = Boon.GetAll().GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.Select(y => y.Name).First());
            
           _uploadLink = UploadString;
        }

        public void CreateJSON()
        {
            var log = new JsonLog();

            SetGeneral(log);
            SetBoss(log);
            SetPlayers(log);
            SetPhases(log);
            SetMechanics(log);

            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var writer = new JsonTextWriter(_sw)
            {
                Formatting = _settings.IndentJSON ? Formatting.Indented : Formatting.None
            };
            serializer.Serialize(writer, log);
        }

        private void SetGeneral(JsonLog log)
        {
            double fightDuration = _log.FightData.FightDuration / 1000.0;
            var duration = TimeSpan.FromSeconds(fightDuration);
            string durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s";
            if (duration.ToString("hh") != "00")
            {
                durationString = duration.ToString("hh") + "h " + durationString;
            }
            log.TriggerID = _log.FightData.ID;
            log.FightName = _log.FightData.Name;
            log.EliteInsightsVersion = Application.ProductVersion;
            log.ArcVersion = _log.LogData.BuildVersion;
            log.RecordedBy = _log.LogData.PoV.Split(':')[0].TrimEnd('\u0000');
            log.TimeStart = _log.LogData.LogStart;
            log.TimeEnd = _log.LogData.LogEnd;
            log.Duration = durationString;
            log.Success = _log.LogData.Success;
            log.UploadLinks = _uploadLink;
        }

        private void SetMechanics(JsonLog log)
        {
            MechanicData mechanicData = _log.MechanicData;
            var mechanicLogs = new List<MechanicLog>();
            foreach (var mLog in mechanicData.Values)
            {
                mechanicLogs.AddRange(mLog);
            }
            mechanicLogs = mechanicLogs.OrderBy(x => x.Time).ToList();
            if (mechanicLogs.Any())
            {
                log.Mechanics = new List<JsonMechanic>(capacity: mechanicLogs.Count);
                for (int i = 0; i < mechanicLogs.Count; i++)
                {
                    log.Mechanics.Add(new JsonMechanic
                    {
                        Time = mechanicLogs[i].Time,
                        Name = mechanicLogs[i].Name,
                        Player = mechanicLogs[i].Player.Character,
                        Description = mechanicLogs[i].Description,
                        Skill = mechanicLogs[i].Skill
                    });
                }
            }
        }

        private void SetBoss(JsonLog log)
        {
            log.Boss = new List<JsonBoss>();
            foreach (Boss target in _log.FightData.Logic.Targets)
            {
                JsonBoss boss = new JsonBoss();
                boss.Id = target.ID;
                boss.Name = target.Character;
                boss.TotalHealth = target.Health;
                int finalBossHealth = target.HealthOverTime.Count > 0
                    ? target.HealthOverTime.Last().Y
                    : 10000;
                boss.FinalHealth = target.Health * (finalBossHealth * 0.01);
                boss.HealthPercentBurned = 100.0 - finalBossHealth * 0.01;
                boss.AvgBoons = _statistics.AvgBossBoons[target];
                boss.AvgConditions = _statistics.AvgBossConditions[target];
                boss.Dps = BuildDPS(_statistics.BossDps[target]);
                boss.Conditions = BuildBossBuffs(_statistics.BossConditions[target]);
                boss.HitboxHeight = target.HitboxHeight;
                boss.HitboxWidth = target.HitboxWidth;
                boss.Dps1s = Build1SDPS(target, null);
                boss.Rotation = BuildRotation(target);
                boss.FirstAware = (int)(target.FirstAware - _log.FightData.FightStart);
                boss.LastAware = (int)(target.LastAware - _log.FightData.FightStart);
                boss.TotalPersonalDamage = BuildPersonalDamage(target, null);
                log.Boss.Add(boss);
            }
        }

        private void SetPlayers(JsonLog log)
        {
            log.Players = new List<JsonPlayer>();

            foreach (var player in _log.PlayerList)
            {
                log.Players.Add(new JsonPlayer
                {
                    Character = player.Character,
                    Account = player.Account,
                    Condition = player.Condition,
                    Concentration = player.Concentration,
                    Healing = player.Healing,
                    Toughness = player.Toughness,
                    Weapons = player.GetWeaponsArray(_log).Where(w => w != null).ToArray(),
                    Group = player.Group,
                    Profession = player.Prof,
                    TotalPersonalDamage = BuildPersonalDamage(player, null),
                    TargetPersonalDamage = BuildTargetPersonalDamage(player),
                    Dps1s = Build1SDPS(player, null),
                    TargetDps1s = Build1SDPS(player),
                    DpsAll = BuildDPS(_statistics.DpsAll[player]),
                    DpsBoss = BuildDPSBoss(_statistics.DpsBoss, player),
                    StatsAll = BuildStatsAll(_statistics.StatsAll[player]),
                    StatsBoss = BuildStatsBoss(_statistics.StatsBoss, player),
                    Defenses = BuildDefenses(_statistics.Defenses[player]),
                    Rotation = BuildRotation(player),
                    Support = BuildSupport(_statistics.Support[player]),
                    SelfBoons = BuildBuffUptime(_statistics.SelfBoons[player]),
                    GroupBoons = BuildBuffUptime(_statistics.GroupBoons[player]),
                    OffGroupBoons = BuildBuffUptime(_statistics.OffGroupBoons[player]),
                    SquadBoons = BuildBuffUptime(_statistics.SquadBoons[player])
                });
            }
        }


        private List<int> Build1SDPS(AbstractMasterPlayer player, Boss target)
        {
            List<int> res = new List<int>();
            foreach (var pt in GraphHelper.GetBossDPSGraph(_log, player, 0, _statistics.Phases[0], GraphHelper.GraphMode.S1, target))
            {
                res.Add(pt.Y);
            }
            return res;
        }

        private List<int>[] Build1SDPS(AbstractMasterPlayer player)
        {
            List<int>[] res = new List<int>[_log.FightData.Logic.Targets.Count];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                res[i] = Build1SDPS(player, _log.FightData.Logic.Targets[i]);
            }
            return res;
        }

        private int[] BuildPersonalDamage(AbstractMasterPlayer player, Boss target)
        {
            int[] res = new int[_statistics.Phases.Count];
            for(int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phase = _statistics.Phases[i];
                res[i] = player.GetJustPlayerDamageLogs(target, _log, phase.Start, phase.End).Sum(x => x.Damage);
            }
            return res;
        }

        private int[][] BuildTargetPersonalDamage(AbstractMasterPlayer player)
        {
            int[][] res = new int[_log.FightData.Logic.Targets.Count][];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                res[i] = BuildPersonalDamage(player, _log.FightData.Logic.Targets[i]);
            }
            return res;
        }

        private List<JsonSkill> BuildRotation(AbstractMasterPlayer player)
        {
            List<JsonSkill> res = new List<JsonSkill>();
            List<CastLog> cls = player.GetCastLogs(_log, 0, _log.FightData.FightDuration);
            SkillData skillList = _log.SkillData;
            foreach (CastLog cl in cls)
            {
                GW2APISkill skill = skillList.Get(cl.SkillId)?.ApiSkill;
                string skillName = skill == null ? skillList.GetName(cl.SkillId) : skill.name;
                if (cl.SkillId == SkillItem.WeaponSwapId)
                {
                    skillName = "Weapon Swap";
                }
                res.Add(new JsonSkill
                {
                    Name = skillName,
                    Time = (int)cl.Time,
                    UnderQuickness = cl.StartActivation == ParseEnum.Activation.Quickness,
                    TimeSaved = cl.EndActivation == ParseEnum.Activation.CancelFire ? (cl.ActualDuration < cl.ExpectedDuration ? cl.ExpectedDuration - cl.ActualDuration : 0) : 0,
                    TimeWasted = cl.EndActivation == ParseEnum.Activation.CancelCancel ? cl.ActualDuration : 0,
                    Auto = skill != null && skill.slot == "Weapon_1"
                });
            }

            return res;
        }

        private void SetPhases(JsonLog log)
        {
            log.Phases = new List<JsonPhase>();

            foreach (var phase in _statistics.Phases)
            {
                JsonPhase phaseJson = new JsonPhase
                {
                    Start = phase.Start,
                    End = phase.End,
                    Name = phase.Name
                };
                phaseJson.TargetIds = new int[phase.Targets.Count];
                int i = 0;
                foreach (Boss target in phase.Targets)
                {
                    phaseJson.TargetIds[i++] = _log.FightData.Logic.Targets.IndexOf(target);
                }
                log.Phases.Add(phaseJson);
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

        private void MakePhaseBossBoon(JsonBossBuffs boon, int phase, Statistics.FinalBossBoon value)
        {
            boon.Uptime[phase] = value.Uptime;
            boon.Presence[phase] = value.Presence;
            boon.Generated[phase] = boon.Generated[phase] ?? new Dictionary<string, double>();
            boon.Overstacked[phase] = boon.Overstacked[phase] ?? new Dictionary<string, double>();

            foreach (var playerBoon in value.Generated.Where(x => x.Value > 0))
            {
                boon.Generated[phase][playerBoon.Key.Character] = playerBoon.Value;
            }

            foreach (var playerBoon in value.Overstacked.Where(x => x.Value > 0))
            {
                boon.Overstacked[phase][playerBoon.Key.Character] = playerBoon.Value;
            }
        }

        private Dictionary<string, JsonBossBuffs> BuildBossBuffs(Dictionary<long, Statistics.FinalBossBoon>[] statBoons)
        {
            int phases = _statistics.Phases.Count;
            var boons = new Dictionary<long, JsonBossBuffs>();

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

                            boons[boon.Key] = new JsonBossBuffs(phases);
                            MakePhaseBossBoon(boons[boon.Key], phaseIndex, boon.Value);
                        }
                        else
                        {
                            boonsNotFound.Add(boon.Key);
                        }
                    }
                }
            }

            var namedBoons = new Dictionary<string, JsonBossBuffs>();
            foreach (var entry in boons)
            {
                if (_boonDict.ContainsKey(entry.Key)) {
                    namedBoons[_boonDict[entry.Key]] = entry.Value;
                }
                else
                {
                    namedBoons[entry.Key.ToString()] = entry.Value;
                }
            }

            return namedBoons;
        }

        private JsonDps BuildDPS(Statistics.FinalDPS[] statDps)
        {
            var dps = new JsonDps(_statistics.Phases.Count);

            MoveArrayLevel(dps, _statistics.Phases.Count, statDps);
            RemoveZeroArrays(dps);

            return dps;
        }

        private JsonDps[] BuildDPSBoss(Dictionary<Boss, Dictionary<Player, Statistics.FinalDPS[]>> statDps, Player player)
        {
            var finalDps = new JsonDps[_log.FightData.Logic.Targets.Count];
            int i = 0;
            foreach (Boss target in _log.FightData.Logic.Targets)
            {
                finalDps[i++] = BuildDPS(statDps[target][player]);
            }
            return finalDps;
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

        private void MakePhaseBoon(JsonBuffs boon, int phase, Statistics.FinalBoonUptime value)
        {
            boon.Overstack[phase] = value.Overstack;
            boon.Generation[phase] = value.Generation;
            boon.Uptime[phase] = value.Uptime;
            boon.Presence[phase] = value.Presence;
        }

        private Dictionary<string, JsonBuffs> BuildBuffUptime(Dictionary<long, Statistics.FinalBoonUptime>[] statUptimes)
        {
            var uptimes = new Dictionary<long, JsonBuffs>();
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

                            uptimes[boon.Key] = new JsonBuffs(phases);
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

            var namedBoons = new Dictionary<string, JsonBuffs>();
            foreach (var entry in uptimes)
            {
                if (_boonDict.ContainsKey(entry.Key)) {
                    namedBoons[_boonDict[entry.Key]] = entry.Value;
                }
                else
                {
                    namedBoons[entry.Key.ToString()] = entry.Value;
                }
            }

            return namedBoons;
        }

        private JsonSupport BuildSupport(Statistics.FinalSupport[] statSupport)
        {
            var support = new JsonSupport(_statistics.Phases.Count);

            MoveArrayLevel(support, _statistics.Phases.Count, statSupport);
            RemoveZeroArrays(support);

            return support;
        }

        private JsonDefenses BuildDefenses(Statistics.FinalDefenses[] statDefense)
        {
            var defense = new JsonDefenses(_statistics.Phases.Count);

            MoveArrayLevel(defense, _statistics.Phases.Count, statDefense);
            RemoveZeroArrays(defense);

            return defense;
        }

        private JsonStatsAll BuildStatsAll(Statistics.FinalStatsAll[] statStat)
        {
            var stats = new JsonStatsAll(_statistics.Phases.Count);

            MoveArrayLevel(stats, _statistics.Phases.Count, statStat);
            RemoveZeroArrays(stats);

            return stats;
        }

        private JsonStats BuildStatsBoss(Statistics.FinalStats[] statStat)
        {
            var stats = new JsonStats(_statistics.Phases.Count);

            MoveArrayLevel(stats, _statistics.Phases.Count, statStat);
            RemoveZeroArrays(stats);

            return stats;
        }

        private JsonStats[] BuildStatsBoss(Dictionary<Boss, Dictionary<Player,Statistics.FinalStats[]>> statStat, Player player)
        {
            var finalStats = new JsonStats[_log.FightData.Logic.Targets.Count];
            int i = 0;
            foreach (Boss target in _log.FightData.Logic.Targets)
            {
                finalStats[i++] = BuildStatsBoss(statStat[target][player]);
            }
            return finalStats;
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