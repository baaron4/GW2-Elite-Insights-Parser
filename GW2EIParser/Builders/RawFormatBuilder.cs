using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using GW2EIParser.Builders.JsonModels;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static GW2EIParser.Builders.JsonModels.JsonBuffDamageModifierData;
using static GW2EIParser.Builders.JsonModels.JsonBuffsGeneration;
using static GW2EIParser.Builders.JsonModels.JsonBuffsUptime;
using static GW2EIParser.Builders.JsonModels.JsonMechanics;
using static GW2EIParser.Builders.JsonModels.JsonRotation;
using static GW2EIParser.Builders.JsonModels.JsonStatistics;
using static GW2EIParser.Builders.JsonModels.JsonTargetBuffs;
using static GW2EIParser.EIData.GeneralStatistics;

namespace GW2EIParser.Builders
{
    public class RawFormatBuilder
    {
        private readonly ParsedLog _log;
        private readonly List<PhaseData> _phases;

        private readonly string[] _uploadLink;
        //
        private readonly Dictionary<string, JsonLog.SkillDesc> _skillDesc = new Dictionary<string, JsonLog.SkillDesc>();
        private readonly Dictionary<string, JsonLog.BuffDesc> _buffDesc = new Dictionary<string, JsonLog.BuffDesc>();
        private readonly Dictionary<string, JsonLog.DamageModDesc> _damageModDesc = new Dictionary<string, JsonLog.DamageModDesc>();
        private readonly Dictionary<string, HashSet<long>> _personalBuffs = new Dictionary<string, HashSet<long>>();

        public RawFormatBuilder(ParsedLog log, string[] UploadString)
        {
            _log = log;
            _phases = log.FightData.GetPhases(log);

            _uploadLink = UploadString;
        }

        public JsonLog CreateJsonLog()
        {
            var log = new JsonLog();

            SetGeneral(log);
            SetTargets(log);
            SetPlayers(log);
            SetPhases(log);
            SetMechanics(log);

            return log;
        }

        public void CreateJSON(StreamWriter sw)
        {
            JsonLog log = CreateJsonLog();
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = contractResolver
            };
            var writer = new JsonTextWriter(sw)
            {
                Formatting = Properties.Settings.Default.IndentJSON ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None
            };
            serializer.Serialize(writer, log);
            writer.Close();
        }

        public void CreateXML(StreamWriter sw)
        {

            JsonLog log = CreateJsonLog();

            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = contractResolver
            };
            var root = new Dictionary<string, JsonLog>()
            {
                {"log", log }
            };
            string json = JsonConvert.SerializeObject(root, settings);

            XmlDocument xml = JsonConvert.DeserializeXmlNode(json);
            var xmlTextWriter = new XmlTextWriter(sw)
            {
                Formatting = Properties.Settings.Default.IndentXML ? System.Xml.Formatting.Indented : System.Xml.Formatting.None
            };

            xml.WriteTo(xmlTextWriter);
            xmlTextWriter.Close();
        }

        private void SetGeneral(JsonLog log)
        {
            log.TriggerID = _log.FightData.ID;
            log.FightName = _log.FightData.Name;
            log.FightIcon = _log.FightData.Logic.Icon;
            log.EliteInsightsVersion = Application.ProductVersion;
            log.ArcVersion = _log.LogData.BuildVersion;
            log.RecordedBy = _log.LogData.PoVName;
            log.TimeStart = _log.LogData.LogStart;
            log.TimeEnd = _log.LogData.LogEnd;
            log.Duration = _log.FightData.DurationString;
            log.Success = _log.FightData.Success;
            log.SkillMap = _skillDesc;
            log.BuffMap = _buffDesc;
            log.DamageModMap = _damageModDesc;
            log.PersonalBuffs = _personalBuffs;
            log.UploadLinks = _uploadLink;
        }

        private void SetMechanics(JsonLog log)
        {
            MechanicData mechanicData = _log.MechanicData;
            var mechanicLogs = new List<MechanicEvent>();
            foreach (List<MechanicEvent> mLog in mechanicData.GetAllMechanics(_log))
            {
                mechanicLogs.AddRange(mLog);
            }
            if (mechanicLogs.Any())
            {
                log.Mechanics = new List<JsonMechanics>();
                var dict = new Dictionary<string, List<JsonMechanic>>();
                foreach (MechanicEvent ml in mechanicLogs)
                {
                    var mech = new JsonMechanic
                    {
                        Time = ml.Time,
                        Actor = ml.Actor.Character
                    };
                    if (dict.TryGetValue(ml.InGameName, out List<JsonMechanic> list))
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
                foreach (KeyValuePair<string, List<JsonMechanic>> pair in dict)
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
            foreach (NPC target in _log.FightData.Logic.Targets)
            {
                var jsTarget = new JsonTarget
                {
                    Id = target.ID,
                    Name = target.Character,
                    Toughness = target.Toughness,
                    Healing = target.Healing,
                    Concentration = target.Concentration,
                    Condition = target.Condition,
                    TotalHealth = target.GetHealth(_log.CombatData),
                    AvgBoons = target.GetGameplayStats(_log).Select(x => x.AvgBoons).ToList(),
                    AvgConditions = target.GetGameplayStats(_log).Select(x => x.AvgConditions).ToList(),
                    DpsAll = target.GetDPSAll(_log).Select(x => new JsonDPS(x)).ToArray(),
                    Buffs = BuildTargetBuffs(target.GetBuffs(_log), target.GetBuffsDictionary(_log), target),
                    HitboxHeight = target.HitboxHeight,
                    HitboxWidth = target.HitboxWidth,
                    Damage1S = BuildTotal1SDamage(target),
                    Rotation = BuildRotation(target.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    FirstAware = (int)(_log.FightData.ToFightSpace(target.FirstAwareLogTime)),
                    LastAware = (int)(_log.FightData.ToFightSpace(target.LastAwareLogTime)),
                    Minions = BuildMinions(target),
                    TotalDamageDist = BuildDamageDist(target, null),
                    TotalDamageTaken = BuildDamageTaken(target),
                    BoonsStates = BuildBuffStates(target.GetBuffGraphs(_log)[ProfHelper.NumberOfBoonsID]),
                    ConditionsStates = BuildBuffStates(target.GetBuffGraphs(_log)[ProfHelper.NumberOfConditionsID]),
                    HealthPercents = _log.CombatData.GetHealthUpdateEvents(target.AgentItem).Select(x => new double[2] { x.Time, x.HPPercent }).ToList()
                };
                double hpLeft = 0.0;
                if (_log.FightData.Success)
                {
                    hpLeft = 0;
                }
                else
                {
                    List<HealthUpdateEvent> hpUpdates = _log.CombatData.GetHealthUpdateEvents(target.AgentItem);
                    if (hpUpdates.Count > 0)
                    {
                        hpLeft = hpUpdates.Last().HPPercent;
                    }
                }
                jsTarget.HealthPercentBurned = 100.0 - hpLeft;
                jsTarget.FinalHealth = (int)Math.Round(target.GetHealth(_log.CombatData) * hpLeft / 100.0);
                log.Targets.Add(jsTarget);
            }
        }

        private void SetPlayers(JsonLog log)
        {
            log.Players = new List<JsonPlayer>();

            foreach (Player player in _log.PlayerList)
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
                    Weapons = player.GetWeaponsArray(_log).Select(w => w ?? "Unknown").ToArray(),
                    Group = player.Group,
                    Profession = player.Prof,
                    Damage1S = BuildTotal1SDamage(player),
                    TargetDamage1S = BuildTarget1SDamage(player),
                    DpsAll = player.GetDPSAll(_log).Select(x => new JsonDPS(x)).ToArray(),
                    DpsTargets = BuildDPSTarget(player),
                    StatsAll = player.GetGameplayStats(_log).Select(x => new JsonGameplayStatsAll(x)).ToArray(),
                    StatsTargets = BuildStatsTarget(player),
                    Defenses = player.GetDefenses(_log).Select(x => new JsonDefensesAll(x)).ToArray(),
                    Rotation = BuildRotation(player.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    Support = player.GetPlayerSupport(_log).Select(x => new JsonPlayerSupport(x)).ToArray(),
                    BuffUptimes = BuildPlayerBuffUptimes(player.GetBuffs(_log, BuffEnum.Self), player),
                    SelfBuffs = BuildPlayerBuffGenerations(player.GetBuffs(_log, BuffEnum.Self)),
                    GroupBuffs = BuildPlayerBuffGenerations(player.GetBuffs(_log, BuffEnum.Group)),
                    OffGroupBuffs = BuildPlayerBuffGenerations(player.GetBuffs(_log, BuffEnum.OffGroup)),
                    SquadBuffs = BuildPlayerBuffGenerations(player.GetBuffs(_log, BuffEnum.Squad)),
                    BuffUptimesActive = BuildPlayerBuffUptimes(player.GetActiveBuffs(_log, BuffEnum.Self), player),
                    SelfBuffsActive = BuildPlayerBuffGenerations(player.GetActiveBuffs(_log, BuffEnum.Self)),
                    GroupBuffsActive = BuildPlayerBuffGenerations(player.GetActiveBuffs(_log, BuffEnum.Group)),
                    OffGroupBuffsActive = BuildPlayerBuffGenerations(player.GetActiveBuffs(_log, BuffEnum.OffGroup)),
                    SquadBuffsActive = BuildPlayerBuffGenerations(player.GetActiveBuffs(_log, BuffEnum.Squad)),
                    DamageModifiers = BuildDamageModifiers(player.GetDamageModifierData(_log, null)),
                    DamageModifiersTarget = BuildDamageModifiersTarget(player),
                    Minions = BuildMinions(player),
                    TotalDamageDist = BuildDamageDist(player, null),
                    TargetDamageDist = BuildDamageDist(player),
                    TotalDamageTaken = BuildDamageTaken(player),
                    DeathRecap = BuildDeathRecap(player.GetDeathRecaps(_log)),
                    Consumables = BuildConsumables(player),
                    BoonsStates = BuildBuffStates(player.GetBuffGraphs(_log)[ProfHelper.NumberOfBoonsID]),
                    ConditionsStates = BuildBuffStates(player.GetBuffGraphs(_log)[ProfHelper.NumberOfConditionsID]),
                    ActiveTimes = _phases.Select(x => x.GetActorActiveDuration(player, _log)).ToList(),
                });
            }
        }

        private List<int>[] BuildTotal1SDamage(AbstractSingleActor p)
        {
            var list = new List<int>[_phases.Count];
            for (int i = 0; i < _phases.Count; i++)
            {
                list[i] = p.Get1SDamageList(_log, i, _phases[i], null);
            }
            return list;
        }

        private List<int>[][] BuildTarget1SDamage(Player p)
        {
            var tarList = new List<int>[_log.FightData.Logic.Targets.Count][];
            for (int j = 0; j < _log.FightData.Logic.Targets.Count; j++)
            {
                NPC target = _log.FightData.Logic.Targets[j];
                var list = new List<int>[_phases.Count];
                for (int i = 0; i < _phases.Count; i++)
                {
                    list[i] = p.Get1SDamageList(_log, i, _phases[i], target);
                }
                tarList[j] = list;
            }
            return tarList;
        }

        private JsonDPS[][] BuildDPSTarget(Player p)
        {
            var res = new JsonDPS[_log.FightData.Logic.Targets.Count][];
            int i = 0;
            foreach (NPC tar in _log.FightData.Logic.Targets)
            {
                res[i++] = p.GetDPSTarget(_log, tar).Select(x => new JsonDPS(x)).ToArray();
            }
            return res;
        }

        private JsonGameplayStats[][] BuildStatsTarget(Player p)
        {
            var res = new JsonGameplayStats[_log.FightData.Logic.Targets.Count][];
            int i = 0;
            foreach (NPC tar in _log.FightData.Logic.Targets)
            {
                res[i++] = p.GetGameplayStats(_log, tar).Select(x => new JsonGameplayStats(x)).ToArray();
            }
            return res;
        }

        private static List<JsonDeathRecap> BuildDeathRecap(List<Player.DeathRecap> recaps)
        {
            if (recaps == null)
            {
                return null;
            }
            var res = new List<JsonDeathRecap>();
            foreach (Player.DeathRecap recap in recaps)
            {
                res.Add(new JsonDeathRecap(recap));
            }
            return res;
        }

        private List<JsonBuffDamageModifierData> BuildDamageModifiers(Dictionary<string, List<Player.DamageModifierData>> extra)
        {
            var dict = new Dictionary<int, List<JsonBuffDamageModifierItem>>();
            foreach (string key in extra.Keys)
            {
                int iKey = key.GetHashCode();
                string nKey = "d" + iKey;
                if (!_damageModDesc.ContainsKey(nKey))
                {
                    _damageModDesc[nKey] = new JsonLog.DamageModDesc(_log.DamageModifiers.DamageModifiersByName[key]);
                }
                dict[iKey] = extra[key].Select(x => new JsonBuffDamageModifierItem(x)).ToList();
            }
            var res = new List<JsonBuffDamageModifierData>();
            foreach (KeyValuePair<int, List<JsonBuffDamageModifierItem>> pair in dict)
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
            var res = new List<JsonBuffDamageModifierData>[_log.FightData.Logic.Targets.Count];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                NPC tar = _log.FightData.Logic.Targets[i];
                res[i] = BuildDamageModifiers(p.GetDamageModifierData(_log, tar));
            }
            return res;
        }

        private List<JsonConsumable> BuildConsumables(Player player)
        {
            List<Player.Consumable> input = player.GetConsumablesList(_log, 0, _log.FightData.FightDuration);
            var res = new List<JsonConsumable>();
            foreach (Player.Consumable food in input)
            {
                if (!_buffDesc.ContainsKey("b" + food.Buff.ID))
                {
                    _buffDesc["b" + food.Buff.ID] = new JsonLog.BuffDesc(food.Buff);
                }
                res.Add(new JsonConsumable(food));
            }
            return input.Count > 0 ? res : null;
        }

        private static List<int[]> BuildBuffStates(BuffsGraphModel bgm)
        {
            if (bgm == null || bgm.BuffChart.Count == 0)
            {
                return null;
            }
            var res = bgm.BuffChart.Select(x => new int[2] { (int)x.Start, x.Value }).ToList();
            return res.Count > 0 ? res : null;
        }

        private List<JsonDamageDist>[][] BuildDamageDist(AbstractSingleActor p)
        {
            var res = new List<JsonDamageDist>[_log.FightData.Logic.Targets.Count][];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                NPC target = _log.FightData.Logic.Targets[i];
                res[i] = BuildDamageDist(p, target);
            }
            return res;
        }

        private List<JsonDamageDist>[][] BuildDamageDist(Minions p)
        {
            var res = new List<JsonDamageDist>[_log.FightData.Logic.Targets.Count][];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                NPC target = _log.FightData.Logic.Targets[i];
                res[i] = BuildDamageDist(p, target);
            }
            return res;
        }

        private List<JsonDamageDist>[] BuildDamageDist(AbstractSingleActor p, NPC target)
        {
            var res = new List<JsonDamageDist>[_phases.Count];
            for (int i = 0; i < _phases.Count; i++)
            {
                PhaseData phase = _phases[i];
                res[i] = BuildDamageDist(p.GetJustPlayerDamageLogs(target, _log, phase));
            }
            return res;
        }

        private List<JsonDamageDist>[] BuildDamageTaken(AbstractSingleActor p)
        {
            var res = new List<JsonDamageDist>[_phases.Count];
            for (int i = 0; i < _phases.Count; i++)
            {
                PhaseData phase = _phases[i];
                res[i] = BuildDamageDist(p.GetDamageTakenLogs(null, _log, phase.Start, phase.End));
            }
            return res;
        }

        private List<JsonDamageDist>[] BuildDamageDist(Minions p, NPC target)
        {
            var res = new List<JsonDamageDist>[_phases.Count];
            for (int i = 0; i < _phases.Count; i++)
            {
                PhaseData phase = _phases[i];
                res[i] = BuildDamageDist(p.GetDamageLogs(target, _log, phase));
            }
            return res;
        }

        private List<JsonDamageDist> BuildDamageDist(List<AbstractDamageEvent> dls)
        {
            var res = new List<JsonDamageDist>();
            var dict = dls.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<SkillItem, List<AbstractDamageEvent>> pair in dict)
            {
                if (pair.Value.Count == 0)
                {
                    continue;
                }
                SkillItem skill = pair.Key;
                bool indirect = pair.Value.Exists(x => x is NonDirectDamageEvent);
                if (indirect)
                {
                    if (!_buffDesc.ContainsKey("b" + pair.Key))
                    {
                        if (_log.Buffs.BuffsByIds.TryGetValue(pair.Key.ID, out Buff buff))
                        {
                            _buffDesc["b" + pair.Key] = new JsonLog.BuffDesc(buff);
                        }
                        else
                        {
                            var auxBoon = new Buff(skill.Name, pair.Key.ID, skill.Icon);
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
                var filteredList = pair.Value.Where(x => !x.HasDowned).ToList();
                if (filteredList.Count == 0)
                {
                    continue;
                }
                string prefix = indirect ? "b" : "s";
                res.Add(new JsonDamageDist(filteredList, indirect, pair.Key.ID));
            }
            return res;
        }

        private List<JsonMinions> BuildMinions(AbstractSingleActor master)
        {
            var mins = new List<JsonMinions>();
            foreach (Minions minions in master.GetMinions(_log).Values)
            {
                var totalDamage = new List<int>();
                var totalShieldDamage = new List<int>();
                var totalTargetDamage = new List<int>[_log.FightData.Logic.Targets.Count];
                var totalTargetShieldDamage = new List<int>[_log.FightData.Logic.Targets.Count];
                foreach (PhaseData phase in _phases)
                {
                    int tot = 0;
                    int shdTot = 0;
                    foreach (AbstractDamageEvent de in minions.GetDamageLogs(null, _log, phase))
                    {
                        tot += de.Damage;
                        shdTot = de.ShieldDamage;
                    }
                    totalDamage.Add(tot);
                    totalShieldDamage.Add(shdTot);
                }
                for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
                {
                    NPC tar = _log.FightData.Logic.Targets[i];
                    var totalTarDamage = new List<int>();
                    var totalTarShieldDamage = new List<int>();
                    foreach (PhaseData phase in _phases)
                    {
                        int tot = 0;
                        int shdTot = 0;
                        foreach (AbstractDamageEvent de in minions.GetDamageLogs(tar, _log, phase))
                        {
                            tot += de.Damage;
                            shdTot = de.ShieldDamage;
                        }
                        totalTarDamage.Add(tot);
                        totalTarShieldDamage.Add(shdTot);
                    }
                    totalTargetDamage[i] = totalTarDamage;
                    totalTargetShieldDamage[i] = totalTarShieldDamage;
                }
                var min = new JsonMinions()
                {
                    Name = minions.Character,
                    Rotation = BuildRotation(minions.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    TotalDamageDist = BuildDamageDist(minions, null),
                    TargetDamageDist = BuildDamageDist(minions),
                    TotalDamage = totalDamage,
                    TotalShieldDamage = totalShieldDamage,
                    TotalTargetShieldDamage = totalTargetShieldDamage,
                    TotalTargetDamage = totalTargetDamage,
                };
                mins.Add(min);
            }
            return mins;
        }

        private List<JsonRotation> BuildRotation(List<AbstractCastEvent> cls)
        {
            var dict = new Dictionary<long, List<JsonSkill>>();
            foreach (AbstractCastEvent cl in cls)
            {
                SkillItem skill = cl.Skill;
                string skillName = skill.Name;
                if (!_skillDesc.ContainsKey("s" + cl.SkillId))
                {
                    _skillDesc["s" + cl.SkillId] = new JsonLog.SkillDesc(skill);
                }
                var jSkill = new JsonSkill(cl);
                if (dict.TryGetValue(cl.SkillId, out List<JsonSkill> list))
                {
                    list.Add(jSkill);
                }
                else
                {
                    dict[cl.SkillId] = new List<JsonSkill>()
                    {
                        jSkill
                    };
                }
            }
            var res = new List<JsonRotation>();
            foreach (KeyValuePair<long, List<JsonSkill>> pair in dict)
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

            foreach (PhaseData phase in _phases)
            {
                var phaseJson = new JsonPhase(phase);
                foreach (NPC tar in phase.Targets)
                {
                    phaseJson.Targets.Add(_log.FightData.Logic.Targets.IndexOf(tar));
                }
                log.Phases.Add(phaseJson);
                for (int j = 1; j < _phases.Count; j++)
                {
                    PhaseData curPhase = _phases[j];
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

        private List<JsonTargetBuffs> BuildTargetBuffs(List<Dictionary<long, FinalNPCBuffs>> npcBuffs, List<Dictionary<long, FinalBuffsDictionary>> npcBuffsDictionary, NPC target)
        {
            var boons = new List<JsonTargetBuffs>();

            foreach (KeyValuePair<long, FinalNPCBuffs> pair in npcBuffs[0])
            {
                if (!_buffDesc.ContainsKey("b" + pair.Key))
                {
                    _buffDesc["b" + pair.Key] = new JsonLog.BuffDesc(_log.Buffs.BuffsByIds[pair.Key]);
                }
                var data = new List<JsonTargetBuffsData>();
                for (int i = 0; i < _phases.Count; i++)
                {
                    var value = new JsonTargetBuffsData(npcBuffs[i][pair.Key], npcBuffsDictionary[i][pair.Key]);
                    data.Add(value);
                }
                var jsonBuffs = new JsonTargetBuffs()
                {
                    States = BuildBuffStates(target.GetBuffGraphs(_log)[pair.Key]),
                    BuffData = data,
                    Id = pair.Key
                };
                boons.Add(jsonBuffs);
            }

            return boons;
        }

        private List<JsonBuffsGeneration> BuildPlayerBuffGenerations(List<Dictionary<long, GeneralStatistics.FinalBuffs>> statUptimes)
        {
            var uptimes = new List<JsonBuffsGeneration>();
            foreach (KeyValuePair<long, GeneralStatistics.FinalBuffs> pair in statUptimes[0])
            {
                Buff buff = _log.Buffs.BuffsByIds[pair.Key];
                if (!_buffDesc.ContainsKey("b" + pair.Key))
                {
                    _buffDesc["b" + pair.Key] = new JsonLog.BuffDesc(buff);
                }
                var data = new List<JsonBuffsGenerationData>();
                for (int i = 0; i < _phases.Count; i++)
                {
                    data.Add(new JsonBuffsGenerationData(statUptimes[i][pair.Key]));
                }
                var jsonBuffs = new JsonBuffsGeneration()
                {
                    BuffData = data,
                    Id = pair.Key
                };
                uptimes.Add(jsonBuffs);
            }

            if (!uptimes.Any())
            {
                return null;
            }

            return uptimes;
        }

        private List<JsonBuffsUptime> BuildPlayerBuffUptimes(List<Dictionary<long, GeneralStatistics.FinalBuffs>> statUptimes, Player player)
        {
            var uptimes = new List<JsonBuffsUptime>();
            foreach (KeyValuePair<long, GeneralStatistics.FinalBuffs> pair in statUptimes[0])
            {
                Buff buff = _log.Buffs.BuffsByIds[pair.Key];
                if (!_buffDesc.ContainsKey("b" + pair.Key))
                {
                    _buffDesc["b" + pair.Key] = new JsonLog.BuffDesc(buff);
                }
                if (buff.Nature == Buff.BuffNature.GraphOnlyBuff && buff.Source == Buff.ProfToEnum(player.Prof))
                {
                    if (player.GetBuffDistribution(_log, 0).GetUptime(pair.Key) > 0)
                    {
                        if (_personalBuffs.TryGetValue(player.Prof, out HashSet<long> list) && !list.Contains(pair.Key))
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
                var data = new List<JsonBuffsUptimeData>();
                for (int i = 0; i < _phases.Count; i++)
                {
                    data.Add(new JsonBuffsUptimeData(statUptimes[i][pair.Key]));
                }
                var jsonBuffs = new JsonBuffsUptime()
                {
                    States = BuildBuffStates(player.GetBuffGraphs(_log)[pair.Key]),
                    BuffData = data,
                    Id = pair.Key
                };
                uptimes.Add(jsonBuffs);
            }

            if (!uptimes.Any())
            {
                return null;
            }

            return uptimes;
        }
    }
}
