using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class BossLogic
    {

        public enum ParseMode { Raid, Fractal, Golem, Unknown };
        
        protected List<Mechanic> mechanicList = new List<Mechanic>();
        protected ParseMode mode;
        public BossLogic()
        {
            mode = ParseMode.Unknown;
        }

        public virtual CombatReplayMap getCombatMap()
        {
            return null;
        }

        protected List<PhaseData> getInitialPhase(ParsedLog log)
        {
            List<PhaseData> phases = new List<PhaseData>();
            long fight_dur = log.getBossData().getAwareDuration();
            phases.Add(new PhaseData(0, fight_dur));
            phases[0].setName("Full Fight");
            return phases;
        }

        public virtual List<PhaseData> getPhases(Boss boss, ParsedLog log, List<CastLog> cast_logs)
        {
            // generic method
            long start = 0;
            long end = 0;
            long fight_dur = log.getBossData().getAwareDuration();
            List<PhaseData> phases = getInitialPhase(log);
            List<CombatItem> invulsBoss = log.getBoonData().Where(x => x.getSkillID() == 762 && boss.getInstid() == x.getDstInstid()).ToList();
            List<CombatItem> invulsBossFiltered = new List<CombatItem>();
            foreach (CombatItem c in invulsBoss)
            {
                if (invulsBossFiltered.Count > 0)
                {
                    CombatItem last = invulsBossFiltered.Last();
                    if (last.getTime() != c.getTime())
                    {
                        invulsBossFiltered.Add(c);
                    }
                }
                else
                {
                    invulsBossFiltered.Add(c);
                }
            }
            for (int i = 0; i < invulsBossFiltered.Count; i++)
            {
                CombatItem c = invulsBossFiltered[i];
                if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    end = c.getTime() - log.getBossData().getFirstAware();
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsBossFiltered.Count - 1)
                    {
                        cast_logs.Add(new CastLog(end, -5, (int)(fight_dur - end), ParseEnum.Activation.None, (int)(fight_dur - end), ParseEnum.Activation.None));
                    }
                }
                else
                {
                    start = c.getTime() - log.getBossData().getFirstAware();
                    cast_logs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                }
            }
            if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
            {
                phases.Add(new PhaseData(start, fight_dur));
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].setName("Phase " + i);
            }
            return phases;
        }

        public virtual List<ParseEnum.ThrashIDS> getAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>();
            return ids;
        }

        public virtual int isCM(List<CombatItem> clist, int health)
        {
            return -1;
        }

        public virtual void getAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
            return;
        }

        public virtual string getReplayIcon()
        {
            return "";
        }

        public List<Mechanic> getMechanics()
        {
            return mechanicList;
        }
       
        public ParseMode getMode()
        {
            return mode;
        }
        //
        protected static List<CombatItem> getFilteredList(ParsedLog log, long skillID, ushort instid)
        {
            bool needStart = true;
            List<CombatItem> main = log.getBoonData().Where(x => x.getSkillID() == skillID && ((x.getDstInstid() == instid && x.isBuffremove() == ParseEnum.BuffRemove.None) || (x.getSrcInstid() == instid && x.isBuffremove() != ParseEnum.BuffRemove.None))).ToList();
            List<CombatItem> filtered = new List<CombatItem>();
            foreach (CombatItem c in main)
            {
                if (needStart && c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    needStart = false;
                    filtered.Add(c);
                }
                else if (!needStart && c.isBuffremove() != ParseEnum.BuffRemove.None)
                {
                    needStart = true;
                    filtered.Add(c);
                }
            }
            return filtered;
        }
    }
}
