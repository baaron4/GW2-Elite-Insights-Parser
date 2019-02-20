using LuckParser.Controllers;
using LuckParser.Parser;
using LuckParser.Models.HtmlModels;
using LuckParser.Models.ParseModels;
using Newtonsoft.Json;
using NUglify;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LuckParser.Setting;
using LuckParser.Models;
using Newtonsoft.Json.Serialization;

namespace LuckParser.Builders
{
    class HTMLBuilder
    {
        private readonly string _scriptVersion;
        private readonly int _scriptVersionRev;

        private readonly ParsedLog _log;
        private readonly List<PhaseData> _phases;

        private readonly string[] _uploadLink;

        private readonly Statistics _statistics;
        private Dictionary<long, Boon> _usedBoons = new Dictionary<long, Boon>();
        private Dictionary<long, SkillItem> _usedSkills = new Dictionary<long, SkillItem>();

        public HTMLBuilder(ParsedLog log, string[] uploadString)
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            _scriptVersion = version.Major + "." + version.Minor;
#if PROD
            _scriptVersion += "." + version.Build;
#endif
            _scriptVersionRev = version.Revision;
            _log = log;
            _phases = log.FightData.GetPhases(log);

            _statistics = log.Statistics;

            _uploadLink = uploadString;
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

        private TargetChartDataDto BuildTargetGraphData(int phaseIndex, Target target)
        {
            PhaseData phase = _phases[phaseIndex];
            return new TargetChartDataDto
            {
                Total = target.Get1SDamageList(_log, phaseIndex, phase, null),
                Health = target.Get1SHealthGraph(_log, _phases)[phaseIndex]
            };
        }

        private List<PlayerChartDataDto> BuildPlayerGraphData(int phaseIndex)
        {
            List<PlayerChartDataDto> list = new List<PlayerChartDataDto>();
            PhaseData phase = _phases[phaseIndex];

            foreach (Player p in _log.PlayerList)
            {
                PlayerChartDataDto pChar = new PlayerChartDataDto()
                {
                    Total = p.Get1SDamageList(_log, phaseIndex, phase, null),
                    Targets = new List<List<int>>()
                };
                foreach (Target target in phase.Targets)
                {
                    pChar.Targets.Add(p.Get1SDamageList(_log, phaseIndex, phase, target));
                }
                list.Add(pChar);
            }
            return list;
        }

        private List<List<object>> BuildDPSData(int phaseIndex)
        {
            List<List<object>> list = new List<List<object>>(_log.PlayerList.Count);
            PhaseData phase = _phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalDPS dpsAll = player.GetDPSAll(_log, phaseIndex);
                list.Add(PhaseDto.GetDPSStatData(dpsAll));
            }
            return list;
        }

        private List<List<List<object>>> BuildDPSTargetsData(int phaseIndex)
        {
            List<List<List<object>>> list = new List<List<List<object>>>(_log.PlayerList.Count);
            PhaseData phase = _phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                List<List<object>> playerData = new List<List<object>>();

                foreach (Target target in phase.Targets)
                {
                    List<object> tar = new List<object>();
                    playerData.Add(PhaseDto.GetDPSStatData(player.GetDPSTarget(_log, phaseIndex, target)));
                }
                list.Add(playerData);
            }
            return list;
        }

        private List<List<object>> BuildDMGStatsData(int phaseIndex)
        {
            List<List<object>> list = new List<List<object>>();
            PhaseData phase = _phases[phaseIndex];
            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalStatsAll stats = player.GetStatsAll(_log, phaseIndex);             
                list.Add(PhaseDto.GetDMGStatData(stats));
            }
            return list;
        }

        private List<List<List<object>>> BuildDMGStatsTargetsData(int phaseIndex)
        {
            List<List<List<object>>> list = new List<List<List<object>>>();

            PhaseData phase = _phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                List<List<object>> playerData = new List<List<object>>();
                foreach (Target target in phase.Targets)
                {
                    Statistics.FinalStats statsTarget = player.GetStatsTarget(_log, phaseIndex, target);
                    playerData.Add(PhaseDto.GetDMGTargetStatData(statsTarget));
                }
                list.Add(playerData);
            }
            return list;
        }

        private List<List<object>> BuildDefenseData(int phaseIndex)
        {
            List<List<object>> list = new List<List<object>>();

            PhaseData phase = _phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalDefenses defenses = player.GetDefenses(_log, phaseIndex);
                list.Add(PhaseDto.GetDefenseStatData(defenses, phase));
            }

            return list;
        }

        private List<List<object>> BuildSupportData(int phaseIndex)
        {
            List<List<object>> list = new List<List<object>>();

            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalSupport support = player.GetSupport(_log, phaseIndex);
                list.Add(PhaseDto.GetSupportStatData(support));
            }
            return list;
        }

        private List<BoonData> BuildBuffUptimeData(List<Boon> listToUse, int phaseIndex)
        {
            List<PhaseData> phases = _phases;
            List<BoonData> list = new List<BoonData>();
            bool boonTable = listToUse.Select(x => x.ID).Contains(740);

            foreach (Player player in _log.PlayerList)
            {
                double avg = 0.0;
                if (boonTable)
                {
                    avg = Math.Round(player.GetStatsAll(_log, phaseIndex).AvgBoons, 1);
                }
                list.Add(new BoonData(player.GetBuffs(_log, phaseIndex, Statistics.BuffEnum.Self), listToUse, avg));
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
                    for (int i = 0; i < _phases.Count; i++)
                    {
                        Dictionary<long, Statistics.FinalBuffs> boons = player.GetBuffs(_log, i, Statistics.BuffEnum.Self);
                        foreach (Boon boon in _statistics.PresentPersonalBuffs[player.InstID])
                        {
                            if (boons.TryGetValue(boon.ID, out var uptime))
                            {
                                if (uptime.Uptime > 0 && specBoonIds.Contains(boon.ID))
                                {
                                    boonToUse.Add(boon);
                                }
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
            long fightDuration = _phases[phaseIndex].GetDuration();
            foreach (Player player in _log.PlayerList)
            {
                list.Add(new BoonData(player.Prof, boonsBySpec, player.GetBuffs(_log, phaseIndex, Statistics.BuffEnum.Self)));
            }
            return list;
        }

        private void BuildDmgModifiersData(int phaseIndex, List<long> boonToUse, List<List<object[]>> data, List<List<List<object[]>>> dataTargets)
        {
            PhaseData phase = _phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                List<List<object[]>> pDataTargets = new List<List<object[]>>();
                data.Add(BoonData.GetDamageModifierData(boonToUse, player.GetExtraBoonData(_log, null), phaseIndex));
                dataTargets.Add(pDataTargets);
                foreach (Target target in phase.Targets)
                {
                    List<object[]> pTarget = new List<object[]>();
                    pDataTargets.Add(BoonData.GetDamageModifierData(boonToUse, player.GetExtraBoonData(_log, target), phaseIndex));
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
        private List<BoonData> BuildBuffGenerationData(List<Boon> listToUse, int phaseIndex, Statistics.BuffEnum target)
        {
            List<BoonData> list = new List<BoonData>();

            foreach (Player player in _log.PlayerList)
            {
                Dictionary<long, Statistics.FinalBuffs> uptimes;
                uptimes = player.GetBuffs(_log, phaseIndex, target);
                list.Add(new BoonData(listToUse, uptimes));
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
        private List<object[]> BuildRotationData(AbstractActor p, int phaseIndex)
        {
            List<object[]> list = new List<object[]>();

            PhaseData phase = _phases[phaseIndex];
            List<CastLog> casting = p.GetCastLogsActDur(_log, phase.Start, phase.End);
            SkillData skillList = _log.SkillData;
            foreach (CastLog cl in casting)
            {
                if (!_usedSkills.ContainsKey(cl.SkillId)) _usedSkills.Add(cl.SkillId, skillList.Get(cl.SkillId));
                list.Add(ActorDetailsDto.GetSkillData(cl, phase.Start));
            }
            return list;
        }

        /// <summary>
        /// Creates the death recap tab for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private List<DeathRecapDto> BuildDeathRecap(Player p)
        {
            List<DeathRecapDto> res = new List<DeathRecapDto>();
            List<Player.DeathRecap> recaps = p.GetDeathRecaps(_log);
            if (recaps == null)
            {
                return null;
            }
            foreach (Player.DeathRecap deathRecap in recaps)
            {
                DeathRecapDto recap = new DeathRecapDto()
                {
                    Time = deathRecap.DeathTime
                };
                res.Add(recap);
                if (deathRecap.ToKill != null)
                {
                    recap.ToKill = DeathRecapDto.BuildDeathRecapItemList(deathRecap.ToKill);
                }
                if (deathRecap.ToDown != null)
                {
                    recap.ToDown = DeathRecapDto.BuildDeathRecapItemList(deathRecap.ToDown);
                }
                
            }
            return res;
        }

        private List<object[]> BuildDMGDistBodyData(List<CastLog> casting, List<DamageLog> damageLogs, long finalTotalDamage)
        {
            List<object[]> list = new List<object[]>();
            Dictionary<long, List<CastLog>> castLogsBySkill = casting.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            Dictionary<long, List<DamageLog>> damageLogsBySkill = damageLogs.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            Dictionary<long, Boon> conditionsById = _statistics.PresentConditions.ToDictionary(x => x.ID);
            SkillData skillList = _log.SkillData;
            foreach (KeyValuePair<long, List<DamageLog>> entry in damageLogsBySkill)
            {   
                list.Add(DmgDistributionDto.GetDMGDtoItem(entry, castLogsBySkill, skillList, _usedSkills, _usedBoons));
            }
            // non damaging
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

                object[] skillData = { 0, entry.Key, 0, -1, 0, casts,
                    0, 0, 0, 0, timeswasted / 1000.0, -timessaved / 1000.0 };
                list.Add(skillData);
            }
            return list;
        }

        private DmgDistributionDto _BuildDMGDistData(Statistics.FinalDPS dps, AbstractMasterActor p, Target target, int phaseIndex)
        {
            DmgDistributionDto dto = new DmgDistributionDto();
            PhaseData phase = _phases[phaseIndex];
            List<CastLog> casting = p.GetCastLogs(_log, phase.Start, phase.End);
            List<DamageLog> damageLogs = p.GetJustPlayerDamageLogs(target, _log, phase.Start, phase.End);
            dto.TotalDamage = dps.Damage;
            dto.ContributedDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            dto.Distribution = BuildDMGDistBodyData(casting, damageLogs, dto.ContributedDamage);

            return dto;
        }

        /// <summary>
        /// Creates the damage distribution table for a given player
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        private DmgDistributionDto BuildPlayerDMGDistData(Player p, Target target, int phaseIndex)
        {
            Statistics.FinalDPS dps = p.GetDPSTarget(_log, phaseIndex, target);
            return _BuildDMGDistData(dps, p, target, phaseIndex);
        }

        /// <summary>
        /// Creates the damage distribution table for a target
        /// </summary>
        private DmgDistributionDto BuildTargetDMGDistData(Target target, int phaseIndex)
        {
            Statistics.FinalDPS dps = target.GetDPSAll(_log, phaseIndex);
            return _BuildDMGDistData(dps, target, null, phaseIndex);
        }

        private DmgDistributionDto _BuildDMGDistData(Statistics.FinalDPS dps, AbstractMasterActor p, Minions minions, Target target, int phaseIndex)
        {
            DmgDistributionDto dto = new DmgDistributionDto();
            PhaseData phase = _phases[phaseIndex];
            List<CastLog> casting = minions.GetCastLogs(_log, phase.Start, phase.End);
            List<DamageLog> damageLogs = minions.GetDamageLogs(target, _log, phase.Start, phase.End);
            dto.ContributedDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            dto.TotalDamage = dps.Damage;
            dto.Distribution = BuildDMGDistBodyData(casting, damageLogs, dto.ContributedDamage);
            return dto;
        }

        /// <summary>
        /// Creates the damage distribution table for a given minion
        /// </summary>
        private DmgDistributionDto BuildPlayerMinionDMGDistData(Player p, Minions minions, Target target, int phaseIndex)
        {
            Statistics.FinalDPS dps = p.GetDPSTarget(_log, phaseIndex, target);

            return _BuildDMGDistData(dps, p, minions, target, phaseIndex);
        }

        /// <summary>
        /// Creates the damage distribution table for a given boss minion
        /// </summary>
        private DmgDistributionDto BuildTargetMinionDMGDistData(Target target, Minions minions, int phaseIndex)
        {
            Statistics.FinalDPS dps = target.GetDPSAll(_log, phaseIndex);
            return _BuildDMGDistData(dps, target, minions, null, phaseIndex);
        }

        /// <summary>
        /// Create the damage taken distribution table for a given player
        /// </summary>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        private DmgDistributionDto BuildDMGTakenDistData(AbstractMasterActor p, int phaseIndex)
        {
            DmgDistributionDto dto = new DmgDistributionDto
            {
                Distribution = new List<object[]>()
            };
            PhaseData phase = _phases[phaseIndex];
            List<DamageLog> damageLogs = p.GetDamageTakenLogs(null, _log, phase.Start, phase.End);
            Dictionary<long, List<DamageLog>> damageLogsBySkill = damageLogs.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            SkillData skillList = _log.SkillData;
            dto.ContributedDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => (long)x.Damage) : 0;
            Dictionary<long, Boon> conditionsById = _statistics.PresentConditions.ToDictionary(x => x.ID);
            foreach (var entry in damageLogsBySkill)
            {
                dto.Distribution.Add(DmgDistributionDto.GetDMGDtoItem(entry, null, skillList, _usedSkills, _usedBoons));
            }
            return dto;
        }

        private void BuildBoonGraphData(List<BoonChartDataDto> list, List<Boon> listToUse, Dictionary<long, BoonsGraphModel> boonGraphData, PhaseData phase)
        {
            foreach (Boon buff in listToUse)
            {
                if (boonGraphData.TryGetValue(buff.ID, out BoonsGraphModel bgm))
                {
                    BoonChartDataDto graph = BuildBoonGraph(bgm, phase);
                    if (graph != null) list.Add(graph);
                }
                boonGraphData.Remove(buff.ID);
            }
        }

        private List<BoonChartDataDto> BuildBoonGraphData(AbstractMasterActor p, int phaseIndex)
        {
            List<BoonChartDataDto> list = new List<BoonChartDataDto>();
            PhaseData phase = _phases[phaseIndex];
            Dictionary<long, BoonsGraphModel> boonGraphData = p.GetBoonGraphs(_log).ToDictionary(x => x.Key, x => x.Value);
            BuildBoonGraphData(list, _statistics.PresentBoons, boonGraphData, phase);
            BuildBoonGraphData(list, _statistics.PresentConditions, boonGraphData, phase);
            BuildBoonGraphData(list, _statistics.PresentOffbuffs, boonGraphData, phase);
            BuildBoonGraphData(list, _statistics.PresentDefbuffs, boonGraphData, phase);
            foreach (BoonsGraphModel bgm in boonGraphData.Values)
            {
                BoonChartDataDto graph = BuildBoonGraph(bgm, phase);
                if (graph != null) list.Add(graph);
            }
            if (p.GetType() == typeof(Player))
            {
                foreach (Target mainTarget in _log.FightData.GetMainTargets(_log))
                {
                    boonGraphData = mainTarget.GetBoonGraphs(_log);
                    foreach (BoonsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.Boon.Name == "Compromised" || x.Boon.Name == "Unnatural Signet" || x.Boon.Name == "Fractured - Enemy"))
                    {
                        BoonChartDataDto graph = BuildBoonGraph(bgm, phase);
                        if (graph != null) list.Add(graph);
                    }
                }
            }
            list.Reverse();
            return list;
        }

        private BoonChartDataDto BuildBoonGraph(BoonsGraphModel bgm, PhaseData phase)
        {
            List<BoonsGraphModel.Segment> bChart = bgm.BoonChart.Where(x => x.End >= phase.Start && x.Start <= phase.End
            ).ToList();
            if (bChart.Count == 0 || (bChart.Count == 1 && bChart.First().Value == 0))
            {
                return null;
            }
            _usedBoons[bgm.Boon.ID] = bgm.Boon;
            return new BoonChartDataDto(bgm, bChart, phase);
        }

        private List<FoodDto> BuildPlayerFoodData(Player p)
        {
            List<FoodDto> list = new List<FoodDto>();
            List<Player.Consumable> consume = p.GetConsumablesList(_log, 0, _log.FightData.FightDuration);

            foreach(Player.Consumable entry in consume)
            {
                _usedBoons[entry.Buff.ID] = entry.Buff;
                list.Add(new FoodDto(entry));
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
            PhaseData phase = _phases[phaseIndex];

            foreach (Player p in _log.PlayerList)
            {
                list.Add(MechanicDto.GetMechanicData(presMech, _log, p, phase));
            }
            return list;
        }

        private List<List<int[]>> BuildEnemyMechanicData(int phaseIndex)
        {
            List<List<int[]>> list = new List<List<int[]>>();
            HashSet<Mechanic> presMech = _log.MechanicData.GetPresentEnemyMechs(0);
            PhaseData phase = _phases[phaseIndex];
            foreach (DummyActor enemy in _log.MechanicData.GetEnemyList(0))
            {
                list.Add(MechanicDto.GetMechanicData(presMech, _log, enemy, phase));
            }
            return list;
        }
        
        private List<MechanicDto> BuildMechanics()
        {
            List<MechanicDto> mechanicDtos = new List<MechanicDto>();
            foreach (Mechanic mech in _log.MechanicData.GetPresentMechanics(0))
            {
                MechanicDto dto = new MechanicDto
                {
                    Name = mech.FullName,
                    ShortName = mech.ShortName,
                    Description = mech.Description,
                    PlayerMech = mech.ShowOnTable && !mech.IsEnemyMechanic,
                    EnemyMech = mech.IsEnemyMechanic
                };
                mechanicDtos.Add(dto);
            }
            return mechanicDtos;
        }

        private List<MechanicChartDataDto> BuildMechanicsChartData()
        {
            List<MechanicChartDataDto> mechanicsChart = new List<MechanicChartDataDto>();
            foreach (Mechanic mech in _log.MechanicData.GetPresentMechanics(0))
            {
                List<MechanicLog> mechanicLogs = _log.MechanicData[mech];
                MechanicChartDataDto dto = new MechanicChartDataDto
                {
                    Color = mech.PlotlySetting.Color,
                    Symbol = mech.PlotlySetting.Symbol,
                    Size = mech.PlotlySetting.Size,
                    Visible = (mech.SkillId == SkillItem.DeathId || mech.SkillId == SkillItem.DownId),
                    Points = BuildMechanicGraphPointData(mechanicLogs, mech.IsEnemyMechanic)
                };
                mechanicsChart.Add(dto);
            }
            return mechanicsChart;
        }

        private List<List<List<object>>> BuildMechanicGraphPointData(List<MechanicLog> mechanicLogs, bool enemyMechanic)
        {
            List<List<List<object>>> list = new List<List<List<object>>>();
            foreach (PhaseData phase in _phases)
            {
                list.Add(MechanicChartDataDto.GetMechanicChartPoints(mechanicLogs, phase, _log, enemyMechanic));
            }
            return list;
        }

        private List<BoonData> BuildTargetCondiData(int phaseIndex, Target target)
        {
            PhaseData phase = _phases[phaseIndex];
            Dictionary<long, Statistics.FinalTargetBuffs> conditions = target.GetBuffs(_log, phaseIndex);
            List<BoonData> list = new List<BoonData>();

            foreach (Player player in _log.PlayerList)
            {
                list.Add(new BoonData(conditions, _statistics.PresentConditions, player));
            }
            return list;
        }

        private BoonData BuildTargetCondiUptimeData(int phaseIndex, Target target)
        {
            PhaseData phase = _phases[phaseIndex];
            Dictionary<long, Statistics.FinalTargetBuffs> buffs = target.GetBuffs(_log, phaseIndex);
            long fightDuration = phase.GetDuration();
            return new BoonData(buffs, _statistics.PresentConditions, Math.Round(target.GetAverageConditions(_log, phaseIndex), 1));
        }

        private BoonData BuildTargetBoonData(int phaseIndex, Target target)
        {
            PhaseData phase = _phases[phaseIndex];
            Dictionary<long, Statistics.FinalTargetBuffs> buffs = target.GetBuffs(_log, phaseIndex);
            long fightDuration = phase.GetDuration();
            return new BoonData(buffs, _statistics.PresentBoons, Math.Round(target.GetAverageBoons(_log, phaseIndex), 1));
        }

        private string ReplaceVariables(string html)
        {
            html = html.Replace("${bootstrapTheme}", !Properties.Settings.Default.LightTheme ? "slate" : "yeti");

            html = html.Replace("${encounterStart}", _log.LogData.LogStart);
            html = html.Replace("${encounterEnd}", _log.LogData.LogEnd);
            html = html.Replace("${evtcVersion}", _log.LogData.BuildVersion);
            html = html.Replace("${fightID}", _log.FightData.ID.ToString());
            html = html.Replace("${eiVersion}", Application.ProductVersion);
            html = html.Replace("${recordedBy}", _log.LogData.PoV.Split(':')[0]);

            string uploadString = "";
            if (Properties.Settings.Default.UploadToDPSReports)
            {
                uploadString += "<p>DPS Reports Link (EI): <a href=\"" + _uploadLink[0] + "\">" + _uploadLink[0] + "</a></p>";
            }
            if (Properties.Settings.Default.UploadToDPSReportsRH)
            {
                uploadString += "<p>DPS Reports Link (RH): <a href=\"" + _uploadLink[1] + "\">" + _uploadLink[1] + "</a></p>";
            }
            if (Properties.Settings.Default.UploadToRaidar)
            {
                uploadString += "<p>Raidar Link: <a href=\"" + _uploadLink[2] + "\">" + _uploadLink[2] + "</a></p>";
            }
            html = html.Replace("<!--${UploadLinks}-->", uploadString);

            return html;
        }
        
        public void CreateHTML(StreamWriter sw, string path)
        {
            string html = Properties.Resources.template_html;

            html = ReplaceVariables(html);

            html = html.Replace("<!--${Css}-->", BuildCss(path));
            html = html.Replace("<!--${Js}-->", BuildEIJs(path));
            html = html.Replace("<!--${JsCRLink}-->", BuildCRLinkJs(path));

            html = html.Replace("'${logDataJson}'", BuildLogData());

            html = html.Replace("<!--${Details}-->", BuildDetails());
            html = html.Replace("<!--${Maps}-->", BuildMaps());
#if DEBUG
            html = html.Replace("<!--${Vue}-->", "<script src=\"https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.js\"></script>");
#else
            html = html.Replace("<!--${Vue}-->", "<script src=\"https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.min.js\"></script>");
#endif

            html = html.Replace("'${graphDataJson}'", BuildGraphJson());

            html = html.Replace("<!--${CombatReplayScript}-->", BuildCombatReplayScript(path));
            html = html.Replace("<!--${CombatReplayBody}-->", BuildCombatReplayContent());
            sw.Write(html);
            return;       
        }

        private string BuildCombatReplayScript(string path)
        {
            if (!Properties.Settings.Default.ParseCombatReplay || !_log.FightData.Logic.CanCombatReplay)
            {
                return "";
            }
            CombatReplayMap map = _log.FightData.Logic.GetCombatMap();
            if (Properties.Settings.Default.HtmlExternalScripts)
            {
#if DEBUG
                string jsFileName = "EliteInsights-CR-" + _scriptVersion + ".js";
#else
                string jsFileName = "EliteInsights-CR-" + _scriptVersion + ".min.js";
#endif
                string jsPath = Path.Combine(path, jsFileName);
                try
                {
                    using (var fs = new FileStream(jsPath, FileMode.Create, FileAccess.Write))
                    using (var scriptWriter = new StreamWriter(fs, Encoding.UTF8))
                    {
#if DEBUG
                        scriptWriter.Write(Properties.Resources.combatreplay_js);
#else
                        scriptWriter.Write(Uglify.Js(Properties.Resources.combatreplay_js, GeneralHelper.JSMinifySettings).Code);
#endif
                    }
                } catch (IOException)
                {
                }
                string content = "<script src=\"./" + jsFileName + "?version=" + _scriptVersionRev + "\"></script>\n";
                content += "<script>"+ CombatReplayHelper.GetDynamicCombatReplayScript(_log, GeneralHelper.PollingRate, map)+ "</script>";
                return content;
            }
            else
            {
                return CombatReplayHelper.CreateCombatReplayScript(_log, map, GeneralHelper.PollingRate);
            }
        }

        private string BuildCombatReplayContent()
        {
            if (!Properties.Settings.Default.ParseCombatReplay || !_log.FightData.Logic.CanCombatReplay)
            {
                return "";
            }
            CombatReplayMap map = _log.FightData.Logic.GetCombatMap();
            (int width, int height) canvasSize = map.GetPixelMapSize();
            return CombatReplayHelper.CreateCombatReplayInterface(canvasSize, _log);
        }

        private string BuildTemplates(string script)
        {
            string tmplScript = script; 
            Dictionary<string, string> templates = new Dictionary<string, string>()
            {
                {"${tmplBuffStats}",Properties.Resources.tmplBuffStats },
                {"${tmplBuffStatsTarget}",Properties.Resources.tmplBuffStatsTarget },
                {"${tmplBuffTable}",Properties.Resources.tmplBuffTable },
                {"${tmplDamageDistPlayer}",Properties.Resources.tmplDamageDistPlayer },
                {"${tmplDamageDistTable}",Properties.Resources.tmplDamageDistTable },
                {"${tmplDamageDistTarget}",Properties.Resources.tmplDamageDistTarget },
                {"${tmplDamageModifierTable}",Properties.Resources.tmplDamageModifierTable },
                {"${tmplDamageTable}",Properties.Resources.tmplDamageTable },
                {"${tmplDamageTaken}",Properties.Resources.tmplDamageTaken },
                {"${tmplDeathRecap}",Properties.Resources.tmplDeathRecap },
                {"${tmplDefenseTable}",Properties.Resources.tmplDefenseTable },
                {"${tmplEncounter}",Properties.Resources.tmplEncounter },
                {"${tmplFood}",Properties.Resources.tmplFood },
                {"${tmplGameplayTable}",Properties.Resources.tmplGameplayTable },
                {"${tmplGeneralLayout}",Properties.Resources.tmplGeneralLayout },
                {"${tmplMechanicsTable}",Properties.Resources.tmplMechanicsTable },
                {"${tmplPersonalBuffTable}",Properties.Resources.tmplPersonalBuffTable },
                {"${tmplPhase}",Properties.Resources.tmplPhase },
                {"${tmplPlayers}",Properties.Resources.tmplPlayers },
                {"${tmplPlayerStats}",Properties.Resources.tmplPlayerStats },
                {"${tmplPlayerTab}",Properties.Resources.tmplPlayerTab },
                {"${tmplSimpleRotation}",Properties.Resources.tmplSimpleRotation },
                {"${tmplSupportTable}",Properties.Resources.tmplSupportTable },
                {"${tmplTargets}",Properties.Resources.tmplTargets },
                {"${tmplTargetStats}",Properties.Resources.tmplTargetStats },
                {"${tmplTargetTab}",Properties.Resources.tmplTargetTab },
                {"${tmplDPSGraph}",Properties.Resources.tmplDPSGraph },
                {"${tmplGraphStats}",Properties.Resources.tmplGraphStats },
                {"${tmplPlayerTabGraph}",Properties.Resources.tmplPlayerTabGraph },
                {"${tmplRotationLegend}",Properties.Resources.tmplRotationLegend },
                {"${tmplTargetTabGraph}",Properties.Resources.tmplTargetTabGraph },
                {"${tmplTargetData}",Properties.Resources.tmplTargetData },
            };
            foreach (var entry in templates)
            {
#if DEBUG
                tmplScript = tmplScript.Replace(entry.Key, entry.Value);
#else
                tmplScript = tmplScript.Replace(entry.Key, Regex.Replace(entry.Value, @"\t|\n|\r", ""));
#endif
            }
            return tmplScript;
        }

        private string BuildCRTemplates(string script)
        {
            string tmplScript = script;
            Dictionary<string, string> CRtemplates = new Dictionary<string, string>()
                {
                    {"${tmplCombatReplayDamageData}", Properties.Resources.tmplCombatReplayDamageData },
                    {"${tmplCombatReplayStatusData}", Properties.Resources.tmplCombatReplayStatusData },
                    {"${tmplCombatReplayDamageTable}", Properties.Resources.tmplCombatReplayDamageTable },
                    {"${tmplCombatReplayPlayerBuffStats}", Properties.Resources.tmplCombatReplayPlayerBuffStats },
                    {"${tmplCombatReplayPlayerStats}", Properties.Resources.tmplCombatReplayPlayerStats },
                    {"${tmplCombatReplayPlayerStatus}", Properties.Resources.tmplCombatReplayPlayerStatus },
                    {"${tmplCombatReplayPlayerRotation}", Properties.Resources.tmplCombatReplayPlayerRotation },
                };
            foreach (var entry in CRtemplates)
            {
                tmplScript = tmplScript.Replace(entry.Key, Regex.Replace(entry.Value, @"\t|\n|\r", ""));
            }
            return tmplScript;
        }

        private string BuildCss(string path)
        {
#if DEBUG
            string scriptContent = Properties.Resources.ei_css;
#else
            string scriptContent = Uglify.Css(Properties.Resources.ei_css).Code;
#endif
            if (Properties.Settings.Default.HtmlExternalScripts)
            {
#if DEBUG
                string cssFilename = "EliteInsights-" + _scriptVersion + ".css";
#else
                string cssFilename = "EliteInsights-" + _scriptVersion + ".min.css";
#endif
                string cssPath = Path.Combine(path, cssFilename);
                try
                {
                    using (var fs = new FileStream(cssPath, FileMode.Create, FileAccess.Write))
                    using (var scriptWriter = new StreamWriter(fs, Encoding.UTF8))
                    {
                        scriptWriter.Write(scriptContent);
                    }
                }
                catch (IOException)
                {
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
            List<string> orderedScripts = new List<string>()
            {
                Properties.Resources.globalJS,
                Properties.Resources.commonsJS,
                Properties.Resources.headerJS,
                Properties.Resources.layoutJS,
                Properties.Resources.generalStatsJS,
                Properties.Resources.buffStatsJS,
                Properties.Resources.graphsJS,
                Properties.Resources.mechanicsJS,
                Properties.Resources.targetStatsJS,
                Properties.Resources.playerStatsJS,
                Properties.Resources.ei_js
            };
            string scriptContent = orderedScripts[0];
            for (int i = 1; i < orderedScripts.Count; i++)
            {
                scriptContent += orderedScripts[i];
            }
            scriptContent = BuildTemplates(scriptContent);
#if !DEBUG
            scriptContent = Uglify.Js(scriptContent, GeneralHelper.JSMinifySettings).Code;
#endif
            if (Properties.Settings.Default.HtmlExternalScripts)
            {
#if DEBUG
                string scriptFilename = "EliteInsights-" + _scriptVersion + ".js";
#else
                string scriptFilename = "EliteInsights-" + _scriptVersion +".min.js";
#endif
                string scriptPath = Path.Combine(path, scriptFilename);
                try
                {
                    using (var fs = new FileStream(scriptPath, FileMode.Create, FileAccess.Write))
                    using (var scriptWriter = new StreamWriter(fs, Encoding.UTF8))
                    {
                        scriptWriter.Write(scriptContent);
                    }
                }
                catch (IOException)
                {
                }
                return "<script src=\"./" + scriptFilename + "?version=" + _scriptVersionRev + "\"></script>";
            }
            else
            {
                return "<script>\r\n" + scriptContent + "\r\n</script>";
            }
        }

        private string BuildCRLinkJs(string path)
        {
            if (!(Properties.Settings.Default.ParseCombatReplay && _log.FightData.Logic.CanCombatReplay))
            {
                return "";
            }
            string scriptContent = Properties.Resources.combatReplayStatsJS;
            scriptContent = BuildCRTemplates(scriptContent);
#if !DEBUG
            scriptContent = Uglify.Js(scriptContent).Code;
#endif
            if (Properties.Settings.Default.HtmlExternalScripts)
            {
#if DEBUG
                string scriptFilename = "EliteInsights-CRLink-" + _scriptVersion + ".js";
#else
                string scriptFilename = "EliteInsights-CRLink-" + _scriptVersion +".min.js";
#endif
                string scriptPath = Path.Combine(path, scriptFilename);
                try
                {
                    using (var fs = new FileStream(scriptPath, FileMode.Create, FileAccess.Write))
                    using (var scriptWriter = new StreamWriter(fs, Encoding.UTF8))
                    {
                        scriptWriter.Write(scriptContent);
                    }
                }
                catch (IOException)
                {
                }
                return "<script src=\"./" + scriptFilename + "?version=" + _scriptVersionRev + "\"></script>";
            }
            else
            {
                return "<script>\r\n" + scriptContent + "\r\n</script>";
            }
        }

        private string BuildGraphJson()
        {
            ChartDataDto chartData = new ChartDataDto();
            List<PhaseChartDataDto> phaseChartData = new List<PhaseChartDataDto>();
            for (int i = 0; i < _phases.Count; i++)
            {
                PhaseChartDataDto phaseData = new PhaseChartDataDto()
                {
                    Players = BuildPlayerGraphData(i)
                };
                foreach(Target target in _phases[i].Targets)
                {
                    phaseData.Targets.Add(BuildTargetGraphData(i, target));
                }

                phaseChartData.Add(phaseData);
             }
            chartData.Phases = phaseChartData;
            chartData.Mechanics = BuildMechanicsChartData();
            return ToJson(chartData);
        }

        private string BuildLogData()
        {
            LogDataDto logData = new LogDataDto();
            foreach(Player player in _log.PlayerList)
            {
                logData.Players.Add(new PlayerDto(player, _log, Properties.Settings.Default.ParseCombatReplay && _log.FightData.Logic.CanCombatReplay));
            }

            foreach(DummyActor enemy in _log.MechanicData.GetEnemyList(0))
            {
                logData.Enemies.Add(new EnemyDto() { Name = enemy.Character });
            }

            foreach (Target target in _log.FightData.Logic.Targets)
            {
                TargetDto targetDto = new TargetDto(target, _log, Properties.Settings.Default.ParseCombatReplay && _log.FightData.Logic.CanCombatReplay);
                
                
                logData.Targets.Add(targetDto);
            }
            //
            logData.PersBuffs = new Dictionary<string, List<long>>();
            Dictionary<string, List<Boon>> persBuffDict = BuildPersonalBoonData(logData.PersBuffs);
            logData.Boons = new List<long>();
            foreach (Boon boon in _statistics.PresentBoons)
            {
                logData.Boons.Add(boon.ID);
                _usedBoons[boon.ID] = boon;
            }
            logData.Conditions = new List<long>();
            foreach (Boon boon in _statistics.PresentConditions)
            {
                logData.Conditions.Add(boon.ID);
                _usedBoons[boon.ID] = boon;
            }
            logData.OffBuffs = new List<long>();
            foreach (Boon boon in _statistics.PresentOffbuffs)
            {
                logData.OffBuffs.Add(boon.ID);
                _usedBoons[boon.ID] = boon;
            }
            logData.DefBuffs = new List<long>();
            foreach (Boon boon in _statistics.PresentDefbuffs)
            {
                logData.DefBuffs.Add(boon.ID);
                _usedBoons[boon.ID] = boon;
            }
            HashSet<long> dmgCommonModifiersBuffs = new HashSet<long>();
            foreach (Player p in _log.PlayerList)
            {
                Dictionary<long, List<AbstractMasterActor.ExtraBoonData>> toCheck = p.GetExtraBoonData(_log, null);
                dmgCommonModifiersBuffs.UnionWith(toCheck.Keys);
            }
            logData.DmgCommonModifiersBuffs = dmgCommonModifiersBuffs.ToList();
            //
            for (int i = 0; i < _phases.Count; i++)
            {
                PhaseData phaseData = _phases[i];
                PhaseDto phaseDto = new PhaseDto(phaseData, _phases, _log)
                {
                    DpsStats = BuildDPSData(i),
                    DpsStatsTargets = BuildDPSTargetsData(i),
                    DmgStatsTargets = BuildDMGStatsTargetsData(i),
                    DmgStats = BuildDMGStatsData(i),
                    DefStats = BuildDefenseData(i),
                    HealStats = BuildSupportData(i),
                    BoonStats = BuildBuffUptimeData(_statistics.PresentBoons, i),
                    OffBuffStats = BuildBuffUptimeData(_statistics.PresentOffbuffs, i),
                    DefBuffStats = BuildBuffUptimeData(_statistics.PresentDefbuffs, i),
                    PersBuffStats = BuildPersonalBuffUptimeData(persBuffDict, i),
                    BoonGenSelfStats = BuildBuffGenerationData(_statistics.PresentBoons, i, Statistics.BuffEnum.Self),
                    BoonGenGroupStats = BuildBuffGenerationData(_statistics.PresentBoons, i, Statistics.BuffEnum.Group),
                    BoonGenOGroupStats = BuildBuffGenerationData(_statistics.PresentBoons, i, Statistics.BuffEnum.OffGroup),
                    BoonGenSquadStats = BuildBuffGenerationData(_statistics.PresentBoons, i, Statistics.BuffEnum.Squad),
                    OffBuffGenSelfStats = BuildBuffGenerationData(_statistics.PresentOffbuffs, i, Statistics.BuffEnum.Self),
                    OffBuffGenGroupStats = BuildBuffGenerationData(_statistics.PresentOffbuffs, i, Statistics.BuffEnum.Group),
                    OffBuffGenOGroupStats = BuildBuffGenerationData(_statistics.PresentOffbuffs, i, Statistics.BuffEnum.OffGroup),
                    OffBuffGenSquadStats = BuildBuffGenerationData(_statistics.PresentOffbuffs, i, Statistics.BuffEnum.Squad),
                    DefBuffGenSelfStats = BuildBuffGenerationData(_statistics.PresentDefbuffs, i, Statistics.BuffEnum.Self),
                    DefBuffGenGroupStats = BuildBuffGenerationData(_statistics.PresentDefbuffs, i, Statistics.BuffEnum.Group),
                    DefBuffGenOGroupStats = BuildBuffGenerationData(_statistics.PresentDefbuffs, i, Statistics.BuffEnum.OffGroup),
                    DefBuffGenSquadStats = BuildBuffGenerationData(_statistics.PresentDefbuffs, i, Statistics.BuffEnum.Squad),
                    TargetsCondiStats = new List<List<BoonData>>(),
                    TargetsCondiTotals = new List<BoonData>(),
                    TargetsBoonTotals = new List<BoonData>(),
                    MechanicStats = BuildPlayerMechanicData(i),
                    EnemyMechanicStats = BuildEnemyMechanicData(i)
                };
                BuildDmgModifiersData(i, logData.DmgCommonModifiersBuffs, phaseDto.DmgModifiersCommon, phaseDto.DmgModifiersTargetsCommon);
                foreach (Target target in phaseData.Targets)
                {
                    phaseDto.TargetsCondiStats.Add(BuildTargetCondiData(i, target));
                    phaseDto.TargetsCondiTotals.Add(BuildTargetCondiUptimeData(i, target));
                    phaseDto.TargetsBoonTotals.Add(HasBoons(i, target) ? BuildTargetBoonData(i, target) : null);
                }
                logData.Phases.Add(phaseDto);
            }
            //
            double fightDuration = _log.FightData.FightDuration / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fightDuration);
            string durationString = duration.Minutes + "m " + duration.Seconds + "s " + duration.Milliseconds + "ms";
            if (duration.Hours > 0)
            {
                durationString = duration.Hours + "h " + durationString;
            }
            logData.EncounterDuration = durationString;
            logData.Success = _log.FightData.Success;
            logData.FightName = FilterStringChars(_log.FightData.Name);
            logData.FightIcon = _log.FightData.Logic.IconUrl;
            logData.LightTheme = Properties.Settings.Default.LightTheme;
            logData.SingleGroup = _log.PlayerList.Where(x => x.Account != ":Conjured Sword").Select(x => x.Group).Distinct().Count() == 1;
            logData.NoMechanics = _log.FightData.Logic.MechanicList.Count == 3;
            return ToJson(logData);
        }

        private bool HasBoons(int phaseIndex, Target target)
        {
            Dictionary<long, Statistics.FinalTargetBuffs> conditions = target.GetBuffs(_log, phaseIndex);
            foreach (Boon boon in _statistics.PresentBoons)
            {
                if (conditions.TryGetValue(boon.ID, out var uptime))
                {
                    if (uptime.Uptime > 0.0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private string BuildDetails()
        {
            string scripts = "";
            for (var i = 0; i < _log.PlayerList.Count; i++) {
                Player player = _log.PlayerList[i];
                string playerScript = "logData.players[" + i + "].details = " + ToJson(BuildPlayerData(player)) + ";\r\n";
                scripts += playerScript;
            }
            for (int i = 0; i < _log.FightData.Logic.Targets.Count; i++)
            {
                Target target = _log.FightData.Logic.Targets[i];
                string targetScript = "logData.targets[" + i + "].details = " + ToJson(BuildTargetData(target)) + ";\r\n";
                scripts += targetScript;
            }
            return "<script>\r\n"+scripts + "\r\n</script>";
        }

        private string BuildMaps()
        {
            string skillsScript = "var usedSkills = " + ToJson(SkillDto.AssembleSkills(_usedSkills.Values)) + ";" +
                "var skillMap = {};" +
                "$.each(usedSkills, function(i, skill) {" +
                    "skillMap['s'+skill.id]=skill;" +
                "});";
            string boonsScript = "var usedBoons = " + ToJson(BoonDto.AssembleBoons(_usedBoons.Values)) + ";" +
                "var buffMap = {};" +
                "$.each(usedBoons, function(i, boon) {" +
                    "buffMap['b'+boon.id]=boon;" +
                "});";
            string mechanicsScript = "var mechanicMap = " + ToJson(BuildMechanics()) + ";";
            return "<script>\r\n" + skillsScript + "\r\n" + boonsScript + "\r\n" + mechanicsScript + "\r\n</script>";
        }

        private ActorDetailsDto BuildPlayerData(Player player)
        {
            ActorDetailsDto dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTargets = new List<List<DmgDistributionDto>>(),
                DmgDistributionsTaken = new List<DmgDistributionDto>(),
                BoonGraph = new List<List<BoonChartDataDto>>(),
                Rotation = new List<List<object[]>>(),
                Food = BuildPlayerFoodData(player),
                Minions = new List<ActorDetailsDto>(),
                DeathRecap = BuildDeathRecap(player)
            };
            for (int i = 0; i < _phases.Count; i++)
            {
                dto.Rotation.Add(BuildRotationData(player, i));
                dto.DmgDistributions.Add(BuildPlayerDMGDistData(player, null, i));
                List<DmgDistributionDto> dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (Target target in _phases[i].Targets)
                {
                    dmgTargetsDto.Add(BuildPlayerDMGDistData(player, target, i));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributionsTaken.Add(BuildDMGTakenDistData(player, i));
                dto.BoonGraph.Add(BuildBoonGraphData(player, i));
            }
            foreach (KeyValuePair<string, Minions> pair in player.GetMinions(_log))
            {
                dto.Minions.Add(BuildPlayerMinionsData(player, pair.Value));
            }

            return dto;
        }

        private ActorDetailsDto BuildPlayerMinionsData(Player player, Minions minion)
        {
            ActorDetailsDto dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTargets = new List<List<DmgDistributionDto>>()
            };
            for (int i = 0; i < _phases.Count; i++)
            {
                List<DmgDistributionDto> dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (Target target in _phases[i].Targets)
                {
                    dmgTargetsDto.Add(BuildPlayerMinionDMGDistData(player, minion, target, i));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributions.Add(BuildPlayerMinionDMGDistData(player, minion, null, i));
            }
            return dto;
        }

        private ActorDetailsDto BuildTargetData(Target target)
        {
            ActorDetailsDto dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTaken = new List<DmgDistributionDto>(),
                BoonGraph = new List<List<BoonChartDataDto>>(),
                Rotation = new List<List<object[]>>()
            };
            for (int i = 0; i < _phases.Count; i++)
            {
                if (_phases[i].Targets.Contains(target))
                {
                    dto.DmgDistributions.Add(BuildTargetDMGDistData(target, i));
                    dto.DmgDistributionsTaken.Add(BuildDMGTakenDistData(target, i));
                    dto.Rotation.Add(BuildRotationData(target, i));
                    dto.BoonGraph.Add(BuildBoonGraphData(target, i));
                } else
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                    dto.DmgDistributionsTaken.Add(new DmgDistributionDto());
                    dto.Rotation.Add(new List<object[]>());
                    dto.BoonGraph.Add(new List<BoonChartDataDto>());
                }
            }

            dto.Minions = new List<ActorDetailsDto>();
            foreach (KeyValuePair<string, Minions> pair in target.GetMinions(_log))
            {
                dto.Minions.Add(BuildTargetsMinionsData(target, pair.Value));
            }
            return dto;
        }

        private ActorDetailsDto BuildTargetsMinionsData(Target target, Minions minion)
        {
            ActorDetailsDto dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>()
            };
            for (int i = 0; i < _phases.Count; i++)
            {
                if (_phases[i].Targets.Contains(target))
                {
                    dto.DmgDistributions.Add(BuildTargetMinionDMGDistData(target, minion, i));
                }
                else
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                }
            }
            return dto;
        }
 
        private string ToJson(object value)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = contractResolver
            };
            return JsonConvert.SerializeObject(value, settings);
        }
    }
}
