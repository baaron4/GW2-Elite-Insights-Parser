using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;

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

            _statistics.Phases = log.Boss.GetPhases(log);
            if (switches.CalculateCombatReplay && _settings.ParseCombatReplay)
            {
                foreach (Player p in log.PlayerList)
                {
                    p.InitCombatReplay(log, _settings.PollingRate, false, true);
                }
                log.Boss.InitCombatReplay(log, _settings.PollingRate, false, true);
            }
            if (switches.CalculateDPS) CalculateDPS();
            if (switches.CalculateStats) CalculateStats();
            if (switches.CalculateDefense) CalculateDefenses();
            if (switches.CalculateSupport) CalculateSupport();
            if (switches.CalculateBoons)
            {
                SetPresentBoons();
                CalculateBoons();
            } 
                      
            if (switches.CalculateConditions) CalculateConditions();
            if (switches.CalculateMechanics)
            {
                log.Boss.AddMechanics(log);
                foreach (Player p in log.PlayerList)
                {
                    p.AddMechanics(log);
                }
                log.MechanicData.ComputePresentMechanics(log);
            }

            return _statistics;
        }

        private Statistics.FinalDPS GetFinalDPS(AbstractPlayer player, int phaseIndex, bool checkRedirection)
        {
            Statistics.FinalDPS final = new Statistics.FinalDPS();

            PhaseData phase = _statistics.Phases[phaseIndex];

            double phaseDuration = (phase.GetDuration()) / 1000.0;

            double damage;
            double dps = 0.0;

            ////////// ALL
            //DPS
            damage = player.GetDamageLogs(0, _log, phase.Start,
                    phase.End)
                .Sum(x => x.Damage);
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.AllDps = (int)dps;
            final.AllDamage = (int)damage;
            //Condi DPS
            damage = player.GetDamageLogs(0, _log, phase.Start,
                    phase.End)
                .Where(x => x.IsCondi > 0).Sum(x => x.Damage);
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.AllCondiDps = (int)dps;
            final.AllCondiDamage = (int)damage;
            //Power DPS
            damage = final.AllDamage - final.AllCondiDamage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.AllPowerDps = (int)dps;
            final.AllPowerDamage = (int)damage;
            final.PlayerPowerDamage = player.GetJustPlayerDamageLogs(0, _log,
                phase.Start, phase.End).Where(x => x.IsCondi == 0).Sum(x => x.Damage);
            /////////// BOSS
            //DPS
            if (checkRedirection && phase.Redirection.Count > 0)
            {
                damage = player.GetDamageLogs(phase.Redirection, _log,
                    phase.Start, phase.End).Sum(x => x.Damage);
            } else
            {
                damage = player.GetDamageLogs(_log.FightData.InstID, _log,
                    phase.Start, phase.End).Sum(x => x.Damage);
            }
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.BossDps = (int)dps;
            final.BossDamage = (int)damage;
            //Condi DPS
            if (checkRedirection && phase.Redirection.Count > 0)
            {
                damage = player.GetDamageLogs(phase.Redirection, _log,
                    phase.Start, phase.End).Where(x => x.IsCondi > 0).Sum(x => x.Damage);
            }
            else
            {
                damage = player.GetDamageLogs(_log.FightData.InstID, _log,
                    phase.Start, phase.End).Where(x => x.IsCondi > 0).Sum(x => x.Damage);
            }
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.BossCondiDps = (int)dps;
            final.BossCondiDamage = (int)damage;
            //Power DPS
            damage = final.BossDamage - final.BossCondiDamage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.BossPowerDps = (int)dps;
            final.BossPowerDamage = (int)damage;
            if (checkRedirection && phase.Redirection.Count > 0)
            {
                final.PlayerBossPowerDamage = player.GetJustPlayerDamageLogs(phase.Redirection, _log,
                    phase.Start, phase.End).Where(x => x.IsCondi == 0).Sum(x => x.Damage);
            }
            else
            {
                final.PlayerBossPowerDamage = player.GetJustPlayerDamageLogs(_log.FightData.InstID, _log,
                    phase.Start, phase.End).Where(x => x.IsCondi == 0).Sum(x => x.Damage);
            }

            return final;
        }

        private void CalculateDPS()
        {
            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalDPS[] phaseDps = new Statistics.FinalDPS[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex <_statistics.Phases.Count; phaseIndex++)
                {
                    phaseDps[phaseIndex] = GetFinalDPS(player,phaseIndex, true);
                }

                _statistics.Dps[player] = phaseDps;
            }

            Statistics.FinalDPS[] phaseBossDps = new Statistics.FinalDPS[_statistics.Phases.Count];
            for (int phaseIndex = 0; phaseIndex <_statistics.Phases.Count; phaseIndex++)
            {
                phaseBossDps[phaseIndex] = GetFinalDPS(_log.Boss, phaseIndex, false);
            }

            _statistics.BossDps = phaseBossDps;
        }

        private void CalculateStats()
        {
            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalStats[] phaseStats = new Statistics.FinalStats[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex <_statistics.Phases.Count; phaseIndex++)
                {
                    Statistics.FinalStats final = new Statistics.FinalStats();

                    PhaseData phase = _statistics.Phases[phaseIndex];
                    long start = phase.Start + _log.FightData.FightStart;
                    long end = phase.End + _log.FightData.FightStart;

                    List<DamageLog> damageLogs  = player.GetJustPlayerDamageLogs(0, _log, phase.Start, phase.End);
                    List<CastLog> castLogs = player.GetCastLogs(_log, phase.Start, phase.End);

                    int instid = player.InstID;

                    final.PowerLoopCount = 0;
                    final.CritablePowerLoopCount = 0;
                    final.CriticalRate = 0;
                    final.CriticalDmg = 0;
                    final.ScholarRate = 0;
                    final.ScholarDmg = 0;
                    final.MovingRate = 0;
                    final.MovingDamage = 0;
                    final.FlankingRate = 0;
                    final.GlanceRate = 0;
                    final.Missed = 0;
                    final.Interrupts = 0;
                    final.Invulned = 0;
                    final.Wasted = 0;
                    final.TimeWasted = 0;
                    final.Saved = 0;
                    final.TimeSaved = 0;
                    final.StackDist = 0;
                    
                    final.PowerLoopCountBoss = 0;
                    final.CritablePowerLoopCountBoss = 0;
                    final.CriticalRateBoss = 0;
                    final.CriticalDmgBoss = 0;
                    final.ScholarRateBoss = 0;
                    final.ScholarDmgBoss = 0;
                    final.MovingRateBoss = 0;
                    final.MovingDamageBoss = 0;
                    final.FlankingRateBoss = 0;
                    final.GlanceRateBoss = 0;
                    final.MissedBoss = 0;
                    final.InterruptsBoss = 0;
                    final.InvulnedBoss = 0;

                    // Add non critable sigil/rune procs here
                    HashSet<long> nonCritable = new HashSet<long>
                    {
                        9292
                    };
                    HashSet<long> idsToCheck = new HashSet<long>();
                    if (phase.Redirection.Count > 0)
                    {
                        foreach (AgentItem a in phase.Redirection)
                        {
                            idsToCheck.Add(a.InstID);
                        }
                    } else
                    {
                        idsToCheck.Add(_log.FightData.InstID);
                    }
                    foreach (DamageLog dl in damageLogs)
                    {
                        if (dl.IsCondi == 0)
                        {

                            if (idsToCheck.Contains(dl.DstInstId))
                            {
                                if (idsToCheck.Count > 1)
                                {
                                    AgentItem target = phase.Redirection.Find(x => x.InstID == dl.DstInstId);
                                    if (dl.Time < target.FirstAware - _log.FightData.FightStart || dl.Time > target.LastAware - _log.FightData.FightStart)
                                    {
                                        continue;
                                    }
                                }
                                if (dl.Result == ParseEnum.Result.Crit)
                                {
                                    final.CriticalRateBoss++;
                                    final.CriticalDmgBoss += dl.Damage;
                                }

                                if (dl.IsNinety > 0)
                                {
                                    final.ScholarRateBoss++;
                                    final.ScholarDmgBoss += (int)(dl.Damage / 11.0); //regular+10% damage
                                }

                                if (dl.IsMoving > 0)
                                {
                                    final.MovingRateBoss++;
                                    final.MovingDamageBoss += (int)(dl.Damage / 21.0);
                                }
                                
                                final.FlankingRateBoss += dl.IsFlanking;

                                if (dl.Result == ParseEnum.Result.Glance)
                                {
                                    final.GlanceRateBoss++;
                                }

                                if (dl.Result == ParseEnum.Result.Blind)
                                {
                                    final.MissedBoss++;
                                }

                                if (dl.Result == ParseEnum.Result.Interrupt)
                                {
                                    final.InterruptsBoss++;
                                }

                                if (dl.Result == ParseEnum.Result.Absorb)
                                {
                                    final.InvulnedBoss++;
                                }
                                final.PowerLoopCountBoss++;
                                if (!nonCritable.Contains(dl.SkillId))
                                {
                                    final.CritablePowerLoopCountBoss++;
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
                                final.ScholarDmg += (int)(dl.Damage / 11.0); //regular+10% damage
                            }

                            if (dl.IsMoving > 0)
                            {
                                final.MovingRate++;
                                final.MovingDamage += (int)(dl.Damage / 21.0);
                            }
                            
                            final.FlankingRate += dl.IsFlanking;

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
                            if (!nonCritable.Contains(dl.SkillId))
                            {
                                final.CritablePowerLoopCount++;
                            }
                        }
                    }
                    foreach (CastLog cl in castLogs)
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
                    
                    final.PowerLoopCount = final.PowerLoopCount == 0 ? 1 : final.PowerLoopCount;
                    
                    final.PowerLoopCountBoss = final.PowerLoopCountBoss == 0 ? 1 : final.PowerLoopCountBoss;

                    // Counts
                    CombatData combatData = _log.CombatData;
                    final.SwapCount = combatData.GetStates(instid, ParseEnum.StateChange.WeaponSwap, start, end).Count;
                    final.DownCount = combatData.GetStates(instid, ParseEnum.StateChange.ChangeDown, start, end).Count;
                    final.DodgeCount = combatData.GetSkillCount(instid, SkillItem.DodgeId, start, end) + combatData.GetBuffCount(instid, 40408, start, end);//dodge = 65001 mirage cloak =40408

                    //Stack Distance
                    if (_settings.ParseCombatReplay && _log.FightData.Logic.CanCombatReplay)
                    {
                        if (_statistics.StackCenterPositions == null)
                        {
                            _statistics.StackCenterPositions = new List<Point3D>();
                            List<List<Point3D>> GroupsPosList = new List<List<Point3D>>();
                            foreach (Player p in _log.PlayerList)
                            {
                                List<Point3D> list = p.CombatReplay.GetActivePositions();  
                                if (list.Count > 1)
                                {
                                    GroupsPosList.Add(list);
                                }
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
                                x = x /activePlayers;
                                y = y / activePlayers;
                                z = z / activePlayers;
                                _statistics.StackCenterPositions.Add(new Point3D(x, y, z, _settings.PollingRate * time));
                            }
                        }
                        List<Point3D> positions = player.CombatReplay.GetPositions().Where(x => x.Time >= phase.Start && x.Time <= phase.End).ToList();
                        int offset = player.CombatReplay.GetPositions().Count(x => x.Time < phase.Start);
                        if (positions.Count > 1)
                        {
                            List<float> distances = new List<float>();
                            for (int time = 0; time < positions.Count; time++)
                            {

                                float deltaX = positions[time].X - _statistics.StackCenterPositions[time + offset].X;
                                float deltaY = positions[time].Y - _statistics.StackCenterPositions[time + offset].Y;
                                //float deltaZ = positions[time].Z - Statistics.StackCenterPositions[time].Z;


                                distances.Add((float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
                            }
                            final.StackDist = distances.Sum() / distances.Count;
                        }
                        else
                        {
                            final.StackDist = -1;
                        }
                    }
                    // R.I.P
                    List<CombatItem> dead = combatData.GetStates(instid, ParseEnum.StateChange.ChangeDead, start, end);
                    final.Died = 0.0;
                    if (dead.Count > 0)
                    {
                        final.Died = dead.Last().Time - start;
                    }

                    List<CombatItem> disconnect = combatData.GetStates(instid, ParseEnum.StateChange.Despawn, start, end);
                    final.Dcd = 0.0;
                    if (disconnect.Count > 0)
                    {
                        final.Dcd = disconnect.Last().Time - start;
                    }

                    phaseStats[phaseIndex] = final;
                }
                _statistics.Stats[player] = phaseStats;
            }
        }

        private void CalculateDefenses()
        {
            foreach (Player player in _log.PlayerList)
            {
                Statistics.FinalDefenses[] phaseDefense = new Statistics.FinalDefenses[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex <_statistics.Phases.Count; phaseIndex++)
                {
                    Statistics.FinalDefenses final = new Statistics.FinalDefenses();

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
                Statistics.FinalSupport[] phaseSupport = new Statistics.FinalSupport[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex <_statistics.Phases.Count; phaseIndex++)
                {
                    Statistics.FinalSupport final = new Statistics.FinalSupport();

                    PhaseData phase =_statistics.Phases[phaseIndex];

                    int[] resArray = player.GetReses(_log, phase.Start, phase.End);
                    int[] cleanseArray = player.GetCleanses(_log, phase.Start, phase.End);
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

        private Dictionary<long, Statistics.FinalBoonUptime>[] GetBoonsForPlayers(List<Player> playerList, Player player)
        {
            Dictionary<long, Statistics.FinalBoonUptime>[] uptimesByPhase =
                new Dictionary<long, Statistics.FinalBoonUptime>[_statistics.Phases.Count];

            for (int phaseIndex = 0; phaseIndex < _statistics.Phases.Count; phaseIndex++)
            {
                PhaseData phase = _statistics.Phases[phaseIndex];
                long fightDuration = phase.End - phase.Start;

                Dictionary<Player, BoonDistribution> boonDistributions = new Dictionary<Player, BoonDistribution>();
                foreach (Player p in playerList)
                {
                    boonDistributions[p] = p.GetBoonDistribution(_log, phaseIndex);
                }

                Dictionary<long, Statistics.FinalBoonUptime> final =
                    new Dictionary<long, Statistics.FinalBoonUptime>();

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

                    Statistics.FinalBoonUptime uptime = new Statistics.FinalBoonUptime();

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
                Dictionary<long, Statistics.FinalBoonUptime>[] selfUptimesByPhase = new Dictionary<long, Statistics.FinalBoonUptime>[_statistics.Phases.Count];
                for (int phaseIndex = 0; phaseIndex <_statistics.Phases.Count; phaseIndex++)
                {
                    Dictionary<long, Statistics.FinalBoonUptime> final = new Dictionary<long, Statistics.FinalBoonUptime>();

                    PhaseData phase =_statistics.Phases[phaseIndex];

                    BoonDistribution selfBoons = player.GetBoonDistribution(_log, phaseIndex);

                    long fightDuration = phase.End - phase.Start;
                    foreach (Boon boon in player.BoonToTrack)
                    {
                        Statistics.FinalBoonUptime uptime = new Statistics.FinalBoonUptime
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
        }

        private void CalculateConditions()
        {
            _statistics.BossConditions = new Dictionary<long, Statistics.FinalBossBoon>[_statistics.Phases.Count];
            for (int phaseIndex = 0; phaseIndex <_statistics.Phases.Count; phaseIndex++)
            {
                BoonDistribution boonDistribution = _log.Boss.GetBoonDistribution(_log, phaseIndex);
                Dictionary<long, Statistics.FinalBossBoon> rates = new Dictionary<long, Statistics.FinalBossBoon>();

                PhaseData phase =_statistics.Phases[phaseIndex];
                long fightDuration = phase.GetDuration();

                foreach (Boon boon in _log.Boss.BoonToTrack)
                {
                    Statistics.FinalBossBoon condition = new Statistics.FinalBossBoon(_log.PlayerList);
                    rates[boon.ID] = condition;
                    if (boonDistribution.ContainsKey(boon.ID))
                    {
                        if (boon.Type == Boon.BoonType.Duration)
                        {
                            condition.Uptime = Math.Round(100.0 * boonDistribution.GetUptime(boon.ID) / fightDuration, 1);
                            foreach(Player p in _log.PlayerList)
                            {
                                long gen = boonDistribution.GetGeneration(boon.ID, p.InstID);
                                condition.Generated[p] = Math.Round(100.0 * gen / fightDuration, 1);
                                condition.Overstacked[p] = Math.Round(100.0 * (boonDistribution.GetOverstack(boon.ID, p.InstID) + gen) / fightDuration, 1);
                            }
                        }
                        else if (boon.Type == Boon.BoonType.Intensity)
                        {
                            condition.Uptime = Math.Round((double) boonDistribution.GetUptime(boon.ID) / fightDuration, 1);
                            foreach (Player p in _log.PlayerList)
                            {
                                long gen = boonDistribution.GetGeneration(boon.ID, p.InstID);
                                condition.Generated[p] = Math.Round((double) gen / fightDuration, 1);
                                condition.Overstacked[p] = Math.Round((double)(boonDistribution.GetOverstack(boon.ID, p.InstID)+ gen) / fightDuration, 1);
                            }
                        }

                        rates[boon.ID] = condition;
                    }
                }

                _statistics.BossConditions[phaseIndex] = rates;
            }
        }
        /// <summary>
        /// Checks the combat data and gets buffs that were present during the fight
        /// </summary>
        private void SetPresentBoons()
        {
            List<CombatItem> combatList = _log.CombatData;
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
            Dictionary<ushort, List<Boon>> presentPersonalBuffs = new Dictionary<ushort, List<Boon>>();
            if (_settings.PlayerBoonsAllProf)
            {
                var playersById = new Dictionary<ushort, Player>();
                foreach (var player in players)
                {
                    presentPersonalBuffs[player.InstID] = new List<Boon>();
                    playersById.Add(player.InstID, player);
                }
                // All class specific boons
                var remainingBoons = Boon.GetRemainingBuffsList();

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
                            presentPersonalBuffs[player.InstID].Add(boon);
                        }
                    }
                }
            }
            foreach (Player player in players)
            {
                player.BoonToTrack.AddRange(_statistics.PresentBoons);
                player.BoonToTrack.AddRange(_statistics.PresentOffbuffs);
                player.BoonToTrack.AddRange(_statistics.PresentDefbuffs);
                if(_settings.PlayerBoonsAllProf)
                {
                    player.BoonToTrack.AddRange(presentPersonalBuffs[player.InstID]);
                }
            }
            // boss boons
            Boss boss = _log.Boss;
            boss.BoonToTrack.AddRange(_statistics.PresentBoons);
            boss.BoonToTrack.AddRange(_statistics.PresentConditions);
            boss.BoonToTrack.AddRange(Boon.GetBossBoonList());
        }
    }
}
