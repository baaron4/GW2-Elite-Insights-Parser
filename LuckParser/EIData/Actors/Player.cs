using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LuckParser.Logic;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData;
using LuckParser.Parser.ParsedData.CombatEvents;
using static LuckParser.EIData.Buff;
using static LuckParser.Models.Statistics;

namespace LuckParser.EIData
{
    public class Player : AbstractMasterActor
    {
        // Fields
        public string Account { get; protected set; }
        public int Group { get; }

        private List<Consumable> _consumeList;
        private List<DeathRecap> _deathRecaps;
        private Dictionary<string, List<DamageModifierData>> _damageModifiers;
        private HashSet<string> _presentDamageModifiers;
        private Dictionary<Target, Dictionary<string, List<DamageModifierData>>> _damageModifiersTargets;
        // statistics
        private Dictionary<Target, List<FinalDPS>> _dpsTarget;
        private Dictionary<Target, List<FinalStats>> _statsTarget;
        private List<FinalStatsAll> _statsAll;
        private List<FinalDefenses> _defenses;
        private List<FinalSupport> _support;
        private List<Dictionary<long, FinalBuffs>> _selfBuffs;
        private List<Dictionary<long, FinalBuffs>> _groupBuffs;
        private List<Dictionary<long, FinalBuffs>> _offGroupBuffs;
        private List<Dictionary<long, FinalBuffs>> _squadBuffs;
        private List<Dictionary<long, FinalBuffs>> _selfActiveBuffs;
        private List<Dictionary<long, FinalBuffs>> _groupActiveBuffs;
        private List<Dictionary<long, FinalBuffs>> _offGroupActiveBuffs;
        private List<Dictionary<long, FinalBuffs>> _squadActiveBuffs;
        //weaponslist
        private string[] _weaponsArray;

        // Constructors
        public Player(AgentItem agent, bool noSquad) : base(agent)
        {
            string[] name = agent.Name.Split('\0');
            if (name.Length < 2)
            {
                throw new InvalidOperationException("Name problem on Player");
            }
            if (name[1].Length == 0 || name[2].Length == 0 || Character.Contains("-"))
            {
                throw new InvalidOperationException("Missing Group on Player");
            }
            Account = name[1].TrimStart(':');
            Group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
            IsFakeActor = Account == "Conjured Sword";
        }

        // Public methods
        public long[] GetCleansesNotSelf(ParsedLog log, PhaseData phase)
        {
            long[] cleanse = { 0, 0 };
            foreach (long id in log.Buffs.BuffsByNature[Buff.BuffNature.Condition].Select(x => x.ID))
            {
                var bevts = log.CombatData.GetBoonData(id).Where(x => x is BuffRemoveAllEvent && x.Time >= phase.Start && x.Time <= phase.End && x.By == AgentItem && log.PlayerAgents.Contains(x.To) && x.To != AgentItem).Select(x => x as BuffRemoveAllEvent).ToList();
                cleanse[0] += bevts.Count;
                cleanse[1] += bevts.Sum(x => Math.Max(x.RemovedDuration, log.FightData.FightDuration));
            }
            return cleanse;
        }
        public long[] GetCleansesSelf(ParsedLog log, PhaseData phase)
        {
            long[] cleanse = { 0, 0 };
            foreach (long id in log.Buffs.BuffsByNature[Buff.BuffNature.Condition].Select(x => x.ID))
            {
                var bevts = log.CombatData.GetBoonData(id).Where(x => x is BuffRemoveAllEvent && x.Time >= phase.Start && x.Time <= phase.End && x.By == AgentItem && x.To == AgentItem).Select(x => x as BuffRemoveAllEvent).ToList();
                cleanse[0] += bevts.Count;
                cleanse[1] += bevts.Sum(x => Math.Max(x.RemovedDuration, log.FightData.FightDuration));
            }
            return cleanse;
        }

        public long[] GetBoonStrips(ParsedLog log, PhaseData phase)
        {
            long[] strips = { 0, 0 };
            foreach (long id in log.Buffs.BuffsByNature[Buff.BuffNature.Boon].Select(x => x.ID))
            {
                var bevts = log.CombatData.GetBoonData(id).Where(x => x is BuffRemoveAllEvent && x.Time >= phase.Start && x.Time <= phase.End && x.By == AgentItem && !log.PlayerAgents.Contains(x.To) && !log.PlayerAgents.Contains(x.To.Master)).Select(x => x as BuffRemoveAllEvent).ToList();
                strips[0] += bevts.Count;
                strips[1] += bevts.Sum(x => Math.Max(x.RemovedDuration, log.FightData.FightDuration));
            }
            return strips;
        }

        public long[] GetReses(ParsedLog log, long start, long end)
        {
            List<AbstractCastEvent> cls = GetCastLogs(log, start, end);
            long[] reses = { 0, 0 };
            foreach (AbstractCastEvent cl in cls)
            {
                if (cl.SkillId == SkillItem.ResurrectId)
                {
                    reses[0]++;
                    reses[1] += cl.ActualDuration;
                }
            }
            return reses;
        }

        public FinalDPS GetDPSTarget(ParsedLog log, int phaseIndex, Target target)
        {
            if (_dpsTarget == null)
            {
                _dpsTarget = new Dictionary<Target, List<FinalDPS>>();
                foreach (Target tar in log.FightData.Logic.Targets)
                {
                    _dpsTarget[tar] = new List<FinalDPS>();
                    foreach (PhaseData phase in log.FightData.GetPhases(log))
                    {
                        _dpsTarget[tar].Add(GetFinalDPS(log, phase, tar));
                    }
                }
            }
            if (target == null)
            {
                return GetDPSAll(log, phaseIndex);
            }
            return _dpsTarget[target][phaseIndex];
        }

        public List<FinalDPS> GetDPSTarget(ParsedLog log, Target target)
        {
            if (_dpsTarget == null)
            {
                _dpsTarget = new Dictionary<Target, List<FinalDPS>>();
                foreach (Target tar in log.FightData.Logic.Targets)
                {
                    _dpsTarget[tar] = new List<FinalDPS>();
                    foreach (PhaseData phase in log.FightData.GetPhases(log))
                    {
                        _dpsTarget[tar].Add(GetFinalDPS(log, phase, tar));
                    }
                }
            }
            if (target == null)
            {
                return GetDPSAll(log);
            }
            return _dpsTarget[target];
        }

        public FinalStatsAll GetStatsAll(ParsedLog log, int phaseIndex)
        {
            if (_statsAll == null)
            {
                SetStats(log);
            }
            return _statsAll[phaseIndex];
        }

        public FinalStats GetStatsTarget(ParsedLog log, int phaseIndex, Target target)
        {
            if (_statsTarget == null)
            {
                SetStats(log);
            }
            if (target == null)
            {
                return GetStatsAll(log, phaseIndex);
            }
            return _statsTarget[target][phaseIndex];
        }

        public List<FinalStatsAll> GetStatsAll(ParsedLog log)
        {
            if (_statsAll == null)
            {
                SetStats(log);
            }
            return _statsAll;
        }

        public List<FinalStats> GetStatsTarget(ParsedLog log, Target target)
        {
            if (_statsTarget == null)
            {
                SetStats(log);
            }
            if (target == null)
            {
                return new List<FinalStats>(GetStatsAll(log));
            }
            return _statsTarget[target];
        }

        private static void FillFinalStats(List<AbstractDamageEvent> dls, FinalStats final, Dictionary<Target, FinalStats> targetsFinal)
        {
            var nonCritable = new HashSet<long>
                    {
                        9292,
                        5492,
                        13014,
                        30770,
                        52370
                    };
            // (x - 1) / x
            foreach (AbstractDamageEvent dl in dls)
            {
                if (!(dl is NonDirectDamageEvent))
                {
                    foreach (KeyValuePair<Target, FinalStats> pair in targetsFinal)
                    {
                        Target target = pair.Key;
                        if (dl.To == target.AgentItem)
                        {
                            FinalStats targetFinal = pair.Value;
                            if (dl.HasCrit)
                            {
                                targetFinal.CriticalCount++;
                                targetFinal.CriticalDmg += dl.Damage;
                            }

                            if (dl.IsFlanking)
                            {
                                targetFinal.FlankingCount++;
                            }

                            if (dl.HasGlanced)
                            {
                                targetFinal.GlanceCount++;
                            }

                            if (dl.IsBlind)
                            {
                                targetFinal.Missed++;
                            }
                            if (dl.HasInterrupted)
                            {
                                targetFinal.Interrupts++;
                            }

                            if (dl.IsAbsorbed)
                            {
                                targetFinal.Invulned++;
                            }
                            targetFinal.DirectDamageCount++;
                            if (!nonCritable.Contains(dl.SkillId))
                            {
                                targetFinal.CritableDirectDamageCount++;
                            }
                        }
                    }
                    if (dl.HasCrit)
                    {
                        final.CriticalCount++;
                        final.CriticalDmg += dl.Damage;
                    }

                    if (dl.IsFlanking)
                    {
                        final.FlankingCount++;
                    }

                    if (dl.HasGlanced)
                    {
                        final.GlanceCount++;
                    }

                    if (dl.IsBlind)
                    {
                        final.Missed++;
                    }
                    if (dl.HasInterrupted)
                    {
                        final.Interrupts++;
                    }

                    if (dl.IsAbsorbed)
                    {
                        final.Invulned++;
                    }
                    final.DirectDamageCount++;
                    if (!nonCritable.Contains(dl.SkillId))
                    {
                        final.CritableDirectDamageCount++;
                    }
                }
            }
        }

        private void SetStats(ParsedLog log)
        {
            int phaseIndex = -1;
            _statsAll = new List<FinalStatsAll>();
            _statsTarget = new Dictionary<Target, List<FinalStats>>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                phaseIndex++;
                var targetDict = new Dictionary<Target, FinalStats>();
                foreach (Target target in log.FightData.Logic.Targets)
                {
                    if (!_statsTarget.ContainsKey(target))
                    {
                        _statsTarget[target] = new List<FinalStats>();
                    }
                    _statsTarget[target].Add(new FinalStats());
                    targetDict[target] = _statsTarget[target].Last();
                }
                var final = new FinalStatsAll();
                FillFinalStats(GetJustPlayerDamageLogs(null, log, phase), final, targetDict);
                _statsAll.Add(final);
                // If conjured sword, stop
                if (IsFakeActor)
                {
                    continue;
                }
                foreach (AbstractCastEvent cl in GetCastLogs(log, phase.Start, phase.End))
                {
                    if (cl.Interrupted)
                    {
                        final.Wasted++;
                        final.TimeWasted += cl.ActualDuration;
                    }
                    if (cl.ReducedAnimation)
                    {
                        if (cl.ActualDuration < cl.ExpectedDuration)
                        {
                            final.Saved++;
                            final.TimeSaved += cl.ExpectedDuration - cl.ActualDuration;
                        }
                    }
                    if (cl.SkillId == SkillItem.WeaponSwapId)
                    {
                        final.SwapCount++;
                    }
                }
                final.TimeSaved = Math.Round(final.TimeSaved / 1000.0, GeneralHelper.TimeDigit);
                final.TimeWasted = Math.Round(final.TimeWasted / 1000.0, GeneralHelper.TimeDigit);

                double avgBoons = 0;
                foreach (long duration in GetBuffPresence(log, phaseIndex).Where(x => log.Buffs.BuffsByIds[x.Key].Nature == Buff.BuffNature.Boon).Select(x => x.Value))
                {
                    avgBoons += duration;
                }
                final.AvgBoons = Math.Round(avgBoons / phase.DurationInMS, GeneralHelper.BoonDigit);
                long activeDuration = phase.GetPlayerActiveDuration(this, log);
                final.AvgActiveBoons = activeDuration > 0 ? Math.Round(avgBoons / activeDuration, GeneralHelper.BoonDigit) : 0.0;

                double avgCondis = 0;
                foreach (long duration in GetBuffPresence(log, phaseIndex).Where(x => log.Buffs.BuffsByIds[x.Key].Nature == Buff.BuffNature.Condition).Select(x => x.Value))
                {
                    avgCondis += duration;
                }
                final.AvgConditions = Math.Round(avgCondis / phase.DurationInMS, GeneralHelper.BoonDigit);
                final.AvgActiveConditions = activeDuration > 0 ? Math.Round(avgCondis / activeDuration, GeneralHelper.BoonDigit) : 0.0;

                if (log.CombatData.HasMovementData)
                {
                    if (CombatReplay == null)
                    {
                        InitCombatReplay(log);
                    }
                    var positions = CombatReplay.PolledPositions.Where(x => x.Time >= phase.Start && x.Time <= phase.End).ToList();
                    List<Point3D> stackCenterPositions = log.Statistics.GetStackCenterPositions(log);
                    int offset = CombatReplay.PolledPositions.Count(x => x.Time < phase.Start);
                    if (positions.Count > 1)
                    {
                        var distances = new List<float>();
                        for (int time = 0; time < positions.Count; time++)
                        {

                            float deltaX = positions[time].X - stackCenterPositions[time + offset].X;
                            float deltaY = positions[time].Y - stackCenterPositions[time + offset].Y;
                            //float deltaZ = positions[time].Z - StackCenterPositions[time].Z;


                            distances.Add((float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
                        }
                        final.StackDist = distances.Sum() / distances.Count;
                    }
                    else
                    {
                        final.StackDist = -1;
                    }
                }
            }
        }

        public FinalDefenses GetDefenses(ParsedLog log, int phaseIndex)
        {
            if (_defenses == null)
            {
                SetDefenses(log);
            }
            return _defenses[phaseIndex];
        }

        public List<FinalDefenses> GetDefenses(ParsedLog log)
        {
            if (_defenses == null)
            {
                SetDefenses(log);
            }
            return _defenses;
        }

        private void SetDefenses(ParsedLog log)
        {
            var dead = new List<(long start, long end)>();
            var down = new List<(long start, long end)>();
            var dc = new List<(long start, long end)>();
            AgentItem.GetAgentStatus(dead, down, dc, log);
            _defenses = new List<FinalDefenses>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                var final = new FinalDefenses();
                _defenses.Add(final);
                long start = phase.Start;
                long end = phase.End;
                List<AbstractDamageEvent> damageLogs = GetDamageTakenLogs(null, log, start, end);
                //List<DamageLog> healingLogs = player.getHealingReceivedLogs(log, phase.getStart(), phase.getEnd());

                final.DamageTaken = damageLogs.Sum(x => (long)x.Damage);
                //final.allHealReceived = healingLogs.Sum(x => x.getDamage());
                final.BlockedCount = damageLogs.Count(x => x.IsBlocked);
                final.InvulnedCount = 0;
                final.DamageInvulned = 0;
                final.EvadedCount = damageLogs.Count(x => x.IsEvaded);
                final.DodgeCount = GetCastLogs(log, start, end).Count(x => x.SkillId == SkillItem.DodgeId || x.SkillId == SkillItem.MirageCloakDodgeId);
                final.DamageBarrier = damageLogs.Sum(x => x.ShieldDamage);
                final.InterruptedCount = damageLogs.Count(x => x.HasInterrupted);
                foreach (AbstractDamageEvent dl in damageLogs.Where(x => x.IsAbsorbed))
                {
                    final.InvulnedCount++;
                    final.DamageInvulned += dl.Damage;
                }

                //		
                final.DownCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DownId).Count(x => x.Actor == this && x.Time >= start && x.Time <= end);
                final.DeadCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DeathId).Count(x => x.Actor == this && x.Time >= start && x.Time <= end);
                final.DcCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DCId).Count(x => x.Actor == this && x.Time >= start && x.Time <= end);

                final.DownDuration = (int)down.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
                final.DeadDuration = (int)dead.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
                final.DcDuration = (int)dc.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            }
        }

        public FinalSupport GetSupport(ParsedLog log, int phaseIndex)
        {
            if (_support == null)
            {
                SetSupport(log);
            }
            return _support[phaseIndex];
        }

        public List<FinalSupport> GetSupport(ParsedLog log)
        {
            if (_support == null)
            {
                SetSupport(log);
            }
            return _support;
        }

        private void SetSupport(ParsedLog log)
        {
            _support = new List<FinalSupport>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                var final = new FinalSupport();
                _support.Add(final);
                PhaseData phase = phases[phaseIndex];

                long[] resArray = GetReses(log, phase.Start, phase.End);
                long[] cleanseArray = GetCleansesNotSelf(log, phase);
                long[] cleanseSelfArray = GetCleansesSelf(log, phase);
                long[] boonStrips = GetBoonStrips(log, phase);
                //List<DamageLog> healingLogs = player.getHealingLogs(log, phase.getStart(), phase.getEnd());
                //final.allHeal = healingLogs.Sum(x => x.getDamage());
                final.Resurrects = resArray[0];
                final.ResurrectTime = resArray[1] / 1000.0;
                final.CondiCleanse = cleanseArray[0];
                final.CondiCleanseTime = cleanseArray[1] / 1000.0;
                final.CondiCleanseSelf = cleanseSelfArray[0];
                final.CondiCleanseTimeSelf = cleanseSelfArray[1] / 1000.0;
                final.BoonStrips = boonStrips[0];
                final.BoonStripsTime = boonStrips[1] / 1000.0;
            }
        }

        public Dictionary<long, FinalBuffs> GetBuffs(ParsedLog log, int phaseIndex, BuffEnum type)
        {
            if (_selfBuffs == null)
            {
                SetBuffs(log);
            }
            switch (type)
            {
                case BuffEnum.Group:
                    return _groupBuffs[phaseIndex];
                case BuffEnum.OffGroup:
                    return _offGroupBuffs[phaseIndex];
                case BuffEnum.Squad:
                    return _squadBuffs[phaseIndex];
                case BuffEnum.Self:
                default:
                    return _selfBuffs[phaseIndex];
            }
        }

        public List<Dictionary<long, FinalBuffs>> GetBuffs(ParsedLog log, BuffEnum type)
        {
            if (_selfBuffs == null)
            {
                SetBuffs(log);
            }
            switch (type)
            {
                case BuffEnum.Group:
                    return _groupBuffs;
                case BuffEnum.OffGroup:
                    return _offGroupBuffs;
                case BuffEnum.Squad:
                    return _squadBuffs;
                case BuffEnum.Self:
                default:
                    return _selfBuffs;
            }
        }

        public Dictionary<long, FinalBuffs> GetActiveBuffs(ParsedLog log, int phaseIndex, BuffEnum type)
        {
            if (_selfActiveBuffs == null)
            {
                SetBuffs(log);
            }
            switch (type)
            {
                case BuffEnum.Group:
                    return _groupActiveBuffs[phaseIndex];
                case BuffEnum.OffGroup:
                    return _offGroupActiveBuffs[phaseIndex];
                case BuffEnum.Squad:
                    return _squadActiveBuffs[phaseIndex];
                case BuffEnum.Self:
                default:
                    return _selfActiveBuffs[phaseIndex];
            }
        }

        public List<Dictionary<long, FinalBuffs>> GetActiveBuffs(ParsedLog log, BuffEnum type)
        {
            if (_selfActiveBuffs == null)
            {
                SetBuffs(log);
            }
            switch (type)
            {
                case BuffEnum.Group:
                    return _groupActiveBuffs;
                case BuffEnum.OffGroup:
                    return _offGroupActiveBuffs;
                case BuffEnum.Squad:
                    return _squadActiveBuffs;
                case BuffEnum.Self:
                default:
                    return _selfActiveBuffs;
            }
        }

        private (List<Dictionary<long, FinalBuffs>>, List<Dictionary<long, FinalBuffs>>) GetBoonsForPlayers(List<Player> playerList, ParsedLog log)
        {
            var uptimesByPhase = new List<Dictionary<long, FinalBuffs>>();
            var uptimesActiveByPhase = new List<Dictionary<long, FinalBuffs>>();

            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                PhaseData phase = phases[phaseIndex];
                long phaseDuration = phase.DurationInMS;

                var boonDistributions = new Dictionary<Player, BuffDistributionDictionary>();
                foreach (Player p in playerList)
                {
                    boonDistributions[p] = p.GetBoonDistribution(log, phaseIndex);
                }

                var boonsToTrack = new HashSet<Buff>(boonDistributions.SelectMany(x => x.Value).Select(x => log.Buffs.BuffsByIds[x.Key]));

                var final =
                    new Dictionary<long, FinalBuffs>();
                var finalActive =
                    new Dictionary<long, FinalBuffs>();

                foreach (Buff boon in boonsToTrack)
                {
                    double totalGeneration = 0;
                    double totalOverstack = 0;
                    double totalWasted = 0;
                    double totalUnknownExtension = 0;
                    double totalExtension = 0;
                    double totalExtended = 0;
                    //
                    double totalActiveGeneration = 0;
                    double totalActiveOverstack = 0;
                    double totalActiveWasted = 0;
                    double totalActiveUnknownExtension = 0;
                    double totalActiveExtension = 0;
                    double totalActiveExtended = 0;
                    bool hasGeneration = false;
                    int activePlayerCount = 0;
                    foreach (KeyValuePair<Player, BuffDistributionDictionary> pair in boonDistributions)
                    {
                        BuffDistributionDictionary boons = pair.Value;
                        long playerActiveDuration = phase.GetPlayerActiveDuration(pair.Key, log);
                        if (boons.ContainsKey(boon.ID))
                        {
                            hasGeneration = hasGeneration || boons.HasSrc(boon.ID, AgentItem);
                            double generation = boons.GetGeneration(boon.ID, AgentItem);
                            double overstack = boons.GetOverstack(boon.ID, AgentItem);
                            double wasted = boons.GetWaste(boon.ID, AgentItem);
                            double unknownExtension = boons.GetUnknownExtension(boon.ID, AgentItem);
                            double extension = boons.GetExtension(boon.ID, AgentItem);
                            double extended = boons.GetExtended(boon.ID, AgentItem);

                            totalGeneration += generation;
                            totalOverstack += overstack;
                            totalWasted += wasted;
                            totalUnknownExtension += unknownExtension;
                            totalExtension += extension;
                            totalExtended += extended;
                            if (playerActiveDuration > 0)
                            {
                                activePlayerCount++;
                                totalActiveGeneration += generation / playerActiveDuration;
                                totalActiveOverstack += overstack / playerActiveDuration;
                                totalActiveWasted += wasted / playerActiveDuration;
                                totalActiveUnknownExtension += unknownExtension / playerActiveDuration;
                                totalActiveExtension += extension / playerActiveDuration;
                                totalActiveExtended += extended / playerActiveDuration;
                            }
                        }
                    }
                    totalGeneration /= phaseDuration;
                    totalOverstack /= phaseDuration;
                    totalWasted /= phaseDuration;
                    totalUnknownExtension /= phaseDuration;
                    totalExtension /= phaseDuration;
                    totalExtended /= phaseDuration;

                    if (hasGeneration)
                    {
                        var uptime = new FinalBuffs();
                        var uptimeActive = new FinalBuffs();
                        final[boon.ID] = uptime;
                        finalActive[boon.ID] = uptimeActive;
                        if (boon.Type == Buff.BuffType.Duration)
                        {
                            uptime.Generation = Math.Round(100.0 * totalGeneration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Overstack = Math.Round(100.0 * (totalOverstack + totalGeneration) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Wasted = Math.Round(100.0 * (totalWasted) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.UnknownExtended = Math.Round(100.0 * (totalUnknownExtension) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.ByExtension = Math.Round(100.0 * (totalExtension) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Extended = Math.Round(100.0 * (totalExtended) / playerList.Count, GeneralHelper.BoonDigit);
                            //
                            if (activePlayerCount > 0)
                            {
                                uptimeActive.Generation = Math.Round(100.0 * totalActiveGeneration / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.Overstack = Math.Round(100.0 * (totalActiveOverstack + totalActiveGeneration) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.Wasted = Math.Round(100.0 * (totalActiveWasted) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.UnknownExtended = Math.Round(100.0 * (totalActiveUnknownExtension) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.ByExtension = Math.Round(100.0 * (totalActiveExtension) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.Extended = Math.Round(100.0 * (totalActiveExtended) / activePlayerCount, GeneralHelper.BoonDigit);
                            }
                        }
                        else if (boon.Type == Buff.BuffType.Intensity)
                        {
                            uptime.Generation = Math.Round(totalGeneration / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Overstack = Math.Round((totalOverstack + totalGeneration) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Wasted = Math.Round((totalWasted) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.UnknownExtended = Math.Round((totalUnknownExtension) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.ByExtension = Math.Round((totalExtension) / playerList.Count, GeneralHelper.BoonDigit);
                            uptime.Extended = Math.Round((totalExtended) / playerList.Count, GeneralHelper.BoonDigit);
                            //
                            if (activePlayerCount > 0)
                            {
                                uptimeActive.Generation = Math.Round(totalActiveGeneration / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.Overstack = Math.Round((totalActiveOverstack + totalActiveGeneration) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.Wasted = Math.Round((totalActiveWasted) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.UnknownExtended = Math.Round((totalActiveUnknownExtension) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.ByExtension = Math.Round((totalActiveExtension) / activePlayerCount, GeneralHelper.BoonDigit);
                                uptimeActive.Extended = Math.Round((totalActiveExtended) / activePlayerCount, GeneralHelper.BoonDigit);
                            }
                        }
                    }
                }

                uptimesByPhase.Add(final);
                uptimesActiveByPhase.Add(finalActive);
            }

            return (uptimesByPhase, uptimesActiveByPhase);
        }

        private void SetBuffs(ParsedLog log)
        {
            // Boons applied to self
            _selfBuffs = new List<Dictionary<long, FinalBuffs>>();
            _selfActiveBuffs = new List<Dictionary<long, FinalBuffs>>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                var final = new Dictionary<long, FinalBuffs>();
                var finalActive = new Dictionary<long, FinalBuffs>();

                PhaseData phase = phases[phaseIndex];

                BuffDistributionDictionary selfBoons = GetBoonDistribution(log, phaseIndex);
                Dictionary<long, long> buffPresence = GetBuffPresence(log, phaseIndex);

                long phaseDuration = phase.DurationInMS;
                long playerActiveDuration = phase.GetPlayerActiveDuration(this, log);
                foreach (Buff boon in TrackedBoons)
                {
                    if (selfBoons.ContainsKey(boon.ID))
                    {
                        var uptime = new FinalBuffs
                        {
                            Uptime = 0,
                            Generation = 0,
                            Overstack = 0,
                            Wasted = 0,
                            UnknownExtended = 0,
                            ByExtension = 0,
                            Extended = 0
                        };
                        var uptimeActive = new FinalBuffs
                        {
                            Uptime = 0,
                            Generation = 0,
                            Overstack = 0,
                            Wasted = 0,
                            UnknownExtended = 0,
                            ByExtension = 0,
                            Extended = 0
                        };
                        final[boon.ID] = uptime;
                        finalActive[boon.ID] = uptimeActive;
                        double generationValue = selfBoons.GetGeneration(boon.ID, AgentItem);
                        double uptimeValue = selfBoons.GetUptime(boon.ID);
                        double overstackValue = selfBoons.GetOverstack(boon.ID, AgentItem);
                        double wasteValue = selfBoons.GetWaste(boon.ID, AgentItem);
                        double unknownExtensionValue = selfBoons.GetUnknownExtension(boon.ID, AgentItem);
                        double extensionValue = selfBoons.GetExtension(boon.ID, AgentItem);
                        double extendedValue = selfBoons.GetExtended(boon.ID, AgentItem);
                        if (boon.Type == Buff.BuffType.Duration)
                        {
                            uptime.Uptime = Math.Round(100.0 * uptimeValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Generation = Math.Round(100.0 * generationValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Overstack = Math.Round(100.0 * (overstackValue + generationValue) / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Wasted = Math.Round(100.0 * wasteValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.UnknownExtended = Math.Round(100.0 * unknownExtensionValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.ByExtension = Math.Round(100.0 * extensionValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Extended = Math.Round(100.0 * extendedValue / phaseDuration, GeneralHelper.BoonDigit);
                            //
                            if (playerActiveDuration > 0)
                            {
                                uptimeActive.Uptime = Math.Round(100.0 * uptimeValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Generation = Math.Round(100.0 * generationValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Overstack = Math.Round(100.0 * (overstackValue + generationValue) / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Wasted = Math.Round(100.0 * wasteValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.UnknownExtended = Math.Round(100.0 * unknownExtensionValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.ByExtension = Math.Round(100.0 * extensionValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Extended = Math.Round(100.0 * extendedValue / playerActiveDuration, GeneralHelper.BoonDigit);
                            }
                        }
                        else if (boon.Type == Buff.BuffType.Intensity)
                        {
                            uptime.Uptime = Math.Round(uptimeValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Generation = Math.Round(generationValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Overstack = Math.Round((overstackValue + generationValue) / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Wasted = Math.Round(wasteValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.UnknownExtended = Math.Round(unknownExtensionValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.ByExtension = Math.Round(extensionValue / phaseDuration, GeneralHelper.BoonDigit);
                            uptime.Extended = Math.Round(extendedValue / phaseDuration, GeneralHelper.BoonDigit);
                            //
                            if (playerActiveDuration > 0)
                            {
                                uptimeActive.Uptime = Math.Round(uptimeValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Generation = Math.Round(generationValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Overstack = Math.Round((overstackValue + generationValue) / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Wasted = Math.Round(wasteValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.UnknownExtended = Math.Round(unknownExtensionValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.ByExtension = Math.Round(extensionValue / playerActiveDuration, GeneralHelper.BoonDigit);
                                uptimeActive.Extended = Math.Round(extendedValue / playerActiveDuration, GeneralHelper.BoonDigit);
                            }
                            //
                            if (buffPresence.TryGetValue(boon.ID, out long presenceValueBoon))
                            {
                                uptime.Presence = Math.Round(100.0 * presenceValueBoon / phaseDuration, GeneralHelper.BoonDigit);
                                //
                                if (playerActiveDuration > 0)
                                {
                                    uptimeActive.Presence = Math.Round(100.0 * presenceValueBoon / playerActiveDuration, GeneralHelper.BoonDigit);
                                }
                            }
                        }
                    }
                }

                _selfBuffs.Add(final);
                _selfActiveBuffs.Add(finalActive);
            }

            // Boons applied to player's group
            var otherPlayersInGroup = log.PlayerList
                .Where(p => p.Group == Group && Agent != p.Agent)
                .ToList();
            (_groupBuffs, _groupActiveBuffs) = GetBoonsForPlayers(otherPlayersInGroup, log);

            // Boons applied to other groups
            var offGroupPlayers = log.PlayerList.Where(p => p.Group != Group).ToList();
            (_offGroupBuffs, _offGroupActiveBuffs) = GetBoonsForPlayers(offGroupPlayers, log);

            // Boons applied to squad
            var otherPlayers = log.PlayerList.Where(p => p.Agent != Agent).ToList();
            (_squadBuffs, _squadActiveBuffs) = GetBoonsForPlayers(otherPlayers, log);
        }

        internal void Anonymize(int index)
        {
            Character = "Player " + index;
            Account = "Account " + index;
        }

        public List<DeathRecap> GetDeathRecaps(ParsedLog log)
        {
            if (_deathRecaps == null)
            {
                SetDeathRecaps(log);
            }
            if (_deathRecaps.Count == 0)
            {
                return null;
            }
            return _deathRecaps;
        }

        public string[] GetWeaponsArray(ParsedLog log)
        {
            if (_weaponsArray == null)
            {
                EstimateWeapons(log);
            }
            return _weaponsArray;
        }

        public List<Consumable> GetConsumablesList(ParsedLog log, long start, long end)
        {
            if (_consumeList == null)
            {
                SetConsumablesList(log);
            }
            return _consumeList.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public Dictionary<string, List<DamageModifierData>> GetDamageModifierData(ParsedLog log, Target target)
        {
            if (_damageModifiers == null)
            {
                SetDamageModifiersData(log);
            }
            if (target != null)
            {
                if (_damageModifiersTargets.TryGetValue(target, out Dictionary<string, List<DamageModifierData>> res))
                {
                    return res;
                }
                else
                {
                    return new Dictionary<string, List<DamageModifierData>>();
                }
            }
            return _damageModifiers;
        }

        public HashSet<string> GetPresentDamageModifier(ParsedLog log)
        {
            if (_presentDamageModifiers == null)
            {
                SetDamageModifiersData(log);
            }
            return _presentDamageModifiers;
        }

        // Private Methods

        private void SetDamageModifiersData(ParsedLog log)
        {
            _damageModifiers = new Dictionary<string, List<DamageModifierData>>();
            _damageModifiersTargets = new Dictionary<Target, Dictionary<string, List<DamageModifierData>>>();
            _presentDamageModifiers = new HashSet<string>();
            // If conjured sword or WvW, stop
            if (IsFakeActor || log.FightData.Logic.Mode == FightLogic.ParseMode.WvW)
            {
                return;
            }
            var damageMods = new List<DamageModifier>(log.DamageModifiers.DamageModifiersPerSource[DamageModifier.ModifierSource.ItemBuff]);
            damageMods.AddRange(log.DamageModifiers.DamageModifiersPerSource[DamageModifier.ModifierSource.CommonBuff]);
            damageMods.AddRange(log.DamageModifiers.GetModifiersPerProf(Prof));
            foreach (DamageModifier mod in damageMods)
            {
                mod.ComputeDamageModifier(_damageModifiers, _damageModifiersTargets, this, log);
            }
            _presentDamageModifiers.UnionWith(_damageModifiers.Keys);
            foreach (Target tar in _damageModifiersTargets.Keys)
            {
                _presentDamageModifiers.UnionWith(_damageModifiersTargets[tar].Keys);
            }
        }

        private void SetDeathRecaps(ParsedLog log)
        {
            _deathRecaps = new List<DeathRecap>();
            List<DeathRecap> res = _deathRecaps;
            List<DeadEvent> deads = log.CombatData.GetDeadEvents(AgentItem);
            List<DownEvent> downs = log.CombatData.GetDownEvents(AgentItem);
            long lastDeathTime = 0;
            List<AbstractDamageEvent> damageLogs = GetDamageTakenLogs(null, log, 0, log.FightData.FightDuration);
            foreach (DeadEvent dead in deads)
            {
                var recap = new DeathRecap()
                {
                    DeathTime = (int)dead.Time
                };
                DownEvent downed = downs.LastOrDefault(x => x.Time <= dead.Time && x.Time >= lastDeathTime);
                if (downed != null)
                {
                    var damageToDown = damageLogs.Where(x => x.Time <= downed.Time && (x.HasHit || x.HasDowned) && x.Time > lastDeathTime).ToList();
                    recap.ToDown = damageToDown.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    int damage = 0;
                    for (int i = damageToDown.Count - 1; i >= 0; i--)
                    {
                        AbstractDamageEvent dl = damageToDown[i];
                        AgentItem ag = dl.From;
                        var item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            IndirectDamage = dl is NonDirectDamageEvent,
                            ID = dl.SkillId,
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
                    var damageToKill = damageLogs.Where(x => x.Time > downed.Time && x.Time <= dead.Time && (x.HasHit || x.HasDowned) && x.Time > lastDeathTime).ToList();
                    recap.ToKill = damageToKill.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        AbstractDamageEvent dl = damageToKill[i];
                        AgentItem ag = dl.From;
                        var item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            IndirectDamage = dl is NonDirectDamageEvent,
                            ID = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : ""
                        };
                        recap.ToKill.Add(item);
                    }
                }
                else
                {
                    recap.ToDown = null;
                    var damageToKill = damageLogs.Where(x => x.Time < dead.Time && x.Damage > 0 && x.Time > lastDeathTime).ToList();
                    recap.ToKill = damageToKill.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    int damage = 0;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        AbstractDamageEvent dl = damageToKill[i];
                        AgentItem ag = dl.From;
                        var item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            IndirectDamage = dl is NonDirectDamageEvent,
                            ID = dl.SkillId,
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
                lastDeathTime = dead.Time;
                res.Add(recap);
            }
        }

        private void EstimateWeapons(ParsedLog log)
        {
            if (Prof == "Sword")
            {
                _weaponsArray = new string[]
                {
                    "Sword",
                    "2Hand",
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
                };
                return;
            }
            string[] weapons = new string[8];//first 2 for first set next 2 for second set, second sets of 4 for underwater
            List<AbstractCastEvent> casting = GetCastLogs(log, 0, log.FightData.FightDuration);
            int swapped = -1;
            long swappedTime = 0;
            var swaps = casting.Where(x => x.SkillId == SkillItem.WeaponSwapId).Select(x =>
            {
                if (x is WeaponSwapEvent wse)
                {
                    return wse.SwappedTo;
                }
                return -1;
            }).ToList();
            foreach (AbstractCastEvent cl in casting)
            {
                if (cl.ActualDuration == 0 && cl.SkillId != SkillItem.WeaponSwapId)
                {
                    continue;
                }
                SkillItem skill = cl.Skill;
                // first iteration
                if (swapped == -1)
                {
                    swapped = skill.FindWeaponSlot(swaps);
                }
                if (!skill.EstimateWeapons(weapons, swapped, cl.Time > swappedTime) && cl is WeaponSwapEvent swe)
                {
                    //wepswap  
                    swapped = swe.SwappedTo;
                    swappedTime = swe.Time;
                }
            }
            _weaponsArray = weapons;
        }

        private void SetConsumablesList(ParsedLog log)
        {
            List<Buff> consumableList = log.Buffs.BuffsByNature[BuffNature.Consumable];
            _consumeList = new List<Consumable>();
            long fightDuration = log.FightData.FightDuration;
            foreach (Buff consumable in consumableList)
            {
                foreach (AbstractBuffEvent c in log.CombatData.GetBoonData(consumable.ID))
                {
                    if (!(c is BuffApplyEvent ba) || AgentItem != ba.To)
                    {
                        continue;
                    }
                    long time = 0;
                    if (!ba.Initial)
                    {
                        time = ba.Time;
                    }
                    if (time <= fightDuration)
                    {
                        Consumable existing = _consumeList.Find(x => x.Time == time && x.Buff.ID == consumable.ID);
                        if (existing != null)
                        {
                            existing.Stack++;
                        }
                        else
                        {
                            _consumeList.Add(new Consumable(consumable, time, ba.AppliedDuration));
                        }
                    }
                }
            }
            _consumeList.Sort((x, y) => x.Time.CompareTo(y.Time));

        }

        protected override void InitAdditionalCombatReplayData(ParsedLog log)
        {
            if (IsFakeActor)
            {
                return;
            }
            // Fight related stuff
            log.FightData.Logic.ComputePlayerCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any())
            {
                CombatReplay.Actors.Add(new FacingActor(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
        }

        //
        private class PlayerSerializable : AbstractMasterActorSerializable
        {
            public int Group { get; set; }
            public long[] Dead { get; set; }
            public long[] Down { get; set; }
            public long[] Dc { get; set; }
        }

        public override AbstractMasterActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            var aux = new PlayerSerializable
            {
                Group = Group,
                Img = CombatReplay.Icon,
                Type = "Player",
                ID = GetCombatReplayID(log),
                Positions = new double[2 * CombatReplay.PolledPositions.Count],
                Dead = new long[2 * CombatReplay.Deads.Count],
                Down = new long[2 * CombatReplay.Downs.Count],
                Dc = new long[2 * CombatReplay.DCs.Count]
            };
            int i = 0;
            foreach (Point3D pos in CombatReplay.PolledPositions)
            {
                (double x, double y) = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions[i++] = x;
                aux.Positions[i++] = y;
            }
            i = 0;
            foreach ((long start, long end) in CombatReplay.Deads)
            {
                aux.Dead[i++] = start;
                aux.Dead[i++] = end;
            }
            i = 0;
            foreach ((long start, long end) in CombatReplay.Downs)
            {
                aux.Down[i++] = start;
                aux.Down[i++] = end;
            }
            i = 0;
            foreach ((long start, long end) in CombatReplay.DCs)
            {
                aux.Dc[i++] = start;
                aux.Dc[i++] = end;
            }

            return aux;
        }

        protected override void InitCombatReplay(ParsedLog log)
        {
            if (!log.CombatData.HasMovementData || IsFakeActor)
            {
                // no combat replay support on fight
                return;
            }
            CombatReplay = new CombatReplay
            {
                Icon = GeneralHelper.GetProfIcon(Prof)
            };
            SetMovements(log);
            // Down and deads
            List<(long, long)> dead = CombatReplay.Deads;
            List<(long, long)> down = CombatReplay.Downs;
            List<(long, long)> dc = CombatReplay.DCs;
            AgentItem.GetAgentStatus(dead, down, dc, log);
            CombatReplay.PollingRate(log.FightData.FightDuration, true);
        }


        /*protected override void setHealingLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getHealingData())
            {
                if (agent.InstID == c.getSrcInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())//selecting player or minion as caster
                {
                    long time = c.getTime() - time_start;
                    addHealingLog(time, c);
                }
            }
            Dictionary<string, Minions> min_list = getMinions(log);
            foreach (Minions mins in min_list.Values)
            {
                healing_logs.AddRange(mins.getHealingLogs(log, 0, log.getBossData().getAwareDuration()));
            }
            healing_logs.Sort((x, y) => x.getTime() < y.getTime() ? -1 : 1);
        }

        protected override void setHealingReceivedLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getHealingReceivedData())
            {
                if (agent.InstID == c.getDstInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())
                {//selecting player as target
                    long time = c.getTime() - time_start;
                    addHealingReceivedLog(time, c);
                }
            }
        }*/
    }
}
