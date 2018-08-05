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
                switch (ParseEnum.getBossIDS(log.getBossData().getID()))
                {
                    // VG
                    case ParseEnum.BossIDS.ValeGuardian:
                        map = new CombatReplayMap("https://i.imgur.com/W7MocGz.png",
                            Tuple.Create(889, 889),
                            Tuple.Create(-6365, -22213, -3150, -18999),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
                        break;
                    // Gorse
                    case ParseEnum.BossIDS.Gorseval:
                        map = new CombatReplayMap("https://i.imgur.com/nTueZcX.png",
                            Tuple.Create(1354, 1415),
                            Tuple.Create(-623, -6754, 3731, -2206),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
                        break;
                    // Sab
                    case ParseEnum.BossIDS.Sabetha:
                        map = new CombatReplayMap("https://i.imgur.com/FwpMbYf.png",
                            Tuple.Create(2790, 2763),
                            Tuple.Create(-8587, -162, -1601, 6753),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
                        break;
                    // Sloth
                    case ParseEnum.BossIDS.Slothasor:
                        map = new CombatReplayMap("https://i.imgur.com/aLHcYSF.png",
                            Tuple.Create(1688, 2581),
                            Tuple.Create(5822, -3491, 9549, 2205),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(2688, 11906, 3712, 14210));
                        break;
                    // Matthias
                    case ParseEnum.BossIDS.Matthias:
                        map = new CombatReplayMap("https://i.imgur.com/3X0YveK.png",
                            Tuple.Create(880, 880),
                            Tuple.Create(-7248, 4585, -4625, 7207),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(2688, 11906, 3712, 14210));
                        break;
                    // KC
                    case ParseEnum.BossIDS.KeepConstruct:
                        map = new CombatReplayMap("https://i.imgur.com/tBAFCEf.png",
                            Tuple.Create(1099, 1114),
                            Tuple.Create(-5467, 8069, -2282, 11297),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(1920, 12160, 2944, 14464));
                        break;
                    // Xera
                    case ParseEnum.BossIDS.Xera:
                        map = new CombatReplayMap("https://i.imgur.com/BoHwwY6.png",
                            Tuple.Create(7112, 6377),
                            Tuple.Create(-5992, -5992, 69, -522),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(1920, 12160, 2944, 14464));
                        break;
                    // Cairn
                    case ParseEnum.BossIDS.Cairn:
                        map = new CombatReplayMap("https://i.imgur.com/NlpsLZa.png",
                            Tuple.Create(607, 607),
                            Tuple.Create(12981, 642, 15725, 3386),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
                        break;
                    // MO
                    case ParseEnum.BossIDS.MursaatOverseer:
                        map = new CombatReplayMap("https://i.imgur.com/lT1FW2r.png",
                            Tuple.Create(889, 889),
                            Tuple.Create(1360, 2701, 3911, 5258),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
                        break;
                    // Samarog
                    case ParseEnum.BossIDS.Samarog:
                        map = new CombatReplayMap("https://i.imgur.com/o2DHN29.png",
                            Tuple.Create(1221, 1171),
                             Tuple.Create(-6526, 1218, -2423, 5146),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
                        break;
                    // Deimos
                    case ParseEnum.BossIDS.Deimos:
                        map = new CombatReplayMap("https://i.imgur.com/TskIM9i.png",
                            Tuple.Create(4267, 5770),
                            Tuple.Create(-9542, 1932, -7078, 5275),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
                        break;
                    // SH
                    case ParseEnum.BossIDS.SoullessHorror:
                        map = new CombatReplayMap("https://i.imgur.com/A45pVJy.png",
                            Tuple.Create(3657, 3657),
                            Tuple.Create(-12223, -771, -8932, 2420),
                            Tuple.Create(-21504, -12288, 24576, 12288),
                            Tuple.Create(19072, 15484, 20992, 16508));
                        break;
                    // Dhuum
                    case ParseEnum.BossIDS.Dhuum:
                        map = new CombatReplayMap("https://i.imgur.com/CLTwWBJ.png",
                            Tuple.Create(3763, 3383),
                            Tuple.Create(13524, -1334, 18039, 2735),
                            Tuple.Create(-21504, -12288, 24576, 12288),
                            Tuple.Create(19072, 15484, 20992, 16508));
                        break;
                    // MAMA
                    case ParseEnum.BossIDS.MAMA:
                        map = new CombatReplayMap("https://i.imgur.com/lFGNKuf.png",
                            Tuple.Create(664, 407),
                            Tuple.Create(1653, 4555, 5733, 7195),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
                        break;
                    // Siax
                    case ParseEnum.BossIDS.Siax:
                        map = new CombatReplayMap("https://i.imgur.com/UzaQHW9.png",
                            Tuple.Create(476, 548),
                            Tuple.Create(663, -4127, 3515, -997),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
                        break;
                    // Ensolyss
                    case ParseEnum.BossIDS.Ensolyss:
                        map = new CombatReplayMap("https://i.imgur.com/kjelZ4t.png",
                            Tuple.Create(366, 366),
                            Tuple.Create(252, 1, 2892, 2881),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
                        break;
                    // Skorvald
                    case ParseEnum.BossIDS.Skorvald:
                        map = new CombatReplayMap("https://i.imgur.com/PO3aoJD.png",
                            Tuple.Create(1759, 1783),
                            Tuple.Create(-22267, 14955, -17227, 20735),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
                        break;
                    // Artsariiv
                    case ParseEnum.BossIDS.Artsariiv:
                        map = new CombatReplayMap("https://i.imgur.com/4wmuc8B.png",
                            Tuple.Create(914, 914),
                            Tuple.Create(8991, 112, 11731, 2812),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
                        break;
                    // Arkk
                    case ParseEnum.BossIDS.Arkk:
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
            switch (ParseEnum.getBossIDS(log.getBossData().getID()))
            {
                case ParseEnum.BossIDS.ValeGuardian:
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
                case ParseEnum.BossIDS.Gorseval:
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
                case ParseEnum.BossIDS.Sabetha:
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
                case ParseEnum.BossIDS.Matthias:
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
                case ParseEnum.BossIDS.KeepConstruct:
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
                case ParseEnum.BossIDS.Xera:
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
                case ParseEnum.BossIDS.Samarog:
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
                case ParseEnum.BossIDS.Deimos:
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
                case ParseEnum.BossIDS.Dhuum:
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
                case ParseEnum.BossIDS.MAMA:
                case ParseEnum.BossIDS.Siax:
                case ParseEnum.BossIDS.Ensolyss:
                case ParseEnum.BossIDS.Skorvald:
                case ParseEnum.BossIDS.Artsariiv:
                case ParseEnum.BossIDS.Arkk:
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

        protected override void setAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>();
            List<CastLog> cls = getCastLogs(log, 0, log.getBossData().getAwareDuration());
            switch (ParseEnum.getBossIDS(log.getBossData().getID()))
            {
                // VG
                case ParseEnum.BossIDS.ValeGuardian:
                    ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Seekers,
                        ParseEnum.ThrashIDS.BlueGuardian,
                        ParseEnum.ThrashIDS.GreenGuardian,
                        ParseEnum.ThrashIDS.RedGuardian
                    };
                    List<CastLog> magicStorms = cls.Where(x => x.getID() == 31419).ToList();
                    foreach (CastLog c in magicStorms)
                    {
                        replay.addCircleActor(new FollowingCircle(true, 0, 100, new Tuple<int, int>((int)c.getTime(), (int)c.getTime()+c.getActDur()), "rgba(255, 255, 0, 0.5)"));
                    }
                    break;
                // Gorse
                case ParseEnum.BossIDS.Gorseval:
                    // TODO: doughnuts (rampage)
                    ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.ChargedSoul
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
                case ParseEnum.BossIDS.Sabetha:
                    // TODO:facing information (flame wall)
                    ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Kernan,
                        ParseEnum.ThrashIDS.Knuckles,
                        ParseEnum.ThrashIDS.Karde
                    };
                    break;
                // Sloth
                case ParseEnum.BossIDS.Slothasor:
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
                case ParseEnum.BossIDS.Matthias:
                    // TODO: needs facing information for hadouken
                    ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Spirit,
                        ParseEnum.ThrashIDS.Spirit2,
                        ParseEnum.ThrashIDS.IcePatch,
                        ParseEnum.ThrashIDS.Tornado,
                        ParseEnum.ThrashIDS.Storm
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
                case ParseEnum.BossIDS.KeepConstruct:
                    // TODO: needs arc circles for blades
                    ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Core,
                        ParseEnum.ThrashIDS.Jessica,
                        ParseEnum.ThrashIDS.Olson,
                        ParseEnum.ThrashIDS.Engul,
                        ParseEnum.ThrashIDS.Faerla,
                        ParseEnum.ThrashIDS.Caulle,
                        ParseEnum.ThrashIDS.Henley,
                        ParseEnum.ThrashIDS.Galletta,
                        ParseEnum.ThrashIDS.Ianim,
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
                case ParseEnum.BossIDS.Xera:             
                    List<CastLog> summon = cls.Where(x => x.getID() == 34887).ToList();
                    foreach (CastLog c in summon)
                    {
                        replay.addCircleActor(new FollowingCircle(true, 0, 180, new Tuple<int, int>((int)c.getTime(), (int)c.getTime() + c.getActDur()), "rgba(255, 255, 0, 0.5)"));
                    }
                    break;
                // Cairn
                case ParseEnum.BossIDS.Cairn:
                    // TODO: needs doughnuts (wave) and facing information (sword)
                    break;
                // MO
                case ParseEnum.BossIDS.MursaatOverseer:
                    ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Jade
                    };
                    break;
                // Samarog
                case ParseEnum.BossIDS.Samarog:
                    // TODO: facing information (shock wave)
                    ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Rigom,
                        ParseEnum.ThrashIDS.Guldhem
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
                case ParseEnum.BossIDS.Deimos:
                    // TODO: facing information (slam)
                    ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Saul,
                        ParseEnum.ThrashIDS.Thief,
                        ParseEnum.ThrashIDS.Drunkard,
                        ParseEnum.ThrashIDS.Gambler,
                        ParseEnum.ThrashIDS.GamblerClones
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
                case ParseEnum.BossIDS.SoullessHorror:
                    // TODO: facing information (slashes) and doughnuts for outer circle attack
                    ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Scythe,
                        ParseEnum.ThrashIDS.TormentedDead,
                        ParseEnum.ThrashIDS.SurgingSoul
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
                case ParseEnum.BossIDS.Dhuum:
                    // TODO: facing information (pull thingy)
                    ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Echo,
                        ParseEnum.ThrashIDS.Enforcer,
                        ParseEnum.ThrashIDS.Messenger
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
                case ParseEnum.BossIDS.MAMA:
                    break;
                // Siax
                case ParseEnum.BossIDS.Siax:
                    ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Hallucination
                    };
                    break;
                // Ensolyss
                case ParseEnum.BossIDS.Ensolyss:
                    break;
                // Skorvald
                case ParseEnum.BossIDS.Skorvald:
                    break;
                // Artsariiv
                case ParseEnum.BossIDS.Artsariiv:
                    break;
                // Arkk
                case ParseEnum.BossIDS.Arkk:
                    break;
            }
            List<AgentItem> aList = log.getAgentData().getNPCAgentList().Where(x => ids.Contains(ParseEnum.getThrashIDS(x.getID()))).ToList();
            foreach (AgentItem a in aList)
            {
                Mob mob = new Mob(a);
                mob.initCombatReplay(log, pollingRate, true);
                thrashMobs.Add(mob);
            }
        }

        protected override void setCombatReplayIcon(ParsedLog log)
        {
            switch (ParseEnum.getBossIDS(log.getBossData().getID()))
            {
                case ParseEnum.BossIDS.ValeGuardian:
                    replay.setIcon("https://i.imgur.com/MIpP5pK.png");
                    break;
                case ParseEnum.BossIDS.Gorseval:
                    replay.setIcon("https://i.imgur.com/5hmMq12.png");
                    break;
                case ParseEnum.BossIDS.Sabetha:
                    replay.setIcon("https://i.imgur.com/UqbFp9S.png");
                    break;
                case ParseEnum.BossIDS.Slothasor:
                    replay.setIcon("https://i.imgur.com/h1xH3ER.png");
                    break;
                case ParseEnum.BossIDS.Matthias:
                    replay.setIcon("https://i.imgur.com/3uMMmTS.png");
                    break;
                case ParseEnum.BossIDS.KeepConstruct:
                    replay.setIcon("https://i.imgur.com/Kq0kL07.png");
                    break;
                case ParseEnum.BossIDS.Xera:
                    replay.setIcon("https://i.imgur.com/lYwJEyV.png");
                    break;
                case ParseEnum.BossIDS.Cairn:
                    replay.setIcon("https://i.imgur.com/gQY37Tf.png");
                    break;
                case ParseEnum.BossIDS.MursaatOverseer:
                    replay.setIcon("https://i.imgur.com/5LNiw4Y.png");
                    break;
                case ParseEnum.BossIDS.Samarog:
                    replay.setIcon("https://i.imgur.com/MPQhKfM.png");
                    break;
                case ParseEnum.BossIDS.Deimos:
                    replay.setIcon("https://i.imgur.com/mWfxBaO.png");
                    break;
                case ParseEnum.BossIDS.SoullessHorror:
                    replay.setIcon("https://i.imgur.com/jAiRplg.png");
                    break;
                case ParseEnum.BossIDS.Dhuum:
                    replay.setIcon("https://i.imgur.com/RKaDon5.png");
                    break;
                case ParseEnum.BossIDS.MAMA:
                    replay.setIcon("https://i.imgur.com/1h7HOII.png");
                    break;
                case ParseEnum.BossIDS.Siax:
                    replay.setIcon("https://i.imgur.com/5C60cQb.png");
                    break;
                case ParseEnum.BossIDS.Ensolyss:
                    replay.setIcon("https://i.imgur.com/GUTNuyP.png");
                    break;
                case ParseEnum.BossIDS.Skorvald:
                    replay.setIcon("https://i.imgur.com/IOPAHRE.png");
                    break;
                case ParseEnum.BossIDS.Artsariiv:
                    replay.setIcon(HTMLHelper.GetLink(log.getBossData().getID() + "-icon"));
                    break;
                case ParseEnum.BossIDS.Arkk:
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