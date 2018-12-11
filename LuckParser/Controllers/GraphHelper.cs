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

        private static List<Point> GetDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phaseIndex, PhaseData phase, AbstractPlayer target)
        {
            ulong targetId = target != null ? target.Agent : 0;
            int askedId = (phaseIndex + "_" + targetId + "_1S").GetHashCode();
            if (p.GetDPSGraph(askedId).Count > 0)
            {
                return p.GetDPSGraph(askedId);
            }
            
            List<Point> dmgList = new List<Point>();
            List<DamageLog> damageLogs = p.GetDamageLogs(target, log, phase.Start, phase.End);            
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
            for (int i = 1; i <= phase.GetDuration("s"); i++)
            {
                dmgList.Add(new Point(i, (int)Math.Round((dmgListFull[1000 * i] - dmgListFull[1000 * (i-1)]))));
            }
            if (phase.GetDuration("s") * 1000 != phase.GetDuration())
            {
                double lastDamage = dmgListFull[(int)phase.GetDuration()];
                double last1SDamage = dmgListFull[(int)phase.GetDuration("s") * 1000];
                double timeDiff = (phase.GetDuration()/1000.0 - phase.GetDuration("s"));
                int dps = (int)Math.Round((lastDamage - last1SDamage) / timeDiff);
                dmgList.Add(new Point((int)phase.GetDuration("s"), dps));
            }
            p.DpsGraph[askedId] = dmgList;
            return p.GetDPSGraph(askedId);
        }
        /// <summary>
        /// Gets the points for the target dps graph for a given player
        /// </summary>
        /// <param name="log"></param>
        /// <param name="p"></param>
        /// <param name="phaseIndex"></param>
        /// <param name="phase"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static List<Point> GetTargetDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phaseIndex, PhaseData phase, Target target)
        {
            return GetDPSGraph(log, p, phaseIndex, phase, target);
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
        public static List<Point> GetTotalDPSGraph(ParsedLog log, AbstractMasterPlayer p, int phaseIndex, PhaseData phase)
        {
            return GetDPSGraph(log, p, phaseIndex, phase, null);
        }
    }
}
