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
        private SettingsContainer settings;

        private Statistics statistics;

        private Boss boss;
        private BossData bossData;
        private CombatData combatData;
        private AgentData agentData;
        private List<Player> players;
        private MechanicData mechanicData;
        private SkillData skillData;

        private List<PhaseData> phases;

        public StatisticsCalculator(SettingsContainer settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Calculate a statistic from a log
        /// </summary>
        /// <param name="log">log to calculate stats from</param>
        /// <returns></returns>
        public Statistics calculateStatistics(ParsedLog log)
        {
            statistics = new Statistics();

            boss = log.getBoss();
            bossData = log.getBossData();
            combatData = log.getCombatData();
            agentData = log.getAgentData();
            players = log.getPlayerList();
            mechanicData = log.getMechanicData();
            skillData = log.getSkillData();

            phases = boss.getPhases(bossData, combatData.getCombatList(), agentData, settings.ParsePhases);

            calculateDPS();
            calculateStats();
            calculateDefenses();
            calculateSupport();
            calculateBoons();
            calculateConditions();

            return statistics;
        }

        private Statistics.FinalDPS getFinalDPS(AbstractPlayer player, int phaseIndex)
        {
            Statistics.FinalDPS final = new Statistics.FinalDPS();

            PhaseData phase = phases[phaseIndex];

            double phaseDuration = (phase.getDuration()) / 1000.0;

            double damage = 0.0;
            double dps = 0.0;

            // All DPS
            damage = player.getDamageLogs(0, bossData, combatData.getCombatList(), agentData, phase.getStart(),
                    phase.getEnd())
                .Sum(x => x.getDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }

            final.allDps = (int)dps;
            final.allDamage = (int)damage;

            // All Condi DPS
            damage = player.getDamageLogs(0, bossData, combatData.getCombatList(), agentData, phase.getStart(),
                    phase.getEnd())
                .Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }

            final.allCondiDps = (int)dps;
            final.allCondiDamage = (int)damage;

            // All Power DPS
            damage = final.allDamage - damage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }

            final.allPowerDps = (int)dps;
            final.allPowerDamage = (int)damage;

            // Boss DPS
            damage = player.getDamageLogs(bossData.getInstid(), bossData, combatData.getCombatList(), agentData,
                phase.getStart(), phase.getEnd()).Sum(x => x.getDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }

            final.bossDps = (int)dps;
            final.bossDamage = (int)damage;


            // Boss Condi DPS
            damage = player.getDamageLogs(bossData.getInstid(), bossData, combatData.getCombatList(), agentData,
                phase.getStart(), phase.getEnd()).Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }

            final.bossCondiDps = (int)dps;
            final.bossCondiDamage = (int)dps;

            // Boss Power DPS
            damage = final.bossDamage - damage;
            if (phaseDuration > 0)
            {
                dps = damage / phaseDuration;
            }

            final.bossPowerDps = (int)dps;
            final.bossPowerDamage = (int)damage;

            return final;
        }

        private void calculateDPS()
        {
            foreach (Player player in players)
            {
                Statistics.FinalDPS[] phaseDps = new Statistics.FinalDPS[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    phaseDps[phaseIndex] = getFinalDPS(player,phaseIndex);
                }

                statistics.dps[player] = phaseDps;
            }

            Statistics.FinalDPS[] phaseBossDps = new Statistics.FinalDPS[phases.Count];
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                phaseBossDps[phaseIndex] = getFinalDPS(boss, phaseIndex);
            }

            statistics.bossDps = phaseBossDps;
        }

        private void calculateStats()
        {
            foreach (Player player in players)
            {
                Statistics.FinalStats[] phaseStats = new Statistics.FinalStats[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    Statistics.FinalStats final = new Statistics.FinalStats();

                    PhaseData phase = phases[phaseIndex];
                    long start = phase.getStart() + bossData.getFirstAware();
                    long end = phase.getEnd() + bossData.getFirstAware();

                    List<DamageLog> damageLogs = player.getDamageLogs(0, bossData, combatData.getCombatList(), agentData, phase.getStart(), phase.getEnd());
                    List<CastLog> castLogs = player.getCastLogs(bossData, combatData.getCombatList(), agentData, phase.getStart(), phase.getEnd());

                    int instid = player.getInstid();

                    final.powerLoopCount = 0;
                    final.criticalRate = 0;
                    final.criticalDmg = 0;
                    final.scholarRate = 0;
                    final.scholarDmg = 0;
                    final.movingRate = 0;
                    final.flankingRate = 0;
                    final.glanceRate = 0;
                    final.missed = 0;
                    final.interupts = 0;
                    final.invulned = 0;
                    final.wasted = 0;
                    final.timeWasted = 0;
                    final.saved = 0;
                    final.timeSaved = 0;

                    foreach (DamageLog log in damageLogs)
                    {
                        if (log.isCondi() == 0)
                        {
                            if (log.getResult().getEnum() == "CRIT")
                            {
                                final.criticalRate++;
                                final.criticalDmg += log.getDamage();
                            }

                            if (log.isNinety() > 0)
                            {
                                final.scholarRate++;
                                final.scholarDmg += (int)(log.getDamage() / 11.0); //regular+10% damage
                            }

                            final.movingRate += log.isMoving();
                            final.flankingRate += log.isFlanking();

                            if (log.getResult().getEnum() == "GLANCE")
                            {
                                final.glanceRate++;
                            }

                            if (log.getResult().getEnum() == "BLIND")
                            {
                                final.missed++;
                            }

                            if (log.getResult().getEnum() == "INTERRUPT")
                            {
                                final.interupts++;
                            }

                            if (log.getResult().getEnum() == "ABSORB")
                            {
                                final.invulned++;
                            }
                            final.powerLoopCount++;
                        }
                    }
                    foreach (CastLog cl in castLogs)
                    {
                        if (cl.endActivation() != null)
                        {
                            if (cl.endActivation().getID() == 4)
                            {
                                final.wasted++;
                                final.timeWasted += cl.getActDur();
                            }
                            if (cl.endActivation().getID() == 3)
                            {
                                final.saved++;
                                if (cl.getActDur() < cl.getExpDur())
                                {
                                    final.timeSaved += cl.getExpDur() - cl.getActDur();
                                }
                            }
                        }
                    }

                    final.timeSaved = final.timeSaved / 1000f;
                    final.timeWasted = final.timeWasted / 1000f;

                    final.totalDmg = damageLogs.Sum(x => x.getDamage());
                    final.powerLoopCount = final.powerLoopCount == 0 ? 1 : final.powerLoopCount;

                    // Counts
                    final.swapCount = combatData.getStates(instid, "WEAPON_SWAP", start, end).Count();
                    final.downCount = combatData.getStates(instid, "CHANGE_DOWN", start, end).Count();
                    final.dodgeCount = combatData.getSkillCount(instid, 65001, start, end) + combatData.getBuffCount(instid, 40408, start, end);//dodge = 65001 mirage cloak =40408
                    final.ressCount = combatData.getSkillCount(instid, 1066, start, end); //Res = 1066

                    // R.I.P
                    List<CombatItem> dead = combatData.getStates(instid, "CHANGE_DEAD", start, end);
                    final.died = 0.0;
                    if (dead.Count() > 0)
                    {
                        final.died = dead[0].getTime() - start;
                    }

                    List<CombatItem> disconect = combatData.getStates(instid, "DESPAWN", start, end);
                    final.dcd = 0.0;
                    if (disconect.Count() > 0)
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
            foreach (Player player in players)
            {
                Statistics.FinalDefenses[] phaseDefense = new Statistics.FinalDefenses[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    Statistics.FinalDefenses final = new Statistics.FinalDefenses();

                    PhaseData phase = phases[phaseIndex];
                    long start = phase.getStart() + bossData.getFirstAware();
                    long end = phase.getEnd() + bossData.getFirstAware();

                    List<DamageLog> damageLogs = player.getDamageTakenLogs(bossData, combatData.getCombatList(), agentData, mechanicData, phase.getStart(), phase.getEnd());

                    int instID = player.getInstid();

                    // TODO damageBlocked and damageEvaded

                    final.damageTaken = damageLogs.Select(x => (long)x.getDamage()).Sum();
                    final.blockedCount = 0;
                    final.invulnedCount = 0;
                    final.damageInvulned = 0;
                    final.evadedCount = 0;
                    final.damageBarrier = 0;
                    foreach (DamageLog log in damageLogs.Where(x => x.getResult().getEnum() == "BLOCK"))
                    {
                        final.blockedCount++;
                    }
                    foreach (DamageLog log in damageLogs.Where(x => x.getResult().getEnum() == "ABSORB"))
                    {
                        final.invulnedCount++;
                        final.damageInvulned += log.getDamage();
                    }
                    foreach (DamageLog log in damageLogs.Where(x => x.getResult().getEnum() == "EVADE"))
                    {
                        final.evadedCount++;
                    }
                    foreach (DamageLog log in damageLogs.Where(x => x.isShields() == 1))
                    {
                        final.damageBarrier += log.getDamage();
                    }

                    phaseDefense[phaseIndex] = final;
                }
                statistics.defenses[player] = phaseDefense;
            }
        }

        // TODO ensure it is working correctly
        private void calculateSupport()
        {
            foreach (Player player in players)
            {
                Statistics.FinalSupport[] phaseSupport = new Statistics.FinalSupport[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    Statistics.FinalSupport final = new Statistics.FinalSupport();

                    PhaseData phase = phases[phaseIndex];
                    long start = phase.getStart() + bossData.getFirstAware();
                    long end = phase.getEnd() + bossData.getFirstAware();

                    // List<DamageLog> damage_logs = p.getDamageTakenLogs(b_data, c_data.getCombatList(), getAgentData());
                    int instid = player.getInstid();
                    int resurrects = 0;
                    double restime = 0.0;
                    int condiCleanse = 0;
                    double condiCleansetime = 0.0;

                    int[] resArray = player.getReses(bossData, combatData.getCombatList(), agentData, phase.getStart(), phase.getEnd());
                    int[] cleanseArray = player.getCleanses(bossData, combatData.getCombatList(), agentData, phase.getStart(), phase.getEnd());
                    final.resurrects = resArray[0];
                    final.ressurrectTime = resArray[1]/1000f;
                    final.condiCleanse = cleanseArray[0];
                    final.condiCleanseTime = cleanseArray[1]/1000f;

                    phaseSupport[phaseIndex] = final;
                }
                statistics.support[player] = phaseSupport;
            }
        }

        private Dictionary<int, Statistics.FinalBoonUptime> getBoonsForList(List<Player> playerList, Player player, int phaseIndex)
        {
            PhaseData phase = phases[phaseIndex];
            long fightDuration = phase.getEnd() - phase.getStart();

            Dictionary<Player, BoonDistribution> boonDistributions = new Dictionary<Player, BoonDistribution>();
            foreach (Player p in playerList)
            {
                boonDistributions[p] = p.getBoonDistribution(bossData, skillData, combatData.getCombatList(),
                    phases, phaseIndex);
            }

            Dictionary<int, Statistics.FinalBoonUptime> final =
                new Dictionary<int, Statistics.FinalBoonUptime>();

            foreach (Boon boon in Boon.getAllBuffList())
            {
                long totalGeneration = 0;
                long totalOverstack = 0;
                long totalUptime = 0;

                foreach (Player p in playerList)
                {
                    BoonDistribution boons = boonDistributions[p];
                    if (boons.ContainsKey(boon.getID()))
                    {
                        totalGeneration += boons.getGeneration(boon.getID(), player.getInstid());
                        totalOverstack += boons.getOverstack(boon.getID(), player.getInstid());
                        totalUptime += boons.getUptime(boon.getID());
                    }
                }

                Statistics.FinalBoonUptime uptime = new Statistics.FinalBoonUptime();

                if (boon.getType() == Boon.BoonType.Duration)
                {
                    uptime.uptime = Math.Round(100.0 * totalUptime / fightDuration / playerList.Count, 1);
                    uptime.generation = Math.Round(100.0f * totalGeneration / fightDuration / playerList.Count, 1);
                    uptime.overstack = Math.Round(100.0f * totalOverstack / fightDuration / playerList.Count, 1);
                }
                else if (boon.getType() == Boon.BoonType.Intensity)
                {
                    uptime.uptime = Math.Round((double)totalUptime / fightDuration / playerList.Count, 1);
                    uptime.generation = Math.Round((double)totalGeneration / fightDuration / playerList.Count, 1);
                    uptime.overstack = Math.Round((double)totalOverstack / fightDuration / playerList.Count, 1);
                }

                uptime.boonType = boon.getType();

                final[boon.getID()] = uptime;
            }

            return final;
        }

        private void calculateBoons()
        {
            // Player Boons
            foreach (Player player in players)
            {
                Dictionary<int, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<int, Statistics.FinalBoonUptime>[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    Dictionary<int, Statistics.FinalBoonUptime> final = new Dictionary<int, Statistics.FinalBoonUptime>();

                    PhaseData phase = phases[phaseIndex];

                    BoonDistribution selfBoons = player.getBoonDistribution(bossData, skillData, combatData.getCombatList(), phases, phaseIndex);

                    long fightDuration = phase.getEnd() - phase.getStart();
                    foreach (Boon boon in Boon.getAllBuffList())
                    {
                        Statistics.FinalBoonUptime uptime = new Statistics.FinalBoonUptime();

                        uptime.uptime = 0;
                        uptime.generation = 0;
                        uptime.overstack = 0;
                        if (selfBoons.ContainsKey(boon.getID()))
                        {
                            if (boon.getType() == Boon.BoonType.Duration)
                            {
                                uptime.uptime = Math.Round(100.0 * selfBoons.getUptime(boon.getID()) / fightDuration, 1);
                                uptime.generation = Math.Round(100.0f * selfBoons.getGeneration(boon.getID(), player.getInstid()) / fightDuration, 1);
                                uptime.overstack = Math.Round(100.0f * selfBoons.getOverstack(boon.getID(), player.getInstid()) / fightDuration, 1);
                            }
                            else if (boon.getType() == Boon.BoonType.Intensity)
                            {
                                uptime.uptime = Math.Round((double)selfBoons.getUptime(boon.getID()) / fightDuration, 1);
                                uptime.generation = Math.Round((double)selfBoons.getGeneration(boon.getID(), player.getInstid()) / fightDuration, 1);
                                uptime.overstack = Math.Round((double)selfBoons.getOverstack(boon.getID(), player.getInstid()) / fightDuration, 1);
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
            foreach (Player player in players)
            {
                Dictionary<int, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<int, Statistics.FinalBoonUptime>[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    List<Player> groupPlayers = new List<Player>();
                    foreach (Player p in players)
                    {
                        if (p.getGroup() == player.getGroup()) groupPlayers.Add(p);
                    }

                    phaseBoons[phaseIndex] = getBoonsForList(groupPlayers, player, phaseIndex);
                }
                statistics.groupBoons[player] = phaseBoons;
            }

            // Off Group Boons
            foreach (Player player in players)
            {
                Dictionary<int, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<int, Statistics.FinalBoonUptime>[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    List<Player> groupPlayers = new List<Player>();
                    foreach (Player p in players)
                    {
                        if (p.getGroup() != player.getGroup()) groupPlayers.Add(p);
                    }

                    phaseBoons[phaseIndex] = getBoonsForList(groupPlayers, player, phaseIndex);
                }
                statistics.offGroupBoons[player] = phaseBoons;
            }

            // Squad Boons
            foreach (Player player in players)
            {
                Dictionary<int, Statistics.FinalBoonUptime>[] phaseBoons = new Dictionary<int, Statistics.FinalBoonUptime>[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    List<Player> groupPlayers = new List<Player>();
                    foreach (Player p in players)
                    {
                        groupPlayers.Add(p);
                    }

                    phaseBoons[phaseIndex] = getBoonsForList(groupPlayers, player, phaseIndex);
                }
                statistics.squadBoons[player] = phaseBoons;
            }
        }

        public void calculateConditions()
        {
            statistics.bossConditions = new Dictionary<int, Statistics.FinalBossBoon>[phases.Count];
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                List<PhaseData> phases = boss.getPhases(bossData, combatData.getCombatList(), agentData, settings.ParsePhases);
                BoonDistribution boonDistribution = boss.getBoonDistribution(bossData, skillData, combatData.getCombatList(), phases, phaseIndex);
                Dictionary<int, Statistics.FinalBossBoon> rates = new Dictionary<int, Statistics.FinalBossBoon>();

                PhaseData phase = phases[phaseIndex];
                long fightDuration = phase.getDuration();

                foreach (Boon boon in Boon.getCondiBoonList())
                {
                    Statistics.FinalBossBoon condition = new Statistics.FinalBossBoon();
                    rates[boon.getID()] = condition;
                    if (boonDistribution.ContainsKey(boon.getID()))
                    {
                        if (boon.getType() == Boon.BoonType.Duration)
                        {
                            condition.boonType = Boon.BoonType.Duration;
                            condition.uptime = Math.Round(100.0 * boonDistribution.getUptime(boon.getID()) / fightDuration, 1);
                        }
                        else if (boon.getType() == Boon.BoonType.Intensity)
                        {
                            condition.boonType = Boon.BoonType.Intensity;
                            condition.uptime = Math.Round((double) boonDistribution.getUptime(boon.getID()) / fightDuration, 1);
                        }

                        rates[boon.getID()] = condition;
                    }
                }

                statistics.bossConditions[phaseIndex] = rates;
            }
        }
    }
}
