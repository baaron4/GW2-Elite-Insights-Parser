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
using static LuckParser.Models.JsonModels.JsonStatistics;
using static LuckParser.Models.JsonModels.JsonPlayerBuffs;
using static LuckParser.Models.JsonModels.JsonTargetBuffs;
using static LuckParser.Models.JsonModels.JsonRotation;
using static LuckParser.Models.JsonModels.JsonBuffDamageModifierData;
using static LuckParser.Models.JsonModels.JsonMechanics;

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
        private readonly Dictionary<string, JsonLog.SkillDesc> _skillDesc = new Dictionary<string, JsonLog.SkillDesc>();
        private readonly Dictionary<string, JsonLog.BuffDesc> _buffDesc = new Dictionary<string, JsonLog.BuffDesc>();
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
            log.SkillMap = _skillDesc;
            log.BuffMap = _buffDesc;
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
                log.Mechanics = new List<JsonMechanics>();
                Dictionary<string, List<JsonMechanic>> dict = new Dictionary<string, List<JsonMechanic>>();
                foreach (MechanicLog ml in mechanicLogs)
                {
                    JsonMechanic mech = new JsonMechanic
                    {
                        Time = ml.Time,
                        Actor = ml.Actor.Character
                    };
                    if (dict.TryGetValue(ml.InGameName, out var list))
                    {
                        list.Add(mech);
                    }
                    else
                    {
                        dict[ml.InGameName] = new List<JsonMechanic>()
                        {
                            mech
                        };
                    }
                }
                foreach (var pair in dict)
                {
                    log.Mechanics.Add(new JsonMechanics()
                    {
                        Name = pair.Key,
                        MechanicsData = pair.Value
                    });
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
                    DpsAll = _statistics.TargetDps[target].Select(x => new JsonDPS(x)).ToArray(),
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
                    BoonsStates = BuildBuffStates(target.GetBoonGraphs(_log)[Boon.NumberOfBoonsID]),
                    ConditionsStates = BuildBuffStates(target.GetBoonGraphs(_log)[Boon.NumberOfConditionsID])
                };
                int finalTargetHealth = target.HealthOverTime.Count > 0
                    ? target.HealthOverTime.Last().Y
                    : 10000;
                jsTarget.FinalHealth = (int)Math.Round(target.Health * (finalTargetHealth * 0.01));
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
                    DpsAll = _statistics.DpsAll[player].Select(x => new JsonDPS(x)).ToArray(),
                    DpsTargets = BuildDPSTarget(_statistics.DpsTarget, player),
                    StatsAll = _statistics.StatsAll[player].Select(x => new JsonStatsAll(x)).ToArray(),
                    StatsTargets = BuildStatsTarget(_statistics.StatsTarget, player),
                    Defenses = _statistics.Defenses[player].Select(x => new JsonDefenses(x)).ToArray(),
                    Rotation = BuildRotation(player.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    Support = _statistics.Support[player].Select(x => new JsonSupport(x)).ToArray(),
                    SelfBuffs = BuildPlayerBuffs(_statistics.SelfBuffs[player], player),
                    GroupBuffs = BuildPlayerBuffs(_statistics.GroupBuffs[player], player),
                    OffGroupBuffs = BuildPlayerBuffs(_statistics.OffGroupBuffs[player], player),
                    SquadBuffs = BuildPlayerBuffs(_statistics.SquadBuffs[player], player),
                    DamageModifiers = BuildDamageModifiers(player.GetExtraBoonData(_log, null)),
                    DamageModifiersTarget = BuildDamageModifiersTarget(player),
                    Minions = BuildMinions(player),
                    TotalDamageDist = BuildDamageDist(player, null),
                    TargetDamageDist = BuildDamageDist(player),
                    TotalDamageTaken = BuildDamageTaken(player),
                    DeathRecap = BuildDeathRecap(player.GetDeathRecaps(_log)),
                    Consumables = BuildConsumables(player),
                    BoonsStates = BuildBuffStates(player.GetBoonGraphs(_log)[Boon.NumberOfBoonsID]),
                    ConditionsStates = BuildBuffStates(player.GetBoonGraphs(_log)[Boon.NumberOfConditionsID]),
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

        private JsonDPS[][] BuildDPSTarget(Dictionary<Target, Dictionary<Player, Statistics.FinalDPS[]>> stats, Player p)
        {
            JsonDPS[][] res = new JsonDPS[_log.FightData.Logic.Targets.Count][];
            int i = 0;
            foreach (Target tar in _log.FightData.Logic.Targets)
            {
                res[i++] = stats[tar][p].Select(x => new JsonDPS(x)).ToArray();            
            }
            return res;
        }

        private JsonStats[][] BuildStatsTarget(Dictionary<Target, Dictionary<Player, Statistics.FinalStats[]>> stats, Player p)
        {
            JsonStats[][] res = new JsonStats[_log.FightData.Logic.Targets.Count][];
            int i = 0;
            foreach (Target tar in _log.FightData.Logic.Targets)
            {
                res[i++] = stats[tar][p].Select(x => new JsonStats(x)).ToArray();
            }
            return res;
        }

        private List<JsonDeathRecap> BuildDeathRecap(List<Player.DeathRecap> recaps)
        {
            List<JsonDeathRecap> res = new List<JsonDeathRecap>();
            foreach (Player.DeathRecap recap in recaps)
            {
                res.Add(new JsonDeathRecap(recap));
            }
            return res;
        }

        private List<JsonBuffDamageModifierData> BuildDamageModifiers(Dictionary<long, List<AbstractMasterActor.ExtraBoonData>> extra)
        {
            Dictionary<long, List<JsonBuffDamageModifierItem>> dict = new Dictionary<long, List<JsonBuffDamageModifierItem>>();
            foreach (long key in extra.Keys)
            {
                string nKey = "b" + key;
                if (!_buffDesc.ContainsKey(nKey))
                {
                    _buffDesc[nKey] = new JsonLog.BuffDesc(Boon.BoonsByIds[key]);
                }
                dict[key] = extra[key].Select(x => new JsonBuffDamageModifierItem(x)).ToList();
            }
            List<JsonBuffDamageModifierData> res = new List<JsonBuffDamageModifierData>();
            foreach (var pair in dict)
            {
                res.Add(new JsonBuffDamageModifierData()
                {
                    Id = pair.Key,
                    DamageModifiers = pair.Value
                });
            }
            return res;
        }

        private List<JsonBuffDamageModifierData>[] BuildDamageModifiersTarget(Player p)
        {
            List<JsonBuffDamageModifierData>[] res = new List<JsonBuffDamageModifierData>[_log.FightData.Logic.Targets.Count];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                Target tar = _log.FightData.Logic.Targets[i];
                res[i] = BuildDamageModifiers(p.GetExtraBoonData(_log, tar));
            }
            return res;
        }

        private List<JsonConsumable> BuildConsumables(Player player)
        {
            List<Player.Consumable> input = player.GetConsumablesList(_log, 0, _log.FightData.FightDuration);
            List<JsonConsumable> res = new List<JsonConsumable>();
            foreach (var food in input)
            {
                if (!_buffDesc.ContainsKey("b" + food.Buff.ID))
                {
                    _buffDesc["b" + food.Buff.ID] = new JsonLog.BuffDesc(food.Buff);
                }
                res.Add(new JsonConsumable(food));
            }
            return input.Count > 0 ? res : null;
        }

        private List<int[]> BuildBuffStates(BoonsGraphModel bgm)
        {
            if (bgm == null || bgm.BoonChart.Count == 0)
            {
                return null;
            }
            List<int[]> res = bgm.BoonChart.Select(x => new int[2] { (int)x.Start, x.Value }).ToList();
            return res.Count > 0 ? res : null;
        }

        private List<JsonDamageDist>[][] BuildDamageDist(AbstractMasterActor p)
        {
            List<JsonDamageDist>[][] res = new List<JsonDamageDist>[_log.FightData.Logic.Targets.Count][];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                Target target = _log.FightData.Logic.Targets[i];
                res[i] = BuildDamageDist(p, target);
            }
            return res;
        }

        private List<JsonDamageDist>[][] BuildDamageDist(Minions p)
        {
            List<JsonDamageDist>[][] res = new List<JsonDamageDist>[_log.FightData.Logic.Targets.Count][];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                Target target = _log.FightData.Logic.Targets[i];
                res[i] = BuildDamageDist(p, target);
            }
            return res;
        }

        private List<JsonDamageDist>[] BuildDamageDist(AbstractMasterActor p, Target target)
        {
            List<JsonDamageDist>[] res = new List<JsonDamageDist>[_statistics.Phases.Count];
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phase = _statistics.Phases[i];
                res[i] = BuildDamageDist(p.GetJustPlayerDamageLogs(target, _log, phase.Start, phase.End));
            }
            return res;
        }

        private List<JsonDamageDist>[] BuildDamageTaken(AbstractMasterActor p)
        {
            List<JsonDamageDist>[] res = new List<JsonDamageDist>[_statistics.Phases.Count];
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phase = _statistics.Phases[i];
                res[i] = BuildDamageDist(p.GetDamageTakenLogs(null, _log, phase.Start, phase.End));
            }
            return res;
        }

        private List<JsonDamageDist>[] BuildDamageDist(Minions p, Target target)
        {
            List<JsonDamageDist>[] res = new List<JsonDamageDist>[_statistics.Phases.Count];
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phase = _statistics.Phases[i];
                res[i] = BuildDamageDist(p.GetDamageLogs(target, _log, phase.Start, phase.End));
            }
            return res;
        }

        private List<JsonDamageDist> BuildDamageDist(List<DamageLog> dls)
        {
            List<JsonDamageDist> res = new List<JsonDamageDist>();
            Dictionary<long, List<DamageLog>> dict = dls.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            SkillData skillList = _log.SkillData;
            foreach (KeyValuePair<long, List<DamageLog>> pair in dict)
            {
                if (pair.Value.Count == 0)
                {
                    continue;
                }
                SkillItem skill = skillList.Get(pair.Key);
                if (pair.Value.First().IsIndirectDamage)
                {
                    if (!_buffDesc.ContainsKey("b" + pair.Key))
                    {
                        if (Boon.BoonsByIds.TryGetValue(pair.Key, out Boon buff))
                        {
                            _buffDesc["b" + pair.Key] = new JsonLog.BuffDesc(buff);
                        }
                        else
                        {
                            Boon auxBoon = new Boon(skill.Name, pair.Key, skill.Icon);
                            _buffDesc["b" + pair.Key] = new JsonLog.BuffDesc(auxBoon);
                        }
                    }
                }
                else
                {
                    if (!_skillDesc.ContainsKey("s" + pair.Key))
                    {
                        _skillDesc["s" + pair.Key] = new JsonLog.SkillDesc(skill);
                    }
                }
                List<DamageLog> filteredList = pair.Value.Where(x => x.Result != ParseEnum.Result.Downed).ToList();
                if (filteredList.Count == 0)
                {
                    continue;
                }
                string prefix = filteredList.First().IsIndirectDamage ? "b" : "s";
                res.Add(new JsonDamageDist(filteredList, filteredList.First().IsIndirectDamage, pair.Key));
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

        private List<JsonRotation> BuildRotation(List<CastLog> cls)
        {
            Dictionary<long, List<JsonSkill>> dict = new Dictionary<long, List<JsonSkill>>();
            SkillData skillList = _log.SkillData;
            foreach (CastLog cl in cls)
            {
                SkillItem skill = skillList.Get(cl.SkillId);
                string skillName = skill.Name;
                if (!_skillDesc.ContainsKey("s" + cl.SkillId))
                {
                    _skillDesc["s" + cl.SkillId] = new JsonLog.SkillDesc(skill);
                }
                JsonSkill jSkill = new JsonSkill(cl);
                if (dict.TryGetValue(cl.SkillId, out var list))
                {
                    list.Add(jSkill);
                } else
                {
                    dict[cl.SkillId] = new List<JsonSkill>()
                    {
                        jSkill
                    };
                }
            }
            List<JsonRotation> res = new List<JsonRotation>();
            foreach (var pair in dict)
            {
                res.Add(new JsonRotation()
                {
                    Id = pair.Key,
                    Skills = pair.Value
                });
            }
            return res;
        }

        private void SetPhases(JsonLog log)
        {
            log.Phases = new List<JsonPhase>();

            foreach (var phase in _statistics.Phases)
            {
                JsonPhase phaseJson = new JsonPhase(phase);
                foreach (Target tar in phase.Targets)
                {
                    phaseJson.Targets.Add(_log.FightData.Logic.Targets.IndexOf(tar));
                }
                log.Phases.Add(phaseJson);
                for (int j = 1; j < _statistics.Phases.Count; j++)
                {
                    PhaseData curPhase = _statistics.Phases[j];
                    if (curPhase.Start < phaseJson.Start || curPhase.End > phaseJson.End ||
                         (curPhase.Start == phaseJson.Start && curPhase.End == phaseJson.End))
                    {
                        continue;
                    }
                    if (phaseJson.SubPhases == null)
                    {
                        phaseJson.SubPhases = new List<int>();
                    }
                    phaseJson.SubPhases.Add(j);
                }
            }
        }

        private List<JsonTargetBuffs> BuildTargetBuffs(Dictionary<long, Statistics.FinalTargetBuffs>[] statBoons, Target target)
        {
            int phases = _statistics.Phases.Count;
            var boons = new List<JsonTargetBuffs>();

            foreach (var pair in statBoons[0])
            {
                if (!_buffDesc.ContainsKey("b" + pair.Key))
                {
                    _buffDesc["b" + pair.Key] = new JsonLog.BuffDesc(Boon.BoonsByIds[pair.Key]);
                }
                List<JsonTargetBuffsData> data = new List<JsonTargetBuffsData>();
                for (int i = 0; i < _statistics.Phases.Count; i++)
                {
                    JsonTargetBuffsData value = new JsonTargetBuffsData(statBoons[pair.Key][i]);
                    data.Add(value);
                }
                JsonTargetBuffs jsonBuffs = new JsonTargetBuffs()
                {
                    States = BuildBuffStates(target.GetBoonGraphs(_log)[pair.Key]),
                    BuffData = data,
                    Id = pair.Key
                };
                boons.Add(jsonBuffs);
            }

            return boons;
        }

        private List<JsonPlayerBuffs> BuildPlayerBuffs(Dictionary<long, Statistics.FinalBuffs>[] statUptimes, Player player)
        {
            var uptimes = new List<JsonPlayerBuffs>();
            int phases = _statistics.Phases.Count;
            foreach (var pair in statUptimes[0])
            {
                Boon buff = Boon.BoonsByIds[pair.Key];
                if (!_buffDesc.ContainsKey("b" + pair.Key))
                {
                    _buffDesc["b" + pair.Key] = new JsonLog.BuffDesc(buff);
                }
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
                List<JsonPlayerBuffsData> data = new List<JsonPlayerBuffsData>();
                for (int i = 0; i < _statistics.Phases.Count; i++)
                {
                    data.Add(new JsonPlayerBuffsData(statUptimes[pair.Key][i]));
                }
                JsonPlayerBuffs jsonBuffs = new JsonPlayerBuffs()
                {
                    States = BuildBuffStates(player.GetBoonGraphs(_log)[pair.Key]),
                    BuffData = data,
                    Id = pair.Key
                };
                uptimes.Add(jsonBuffs);
            }

            if (!uptimes.Any()) return null;

            return uptimes;
        }
    }
}