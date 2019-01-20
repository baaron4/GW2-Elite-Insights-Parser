using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using LuckParser.Controllers;
using LuckParser.Models;
using LuckParser.Parser;
using LuckParser.Models.JsonModels;
using LuckParser.Models.ParseModels;
using Newtonsoft.Json;
using LuckParser.Setting;
using Newtonsoft.Json.Serialization;

namespace LuckParser.Builders
{
    class RawFormatBuilder
    {
        readonly SettingsContainer _settings;

        readonly ParsedLog _log;

        readonly Statistics _statistics;
        readonly StreamWriter _sw;

        private readonly string[] _uploadLink;
        //
        private readonly Dictionary<string, string> _skillNames = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _buffNames = new Dictionary<string, string>();
        private readonly Dictionary<string, HashSet<long>> _personalBuffs = new Dictionary<string, HashSet<long>>();

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

        public RawFormatBuilder(StreamWriter sw, ParsedLog log, SettingsContainer settings, Statistics statistics, string[] UploadString)
        {
            _log = log;
            _sw = sw;
            _settings = settings;

            _statistics = statistics;
            
           _uploadLink = UploadString;
        }

        public void CreateJSON()
        {
            var log = new JsonLog();

            SetGeneral(log);
            SetTargets(log);
            SetPlayers(log);
            SetPhases(log);
            SetMechanics(log);

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = contractResolver
            };
            var writer = new JsonTextWriter(_sw)
            {
                Formatting = _settings.IndentJSON ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None
            };
            serializer.Serialize(writer, log);
        }

        public void CreateXML()
        {
            var log = new JsonLog();

            SetGeneral(log);
            SetTargets(log);
            SetPlayers(log);
            SetPhases(log);
            SetMechanics(log);

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = contractResolver
            };
            Dictionary<string, JsonLog> root = new Dictionary<string, JsonLog>()
            {
                {"log", log }
            };
            string json = JsonConvert.SerializeObject(root, settings);

            XmlDocument xml = JsonConvert.DeserializeXmlNode(json);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(_sw)
            {
                Formatting = _settings.IndentXML ? System.Xml.Formatting.Indented : System.Xml.Formatting.None
            };

            xml.WriteTo(xmlTextWriter);
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
            log.Success = _log.FightData.Success;
            log.SkillNames = _skillNames;
            log.BuffNames = _buffNames;
            log.PersonalBuffs = _personalBuffs;
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
            if (mechanicLogs.Any())
            {
                log.Mechanics = new Dictionary<string, List<JsonMechanic>>();
                foreach (MechanicLog ml in mechanicLogs)
                {
                    JsonMechanic mech = new JsonMechanic
                    {
                        Time = ml.Time,
                        Actor = ml.Actor.Character
                    };
                    if (log.Mechanics.TryGetValue(ml.InGameName, out var list))
                    {
                        list.Add(mech);
                    }
                    else
                    {
                        log.Mechanics[ml.InGameName] = new List<JsonMechanic>()
                        {
                            mech
                        };
                    }
                }
            }
        }

        private void SetTargets(JsonLog log)
        {
            log.Targets = new List<JsonTarget>();
            foreach (Target target in _log.FightData.Logic.Targets)
            {
                JsonTarget jsTarget = new JsonTarget
                {
                    Id = target.ID,
                    Name = target.Character,
                    Toughness = target.Toughness,
                    Healing = target.Healing,
                    Concentration = target.Concentration,
                    Condition = target.Condition,
                    TotalHealth = target.Health,
                    AvgBoons = _statistics.AvgTargetBoons[target],
                    AvgConditions = _statistics.AvgTargetConditions[target],
                    DpsAll = _statistics.TargetDps[target],
                    Buffs = BuildTargetBuffs(_statistics.TargetBuffs[target], target),
                    HitboxHeight = target.HitboxHeight,
                    HitboxWidth = target.HitboxWidth,
                    Damage1S = BuildTotal1SDamage(target),
                    Rotation = BuildRotation(target.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    FirstAware = (int)(_log.FightData.ToFightSpace(target.FirstAware)),
                    LastAware = (int)(_log.FightData.ToFightSpace(target.LastAware)),
                    Minions = BuildMinions(target),
                    TotalDamageDist = BuildDamageDist(target, null),
                    TotalDamageTaken = BuildDamageTaken(target),
                    AvgBoonsStates = BuildBuffStates(target.GetBoonGraphs(_log)[Boon.NumberOfBoonsID]),
                    AvgConditionsStates = BuildBuffStates(target.GetBoonGraphs(_log)[Boon.NumberOfConditionsID])
                };
                int finalTargetHealth = target.HealthOverTime.Count > 0
                    ? target.HealthOverTime.Last().Y
                    : 10000;
                jsTarget.FinalHealth = target.Health * (finalTargetHealth * 0.01);
                jsTarget.HealthPercentBurned = 100.0 - finalTargetHealth * 0.01;
                log.Targets.Add(jsTarget);
            }
        }

        private void SetPlayers(JsonLog log)
        {
            log.Players = new List<JsonPlayer>();

            foreach (var player in _log.PlayerList)
            {
                log.Players.Add(new JsonPlayer
                {
                    Name = player.Character,
                    Account = player.Account,
                    Condition = player.Condition,
                    Concentration = player.Concentration,
                    Healing = player.Healing,
                    Toughness = player.Toughness,
                    HitboxHeight = player.HitboxHeight,
                    HitboxWidth = player.HitboxWidth,
                    Weapons = player.GetWeaponsArray(_log).Where(w => w != null).ToArray(),
                    Group = player.Group,
                    Profession = player.Prof,
                    Damage1S = BuildTotal1SDamage(player),
                    TargetDamage1S = BuildTarget1SDamage(player),
                    DpsAll = _statistics.DpsAll[player],
                    DpsTargets = BuildDPSTarget(_statistics.DpsTarget, player),
                    StatsAll = _statistics.StatsAll[player],
                    StatsTargets = BuildStatsTarget(_statistics.StatsTarget, player),
                    Defenses = _statistics.Defenses[player],
                    Rotation = BuildRotation(player.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    Support = _statistics.Support[player],
                    SelfBuffs = BuildBuffUptime(_statistics.SelfBuffs[player], player),
                    GroupBuffs = BuildBuffUptime(_statistics.GroupBuffs[player], player),
                    OffGroupBuffs = BuildBuffUptime(_statistics.OffGroupBuffs[player], player),
                    SquadBuffs = BuildBuffUptime(_statistics.SquadBuffs[player], player),
                    DamageModifiers = BuildDamageModifiers(player.GetExtraBoonData(_log, null)),
                    DamageModifiersTarget = BuildDamageModifiersTarget(player),
                    Minions = BuildMinions(player),
                    TotalDamageDist = BuildDamageDist(player, null),
                    TargetDamageDist = BuildDamageDist(player),
                    TotalDamageTaken = BuildDamageTaken(player),
                    DeathRecap = player.GetDeathRecaps(_log),
                    Consumables = BuildConsumables(player),
                    AvgBoonsStates = BuildBuffStates(player.GetBoonGraphs(_log)[Boon.NumberOfBoonsID]),
                    AvgConditionsStates = BuildBuffStates(player.GetBoonGraphs(_log)[Boon.NumberOfConditionsID]),
                });
            }
        }

        private List<int>[] BuildTotal1SDamage(AbstractMasterActor p)
        {
            List<int>[] list = new List<int>[_statistics.Phases.Count];
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                list[i] = p.Get1SDamageList(_log, i, _statistics.Phases[i], null);
            }
            return list;
        }

        private List<int>[][] BuildTarget1SDamage(Player p)
        {
            List<int>[][] tarList = new List<int>[_log.FightData.Logic.Targets.Count][];
            for (int j = 0; j < _log.FightData.Logic.Targets.Count; j++)
            {
                Target target = _log.FightData.Logic.Targets[j];
                List<int>[] list = new List<int>[_statistics.Phases.Count];
                for (int i = 0; i < _statistics.Phases.Count; i++)
                {
                    list[i] = p.Get1SDamageList(_log, i, _statistics.Phases[i], target);
                }
                tarList[j] = list;
            }
            return tarList;
        }

        private Statistics.FinalDPS[][] BuildDPSTarget(Dictionary<Target, Dictionary<Player, Statistics.FinalDPS[]>> stats, Player p)
        {
            Statistics.FinalDPS[][] res = new Statistics.FinalDPS[_log.FightData.Logic.Targets.Count][];
            int i = 0;
            foreach (Target tar in _log.FightData.Logic.Targets)
            {
                res[i++] = stats[tar][p];
            }
            return res;
        }

        private Statistics.FinalStats[][] BuildStatsTarget(Dictionary<Target, Dictionary<Player, Statistics.FinalStats[]>> stats, Player p)
        {
            Statistics.FinalStats[][] res = new Statistics.FinalStats[_log.FightData.Logic.Targets.Count][];
            int i = 0;
            foreach (Target tar in _log.FightData.Logic.Targets)
            {
                res[i++] = stats[tar][p];
            }
            return res;
        }

        private Dictionary<string, List<AbstractMasterActor.ExtraBoonData>> BuildDamageModifiers(Dictionary<long, List<AbstractMasterActor.ExtraBoonData>> extra)
        {
            Dictionary<string, List<AbstractMasterActor.ExtraBoonData>> res = new Dictionary<string, List<AbstractMasterActor.ExtraBoonData>>();
            foreach (long key in extra.Keys)
            {
                _buffNames["b" + key] = Boon.BoonsByIds[key].Name;
                res["b" + key] = extra[key];
            }
            return res;
        }

        private Dictionary<string, List<AbstractMasterActor.ExtraBoonData>>[] BuildDamageModifiersTarget(Player p)
        {
            Dictionary<string, List<AbstractMasterActor.ExtraBoonData>>[] res = new Dictionary<string, List<AbstractMasterActor.ExtraBoonData>>[_log.FightData.Logic.Targets.Count];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                Target tar = _log.FightData.Logic.Targets[i];
                res[i] = BuildDamageModifiers(p.GetExtraBoonData(_log, tar));
            }
            return res;
        }

        private List<Player.Consumable> BuildConsumables(Player player)
        {
            List<Player.Consumable> res = player.GetConsumablesList(_log, 0, _log.FightData.FightDuration);
            foreach(var food in res)
            {
                _buffNames["b"+food.Item.ID] = food.Item.Name;
            }
            return res.Count > 0 ? res : null;
        }

        private List<int[]> BuildBuffStates(BoonsGraphModel bgm)
        {
            if (bgm == null || bgm.BoonChart.Count == 0)
            {
                return null;
            }
            List<int[]> res = new List<int[]>();
            foreach (var seg in bgm.BoonChart)
            {
                res.Add(new int[2] {
                    (int)seg.Start,
                    seg.Value
                });
            }
            return res.Count > 0 ? res : null;
        }

        private Dictionary<string, JsonDamageDist>[][] BuildDamageDist(AbstractMasterActor p)
        {
            Dictionary<string, JsonDamageDist>[][] res = new Dictionary<string, JsonDamageDist>[_log.FightData.Logic.Targets.Count][];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                Target target = _log.FightData.Logic.Targets[i];
                res[i] = BuildDamageDist(p, target);
            }
            return res;
        }

        private Dictionary<string, JsonDamageDist>[][] BuildDamageDist(Minions p)
        {
            Dictionary<string, JsonDamageDist>[][] res = new Dictionary<string, JsonDamageDist>[_log.FightData.Logic.Targets.Count][];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                Target target = _log.FightData.Logic.Targets[i];
                res[i] = BuildDamageDist(p, target);
            }
            return res;
        }

        private Dictionary<string, JsonDamageDist>[] BuildDamageDist(AbstractMasterActor p, Target target)
        {
            Dictionary<string, JsonDamageDist>[] res = new Dictionary<string, JsonDamageDist>[_statistics.Phases.Count];
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phase = _statistics.Phases[i];
                res[i] = BuildDamageDist(p.GetJustPlayerDamageLogs(target, _log, phase.Start, phase.End));
            }
            return res;
        }

        private Dictionary<string, JsonDamageDist>[] BuildDamageTaken(AbstractMasterActor p)
        {
            Dictionary<string, JsonDamageDist>[] res = new Dictionary<string, JsonDamageDist>[_statistics.Phases.Count];
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phase = _statistics.Phases[i];
                res[i] = BuildDamageDist(p.GetDamageTakenLogs(null, _log, phase.Start, phase.End));
            }
            return res;
        }

        private Dictionary<string, JsonDamageDist>[] BuildDamageDist(Minions p, Target target)
        {
            Dictionary<string, JsonDamageDist>[] res = new Dictionary<string, JsonDamageDist>[_statistics.Phases.Count];
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phase = _statistics.Phases[i];
                res[i] = BuildDamageDist(p.GetDamageLogs(target, _log, phase.Start, phase.End));
            }
            return res;
        }

        private Dictionary<string, JsonDamageDist> BuildDamageDist(List<DamageLog> dls)
        {
            Dictionary<string, JsonDamageDist> res = new Dictionary<string, JsonDamageDist>();
            Dictionary<long, List<DamageLog>> dict = dls.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            SkillData skillList = _log.SkillData;
            foreach (KeyValuePair<long, List<DamageLog>> pair in dict)
            {
                if (pair.Value.Count == 0)
                {
                    continue;
                }
                SkillItem skill = skillList.Get(pair.Key);
                if (!pair.Value.First().IsCondi && skill != null)
                {
                    if(!_skillNames.ContainsKey("s" + pair.Key))
                    {
                        _skillNames["s" + pair.Key] = skill.Name;
                    }
                }
                List<DamageLog> filteredList = pair.Value.Where(x => x.Result != ParseEnum.Result.Downed).ToList();
                if (filteredList.Count == 0)
                {
                    continue;
                }
                string prefix = filteredList.First().IsCondi ? "b" : "s";
                res[prefix + pair.Key] = new JsonDamageDist()
                {
                    Hits = filteredList.Count,
                    Damage = filteredList.Sum(x => x.Damage),
                    Min = filteredList.Min(x => x.Damage),
                    Max = filteredList.Max(x => x.Damage),
                    Flank = filteredList.Count(x => x.IsFlanking),
                    Crit = filteredList.Count(x => x.Result == ParseEnum.Result.Crit),
                    Glance = filteredList.Count(x => x.Result == ParseEnum.Result.Glance),
                };
        }

            return res;
        }

        private List<JsonMinions> BuildMinions(AbstractMasterActor master)
        {
            List<JsonMinions> mins = new List<JsonMinions>();
            foreach (Minions minions in master.GetMinions(_log).Values)
            {
                JsonMinions min = new JsonMinions()
                {
                    Name = minions.Character,
                    Rotation = BuildRotation(minions.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    TotalDamageDist = BuildDamageDist(minions, null),
                    TargetDamageDist = BuildDamageDist(minions),
                };
                mins.Add(min);
            }
            return mins;
        }

        private Dictionary<string, List<JsonSkill>> BuildRotation(List<CastLog> cls)
        {
            Dictionary<string, List<JsonSkill>> res = new Dictionary<string, List<JsonSkill>>();
            SkillData skillList = _log.SkillData;
            foreach (CastLog cl in cls)
            {
                SkillItem skill = skillList.Get(cl.SkillId);
                GW2APISkill skillApi = skill?.ApiSkill;
                string skillName = skill.Name;
                _skillNames["s" + cl.SkillId] = skillName;
                int timeGained = 0;
                if (cl.EndActivation == ParseEnum.Activation.CancelFire && cl.ActualDuration < cl.ExpectedDuration)
                {
                    timeGained = cl.ExpectedDuration - cl.ActualDuration;
                }
                else if (cl.EndActivation == ParseEnum.Activation.CancelCancel)
                {
                    timeGained = -cl.ActualDuration;
                }
                JsonSkill jSkill = new JsonSkill
                {
                    Time = (int)cl.Time,
                    Duration = cl.ActualDuration,
                    TimeGained = timeGained,
                    AutoAttack = skillApi != null && skillApi.Slot == "Weapon_1",
                    Quickness = cl.StartActivation == ParseEnum.Activation.Quickness
                };
                if (res.TryGetValue("s" + cl.SkillId, out var list))
                {
                    list.Add(jSkill);
                } else
                {
                    res["s"+cl.SkillId] = new List<JsonSkill>()
                    {
                        jSkill
                    };
                }
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
                    Name = phase.Name,
                    Targets = new List<int>()
                };
                foreach (Target tar in phase.Targets)
                {
                    phaseJson.Targets.Add(_log.FightData.Logic.Targets.IndexOf(tar));
                }
                log.Phases.Add(phaseJson);
            }
        }

        private Dictionary<string, double> ConvertKeys(Dictionary<Player, double> toConvert)
        {
            Dictionary<string, double> res = new Dictionary<string, double>();
            foreach(var pair in toConvert)
            {
                res[pair.Key.Character] = pair.Value;
            }
            return res;
        }

        private Dictionary<string, JsonTargetBuffs> BuildTargetBuffs(Dictionary<long, Statistics.FinalTargetBuffs>[] statBoons, Target target)
        {
            int phases = _statistics.Phases.Count;
            var boons = new Dictionary<string, JsonTargetBuffs>();

            foreach (var pair in statBoons[0])
            {
                _buffNames["b" + pair.Key] = Boon.BoonsByIds[pair.Key].Name;
                List<JsonTargetBuffs.JsonTargetBuffsData> data = new List<JsonTargetBuffs.JsonTargetBuffsData>();
                for (int i = 0; i < _statistics.Phases.Count; i++)
                {
                    JsonTargetBuffs.JsonTargetBuffsData value = new JsonTargetBuffs.JsonTargetBuffsData();
                    Statistics.FinalTargetBuffs tarBuffs = statBoons[pair.Key][i];
                    value.Uptime = tarBuffs.Uptime;
                    value.Presence = tarBuffs.Presence;
                    value.Generated = ConvertKeys(tarBuffs.Generated);
                    value.Overstacked = ConvertKeys(tarBuffs.Overstacked);
                    value.Wasted = ConvertKeys(tarBuffs.Wasted);
                    value.UnknownExtension = ConvertKeys(tarBuffs.UnknownExtension);
                    value.Extension = ConvertKeys(tarBuffs.Extension);
                    value.Extended = ConvertKeys(tarBuffs.Extended);
                    data.Add(value);
                }
                JsonTargetBuffs jsonBuffs = new JsonTargetBuffs()
                {
                    States = BuildBuffStates(target.GetBoonGraphs(_log)[pair.Key]),
                    Data = data
                };
                boons["b" + pair.Key] = jsonBuffs;
            }

            return boons;
        }

        private Dictionary<string, JsonBuffs> BuildBuffUptime(Dictionary<long, Statistics.FinalBuffs>[] statUptimes, Player player)
        {
            var uptimes = new Dictionary<string, JsonBuffs>();
            int phases = _statistics.Phases.Count;
            foreach (var pair in statUptimes[0])
            {
                Boon buff = Boon.BoonsByIds[pair.Key];
                _buffNames["b" + pair.Key] = buff.Name;
                if (buff.Nature == Boon.BoonNature.GraphOnlyBuff && buff.Source == Boon.ProfToEnum(player.Prof))
                {
                    if (player.GetBoonDistribution(_log, 0).GetUptime(pair.Key) > 0)
                    {
                        if (_personalBuffs.TryGetValue(player.Prof, out var list) && !list.Contains(pair.Key))
                        {
                            list.Add(pair.Key);
                        }
                        else
                        {
                            _personalBuffs[player.Prof] = new HashSet<long>()
                                {
                                    pair.Key
                                };
                        }
                    }
                }
                List<Statistics.FinalBuffs> data = new List<Statistics.FinalBuffs>();
                for (int i = 0; i < _statistics.Phases.Count; i++)
                {
                    data.Add(statUptimes[pair.Key][i]);
                }
                JsonBuffs jsonBuffs = new JsonBuffs()
                {
                    States = BuildBuffStates(player.GetBoonGraphs(_log)[pair.Key]),
                    Data = data
                };
                uptimes["b" + pair.Key] = jsonBuffs;
            }

            if (!uptimes.Any()) return null;

            return uptimes;
        }
    }
}