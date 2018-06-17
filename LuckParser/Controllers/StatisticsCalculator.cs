using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;

namespace LuckParser.Controllers
{
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

        private List<PhaseData> phases;

        public StatisticsCalculator(SettingsContainer settings)
        {
            this.settings = settings;
        }

        public Statistics calculateStatistics(ParsedLog log)
        {
            statistics = new Statistics();

            boss = log.getBoss();
            bossData = log.getBossData();
            combatData = log.getCombatData();
            agentData = log.getAgentData();
            players = log.getPlayerList();
            mechanicData = log.getMechanicData();

            phases = boss.getPhases(bossData, combatData.getCombatList(), agentData, settings.ParsePhases);

            calculateFinalDPS();
            calculateFinalStats();
            calculateFinalDefenses();

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

        private void calculateFinalDPS()
        {
            foreach (Player player in players)
            {
                Statistics.FinalDPS[] phaseDps = new Statistics.FinalDPS[phases.Count];
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    phaseDps[phaseIndex] = getFinalDPS(player,phaseIndex);
                }

                statistics.finalDps[player] = phaseDps;
            }

            Statistics.FinalDPS[] phaseBossDps = new Statistics.FinalDPS[phases.Count];
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                phaseBossDps[phaseIndex] = getFinalDPS(boss, phaseIndex);
            }

            statistics.finalBossDps = phaseBossDps;
        }

        private void calculateFinalStats()
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
                statistics.finalStats[player] = phaseStats;
            }
        }

        private void calculateFinalDefenses()
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
                statistics.finalDefenses[player] = phaseDefense;
            }
        }
    }
}
