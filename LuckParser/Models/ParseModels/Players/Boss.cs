using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Boss : AbstractMasterPlayer
    {
        // Constructors
        public Boss(AgentItem agent) : base(agent)
        {
        }

        private List<PhaseData> phases = new List<PhaseData>();
        private List<long> phaseData = new List<long>();

        public List<PhaseData> getPhases(ParsedLog log, bool getAllPhases)
        {

            if (phases.Count == 0)
            {
                if (!getAllPhases)
                {
                    long fight_dur = log.getBossData().getAwareDuration();
                    phases.Add(new PhaseData(0, fight_dur));
                    phases[0].setName("Full Fight");
                    getCastLogs(log, 0, fight_dur);
                    return phases;
                }
                setPhases(log);
            }
            return phases;
        }

        public void addPhaseData(long data)
        {
            phaseData.Add(data);
        }

        public Tuple<int, int> getMapOffsets(ParsedLog log)
        {
            switch (log.getBossData().getID())
            {
                case 0x4D37:
                    return Tuple.Create(380, 400);
                case 0x4BFA:
                    return Tuple.Create(1450,375);
            }
            return Tuple.Create(0, 0);
        }

        public Tuple<int, int> getMapSize(ParsedLog log)
        {
            switch (log.getBossData().getID())
            {
                case 0x4D37:
                    return Tuple.Create(150, 150);
                case 0x4BFA:
                    return Tuple.Create(200, 200);
            }
            return Tuple.Create(0, 0);
        }

        public Tuple<int, int> getMapCoord(ParsedLog log, float realX, float realY)
        {
            Tuple<int, int, int, int> apiRect = getMapApiRect(log);
            switch (log.getBossData().getID())
            {
                case 0x4D37:
                case 0x4BFA:
                    return Tuple.Create((int)Math.Round(1920 * (realX - apiRect.Item1) / (apiRect.Item3 - apiRect.Item1)),
                        (int)Math.Round(1024 * (realY - apiRect.Item2) / (apiRect.Item4 - apiRect.Item2)) - 73);
            }
            return Tuple.Create(0, 0);
        }      

        public string getMap(ParsedLog log)
        {
            switch (log.getBossData().getID())
            {
                case 0x4D37:
                case 0x4BFA:
                    return "https://wiki.guildwars2.com/images/6/63/Hall_of_Chains_map.jpg";
            }
            return "";
        }

        // Private Methods
        private Tuple<int, int, int, int> getMapApiRect(ParsedLog log)
        {
            switch (log.getBossData().getID())
            {
                case 0x4D37:
                case 0x4BFA:
                    return Tuple.Create(-21504, -12288, 24576, 12288);
            }
            return Tuple.Create(0, 0,0,0);
        }

        private void setPhases(ParsedLog log)
        {
            long fight_dur = log.getBossData().getAwareDuration();
            phases.Add(new PhaseData(0, fight_dur));
            phases[0].setName("Full Fight");
            long start = 0;
            long end = 0;
            getCastLogs(log, 0, fight_dur);
            switch (log.getBossData().getID())
            {
                case 0x3C4E:
                    // Invul check
                    List<CombatItem> invulsVG = log.getBoonData().Where(x => x.getSkillID() == 757 && getInstid() == x.getDstInstid()).ToList();
                    for (int i = 0; i < invulsVG.Count; i++)
                    {
                        CombatItem c = invulsVG[i];
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            end = c.getTime() - log.getBossData().getFirstAware();
                            phases.Add(new PhaseData(start, end));
                            if (i == invulsVG.Count - 1)
                            {
                                cast_logs.Add(new CastLog(end, -5, (int)(fight_dur - end), ParseEnum.Activation.None, (int)(fight_dur - end), ParseEnum.Activation.None));
                            }
                        }
                        else
                        {
                            start = c.getTime() - log.getBossData().getFirstAware();
                            phases.Add(new PhaseData(end, start));
                            cast_logs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                        }
                    }
                    if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                    {
                        phases.Add(new PhaseData(start, fight_dur));
                    }
                    string[] namesVG = new string[] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3" };
                    for (int i = 1; i < phases.Count; i++)
                    {
                        phases[i].setName(namesVG[i - 1]);
                    }
                    break;
                case 0x3C45:
                    // Ghostly protection check
                    List<CombatItem> invulsGorse = log.getCombatList().Where(x => x.getIFF() == ParseEnum.IFF.Friend && x.getSkillID() == 31790).ToList();
                    for (int i = 0; i < invulsGorse.Count; i++)
                    {
                        CombatItem c = invulsGorse[i];
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            end = c.getTime() - log.getBossData().getFirstAware();
                            phases.Add(new PhaseData(start, end));
                            if (i == invulsGorse.Count - 1)
                            {
                                cast_logs.Add(new CastLog(end, -5, (int)(fight_dur - end), ParseEnum.Activation.None, (int)(fight_dur - end), ParseEnum.Activation.None));
                            }
                        }
                        else
                        {
                            start = c.getTime() - log.getBossData().getFirstAware();
                            phases.Add(new PhaseData(end, start));
                            cast_logs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                        }
                    }
                    if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                    {
                        phases.Add(new PhaseData(start, fight_dur));
                    }
                    string[] namesGorse = new string[] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3"};
                    for (int i = 1; i < phases.Count; i++)
                    {
                        phases[i].setName(namesGorse[i - 1]);
                    }
                    break;
                case 0x3C0F:
                    // Invul check
                    List<CombatItem> invulsSab = log.getBoonData().Where(x => x.getSkillID() == 757 && getInstid() == x.getDstInstid()).ToList();
                    for (int i = 0; i < invulsSab.Count; i++)
                    {
                        CombatItem c = invulsSab[i];
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            end = c.getTime() - log.getBossData().getFirstAware();
                            phases.Add(new PhaseData(start, end));
                            if (i == invulsSab.Count - 1)
                            {
                                cast_logs.Add(new CastLog(end, -5, (int)(fight_dur - end), ParseEnum.Activation.None, (int)(fight_dur - end), ParseEnum.Activation.None));
                            }
                        }
                        else
                        {
                            start = c.getTime() - log.getBossData().getFirstAware();
                            phases.Add(new PhaseData(end, start));
                            cast_logs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                        }
                    }
                    if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                    {
                        phases.Add(new PhaseData(start, fight_dur));
                    }
                    string[] namesSab = new string[] { "Phase 1", "Kernan", "Phase 2", "Knuckles", "Phase 3", "Karde", "Phase 4" };
                    for (int i = 1; i < phases.Count; i++)
                    {
                        phases[i].setName(namesSab[i-1]);
                    }
                    break;
                case 0x3EF3:
                    // Special buff cast check
                    CombatItem heat_wave = log.getCombatList().Find(x => x.getSkillID() == 34526);
                    List<long> phase_starts = new List<long>();
                    if (heat_wave != null)
                    {
                        phase_starts.Add(heat_wave.getTime() - log.getBossData().getFirstAware());
                        CombatItem down_pour = log.getCombatList().Find(x => x.getSkillID() == 34554);
                        if (down_pour != null)
                        {
                            phase_starts.Add(down_pour.getTime() - log.getBossData().getFirstAware());
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
                    if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                    {
                        phases.Add(new PhaseData(start, fight_dur));
                    }
                    string[] namesMat = new string[] { "Fire Phase", "Ice Phase", "Storm Phase", "Abomination Phase" };
                    for (int i = 1; i < phases.Count; i++)
                    {
                        phases[i].setName(namesMat[i-1]);
                    }
                    break;
                case 0x3F6B:
                    // Main phases
                    List<CastLog> clsKC = cast_logs.Where(x => x.getID() == 35048).ToList();
                    foreach (CastLog cl in clsKC)
                    {
                        end = cl.getTime();
                        phases.Add(new PhaseData(start, end));
                        start = end + cl.getActDur();
                    }
                    if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                    {
                        phases.Add(new PhaseData(start, fight_dur));
                        start = fight_dur;
                    }
                    for (int i = 1; i < phases.Count; i++)
                    {
                        phases[i].setName("Phase " + i);
                    }
                    // add burn phases
                    int offset = phases.Count;
                    List<CombatItem> orbItems = log.getBoonData().Where(x => x.getDstInstid() == agent.getInstid() && x.getSkillID() == 35096).ToList();
                    // Get number of orbs and filter the list
                    List<CombatItem> orbItemsFiltered = new List<CombatItem>();
                    Dictionary<long, int> orbs = new Dictionary<long, int>();
                    foreach (CombatItem c in orbItems)
                    {
                        long time = c.getTime() - log.getBossData().getFirstAware();
                        if (!orbs.ContainsKey(time))
                        {
                            orbs[time] = 0;
                        }
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            orbs[time] = orbs[time] + 1;
                        }
                        if (orbItemsFiltered.Count > 0)
                        {
                            CombatItem last = orbItemsFiltered.Last();
                            if (last.getTime() != c.getTime())
                            {
                                orbItemsFiltered.Add(c);
                            }
                        }
                        else
                        {
                            orbItemsFiltered.Add(c);
                        }

                    }
                    foreach (CombatItem c in orbItemsFiltered)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            start = c.getTime() - log.getBossData().getFirstAware();
                        } else
                        {
                            end = c.getTime() - log.getBossData().getFirstAware();
                            phases.Add(new PhaseData(start, end));
                        }
                    }
                    if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                    {
                        phases.Add(new PhaseData(start, fight_dur));
                        start = fight_dur;
                    }
                    for (int i = offset; i < phases.Count; i++)
                    {
                        phases[i].setName("Burn " + (i - offset + 1) + " (" + orbs[phases[i].getStart()] +" orbs)");
                    }
                    phases.Sort((x, y) => (x.getStart() < y.getStart()) ? -1 : 1);
                    break;
                case 0x3F76:
                    // split happened
                    if (phaseData.Count == 1)
                    {
                        CombatItem invulXera = log.getBoonData().Find(x => x.getDstInstid() == agent.getInstid() && (x.getSkillID() == 762 || x.getSkillID() == 34113));
                        end = invulXera.getTime() - log.getBossData().getFirstAware();
                        phases.Add(new PhaseData(start, end));
                        start = phaseData[0] - log.getBossData().getFirstAware();
                        cast_logs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                    }
                    if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                    {
                        phases.Add(new PhaseData(start, fight_dur));
                    }
                    for (int i = 1; i < phases.Count; i++)
                    {
                        phases[i].setName("Phase " + i);
                    }
                    break;
                case 0x4324:
                    // Determined check
                    List<CombatItem> invulsSam = log.getBoonData().Where(x => x.getSkillID() == 762 && getInstid() == x.getDstInstid()).ToList();
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
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            end = c.getTime() - log.getBossData().getFirstAware();
                            phases.Add(new PhaseData(start, end));
                            if (i == invulsSamFiltered.Count - 1)
                            {
                                cast_logs.Add(new CastLog(end, -5, (int)(fight_dur - end), ParseEnum.Activation.None, (int)(fight_dur - end), ParseEnum.Activation.None));
                            }
                        }
                        else
                        {
                            start = c.getTime() - log.getBossData().getFirstAware();
                            phases.Add(new PhaseData(end, start));
                            cast_logs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                        }
                    }
                    if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                    {
                        phases.Add(new PhaseData(start, fight_dur));
                    }
                    string[] namesSam = new string[] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3" };
                    for (int i = 1; i < phases.Count; i++)
                    {
                        phases[i].setName(namesSam[i - 1]);
                    }
                    break;
                case 0x4302:
                    // Determined + additional data on inst change
                    CombatItem invulDei = log.getBoonData().Find(x => x.getSkillID() == 762 && x.isBuffremove() == ParseEnum.BuffRemove.None && x.getDstInstid() == getInstid()); 
                    if (invulDei != null)
                    {
                        end = invulDei.getTime() - log.getBossData().getFirstAware();
                        phases.Add(new PhaseData(start, end));
                        start = (phaseData.Count == 1 ? phaseData[0] - log.getBossData().getFirstAware() : fight_dur) ;
                        cast_logs.Add(new CastLog(end, -6, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                    }
                    if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                    {
                        phases.Add(new PhaseData(start, fight_dur));
                    }
                    for (int i = 1; i < phases.Count; i++)
                    {
                        phases[i].setName("Phase " + i);
                    }
                    break;
                case 0x4BFA:
                    // Sometimes the preevent is not in the evtc
                    List<CastLog> dhuumCast = getCastLogs(log, 0, 20000);
                    if (dhuumCast.Count > 0)
                    {
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
                        if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                        {
                            phases.Add(new PhaseData(start, fight_dur));
                        }
                        string[] namesDh = new string[] { "Main Fight", "Ritual" };
                        for (int i = 1; i < phases.Count; i++)
                        {
                            phases[i].setName(namesDh[i - 1]);
                        }
                    } else
                    {
                        CombatItem invulDhuum = log.getBoonData().Find(x => x.getSkillID() == 762 && x.isBuffremove() != ParseEnum.BuffRemove.None && x.getSrcInstid() == getInstid() && x.getTime() > 115000 + log.getBossData().getFirstAware());
                        if (invulDhuum != null)
                        {
                            end = invulDhuum.getTime() - log.getBossData().getFirstAware();
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
                        if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                        {
                            phases.Add(new PhaseData(start, fight_dur));
                        }
                        string[] namesDh = new string[] { "Roleplay", "Main Fight", "Ritual" };
                        for (int i = 1; i < phases.Count; i++)
                        {
                            phases[i].setName(namesDh[i - 1]);
                        }
                    }
                    break;
                case 0x427D:
                case 0x4284:
                case 0x4234:
                case 0x44E0:
                case 0x461D:
                case 0x455F:
                    List<CombatItem> invulsBoss = log.getBoonData().Where(x => x.getSkillID() == 762 && agent.getInstid() == x.getDstInstid()).ToList();
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
                    break;
                default:
                    break; ;
            }
        }
        
        protected override void setDamagetakenLogs(ParsedLog log)
        {
            // nothing to do
            /*long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getDamageTakenData())
            {
                if (agent.getInstid() == c.getDstInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())
                {//selecting player as target
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in log.getAgentData().getAllAgentsList())
                    {//selecting all
                        addDamageTakenLog(time, item.getInstid(), c);
                    }
                }
            }*/
        }

        /*protected override void setHealingLogs(ParsedLog log)
        {
            // nothing to do
        }

        protected override void setHealingReceivedLogs(ParsedLog log)
        {
            // nothing to do
        }*/
    }
}