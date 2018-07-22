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

        public Tuple<int,int> getPixelMapSize(ParsedLog log)
        {
            Tuple<int, int> sizes = getMapSize(log);
            double ratio = (double)sizes.Item1 / sizes.Item2;
            if (ratio > 1.0)
            {
                return new Tuple<int, int>(850, (int)Math.Round(850 / ratio));
            } else if (ratio < 1.0) 
            {
                return new Tuple<int, int>((int)Math.Round(ratio* 850), 850);
            } else
            {
                return new Tuple<int, int>(850, 850);
            }
        }

        public Tuple<int, int> getMapCoord(ParsedLog log, float realX, float realY)
        {
            //Tuple<int, int, int, int> apiRect = getMapApiRect(log);
            Tuple<int, int, int, int> imageRect = getMapImageRect(log);
            Tuple<int, int> pixelSizes = getPixelMapSize(log);
            Tuple<int, int> sizes = getMapSize(log);
            float scaleX = (float)pixelSizes.Item1 / sizes.Item1;
            float scaleY = (float)pixelSizes.Item2 / sizes.Item2;
            Tuple<int, int> mapSize = getMapSize(log);
            float x = (Math.Max(Math.Min(realX, imageRect.Item3), imageRect.Item1) - imageRect.Item1) / (imageRect.Item3 - imageRect.Item1) ;
            float y = (Math.Max(Math.Min(realY, imageRect.Item4), imageRect.Item2) - imageRect.Item2) / (imageRect.Item4 - imageRect.Item2);
            return Tuple.Create((int)Math.Round(scaleX * mapSize.Item1 * x), (int)Math.Round(scaleY * (sizes.Item2 - mapSize.Item2 * y)));
        }      

        public string getMap(ParsedLog log)
        {
            switch (log.getBossData().getID())
            {
                // VG
                case 15438:
                    return "https://i.imgur.com/W7MocGz.png";
                // Gorse
                case 15429:
                    return "https://i.imgur.com/Gqpp7B1.png";
                // Sab
                case 15375:
                    return "https://i.imgur.com/FwpMbYf.png";
                // Sloth
                case 16123:
                    return "https://i.imgur.com/aLHcYSF.png";
                // Matthias
                case 16115:
                    return "https://i.imgur.com/3X0YveK.png";
                // KC
                case 16235:
                    return "https://i.imgur.com/6ZJhPOw.png";
                // Xera
                case 16246:
                    return "https://i.imgur.com/BoHwwY6.png";
                // Cairn
                case 17194:
                    return "https://i.imgur.com/NlpsLZa.png";
                // MO
                case 17172:
                    return "https://i.imgur.com/lT1FW2r.png";
                // Samarog
                case 17188:
                    return "https://i.imgur.com/o2DHN29.png";
                // Deimos
                case 17154:
                    return "https://i.imgur.com/9vyE9bj.png";
                // SH
                case 0x4D37:
                    return "https://i.imgur.com/A45pVJy.png";
                // Dhuum
                case 0x4BFA:
                    return "https://i.imgur.com/CLTwWBJ.png";
                // MAMA
                case 0x427D:
                    return "https://i.imgur.com/lFGNKuf.png";
                // Siax
                case 0x4284:
                    return "https://i.imgur.com/UzaQHW9.png";
                // Ensolyss
                case 0x4234:
                    return "https://i.imgur.com/kjelZ4t.png";
                // Skorvald
                case 0x44E0:
                    return "https://i.imgur.com/PO3aoJD.png";
                // Artsariiv
                case 0x461D:
                    return "https://i.imgur.com/4wmuc8B.png";
                // Arkk
                case 0x455F:
                    return "https://i.imgur.com/BIybWJe.png";
            }
            return "";
        }

        // Private Methods
        private Tuple<int, int> getMapSize(ParsedLog log)
        {
            switch (log.getBossData().getID())
            {
                // VG
                case 15438:
                    return Tuple.Create(889, 889);
                // Gorse
                case 15429:
                    return Tuple.Create(1354, 1415);
                // Sab
                case 15375:
                    return Tuple.Create(2790, 2763);
                // Sloth
                case 16123:
                    return Tuple.Create(1688, 2581);
                // Matthias
                case 16115:
                    return Tuple.Create(880, 880);
                // KC
                case 16235:
                    return Tuple.Create(1099, 1114);
                // Xera
                case 16246:
                    return Tuple.Create(7112, 6377);
                // Cairn
                case 17194:
                    return Tuple.Create(607, 607);
                // MO
                case 17172:
                    return Tuple.Create(889, 889);
                // Samarog
                case 17188:
                    return Tuple.Create(1221, 1171);
                // Deimos
                case 17154:
                    return Tuple.Create(889, 919);
                // SH
                case 0x4D37:
                    return Tuple.Create(3657, 3657);
                // Dhuum
                case 0x4BFA:
                    return Tuple.Create(3763, 3383);
                // MAMA
                case 0x427D:
                    return Tuple.Create(664, 407);
                // Siax
                case 0x4284:
                    return Tuple.Create(476, 548);
                // Ensolyss
                case 0x4234:
                    return Tuple.Create(366, 366);
                // Skorvald
                case 0x44E0:
                    return Tuple.Create(1759, 1783);
                // Artsariiv
                case 0x461D:
                    return Tuple.Create(914, 914);
                // Arkk
                case 0x455F:
                    return Tuple.Create(914, 914);
            }
            return Tuple.Create(0, 0);
        }
        private Tuple<int, int, int, int> getMapImageRect(ParsedLog log)
        {
            switch (log.getBossData().getID())
            {
                // VG
                case 15438:
                    return Tuple.Create(-6365, -22213, -3150, -18999);
                // Gorse
                case 15429:
                    return Tuple.Create(-623, -6754, 3731, -2206);
                // Sab
                case 15375:
                    return Tuple.Create(-8587, -162, -1601, 6753);
                // Sloth
                case 16123:
                    return Tuple.Create(5822, -3491, 9549, 2205);
                // Matthias
                case 16115:
                    return Tuple.Create(-7253, 4575 , -4630, 7197);
                // KC
                case 16235:
                    return Tuple.Create(-5467, 8069, -2282, 11297);
                // Xera
                case 16246:
                    return Tuple.Create(-5992, -5992, 69, -522);
                // Cairn
                case 17194:
                    return Tuple.Create(12981, 642, 15725, 3386);
                // MO
                case 17172:
                    return Tuple.Create(1360, 2701, 3911, 5258);
                // Samarog
                case 17188:
                    return Tuple.Create(-6526, 1218, -2423, 5146);
                // Deimos
                case 17154:
                    return Tuple.Create(-9542, 1932, -7266, 4292);
                // SH
                case 19767:
                    return Tuple.Create(-12228, -786, -8937, 2405);
                // Dhuum
                case 19450:
                    return Tuple.Create(13524, -1334, 18039, 2735);
                // MAMA
                case 0x427D:
                    return Tuple.Create(1653, 4555, 5733, 7195);
                // Siax
                case 0x4284:
                    return Tuple.Create(663, -4127, 3515, -997);
                // Ensolyss
                case 0x4234:
                    return Tuple.Create(252, 1, 2892, 2881);
                // Skorvald
                case 0x44E0:
                    return Tuple.Create(-22267, 14955, -17227, 20735);
                // Artsariiv
                case 0x461D:
                    return Tuple.Create(8991, 112, 11731, 2812);
                // Arkk
                case 0x455F:
                    return Tuple.Create(-19231, -18137, -16591, -15677);
            }
            return Tuple.Create(0, 0, 0, 0);
        }
        private Tuple<int, int, int, int> getMapApiRect(ParsedLog log)
        {
            switch (log.getBossData().getID())
            {
                case 15438:
                case 15429:
                case 15375:
                    return Tuple.Create(-15360, -36864, 15360, 39936);
                case 16123:
                case 16115:
                    return Tuple.Create(-12288, -27648, 12288, 27648);
                case 16235:
                case 16246:
                    return Tuple.Create(-12288, -27648, 12288, 27648);
                case 17194:
                case 17172:
                case 17188:
                case 17154:
                    return Tuple.Create(-27648, -9216, 27648, 12288);
                case 19767:
                case 19450:
                    return Tuple.Create(-21504, -12288, 24576, 12288);
            }
            return Tuple.Create(0, 0, 0, 0);
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
                    bool needApplication = true;
                    foreach( CombatItem c in invulsSam)
                    {
                        if (needApplication && c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            invulsSamFiltered.Add(c);
                            needApplication = false;
                        }
                        else if (!needApplication && c.isBuffremove() != ParseEnum.BuffRemove.None)
                        {
                            invulsSamFiltered.Add(c);
                            needApplication = true;
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