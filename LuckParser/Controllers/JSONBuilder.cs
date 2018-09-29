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

        readonly bool _devMode;

        readonly Statistics _statistics;
        readonly StreamWriter _sw;

        private readonly String[] _uploadLink;
        //
        private readonly Dictionary<long, string> _skillNames = new Dictionary<long, string>();
        private readonly Dictionary<long, string> _buffNames = new Dictionary<long, string>();
        private readonly Dictionary<long, string> _buffIcons = new Dictionary<long, string>();
        private readonly Dictionary<long, string> _skillIcons = new Dictionary<long, string>();
        private readonly Dictionary<string, MechanicDesc> _mechanicData = new Dictionary<string, MechanicDesc>();

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

        public JSONBuilder(StreamWriter sw, ParsedLog log, SettingsContainer settings, Statistics statistics, bool devMode, string[] UploadString)
        {
            _log = log;
            _sw = sw;
            _settings = settings;
            GraphHelper.Settings = settings;

            _statistics = statistics;
            
           _uploadLink = UploadString;
            _devMode = devMode;
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
                Formatting = _settings.IndentJSON && !_devMode ? Formatting.Indented : Formatting.None
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
            log.Success = _log.LogData.Success ? 1 : 0;
            log.SkillNames = _skillNames;
            log.BuffNames = _buffNames;
            if (!_devMode)
            {
                log.UploadLinks = _uploadLink;
            } else
            {
                log.ED = new JsonExtraLog()
                {
                    BuffIcons = _buffIcons,
                    SkillIcons = _skillIcons,
                    FightIcon = _log.FightData.Logic.IconUrl,
                    MechanicData = _mechanicData
                };
            }
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
                log.Mechanics = new List<JsonMechanic>();
                foreach (MechanicLog ml in mechanicLogs)
                {
                    JsonMechanic mech = new JsonMechanic
                    {
                        Time = ml.Time,
                        Player = ml.Player.Character,
                        Name = ml.InGameName,
                    };
                    if (_devMode)
                    {
                        if (!_mechanicData.ContainsKey(ml.ShortName))
                        {
                            _mechanicData[ml.ShortName] = new MechanicDesc()
                            {
                                PlotlySymbol = ml.PlotlySymbol,
                                PlotlyColor = ml.PlotlyColor,
                                Description = ml.Description,
                                PlotlyName = ml.PlotlyName
                            };
                        }
                        mech.ED = new JsonMechanic.JsonExtraMechanic()
                        {
                            SN = ml.ShortName,
                            S = ml.Skill
                        };
                        mech.ED.E = ml.Enemy ? 1 : 0;
                        if (mech.ED.E == 1)
                        {
                            if (ml.Player.GetType() == typeof(Boss))
                            {
                                mech.ED.TI = -1;
                            }
                            else
                            {
                                mech.ED.TI = _log.FightData.Logic.Targets.IndexOf((Boss)ml.Player);
                            }
                        }
                        else
                        {
                            mech.ED.TI = _log.PlayerList.IndexOf((Player)ml.Player);
                        }
                    }
                    log.Mechanics.Add(mech);
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
                boss.Conditions = BuildBossBuffs(_statistics.BossConditions[target], target);
                boss.HitboxHeight = target.HitboxHeight;
                boss.HitboxWidth = target.HitboxWidth;
                boss.Dps1s = Build1SDPS(target, null);
                boss.Rotation = BuildRotation(target.GetCastLogs(_log, 0, _log.FightData.FightDuration));
                boss.FirstAware = (int)(target.FirstAware - _log.FightData.FightStart);
                boss.LastAware = (int)(target.LastAware - _log.FightData.FightStart);
                boss.TotalPersonalDamage = BuildPersonalDamage(target, null);
                boss.Minions = BuildMinions(target);
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
                    Rotation = BuildRotation(player.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    Support = BuildSupport(_statistics.Support[player]),
                    SelfBoons = BuildBuffUptime(_statistics.SelfBoons[player], player),
                    GroupBoons = BuildBuffUptime(_statistics.GroupBoons[player], player),
                    OffGroupBoons = BuildBuffUptime(_statistics.OffGroupBoons[player], player),
                    SquadBoons = BuildBuffUptime(_statistics.SquadBoons[player], player),
                    Minions = BuildMinions(player)
                });
            }
        }

        private List<JsonMinions> BuildMinions(AbstractMasterPlayer master)
        {
            List<JsonMinions> mins = new List<JsonMinions>();
            foreach (Minions minions in master.GetMinions(_log).Values)
            {
                JsonMinions min = new JsonMinions()
                {
                    Name = minions.Character,
                    Rotation = BuildRotation(minions.GetCastLogs(_log,0,_log.FightData.FightDuration)),
                };
                min.TotalDamage = new int[_statistics.Phases.Count];
                for (int i = 0; i < _statistics.Phases.Count; i++)
                {
                    min.TotalDamage[i] = minions.GetDamageLogs(null, _log, _statistics.Phases[i].Start, _statistics.Phases[i].End).Sum(x => x.Damage);
                }
                min.TotalTargetDamage = new int[_log.FightData.Logic.Targets.Count][];
                for (int j = 0; j < _log.FightData.Logic.Targets.Count; j++)
                {
                    Boss target = _log.FightData.Logic.Targets[j];
                    min.TotalTargetDamage[j] = new int[_statistics.Phases.Count];
                    for (int i = 0; i < _statistics.Phases.Count; i++)
                    {
                        min.TotalDamage[i] = minions.GetDamageLogs(target, _log, _statistics.Phases[i].Start, _statistics.Phases[i].End).Sum(x => x.Damage);
                    }
                }
                if (_devMode)
                {
                    min.ED = new JsonMinions.JsonExtraMinions()
                    {
                        ID = minions.MinionID,
                    };
                }
                mins.Add(min);
            }
            return mins;
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

        private List<JsonSkill> BuildRotation(List<CastLog> cls)
        {
            List<JsonSkill> res = new List<JsonSkill>();
            SkillData skillList = _log.SkillData;
            foreach (CastLog cl in cls)
            {
                GW2APISkill skill = skillList.Get(cl.SkillId)?.ApiSkill;
                string skillName = skill == null ? skillList.GetName(cl.SkillId) : skill.name;
                if (cl.SkillId == SkillItem.WeaponSwapId)
                {
                    skillName = "Weapon Swap";
                }
                _skillNames[cl.SkillId] = skillName;           
                JsonSkill jSkill = new JsonSkill
                {
                    Skill = cl.SkillId,
                    Time = (int)cl.Time,
                    Duration = cl.ActualDuration
                };
                if (_devMode)
                {
                    if (!_skillIcons.ContainsKey(cl.SkillId))
                    {
                        string skillIcon = "";
                        if (skill != null && cl.SkillId != -2)
                        {
                            skillIcon = skill.icon;
                        }
                        else
                        {
                            if (skillName == "Dodge")
                            {
                                skillIcon = HTMLHelper.GetLink("Dodge");
                            }
                            else if (skillName == "Resurrect")
                            {
                                skillIcon = HTMLHelper.GetLink("Resurrect");
                            }
                            else if (skillName == "Bandage")
                            {
                                skillIcon = HTMLHelper.GetLink("Bandage");
                            }
                            else if (cl.SkillId == SkillItem.WeaponSwapId)
                            {
                                skillIcon = HTMLHelper.GetLink("Swap");
                            }
                        }
                        if (skillIcon.Length > 0)
                        {
                            _skillIcons[cl.SkillId] = skillIcon;
                        }
                    }
                    int timeGained = 0;
                    if (cl.EndActivation == ParseEnum.Activation.CancelFire && cl.ActualDuration < cl.ExpectedDuration)
                    {
                        timeGained = cl.ExpectedDuration - cl.ActualDuration;
                    } else if (cl.EndActivation == ParseEnum.Activation.CancelCancel)
                    {
                        timeGained = -cl.ActualDuration;
                    }
                    jSkill.ED = new JsonSkill.JsonExtraSkill()
                    {
                        UQ = cl.StartActivation == ParseEnum.Activation.Quickness ? 1 : 0,
                        TS = timeGained,
                        A = skill != null && skill.slot == "Weapon_1" ? 1 : 0
                    };
                }
                res.Add(jSkill);
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
                if (_devMode)
                {
                    phaseJson.ED = new JsonPhase.JsonExtraPhase();
                    phaseJson.ED.TI = new int[phase.Targets.Count];
                    int i = 0;
                    foreach (Boss target in phase.Targets)
                    {
                        phaseJson.ED.TI[i++] = _log.FightData.Logic.Targets.IndexOf(target);
                    }
                    phaseJson.ED.DA = phase.DrawArea ? 1 : 0;
                    phaseJson.ED.DE = phase.DrawEnd ? 1 : 0;
                    phaseJson.ED.DS = phase.DrawStart ? 1 : 0;
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

        private Dictionary<long, JsonBossBuffs> BuildBossBuffs(Dictionary<long, Statistics.FinalBossBoon>[] statBoons, Boss boss)
        {
            int phases = _statistics.Phases.Count;
            var boons = new Dictionary<long, JsonBossBuffs>();

            var boonsFound = new List<long>();
            var boonsNotFound = new List<long>();

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                foreach (var boon in statBoons[phaseIndex])
                {
                    _buffNames[boon.Key] = Boon.BoonsByIds[boon.Key].Name;
                    if (_devMode)
                    {
                        _buffIcons[boon.Key] = Boon.BoonsByIds[boon.Key].Link;
                    }
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
                            if (boss.GetBoonGraphs(_log).TryGetValue(boon.Key, out var bgm))
                            {
                                foreach (BoonsGraphModel.Segment seg in bgm.BoonChart)
                                {
                                    boons[boon.Key].States.Add((int)seg.Start);
                                    boons[boon.Key].States.Add(seg.Value);
                                }
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

        private Dictionary<long, JsonBuffs> BuildBuffUptime(Dictionary<long, Statistics.FinalBoonUptime>[] statUptimes, Player player)
        {
            var uptimes = new Dictionary<long, JsonBuffs>();
            int phases = _statistics.Phases.Count;

            var boonsFound = new HashSet<long>();
            var boonsNotFound = new HashSet<long>();

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                foreach (var boon in statUptimes[phaseIndex])
                {
                    _buffNames[boon.Key] = Boon.BoonsByIds[boon.Key].Name;
                    if (_devMode)
                    {
                        _buffIcons[boon.Key] = Boon.BoonsByIds[boon.Key].Link;
                    }
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
                            if (player.GetBoonGraphs(_log).TryGetValue(boon.Key, out var bgm))
                            {
                                foreach (BoonsGraphModel.Segment seg in bgm.BoonChart)
                                {
                                    uptimes[boon.Key].States.Add((int)seg.Start);
                                    uptimes[boon.Key].States.Add(seg.Value);
                                }
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