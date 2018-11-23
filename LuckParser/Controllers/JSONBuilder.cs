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

        private readonly string[] _uploadLink;
        //
        private readonly Dictionary<long, string> _skillNames = new Dictionary<long, string>();
        private readonly Dictionary<long, string> _buffNames = new Dictionary<long, string>();
        private readonly Dictionary<string, List<long>> _personalBuffs = new Dictionary<string, List<long>>();

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
            GraphHelper.Settings = settings;

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
            log.Success = _log.FightData.Success ? 1 : 0;
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
                        Player = ml.Player.Character
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
                    TotalHealth = target.Health,
                    AvgBoons = _statistics.AvgTargetBoons[target],
                    AvgConditions = _statistics.AvgTargetConditions[target],
                    Dps = BuildDPS(_statistics.TargetDps[target]),
                    Buffs = BuildTargetBuffs(_statistics.TargetConditions[target], target),
                    HitboxHeight = target.HitboxHeight,
                    HitboxWidth = target.HitboxWidth,
                    Dps1s = Build1SDPS(target, null),
                    Rotation = BuildRotation(target.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    FirstAware = (int)(target.FirstAware - _log.FightData.FightStart),
                    LastAware = (int)(target.LastAware - _log.FightData.FightStart),
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
                    Character = player.Character,
                    Account = player.Account,
                    Condition = player.Condition,
                    Concentration = player.Concentration,
                    Healing = player.Healing,
                    Toughness = player.Toughness,
                    Weapons = player.GetWeaponsArray(_log).Where(w => w != null).ToArray(),
                    Group = player.Group,
                    Profession = player.Prof,
                    Dps1s = Build1SDPS(player, null),
                    TargetDps1s = Build1SDPS(player),
                    DpsAll = BuildDPS(_statistics.DpsAll[player]),
                    DpsTargets = BuildDPSTarget(_statistics.DpsTarget, player),
                    StatsAll = BuildStatsAll(_statistics.StatsAll[player]),
                    StatsTargets = BuildStatsTarget(_statistics.StatsTarget, player),
                    Defenses = BuildDefenses(_statistics.Defenses[player]),
                    Rotation = BuildRotation(player.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    Support = BuildSupport(_statistics.Support[player]),
                    SelfBuffs = BuildBuffUptime(_statistics.SelfBoons[player], player),
                    GroupBuffs = BuildBuffUptime(_statistics.GroupBoons[player], player),
                    OffGroupBuffs = BuildBuffUptime(_statistics.OffGroupBoons[player], player),
                    SquadBuffs = BuildBuffUptime(_statistics.SquadBoons[player], player),
                    Minions = BuildMinions(player),
                    TotalDamageDist = BuildDamageDist(player, null),
                    TargetDamageDist = BuildDamageDist(player),
                    TotalDamageTaken = BuildDamageTaken(player),
                    DeathRecap = BuilDeathRecap(player),
                    Consumables = BuildConsumables(player),
                    AvgBoonsStates = BuildBuffStates(player.GetBoonGraphs(_log)[Boon.NumberOfBoonsID]),
                    AvgConditionsStates = BuildBuffStates(player.GetBoonGraphs(_log)[Boon.NumberOfConditionsID]),
                });
            }
        }

        private List<JsonConsumable> BuildConsumables(Player player)
        {
            List<JsonConsumable> res = new List<JsonConsumable>();
            foreach(var food in player.GetConsumablesList(_log,0,_log.FightData.FightDuration))
            {
                JsonConsumable val = new JsonConsumable() {
                    ID = food.Item.ID,
                    Time = food.Time,
                    Duration = food.Duration,
                    Stack = food.Stack
                };
                _buffNames[food.Item.ID] = food.Item.Name;
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

        private List<JsonDeathRecap> BuilDeathRecap(Player player)
        {
            List<JsonDeathRecap> res = new List<JsonDeathRecap>();
            long start = _log.FightData.FightStart;
            long end = _log.FightData.FightEnd;
            List<CombatItem> deads = _log.CombatData.GetStates(player.InstID, ParseEnum.StateChange.ChangeDead, start, end);
            List<CombatItem> downs = _log.CombatData.GetStates(player.InstID, ParseEnum.StateChange.ChangeDown, start, end);
            long lastTime = start;
            List<DamageLog> damageLogs = player.GetDamageTakenLogs(null, _log, 0, _log.FightData.FightDuration);
            foreach (CombatItem dead in deads)
            {
                JsonDeathRecap recap = new JsonDeathRecap()
                {
                    Time = (int)(dead.Time - start)
                };
                CombatItem downed = downs.LastOrDefault(x => x.Time <= dead.Time && x.Time >= lastTime);
                if (downed != null)
                {
                    List<DamageLog> damageToDown = damageLogs.Where(x => x.Time < downed.Time - start && x.Damage > 0 && x.Time > lastTime - start).ToList();
                    recap.ToDown = damageToDown.Count > 0 ? new List<JsonDeathRecap.DamageItem>() : null;
                    int damage = 0;
                    for (int i = damageToDown.Count - 1; i >= 0; i--)
                    {
                        DamageLog dl = damageToDown[i];
                        AgentItem ag = _log.AgentData.GetAgentByInstID(dl.SrcInstId, dl.Time + start);
                        JsonDeathRecap.DamageItem item = new JsonDeathRecap.DamageItem()
                        {
                            Time = (int)dl.Time,
                            Condi = dl.IsCondi,
                            Skill = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : ""
                        };
                        damage += dl.Damage;
                        recap.ToDown.Add(item);
                        if (damage > 20000)
                        {
                            break;
                        }
                    }
                    List<DamageLog> damageToKill = damageLogs.Where(x => x.Time > downed.Time - start && x.Time < dead.Time - start && x.Damage > 0 && x.Time > lastTime - start).ToList();
                    recap.ToKill = damageToKill.Count > 0 ? new List<JsonDeathRecap.DamageItem>() : null;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        DamageLog dl = damageToKill[i];
                        AgentItem ag = _log.AgentData.GetAgentByInstID(dl.SrcInstId, dl.Time + start);
                        JsonDeathRecap.DamageItem item = new JsonDeathRecap.DamageItem()
                        {
                            Time = (int)dl.Time,
                            Condi = dl.IsCondi,
                            Skill = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : ""
                        };
                        recap.ToKill.Add(item);
                    }
                }
                else
                {
                    recap.ToDown = null;
                    List<DamageLog> damageToKill = damageLogs.Where(x => x.Time < dead.Time - start && x.Damage > 0 && x.Time > lastTime - start).ToList();
                    recap.ToKill = damageToKill.Count >0 ? new List<JsonDeathRecap.DamageItem>() : null;
                    int damage = 0;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        DamageLog dl = damageToKill[i];
                        AgentItem ag = _log.AgentData.GetAgentByInstID(dl.SrcInstId, dl.Time + start);
                        JsonDeathRecap.DamageItem item = new JsonDeathRecap.DamageItem()
                        {
                            Time = (int)dl.Time,
                            Condi = dl.IsCondi,
                            Skill = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : ""
                        };
                        damage += dl.Damage;
                        recap.ToKill.Add(item);
                        if (damage > 20000)
                        {
                            break;
                        }
                    }
                }
                lastTime = dead.Time;
                res.Add(recap);
            }
            return res.Count > 0 ? res : null;
        }

        private Dictionary<long, JsonDamageDist>[][] BuildDamageDist(AbstractMasterPlayer p)
        {
            Dictionary<long, JsonDamageDist>[][] res = new Dictionary<long, JsonDamageDist>[_log.FightData.Logic.Targets.Count][];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                Target target = _log.FightData.Logic.Targets[i];
                res[i] = BuildDamageDist(p, target);
            }
            return res;
        }

        private Dictionary<long, JsonDamageDist>[][] BuildDamageDist(Minions p)
        {
            Dictionary<long, JsonDamageDist>[][] res = new Dictionary<long, JsonDamageDist>[_log.FightData.Logic.Targets.Count][];
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                Target target = _log.FightData.Logic.Targets[i];
                res[i] = BuildDamageDist(p, target);
            }
            return res;
        }

        private Dictionary<long, JsonDamageDist>[] BuildDamageDist(AbstractMasterPlayer p, Target target)
        {
            Dictionary<long, JsonDamageDist>[] res = new Dictionary<long, JsonDamageDist>[_statistics.Phases.Count];
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phase = _statistics.Phases[i];
                res[i] = BuildDamageDist(p.GetJustPlayerDamageLogs(target, _log, phase.Start, phase.End));
            }
            return res;
        }

        private Dictionary<long, JsonDamageDist>[] BuildDamageTaken(AbstractMasterPlayer p)
        {
            Dictionary<long, JsonDamageDist>[] res = new Dictionary<long, JsonDamageDist>[_statistics.Phases.Count];
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phase = _statistics.Phases[i];
                res[i] = BuildDamageDist(p.GetDamageTakenLogs(null, _log, phase.Start, phase.End));
            }
            return res;
        }

        private Dictionary<long, JsonDamageDist>[] BuildDamageDist(Minions p, Target target)
        {
            Dictionary<long, JsonDamageDist>[] res = new Dictionary<long, JsonDamageDist>[_statistics.Phases.Count];
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phase = _statistics.Phases[i];
                res[i] = BuildDamageDist(p.GetDamageLogs(target, _log, phase.Start, phase.End));
            }
            return res;
        }

        private Dictionary<long, JsonDamageDist> BuildDamageDist(List<DamageLog> dls)
        {
            Dictionary<long, JsonDamageDist> res = new Dictionary<long, JsonDamageDist>();
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
                    if(!_skillNames.ContainsKey(pair.Key))
                    {
                        _skillNames[pair.Key] = skill.Name;
                    }
                }
                List<DamageLog> filteredList = pair.Value.Where(x => x.Result != ParseEnum.Result.Downed).ToList();
                res[pair.Key] = new JsonDamageDist()
                {
                    Hits = filteredList.Count,
                    Damage = filteredList.Sum(x => x.Damage),
                    Condi = filteredList.First().IsCondi,
                    Min = filteredList.Min(x => x.Damage),
                    Max = filteredList.Max(x => x.Damage),
                    Flank = filteredList.Count(x => x.IsFlanking > 0),
                    Crit = filteredList.Count(x => x.Result == ParseEnum.Result.Crit),
                    Glance = filteredList.Count(x => x.Result == ParseEnum.Result.Glance),
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
                    Name = minions.Character,
                    Rotation = BuildRotation(minions.GetCastLogs(_log, 0, _log.FightData.FightDuration)),
                    TotalDamageDist = BuildDamageDist(minions, null),
                    TargetDamageDist = BuildDamageDist(minions),
                };
                mins.Add(min);
            }
            return mins;
        }


        private List<int> Build1SDPS(AbstractMasterPlayer player, Target target)
        {
            List<int> res = new List<int>();
            foreach (var pt in GraphHelper.GetTargetDPSGraph(_log, player, 0, _statistics.Phases[0], GraphHelper.GraphMode.S1, target))
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

        private Dictionary<long, List<JsonSkill>> BuildRotation(List<CastLog> cls)
        {
            Dictionary<long, List<JsonSkill>> res = new Dictionary<long, List<JsonSkill>>();
            SkillData skillList = _log.SkillData;
            foreach (CastLog cl in cls)
            {
                SkillItem skill = skillList.Get(cl.SkillId);
                GW2APISkill skillApi = skill?.ApiSkill;
                string skillName = skill.Name;
                _skillNames[cl.SkillId] = skillName;
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
                    AutoAttack = skillApi != null && skillApi.slot == "Weapon_1" ? 1 : 0,
                    Quickness = cl.StartActivation == ParseEnum.Activation.Quickness ? 1 : 0
                };
                if (res.TryGetValue(cl.SkillId, out var list))
                {
                    list.Add(jSkill);
                } else
                {
                    res[cl.SkillId] = new List<JsonSkill>()
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
                    Name = phase.Name
                };
                log.Phases.Add(phaseJson);
            }
        }

        // Statistics to Json Converters ////////////////////////////////////////////////////

        private bool ContainsTargetBoon(long boon, Dictionary<long, Statistics.FinalTargetBoon>[] statBoons)
        {
            for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
            {
                if (statBoons[phaseIndex][boon].Uptime > 0) return true;
                if (statBoons[phaseIndex][boon].Generated.Any(x => x.Value > 0)) return true;
                if (statBoons[phaseIndex][boon].Overstacked.Any(x => x.Value > 0)) return true;
            }

            return false;
        }

        private void MakePhaseTargetBoon(JsonTargetBuffs boon, int phase, Statistics.FinalTargetBoon value)
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

        private Dictionary<long, JsonTargetBuffs> BuildTargetBuffs(Dictionary<long, Statistics.FinalTargetBoon>[] statBoons, Target target)
        {
            int phases = _statistics.Phases.Count;
            var boons = new Dictionary<long, JsonTargetBuffs>();

            var boonsFound = new List<long>();
            var boonsNotFound = new List<long>();

            for (int phaseIndex = 0; phaseIndex < phases; phaseIndex++)
            {
                foreach (var boon in statBoons[phaseIndex])
                {
                    _buffNames[boon.Key] = Boon.BoonsByIds[boon.Key].Name;
                    if (boonsFound.Contains(boon.Key))
                    {
                        MakePhaseTargetBoon(boons[boon.Key], phaseIndex, boon.Value);
                    }
                    else if (!boonsNotFound.Contains(boon.Key))
                    {
                        if (ContainsTargetBoon(boon.Key, statBoons))
                        {
                            boonsFound.Add(boon.Key);

                            boons[boon.Key] = new JsonTargetBuffs(phases);
                            MakePhaseTargetBoon(boons[boon.Key], phaseIndex, boon.Value);
                            if (target.GetBoonGraphs(_log).TryGetValue(boon.Key, out var bgm))
                            {
                                boons[boon.Key].States = BuildBuffStates(bgm);
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
                    Boon buff = Boon.BoonsByIds[boon.Key];
                    _buffNames[boon.Key] = buff.Name;
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
                                _personalBuffs[player.Prof] = new List<long>()
                                {
                                    boon.Key
                                };
                            }
                        }
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
                                uptimes[boon.Key].States = BuildBuffStates(bgm);
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
                    entry.SetValue(upperObject[i].GetType().GetField(field.Name).GetValue(upperObject[i]), i);
                }
            }
        }
    }
}