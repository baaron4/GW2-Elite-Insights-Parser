using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels
{
    public class LogDataDto
    {
        public List<TargetDto> Targets { get; internal set; } = new List<TargetDto>();
        public List<PlayerDto> Players { get; } = new List<PlayerDto>();
        public List<EnemyDto> Enemies { get; } = new List<EnemyDto>();
        public List<PhaseDto> Phases { get; } = new List<PhaseDto>();
        public List<long> Boons { get; } = new List<long>();
        public List<long> OffBuffs { get; } = new List<long>();
        public List<long> DefBuffs { get; } = new List<long>();
        public List<long> FractalInstabilities { get; } = new List<long>();
        public List<long> DmgModifiersItem { get; } = new List<long>();
        public List<long> DmgModifiersCommon { get; } = new List<long>();
        public Dictionary<string, List<long>> DmgModifiersPers { get; } = new Dictionary<string, List<long>>();
        public Dictionary<string, List<long>> PersBuffs { get; } = new Dictionary<string, List<long>>();
        public List<long> Conditions { get; } = new List<long>();
        public Dictionary<string, SkillDto> SkillMap { get; } = new Dictionary<string, SkillDto>();
        public Dictionary<string, BuffDto> BuffMap { get; } = new Dictionary<string, BuffDto>();
        public Dictionary<string, DamageModDto> DamageModMap { get; } = new Dictionary<string, DamageModDto>();
        public List<MechanicDto> MechanicMap { get; internal set; } = new List<MechanicDto>();
        public CombatReplayDto CrData { get; internal set; } = null;
        public string EncounterDuration { get; internal set; }
        public bool Success { get; internal set; }
        public bool Wvw { get; internal set; }
        public bool HasCommander { get; internal set; }
        public bool Targetless { get; internal set; }
        public string FightName { get; internal set; }
        public string FightIcon { get; internal set; }
        public bool LightTheme { get; internal set; }
        public bool NoMechanics { get; internal set; }
        public bool SingleGroup { get; internal set; }
        public List<string> LogErrors { get; internal set; }

        public string EncounterStart { get; internal set; }
        public string EncounterEnd { get; internal set; }
        public string ArcVersion { get; internal set; }
        public ulong Gw2Build { get; internal set; }
        public long FightID { get; internal set; }
        public string Parser { get; internal set; }
        public string RecordedBy { get; internal set; }
        public List<string> UploadLinks { get; internal set; }


        private static Dictionary<string, List<Buff>> BuildPersonalBoonData(ParsedEvtcLog log, Dictionary<string, List<long>> dict, Dictionary<long, Buff> usedBuffs)
        {
            var boonsBySpec = new Dictionary<string, List<Buff>>();
            // Collect all personal buffs by spec
            foreach (KeyValuePair<string, List<Player>> pair in log.PlayerListBySpec)
            {
                List<Player> players = pair.Value;
                var specBoonIds = new HashSet<long>(log.Buffs.GetRemainingBuffsList(pair.Key).Select(x => x.ID));
                var boonToUse = new HashSet<Buff>();
                foreach (Player player in players)
                {
                    for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
                    {
                        Dictionary<long, FinalPlayerBuffs> boons = player.GetBuffs(log, i, BuffEnum.Self);
                        foreach (Buff boon in log.Statistics.PresentPersonalBuffs[player])
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
                    usedBuffs[boon.ID] = boon;
                }
            }
            return boonsBySpec;
        }

        private static Dictionary<string, List<DamageModifier>> BuildPersonalDamageModData(ParsedEvtcLog log, Dictionary<string, List<long>> dict, HashSet<DamageModifier> usedDamageMods)
        {
            var damageModBySpecs = new Dictionary<string, List<DamageModifier>>();
            // Collect all personal damage mods by spec
            foreach (KeyValuePair<string, List<Player>> pair in log.PlayerListBySpec)
            {
                var specDamageModsName = new HashSet<string>(log.DamageModifiers.GetModifiersPerProf(pair.Key).Select(x => x.Name));
                var damageModsToUse = new HashSet<DamageModifier>();
                foreach (Player player in pair.Value)
                {
                    var presentDamageMods = new HashSet<string>(player.GetPresentDamageModifier(log).Intersect(specDamageModsName));
                    foreach (string name in presentDamageMods)
                    {
                        damageModsToUse.Add(log.DamageModifiers.DamageModifiersByName[name]);
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
                    usedDamageMods.Add(mod);
                }
            }
            return damageModBySpecs;
        }

        private static bool HasBoons(ParsedEvtcLog log, int phaseIndex, NPC target)
        {
            Dictionary<long, FinalBuffs> conditions = target.GetBuffs(log, phaseIndex);
            foreach (Buff boon in log.Statistics.PresentBoons)
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

        internal static LogDataDto BuildLogData(ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills,Dictionary<long, Buff> usedBuffs, HashSet<DamageModifier> usedDamageMods, bool cr, bool light, string[] uploadLinks, string parser, Version version)
        {
            GeneralStatistics statistics = log.Statistics;
            log.UpdateProgressWithCancellationCheck("HTML: building Log Data");
            var logData = new LogDataDto
            {
                EncounterStart = log.LogData.LogStartStd,
                EncounterEnd = log.LogData.LogEndStd,
                ArcVersion = log.LogData.ArcVersion,
                Gw2Build = log.LogData.GW2Build,
                FightID = log.FightData.TriggerID,
                Parser = parser + " " + version.ToString(4),
                RecordedBy = log.LogData.PoVName,
                UploadLinks = uploadLinks.ToList()
            };
            if (cr)
            {
                logData.CrData = new CombatReplayDto(log);
            }
            log.UpdateProgressWithCancellationCheck("HTML: building Players");
            foreach (Player player in log.PlayerList)
            {
                logData.HasCommander = logData.HasCommander || player.HasCommanderTag;
                logData.Players.Add(new PlayerDto(player, log, cr, ActorDetailsDto.BuildPlayerData(log, player, usedSkills, usedBuffs)));
            }

            log.UpdateProgressWithCancellationCheck("HTML: building Enemies");
            foreach (AbstractActor enemy in log.MechanicData.GetEnemyList(log, 0))
            {
                logData.Enemies.Add(new EnemyDto() { Name = enemy.Character });
            }

            log.UpdateProgressWithCancellationCheck("HTML: building Targets");
            foreach (NPC target in log.FightData.Logic.Targets)
            {
                var targetDto = new TargetDto(target, log, cr, ActorDetailsDto.BuildTargetData(log, target, usedSkills, usedBuffs, cr));
                logData.Targets.Add(targetDto);
            }
            //
            log.UpdateProgressWithCancellationCheck("HTML: building Skill/Buff dictionaries");
            Dictionary<string, List<Buff>> persBuffDict = BuildPersonalBoonData(log, logData.PersBuffs, usedBuffs);
            Dictionary<string, List<DamageModifier>> persDamageModDict = BuildPersonalDamageModData(log, logData.DmgModifiersPers, usedDamageMods);
            var allDamageMods = new HashSet<string>();
            foreach (Player p in log.PlayerList)
            {
                allDamageMods.UnionWith(p.GetPresentDamageModifier(log));
            }
            var commonDamageModifiers = new List<DamageModifier>();
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(ParserHelper.Source.Common, out List<DamageModifier> list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        commonDamageModifiers.Add(dMod);
                        logData.DmgModifiersCommon.Add(dMod.Name.GetHashCode());
                        usedDamageMods.Add(dMod);
                    }
                }
            }
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(ParserHelper.Source.FightSpecific, out list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        commonDamageModifiers.Add(dMod);
                        logData.DmgModifiersCommon.Add(dMod.Name.GetHashCode());
                        usedDamageMods.Add(dMod);
                    }
                }
            }
            var itemDamageModifiers = new List<DamageModifier>();
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(ParserHelper.Source.Item, out list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        itemDamageModifiers.Add(dMod);
                        logData.DmgModifiersItem.Add(dMod.Name.GetHashCode());
                        usedDamageMods.Add(dMod);
                    }
                }
            }
            foreach (Buff boon in statistics.PresentBoons)
            {
                logData.Boons.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentConditions)
            {
                logData.Conditions.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentOffbuffs)
            {
                logData.OffBuffs.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentDefbuffs)
            {
                logData.DefBuffs.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentFractalInstabilities)
            {
                logData.FractalInstabilities.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            //
            log.UpdateProgressWithCancellationCheck("HTML: building Phases");
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phaseData = phases[i];
                var phaseDto = new PhaseDto(phaseData, phases, log)
                {
                    DpsStats = PhaseDto.BuildDPSData(log, i),
                    DpsStatsTargets = PhaseDto.BuildDPSTargetsData(log, i),
                    DmgStatsTargets = PhaseDto.BuildDMGStatsTargetsData(log, i),
                    DmgStats = PhaseDto.BuildDMGStatsData(log, i),
                    DefStats = PhaseDto.BuildDefenseData(log, i),
                    SupportStats = PhaseDto.BuildSupportData(log, i),
                    //
                    BoonStats = BuffData.BuildBuffUptimeData(log, statistics.PresentBoons, i),
                    OffBuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentOffbuffs, i),
                    DefBuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentDefbuffs, i),
                    PersBuffStats = BuffData.BuildPersonalBuffUptimeData(log, persBuffDict, i),
                    BoonGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Self),
                    BoonGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Group),
                    BoonGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.OffGroup),
                    BoonGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Squad),
                    OffBuffGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Self),
                    OffBuffGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Group),
                    OffBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.OffGroup),
                    OffBuffGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Squad),
                    DefBuffGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Self),
                    DefBuffGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Group),
                    DefBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.OffGroup),
                    DefBuffGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Squad),
                    //
                    BoonActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentBoons, i),
                    OffBuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentOffbuffs, i),
                    DefBuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentDefbuffs, i),
                    PersBuffActiveStats = BuffData.BuildActivePersonalBuffUptimeData(log, persBuffDict, i),
                    BoonGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Self),
                    BoonGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Group),
                    BoonGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.OffGroup),
                    BoonGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, i, BuffEnum.Squad),
                    OffBuffGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Self),
                    OffBuffGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Group),
                    OffBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.OffGroup),
                    OffBuffGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, i, BuffEnum.Squad),
                    DefBuffGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Self),
                    DefBuffGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Group),
                    DefBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.OffGroup),
                    DefBuffGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, i, BuffEnum.Squad),
                    //
                    DmgModifiersCommon = DamageModData.BuildDmgModifiersData(log, i, commonDamageModifiers),
                    DmgModifiersItem = DamageModData.BuildDmgModifiersData(log, i, itemDamageModifiers),
                    DmgModifiersPers = DamageModData.BuildPersonalDmgModifiersData(log, i, persDamageModDict),
                    TargetsCondiStats = new List<List<BuffData>>(),
                    TargetsCondiTotals = new List<BuffData>(),
                    TargetsBoonTotals = new List<BuffData>(),
                    MechanicStats = MechanicDto.BuildPlayerMechanicData(log, i),
                    EnemyMechanicStats = MechanicDto.BuildEnemyMechanicData(log, i)
                };
                foreach (NPC target in phaseData.Targets)
                {
                    phaseDto.TargetsCondiStats.Add(BuffData.BuildTargetCondiData(log, i, target));
                    phaseDto.TargetsCondiTotals.Add(BuffData.BuildTargetCondiUptimeData(log, i, target));
                    phaseDto.TargetsBoonTotals.Add(HasBoons(log, i, target) ? BuffData.BuildTargetBoonData(log, i, target) : null);
                }
                logData.Phases.Add(phaseDto);
            }
            //
            log.UpdateProgressWithCancellationCheck("HTML: building Meta Data");
            logData.EncounterDuration = log.FightData.DurationString;
            logData.Success = log.FightData.Success;
            logData.Wvw = log.FightData.Logic.Mode == FightLogic.ParseMode.WvW;
            logData.Targetless = log.FightData.Logic.Targetless;
            logData.FightName = log.FightData.GetFightName(log);
            logData.FightIcon = log.FightData.Logic.Icon;
            logData.LightTheme = light;
            logData.SingleGroup = log.PlayerList.Where(x => !x.IsFakeActor).Select(x => x.Group).Distinct().Count() == 1;
            logData.NoMechanics = log.FightData.Logic.HasNoFightSpecificMechanics;
            if (log.LogData.LogErrors.Count > 0)
            {
                logData.LogErrors = new List<string>(log.LogData.LogErrors);
            }
            //
            SkillDto.AssembleSkills(usedSkills.Values, logData.SkillMap);
            DamageModDto.AssembleDamageModifiers(usedDamageMods, logData.DamageModMap);
            BuffDto.AssembleBoons(usedBuffs.Values, logData.BuffMap, log);
            MechanicDto.BuildMechanics(log.MechanicData.GetPresentMechanics(log, 0), logData.MechanicMap);
            return logData;
        }

    }
}
