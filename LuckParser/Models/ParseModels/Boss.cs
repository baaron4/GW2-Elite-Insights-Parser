using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Boss : AbstractPlayer
    {
        
        public struct PhaseData
        {
            public long start;
            public long end;

            public PhaseData(long start, long end)
            {
                this.start = start;
                this.end = end;
            }
        }
        
        // Constructors
        public Boss(AgentItem agent) : base(agent)
        {
            prof = "Boss";
        }

        private List<PhaseData> phases = new List<PhaseData>();
        private List<long> phaseData = new List<long>();

        public List<PhaseData> getPhases(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            if (phases.Count == 0)
            {
                setPhases(bossData, combatList, agentData);
            }
            return phases;
        }

        public void addPhaseData(long data)
        {
            phaseData.Add(data);
        }
        
        // Private Methods
        private void setPhases(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long fight_dur = bossData.getAwareDuration();
            phases.Add(new PhaseData(0, fight_dur));
            string name = getCharacter();
            long start = 0;
            long end = 0;
            getCastLogs(bossData, combatList, agentData, 0, fight_dur);
            switch (name)
            {
                case "Vale Guardian":
                    // Invul check
                    List<CombatItem> invulsVG = combatList.Where(x => x.getSkillID() == 757 && getInstid() == x.getDstInstid() && x.isBuff() == 1).ToList();
                    for (int i = 0; i < invulsVG.Count; i++)
                    {
                        CombatItem c = invulsVG[i];
                        if (c.isBuffremove().getID() == 0)
                        {
                            end = c.getTime() - bossData.getFirstAware();
                            phases.Add(new PhaseData(start, end));
                            if (i == invulsVG.Count - 1)
                            {
                                cast_logs.Add(new CastLog(end, -5, (int)(fight_dur - end), new ParseEnums.Activation(0), (int)(fight_dur - end), new ParseEnums.Activation(0)));
                            }
                        }
                        else
                        {
                            start = c.getTime() - bossData.getFirstAware();
                            cast_logs.Add(new CastLog(end, -5, (int)(start - end), new ParseEnums.Activation(0), (int)(start - end), new ParseEnums.Activation(0)));
                        }
                    }
                    break;
                case "Gorseval the Multifarious":
                    // Ghostly protection check
                    List<CastLog> clsG = cast_logs.Where(x => x.getID() == 31759).ToList();
                    foreach (CastLog cl in clsG)
                    {
                        end = cl.getTime();
                        phases.Add(new PhaseData(start, end));
                        start = end + cl.getActDur();
                    }
                    break;
                case "Sabetha the Saboteur":
                    // Invul check
                    List<CombatItem> invulsSab = combatList.Where(x => x.getSkillID() == 757 && getInstid() == x.getDstInstid() && x.isBuff() == 1).ToList();
                    for (int i = 0; i < invulsSab.Count; i++)
                    {
                        CombatItem c = invulsSab[i];
                        if (c.isBuffremove().getID() == 0)
                        {
                            end = c.getTime() - bossData.getFirstAware();
                            phases.Add(new PhaseData(start, end));
                            if (i == invulsSab.Count - 1)
                            {
                                cast_logs.Add(new CastLog(end, -5, (int)(fight_dur - end), new ParseEnums.Activation(0), (int)(fight_dur - end), new ParseEnums.Activation(0)));
                            }
                        }
                        else
                        {
                            start = c.getTime() - bossData.getFirstAware();
                            cast_logs.Add(new CastLog(end, -5, (int)(start - end), new ParseEnums.Activation(0), (int)(start - end), new ParseEnums.Activation(0)));
                        }
                    }
                    break;
                case "Matthias Gabrel":
                    // Special buff cast check
                    CombatItem heat_wave = combatList.Find(x => x.getSkillID() == 34526);
                    List<long> phase_starts = new List<long>();
                    if (heat_wave != null)
                    {
                        phase_starts.Add(heat_wave.getTime() - bossData.getFirstAware());
                        CombatItem down_pour = combatList.Find(x => x.getSkillID() == 34554);
                        if (down_pour != null)
                        {
                            phase_starts.Add(down_pour.getTime() - bossData.getFirstAware());
                            CastLog abo = cast_logs.Find(x => x.getID() == 34427);
                            if (abo != null)
                            {
                                phase_starts.Add(abo.getTime());
                            }
                        }
                    }
                    foreach (long t in phase_starts)
                    {
                        end = t;
                        phases.Add(new PhaseData(start, end));
                        // make sure stuff from the precedent phase mix witch each other
                        start = t+1;
                    }
                    break;
                case "Xera":
                    // split happened
                    if (phaseData.Count == 2)
                    {
                        end = phaseData[0] - bossData.getFirstAware();
                        phases.Add(new PhaseData(start, end));
                        start = phaseData[1] - bossData.getFirstAware();
                        cast_logs.Add(new CastLog(end, -5, (int)(start - end), new ParseEnums.Activation(0), (int)(start - end), new ParseEnums.Activation(0)));                       
                    }
                    break;
                case "Samarog":
                    // Determined check
                    List<CombatItem> invulsSam = combatList.Where(x => x.getSkillID() == 762 && getInstid() == x.getDstInstid() && x.isBuff() == 1).ToList();
                    // Samarog receives determined twice and its removed twice, filter it
                    List<CombatItem> invulsSamFiltered = new List<CombatItem>();
                    foreach( CombatItem c in invulsSam)
                    {
                        if (invulsSamFiltered.Count > 0)
                        {
                            CombatItem last = invulsSamFiltered.Last();
                            if (last.getTime() != c.getTime())
                            {
                                invulsSamFiltered.Add(c);
                            }
                        } else
                        {
                            invulsSamFiltered.Add(c);
                        }
                    }
                    for (int i = 0; i < invulsSamFiltered.Count; i++)
                    {
                        CombatItem c = invulsSamFiltered[i];
                        if (c.isBuffremove().getID() == 0)
                        {
                            end = c.getTime() - bossData.getFirstAware();
                            phases.Add(new PhaseData(start, end));
                            if (i == invulsSamFiltered.Count - 1)
                            {
                                cast_logs.Add(new CastLog(end, -5, (int)(fight_dur - end), new ParseEnums.Activation(0), (int)(fight_dur - end), new ParseEnums.Activation(0)));
                            }
                        }
                        else
                        {
                            start = c.getTime() - bossData.getFirstAware();
                            cast_logs.Add(new CastLog(end, -5, (int)(start - end), new ParseEnums.Activation(0), (int)(start - end), new ParseEnums.Activation(0)));
                        }
                    }
                    break;
                case "Deimos":
                    // Determined + additional data on inst change
                    CombatItem invulDei = combatList.Find(x => x.getSkillID() == 762 && x.isBuff() == 1 && x.isBuffremove().getID() == 0 && x.getDstInstid() == getInstid()); 
                    if (invulDei != null)
                    {
                        end = invulDei.getTime() - bossData.getFirstAware();
                        phases.Add(new PhaseData(start, end));
                        start = (phaseData.Count == 1 ? phaseData[0] : fight_dur) - bossData.getFirstAware();
                        cast_logs.Add(new CastLog(end, -6, (int)(start - end), new ParseEnums.Activation(0), (int)(start - end), new ParseEnums.Activation(0)));
                    }
                    break;
                case "Dhuum":
                    if (fight_dur > 120000)
                    {
                        end = 120000;
                        phases.Add(new PhaseData(start, end));
                        start = end + 1;
                        CastLog shield = cast_logs.Find(x => x.getID() == 47396);
                        if (shield != null)
                        {
                            end = shield.getTime();
                            phases.Add(new PhaseData(start, end));
                            start = shield.getTime() + shield.getActDur();
                            if (start < fight_dur - 5000)
                            {
                                phases.Add(new PhaseData(start, fight_dur));
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            if (fight_dur - start > 5000 && start >= phases.Last().end)
            {
                phases.Add(new PhaseData(start, fight_dur));
            }
        }

        protected override void setDamageLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            foreach (CombatItem c in combatList)
            {
                if (instid == c.getSrcInstid() || instid == c.getSrcMasterInstid())//selecting player or minion as caster
                {
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in agentData.getAllAgentsList())
                    {//selecting all
                        addDamageLog(time, item.getInstid(), c, damage_logs);
                    }
                }
            }
        }
        protected override void setDamagetakenLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData, MechanicData m_data)
        {
            long time_start = bossData.getFirstAware();
            foreach (CombatItem c in combatList)
            {
                if (instid == c.getDstInstid())
                {//selecting player as target
                    LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in agentData.getAllAgentsList())
                    {//selecting all

                        setDamageTakenLog(time, item.getInstid(), c);
                    }
                }
            }
        }
    }
}