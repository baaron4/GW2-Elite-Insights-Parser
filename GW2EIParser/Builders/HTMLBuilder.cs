using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GW2EIParser.Builders.HtmlModels;
using GW2EIParser.EIData;
using GW2EIParser.Logic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GW2EIParser.Builders
{
    public class HTMLBuilder
    {
        private readonly string _scriptVersion;
        private readonly int _scriptVersionRev;

        private readonly ParsedLog _log;
        private readonly List<PhaseData> _phases;
        private readonly bool _cr;

        private readonly string[] _uploadLink;

        private readonly GeneralStatistics _statistics;
        private readonly Dictionary<long, Buff> _usedBoons = new Dictionary<long, Buff>();
        private readonly HashSet<DamageModifier> _usedDamageMods = new HashSet<DamageModifier>();
        private readonly Dictionary<long, SkillItem> _usedSkills = new Dictionary<long, SkillItem>();

        public HTMLBuilder(ParsedLog log, string[] uploadString)
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            _scriptVersion = version.Major + "." + version.Minor;
#if !DEBUG
            _scriptVersion += "." + version.Build;
#endif
            _scriptVersionRev = version.Revision;
            _log = log;
            _phases = log.FightData.GetPhases(log);

            _statistics = log.Statistics;

            _uploadLink = uploadString;

            _cr = Properties.Settings.Default.ParseCombatReplay && _log.CanCombatReplay;
        }

        private TargetChartDataDto BuildTargetGraphData(int phaseIndex, NPC target)
        {
            PhaseData phase = _phases[phaseIndex];
            return new TargetChartDataDto
            {
                Total = target.Get1SDamageList(_log, phaseIndex, phase, null),
                Health = target.Get1SHealthGraph(_log)[phaseIndex]
            };
        }

        private List<PlayerChartDataDto> BuildPlayerGraphData(int phaseIndex)
        {
            var list = new List<PlayerChartDataDto>();
            PhaseData phase = _phases[phaseIndex];

            foreach (Player p in _log.PlayerList)
            {
                var pChar = new PlayerChartDataDto()
                {
                    Total = p.Get1SDamageList(_log, phaseIndex, phase, null),
                    Targets = new List<List<int>>()
                };
                foreach (NPC target in phase.Targets)
                {
                    pChar.Targets.Add(p.Get1SDamageList(_log, phaseIndex, phase, target));
                }
                list.Add(pChar);
            }
            return list;
        }

        private List<List<object>> BuildDPSData(int phaseIndex)
        {
            var list = new List<List<object>>(_log.PlayerList.Count);
            foreach (Player player in _log.PlayerList)
            {
                FinalDPS dpsAll = player.GetDPSAll(_log, phaseIndex);
                list.Add(PhaseDto.GetDPSStatData(dpsAll));
            }
            return list;
        }

        private List<List<List<object>>> BuildDPSTargetsData(int phaseIndex)
        {
            var list = new List<List<List<object>>>(_log.PlayerList.Count);
            PhaseData phase = _phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                var playerData = new List<List<object>>();

                foreach (NPC target in phase.Targets)
                {
                    var tar = new List<object>();
                    playerData.Add(PhaseDto.GetDPSStatData(player.GetDPSTarget(_log, phaseIndex, target)));
                }
                list.Add(playerData);
            }
            return list;
        }

        private List<List<object>> BuildDMGStatsData(int phaseIndex)
        {
            var list = new List<List<object>>();
            foreach (Player player in _log.PlayerList)
            {
                FinalGameplayStatsAll stats = player.GetGameplayStats(_log, phaseIndex);
                list.Add(PhaseDto.GetDMGStatData(stats));
            }
            return list;
        }

        private List<List<List<object>>> BuildDMGStatsTargetsData(int phaseIndex)
        {
            var list = new List<List<List<object>>>();

            PhaseData phase = _phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                var playerData = new List<List<object>>();
                foreach (NPC target in phase.Targets)
                {
                    FinalGameplayStats statsTarget = player.GetGameplayStats(_log, phaseIndex, target);
                    playerData.Add(PhaseDto.GetDMGTargetStatData(statsTarget));
                }
                list.Add(playerData);
            }
            return list;
        }

        private List<List<object>> BuildDefenseData(int phaseIndex)
        {
            var list = new List<List<object>>();

            PhaseData phase = _phases[phaseIndex];

            foreach (Player player in _log.PlayerList)
            {
                FinalDefensesAll defenses = player.GetDefenses(_log, phaseIndex);
                list.Add(PhaseDto.GetDefenseStatData(defenses, phase));
            }

            return list;
        }

        private List<List<object>> BuildSupportData(int phaseIndex)
        {
            var list = new List<List<object>>();

            foreach (Player player in _log.PlayerList)
            {
                FinalPlayerSupport support = player.GetPlayerSupport(_log, phaseIndex);
                list.Add(PhaseDto.GetSupportStatData(support));
            }
            return list;
        }

        private List<BuffData> BuildBuffUptimeData(List<Buff> listToUse, int phaseIndex)
        {
            List<PhaseData> phases = _phases;
            var list = new List<BuffData>();
            bool boonTable = listToUse.Select(x => x.Nature).Contains(Buff.BuffNature.Boon);

            foreach (Player player in _log.PlayerList)
            {
                double avg = 0.0;
                if (boonTable)
                {
                    avg = player.GetGameplayStats(_log, phaseIndex).AvgBoons;
                }
                list.Add(new BuffData(player.GetBuffs(_log, phaseIndex, BuffEnum.Self), listToUse, avg));
            }
            return list;
        }

        private List<BuffData> BuildActiveBuffUptimeData(List<Buff> listToUse, int phaseIndex)
        {
            var list = new List<BuffData>();
            bool boonTable = listToUse.Select(x => x.Nature).Contains(Buff.BuffNature.Boon);

            foreach (Player player in _log.PlayerList)
            {
                double avg = 0.0;
                if (boonTable)
                {
                    avg = player.GetGameplayStats(_log, phaseIndex).AvgActiveBoons;
                }
                list.Add(new BuffData(player.GetActiveBuffs(_log, phaseIndex, BuffEnum.Self), listToUse, avg));
            }
            return list;
        }

        private Dictionary<string, List<Buff>> BuildPersonalBoonData(Dictionary<string, List<long>> dict)
        {
            var boonsBySpec = new Dictionary<string, List<Buff>>();
            // Collect all personal buffs by spec
            foreach (KeyValuePair<string, List<Player>> pair in _log.PlayerListBySpec)
            {
                List<Player> players = pair.Value;
                var specBoonIds = new HashSet<long>(_log.Buffs.GetRemainingBuffsList(pair.Key).Select(x => x.ID));
                var boonToUse = new HashSet<Buff>();
                foreach (Player player in players)
                {
                    for (int i = 0; i < _phases.Count; i++)
                    {
                        Dictionary<long, FinalPlayerBuffs> boons = player.GetBuffs(_log, i, BuffEnum.Self);
                        foreach (Buff boon in _statistics.PresentPersonalBuffs[player.InstID])
                        {
                            if (boons.TryGetValue(boon.ID, out FinalPlayerBuffs uptime))
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
            foreach (KeyValuePair<string, List<Buff>> pair in boonsBySpec)
            {
                dict[pair.Key] = new List<long>();
                foreach (Buff boon in pair.Value)
                {
                    dict[pair.Key].Add(boon.ID);
                    _usedBoons[boon.ID] = boon;
                }
            }
            return boonsBySpec;
        }

        private Dictionary<string, List<DamageModifier>> BuildPersonalDamageModData(Dictionary<string, List<long>> dict)
        {
            var damageModBySpecs = new Dictionary<string, List<DamageModifier>>();
            // Collect all personal damage mods by spec
            foreach (KeyValuePair<string, List<Player>> pair in _log.PlayerListBySpec)
            {
                var specDamageModsName = new HashSet<string>(_log.DamageModifiers.GetModifiersPerProf(pair.Key).Select(x => x.Name));
                var damageModsToUse = new HashSet<DamageModifier>();
                foreach (Player player in pair.Value)
                {
                    var presentDamageMods = new HashSet<string>(player.GetPresentDamageModifier(_log).Intersect(specDamageModsName));
                    foreach (string name in presentDamageMods)
                    {
                        damageModsToUse.Add(_log.DamageModifiers.DamageModifiersByName[name]);
                    }
                }
                damageModBySpecs[pair.Key] = damageModsToUse.ToList();
            }
            foreach (KeyValuePair<string, List<DamageModifier>> pair in damageModBySpecs)
            {
                dict[pair.Key] = new List<long>();
                foreach (DamageModifier mod in pair.Value)
                {
                    dict[pair.Key].Add(mod.Name.GetHashCode());
                    _usedDamageMods.Add(mod);
                }
            }
            return damageModBySpecs;
        }

        private List<BuffData> BuildPersonalBuffUptimeData(Dictionary<string, List<Buff>> boonsBySpec, int phaseIndex)
        {
            var list = new List<BuffData>();
            foreach (Player player in _log.PlayerList)
            {
                list.Add(new BuffData(player.Prof, boonsBySpec, player.GetBuffs(_log, phaseIndex, BuffEnum.Self)));
            }
            return list;
        }

        private List<BuffData> BuildActivePersonalBuffUptimeData(Dictionary<string, List<Buff>> boonsBySpec, int phaseIndex)
        {
            var list = new List<BuffData>();
            foreach (Player player in _log.PlayerList)
            {
                list.Add(new BuffData(player.Prof, boonsBySpec, player.GetActiveBuffs(_log, phaseIndex, BuffEnum.Self)));
            }
            return list;
        }

        private List<DamageModData> BuildDmgModifiersData(int phaseIndex, List<DamageModifier> damageModsToUse)
        {
            var pData = new List<DamageModData>();
            foreach (Player player in _log.PlayerList)
            {
                pData.Add(new DamageModData(player, _log, damageModsToUse, phaseIndex));
            }
            return pData;
        }

        private List<DamageModData> BuildPersonalDmgModifiersData(int phaseIndex, Dictionary<string, List<DamageModifier>> damageModsToUse)
        {
            var pData = new List<DamageModData>();
            foreach (Player player in _log.PlayerList)
            {
                pData.Add(new DamageModData(player, _log, damageModsToUse[player.Prof], phaseIndex));
            }
            return pData;
        }

        /// <summary>
        /// Create the self buff generation table
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="listToUse"></param>
        /// <param name="tableId"></param>
        /// <param name="phaseIndex"></param>
        private List<BuffData> BuildBuffGenerationData(List<Buff> listToUse, int phaseIndex, BuffEnum target)
        {
            var list = new List<BuffData>();

            foreach (Player player in _log.PlayerList)
            {
                Dictionary<long, FinalPlayerBuffs> uptimes;
                uptimes = player.GetBuffs(_log, phaseIndex, target);
                list.Add(new BuffData(listToUse, uptimes));
            }
            return list;
        }

        private List<BuffData> BuildActiveBuffGenerationData(List<Buff> listToUse, int phaseIndex, BuffEnum target)
        {
            var list = new List<BuffData>();

            foreach (Player player in _log.PlayerList)
            {
                Dictionary<long, FinalPlayerBuffs> uptimes;
                uptimes = player.GetActiveBuffs(_log, phaseIndex, target);
                list.Add(new BuffData(listToUse, uptimes));
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
            var list = new List<object[]>();

            PhaseData phase = _phases[phaseIndex];
            List<AbstractCastEvent> casting = p.GetCastLogsActDur(_log, phase.Start, phase.End);
            foreach (AbstractCastEvent cl in casting)
            {
                if (!_usedSkills.ContainsKey(cl.SkillId))
                {
                    _usedSkills.Add(cl.SkillId, cl.Skill);
                }

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
            var res = new List<DeathRecapDto>();
            List<DeathRecap> recaps = p.GetDeathRecaps(_log);
            if (!recaps.Any())
            {
                return null;
            }
            foreach (DeathRecap deathRecap in recaps)
            {
                var recap = new DeathRecapDto()
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

        private List<object[]> BuildDMGDistBodyData(List<AbstractCastEvent> casting, List<AbstractDamageEvent> damageLogs)
        {
            var list = new List<object[]>();
            var castLogsBySkill = casting.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            var damageLogsBySkill = damageLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            var conditionsById = _statistics.PresentConditions.ToDictionary(x => x.ID);
            foreach (KeyValuePair<SkillItem, List<AbstractDamageEvent>> entry in damageLogsBySkill)
            {
                list.Add(DmgDistributionDto.GetDMGDtoItem(entry, castLogsBySkill, _usedSkills, _usedBoons, _log.Buffs));
            }
            // non damaging
            foreach (KeyValuePair<SkillItem, List<AbstractCastEvent>> entry in castLogsBySkill)
            {
                if (damageLogsBySkill.ContainsKey(entry.Key))
                {
                    continue;
                }

                if (!_usedSkills.ContainsKey(entry.Key.ID))
                {
                    _usedSkills.Add(entry.Key.ID, entry.Key);
                }

                int casts = entry.Value.Count;
                int timeswasted = 0, timessaved = 0;
                foreach (AbstractCastEvent cl in entry.Value)
                {
                    if (cl.Interrupted)
                    {
                        timeswasted += cl.ActualDuration;
                    }
                    else if (cl.ReducedAnimation && cl.ActualDuration < cl.ExpectedDuration)
                    {
                        timessaved += cl.ExpectedDuration - cl.ActualDuration;
                    }
                }

                object[] skillData = { false, entry.Key.ID, 0, -1, 0, casts,
                    0, 0, 0, 0, timeswasted / 1000.0, timessaved / 1000.0, 0 };
                list.Add(skillData);
            }
            return list;
        }

        private DmgDistributionDto BuildDMGDistDataInternal(FinalDPS dps, AbstractSingleActor p, NPC target, int phaseIndex)
        {
            var dto = new DmgDistributionDto();
            PhaseData phase = _phases[phaseIndex];
            List<AbstractCastEvent> casting = p.GetCastLogs(_log, phase.Start, phase.End);
            List<AbstractDamageEvent> damageLogs = p.GetJustPlayerDamageLogs(target, _log, phase);
            dto.TotalDamage = dps.Damage;
            dto.ContributedDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            dto.ContributedShieldDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.ShieldDamage) : 0;
            dto.Distribution = BuildDMGDistBodyData(casting, damageLogs);

            return dto;
        }

        /// <summary>
        /// Creates the damage distribution table for a given player
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        private DmgDistributionDto BuildPlayerDMGDistData(Player p, NPC target, int phaseIndex)
        {
            FinalDPS dps = p.GetDPSTarget(_log, phaseIndex, target);
            return BuildDMGDistDataInternal(dps, p, target, phaseIndex);
        }

        /// <summary>
        /// Creates the damage distribution table for a target
        /// </summary>
        private DmgDistributionDto BuildTargetDMGDistData(NPC target, int phaseIndex)
        {
            FinalDPS dps = target.GetDPSAll(_log, phaseIndex);
            return BuildDMGDistDataInternal(dps, target, null, phaseIndex);
        }

        private DmgDistributionDto BuildDMGDistDataMinionsInternal(FinalDPS dps, Minions minions, NPC target, int phaseIndex)
        {
            var dto = new DmgDistributionDto();
            PhaseData phase = _phases[phaseIndex];
            List<AbstractCastEvent> casting = minions.GetCastLogs(_log, phase.Start, phase.End);
            List<AbstractDamageEvent> damageLogs = minions.GetDamageLogs(target, _log, phase);
            dto.ContributedDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.Damage) : 0;
            dto.ContributedShieldDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.ShieldDamage) : 0;
            dto.TotalDamage = dps.Damage;
            dto.Distribution = BuildDMGDistBodyData(casting, damageLogs);
            return dto;
        }

        /// <summary>
        /// Creates the damage distribution table for a given minion
        /// </summary>
        private DmgDistributionDto BuildPlayerMinionDMGDistData(Player p, Minions minions, NPC target, int phaseIndex)
        {
            FinalDPS dps = p.GetDPSTarget(_log, phaseIndex, target);

            return BuildDMGDistDataMinionsInternal(dps, minions, target, phaseIndex);
        }

        /// <summary>
        /// Creates the damage distribution table for a given boss minion
        /// </summary>
        private DmgDistributionDto BuildTargetMinionDMGDistData(NPC target, Minions minions, int phaseIndex)
        {
            FinalDPS dps = target.GetDPSAll(_log, phaseIndex);
            return BuildDMGDistDataMinionsInternal(dps, minions, null, phaseIndex);
        }

        /// <summary>
        /// Create the damage taken distribution table for a given player
        /// </summary>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        private DmgDistributionDto BuildDMGTakenDistData(AbstractSingleActor p, int phaseIndex)
        {
            var dto = new DmgDistributionDto
            {
                Distribution = new List<object[]>()
            };
            PhaseData phase = _phases[phaseIndex];
            List<AbstractDamageEvent> damageLogs = p.GetDamageTakenLogs(null, _log, phase.Start, phase.End);
            var damageLogsBySkill = damageLogs.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            dto.ContributedDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => (long)x.Damage) : 0;
            dto.ContributedShieldDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => (long)x.ShieldDamage) : 0;
            var conditionsById = _statistics.PresentConditions.ToDictionary(x => x.ID);
            foreach (KeyValuePair<SkillItem, List<AbstractDamageEvent>> entry in damageLogsBySkill)
            {
                dto.Distribution.Add(DmgDistributionDto.GetDMGDtoItem(entry, null, _usedSkills, _usedBoons, _log.Buffs));
            }
            return dto;
        }

        private void BuildBoonGraphData(List<BuffChartDataDto> list, List<Buff> listToUse, Dictionary<long, BuffsGraphModel> boonGraphData, PhaseData phase)
        {
            foreach (Buff buff in listToUse)
            {
                if (boonGraphData.TryGetValue(buff.ID, out BuffsGraphModel bgm))
                {
                    BuffChartDataDto graph = BuildBoonGraph(bgm, phase);
                    if (graph != null)
                    {
                        list.Add(graph);
                    }
                }
                boonGraphData.Remove(buff.ID);
            }
        }

        private List<BuffChartDataDto> BuildBoonGraphData(AbstractSingleActor p, int phaseIndex)
        {
            var list = new List<BuffChartDataDto>();
            PhaseData phase = _phases[phaseIndex];
            var boonGraphData = p.GetBuffGraphs(_log).ToDictionary(x => x.Key, x => x.Value);
            BuildBoonGraphData(list, _statistics.PresentBoons, boonGraphData, phase);
            BuildBoonGraphData(list, _statistics.PresentConditions, boonGraphData, phase);
            BuildBoonGraphData(list, _statistics.PresentOffbuffs, boonGraphData, phase);
            BuildBoonGraphData(list, _statistics.PresentDefbuffs, boonGraphData, phase);
            foreach (BuffsGraphModel bgm in boonGraphData.Values)
            {
                BuffChartDataDto graph = BuildBoonGraph(bgm, phase);
                if (graph != null)
                {
                    list.Add(graph);
                }
            }
            if (p.GetType() == typeof(Player))
            {
                foreach (NPC mainTarget in _log.FightData.GetMainTargets(_log))
                {
                    boonGraphData = mainTarget.GetBuffGraphs(_log);
                    foreach (BuffsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.Buff.Name == "Compromised" || x.Buff.Name == "Unnatural Signet" || x.Buff.Name == "Fractured - Enemy" || x.Buff.Name == "Erratic Energy"))
                    {
                        BuffChartDataDto graph = BuildBoonGraph(bgm, phase);
                        if (graph != null)
                        {
                            list.Add(graph);
                        }
                    }
                }
            }
            list.Reverse();
            return list;
        }

        private BuffChartDataDto BuildBoonGraph(BuffsGraphModel bgm, PhaseData phase)
        {
            var bChart = bgm.BuffChart.Where(x => x.End >= phase.Start && x.Start <= phase.End
            ).ToList();
            if (bChart.Count == 0 || (bChart.Count == 1 && bChart.First().Value == 0))
            {
                return null;
            }
            _usedBoons[bgm.Buff.ID] = bgm.Buff;
            return new BuffChartDataDto(bgm, bChart, phase);
        }

        private List<FoodDto> BuildPlayerFoodData(Player p)
        {
            var list = new List<FoodDto>();
            List<Consumable> consume = p.GetConsumablesList(_log, 0, _log.FightData.FightEnd);

            foreach (Consumable entry in consume)
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
            var list = new List<List<int[]>>();
            HashSet<Mechanic> presMech = _log.MechanicData.GetPresentPlayerMechs(_log, 0);
            PhaseData phase = _phases[phaseIndex];

            foreach (Player p in _log.PlayerList)
            {
                list.Add(MechanicDto.GetMechanicData(presMech, _log, p, phase));
            }
            return list;
        }

        private List<List<int[]>> BuildEnemyMechanicData(int phaseIndex)
        {
            var list = new List<List<int[]>>();
            HashSet<Mechanic> presMech = _log.MechanicData.GetPresentEnemyMechs(_log, 0);
            PhaseData phase = _phases[phaseIndex];
            foreach (AbstractActor enemy in _log.MechanicData.GetEnemyList(_log, 0))
            {
                list.Add(MechanicDto.GetMechanicData(presMech, _log, enemy, phase));
            }
            return list;
        }

        private List<MechanicChartDataDto> BuildMechanicsChartData()
        {
            var mechanicsChart = new List<MechanicChartDataDto>();
            foreach (Mechanic mech in _log.MechanicData.GetPresentMechanics(_log, 0))
            {
                List<MechanicEvent> mechanicLogs = _log.MechanicData.GetMechanicLogs(_log, mech);
                var dto = new MechanicChartDataDto
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

        private List<List<List<object>>> BuildMechanicGraphPointData(List<MechanicEvent> mechanicLogs, bool enemyMechanic)
        {
            var list = new List<List<List<object>>>();
            foreach (PhaseData phase in _phases)
            {
                list.Add(MechanicChartDataDto.GetMechanicChartPoints(mechanicLogs, phase, _log, enemyMechanic));
            }
            return list;
        }

        private List<BuffData> BuildTargetCondiData(int phaseIndex, NPC target)
        {
            Dictionary<long, FinalBuffsDictionary> conditions = target.GetBuffsDictionary(_log, phaseIndex);
            var list = new List<BuffData>();

            foreach (Player player in _log.PlayerList)
            {
                list.Add(new BuffData(conditions, _statistics.PresentConditions, player));
            }
            return list;
        }

        private BuffData BuildTargetCondiUptimeData(int phaseIndex, NPC target)
        {
            Dictionary<long, FinalBuffs> buffs = target.GetBuffs(_log, phaseIndex);
            return new BuffData(buffs, _statistics.PresentConditions, target.GetGameplayStats(_log, phaseIndex).AvgConditions);
        }

        private BuffData BuildTargetBoonData(int phaseIndex, NPC target)
        {
            Dictionary<long, FinalBuffs> buffs = target.GetBuffs(_log, phaseIndex);
            return new BuffData(buffs, _statistics.PresentBoons, target.GetGameplayStats(_log, phaseIndex).AvgBoons);
        }

        private string ReplaceVariables(string html)
        {
            html = html.Replace("${bootstrapTheme}", !Properties.Settings.Default.LightTheme ? "slate" : "yeti");

            html = html.Replace("${encounterStart}", _log.LogData.LogStart);
            html = html.Replace("${encounterEnd}", _log.LogData.LogEnd);
            html = html.Replace("${evtcVersion}", _log.LogData.BuildVersion);
            html = html.Replace("${fightID}", _log.FightData.TriggerID.ToString());
            html = html.Replace("${eiVersion}", Application.ProductVersion);
            html = html.Replace("${recordedBy}", _log.LogData.PoVName);

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
#if DEBUG
            html = html.Replace("<!--${Vue}-->", "<script src=\"https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.js\"></script>");
#else
            html = html.Replace("<!--${Vue}-->", "<script src=\"https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.min.js\"></script>");
#endif

            html = html.Replace("'${graphDataJson}'", BuildGraphJson());

            html = html.Replace("<!--${CombatReplayScript}-->", BuildCombatReplayScript(path));
            sw.Write(html);
            return;
        }

        private string BuildCombatReplayScript(string path)
        {
            if (!_cr)
            {
                return "";
            }
            string scriptContent = Properties.Resources.combatreplay_js;
            if (Properties.Settings.Default.HtmlExternalScripts && path != null)
            {
#if DEBUG
                string jsFileName = "EliteInsights-CR-" + _scriptVersion + ".debug.js";
#else
                string jsFileName = "EliteInsights-CR-" + _scriptVersion + ".js";
#endif
                string jsPath = Path.Combine(path, jsFileName);
                try
                {
                    using (var fs = new FileStream(jsPath, FileMode.Create, FileAccess.Write))
                    using (var scriptWriter = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
                    {
                        scriptWriter.Write(scriptContent);
                    }
                }
                catch (IOException)
                {
                }
                string content = "<script src=\"./" + jsFileName + "?version=" + _scriptVersionRev + "\"></script>\n";
                return content;
            }
            else
            {
                return "<script>\r\n" + scriptContent + "\r\n</script>";
            }
        }

        private static string BuildTemplates(string script)
        {
            string tmplScript = script;
            var templates = new Dictionary<string, string>()
            {
                {"${tmplBuffStats}",Properties.Resources.tmplBuffStats },
                {"${tmplBuffStatsTarget}",Properties.Resources.tmplBuffStatsTarget },
                {"${tmplBuffTable}",Properties.Resources.tmplBuffTable },
                {"${tmplDamageDistPlayer}",Properties.Resources.tmplDamageDistPlayer },
                {"${tmplDamageDistTable}",Properties.Resources.tmplDamageDistTable },
                {"${tmplDamageDistTarget}",Properties.Resources.tmplDamageDistTarget },
                {"${tmplDamageModifierTable}",Properties.Resources.tmplDamageModifierTable },
                {"${tmplDamageModifierStats}",Properties.Resources.tmplDamageModifierStats },
                {"${tmplDamageModifierPersStats}",Properties.Resources.tmplDamageModifierPersStats },
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
            foreach (KeyValuePair<string, string> entry in templates)
            {
#if DEBUG
                tmplScript = tmplScript.Replace(entry.Key, entry.Value);
#else
                tmplScript = tmplScript.Replace(entry.Key, Regex.Replace(entry.Value, @"\t|\n|\r", ""));
#endif
            }
            return tmplScript;
        }

        private static string BuildCRTemplates(string script)
        {
            string tmplScript = script;
            var CRtemplates = new Dictionary<string, string>()
                {
                    {"${tmplCombatReplayDamageData}", Properties.Resources.tmplCombatReplayDamageData },
                    {"${tmplCombatReplayStatusData}", Properties.Resources.tmplCombatReplayStatusData },
                    {"${tmplCombatReplayDamageTable}", Properties.Resources.tmplCombatReplayDamageTable },
                    {"${tmplCombatReplayActorBuffStats}", Properties.Resources.tmplCombatReplayActorBuffStats },
                    {"${tmplCombatReplayPlayerStats}", Properties.Resources.tmplCombatReplayPlayerStats },
                    {"${tmplCombatReplayPlayerStatus}", Properties.Resources.tmplCombatReplayPlayerStatus },
                    {"${tmplCombatReplayActorRotation}", Properties.Resources.tmplCombatReplayActorRotation },
                    {"${tmplCombatReplayTargetStats}", Properties.Resources.tmplCombatReplayTargetStats },
                    {"${tmplCombatReplayTargetStatus}", Properties.Resources.tmplCombatReplayTargetStatus },
                    {"${tmplCombatReplayTargetsStats}", Properties.Resources.tmplCombatReplayTargetsStats },
                    {"${tmplCombatReplayPlayersStats}", Properties.Resources.tmplCombatReplayPlayersStats },
                    {"${tmplCombatReplayUI}", Properties.Resources.tmplCombatReplayUI },
                    {"${tmplCombatReplayPlayerSelect}", Properties.Resources.tmplCombatReplayPlayerSelect },
                    {"${tmplCombatReplayRangeSelect}", Properties.Resources.tmplCombatReplayRangeSelect },
                    {"${tmplCombatReplayAnimationControl}", Properties.Resources.tmplCombatReplayAnimationControl },
                    {"${tmplCombatReplayMechanicsList}", Properties.Resources.tmplCombatReplayMechanicsList },
                };
            foreach (KeyValuePair<string, string> entry in CRtemplates)
            {
                tmplScript = tmplScript.Replace(entry.Key, Regex.Replace(entry.Value, @"\t|\n|\r", ""));
            }
            return tmplScript;
        }

        private string BuildCss(string path)
        {
            string scriptContent = Properties.Resources.ei_css;

            if (Properties.Settings.Default.HtmlExternalScripts && path != null)
            {
#if DEBUG
                string cssFilename = "EliteInsights-" + _scriptVersion + ".debug.css";
#else
                string cssFilename = "EliteInsights-" + _scriptVersion + ".css";
#endif
                string cssPath = Path.Combine(path, cssFilename);
                try
                {
                    using (var fs = new FileStream(cssPath, FileMode.Create, FileAccess.Write))
                    using (var scriptWriter = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
                    {
                        scriptWriter.Write(scriptContent);
                    }
                }
                catch (IOException)
                {
                }
                return "<link rel=\"stylesheet\" type=\"text/css\" href=\"./" + cssFilename + "?version=" + _scriptVersionRev + "\">";
            }
            else
            {
                return "<style type=\"text/css\">\r\n" + scriptContent + "\r\n</style>";
            }
        }

        private string BuildEIJs(string path)
        {
            var orderedScripts = new List<string>()
            {
                Properties.Resources.globalJS,
                Properties.Resources.commonsJS,
                Properties.Resources.headerJS,
                Properties.Resources.layoutJS,
                Properties.Resources.generalStatsJS,
                Properties.Resources.damageModifierStatsJS,
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

            if (Properties.Settings.Default.HtmlExternalScripts && path != null)
            {
#if DEBUG
                string scriptFilename = "EliteInsights-" + _scriptVersion + ".debug.js";
#else
                string scriptFilename = "EliteInsights-" + _scriptVersion + ".js";
#endif
                string scriptPath = Path.Combine(path, scriptFilename);
                try
                {
                    using (var fs = new FileStream(scriptPath, FileMode.Create, FileAccess.Write))
                    using (var scriptWriter = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
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
            if (!_cr)
            {
                return "";
            }
            var orderedScripts = new List<string>()
            {
                Properties.Resources.combatReplayStatsUI,
                Properties.Resources.combatReplayStatsJS,
            };
            string scriptContent = orderedScripts[0];
            for (int i = 1; i < orderedScripts.Count; i++)
            {
                scriptContent += orderedScripts[i];
            }
            scriptContent = BuildCRTemplates(scriptContent);

            if (Properties.Settings.Default.HtmlExternalScripts && path != null)
            {
#if DEBUG
                string scriptFilename = "EliteInsights-CRLink-" + _scriptVersion + ".debug.js";
#else
                string scriptFilename = "EliteInsights-CRLink-" + _scriptVersion + ".js";
#endif
                string scriptPath = Path.Combine(path, scriptFilename);
                try
                {
                    using (var fs = new FileStream(scriptPath, FileMode.Create, FileAccess.Write))
                    using (var scriptWriter = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
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
            var chartData = new ChartDataDto();
            var phaseChartData = new List<PhaseChartDataDto>();
            for (int i = 0; i < _phases.Count; i++)
            {
                var phaseData = new PhaseChartDataDto()
                {
                    Players = BuildPlayerGraphData(i)
                };
                foreach (NPC target in _phases[i].Targets)
                {
                    phaseData.Targets.Add(BuildTargetGraphData(i, target));
                }
                if (i == 0)
                {
                    phaseData.TargetsHealthForCR = new List<double[]>();
                    foreach (NPC target in _log.FightData.Logic.Targets)
                    {
                        phaseData.TargetsHealthForCR.Add(target.Get1SHealthGraph(_log)[0]);
                    }
                }

                phaseChartData.Add(phaseData);
            }
            chartData.Phases = phaseChartData;
            chartData.Mechanics = BuildMechanicsChartData();
            return ToJson(chartData);
        }

        private string BuildLogData()
        {
            var logData = new LogDataDto();
            if (_cr)
            {
                logData.CrData = new CombatReplayDto(_log);
            }
            foreach (Player player in _log.PlayerList)
            {
                logData.Players.Add(new PlayerDto(player, _log, _cr, BuildPlayerData(player)));
            }

            foreach (AbstractActor enemy in _log.MechanicData.GetEnemyList(_log, 0))
            {
                logData.Enemies.Add(new EnemyDto() { Name = enemy.Character });
            }

            foreach (NPC target in _log.FightData.Logic.Targets)
            {
                var targetDto = new TargetDto(target, _log, _cr, BuildTargetData(target));


                logData.Targets.Add(targetDto);
            }
            //
            Dictionary<string, List<Buff>> persBuffDict = BuildPersonalBoonData(logData.PersBuffs);
            Dictionary<string, List<DamageModifier>> persDamageModDict = BuildPersonalDamageModData(logData.DmgModifiersPers);
            var allDamageMods = new HashSet<string>();
            foreach (Player p in _log.PlayerList)
            {
                allDamageMods.UnionWith(p.GetPresentDamageModifier(_log));
            }
            var commonDamageModifiers = new List<DamageModifier>();
            foreach (DamageModifier dMod in _log.DamageModifiers.DamageModifiersPerSource[DamageModifier.ModifierSource.CommonBuff])
            {
                if (allDamageMods.Contains(dMod.Name))
                {
                    commonDamageModifiers.Add(dMod);
                    logData.DmgModifiersCommon.Add(dMod.Name.GetHashCode());
                    _usedDamageMods.Add(dMod);
                }
            }
            var itemDamageModifiers = new List<DamageModifier>();
            foreach (DamageModifier dMod in _log.DamageModifiers.DamageModifiersPerSource[DamageModifier.ModifierSource.ItemBuff])
            {
                if (allDamageMods.Contains(dMod.Name))
                {
                    itemDamageModifiers.Add(dMod);
                    logData.DmgModifiersItem.Add(dMod.Name.GetHashCode());
                    _usedDamageMods.Add(dMod);
                }
            }
            foreach (Buff boon in _statistics.PresentBoons)
            {
                logData.Boons.Add(boon.ID);
                _usedBoons[boon.ID] = boon;
            }
            foreach (Buff boon in _statistics.PresentConditions)
            {
                logData.Conditions.Add(boon.ID);
                _usedBoons[boon.ID] = boon;
            }
            foreach (Buff boon in _statistics.PresentOffbuffs)
            {
                logData.OffBuffs.Add(boon.ID);
                _usedBoons[boon.ID] = boon;
            }
            foreach (Buff boon in _statistics.PresentDefbuffs)
            {
                logData.DefBuffs.Add(boon.ID);
                _usedBoons[boon.ID] = boon;
            }
            //
            for (int i = 0; i < _phases.Count; i++)
            {
                PhaseData phaseData = _phases[i];
                var phaseDto = new PhaseDto(phaseData, _phases, _log)
                {
                    DpsStats = BuildDPSData(i),
                    DpsStatsTargets = BuildDPSTargetsData(i),
                    DmgStatsTargets = BuildDMGStatsTargetsData(i),
                    DmgStats = BuildDMGStatsData(i),
                    DefStats = BuildDefenseData(i),
                    SupportStats = BuildSupportData(i),
                    //
                    BoonStats = BuildBuffUptimeData(_statistics.PresentBoons, i),
                    OffBuffStats = BuildBuffUptimeData(_statistics.PresentOffbuffs, i),
                    DefBuffStats = BuildBuffUptimeData(_statistics.PresentDefbuffs, i),
                    PersBuffStats = BuildPersonalBuffUptimeData(persBuffDict, i),
                    BoonGenSelfStats = BuildBuffGenerationData(_statistics.PresentBoons, i, BuffEnum.Self),
                    BoonGenGroupStats = BuildBuffGenerationData(_statistics.PresentBoons, i, BuffEnum.Group),
                    BoonGenOGroupStats = BuildBuffGenerationData(_statistics.PresentBoons, i, BuffEnum.OffGroup),
                    BoonGenSquadStats = BuildBuffGenerationData(_statistics.PresentBoons, i, BuffEnum.Squad),
                    OffBuffGenSelfStats = BuildBuffGenerationData(_statistics.PresentOffbuffs, i, BuffEnum.Self),
                    OffBuffGenGroupStats = BuildBuffGenerationData(_statistics.PresentOffbuffs, i, BuffEnum.Group),
                    OffBuffGenOGroupStats = BuildBuffGenerationData(_statistics.PresentOffbuffs, i, BuffEnum.OffGroup),
                    OffBuffGenSquadStats = BuildBuffGenerationData(_statistics.PresentOffbuffs, i, BuffEnum.Squad),
                    DefBuffGenSelfStats = BuildBuffGenerationData(_statistics.PresentDefbuffs, i, BuffEnum.Self),
                    DefBuffGenGroupStats = BuildBuffGenerationData(_statistics.PresentDefbuffs, i, BuffEnum.Group),
                    DefBuffGenOGroupStats = BuildBuffGenerationData(_statistics.PresentDefbuffs, i, BuffEnum.OffGroup),
                    DefBuffGenSquadStats = BuildBuffGenerationData(_statistics.PresentDefbuffs, i, BuffEnum.Squad),
                    //
                    BoonActiveStats = BuildActiveBuffUptimeData(_statistics.PresentBoons, i),
                    OffBuffActiveStats = BuildActiveBuffUptimeData(_statistics.PresentOffbuffs, i),
                    DefBuffActiveStats = BuildActiveBuffUptimeData(_statistics.PresentDefbuffs, i),
                    PersBuffActiveStats = BuildActivePersonalBuffUptimeData(persBuffDict, i),
                    BoonGenActiveSelfStats = BuildActiveBuffGenerationData(_statistics.PresentBoons, i, BuffEnum.Self),
                    BoonGenActiveGroupStats = BuildActiveBuffGenerationData(_statistics.PresentBoons, i, BuffEnum.Group),
                    BoonGenActiveOGroupStats = BuildActiveBuffGenerationData(_statistics.PresentBoons, i, BuffEnum.OffGroup),
                    BoonGenActiveSquadStats = BuildActiveBuffGenerationData(_statistics.PresentBoons, i, BuffEnum.Squad),
                    OffBuffGenActiveSelfStats = BuildActiveBuffGenerationData(_statistics.PresentOffbuffs, i, BuffEnum.Self),
                    OffBuffGenActiveGroupStats = BuildActiveBuffGenerationData(_statistics.PresentOffbuffs, i, BuffEnum.Group),
                    OffBuffGenActiveOGroupStats = BuildActiveBuffGenerationData(_statistics.PresentOffbuffs, i, BuffEnum.OffGroup),
                    OffBuffGenActiveSquadStats = BuildActiveBuffGenerationData(_statistics.PresentOffbuffs, i, BuffEnum.Squad),
                    DefBuffGenActiveSelfStats = BuildActiveBuffGenerationData(_statistics.PresentDefbuffs, i, BuffEnum.Self),
                    DefBuffGenActiveGroupStats = BuildActiveBuffGenerationData(_statistics.PresentDefbuffs, i, BuffEnum.Group),
                    DefBuffGenActiveOGroupStats = BuildActiveBuffGenerationData(_statistics.PresentDefbuffs, i, BuffEnum.OffGroup),
                    DefBuffGenActiveSquadStats = BuildActiveBuffGenerationData(_statistics.PresentDefbuffs, i, BuffEnum.Squad),
                    //
                    DmgModifiersCommon = BuildDmgModifiersData(i, commonDamageModifiers),
                    DmgModifiersItem = BuildDmgModifiersData(i, itemDamageModifiers),
                    DmgModifiersPers = BuildPersonalDmgModifiersData(i, persDamageModDict),
                    TargetsCondiStats = new List<List<BuffData>>(),
                    TargetsCondiTotals = new List<BuffData>(),
                    TargetsBoonTotals = new List<BuffData>(),
                    MechanicStats = BuildPlayerMechanicData(i),
                    EnemyMechanicStats = BuildEnemyMechanicData(i)
                };
                foreach (NPC target in phaseData.Targets)
                {
                    phaseDto.TargetsCondiStats.Add(BuildTargetCondiData(i, target));
                    phaseDto.TargetsCondiTotals.Add(BuildTargetCondiUptimeData(i, target));
                    phaseDto.TargetsBoonTotals.Add(HasBoons(i, target) ? BuildTargetBoonData(i, target) : null);
                }
                logData.Phases.Add(phaseDto);
            }
            //
            logData.EncounterDuration = _log.FightData.DurationString;
            logData.Success = _log.FightData.Success;
            logData.Wvw = _log.FightData.Logic.Mode == FightLogic.ParseMode.WvW;
            logData.Targetless = _log.FightData.Logic.Targetless;
            logData.FightName = _log.FightData.Name;
            logData.FightIcon = _log.FightData.Logic.Icon;
            logData.LightTheme = Properties.Settings.Default.LightTheme;
            logData.SingleGroup = _log.PlayerList.Where(x => !x.IsFakeActor).Select(x => x.Group).Distinct().Count() == 1;
            logData.NoMechanics = _log.FightData.Logic.HasNoFightSpecificMechanics;
            //
            SkillDto.AssembleSkills(_usedSkills.Values, logData.SkillMap);
            DamageModDto.AssembleDamageModifiers(_usedDamageMods, logData.DamageModMap);
            BuffDto.AssembleBoons(_usedBoons.Values, logData.BuffMap);
            MechanicDto.BuildMechanics(_log.MechanicData.GetPresentMechanics(_log, 0), logData.MechanicMap);
            return ToJson(logData);
        }

        private bool HasBoons(int phaseIndex, NPC target)
        {
            Dictionary<long, FinalBuffs> conditions = target.GetBuffs(_log, phaseIndex);
            foreach (Buff boon in _statistics.PresentBoons)
            {
                if (conditions.TryGetValue(boon.ID, out FinalBuffs uptime))
                {
                    if (uptime.Uptime > 0.0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private ActorDetailsDto BuildPlayerData(Player player)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTargets = new List<List<DmgDistributionDto>>(),
                DmgDistributionsTaken = new List<DmgDistributionDto>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>(),
                Food = BuildPlayerFoodData(player),
                Minions = new List<ActorDetailsDto>(),
                DeathRecap = BuildDeathRecap(player)
            };
            for (int i = 0; i < _phases.Count; i++)
            {
                dto.Rotation.Add(BuildRotationData(player, i));
                dto.DmgDistributions.Add(BuildPlayerDMGDistData(player, null, i));
                var dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (NPC target in _phases[i].Targets)
                {
                    dmgTargetsDto.Add(BuildPlayerDMGDistData(player, target, i));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributionsTaken.Add(BuildDMGTakenDistData(player, i));
                dto.BoonGraph.Add(BuildBoonGraphData(player, i));
            }
            foreach (KeyValuePair<long, Minions> pair in player.GetMinions(_log))
            {
                dto.Minions.Add(BuildPlayerMinionsData(player, pair.Value));
            }

            return dto;
        }

        private ActorDetailsDto BuildPlayerMinionsData(Player player, Minions minion)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTargets = new List<List<DmgDistributionDto>>()
            };
            for (int i = 0; i < _phases.Count; i++)
            {
                var dmgTargetsDto = new List<DmgDistributionDto>();
                foreach (NPC target in _phases[i].Targets)
                {
                    dmgTargetsDto.Add(BuildPlayerMinionDMGDistData(player, minion, target, i));
                }
                dto.DmgDistributionsTargets.Add(dmgTargetsDto);
                dto.DmgDistributions.Add(BuildPlayerMinionDMGDistData(player, minion, null, i));
            }
            return dto;
        }

        private ActorDetailsDto BuildTargetData(NPC target)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>(),
                DmgDistributionsTaken = new List<DmgDistributionDto>(),
                BoonGraph = new List<List<BuffChartDataDto>>(),
                Rotation = new List<List<object[]>>()
            };
            for (int i = 0; i < _phases.Count; i++)
            {
                if (_phases[i].Targets.Contains(target) || (i == 0 && _cr))
                {
                    dto.DmgDistributions.Add(BuildTargetDMGDistData(target, i));
                    dto.DmgDistributionsTaken.Add(BuildDMGTakenDistData(target, i));
                    dto.Rotation.Add(BuildRotationData(target, i));
                    dto.BoonGraph.Add(BuildBoonGraphData(target, i));
                }
                else
                {
                    dto.DmgDistributions.Add(new DmgDistributionDto());
                    dto.DmgDistributionsTaken.Add(new DmgDistributionDto());
                    dto.Rotation.Add(new List<object[]>());
                    dto.BoonGraph.Add(new List<BuffChartDataDto>());
                }
            }

            dto.Minions = new List<ActorDetailsDto>();
            foreach (KeyValuePair<long, Minions> pair in target.GetMinions(_log))
            {
                dto.Minions.Add(BuildTargetsMinionsData(target, pair.Value));
            }
            return dto;
        }

        private ActorDetailsDto BuildTargetsMinionsData(NPC target, Minions minion)
        {
            var dto = new ActorDetailsDto
            {
                DmgDistributions = new List<DmgDistributionDto>()
            };
            for (int i = 0; i < _phases.Count; i++)
            {
                if (_phases[i].Targets.Contains(target) || (i == 0 && _cr))
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

        private static string ToJson(object value)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = contractResolver
            };
            return JsonConvert.SerializeObject(value, settings);
        }
    }
}
