using LuckParser.Controllers;
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
        private CombatReplayMap map = null;
        private List<Mob> thrashMobs = new List<Mob>();

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



        public CombatReplayMap getCombatMap(ParsedLog log)
        {
            if (map == null)
            {
                switch (log.getBossData().getID())
                {
                    // VG
                    case 15438:
                        map = new CombatReplayMap("https://i.imgur.com/W7MocGz.png",
                            Tuple.Create(889, 889),
                            Tuple.Create(-6365, -22213, -3150, -18999),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
                        break;
                    // Gorse
                    case 15429:
                        map = new CombatReplayMap("https://i.imgur.com/nTueZcX.png",
                            Tuple.Create(1354, 1415),
                            Tuple.Create(-623, -6754, 3731, -2206),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
                        break;
                    // Sab
                    case 15375:
                        map = new CombatReplayMap("https://i.imgur.com/FwpMbYf.png",
                            Tuple.Create(2790, 2763),
                            Tuple.Create(-8587, -162, -1601, 6753),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
                        break;
                    // Sloth
                    case 16123:
                        map = new CombatReplayMap("https://i.imgur.com/aLHcYSF.png",
                            Tuple.Create(1688, 2581),
                            Tuple.Create(5822, -3491, 9549, 2205),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(2688, 11906, 3712, 14210));
                        break;
                    // Matthias
                    case 16115:
                        map = new CombatReplayMap("https://i.imgur.com/3X0YveK.png",
                            Tuple.Create(880, 880),
                            Tuple.Create(-7248, 4585, -4625, 7207),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(2688, 11906, 3712, 14210));
                        break;
                    // KC
                    case 16235:
                        map = new CombatReplayMap("https://i.imgur.com/tBAFCEf.png",
                            Tuple.Create(1099, 1114),
                            Tuple.Create(-5467, 8069, -2282, 11297),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(1920, 12160, 2944, 14464));
                        break;
                    // Xera
                    case 16246:
                        map = new CombatReplayMap("https://i.imgur.com/BoHwwY6.png",
                            Tuple.Create(7112, 6377),
                            Tuple.Create(-5992, -5992, 69, -522),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(1920, 12160, 2944, 14464));
                        break;
                    // Cairn
                    case 17194:
                        map = new CombatReplayMap("https://i.imgur.com/NlpsLZa.png",
                            Tuple.Create(607, 607),
                            Tuple.Create(12981, 642, 15725, 3386),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
                        break;
                    // MO
                    case 17172:
                        map = new CombatReplayMap("https://i.imgur.com/lT1FW2r.png",
                            Tuple.Create(889, 889),
                            Tuple.Create(1360, 2701, 3911, 5258),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
                        break;
                    // Samarog
                    case 17188:
                        map = new CombatReplayMap("https://i.imgur.com/o2DHN29.png",
                            Tuple.Create(1221, 1171),
                             Tuple.Create(-6526, 1218, -2423, 5146),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
                        break;
                    // Deimos
                    case 17154:
                        map = new CombatReplayMap("https://i.imgur.com/TskIM9i.png",
                            Tuple.Create(4267, 5770),
                            Tuple.Create(-9542, 1932, -7078, 5275),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
                        break;
                    // SH
                    case 0x4D37:
                        map = new CombatReplayMap("https://i.imgur.com/A45pVJy.png",
                            Tuple.Create(3657, 3657),
                            Tuple.Create(-12223, -771, -8932, 2420),
                            Tuple.Create(-21504, -12288, 24576, 12288),
                            Tuple.Create(19072, 15484, 20992, 16508));
                        break;
                    // Dhuum
                    case 0x4BFA:
                        map = new CombatReplayMap("https://i.imgur.com/CLTwWBJ.png",
                            Tuple.Create(3763, 3383),
                            Tuple.Create(13524, -1334, 18039, 2735),
                            Tuple.Create(-21504, -12288, 24576, 12288),
                            Tuple.Create(19072, 15484, 20992, 16508));
                        break;
                    // MAMA
                    case 0x427D:
                        map = new CombatReplayMap("https://i.imgur.com/lFGNKuf.png",
                            Tuple.Create(664, 407),
                            Tuple.Create(1653, 4555, 5733, 7195),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
                        break;
                    // Siax
                    case 0x4284:
                        map = new CombatReplayMap("https://i.imgur.com/UzaQHW9.png",
                            Tuple.Create(476, 548),
                            Tuple.Create(663, -4127, 3515, -997),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
                        break;
                    // Ensolyss
                    case 0x4234:
                        map = new CombatReplayMap("https://i.imgur.com/kjelZ4t.png",
                            Tuple.Create(366, 366),
                            Tuple.Create(252, 1, 2892, 2881),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
                        break;
                    // Skorvald
                    case 0x44E0:
                        map = new CombatReplayMap("https://i.imgur.com/PO3aoJD.png",
                            Tuple.Create(1759, 1783),
                            Tuple.Create(-22267, 14955, -17227, 20735),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
                        break;
                    // Artsariiv
                    case 0x461D:
                        map = new CombatReplayMap("https://i.imgur.com/4wmuc8B.png",
                            Tuple.Create(914, 914),
                            Tuple.Create(8991, 112, 11731, 2812),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
                        break;
                    // Arkk
                    case 0x455F:
                        map = new CombatReplayMap("https://i.imgur.com/BIybWJe.png",
                            Tuple.Create(914, 914),
                            Tuple.Create(-19231, -18137, -16591, -15677),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
                        break;
                }
            }
            return map;
        }

        public List<Mob> getThrashMobs()
        {
            return thrashMobs;
        }

        // Private Methods

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
                    string[] namesGorse = new string[] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3" };
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
                        phases[i].setName(namesSab[i - 1]);
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
                        start = t + 1;
                    }
                    if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                    {
                        phases.Add(new PhaseData(start, fight_dur));
                    }
                    string[] namesMat = new string[] { "Fire Phase", "Ice Phase", "Storm Phase", "Abomination Phase" };
                    for (int i = 1; i < phases.Count; i++)
                    {
                        phases[i].setName(namesMat[i - 1]);
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
                        }
                        else
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
                        phases[i].setName("Burn " + (i - offset + 1) + " (" + orbs[phases[i].getStart()] + " orbs)");
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
                    foreach (CombatItem c in invulsSam)
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
                        start = (phaseData.Count == 1 ? phaseData[0] - log.getBossData().getFirstAware() : fight_dur);
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
                    int offsetDei = phases.Count;
                    CombatItem teleport = log.getCombatList().FirstOrDefault(x => x.getSkillID() == 38169);
                    int splits = 0;
                    while (teleport != null && splits < 3)
                    {
                        start = teleport.getTime() - log.getBossData().getFirstAware();
                        CombatItem teleportBack = log.getCombatList().FirstOrDefault(x => x.getSkillID() == 38169 && x.getTime() - log.getBossData().getFirstAware() > start + 10000);
                        if (teleportBack != null)
                        {
                            end = teleportBack.getTime() - log.getBossData().getFirstAware();
                        }
                        else
                        {
                            end = fight_dur;
                        }
                        phases.Add(new PhaseData(start, end));
                        splits++;
                        teleport = log.getCombatList().FirstOrDefault(x => x.getSkillID() == 38169 && x.getTime() - log.getBossData().getFirstAware() > end + 10000);
                    }

                    string[] namesDeiSplit = new string[] { "Thief", "Gambler", "Drunkard" };
                    for (int i = offsetDei; i < phases.Count; i++)
                    {
                        phases[i].setName(namesDeiSplit[i - offsetDei]);
                    }
                    phases.Sort((x, y) => (x.getStart() < y.getStart()) ? -1 : 1);
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
                    }
                    else
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
                    break;
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


        public enum ThrashIDS : ushort {
            // VG
            Seekers         = 15426,
            RedGuardian     = 15433,
            BlueGuardian    = 15431,
            GreenGuardian   = 15420,
            // Gorse
            ChargedSoul     = 15434,
            // Sab
            Kernan          = 15372,
            Knuckles        = 15404,
            Karde           = 15430,
            // Matthias
            Spirit          = 16105,
            Spirit2         = 16114,
            IcePatch        = 16139,
            Storm           = 16108,
            Tornado         = 16068,
            //BloodStone      = 13864,
            // KC
            Olson           = 16244,
            Engul           = 16274,
            Faerla          = 16264,
            Caulle          = 16282,
            Henley          = 16236,
            Jessica         = 16278,
            Galletta        = 16228,
            Ianim           = 16248,
            Core            = 16261,
            // MO
            Jade            = 17181,
            // Samarog
            Guldhem         = 17208,
            Rigom           = 17124,
            // Deimos
            Saul            = 17126,
            Thief           = 17206,
            Gambler         = 17335,
            GamblerClones   = 17161,
            GamblerReal     = 17355,
            Drunkard        = 17163,
            Oil             = 17332,
            Tear            = 17303,
            // SH
            TormentedDead   = 19422,
            SurgingSoul     = 19474,
            Scythe          = 19396,
            // Dhuum
            Messenger       = 19807,
            Echo            = 19628,
            Enforcer        = 19681,
            // Siax
            Hallucination   = 17002,
            //
            Unknown
        };
        public static ThrashIDS getThrashIDS(ushort id)
        {
            return Enum.IsDefined(typeof(ThrashIDS),id) ? (ThrashIDS) id : ThrashIDS.Unknown ;
        }


        protected override void setAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            List<ThrashIDS> ids = new List<ThrashIDS>();
            List<CastLog> cls = getCastLogs(log, 0, log.getBossData().getAwareDuration());
            switch (log.getBossData().getID())
            {
                // VG
                case 15438:
                    ids = new List<ThrashIDS>
                    {
                        ThrashIDS.Seekers,
                        ThrashIDS.BlueGuardian,
                        ThrashIDS.GreenGuardian,
                        ThrashIDS.RedGuardian
                    };
                    List<CastLog> magicStorms = cls.Where(x => x.getID() == 31419).ToList();
                    foreach (CastLog c in magicStorms)
                    {
                        replay.addCircleActor(new FollowingCircle(true, 0, 100, new Tuple<int, int>((int)c.getTime(), (int)c.getTime()+c.getActDur()), "rgba(255, 255, 0, 0.5)"));
                    }
                    break;
                // Gorse
                case 15429:
                    // TODO: doughnuts (rampage)
                    ids = new List<ThrashIDS>
                    {
                        ThrashIDS.ChargedSoul
                    }; 
                    List<CastLog> blooms = cls.Where(x => x.getID() == 31616).ToList();
                    foreach (CastLog c in blooms)
                    {
                        int start = (int)c.getTime();
                        int end = start + c.getActDur();
                        replay.addCircleActor(new FollowingCircle(true, c.getExpDur()+(int)c.getTime(), 600, new Tuple<int, int>(start, end), "rgba(255, 125, 0, 0.5)"));
                        replay.addCircleActor(new FollowingCircle(false, 0, 600, new Tuple<int, int>(start, end), "rgba(255, 125, 0, 0.5)"));
                    }
                    break;
                // Sab
                case 15375:
                    // TODO:facing information (flame wall)
                    ids = new List<ThrashIDS>
                    {
                        ThrashIDS.Kernan,
                        ThrashIDS.Knuckles,
                        ThrashIDS.Karde
                    };
                    break;
                // Sloth
                case 16123:
                    // TODO:facing information (breath)
                    List<CastLog> sleepy = cls.Where(x => x.getID() == 34515).ToList();
                    foreach (CastLog c in sleepy)
                    {
                        replay.addCircleActor(new FollowingCircle(true, 0, 180, new Tuple<int, int>((int)c.getTime(), (int)c.getTime() + c.getActDur()), "rgba(255, 255, 0, 0.5)"));
                    }

                    List<CastLog> tantrum = cls.Where(x => x.getID() == 34547).ToList();
                    foreach (CastLog c in tantrum)
                    {
                        int start = (int)c.getTime();
                        int end = start + c.getActDur();
                        replay.addCircleActor(new FollowingCircle(false, 0, 300, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.4)"));
                        replay.addCircleActor(new FollowingCircle(true, end, 300, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.4)"));
                    }
                    List<CastLog> shakes = cls.Where(x => x.getID() == 34482).ToList();
                    foreach (CastLog c in shakes)
                    {
                        int start = (int)c.getTime();
                        int end = start + c.getActDur();
                        replay.addCircleActor(new FollowingCircle(false, 0, 700, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.4)"));
                        replay.addCircleActor(new FollowingCircle(true, end, 700, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.4)"));
                    }
                    break;
                // Matthias
                case 16115:
                    // TODO: needs facing information for hadouken
                    ids = new List<ThrashIDS>
                    {
                        ThrashIDS.Spirit,
                        ThrashIDS.Spirit2,
                        ThrashIDS.IcePatch,
                        ThrashIDS.Tornado,
                        ThrashIDS.Storm
                    };
                    List<CastLog> humanShield = cls.Where(x => x.getID() == 34468).ToList();
                    List<CastLog> humanShards = cls.Where(x => x.getID() == 34480).ToList();
                    for (var i = 0; i < humanShield.Count; i++)
                    {
                        var shield = humanShield[i];
                        if (i < humanShards.Count)
                        {
                            var shard = humanShards[i];
                            replay.addCircleActor(new FollowingCircle(true, 0, 120, new Tuple<int, int>((int)shield.getTime(), (int)shard.getTime() + shard.getActDur()), "rgba(255, 0, 255, 0.5)"));
                        }
                    }
                    List<CastLog> aboShield = cls.Where(x => x.getID() == 34510).ToList();
                    List<CastLog> aboShards = cls.Where(x => x.getID() == 34440).ToList();
                    for (var i = 0; i < aboShield.Count; i++)
                    {
                        var shield = aboShield[i];
                        if (i < aboShards.Count)
                        {
                            var shard = aboShards[i];
                            replay.addCircleActor(new FollowingCircle(true, 0, 120, new Tuple<int, int>((int)shield.getTime(), (int)shard.getTime() + shard.getActDur()), "rgba(255, 0, 255, 0.5)"));
                        }
                    }
                    List<CastLog> rageShards = cls.Where(x => x.getID() == 34404 || x.getID() == 34411).ToList();
                    foreach (CastLog c in rageShards)
                    {
                        int start = (int)c.getTime();
                        int end = start + c.getActDur();
                        replay.addCircleActor(new FollowingCircle(false, 0, 300, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)"));
                        replay.addCircleActor(new FollowingCircle(true, end, 300, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)"));
                    }
                    break;
                // KC
                case 16235:
                    // TODO: needs arc circles for blades
                    ids = new List<ThrashIDS>
                    {
                        ThrashIDS.Core,
                        ThrashIDS.Jessica,
                        ThrashIDS.Olson,
                        ThrashIDS.Engul,
                        ThrashIDS.Faerla,
                        ThrashIDS.Caulle,
                        ThrashIDS.Henley,
                        ThrashIDS.Galletta,
                        ThrashIDS.Ianim,
                    };
                    List<CastLog> magicCharge = cls.Where(x => x.getID() == 35048).ToList();
                    List<CastLog> magicExplose = cls.Where(x => x.getID() == 34894).ToList();
                    for (var i = 0; i < magicCharge.Count; i++)
                    {
                        CastLog charge = magicCharge[i];
                        if (i < magicExplose.Count)
                        {
                            CastLog fire = magicExplose[i];
                            int start = (int)charge.getTime();
                            int end = (int)fire.getTime() + fire.getActDur();
                            replay.addCircleActor(new FollowingCircle(false, 0, 300, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)"));
                            replay.addCircleActor(new FollowingCircle(true, end, 300, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)"));
                        }
                    }
                    List<CastLog> towerDrop = cls.Where(x => x.getID() == 35086).ToList();
                    foreach (CastLog c in towerDrop)
                    {
                        int start = (int)c.getTime();
                        int end = start + c.getActDur();
                        Point3D pos = replay.getPositions().FirstOrDefault(x => x.time > end);
                        replay.addCircleActor(new ImmobileCircle(false, 0, 240, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)",pos));
                        replay.addCircleActor(new ImmobileCircle(true, end, 240, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)",pos));
                    }
                    break;
                // Xera
                case 16246:             
                    List<CastLog> summon = cls.Where(x => x.getID() == 34887).ToList();
                    foreach (CastLog c in summon)
                    {
                        replay.addCircleActor(new FollowingCircle(true, 0, 180, new Tuple<int, int>((int)c.getTime(), (int)c.getTime() + c.getActDur()), "rgba(255, 255, 0, 0.5)"));
                    }
                    break;
                // Cairn
                case 17194:
                    // TODO: needs doughnuts (wave) and facing information (sword)
                    break;
                // MO
                case 17172:
                    ids = new List<ThrashIDS>
                    {
                        ThrashIDS.Jade
                    };
                    break;
                // Samarog
                case 17188:
                    // TODO: facing information (shock wave)
                    ids = new List<ThrashIDS>
                    {
                        ThrashIDS.Rigom,
                        ThrashIDS.Guldhem
                    };
                    List<CombatItem> brutalize = log.getBoonData().Where(x => x.getSkillID() == 38226 && x.isBuffremove() != ParseEnum.BuffRemove.Manual).ToList();
                    int brutStart = 0;
                    int brutEnd = 0;
                    foreach(CombatItem c in brutalize)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            brutStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        } else
                        {
                            brutEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                            replay.addCircleActor(new FollowingCircle(true, 0, 180, new Tuple<int, int>(brutStart, brutEnd), "rgba(255, 255, 0, 0.5)"));
                        }
                    }
                    break;
                // Deimos
                case 17154:
                    // TODO: facing information (slam)
                    ids = new List<ThrashIDS>
                    {
                        ThrashIDS.Saul,
                        ThrashIDS.Thief,
                        ThrashIDS.Drunkard,
                        ThrashIDS.Gambler,
                        ThrashIDS.GamblerClones
                    };
                    List<CastLog> mindCrush = cls.Where(x => x.getID() == 37613).ToList();
                    foreach (CastLog c in mindCrush)
                    {
                        int start = (int)c.getTime();
                        int end = start + 5000;
                        replay.addCircleActor(new FollowingCircle(true, end, 180, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)"));
                        replay.addCircleActor(new FollowingCircle(false, 0, 180, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)"));
                        if (!log.getBossData().getCM())
                        {
                            replay.addCircleActor(new ImmobileCircle(true, 0, 180, new Tuple<int, int>(start, end), "rgba(0, 0, 255, 0.3)", new Point3D(-8421.818f, 3091.72949f, -9.818082e8f, 216)));
                        }
                    }
                    break;
                // SH
                case 0x4D37:
                    // TODO: facing information (slashes) and doughnuts for outer circle attack
                    ids = new List<ThrashIDS>
                    {
                        ThrashIDS.Scythe,
                        ThrashIDS.TormentedDead,
                        ThrashIDS.SurgingSoul
                    };
                    List<CastLog> howling = cls.Where(x => x.getID() == 48662).ToList();
                    foreach (CastLog c in howling)
                    {
                        int start = (int)c.getTime();
                        int end = start + c.getActDur();
                        replay.addCircleActor(new FollowingCircle(true, (int)c.getTime() + c.getExpDur(), 180, new Tuple<int, int>(start, end), "rgba(255, 255, 0, 0.5)"));
                        replay.addCircleActor(new FollowingCircle(true, 0, 180, new Tuple<int, int>(start, end), "rgba(255, 255, 0, 0.5)"));
                    }
                    List<CastLog> vortex = cls.Where(x => x.getID() == 47327).ToList();
                    foreach (CastLog c in vortex)
                    {
                        int start = (int)c.getTime();
                        int end = start + 4000;
                        Point3D pos = replay.getPositions().FirstOrDefault(x => x.time > start);
                        replay.addCircleActor(new ImmobileCircle(false, 0, 300, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", pos));
                        replay.addCircleActor(new ImmobileCircle(true, end, 300, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", pos));
                    }
                    break;
                // Dhuum
                case 0x4BFA:
                    // TODO: facing information (pull thingy)
                    ids = new List<ThrashIDS>
                    {
                        ThrashIDS.Echo,
                        ThrashIDS.Enforcer,
                        ThrashIDS.Messenger
                    };                   
                    List<CastLog> deathmark = cls.Where(x => x.getID() == 48176).ToList();
                    CastLog majorSplit = cls.Find(x => x.getID() == 47396);
                    foreach (CastLog c in deathmark)
                    {
                        int start = (int)c.getTime();
                        int cast_end = start + c.getActDur();
                        int zone_end = cast_end + 120000;
                        if (majorSplit != null)
                        {
                            cast_end = Math.Min(cast_end, (int)majorSplit.getTime());
                            zone_end = Math.Min(zone_end, (int)majorSplit.getTime());
                        }
                        Point3D pos = replay.getPositions().FirstOrDefault(x => x.time > cast_end);
                        replay.addCircleActor(new ImmobileCircle(true, cast_end, 300, new Tuple<int, int>(start, cast_end), "rgba(200, 255, 100, 0.5)", pos));
                        replay.addCircleActor(new ImmobileCircle(false, 0, 300, new Tuple<int, int>(start, cast_end), "rgba(200, 255, 100, 0.5)", pos));
                        replay.addCircleActor(new ImmobileCircle(true, 0, 300, new Tuple<int, int>(cast_end, zone_end), "rgba(200, 255, 100, 0.5)", pos));
                    }
                    List<CastLog> cataCycle = cls.Where(x => x.getID() == 48398).ToList();
                    foreach (CastLog c in cataCycle)
                    {
                        int start = (int)c.getTime();
                        int end = start + c.getActDur();
                        replay.addCircleActor(new FollowingCircle(true, end , 180, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.7)"));
                        replay.addCircleActor(new FollowingCircle(true, 0, 180, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)"));
                    }
                    if (majorSplit != null)
                    {
                        int start = (int)majorSplit.getTime();
                        int end = (int)log.getBossData().getAwareDuration();
                        replay.addCircleActor(new FollowingCircle(true, 0, 300, new Tuple<int, int>(start, end), "rgba(0, 0, 255, 0.3)"));
                    }
                    break;
                // MAMA
                case 0x427D:
                    break;
                // Siax
                case 0x4284:
                    ids = new List<ThrashIDS>
                    {
                        ThrashIDS.Hallucination
                    };
                    break;
                // Ensolyss
                case 0x4234:
                    break;
                // Skorvald
                case 0x44E0:
                    break;
                // Artsariiv
                case 0x461D:
                    break;
                // Arkk
                case 0x455F:
                    break;
            }
            List<AgentItem> aList = log.getAgentData().getNPCAgentList().Where(x => ids.Contains(getThrashIDS(x.getID()))).ToList();
            foreach (AgentItem a in aList)
            {
                Mob mob = new Mob(a);
                mob.initCombatReplay(log, pollingRate, true);
                thrashMobs.Add(mob);
            }
        }

        protected override void setCombatReplayIcon(ParsedLog log)
        {
            switch (log.getBossData().getID())
            {
                // VG
                case 15438:
                    replay.setIcon("https://i.imgur.com/MIpP5pK.png");
                    break;
                // Gorse
                case 15429:
                    replay.setIcon("https://i.imgur.com/5hmMq12.png");
                    break;
                // Sab
                case 15375:
                    replay.setIcon("https://i.imgur.com/UqbFp9S.png");
                    break;
                // Sloth
                case 16123:
                    replay.setIcon("https://i.imgur.com/h1xH3ER.png");
                    break;
                // Matthias
                case 16115:
                    replay.setIcon("https://i.imgur.com/3uMMmTS.png");
                    break;
                // KC
                case 16235:
                    replay.setIcon("https://i.imgur.com/Kq0kL07.png");
                    break;
                // Xera
                case 16246:
                    replay.setIcon("https://i.imgur.com/lYwJEyV.png");
                    break;
                // Cairn
                case 17194:
                    replay.setIcon("https://i.imgur.com/gQY37Tf.png");
                    break;
                // MO
                case 17172:
                    replay.setIcon("https://i.imgur.com/5LNiw4Y.png");
                    break;
                // Samarog
                case 17188:
                    replay.setIcon("https://i.imgur.com/MPQhKfM.png");
                    break;
                // Deimos
                case 17154:
                    replay.setIcon("https://i.imgur.com/mWfxBaO.png");
                    break;
                // SH
                case 0x4D37:
                    replay.setIcon("https://i.imgur.com/jAiRplg.png");
                    break;
                // Dhuum
                case 0x4BFA:
                    replay.setIcon("https://i.imgur.com/RKaDon5.png");
                    break;
                // MAMA
                case 0x427D:
                    replay.setIcon("https://i.imgur.com/1h7HOII.png");
                    break;
                // Siax
                case 0x4284:
                    replay.setIcon("https://i.imgur.com/5C60cQb.png");
                    break;
                // Ensolyss
                case 0x4234:
                    replay.setIcon("https://i.imgur.com/GUTNuyP.png");
                    break;
                // Skorvald
                case 0x44E0:
                    replay.setIcon("https://i.imgur.com/IOPAHRE.png");
                    break;
                // Artsariiv
                case 0x461D:
                    replay.setIcon(HTMLHelper.GetLink(log.getBossData().getID() + "-icon"));
                    break;
                // Arkk
                case 0x455F:
                    replay.setIcon("https://i.imgur.com/u6vv8cW.png");
                    break;
            }
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