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
using LuckParser.Models.DataModels;
using LuckParser.Models.JsonModels;
using LuckParser.Models.ParseModels;
using Newtonsoft.Json;

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

            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
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

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
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
            log.triggerID = _log.FightData.ID;
            log.fightName = _log.FightData.Name;
            log.eliteInsightsVersion = Application.ProductVersion;
            log.arcVersion = _log.LogData.BuildVersion;
            log.recordedBy = _log.LogData.PoV.Split(':')[0].TrimEnd('\u0000');
            log.timeStart = _log.LogData.LogStart;
            log.timeEnd = _log.LogData.LogEnd;
            log.duration = durationString;
            log.success = _log.FightData.Success ? 1 : 0;
            log.skillNames = _skillNames;
            log.buffNames = _buffNames;
            log.personalBuffs = _personalBuffs;
            log.uploadLinks = _uploadLink;
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
                log.mechanics = new Dictionary<string, List<JsonMechanic>>();
                foreach (MechanicLog ml in mechanicLogs)
                {
                    JsonMechanic mech = new JsonMechanic
                    {
                        time = ml.Time,
                        player = ml.Player.Character
                    };
                    if (log.mechanics.TryGetValue(ml.InGameName, out var list))
                    {
                        list.Add(mech);
                    }
                    else
                    {
                        log.mechanics[ml.InGameName] = new List<JsonMechanic>()
                        {
                            mech
                        };
                    }
                }
            }
        }

        private void SetTargets(JsonLog log)
        {
            log.targets = new List<JsonTarget>();
            foreach (Target target in _log.FightData.Logic.Targets)
            {
                JsonTarget jsTarget = new JsonTarget
                {
                    id = target.ID,
                    name = target.Character,
                    totalHealth = target.Health,
                    avgBoons = _statistics.AvgTargetBoons[target],
                    avgConditions = _statistics.AvgTargetConditions[target],
                    dps = BuildDPS(_statistics.TargetDps[target]),
                    buffs = BuildTargetBuffs(_statistics.TargetBuffs[target], target),
                    hitboxHeight = target.HitboxHeight,
                    hitboxWidth = target.HitboxWidth,
                    damage1S = BuildTotal1SDamage(target),
                    rotation = BuildRotation(target.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    firstAware = (int)(_log.FightData.ToFightSpace(target.FirstAware)),
                    lastAware = (int)(_log.FightData.ToFightSpace(target.LastAware)),
                    minions = BuildMinions(target),
                    totalDamageDist = BuildDamageDist(target, null),
                    totalDamageTaken = BuildDamageTaken(target),
                    avgBoonsStates = BuildBuffStates(target.GetBoonGraphs(_log)[Boon.NumberOfBoonsID]),
                    avgConditionsStates = BuildBuffStates(target.GetBoonGraphs(_log)[Boon.NumberOfConditionsID])
                };
                int finalTargetHealth = target.HealthOverTime.Count > 0
                    ? target.HealthOverTime.Last().Y
                    : 10000;
                jsTarget.finalHealth = target.Health * (finalTargetHealth * 0.01);
                jsTarget.healthPercentBurned = 100.0 - finalTargetHealth * 0.01;
                log.targets.Add(jsTarget);
            }
        }

        private void SetPlayers(JsonLog log)
        {
            log.players = new List<JsonPlayer>();

            foreach (var player in _log.PlayerList)
            {
                log.players.Add(new JsonPlayer
                {
                    character = player.Character,
                    account = player.Account,
                    condition = player.Condition,
                    concentration = player.Concentration,
                    healing = player.Healing,
                    toughness = player.Toughness,
                    weapons = player.GetWeaponsArray(_log).Where(w => w != null).ToArray(),
                    group = player.Group,
                    profession = player.Prof,
                    damage1S = BuildTotal1SDamage(player),
                    targetDamage1S = BuildTarget1SDamage(player),
                    dpsAll = BuildDPS(_statistics.DpsAll[player]),
                    dpsTargets = BuildDPSTarget(_statistics.DpsTarget, player),
                    statsAll = BuildStatsAll(_statistics.StatsAll[player]),
                    statsTargets = BuildStatsTarget(_statistics.StatsTarget, player),
                    defenses = BuildDefenses(_statistics.Defenses[player]),
                    totation = BuildRotation(player.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    support = BuildSupport(_statistics.Support[player]),
                    selfBuffs = BuildBuffUptime(_statistics.SelfBuffs[player], player),
                    groupBuffs = BuildBuffUptime(_statistics.GroupBuffs[player], player),
                    offGroupBuffs = BuildBuffUptime(_statistics.OffGroupBuffs[player], player),
                    squadBuffs = BuildBuffUptime(_statistics.SquadBuffs[player], player),
                    minions = BuildMinions(player),
                    totalDamageDist = BuildDamageDist(player, null),
                    targetDamageDist = BuildDamageDist(player),
                    totalDamageTaken = BuildDamageTaken(player),
                    deathRecap = player.GetDeathRecaps(_log),
                    consumables = BuildConsumables(player),
                    avgBoonsStates = BuildBuffStates(player.GetBoonGraphs(_log)[Boon.NumberOfBoonsID]),
                    avgConditionsStates = BuildBuffStates(player.GetBoonGraphs(_log)[Boon.NumberOfConditionsID]),
                });
            }
        }

        private List<int>[] BuildTotal1SDamage(AbstractMasterPlayer p)
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

        private List<JsonConsumable> BuildConsumables(Player player)
        {
            List<JsonConsumable> res = new List<JsonConsumable>();
            foreach(var food in player.GetConsumablesList(_log,0,_log.FightData.FightDuration))
            {
                JsonConsumable val = new JsonConsumable() {
                    id = food.Item.ID,
                    time = food.Time,
                    duration = food.Duration,
                    stack = food.Stack
                };
                _buffNames["b"+food.Item.ID] = food.Item.Name;
                res.Add(val);
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

        private Dictionary<string, JsonDamageDist>[][] BuildDamageDist(AbstractMasterPlayer p)
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

        private Dictionary<string, JsonDamageDist>[] BuildDamageDist(AbstractMasterPlayer p, Target target)
        {
            Dictionary<string, JsonDamageDist>[] res = new Dictionary<string, JsonDamageDist>[_statistics.Phases.Count];
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phase = _statistics.Phases[i];
                res[i] = BuildDamageDist(p.GetJustPlayerDamageLogs(target, _log, phase.Start, phase.End));
            }
            return res;
        }

        private Dictionary<string, JsonDamageDist>[] BuildDamageTaken(AbstractMasterPlayer p)
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
                if (pair.Value.First().IsCondi == 0 && skill != null)
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
                string prefix = filteredList.First().IsCondi > 0 ? "b" : "s";
                res[prefix + pair.Key] = new JsonDamageDist()
                {
                    hits = filteredList.Count,
                    damage = filteredList.Sum(x => x.Damage),
                    min = filteredList.Min(x => x.Damage),
                    max = filteredList.Max(x => x.Damage),
                    flank = filteredList.Count(x => x.IsFlanking > 0),
                    crit = filteredList.Count(x => x.Result == ParseEnum.Result.Crit),
                    glance = filteredList.Count(x => x.Result == ParseEnum.Result.Glance),
                };
        }

            return res;
        }

        private List<JsonMinions> BuildMinions(AbstractMasterPlayer master)
        {
            List<JsonMinions> mins = new List<JsonMinions>();
            foreach (Minions minions in master.GetMinions(_log).Values)
            {
                JsonMinions min = new JsonMinions()
                {
                    name = minions.Character,
                    rotation = BuildRotation(minions.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    totalDamageDist = BuildDamageDist(minions, null),
                    targetDamageDist = BuildDamageDist(minions),
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
                    time = (int)cl.Time,
                    duration = cl.ActualDuration,
                    timeGained = timeGained,
                    autoAttack = skillApi != null && skillApi.slot == "Weapon_1" ? 1 : 0,
                    quickness = cl.StartActivation == ParseEnum.Activation.Quickness ? 1 : 0
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
            log.phases = new List<JsonPhase>();

            foreach (var phase in _statistics.Phases)
            {
                JsonPhase phaseJson = new JsonPhase
                {
                    start = phase.Start,
                    end = phase.End,
                    name = phase.Name
                };
                log.phases.Add(phaseJson);
            }
        }

        // Statistics to Json Converters ////////////////////////////////////////////////////

        private bool ContainsTargetBoon(long boon, Dictionary<long, Statistics.FinalTargetBuffs>[] statBoons)
        {
            for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
            {
                if (statBoons[phaseIndex][boon].Uptime > 0) return true;
                if (statBoons[phaseIndex][boon].Generated.Any(x => x.Value > 0)) return true;
                if (statBoons[phaseIndex][boon].Overstacked.Any(x => x.Value > 0)) return true;
                if (statBoons[phaseIndex][boon].Wasted.Any(x => x.Value > 0)) return true;
                if (statBoons[phaseIndex][boon].UnknownExtension.Any(x => x.Value > 0)) return true;
            }

            return false;
        }

        private void MakePhaseTargetBoon(JsonTargetBuffs boon, int phase, Statistics.FinalTargetBuffs value)
        {
            boon.uptime[phase] = value.Uptime;
            boon.presence[phase] = value.Presence;
            boon.generated[phase] = boon.generated[phase] ?? new Dictionary<string, double>();
            boon.overstacked[phase] = boon.overstacked[phase] ?? new Dictionary<string, double>();
            boon.wasted[phase] = boon.wasted[phase] ?? new Dictionary<string, double>();
            boon.unknownExtension[phase] = boon.unknownExtension[phase] ?? new Dictionary<string, double>();

            foreach (var playerBoon in value.Generated.Where(x => x.Value > 0))
            {
                boon.generated[phase][playerBoon.Key.Character] = playerBoon.Value;
            }

            foreach (var playerBoon in value.Overstacked.Where(x => x.Value > 0))
            {
                boon.overstacked[phase][playerBoon.Key.Character] = playerBoon.Value;
            }

            foreach (var playerBoon in value.Wasted.Where(x => x.Value > 0))
            {
                boon.wasted[phase][playerBoon.Key.Character] = playerBoon.Value;
            }

            foreach (var playerBoon in value.UnknownExtension.Where(x => x.Value > 0))
            {
                boon.wasted[phase][playerBoon.Key.Character] = playerBoon.Value;
            }
        }

        private Dictionary<string, JsonTargetBuffs> BuildTargetBuffs(Dictionary<long, Statistics.FinalTargetBuffs>[] statBoons, Target target)
        {
            int phases = _statistics.Phases.Count;
            var boons = new Dictionary<string, JsonTargetBuffs>();

            var boonsFound = new HashSet<long>();
            var boonsNotFound = new HashSet<long>();

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                foreach (var boon in statBoons[phaseIndex])
                {
                    _buffNames["b" + boon.Key] = Boon.BoonsByIds[boon.Key].Name;
                    if (boonsFound.Contains(boon.Key))
                    {
                        MakePhaseTargetBoon(boons["b" + boon.Key], phaseIndex, boon.Value);
                    }
                    else if (!boonsNotFound.Contains(boon.Key))
                    {
                        if (ContainsTargetBoon(boon.Key, statBoons))
                        {
                            boonsFound.Add(boon.Key);

                            boons["b" + boon.Key] = new JsonTargetBuffs(phases);
                            MakePhaseTargetBoon(boons["b" + boon.Key], phaseIndex, boon.Value);
                            if (target.GetBoonGraphs(_log).TryGetValue(boon.Key, out var bgm))
                            {
                                boons["b" + boon.Key].states = BuildBuffStates(bgm);
                            }
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

        private JsonDps BuildDPS(Statistics.FinalDPS[] statDps)
        {
            var dps = new JsonDps(_statistics.Phases.Count);

            MoveArrayLevel(dps, _statistics.Phases.Count, statDps);
            RemoveZeroArrays(dps);

            return dps;
        }

        private JsonDps[] BuildDPSTarget(Dictionary<Target, Dictionary<Player, Statistics.FinalDPS[]>> statDps, Player player)
        {
            var finalDps = new JsonDps[_log.FightData.Logic.Targets.Count];
            int i = 0;
            foreach (Target target in _log.FightData.Logic.Targets)
            {
                finalDps[i++] = BuildDPS(statDps[target][player]);
            }
            return finalDps;
        }

        private bool ContainsBoon(long boon, Dictionary<long, Statistics.FinalBuffs>[] statUptimes)
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

        private void MakePhaseBoon(JsonBuffs boon, int phase, Statistics.FinalBuffs value)
        {
            boon.overstack[phase] = value.Overstack;
            boon.wasted[phase] = value.Wasted;
            boon.unknownExtension[phase] = value.UnknownExtension;
            boon.generation[phase] = value.Generation;
            boon.uptime[phase] = value.Uptime;
            boon.presence[phase] = value.Presence;
        }

        private Dictionary<string, JsonBuffs> BuildBuffUptime(Dictionary<long, Statistics.FinalBuffs>[] statUptimes, Player player)
        {
            var uptimes = new Dictionary<string, JsonBuffs>();
            int phases = _statistics.Phases.Count;

            var boonsFound = new HashSet<long>();
            var boonsNotFound = new HashSet<long>();

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                foreach (var boon in statUptimes[phaseIndex])
                {
                    Boon buff = Boon.BoonsByIds[boon.Key];
                    _buffNames["b" + boon.Key] = buff.Name;
                    if (buff.Nature == Boon.BoonNature.GraphOnlyBuff && buff.Source == Boon.ProfToEnum(player.Prof))
                    {
                        if (player.GetBoonDistribution(_log, 0).GetUptime(boon.Key) > 0)
                        {
                            if (_personalBuffs.TryGetValue(player.Prof, out var list) && !list.Contains(boon.Key))
                            {
                                list.Add(boon.Key);
                            }
                            else
                            {
                                _personalBuffs[player.Prof] = new HashSet<long>()
                                {
                                    boon.Key
                                };
                            }
                        }
                    }
                    if (boonsFound.Contains(boon.Key))
                    {
                        MakePhaseBoon(uptimes["b" + boon.Key], phaseIndex, boon.Value);
                    }
                    else if (!boonsNotFound.Contains(boon.Key))
                    {
                        if (ContainsBoon(boon.Key, statUptimes))
                        {
                            boonsFound.Add(boon.Key);

                            uptimes["b" + boon.Key] = new JsonBuffs(phases);
                            MakePhaseBoon(uptimes["b" + boon.Key], phaseIndex, boon.Value);
                            if (player.GetBoonGraphs(_log).TryGetValue(boon.Key, out var bgm))
                            {
                                uptimes["b" + boon.Key].states = BuildBuffStates(bgm);
                            }
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

        private JsonStats BuildStatsTarget(Statistics.FinalStats[] statStat)
        {
            var stats = new JsonStats(_statistics.Phases.Count);

            MoveArrayLevel(stats, _statistics.Phases.Count, statStat);
            RemoveZeroArrays(stats);

            return stats;
        }

        private JsonStats[] BuildStatsTarget(Dictionary<Target, Dictionary<Player,Statistics.FinalStats[]>> statStat, Player player)
        {
            var finalStats = new JsonStats[_log.FightData.Logic.Targets.Count];
            int i = 0;
            foreach (Target target in _log.FightData.Logic.Targets)
            {
                finalStats[i++] = BuildStatsTarget(statStat[target][player]);
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
                    string upperName = GeneralHelper.UppercaseFirst(field.Name);
                    entry.SetValue(upperObject[i].GetType().GetField(upperName).GetValue(upperObject[i]), i);
                }
            }
        }
    }
}