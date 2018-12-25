using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LuckParser.Models.ParseModels;
using static LuckParser.Models.DataModels.Statistics;

namespace LuckParser.Models.DataModels
{
    /// <summary>
    /// Calculates statistical information from a log
    /// </summary>
    class StatisticsCalculator
    {
        public class Switches
        {
            public bool CalculateDPS = false;
            public bool CalculateStats = false;
            public bool CalculateDefense = false;
            public bool CalculateSupport = false;
            public bool CalculateBoons = false;
            public bool CalculateConditions = false;
            public bool CalculateCombatReplay = false;
            public bool CalculateMechanics = false;
        }

        private readonly SettingsContainer _settings;

        private Statistics _statistics;

        private ParsedLog _log;

        public StatisticsCalculator(SettingsContainer settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Calculate a statistic from a log
        /// </summary>
        /// <param name="log"></param>
        /// <param name="switches"></param>
        /// <returns></returns>
        public Statistics CalculateStatistics(ParsedLog log, Switches switches)
        {
            _statistics = new Statistics();

            _log = log;

            SetPresentBoons();
            _statistics.Phases = log.FightData.GetPhases(log);
            if (switches.CalculateCombatReplay && _settings.ParseCombatReplay)
            {
                foreach (Player p in log.PlayerList)
                {
                    if (p.Account == ":Conjured Sword")
                    {
                        continue;
                    }
                    p.InitCombatReplay(log, _settings.PollingRate, false, true);
                }
                foreach (Target target in log.FightData.Logic.Targets)
                {
                    target.InitCombatReplay(log, _settings.PollingRate, true, log.FightData.GetMainTargets(log).Contains(target));
                }
                log.FightData.Logic.InitTrashMobCombatReplay(log, _settings.PollingRate);

                // Ensuring all combat replays are initialized before extra data (and agent interaction) is computed
                foreach (Player p in log.PlayerList)
                {
                    if (p.Account == ":Conjured Sword")
                    {
                        continue;
                    }
                    p.ComputeAdditionalCombatReplayData(log);
                }
                foreach (Target target in log.FightData.Logic.Targets)
                {
                    target.ComputeAdditionalCombatReplayData(log);
                }

                foreach (Mob mob in log.FightData.Logic.TrashMobs)
                {
                    mob.ComputeAdditionalCombatReplayData(log);
                }


            }
            if (switches.CalculateDPS) CalculateDPS();
            if (switches.CalculateBoons) CalculateBoons();
            if (switches.CalculateStats) CalculateStats();
            if (switches.CalculateDefense) CalculateDefenses();
            if (switches.CalculateSupport) CalculateSupport();

            if (switches.CalculateConditions) CalculateConditions();
            if (switches.CalculateMechanics)
            {
                log.FightData.Logic.ComputeMechanics(log);
            }
            // target health
            _statistics.TargetsHealth = new Dictionary<Target, double[]>[_statistics.Phases.Count];
            for (var j = 0; j < _statistics.Phases.Count; j++)
            {
                _statistics.TargetsHealth[j] = new Dictionary<Target, double[]>();
                PhaseData phase = _statistics.Phases[j];
                int seconds = (int)phase.GetDuration("s");
                bool needsLastPoint = seconds * 1000 != phase.GetDuration();
                foreach (Target target in _log.FightData.Logic.Targets)
                {
                    double[] health = new double[seconds + (needsLastPoint ? 2 : 1)];
                    int i = 0;
                    double curHealth = 100.0;
                    foreach (Point p in target.HealthOverTime)
                    {
                        double hp = p.Y / 100.0;
                        int timeInPhase = 1 + (p.X - (int)phase.Start) / 1000;
                        if (timeInPhase > seconds)
                        {
                            break;
                        }
                        while (i < timeInPhase)
                        {
                            health[i++] = curHealth;
                        }
                        curHealth = hp;
                        if (timeInPhase >= 0)
                        {
                            health[timeInPhase] = curHealth;
                        }
                    }
                    for (; i <= seconds; i++) health[i] = curHealth;
                    if (needsLastPoint)
                    {
                        Point last = target.HealthOverTime.LastOrDefault(x => x.X <= phase.End);
                        health[seconds + 1] = last != null ? last.Y / 100 : health[seconds];
                    }
                    _statistics.TargetsHealth[j][target] = health;
                }
            }

            //

            return _statistics;
        }

        private FinalDPS GetFinalDPS(AbstractPlayer player, int phaseIndex, Target target)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];
            double phaseDuration = (phase.GetDuration()) / 1000.0;
            double damage;
            double dps = 0.0;
            FinalDPS final = new FinalDPS();
            //DPS
            damage = player.GetDamageLogs(target, _log,
                    phase.Start, phase.End).Sum(x => x.Damage);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.Dps = (int)dps;
            final.Damage = (int)damage;
            //Condi DPS
            damage = player.GetDamageLogs(target, _log,
                    phase.Start, phase.End).Sum(x => x.IsCondi > 0 ? x.Damage : 0);

            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.CondiDps = (int)dps;
            final.CondiDamage = (int)damage;
            //Power DPS
            damage = final.Damage - final.CondiDamage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.PowerDps = (int)dps;
            final.PowerDamage = (int)damage;
            return final;
        }

        private void CalculateDPS()
        {
            foreach (Player player in _log.PlayerList)
            {
                FinalDPS[] phaseDps = new FinalDPS[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
                {
                    phaseDps[phaseIndex] = GetFinalDPS(player, phaseIndex, null);
                }
                _statistics.DpsAll[player] = phaseDps;
            }
            foreach (Target target in _log.FightData.Logic.Targets)
            {
                Dictionary<Player, FinalDPS[]> stats = new Dictionary<Player, FinalDPS[]>();
                foreach (Player player in _log.PlayerList)
                {
                    FinalDPS[] phaseDpsTarget = new FinalDPS[_statistics.Phases.Count];
                    for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
                    {
                        phaseDpsTarget[phaseIndex] = GetFinalDPS(player, phaseIndex, target);
                    }
                    stats[player] = phaseDpsTarget;
                }
                _statistics.DpsTarget[target] = stats;
                FinalDPS[] phaseTargetDps = new FinalDPS[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
                {
                    phaseTargetDps[phaseIndex] = GetFinalDPS(target, phaseIndex, null);
                }
                _statistics.TargetDps[target] = phaseTargetDps;
            }
        }

        private void FillFinalStats(List<DamageLog> dls, FinalStats final, Dictionary<Target, FinalStats> targetsFinal)
        {
            HashSet<long> nonCritable = new HashSet<long>
                    {
                        9292,
                        52370
                    };
            // (x - 1) / x
            double fiveGain = 0.05 / 1.05;
            double tenGain = 0.1 / 1.1;
            foreach (DamageLog dl in dls)
            {
                if (dl.IsCondi == 0)
                {
                    foreach (var pair in targetsFinal)
                    {
                        Target target = pair.Key;
                        if (dl.DstInstId == target.InstID && dl.Time <= _log.FightData.ToFightSpace(target.LastAware) && dl.Time >= _log.FightData.ToFightSpace(target.FirstAware))
                        {
                            FinalStats targetFinal = pair.Value;
                            if (dl.Result == ParseEnum.Result.Crit)
                            {
                                targetFinal.CriticalRate++;
                                targetFinal.CriticalDmg += dl.Damage;
                            }

                            if (dl.IsNinety > 0)
                            {
                                targetFinal.ScholarRate++;
                                targetFinal.ScholarDmg += (int)(fiveGain * dl.Damage);
                            }

                            if (dl.IsFifty > 0)
                            {
                                targetFinal.EagleRate++;
                                targetFinal.EagleDmg += (int)(tenGain * dl.Damage);
                            }

                            if (dl.IsMoving > 0)
                            {
                                targetFinal.MovingRate++;
                                targetFinal.MovingDamage += (int)(fiveGain * dl.Damage);
                            }

                            if (dl.IsFlanking > 0)
                            {
                                targetFinal.FlankingDmg += (int)(tenGain * dl.Damage);
                                targetFinal.FlankingRate++;
                            }

                            if (dl.Result == ParseEnum.Result.Glance)
                            {
                                targetFinal.GlanceRate++;
                            }

                            if (dl.Result == ParseEnum.Result.Blind)
                            {
                                targetFinal.Missed++;
                            }
                            if (dl.Result == ParseEnum.Result.Interrupt)
                            {
                                targetFinal.Interrupts++;
                            }

                            if (dl.Result == ParseEnum.Result.Absorb)
                            {
                                targetFinal.Invulned++;
                            }
                            targetFinal.PowerLoopCount++;
                            targetFinal.PowerDamage += dl.Damage;
                            if (!nonCritable.Contains(dl.SkillId))
                            {
                                targetFinal.CritablePowerLoopCount++;
                            }
                        }
                    }
                    if (dl.Result == ParseEnum.Result.Crit)
                    {
                        final.CriticalRate++;
                        final.CriticalDmg += dl.Damage;
                    }

                    if (dl.IsNinety > 0)
                    {
                        final.ScholarRate++;
                        final.ScholarDmg += (int)(fiveGain * dl.Damage);
                    }

                    if (dl.IsFifty > 0)
                    {
                        final.EagleRate++;
                        final.EagleDmg += (int)(tenGain * dl.Damage);
                    }

                    if (dl.IsMoving > 0)
                    {
                        final.MovingRate++;
                        final.MovingDamage += (int)(fiveGain * dl.Damage);
                    }

                    if (dl.IsFlanking > 0)
                    {
                        final.FlankingDmg += (int)(tenGain * dl.Damage);
                        final.FlankingRate++;
                    }

                    if (dl.Result == ParseEnum.Result.Glance)
                    {
                        final.GlanceRate++;
                    }

                    if (dl.Result == ParseEnum.Result.Blind)
                    {
                        final.Missed++;
                    }
                    if (dl.Result == ParseEnum.Result.Interrupt)
                    {
                        final.Interrupts++;
                    }

                    if (dl.Result == ParseEnum.Result.Absorb)
                    {
                        final.Invulned++;
                    }
                    final.PowerLoopCount++;
                    final.PowerDamage += dl.Damage;
                    if (!nonCritable.Contains(dl.SkillId))
                    {
                        final.CritablePowerLoopCount++;
                    }
                }
            }
        }

        private FinalStatsAll GetFinalStats(Player p, int phaseIndex)
        {
            PhaseData phase = _statistics.Phases[phaseIndex];
            long start = _log.FightData.ToLogSpace(phase.Start);
            long end = _log.FightData.ToLogSpace(phase.End);
            Dictionary<Target, FinalStats> targetDict = new Dictionary<Target, FinalStats>();
            foreach (Target target in _log.FightData.Logic.Targets)
            {
                if (!_statistics.StatsTarget.ContainsKey(target))
                {
                    _statistics.StatsTarget[target] = new Dictionary<Player, FinalStats[]>();
                }
                Dictionary<Player, FinalStats[]> targetStats = _statistics.StatsTarget[target];
                if (!targetStats.ContainsKey(p))
                {
                    targetStats[p] = new FinalStats[_statistics.Phases.Count];
                }
                targetStats[p][phaseIndex] = new FinalStats();
                targetDict[target] = targetStats[p][phaseIndex];
            }
            FinalStatsAll final = new FinalStatsAll();
            FillFinalStats(p.GetJustPlayerDamageLogs(null, _log, phase.Start, phase.End), final, targetDict);
            if (p.Account == ":Conjured Sword")
            {
                return final;
            }
            foreach (CastLog cl in p.GetCastLogs(_log, phase.Start, phase.End))
            {
                if (cl.EndActivation == ParseEnum.Activation.CancelCancel)
                {
                    final.Wasted++;
                    final.TimeWasted += cl.ActualDuration;
                }
                if (cl.EndActivation == ParseEnum.Activation.CancelFire)
                {
                    if (cl.ActualDuration < cl.ExpectedDuration)
                    {
                        final.Saved++;
                        final.TimeSaved += cl.ExpectedDuration - cl.ActualDuration;
                    }
                }
            }
            final.TimeSaved = Math.Round(final.TimeSaved / 1000.0, 3);
            final.TimeWasted = Math.Round(final.TimeWasted / 1000.0, 3);

            double avgBoons = 0;
            foreach (long duration in p.GetBoonPresence(_log, phaseIndex).Values)
            {
                avgBoons += duration;
            }
            final.AvgBoons = avgBoons / phase.GetDuration();

            double avgCondis = 0;
            foreach (long duration in p.GetCondiPresence(_log, phaseIndex).Values)
            {
                avgCondis += duration;
            }
            final.AvgConditions = avgCondis / phase.GetDuration();


            // Counts
            CombatData combatData = _log.CombatData;
            final.SwapCount = p.GetCastLogs(_log, 0, _log.FightData.FightDuration).Count(x => x.SkillId == SkillItem.WeaponSwapId);

            if (_settings.ParseCombatReplay && _log.FightData.Logic.CanCombatReplay)
            {
                if (_statistics.StackCenterPositions == null)
                {
                    _statistics.StackCenterPositions = new List<Point3D>();
                    List<List<Point3D>> GroupsPosList = new List<List<Point3D>>();
                    foreach (Player player in _log.PlayerList)
                    {
                        if (player.Account == ":Conjured Sword")
                        {
                            continue;
                        }
                        GroupsPosList.Add(player.CombatReplay.GetActivePositions());
                    }
                    for (int time = 0; time < GroupsPosList[0].Count; time++)
                    {
                        float x = 0;
                        float y = 0;
                        float z = 0;
                        int activePlayers = GroupsPosList.Count;
                        foreach (List<Point3D> points in GroupsPosList)
                        {
                            Point3D point = points[time];
                            if (point != null)
                            {
                                x += point.X;
                                y += point.Y;
                                z += point.Z;
                            }
                            else
                            {
                                activePlayers--;
                            }

                        }
                        x = x / activePlayers;
                        y = y / activePlayers;
                        z = z / activePlayers;
                        _statistics.StackCenterPositions.Add(new Point3D(x, y, z, _settings.PollingRate * time));
                    }
                }
                List<Point3D> positions = p.CombatReplay.Positions.Where(x => x.Time >= phase.Start && x.Time <= phase.End).ToList();
                int offset = p.CombatReplay.Positions.Count(x => x.Time < phase.Start);
                if (positions.Count > 1)
                {
                    List<float> distances = new List<float>();
                    for (int time = 0; time < positions.Count; time++)
                    {

                        float deltaX = positions[time].X - _statistics.StackCenterPositions[time + offset].X;
                        float deltaY = positions[time].Y - _statistics.StackCenterPositions[time + offset].Y;
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
            return final;
        }

        private void CalculateStats()
        {
            foreach (Target target in _log.FightData.Logic.Targets)
            {
                double[] avgBoons = new double[_statistics.Phases.Count];
                double[] avgCondis = new double[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
                {
                    double avgBoon = 0;
                    foreach (long duration in target.GetBoonPresence(_log, phaseIndex).Values)
                    {
                        avgBoon += duration;
                    }
                    avgBoon /= _statistics.Phases[phaseIndex].GetDuration();
                    avgBoons[phaseIndex] = avgBoon;

                    double avgCondi = 0;
                    foreach (long duration in target.GetCondiPresence(_log, phaseIndex).Values)
                    {
                        avgCondi += duration;
                    }
                    avgCondi /= _statistics.Phases[phaseIndex].GetDuration();
                    avgCondis[phaseIndex] = avgCondi;
                }
                _statistics.AvgTargetBoons[target] = avgBoons;
                _statistics.AvgTargetConditions[target] = avgCondis;
            }
            foreach (Player player in _log.PlayerList)
            {
                FinalStatsAll[] phaseStats = new FinalStatsAll[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
                {
                    phaseStats[phaseIndex] = GetFinalStats(player, phaseIndex);
                }
                _statistics.StatsAll[player] = phaseStats;
            }
        }

        private void CalculateDefenses()
        {
            CombatData combatData = _log.CombatData;
            foreach (Player player in _log.PlayerList)
            {
                FinalDefenses[] phaseDefense = new FinalDefenses[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
                {
                    FinalDefenses final = new FinalDefenses();

                    PhaseData phase = _statistics.Phases[phaseIndex];
                    long start = _log.FightData.ToLogSpace(phase.Start);
                    long end = _log.FightData.ToLogSpace(phase.End);

                    List<DamageLog> damageLogs = player.GetDamageTakenLogs(null, _log, phase.Start, phase.End);
                    //List<DamageLog> healingLogs = player.getHealingReceivedLogs(log, phase.getStart(), phase.getEnd());

                    final.DamageTaken = damageLogs.Sum(x => (long)x.Damage);
                    //final.allHealReceived = healingLogs.Sum(x => x.getDamage());
                    final.BlockedCount = damageLogs.Count(x => x.Result == ParseEnum.Result.Block);
                    final.InvulnedCount = 0;
                    final.DamageInvulned = 0;
                    final.EvadedCount = damageLogs.Count(x => x.Result == ParseEnum.Result.Evade);
                    final.DodgeCount = player.GetCastLogs(_log, 0, _log.FightData.FightDuration).Count(x => x.SkillId == SkillItem.DodgeId);
                    final.DamageBarrier = damageLogs.Sum(x => x.ShieldDamage);
                    final.InterruptedCount = damageLogs.Count(x => x.Result == ParseEnum.Result.Interrupt);
                    foreach (DamageLog log in damageLogs.Where(x => x.Result == ParseEnum.Result.Absorb))
                    {
                        final.InvulnedCount++;
                        final.DamageInvulned += log.Damage;
                    }
                    List<CombatItem> deads = combatData.GetStatesData(player.InstID, ParseEnum.StateChange.ChangeDead, start, end);
                    List<CombatItem> downs = combatData.GetStatesData(player.InstID, ParseEnum.StateChange.ChangeDown, start, end);
                    List<CombatItem> dcs = combatData.GetStatesData(player.InstID, ParseEnum.StateChange.Despawn, start, end);
                    final.DownCount = downs.Count;
                    final.DeadCount = deads.Count;
                    final.DcCount = dcs.Count;

                    phaseDefense[phaseIndex] = final;
                }
                List<Tuple<long, long>> dead = new List<Tuple<long, long>>();
                List<Tuple<long, long>> down = new List<Tuple<long, long>>();
                List<Tuple<long, long>> dc = new List<Tuple<long, long>>();
                combatData.GetAgentStatus(player.FirstAware, player.LastAware, player.InstID, dead, down, dc);

                for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
                {
                    FinalDefenses defenses = phaseDefense[phaseIndex];
                    PhaseData phase = _statistics.Phases[phaseIndex];
                    long start = phase.Start;
                    long end = phase.End;
                    defenses.DownDuration = (int)down.Where(x => x.Item2 >= start && x.Item1 <= end).Sum(x => Math.Min(end, x.Item2) - Math.Max(x.Item1, start));
                    defenses.DeadDuration = (int)dead.Where(x => x.Item2 >= start && x.Item1 <= end).Sum(x => Math.Min(end, x.Item2) - Math.Max(x.Item1, start));
                    defenses.DcDuration = (int)dc.Where(x => x.Item2 >= start && x.Item1 <= end).Sum(x => Math.Min(end, x.Item2) - Math.Max(x.Item1, start));
                }

                _statistics.Defenses[player] = phaseDefense;
            }
        }

        private void CalculateSupport()
        {
            foreach (Player player in _log.PlayerList)
            {
                FinalSupport[] phaseSupport = new FinalSupport[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
                {
                    FinalSupport final = new FinalSupport();

                    PhaseData phase = _statistics.Phases[phaseIndex];

                    int[] resArray = player.GetReses(_log, phase.Start, phase.End);
                    int[] cleanseArray = player.GetCleanses(_log, phaseIndex);
                    //List<DamageLog> healingLogs = player.getHealingLogs(log, phase.getStart(), phase.getEnd());
                    //final.allHeal = healingLogs.Sum(x => x.getDamage());
                    final.Resurrects = resArray[0];
                    final.ResurrectTime = resArray[1] / 1000.0;
                    final.CondiCleanse = cleanseArray[0];
                    final.CondiCleanseTime = cleanseArray[1] / 1000.0;

                    phaseSupport[phaseIndex] = final;
                }
                _statistics.Support[player] = phaseSupport;
            }
        }

        private Dictionary<long, FinalBuffs>[] GetBoonsForPlayers(List<Player> playerList, Player player)
        {
            Dictionary<long, FinalBuffs>[] uptimesByPhase =
                new Dictionary<long, FinalBuffs>[_statistics.Phases.Count];

            for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
            {
                PhaseData phase = _statistics.Phases[phaseIndex];
                long fightDuration = phase.End - phase.Start;

                Dictionary<Player, BoonDistribution> boonDistributions = new Dictionary<Player, BoonDistribution>();
                foreach (Player p in playerList)
                {
                    boonDistributions[p] = p.GetBoonDistribution(_log, phaseIndex);
                }

                HashSet<Boon> boonsToTrack = new HashSet<Boon>(boonDistributions.SelectMany(x => x.Value).Select(x => Boon.BoonsByIds[x.Key]));

                Dictionary<long, FinalBuffs> final =
                    new Dictionary<long, FinalBuffs>();

                foreach (Boon boon in boonsToTrack)
                {
                    long totalGeneration = 0;
                    long totalOverstack = 0;
                    long totalWasted = 0;
                    long totalUnknownExtension = 0;
                    long totalExtension = 0;

                    foreach (BoonDistribution boons in boonDistributions.Values)
                    {
                        if (boons.ContainsKey(boon.ID))
                        {
                            totalGeneration += boons.GetGeneration(boon.ID, player.InstID);
                            totalOverstack += boons.GetOverstack(boon.ID, player.InstID);
                            totalWasted += boons.GetWaste(boon.ID, player.InstID);
                            totalUnknownExtension += boons.GetUnknownExtension(boon.ID, player.InstID);
                            totalExtension += boons.GetExtension(boon.ID, player.InstID);
                        }
                    }

                    FinalBuffs uptime = new FinalBuffs();

                    if (boon.Type == Boon.BoonType.Duration)
                    {
                        uptime.Generation = Math.Round(100.0 * totalGeneration / fightDuration / playerList.Count, 2);
                        uptime.Overstack = Math.Round(100.0 * (totalOverstack + totalGeneration) / fightDuration / playerList.Count, 2);
                        uptime.Wasted = Math.Round(100.0 * (totalWasted) / fightDuration / playerList.Count, 2);
                        uptime.UnknownExtension = Math.Round(100.0 * (totalUnknownExtension) / fightDuration / playerList.Count, 2);
                        uptime.Extension = Math.Round(100.0 * (totalExtension) / fightDuration / playerList.Count, 2);
                    }
                    else if (boon.Type == Boon.BoonType.Intensity)
                    {
                        uptime.Generation = Math.Round((double)totalGeneration / fightDuration / playerList.Count, 2);
                        uptime.Overstack = Math.Round((double)(totalOverstack + totalGeneration) / fightDuration / playerList.Count, 2);
                        uptime.Wasted = Math.Round((double)(totalWasted) / fightDuration / playerList.Count, 2);
                        uptime.UnknownExtension = Math.Round((double)(totalUnknownExtension) / fightDuration / playerList.Count, 2);
                        uptime.Extension = Math.Round((double)(totalExtension) / fightDuration / playerList.Count, 2);
                    }
                    final[boon.ID] = uptime;
                }

                uptimesByPhase[phaseIndex] = final;
            }

            return uptimesByPhase;
        }

        private void CalculateBoons()
        {
            foreach (Player player in _log.PlayerList)
            {
                // Boons applied to self
                Dictionary<long, FinalBuffs>[] selfUptimesByPhase = new Dictionary<long, FinalBuffs>[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
                {
                    Dictionary<long, FinalBuffs> final = new Dictionary<long, FinalBuffs>();

                    PhaseData phase = _statistics.Phases[phaseIndex];

                    BoonDistribution selfBoons = player.GetBoonDistribution(_log, phaseIndex);
                    Dictionary<long, long> boonPresence = player.GetBoonPresence(_log, phaseIndex);
                    Dictionary<long, long> condiPresence = player.GetCondiPresence(_log, phaseIndex);

                    long fightDuration = phase.End - phase.Start;
                    foreach (Boon boon in player.TrackedBoons)
                    {
                        FinalBuffs uptime = new FinalBuffs
                        {
                            Uptime = 0,
                            Generation = 0,
                            Overstack = 0,
                            Wasted = 0,
                            UnknownExtension = 0,
                            Extension = 0
                        };
                        if (selfBoons.ContainsKey(boon.ID))
                        {
                            long generation = selfBoons.GetGeneration(boon.ID, player.InstID);
                            if (boon.Type == Boon.BoonType.Duration)
                            {
                                uptime.Uptime = Math.Round(100.0 * selfBoons.GetUptime(boon.ID) / fightDuration, 2);
                                uptime.Generation = Math.Round(100.0 * generation / fightDuration, 2);
                                uptime.Overstack = Math.Round(100.0 * (selfBoons.GetOverstack(boon.ID, player.InstID) + generation) / fightDuration, 2);
                                uptime.Wasted = Math.Round(100.0 * selfBoons.GetWaste(boon.ID, player.InstID) / fightDuration, 2);
                                uptime.UnknownExtension = Math.Round(100.0 * selfBoons.GetUnknownExtension(boon.ID, player.InstID) / fightDuration, 2);
                                uptime.Extension = Math.Round(100.0 * selfBoons.GetExtension(boon.ID, player.InstID) / fightDuration, 2);
                            }
                            else if (boon.Type == Boon.BoonType.Intensity)
                            {
                                uptime.Uptime = Math.Round((double)selfBoons.GetUptime(boon.ID) / fightDuration, 2);
                                uptime.Generation = Math.Round((double)generation / fightDuration, 2);
                                uptime.Overstack = Math.Round((double)(selfBoons.GetOverstack(boon.ID, player.InstID) + generation) / fightDuration, 2);
                                uptime.Wasted = Math.Round((double)selfBoons.GetWaste(boon.ID, player.InstID) / fightDuration, 2);
                                uptime.UnknownExtension = Math.Round((double)selfBoons.GetUnknownExtension(boon.ID, player.InstID) / fightDuration, 2);
                                uptime.Extension = Math.Round((double)selfBoons.GetExtension(boon.ID, player.InstID) / fightDuration, 2);
                                if (boonPresence.TryGetValue(boon.ID, out long presenceValueBoon))
                                {
                                    uptime.Presence = Math.Round(100.0 * presenceValueBoon / fightDuration, 2);
                                }
                                else if (condiPresence.TryGetValue(boon.ID, out long presenceValueCondi))
                                {
                                    uptime.Presence = Math.Round(100.0 * presenceValueCondi / fightDuration, 2);
                                }
                            }
                        }
                        final[boon.ID] = uptime;
                    }

                    selfUptimesByPhase[phaseIndex] = final;
                }
                _statistics.SelfBuffs[player] = selfUptimesByPhase;

                // Boons applied to player's group
                var otherPlayersInGroup = _log.PlayerList
                    .Where(p => p.Group == player.Group && player.InstID != p.InstID)
                    .ToList();
                _statistics.GroupBuffs[player] = GetBoonsForPlayers(otherPlayersInGroup, player);

                // Boons applied to other groups
                var offGroupPlayers = _log.PlayerList.Where(p => p.Group != player.Group).ToList();
                _statistics.OffGroupBuffs[player] = GetBoonsForPlayers(offGroupPlayers, player);

                // Boons applied to squad
                var otherPlayers = _log.PlayerList.Where(p => p.InstID != player.InstID).ToList();
                _statistics.SquadBuffs[player] = GetBoonsForPlayers(otherPlayers, player);
            }
        }

        private void CalculateConditions()
        {
            foreach (Target target in _log.FightData.Logic.Targets)
            {
                Dictionary<long, FinalTargetBuffs>[] stats = new Dictionary<long, FinalTargetBuffs>[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
                {
                    BoonDistribution boonDistribution = target.GetBoonDistribution(_log, phaseIndex);
                    Dictionary<long, FinalTargetBuffs> rates = new Dictionary<long, FinalTargetBuffs>();
                    Dictionary<long, long> boonPresence = target.GetBoonPresence(_log, phaseIndex);
                    Dictionary<long, long> condiPresence = target.GetCondiPresence(_log, phaseIndex);

                    PhaseData phase = _statistics.Phases[phaseIndex];
                    long fightDuration = phase.GetDuration();

                    foreach (Boon boon in target.TrackedBoons)
                    {
                        FinalTargetBuffs buff = new FinalTargetBuffs(_log.PlayerList);
                        rates[boon.ID] = buff;
                        if (boonDistribution.ContainsKey(boon.ID))
                        {
                            if (boon.Type == Boon.BoonType.Duration)
                            {
                                buff.Uptime = Math.Round(100.0 * boonDistribution.GetUptime(boon.ID) / fightDuration, 2);
                                foreach (Player p in _log.PlayerList)
                                {
                                    long gen = boonDistribution.GetGeneration(boon.ID, p.InstID);
                                    buff.Generated[p] = Math.Round(100.0 * gen / fightDuration, 2);
                                    buff.Overstacked[p] = Math.Round(100.0 * (boonDistribution.GetOverstack(boon.ID, p.InstID) + gen) / fightDuration, 2);
                                    buff.Wasted[p] = Math.Round(100.0 * boonDistribution.GetWaste(boon.ID, p.InstID) / fightDuration, 2);
                                    buff.UnknownExtension[p] = Math.Round(100.0 * boonDistribution.GetUnknownExtension(boon.ID, p.InstID) / fightDuration, 2);
                                    buff.Extension[p] = Math.Round(100.0 * boonDistribution.GetExtension(boon.ID, p.InstID) / fightDuration, 2);
                                }
                            }
                            else if (boon.Type == Boon.BoonType.Intensity)
                            {
                                buff.Uptime = Math.Round((double)boonDistribution.GetUptime(boon.ID) / fightDuration, 2);
                                foreach (Player p in _log.PlayerList)
                                {
                                    long gen = boonDistribution.GetGeneration(boon.ID, p.InstID);
                                    buff.Generated[p] = Math.Round((double)gen / fightDuration, 2);
                                    buff.Overstacked[p] = Math.Round((double)(boonDistribution.GetOverstack(boon.ID, p.InstID) + gen) / fightDuration, 2);
                                    buff.Wasted[p] = Math.Round((double)boonDistribution.GetWaste(boon.ID, p.InstID) / fightDuration, 2);
                                    buff.UnknownExtension[p] = Math.Round((double)boonDistribution.GetUnknownExtension(boon.ID, p.InstID) / fightDuration, 2);
                                    buff.Extension[p] = Math.Round((double)boonDistribution.GetExtension(boon.ID, p.InstID) / fightDuration, 2);
                                }
                                if (boonPresence.TryGetValue(boon.ID, out long presenceValueBoon))
                                {
                                    buff.Presence = Math.Round(100.0 * presenceValueBoon / fightDuration, 2);
                                }
                                else if (condiPresence.TryGetValue(boon.ID, out long presenceValueCondi))
                                {
                                    buff.Presence = Math.Round(100.0 * presenceValueCondi / fightDuration, 2);
                                }
                            }

                            rates[boon.ID] = buff;
                        }
                    }
                    stats[phaseIndex] = rates;
                }
                _statistics.TargetBuffs[target] = stats;
            }
        }
        /// <summary>
        /// Checks the combat data and gets buffs that were present during the fight
        /// </summary>
        private void SetPresentBoons()
        {
            List<CombatItem> combatList = _log.CombatData.AllCombatItems;
            var skillIDs = new HashSet<long>(combatList.Select(x => x.SkillID));
            // Main boons
            foreach (Boon boon in Boon.GetBoonList())
            {
                if (skillIDs.Contains(boon.ID))
                {
                    _statistics.PresentBoons.Add(boon);
                }
            }
            // Main Conditions
            foreach (Boon boon in Boon.GetCondiBoonList())
            {
                if (skillIDs.Contains(boon.ID))
                {
                    _statistics.PresentConditions.Add(boon);
                }
            }

            // Important class specific boons
            foreach (Boon boon in Boon.GetOffensiveTableList())
            {
                if (skillIDs.Contains(boon.ID))
                {
                    _statistics.PresentOffbuffs.Add(boon);
                }
            }

            foreach (Boon boon in Boon.GetDefensiveTableList())
            {
                if (skillIDs.Contains(boon.ID))
                {
                    _statistics.PresentDefbuffs.Add(boon);
                }

            }

            var players = _log.PlayerList;
            Dictionary<ushort, Player> playersById = new Dictionary<ushort, Player>();
            foreach (Player player in players)
            {
                _statistics.PresentPersonalBuffs[player.InstID] = new HashSet<Boon>();
                playersById.Add(player.InstID, player);
            }
            // All class specific boons
            List<Boon> remainingBuffs = new List<Boon>(Boon.GetRemainingBuffsList());
            remainingBuffs.AddRange(Boon.GetConsumableList());
            Dictionary<long, Boon> remainingBuffsByIds = remainingBuffs.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList().FirstOrDefault());

            foreach (CombatItem item in combatList)
            {
                if (playersById.TryGetValue(item.DstInstid, out Player player))
                {
                    if (remainingBuffsByIds.TryGetValue(item.SkillID, out Boon boon))
                    {
                        _statistics.PresentPersonalBuffs[player.InstID].Add(boon);
                    }
                }
            }

        }
    }
}
