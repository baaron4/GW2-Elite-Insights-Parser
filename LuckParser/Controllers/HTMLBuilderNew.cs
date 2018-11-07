using LuckParser.Models.DataModels;
using LuckParser.Models.HtmlModels;
using LuckParser.Models.ParseModels;
using NUglify;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LuckParser.Controllers
{
    class HTMLBuilderNew
    {
        private const string _scriptVersion = "0.5";
        private const int _scriptVersionRev = 11;
        private readonly SettingsContainer _settings;

        private readonly ParsedLog _log;

        private readonly Statistics _statistics;
        private Dictionary<long, Boon> _usedBoons = new Dictionary<long, Boon>();
        private Dictionary<long, SkillItem> _usedSkills = new Dictionary<long, SkillItem>();

        public HTMLBuilderNew(ParsedLog log, SettingsContainer settings, Statistics statistics)
        {
            _log = log;

            _settings = settings;
            HTMLHelper.Settings = settings;
            GraphHelper.Settings = settings;

            _statistics = statistics;
        }

        private static string FilterStringChars(string str)
        {
            string filtered = "";
            string filter = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
            foreach (char c in str)
            {
                if (filter.Contains(c))
                {
                    filtered += c;
                }
            }
            return filtered;
        }

        private List<int> ConvertGraph(List<Point> points)
        {
            List<int> graph = new List<int>();
            foreach (Point point in points)
            {
                graph.Add(point.Y);
            }
            return graph;
        }

        private double[] BuildTargetHealthData(int phaseIndex, Boss target)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];
            int duration = (int)phase.GetDuration("s");
            double[] chart = _statistics.TargetHealth[target].Skip((int)phase.Start / 1000).Take(duration + 1).ToArray();
            return chart;
        }

        private TargetChartDataDto BuildTargetGraphData(int phaseIndex, Boss target)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];
            return new TargetChartDataDto
            {
                total = ConvertGraph(GraphHelper.GetTotalDPSGraph(_log, target, phaseIndex, phase, GraphHelper.GraphMode.S1)),
                health = BuildTargetHealthData(phaseIndex, target)
            };
        }
        
        /// <summary>
        /// Creates the dps graph
        /// </summary>
        private List<PlayerChartDataDto> BuildPlayerGraphData(int phaseIndex)
        {
            List<PlayerChartDataDto> list = new List<PlayerChartDataDto>();
            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player p in _log.PlayerList)
            {
                PlayerChartDataDto pChar = new PlayerChartDataDto()
                {
                    total = ConvertGraph(GraphHelper.GetTotalDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S1)),
                    targets = new List<List<int>>()
                };
                foreach (Boss target in phase.Targets)
                {
                    pChar.targets.Add(ConvertGraph(GraphHelper.GetTargetDPSGraph(_log, p, phaseIndex, phase, GraphHelper.GraphMode.S1, target)));
                }
                list.Add(pChar);
            }
            return list;
        }
        /// <summary>
        /// Creates the dps table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<object>> BuildDPSData(int phaseIndex)
        {
            List<List<object>> list = new List<List<object>>(_log.PlayerList.Count);
            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalDPS dpsAll = _statistics.DpsAll[player][phaseIndex];
                Statistics.FinalStatsAll stats = _statistics.StatsAll[player][phaseIndex];

                List<object> playerData = new List<object>
                {
                    dpsAll.Damage,
                    dpsAll.Dps,
                    dpsAll.PowerDamage,
                    dpsAll.PowerDps,
                    dpsAll.CondiDamage,
                    dpsAll.CondiDps
                };

                list.Add(playerData);
            }
            return list;
        }
        private List<List<List<object>>> BuildDPSTargetsData(int phaseIndex)
        {
            List<List<List<object>>> list = new List<List<List<object>>>(_log.PlayerList.Count);
            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                List<List<object>> playerData = new List<List<object>>();

                foreach (Boss target in phase.Targets)
                {
                    List<object> tar = new List<object>();
                    playerData.Add(tar);
                    Statistics.FinalDPS dpsTarget = _statistics.DpsTarget[target][player][phaseIndex];
                    tar.Add(dpsTarget.Damage);
                    tar.Add(dpsTarget.Dps);
                    tar.Add(dpsTarget.PowerDamage);
                    tar.Add(dpsTarget.PowerDps);
                    tar.Add(dpsTarget.CondiDamage);
                    tar.Add(dpsTarget.CondiDps);
                }
                list.Add(playerData);
            }
            return list;
        }
        /// <summary>
        /// Creates the damage stats table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<object>> BuildDMGStatsData(int phaseIndex)
        {
            List<List<object>> list = new List<List<object>>();
            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalStatsAll stats = _statistics.StatsAll[player][phaseIndex];
                Statistics.FinalDPS dps = _statistics.DpsAll[player][phaseIndex];

                List<object> playerData = new List<object>
                {
                    stats.PowerLoopCount, //0
                    stats.CritablePowerLoopCount, //1
                    stats.CriticalRate, //2
                    stats.CriticalDmg, //3
                    
                    stats.ScholarRate, //4
                    stats.ScholarDmg, //5
                    dps.PlayerPowerDamage,//6
                    
                    stats.MovingRate, //7
                    stats.MovingDamage, //8
                    
                    stats.FlankingRate, //9
                    
                    stats.GlanceRate, //10

                    stats.Missed, //11
                    stats.Interrupts, //12
                    stats.Invulned, //13

                    stats.TimeWasted, //14
                    stats.Wasted, //15

                    stats.TimeSaved, //16
                    stats.Saved, //17

                    stats.SwapCount, //18
                    Math.Round(stats.StackDist, 2) //19
                };
                list.Add(playerData);
            }
            return list;
        }
        /// <summary>
        /// Creates the damage stats table for hits on just boss
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<List<object>>> BuildDMGStatsTargetsData(int phaseIndex)
        {
            List<List<List<object>>> list = new List<List<List<object>>>();

            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                List<List<object>> playerData = new List<List<object>>();
                foreach (Boss target in phase.Targets)
                {
                    Statistics.FinalStats statsTarget = _statistics.StatsTarget[target][player][phaseIndex];
                    Statistics.FinalDPS dpsTarget = _statistics.DpsTarget[target][player][phaseIndex];
                    playerData.Add(new List<object>(){
                        statsTarget.PowerLoopCount, //0

                        statsTarget.CritablePowerLoopCount, //1
                        statsTarget.CriticalRate, //2
                        statsTarget.CriticalDmg, //3
                        
                        statsTarget.ScholarRate, //4
                        statsTarget.ScholarDmg, //5
                        dpsTarget.PlayerPowerDamage,//6
                        
                        statsTarget.MovingRate, //7
                        statsTarget.MovingDamage, //8
                        
                        statsTarget.FlankingRate, //9
                        
                        statsTarget.GlanceRate, //10

                        statsTarget.Missed, //11
                        statsTarget.Interrupts, //12
                        statsTarget.Invulned //13
                    });
                }
                list.Add(playerData);
            }
            return list;
        }
        /// <summary>
        /// Creates the defense table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<object>> BuildDefenseData(int phaseIndex)
        {
            List<List<object>> list = new List<List<object>>();

            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalDefenses defenses = _statistics.Defenses[player][phaseIndex];
                Statistics.FinalStatsAll stats = _statistics.StatsAll[player][phaseIndex];

                List<object> playerData = new List<object>
                {
                    defenses.DamageTaken,
                    defenses.DamageBarrier,
                    defenses.BlockedCount,
                    defenses.InvulnedCount,
                    defenses.EvadedCount,
                    stats.DodgeCount,
                    stats.DownCount
                };

                if (stats.Died != 0.0)
                {
                    if (stats.Died < 0)
                    {
                        playerData.Add(-stats.Died + " time(s)");
                        playerData.Add("");
                    }
                    else
                    {
                        TimeSpan timedead = TimeSpan.FromMilliseconds(stats.Died);
                        playerData.Add(timedead.Minutes + " m " + timedead.Seconds + " s");
                        playerData.Add(timedead + "(" + Math.Round((timedead.TotalMilliseconds / phase.GetDuration()) * 100, 1) + "% Alive)");
                    }
                }
                else
                {
                    playerData.Add(0);
                    playerData.Add("Never died");
                }

                list.Add(playerData);
            }

            return list;
        }
        /// <summary>
        /// Creates the support table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<object>> BuildSupportData(int phaseIndex)
        {
            List<List<object>> list = new List<List<object>>();

            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalSupport support = _statistics.Support[player][phaseIndex];
                List<object> playerData = new List<object>(4)
                {
                    support.CondiCleanse,
                    support.CondiCleanseTime,
                    support.Resurrects,
                    support.ResurrectTime
                };
                list.Add(playerData);
            }
            return list;
        }
        /// <summary>
        /// Create the buff uptime table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="listToUse"></param>
        /// <param name="tableId"></param>
        /// <param name="phaseIndex"></param>
        private List<BoonData> BuildBuffUptimeData(List<Boon> listToUse, int phaseIndex)
        {
            List<PhaseData> phases = _statistics.Phases;
            List<BoonData> list = new List<BoonData>();
            bool boonTable = listToUse.Select(x => x.ID).Contains(740);
                
            foreach (Player player in _log.PlayerList)
            {
                BoonData boonData = new BoonData();

                Dictionary<long, Statistics.FinalBoonUptime> boons = _statistics.SelfBoons[player][phaseIndex];
                long fightDuration = phases[phaseIndex].GetDuration();
                       
                if (boonTable)
                {
                    boonData.avg = Math.Round(_statistics.StatsAll[player][phaseIndex].AvgBoons, 1);
                }
                foreach (Boon boon in listToUse)
                {
                    List<object> boonVals = new List<object>();
                    boonData.data.Add(boonVals);

                    boonVals.Add(boons[boon.ID].Uptime);
                    if (boonTable && boon.Type == Boon.BoonType.Intensity && boons[boon.ID].Presence > 0)
                    {
                        boonVals.Add(boons[boon.ID].Presence);
                    }
                }
                list.Add(boonData);
            }
            return list;
        }

        private Dictionary<string, List<Boon>> BuildPersonalBoonData(Dictionary<string, List<long>> dict)
        {
            Dictionary<string, List<Boon>> boonsBySpec = new Dictionary<string, List<Boon>>();
            // Collect all personal buffs by spec
            foreach (var pair in _log.PlayerListBySpec)
            {
                List<Player> players = pair.Value;
                HashSet<long> specBoonIds = new HashSet<long>(Boon.GetRemainingBuffsList(pair.Key).Select(x => x.ID));
                HashSet<Boon> boonToUse = new HashSet<Boon>();
                foreach (Player player in players)
                {
                    for (int i = 0; i < _statistics.Phases.Count; i++)
                    {
                        Dictionary<long, Statistics.FinalBoonUptime> boons = _statistics.SelfBoons[player][i];
                        foreach (Boon boon in _statistics.PresentPersonalBuffs[player.InstID])
                        {
                            if (boons[boon.ID].Uptime > 0 && specBoonIds.Contains(boon.ID))
                            {
                                boonToUse.Add(boon);
                            }
                        }
                    }
                }
                boonsBySpec[pair.Key] = boonToUse.ToList();
            }
            foreach (var pair in boonsBySpec)
            {
                dict[pair.Key] = new List<long>();
                foreach(Boon boon in pair.Value)
                {
                    dict[pair.Key].Add(boon.ID);
                    _usedBoons[boon.ID] = boon;
                }
            }
            return boonsBySpec;
        }

        private List<BoonData> BuildPersonalBuffUptimeData(Dictionary<string, List<Boon>> boonsBySpec, int phaseIndex)
        {
            List<BoonData> list = new List<BoonData>();
            foreach (Player player in _log.PlayerList)
            {
                BoonData boonData = new BoonData();

                Dictionary<long, Statistics.FinalBoonUptime> boons = _statistics.SelfBoons[player][phaseIndex];
                long fightDuration = _statistics.Phases[phaseIndex].GetDuration();
                
                foreach (Boon boon in boonsBySpec[player.Prof])
                {
                    List<object> boonVals = new List<object>();
                    boonData.data.Add(boonVals);
                    if (boons.TryGetValue(boon.ID, out var uptime))
                    {
                        boonVals.Add(uptime.Uptime);
                    } else
                    {
                        boonVals.Add(0);
                    }
                }
                list.Add(boonData);
            }
            return list;
        }

        private void BuildDmgModifiersData(int phaseIndex, List<List<object[]>> data, List<List<List<object[]>>> dataTargets)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                List<object[]> pData = new List<object[]>();
                List<List<object[]>> pDataTargets = new List<List<object[]>>();
                data.Add(pData);
                dataTargets.Add(pDataTargets);
                Dictionary<long, List<AbstractMasterPlayer.ExtraBoonData>> extraBoonDataAll = player.GetExtraBoonData(_log, null);
                foreach (var pair in extraBoonDataAll)
                {
                    var extraData = pair.Value[phaseIndex];
                    pData.Add(new object[]
                                {
                                    pair.Key,
                                    extraData.HitCount,
                                    extraData.TotalHitCount,
                                    extraData.DamageGain,
                                    extraData.TotalDamage
                                });
                }
                foreach (Boss target in phase.Targets)
                {
                    List<object[]> pTarget = new List<object[]>();
                    pDataTargets.Add(pTarget);
                    Dictionary<long, List<AbstractMasterPlayer.ExtraBoonData>> extraBoonDataTarget = player.GetExtraBoonData(_log, target);
                    foreach (var pair in extraBoonDataTarget)
                    {
                        var extraData = pair.Value[phaseIndex];
                        pTarget.Add(new object[]
                                    {
                                        pair.Key,
                                    extraData.HitCount,
                                    extraData.TotalHitCount,
                                    extraData.DamageGain,
                                    extraData.TotalDamage
                                    });
                    }
                }
            }
        }

        /// <summary>
        /// Create the self buff generation table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="listToUse"></param>
        /// <param name="tableId"></param>
        /// <param name="phaseIndex"></param>
        private List<BoonData> BuildBuffGenerationData(List<Boon> listToUse, int phaseIndex, string target)
        {
            List<BoonData> list = new List<BoonData>();

            foreach (Player player in _log.PlayerList)
            {
                BoonData boonData = new BoonData();

                Dictionary<long, Statistics.FinalBoonUptime> uptimes;
                if (target == "self") uptimes = _statistics.SelfBoons[player][phaseIndex];
                else if (target == "group") uptimes = _statistics.GroupBoons[player][phaseIndex];
                else if (target == "off") uptimes = _statistics.OffGroupBoons[player][phaseIndex];
                else if (target == "squad") uptimes = _statistics.SquadBoons[player][phaseIndex];
                else throw new InvalidOperationException("unknown target type");

                Dictionary<long, string> rates = new Dictionary<long, string>();
                foreach (Boon boon in listToUse)
                {
                    Statistics.FinalBoonUptime uptime = uptimes[boon.ID];
                    List<object> val = new List<object>(2)
                    {
                        uptime.Generation,
                        uptime.Overstack
                    };
                    boonData.data.Add(val);
                }
                list.Add(boonData);
            }
            return list;
        }

        /// <summary>
        /// Creates the rotation tab for a given player
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="simpleRotSize"></param>
        /// <param name="phaseIndex"></param>
        private List<double[]> BuildSimpleRotationTabData(AbstractPlayer p, int phaseIndex)
        {
            List<double[]> list = new List<double[]>();

            PhaseData phase = _statistics.Phases[phaseIndex];
            List<CastLog> casting = p.GetCastLogs(_log, phase.Start, phase.End);
            SkillData skillList = _log.SkillData;
            foreach (CastLog cl in casting)
            {
                if (!_usedSkills.ContainsKey(cl.SkillId)) _usedSkills.Add(cl.SkillId, skillList.Get(cl.SkillId));
                double[] rotEntry = new double[5];
                list.Add(rotEntry);
                rotEntry[0] = (cl.Time - phase.Start) / 1000.0;
                rotEntry[1] = cl.SkillId;
                rotEntry[2] = cl.SkillId == SkillItem.DodgeId ? 750 : cl.SkillId == SkillItem.WeaponSwapId ? 100 : cl.ActualDuration;
                rotEntry[3] = EncodeEndActivation(cl.EndActivation);
                rotEntry[4] = cl.StartActivation == ParseEnum.Activation.Quickness ? 1 : 0;
            }
            return list;
        }

        private int EncodeEndActivation(ParseEnum.Activation endActivation)
        {
            switch (endActivation)
            {
                case ParseEnum.Activation.CancelFire: return 1;
                case ParseEnum.Activation.CancelCancel: return 2;
                case ParseEnum.Activation.Reset: return 3;
                default: return 0;
            }
        }

        /// <summary>
        /// Creates the death recap tab for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private List<DeathRecapDto> BuildDeathRecap(Player p)
        {
            List<DeathRecapDto> res = new List<DeathRecapDto>();
            long start = _log.FightData.FightStart;
            long end = _log.FightData.FightEnd;
            List<CombatItem> deads = _log.CombatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeDead, start, end);
            List<CombatItem> downs = _log.CombatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeDown, start, end);
            long lastTime = start;
            List<DamageLog> damageLogs = p.GetDamageTakenLogs(_log, 0, _log.FightData.FightDuration);
            foreach (CombatItem dead in deads)
            {
                DeathRecapDto recap = new DeathRecapDto()
                {
                    time = (int)(dead.Time - start)
                };
                CombatItem down = downs.LastOrDefault(x => x.Time <= dead.Time && x.Time >= lastTime);
                if (down != null)
                {
                    List<DamageLog> damageToDown = damageLogs.Where(x => x.Time < down.Time - start && x.Damage > 0 && x.Time > lastTime - start).ToList();
                    recap.toDown = damageToDown.Count > 0 ? new List<object[]>() : null;
                    int damage = 0;
                    for (int i = damageToDown.Count - 1; i >= 0; i--)
                    {
                        DamageLog dl = damageToDown[i];
                        AgentItem ag = _log.AgentData.GetAgentByInstID(dl.SrcInstId, dl.Time + start);
                        object[] item = new object[] {
                            dl.Time,
                            dl.SkillId,
                            dl.Damage,
                            ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : "",
                            dl.IsCondi
                        };
                        damage += dl.Damage;
                        recap.toDown.Add(item);
                        if (damage > 20000)
                        {
                            break;
                        }
                    }
                    List<DamageLog> damageToKill = damageLogs.Where(x => x.Time > down.Time - start && x.Time < dead.Time - start && x.Damage > 0 && x.Time > lastTime - start).ToList();
                    recap.toKill = damageToKill.Count > 0 ? new List<object[]>() : null;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        DamageLog dl = damageToKill[i];
                        AgentItem ag = _log.AgentData.GetAgentByInstID(dl.SrcInstId, dl.Time + start);
                        object[] item = new object[] {
                            dl.Time,
                            dl.SkillId,
                            dl.Damage,
                            ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : ""
                        };
                        recap.toKill.Add(item);
                    }
                }
                else
                {
                    recap.toDown = null;
                    List<DamageLog> damageToKill = damageLogs.Where(x => x.Time < dead.Time - start && x.Damage > 0 && x.Time > lastTime - start).ToList();
                    recap.toKill = damageToKill.Count > 0 ? new List<object[]>() : null;
                    int damage = 0;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        DamageLog dl = damageToKill[i];
                        AgentItem ag = _log.AgentData.GetAgentByInstID(dl.SrcInstId, dl.Time + start);
                        object[] item = new object[] {
                            dl.Time,
                            dl.SkillId,
                            dl.Damage,
                            ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : ""
                        };
                        damage += dl.Damage;
                        recap.toKill.Add(item);
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

        private List<double[]> BuildDMGDistBodyData(List<CastLog> casting, List<DamageLog> damageLogs, long finalTotalDamage)
        {
            List<double[]> list = new List<double[]>();
            Dictionary<long, List<CastLog>> castLogsBySkill = casting.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            Dictionary<long, List<DamageLog>> damageLogsBySkill = damageLogs.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            Dictionary<long, Boon> conditionsById = _statistics.PresentConditions.ToDictionary(x => x.ID);
            SkillData skillList = _log.SkillData;
            foreach (KeyValuePair<long, List<DamageLog>> entry in damageLogsBySkill)
            {
                int totaldamage = 0, mindamage = 0, maxdamage = 0, casts = 0, hits = 0, crit = 0, flank = 0, glance = 0, timeswasted = 0, timessaved = 0;
                foreach (DamageLog dl in entry.Value)
                {
                    int curdmg = dl.Damage;
                    totaldamage += curdmg;
                    if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                    if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                    hits++;
                    if (dl.Result == ParseEnum.Result.Crit) crit++;
                    if (dl.Result == ParseEnum.Result.Glance) glance++;
                    if (dl.IsFlanking == 1) flank++;
                }

                bool isCondi = conditionsById.ContainsKey(entry.Key) || entry.Key == 873;
                if (isCondi)
                {
                    Boon condi = entry.Key == 873 ? Boon.BoonsByIds[873] : conditionsById[entry.Key];
                    if (!_usedBoons.ContainsKey(condi.ID)) _usedBoons.Add(condi.ID, condi);
                }
                else
                {
                    if (!_usedSkills.ContainsKey(entry.Key)) _usedSkills.Add(entry.Key, skillList.Get(entry.Key));
                }

                if (!isCondi && castLogsBySkill.TryGetValue(entry.Key, out List<CastLog> clList))
                {

                    casts = clList.Count;
                    foreach (CastLog cl in clList)
                    {
                        if (cl.EndActivation == ParseEnum.Activation.CancelCancel) timeswasted += cl.ActualDuration;
                        if (cl.EndActivation == ParseEnum.Activation.CancelFire && cl.ActualDuration < cl.ExpectedDuration)
                        {
                            timessaved += cl.ExpectedDuration - cl.ActualDuration;
                        }
                    }
                }

                double[] skillData = {
                    isCondi?1:0,
                    entry.Key,
                    totaldamage, mindamage, maxdamage,
                    casts, hits, crit, flank, glance,
                    timeswasted / 1000.0,
                    -timessaved / 1000.0};
                list.Add(skillData);
            }

            foreach (KeyValuePair<long, List<CastLog>> entry in castLogsBySkill)
            {
                if (damageLogsBySkill.ContainsKey(entry.Key)) continue;

                if (!_usedSkills.ContainsKey(entry.Key)) _usedSkills.Add(entry.Key, skillList.Get(entry.Key));

                int casts = entry.Value.Count;
                int timeswasted = 0, timessaved = 0;
                foreach (CastLog cl in entry.Value)
                {
                    if (cl.EndActivation == ParseEnum.Activation.CancelCancel) timeswasted += cl.ActualDuration;
                    if (cl.EndActivation == ParseEnum.Activation.CancelFire && cl.ActualDuration < cl.ExpectedDuration)
                    {
                        timessaved += cl.ExpectedDuration - cl.ActualDuration;
                    }
                }

                double[] skillData = { 0, entry.Key, 0, -1, 0, casts,
                    0, 0, 0, 0, timeswasted / 1000.0, -timessaved / 1000.0 };
                list.Add(skillData);
            }
            return list;
        }

        private DmgDistributionDto _BuildDMGDistData(Statistics.FinalDPS dps, AbstractMasterPlayer p, Boss target, int phaseIndex)
        {
            DmgDistributionDto dto = new DmgDistributionDto();
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<CastLog> casting = p.GetCastLogs(_log, phase.Start, phase.End);
            List<DamageLog> damageLogs = p.GetJustPlayerDamageLogs(target, _log, phase.Start, phase.End);
            dto.totalDamage = dps.Damage;
            dto.contributedDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            dto.distribution = BuildDMGDistBodyData(casting, damageLogs, dto.contributedDamage);

            return dto;
        }

        /// <summary>
        /// Creates the damage distribution table for a given player
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="toBoss"></param>
        /// <param name="phaseIndex"></param>
        private DmgDistributionDto BuildPlayerDMGDistData(Player p, Boss target, int phaseIndex)
        {
            Statistics.FinalDPS dps = target != null ? _statistics.DpsTarget[target][p][phaseIndex] : _statistics.DpsAll[p][phaseIndex];
            return _BuildDMGDistData(dps, p, target, phaseIndex);
        }

        /// <summary>
        /// Creates the damage distribution table for a the boss
        /// </summary>
        private DmgDistributionDto BuildTargetDMGDistData(Boss target, int phaseIndex)
        {
            Statistics.FinalDPS dps = _statistics.TargetDps[target][phaseIndex];
            return _BuildDMGDistData(dps, target, null, phaseIndex);
        }

        private DmgDistributionDto _BuildDMGDistData(Statistics.FinalDPS dps, AbstractMasterPlayer p, Minions minions, Boss target, int phaseIndex)
        {
            DmgDistributionDto dto = new DmgDistributionDto();
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<CastLog> casting = minions.GetCastLogs(_log, phase.Start, phase.End);
            List<DamageLog> damageLogs = minions.GetDamageLogs(target, _log, phase.Start, phase.End);
            dto.contributedDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            dto.totalDamage = dps.Damage;
            dto.distribution = BuildDMGDistBodyData(casting, damageLogs, dto.contributedDamage);
            return dto;
        }

        /// <summary>
        /// Creates the damage distribution table for a given minion
        /// </summary>
        private DmgDistributionDto BuildPlayerMinionDMGDistData(Player p, Minions minions, Boss target, int phaseIndex)
        {
            Statistics.FinalDPS dps = target != null ? _statistics.DpsTarget[target][p][phaseIndex] : _statistics.DpsAll[p][phaseIndex];

            return _BuildDMGDistData(dps, p, minions, target, phaseIndex);
        }

        /// <summary>
        /// Creates the damage distribution table for a given boss minion
        /// </summary>
        private DmgDistributionDto BuildTargetMinionDMGDistData(Boss target, Minions minions, int phaseIndex)
        {
            Statistics.FinalDPS dps = _statistics.TargetDps[target][phaseIndex];
            return _BuildDMGDistData(dps, target, minions, null, phaseIndex);
        }

        /// <summary>
        /// Create the damage taken distribution table for a given player
        /// </summary>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        private DmgDistributionDto BuildDMGTakenDistData(Player p, int phaseIndex)
        {
            DmgDistributionDto dto = new DmgDistributionDto
            {
                distribution = new List<double[]>()
            };
            PhaseData phase = _statistics.Phases[phaseIndex];
            List<DamageLog> damageLogs = p.GetDamageTakenLogs(_log, phase.Start, phase.End);
            Dictionary<long, List<DamageLog>> damageLogsBySkill = damageLogs.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            SkillData skillList = _log.SkillData;
            dto.contributedDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => (long)x.Damage) : 0;
            Dictionary<long, Boon> conditionsById = _statistics.PresentConditions.ToDictionary(x => x.ID);
            foreach (var entry in damageLogsBySkill)
            {
                int totaldamage = 0;
                int mindamage = 0;
                int hits = 0;
                int maxdamage = 0;
                int crit = 0;
                int flank = 0;
                int glance = 0;

                foreach (DamageLog dl in entry.Value)
                {
                    int curdmg = dl.Damage;
                    totaldamage += curdmg;
                    if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                    if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                    if (curdmg >= 0) { hits++; };
                    ParseEnum.Result result = dl.Result;
                    if (result == ParseEnum.Result.Crit) { crit++; } else if (result == ParseEnum.Result.Glance) { glance++; }
                    if (dl.IsFlanking == 1) { flank++; }
                }

                bool isCondi = conditionsById.ContainsKey(entry.Key) || entry.Key == 873;
                if (isCondi)
                {
                    Boon condi = entry.Key == 873 ? Boon.BoonsByIds[873] : conditionsById[entry.Key];
                    if (!_usedBoons.ContainsKey(condi.ID)) _usedBoons.Add(condi.ID, condi);
                }
                else
                {
                    if (!_usedSkills.ContainsKey(entry.Key)) _usedSkills.Add(entry.Key, skillList.Get(entry.Key));
                }
                double[] row = new double[12] {
                        isCondi ? 1 : 0, // isCondi
                        entry.Key,
                        totaldamage,
                        mindamage, maxdamage,
                        0, hits,
                        crit, flank, glance,
                        0, 0
                    };
                dto.distribution.Add(row);
            }

            return dto;
        }

        private List<BoonChartDataDto> BuildPlayerBoonGraphData(AbstractMasterPlayer p, int phaseIndex)
        {
            List<BoonChartDataDto> list = new List<BoonChartDataDto>();
            PhaseData phase = _statistics.Phases[phaseIndex];
            if (_statistics.PresentBoons.Count > 0)
            {
                Dictionary<long, BoonsGraphModel> boonGraphData = p.GetBoonGraphs(_log);
                foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse())
                {
                    BoonChartDataDto graph = BuildPlayerTabBoonGraph(bgm, phase);
                    if (graph != null) list.Add(graph);
                }
                if (p.GetType() == typeof(Player))
                {
                    foreach (Boss mainTarget in _log.FightData.GetMainTargets(_log))
                    {
                        boonGraphData = mainTarget.GetBoonGraphs(_log);
                        foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.BoonName == "Compromised" || x.BoonName == "Unnatural Signet" || x.BoonName == "Fractured - Enemy"))
                        {
                            BoonChartDataDto graph = BuildPlayerTabBoonGraph(bgm, phase);
                            if (graph != null) list.Add(graph);
                        }
                    }
                }               
            }
            return list;
        }

        private BoonChartDataDto BuildPlayerTabBoonGraph(BoonsGraphModel bgm, PhaseData phase)
        {
            //TODO line: {shape: 'hv'}
            long roundedEnd = phase.Start + 1000*phase.GetDuration("s");
            List<BoonsGraphModel.Segment> bChart = bgm.BoonChart.Where(x => x.End >= phase.Start && x.Start <= roundedEnd).ToList();
            if (bChart.Count == 0 || (bChart.Count == 1 && bChart.First().Value == 0))
            {
                return null;
            }
            BoonChartDataDto dto = new BoonChartDataDto
            {
                name = bgm.BoonName,
                visible = bgm.BoonName == "Might" || bgm.BoonName == "Quickness",
                color = HTMLHelper.GetLink("Color-" + bgm.BoonName),
                states = new List<double[]>(bChart.Count + 1)
            };

            foreach (BoonsGraphModel.Segment seg in bChart)
            {
                double segStart = Math.Round(Math.Max(seg.Start - phase.Start, 0) / 1000.0, 3);
                dto.states.Add(new double[] { segStart, seg.Value });
            }
            BoonsGraphModel.Segment lastSeg = bChart.Last();
            double segEnd = Math.Round(Math.Min(lastSeg.End - phase.Start, roundedEnd - phase.Start) / 1000.0, 3);
            dto.states.Add(new double[] { segEnd, lastSeg.Value });

            return dto;
        }

        private List<FoodDto> BuildPlayerFoodData(Player p)
        {
            List<FoodDto> list = new List<FoodDto>();
            List<Player.Consumable> consume = p.GetConsumablesList(_log, 0, _log.FightData.FightDuration);

            foreach(Player.Consumable entry in consume)
            {
                FoodDto dto = new FoodDto
                {
                    time = entry.Time / 1000.0,
                    duration = entry.Duration / 1000.0,
                    stack = entry.Stack,
                    id = entry.Item.ID,
                    dimished = entry.Item.ID == 46587 || entry.Item.ID == 46668
                };
                _usedBoons[entry.Item.ID] = entry.Item;
                list.Add(dto);
            }

            return list;
        }

        /// <summary>
        /// Creates the mechanics table of the fight
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="phaseIndex"></param>
        private List<List<int[]>> BuildPlayerMechanicData(int phaseIndex)
        {
            List<List<int[]>> list = new List<List<int[]>>();
            HashSet<Mechanic> presMech = _log.MechanicData.GetPresentPlayerMechs(0);
            PhaseData phase = _statistics.Phases[phaseIndex];

            foreach (Player p in _log.PlayerList)
            {
                List<int[]> playerData = new List<int[]>(presMech.Count);
                foreach (Mechanic mech in presMech)
                {
                    long timeFilter = 0;
                    int filterCount = 0;
                    List<MechanicLog> mls = _log.MechanicData[mech].Where(x => x.Player.InstID == p.InstID && phase.InInterval(x.Time)).ToList();
                    int count = mls.Count;
                    foreach (MechanicLog ml in mls)
                    {
                        if (mech.InternalCooldown != 0 && ml.Time - timeFilter < mech.InternalCooldown)//ICD check
                        {
                            filterCount++;
                        }
                        timeFilter = ml.Time;

                    }
                    int[] mechEntry = {count - filterCount,count};
                    playerData.Add(mechEntry);
                }
                list.Add(playerData);
            }
            return list;
        }

        private List<List<int[]>> BuildEnemyMechanicData(int phaseIndex)
        {
            List<List<int[]>> list = new List<List<int[]>>();
            HashSet<Mechanic> presMech = _log.MechanicData.GetPresentEnemyMechs(0);
            PhaseData phase = _statistics.Phases[phaseIndex];
            foreach (AbstractMasterPlayer p in _log.MechanicData.GetEnemyList(0))
            {
                List<int[]> enemyData = new List<int[]>(presMech.Count);
                foreach (Mechanic mech in presMech)
                {
                    long timeFilter = 0;
                    int filterCount = 0;
                    List<MechanicLog> mls = _log.MechanicData[mech].Where(x => x.Player.InstID == p.InstID && phase.InInterval(x.Time)).ToList();
                    int count = mls.Count;
                    foreach (MechanicLog ml in mls)
                    {
                        if (mech.InternalCooldown != 0 && ml.Time - timeFilter < mech.InternalCooldown)//ICD check
                        {
                            filterCount++;
                        }
                        timeFilter = ml.Time;

                    }
                    enemyData.Add(new int[] { count - filterCount, count });
                }
                list.Add(enemyData);
            }
            return list;
        }
        
        private List<MechanicDto> BuildMechanics()
        {
            List<MechanicDto> mechanicDtos = new List<MechanicDto>();
            HashSet<Mechanic> playerMechs = _log.MechanicData.GetPresentPlayerMechs(0);
            HashSet<Mechanic> enemyMechs = _log.MechanicData.GetPresentEnemyMechs(0);
            foreach (Mechanic mech in _log.MechanicData.GetPresentMechanics(0))
            {
                List<MechanicLog> mechanicLogs = _log.MechanicData[mech];
                MechanicDto dto = new MechanicDto
                {
                    name = mech.PlotlyName,
                    shortName = mech.ShortName,
                    description = mech.Description,
                    playerMech = playerMechs.Contains(mech),
                    enemyMech = enemyMechs.Contains(mech)
                };
                mechanicDtos.Add(dto);
            }
            return mechanicDtos;
        }

        private List<MechanicChartDataDto> BuildMechanicsChartData()
        {
            List<MechanicChartDataDto> mechanicsChart = new List<MechanicChartDataDto>();
            HashSet<Mechanic> playerMechs = _log.MechanicData.GetPresentPlayerMechs(0);
            HashSet<Mechanic> enemyMechs = _log.MechanicData.GetPresentEnemyMechs(0);
            foreach (Mechanic mech in _log.MechanicData.GetPresentMechanics(0))
            {
                List<MechanicLog> mechanicLogs = _log.MechanicData[mech];
                MechanicChartDataDto dto = new MechanicChartDataDto
                {
                    color = mech.PlotlyColor,
                    symbol = mech.PlotlySymbol,
                    size = mech.PlotlySize != null ? int.Parse(mech.PlotlySize) : 0,
                    visible = (mech.SkillId == SkillItem.DeathId || mech.SkillId == SkillItem.DownId),
                    points = BuildMechanicGraphPointData(mechanicLogs, mech.IsEnemyMechanic)
                };
                mechanicsChart.Add(dto);
            }
            return mechanicsChart;
        }

        private List<List<List<double>>> BuildMechanicGraphPointData(List<MechanicLog> mechanicLogs, bool enemyMechanic)
        {
            List<List<List<double>>> list = new List<List<List<double>>>();
            foreach (PhaseData phase in _statistics.Phases)
            {
                List<List<double>> phaseData = new List<List<double>>();
                list.Add(phaseData);
                if (!enemyMechanic)
                {
                    Dictionary<AbstractMasterPlayer, int> playerIndex = new Dictionary<AbstractMasterPlayer, int>();
                    for (var p = 0; p < _log.PlayerList.Count; p++)
                    {
                        playerIndex.Add(_log.PlayerList[p], p);
                        phaseData.Add(new List<double>());
                    }
                    foreach (MechanicLog ml in mechanicLogs.Where(x => phase.InInterval(x.Time)))
                    {
                        double time = (ml.Time - phase.Start) / 1000.0;
                        if (playerIndex.TryGetValue(ml.Player, out int p))
                        {
                            phaseData[p].Add(time);
                        }
                    }
                } else
                {
                    Dictionary<AbstractMasterPlayer, int> targetIndex = new Dictionary<AbstractMasterPlayer, int>();
                    for (var p = 0; p < phase.Targets.Count; p++)
                    {
                        targetIndex.Add(phase.Targets[p], p);
                        phaseData.Add(new List<double>());
                    }
                    phaseData.Add(new List<double>());
                    foreach (MechanicLog ml in mechanicLogs.Where(x => phase.InInterval(x.Time)))
                    {
                        double time = (ml.Time - phase.Start) / 1000.0;
                        if (targetIndex.TryGetValue(ml.Player, out int p))
                        {
                            phaseData[p].Add(time);
                        }
                        else
                        {
                            phaseData[phaseData.Count - 1].Add(time);
                        }
                    }
                }
            }
            return list;
        }

        private List<BoonData> BuildTargetCondiData(int phaseIndex, Boss target)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];
            Dictionary<long, Statistics.FinalTargetBoon> conditions = _statistics.TargetConditions[target][phaseIndex];
            List<BoonData> list = new List<BoonData>();

            foreach (Player player in _log.PlayerList)
            {
                BoonData playerData = new BoonData
                {
                    data = new List<List<object>>()
                };

                foreach (Boon boon in _statistics.PresentConditions)
                {
                    List<object> boonData = new List<object>();
                    Statistics.FinalTargetBoon toUse = conditions[boon.ID];
                    boonData.Add(toUse.Generated[player]);
                    boonData.Add(toUse.Overstacked[player]);
                    playerData.data.Add(boonData);
                }
                list.Add(playerData);
            }
            return list;
        }

        private BoonData BuildTargetCondiUptimeData(int phaseIndex, Boss target)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];
            Dictionary<long, Statistics.FinalTargetBoon> conditions = _statistics.TargetConditions[target][phaseIndex];
            long fightDuration = phase.GetDuration();
            BoonData tagetData = new BoonData
            {
                data = new List<List<object>>()
            };
            tagetData.avg = Math.Round(_statistics.AvgTargetConditions[target][phaseIndex], 1);
            foreach (Boon boon in _statistics.PresentConditions)
            {
                List<object> boonData = new List<object>
                {
                    conditions[boon.ID].Uptime
                };

                if (boon.Type != Boon.BoonType.Duration && conditions[boon.ID].Presence > 0)
                {
                    boonData.Add(conditions[boon.ID].Presence);
                }

                tagetData.data.Add(boonData);
            }
            return tagetData;
        }

        private BoonData BuildTargetBoonData(int phaseIndex, Boss target)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];
            Dictionary<long, Statistics.FinalTargetBoon> conditions = _statistics.TargetConditions[target][phaseIndex];
            long fightDuration = phase.GetDuration();
            BoonData targetData = new BoonData
            {
                data = new List<List<object>>()
            };
            targetData.avg = Math.Round(_statistics.AvgTargetBoons[target][phaseIndex], 1);
            foreach (Boon boon in _statistics.PresentBoons)
            {
                List<object> boonData = new List<object>
                {
                    conditions[boon.ID].Uptime
                };

                if (boon.Type != Boon.BoonType.Duration && conditions[boon.ID].Presence > 0)
                {
                    boonData.Add(conditions[boon.ID].Presence);
                }

                targetData.data.Add(boonData);
            }
            return targetData;
        }
        /// <summary>
        /// Creates the combat replay tab
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateReplayTab(StreamWriter sw)
        {
            CombatReplayMap map = _log.FightData.Logic.GetCombatMap();
            Tuple<int, int> canvasSize = map.GetPixelMapSize();
            HTMLHelper.WriteCombatReplayInterface(sw, canvasSize, _log);
            HTMLHelper.WriteCombatReplayScript(sw, _log, canvasSize, map, _settings.PollingRate);
        }

        private string ReplaceVariables(string html)
        {
            html = html.Replace("${bootstrapTheme}", !_settings.LightTheme ? "slate" : "cosmo");

            html = html.Replace("${encounterStart}", _log.LogData.LogStart);
            html = html.Replace("${encounterEnd}", _log.LogData.LogEnd);
            html = html.Replace("${evtcVersion}", _log.LogData.BuildVersion);
            html = html.Replace("${fightID}", _log.FightData.ID.ToString());
            html = html.Replace("${eiVersion}", Application.ProductVersion);
            html = html.Replace("${recordedBy}", _log.LogData.PoV.Split(':')[0]);

            return html;
        }

        /// <summary>
        /// Creates the whole html
        /// </summary>
        /// <param name="sw">Stream writer</param>
        public void CreateHTML(StreamWriter sw, string path)
        {
            string html = Properties.Resources.template_html;

            html = ReplaceVariables(html);

            html = html.Replace("<!--${Css}-->", BuildCss(path));
            html = html.Replace("<!--${Js}-->", BuildEIJs(path));

            html = html.Replace("<!--${Templates}-->", BuildTemplates());
            html = html.Replace("'${logDataJson}'", BuildLogData());

            html = html.Replace("<!--${Details}-->", BuildDetails());
            html = html.Replace("<!--${Maps}-->", BuildMaps());
#if DEBUG
            html = html.Replace("<!--${Vue}-->", "<script src=\"https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.js\"></script>");
#else
            html = html.Replace("<!--${Vue}-->", "<script src=\"https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.min.js\"></script>");
#endif

            html = html.Replace("'${graphDataJson}'", BuildGraphJson());

            html = html.Replace("<!--${combatReplay}-->", BuildCombatReplayContent());
            sw.Write(html);
            return;       
        }

        private string BuildCombatReplayContent()
        {
            if (!_settings.ParseCombatReplay || !_log.FightData.Logic.CanCombatReplay)
            {
                return "";
            }
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write("<div id=\"replay_template\">");
                    CreateReplayTab(sw);
                    sw.Write("</div>");
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private string BuildTemplates()
        {
            string templatesScript = "";
            Dictionary<string, string> templates = new Dictionary<string, string>()
            {
                {"tmplBuffStats",Properties.Resources.tmplBuffStats },
                {"tmplBuffStatsTarget",Properties.Resources.tmplBuffStatsTarget },
                {"tmplBuffTable",Properties.Resources.tmplBuffTable },
                {"tmplDamageDistPlayer",Properties.Resources.tmplDamageDistPlayer },
                {"tmplDamageDistTable",Properties.Resources.tmplDamageDistTable },
                {"tmplDamageDistTarget",Properties.Resources.tmplDamageDistTarget },
                {"tmplDamageModifierTable",Properties.Resources.tmplDamageModifierTable },
                {"tmplDamageTable",Properties.Resources.tmplDamageTable },
                {"tmplDamageTakenPlayer",Properties.Resources.tmplDamageTakenPlayer },
                {"tmplDeathRecap",Properties.Resources.tmplDeathRecap },
                {"tmplDefenseTable",Properties.Resources.tmplDefenseTable },
                {"tmplEncounter",Properties.Resources.tmplEncounter },
                {"tmplFood",Properties.Resources.tmplFood },
                {"tmplGameplayTable",Properties.Resources.tmplGameplayTable },
                {"tmplGeneralLayout",Properties.Resources.tmplGeneralLayout },
                {"tmplMechanicsTable",Properties.Resources.tmplMechanicsTable },
                {"tmplPersonalBuffTable",Properties.Resources.tmplPersonalBuffTable },
                {"tmplPhase",Properties.Resources.tmplPhase },
                {"tmplPlayers",Properties.Resources.tmplPlayers },
                {"tmplPlayerStats",Properties.Resources.tmplPlayerStats },
                {"tmplPlayerTab",Properties.Resources.tmplPlayerTab },
                {"tmplSimpleRotation",Properties.Resources.tmplSimpleRotation },
                {"tmplSupportTable",Properties.Resources.tmplSupportTable },
                {"tmplTargets",Properties.Resources.tmplTargets },
                {"tmplTargetStats",Properties.Resources.tmplTargetStats },
                {"tmplTargetTab",Properties.Resources.tmplTargetTab },
                {"tmplDPSGraph",Properties.Resources.tmplDPSGraph },
                {"tmplGraphStats",Properties.Resources.tmplGraphStats },
                {"tmplPlayerTabGraph",Properties.Resources.tmplPlayerTabGraph },
                {"tmplRotationLegend",Properties.Resources.tmplRotationLegend },
            };
            foreach (var entry in templates)
            {
                string template = "<script type=\"text/x-template\" id=\"" + entry.Key + "\">\r\n";
                template += entry.Value;
                template += "\r\n</script>\r\n";
                templatesScript += template;
            }
            return templatesScript;
        }

        private string BuildCss(string path)
        {
#if DEBUG
            string scriptContent = Properties.Resources.ei_css;
#else
            string scriptContent = Uglify.Css(Properties.Resources.ei_css).Code;
#endif
            string cssFilename = "ei-" + _scriptVersion + ".css";
            if (Properties.Settings.Default.NewHtmlExternalScripts)
            {
                string cssPath = Path.Combine(path, cssFilename);
                using (var fs = new FileStream(cssPath, FileMode.Create, FileAccess.Write))
                using (var scriptWriter = new StreamWriter(fs))
                {
                    scriptWriter.Write(scriptContent);
                }
                return "<link rel=\"stylesheet\" type=\"text/css\" href=\"./"+ cssFilename + "?version="+_scriptVersionRev+"\">";
            }
            else
            {
                return "<style type=\"text/css\">\r\n" + scriptContent + "\r\n</style>";
            }
        }

        private string BuildEIJs(string path)
        {
#if DEBUG
            string scriptContent = Properties.Resources.ei_js;
#else
            string scriptContent = Uglify.Js(Properties.Resources.ei_js).Code;
#endif
            string scriptFilename = "ei-" + _scriptVersion + ".js";
            if (Properties.Settings.Default.NewHtmlExternalScripts)
            {
                string scriptPath = Path.Combine(path, scriptFilename);
                using (var fs = new FileStream(scriptPath, FileMode.Create, FileAccess.Write))
                using (var scriptWriter = new StreamWriter(fs))
                {
                    scriptWriter.Write(scriptContent);
                }
                return "<script src=\"./" + scriptFilename + "?version=" + _scriptVersionRev + "\"></script>";
            }
            else
            {
                return "<script>\r\n" + scriptContent + "\r\n</script>";
            }
        }

        private void BuildWeaponSets(PlayerDto playerDto, Player player)
        {
            string[] weps = player.GetWeaponsArray(_log);
            List<string> firstSet = new List<string>();
            List<string> secondSet = new List<string>();
            for (int j = 0; j < weps.Length; j++)
            {
                var wep = weps[j];
                if (wep != null)
                {
                    if (wep != "2Hand")
                    {
                        if (j > 1)
                        {
                            secondSet.Add(wep);
                        }
                        else
                        {
                            firstSet.Add(wep);
                        }
                    }
                }
                else
                {
                    if (j > 1)
                    {
                        secondSet.Add("Unknown");
                    }
                    else
                    {
                        firstSet.Add("Unknown");
                    }
                }
            }
            if (firstSet[0] == "Unknown" && firstSet[1] == "Unknown")
            {
                playerDto.firstSet = new List<string>();
            }
            else
            {
                playerDto.firstSet = firstSet;
            }
            if (secondSet[0] == "Unknown" && secondSet[1] == "Unknown")
            {
                playerDto.secondSet = new List<string>();
            }
            else
            {
                playerDto.secondSet = secondSet;
            }
        }

        private string BuildGraphJson()
        {
            ChartDataDto chartData = new ChartDataDto();
            List<PhaseChartDataDto> phaseChartData = new List<PhaseChartDataDto>();
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseChartDataDto phaseData = new PhaseChartDataDto()
                {
                    players = BuildPlayerGraphData(i)
                };
                foreach(Boss target in _statistics.Phases[i].Targets)
                {
                    phaseData.targets.Add(BuildTargetGraphData(i, target));
                }

                phaseChartData.Add(phaseData);
             }
            chartData.phases = phaseChartData;
            chartData.mechanics = BuildMechanicsChartData();
            return ToJson(chartData, typeof(ChartDataDto));
        }

        private string BuildLogData()
        {
            LogDataDto logData = new LogDataDto();
            foreach(Player player in _log.PlayerList)
            {
                PlayerDto playerDto = new PlayerDto(
                    player.Group,
                    player.Character,
                    player.Account.TrimStart(':'),
                    player.Prof)
                {
                    condi = player.Condition,
                    conc = player.Concentration,
                    heal = player.Healing,
                    tough = player.Toughness,
                    colTarget = HTMLHelper.GetLink("Color-" + player.Prof),
                    colCleave = HTMLHelper.GetLink("Color-" + player.Prof + "-NonBoss"),
                    colTotal = HTMLHelper.GetLink("Color-" + player.Prof + "-Total"),
                    isConjure = player.Account == ":Conjured Sword",
                };
                BuildWeaponSets(playerDto, player);
                foreach (KeyValuePair<string, Minions> pair in player.GetMinions(_log))
                {
                    playerDto.minions.Add(new MinionDto(pair.Value.MinionID, pair.Key.TrimEnd(" \0".ToArray())));
                }

                logData.players.Add(playerDto);
            }

            foreach(AbstractMasterPlayer enemy in _log.MechanicData.GetEnemyList(0))
            {
                logData.enemies.Add(new EnemyDto(enemy.Character));
            }

            foreach (Boss target in _log.FightData.Logic.Targets)
            {
                TargetDto tar = new TargetDto(target.ID, target.Character, GeneralHelper.GetNPCIcon(target.ID))
                {
                    health = target.Health,
                    hbHeight = target.HitboxHeight,
                    hbWidth = target.HitboxWidth,
                    tough = target.Toughness
                };
                if (_log.LogData.Success)
                {
                    tar.percent = 100;
                    tar.hpLeft = 0;
                }
                else
                {
                    if (_log.Boss.HealthOverTime.Count > 0)
                    {
                        tar.percent = Math.Round(target.HealthOverTime[target.HealthOverTime.Count - 1].Y * 0.01, 2);
                        tar.hpLeft = (int)Math.Floor(100.0 - target.HealthOverTime[target.HealthOverTime.Count - 1].Y * 0.01);
                    }
                }
                foreach (KeyValuePair<string, Minions> pair in target.GetMinions(_log))
                {
                    tar.minions.Add(new MinionDto(pair.Value.MinionID, pair.Key.TrimEnd(" \0".ToArray())));
                }
                logData.targets.Add(tar);
            }

            Dictionary<string, List<long>> persBuffs = new Dictionary<string, List<long>>();
            Dictionary<string, List<Boon>> persBuffDict = BuildPersonalBoonData(persBuffs);
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                PhaseData phaseData = _statistics.Phases[i];
                PhaseDto phaseDto = new PhaseDto(phaseData.Name, phaseData.GetDuration())
                {
                    start = phaseData.Start / 1000.0,
                    end = phaseData.End / 1000.0,
                    dpsStats = BuildDPSData(i),
                    dpsStatsTargets = BuildDPSTargetsData(i),
                    dmgStatsTargets = BuildDMGStatsTargetsData(i),
                    dmgStats = BuildDMGStatsData(i),
                    defStats = BuildDefenseData(i),
                    healStats = BuildSupportData(i),
                    boonStats = BuildBuffUptimeData(_statistics.PresentBoons, i),
                    offBuffStats = BuildBuffUptimeData(_statistics.PresentOffbuffs, i),
                    defBuffStats = BuildBuffUptimeData(_statistics.PresentDefbuffs, i),
                    persBuffStats = BuildPersonalBuffUptimeData(persBuffDict, i),
                    boonGenSelfStats = BuildBuffGenerationData(_statistics.PresentBoons, i, "self"),
                    boonGenGroupStats = BuildBuffGenerationData(_statistics.PresentBoons, i, "group"),
                    boonGenOGroupStats = BuildBuffGenerationData(_statistics.PresentBoons, i, "off"),
                    boonGenSquadStats = BuildBuffGenerationData(_statistics.PresentBoons, i, "squad"),
                    offBuffGenSelfStats = BuildBuffGenerationData(_statistics.PresentOffbuffs, i, "self"),
                    offBuffGenGroupStats = BuildBuffGenerationData(_statistics.PresentOffbuffs, i, "group"),
                    offBuffGenOGroupStats = BuildBuffGenerationData(_statistics.PresentOffbuffs, i, "off"),
                    offBuffGenSquadStats = BuildBuffGenerationData(_statistics.PresentOffbuffs, i, "squad"),
                    defBuffGenSelfStats = BuildBuffGenerationData(_statistics.PresentDefbuffs, i, "self"),
                    defBuffGenGroupStats = BuildBuffGenerationData(_statistics.PresentDefbuffs, i, "group"),
                    defBuffGenOGroupStats = BuildBuffGenerationData(_statistics.PresentDefbuffs, i, "off"),
                    defBuffGenSquadStats = BuildBuffGenerationData(_statistics.PresentDefbuffs, i, "squad"),
                    targetsCondiStats = new List<List<BoonData>>(),
                    targetsCondiTotals = new List<BoonData>(),
                    targetsBoonTotals = new List<BoonData>(),
                    mechanicStats = BuildPlayerMechanicData(i),
                    enemyMechanicStats = BuildEnemyMechanicData(i)
                };
                foreach (Boss target in phaseData.Targets)
                {
                    phaseDto.targets.Add(_log.FightData.Logic.Targets.IndexOf(target));
                }
                BuildDmgModifiersData(i, phaseDto.dmgModifiersCommon, phaseDto.dmgModifiersTargetsCommon);
                foreach (Boss target in phaseData.Targets)
                {
                    phaseDto.targetsCondiStats.Add(BuildTargetCondiData(i, target));
                    phaseDto.targetsCondiTotals.Add(BuildTargetCondiUptimeData(i, target));
                    phaseDto.targetsBoonTotals.Add(HasBoons(i, target) ? BuildTargetBoonData(i, target) : null);
                }
                // add phase markup to full fight graph
                phaseDto.markupLines = new List<double>();
                phaseDto.markupAreas = new List<AreaLabelDto>();
                for (int j = 1; j < _statistics.Phases.Count; j++)
                {
                    PhaseData curPhase = _statistics.Phases[j];
                    if (curPhase.Start < phaseData.Start || curPhase.End > phaseData.End || 
                        (curPhase.Start == phaseData.Start && curPhase.End == phaseData.End ))
                    {
                        continue;
                    }
                    if (phaseDto.subPhases == null)
                    {
                        phaseDto.subPhases = new List<int>();
                    }
                    phaseDto.subPhases.Add(j); 
                    long start = curPhase.Start - phaseData.Start;
                    long end = curPhase.End - phaseData.Start;
                    if (curPhase.DrawStart) phaseDto.markupLines.Add(start / 1000.0);
                    if (curPhase.DrawEnd) phaseDto.markupLines.Add(end / 1000.0);
                    AreaLabelDto phaseArea = new AreaLabelDto
                    {
                        start = start / 1000.0,
                        end = end / 1000.0,
                        label = curPhase.Name,
                        highlight = curPhase.DrawArea
                    };
                    phaseDto.markupAreas.Add(phaseArea);
                }
                if (phaseDto.markupAreas.Count == 0) phaseDto.markupAreas = null;
                if (phaseDto.markupLines.Count == 0) phaseDto.markupLines = null;
                logData.phases.Add(phaseDto);
            }

            logData.boons = new List<long>();
            foreach (Boon boon in _statistics.PresentBoons)
            {
                logData.boons.Add(boon.ID);
                _usedBoons[boon.ID] = boon;
            }
            logData.conditions = new List<long>();
            foreach (Boon boon in _statistics.PresentConditions)
            {
                logData.conditions.Add(boon.ID);
                _usedBoons[boon.ID] = boon;
            }
            logData.offBuffs = new List<long>();
            foreach (Boon boon in _statistics.PresentOffbuffs)
            {
                logData.offBuffs.Add(boon.ID);
                _usedBoons[boon.ID] = boon;
            }
            logData.defBuffs = new List<long>();
            foreach (Boon boon in _statistics.PresentDefbuffs)
            {
                logData.defBuffs.Add(boon.ID);
                _usedBoons[boon.ID] = boon;
            }
            logData.persBuffs = persBuffs;
            //
            double fightDuration = _log.FightData.FightDuration / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fightDuration);
            string durationString = duration.Minutes + "m " + duration.Seconds + "s " + duration.Milliseconds + "ms";
            if (duration.Hours > 0)
            {
                durationString = duration.Hours + "h " + durationString;
            }
            logData.encounterDuration = durationString;
            logData.success = _log.LogData.Success;
            logData.fightName = FilterStringChars(_log.FightData.Name);
            logData.fightIcon = _log.FightData.Logic.IconUrl;

            return ToJson(logData, typeof(LogDataDto));
        }

        private bool HasBoons(int phaseIndex, Boss target)
        {
            Dictionary<long, Statistics.FinalTargetBoon> conditions = _statistics.TargetConditions[target][phaseIndex];
            foreach (Boon boon in _statistics.PresentBoons)
            {
                if (conditions[boon.ID].Uptime > 0.0)
                {
                    return true;
                }
            }
            return false;
        }

        private string BuildDetails()
        {
            string scripts = "";
            for (var i = 0; i < _log.PlayerList.Count; i++) {
                Player player = _log.PlayerList[i];
                string playerScript = "logData.players[" + i + "].details = " + ToJson(BuildPlayerData(player), typeof(PlayerDetailsDto)) + ";\r\n";
                scripts += playerScript;
            }
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                Boss target = _log.FightData.Logic.Targets[i];
                string targetScript = "logData.targets[" + i + "].details = " + ToJson(BuildTargetData(target), typeof(PlayerDetailsDto)) + ";\r\n";
                scripts += targetScript;
            }
            return "<script>\r\n"+scripts + "\r\n</script>";
        }

        private string BuildMaps()
        {
            string skillsScript = "var usedSkills = " + ToJson(AssembleSkills(_usedSkills.Values), typeof(ICollection<SkillDto>)) + ";" +
                "logData.skillMap = {};" +
                "$.each(usedSkills, function(i, skill) {" +
                    "logData.skillMap['s'+skill.id]=skill;" +
                "});";
            string boonsScript = "var usedBoons = " + ToJson(AssembleBoons(_usedBoons.Values), typeof(ICollection<BoonDto>)) + ";" +
                "logData.buffMap = {};" +
                "$.each(usedBoons, function(i, boon) {" +
                    "logData.buffMap['b'+boon.id]=boon;" +
                "});";
            string mechanicsScript = "logData.mechanics = " + ToJson(BuildMechanics(), typeof(List<MechanicDto>)) + ";";
            return "<script>\r\n" + skillsScript + "\r\n" + boonsScript + "\r\n" + mechanicsScript + "\r\n</script>";
        }

        private PlayerDetailsDto BuildPlayerData(Player player)
        {
            PlayerDetailsDto dto = new PlayerDetailsDto
            {
                dmgDistributions = new List<DmgDistributionDto>(),
                dmgDistributionsTargets = new List<List<DmgDistributionDto>>(),
                dmgDistributionsTaken = new List<DmgDistributionDto>(),
                boonGraph = new List<List<BoonChartDataDto>>(),
                rotation = new List<List<double[]>>(),
                food = BuildPlayerFoodData(player),
                minions = new List<PlayerDetailsDto>(),
                deathRecap = BuildDeathRecap(player)
            };
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                dto.rotation.Add(BuildSimpleRotationTabData(player, i));
                dto.dmgDistributions.Add(BuildPlayerDMGDistData(player, null, i));
                List<DmgDistributionDto> dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (Boss target in _statistics.Phases[i].Targets)
                {
                    dmgTargetsDto.Add(BuildPlayerDMGDistData(player, target, i));
                }
                dto.dmgDistributionsTargets.Add(dmgTargetsDto);
                dto.dmgDistributionsTaken.Add(BuildDMGTakenDistData(player, i));
                dto.boonGraph.Add(BuildPlayerBoonGraphData(player, i));
            }
            foreach (KeyValuePair<string, Minions> pair in player.GetMinions(_log))
            {
                dto.minions.Add(BuildPlayerMinionsData(player, pair.Value));
            }

            return dto;
        }

        private PlayerDetailsDto BuildPlayerMinionsData(Player player, Minions minion)
        {
            PlayerDetailsDto dto = new PlayerDetailsDto
            {
                dmgDistributions = new List<DmgDistributionDto>(),
                dmgDistributionsTargets = new List<List<DmgDistributionDto>>()
            };
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                List<DmgDistributionDto> dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (Boss target in _statistics.Phases[i].Targets)
                {
                    dmgTargetsDto.Add(BuildPlayerMinionDMGDistData(player, minion, target, i));
                }
                dto.dmgDistributionsTargets.Add(dmgTargetsDto);
                dto.dmgDistributions.Add(BuildPlayerMinionDMGDistData(player, minion, null, i));
            }
            return dto;
        }

        private PlayerDetailsDto BuildTargetData(Boss target)
        {
            PlayerDetailsDto dto = new PlayerDetailsDto
            {
                dmgDistributions = new List<DmgDistributionDto>(),
                boonGraph = new List<List<BoonChartDataDto>>(),
                rotation = new List<List<double[]>>()
            };
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                if (_statistics.Phases[i].Targets.Contains(target))
                {
                    dto.dmgDistributions.Add(BuildTargetDMGDistData(target, i));
                    dto.rotation.Add(BuildSimpleRotationTabData(target, i));
                    dto.boonGraph.Add(BuildPlayerBoonGraphData(target, i));
                } else
                {
                    dto.dmgDistributions.Add(new DmgDistributionDto());
                    dto.rotation.Add(new List<double[]>());
                    dto.boonGraph.Add(new List<BoonChartDataDto>());
                }
            }

            dto.minions = new List<PlayerDetailsDto>();
            foreach (KeyValuePair<string, Minions> pair in target.GetMinions(_log))
            {
                dto.minions.Add(BuildTargetsMinionsData(target, pair.Value));
            }
            return dto;
        }

        private PlayerDetailsDto BuildTargetsMinionsData(Boss target, Minions minion)
        {
            PlayerDetailsDto dto = new PlayerDetailsDto
            {
                dmgDistributions = new List<DmgDistributionDto>()
            };
            for (int i = 0; i < _statistics.Phases.Count; i++)
            {
                if (_statistics.Phases[i].Targets.Contains(target))
                {
                    dto.dmgDistributions.Add(BuildTargetMinionDMGDistData(target, minion, i));
                }
                else
                {
                    dto.dmgDistributions.Add(new DmgDistributionDto());
                }
            }
            return dto;
        }

        private List<BoonDto> AssembleBoons(ICollection<Boon> boons)
        {
            List<BoonDto> dtos = new List<BoonDto>();
            foreach (Boon boon in boons)
            {
                dtos.Add(new BoonDto(
                                boon.ID,
                                boon.Name,
                                boon.Link,
                                boon.Type == Boon.BoonType.Intensity
                                )
                        );
            }
            return dtos;
        }

        private List<SkillDto> AssembleSkills(ICollection<SkillItem> skills)
        {
            List<SkillDto> dtos = new List<SkillDto>();
            foreach (SkillItem skill in skills)
            {
                GW2APISkill apiSkill = skill.ApiSkill;
                SkillDto dto = new SkillDto(skill.ID, skill.Name, skill.Icon, apiSkill?.slot == "Weapon_1");
                dtos.Add(dto);
            }
            return dtos;
        }
 
        private string ToJson(object value, Type type)
        {
            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
            DataContractJsonSerializer ser = new DataContractJsonSerializer(type, settings);
            MemoryStream memoryStream = new MemoryStream();
            ser.WriteObject(memoryStream, value);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }
}
