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
            public bool calculateDPS = false;
            public bool calculateStats = false;
            public bool calculateDefense = false;
            public bool calculateSupport = false;
            public bool calculateBoons = false;
            public bool calculateConditions = false;
            public bool calculateCombatReplay = false;
            public bool calculateMechanics = false;
        }

        private SettingsContainer settings;

        private Statistics statistics;

        private ParsedLog log;

        public StatisticsCalculator(SettingsContainer settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Calculate a statistic from a log
        /// </summary>
        /// <param name="log">log to calculate stats from</param>
        /// <returns></returns>
        public Statistics calculateStatistics(ParsedLog log, Switches switches)
        {
            statistics = new Statistics();

            this.log = log;

            statistics.phases = log.getBoss().getPhases(log, settings.ParsePhases);
            if (switches.calculateCombatReplay && settings.ParseCombatReplay)
            {
                foreach (Player p in log.getPlayerList())
                {
                    p.initCombatReplay(log, settings.PollingRate, false, true);
                }
                log.getBoss().initCombatReplay(log, settings.PollingRate, false, true);
            }
            if (switches.calculateDPS) calculateDPS();
            if (switches.calculateStats) calculateStats();
            if (switches.calculateDefense) calculateDefenses();
            if (switches.calculateSupport) calculateSupport();
            if (switches.calculateBoons)
            {
                setPresentBoons();
                calculateBoons();
            } 
                      
            if (switches.calculateConditions) calculateConditions();
            if (switches.calculateMechanics)
            {
                log.getBoss().addMechanics(log);
                foreach (Player p in log.getPlayerList())
                {
                    p.addMechanics(log);
                }
            }

            return statistics;
        }

        private Statistics.FinalDPS getFinalDPS(AbstractPlayer player, int phaseIndex, bool checkRedirection)
        {
            Statistics.FinalDPS final = new Statistics.FinalDPS();

            PhaseData phase = statistics.phases[phaseIndex];

            double phaseDuration = (phase.getDuration()) / 1000.0;

            double damage = 0.0;
            double dps = 0.0;

            ////////// ALL
            //DPS
            damage = player.getDamageLogs(0, log, phase.getStart(),
                    phase.getEnd())
                .Sum(x => x.getDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.allDps = (int)dps;
            final.allDamage = (int)damage;
            //Condi DPS
            damage = player.getDamageLogs(0, log, phase.getStart(),
                    phase.getEnd())
                .Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.allCondiDps = (int)dps;
            final.allCondiDamage = (int)damage;
            //Power DPS
            damage = final.allDamage - final.allCondiDamage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.allPowerDps = (int)dps;
            final.allPowerDamage = (int)damage;
            final.playerPowerDamage = player.getJustPlayerDamageLogs(0, log,
                phase.getStart(), phase.getEnd()).Where(x => x.isCondi() == 0).Sum(x => x.getDamage());
            /////////// BOSS
            //DPS
            if (checkRedirection && phase.getRedirection().Count > 0)
            {
                damage = player.getDamageLogs(phase.getRedirection(), log,
                    phase.getStart(), phase.getEnd()).Sum(x => x.getDamage());
            } else
            {
                damage = player.getDamageLogs(log.getBossData().getInstid(), log,
                    phase.getStart(), phase.getEnd()).Sum(x => x.getDamage());
            }
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.bossDps = (int)dps;
            final.bossDamage = (int)damage;
            //Condi DPS
            if (checkRedirection && phase.getRedirection().Count > 0)
            {
                damage = player.getDamageLogs(phase.getRedirection(), log,
                    phase.getStart(), phase.getEnd()).Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            }
            else
            {
                damage = player.getDamageLogs(log.getBossData().getInstid(), log,
                    phase.getStart(), phase.getEnd()).Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            }
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.bossCondiDps = (int)dps;
            final.bossCondiDamage = (int)damage;
            //Power DPS
            damage = final.bossDamage - final.bossCondiDamage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }
            final.bossPowerDps = (int)dps;
            final.bossPowerDamage = (int)damage;
            if (checkRedirection && phase.getRedirection().Count > 0)
            {
                final.playerBossPowerDamage = player.getJustPlayerDamageLogs(phase.getRedirection(), log,
                    phase.getStart(), phase.getEnd()).Where(x => x.isCondi() == 0).Sum(x => x.getDamage());
            }
            else
            {
                final.playerBossPowerDamage = player.getJustPlayerDamageLogs(log.getBossData().getInstid(), log,
                    phase.getStart(), phase.getEnd()).Where(x => x.isCondi() == 0).Sum(x => x.getDamage());
            }

            return final;
        }

        private void calculateDPS()
        {
            foreach (Player player in log.getPlayerList())
            {
                Statistics.FinalDPS[] phaseDps = new Statistics.FinalDPS[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {
                    phaseDps[phaseIndex] = getFinalDPS(player,phaseIndex, true);
                }

                statistics.dps[player] = phaseDps;
            }

            Statistics.FinalDPS[] phaseBossDps = new Statistics.FinalDPS[statistics.phases.Count];
            for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
            {
                phaseBossDps[phaseIndex] = getFinalDPS(log.getBoss(), phaseIndex, false);
            }

            statistics.bossDps = phaseBossDps;
        }

        private void calculateStats()
        {
            foreach (Player player in log.getPlayerList())
            {
                Statistics.FinalStats[] phaseStats = new Statistics.FinalStats[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {
                    Statistics.FinalStats final = new Statistics.FinalStats();

                    PhaseData phase = statistics.phases[phaseIndex];
                    long start = phase.getStart() + log.getBossData().getFirstAware();
                    long end = phase.getEnd() + log.getBossData().getFirstAware();

                    List<DamageLog> damageLogs  = player.getJustPlayerDamageLogs(0, log, phase.getStart(), phase.getEnd());
                    List<CastLog> castLogs = player.getCastLogs(log, phase.getStart(), phase.getEnd());

                    int instid = player.getInstid();

                    final.powerLoopCount = 0;
                    final.critablePowerLoopCount = 0;
                    final.criticalRate = 0;
                    final.criticalDmg = 0;
                    final.scholarRate = 0;
                    final.scholarDmg = 0;
                    final.movingRate = 0;
                    final.movingDamage = 0;
                    final.flankingRate = 0;
                    final.glanceRate = 0;
                    final.missed = 0;
                    final.interupts = 0;
                    final.invulned = 0;
                    final.wasted = 0;
                    final.timeWasted = 0;
                    final.saved = 0;
                    final.timeSaved = 0;
                    final.stackDist = 0;
                    
                    final.powerLoopCountBoss = 0;
                    final.critablePowerLoopCountBoss = 0;
                    final.criticalRateBoss = 0;
                    final.criticalDmgBoss = 0;
                    final.scholarRateBoss = 0;
                    final.scholarDmgBoss = 0;
                    final.movingRateBoss = 0;
                    final.movingDamageBoss = 0;
                    final.flankingRateBoss = 0;
                    final.glanceRateBoss = 0;
                    final.missedBoss = 0;
                    final.interuptsBoss = 0;
                    final.invulnedBoss = 0;

                    // Add non critable sigil/rune procs here
                    HashSet<long> nonCritable = new HashSet<long>
                    {
                        9292
                    };
                    HashSet<long> idsToCheck = new HashSet<long>();
                    if (phase.getRedirection().Count > 0)
                    {
                        foreach (AgentItem a in phase.getRedirection())
                        {
                            idsToCheck.Add(a.getInstid());
                        }
                    } else
                    {
                        idsToCheck.Add(log.getBossData().getInstid());
                    }
                    foreach (DamageLog dl in damageLogs)
                    {
                        if (dl.isCondi() == 0)
                        {

                            if (idsToCheck.Contains(dl.getDstInstidt()))
                            {
                                if (idsToCheck.Count > 1)
                                {
                                    AgentItem target = phase.getRedirection().Find(x => x.getInstid() == dl.getDstInstidt());
                                    if (dl.getTime() < target.getFirstAware() - log.getBossData().getFirstAware() || dl.getTime() > target.getLastAware() - log.getBossData().getFirstAware())
                                    {
                                        continue;
                                    }
                                }
                                if (dl.getResult() == ParseEnum.Result.Crit)
                                {
                                    final.criticalRateBoss++;
                                    final.criticalDmgBoss += dl.getDamage();
                                }

                                if (dl.isNinety() > 0)
                                {
                                    final.scholarRateBoss++;
                                    final.scholarDmgBoss += (int)(dl.getDamage() / 11.0); //regular+10% damage
                                }

                                if (dl.isMoving() > 0)
                                {
                                    final.movingRateBoss++;
                                    final.movingDamageBoss += (int)(dl.getDamage() / 21.0);
                                }
                                
                                final.flankingRateBoss += dl.isFlanking();

                                if (dl.getResult() == ParseEnum.Result.Glance)
                                {
                                    final.glanceRateBoss++;
                                }

                                if (dl.getResult() == ParseEnum.Result.Blind)
                                {
                                    final.missedBoss++;
                                }

                                if (dl.getResult() == ParseEnum.Result.Interrupt)
                                {
                                    final.interuptsBoss++;
                                }

                                if (dl.getResult() == ParseEnum.Result.Absorb)
                                {
                                    final.invulnedBoss++;
                                }
                                final.powerLoopCountBoss++;
                                if (!nonCritable.Contains(dl.getID()))
                                {
                                    final.critablePowerLoopCountBoss++;
                                }
                            }

                            if (dl.getResult() == ParseEnum.Result.Crit)
                            {
                                final.criticalRate++;
                                final.criticalDmg += dl.getDamage();
                            }

                            if (dl.isNinety() > 0)
                            {
                                final.scholarRate++;
                                final.scholarDmg += (int)(dl.getDamage() / 11.0); //regular+10% damage
                            }

                            if (dl.isMoving() > 0)
                            {
                                final.movingRate++;
                                final.movingDamage += (int)(dl.getDamage() / 21.0);
                            }
                            
                            final.flankingRate += dl.isFlanking();

                            if (dl.getResult() == ParseEnum.Result.Glance)
                            {
                                final.glanceRate++;
                            }

                            if (dl.getResult() == ParseEnum.Result.Blind)
                            {
                                final.missed++;
                            }

                            if (dl.getResult() == ParseEnum.Result.Interrupt)
                            {
                                final.interupts++;
                            }

                            if (dl.getResult() == ParseEnum.Result.Absorb)
                            {
                                final.invulned++;
                            }
                            final.powerLoopCount++;
                            if (!nonCritable.Contains(dl.getID()))
                            {
                                final.critablePowerLoopCount++;
                            }
                        }
                    }
                    foreach (CastLog cl in castLogs)
                    {
                        if (cl.endActivation() == ParseEnum.Activation.CancelCancel)
                        {
                            final.wasted++;
                            final.timeWasted += cl.getActDur();
                        }
                        if (cl.endActivation() == ParseEnum.Activation.CancelFire)
                        {
                            final.saved++;
                            if (cl.getActDur() < cl.getExpDur())
                            {
                                final.timeSaved += cl.getExpDur() - cl.getActDur();
                            }
                        }
                    }

                    final.timeSaved = final.timeSaved / 1000f;
                    final.timeWasted = final.timeWasted / 1000f;
                    
                    final.powerLoopCount = final.powerLoopCount == 0 ? 1 : final.powerLoopCount;
                    
                    final.powerLoopCountBoss = final.powerLoopCountBoss == 0 ? 1 : final.powerLoopCountBoss;

                    // Counts
                    CombatData combatData = log.getCombatData();
                    final.swapCount = combatData.getStates(instid, ParseEnum.StateChange.WeaponSwap, start, end).Count;
                    final.downCount = combatData.getStates(instid, ParseEnum.StateChange.ChangeDown, start, end).Count;
                    final.dodgeCount = combatData.getSkillCount(instid, 65001, start, end) + combatData.getBuffCount(instid, 40408, start, end);//dodge = 65001 mirage cloak =40408
                    final.ressCount = combatData.getSkillCount(instid, 1066, start, end); //Res = 1066

                    //Stack Distance
                    if (settings.ParseCombatReplay && log.getBoss().getCombatReplay() != null)
                    {
                        if (statistics.StackCenterPositions == null)
                        {
                            statistics.StackCenterPositions = new List<Point3D>();
                            List<List<Point3D>> GroupsPosList = new List<List<Point3D>>();
                            foreach (Player p in log.getPlayerList())
                            {
                                List<Point3D> list = p.getCombatReplay().getActivePositions();  
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
                                int active_players = GroupsPosList.Count;
                                for (int play = 0; play < GroupsPosList.Count; play++)
                                {
                                    Point3D point = GroupsPosList[play][time];
                                    if (point != null)
                                    {
                                        x += point.X;
                                        y += point.Y;
                                        z += point.Z;
                                    }
                                    else
                                    {
                                        active_players--;
                                    }
                                   
                                }
                                x = x /active_players;
                                y = y / active_players;
                                z = z / active_players;
                                statistics.StackCenterPositions.Add(new Point3D(x, y, z, time));
                            }
                        }
                        List<Point3D> positions = player.getCombatReplay().getPositions().Where(x => x.time >= phase.getStart() && x.time <= phase.getEnd()).ToList();
                        int offset = player.getCombatReplay().getPositions().Count(x => x.time < phase.getStart());
                        if (positions.Count > 1)
                        {
                            List<float> distances = new List<float>();
                            for (int time = 0; time < positions.Count; time++)
                            {

                                float deltaX = positions[time].X - statistics.StackCenterPositions[time + offset].X;
                                float deltaY = positions[time].Y - statistics.StackCenterPositions[time + offset].Y;
                                //float deltaZ = positions[time].Z - Statistics.StackCenterPositions[time].Z;


                                distances.Add((float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
                            }
                            final.stackDist = distances.Sum() / distances.Count;
                        }
                        else
                        {
                            final.stackDist = -1;
                        }
                    }
                    // R.I.P
                    List<CombatItem> dead = combatData.getStates(instid, ParseEnum.StateChange.ChangeDead, start, end);
                    final.died = 0.0;
                    if (dead.Count > 0)
                    {
                        final.died = dead[0].getTime() - start;
                    }

                    List<CombatItem> disconect = combatData.getStates(instid, ParseEnum.StateChange.Despawn, start, end);
                    final.dcd = 0.0;
                    if (disconect.Count > 0)
                    {
                        final.dcd = disconect[0].getTime() - start;
                    }

                    phaseStats[phaseIndex] = final;
                }
                statistics.stats[player] = phaseStats;
            }
        }

        private void calculateDefenses()
        {
            foreach (Player player in log.getPlayerList())
            {
                Statistics.FinalDefenses[] phaseDefense = new Statistics.FinalDefenses[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {
                    Statistics.FinalDefenses final = new Statistics.FinalDefenses();

                    PhaseData phase =statistics.phases[phaseIndex];
                    long start = phase.getStart() + log.getBossData().getFirstAware();
                    long end = phase.getEnd() + log.getBossData().getFirstAware();

                    List<DamageLog> damageLogs = player.getDamageTakenLogs(log, phase.getStart(), phase.getEnd());
                    //List<DamageLog> healingLogs = player.getHealingReceivedLogs(log, phase.getStart(), phase.getEnd());

                    int instID = player.getInstid();
                 
                    final.damageTaken = damageLogs.Sum(x => (long)x.getDamage());
                    //final.allHealReceived = healingLogs.Sum(x => x.getDamage());
                    final.blockedCount = damageLogs.Count(x => x.getResult() == ParseEnum.Result.Block);
                    final.invulnedCount = 0;
                    final.damageInvulned = 0;
                    final.evadedCount = damageLogs.Count(x => x.getResult() == ParseEnum.Result.Evade);
                    final.damageBarrier = damageLogs.Sum(x => x.isShields() == 1 ? x.getDamage() : 0);
                    foreach (DamageLog log in damageLogs.Where(x => x.getResult() == ParseEnum.Result.Absorb))
                    {
                        final.invulnedCount++;
                        final.damageInvulned += log.getDamage();
                    }

                    phaseDefense[phaseIndex] = final;
                }
                statistics.defenses[player] = phaseDefense;
            }
        }

       
        private void calculateSupport()
        {
            foreach (Player player in log.getPlayerList())
            {
                Statistics.FinalSupport[] phaseSupport = new Statistics.FinalSupport[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {
                    Statistics.FinalSupport final = new Statistics.FinalSupport();

                    PhaseData phase =statistics.phases[phaseIndex];
                    long start = phase.getStart() + log.getBossData().getFirstAware();
                    long end = phase.getEnd() + log.getBossData().getFirstAware();

                    // List<DamageLog> damage_logs = p.getDamageTakenLogs(b_data, c_data.getCombatList(), getAgentData());
                    int instid = player.getInstid();

                    int[] resArray = player.getReses(log, phase.getStart(), phase.getEnd());
                    int[] cleanseArray = player.getCleanses(log, phase.getStart(), phase.getEnd());
                    //List<DamageLog> healingLogs = player.getHealingLogs(log, phase.getStart(), phase.getEnd());
                    //final.allHeal = healingLogs.Sum(x => x.getDamage());
                    final.resurrects = resArray[0];
                    final.ressurrectTime = resArray[1]/1000f;
                    final.condiCleanse = cleanseArray[0];
                    final.condiCleanseTime = cleanseArray[1]/1000f;

                    phaseSupport[phaseIndex] = final;
                }
                statistics.support[player] = phaseSupport;
            }
        }

        private Dictionary<long, Statistics.FinalBoonUptime> getBoonsForList(List<Player> playerList, Player player, List<Boon> to_track, int phaseIndex)
        {
            PhaseData phase =statistics.phases[phaseIndex];
            long fightDuration = phase.getEnd() - phase.getStart();

            Dictionary<Player, BoonDistribution> boonDistributions = new Dictionary<Player, BoonDistribution>();
            foreach (Player p in playerList)
            {
                boonDistributions[p] = p.getBoonDistribution(log,statistics.phases, to_track, phaseIndex);
            }

            Dictionary<long, Statistics.FinalBoonUptime> final =
                new Dictionary<long, Statistics.FinalBoonUptime>();

            foreach (Boon boon in to_track)
            {
                long totalGeneration = 0;
                long totalOverstack = 0;

                foreach (BoonDistribution boons in boonDistributions.Values)
                {
                    if (boons.ContainsKey(boon.getID()))
                    {
                        totalGeneration += boons.getGeneration(boon.getID(), player.getInstid());
                        totalOverstack += boons.getOverstack(boon.getID(), player.getInstid());
                    }
                }

                Statistics.FinalBoonUptime uptime = new Statistics.FinalBoonUptime();

                if (boon.getType() == Boon.BoonType.Duration)
                {
                    uptime.generation = Math.Round(100.0f * totalGeneration / fightDuration / playerList.Count, 1);
                    uptime.overstack = Math.Round(100.0f * (totalOverstack + totalGeneration)/ fightDuration / playerList.Count, 1);
                }
                else if (boon.getType() == Boon.BoonType.Intensity)
                {
                    uptime.generation = Math.Round((double)totalGeneration / fightDuration / playerList.Count, 1);
                    uptime.overstack = Math.Round((double)(totalOverstack + totalGeneration) / fightDuration / playerList.Count, 1);
                }

                uptime.boonType = boon.getType();

                final[boon.getID()] = uptime;
            }

            return final;
        }

        private void calculateBoons()
        {
            // Player Boons
            foreach (Player player in log.getPlayerList())
            {
                List<Boon> boon_to_track = new List<Boon>();
                boon_to_track.AddRange(statistics.present_boons);
                boon_to_track.AddRange(statistics.present_offbuffs);
                boon_to_track.AddRange(statistics.present_defbuffs);
                boon_to_track.AddRange(statistics.present_personnal[player.getInstid()]);
                Dictionary<long, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<long, Statistics.FinalBoonUptime>[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {
                    Dictionary<long, Statistics.FinalBoonUptime> final = new Dictionary<long, Statistics.FinalBoonUptime>();

                    PhaseData phase =statistics.phases[phaseIndex];

                    BoonDistribution selfBoons = player.getBoonDistribution(log,statistics.phases, boon_to_track, phaseIndex);

                    long fightDuration = phase.getEnd() - phase.getStart();
                    foreach (Boon boon in boon_to_track)
                    {
                        Statistics.FinalBoonUptime uptime = new Statistics.FinalBoonUptime();

                        uptime.uptime = 0;
                        uptime.generation = 0;
                        uptime.overstack = 0;
                        if (selfBoons.ContainsKey(boon.getID()))
                        {
                            long generation = selfBoons.getGeneration(boon.getID(), player.getInstid());
                            if (boon.getType() == Boon.BoonType.Duration)
                            {
                                uptime.uptime = Math.Round(100.0 * selfBoons.getUptime(boon.getID()) / fightDuration, 1);
                                uptime.generation = Math.Round(100.0f * generation / fightDuration, 1);
                                uptime.overstack = Math.Round(100.0f * (selfBoons.getOverstack(boon.getID(), player.getInstid()) + generation) / fightDuration, 1);
                            }
                            else if (boon.getType() == Boon.BoonType.Intensity)
                            {
                                uptime.uptime = Math.Round((double)selfBoons.getUptime(boon.getID()) / fightDuration, 1);
                                uptime.generation = Math.Round((double)generation / fightDuration, 1);
                                uptime.overstack = Math.Round((double)(selfBoons.getOverstack(boon.getID(), player.getInstid()) + generation) / fightDuration, 1);
                            }

                            uptime.boonType = boon.getType();
                        }
                        final[boon.getID()] = uptime;
                    }

                    phaseBoons[phaseIndex] = final;
                }
                statistics.selfBoons[player] = phaseBoons;
            }

            // Group Boons
            foreach (Player player in log.getPlayerList())
            {
                List<Boon> boon_to_track = new List<Boon>();
                boon_to_track.AddRange(statistics.present_boons);
                boon_to_track.AddRange(statistics.present_offbuffs);
                boon_to_track.AddRange(statistics.present_defbuffs);
                boon_to_track.AddRange(statistics.present_personnal[player.getInstid()]);
                List<Player> groupPlayers = new List<Player>();
                foreach (Player p in log.getPlayerList())
                {
                    if (p.getGroup() == player.getGroup() && player.getInstid() != p.getInstid()) groupPlayers.Add(p);
                }
                Dictionary<long, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<long, Statistics.FinalBoonUptime>[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {
                    phaseBoons[phaseIndex] = getBoonsForList(groupPlayers, player, boon_to_track, phaseIndex);
                }
                statistics.groupBoons[player] = phaseBoons;
            }

            // Off Group Boons
            foreach (Player player in log.getPlayerList())
            {
                List<Boon> boon_to_track = new List<Boon>();
                boon_to_track.AddRange(statistics.present_boons);
                boon_to_track.AddRange(statistics.present_offbuffs);
                boon_to_track.AddRange(statistics.present_defbuffs);
                boon_to_track.AddRange(statistics.present_personnal[player.getInstid()]);
                List<Player> groupPlayers = new List<Player>();
                foreach (Player p in log.getPlayerList())
                {
                    if (p.getGroup() != player.getGroup()) groupPlayers.Add(p);
                }
                Dictionary<long, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<long, Statistics.FinalBoonUptime>[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {                    
                    phaseBoons[phaseIndex] = getBoonsForList(groupPlayers, player, boon_to_track, phaseIndex);
                }
                statistics.offGroupBoons[player] = phaseBoons;
            }

            // Squad Boons
            foreach (Player player in log.getPlayerList())
            {
                List<Boon> boon_to_track = new List<Boon>();
                boon_to_track.AddRange(statistics.present_boons);
                boon_to_track.AddRange(statistics.present_offbuffs);
                boon_to_track.AddRange(statistics.present_defbuffs);
                boon_to_track.AddRange(statistics.present_personnal[player.getInstid()]);
                List<Player> groupPlayers = new List<Player>();
                foreach (Player p in log.getPlayerList())
                {
                    if (p.getInstid() != player.getInstid())
                        groupPlayers.Add(p);
                }
                Dictionary<long, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<long, Statistics.FinalBoonUptime>[statistics.phases.Count];
                for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
                {                
                    phaseBoons[phaseIndex] = getBoonsForList(groupPlayers, player, boon_to_track, phaseIndex);
                }
                statistics.squadBoons[player] = phaseBoons;
            }
        }

        public void calculateConditions()
        {
            statistics.bossConditions = new Dictionary<long, Statistics.FinalBossBoon>[statistics.phases.Count];
            List<Boon> boon_to_track = Boon.getCondiBoonList();
            boon_to_track.AddRange(Boon.getBoonList());
            for (int phaseIndex = 0; phaseIndex <statistics.phases.Count; phaseIndex++)
            {
                BoonDistribution boonDistribution = log.getBoss().getBoonDistribution(log,statistics.phases, boon_to_track, phaseIndex);
                Dictionary<long, Statistics.FinalBossBoon> rates = new Dictionary<long, Statistics.FinalBossBoon>();

                PhaseData phase =statistics.phases[phaseIndex];
                long fightDuration = phase.getDuration();

                foreach (Boon boon in boon_to_track)
                {
                    Statistics.FinalBossBoon condition = new Statistics.FinalBossBoon(log.getPlayerList());
                    rates[boon.getID()] = condition;
                    if (boonDistribution.ContainsKey(boon.getID()))
                    {
                        if (boon.getType() == Boon.BoonType.Duration)
                        {
                            condition.boonType = Boon.BoonType.Duration;
                            condition.uptime = Math.Round(100.0 * boonDistribution.getUptime(boon.getID()) / fightDuration, 1);
                            foreach(Player p in log.getPlayerList())
                            {
                                long gen = boonDistribution.getGeneration(boon.getID(), p.getInstid());
                                condition.generated[p] = Math.Round(100.0 * gen / fightDuration, 1);
                                condition.overstacked[p] = Math.Round(100.0 * (boonDistribution.getOverstack(boon.getID(), p.getInstid()) + gen) / fightDuration, 1);
                            }
                        }
                        else if (boon.getType() == Boon.BoonType.Intensity)
                        {
                            condition.boonType = Boon.BoonType.Intensity;
                            condition.uptime = Math.Round((double) boonDistribution.getUptime(boon.getID()) / fightDuration, 1);
                            foreach (Player p in log.getPlayerList())
                            {
                                long gen = boonDistribution.getGeneration(boon.getID(), p.getInstid());
                                condition.generated[p] = Math.Round((double) gen / fightDuration, 1);
                                condition.overstacked[p] = Math.Round((double)(boonDistribution.getOverstack(boon.getID(), p.getInstid())+ gen) / fightDuration, 1);
                            }
                        }

                        rates[boon.getID()] = condition;
                    }
                }

                statistics.bossConditions[phaseIndex] = rates;
            }
        }
        /// <summary>
        /// Checks the combat data and gets buffs that were present during the fight
        /// </summary>
        private void setPresentBoons()
        {
            List<CombatItem> c_list = log.getCombatData().getCombatList();
            if (settings.PlayerBoonsUniversal)
            {//Main boons
                foreach (Boon boon in Boon.getBoonList())
                {
                    if (c_list.Exists(x => x.getSkillID() == boon.getID()))
                    {
                        statistics.present_boons.Add(boon);
                    }
                }
            }
            if (settings.PlayerBoonsImpProf)
            {//Important Class specefic boons
                foreach (Boon boon in Boon.getOffensiveTableList())
                {
                    if (c_list.Exists(x => x.getSkillID() == boon.getID()))
                    {
                        statistics.present_offbuffs.Add(boon);
                    }
                }
                foreach (Boon boon in Boon.getDefensiveTableList())
                {
                    if (c_list.Exists(x => x.getSkillID() == boon.getID()))
                    {
                        statistics.present_defbuffs.Add(boon);
                    }
                }
            }

            foreach (Player p in log.getPlayerList())
            {
                statistics.present_personnal[p.getInstid()] = new List<Boon>();
                if (settings.PlayerBoonsAllProf)
                {//All class specefic boons
                    List<Boon> notYetFoundBoons = Boon.getRemainingBuffsList();
                    c_list.ForEach(item =>
                    {
                        if (item.getDstInstid() == p.getInstid()) {
                            Boon foundBoon = notYetFoundBoons.Find(boon => boon.getID() == item.getSkillID());
                            if (foundBoon != null)
                            {
                                notYetFoundBoons.Remove(foundBoon);
                                statistics.present_personnal[p.getInstid()].Add(foundBoon);
                            }
                        }
                    });
                }
            }
        }
    }
}
