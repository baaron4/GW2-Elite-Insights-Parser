using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIBuilders.HtmlModels.EXTBarrier;
using GW2EIBuilders.HtmlModels.EXTHealing;
using GW2EIBuilders.HtmlModels.HTMLActors;
using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLMetaData;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.Extensions;
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
        // Present buffs and damamge modifiers
        public List<long> Boons { get; } = new List<long>();
        public List<long> OffBuffs { get; } = new List<long>();
        public List<long> SupBuffs { get; } = new List<long>();
        public List<long> DefBuffs { get; } = new List<long>();
        public List<long> Debuffs { get; } = new List<long>();
        public List<long> GearBuffs { get; } = new List<long>();
        public List<long> Nourishments { get; } = new List<long>();
        public List<long> Enhancements { get; } = new List<long>();
        public List<long> OtherConsumables { get; } = new List<long>();
        public List<object[]> InstanceBuffs { get; } = new List<object[]>();
        public List<long> DmgModifiersItem { get; } = new List<long>();
        public List<long> DmgIncModifiersItem { get; } = new List<long>();
        public List<long> DmgModifiersCommon { get; } = new List<long>();
        public List<long> DmgIncModifiersCommon { get; } = new List<long>();
        public Dictionary<string, List<long>> DmgModifiersPers { get; } = new Dictionary<string, List<long>>();
        public Dictionary<string, List<long>> DmgIncModifiersPers { get; } = new Dictionary<string, List<long>>();
        public Dictionary<string, List<long>> PersBuffs { get; } = new Dictionary<string, List<long>>();
        public List<long> Conditions { get; } = new List<long>();
        // Dictionaries
        public Dictionary<string, SkillDto> SkillMap { get; } = new Dictionary<string, SkillDto>();
        public Dictionary<string, BuffDto> BuffMap { get; } = new Dictionary<string, BuffDto>();
        public Dictionary<string, DamageModDto> DamageModMap { get; } = new Dictionary<string, DamageModDto>();
        public Dictionary<string, DamageModDto> DamageIncModMap { get; } = new Dictionary<string, DamageModDto>();
        public List<MechanicDto> MechanicMap { get; set; } = new List<MechanicDto>();
        // Extra components
        public CombatReplayDto CrData { get; set; } = null;
        public ChartDataDto GraphData { get; set; } = null;
        public HealingStatsExtension HealingStatsExtension { get; set; } = null;
        public BarrierStatsExtension BarrierStatsExtension { get; set; } = null;
        // meta data
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
        public long TriggerID { get; set; }
        public long EncounterID { get; set; }
        public string Parser { get; set; }
        public string RecordedBy { get; set; }
        public string RecordedAccountBy { get; set; }
        public int FractalScale { get; set; }
        public List<string> UploadLinks { get; set; }
        public List<string> UsedExtensions { get; set; }
        public List<List<string>> PlayersRunningExtensions { get; set; }
        //
        private LogDataDto(ParsedEvtcLog log, bool light, Version parserVersion, string[] uploadLinks)
        {
            log.UpdateProgressWithCancellationCheck("HTML: building Meta Data");
            EncounterStart = log.LogData.LogStartStd;
            EncounterEnd = log.LogData.LogEndStd;
            ArcVersion = log.LogData.ArcVersion;
            EvtcVersion = log.LogData.EvtcVersion;
            Gw2Build = log.LogData.GW2Build;
            TriggerID = log.FightData.TriggerID;
            EncounterID = log.FightData.Logic.EncounterID;
            Parser = "Elite Insights " + parserVersion.ToString();
            RecordedBy = log.LogData.PoVName;
            RecordedAccountBy = log.LogData.PoVAccount;
            FractalScale = log.CombatData.GetFractalScaleEvent() != null ? log.CombatData.GetFractalScaleEvent().Scale : 0;
            UploadLinks = uploadLinks.ToList();
            if (log.LogData.UsedExtensions.Any())
            {
                UsedExtensions = new List<string>();
                PlayersRunningExtensions = new List<List<string>>();
                foreach (AbstractExtensionHandler extension in log.LogData.UsedExtensions)
                {
                    UsedExtensions.Add(extension.Name + " - " + extension.Version);
                    var set = new HashSet<string>();
                    if (log.LogData.PoV != null)
                    {
                        set.Add(log.FindActor(log.LogData.PoV).Character);
                        foreach (AgentItem agent in extension.RunningExtension)
                        {
                            set.Add(log.FindActor(agent).Character);
                        }
                    }
                    PlayersRunningExtensions.Add(set.ToList());
                }
            }

            EncounterDuration = log.FightData.DurationString;
            Success = log.FightData.Success;
            Wvw = log.FightData.Logic.Mode == FightLogic.ParseMode.WvW;
            Targetless = log.FightData.Logic.Targetless;
            FightName = log.FightData.FightName;
            FightIcon = log.FightData.Logic.Icon;
            LightTheme = light;
            SingleGroup = log.PlayerList.Select(x => x.Group).Distinct().Count() == 1;
            HasBreakbarDamage = log.CombatData.HasBreakbarDamageData;
            NoMechanics = log.FightData.Logic.HasNoFightSpecificMechanics;
            if (log.LogData.LogErrors.Count > 0)
            {
                LogErrors = new List<string>(log.LogData.LogErrors);
            }
        }

        private static Dictionary<Spec, IReadOnlyList<Buff>> BuildPersonalBuffData(ParsedEvtcLog log, Dictionary<string, List<long>> persBuffDict, Dictionary<long, Buff> usedBuffs)
        {
            var boonsBySpec = new Dictionary<Spec, IReadOnlyList<Buff>>();
            // Collect all personal buffs by spec
            foreach (KeyValuePair<Spec, List<AbstractSingleActor>> pair in log.FriendliesListBySpec)
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
            foreach (KeyValuePair<Spec, IReadOnlyList<Buff>> pair in boonsBySpec)
            {
                persBuffDict[pair.Key.ToString()] = new List<long>();
                foreach (Buff boon in pair.Value)
                {
                    persBuffDict[pair.Key.ToString()].Add(boon.ID);
                    usedBuffs[boon.ID] = boon;
                }
            }
            return boonsBySpec;
        }

        private static Dictionary<Spec, IReadOnlyList<OutgoingDamageModifier>> BuildPersonalOutgoingDamageModData(ParsedEvtcLog log, Dictionary<string, List<long>> dgmModDict, HashSet<OutgoingDamageModifier> usedDamageMods)
        {
            var damageModBySpecs = new Dictionary<Spec, IReadOnlyList<OutgoingDamageModifier>>();
            // Collect all personal damage mods by spec
            foreach (KeyValuePair<Spec, List<AbstractSingleActor>> pair in log.FriendliesListBySpec)
            {
                var specDamageModsName = new HashSet<string>(log.DamageModifiers.GetOutgoingModifiersPerSpec(pair.Key).Select(x => x.Name));
                var damageModsToUse = new HashSet<OutgoingDamageModifier>();
                foreach (AbstractSingleActor actor in pair.Value)
                {
                    var presentDamageMods = new HashSet<string>(actor.GetPresentOutgoingDamageModifier(log).Intersect(specDamageModsName));
                    foreach (string name in presentDamageMods)
                    {
                        damageModsToUse.Add(log.DamageModifiers.OutgoingDamageModifiersByName[name]);
                    }
                }
                damageModBySpecs[pair.Key] = damageModsToUse.ToList();
            }
            foreach (KeyValuePair<Spec, IReadOnlyList<OutgoingDamageModifier>> pair in damageModBySpecs)
            {
                dgmModDict[pair.Key.ToString()] = new List<long>();
                foreach (OutgoingDamageModifier mod in pair.Value)
                {
                    dgmModDict[pair.Key.ToString()].Add(mod.ID);
                    usedDamageMods.Add(mod);
                }
            }
            return damageModBySpecs;
        }

        private static Dictionary<Spec, IReadOnlyList<IncomingDamageModifier>> BuildPersonalIncomingDamageModData(ParsedEvtcLog log, Dictionary<string, List<long>> dgmModDict, HashSet<IncomingDamageModifier> usedDamageMods)
        {
            var damageModBySpecs = new Dictionary<Spec, IReadOnlyList<IncomingDamageModifier>>();
            // Collect all personal damage mods by spec
            foreach (KeyValuePair<Spec, List<AbstractSingleActor>> pair in log.FriendliesListBySpec)
            {
                var specDamageModsName = new HashSet<string>(log.DamageModifiers.GetIncomingModifiersPerSpec(pair.Key).Select(x => x.Name));
                var damageModsToUse = new HashSet<IncomingDamageModifier>();
                foreach (AbstractSingleActor actor in pair.Value)
                {
                    var presentDamageMods = new HashSet<string>(actor.GetPresentIncomingDamageModifier(log).Intersect(specDamageModsName));
                    foreach (string name in presentDamageMods)
                    {
                        damageModsToUse.Add(log.DamageModifiers.IncomingDamageModifiersByName[name]);
                    }
                }
                damageModBySpecs[pair.Key] = damageModsToUse.ToList();
            }
            foreach (KeyValuePair<Spec, IReadOnlyList<IncomingDamageModifier>> pair in damageModBySpecs)
            {
                dgmModDict[pair.Key.ToString()] = new List<long>();
                foreach (IncomingDamageModifier mod in pair.Value)
                {
                    dgmModDict[pair.Key.ToString()].Add(mod.ID);
                    usedDamageMods.Add(mod);
                }
            }
            return damageModBySpecs;
        }

        private void BuildBuffDictionaries(ParsedEvtcLog log, Dictionary<long, Buff> usedBuffs)
        {
            StatisticsHelper statistics = log.StatisticsHelper;
            foreach (Buff boon in statistics.PresentBoons)
            {
                Boons.Add(boon.ID);
                usedBuffs[boon.ID] = boon;
            }
            foreach (Buff condition in statistics.PresentConditions)
            {
                Conditions.Add(condition.ID);
                usedBuffs[condition.ID] = condition;
            }
            foreach (Buff offBuff in statistics.PresentOffbuffs)
            {
                OffBuffs.Add(offBuff.ID);
                usedBuffs[offBuff.ID] = offBuff;
            }
            foreach (Buff supBuff in statistics.PresentSupbuffs)
            {
                SupBuffs.Add(supBuff.ID);
                usedBuffs[supBuff.ID] = supBuff;
            }
            foreach (Buff defBuff in statistics.PresentDefbuffs)
            {
                DefBuffs.Add(defBuff.ID);
                usedBuffs[defBuff.ID] = defBuff;
            }
            foreach (Buff debuff in statistics.PresentDebuffs)
            {
                Debuffs.Add(debuff.ID);
                usedBuffs[debuff.ID] = debuff;
            }
            foreach (Buff gearBuff in statistics.PresentGearbuffs)
            {
                GearBuffs.Add(gearBuff.ID);
                usedBuffs[gearBuff.ID] = gearBuff;
            }
            foreach (Buff nourishment in statistics.PresentNourishements)
            {
                Nourishments.Add(nourishment.ID);
                usedBuffs[nourishment.ID] = nourishment;
            }
            foreach (Buff enhancement in statistics.PresentEnhancements)
            {
                Enhancements.Add(enhancement.ID);
                usedBuffs[enhancement.ID] = enhancement;
            }
            foreach (Buff otherConsumables in statistics.PresentOtherConsumables)
            {
                OtherConsumables.Add(otherConsumables.ID);
                usedBuffs[otherConsumables.ID] = otherConsumables;
            }
            foreach ((Buff instanceBuff, int stack) in log.FightData.Logic.GetInstanceBuffs(log))
            {
                InstanceBuffs.Add(new object[] { instanceBuff.ID, stack });
                usedBuffs[instanceBuff.ID] = instanceBuff;
            }
        }

        private void BuildOutgoingDamageModDictionaries(ParsedEvtcLog log, HashSet<OutgoingDamageModifier> usedDamageMods, 
            HashSet<string> allDamageMods, List<OutgoingDamageModifier> commonDamageModifiers, List<OutgoingDamageModifier> itemDamageModifiers)
        {
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                allDamageMods.UnionWith(actor.GetPresentOutgoingDamageModifier(log));
            }
            if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.Common, out IReadOnlyList<OutgoingDamageModifier> list))
            {
                foreach (OutgoingDamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        commonDamageModifiers.Add(dMod);
                        DmgModifiersCommon.Add(dMod.ID);
                        usedDamageMods.Add(dMod);
                    }
                }
            }
            if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.FightSpecific, out list))
            {
                foreach (OutgoingDamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        commonDamageModifiers.Add(dMod);
                        DmgModifiersCommon.Add(dMod.ID);
                        usedDamageMods.Add(dMod);
                    }
                }
            }
            if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.Item, out list))
            {
                foreach (OutgoingDamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        itemDamageModifiers.Add(dMod);
                        DmgModifiersItem.Add(dMod.ID);
                        usedDamageMods.Add(dMod);
                    }
                }
            }
            if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.Gear, out list))
            {
                foreach (OutgoingDamageModifier dMod in list)
                {
                    if (allDamageMods.Contains(dMod.Name))
                    {
                        itemDamageModifiers.Add(dMod);
                        DmgModifiersItem.Add(dMod.ID);
                        usedDamageMods.Add(dMod);
                    }
                }
            }
        }

        private void BuildIncomingDamageModDictionaries(ParsedEvtcLog log, HashSet<IncomingDamageModifier> usedIncDamageMods,
            HashSet<string> allIncDamageMods, List<IncomingDamageModifier> commonIncDamageModifiers, List<IncomingDamageModifier> itemIncDamageModifiers)
        {
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                allIncDamageMods.UnionWith(actor.GetPresentIncomingDamageModifier(log));
            }
            if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.Common, out IReadOnlyList<IncomingDamageModifier> list))
            {
                foreach (IncomingDamageModifier dMod in list)
                {
                    if (allIncDamageMods.Contains(dMod.Name))
                    {
                        commonIncDamageModifiers.Add(dMod);
                        DmgIncModifiersCommon.Add(dMod.ID);
                        usedIncDamageMods.Add(dMod);
                    }
                }
            }
            if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.FightSpecific, out list))
            {
                foreach (IncomingDamageModifier dMod in list)
                {
                    if (allIncDamageMods.Contains(dMod.Name))
                    {
                        commonIncDamageModifiers.Add(dMod);
                        DmgIncModifiersCommon.Add(dMod.ID);
                        usedIncDamageMods.Add(dMod);
                    }
                }
            }
            if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.Item, out list))
            {
                foreach (IncomingDamageModifier dMod in list)
                {
                    if (allIncDamageMods.Contains(dMod.Name))
                    {
                        commonIncDamageModifiers.Add(dMod);
                        DmgIncModifiersItem.Add(dMod.ID);
                        usedIncDamageMods.Add(dMod);
                    }
                }
            }
            if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.Gear, out list))
            {
                foreach (IncomingDamageModifier dMod in list)
                {
                    if (allIncDamageMods.Contains(dMod.Name))
                    {
                        commonIncDamageModifiers.Add(dMod);
                        DmgIncModifiersItem.Add(dMod.ID);
                        usedIncDamageMods.Add(dMod);
                    }
                }
            }
        }

        public static LogDataDto BuildLogData(ParsedEvtcLog log, bool cr, bool light, Version parserVersion, string[] uploadLinks)
        {

            var usedBuffs = new Dictionary<long, Buff>();
            var usedDamageMods = new HashSet<OutgoingDamageModifier>();
            var usedIncDamageMods = new HashSet<IncomingDamageModifier>();
            var usedSkills = new Dictionary<long, SkillItem>();
            log.UpdateProgressWithCancellationCheck("HTML: building Log Data");
            var logData = new LogDataDto(log, light, parserVersion, uploadLinks);
            if (cr)
            {
                log.UpdateProgressWithCancellationCheck("HTML: building Combat Replay");
                logData.CrData = new CombatReplayDto(log);
            }
            log.UpdateProgressWithCancellationCheck("HTML: building Graph Data");
            logData.GraphData = new ChartDataDto(log);
            log.UpdateProgressWithCancellationCheck("HTML: building Players");
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                logData.HasCommander = logData.HasCommander || (actor is Player p && p.IsCommander(log));
                logData.Players.Add(new PlayerDto(actor, log, ActorDetailsDto.BuildPlayerData(log, actor, usedSkills, usedBuffs)));
            }

            log.UpdateProgressWithCancellationCheck("HTML: building Enemies");
            foreach (AbstractSingleActor enemy in log.MechanicData.GetEnemyList(log, log.FightData.FightStart, log.FightData.FightEnd))
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
            log.UpdateProgressWithCancellationCheck("HTML: building Skill/Buff/Damage Modifier dictionaries");
            Dictionary<Spec, IReadOnlyList<Buff>> persBuffDict = BuildPersonalBuffData(log, logData.PersBuffs, usedBuffs);
            Dictionary<Spec, IReadOnlyList<OutgoingDamageModifier>> persOutDamageModDict = BuildPersonalOutgoingDamageModData(log, logData.DmgModifiersPers, usedDamageMods);
            Dictionary<Spec, IReadOnlyList<IncomingDamageModifier>> persIncDamageModDict = BuildPersonalIncomingDamageModData(log, logData.DmgIncModifiersPers, usedIncDamageMods);
            var allOutDamageMods = new HashSet<string>();
            var commonOutDamageModifiers = new List<OutgoingDamageModifier>();
            var itemOutDamageModifiers = new List<OutgoingDamageModifier>();
            var allIncDamageMods = new HashSet<string>();
            var commonIncDamageModifiers = new List<IncomingDamageModifier>();
            var itemIncDamageModifiers = new List<IncomingDamageModifier>();
            logData.BuildBuffDictionaries(log, usedBuffs);
            logData.BuildOutgoingDamageModDictionaries(log, usedDamageMods,
                allOutDamageMods, commonOutDamageModifiers, itemOutDamageModifiers);
            logData.BuildIncomingDamageModDictionaries(log, usedIncDamageMods,
                allIncDamageMods, commonIncDamageModifiers, itemIncDamageModifiers);
            //
            log.UpdateProgressWithCancellationCheck("HTML: building Phases");
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                var phaseDto = new PhaseDto(phase, phases, log, persBuffDict, 
                    commonOutDamageModifiers, itemOutDamageModifiers, persOutDamageModDict,
                    commonIncDamageModifiers, itemIncDamageModifiers, persIncDamageModDict
                    );
                logData.Phases.Add(phaseDto);
            }
            //
            if (log.CombatData.HasEXTHealing)
            {
                log.UpdateProgressWithCancellationCheck("HTML: building Healing Extension");
                logData.HealingStatsExtension = new HealingStatsExtension(log, usedSkills, usedBuffs);
                if (log.CombatData.HasEXTBarrier)
                {
                    log.UpdateProgressWithCancellationCheck("HTML: building Barrier Extension");
                    logData.BarrierStatsExtension = new BarrierStatsExtension(log, usedSkills, usedBuffs);
                }
            }
            //
            SkillDto.AssembleSkills(usedSkills.Values, logData.SkillMap, log);
            DamageModDto.AssembleDamageModifiers(usedDamageMods, logData.DamageModMap);
            DamageModDto.AssembleDamageModifiers(usedIncDamageMods, logData.DamageIncModMap);
            BuffDto.AssembleBoons(usedBuffs.Values, logData.BuffMap, log);
            MechanicDto.BuildMechanics(log.MechanicData.GetPresentMechanics(log, log.FightData.FightStart, log.FightData.FightEnd), logData.MechanicMap);
            return logData;
        }

    }
}
