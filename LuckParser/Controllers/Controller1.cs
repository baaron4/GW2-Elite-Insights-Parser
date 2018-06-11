using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Models.ParseModels;
using LuckParser.Models.ParseEnums;
using System.Drawing;
using System.Net;
using LuckParser.Models;
using System.IO.Compression;
using System.Windows.Forms;
using static LuckParser.Models.ParseModels.Boss;
//recomend CTRL+M+O to collapse all
namespace LuckParser.Controllers
{
    public class Controller1
    {
        private GW2APIController APIContrioller = new GW2APIController();            
        private static String FilterStringChars(string str)
        {
            string filtered = "";
            string filter = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
            foreach (char c in str)
            {
                if (filter.Contains(c))
                {
                    filtered += c;
                }
            }
            return filtered;
        }

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
                long agent = ParseHelper.getLong(stream);

                // 4 bytes: profession
                int prof = ParseHelper.getInt(stream);

                // 4 bytes: is_elite
                int is_elite = ParseHelper.getInt(stream);

                // 4 bytes: toughness
                int toughness = ParseHelper.getInt(stream);

                // 4 bytes: healing
                int healing = ParseHelper.getInt(stream);

                // 4 bytes: condition
                int condition = ParseHelper.getInt(stream);

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
                long src_agent = ParseHelper.getLong(stream);

                // 8 bytes: dst_agent
                long dst_agent = ParseHelper.getLong(stream);

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
            List<long[]> bossHealthOverTime = new List<long[]>();

            // Grab values threw combat data
            foreach (CombatItem c in combat_list)
            {
                if (c.getSrcInstid() == boss_data.getInstid() && c.isStateChange().getID() == 12)//max health update
                {
                    boss_data.setHealth((int)c.getDstAgent());

                }
                if (c.isStateChange().getID() == 13 && log_data.getPOV() == "N/A")//Point of View
                {
                    long pov_agent = c.getSrcAgent();
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
                    bossHealthOverTime.Add(new long[] { c.getTime() - boss_data.getFirstAware(), c.getDstAgent() });
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
                        bossHealthOverTime = new List<long[]>();//reset boss health over time
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
                                bossHealthOverTime.Add(new long[] { c.getTime() - boss_data.getFirstAware(), c.getDstAgent() });
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
            // Sort
            p_list = p_list.OrderBy(a => int.Parse(a.getGroup())).ToList();//p_list.Sort((a, b)=>int.Parse(a.getGroup()) - int.Parse(b.getGroup()))
            setMechData();
        }


        //Statistics--------------------------------------------------------------------------------------------------------------------------------------------------------
        private List<Boon> present_boons =  new List<Boon>();//Used only for Boon tables
        private List<Boon> present_offbuffs = new List<Boon>();//Used only for Off Buff tables
        private List<Boon> present_defbuffs = new List<Boon>();//Used only for Def Buff tables
        private Dictionary<int, List<Boon>> present_personnal = new Dictionary<int, List<Boon>>();//Used only for personnal
        /// <summary>
        /// Checks the combat data and gets buffs that were present during the fight
        /// </summary>
        /// <param name="SnapSettings">Settings to use</param>
        private void setPresentBoons(bool[] SnapSettings) {
            List<SkillItem> s_list = skill_data.getSkillList();
            if (SnapSettings[3])
            {//Main boons
                foreach (Boon boon in Boon.getBoonList())
                {
                    if (s_list.Exists(x => x.getID() == boon.getID()))
                    {
                        present_boons.Add(boon);
                    }
                }
            }
            if (SnapSettings[4])
            {//Important Class specefic boons
                foreach (Boon boon in Boon.getOffensiveTableList())
                {
                    if (s_list.Exists(x => x.getID() == boon.getID()))
                    {
                        present_offbuffs.Add(boon);
                    }
                }
                foreach (Boon boon in Boon.getDefensiveTableList())
                {
                    if (s_list.Exists(x => x.getID() == boon.getID()))
                    {
                        present_defbuffs.Add(boon);
                    }
                }
            }
            if (SnapSettings[5])
            {//All class specefic boons
                List<CombatItem> c_list = combat_data.getCombatList();
                foreach (Player p in p_list)
                {
                    present_personnal[p.getInstid()] = new List<Boon>();
                    foreach (Boon boon in Boon.getRemainingBuffsList(p.getProf()))
                    {
                        if (c_list.Exists(x => x.getSkillID() == boon.getID() && x.getDstInstid() == p.getInstid()))
                        {
                            present_personnal[p.getInstid()].Add(boon);
                        }
                    }
                }
            }        
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
        //Generate HTML---------------------------------------------------------------------------------------------------------------------------------------------------------
        //Methods that make it easier to create Javascript graphs      
         /// <summary>
        /// Creates the dps graph
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateDPSGraph(StreamWriter sw, int phase_index)
        {
            //Generate DPS graph
            sw.Write("<div id=\"DPSGraph"+ phase_index + "\" style=\"height: 600px;width:1200px; display:inline-block \"></div>");
            sw.Write("<script>");

            sw.Write("var data = [");
            int maxDPS = 0;
            List<int[]> totalDpsAllPlayers = new List<int[]>();
            foreach (Player p in p_list)
            {
                //Adding dps axis
                List<int[]> playerbossdpsgraphdata = new List<int[]>(HTMLHelper.getBossDPSGraph(boss_data, combat_data, agent_data,p, boss, phase_index));
                if (totalDpsAllPlayers.Count == 0)
                {
                    //totalDpsAllPlayers = new List<int[]>(playerbossdpsgraphdata);
                    foreach (int[] point in playerbossdpsgraphdata)
                    {
                        int time = point[0];
                        int dmg = point[1];
                        totalDpsAllPlayers.Add(new int[] { time, dmg });
                    }
                }

                sw.Write("{y: [");
                int pbdgdCount = 0;
                foreach (int[] dp in playerbossdpsgraphdata)
                {
                    if (pbdgdCount == playerbossdpsgraphdata.Count - 1)
                    {
                        sw.Write("'" + dp[1] + "'");
                    }
                    else
                    {
                        sw.Write("'" + dp[1] + "',");
                    }
                    pbdgdCount++;

                    if (dp[1] > maxDPS) { maxDPS = dp[1]; }
                    if (totalDpsAllPlayers.Count != 0)
                    {
                        if (totalDpsAllPlayers.FirstOrDefault(x => x[0] == dp[0]) != null)
                            totalDpsAllPlayers.FirstOrDefault(x => x[0] == dp[0])[1] += dp[1];
                    }
                }
                if (playerbossdpsgraphdata.Count == 0)
                {
                    sw.Write("'0'");
                }

                sw.Write("],");
                //add time axis
                sw.Write("x: [");
                pbdgdCount = 0;
                foreach (int[] dp in playerbossdpsgraphdata)
                {
                    if (pbdgdCount == playerbossdpsgraphdata.Count - 1)
                    {
                        sw.Write("'" + dp[0] + "'");
                    }
                    else
                    {
                        sw.Write("'" + dp[0] + "',");
                    }
                    pbdgdCount++;
                }
                if (playerbossdpsgraphdata.Count == 0)
                {
                    sw.Write("'0'");
                }

                sw.Write("],");
                sw.Write("mode: 'lines'," +
                        "line: {shape: 'spline',color:'" + GetLink("Color-" + p.getProf()) + "'}," +
                        "name: '" + p.getCharacter() + " DPS'" +
                        "},");
                if (SnapSettings[0])
                {//Turns display on or off
                    sw.Write("{");
                    //Adding dps axis
                    List<int[]> playertotaldpsgraphdata = HTMLHelper.getTotalDPSGraph(boss_data, combat_data, agent_data,p,boss,phase_index);
                    sw.Write("y: [");
                    pbdgdCount = 0;
                    foreach (int[] dp in playertotaldpsgraphdata)
                    {
                        if (pbdgdCount == playertotaldpsgraphdata.Count - 1)
                        {
                            sw.Write("'" + dp[1] + "'");
                        }
                        else
                        {
                            sw.Write("'" + dp[1] + "',");
                        }
                        pbdgdCount++;

                    }
                    //cuts off extra comma
                    if (playertotaldpsgraphdata.Count == 0)
                    {
                        sw.Write("'0'");
                    }

                    sw.Write("],");
                    //add time axis
                    sw.Write("x: [");
                    pbdgdCount = 0;
                    foreach (int[] dp in playertotaldpsgraphdata)
                    {
                        if (pbdgdCount == playertotaldpsgraphdata.Count - 1)
                        {
                            sw.Write("'" + dp[0] + "'");
                        }
                        else
                        {
                            sw.Write("'" + dp[0] + "',");
                        }

                        pbdgdCount++;
                    }
                    if (playertotaldpsgraphdata.Count == 0)
                    {
                        sw.Write("'0'");
                    }

                    sw.Write("],");
                    sw.Write("mode: 'lines'," +
                            "line: {shape: 'spline',color:'" + GetLink("Color-" + p.getProf()) + "'}," +
                            "visible:'legendonly'," +
                            "name: '" + p.getCharacter() + "TDPS'" + "},");
                }
            }
            //All Player dps
            sw.Write("{");
            //Adding dps axis

            sw.Write("y: [");
            int tdalpcount = 0;
            foreach (int[] dp in totalDpsAllPlayers)
            {
                if (tdalpcount == totalDpsAllPlayers.Count - 1)
                {
                    sw.Write("'" + dp[1] + "'");
                }
                else
                {
                    sw.Write("'" + dp[1] + "',");
                }
                tdalpcount++;
            }

            sw.Write("],");
            //add time axis
            sw.Write("x: [");
            tdalpcount = 0;
            foreach (int[] dp in totalDpsAllPlayers)
            {
                if (tdalpcount == totalDpsAllPlayers.Count - 1)
                {
                    sw.Write("'" + dp[0] + "'");
                }
                else
                {
                    sw.Write("'" + dp[0] + "',");
                }

                tdalpcount++;
            }

            sw.Write("],");
            sw.Write(" mode: 'lines'," +
                    "line: {shape: 'spline'}," +
                    "visible:'legendonly'," +
                    "name: 'All Player Dps'");
            sw.Write("},");
            List<Mechanic> presMech = mech_data.GetMechList(boss_data.getID());
            List<string> distMech = presMech.Select(x => x.GetAltName()).Distinct().ToList();
            foreach (string mechAltString in distMech)
            {
                List<Mechanic> mechs = presMech.Where(x => x.GetAltName() == mechAltString).ToList();
                List<MechanicLog> filterdList = new List<MechanicLog>();
                foreach (Mechanic me in mechs)
                {
                    filterdList.AddRange(mech_data.GetMDataLogs().Where(x => x.GetSkill() == me.GetSkill()).ToList());
                }
                Mechanic mech = mechs[0];
                //List<MechanicLog> filterdList = mech_data.GetMDataLogs().Where(x => x.GetName() == mech.GetName()).ToList();
                sw.Write("{");
                sw.Write("y: [");

                int mechcount = 0;
                foreach (MechanicLog ml in filterdList)
                {
                    int[] check = HTMLHelper.getBossDPSGraph(boss_data, combat_data, agent_data,ml.GetPlayer(), boss, phase_index).FirstOrDefault(x => x[0] == ml.GetTime());
                    if (mechcount == filterdList.Count - 1)
                    {
                        if (check != null)
                        {
                            sw.Write("'" + check[1] + "'");
                        }
                        else
                        {
                            sw.Write("'" + 10000 + "'");
                        }

                    }
                    else
                    {
                        if (check != null)
                        {
                            sw.Write("'" + check[1] + "',");
                        }
                        else
                        {
                            sw.Write("'" + 10000 + "',");
                        }
                    }

                    mechcount++;
                }
                sw.Write("],");
                //add time axis
                sw.Write("x: [");
                tdalpcount = 0;
                mechcount = 0;
                foreach (MechanicLog ml in filterdList)
                {
                    if (mechcount == filterdList.Count - 1)
                    {
                        sw.Write("'" + ml.GetTime() + "'");
                    }
                    else
                    {
                        sw.Write("'" + ml.GetTime() + "',");
                    }

                    mechcount++;
                }

                sw.Write("],");
                sw.Write(" mode: 'markers',");
                if (mech.GetName() == "DEAD" || mech.GetName() == "DOWN")
                {
                    //sw.Write("visible:'legendonly',");
                }
                else
                {
                    sw.Write("visible:'legendonly',");
                }
                sw.Write("type:'scatter'," +
                        "marker:{" + mech.GetPlotly() + "size: 15" + "}," +
                        "text:[");
                foreach (MechanicLog ml in filterdList)
                {
                    if (mechcount == filterdList.Count - 1)
                    {
                        sw.Write("'" + ml.GetPlayer().getCharacter() + "'");
                    }
                    else
                    {
                        sw.Write("'" + ml.GetPlayer().getCharacter() + "',");
                    }

                    mechcount++;
                }

                sw.Write("]," +
                        " name: '" + mech.GetAltName() + "'");
                sw.Write("},");
            }
            //Downs and deaths

            List<String> DnDStringList = new List<string>();
            DnDStringList.Add("DOWN");
            DnDStringList.Add("DEAD");
            foreach (string state in DnDStringList)
            {
                int mcount = 0;
                List<MechanicLog> DnDList = mech_data.GetMDataLogs().Where(x => x.GetName() == state).ToList();
                sw.Write("{");
                sw.Write("y: [");
                foreach (MechanicLog ml in DnDList)
                {
                    int[] check = HTMLHelper.getBossDPSGraph(boss_data, combat_data, agent_data, ml.GetPlayer(),boss, phase_index).FirstOrDefault(x => x[0] == ml.GetTime());
                    if (mcount == DnDList.Count - 1)
                    {
                        if (check != null)
                        {
                            sw.Write("'" + check[1] + "'");
                        }
                        else
                        {
                            sw.Write("'" + 10000 + "'");
                        }

                    }
                    else
                    {
                        if (check != null)
                        {
                            sw.Write("'" + check[1] + "',");
                        }
                        else
                        {
                            sw.Write("'" + 10000 + "',");
                        }
                    }

                    mcount++;
                }
                sw.Write("],");
                //add time axis
                sw.Write("x: [");
                tdalpcount = 0;
                mcount = 0;
                foreach (MechanicLog ml in DnDList)
                {
                    if (mcount == DnDList.Count - 1)
                    {
                        sw.Write("'" + ml.GetTime() + "'");
                    }
                    else
                    {
                        sw.Write("'" + ml.GetTime() + "',");
                    }

                    mcount++;
                }

                sw.Write("],");
                sw.Write(" mode: 'markers',");
                if (state == "DEAD" || state == "DOWN")
                {
                    //sw.Write("visible:'legendonly',");
                }
                else
                {
                    sw.Write("visible:'legendonly',");
                }
                sw.Write("type:'scatter'," +
                    "marker:{" + mech_data.GetPLoltyShape(state) + "size: 15" + "}," +
                    "text:[");
                foreach (MechanicLog ml in DnDList)
                {
                    if (mcount == DnDList.Count - 1)
                    {
                        sw.Write("'" + ml.GetPlayer().getCharacter() + "'");
                    }
                    else
                    {
                        sw.Write("'" + ml.GetPlayer().getCharacter() + "',");
                    }

                    mcount++;
                }

                sw.Write("]," +
                        " name: '" + state + "'");
                sw.Write("},");
            }
            if (maxDPS > 0)
            {
                //sw.Write(",");
                //Boss Health
                sw.Write("{");
                //Adding dps axis
                sw.Write("y: [");

                float scaler = boss_data.getHealth() / maxDPS;
                int hotCount = 0;
                List<long[]> BossHOT = boss_data.getHealthOverTime();
                foreach (long[] dp in BossHOT)
                {
                    if (hotCount == BossHOT.Count - 1)
                    {
                        sw.Write("'" + (dp[1] / 10000f) * maxDPS + "'");
                    }
                    else
                    {
                        sw.Write("'" + (dp[1] / 10000f) * maxDPS + "',");
                    }
                    hotCount++;

                }

                sw.Write("],");
                //text axis is boss hp in %
                sw.Write("text: [");

                float scaler2 = boss_data.getHealth() / 100;
                hotCount = 0;
                foreach (long[] dp in BossHOT)
                {
                    if (hotCount == BossHOT.Count - 1)
                    {
                        sw.Write("'" + dp[1] / 100f + "% HP'");
                    }
                    else
                    {
                        sw.Write("'" + dp[1] / 100f + "% HP',");
                    }
                    hotCount++;

                }

                sw.Write("],");
                //add time axis
                sw.Write("x: [");
                hotCount = 0;
                foreach (long[] dp in BossHOT)
                {
                    if (hotCount == BossHOT.Count - 1)
                    {
                        sw.Write("'" + dp[0] / 1000f + "'");
                    }
                    else
                    {
                        sw.Write("'" + dp[0] / 1000f + "',");
                    }

                    hotCount++;
                }

                sw.Write("],");
                sw.Write(" mode: 'lines'," +
                        " line: {shape: 'spline', dash: 'dashdot'}," +
                        "hoverinfo: 'text'," +
                        " name: 'Boss health'");
                sw.Write("}");
            }
            else
            {
                sw.Write("{}");
            }
            sw.Write("];" +
                    "var layout = {" +
                        "xaxis:{title:'DPS'}," +
                        "yaxis:{title:'Time(sec)'}," +
                        //"legend: { traceorder: 'reversed' }," +
                        "hovermode: 'compare'," +
                        "legend: {orientation: 'h'}," +
                        // "yaxis: { title: 'DPS', domain: [0.51, 1] }," +


                        "font: { color: '#ffffff' }," +
                        "paper_bgcolor: 'rgba(0,0,0,0)'," +
                        "plot_bgcolor: 'rgba(0,0,0,0)'" +
                    "};" +
                    "Plotly.newPlot('DPSGraph" + phase_index + "', data, layout);");
            sw.Write("</script> ");
        }
        private void GetRoles()
        {
            //tags: tank,healer,dps(power/condi)
            //Roles:greenteam,green split,cacnoneers,flakkiter,eater,KCpusher,agony,epi,handkiter,golemkiter,orbs
        }
        private void PrintWeapons(StreamWriter sw, Player p)
        {
            //print weapon sets
            string[] wep = p.getWeaponsArray(skill_data,combat_data,boss_data,agent_data);
            sw.Write("<div>");
            if (wep[0] != null)
            {
                sw.Write("<img src=\"" + GetLink(wep[0]) + " \" alt=\"" + wep[0] + "\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" +wep[0] + "\">");
            }
            else if(wep[1] != null)
            {
                sw.Write("<img src=\"" + GetLink("Question") + " \" alt=\"Unknown\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Unknown\">");
            }
            if (wep[1] != null)
            {
                if (wep[1] != "2Hand")
                {
                    sw.Write("<img src=\"" + GetLink(wep[1]) + " \" alt=\"" + wep[1] + "\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" + wep[1] + "\">");
                }
            }
            else
            {
                sw.Write("<img src=\"" + GetLink("Question") + " \" alt=\"Unknown\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Unknown\">");
            }
            if (wep[2] == null && wep[3] == null)
            {
                
            }
            else {
                sw.Write(" / ");
            }
            
            if (wep[2] != null)
            {
                sw.Write("<img src=\"" + GetLink(wep[2]) + " \" alt=\"" + wep[2] + "\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" + wep[2] + "\">");
            }
            else if(wep[3] != null)
            {
                sw.Write("<img src=\"" + GetLink("Question") + " \" alt=\"Unknown\" height=\"18\" width=\"18\" >");
            }
            if (wep[3] != null)
            {
                if (wep[3] != "2Hand")
                {
                    sw.Write("<img src=\"" + GetLink(wep[3]) + " \" alt=\"" + wep[3] + "\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" + wep[3] + "\">");
                }
            }
            else
            {
                //sw.Write("<img src=\"" + GetLink("Question") + " \" alt=\"Unknown\" height=\"18\" width=\"18\" >");
            }
            sw.Write("<br>");
            sw.Write("</div>");
        }

        bool[] SnapSettings;
        /// <summary>
        /// Creates the composition table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateCompTable(StreamWriter sw) {
            int groupCount = 0;
            int firstGroup = 11;
            foreach (Player play in p_list)
            {
                int playerGroup = int.Parse(play.getGroup());
                if (playerGroup > groupCount)
                {
                    groupCount = playerGroup;
                }
                if (playerGroup < firstGroup)
                {
                    firstGroup = playerGroup;
                }
            }
            //generate comp table
            sw.Write("<table class=\"table\"");
            {
                sw.Write("<tbody>");
                for (int n = firstGroup; n <= groupCount; n++)
                {
                    sw.Write("<tr>");
                    List<Player> sortedList = p_list.Where(x => int.Parse(x.getGroup()) == n).ToList();
                    if (sortedList.Count > 0)
                    {
                        foreach (Player gPlay in sortedList)
                        {
                            string charName = "";
                            if (gPlay.getCharacter().Length > 10)
                            {
                                charName = gPlay.getCharacter().Substring(0, 10);
                            }
                            else
                            {
                                charName = gPlay.getCharacter().ToString();
                            }
                            //Getting Build
                            string build = "";
                            if (gPlay.getCondition() > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/5/54/Condition_Damage.png\" alt=\"Condition Damage\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Condition Damage-" + gPlay.getCondition() + "\">";//"<span class=\"badge badge-warning\">Condi("+ gPlay.getCondition() + ")</span>";
                            }
                            if (gPlay.getHealing() > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/8/81/Healing_Power.png\" alt=\"Healing Power\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Healing Power-" + gPlay.getHealing() + "\">";//"<span class=\"badge badge-success\">Heal("+ gPlay.getHealing() + ")</span>";
                            }
                            if (gPlay.getToughness() > 0)
                            {
                                build += "<img src=\"https://wiki.guildwars2.com/images/1/12/Toughness.png\" alt=\"Toughness\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"Toughness-" + gPlay.getToughness() + "\">";//"<span class=\"badge badge-secondary\">Tough("+ gPlay.getToughness() + ")</span>";
                            }
                            sw.Write("<td style=\"width: 120px; border:1px solid #EE5F5B;\">");
                            {
                                sw.Write("<img src=\"" + GetLink(gPlay.getProf().ToString()) + " \" alt=\"" + gPlay.getProf().ToString() + "\" height=\"18\" width=\"18\" >");
                                sw.Write(build);
                                PrintWeapons(sw,gPlay);
                                sw.Write(charName);
                            }
                            sw.Write("</td>");
                        }
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</tbody>");
            }
            
            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the dps table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="fight_duration">Duration of the fight</param>
        private void CreateDPSTable(StreamWriter sw, int phase_index) {
            //generate dps table
            PhaseData phase = boss.getPhases(boss_data, combat_data.getCombatList(), agent_data)[phase_index];
            sw.Write("<script> $(function () { $('#dps_table"+ phase_index + "').DataTable({ \"order\": [[4, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dps_table" + phase_index + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        sw.Write("<th>Account</th>");
                        sw.Write("<th>Boss DPS</th>");
                        sw.Write("<th>Power</th>");
                        sw.Write("<th>Condi</th>");
                        sw.Write("<th>All DPS</th>");
                        sw.Write("<th>Power</th>");
                        sw.Write("<th>Condi</th>");
                        sw.Write("<th><img src=" + GetLink("Downs") + " alt=\"Downs\" title=\"Times downed\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Dead") + " alt=\"Dead\" title=\"Time died\" height=\"18\" width=\"18\"></th>");
                    }
                    sw.Write("</tr>");
                }
                
                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                foreach (Player player in p_list)
                {
                    string[] dmg = HTMLHelper.getFinalDPS(boss_data,combat_data, agent_data,player,boss, phase_index).Split('|');
                    string[] stats = HTMLHelper.getFinalStats(boss_data, combat_data, agent_data,player,boss, phase_index);
                    //gather data for footer
                    footerList.Add(new string[] { player.getGroup().ToString(), dmg[0], dmg[1], dmg[2], dmg[3], dmg[4], dmg[5], dmg[6], dmg[7], dmg[8], dmg[9], dmg[10], dmg[11] });
                    sw.Write("<tr>");
                    {
                        sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                        sw.Write("<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>");
                        sw.Write("<td>" + player.getCharacter().ToString() + "</td>");
                        sw.Write("<td>" + player.getAccount().TrimStart(':') + "</td>");
                        //Boss dps
                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dmg[7] + " dmg \">" + dmg[6] + "</span>" + "</td>");
                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dmg[9] + " dmg \">" + dmg[8] + "</span>" + "</td>");
                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dmg[11] + " dmg \">" + dmg[10] + "</span>" + "</td>");
                        //All DPS
                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dmg[1] + " dmg \">" + dmg[0] + "</span>" + "</td>");
                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dmg[3] + " dmg \">" + dmg[2] + "</span>" + "</td>");
                        sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dmg[5] + " dmg \">" + dmg[4] + "</span>" + "</td>");
                        sw.Write("<td>" + stats[6] + "</td>");
                        TimeSpan timedead = TimeSpan.FromMilliseconds(Double.Parse(stats[9]));
                        long fight_duration = (phase.end - phase.start)/1000;
                        if (timedead > TimeSpan.Zero)
                        {
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + " (" + (int)((timedead.TotalSeconds / fight_duration) * 100) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</span>" + " </td>");
                        }
                        else
                        {
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died\"> 0</span>" + " </td>");
                        }
                    }            
                    sw.Write("</tr>");           
                }
                sw.Write("</tbody>");
                if (p_list.Count() > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footerList.Select(x => x[0]).Distinct())
                        {
                            List<string[]> groupList = footerList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[8])) + " dmg \">" + groupList.Sum(c => int.Parse(c[7])) + "</span>" + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[10])) + " dmg \">" + groupList.Sum(c => int.Parse(c[9])) + "</span>" + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[12])) + " dmg \">" + groupList.Sum(c => int.Parse(c[11])) + "</span>" + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[2])) + " dmg \">" + groupList.Sum(c => int.Parse(c[1])) + "</span>" + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[4])) + " dmg \">" + groupList.Sum(c => int.Parse(c[3])) + "</span>" + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => int.Parse(c[6])) + " dmg \">" + groupList.Sum(c => int.Parse(c[5])) + "</span>" + "</td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Total</td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[8])) + " dmg \">" + footerList.Sum(c => int.Parse(c[7])) + "</span>" + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[10])) + " dmg \">" + footerList.Sum(c => int.Parse(c[9])) + "</span>" + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[12])) + " dmg \">" + footerList.Sum(c => int.Parse(c[11])) + "</span>" + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[2])) + " dmg \">" + footerList.Sum(c => int.Parse(c[1])) + "</span>" + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[4])) + " dmg \">" + footerList.Sum(c => int.Parse(c[3])) + "</span>" + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => int.Parse(c[6])) + " dmg \">" + footerList.Sum(c => int.Parse(c[5])) + "</span>" + "</td>");
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</tfoot>");
                }
            }
           
            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the damage stats table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="fight_duration">Duration of the fight</param>
        private void CreateDMGStatsTable(StreamWriter sw, int phase_index) {
            //generate dmgstats table
            PhaseData phase = boss.getPhases(boss_data, combat_data.getCombatList(), agent_data)[phase_index];
            sw.Write("<script> $(function () { $('#dmgstats_table"+ phase_index + "').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dmgstats_table"+ phase_index + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        sw.Write("<th><img src=" + GetLink("Crit") + " alt=\"Crits\" title=\"Percent time hits critical\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Scholar") + " alt=\"Scholar\" title=\"Percent time hits while above 90% health\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("SwS") + " alt=\"SwS\" title=\"Percent time hits while moveing\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Flank") + " alt=\"Flank\" title=\"Percent time hits while flanking\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Glance") + " alt=\"Glance\" title=\"Percent time hits while glanceing\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Blinded") + " alt=\"Miss\" title=\"Number of hits while blinded\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Interupts") + " alt=\"Interupts\" title=\"Number of hits interupted?/hits used to interupt\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Invuln") + " alt=\"Ivuln\" title=\"times the enemy was invulnerable to attacks\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Wasted") + " alt=\"Wasted\" title=\"Time wasted(in seconds) interupting skill casts\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Saved") + " alt=\"Saved\" title=\"Time saved(in seconds) interupting skill casts\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Swap") + " alt=\"Swap\" title=\"Times weapon swapped\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Downs") + " alt=\"Downs\" title=\"Times downed\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Dead") + " alt=\"Dead\" title=\"Time died\" height=\"18\" width=\"18\"></th>");
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                {
                    foreach (Player player in p_list)
                    {
                        string[] stats = HTMLHelper.getFinalStats(boss_data,combat_data,agent_data,player,boss, phase_index);
                        TimeSpan timedead = TimeSpan.FromMilliseconds(Double.Parse(stats[9]));//dead 
                                                                                              //gather data for footer
                        footerList.Add(new string[] { player.getGroup().ToString(), stats[0], stats[1], stats[2], stats[3], stats[4], stats[10], stats[11], stats[12], stats[13], stats[5], stats[6] });
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>");
                            sw.Write("<td>" + player.getCharacter().ToString() + "</td>");

                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[1] + " out of " + stats[0] + "hits \">" + (int)(Double.Parse(stats[1]) / Double.Parse(stats[0]) * 100) + "%</span>" + "</td>");//crit
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[2] + " out of " + stats[0] + " hits <br> Pure Scholar Damage: " + stats[19] + "<br> Effective Damage Increase: " + (int)(Double.Parse(stats[19]) / Double.Parse(stats[20]) * 100) + "% \">" + (int)(Double.Parse(stats[2]) / Double.Parse(stats[0]) * 100) + "%</span>" + "</td>");//scholar
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[3] + " out of " + stats[0] + "hits \">" + (int)(Double.Parse(stats[3]) / Double.Parse(stats[0]) * 100) + "%</span>" + "</td>");//sws
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[4] + " out of " + stats[0] + "hits \">" + (int)(Double.Parse(stats[4]) / Double.Parse(stats[0]) * 100) + "%</span>" + "</td>");//flank
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[10] + " out of " + stats[0] + "hits \">" + (int)(Double.Parse(stats[10]) / Double.Parse(stats[0]) * 100) + "%</span>" + "</td>");//glance
                            sw.Write("<td>" + stats[11] + "</td>");//misses
                            sw.Write("<td>" + stats[12] + "</td>");//interupts
                            sw.Write("<td>" + stats[13] + "</td>");//dmg invulned
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[15] + "cancels \">" + stats[14] + "</span>" + "</td>");//time wasted
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[18] + "cancels \">" + stats[17] + "</span>" + "</td>");//timesaved
                            sw.Write("<td>" + stats[5] + "</td>");//w swaps
                            sw.Write("<td>" + stats[6] + "</td>");//downs
                            long fight_duration = (phase.end - phase.start) / 1000;
                            if (timedead > TimeSpan.Zero)
                            {
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + "(" + (int)((timedead.TotalSeconds / fight_duration) * 100) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</span>" + " </td>");
                            }
                            else
                            {
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died 100% Alive) \"> </span>" + " </td>");
                            }
                        }
                        sw.Write("</tr>");
                    }
                }
                sw.Write("</tbody>");
                if (p_list.Count() > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footerList.Select(x => x[0]).Distinct())
                        {
                            List<string[]> groupList = footerList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                sw.Write("<td>" + (int)(100 * groupList.Sum(c => Double.Parse(c[2]) / Double.Parse(c[1])) / groupList.Count) + "%</td>");
                                sw.Write("<td>" + (int)(100 * groupList.Sum(c => Double.Parse(c[3]) / Double.Parse(c[1])) / groupList.Count) + "%</td>");
                                sw.Write("<td>" + (int)(100 * groupList.Sum(c => Double.Parse(c[4]) / Double.Parse(c[1])) / groupList.Count) + "%</td>");
                                sw.Write("<td>" + (int)(100 * groupList.Sum(c => Double.Parse(c[5]) / Double.Parse(c[1])) / groupList.Count) + "%</td>");
                                sw.Write("<td>" + (int)(100 * groupList.Sum(c => Double.Parse(c[6]) / Double.Parse(c[1])) / groupList.Count) + "%</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[7])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[8])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[9])) + "</td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[10])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[11])) + "</td>");
                                sw.Write("<td></td>");
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Total</td>");
                            sw.Write("<td>" + (int)(100 * footerList.Sum(c => Double.Parse(c[2]) / Double.Parse(c[1])) / footerList.Count) + "%</td>");
                            sw.Write("<td>" + (int)(100 * footerList.Sum(c => Double.Parse(c[3]) / Double.Parse(c[1])) / footerList.Count) + "%</td>");
                            sw.Write("<td>" + (int)(100 * footerList.Sum(c => Double.Parse(c[4]) / Double.Parse(c[1])) / footerList.Count) + "%</td>");
                            sw.Write("<td>" + (int)(100 * footerList.Sum(c => Double.Parse(c[5]) / Double.Parse(c[1])) / footerList.Count) + "%</td>");
                            sw.Write("<td>" + (int)(100 * footerList.Sum(c => Double.Parse(c[6]) / Double.Parse(c[1])) / footerList.Count) + "%</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[7])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[8])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[9])) + "</td>");
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[10])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[11])) + "</td>");
                            sw.Write("<td></td>");
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</tfoot>");
                }
            }
            sw.Write("</table>");

        }
        /// <summary>
        /// Creates the defense table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="fight_duration">Duration of the fight</param>
        private void CreateDefTable(StreamWriter sw, int phase_index) {
            //generate Tankstats table
            PhaseData phase = boss.getPhases(boss_data, combat_data.getCombatList(), agent_data)[phase_index];
            sw.Write("<script> $(function () { $('#defstats_table"+ phase_index + "').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"defstats_table"+ phase_index+"\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        sw.Write("<th>Dmg Taken</th>");
                        sw.Write("<th>Dmg Barrier</th>");
                        sw.Write("<th>Blocked</th>");
                        sw.Write("<th>Invulned</th>");
                        sw.Write("<th>Evaded</th>");
                        sw.Write("<th><span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Dodges or Mirage Cloak \">Dodges</span></th>");
                        sw.Write("<th><img src=" + GetLink("Downs") + " alt=\"Downs\" title=\"Times downed\" height=\"18\" width=\"18\"></th>");
                        sw.Write("<th><img src=" + GetLink("Dead") + " alt=\"Dead\" title=\"Time died\" height=\"18\" width=\"18\">" + "</th>");
                    }                
                    sw.Write("</tr>");
                }
                
                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                {
                    foreach (Player player in p_list)
                    {
                        string[] stats = HTMLHelper.getFinalDefenses(boss_data,combat_data,agent_data,mech_data,player,boss, phase_index);
                        TimeSpan timedead = TimeSpan.FromMilliseconds(Double.Parse(stats[9]));//dead
                                                                                              //gather data for footer
                        footerList.Add(new string[] { player.getGroup().ToString(), stats[0], stats[10], stats[1], stats[3], stats[6], stats[5], stats[8] });
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>");
                            sw.Write("<td>" + player.getCharacter().ToString() + "</td>");
                            sw.Write("<td>" + stats[0] + "</td>");//dmg taken
                            sw.Write("<td>" + stats[10] + "</td>");//dmgbarriar
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[4] + "Damage \">" + stats[1] + "</span>" + "</td>");//Blocks  
                            sw.Write("<td>" + stats[3] + "</td>");//invulns
                            sw.Write("<td>" + stats[6] + "</td>");// evades
                            sw.Write("<td>" + stats[5] + "</td>");//dodges
                            sw.Write("<td>" + stats[8] + "</td>");//downs
                            long fight_duration = (phase.end - phase.start)/1000;
                            if (timedead > TimeSpan.Zero)
                            {
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + "(" + (int)((timedead.TotalSeconds / fight_duration) * 100) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</span>" + " </td>");
                            }
                            else
                            {
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died 100% Alive) \"> </span>" + " </td>");
                            }
                        }
                        sw.Write("</tr>");
                    }
                }
                sw.Write("</tbody>");
                if (p_list.Count() > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footerList.Select(x => x[0]).Distinct())
                        {
                            List<string[]> groupList = footerList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[1])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[2])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[3])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[4])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[5])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[6])) + "</td>");
                                sw.Write("<td>" + groupList.Sum(c => int.Parse(c[7])) + "</td>");
                                sw.Write("<td></td>");
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Total</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[1])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[2])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[3])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[4])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[5])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[6])) + "</td>");
                            sw.Write("<td>" + footerList.Sum(c => int.Parse(c[7])) + "</td>");
                            sw.Write("<td></td>");
                        }
                        sw.Write("</tr>");

                    }
                    sw.Write("</tfoot>");
                }
            }
           
            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the support table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="fight_duration">Duration of the fight</param>
        private void CreateSupTable(StreamWriter sw, int phase_index) {
            //generate suppstats table
            sw.Write("<script> $(function () { $('#supstats_table"+ phase_index+"').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"supstats_table"+ phase_index+"\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Sub</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Name</th>");
                        sw.Write("<th>Condi Cleanse</th>");
                        sw.Write("<th>Resurrects</th>");
                    }

                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                List<string[]> footerList = new List<string[]>();
                sw.Write("<tbody>");
                {
                    foreach (Player player in p_list)
                    {
                        string[] stats = HTMLHelper.getFinalSupport(boss_data,combat_data,agent_data,player, boss,phase_index);
                        //gather data for footer
                        footerList.Add(new string[] { player.getGroup().ToString(), stats[3], stats[2], stats[1], stats[0] });
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>");
                            sw.Write("<td>" + player.getCharacter().ToString() + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[3] + " seconds \">" + stats[2] + "</span>" + "</td>");//condicleanse                                                                                                                                                                   //HTML_defstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[6] + " Evades \">" + stats[7] + "dmg</span>" + "</td>";//evades
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[1] + " seconds \">" + stats[0] + "</span>" + "</td>");//res
                        }
                        sw.Write("</tr>");
                    }
                }
                sw.Write("</tbody>");
                if (p_list.Count() > 1)
                { 
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footerList.Select(x => x[0]).Distinct())
                        {
                            List<string[]> groupList = footerList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => Double.Parse(c[1])).ToString() + " seconds \">" + groupList.Sum(c => int.Parse(c[2])).ToString() + " condis</span>" + "</td>");
                                sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + groupList.Sum(c => Double.Parse(c[3])).ToString() + " seconds \">" + groupList.Sum(c => int.Parse(c[4])) + "</span>" + "</td>");
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Total</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => Double.Parse(c[1])).ToString() + " seconds \">" + footerList.Sum(c => int.Parse(c[2])).ToString() + " condis</span>" + "</td>");
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + footerList.Sum(c => Double.Parse(c[3])).ToString() + " seconds \">" + footerList.Sum(c => int.Parse(c[4])).ToString() + "</span>" + "</td>");
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</tfoot>");
                }
            }
            
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the buff uptime table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="list_to_use">Boon list to use</param>
        /// <param name="table_id">id of the table</param>
        private void CreateUptimeTable(StreamWriter sw, List<Boon> list_to_use, string table_id, int phase_index)
        {
            //Generate Boon table------------------------------------------------------------------------------------------------
            sw.Write("<script> $(function () { $('#" + table_id+ phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});</script>");
            List<List<string>> footList = new List<List<string>>();
            sw.Write("<table class=\"display table table-striped table-hover compact\" cellspacing=\"0\" id=\"" + table_id+ phase_index + "\">");
            {
                HTMLHelper.writeBoonTableHeader(sw, list_to_use);
                HashSet<int> intensityBoon = new HashSet<int>();
                bool boonTable = list_to_use.Select(x => x.getID()).Contains(740);
                sw.Write("<tbody>");
                {
                    foreach (Player player in p_list)
                    {
                        Dictionary<int, string> boonArray = HTMLHelper.getfinalboons(boss_data,combat_data,skill_data,player);
                        List<string> boonArrayToList = new List<string>();
                        boonArrayToList.Add(player.getGroup());
                        int count = 0;

                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>");
                            if (boonTable)
                            {
                                long fight_duration = boss_data.getLastAware() - boss_data.getFirstAware();
                                Dictionary<int, long> boonPresence = player.getBoonPresence(boss_data, skill_data, combat_data.getCombatList());
                                double avg_boons = 0.0;
                                foreach(Boon boon in list_to_use)
                                {
                                    if (boonPresence.ContainsKey(boon.getID()))
                                    {
                                        avg_boons += boonPresence[boon.getID()];
                                    }
                                }
                                avg_boons /= fight_duration;
                                sw.Write("<td data-toggle=\"tooltip\" title=\"Average number of boons: " + Math.Round(avg_boons,1) + "\">" + player.getCharacter().ToString() + " </td>");
                            } else
                            {
                                sw.Write("<td>" + player.getCharacter().ToString() + "</td>");
                            }
                            foreach (Boon boon in list_to_use)
                            {
                                if (boon.getType() == Boon.BoonType.Intensity)
                                {
                                    intensityBoon.Add(count);
                                }
                                if (boonArray.ContainsKey(boon.getID()))
                                {
                                    string toWrite = Math.Round(float.Parse(boonArray[boon.getID()].Trim('%')), 1) + (intensityBoon.Contains(count) ? "" : "%");
                                    sw.Write("<td>" + toWrite + "</td>");
                                    boonArrayToList.Add(boonArray[boon.getID()]);
                                }
                                else
                                {
                                    sw.Write("<td>" + 0 + "</td>");
                                    boonArrayToList.Add("0");
                                }
                                count++;
                            }
                        }
                        sw.Write("</tr>");
                        //gather data for footer
                        footList.Add(boonArrayToList);
                    }
                }

                sw.Write("</tbody>");
                if (p_list.Count() > 1)
                {
                    sw.Write("<tfoot>");
                    {
                        foreach (string groupNum in footList.Select(x => x[0]).Distinct())//selecting group
                        {
                            List<List<string>> groupList = footList.Where(x => x[0] == groupNum).ToList();
                            sw.Write("<tr>");
                            {
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>Group " + groupNum + "</td>");
                                for (int i = 1; i < groupList[0].Count; i++)
                                {
                                    if (intensityBoon.Contains(i - 1))
                                    {
                                        sw.Write("<td>" + Math.Round(groupList.Sum(c => Double.Parse(c[i])) / groupList.Count, 1) + "</td>");
                                    }
                                    else
                                    {
                                        sw.Write("<td>" + Math.Round(groupList.Sum(c => Double.Parse(c[i].TrimEnd('%'))) / groupList.Count, 1) + "%</td>");
                                    }

                                }
                            }
                            sw.Write("</tr>");
                        }
                        sw.Write("<tr>");
                        {
                            sw.Write("<td></td>");
                            sw.Write("<td></td>");
                            sw.Write("<td>Averages</td>");
                            for (int i = 1; i < footList[0].Count; i++)
                            {
                                if (intensityBoon.Contains(i - 1))
                                {
                                    sw.Write("<td>" + Math.Round(footList.Sum(c => Double.Parse(c[i])) / footList.Count, 1) + "</td>");
                                }
                                else
                                {
                                    sw.Write("<td>" + Math.Round(footList.Sum(c => Double.Parse(c[i].TrimEnd('%'))) / footList.Count, 1) + "%</td>");
                                }
                            }
                        }
                        sw.Write("</tr>");
                    }
                    sw.Write("</tfoot>");
                }
            }     
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the self buff generation table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="list_to_use">Boon list to use</param>
        /// <param name="table_id">id of the table</param>
        private void CreateGenSelfTable(StreamWriter sw, List<Boon> list_to_use, string table_id, int phase_index)
        { //Generate BoonGenSelf table
            sw.Write("<script> $(function () { $('#" + table_id+ phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\" cellspacing=\"0\" width=\"100%\" id=\"" + table_id+ phase_index + "\">");
            {
                HTMLHelper.writeBoonTableHeader(sw, list_to_use);
                sw.Write("<tbody>");
                {
                    foreach (Player player in p_list)
                    {
                        List<Player> playerID = new List<Player>();
                        playerID.Add(player);
                        Dictionary<int, string> boonArray = HTMLHelper.getfinalboons(boss_data,combat_data,skill_data,player, playerID);
                        HTMLHelper.writeBoonGenTableBody(sw, player, list_to_use, boonArray, GetLink(player.getProf().ToString()));
                    }
                }
                sw.Write("</tbody>");
            }
           
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the group buff generation table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="list_to_use">Boon list to use</param>
        /// <param name="table_id">id of the table</param>
        private void CreateGenGroupTable(StreamWriter sw, List<Boon> list_to_use, string table_id, int phase_index)
        { //Generate BoonGenGroup table
            sw.Write("<script> $(function () { $('#" + table_id+ phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"" + table_id+ phase_index + "\">");
            {
                HTMLHelper.writeBoonTableHeader(sw, list_to_use);
                sw.Write("<tbody>");
                {
                    List<Player> playerIDS = new List<Player>();
                    foreach (Player player in p_list)
                    {
                        foreach (Player p in p_list)
                        {
                            if (p.getGroup() == player.getGroup())
                                playerIDS.Add(p);
                        }
                        Dictionary<int, string> boonArray = HTMLHelper.getfinalboons(boss_data,combat_data,skill_data,player, playerIDS);
                        HTMLHelper.writeBoonGenTableBody(sw, player, list_to_use, boonArray, GetLink(player.getProf().ToString()));
                    }
                }          
                sw.Write("</tbody>");
            }            
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the off squade buff generation table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="list_to_use">Boon list to use</param>
        /// <param name="table_id">id of the table</param>
        private void CreateGenOGroupTable(StreamWriter sw, List<Boon> list_to_use, string table_id, int phase_index)
        {  //Generate BoonGenOGroup table
            sw.Write("<script> $(function () { $('#" + table_id + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"" + table_id + phase_index + "\">");
            {
                HTMLHelper.writeBoonTableHeader(sw, list_to_use);
                sw.Write("<tbody>");
                {
                    List<Player> playerIDS = new List<Player>();
                    foreach (Player player in p_list)
                    {
                        foreach (Player p in p_list)
                        {
                            if (p.getGroup() != player.getGroup())
                                playerIDS.Add(p);
                        }
                        Dictionary<int, string> boonArray = HTMLHelper.getfinalboons(boss_data,combat_data,skill_data,player, playerIDS);
                        HTMLHelper.writeBoonGenTableBody(sw, player, list_to_use, boonArray, GetLink(player.getProf().ToString()));
                    }
                }
                sw.Write("</tbody>");
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the squad buff generation table
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="list_to_use">Boon list to use</param>
        /// <param name="table_id">id of the table</param>
        private void CreateGenSquadTable(StreamWriter sw, List<Boon> list_to_use, string table_id, int phase_index) {
            //Generate BoonGenSquad table
            sw.Write("<script> $(function () { $('#" + table_id + phase_index + "').DataTable({ \"order\": [[0, \"asc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"" + table_id + phase_index + "\">");
            {
                HTMLHelper.writeBoonTableHeader(sw, list_to_use);
                sw.Write("<tbody>");
                {
                    List<Player> playerIDS = new List<Player>();
                    foreach (Player p in p_list)
                    {
                        playerIDS.Add(p);
                    }
                    foreach (Player player in p_list)
                    {
                        Dictionary<int, string> boonArray = HTMLHelper.getfinalboons(boss_data,combat_data,skill_data,player, playerIDS);
                        HTMLHelper.writeBoonGenTableBody(sw, player, list_to_use, boonArray, GetLink(player.getProf().ToString()));
                    }
                }
                sw.Write("</tbody>");
            }         
            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the player tab
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="settingsSnap">Settings to use</param>
        private void CreatePlayerTab(StreamWriter sw, bool[] settingsSnap, int phase_index)
        {
            PhaseData phase = boss.getPhases(boss_data, combat_data.getCombatList(), agent_data)[phase_index];
            long start = phase.start + boss_data.getFirstAware();
            long end = phase.end + boss_data.getFirstAware();
            List<SkillItem> s_list = skill_data.getSkillList();
            //generate Player list Graphs
            foreach (Player p in p_list)
            {
                List<CastLog> casting = p.getCastLogs(boss_data, combat_data.getCombatList(), agent_data, phase.start, phase.end);

                bool died = p.getDeath(boss_data, combat_data.getCombatList(), phase.start, phase.end) > 0;
                string charname = p.getCharacter();
                string pid = p.getInstid() + "_" + phase_index;
                sw.Write("<div class=\"tab-pane fade\" id=\"" + pid + "\">");
                {
                    sw.Write("<h1 align=\"center\"> " + charname + "<img src=\"" + GetLink(p.getProf().ToString()) + " \" alt=\"" + p.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</h1>");
                    sw.Write("<ul class=\"nav nav-tabs\">");
                    {
                        sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#home" + pid + "\">" + p.getCharacter() + "</a></li>");
                        if (SnapSettings[10])
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#SimpleRot" + pid + "\">Simple Rotation</a></li>");

                        }
                        if (died)
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#DeathRecap" + pid + "\">Death Recap</a></li>");

                        }
                        //foreach pet loop here                        
                        foreach (AgentItem agent in p.getMinionsDamageLogs(0, boss_data, combat_data.getCombatList(), agent_data).Keys)
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#minion" + pid + "_" + agent.getInstid() + "\">" + agent.getName() + "</a></li>");
                        }
                        //inc dmg
                        sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#incDmg" + pid + "\">Damage Taken</a></li>");
                    }
                    sw.Write("</ul>");
                    sw.Write("<div id=\"myTabContent\" class=\"tab-content\">");
                    {
                        sw.Write("<div class=\"tab-pane fade show active\" id=\"home" + pid + "\">");
                        {
                            List<int[]> consume = p.getConsumablesList(boss_data, skill_data, combat_data.getCombatList(), phase.start, phase.end);
                            List<int[]> initial = consume.Where(x => x[1] == 0).ToList();
                            List<int[]> refreshed = consume.Where(x => x[1] > 0).ToList();
                            if (initial.Count() > 0)
                            {
                                Boon food = null;
                                Boon utility = null;
                                foreach (int[] buff in initial)
                                {

                                    Boon foodCheck = Boon.getFoodList().FirstOrDefault(x => x.getID() == buff[0]);
                                    if (foodCheck != null)
                                    {
                                        food = foodCheck;
                                        continue;
                                    }
                                    Boon utilCheck = Boon.getUtilityList().FirstOrDefault(x => x.getID() == buff[0]);
                                    if (utilCheck != null)
                                    {
                                        utility = utilCheck;
                                        continue;
                                    }
                                }
                                sw.Write("<p>Started with " );
                                if (food != null)
                                {
                                    sw.Write(food.getName() + "<img src=\"" +food.getLink() + " \" alt=\"" + food.getName() + "\" height=\"18\" width=\"18\" >");
                                }
                                if (utility != null)
                                {
                                    sw.Write(utility.getName() + "<img src=\"" + utility.getLink() + " \" alt=\"" + utility.getName() + "\" height=\"18\" width=\"18\" >");
                                }
                                sw.Write("</p>");
                            }
                            if (refreshed.Count() > 0)
                            {
                                Boon food = null;
                                Boon utility = null;
                                foreach (int[] buff in refreshed)
                                {

                                    Boon foodCheck = Boon.getFoodList().FirstOrDefault(x => x.getID() == buff[0]);
                                    if (foodCheck != null)
                                    {
                                        food = foodCheck;
                                        continue;
                                    }
                                    Boon utilCheck = Boon.getUtilityList().FirstOrDefault(x => x.getID() == buff[0]);
                                    if (utilCheck != null)
                                    {
                                        utility = utilCheck;
                                        continue;
                                    }
                                }
                                sw.Write("<p>Refreshed ");
                                if (food != null)
                                {
                                    sw.Write(food.getName() + "<img src=\"" + food.getLink() + " \" alt=\"" + food.getName() + "\" height=\"18\" width=\"18\" >");
                                }
                                if (utility != null)
                                {
                                    sw.Write(utility.getName() + "<img src=\"" + utility.getLink() + " \" alt=\"" + utility.getName() + "\" height=\"18\" width=\"18\" >");
                                }
                                sw.Write("</p>");
                            }
                            sw.Write("<div id=\"Graph" + pid + "\" style=\"height: 800px;width:1000px; display:inline-block \"></div>");
                            sw.Write("<script>");
                            {
                                sw.Write("var data = [");
                                {
                                    if (SnapSettings[6])//Display rotation
                                    {
                                        foreach (CastLog cl in casting)
                                        {
                                            HTMLHelper.writeCastingItem(sw, cl, skill_data, phase.start, phase.end);
                                        }
                                    }
                                    if (present_boons.Count() > 0)
                                    {
                                        List<Boon> parseBoonsList = new List<Boon>();                                       
                                        parseBoonsList.AddRange(present_boons);
                                        parseBoonsList.AddRange(present_offbuffs);
                                        parseBoonsList.AddRange(present_defbuffs);
                                        if (present_personnal.ContainsKey(p.getInstid()))
                                        {
                                            parseBoonsList.AddRange(present_personnal[p.getInstid()]);
                                        }
                                        Dictionary<int, BoonsGraphModel> boonGraphData = p.getBoonGraphs(boss_data, skill_data, combat_data.getCombatList());
                                        foreach (int boonid in boonGraphData.Keys.Reverse())
                                        {
                                            if (parseBoonsList.FirstOrDefault(x => x.getID() == boonid) != null || boonid == -2)
                                            {
                                                sw.Write("{");
                                                {
                                                    BoonsGraphModel bgm = boonGraphData[boonid];
                                                    List<Point> bChart = bgm.getBoonChart().Where(x => x.X >= phase.start / 1000 && x.X <= phase.end / 1000).ToList();
                                                    int bChartCount = 0;
                                                    sw.Write("y: [");
                                                    {
                                                        foreach (Point pnt in bChart)
                                                        {
                                                            if (bChartCount == bChart.Count - 1)
                                                            {
                                                                sw.Write("'" + pnt.Y + "'");
                                                            }
                                                            else
                                                            {
                                                                sw.Write("'" + pnt.Y + "',");
                                                            }
                                                            bChartCount++;
                                                        }
                                                        if (bgm.getBoonChart().Count == 0)
                                                        {
                                                            sw.Write("'0'");
                                                        }
                                                    }
                                                    sw.Write("],");
                                                    sw.Write("x: [");
                                                    {
                                                        bChartCount = 0;
                                                        foreach (Point pnt in bChart)
                                                        {
                                                            if (bChartCount == bChart.Count - 1)
                                                            {
                                                                sw.Write("'" + (pnt.X - (int)phase.start / 1000) + "'");
                                                            }
                                                            else
                                                            {
                                                                sw.Write("'" + (pnt.X - (int)phase.start / 1000) + "',");
                                                            }
                                                            bChartCount++;
                                                        }
                                                        if (bgm.getBoonChart().Count == 0)
                                                        {
                                                            sw.Write("'0'");
                                                        }
                                                    }
                                                    sw.Write("],");
                                                    sw.Write("yaxis: 'y2'," +
                                                             "type: 'scatter',");
                                                    //  "legendgroup: '"+Boon.getEnum(bgm.getBoonName()).getPloltyGroup()+"',";
                                                    if (bgm.getBoonName() == "Might" || bgm.getBoonName() == "Quickness")
                                                    {
                                                    }
                                                    else
                                                    {
                                                        sw.Write(" visible: 'legendonly',");
                                                    }
                                                    sw.Write("line: {shape: 'hv', color:'" + GetLink("Color-" + bgm.getBoonName()) + "'},");
                                                    sw.Write("fill: 'tozeroy'," +
                                                            "name: \"" + bgm.getBoonName() + "\"");
                                                }
                                                sw.Write(" },");
                                            }
                                        }
                                    }
                                    int maxDPS = 0;
                                    if (SnapSettings[2])
                                    {//show boss dps plot
                                     //Adding dps axis
                                        List<int[]> playerbossdpsgraphdata = HTMLHelper.getBossDPSGraph(boss_data,combat_data,agent_data,p, boss,phase_index);
                                        int pbdgCount = 0;
                                        sw.Write("{");
                                        {
                                            sw.Write("y: [");
                                            {
                                                foreach (int[] dp in playerbossdpsgraphdata)
                                                {
                                                    if (maxDPS < dp[1])
                                                    {
                                                        maxDPS = dp[1];
                                                    }
                                                    if (pbdgCount == playerbossdpsgraphdata.Count - 1)
                                                    {
                                                        sw.Write("'" + dp[1] + "'");
                                                    }
                                                    else
                                                    {
                                                        sw.Write("'" + dp[1] + "',");
                                                    }
                                                    pbdgCount++;
                                                }
                                                if (playerbossdpsgraphdata.Count == 0)
                                                {
                                                    sw.Write("'0'");
                                                }
                                            }
                                            sw.Write("],");
                                            //add time axis
                                            sw.Write("x: [");
                                            {
                                                pbdgCount = 0;
                                                foreach (int[] dp in playerbossdpsgraphdata)
                                                {
                                                    if (pbdgCount == playerbossdpsgraphdata.Count - 1)
                                                    {
                                                        sw.Write("'" + dp[0] + "'");
                                                    }
                                                    else
                                                    {
                                                        sw.Write("'" + dp[0] + "',");
                                                    }

                                                    pbdgCount++;
                                                }
                                                if (playerbossdpsgraphdata.Count == 0)
                                                {
                                                    sw.Write("'0'");
                                                }
                                            }
                                            sw.Write("],");
                                            sw.Write("mode: 'lines'," +
                                                    "line: {shape: 'spline',color:'" + GetLink("Color-" + p.getProf()) + "'}," +
                                                    "yaxis: 'y3'," +
                                                    // "legendgroup: 'Damage',"+
                                                    "name: 'Boss DPS'");
                                        }

                                        sw.Write("},");
                                    }
                                    if (SnapSettings[1])
                                    {//show total dps plot
                                        sw.Write("{");
                                        { //Adding dps axis
                                            List<int[]> playertotaldpsgraphdata = HTMLHelper.getTotalDPSGraph(boss_data,combat_data,agent_data,p,boss, phase_index);
                                            int ptdgCount = 0;
                                            sw.Write("y: [");
                                            {
                                                foreach (int[] dp in playertotaldpsgraphdata)
                                                {
                                                    if (ptdgCount == playertotaldpsgraphdata.Count - 1)
                                                    {
                                                        sw.Write("'" + dp[1] + "'");
                                                    }
                                                    else
                                                    {
                                                        sw.Write("'" + dp[1] + "',");
                                                    }
                                                    ptdgCount++;
                                                }
                                                if (playertotaldpsgraphdata.Count == 0)
                                                {
                                                    sw.Write("'0'");
                                                }
                                            }
                                            sw.Write("],");
                                            //add time axis
                                            sw.Write("x: [");
                                            {
                                                ptdgCount = 0;
                                                foreach (int[] dp in playertotaldpsgraphdata)
                                                {
                                                    if (ptdgCount == playertotaldpsgraphdata.Count - 1)
                                                    {
                                                        sw.Write("'" + dp[0] + "'");
                                                    }
                                                    else
                                                    {
                                                        sw.Write("'" + dp[0] + "',");
                                                    }
                                                    ptdgCount++;
                                                }
                                                if (playertotaldpsgraphdata.Count == 0)
                                                {
                                                    sw.Write("'0'");
                                                }
                                            }
                                            sw.Write("],");
                                            sw.Write(" mode: 'lines'," +
                                                   "line: {shape: 'spline',color:'rgb(0,250,0)'}," +
                                                   "yaxis: 'y3'," +
                                                   // "legendgroup: 'Damage'," +
                                                   "name: 'Total DPS'");

                                        }
                                        sw.Write("}");
                                    }
                                }
                                sw.Write("];");
                                sw.Write("var layout = {");
                                {
                                    sw.Write("barmode:'stack',");
                                   sw.Write("yaxis: {" +
                                                "title: 'Rotation', domain: [0, 0.09], fixedrange: true, showgrid: false," +
                                                "range: [0, 2]" +
                                            "}," +
                                            "legend: { traceorder: 'reversed' }," +
                                            "hovermode: 'compare'," +
                                            "yaxis2: { title: 'Boons', domain: [0.11, 0.50], fixedrange: true }," +
                                            "yaxis3: { title: 'DPS', domain: [0.51, 1] },"
                                    );
                                    sw.Write("images: [");
                                    {
                                        if (SnapSettings[7])//Display rotation
                                        {
                                            int castCount = 0;
                                            foreach (CastLog cl in casting)
                                            {
                                                HTMLHelper.writeCastingItemIcon(sw, cl, skill_data, phase.start, castCount == casting.Count - 1);
                                                castCount++;
                                            }
                                        }
                                    }
                                    sw.Write("],");
                                    sw.Write("font: { color: '#ffffff' }," +
                                            "paper_bgcolor: 'rgba(0,0,0,0)'," +
                                            "plot_bgcolor: 'rgba(0,0,0,0)'");
                                }
                                sw.Write("};");
                                sw.Write("Plotly.newPlot('Graph" + pid + "', data, layout);");
                            }
                            sw.Write("</script> ");
                            sw.Write("<ul class=\"nav nav-tabs\">");
                            {
                                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#distTabAll" + pid + "\">" + "All" + "</a></li>");
                                sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#distTabBoss" + pid + "\">" + "Boss" + "</a></li>");
                            }
                            sw.Write("</ul>");
                            sw.Write("<div class=\"tab-content\">");
                            {
                                sw.Write("<div class=\"tab-pane fade show active\" id=\"distTabAll" + pid + "\">");
                                {
                                    CreateDMGDistTable(sw, p, false, phase_index);
                                }
                                sw.Write("</div>");
                                sw.Write("<div class=\"tab-pane fade\" id=\"distTabBoss" + pid + "\">");
                                {
                                    CreateDMGDistTable(sw, p, true, phase_index);
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("</div>");
                        foreach (AgentItem agent in p.getMinionsDamageLogs(0, boss_data, combat_data.getCombatList(), agent_data).Keys)
                        {
                            string id = pid + "_" + agent.getInstid();
                            sw.Write("<div class=\"tab-pane fade \" id=\"minion" + id  + "\">");
                            {
                                sw.Write("<ul class=\"nav nav-tabs\">");
                                {
                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#distTabAll" + id + "\">" + "All" + "</a></li>");
                                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#distTabBoss" + id + "\">" + "Boss" + "</a></li>");
                                }
                                sw.Write("</ul>");
                                sw.Write("<div class=\"tab-content\">");
                                {
                                    sw.Write("<div class=\"tab-pane fade show active\" id=\"distTabAll" + id + "\">");
                                    {
                                        CreateDMGDistTable(sw, p, agent, false, phase_index);
                                    }
                                    sw.Write("</div>");
                                    sw.Write("<div class=\"tab-pane fade\" id=\"distTabBoss" + id + "\">");
                                    {
                                        CreateDMGDistTable(sw, p, agent, true, phase_index);
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                        }
                        if (SnapSettings[10])
                        {
                            sw.Write("<div class=\"tab-pane fade \" id=\"SimpleRot" + pid + "\">");
                            {
                                int simpleRotSize = 20;
                                if (settingsSnap[12])
                                {
                                    simpleRotSize = 30;
                                }
                                CreateSimpleRotationTab(sw, p,simpleRotSize, phase_index);
                            }
                            sw.Write("</div>");
                        }
                        if (died && phase_index == 0)
                        {
                            sw.Write("<div class=\"tab-pane fade \" id=\"DeathRecap" + pid + "\">");
                            {
                                CreateDeathRecap(sw, p);
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("<div class=\"tab-pane fade \" id=\"incDmg" + pid + "\">");
                        {
                            CreateDMGTakenDistTable(sw, p, phase_index);
                        }
                        sw.Write("</div>");
                    }
                    sw.Write("</div>");
                }
                sw.Write("</div>");
            }

        }
        /// <summary>
        /// Creates the rotation tab for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        /// <param name="simpleRotSize">Size of the images</param>
        private void CreateSimpleRotationTab(StreamWriter sw,Player p,int simpleRotSize, int phase_index) {
            if (SnapSettings[6])//Display rotation
            {
                PhaseData phase = boss.getPhases(boss_data, combat_data.getCombatList(), agent_data)[phase_index];
                List<CastLog> casting = p.getCastLogs(boss_data, combat_data.getCombatList(), agent_data, phase.start, phase.end);
                GW2APISkill autoSkill = null;
                int autosCount = 0;
                foreach (CastLog cl in casting)
                {
                    GW2APISkill apiskill = null;
                    SkillItem skill = skill_data.getSkillList().FirstOrDefault(x => x.getID() == cl.getID());
                    if (skill != null)
                    {
                        apiskill = skill.GetGW2APISkill();
                    }


                    if (apiskill != null)
                    {
                        if (apiskill.slot != "Weapon_1")
                        {
                            if (autosCount > 0 && SnapSettings[11])
                            {
                                sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + autoSkill.icon + "\" data-toggle=\"tooltip\" title= \"" + autoSkill.name + "[Auto Attack] x"+autosCount+ " \" height=\""+simpleRotSize+ "\" width=\"" + simpleRotSize + "\"></div></span>");
                                autosCount = 0;
                            }
                            sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + apiskill.icon + "\" data-toggle=\"tooltip\" title= \"" + apiskill.name + " Time: " + cl.getTime() + "ms " + "Dur: " + cl.getActDur() + "ms \" height=\"" + simpleRotSize + "\" width=\"" + simpleRotSize + "\"></div></span>");
                        }
                        else
                        {
                            if (autosCount == 0)
                            {
                                autoSkill = apiskill;
                            }
                            autosCount++;
                        }
                    }
                    else
                    {
                        string skillName = "";
                        string skillLink = "";

                        if (cl.getID() == -2)
                        {//wepswap
                            skillName = "Weapon Swap";
                            skillLink = GetLink("Swap");
                            sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + skillLink + "\" data-toggle=\"tooltip\" title= \"" + skillName + " Time: " + cl.getTime() + "ms " + "Dur: " + cl.getActDur() + "ms \" height=\"" + simpleRotSize + "\" width=\"" + simpleRotSize + "\"></div></span>");
                            sw.Write("<br>");
                            continue;
                        }
                        else if (cl.getID() == 1066)
                        {
                            skillName = "Resurrect";
                            skillLink = GetLink("Downs");
                            sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + skillLink + "\" data-toggle=\"tooltip\" title= \"" + skillName + " Time: " + cl.getTime() + "ms " + "Dur: " + cl.getActDur() + "ms \" height=\"" + simpleRotSize + "\" width=\"" + simpleRotSize + "\"></div></span>");

                        }
                        else
                        if (cl.getID() == 1175)
                        {
                            skillName = "Bandage";
                            skillLink = GetLink("Bandage");
                            sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + skillLink + "\" data-toggle=\"tooltip\" title= \"" + skillName + " Time: " + cl.getTime() + "ms " + "Dur: " + cl.getActDur() + "ms \" height=\"" + simpleRotSize + "\" width=\"" + simpleRotSize + "\"></div></span>");

                        }
                        else
                        if (cl.getID() == 65001)
                        {
                            skillName = "Dodge";
                            skillLink = GetLink("Dodge");
                            sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + skillLink + "\" data-toggle=\"tooltip\" title= \"" + skillName + " Time: " + cl.getTime() + "ms " + "Dur: " + cl.getActDur() + "ms \" height=\"" + simpleRotSize + "\" width=\"" + simpleRotSize + "\"></div></span>");

                        }
                        else if(skill != null){
                            
                            sw.Write("<span class=\"rot-skill\"><div class=\"rot-crop\"><img src=\"" + GetLink("Blank") + "\" data-toggle=\"tooltip\" title= \"" + skill.getName() + " Time: " + cl.getTime() + "ms " + "Dur: " + cl.getActDur() + "ms \" height=\"" + simpleRotSize + "\" width=\"" + simpleRotSize + "\"></div></span>");

                        }

                    }

                }
            }

        }
        /// <summary>
        /// Creates the death recap tab for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private void CreateDeathRecap(StreamWriter sw, Player p)
        {
            List<DamageLog> damageLogs = p.getDamageTakenLogs(boss_data, combat_data.getCombatList(), agent_data, mech_data, 0, boss_data.getAwareDuration());
            List<SkillItem> s_list = skill_data.getSkillList();
            long start = boss_data.getFirstAware();
            long end = boss_data.getLastAware();
            List<CombatItem> down = combat_data.getStates(p.getInstid(), "CHANGE_DOWN", start, end);
            if (down.Count > 0)
            {
                List<CombatItem> ups = combat_data.getStates(p.getInstid(), "CHANGE_UP", start, end);
                down = down.GetRange(ups.Count(), down.Count()-ups.Count());
            }
            List<CombatItem> dead = combat_data.getStates(p.getInstid(), "CHANGE_DEAD", start, end);
            List<DamageLog> damageToDown = new List<DamageLog>();
            List<DamageLog> damageToKill = new List<DamageLog>();
            if (down.Count > 0)
            {//went to down state before death
                damageToDown = damageLogs.Where(x => x.getTime() < down.Last().getTime() - start && x.getDamage() > 0).ToList();
                damageToKill = damageLogs.Where(x => x.getTime() > down.Last().getTime() - start && x.getTime() < dead.Last().getTime() - start && x.getDamage() > 0).ToList();
                //Filter last 30k dmg taken
                int totaldmg = 0;
                for (int i = damageToDown.Count() - 1; i > 0; i--)
                {
                    totaldmg += damageToDown[i].getDamage();
                    if (totaldmg > 30000)
                    {
                        damageToDown = damageToDown.GetRange(i, damageToDown.Count() -i);
                        break;
                    }
                }
                sw.Write("<center>");
                sw.Write("<p>Took " + damageToDown.Sum(x => x.getDamage()) + " damage in " +
                ((damageToDown.Last().getTime() - damageToDown.First().getTime()) / 1000f).ToString() + " seconds to enter downstate");
                if (damageToKill.Count() > 0)
                {
                    sw.Write("<p>Took " + damageToKill.Sum(x => x.getDamage()) + " damage in " +
                       ((damageToKill.Last().getTime() - damageToKill.First().getTime()) / 1000f).ToString() + " seconds to die</p>");
                } else
                {
                    sw.Write("<p>Instant death after a down</p>");
                }
                sw.Write("</center>");
            }
            else
            {
                damageToKill = damageLogs.Where(x =>  x.getTime() < dead.Last().getTime() && x.getDamage() > 0).ToList();
                //Filter last 30k dmg taken
                int totaldmg = 0;
                for (int i = damageToKill.Count() - 1; i > 0; i--)
                {
                    totaldmg += damageToKill[i].getDamage();
                    if (totaldmg > 30000)
                    {
                        damageToKill = damageToKill.GetRange(i, damageToKill.Count() - 1-i);
                        break;
                    }
                }
                sw.Write("<center><h3>Player was insta killed by a mechanic, fall damage or by /gg</h3></center>");
            }
            string pid = p.getInstid().ToString();
            sw.Write("<center><div id=\"BarDeathRecap" + pid + "\"></div> </center>");
            sw.Write("<script>");
            {
                sw.Write("var data = [{");
                //Time on X
                sw.Write("x : [");
                if (damageToDown.Count() != 0)
                {
                    for (int d = 0;d<damageToDown.Count();d++)
                    {
                        sw.Write("'"+damageToDown[d].getTime()/1000f+"s',");
                    }
                }
                for (int d = 0; d < damageToKill.Count(); d++)
                {
                    sw.Write("'" + damageToKill[d].getTime()/1000f + "s'");

                    if (d != damageToKill.Count() - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("],");
                //damage on Y
                sw.Write("y : [");
                if (damageToDown.Count() != 0)
                {
                    for (int d = 0; d < damageToDown.Count(); d++)
                    {
                        sw.Write("'" + damageToDown[d].getDamage() + "',");
                    }
                }
                for (int d = 0; d < damageToKill.Count(); d++)
                {
                    sw.Write("'" + damageToKill[d].getDamage() + "'");

                    if (d != damageToKill.Count() - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("],");
                //Color 
                sw.Write("marker : {color:[");
                if (damageToDown.Count() != 0)
                {
                    for (int d = 0; d < damageToDown.Count(); d++)
                    {
                        sw.Write("'rgb(0,255,0,1)',");
                    }
                }
                for (int d = 0; d < damageToKill.Count(); d++)
                {
                   
                    if (down.Count() == 0)
                    {
                        //damagetoKill was instant(not in downstate)
                        sw.Write("'rgb(0,255,0,1)'");
                    }
                    else
                    {
                        //damageto killwas from downstate
                        sw.Write("'rgb(255,0,0,1)'");
                    }
                   

                    if (d != damageToKill.Count() - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("]},");
                //text
                sw.Write("text : [");
                if (damageToDown.Count() != 0)
                {
                    for (int d = 0; d < damageToDown.Count(); d++)
                    {
                        sw.Write("'" + agent_data.GetAgentWInst(damageToDown[d].getInstidt()).getName().Replace("\0", "").Replace("\'", "\\'") + "<br>"+
                            skill_data.getName( damageToDown[d].getID()).Replace("\'", "\\'") + " hit you for "+damageToDown[d].getDamage() + "',");
                    }
                }
                for (int d = 0; d < damageToKill.Count(); d++)
                {
                    sw.Write("'" + agent_data.GetAgentWInst(damageToKill[d].getInstidt()).getName().Replace("\0","").Replace("\'", "\\'") + "<br>" +
                           "hit you with <b>"+ skill_data.getName(damageToKill[d].getID()).Replace("\'", "\\'") + "</b> for " + damageToKill[d].getDamage() + "'");

                    if (d != damageToKill.Count() - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write("],");
                sw.Write("type:'bar',");
                
                sw.Write("}];");

                sw.Write("var layout = { title: 'Last 30k Damage Taken before death', font: { color: '#ffffff' },width: 1100," +
                    "paper_bgcolor: 'rgba(0,0,0,0)', plot_bgcolor: 'rgba(0,0,0,0)',showlegend: false,bargap :0.05,yaxis:{title:'Damage'},xaxis:{title:'Time(seconds)',type:'catagory'}};");
                sw.Write("Plotly.newPlot('BarDeathRecap" + pid + "', data, layout);");
                
            }
            sw.Write("</script>");
        }
        /// <summary>
        /// Creates the damage distribution table for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private void CreateDMGDistTable(StreamWriter sw, AbstractPlayer p, bool toBoss, int phase_index)
        {
            PhaseData phase = boss.getPhases(boss_data, combat_data.getCombatList(), agent_data)[phase_index];
            List<CastLog> casting = p.getCastLogs(boss_data, combat_data.getCombatList(), agent_data, phase.start, phase.end);
            List<DamageLog> damageLogs = p.getJustPlayerDamageLogs(toBoss ? boss_data.getInstid() : 0, boss_data, combat_data.getCombatList(), agent_data, phase.start, phase.end);
            string finalDPSdata = HTMLHelper.getFinalDPS(boss_data, combat_data, agent_data, p, boss, phase_index);
            int totalDamage = toBoss ? Int32.Parse(finalDPSdata.Split('|')[7]) : Int32.Parse(finalDPSdata.Split('|')[1]);
            int finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.getDamage()) : 0;
            if (totalDamage > 0)
            {
                string contribution = String.Format("{0:0.00}", 100.0 * finalTotalDamage / totalDamage);
                sw.Write("<div>" + p.getCharacter() + " did " + contribution + "% of its own total " + (toBoss ? "boss " : "") + "dps</div>");
            }
            string tabid = p.getInstid() +"_"+ phase_index + (toBoss ? "_boss" : "");
            sw.Write("<script> $(function () { $('#dist_table_" + tabid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dist_table_" + tabid + "\">");
            {
                List<SkillItem> s_list = skill_data.getSkillList();
                HTMLHelper.writeDamageDistTableHeader(sw);
                sw.Write("<tbody>");
                {
                    HashSet<int> usedIDs = new HashSet<int>();
                    foreach (int id in casting.Select(x => x.getID()).Distinct())
                    {//foreach casted skill
                        usedIDs.Add(id);
                        SkillItem skill = s_list.FirstOrDefault(x => x.getID() == id);
                        List<DamageLog> list_to_use = damageLogs.Where(x => x.getID() == id).ToList();
                        if (skill != null && list_to_use.Count > 0)
                        {
                            List<CastLog> clList = casting.Where(x => x.getID() == id).ToList();
                            int casts = clList.Count();
                            double timeswasted = 0;
                            int countwasted = 0;
                            double timessaved = 0;
                            int countsaved = 0;
                            foreach (CastLog cl in clList)
                            {
                                if (cl.getExpDur() < cl.getActDur())
                                {
                                    countsaved++;
                                    timessaved += ((double)(cl.getExpDur() - cl.getActDur()) / 1000f);
                                }
                                else if (cl.getExpDur() > cl.getActDur())
                                {
                                    countwasted++;
                                    timeswasted += ((double)(cl.getActDur()) / 1000f);
                                }
                            }
                            HTMLHelper.writeDamageDistTableSkill(sw, skill, list_to_use, finalTotalDamage, casts,timeswasted,timessaved);
                        }
                    }
                    HTMLHelper.writeDamageDistTableCondi(sw,usedIDs, damageLogs, finalTotalDamage);
                    foreach (int id in damageLogs.Where(x => !usedIDs.Contains(x.getID())).Select(x => x.getID()).Distinct().ToList())//Foreach instant cast skill
                    {
                        SkillItem skill = s_list.FirstOrDefault(x => x.getID() == id);
                        List<CastLog> clList = casting.Where(x => x.getID() == id).ToList();
                        int casts = clList.Count();
                        double timeswasted = 0;
                        int countwasted = 0;
                        double timessaved = 0;
                        int countsaved = 0;
                        foreach (CastLog cl in clList)
                        {
                            if (cl.getExpDur() < cl.getActDur())
                            {
                                countsaved++;
                                timessaved += ((double)(cl.getExpDur() - cl.getActDur()) / 1000f);
                            }
                            else if (cl.getExpDur() > cl.getActDur())
                            {
                                countwasted++;
                                timeswasted += ((double)(cl.getActDur()) / 1000f);
                            }
                        }

                        int totaldamage = 0;
                        int mindamage = 0;
                        int avgdamage = 0;
                        int hits = 0;
                        int maxdamage = 0;
                        int crit = 0;
                        int flank = 0;
                        int glance = 0;
                        double hpcast = 0;
                        foreach (DamageLog dl in damageLogs.Where(x => x.getID() == id))
                        {
                            int curdmg = dl.getDamage();
                            totaldamage += curdmg;
                            if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                            if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                            hits++;
                            int result = dl.getResult().getID();
                            if (result == 1) { crit++; } else if (result == 2) { glance++; }
                            if (dl.isFlanking() == 1) { flank++; }
                        }
                        avgdamage = (int)(totaldamage / (double)hits);
                        if (casts > 0) { hpcast = Math.Round(hits / (double)casts, 2); }
                        if (skill != null)
                        {
                            if (totaldamage != 0 && skill.GetGW2APISkill() != null)
                            {
                                sw.Write("<tr>");
                                {
                                    sw.Write("<td align=\"left\"><img src=" + skill.GetGW2APISkill().icon + " alt=\"" + skill.getName() + "\" title=\"" + skill.getID() + "\" height=\"18\" width=\"18\">" + skill.getName() + "</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                                    sw.Write("<td>" + totaldamage + "</td>");
                                    sw.Write("<td>" + mindamage + "</td>");
                                    sw.Write("<td>" + avgdamage + "</td>");
                                    sw.Write("<td>" + maxdamage + "</td>");
                                    sw.Write("<td>" + casts + "</td>");
                                    sw.Write("<td>" + hits + "</td>");
                                    sw.Write("<td>" + hpcast + "</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)crit / hits) + "%</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)flank / hits) + "%</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)glance / hits) + "%</td>");
                                    sw.Write("<td>" + Math.Round(timeswasted, 2) + "s</td>");
                                    sw.Write("<td>" + Math.Round(timessaved, 2) + "s</td>");
                                }
                                sw.Write("</tr>");
                            }
                            else if (totaldamage != 0)
                            {
                                sw.Write("<tr>");
                                {
                                    sw.Write("<td align=\"left\">" + skill.getName() + "</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                                    sw.Write("<td>" + totaldamage + "</td>");
                                    sw.Write("<td>" + mindamage + "</td>");
                                    sw.Write("<td>" + avgdamage + "</td>");
                                    sw.Write("<td>" + maxdamage + "</td>");
                                    sw.Write("<td>" + casts + "</td>");
                                    sw.Write("<td>" + hits + "</td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)crit / hits) + "%</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)flank / hits) + "%</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)glance / hits) + "%</td>");
                                    sw.Write("<td>" + Math.Round(timeswasted, 2) + "s</td>");
                                    sw.Write("<td>" + Math.Round(timessaved, 2) + "s</td>");
                                }
                                sw.Write("</tr>");
                            }
                            else if (skill.GetGW2APISkill() != null)
                            {
                                sw.Write("<tr>");
                                {
                                    sw.Write("<td align=\"left\"><img src=" + skill.GetGW2APISkill().icon + " alt=\"" + skill.getName() + "\" title=\"" + skill.getID() + "\" height=\"18\" width=\"18\">" + skill.getName() + "</td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td>" + casts + "</td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td>" + Math.Round(timeswasted, 2) + "s</td>");
                                    sw.Write("<td>" + Math.Round(timessaved, 2) + "s</td>");
                                }
                                sw.Write("</tr>");
                            }
                            else
                            {
                                sw.Write("<tr>");
                                {
                                    sw.Write("<td align=\"left\">" + skill.getName() + "</td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td>" + casts + "</td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td>" + Math.Round(timeswasted, 2) + "s</td>");
                                    sw.Write("<td>" + Math.Round(timessaved, 2) + "s</td>");
                                }
                                sw.Write("</tr>");
                            }
                        }
                    }
                }
                sw.Write("</tbody>");
                HTMLHelper.writeDamageDistTableFoot(sw, finalTotalDamage);
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the damage distribution table for a given minion
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">Player, master of the minion</param>
        /// <param name="damageLogs">Damage logs to use</param>
        /// <param name="agent">The minion</param>
        private void CreateDMGDistTable(StreamWriter sw, AbstractPlayer p, AgentItem agent, bool toBoss, int phase_index)
        {
            string finalDPSdata = HTMLHelper.getFinalDPS(boss_data, combat_data, agent_data, p, boss, phase_index);
            int totalDamage = toBoss ? Int32.Parse(finalDPSdata.Split('|')[7]) : Int32.Parse(finalDPSdata.Split('|')[1]);
            string tabid = p.getInstid() +"_"+phase_index + "_" + agent.getInstid() + (toBoss ? "_boss" : "");
            PhaseData phase = boss.getPhases(boss_data, combat_data.getCombatList(), agent_data)[phase_index];
            List<DamageLog> damageLogs = p.getMinionsDamageLogs(toBoss ? boss_data.getInstid() : 0, boss_data, combat_data.getCombatList(), agent_data)[agent].Where(x => x.getTime() >= phase.start && x.getTime() <= phase.end).ToList();
            int finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.getDamage()) : 0;
            if (totalDamage > 0)
            {
                string contribution = String.Format("{0:0.00}", 100.0 * finalTotalDamage / totalDamage);
                sw.Write("<div>" + agent.getName() + " did " + contribution + "% of " + p.getCharacter() + "'s total " + (toBoss ? "boss " : "") + "dps</div>");
            }
            sw.Write("<script> $(function () { $('#dist_table_" + tabid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dist_table_" + tabid + "\">");
            {
                SkillData s_data = skill_data;
                List<SkillItem> s_list = s_data.getSkillList();
                HTMLHelper.writeDamageDistTableHeader(sw);
                sw.Write("<tbody>");
                {
                    HashSet<int> usedIDs = new HashSet<int>();
                    HTMLHelper.writeDamageDistTableCondi(sw, usedIDs, damageLogs, finalTotalDamage);
                    foreach (int id in damageLogs.Where(x => !usedIDs.Contains(x.getID())).Select(x => x.getID()).Distinct())
                    {//foreach casted skill
                        SkillItem skill = s_list.FirstOrDefault(x => x.getID() == id);                               
                        if (skill != null)
                        {
                            HTMLHelper.writeDamageDistTableSkill(sw, skill, damageLogs.Where(x => x.getID() == id).ToList(), finalTotalDamage);
                        }
                    }
                }
                sw.Write("</tbody>");
                HTMLHelper.writeDamageDistTableFoot(sw, finalTotalDamage);
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the damage taken distribution table for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private void CreateDMGTakenDistTable(StreamWriter sw, Player p, int phase_index)
        {
            PhaseData phase = boss.getPhases(boss_data, combat_data.getCombatList(), agent_data)[phase_index];
            List<DamageLog> damageLogs = p.getDamageTakenLogs(boss_data, combat_data.getCombatList(), agent_data,mech_data, phase.start, phase.end);
            List<SkillItem> s_list = skill_data.getSkillList();
            int finalTotalDamage = damageLogs.Count > 0 ? damageLogs.Sum(x => x.getDamage()) : 0;
            string pid = p.getInstid() + "_" + phase_index;
            sw.Write("<script> $(function () { $('#distTaken_table_" + pid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"distTaken_table_" + pid + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Skill</th>");
                        sw.Write("<th>Damage</th>");
                        sw.Write("<th>Percent</th>");
                        sw.Write("<th>Hits</th>");
                        sw.Write("<th>Min</th>");
                        sw.Write("<th>Avg</th>");
                        sw.Write("<th>Max</th>");
                        sw.Write("<th>Crit</th>");
                        sw.Write("<th>Flank</th>");
                        sw.Write("<th>Glance</th>");
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    HashSet<int> usedIDs = new HashSet<int>();
                    List<Boon> condiList = Boon.getCondiBoonList();
                    foreach (Boon condi in condiList)
                    {
                        int condiID = condi.getID();
                        int totaldamage = 0;
                        int mindamage = 0;
                        int avgdamage = 0;
                        int hits = 0;
                        int maxdamage = 0;
                        usedIDs.Add(condiID);
                        foreach (DamageLog dl in damageLogs.Where(x => x.getID() == condiID))
                        {
                            int curdmg = dl.getDamage();
                            totaldamage += curdmg;
                            if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                            if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                            hits++;
                            int result = dl.getResult().getID();

                        }
                        avgdamage = (int)(totaldamage / (double)hits);
                        if (totaldamage > 0)
                        {
                            string condiName = condi.getName();// Boon.getCondiName(condiID);
                            sw.Write("<tr>");
                            {
                                sw.Write("<td align=\"left\"><img src=" + condi.getLink() + " alt=\"" + condiName + "\" title=\"" + condiID + "\" height=\"18\" width=\"18\">" + condiName + "</td>");
                                sw.Write("<td>" + totaldamage + "</td>");
                                sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                                sw.Write("<td>" + hits + "</td>");
                                sw.Write("<td>" + mindamage + "</td>");
                                sw.Write("<td>" + avgdamage + "</td>");
                                sw.Write("<td>" + maxdamage + "</td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                            }
                            sw.Write("</tr>");
                        }
                    }
                    foreach (int id in damageLogs.Where(x => !usedIDs.Contains( x.getID())).Select(x => x.getID()).Distinct())
                    {//foreach casted skill
                        SkillItem skill = s_list.FirstOrDefault(x => x.getID() == id);

                        int totaldamage = 0;
                        int mindamage = 0;
                        int avgdamage = 0;
                        int hits = 0;
                        int maxdamage = 0;
                        int crit = 0;
                        int flank = 0;
                        int glance = 0;
                        foreach (DamageLog dl in damageLogs.Where(x => x.getID() == id))
                        {
                            int curdmg = dl.getDamage();
                            totaldamage += curdmg;
                            if (0 == mindamage || curdmg < mindamage) { mindamage = curdmg; }
                            if (0 == maxdamage || curdmg > maxdamage) { maxdamage = curdmg; }
                            if (curdmg != 0) { hits++; };
                            int result = dl.getResult().getID();
                            if (result == 1) { crit++; } else if (result == 2) { glance++; }
                            if (dl.isFlanking() == 1) { flank++; }
                        }
                        avgdamage = (int)(totaldamage / (double)hits);

                        if (skill != null)
                        {
                            if (totaldamage > 0 && skill.GetGW2APISkill() != null)
                            {
                                sw.Write("<tr>");
                                {
                                    sw.Write("<td align=\"left\"><img src=" + skill.GetGW2APISkill().icon + " alt=\"" + skill.getName() + "\" title=\"" + skill.getID() + "\" height=\"18\" width=\"18\">" + skill.getName() + "</td>");
                                    sw.Write("<td>" + totaldamage + "</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                                    sw.Write("<td>" + hits + "</td>");
                                    sw.Write("<td>" + mindamage + "</td>");
                                    sw.Write("<td>" + avgdamage + "</td>");
                                    sw.Write("<td>" + maxdamage + "</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)crit / hits) + "%</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)flank / hits) + "%</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)glance / hits) + "%</td>");
                                }                             
                                sw.Write("</tr>");
                            }
                            else if (totaldamage > 0)
                            {
                                sw.Write("<tr>");
                                {
                                    sw.Write("<td align=\"left\">" + skill.getName() + "</td>");
                                    sw.Write("<td>" + totaldamage + "</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                                    sw.Write("<td>" + hits + "</td>");
                                    sw.Write("<td>" + mindamage + "</td>");
                                    sw.Write("<td>" + avgdamage + "</td>");
                                    sw.Write("<td>" + maxdamage + "</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)crit / hits) + "%</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)flank / hits) + "%</td>");
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)glance / hits) + "%</td>");
                                }
                                sw.Write("</tr>");
                            }
                        }
                    }
                }
                sw.Write("</tbody>");
            }       
            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the mechanics table of the fight
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateMechanicTable(StreamWriter sw, int phase_index) {
            Dictionary<string,List<Mechanic>> presMech = new Dictionary<string, List<Mechanic>>();
            PhaseData phase = boss.getPhases(boss_data, combat_data.getCombatList(), agent_data)[phase_index];
            foreach (Mechanic item in mech_data.GetMechList(boss_data.getID()))
            {
                if (mech_data.GetMDataLogs().FirstOrDefault(x => x.GetSkill() == item.GetSkill()) != null)
                {
                    if (!presMech.ContainsKey(item.GetAltName()))
                    {
                        presMech[item.GetAltName()] = new List<Mechanic>();
                    }
                    presMech[item.GetAltName()].Add(item);                   
                }
            }
            if (presMech.Count() > 0)
            {
                sw.Write("<script> $(function () { $('#mech_table"+ phase_index+"').DataTable({ \"order\": [[0, \"desc\"]]});});</script>");
                sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"mech_table"+ phase_index+"\">");
                {
                    sw.Write("<thead>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<th>Player</th>");
                            foreach (string mechalt in presMech.Keys)
                            {
                                sw.Write("<th><span>" + mechalt + "</span></th>");
                            }
                        }                   
                        sw.Write("</tr>");
                    }
                    
                    sw.Write("</thead>");
                    sw.Write("<tbody>");
                    {
                        foreach (Player p in p_list)
                        {
                            sw.Write("<tr>");
                            {
                                sw.Write("<td>" + p.getCharacter() + "</td>");
                                foreach (List<Mechanic> mechs in presMech.Values)
                                {
                                    int count = 0;
                                    foreach (Mechanic mech in mechs)
                                    {
                                        List<MechanicLog> test = mech_data.GetMDataLogs().Where(x => x.GetSkill() == mech.GetSkill() && x.GetPlayer() == p && x.GetTime() >= phase.start / 1000 && x.GetTime() <= phase.end / 1000).ToList();
                                        count += test.Count();                                     
                                    }
                                    sw.Write("<td>" + count + "</td>");
                                }
                            }
                            sw.Write(" </tr>");
                        }
                    }
                    sw.Write("</tbody>");
                }
                sw.Write("</table>");
            }
        }
        /// <summary>
        /// Creates the event list of the generation. Debbuging only
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateEventList(StreamWriter sw) {
            sw.Write("<ul class=\"list-group\">");
            {
                foreach (CombatItem c in combat_data.getCombatList())
                {
                    if (c.isStateChange().getID() > 0)
                    {
                        AgentItem agent = agent_data.GetAgent(c.getSrcAgent());
                        if (agent != null)
                        {
                            switch (c.isStateChange().getID())
                            {
                                case 1:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " entered combat in" + c.getDstAgent() + "subgroup" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case 2:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " exited combat" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case 3:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is now alive" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case 4:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is now dead" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case 5:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is now downed" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case 6:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is now in logging range of POV player" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case 7:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is now out of range of logging player" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case 8:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is at " + c.getDstAgent() / 100 + "% health" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case 9:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   " LOG START" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case 10:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                  "LOG END" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case 11:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " weapon swapped to " + c.getDstAgent() + "(0/1 water, 4/5 land)" +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case 12:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " max health changed to  " + c.getDstAgent() +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                case 13:
                                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                   agent.getName() + " is recording log " +
                                                  // " <span class=\"badge badge-primary badge-pill\">14</span>"+
                                                  "</li>");
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }          
            sw.Write("</ul>");
        }
        /// <summary>
        /// Creates a skill list. Debugging only
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateSkillList(StreamWriter sw) {
            sw.Write("<ul class=\"list-group\">");
            {
                foreach (SkillItem skill in skill_data.getSkillList())
                {
                    sw.Write("<li class=\"list-group-item d-flex justify-content-between align-items-center\">" +
                                                  skill.getID() + " : " + skill.getName() +
                             "</li>");
                }
            }
            sw.Write("</ul>");
        }
        /// <summary>
        /// Creates the condition uptime table of the given boss
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="boss">The boss</param>
        private void CreateCondiUptimeTable(StreamWriter sw,Boss boss, int phase_index)
        {
            //Generate Boon table------------------------------------------------------------------------------------------------
            sw.Write("<script> $(function () { $('#condi_table"+ phase_index+"').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"condi_table"+ phase_index+"\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Name</th>");
                        foreach (Boon boon in Boon.getCondiBoonList())
                        {
                            sw.Write("<th>" + "<img src=\"" + boon.getLink() + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }                 
                    sw.Write("</tr>");
                }            
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<td>" + boss.getCharacter().ToString() + "</td>");
                        Dictionary<int, string> boonArray = HTMLHelper.getfinalcondis(boss_data,combat_data,skill_data,boss);
                        foreach (Boon boon in Boon.getCondiBoonList())
                        {
                            sw.Write("<td>" + boonArray[boon.getID()] + "</td>");
                        }
                    }            
                    sw.Write("</tr>");
                }             
                sw.Write("</tbody>");
            }       
            sw.Write("</table>");
        }
        /// <summary>
        /// Creates the boss summary tab
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateBossSummary(StreamWriter sw, int phase_index)
        {
            //generate Player list Graphs
            PhaseData phase = boss.getPhases(boss_data, combat_data.getCombatList(), agent_data)[phase_index];
            List<CastLog> casting = boss.getCastLogs(boss_data, combat_data.getCombatList(), agent_data, phase.start, phase.end);
            List<SkillItem> s_list = skill_data.getSkillList();
            string charname = boss.getCharacter();
            string pid = boss.getInstid() + "_" + phase_index;
            sw.Write("<h1 align=\"center\"> " + charname + "</h1>");
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#home" + pid + "\">" + boss.getCharacter() + "</a></li>");
                //foreach pet loop here
                foreach (AgentItem agent in boss.getMinionsDamageLogs(0, boss_data, combat_data.getCombatList(), agent_data).Keys)
                {
                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#minion" + pid + "_" + agent.getInstid()+ "\">" + agent.getName() + "</a></li>");
                }
            }         
            sw.Write("</ul>");
            //condi stats tab
            sw.Write("<div id=\"myTabContent\" class=\"tab-content\"><div class=\"tab-pane fade show active\" id=\"home" + pid + "\">");
            {
                CreateCondiUptimeTable(sw, boss, phase_index);
                sw.Write("<div id=\"Graph" + pid + "\" style=\"height: 800px;width:1000px; display:inline-block \"></div>");
                sw.Write("<script>");
                {
                    sw.Write("var data = [");
                    {
                        if (SnapSettings[6])//Display rotation
                        {

                            foreach (CastLog cl in casting)
                            {
                                HTMLHelper.writeCastingItem(sw, cl, skill_data, phase.start, phase.end);
                            }
                        }
                        //============================================
                        List<Boon> parseBoonsList = new List<Boon>();
                        //Condis
                        parseBoonsList.AddRange(Boon.getCondiBoonList());
                        //Every buffs and boons
                        parseBoonsList.AddRange(Boon.getAllBuffList());
                        Dictionary<int,BoonsGraphModel> boonGraphData = boss.getBoonGraphs(boss_data, skill_data,combat_data.getCombatList());
                        foreach (int boonid in boonGraphData.Keys.Reverse())
                        {
                            if (parseBoonsList.FirstOrDefault(x => x.getID() == boonid) != null)
                            {
                                sw.Write("{");
                                {
                                   
                                    BoonsGraphModel bgm = boonGraphData[boonid];
                                    List<Point> bChart = bgm.getBoonChart().Where(x => x.X >= phase.start / 1000 && x.X <= phase.end/1000).ToList();
                                    int bChartCount = 0;
                                    sw.Write("y: [");
                                    {
                                        foreach (Point pnt in bChart)
                                        {
                                            if (bChartCount == bChart.Count - 1)
                                            {
                                                sw.Write("'" + pnt.Y + "'");
                                            }
                                            else
                                            {
                                                sw.Write("'" + pnt.Y + "',");
                                            }
                                            bChartCount++;
                                        }
                                        if (bgm.getBoonChart().Count == 0)
                                        {
                                            sw.Write("'0'");
                                        }
                                    }
                                    sw.Write("],");
                                    sw.Write("x: [");
                                    {
                                        bChartCount = 0;
                                        foreach (Point pnt in bChart)
                                        {
                                            if (bChartCount == bChart.Count - 1)
                                            {
                                                sw.Write("'" + (pnt.X - (int)phase.start / 1000) + "'");
                                            }
                                            else
                                            {
                                                sw.Write("'" + (pnt.X - (int)phase.start / 1000) + "',");
                                            }
                                            bChartCount++;
                                        }
                                        if (bgm.getBoonChart().Count == 0)
                                        {
                                            sw.Write("'0'");
                                        }
                                    }
                                    sw.Write("],");
                                    sw.Write(" yaxis: 'y2'," +
                                         " type: 'scatter',");
                                    //  "legendgroup: '"+Boon.getEnum(bgm.getBoonName()).getPloltyGroup()+"',";
                                    if (bgm.getBoonName() == "Might" || bgm.getBoonName() == "Quickness")
                                    {

                                    }
                                    else
                                    {
                                        sw.Write(" visible: 'legendonly',");
                                    }
                                    sw.Write(" line: {color:'" + GetLink("Color-" + bgm.getBoonName()) + "'},");
                                    sw.Write(" fill: 'tozeroy'," +
                                         " name: \"" + bgm.getBoonName() + "\"");
                                }
                                sw.Write(" },");
                            }
                        }
                        //int maxDPS = 0;
                        if (SnapSettings[1])
                        {//show total dps plot
                            sw.Write("{");
                            //Adding dps axis
                            List<int[]> playertotaldpsgraphdata = HTMLHelper.getTotalDPSGraph(boss_data,combat_data,agent_data,boss,boss, phase_index);
                            sw.Write("y: [");
                            int ptdgCount = 0;
                            foreach (int[] dp in playertotaldpsgraphdata)
                            {
                                if (ptdgCount == playertotaldpsgraphdata.Count - 1)
                                {
                                    sw.Write("'" + dp[1] + "'");
                                }
                                else
                                {
                                    sw.Write("'" + dp[1] + "',");
                                }

                                ptdgCount++;
                            }
                            if (playertotaldpsgraphdata.Count == 0)
                            {
                                sw.Write("'0'");
                            }
                            sw.Write("],");
                            //add time axis
                            sw.Write("x: [");
                            ptdgCount = 0;
                            foreach (int[] dp in playertotaldpsgraphdata)
                            {
                                if (ptdgCount == playertotaldpsgraphdata.Count - 1)
                                {
                                    sw.Write("'" + dp[0] + "'");
                                }
                                else
                                {
                                    sw.Write("'" + dp[0] + "',");
                                }
                                ptdgCount++;
                            }
                            if (playertotaldpsgraphdata.Count == 0)
                            {
                                sw.Write("'0'");
                            }
                            sw.Write("],");
                            sw.Write(" mode: 'lines'," +
                                        " line: {shape: 'spline',color:'rgb(0,250,0)'}," +
                               " yaxis: 'y3'," +
                               // "legendgroup: 'Damage'," +
                               " name: 'Total DPS'" + "}");
                        }
                    }                   
                    sw.Write("];");
                    sw.Write("var layout = {");
                    {
                        sw.Write("yaxis: {" +
                               "title: 'Rotation', domain: [0, 0.09], fixedrange: true, showgrid: false," +
                               "range: [0, 2]" +
                           "}," +

                           "legend: { traceorder: 'reversed' }," +
                           "hovermode: 'compare'," +
                           "yaxis2: { title: 'Condis/Boons', domain: [0.11, 0.50], fixedrange: true }," +
                           "yaxis3: { title: 'DPS', domain: [0.51, 1] },");
                        sw.Write("images: [");
                        {
                            if (SnapSettings[7])//Display rotation
                            {
                                int castCount = 0;
                                foreach (CastLog cl in casting)
                                {
                                    HTMLHelper.writeCastingItemIcon(sw, cl, skill_data, phase.start, castCount == casting.Count - 1);
                                    castCount++;
                                }
                            }
                        }
                        sw.Write("],");
                        sw.Write("font: { color: '#ffffff' }," +
                                "paper_bgcolor: 'rgba(0,0,0,0)'," +
                                "plot_bgcolor: 'rgba(0,0,0,0)'");
                    }
                    sw.Write("};");
                    sw.Write("Plotly.newPlot('Graph" + pid + "', data, layout);");
                }
                sw.Write("</script> ");
                CreateDMGDistTable(sw, boss, false, phase_index);
                sw.Write("</div>");
                foreach (AgentItem agent in boss.getMinionsDamageLogs(0, boss_data, combat_data.getCombatList(), agent_data).Keys)
                {
                    sw.Write("<div class=\"tab-pane fade \" id=\"minion" + pid + "_" + agent.getInstid() + "\">");
                    {
                        CreateDMGDistTable(sw, boss, agent, false, phase_index);
                    }
                    sw.Write("</div>");
                }
            }         
            sw.Write("</div>");
        }
        /// <summary>
        /// To define
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateEstimateTabs(StreamWriter sw, int phase_index)
        {
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est_role"+ phase_index+"\">Roles</a>" +
                        "</li>" +

                        "<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est_cc" + phase_index + "\">CC</a>" +
                        "</li>" +
                         "<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est" + phase_index + "\">Maybe more</a>" +
                        "</li>");
            }
            sw.Write("</ul>");
            sw.Write("<div id=\"myTabContent\" class=\"tab-content\">");
            {
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est_role" + phase_index + "\">");
                {
                    //Use cards
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est_cc" + phase_index + "\">");
                {
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est" + phase_index + "\">");
                {
                }
                sw.Write("</div>");
            }
            sw.Write("</div>");
        }
        /// <summary>
        /// Creates custom css'
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="simpleRotSize">Size of the simple rotation images</param>
        private void CreateCustomCSS(StreamWriter sw,int simpleRotSize)
        {
            sw.Write("<style>");
            {
                sw.Write("table.dataTable.stripe tfoot tr, table.dataTable.display tfoot tr { background-color: #f9f9f9;}");
                sw.Write("td, th {text-align: center; white-space: nowrap;}");
                sw.Write("table.dataTable  td {color: black;}");
                sw.Write(".sorting_disabled {padding: 5px !important;}");
                sw.Write("table.dataTable.table-condensed.sorting, table.dataTable.table-condensed.sorting_asc, table.dataTable.table-condensed.sorting_desc ");
                sw.Write("{right: 4px !important;}table.dataTable thead.sorting_desc { color: red;}");
                sw.Write("table.dataTable thead.sorting_asc{color: green;}");
                sw.Write(".text-left {text-align: left;}");
                sw.Write("table.dataTable.table-condensed > thead > tr > th.sorting { padding-right: 5px !important; }");
                sw.Write(".rot-table {width: 100%;border-collapse: separate;border-spacing: 5px 0px;}");
                sw.Write(".rot-table > tbody > tr > td {padding: 1px;text-align: left;}");
                sw.Write(".rot-table > thead {vertical-align: bottom;border-bottom: 2px solid #ddd;}");
                sw.Write(".rot-table > thead > tr > th {padding: 10px 1px 9px 1px;line-height: 18px;text-align: left;}");
                sw.Write("div.dataTables_wrapper { width: 1100px; margin: 0 auto; }");
                sw.Write("th.dt-left, td.dt-left { text-align: left; }");
                sw.Write("table.dataTable.display tbody tr.condi {background-color: #ff6666;}");
                sw.Write(".rot-skill{width: " + simpleRotSize + "px;height: " + simpleRotSize + "px;display: inline - block;}");
                sw.Write(".rot-crop{width : " + simpleRotSize + "px;height: " + simpleRotSize + "px; display: inline-block}");
            }
            sw.Write("</style>");
        }
        /// <summary>
        /// Creates the whole html
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="settingsSnap">Settings</param>
        public void CreateHTML(StreamWriter sw, bool[] settingsSnap)
        {

            SnapSettings = settingsSnap;
            double fight_duration = (boss_data.getAwareDuration()) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fight_duration);
            string durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s";
            if (duration.ToString("hh") != "00")
            {
                durationString = duration.ToString("hh") + "h " + durationString;
            }
            string bossname = FilterStringChars(boss_data.getName());
            setPresentBoons(settingsSnap);
            List<PhaseData> phases = boss.getPhases(boss_data, combat_data.getCombatList(), agent_data);           
            // HTML STARTS
            sw.Write("<!DOCTYPE html><html lang=\"en\">");
            {
                sw.Write("<head>");
                {
                    sw.Write("<meta charset=\"utf-8\">" +
                      "<link rel=\"stylesheet\" href=\"https://bootswatch.com/4/slate/bootstrap.min.css \"  crossorigin=\"anonymous\">" +
                      "<link rel=\"stylesheet\" href=\"https://bootswatch.com/4/slate/bootstrap.css \"  crossorigin=\"anonymous\">" +
                      "<link href=\"https://fonts.googleapis.com/css?family=Open+Sans \" rel=\"stylesheet\">" +
                      "<link rel=\"stylesheet\" type=\"text/css\" href=\"https://cdn.datatables.net/1.10.16/css/jquery.dataTables.min.css \">" +
                      //JQuery
                      "<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js \"></script> " +
                      //popper
                      "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.6/umd/popper.min.js \"></script>" +
                      //js
                      "<script src=\"https://cdn.plot.ly/plotly-latest.min.js \"></script>" +
                      "<script src=\"https://cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js \"></script>" +
                      "<script src=\"https://cdn.datatables.net/plug-ins/1.10.13/sorting/alt-string.js \"></script>" +
                      "<script src=\"https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/js/bootstrap.min.js \"></script>");              
                    int simpleRotSize = 20;
                    if (settingsSnap[12])
                    {
                        simpleRotSize = 30;
                    }
                    CreateCustomCSS(sw,simpleRotSize);
                }
                sw.Write("<script>$.extend( $.fn.dataTable.defaults, {searching: false, ordering: true,paging: false,dom:\"t\"} );</script>");
                sw.Write("</head>");
                sw.Write("<body class=\"d-flex flex-column align-items-center\">");
                {
                    sw.Write("<div style=\"width: 1100px;\"class=\"d-flex flex-column\">");
                    {
                        sw.Write("<p> Time Start: " + log_data.getLogStart() + " | Time End: " + log_data.getLogEnd() + " </p> ");                     
                        sw.Write("<div class=\"d-flex flex-row justify-content-center align-items-center flex-wrap mb-3\">");
                        {
                            sw.Write("<div class=\"mr-3\">");
                            {
                                sw.Write("<div style=\"width: 400px;\" class=\"card border-danger d-flex flex-column\">");
                                {
                                    sw.Write("<h3 class=\"card-header text-center\">" + bossname + "</h3>");
                                    sw.Write("<div class=\"card-body d-flex flex-column align-items-center\">");
                                    {
                                        sw.Write("<blockquote class=\"card-blockquote mb-0\">");
                                        {
                                            sw.Write("<div style=\"width: 300px;\" class=\"d-flex flex-row justify-content-between align-items-center\">");
                                            {
                                                sw.Write("<div>");
                                                {
                                                    sw.Write("<img src=\"" + GetLink(boss_data.getID() + "-icon") + " \"alt=\"" + bossname + "-icon" + "\" style=\"height: 120px; width: 120px;\" >");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div>");
                                                {
                                                    sw.Write("<div class=\"progress\" style=\"width: 100 %; height: 20px;\">");
                                                    {
                                                        if (log_data.getBosskill())
                                                        {
                                                            string tp = boss_data.getHealth().ToString() + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-success\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:100%; ;\" aria-valuenow=\"100\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                        }
                                                        else
                                                        {
                                                            double finalPercent = 0;
                                                            if (boss_data.getHealthOverTime().Count > 0)
                                                            {
                                                                finalPercent = 100.0 - boss_data.getHealthOverTime()[boss_data.getHealthOverTime().Count - 1][1] * 0.01;
                                                            }
                                                            string tp = Math.Round(boss_data.getHealth() * finalPercent / 100.0) + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-success\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:" + finalPercent + "%;\" aria-valuenow=\""+ finalPercent+"\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                            tp = Math.Round(boss_data.getHealth() * (100.0-finalPercent) / 100.0) + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-danger\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:" + (100.0 - finalPercent) + "%;\" aria-valuenow=\""+ (100.0 - finalPercent )+ "\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                            
                                                        }
                                                    }
                                                    sw.Write("</div>");
                                                    sw.Write("<p class=\"small\" style=\"text-align:center; color: #FFF;\">" + boss_data.getHealth().ToString() + " Health</p>");
                                                    if (log_data.getBosskill())
                                                    {
                                                        sw.Write("<p class='text text-success'> Result: Success</p>");
                                                    }
                                                    else
                                                    {
                                                        sw.Write("<p class='text text-warning'> Result: Fail</p>");
                                                    }
                                                    sw.Write("<p>Duration " + durationString + " </p> ");
                                                }
                                                sw.Write("</div>");
                                            }
                                            sw.Write("</div>");
                                        }
                                        sw.Write("</blockquote>");
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                            sw.Write("<div class=\"ml-3 mt-3\">");
                            {
                                CreateCompTable(sw);
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("</div>");
                        //if (p_list.Count() == 1)//Create condensed version of log
                        //{
                        //    CreateSoloHTML(sw,settingsSnap);
                        //    return;
                        //}
                        if (phases.Count > 1)
                        {
                            sw.Write("<ul class=\"nav nav-tabs\">");
                            {
                                for (int i = 0; i < phases.Count; i++)
                                {
                                    string active = (i > 0 ? "" : "active");
                                    string name = i > 0 ? "Phase " + i : "Full Fight";
                                    sw.Write("<li class=\"nav-item\">" +
                                            "<a class=\"nav-link "+active+"\" data-toggle=\"tab\" href=\"#phase" + i + "\">" + name + "</a>" +
                                        "</li>");
                                }
                            }
                            sw.Write("</ul>");
                        }
                        sw.Write("<div id=\"myTabContent" + "\" class=\"tab-content\">");
                        {
                            for (int i = 0; i < phases.Count; i++)
                            {
                                string active = (i > 0 ? "" : "show active");


                                sw.Write("<div class=\"tab-pane fade " + active + "\" id=\"phase" + i + "\">");
                                {
                                    string Html_playerDropdown = "";
                                    foreach (Player p in p_list)
                                    {
                                        string charname = p.getCharacter();
                                        Html_playerDropdown += "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#" + p.getInstid() + "_" + i + "\">" + charname +
                                            "<img src=\"" + GetLink(p.getProf().ToString()) + " \" alt=\"" + p.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</a>";
                                    }
                                    sw.Write("<ul class=\"nav nav-tabs\">");
                                    {
                                        sw.Write("<li class=\"nav-item\">" +
                                                    "<a class=\"nav-link active\" data-toggle=\"tab\" href=\"#stats" + i + "\">Stats</a>" +
                                                "</li>" +

                                                "<li class=\"nav-item\">" +
                                                    "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#dmgGraph" + i + "\">Damage Graph</a>" +
                                                "</li>" +
                                                 "<li class=\"nav-item\">" +
                                                    "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#boons" + i + "\">Boons</a>" +
                                                "</li>" +
                                                "<li class=\"nav-item\">" +
                                                    "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#mechTable" + i + "\">Mechanics</a>" +
                                                "</li>" +
                                                "<li class=\"nav-item dropdown\">" +
                                                    "<a class=\"nav-link dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"true\">Player</a>" +
                                                    "<div class=\"dropdown-menu \" x-placement=\"bottom-start\">" +
                                                        Html_playerDropdown +
                                                    "</div>" +
                                                "</li>");
                                        if (settingsSnap[9])
                                        {
                                            sw.Write("<li class=\"nav-item\">" +
                                                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#bossSummary" + i + "\">Boss</a>" +
                                                        "</li>");
                                        }
                                        if (settingsSnap[8])
                                        {
                                            sw.Write("<li class=\"nav-item\">" +
                                                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#eventList" + i + "\">Event List</a>" +
                                                        "</li>");
                                        }
                                        if (settingsSnap[13])
                                        {
                                            sw.Write("<li class=\"nav-item\">" +
                                                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#estimates" + i + "\">Estimates</a>" +
                                                        "</li>");
                                        }
                                    }
                                    sw.Write("</ul>");
                                    sw.Write("<div id=\"myTabContent" + "\" class=\"tab-content\">");
                                    {
                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"stats" + i + "\">");
                                        {
                                            //Stats Tab
                                            sw.Write("<h3 align=\"center\"> Stats </h3>");

                                            sw.Write("<ul class=\"nav nav-tabs\">" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#dpsStats" + i + "\">DPS</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offStats" + i + "\">Damage Stats</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defStats" + i + "\">Defensive Stats</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#healStats" + i + "\">Heal Stats</a></li>" +
                                                "</ul>");
                                            sw.Write("<div id=\"statsSubTab" + i + "\" class=\"tab-content\">");
                                            {
                                                sw.Write("<div class=\"tab-pane fade show active\" id=\"dpsStats" + i + "\">");
                                                {
                                                    // DPS table
                                                    CreateDPSTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade \" id=\"offStats" + i + "\">");
                                                {
                                                    // HTML_dmgstats 
                                                    CreateDMGStatsTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade \" id=\"defStats" + i + "\">");
                                                {
                                                    // def stats
                                                    CreateDefTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade\" id=\"healStats" + i + "\">");
                                                {
                                                    //  HTML_supstats
                                                    CreateSupTable(sw, i);
                                                }
                                                sw.Write("</div>");
                                            }
                                            sw.Write("</div>");

                                        }
                                        sw.Write("</div>");

                                        sw.Write("<div class=\"tab-pane fade\" id=\"dmgGraph" + i + "\">");
                                        {
                                            //Html_dpsGraph
                                            CreateDPSGraph(sw, i);
                                        }
                                        sw.Write("</div>");
                                        //Boon Stats
                                        sw.Write("<div class=\"tab-pane fade \" id=\"boons" + i + "\">");
                                        {
                                            //Boons Tab
                                            sw.Write("<h3 align=\"center\"> Boons </h3>");

                                            sw.Write("<ul class=\"nav nav-tabs\">" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#mainBoon" + i + "\">Boons</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offBuff" + i + "\">Damage Buffs</a></li>" +
                                                    "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defBuff" + i + "\">Defensive Buffs</a></li>" +
                                                "</ul>");
                                            sw.Write("<div id=\"boonsSubTab" + i + "\" class=\"tab-content\">");
                                            {
                                                sw.Write("<div class=\"tab-pane fade show active  \" id=\"mainBoon" + i + "\">");
                                                {
                                                    sw.Write("<ul class=\"nav nav-tabs\">" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#boonsUptime" + i + "\">Uptime</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenSelf" + i + "\">Generation (Self)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenGroup" + i + "\">Generation (Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenOGroup" + i + "\">Generation (Off-Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenSquad" + i + "\">Generation (Squad)</a></li>" +
                                                           "</ul>");
                                                    sw.Write("<div id=\"mainBoonsSubTab" + i + "\" class=\"tab-content\">");
                                                    {
                                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"boonsUptime" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boon Uptime</p>");
                                                            // Html_boons
                                                            CreateUptimeTable(sw, present_boons, "boons_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenSelf" + i + "\">");
                                                        {
                                                            //Html_boonGenSelf
                                                            sw.Write("<p> Boons generated by a character for themselves</p>");
                                                            CreateGenSelfTable(sw, present_boons, "boongenself_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boons generated by a character for their sub group</p>");
                                                            // Html_boonGenGroup
                                                            CreateGenGroupTable(sw, present_boons, "boongengroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenOGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boons generated by a character for any subgroup that is not their own</p>");
                                                            // Html_boonGenOGroup
                                                            CreateGenOGroupTable(sw, present_boons, "boongenogroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenSquad" + i + "\">");
                                                        {
                                                            sw.Write("<p> Boons generated by a character for the entire squad</p>");
                                                            //  Html_boonGenSquad
                                                            CreateGenSquadTable(sw, present_boons, "boongensquad_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade  \" id=\"offBuff" + i + "\">");
                                                {
                                                    sw.Write("<ul class=\"nav nav-tabs\">" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#offensiveUptime" + i + "\">Uptime</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenSelf" + i + "\">Generation (Self)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenGroup" + i + "\">Generation (Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenOGroup" + i + "\">Generation (Off-Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenSquad" + i + "\">Generation (Squad)</a></li>" +
                                                           "</ul>");
                                                    sw.Write("<div id=\"offBuffSubTab" + i + "\" class=\"tab-content\">");
                                                    {
                                                        //Offensive Buffs stats
                                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"offensiveUptime" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs Uptime</p>");
                                                            CreateUptimeTable(sw, present_offbuffs, "offensive_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenSelf" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for themselves</p>");
                                                            CreateGenSelfTable(sw, present_offbuffs, "offensivegenself_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for their sub group</p>");
                                                            CreateGenGroupTable(sw, present_offbuffs, "offensivegengroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenOGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for any subgroup that is not their own</p>");
                                                            CreateGenOGroupTable(sw, present_offbuffs, "offensivegenogroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenSquad" + i + "\">");
                                                        {
                                                            sw.Write("<p> Offensive Buffs generated by a character for the entire squad</p>");
                                                            CreateGenSquadTable(sw, present_offbuffs, "offensivegensquad_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div class=\"tab-pane fade  \" id=\"defBuff" + i + "\">");
                                                {
                                                    sw.Write("<ul class=\"nav nav-tabs\">" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#defensiveUptime" + i + "\">Uptime</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenSelf" + i + "\">Generation (Self)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenGroup" + i + "\">Generation (Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenOGroup" + i + "\">Generation (Off-Group)</a></li>" +
                                                               "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenSquad" + i + "\">Generation (Squad)</a></li>" +
                                                           "</ul>");
                                                    sw.Write("<div id=\"defBuffSubTab" + i + "\" class=\"tab-content\">");
                                                    {
                                                        //Defensive Buffs stats
                                                        sw.Write("<div class=\"tab-pane fade show active\" id=\"defensiveUptime" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs Uptime</p>");
                                                            CreateUptimeTable(sw, present_defbuffs, "defensive_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenSelf" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for themselves</p>");
                                                            CreateGenSelfTable(sw, present_defbuffs, "defensivegenself_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for their sub group</p>");
                                                            CreateGenGroupTable(sw, present_defbuffs, "defensivegengroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenOGroup" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for any subgroup that is not their own</p>");
                                                            CreateGenOGroupTable(sw, present_defbuffs, "defensivegenogroup_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                        sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenSquad" + i + "\">");
                                                        {
                                                            sw.Write("<p> Defensive Buffs generated by a character for the entire squad</p>");
                                                            CreateGenSquadTable(sw, present_defbuffs, "defensivegensquad_table", i);
                                                        }
                                                        sw.Write("</div>");
                                                    }
                                                    sw.Write("</div>");
                                                }
                                                sw.Write("</div>");
                                            }
                                            sw.Write("</div>");
                                        }
                                        sw.Write("</div>");
                                        //mechanics
                                        sw.Write("<div class=\"tab-pane fade\" id=\"mechTable" + i + "\">");
                                        {
                                            sw.Write("<p>Mechanics</p>");
                                            CreateMechanicTable(sw, i);
                                        }
                                        sw.Write("</div>");
                                        //boss summary
                                        if (settingsSnap[9])
                                        {
                                            sw.Write("<div class=\"tab-pane fade\" id=\"bossSummary" + i + "\">");
                                            {
                                                CreateBossSummary(sw, i);
                                            }
                                            sw.Write("</div>");
                                        }
                                        //event list
                                        if (settingsSnap[8] && i == 0)
                                        {
                                            sw.Write("<div class=\"tab-pane fade\" id=\"eventList" + i + "\">");
                                            {
                                                sw.Write("<p>List of all events.</p>");
                                                // CreateEventList(sw);
                                                CreateSkillList(sw);
                                            }
                                            sw.Write("</div>");
                                        }
                                        //boss summary
                                        if (settingsSnap[13])
                                        {
                                            sw.Write("<div class=\"tab-pane fade\" id=\"estimates" + i + "\">");
                                            {
                                                CreateEstimateTabs(sw, i);
                                            }
                                            sw.Write("</div>");
                                        }
                                        //Html_playertabs
                                        CreatePlayerTab(sw, settingsSnap, i);
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");

                            }
                        }
                        sw.Write("</div>");
                        sw.Write("<div>");
                        for (int i = 1; i < phases.Count; i++)
                        {
                            sw.Write("<p>Phase " + i + " started at " + phases[i].start / 1000 + "s and ended at " + phases[i].end / 1000 + "s</p>");
                        }
                        sw.Write("</div>");
                        sw.Write("<p style=\"margin-top:10px;\"> ARC:" + log_data.getBuildVersion().ToString() + " | Bossid " + boss_data.getID().ToString() + " </p> ");
                        sw.Write("<p style=\"margin-top:-15px;\">File recorded by: " + log_data.getPOV() + "</p>");
                    }
                    sw.Write("</div>");
                }         
                sw.Write("</body>");
                sw.Write("<script> $(document).ready(function(){$('[data-toggle=\"tooltip\"]').tooltip(); });</script >");
            }     
            //end
            sw.Write("</html>");
            return;
        }
        public void CreateSoloHTML(StreamWriter sw, bool[] settingsSnap)
        {
            double fight_duration = (boss_data.getAwareDuration()) / 1000.0;
            Player p = p_list[0];               
            List<CastLog> casting = p.getCastLogs(boss_data, combat_data.getCombatList(), agent_data, 0, boss_data.getAwareDuration());
            List<SkillItem> s_list = skill_data.getSkillList();

            CreateDPSTable(sw, 0);
            CreateDMGStatsTable(sw, 0);
            CreateDefTable(sw, 0);
            CreateSupTable(sw, 0);
            // CreateDPSGraph(sw);
            sw.Write("<div id=\"Graph" + p.getInstid() + "\" style=\"height: 800px;width:1000px; display:inline-block \"></div>");
            sw.Write("<script>");
            {
                sw.Write("var data = [");
                {
                    if (SnapSettings[6])//Display rotation
                    {

                        foreach (CastLog cl in casting)
                        {
                            HTMLHelper.writeCastingItem(sw, cl, skill_data, 0, boss_data.getAwareDuration());
                        }
                    }
                    if (present_boons.Count() > 0)
                    {
                        List<Boon> parseBoonsList = new List<Boon>();
                        parseBoonsList.AddRange(present_boons);
                        parseBoonsList.AddRange(present_offbuffs);
                        parseBoonsList.AddRange(present_defbuffs);
                        if (present_personnal.ContainsKey(p.getInstid()))
                        {
                            parseBoonsList.AddRange(present_personnal[p.getInstid()]);
                        }
                        Dictionary<int,BoonsGraphModel> boonGraphData = p.getBoonGraphs(boss_data, skill_data, combat_data.getCombatList());
                        foreach (int boonid in boonGraphData.Keys.Reverse())
                        {
                            if (parseBoonsList.FirstOrDefault(x => x.getID() == boonid) != null)
                            {
                                sw.Write("{");
                                {
                                    BoonsGraphModel bgm = boonGraphData[boonid];
                                    List<Point> bChart = bgm.getBoonChart();
                                    int bChartCount = 0;
                                    sw.Write("y: [");
                                    {
                                        foreach (Point pnt in bChart)
                                        {
                                            if (bChartCount == bChart.Count - 1)
                                            {
                                                sw.Write("'" + pnt.Y + "'");
                                            }
                                            else
                                            {
                                                sw.Write("'" + pnt.Y + "',");
                                            }
                                            bChartCount++;
                                        }
                                        if (bgm.getBoonChart().Count == 0)
                                        {
                                            sw.Write("'0'");
                                        }
                                    }
                                    sw.Write("],");
                                    sw.Write("x: [");
                                    {
                                        bChartCount = 0;
                                        foreach (Point pnt in bChart)
                                        {
                                            if (bChartCount == bChart.Count - 1)
                                            {
                                                sw.Write("'" + pnt.X + "'");
                                            }
                                            else
                                            {
                                                sw.Write("'" + pnt.X + "',");
                                            }
                                            bChartCount++;
                                        }
                                        if (bgm.getBoonChart().Count == 0)
                                        {
                                            sw.Write("'0'");
                                        }
                                    }
                                    sw.Write("],");
                                    sw.Write("yaxis: 'y2'," +
                                             "type: 'scatter',");
                                    //  "legendgroup: '"+Boon.getEnum(bgm.getBoonName()).getPloltyGroup()+"',";
                                    if (bgm.getBoonName() == "Might" || bgm.getBoonName() == "Quickness")
                                    {
                                    }
                                    else
                                    {
                                        sw.Write(" visible: 'legendonly',");
                                    }
                                    sw.Write("line: {shape: 'hv', color:'" + GetLink("Color-" + bgm.getBoonName()) + "'},");
                                    sw.Write("fill: 'tozeroy'," +
                                            "name: \"" + bgm.getBoonName() + "\"");
                                }
                                sw.Write(" },");
                            }
                        }
                    }
                    int maxDPS = 0;
                    if (SnapSettings[2])
                    {//show boss dps plot
                     //Adding dps axis
                        List<int[]> playerbossdpsgraphdata = HTMLHelper.getBossDPSGraph(boss_data,combat_data,agent_data,p,boss, 0);
                        int pbdgCount = 0;
                        sw.Write("{");
                        {
                            sw.Write("y: [");
                            {
                                foreach (int[] dp in playerbossdpsgraphdata)
                                {
                                    if (maxDPS < dp[1])
                                    {
                                        maxDPS = dp[1];
                                    }
                                    if (pbdgCount == playerbossdpsgraphdata.Count - 1)
                                    {
                                        sw.Write("'" + dp[1] + "'");
                                    }
                                    else
                                    {
                                        sw.Write("'" + dp[1] + "',");
                                    }
                                    pbdgCount++;
                                }
                                if (playerbossdpsgraphdata.Count == 0)
                                {
                                    sw.Write("'0'");
                                }
                            }
                            sw.Write("],");
                            //add time axis
                            sw.Write("x: [");
                            {
                                pbdgCount = 0;
                                foreach (int[] dp in playerbossdpsgraphdata)
                                {
                                    if (pbdgCount == playerbossdpsgraphdata.Count - 1)
                                    {
                                        sw.Write("'" + dp[0] + "'");
                                    }
                                    else
                                    {
                                        sw.Write("'" + dp[0] + "',");
                                    }

                                    pbdgCount++;
                                }
                                if (playerbossdpsgraphdata.Count == 0)
                                {
                                    sw.Write("'0'");
                                }
                            }
                            sw.Write("],");
                            sw.Write("mode: 'lines'," +
                                    "line: {shape: 'spline',color:'" + GetLink("Color-" + p.getProf()) + "'}," +
                                    "yaxis: 'y3'," +
                                    // "legendgroup: 'Damage',"+
                                    "name: 'Boss DPS'");
                        }

                        sw.Write("},");
                    }
                    if (SnapSettings[1])
                    {//show total dps plot
                        sw.Write("{");
                        { //Adding dps axis
                            List<int[]> playertotaldpsgraphdata = HTMLHelper.getTotalDPSGraph(boss_data,combat_data,agent_data,p, boss,0);
                            int ptdgCount = 0;
                            sw.Write("y: [");
                            {
                                foreach (int[] dp in playertotaldpsgraphdata)
                                {
                                    if (ptdgCount == playertotaldpsgraphdata.Count - 1)
                                    {
                                        sw.Write("'" + dp[1] + "'");
                                    }
                                    else
                                    {
                                        sw.Write("'" + dp[1] + "',");
                                    }
                                    ptdgCount++;
                                }
                                if (playertotaldpsgraphdata.Count == 0)
                                {
                                    sw.Write("'0'");
                                }
                            }
                            sw.Write("],");
                            //add time axis
                            sw.Write("x: [");
                            {
                                ptdgCount = 0;
                                foreach (int[] dp in playertotaldpsgraphdata)
                                {
                                    if (ptdgCount == playertotaldpsgraphdata.Count - 1)
                                    {
                                        sw.Write("'" + dp[0] + "'");
                                    }
                                    else
                                    {
                                        sw.Write("'" + dp[0] + "',");
                                    }
                                    ptdgCount++;
                                }
                                if (playertotaldpsgraphdata.Count == 0)
                                {
                                    sw.Write("'0'");
                                }
                            }
                            sw.Write("],");
                            sw.Write(" mode: 'lines'," +
                                   "line: {shape: 'spline',color:'rgb(0,250,0)'}," +
                                   "yaxis: 'y3'," +
                                   // "legendgroup: 'Damage'," +
                                   "name: 'Total DPS'");

                        }
                        sw.Write("}");
                    }
                }
                sw.Write("];");
                sw.Write("var layout = {");
                {
                    sw.Write("barmode:'stack',");
                    sw.Write("yaxis: {" +
                                 "title: 'Rotation', domain: [0, 0.09], fixedrange: true, showgrid: false," +
                                 "range: [0, 2]" +
                             "}," +
                             "legend: { traceorder: 'reversed' }," +
                             "hovermode: 'compare'," +
                             "yaxis2: { title: 'Boons', domain: [0.11, 0.50], fixedrange: true }," +
                             "yaxis3: { title: 'DPS', domain: [0.51, 1] },"
                     );
                    sw.Write("images: [");
                    {
                        if (SnapSettings[7])//Display rotation
                        {
                            int castCount = 0;
                            foreach (CastLog cl in casting)
                            {
                                HTMLHelper.writeCastingItemIcon(sw, cl, skill_data, 0, castCount == casting.Count - 1);
                                castCount++;
                            }
                        }
                    }
                    sw.Write("],");
                    sw.Write("font: { color: '#ffffff' }," +
                            "paper_bgcolor: 'rgba(0,0,0,0)'," +
                            "plot_bgcolor: 'rgba(0,0,0,0)'");
                }
                sw.Write("};");
                sw.Write("Plotly.newPlot('Graph" + p.getInstid() + "', data, layout);");
            }
            sw.Write("</script> ");
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#distTabAll" + p.getInstid() + "\">" + "All" + "</a></li>");
                sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#distTabBoss" + p.getInstid() + "\">" + "Boss" + "</a></li>");
            }
            sw.Write("</ul>");
            sw.Write("<div class=\"tab-content\">");
            {
                sw.Write("<div class=\"tab-pane fade show active\" id=\"distTabAll" + p.getInstid() + "\">");
                {
                    CreateDMGDistTable(sw, p, false,0);
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade\" id=\"distTabBoss" + p.getInstid() + "\">");
                {
                    CreateDMGDistTable(sw, p, true,0);
                }
                sw.Write("</div>");
            }
            sw.Write("</div>");
        }
        //Creating CSV---------------------------------------------------------------------------------
        public void CreateCSV(StreamWriter sw,String delimiter)
        {
            double fight_duration = (boss_data.getAwareDuration()) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fight_duration);
            String durationString = duration.ToString("mm") +":" + duration.ToString("ss") ;

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

                Dictionary<int, string> boonArray = HTMLHelper.getfinalboons(boss_data,combat_data,skill_data,p);
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
        public string GetLink(string name)
        {
            switch (name)
            {
                case "Question":
                    return "https://wiki.guildwars2.com/images/thumb/d/de/Sword_slot.png/40px-Sword_slot.png";
                case "Sword":
                    return "https://wiki.guildwars2.com/images/0/07/Crimson_Antique_Blade.png";
                case "Axe":
                    return "https://wiki.guildwars2.com/images/d/d4/Crimson_Antique_Reaver.png";
                case "Dagger":
                    return "https://wiki.guildwars2.com/images/6/65/Crimson_Antique_Razor.png";
                case "Mace":
                    return "https://wiki.guildwars2.com/images/6/6d/Crimson_Antique_Flanged_Mace.png";
                case "Pistol":
                    return "https://wiki.guildwars2.com/images/4/46/Crimson_Antique_Revolver.png";
                case "Scepter":
                    return "https://wiki.guildwars2.com/images/e/e2/Crimson_Antique_Wand.png";
                case "Focus":
                    return "https://wiki.guildwars2.com/images/8/87/Crimson_Antique_Artifact.png";
                case "Shield":
                    return "https://wiki.guildwars2.com/images/b/b0/Crimson_Antique_Bastion.png";
                case "Torch":
                    return "https://wiki.guildwars2.com/images/7/76/Crimson_Antique_Brazier.png";
                case "Warhorn":
                    return "https://wiki.guildwars2.com/images/1/1c/Crimson_Antique_Herald.png";
                case "Greatsword":
                    return "https://wiki.guildwars2.com/images/5/50/Crimson_Antique_Claymore.png";
                case "Hammer":
                    return "https://wiki.guildwars2.com/images/3/38/Crimson_Antique_Warhammer.png";
                case "Longbow":
                    return "https://wiki.guildwars2.com/images/f/f0/Crimson_Antique_Greatbow.png";
                case "Shortbow":
                    return "https://wiki.guildwars2.com/images/1/17/Crimson_Antique_Short_Bow.png";
                case "Rifle":
                    return "https://wiki.guildwars2.com/images/1/19/Crimson_Antique_Musket.png";
                case "Staff":
                    return "https://wiki.guildwars2.com/images/5/5f/Crimson_Antique_Spire.png";
                case "Vale Guardian-icon":
                    return "https://wiki.guildwars2.com/images/f/fb/Mini_Vale_Guardian.png";
                case "Gorseval the Multifarious-icon":
                    return "https://wiki.guildwars2.com/images/d/d1/Mini_Gorseval_the_Multifarious.png";
                case "Sabetha the Saboteur-icon":
                    return "https://wiki.guildwars2.com/images/5/54/Mini_Sabetha.png";
                case "Slothasor-icon":
                    return "https://wiki.guildwars2.com/images/e/ed/Mini_Slubling.png";
                case "Matthias Gabrel-icon":
                    return "https://wiki.guildwars2.com/images/5/5d/Mini_Matthias_Abomination.png";
                case "Keep Construct-icon":
                    return "https://wiki.guildwars2.com/images/e/ea/Mini_Keep_Construct.png";
                case "Xera-icon":
                    return "https://wiki.guildwars2.com/images/4/4b/Mini_Xera.png";
                case "Cairn the Indomitable-icon":
                    return "https://wiki.guildwars2.com/images/b/b8/Mini_Cairn_the_Indomitable.png";
                case "Mursaat Overseer-icon":
                    return "https://wiki.guildwars2.com/images/c/c8/Mini_Mursaat_Overseer.png";
                case "Samarog-icon":
                    return "https://wiki.guildwars2.com/images/f/f0/Mini_Samarog.png";
                case "Deimos-icon":
                    return "https://wiki.guildwars2.com/images/e/e0/Mini_Ragged_White_Mantle_Figurehead.png";
                case "Soulless Horror-icon":
                    return "https://wiki.guildwars2.com/images/d/d4/Mini_Desmina.png";
                case "Dhuum-icon":
                    return "https://wiki.guildwars2.com/images/e/e4/Mini_Dhuum.png";
                case "Vale Guardian-ext":
                    return "vg";
                case "Gorseval the Multifarious-ext":
                    return "gors";
                case "Sabetha the Saboteur-ext":
                    return "sab";
                case "Slothasor-ext":
                    return "sloth";
                case "Matthias Gabrel-ext":
                    return "matt";
                case "Keep Construct-ext":
                    return "kc";
                case "Xera-ext":
                    return "xera";
                case "Cairn the Indomitable-ext":
                    return "cairn";
                case "Mursaat Overseer-ext":
                    return "mo";
                case "Samarog-ext":
                    return "sam";
                case "Deimos-ext":
                    return "dei";
                case "Soulless Horror-ext":
                    return "sh";
                case "Dhuum-ext":
                    return "dhuum";
                    //ID version for multilingual
                case "15438-icon":
                    return "https://wiki.guildwars2.com/images/f/fb/Mini_Vale_Guardian.png";
                case "15429-icon":
                    return "https://wiki.guildwars2.com/images/d/d1/Mini_Gorseval_the_Multifarious.png";
                case "15375-icon":
                    return "https://wiki.guildwars2.com/images/5/54/Mini_Sabetha.png";
                case "16123-icon":
                    return "https://wiki.guildwars2.com/images/e/ed/Mini_Slubling.png";
                case "16115-icon":
                    return "https://wiki.guildwars2.com/images/5/5d/Mini_Matthias_Abomination.png";
                case "16235-icon":
                    return "https://wiki.guildwars2.com/images/e/ea/Mini_Keep_Construct.png";
                case "16246-icon":
                    return "https://wiki.guildwars2.com/images/4/4b/Mini_Xera.png";
                case "17194-icon":
                    return "https://wiki.guildwars2.com/images/b/b8/Mini_Cairn_the_Indomitable.png";
                case "17172-icon":
                    return "https://wiki.guildwars2.com/images/c/c8/Mini_Mursaat_Overseer.png";
                case "17188-icon":
                    return "https://wiki.guildwars2.com/images/f/f0/Mini_Samarog.png";
                case "17154-icon":
                    return "https://wiki.guildwars2.com/images/e/e0/Mini_Ragged_White_Mantle_Figurehead.png";
                case "19767-icon":
                    return "https://wiki.guildwars2.com/images/d/d4/Mini_Desmina.png";
                case "19450-icon":
                    return "https://wiki.guildwars2.com/images/e/e4/Mini_Dhuum.png";
                case "15438-ext":
                    return "vg";
                case "15429-ext":
                    return "gors";
                case "15375-ext":
                    return "sab";
                case "16123-ext":
                    return "sloth";
                case "16115-ext":
                    return "matt";
                case "16235-ext":
                    return "kc";
                case "16246-ext":
                    return "xera";
                case "17194-ext":
                    return "cairn";
                case "17172-ext":
                    return "mo";
                case "17188-ext":
                    return "sam";
                case "17154-ext":
                    return "dei";
                case "19767-ext":
                    return "sh";
                case "19450-ext":
                    return "dhuum";

                case "Warrior":
                    return "https://wiki.guildwars2.com/images/4/43/Warrior_tango_icon_20px.png";
                case "Berserker":
                    return "https://wiki.guildwars2.com/images/d/da/Berserker_tango_icon_20px.png";
                case "Spellbreaker":
                    return "https://wiki.guildwars2.com/images/e/ed/Spellbreaker_tango_icon_20px.png";
                case "Guardian":
                    return "https://wiki.guildwars2.com/images/8/8c/Guardian_tango_icon_20px.png";
                case "Dragonhunter":
                    return "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png";
                case "DragonHunter":
                    return "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png";
                case "Firebrand":
                    return "https://wiki.guildwars2.com/images/0/02/Firebrand_tango_icon_20px.png";
                case "Revenant":
                    return "https://wiki.guildwars2.com/images/b/b5/Revenant_tango_icon_20px.png";
                case "Herald":
                    return "https://wiki.guildwars2.com/images/6/67/Herald_tango_icon_20px.png";
                case "Renegade":
                    return "https://wiki.guildwars2.com/images/7/7c/Renegade_tango_icon_20px.png";
                case "Engineer":
                    return "https://wiki.guildwars2.com/images/2/27/Engineer_tango_icon_20px.png";
                case "Scrapper":
                    return "https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png";
                case "Holosmith":
                    return "https://wiki.guildwars2.com/images/2/28/Holosmith_tango_icon_20px.png";
                case "Ranger":
                    return "https://wiki.guildwars2.com/images/4/43/Ranger_tango_icon_20px.png";
                case "Druid":
                    return "https://wiki.guildwars2.com/images/d/d2/Druid_tango_icon_20px.png";
                case "Soulbeast":
                    return "https://wiki.guildwars2.com/images/7/7c/Soulbeast_tango_icon_20px.png";
                case "Thief":
                    return "https://wiki.guildwars2.com/images/7/7a/Thief_tango_icon_20px.png";
                case "Daredevil":
                    return "https://wiki.guildwars2.com/images/e/e1/Daredevil_tango_icon_20px.png";
                case "Deadeye":
                    return "https://wiki.guildwars2.com/images/c/c9/Deadeye_tango_icon_20px.png";
                case "Elementalist":
                    return "https://wiki.guildwars2.com/images/a/aa/Elementalist_tango_icon_20px.png";
                case "Tempest":
                    return "https://wiki.guildwars2.com/images/4/4a/Tempest_tango_icon_20px.png";
                case "Weaver":
                    return "https://wiki.guildwars2.com/images/f/fc/Weaver_tango_icon_20px.png";
                case "Mesmer":
                    return "https://wiki.guildwars2.com/images/6/60/Mesmer_tango_icon_20px.png";
                case "Chronomancer":
                    return "https://wiki.guildwars2.com/images/f/f4/Chronomancer_tango_icon_20px.png";
                case "Mirage":
                    return "https://wiki.guildwars2.com/images/d/df/Mirage_tango_icon_20px.png";
                case "Necromancer":
                    return "https://wiki.guildwars2.com/images/4/43/Necromancer_tango_icon_20px.png";
                case "Reaper":
                    return "https://wiki.guildwars2.com/images/1/11/Reaper_tango_icon_20px.png";
                case "Scourge":
                    return "https://wiki.guildwars2.com/images/0/06/Scourge_tango_icon_20px.png";

                case"Color-Warrior":return "rgb(255,209,102)";
                case"Color-Berserker": return "rgb(255,209,102)";
                case"Color-Spellbreaker": return "rgb(255,209,102)";
                case"Color-Guardian": return "rgb(114,193,217)";
                case"Color-Dragonhunter": return "rgb(114,193,217)";
                case"Color-Firebrand": return "rgb(114,193,217)";
                case"Color-Revenant": return "rgb(209,110,90)";
                case"Color-Herald": return "rgb(209,110,90)";
                case"Color-Renegade": return "rgb(209,110,90)";
                case"Color-Engineer": return "rgb(208,156,89)";
                case"Color-Scrapper": return "rgb(208,156,89)";
                case"Color-Holosmith": return "rgb(208,156,89)";
                case"Color-Ranger": return "rgb(140,220,130)";
                case"Color-Druid": return "rgb(140,220,130)";
                case"Color-Soulbeast": return "rgb(140,220,130)";
                case"Color-Thief": return "rgb(192,143,149)";
                case"Color-Daredevil": return "rgb(192,143,149)";
                case"Color-Deadeye": return "rgb(192,143,149)";
                case"Color-Elementalist": return "rgb(246,138,135)";
                case"Color-Tempest": return "rgb(246,138,135)";
                case"Color-Weaver": return "rgb(246,138,135)";
                case"Color-Mesmer": return "rgb(182,121,213)";
                case"Color-Chronomancer": return "rgb(182,121,213)";
                case"Color-Mirage": return "rgb(182,121,213)";
                case"Color-Necromancer": return "rgb(82,167,111)";
                case"Color-Reaper": return "rgb(82,167,111)";
                case"Color-Scourge": return "rgb(82,167,111)";

                case "Crit":
                    return "https://wiki.guildwars2.com/images/9/95/Critical_Chance.png";
                case "Scholar":
                    return "https://wiki.guildwars2.com/images/thumb/2/2b/Superior_Rune_of_the_Scholar.png/40px-Superior_Rune_of_the_Scholar.png";
                case "SwS":
                    return "https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png";
                case "Downs":
                    return "https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png";
                case "Dead":
                    return "https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png";
                case "Flank":
                    return "https://wiki.guildwars2.com/images/thumb/b/bb/Hunter%27s_Tactics.png/40px-Hunter%27s_Tactics.png";
                case "Glance":
                    return "https://wiki.guildwars2.com/images/f/f9/Weakness.png";
                case "Miss":
                    return "https://wiki.guildwars2.com/images/3/33/Blinded.png";
                case "Interupts":
                    return "https://wiki.guildwars2.com/images/thumb/7/79/Daze.png/20px-Daze.png";
                case "Invuln":
                    return "https://wiki.guildwars2.com/images/e/eb/Determined.png";
                case "Blinded":
                    return "https://wiki.guildwars2.com/images/thumb/3/33/Blinded.png/20px-Blinded.png";
                case "Wasted":
                    return "https://wiki.guildwars2.com/images/b/b3/Out_Of_Health_Potions.png";
                case "Saved":
                    return "https://wiki.guildwars2.com/images/e/eb/Ready.png";
                case "Swap":
                    return "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png";
                case "Blank":
                    return "https://wiki.guildwars2.com/images/thumb/d/de/Sword_slot.png/40px-Sword_slot.png";
                case "Dodge":
                    return "https://wiki.guildwars2.com/images/c/cc/Dodge_Instructor.png";
                case "Bandage":
                    return "https://render.guildwars2.com/file/D2D7D11874060D68760BFD519CFC77B6DF14981F/102928.png";
                    
                case "Color-Aegis": return "rgb(102,255,255)";
                case "Color-Fury": return "rgb(255,153,0)";
                case "Color-Might": return "rgb(153,0,0)";
                case "Color-Protection": return "rgb(102,255,255)";
                case "Color-Quickness": return "rgb(255,0,255)";
                case "Color-Regeneration": return "rgb(0,204,0)";
                case "Color-Resistance": return "rgb(255, 153, 102)";
                case "Color-Retaliation": return "rgb(255, 51, 0)";
                case "Color-Stability": return "rgb(153, 102, 0)";
                case "Color-Swiftness": return "rgb(255,255,0)";
                case "Color-Vigor": return "rgb(102, 153, 0)";

                case "Color-Alacrity": return "rgb(0,102,255)";
                case "Color-Glyph of Empowerment": return "rgb(204, 153, 0)";
                case "Color-Grace of the Land": return "rgb(,,)";
                case "Color-Sun Spirit": return "rgb(255, 102, 0)";
                case "Color-Banner of Strength": return "rgb(153, 0, 0)";
                case "Color-Banner of Discipline": return "rgb(0, 51, 0)";
                case "Color-Spotter": return "rgb(0,255,0)";
                case "Color-Stone Spirit": return "rgb(204, 102, 0)";
                case "Color-Storm Spirit": return "rgb(102, 0, 102)";
                case "Color-Empower Allies": return "rgb(255, 153, 0)";

                case "Condi": return "https://wiki.guildwars2.com/images/5/54/Condition_Damage.png";
                case "Healing": return "https://wiki.guildwars2.com/images/8/81/Healing_Power.png";
                case "Tough": return "https://wiki.guildwars2.com/images/1/12/Toughness.png";
                default:
                    return "";
            }

        }   
        
    }
}
