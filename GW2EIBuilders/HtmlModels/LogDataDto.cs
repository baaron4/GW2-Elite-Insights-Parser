using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels
{
    internal class LogDataDto
    {
        public List<TargetDto> Targets { get; set; } = new List<TargetDto>();
        public List<PlayerDto> Players { get; } = new List<PlayerDto>();
        public List<EnemyDto> Enemies { get; } = new List<EnemyDto>();
        public List<PhaseDto> Phases { get; } = new List<PhaseDto>();
        public List<long> Boons { get; } = new List<long>();
        public List<long> OffBuffs { get; } = new List<long>();
        public List<long> SupBuffs { get; } = new List<long>();
        public List<long> DefBuffs { get; } = new List<long>();
        public List<long> GearBuffs { get; } = new List<long>();
        public List<long> FractalInstabilities { get; } = new List<long>();
        public List<long> DmgModifiersItem { get; } = new List<long>();
        public List<long> DmgModifiersCommon { get; } = new List<long>();
        public Dictionary<string, List<long>> DmgModifiersPers { get; } = new Dictionary<string, List<long>>();
        public Dictionary<string, List<long>> PersBuffs { get; } = new Dictionary<string, List<long>>();
        public List<long> Conditions { get; } = new List<long>();
        public Dictionary<string, SkillDto> SkillMap { get; } = new Dictionary<string, SkillDto>();
        public Dictionary<string, BuffDto> BuffMap { get; } = new Dictionary<string, BuffDto>();
        public Dictionary<string, DamageModDto> DamageModMap { get; } = new Dictionary<string, DamageModDto>();
        public List<MechanicDto> MechanicMap { get; set; } = new List<MechanicDto>();
        public CombatReplayDto CrData { get; set; } = null;
        public string EncounterDuration { get; set; }
        public bool Success { get; set; }
        public bool Wvw { get; set; }
        public bool HasCommander { get; set; }
        public bool Targetless { get; set; }
        public string FightName { get; set; }
        public string FightIcon { get; set; }
        public bool LightTheme { get; set; }
        public bool NoMechanics { get; set; }
        public bool SingleGroup { get; set; }
        public bool HasBreakbarDamage { get; set; }
        public List<string> LogErrors { get; set; }

        public string EncounterStart { get; set; }
        public string EncounterEnd { get; set; }
        public string ArcVersion { get; set; }
        public long EvtcVersion { get; set; }
        public ulong Gw2Build { get; set; }
        public long FightID { get; set; }
        public string Parser { get; set; }
        public string RecordedBy { get; set; }
        public List<string> UploadLinks { get; set; }
        public List<string> UsedExtensions { get; set; }


        private static Dictionary<string, IReadOnlyList<Buff>> BuildPersonalBuffData(ParsedEvtcLog log, Dictionary<string, List<long>> dict, Dictionary<long, Buff> usedBuffs)
        {
            var boonsBySpec = new Dictionary<string, IReadOnlyList<Buff>>();
            // Collect all personal buffs by spec
            foreach (KeyValuePair<string, List<AbstractSingleActor>> pair in log.FriendliesListBySpec)
            {
                List<AbstractSingleActor> friendlies = pair.Value;
                var specBoonIds = new HashSet<long>(log.Buffs.GetPersonalBuffsList(pair.Key).Select(x => x.ID));
                var boonToUse = new HashSet<Buff>();
                foreach (AbstractSingleActor actor in friendlies)
                {
                    foreach (PhaseData phase in log.FightData.GetPhases(log))
                    {
                        IReadOnlyDictionary<long, FinalActorBuffs> boons = actor.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
                        foreach (Buff boon in log.StatisticsHelper.GetPresentRemainingBuffsOnPlayer(actor))
                        {
                            if (boons.TryGetValue(boon.ID, out FinalActorBuffs uptime))
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
            foreach (KeyValuePair<string, IReadOnlyList<Buff>> pair in boonsBySpec)
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

        private static Dictionary<string, IReadOnlyList<DamageModifier>> BuildPersonalDamageModData(ParsedEvtcLog log, Dictionary<string, List<long>> dict, HashSet<DamageModifier> usedDamageMods)
        {
            var damageModBySpecs = new Dictionary<string, IReadOnlyList<DamageModifier>>();
            // Collect all personal damage mods by spec
            foreach (KeyValuePair<string, List<AbstractSingleActor>> pair in log.FriendliesListBySpec)
            {
                var specDamageModsName = new HashSet<string>(log.DamageModifiers.GetModifiersPerProf(pair.Key).Select(x => x.Name));
                var damageModsToUse = new HashSet<DamageModifier>();
                foreach (AbstractSingleActor actor in pair.Value)
                {
                    var presentDamageMods = new HashSet<string>(actor.GetPresentDamageModifier(log).Intersect(specDamageModsName));
                    foreach (string name in presentDamageMods)
                    {
                        damageModsToUse.Add(log.DamageModifiers.DamageModifiersByName[name]);
                    }
                }
                damageModBySpecs[pair.Key] = damageModsToUse.ToList();
            }
            foreach (KeyValuePair<string, IReadOnlyList<DamageModifier>> pair in damageModBySpecs)
            {
                dict[pair.Key] = new List<long>();
                foreach (DamageModifier mod in pair.Value)
                {
                    dict[pair.Key].Add(mod.ID);
                    usedDamageMods.Add(mod);
                }
            }
            return damageModBySpecs;
        }

        public static LogDataDto BuildLogData(ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, HashSet<DamageModifier> usedDamageMods, bool cr, bool light, Version parserVersion, string[] uploadLinks)
        {
            StatisticsHelper statistics = log.StatisticsHelper;
            log.UpdateProgressWithCancellationCheck("HTML: building Log Data");
            var logData = new LogDataDto
            {
                EncounterStart = log.LogData.LogStartStd,
                EncounterEnd = log.LogData.LogEndStd,
                ArcVersion = log.LogData.ArcVersion,
                EvtcVersion = log.LogData.EvtcVersion,
                Gw2Build = log.LogData.GW2Build,
                FightID = log.FightData.TriggerID,
                Parser = "Elite Insights " + parserVersion.ToString(),
                RecordedBy = log.LogData.PoVName,
                UploadLinks = uploadLinks.ToList(),
                UsedExtensions = log.LogData.UsedExtensions.Any() ? log.LogData.UsedExtensions.Select(x => x.Name + " - " + x.Version).ToList() : null
            };
            if (cr)
            {
                logData.CrData = new CombatReplayDto(log);
            }
            log.UpdateProgressWithCancellationCheck("HTML: building Players");
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                logData.HasCommander = logData.HasCommander || actor.HasCommanderTag;
                logData.Players.Add(new PlayerDto(actor, log, ActorDetailsDto.BuildPlayerData(log, actor, usedSkills, usedBuffs)));
            }

            log.UpdateProgressWithCancellationCheck("HTML: building Enemies");
            foreach (AbstractSingleActor enemy in log.MechanicData.GetEnemyList(log, 0, log.FightData.FightEnd))
            {
                logData.Enemies.Add(new EnemyDto() { Name = enemy.Character });
            }

            log.UpdateProgressWithCancellationCheck("HTML: building Targets");
            foreach (AbstractSingleActor target in log.FightData.Logic.Targets)
            {
                var targetDto = new TargetDto(target, log, ActorDetailsDto.BuildTargetData(log, target, usedSkills, usedBuffs, cr));
                logData.Targets.Add(targetDto);
            }
            //
            log.UpdateProgressWithCancellationCheck("HTML: building Skill/Buff dictionaries");
            Dictionary<string, IReadOnlyList<Buff>> persBuffDict = BuildPersonalBuffData(log, logData.PersBuffs, usedBuffs);
            Dictionary<string, IReadOnlyList<DamageModifier>> persDamageModDict = BuildPersonalDamageModData(log, logData.DmgModifiersPers, usedDamageMods);
            var allDamageMods = new HashSet<string>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                allDamageMods.UnionWith(actor.GetPresentDamageModifier(log));
            }
            var commonDamageModifiers = new List<DamageModifier>();
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(Source.Common, out IReadOnlyList<DamageModifier> list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        commonDamageModifiers.Add(dMod);
                        logData.DmgModifiersCommon.Add(dMod.ID);
                        usedDamageMods.Add(dMod);
                    }
                }
            }
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(Source.FightSpecific, out list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        commonDamageModifiers.Add(dMod);
                        logData.DmgModifiersCommon.Add(dMod.ID);
                        usedDamageMods.Add(dMod);
                    }
                }
            }
            var itemDamageModifiers = new List<DamageModifier>();
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(Source.Item, out list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        itemDamageModifiers.Add(dMod);
                        logData.DmgModifiersItem.Add(dMod.ID);
                        usedDamageMods.Add(dMod);
                    }
                }
            }
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(Source.Gear, out list))
            {
                foreach (DamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        itemDamageModifiers.Add(dMod);
                        logData.DmgModifiersItem.Add(dMod.ID);
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
            foreach (Buff boon in statistics.PresentSupbuffs)
            {
                logData.SupBuffs.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentDefbuffs)
            {
                logData.DefBuffs.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentGearbuffs)
            {
                logData.GearBuffs.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff boon in statistics.PresentFractalInstabilities)
            {
                logData.FractalInstabilities.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            //
            log.UpdateProgressWithCancellationCheck("HTML: building Phases");
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                var phaseDto = new PhaseDto(phase, phases, log, persBuffDict, commonDamageModifiers, itemDamageModifiers, persDamageModDict);
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
            logData.SingleGroup = log.PlayerList.Select(x => x.Group).Distinct().Count() == 1;
            logData.HasBreakbarDamage = log.CombatData.HasBreakbarDamageData;
            logData.NoMechanics = log.FightData.Logic.HasNoFightSpecificMechanics;
            if (log.LogData.LogErrors.Count > 0)
            {
                logData.LogErrors = new List<string>(log.LogData.LogErrors);
            }
            //
            SkillDto.AssembleSkills(usedSkills.Values, logData.SkillMap, log);
            DamageModDto.AssembleDamageModifiers(usedDamageMods, logData.DamageModMap);
            BuffDto.AssembleBoons(usedBuffs.Values, logData.BuffMap, log);
            MechanicDto.BuildMechanics(log.MechanicData.GetPresentMechanics(log, 0, log.FightData.FightEnd), logData.MechanicMap);
            return logData;
        }

    }
}
