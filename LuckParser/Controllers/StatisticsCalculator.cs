using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using static LuckParser.Models.DataModels.Statistics;

namespace LuckParser.Controllers
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
            int seconds = (int)_statistics.Phases[0].GetDuration("s");
            _statistics.TargetHealth = new Dictionary<Target, double[]>();
            foreach (Target target in _log.FightData.Logic.Targets)
            {
                double[] health = new double[seconds + 1];
                int i = 0;
                double curHealth = 100.0;
                foreach (Point p in target.HealthOverTime)
                {
                    double hp = p.Y / 100.0;
                    int timeInPhase = 1 + (p.X - (int)_statistics.Phases[0].Start) / 1000;
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
                _statistics.TargetHealth[target] = health;
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

            double fiveGain = 1.0462962963;
            double tenGain = 1.09259259259;
            bool afterFlank = false;
            long prevTime = -1;
            long prevSkill = long.MinValue;
            foreach ( DamageLog dl in dls)
            {
                if (dl.IsCondi == 0)
                {
                    afterFlank = (afterFlank || ((dl.Time - prevTime) <= 1 && dl.SkillId == prevSkill));
                    foreach(var pair in targetsFinal)
                    {
                        Target target = pair.Key;
                        if (dl.DstInstId == target.InstID && dl.Time <= target.LastAware - _log.FightData.FightStart && dl.Time >= target.FirstAware - _log.FightData.FightStart)
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
                                targetFinal.ScholarDmg += (int)((fiveGain - 1.0) / fiveGain * dl.Damage);
                            }

                            if (dl.IsFifty > 0)
                            {
                                targetFinal.EagleRate++;
                                targetFinal.EagleDmg += (int)((tenGain - 1.0) / tenGain * dl.Damage);
                            }

                            if (dl.IsMoving > 0)
                            {
                                targetFinal.MovingRate++;
                                targetFinal.MovingDamage += (int)(dl.Damage / 21.0);
                            }
                            if (afterFlank)
                            {
                                targetFinal.FlankingDmg += (int)((tenGain - 1.0) / tenGain * dl.Damage);
                            }
                            if (dl.IsFlanking > 0)
                            {
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
                            targetFinal.PlayerPowerDamage += dl.Damage;
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
                        final.ScholarDmg += (int)((fiveGain - 1.0)/fiveGain * dl.Damage); 
                    }

                    if (dl.IsFifty > 0)
                    {
                        final.EagleRate++;
                        final.EagleDmg += (int)((tenGain - 1.0) / tenGain * dl.Damage);
                    }

                    if (dl.IsMoving > 0)
                    {
                        final.MovingRate++;
                        final.MovingDamage += (int)(dl.Damage / 21.0);
                    }
                    if (afterFlank)
                    {
                        final.FlankingDmg += (int)((tenGain - 1.0) / tenGain * dl.Damage);
                        //check if bonus consumed
                        afterFlank = ((dl.Time - prevTime) <= 1 && dl.SkillId == prevSkill);
                        prevTime = dl.Time;
                        prevSkill = dl.SkillId;
                    }
                    if (dl.IsFlanking > 0)
                    {
                        afterFlank = true;
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
                    final.PlayerPowerDamage += dl.Damage;
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
            long start = phase.Start + _log.FightData.FightStart;
            long end = phase.End + _log.FightData.FightStart;
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
                    final.Saved++;
                    if (cl.ActualDuration < cl.ExpectedDuration)
                    {
                        final.TimeSaved += cl.ExpectedDuration - cl.ActualDuration;
                    }
                }
            }
            final.TimeSaved = final.TimeSaved / 1000f;
            final.TimeWasted = final.TimeWasted / 1000f;

            double avgBoons = 0;
            foreach (long duration in p.GetBoonPresence(_log, phaseIndex).Values)
            {
                avgBoons += duration;
            }
            final.AvgBoons = avgBoons/ phase.GetDuration();

            double avgCondis = 0;
            foreach (long duration in p.GetCondiPresence(_log, phaseIndex).Values)
            {
                avgCondis += duration;
            }
            final.AvgConditions = avgCondis / phase.GetDuration();


            // Counts
            CombatData combatData = _log.CombatData;
            final.SwapCount = combatData.GetStates(p.InstID, ParseEnum.StateChange.WeaponSwap, start, end).Count;
            final.DownCount = combatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeDown, start, end).Count;
            final.DodgeCount = combatData.GetSkillCount(p.InstID, SkillItem.DodgeId, start, end) + combatData.GetBuffCount(p.InstID, 40408, start, end);//dodge = 65001 mirage cloak =40408

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

            List<CombatItem> dead = combatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeDead, start, end);
            List<CombatItem> up = combatData.GetStates(p.InstID, ParseEnum.StateChange.ChangeUp, start, end);
            final.Died = 0.0;
            if (dead.Count > 0)
            {
                final.Died = up.Count > 0 && up.Last().Time > dead.Last().Time ? -dead.Count : dead.Last().Time - start;
            }

            List<CombatItem> disconnect = combatData.GetStates(p.InstID, ParseEnum.StateChange.Despawn, start, end);
            final.Dcd = 0.0;
            if (disconnect.Count > 0)
            {
                final.Dcd = disconnect.Last().Time - start;
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
                for (int phaseIndex = 0; phaseIndex <_statistics.Phases.Count; phaseIndex++)
                {
                    phaseStats[phaseIndex] = GetFinalStats(player, phaseIndex);
                }
                _statistics.StatsAll[player] = phaseStats;
            }
        }

        private void CalculateDefenses()
        {
            foreach (Player player in _log.PlayerList)
            {
                FinalDefenses[] phaseDefense = new FinalDefenses[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex <_statistics.Phases.Count; phaseIndex++)
                {
                    FinalDefenses final = new FinalDefenses();

                    PhaseData phase =_statistics.Phases[phaseIndex];

                    List<DamageLog> damageLogs = player.GetDamageTakenLogs(_log, phase.Start, phase.End);
                    //List<DamageLog> healingLogs = player.getHealingReceivedLogs(log, phase.getStart(), phase.getEnd());
                 
                    final.DamageTaken = damageLogs.Sum(x => (long)x.Damage);
                    //final.allHealReceived = healingLogs.Sum(x => x.getDamage());
                    final.BlockedCount = damageLogs.Count(x => x.Result == ParseEnum.Result.Block);
                    final.InvulnedCount = 0;
                    final.DamageInvulned = 0;
                    final.EvadedCount = damageLogs.Count(x => x.Result == ParseEnum.Result.Evade);
                    final.DamageBarrier = damageLogs.Sum(x => x.IsShields == 1 ? x.Damage : 0);
                    foreach (DamageLog log in damageLogs.Where(x => x.Result == ParseEnum.Result.Absorb))
                    {
                        final.InvulnedCount++;
                        final.DamageInvulned += log.Damage;
                    }

                    phaseDefense[phaseIndex] = final;
                }
                _statistics.Defenses[player] = phaseDefense;
            }
        }

        private void CalculateSupport()
        {
            foreach (Player player in _log.PlayerList)
            {
                FinalSupport[] phaseSupport = new FinalSupport[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex <_statistics.Phases.Count; phaseIndex++)
                {
                    FinalSupport final = new FinalSupport();

                    PhaseData phase =_statistics.Phases[phaseIndex];

                    int[] resArray = player.GetReses(_log, phase.Start, phase.End);
                    int[] cleanseArray = player.GetCleanses(_log, phaseIndex);
                    //List<DamageLog> healingLogs = player.getHealingLogs(log, phase.getStart(), phase.getEnd());
                    //final.allHeal = healingLogs.Sum(x => x.getDamage());
                    final.Resurrects = resArray[0];
                    final.ResurrectTime = resArray[1]/1000f;
                    final.CondiCleanse = cleanseArray[0];
                    final.CondiCleanseTime = cleanseArray[1]/1000f;

                    phaseSupport[phaseIndex] = final;
                }
                _statistics.Support[player] = phaseSupport;
            }
        }

        private Dictionary<long, FinalBoonUptime>[] GetBoonsForPlayers(List<Player> playerList, Player player)
        {
            Dictionary<long, FinalBoonUptime>[] uptimesByPhase =
                new Dictionary<long, FinalBoonUptime>[_statistics.Phases.Count];

            for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
            {
                PhaseData phase = _statistics.Phases[phaseIndex];
                long fightDuration = phase.End - phase.Start;

                Dictionary<Player, BoonDistribution> boonDistributions = new Dictionary<Player, BoonDistribution>();
                foreach (Player p in playerList)
                {
                    boonDistributions[p] = p.GetBoonDistribution(_log, phaseIndex);
                }

                Dictionary<long, FinalBoonUptime> final =
                    new Dictionary<long, FinalBoonUptime>();

                foreach (Boon boon in player.BoonToTrack)
                {
                    long totalGeneration = 0;
                    long totalOverstack = 0;

                    foreach (BoonDistribution boons in boonDistributions.Values)
                    {
                        if (boons.ContainsKey(boon.ID))
                        {
                            totalGeneration += boons.GetGeneration(boon.ID, player.InstID);
                            totalOverstack += boons.GetOverstack(boon.ID, player.InstID);
                        }
                    }

                    FinalBoonUptime uptime = new FinalBoonUptime();

                    if (boon.Type == Boon.BoonType.Duration)
                    {
                        uptime.Generation = Math.Round(100.0f * totalGeneration / fightDuration / playerList.Count, 1);
                        uptime.Overstack = Math.Round(100.0f * (totalOverstack + totalGeneration) / fightDuration / playerList.Count, 1);
                    }
                    else if (boon.Type == Boon.BoonType.Intensity)
                    {
                        uptime.Generation = Math.Round((double) totalGeneration / fightDuration / playerList.Count, 1);
                        uptime.Overstack = Math.Round((double) (totalOverstack + totalGeneration) / fightDuration / playerList.Count, 1);
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
                Dictionary<long, FinalBoonUptime>[] selfUptimesByPhase = new Dictionary<long, FinalBoonUptime>[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex <_statistics.Phases.Count; phaseIndex++)
                {
                    Dictionary<long, FinalBoonUptime> final = new Dictionary<long, FinalBoonUptime>();

                    PhaseData phase =_statistics.Phases[phaseIndex];

                    BoonDistribution selfBoons = player.GetBoonDistribution(_log, phaseIndex);
                    Dictionary<long, long> boonPresence = player.GetBoonPresence(_log, phaseIndex);
                    Dictionary<long, long> condiPresence = player.GetCondiPresence(_log, phaseIndex);

                    long fightDuration = phase.End - phase.Start;
                    foreach (Boon boon in player.BoonToTrack)
                    {
                        FinalBoonUptime uptime = new FinalBoonUptime
                        {
                            Uptime = 0,
                            Generation = 0,
                            Overstack = 0
                        };
                        if (selfBoons.ContainsKey(boon.ID))
                        {
                            long generation = selfBoons.GetGeneration(boon.ID, player.InstID);
                            if (boon.Type == Boon.BoonType.Duration)
                            {
                                uptime.Uptime = Math.Round(100.0 * selfBoons.GetUptime(boon.ID) / fightDuration, 1);
                                uptime.Generation = Math.Round(100.0f * generation / fightDuration, 1);
                                uptime.Overstack = Math.Round(100.0f * (selfBoons.GetOverstack(boon.ID, player.InstID) + generation) / fightDuration, 1);
                            }
                            else if (boon.Type == Boon.BoonType.Intensity)
                            {
                                uptime.Uptime = Math.Round((double)selfBoons.GetUptime(boon.ID) / fightDuration, 1);
                                uptime.Generation = Math.Round((double)generation / fightDuration, 1);
                                uptime.Overstack = Math.Round((double)(selfBoons.GetOverstack(boon.ID, player.InstID) + generation) / fightDuration, 1);
                                if (boonPresence.TryGetValue(boon.ID, out long presenceValueBoon))
                                {
                                    uptime.Presence = Math.Round(100.0 * presenceValueBoon / fightDuration, 1);
                                } else if (condiPresence.TryGetValue(boon.ID, out long presenceValueCondi))
                                {
                                    uptime.Presence = Math.Round(100.0 * presenceValueCondi / fightDuration, 1);
                                }
                            }
                        }
                        final[boon.ID] = uptime;
                    }

                    selfUptimesByPhase[phaseIndex] = final;
                }
                _statistics.SelfBoons[player] = selfUptimesByPhase;

                // Boons applied to player's group
                var otherPlayersInGroup = _log.PlayerList
                    .Where(p => p.Group == player.Group && player.InstID != p.InstID)
                    .ToList();
                _statistics.GroupBoons[player] = GetBoonsForPlayers(otherPlayersInGroup, player);

                // Boons applied to other groups
                var offGroupPlayers = _log.PlayerList.Where(p => p.Group != player.Group).ToList();
                _statistics.OffGroupBoons[player] = GetBoonsForPlayers(offGroupPlayers, player);

                // Boons applied to squad
                var otherPlayers = _log.PlayerList.Where(p => p.InstID != player.InstID).ToList();
                _statistics.SquadBoons[player] = GetBoonsForPlayers(otherPlayers, player);
            }
            // a little hack to remove the contribution on boon average
            // to remove once we switch to new html builder
            Player CASword = _log.PlayerList.Find(x => x.Account == ":Conjured Sword");
            if (CASword != null)
            {
                var caBoons = _statistics.SelfBoons[CASword];
                List<Player> swordlessPList = _log.PlayerList.Where(x => x.Account != ":Conjured Sword").ToList();
                for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
                {
                    var caPhaseBoons = caBoons[phaseIndex];
                    foreach (Player p in swordlessPList)
                    {
                        var phaseBoons = _statistics.SelfBoons[p][phaseIndex];
                        foreach (long boonId in phaseBoons.Keys)
                        {
                            var uptime = phaseBoons[boonId];
                            if (caPhaseBoons.TryGetValue(boonId, out var caUptime))
                            {
                                caUptime.Uptime += uptime.Uptime;
                            }
                            else
                            {
                                caPhaseBoons[boonId] = new FinalBoonUptime()
                                {
                                    Uptime = uptime.Uptime
                                };
                            }
                        }
                    }
                    foreach (var uptime in caPhaseBoons.Values)
                    {
                        uptime.Uptime = Math.Round(uptime.Uptime/swordlessPList.Count,1);
                    }
                }
            }
        }

        private void CalculateConditions()
        {
            foreach (Target target in _log.FightData.Logic.Targets)
            {
                Dictionary<long, FinalTargetBoon>[] stats = new Dictionary<long, FinalTargetBoon>[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
                {
                    BoonDistribution boonDistribution = target.GetBoonDistribution(_log, phaseIndex);
                    Dictionary<long, FinalTargetBoon> rates = new Dictionary<long, FinalTargetBoon>();
                    Dictionary<long, long> boonPresence = target.GetBoonPresence(_log, phaseIndex);
                    Dictionary<long, long> condiPresence = target.GetCondiPresence(_log, phaseIndex);

                    PhaseData phase = _statistics.Phases[phaseIndex];
                    long fightDuration = phase.GetDuration();

                    foreach (Boon boon in target.BoonToTrack)
                    {
                        FinalTargetBoon condition = new FinalTargetBoon(_log.PlayerList);
                        rates[boon.ID] = condition;
                        if (boonDistribution.ContainsKey(boon.ID))
                        {
                            if (boon.Type == Boon.BoonType.Duration)
                            {
                                condition.Uptime = Math.Round(100.0 * boonDistribution.GetUptime(boon.ID) / fightDuration, 1);
                                foreach (Player p in _log.PlayerList)
                                {
                                    long gen = boonDistribution.GetGeneration(boon.ID, p.InstID);
                                    condition.Generated[p] = Math.Round(100.0 * gen / fightDuration, 1);
                                    condition.Overstacked[p] = Math.Round(100.0 * (boonDistribution.GetOverstack(boon.ID, p.InstID) + gen) / fightDuration, 1);
                                }
                            }
                            else if (boon.Type == Boon.BoonType.Intensity)
                            {
                                condition.Uptime = Math.Round((double)boonDistribution.GetUptime(boon.ID) / fightDuration, 1);
                                foreach (Player p in _log.PlayerList)
                                {
                                    long gen = boonDistribution.GetGeneration(boon.ID, p.InstID);
                                    condition.Generated[p] = Math.Round((double)gen / fightDuration, 1);
                                    condition.Overstacked[p] = Math.Round((double)(boonDistribution.GetOverstack(boon.ID, p.InstID) + gen) / fightDuration, 1);
                                }
                                if (boonPresence.TryGetValue(boon.ID, out long presenceValueBoon))
                                {
                                    condition.Presence = Math.Round(100.0 * presenceValueBoon / fightDuration, 1);
                                }
                                else if (condiPresence.TryGetValue(boon.ID, out long presenceValueCondi))
                                {
                                    condition.Presence = Math.Round(100.0 * presenceValueCondi / fightDuration, 1);
                                }
                            }

                            rates[boon.ID] = condition;
                        }
                    }
                    stats[phaseIndex] = rates;
                }
                _statistics.TargetConditions[target] = stats;
            }         
        }
        /// <summary>
        /// Checks the combat data and gets buffs that were present during the fight
        /// </summary>
        private void SetPresentBoons()
        {
            List<CombatItem> combatList = _log.CombatData.AllCombatItems;
            var skillIDs = new HashSet<long>(combatList.Select(x => x.SkillID));
            if (_settings.PlayerBoonsUniversal)
            {
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
            }

            if (_settings.PlayerBoonsImpProf)
            {
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
            }

            var players = _log.PlayerList;
            if (_settings.PlayerBoonsAllProf)
            {
                var playersById = new Dictionary<ushort, Player>();
                foreach (var player in players)
                {
                    _statistics.PresentPersonalBuffs[player.InstID] = new List<Boon>();
                    playersById.Add(player.InstID, player);
                }
                // All class specific boons
                var remainingBoons = new List<Boon>(Boon.GetRemainingBuffsList());
                remainingBoons.AddRange(Boon.GetConsumableList());
                var classSpecificBoonsById = new Dictionary<long, Boon>();
                foreach (var boon in remainingBoons)
                {
                    if (boon.ID == -1) continue;

                    classSpecificBoonsById.Add(boon.ID, boon);
                }

                foreach (var item in combatList)
                {
                    if (playersById.TryGetValue(item.DstInstid, out Player player))
                    {
                        if (classSpecificBoonsById.TryGetValue(item.SkillID, out Boon boon))
                        {
                            _statistics.PresentPersonalBuffs[player.InstID].Add(boon);
                        }
                    }
                }
            }
            foreach (Player player in players)
            {
                player.BoonToTrack.AddRange(_statistics.PresentBoons);
                player.BoonToTrack.AddRange(_statistics.PresentConditions);
                player.BoonToTrack.AddRange(_statistics.PresentOffbuffs);
                player.BoonToTrack.AddRange(_statistics.PresentDefbuffs);
                if(_settings.PlayerBoonsAllProf)
                {
                    player.BoonToTrack.AddRange(_statistics.PresentPersonalBuffs[player.InstID]);
                }
            }
            // target boons
            foreach (Target target in _log.FightData.Logic.Targets)
            {
                target.BoonToTrack.AddRange(_statistics.PresentBoons);
                target.BoonToTrack.AddRange(_statistics.PresentConditions);
                foreach (Boon boon in Boon.BoonsBySource[Boon.BoonSource.Enemy])
                {
                    if (_log.CombatData.BoonData.ContainsKey(boon.ID))
                    {
                        target.BoonToTrack.Add(boon);
                    }
                }
            }
        }
    }
}
