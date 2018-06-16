using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuckParser.Models.ParseModels;
using LuckParser.Models.ParseEnums;
using System.Drawing;
using System.Net;
using LuckParser.Models;
using System.IO.Compression;
using LuckParser.Models.DataModels;
//recomend CTRL+M+O to collapse all
namespace LuckParser.Controllers
{
    public class Controller1
    {
        private GW2APIController APIContrioller = new GW2APIController();            

        //Main data storage after binary parse
        private LogData log_data;
        private BossData boss_data;
        private AgentData agent_data = new AgentData();
        private SkillData skill_data = new SkillData();
        private CombatData combat_data = new CombatData();
        private MechanicData mech_data = new MechanicData();
        private List<Player> p_list = new List<Player>();
        private Boss boss;

        // Public Methods
        public LogData getLogData()
        {
            return log_data;
        }
        public BossData getBossData()
        {
            return boss_data;
        }

        
        //Main Parse method------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Parses the given log
        /// </summary>
        /// <param name="evtc">The path to the log to parse</param>
        /// <returns></returns>
        public bool ParseLog(string evtc)
        {
            MemoryStream stream = new MemoryStream();
            //used to stream from a database, probably could use better stream now
            using(var client = new WebClient())
            using(var origstream = client.OpenRead(evtc))
            {
                if(evtc.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    using(var arch = new ZipArchive(origstream, ZipArchiveMode.Read))
                    {
                        if(arch.Entries.Count != 1)
                        {
                            return false;
                        }
                        using(var data = arch.Entries[0].Open())
                        {
                            data.CopyTo(stream);
                        }
                    }
                }
                else
                {
                    origstream.CopyTo(stream);
                }
                stream.Position = 0;

                parseBossData(stream);
                parseAgentData(stream);
                parseSkillData(stream);
                parseCombatList(stream);
                fillMissingData(stream);

                stream.Close();
            }
            ////CreateHTML(); is now runnable dont run here
            return (true);
        }

        //sub Parse methods
        /// <summary>
        /// Parses boss related data
        /// </summary>
        private void parseBossData(MemoryStream stream)
        {
            // 12 bytes: arc build version
            String build_version = ParseHelper.getString(stream, 12);
            this.log_data = new LogData(build_version);

            // 1 byte: skip
            ParseHelper.safeSkip(stream, 1);

            // 2 bytes: boss instance ID
            ushort instid = ParseHelper.getShort(stream);

            // 1 byte: position
            ParseHelper.safeSkip(stream, 1);

            //Save
            // TempData["Debug"] = build_version +" "+ instid.ToString() ;
            this.boss_data = new BossData(instid);
        }
        /// <summary>
        /// Parses agent related data
        /// </summary>
        private void parseAgentData(MemoryStream stream)
        {
            // 4 bytes: player count
            int player_count = ParseHelper.getInt(stream);

            // 96 bytes: each player
            for (int i = 0; i < player_count; i++)
            {
                // 8 bytes: agent
                ulong agent = ParseHelper.getULong(stream);

                // 4 bytes: profession
                uint prof = ParseHelper.getUInt(stream);

                // 4 bytes: is_elite
                uint is_elite = ParseHelper.getUInt(stream);

                // 2 bytes: toughness
                int toughness = ParseHelper.getShort(stream);
                // skip concentration
                ParseHelper.safeSkip(stream, 2);
                // 2 bytes: healing
                int healing = ParseHelper.getShort(stream);
                ParseHelper.safeSkip(stream, 2);
                // 2 bytes: condition
                int condition = ParseHelper.getShort(stream);
                ParseHelper.safeSkip(stream, 2);
                // 68 bytes: name
                String name = ParseHelper.getString(stream, 68);
                //Save
                Agent a = new Agent(agent, name, prof, is_elite);
                if (a != null)
                {
                    // NPC
                    if (a.getProf(this.log_data.getBuildVersion(), APIContrioller) == "NPC")
                    {
                        agent_data.addItem(a, new AgentItem(agent, name, a.getName() + ":" + prof.ToString().PadLeft(5, '0')), this.log_data.getBuildVersion(), APIContrioller);//a.getName() + ":" + String.format("%05d", prof)));
                    }
                    // Gadget
                    else if (a.getProf(this.log_data.getBuildVersion(), APIContrioller) == "GDG")
                    {
                        agent_data.addItem(a, new AgentItem(agent, name, a.getName() + ":" + (prof & 0x0000ffff).ToString().PadLeft(5, '0')), this.log_data.getBuildVersion(), APIContrioller);//a.getName() + ":" + String.format("%05d", prof & 0x0000ffff)));
                    }
                    // Player
                    else
                    {
                        agent_data.addItem(a, new AgentItem(agent, name, a.getProf(this.log_data.getBuildVersion(), APIContrioller), toughness, healing, condition), this.log_data.getBuildVersion(), APIContrioller);
                    }
                }
                // Unknown
                else
                {
                    agent_data.addItem(a, new AgentItem(agent, name, prof.ToString(), toughness, healing, condition), this.log_data.getBuildVersion(), APIContrioller);
                }
            }

        }
        /// <summary>
        /// Parses skill related data
        /// </summary>
        private void parseSkillData(MemoryStream stream)
        {
            GW2APIController apiController = new GW2APIController();
            // 4 bytes: player count
            int skill_count = ParseHelper.getInt(stream);
            //TempData["Debug"] += "Skill Count:" + skill_count.ToString();
            // 68 bytes: each skill
            for (int i = 0; i < skill_count; i++)
            {
                // 4 bytes: skill ID
                int skill_id = ParseHelper.getInt(stream);

                // 64 bytes: name
                String name = ParseHelper.getString(stream, 64);
                String nameTrim = name.Replace("\0", "");
                int n;
                bool isNumeric = int.TryParse(nameTrim, out n);//check to see if name was id
                if (n == skill_id && skill_id != 0)
                {
                    //was it a known boon?
                    foreach (Boon b in Boon.getBoonList())
                    {
                        if (skill_id == b.getID())
                        {
                            nameTrim = b.getName();
                        }
                    }
                }
                //Save

                SkillItem skill = new SkillItem(skill_id, nameTrim);

                skill.SetGW2APISkill(apiController);
                skill_data.addItem(skill);
            }
        }
        /// <summary>
        /// Parses combat related data
        /// </summary>
        private void parseCombatList(MemoryStream stream)
        {
            // 64 bytes: each combat
            while (stream.Length - stream.Position >= 64)
            {
                // 8 bytes: time
                long time = ParseHelper.getLong(stream);

                // 8 bytes: src_agent
                ulong src_agent = ParseHelper.getULong(stream);

                // 8 bytes: dst_agent
                ulong dst_agent = ParseHelper.getULong(stream);

                // 4 bytes: value
                int value = ParseHelper.getInt(stream);

                // 4 bytes: buff_dmg
                int buff_dmg = ParseHelper.getInt(stream);

                // 2 bytes: overstack_value
                ushort overstack_value = ParseHelper.getShort(stream);

                // 2 bytes: skill_id
                ushort skill_id = ParseHelper.getShort(stream);

                // 2 bytes: src_instid
                ushort src_instid = ParseHelper.getShort(stream);

                // 2 bytes: dst_instid
                ushort dst_instid = ParseHelper.getShort(stream);

                // 2 bytes: src_master_instid
                ushort src_master_instid = ParseHelper.getShort(stream);

                // 9 bytes: garbage
                ParseHelper.safeSkip(stream, 9);

                // 1 byte: iff
                //IFF iff = IFF.getEnum(f.read());
                IFF iff = new IFF(Convert.ToByte(stream.ReadByte())); //Convert.ToByte(stream.ReadByte());

                // 1 byte: buff
                ushort buff = (ushort)stream.ReadByte();

                // 1 byte: result
                //Result result = Result.getEnum(f.read());
                Result result = new Result(Convert.ToByte(stream.ReadByte()));

                // 1 byte: is_activation
                //Activation is_activation = Activation.getEnum(f.read());
                Activation is_activation = new Activation(Convert.ToByte(stream.ReadByte()));

                // 1 byte: is_buffremove
                //BuffRemove is_buffremove = BuffRemove.getEnum(f.read());
                BuffRemove is_buffremoved = new BuffRemove(Convert.ToByte(stream.ReadByte()));

                // 1 byte: is_ninety
                ushort is_ninety = (ushort)stream.ReadByte();

                // 1 byte: is_fifty
                ushort is_fifty = (ushort)stream.ReadByte();

                // 1 byte: is_moving
                ushort is_moving = (ushort)stream.ReadByte();

                // 1 byte: is_statechange
                //StateChange is_statechange = StateChange.getEnum(f.read());
                StateChange is_statechange = new StateChange(Convert.ToByte(stream.ReadByte()));

                // 1 byte: is_flanking
                ushort is_flanking = (ushort)stream.ReadByte();

                // 1 byte: is_flanking
                ushort is_shields = (ushort)stream.ReadByte();
                // 2 bytes: garbage
                ParseHelper.safeSkip(stream, 2);

                //save
                // Add combat
                combat_data.addItem(new CombatItem(time, src_agent, dst_agent, value, buff_dmg, overstack_value, skill_id,
                        src_instid, dst_instid, src_master_instid, iff, buff, result, is_activation, is_buffremoved,
                        is_ninety, is_fifty, is_moving, is_statechange, is_flanking, is_shields));
            }
        }

        public void CreateHTML(StreamWriter sw, bool[] settingsSnap)
        {
            ParsedLog log = new ParsedLog(log_data, boss_data, agent_data, skill_data, combat_data, mech_data, p_list, boss);
            HTMLBuilder htmlBuilder = new HTMLBuilder(log);
            htmlBuilder.CreateHTML(sw, settingsSnap);
        }

        /// <summary>
        /// Parses all the data again and link related stuff to each other
        /// </summary>
        private void fillMissingData(MemoryStream stream)
        {
            // Set Agent instid, first_aware and last_aware
            List<AgentItem> player_list = agent_data.getPlayerAgentList();
            List<AgentItem> agent_list = agent_data.getAllAgentsList();
            List<CombatItem> combat_list = combat_data.getCombatList();
            foreach (AgentItem a in agent_list)
            {
                bool assigned_first = false;
                foreach (CombatItem c in combat_list)
                {
                    if (a.getAgent() == c.getSrcAgent() && c.getSrcInstid() != 0)
                    {
                        if (!assigned_first)
                        {
                            a.setInstid(c.getSrcInstid());
                            a.setFirstAware(c.getTime());
                            assigned_first = true;
                        }
                        a.setLastAware(c.getTime());
                    }
                    else if (a.getAgent() == c.getDstAgent() && c.getDstInstid() != 0)
                    {
                        if (!assigned_first)
                        {
                            a.setInstid(c.getDstInstid());
                            a.setFirstAware(c.getTime());
                            assigned_first = true;
                        }
                        a.setLastAware(c.getTime());
                    }

                }
            }


            // Set Boss data agent, instid, first_aware, last_aware and name
            List<AgentItem> NPC_list = agent_data.getNPCAgentList();
            foreach (AgentItem NPC in NPC_list)
            {
                if (NPC.getProf().EndsWith(boss_data.getID().ToString()))
                {
                    if (boss_data.getAgent() == 0)
                    {
                        boss_data.setAgent(NPC.getAgent());
                        boss_data.setInstid(NPC.getInstid());
                        boss_data.setFirstAware(NPC.getFirstAware());
                        boss_data.setName(NPC.getName());
                        boss_data.setTough(NPC.getToughness());
                    }
                    boss_data.setLastAware(NPC.getLastAware());
                }
            }
            AgentItem bossAgent = agent_data.GetAgent(boss_data.getAgent());
            boss = new Boss(bossAgent);
            List<Point> bossHealthOverTime = new List<Point>();

            // Grab values threw combat data
            foreach (CombatItem c in combat_list)
            {
                if (c.getSrcInstid() == boss_data.getInstid() && c.isStateChange().getID() == 12)//max health update
                {
                    boss_data.setHealth((int)c.getDstAgent());

                }
                if (c.isStateChange().getID() == 13 && log_data.getPOV() == "N/A")//Point of View
                {
                    ulong pov_agent = c.getSrcAgent();
                    foreach (AgentItem p in player_list)
                    {
                        if (pov_agent == p.getAgent())
                        {
                            log_data.setPOV(p.getName());
                        }
                    }

                }
                else if (c.isStateChange().getID() == 9)//Log start
                {
                    log_data.setLogStart(c.getValue());
                }
                else if (c.isStateChange().getID() == 10)//log end
                {
                    log_data.setLogEnd(c.getValue());

                }
                //set health update
                if (c.getSrcInstid() == boss_data.getInstid() && c.isStateChange().getID() == 8)
                {
                    bossHealthOverTime.Add(new Point ( (int)(c.getTime() - boss_data.getFirstAware()), (int)c.getDstAgent() ));
                }

            }


            // Dealing with second half of Xera | ((22611300 * 0.5) + (25560600 * 0.5)

            if (boss_data.getID() == 16246)
            {
                int xera_2_instid = 0;
                foreach (AgentItem NPC in NPC_list)
                {
                    if (NPC.getProf().Contains("16286"))
                    {
                        bossHealthOverTime = new List<Point>();//reset boss health over time
                        xera_2_instid = NPC.getInstid();
                        boss_data.setHealth(24085950);
                        boss.addPhaseData(boss_data.getLastAware());
                        boss.addPhaseData(NPC.getFirstAware());
                        boss_data.setLastAware(NPC.getLastAware());
                        foreach (CombatItem c in combat_list)
                        {
                            if (c.getSrcInstid() == xera_2_instid)
                            {
                                c.setSrcInstid(boss_data.getInstid());
                            }
                            if (c.getDstInstid() == xera_2_instid)
                            {
                                c.setDstInstid(boss_data.getInstid());
                            }
                            //set health update
                            if (c.getSrcInstid() == boss_data.getInstid() && c.isStateChange().getID() == 8)
                            {
                                bossHealthOverTime.Add(new Point ( (int)(c.getTime() - boss_data.getFirstAware()), (int)c.getDstAgent() ));
                            }
                        }
                        break;
                    }
                }
            }
            //Dealing with Deimos split
            if (boss_data.getID() == 17154)
            {
                int deimos_2_instid = 0;
                foreach (AgentItem NPC in NPC_list)
                {
                    if (NPC.getProf().Contains("57069"))
                    {
                        deimos_2_instid = NPC.getInstid();
                        long oldAware = boss_data.getLastAware();
                        if (NPC.getLastAware() < boss_data.getLastAware())
                        {
                            // No split
                            break;
                        }
                        boss.addPhaseData(boss_data.getLastAware());
                        boss_data.setLastAware(NPC.getLastAware());
                        //List<CombatItem> fuckyou = combat_list.Where(x => x.getDstInstid() == deimos_2_instid ).ToList().Sum(x);
                        //int stop = 0;
                        foreach (CombatItem c in combat_list)
                        {
                            if (c.getTime() > oldAware)
                            {
                                if (c.getSrcInstid() == deimos_2_instid)
                                {
                                    c.setSrcInstid(boss_data.getInstid());

                                }
                                if (c.getDstInstid() == deimos_2_instid)
                                {
                                    c.setDstInstid(boss_data.getInstid());
                                }
                            }

                        }
                        break;
                    }
                }
            }
            boss_data.setHealthOverTime(bossHealthOverTime);//after xera in case of change

            // Re parse to see if the boss is dead and update last aware
            foreach (CombatItem c in combat_list)
            {
                //set boss dead
                if (c.isStateChange().getEnum() == "REWARD")//got reward
                {
                    log_data.setBossKill(true);
                    boss_data.setLastAware(c.getTime());
                    break;
                }
                //set boss dead
                if (c.getSrcInstid() == boss_data.getInstid() && c.isStateChange().getID() == 4 && !log_data.getBosskill())//change dead
                {
                    log_data.setBossKill(true);
                    boss_data.setLastAware(c.getTime());
                }

            }

            //players
            if (p_list.Count == 0)
            {

                //Fix Disconected players
                List<AgentItem> playerAgentList = agent_data.getPlayerAgentList();

                foreach (AgentItem playerAgent in playerAgentList)
                {
                    List<CombatItem> lp = combat_data.getStates(playerAgent.getInstid(), "DESPAWN", boss_data.getFirstAware(), boss_data.getLastAware());
                    Player player = new Player(playerAgent);
                    bool skip = false;
                    foreach (Player p in p_list)
                    {
                        if (p.getAccount() == player.getAccount())//is this a copy of original?
                        {
                            skip = true;
                        }
                    }
                    if (skip)
                    {
                        continue;
                    }
                    if (lp.Count > 0)
                    {
                        //make all actions of other isntances to original instid
                        int extra_login_Id = 0;
                        foreach (AgentItem extra in NPC_list)
                        {
                            if (extra.getAgent() == playerAgent.getAgent())
                            {

                                extra_login_Id = extra.getInstid();


                                foreach (CombatItem c in combat_list)
                                {
                                    if (c.getSrcInstid() == extra_login_Id)
                                    {
                                        c.setSrcInstid(playerAgent.getInstid());
                                    }
                                    if (c.getDstInstid() == extra_login_Id)
                                    {
                                        c.setDstInstid(playerAgent.getInstid());
                                    }

                                }
                                break;
                            }
                        }

                        player.SetDC(lp[0].getTime());
                        p_list.Add(player);
                    }
                    else//didnt dc
                    {
                        if (player.GetDC() == 0)
                        {
                            p_list.Add(player);
                        }

                    }
                }

            }
            agent_data.clean();
            // Sort
            p_list = p_list.OrderBy(a => int.Parse(a.getGroup())).ToList();//p_list.Sort((a, b)=>int.Parse(a.getGroup()) - int.Parse(b.getGroup()))
            setMechData();
        }

        private void setMechData() {
            List<int> mIDList = new List<int>();
            foreach (Player p in p_list)
            {
                List<CombatItem> down = combat_data.getStates(p.getInstid(), "CHANGE_DOWN", boss_data.getFirstAware(), boss_data.getLastAware());
                foreach (CombatItem pnt in down) {
                    mech_data.AddItem(new MechanicLog((long)((pnt.getTime() - boss_data.getFirstAware()) / 1000f), 0, "DOWN", 0, p, mech_data.GetPLoltyShape("DOWN")));
                }
                List<CombatItem> dead = combat_data.getStates(p.getInstid(), "CHANGE_DEAD", boss_data.getFirstAware(), boss_data.getLastAware());
                foreach (CombatItem pnt in dead)
                {
                    mech_data.AddItem(new MechanicLog((long)((pnt.getTime() - boss_data.getFirstAware()) / 1000f), 0, "DEAD", 0, p, mech_data.GetPLoltyShape("DEAD")));
                }
                List<DamageLog> dls = p.getDamageTakenLogs(boss_data, combat_data.getCombatList(), agent_data, mech_data, 0, boss_data.getAwareDuration());
                //Player hit by skill 3
                MechanicLog prevMech = null;
                foreach (DamageLog dLog in dls)
                {
                    string name = skill_data.getName(dLog.getID());
                    if (dLog.getResult().getID() < 3 ) {

                        foreach (Mechanic mech in mech_data.GetMechList(boss_data.getID()).Where(x=>x.GetMechType() == 3))
                        {
                            //Prevent multi hit attacks form multi registering
                            if (prevMech != null)
                            {
                                if (dLog.getID() == prevMech.GetSkill() && mech.GetName() == prevMech.GetName() && (dLog.getTime() / 1000f) == prevMech.GetTime())
                                {
                                    break;
                                }
                            }
                            if (dLog.getID() == mech.GetSkill())
                            {
                                
                                
                                 prevMech = new MechanicLog((long)(dLog.getTime() / 1000f), dLog.getID(), mech.GetName(), dLog.getDamage(), p, mech.GetPlotly());
                                
                                mech_data.AddItem(prevMech);
                                break;
                            }
                        }
                    }
                }
                //Player gain buff 0,7
                foreach (CombatItem c in combat_data.getCombatList().Where(x=>x.isBuffremove().getID() == 0 &&x.isStateChange().getID() == 0))
                {
                    if (p.getInstid() == c.getDstInstid())
                    {
                        if (c.isBuff() == 1 && c.getValue() > 0 && c.isBuffremove().getID() == 0 && c.getResult().getID() < 3)
                        {
                            String name = skill_data.getName(c.getSkillID());
                            //buff on player 0
                            foreach (Mechanic mech in mech_data.GetMechList(boss_data.getID()).Where(x => x.GetMechType() == 0))
                            {
                                if (c.getSkillID() == mech.GetSkill())
                                {
                                    //dst player
                                    mech_data.AddItem(new MechanicLog((long)((c.getTime() - boss_data.getFirstAware())/1000f), c.getSkillID(), mech.GetName(), c.getValue(), p, mech.GetPlotly()));
                                    break;
                                }
                            }
                            //player on player 7
                            foreach (Mechanic mech in mech_data.GetMechList(boss_data.getID()).Where(x => x.GetMechType() == 7))
                            {
                                if (c.getSkillID() == mech.GetSkill())
                                {
                                    //dst player
                                    mech_data.AddItem(new MechanicLog((long)((c.getTime() - boss_data.getFirstAware()) / 1000f), c.getSkillID(), mech.GetName(), c.getValue(), p, mech.GetPlotly()));
                                    //src player
                                    mech_data.AddItem(new MechanicLog((long)((c.getTime() - boss_data.getFirstAware()) / 1000f), c.getSkillID(), mech.GetName(), c.getValue(), p_list.FirstOrDefault(i=>i.getInstid() == c.getSrcInstid()), mech.GetPlotly()));
                                    break;
                                }
                            }

                        }
                    }
                }
            }

        }

        bool[] SnapSettings;

        //Creating CSV---------------------------------------------------------------------------------
        public void CreateCSV(StreamWriter sw,String delimiter, bool[] settingsSnap)
        {
            double fight_duration = (boss_data.getAwareDuration()) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fight_duration);
            String durationString = duration.ToString("mm") +":" + duration.ToString("ss") ;
            SnapSettings = settingsSnap;
            HTMLHelper.SnapSettings = settingsSnap;
            sw.Write("Group" + delimiter + 
                    "Class" + delimiter + 
                    "Character" + delimiter + 
                    "Account Name" + delimiter +
                    "Boss DPS" + delimiter +
                    "Boss Physical" + delimiter +
                    "Boss Condi" + delimiter + 
                    "All DPS"+ delimiter +
                    "Quick" +delimiter+ 
                    "Alacrity"+delimiter +
                    "Might" + delimiter +
                    "Boss Team DPS" + delimiter +
                    "All Team DPS" + delimiter +
                    "Time" + delimiter +
                    "Cleave" + delimiter +
                    "Team Cleave"  );
            sw.Write("\r\n");

            int[] teamStats= { 0,0,0};
            foreach (Player p in p_list)
            {
                string[] finaldps = HTMLHelper.getFinalDPS(boss_data,combat_data,agent_data,p,boss, 0).Split('|');
                teamStats[0] += Int32.Parse(finaldps[6]);
                teamStats[1] += Int32.Parse(finaldps[0]);
                teamStats[2] += (Int32.Parse(finaldps[0]) - Int32.Parse(finaldps[6]));
            }

            foreach (Player p in p_list)
            {
                string[] finaldps = HTMLHelper.getFinalDPS(boss_data, combat_data, agent_data, p, boss, 0).Split('|');
                sw.Write(p.getGroup() + delimiter + // group
                        p.getProf() + delimiter +  // class
                        p.getCharacter() + delimiter + // character
                        p.getAccount().Substring(1) + delimiter + // account
                        finaldps[6] + delimiter + // dps
                        finaldps[8] + delimiter + // physical
                        finaldps[10] + delimiter + // condi
                        finaldps[0] + delimiter); // all dps

                Dictionary<int, string> boonArray = HTMLHelper.getfinalboons(boss_data,combat_data,skill_data,agent_data, boss,p, 0);
                sw.Write(boonArray[1187] + delimiter + // Quickness
                        boonArray[30328] + delimiter + // Alacrity
                        boonArray[740] + delimiter); // Might

                sw.Write(teamStats[0] + delimiter  // boss dps
                        + teamStats[1] + delimiter // all
                        + durationString + delimiter + // duration
                    ( Int32.Parse(finaldps[0])-Int32.Parse(finaldps[6])).ToString()+delimiter // cleave
                        +teamStats[2]); // team cleave
                sw.Write("\r\n");
            }
           

        }
        /// <summary>
        /// Returns the img links and colors code for the given data
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
       
        
    }
}
