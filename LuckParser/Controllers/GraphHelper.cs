using System;
using System.Collections.Generic;
using System.Drawing;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;

namespace LuckParser.Controllers
{
    class GraphHelper
    {
        public static SettingsContainer Settings;

        public enum GraphMode { Full, S10, S30, S1 };

        private static List<Point> GetDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phaseIndex, PhaseData phase, ushort dstid, GraphMode mode)
        {
            int askedId = (phaseIndex + "_" + dstid + "_" + mode).GetHashCode();
            if (p.GetDPSGraph(askedId).Count > 0)
            {
                return p.GetDPSGraph(askedId);
            }

            List<Point> dmgList = new List<Point>();
            List<Point> dmgList1s = new List<Point>();
            List<Point> dmgList10s = new List<Point>();
            List<Point> dmgList30s = new List<Point>();
            List<DamageLog> damageLogs;
            if (dstid != 0 && phase.Redirection.Count > 0)
            {      
                damageLogs = p.GetDamageLogs(phase.Redirection, log, phase.Start, phase.End);
            } else
            {
                damageLogs = p.GetDamageLogs(dstid, log, phase.Start, phase.End);
            }
            // fill the graph, full precision
            List<double> dmgListFull = new List<double>();
            for (int i = 0; i <= phase.GetDuration(); i++)
            {
                dmgListFull.Add(0.0);
            }
            int totalTime = 1;
            int totalDamage = 0;
            foreach (DamageLog dl in damageLogs)
            {
                int time = (int)(dl.Time - phase.Start);
                // fill
                for (; totalTime < time; totalTime++)
                {
                    dmgListFull[totalTime] = totalDamage;
                }
                totalDamage += dl.Damage;
                dmgListFull[totalTime] = totalDamage;
            }
            // fill
            for (; totalTime <= phase.GetDuration(); totalTime++)
            {
                dmgListFull[totalTime] = totalDamage;
            }
            /*CombatReplay replay = p.Replay;
            if (replay != null && dstid == 0 && phaseIndex == 0)
            {
                foreach (int i in replay.GetTimes())
                {
                    int limitId = 0;
                    replay.AddDPS((int)Math.Round(1000 * (dmgListFull[i] - dmgListFull[limitId]) / (i - limitId)));
                    if (Settings.Show10s)
                    {
                        limitId = Math.Max(i - 10000, 0);
                        replay.AddDPS10s((int)Math.Round(1000 * (dmgListFull[i] - dmgListFull[limitId]) / (i - limitId)));
                    }
                    if (Settings.Show30s)
                    {
                        limitId = Math.Max(i - 30000, 0);
                        replay.AddDPS30s((int)Math.Round(1000 * (dmgListFull[i] - dmgListFull[limitId]) / (i - limitId)));
                    }
                }
            }*/
            dmgList.Add(new Point(0, 0));
            dmgList1s.Add(new Point(0, 0));
            dmgList10s.Add(new Point(0, 0));
            dmgList30s.Add(new Point(0, 0));
            for (int i = 1; i <= phase.GetDuration("s"); i++)
            {
                int limitId = 0;
                dmgList.Add(new Point(i, (int)Math.Round((dmgListFull[1000 * i] - dmgListFull[1000 * limitId]) / (i - limitId))));
                limitId = i - 1;
                dmgList1s.Add(new Point(i, (int)Math.Round((dmgListFull[1000 * i] - dmgListFull[1000 * limitId]) / (i - limitId))));
                if (Settings.Show10s)
                {
                    limitId = Math.Max(i - 10, 0);
                    dmgList10s.Add(new Point(i, (int)Math.Round((dmgListFull[1000 * i] - dmgListFull[1000 * limitId]) / (i - limitId))));
                }
                if (Settings.Show30s)
                {
                    limitId = Math.Max(i - 30, 0);
                    dmgList30s.Add(new Point(i, (int)Math.Round((dmgListFull[1000 * i] - dmgListFull[1000 * limitId]) / (i - limitId))));
                }
            }
            int id = (phaseIndex + "_" + dstid + "_" + GraphMode.Full).GetHashCode();
            p.DpsGraph[id] = dmgList;
            id = (phaseIndex + "_" + dstid + "_" + GraphMode.S1).GetHashCode();
            p.DpsGraph[id] = dmgList1s;
            if (Settings.Show10s)
            {
                id = (phaseIndex + "_" + dstid + "_" + GraphMode.S10).GetHashCode();
                p.DpsGraph[id] = dmgList10s;
            }
            if (Settings.Show30s)
            {
                id = (phaseIndex + "_" + dstid + "_" + GraphMode.S30).GetHashCode();
                p.DpsGraph[id] = dmgList30s;
            }
            return p.GetDPSGraph(askedId);
        }
        /// <summary>
        /// Gets the points for the boss dps graph for a given player
        /// </summary>
        /// <param name="log"></param>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        /// <param name="phase"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static List<Point> GetBossDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phaseIndex, PhaseData phase, GraphMode mode)
        {
            return GetDPSGraph(log, p, phaseIndex, phase, log.Boss.InstID, mode);
        }

        /// <summary>
        /// Gets the points for the total dps graph for a given player
        /// </summary>
        /// <param name="log"></param>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        /// <param name="phase"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static List<Point> GetTotalDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phaseIndex, PhaseData phase, GraphMode mode)
        {
            return GetDPSGraph(log, p, phaseIndex, phase, 0, mode);
        }

        /// <summary>
        /// Gets the points for the cleave dps graph for a given player
        /// </summary>
        /// <param name="log"></param>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        /// <param name="phase"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static List<Point> GetCleaveDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phaseIndex, PhaseData phase, GraphMode mode)
        {           
            int askedId = (phaseIndex + "_" + (-1) + "_" + mode).GetHashCode();
            if (p.GetDPSGraph(askedId).Count > 0)
            {
                return p.GetDPSGraph(askedId);
            }
            List<Point> total = GetTotalDPSGraph(log, p, phaseIndex, phase, mode);
            List<Point> boss = GetBossDPSGraph(log, p, phaseIndex, phase, mode);
            List<Point> cleave = new List<Point>();
            for (int i = 0; i < boss.Count; i++)
            {
                cleave.Add(new Point(boss[i].X, total[i].Y - boss[i].Y));
            }
            p.DpsGraph[askedId] = cleave;
            return cleave;
        }
    }
}
