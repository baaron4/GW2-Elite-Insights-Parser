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
//recomend CTRL+M+O to collapse all
namespace LuckParser.Controllers
{
    public class Controller1
    {
        private static byte[] StreamToBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        private GW2APIController APIContrioller = new GW2APIController();

        // Private Methods
        //for pulling from binary
        private MemoryStream stream = new MemoryStream();
        private void safeSkip(long bytes_to_skip)
        {

            while (bytes_to_skip > 0)
            {
                int dummyByte = stream.ReadByte();
                long bytes_actually_skipped = 1;
                if (bytes_actually_skipped > 0)
                {
                    bytes_to_skip -= bytes_actually_skipped;
                }
                else if (bytes_actually_skipped == 0)
                {
                    if (stream.ReadByte() == -1)
                    {
                        break;
                    }
                    else
                    {
                        bytes_to_skip--;
                    }
                }
            }

            return;
        }
        private int getbyte()
        {
            byte byt = Convert.ToByte(stream.ReadByte());
            // stream.Position++;

            return byt;
        }
        private ushort getShort()
        {
            byte[] bytes = new byte[2];
            for (int b = 0; b < bytes.Length; b++)
            {
                bytes[b] = Convert.ToByte(stream.ReadByte());
                //stream.Position++;
            }
            // return Short.toUnsignedInt(ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).getShort());
            return BitConverter.ToUInt16(bytes, 0);
        }
        private int getInt()
        {
            byte[] bytes = new byte[4];
            for (int b = 0; b < bytes.Length; b++)
            {
                bytes[b] = Convert.ToByte(stream.ReadByte());
                // stream.Position++;
            }
            //return ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).getInt();
            return BitConverter.ToInt32(bytes, 0);
        }
        private long getLong()
        {
            byte[] bytes = new byte[8];
            for (int b = 0; b < bytes.Length; b++)
            {
                bytes[b] = Convert.ToByte(stream.ReadByte());
                // stream.Position++;
            }

            // return ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).getLong();
            return BitConverter.ToInt64(bytes, 0);
        }
        private String getString(int length)
        {
            byte[] bytes = new byte[length];
            for (int b = 0; b < bytes.Length; b++)
            {
                bytes[b] = Convert.ToByte(stream.ReadByte());
                // stream.Position++;
            }

            string s = new String(System.Text.Encoding.UTF8.GetString(bytes).ToCharArray()).TrimEnd();
            if (s != null)
            {
                return s;
            }
            return "UNKNOWN";
        }
        private String FilterStringChars(string str)
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

        // Public Methods
        public LogData getLogData()
        {
            return log_data;
        }
        public BossData getBossData()
        {
            return boss_data;
        }
        public AgentData getAgentData()
        {
            return agent_data;
        }
        public SkillData getSkillData()
        {
            return skill_data;
        }
        public CombatData getCombatData()
        {
            return combat_data;
        }
        public MechanicData getMechData() {
            return mech_data;
        }


        //Main Parse method------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Parses the given log
        /// </summary>
        /// <param name="evtc">The path to the log to parse</param>
        /// <returns></returns>
        public bool ParseLog(string evtc)
        {
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

                parseBossData();
                parseAgentData();
                parseSkillData();
                parseCombatList();
                fillMissingData();

                stream.Close();
            }
            ////CreateHTML(); is now runnable dont run here
            return (true);
        }

        //sub Parse methods
        /// <summary>
        /// Parses boss related data
        /// </summary>
        private void parseBossData()
        {
            // 12 bytes: arc build version
            String build_version = getString(12);
            this.log_data = new LogData(build_version);

            // 1 byte: skip
            safeSkip(1);

            // 2 bytes: boss instance ID
            ushort instid = getShort();

            // 1 byte: position
            safeSkip(1);

            //Save
            // TempData["Debug"] = build_version +" "+ instid.ToString() ;
            this.boss_data = new BossData(instid);
        }
        /// <summary>
        /// Parses agent related data
        /// </summary>
        private void parseAgentData()
        {
            // 4 bytes: player count
            int player_count = getInt();
          
            // 96 bytes: each player
            for (int i = 0; i < player_count; i++)
            {
                // 8 bytes: agent
                long agent = getLong();

                // 4 bytes: profession
                int prof = getInt();

                // 4 bytes: is_elite
                int is_elite = getInt();

                // 4 bytes: toughness
                int toughness = getInt();

                // 4 bytes: healing
                int healing = getInt();

                // 4 bytes: condition
                int condition = getInt();

                // 68 bytes: name
                String name = getString(68);
                //Save
                Agent a = new Agent(agent, name, prof, is_elite);
                if (a != null)
                {
                    // NPC
                    if (a.getProf(this.log_data.getBuildVersion(),APIContrioller) == "NPC")
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
        private void parseSkillData()
        {
            GW2APIController apiController = new GW2APIController();
            // 4 bytes: player count
            int skill_count = getInt();
            //TempData["Debug"] += "Skill Count:" + skill_count.ToString();
            // 68 bytes: each skill
            for (int i = 0; i < skill_count; i++)
            {
                // 4 bytes: skill ID
                int skill_id = getInt();

                // 64 bytes: name
                String name = getString(64);
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
        private void parseCombatList()
        {
            // 64 bytes: each combat
            while (stream.Length - stream.Position >= 64)
            {
                // 8 bytes: time
                long time = getLong();

                // 8 bytes: src_agent
                long src_agent = getLong();

                // 8 bytes: dst_agent
                long dst_agent = getLong();

                // 4 bytes: value
                int value = getInt();

                // 4 bytes: buff_dmg
                int buff_dmg = getInt();

                // 2 bytes: overstack_value
                ushort overstack_value = getShort();

                // 2 bytes: skill_id
                ushort skill_id = getShort();

                // 2 bytes: src_instid
                ushort src_instid = getShort();

                // 2 bytes: dst_instid
                ushort dst_instid = getShort();

                // 2 bytes: src_master_instid
                ushort src_master_instid = getShort();

                // 9 bytes: garbage
                safeSkip(9);

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
                safeSkip(2);

                //save
                // Add combat
                combat_data.addItem(new CombatItem(time, src_agent, dst_agent, value, buff_dmg, overstack_value, skill_id,
                        src_instid, dst_instid, src_master_instid, iff, buff, result, is_activation, is_buffremoved,
                        is_ninety, is_fifty, is_moving, is_statechange, is_flanking,is_shields));
            }
        }
        /// <summary>
        /// Parses all the data again and link related stuff to each other
        /// </summary>
        private void fillMissingData()
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
           
            if(boss_data.getID() == 16246) {
                int xera_2_instid = 0;
                foreach (AgentItem NPC in NPC_list)
                {
                    if (NPC.getProf().Contains("16286"))
                    {
                        bossHealthOverTime = new List<long[]>();//reset boss health over time
                        xera_2_instid = NPC.getInstid();
                        boss_data.setHealth(24085950);
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
                        boss_data.setLastAware(NPC.getLastAware());
                        //List<CombatItem> fuckyou = combat_list.Where(x => x.getDstInstid() == deimos_2_instid ).ToList().Sum(x);
                        //int stop = 0;
                        foreach (CombatItem c in combat_list)
                        {
                            if (c.getTime() > oldAware)
                            {
                                int lol = c.isStateChange().getID();
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
                List<AgentItem> playerAgentList = getAgentData().getPlayerAgentList();

                foreach (AgentItem playerAgent in playerAgentList)
                {
                    List<Point> lp = combat_data.getStates(playerAgent.getInstid(), "DESPAWN");
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
                        
                        player.SetDC(lp[0].X);
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
            List<SkillItem> s_list = getSkillData().getSkillList();
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
                List<CombatItem> c_data = getCombatData().getCombatList();
                foreach (Player p in p_list)
                {
                    present_personnal[p.getInstid()] = new List<Boon>();
                    foreach (Boon boon in Boon.getRemainingBuffsList(p.getProf()))
                    {
                        if (c_data.Exists(x => x.getSkillID() == boon.getID() && x.getDstInstid() == p.getInstid()))
                        {
                            present_personnal[p.getInstid()].Add(boon);
                        }
                    }
                }
            }        
        }
        private String getFinalDPS(Player p)
        {
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();

            int totalboss_dps = 0;
            int totalboss_damage = 0;
            int totalbosscondi_dps = 0;
            int totalbosscondi_damage = 0;
            int totalbossphys_dps = 0;
            int totalbossphys_damage = 0;
            int totalAll_dps = 0;
            int totalAll_damage = 0;
            int totalAllcondi_dps = 0;
            int totalAllcondi_damage = 0;
            int totalAllphys_dps = 0;
            int totalAllphys_damage = 0;
            double fight_duration = (b_data.getLastAware() - b_data.getFirstAware()) / 1000.0;


            double damage = 0.0;
            double dps = 0.0;
            // All DPS
            
            damage = p.getDamageLogs(0, b_data, c_data.getCombatList(), getAgentData()).Sum(x => x.getDamage());//p.getDamageLogs(b_data, c_data.getCombatList()).stream().mapToDouble(DamageLog::getDamage).sum();
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalAll_dps = (int)dps;
            totalAll_damage = (int)damage;
            //Allcondi
            damage = p.getDamageLogs(0, b_data, c_data.getCombatList(), getAgentData()).Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalAllcondi_dps = (int)dps;
            totalAllcondi_damage = (int)damage;
            //All Power
            damage = totalAll_damage - damage;
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalAllphys_dps = (int)dps;
            totalAllphys_damage = (int)damage;

            // boss DPS
            damage = p.getDamageLogs(b_data.getInstid(), b_data, c_data.getCombatList(), getAgentData()).Sum(x => x.getDamage());//p.getDamageLogs(b_data, c_data.getCombatList()).stream().mapToDouble(DamageLog::getDamage).sum();
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalboss_dps = (int)dps;
            totalboss_damage = (int)damage;
            //bosscondi
            damage = p.getDamageLogs(b_data.getInstid(), b_data, c_data.getCombatList(), getAgentData()).Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalbosscondi_dps = (int)dps;
            totalbosscondi_damage = (int)damage;
            //boss Power
            damage = totalboss_damage - damage;
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalbossphys_dps = (int)dps;
            totalbossphys_damage = (int)damage;
            //Placeholders for further calc
            return totalAll_dps.ToString() + "|" + totalAll_damage.ToString() + "|" + totalAllphys_dps.ToString() + "|" + totalAllphys_damage.ToString() + "|" + totalAllcondi_dps.ToString() + "|" + totalAllcondi_damage.ToString() + "|"
                + totalboss_dps.ToString() + "|" + totalboss_damage.ToString() + "|" + totalbossphys_dps.ToString() + "|" + totalbossphys_damage.ToString() + "|" + totalbosscondi_dps.ToString() + "|" + totalbosscondi_damage.ToString();
        }
        private String[] getFinalStats(Player p)
        {
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            String[] statsArray;
            List<DamageLog> damage_logs = p.getDamageLogs(0, b_data, c_data.getCombatList(), getAgentData());
            List<CastLog> cast_logs = p.getCastLogs( b_data, c_data.getCombatList(), getAgentData());
            int instid = p.getInstid();

            // Rates
            int power_loop_count = 0;
            int critical_rate = 0;
            int scholar_rate = 0;
            int scholar_dmg = 0;
            int totaldamage = p.getDamageLogs(0, b_data, c_data.getCombatList(), getAgentData()).Sum(x => x.getDamage());

            int moving_rate = 0;
            int flanking_rate = 0;
            //glancerate
            int glance_rate = 0;
            //missed
            int missed = 0;
            //interupted
            int interupts = 0;
            //times enemy invulned
            int invulned = 0;

            //timeswasted
            int wasted = 0;
            double time_wasted = 0;
            //Time saved
            int saved = 0;
            double time_saved = 0;
            //avgboons
            double avgBoons = 0.0;

            foreach (DamageLog log in damage_logs)
            {
                if (log.isCondi() == 0)
                {
                    if (log.getResult().getEnum() == "CRIT")
                    {
                        critical_rate++;
                    }
                    if (log.isNinety()>0) {
                        scholar_rate++;
                        
                        scholar_dmg += (int)(log.getDamage() / 11.0); //regular+10% damage
                    }
                    //scholar_rate += log.isNinety();
                    moving_rate += log.isMoving();
                    flanking_rate += log.isFlanking();
                    if (log.getResult().getEnum() == "GLANCE")
                    {
                        glance_rate++;
                    }
                    if (log.getResult().getEnum() == "BLIND")
                    {
                        missed++;
                    }
                    if (log.getResult().getEnum() == "INTERRUPT")
                    {
                        interupts++;
                    }
                    if (log.getResult().getEnum() == "ABSORB")
                    {
                        invulned++;
                    }
                    //if (log.isActivation().getEnum() == "CANCEL_FIRE" || log.isActivation().getEnum() == "CANCEL_CANCEL")
                    //{
                    //    wasted++;
                    //    time_wasted += log.getDamage();
                    //}
                    power_loop_count++;
                }
            }
            foreach (CastLog cl in cast_logs) {
                if (cl.endActivation() != null)
                {
                    if (cl.endActivation().getID() == 4)
                    {
                        wasted++;
                        time_wasted += cl.getActDur();
                    }
                    if (cl.endActivation().getID() == 3)
                    {
                        saved++;
                        if (cl.getActDur() < cl.getExpDur())
                        {
                            time_saved += cl.getExpDur() - cl.getActDur();
                        }
                    }
                }
            }

            power_loop_count = (power_loop_count == 0) ? 1 : power_loop_count;

            // Counts
            int swap = c_data.getStates(instid, "WEAPON_SWAP").Count();
            int down = c_data.getStates(instid, "CHANGE_DOWN").Count();
            int dodge = c_data.getSkillCount(instid, 65001) + c_data.getBuffCount(instid, 40408);//dodge = 65001 mirage cloak =40408
            int ress = c_data.getSkillCount(instid, 1066); //Res = 1066

            // R.I.P
            List<Point> dead = c_data.getStates(instid, "CHANGE_DEAD");
            
            double died = 0.0;
            if (dead.Count() > 0)
            {
                died = dead[0].X - b_data.getFirstAware();
            }
            List<Point> disconect = c_data.getStates(instid, "DESPAWN");
            double dcd = 0.0;
            if (disconect.Count() > 0)
            {
                dcd = disconect[0].X - b_data.getFirstAware();
            }
            statsArray = new string[] { power_loop_count.ToString(), critical_rate.ToString(), scholar_rate.ToString(), moving_rate.ToString(),
                flanking_rate.ToString(), swap.ToString(),down.ToString(),dodge.ToString(),ress.ToString(),died.ToString("0.00"),
            glance_rate.ToString(),missed.ToString(),interupts.ToString(),invulned.ToString(),(time_wasted/1000f).ToString(),wasted.ToString(),avgBoons.ToString(),(time_saved/1000f).ToString(),saved.ToString(),
            scholar_dmg.ToString(),totaldamage.ToString(),dcd.ToString("0.00"),
            };
            return statsArray;
        }
        private string getDamagetaken(Player p)
        {
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            int instid = p.getInstid();
            int damagetaken = p.getDamagetaken(b_data, c_data.getCombatList(), getAgentData(),getMechData()).Sum();
            return damagetaken.ToString();
        }
        private string[] getFinalDefenses(Player p)
        {
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            List<DamageLog> damage_logs = p.getDamageTakenLogs(b_data, c_data.getCombatList(), getAgentData(),getMechData());
            int instid = p.getInstid();

            int damagetaken = p.getDamagetaken(b_data, c_data.getCombatList(), getAgentData(),getMechData()).Sum();
            int blocked = 0;
            //int dmgblocked = 0;
            int invulned = 0;
            int dmginvulned = 0;
            int dodge = c_data.getSkillCount(instid, 65001);//dodge = 65001
            dodge += c_data.getBuffCount(instid, 40408);//mirage cloak add
            int evades = 0;
            //int dmgevaded = 0;
            int dmgBarriar = 0;
            foreach (DamageLog log in damage_logs.Where(x => x.getResult().getEnum() == "BLOCK"))
            {
                blocked++;
                //dmgblocked += log.getDamage();
            }
            foreach (DamageLog log in damage_logs.Where(x => x.getResult().getEnum() == "ABSORB"))
            {
                invulned++;
                dmginvulned += log.getDamage();
            }
            foreach (DamageLog log in damage_logs.Where(x => x.getResult().getEnum() == "EVADE"))
            {
                evades++;
                // dmgevaded += log.getDamage();
            }
            foreach (DamageLog log in damage_logs.Where(x => x.isShields() == 1))
            {

                dmgBarriar += log.getDamage();
            }
            int down = c_data.getStates(instid, "CHANGE_DOWN").Count();
            // R.I.P
            List<Point> dead = c_data.getStates(instid, "CHANGE_DEAD");
            double died = 0.0;
            if (dead.Count() > 0)
            {
                died = dead[0].X - b_data.getFirstAware();
            }
            String[] statsArray = new string[] { damagetaken.ToString(),
                blocked.ToString(),"0"/*dmgblocked.ToString()*/,invulned.ToString(),dmginvulned.ToString(),
                dodge.ToString(),evades.ToString(),"0"/*dmgevaded.ToString()*/,
            down.ToString(),died.ToString("0.00"),dmgBarriar.ToString()};
            return statsArray;
        }
        //(currently not correct)
        private string[] getFinalSupport(Player p)
        {
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            // List<DamageLog> damage_logs = p.getDamageTakenLogs(b_data, c_data.getCombatList(), getAgentData());
            int instid = p.getInstid();
            int resurrects = 0;
            double restime = 0.0;
            int condiCleanse = 0;
            double condiCleansetime = 0.0;

            int[] resArray = p.getReses(b_data, c_data.getCombatList(), getAgentData());
            int[] cleanseArray = p.getCleanses(b_data, c_data.getCombatList(), getAgentData());
            resurrects = resArray[0];
            restime = resArray[1];
            condiCleanse = cleanseArray[0];
            condiCleansetime = cleanseArray[1];


            String[] statsArray = new string[] { resurrects.ToString(), (restime/1000f).ToString(), condiCleanse.ToString(), (condiCleansetime/1000f).ToString() };
            return statsArray;
        }
        private Dictionary<int, string> getfinalboons(Player p)
        {
            BossData b_data = getBossData();
            BoonDistribution boon_distrib = p.getBoonDistribution(b_data, getSkillData(), getCombatData().getCombatList());
            Dictionary<int, string> rates = new Dictionary<int, string>();
            long fight_duration = b_data.getLastAware() - b_data.getFirstAware();
            foreach (Boon boon in Boon.getAllBuffList())
            {
                string rate = "0";
                if (boon_distrib.ContainsKey(boon.getID()))
                {
                    if (boon.getType().Equals("duration"))
                    {
                        rate = Math.Round(100.0 * boon_distrib.getUptime(boon.getID()) / fight_duration, 1) + "%";
                    }
                    else if (boon.getType().Equals("intensity"))
                    {
                        rate = Math.Round((double)boon_distrib.getUptime(boon.getID()) / fight_duration, 1).ToString();
                    }

                }
                rates[boon.getID()] = rate;
            }
            return rates;
        }
        private Dictionary<int, string> getfinalboons(Player p, List<Player> trgetPlayers)
        {
            if (trgetPlayers.Count() == 0)
            {
                return getfinalboons(p);
            }
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            SkillData s_data = getSkillData();
            long fight_duration = b_data.getLastAware() - b_data.getFirstAware();
            Dictionary<Player, BoonDistribution> boon_logsDist = new Dictionary<Player, BoonDistribution>();
            foreach (Player player in trgetPlayers)
            {
                boon_logsDist[player] = player.getBoonDistribution(b_data, s_data, c_data.getCombatList());
            }
            Dictionary<int, string> rates = new Dictionary<int, string>();
            foreach (Boon boon in Boon.getAllBuffList())
            {
                string rate = "0";
                long total = 0;
                long totaloverstack = 0;
                foreach (Player player in trgetPlayers)
                {
                    BoonDistribution boon_dist = boon_logsDist[player];
                    if (boon_dist.ContainsKey(boon.getID()))
                    {
                        total += boon_dist.getGeneration(boon.getID(), p.getInstid());
                        totaloverstack += boon_dist.getOverstack(boon.getID(), p.getInstid());
                    }
                }
                totaloverstack += total;
                if (total > 0)
                {
                    if (boon.getType().Equals("duration"))
                    {
                        rate = "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\"" 
                            + Math.Round(100.0 * totaloverstack / fight_duration / trgetPlayers.Count, 1) + "% with overstack \">" 
                            + Math.Round(100.0 * total / fight_duration / trgetPlayers.Count, 1) 
                            + "%</span>";
                    }
                    else if (boon.getType().Equals("intensity"))
                    {
                        rate = "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\"" 
                            + Math.Round((double)totaloverstack / fight_duration / trgetPlayers.Count, 1).ToString() + " with overstack \">" 
                            + Math.Round((double)total / fight_duration / trgetPlayers.Count, 1).ToString() 
                            + "</span>";
                    }

                }
                rates[boon.getID()] = rate;
            }
            return rates;
        }
        private Dictionary<int, string> getfinalcondis(Player p)
        {
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            SkillData s_data = getSkillData();
            BoonDistribution boon_distrib = p.getBoonDistribution(b_data, s_data, c_data.getCombatList());
            Dictionary<int, string> rates = new Dictionary<int, string>();
            foreach (Boon boon in Boon.getCondiBoonList())
            {
                rates[boon.getID()] = "0";
                if (boon_distrib.ContainsKey(boon.getID()))
                {
                    string rate = "0";
                    if (boon.getType().Equals("duration"))
                    {
                        long fight_duration = b_data.getLastAware() - b_data.getFirstAware();
                        rate = Math.Round(100.0 * boon_distrib.getUptime(boon.getID()) / fight_duration, 1) + "%";
                    }
                    else if (boon.getType().Equals("intensity"))
                    {
                        long fight_duration = b_data.getLastAware() - b_data.getFirstAware();
                        rate = Math.Round((double)boon_distrib.getUptime(boon.getID()) / fight_duration, 1).ToString();
                    }

                    rates[boon.getID()] = rate;
                }
            }
            return rates;
        }
        private void setMechData() {
            List<int> mIDList = new List<int>();
            CombatData c_data = getCombatData();
            foreach (Player p in p_list)
            {

                List<Point> down = c_data.getStates(p.getInstid(), "CHANGE_DOWN");
                foreach (Point pnt in down) {
                    mech_data.AddItem(new MechanicLog((long)((pnt.X - boss_data.getFirstAware()) / 1000f), 0, "DOWN", 0, p, mech_data.GetPLoltyShape("DOWN")));
                }
                List<Point> dead = c_data.getStates(p.getInstid(), "CHANGE_DEAD");
                foreach (Point pnt in dead)
                {
                    mech_data.AddItem(new MechanicLog((long)((pnt.X - boss_data.getFirstAware()) / 1000f), 0, "DEAD", 0, p, mech_data.GetPLoltyShape("DEAD")));
                }
                List<DamageLog> dls = p.getDamageTakenLogs(boss_data, combat_data.getCombatList(), agent_data, mech_data);
                //Player hit by skill 3
                MechanicLog prevMech = null;
                foreach (DamageLog dLog in dls)
                {
                    string name = skill_data.getName(dLog.getID());
                    if (dLog.getResult().getID() < 3 ) {

                        foreach (Mechanic mech in getMechData().GetMechList(boss_data.getID()).Where(x=>x.GetMechType() == 3))
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
                            foreach (Mechanic mech in getMechData().GetMechList(boss_data.getID()).Where(x => x.GetMechType() == 0))
                            {
                                if (c.getSkillID() == mech.GetSkill())
                                {
                                    //dst player
                                    mech_data.AddItem(new MechanicLog((long)((c.getTime() - boss_data.getFirstAware())/1000f), c.getSkillID(), mech.GetName(), c.getValue(), p, mech.GetPlotly()));
                                    break;
                                }
                            }
                            //player on player 7
                            foreach (Mechanic mech in getMechData().GetMechList(boss_data.getID()).Where(x => x.GetMechType() == 7))
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
        /// Gets the points for the boss dps graph for a given player
        /// </summary>
        /// <param name="p">The player</param>
        /// <returns></returns>
        private List<int[]> getBossDPSGraph(Player p) {
            List<int[]> bossdmgList = new List<int[]>();
            if (p.getBossDPSGraph().Count == 0)
            {
                BossData b_data = getBossData();
                CombatData c_data = getCombatData();

                // bossdmgList.Add(new int[] {1, 0 });
                // List<DamageLog> damage_logs_all = p.getDamageLogs(0, b_data, c_data.getCombatList(), getAgentData());
                List<DamageLog> damage_logs_boss = p.getDamageLogs(b_data.getInstid(), b_data, c_data.getCombatList(), getAgentData());
                int totaldmg = 0;

                int timeGraphed = 0;
                foreach (DamageLog log in damage_logs_boss)
                {

                    totaldmg += log.getDamage();

                    long time = log.getTime();
                    if (time > 1000)
                    {

                        //to reduce processing time only graph 1 point per sec
                        if (Math.Floor(time / 1000f) > timeGraphed)
                        {

                            if ((Math.Floor(time / 1000f) - timeGraphed) < 2)
                            {
                                timeGraphed = (int)Math.Floor(time / 1000f);
                                bossdmgList.Add(new int[] { (int)time / 1000, (int)(totaldmg / (time / 1000f)) });
                            }
                            else
                            {
                                int gap = (int)Math.Floor(time / 1000f) - timeGraphed;
                                bool startOfFight = true;
                                if (bossdmgList.Count > 0)
                                {
                                    startOfFight = false;
                                }

                                for (int itr = 0; itr < gap - 1; itr++)
                                {
                                    timeGraphed++;
                                    if (!startOfFight)
                                    {
                                        bossdmgList.Add(new int[] { timeGraphed, (int)(totaldmg / (float)timeGraphed) });
                                    }
                                    else
                                    {//hasnt hit boss yet gap
                                        bossdmgList.Add(new int[] { timeGraphed, 0 });
                                    }

                                }
                            }

                        }


                    }

                }
                p.setBossDPSGraph(bossdmgList);
            }
            else {
                bossdmgList = p.getBossDPSGraph();
            }
            
            return bossdmgList;
        }
        /// <summary>
        /// Gets the points for the total dps graph for a given player
        /// </summary>
        /// <param name="p">The player</param>
        /// <returns></returns>
        private List<int[]> getTotalDPSGraph(Player p)
        {
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            List<int[]> totaldmgList = new List<int[]>();
           // totaldmgList.Add(new int[] { 1, 0 });
            List<DamageLog> damage_logs_all = p.getDamageLogs(0, b_data, c_data.getCombatList(), getAgentData());
            //List<DamageLog> damage_logs_boss = p.getDamageLogs(b_data.getInstid(), b_data, c_data.getCombatList(), getAgentData());
            int totaldmg = 0;
            int timeGraphed = 0;
            foreach (DamageLog log in damage_logs_all)
            {
                totaldmg += log.getDamage();
                long time = log.getTime();
                if (time > 1000)
                {
                    
                    // to reduce processing time only graph 1 point per sec
                    if (Math.Floor(time / 1000f) > timeGraphed)
                    {
                        if ((Math.Floor(time / 1000f) - timeGraphed) < 2)
                        {
                            timeGraphed = (int)Math.Floor(time / 1000f);
                            totaldmgList.Add(new int[] { (int)time / 1000, (int)(totaldmg / (time / 1000f)) });
                        }
                        else
                        {
                            int gap = (int)Math.Floor(time / 1000f) - timeGraphed;
                            bool startOfFight = true;
                            if (totaldmgList.Count > 0)
                            {
                                startOfFight = false;
                            }
                            for (int itr = 0; itr < gap - 1; itr++)
                            {
                                timeGraphed++;
                               // totaldmgList.Add(new int[] { timeGraphed, (int)(totaldmg / (float)timeGraphed) });
                                if (!startOfFight)
                                {
                                    totaldmgList.Add(new int[] { timeGraphed, (int)(totaldmg / (float)timeGraphed) });
                                }
                                else
                                {//hasnt hit boss yet gap
                                    totaldmgList.Add(new int[] { timeGraphed, 0 });
                                }
                            }
                        }
                    }
                }
            }
            return totaldmgList;
        }
        /// <summary>
        /// Creates the dps graph
        /// </summary>
        /// <param name="sw">Stream writer</param>
        private void CreateDPSGraph(StreamWriter sw)
        {
            //Generate DPS graph
            sw.Write("<div id=\"DPSGraph\" style=\"height: 600px;width:1200px; display:inline-block \"></div>");
            sw.Write("<script>");

            sw.Write("var data = [");
            int maxDPS = 0;
            List<int[]> totalDpsAllPlayers = new List<int[]>();
            foreach (Player p in p_list)
            {
                //Adding dps axis
                List<int[]> playerbossdpsgraphdata = new List<int[]>(getBossDPSGraph(p));
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
                    List<int[]> playertotaldpsgraphdata = getTotalDPSGraph(p);
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
                    int[] check = getBossDPSGraph(ml.GetPlayer()).FirstOrDefault(x => x[0] == ml.GetTime());
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
                    int[] check = getBossDPSGraph(ml.GetPlayer()).FirstOrDefault(x => x[0] == ml.GetTime());
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
                    "marker:{" + getMechData().GetPLoltyShape(state) + "size: 15" + "}," +
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
                    "Plotly.newPlot('DPSGraph', data, layout);");
            sw.Write("</script> ");
        }
        private void GetRoles()
        {
            //tags: tank,healer,dps(power/condi)
            //Roles:greenteam,green split,cacnoneers,flakkiter,eater,KCpusher,agony,epi,handkiter,golemkiter,orbs
        }
        private void PrintWeapons(StreamWriter sw, Player p)
        {
             SkillData s_data = getSkillData();
             CombatData c_data = getCombatData();
             BossData b_data = getBossData();
            AgentData a_data = getAgentData();
            //print weapon sets
            string[] wep = p.getWeaponsArray(s_data,c_data,b_data,a_data);
            if (wep[0] != null)
            {
                sw.Write("<img src=\"" + GetLink(wep[0]) + " \" alt=\"" + wep[0] + "\" data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" +wep[0] + "\">");
            }
            else
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
                sw.Write("/");
            }
            
            if (wep[2] != null)
            {
                sw.Write("<img src=\"" + GetLink(wep[2]) + " \" alt=\"" + wep[2] + "\"  data-toggle=\"tooltip\" title=\"\" height=\"18\" width=\"18\" data-original-title=\"" + wep[2] + "\">");
            }
            else
            {
               // sw.Write("<img src=\"" + GetLink("Question") + " \" alt=\"Unknown\" height=\"18\" width=\"18\" >");
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
                                PrintWeapons(sw,gPlay);
                                sw.Write(build + "<br/>" + charName);
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
        private void CreateDPSTable(StreamWriter sw,double fight_duration) {
            //generate dps table
            sw.Write("<script> $(function () { $('#dps_table').DataTable({ \"order\": [[4, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dps_table\">");
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
                    string[] dmg = getFinalDPS(player).Split('|');
                    string[] stats = getFinalStats(player);
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
                        
                        if (timedead > TimeSpan.Zero)
                        {
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + " (" + (int)((timedead.TotalSeconds / fight_duration) * 100) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</span>" + " </td>");
                        }
                        else
                        {
                            sw.Write("<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died 100% Alive \"> 0</span>" + " </td>");
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
        private void CreateDMGStatsTable(StreamWriter sw,double fight_duration) {
            //generate dmgstats table
            sw.Write("<script> $(function () { $('#dmgstats_table').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dmgstats_table\">");
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
                        string[] stats = getFinalStats(player);
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
        private void CreateDefTable(StreamWriter sw, double fight_duration) {
            //generate Tankstats table
            sw.Write("<script> $(function () { $('#defstats_table').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"defstats_table\">");
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
                        string[] stats = getFinalDefenses(player);
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
        private void CreateSupTable(StreamWriter sw, double fight_duration) {
            //generate suppstats table
            sw.Write("<script> $(function () { $('#supstats_table').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"supstats_table\">");
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
                        string[] stats = getFinalSupport(player);
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
        private void CreateUptimeTable(StreamWriter sw, List<Boon> list_to_use, string table_id)
        {
            //Generate Boon table------------------------------------------------------------------------------------------------
            sw.Write("<script> $(function () { $('#" + table_id + "').DataTable({ \"order\": [[0, \"asc\"]]});});</script>");
            List<List<string>> footList = new List<List<string>>();
            sw.Write("<table class=\"display table table-striped table-hover compact\" cellspacing=\"0\" id=\"" + table_id + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th width=\"50px\">Sub</th>");
                        sw.Write("<th width=\"50px\"></th>");
                        sw.Write("<th>Name</th>");
                        foreach (Boon boon in list_to_use)
                        {
                            sw.Write("<th width=\"50px\">" + "<img src=\"" + boon.getLink() + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }                 
                    sw.Write("</tr> ");
                }           
                sw.Write("</thead>");
                HashSet<int> intensityBoon = new HashSet<int>();
                bool boonTable = list_to_use.Select(x => x.getID()).Contains(740);
                sw.Write("<tbody>");
                {
                    foreach (Player player in p_list)
                    {
                        Dictionary<int, string> boonArray = getfinalboons(player);
                        List<string> boonArrayToList = new List<string>();
                        boonArrayToList.Add(player.getGroup());
                        int count = 0;

                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>");
                            if (boonTable)
                            {
                                long fight_duration = getBossData().getLastAware() - getBossData().getFirstAware();
                                Dictionary<int, long> boonPresence = player.getBoonPresence(getBossData(), getSkillData(), getCombatData().getCombatList());
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
                                if (boon.getType() == "intensity")
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
        private void CreateGenSelfTable(StreamWriter sw, List<Boon> list_to_use, string table_id)
        { //Generate BoonGenSelf table
            sw.Write("<script> $(function () { $('#" + table_id + "').DataTable({ \"order\": [[0, \"asc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\" cellspacing=\"0\" width=\"100%\" id=\"" + table_id + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th width=\"50px\">Sub</th>");
                        sw.Write("<th width=\"50px\"></th>");
                        sw.Write("<th>Name</th>");
                        foreach (Boon boon in list_to_use)
                        {
                            sw.Write("<th width=\"50px\">" + "<img src=\"" + boon.getLink() + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }
                    
                    sw.Write("</tr>");
                }
                
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    foreach (Player player in p_list)
                    {
                        List<Player> playerID = new List<Player>();
                        playerID.Add(player);
                        Dictionary<int, string> boonArray = getfinalboons(player, playerID);
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"20\" width=\"20\" >" + "</td>");
                            sw.Write("<td>" + player.getCharacter().ToString() + "</td>");
                            foreach (Boon boon in list_to_use)
                            {
                                if (boonArray.ContainsKey(boon.getID()))
                                {
                                    sw.Write("<td>" + boonArray[boon.getID()] + "</td>");
                                }
                                else
                                {
                                    sw.Write("<td>" + 0 + "</td>");
                                }
                            }
                        }
                        sw.Write("</tr>");
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
        private void CreateGenGroupTable(StreamWriter sw, List<Boon> list_to_use, string table_id)
        { //Generate BoonGenGroup table
            sw.Write("<script> $(function () { $('#" + table_id + "').DataTable({ \"order\": [[0, \"asc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"" + table_id + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th width=\"50px\">Sub</th>");
                        sw.Write("<th width=\"50px\"></th>");
                        sw.Write("<th>Name</th>");
                        foreach (Boon boon in list_to_use)
                        {
                            sw.Write("<th width=\"50px\">" + "<img src=\"" + boon.getLink() + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }
                    
                    sw.Write("</tr>");
                }           
                sw.Write("</thead>");
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
                        Dictionary<int, string> boonArray = getfinalboons(player, playerIDS);
                        playerIDS.Clear();
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"20\" width=\"20\" >" + "</td>");
                            sw.Write("<td>" + player.getCharacter().ToString() + "</td>");


                            foreach (Boon boon in list_to_use)
                            {
                                if (boonArray.ContainsKey(boon.getID()))
                                {
                                    sw.Write("<td>" + boonArray[boon.getID()] + "</td>");
                                }
                                else
                                {
                                    sw.Write("<td>" + 0 + "</td>");
                                }
                            }
                        }                  
                        sw.Write("</tr>");
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
        private void CreateGenOGroupTable(StreamWriter sw, List<Boon> list_to_use, string table_id)
        {  //Generate BoonGenOGroup table
            sw.Write("<script> $(function () { $('#" + table_id + "').DataTable({ \"order\": [[0, \"asc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"" + table_id + "\">");
            {
                sw.Write(" <thead> <tr> <th width=\"50px\">Sub</th><th width=\"50px\"></th><th>Name</th>");
                foreach (Boon boon in list_to_use)
                {
                    sw.Write("<th width=\"50px\">" + "<img src=\"" + boon.getLink() + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                }
                sw.Write(" </tr> </thead>");
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
                        Dictionary<int, string> boonArray = getfinalboons(player, playerIDS);
                        playerIDS.Clear();
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"20\" width=\"20\" >" + "</td>");
                            sw.Write("<td>" + player.getCharacter().ToString() + "</td>");
                            foreach (Boon boon in list_to_use)
                            {
                                if (boonArray.ContainsKey(boon.getID()))
                                {
                                    sw.Write("<td>" + boonArray[boon.getID()] + "</td>");
                                }
                                else
                                {
                                    sw.Write("<td>" + 0 + "</td>");
                                }
                            }
                        }
                        sw.Write("</tr>");
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
        private void CreateGenSquadTable(StreamWriter sw, List<Boon> list_to_use, string table_id) {
            //Generate BoonGenSquad table
            sw.Write("<script> $(function () { $('#" + table_id + "').DataTable({ \"order\": [[0, \"asc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"" + table_id + "\">");
            {
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th width=\"50px\">Sub</th>");
                        sw.Write("<th width=\"50px\"></th>");
                        sw.Write("<th>Name</th>");
                        foreach (Boon boon in list_to_use)
                        {
                            sw.Write("<th width=\"50px\">" + "<img src=\"" + boon.getLink() + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>");
                        }
                    }            
                    sw.Write("</tr>");
                }           
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    List<Player> playerIDS = new List<Player>();
                    foreach (Player p in p_list)
                    {
                        playerIDS.Add(p);
                    }
                    foreach (Player player in p_list)
                    {
                        Dictionary<int, string> boonArray = getfinalboons(player, playerIDS);
                        sw.Write("<tr>");
                        {
                            sw.Write("<td>" + player.getGroup().ToString() + "</td>");
                            sw.Write("<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"20\" width=\"20\" >" + "</td>");
                            sw.Write("<td>" + player.getCharacter().ToString() + "</td>");
                            foreach (Boon boon in list_to_use)
                            {
                                if (boonArray.ContainsKey(boon.getID()))
                                {
                                    sw.Write("<td>" + boonArray[boon.getID()] + "</td>");
                                }
                                else
                                {
                                    sw.Write("<td>" + 0 + "</td>");
                                }
                            }
                        }

                        sw.Write("</tr>");
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
        private void CreatePlayerTab(StreamWriter sw, bool[] settingsSnap)
        {
            //generate Player list Graphs
            foreach (Player p in p_list)
            {
                CombatData c_data = getCombatData();
                BossData b_data = getBossData();
                List<CastLog> casting = p.getCastLogs(b_data, c_data.getCombatList(), getAgentData());
                SkillData s_data = getSkillData();
                List<SkillItem> s_list = s_data.getSkillList();
                bool died = p.getDeath(getCombatData().getCombatList()) > 0;
                string charname = p.getCharacter();
                sw.Write("<div class=\"tab-pane fade\" id=\"" + p.getInstid() + "\">");
                {
                    sw.Write("<h1 align=\"center\"> " + charname + "<img src=\"" + GetLink(p.getProf().ToString()) + " \" alt=\"" + p.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</h1>");
                    sw.Write("<ul class=\"nav nav-tabs\">");
                    {
                        sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#home" + p.getInstid() + "\">" + p.getCharacter() + "</a></li>");
                        if (SnapSettings[10])
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#SimpleRot" + p.getInstid() + "\">Simple Rotation</a></li>");

                        }
                        if (died)
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#DeathRecap" + p.getInstid() + "\">Death Recap</a></li>");

                        }
                        //foreach pet loop here                        
                        foreach (AgentItem agent in p.getMinionsDamageLogs(0, b_data, c_data.getCombatList(), getAgentData()).Keys)
                        {
                            sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#minion" + p.getInstid() + "_" + agent.getInstid() + "\">" + agent.getName() + "</a></li>");
                        }
                        //inc dmg
                        sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#incDmg" + p.getInstid() + "\">Damage Taken</a></li>");
                    }
                    sw.Write("</ul>");
                    sw.Write("<div id=\"myTabContent\" class=\"tab-content\">");
                    {
                        sw.Write("<div class=\"tab-pane fade show active\" id=\"home" + p.getInstid() + "\">");
                        {
                            List<int[]> consume = p.getConsumablesList(b_data,s_data, c_data.getCombatList());
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
                            sw.Write("<div id=\"Graph" + p.getInstid() + "\" style=\"height: 800px;width:1000px; display:inline-block \"></div>");
                            sw.Write("<script>");
                            {
                                sw.Write("var data = [");
                                {
                                    if (SnapSettings[6])//Display rotation
                                    {
                                        
                                        foreach (CastLog cl in casting)
                                        {

                                            string skillName = "";
                                            GW2APISkill skill = null;
                                            if (s_list.FirstOrDefault(x => x.getID() == cl.getID()) != null)
                                            {
                                                skill = s_list.FirstOrDefault(x => x.getID() == cl.getID()).GetGW2APISkill();
                                            }
                                            if (skill == null)
                                            {
                                                skillName = s_data.getName(cl.getID());
                                            }
                                            else
                                            {
                                                skillName = skill.name;
                                            }
                                            float dur = 0.0f;
                                            if (skillName == "Dodge")
                                            {
                                                dur = 0.5f;
                                            }
                                            else if (cl.getID() == -2)
                                            {//wepswap
                                                skillName = "Weapon Swap";
                                                dur = 0.1f;
                                            }
                                            else if (skillName == "Resurrect")
                                            {
                                                dur = cl.getActDur() / 1000f;
                                            }
                                            else if (skillName == "Bandage")
                                            {
                                                dur = cl.getActDur() / 1000f;
                                            }
                                            else
                                            {
                                                dur = cl.getActDur() / 1000f;
                                            }
                                            skillName = skillName.Replace("\"", "");
                                            sw.Write("{");
                                            {
                                                
                                                    sw.Write("y: ['1.5'],");
                                               
                                                
                                               sw.Write(
                                                      "x: ['" + dur + "']," +
                                                      "base:'" + cl.getTime() / 1000f + "'," +
                                                      "name: \"" + skillName + " " + dur + "s\"," +//get name should be handled by api
                                                      "orientation:'h'," +
                                                      "mode: 'markers'," +
                                                      "type: 'bar',");
                                                if (skill != null)
                                                {
                                                    if (skill.slot == "Weapon_1")
                                                    {
                                                        sw.Write("width:'0.5',");
                                                    }
                                                    else
                                                    {
                                                        sw.Write("width:'1',");
                                                    }

                                                }
                                                else
                                                {
                                                    sw.Write("width:'1',");
                                                }
                                                sw.Write("hoverinfo: 'name'," +
                                                        "hoverlabel:{namelength:'-1'},");
                                                sw.Write("marker: {");
                                                {
                                                    if (cl.endActivation() != null)
                                                    {
                                                        if (cl.endActivation().getID() == 3)
                                                        {
                                                            sw.Write("color: 'rgb(40,40,220)',");
                                                        }
                                                        else if (cl.endActivation().getID() == 4)
                                                        {
                                                            sw.Write("color: 'rgb(220,40,40)',");
                                                        }
                                                        else if (cl.endActivation().getID() == 5)
                                                        {
                                                            sw.Write("color: 'rgb(40,220,40)',");
                                                        }
                                                        else
                                                        {
                                                            sw.Write("color: 'rgb(220,220,0)',");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        sw.Write("color: 'rgb(220,220,0)',");
                                                    }
                                                    sw.Write("width: '5',");
                                                    sw.Write("line:{");
                                                    {
                                                        if (cl.startActivation() != null)
                                                        {
                                                            if (cl.startActivation().getID() == 1)
                                                            {
                                                                sw.Write("color: 'rgb(20,20,20)',");
                                                            }
                                                            else if (cl.startActivation().getID() == 2)
                                                            {
                                                                sw.Write("color: 'rgb(220,40,220)',");
                                                            }
                                                        }
                                                        sw.Write("width: '1'");
                                                    }
                                                    sw.Write("}");
                                                }
                                                sw.Write("},");
                                                sw.Write("showlegend: false");
                                            }
                                            sw.Write(" },");
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
                                        Dictionary<int, BoonsGraphModel> boonGraphData = p.getBoonGraphs(getBossData(), getSkillData(), getCombatData().getCombatList());
                                        foreach (int boonid in boonGraphData.Keys.Reverse())
                                        {
                                            if (parseBoonsList.FirstOrDefault(x => x.getID() == boonid) != null || boonid == -2)
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
                                        List<int[]> playerbossdpsgraphdata = getBossDPSGraph(p);
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
                                            List<int[]> playertotaldpsgraphdata = getTotalDPSGraph(p);
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
                                                string skillIcon = "";
                                                GW2APISkill skill = null;
                                                if (s_list.FirstOrDefault(x => x.getID() == cl.getID()) != null)
                                                {
                                                    skill = s_list.FirstOrDefault(x => x.getID() == cl.getID()).GetGW2APISkill();
                                                }
                                                if (skill != null && cl.getID() != -2)
                                                {
                                                    if (skill.slot != "Weapon_1")
                                                    {
                                                        skillIcon = skill.icon;
                                                        sw.Write("{" +
                                                                     "source: '" + skillIcon + "'," +
                                                                     "xref: 'x'," +
                                                                     "yref: 'y'," +
                                                                     "x: " + (cl.getTime() / 1000f) + "," +
                                                                     "y: 0," +
                                                                     "sizex: 1.1," +
                                                                     "sizey: 1.1," +
                                                                     "xanchor: 'left'," +
                                                                     "yanchor: 'bottom'" +
                                                                "}");
                                                    }
                                                }
                                                else
                                                {
                                                    string skillName = "";

                                                    if (cl.getID() == -2)
                                                    { //wepswap
                                                        skillName = "Weapon Swap";
                                                        // skillIcon = "https://wiki.guildwars2.com/images/archive/c/ce/20140606174035%21Weapon_Swap_Button.png";
                                                    }
                                                    else
                                                    {
                                                        skillName = skill_data.getName(cl.getID());
                                                    }


                                                    if (skillName == "Dodge")
                                                    {
                                                        // skillIcon = "https://wiki.guildwars2.com/images/c/cc/Dodge_Instructor.png";
                                                    }
                                                    else if (skillName == "Resurrect")
                                                    {
                                                        //skillIcon = "https://wiki.guildwars2.com/images/archive/d/dd/20120611120554%21Downed.png";
                                                    }
                                                    else if (skillName == "Bandage")
                                                    {
                                                        // skillIcon = "https://wiki.guildwars2.com/images/0/0c/Bandage.png";
                                                    }
                                                    sw.Write("{" +
                                                                  "source: '" + skillIcon + "'," +
                                                                  "xref: 'x'," +
                                                                  "yref: 'y'," +
                                                                  "x: " + cl.getTime() / 1000f + "," +
                                                                  "y: 0," +
                                                                  "sizex: 1.1," +
                                                                  "sizey: 1.1," +
                                                                  "xanchor: 'left'," +
                                                                  "yanchor: 'bottom'" +
                                                              "}");
                                                }
                                                if (castCount == casting.Count - 1)
                                                {
                                                }
                                                else
                                                {
                                                    sw.Write(",");
                                                }
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
                                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#distTabAll" + p.getInstid() +"\">" + "All" + "</a></li>");
                                sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#distTabBoss" + p.getInstid() + "\">" + "Boss" + "</a></li>");
                            }
                            sw.Write("</ul>");
                            sw.Write("<div class=\"tab-content\">");
                            {
                                sw.Write("<div class=\"tab-pane fade show active\" id=\"distTabAll" + p.getInstid() + "\">");
                                {
                                    CreateDMGDistTable(sw, p, false);
                                }
                                sw.Write("</div>");
                                sw.Write("<div class=\"tab-pane fade\" id=\"distTabBoss" + p.getInstid() + "\">");
                                {
                                    CreateDMGDistTable(sw, p, true);
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("</div>");
                        foreach (AgentItem agent in p.getMinionsDamageLogs(0, b_data, c_data.getCombatList(), getAgentData()).Keys)
                        {
                            string id = p.getInstid() + "_" + agent.getInstid();
                            sw.Write("<div class=\"tab-pane fade \" id=\"minion" + id + "\">");
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
                                        CreateDMGDistTable(sw, p, agent, false);
                                    }
                                    sw.Write("</div>");
                                    sw.Write("<div class=\"tab-pane fade\" id=\"distTabBoss" + id + "\">");
                                    {
                                        CreateDMGDistTable(sw, p, agent, true);
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");
                            }
                            sw.Write("</div>");
                        }
                        if (SnapSettings[10])
                        {
                            sw.Write("<div class=\"tab-pane fade \" id=\"SimpleRot" + p.getInstid() + "\">");
                            {
                                int simpleRotSize = 20;
                                if (settingsSnap[12])
                                {
                                    simpleRotSize = 30;
                                }
                                CreateSimpleRotationTab(sw, p,simpleRotSize);
                            }
                            sw.Write("</div>");
                        }
                        if (died)
                        {
                            sw.Write("<div class=\"tab-pane fade \" id=\"DeathRecap" + p.getInstid() + "\">");
                            {
                                CreateDeathRecap(sw, p);
                            }
                            sw.Write("</div>");
                        }
                        sw.Write("<div class=\"tab-pane fade \" id=\"incDmg" + p.getInstid() + "\">");
                        {
                            CreateDMGTakenDistTable(sw, p);
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
        private void CreateSimpleRotationTab(StreamWriter sw,Player p,int simpleRotSize) {
            if (SnapSettings[6])//Display rotation
            {
                SkillData s_data = getSkillData();
                List<SkillItem> s_list = s_data.getSkillList();
                CombatData c_data = getCombatData();
                BossData b_data = getBossData();
                List<CastLog> casting = p.getCastLogs(b_data, c_data.getCombatList(), getAgentData());
                GW2APISkill autoSkill = null;
                int autosCount = 0;
                foreach (CastLog cl in casting)
                {
                    GW2APISkill apiskill = null;
                    SkillItem skill = s_list.FirstOrDefault(x => x.getID() == cl.getID());
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
            CombatData c_data = getCombatData();
            BossData b_data = getBossData();
            AgentData a_data = getAgentData();
            List<DamageLog> damageLogs = p.getDamageTakenLogs(b_data, c_data.getCombatList(), getAgentData(), getMechData());
            SkillData s_data = getSkillData();
            List<SkillItem> s_list = s_data.getSkillList();
            List<Point> down = getCombatData().getStates(p.getInstid(), "CHANGE_DOWN");
            if (down.Count > 0)
            {
                List<Point> ups = getCombatData().getStates(p.getInstid(), "CHANGE_UP");
                down = down.GetRange(ups.Count(), down.Count()-ups.Count());
            }
            List<Point> dead = getCombatData().getStates(p.getInstid(), "CHANGE_DEAD");
            List<DamageLog> damageToDown = new List<DamageLog>();
            List<DamageLog> damageToKill = new List<DamageLog>(); ;
            if (down.Count > 0)
            {//went to down state before death
                damageToDown = damageLogs.Where(x => x.getTime() < down.Last().X - b_data.getFirstAware() && x.getDamage() > 0).ToList();
                damageToKill = damageLogs.Where(x => x.getTime() > down.Last().X - b_data.getFirstAware() && x.getTime() < dead.Last().X - b_data.getFirstAware() && x.getDamage() > 0).ToList();
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
                damageToKill = damageLogs.Where(x =>  x.getTime() < dead.Last().X && x.getDamage() > 0).ToList();
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
            
            sw.Write("<center><div id=\"BarDeathRecap" + p.getInstid()+"\"></div> </center>");
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
                        sw.Write("'" + a_data.GetAgentWInst(damageToDown[d].getInstidt()).getName().Replace("\0", "").Replace("\'", "\\'") + "<br>"+
                            s_data.getName( damageToDown[d].getID()).Replace("\'", "\\'") + " hit you for "+damageToDown[d].getDamage() + "',");
                    }
                }
                for (int d = 0; d < damageToKill.Count(); d++)
                {
                    sw.Write("'" + a_data.GetAgentWInst(damageToKill[d].getInstidt()).getName().Replace("\0","").Replace("\'", "\\'") + "<br>" +
                           "hit you with <b>"+ s_data.getName(damageToKill[d].getID()).Replace("\'", "\\'") + "</b> for " + damageToKill[d].getDamage() + "'");

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
                sw.Write("Plotly.newPlot('BarDeathRecap" + p.getInstid() + "', data, layout);");
                
            }
            sw.Write("</script>");
        }
        /// <summary>
        /// Creates the damage distribution table for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private void CreateDMGDistTable(StreamWriter sw, Player p, bool toBoss)
        {
            CombatData c_data = getCombatData();
            BossData b_data = getBossData();
            List<CastLog> casting = p.getCastLogs(b_data, c_data.getCombatList(), getAgentData());
            List<DamageLog> damageLogs = p.getJustPlayerDamageLogs(toBoss ? b_data.getInstid() : 0, b_data, c_data.getCombatList(), getAgentData());
            int totalDamage = toBoss ? Int32.Parse(getFinalDPS(p).Split('|')[7]) : Int32.Parse(getFinalDPS(p).Split('|')[1]);
            int finalTotalDamage = damageLogs.Sum(x => x.getDamage());
            if (totalDamage > 0)
            {
                string contribution = String.Format("{0:0.00}", 100.0 * finalTotalDamage / totalDamage);
                sw.Write("<div>" + p.getCharacter() + " did " + contribution + "% of its own total " + (toBoss ? "boss " : "") + "dps</div>");
            }
            string tabid = p.getInstid() + (toBoss ? "_boss" : "");
            sw.Write("<script> $(function () { $('#dist_table_" + tabid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dist_table_" + tabid + "\">");
            {
                SkillData s_data = getSkillData();
                List<SkillItem> s_list = s_data.getSkillList();
                sw.Write("<thead>");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Skill</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>Damage</th>");
                        sw.Write("<th>Min</th>");
                        sw.Write("<th>Avg</th>");
                        sw.Write("<th>Max</th>");
                        sw.Write("<th>Casts</th>");
                        sw.Write("<th>Hits</th>");
                        sw.Write("<th>Hits per Cast</th>");
                        sw.Write("<th>Crit</th>");
                        sw.Write("<th>Flank</th>");
                        sw.Write("<th>Glance</th>");
                        sw.Write("<th>Wasted</th>");
                        sw.Write("<th>Saved</th>");
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</thead>");
                sw.Write("<tbody>");
                {
                    HashSet<int> usedIDs = new HashSet<int>();
                    foreach (int id in casting.Select(x => x.getID()).Distinct())
                    {//foreach casted skill
                        usedIDs.Add(id);
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
                                    sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>" + "<td>" + totaldamage + "</td>");
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
                                    sw.Write("<td>" + hpcast + "</td>");
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
                                    sw.Write("<td></td>");
                                    sw.Write("<td>" + casts + "</td>");
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
                                    sw.Write("<td></td>");
                                    sw.Write("<td>" + casts + "</td>");
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
                    foreach (Boon condi in Boon.getCondiBoonList())
                    {
                        int totaldamage = 0;
                        int mindamage = 0;
                        int avgdamage = 0;
                        int hits = 0;
                        int maxdamage = 0;
                        int condiID = condi.getID();
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
                        if (totaldamage != 0)
                        {
                            string condiName = condi.getName();// Boon.getCondiName(condiID);
                            sw.Write("<tr class=\"condi\">");
                            {
                                sw.Write("<td align=\"left\"><img src=" + condi.getLink() + " alt=\"" + condiName + "\" title=\"" + condiID + "\" height=\"18\" width=\"18\">" + condiName + "</td>");
                                sw.Write("<td>" + String.Format("{0:0.00}", 100 * (double)totaldamage / finalTotalDamage) + "%</td>");
                                sw.Write("<td>" + totaldamage + "</td>");
                                sw.Write("<td>" + mindamage + "</td>");
                                sw.Write("<td>" + avgdamage + "</td>");
                                sw.Write("<td>" + maxdamage + "</td>");
                                sw.Write("<td></td>");
                                sw.Write("<td>" + hits + "</td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                                sw.Write("<td></td>");
                            }
                            sw.Write("</tr>");
                        }
                    }
                    List<int> remainIDs = damageLogs.Where(x => !usedIDs.Contains(x.getID())).Select(x => x.getID()).Distinct().ToList();
                    foreach (int id in remainIDs)//Foreach instant cast skill
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

                sw.Write("<tfoot class=\"text-dark\">");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Total</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th>" + finalTotalDamage + "</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</tfoot>");

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
        private void CreateDMGDistTable(StreamWriter sw, Player p, AgentItem agent, bool toBoss)
        {
            int totalDamage = toBoss ? Int32.Parse(getFinalDPS(p).Split('|')[7]) : Int32.Parse(getFinalDPS(p).Split('|')[1]);
            string tabid = p.getInstid() + "_" + agent.getInstid() + (toBoss ? "_boss" : "");
            CombatData c_data = getCombatData();
            BossData b_data = getBossData();
            List<DamageLog> damageLogs = p.getMinionsDamageLogs(toBoss ? b_data.getInstid() : 0, b_data, c_data.getCombatList(), getAgentData())[agent];
            int finalTotalDamage = damageLogs.Sum(x => x.getDamage());
            if (totalDamage > 0)
            {
                string contribution = String.Format("{0:0.00}", 100.0 * finalTotalDamage / totalDamage);
                sw.Write("<div>" + agent.getName() + " did " + contribution + "% of " + p.getCharacter() + "'s total " + (toBoss ? "boss " : "") + "dps</div>");
            }
            sw.Write("<script> $(function () { $('#dist_table_" + tabid + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dist_table_" + tabid + "\">");
            {
                SkillData s_data = getSkillData();
                List<SkillItem> s_list = s_data.getSkillList();
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
                    //CONDIS
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
                        if (totaldamage != 0)
                        {
                            string condiName = condi.getName();// Boon.getCondiName(condiID);
                            sw.Write("<tr class=\"condi\">");
                            {
                                sw.Write("<td align=\"left\"><img src=" + condi.getLink() + " alt=\"" + condiName + "\" title=\"" + condiID + "\" height=\"18\" width=\"18\">" + condiName + "</td>");
                                sw.Write("<td>" + totaldamage + "</td>");
                                sw.Write("<td>" + String.Format("{0:0.00}", (100 * (double)totaldamage / finalTotalDamage)) + "%</td>");
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
                    foreach (int id in damageLogs.Where(x => !usedIDs.Contains(x.getID())).Select(x => x.getID()).Distinct())
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
                            hits++;
                            int result = dl.getResult().getID();
                            if (result == 1) { crit++; } else if (result == 2) { glance++; }
                            if (dl.isFlanking() == 1) { flank++; }
                        }
                        avgdamage = (int)(totaldamage / (double)hits);
                        if (skill != null)
                        {
                            if (totaldamage != 0 && skill.GetGW2APISkill() != null)
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
                            else if (totaldamage != 0)
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
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
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
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                    sw.Write("<td></td>");
                                }
                                sw.Write("</tr>");
                            }
                        }
                    }

                }
                sw.Write("</tbody>");
                sw.Write("<tfoot class=\"text-dark\">");
                {
                    sw.Write("<tr>");
                    {
                        sw.Write("<th>Total</th>");
                        sw.Write("<th>" + finalTotalDamage + "</th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                        sw.Write("<th></th>");
                    }
                    sw.Write("</tr>");
                }
                sw.Write("</tfoot>");
            }
            sw.Write("</table>");
        }
        /// <summary>
        /// Create the damage taken distribution table for a given player
        /// </summary>
        /// <param name="sw">Stream writer</param>
        /// <param name="p">The player</param>
        private void CreateDMGTakenDistTable(StreamWriter sw, Player p)
        {
            CombatData c_data = getCombatData();
            BossData b_data = getBossData();
            List<DamageLog> damageLogs = p.getDamageTakenLogs( b_data, c_data.getCombatList(), getAgentData(),getMechData());
            SkillData s_data = getSkillData();
            List<SkillItem> s_list = s_data.getSkillList();
            int finalTotalDamage = damageLogs.Sum(x => x.getDamage());
            sw.Write("<script> $(function () { $('#distTaken_table_" + p.getInstid() + "').DataTable({\"columnDefs\": [ { \"title\": \"Skill\", className: \"dt-left\", \"targets\": [ 0 ]}], \"order\": [[2, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"distTaken_table_" + p.getInstid() + "\">");
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
        private void CreateMechanicTable(StreamWriter sw) {
            List<Mechanic> presMech = new List<Mechanic>();
            foreach (Mechanic item in mech_data.GetMechList(boss_data.getID()))
            {
                if (mech_data.GetMDataLogs().FirstOrDefault(x => x.GetSkill() == item.GetSkill()) != null)
                {
                    presMech.Add(item);
                }
            }
            if (presMech.Count() > 0)
            {
                sw.Write("<script> $(function () { $('#mech_table').DataTable({ \"order\": [[0, \"desc\"]]});});</script>");
                sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"mech_table\">");
                {
                    sw.Write("<thead>");
                    {
                        sw.Write("<tr>");
                        {
                            sw.Write("<th>Player</th>");
                            foreach (string mechalt in presMech.Select(x => x.GetAltName()).Distinct().ToList())
                            {
                                sw.Write("<th><span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + presMech.First(x => x.GetAltName() == mechalt).GetName() + "\">" + mechalt + "</span></th>");
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
                                foreach (string mechalt in presMech.Select(x => x.GetAltName()).Distinct().ToList())
                                {
                                    int count = 0;
                                    foreach (Mechanic mech in mech_data.GetMechList(boss_data.getID()).Where(x => mechalt == x.GetAltName()))
                                    {
                                        List<MechanicLog> test = mech_data.GetMDataLogs().Where(x => x.GetSkill() == mech.GetSkill() && x.GetPlayer() == p).ToList();
                                        count += mech_data.GetMDataLogs().Where(x => x.GetSkill() == mech.GetSkill() && x.GetPlayer() == p).Count();
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
                SkillData s_data = getSkillData();
                foreach (SkillItem skill in s_data.getSkillList())
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
        private void CreateCondiUptimeTable(StreamWriter sw,Player boss)
        {
            //Generate Boon table------------------------------------------------------------------------------------------------
            sw.Write("<script> $(function () { $('#condi_table').DataTable({ \"order\": [[3, \"desc\"]]});});</script>");
            sw.Write("<table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"condi_table\">");
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
                        Dictionary<int, string> boonArray = getfinalcondis(boss);
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
        private void CreateBossSummary(StreamWriter sw)
        {
            //generate Player list Graphs
            AgentItem bossAgent = agent_data.GetAgent(boss_data.getAgent());
            Boss boss = new Boss(bossAgent);
            CombatData c_data = getCombatData();
            BossData b_data = getBossData();
            List<CastLog> casting = boss.getCastLogs(b_data, c_data.getCombatList(), getAgentData());
            SkillData s_data = getSkillData();
            List<SkillItem> s_list = s_data.getSkillList();
            string charname = boss.getCharacter();
            sw.Write("<h1 align=\"center\"> " + charname + "</h1>");
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#home" + boss.getInstid() + "\">" + boss.getCharacter() + "</a></li>");
                //foreach pet loop here
                foreach (AgentItem agent in boss.getMinionsDamageLogs(0, b_data, c_data.getCombatList(), getAgentData()).Keys)
                {
                    sw.Write("<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#minion" + boss.getInstid() + "_" + agent.getInstid() + "\">" + agent.getName() + "</a></li>");
                }
            }         
            sw.Write("</ul>");
            //condi stats tab
            sw.Write("<div id=\"myTabContent\" class=\"tab-content\"><div class=\"tab-pane fade show active\" id=\"home" + boss.getInstid() + "\">");
            {
                CreateCondiUptimeTable(sw, boss);
                sw.Write("<div id=\"Graph" + boss.getInstid() + "\" style=\"height: 800px;width:1000px; display:inline-block \"></div>");
                sw.Write("<script>");
                {
                    sw.Write("var data = [");
                    {
                        if (SnapSettings[6])//Display rotation
                        {

                            foreach (CastLog cl in casting)
                            {

                                string skillName = "";
                                GW2APISkill skill = null;
                                if (s_list.FirstOrDefault(x => x.getID() == cl.getID()) != null)
                                {
                                    skill = s_list.FirstOrDefault(x => x.getID() == cl.getID()).GetGW2APISkill();
                                }
                                if (skill == null)
                                {
                                    skillName = s_data.getName(cl.getID());
                                }
                                else
                                {
                                    skillName = skill.name;
                                }
                                float dur = 0.0f;
                                if (skillName == "Dodge")
                                {
                                    dur = 0.5f;
                                }
                                else if (cl.getID() == -2)
                                {//wepswap
                                    skillName = "Weapon Swap";
                                    dur = 0.1f;
                                }
                                else if (skillName == "Resurrect")
                                {
                                    dur = cl.getActDur() / 1000f;
                                }
                                else if (skillName == "Bandage")
                                {
                                    dur = cl.getActDur() / 1000f;
                                }
                                else
                                {
                                    dur = cl.getActDur() / 1000f;
                                }
                                skillName = skillName.Replace("\"", "");
                                sw.Write("{");
                                {

                                    sw.Write("y: ['1.5'],");


                                    sw.Write(
                                           "x: ['" + dur + "']," +
                                           "base:'" + cl.getTime() / 1000f + "'," +
                                           "name: \"" + skillName + " " + dur + "s\"," +//get name should be handled by api
                                           "orientation:'h'," +
                                           "mode: 'markers'," +
                                           "type: 'bar',");
                                    if (skill != null)
                                    {
                                        if (skill.slot == "Weapon_1")
                                        {
                                            sw.Write("width:'0.5',");
                                        }
                                        else
                                        {
                                            sw.Write("width:'1',");
                                        }

                                    }
                                    else
                                    {
                                        sw.Write("width:'1',");
                                    }
                                    sw.Write("hoverinfo: 'name'," +
                                            "hoverlabel:{namelength:'-1'},");
                                    sw.Write("marker: {");
                                    {
                                        if (cl.endActivation() != null)
                                        {
                                            if (cl.endActivation().getID() == 3)
                                            {
                                                sw.Write("color: 'rgb(40,40,220)',");
                                            }
                                            else if (cl.endActivation().getID() == 4)
                                            {
                                                sw.Write("color: 'rgb(220,40,40)',");
                                            }
                                            else if (cl.endActivation().getID() == 5)
                                            {
                                                sw.Write("color: 'rgb(40,220,40)',");
                                            }
                                            else
                                            {
                                                sw.Write("color: 'rgb(220,220,0)',");
                                            }
                                        }
                                        else
                                        {
                                            sw.Write("color: 'rgb(220,220,0)',");
                                        }
                                        sw.Write("width: '5',");
                                        sw.Write("line:{");
                                        {
                                            if (cl.startActivation() != null)
                                            {
                                                if (cl.startActivation().getID() == 1)
                                                {
                                                    sw.Write("color: 'rgb(20,20,20)',");
                                                }
                                                else if (cl.startActivation().getID() == 2)
                                                {
                                                    sw.Write("color: 'rgb(220,40,220)',");
                                                }
                                            }
                                            sw.Write("width: '1'");
                                        }
                                        sw.Write("}");
                                    }
                                    sw.Write("},");
                                    sw.Write("showlegend: false");
                                }
                                sw.Write(" },");
                            }
                        }
                        //============================================
                        List<Boon> parseBoonsList = new List<Boon>();
                        //Condis
                        parseBoonsList.AddRange(Boon.getCondiBoonList());
                        //Every buffs and boons
                        parseBoonsList.AddRange(Boon.getAllBuffList());
                        Dictionary<int,BoonsGraphModel> boonGraphData = boss.getBoonGraphs(getBossData(), getSkillData(),getCombatData().getCombatList());
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
                            List<int[]> playertotaldpsgraphdata = getTotalDPSGraph(boss);
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
                                    string skillIcon = "";
                                    GW2APISkill skill = null;
                                    if (s_list.FirstOrDefault(x => x.getID() == cl.getID()) != null)
                                    {
                                        skill = s_list.FirstOrDefault(x => x.getID() == cl.getID()).GetGW2APISkill();
                                    }
                                    if (skill != null && cl.getID() != -2)
                                    {
                                        if (skill.slot != "Weapon_1")
                                        {
                                            skillIcon = skill.icon;
                                            sw.Write("{" +
                                                      "source: '" + skillIcon + "'," +
                                                      "xref: 'x'," +
                                                      "yref: 'y'," +
                                                      "x: " + (cl.getTime() / 1000f) + "," +
                                                      "y: 0," +
                                                      "sizex: 1.1," +
                                                      "sizey: 1.1," +
                                                      "xanchor: 'left'," +
                                                      "yanchor: 'bottom'" +
                                                  "}");
                                        }
                                    }
                                    else
                                    {
                                        string skillName = "";
                                        if (cl.getID() == -2)
                                        { //wepswap
                                            skillName = "Weapon Swap";
                                            // skillIcon = "https://wiki.guildwars2.com/images/archive/c/ce/20140606174035%21Weapon_Swap_Button.png";
                                        }
                                        else
                                        {
                                            skillName = skill_data.getName(cl.getID());
                                        }
                                        if (skillName == "Dodge")
                                        {
                                            // skillIcon = "https://wiki.guildwars2.com/images/c/cc/Dodge_Instructor.png";
                                        }
                                        else if (skillName == "Resurrect")
                                        {
                                            //skillIcon = "https://wiki.guildwars2.com/images/archive/d/dd/20120611120554%21Downed.png";
                                        }
                                        else if (skillName == "Bandage")
                                        {
                                            // skillIcon = "https://wiki.guildwars2.com/images/0/0c/Bandage.png";
                                        }
                                        sw.Write("{" +
                                                      "source: '" + skillIcon + "'," +
                                                      "xref: 'x'," +
                                                      "yref: 'y'," +
                                                      "x: " + cl.getTime() / 1000f + "," +
                                                      "y: 0," +
                                                      "sizex: 1.1," +
                                                      "sizey: 1.1," +
                                                      "xanchor: 'left'," +
                                                      "yanchor: 'bottom'" +
                                                  "}");
                                    }
                                    if (castCount == casting.Count - 1)
                                    {
                                    }
                                    else
                                    {
                                        sw.Write(",");
                                    }
                                }
                            }
                        }
                        sw.Write("],");
                        sw.Write("font: { color: '#ffffff' }," +
                                "paper_bgcolor: 'rgba(0,0,0,0)'," +
                                "plot_bgcolor: 'rgba(0,0,0,0)'");
                    }
                    sw.Write("};");
                    sw.Write("Plotly.newPlot('Graph" + boss.getInstid() + "', data, layout);");
                }
                sw.Write("</script> ");
                CreateDMGDistTable(sw, boss, false);
                sw.Write("</div>");
                foreach (AgentItem agent in boss.getMinionsDamageLogs(0, b_data, c_data.getCombatList(), getAgentData()).Keys)
                {
                    sw.Write("<div class=\"tab-pane fade \" id=\"minion" + boss.getInstid() + "_" + agent.getInstid() + "\">");
                    {
                        CreateDMGDistTable(sw, boss, agent, false);
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
        private void CreateEstimateTabs(StreamWriter sw)
        {
            sw.Write("<ul class=\"nav nav-tabs\">");
            {
                sw.Write("<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est_role\">Roles</a>" +
                        "</li>" +

                        "<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est_cc\">CC</a>" +
                        "</li>" +
                         "<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#est\">Maybe more</a>" +
                        "</li>");
            }
            sw.Write("</ul>");
            sw.Write("<div id=\"myTabContent\" class=\"tab-content\">");
            {
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est_role\">");
                {
                    //Use cards
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est_cc\">");
                {
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade show active\" id=\"est\">");
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
            BossData b_data = getBossData();
            double fight_duration = (b_data.getLastAware() - b_data.getFirstAware()) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fight_duration);
            string durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s";
            if (duration.ToString("hh") != "00")
            {
                durationString = duration.ToString("hh") + "h " + durationString;
            }
            string bossname = FilterStringChars(b_data.getName());
            setPresentBoons(settingsSnap);
            string Html_playerDropdown = "";
            foreach (Player p in p_list)
            {

                string charname = p.getCharacter();
                Html_playerDropdown += "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#" + p.getInstid() + "\">" + charname +
                    "<img src=\"" + GetLink(p.getProf().ToString()) + " \" alt=\"" + p.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</a>";
            }
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
                                                    sw.Write("<img src=\"" + GetLink(b_data.getID() + "-icon") + " \"alt=\"" + bossname + "-icon" + "\" style=\"height: 120px; width: 120px;\" >");
                                                }
                                                sw.Write("</div>");
                                                sw.Write("<div>");
                                                {
                                                    sw.Write("<div class=\"progress\" style=\"width: 100 %; height: 20px;\">");
                                                    {
                                                        if (log_data.getBosskill())
                                                        {
                                                            string tp = getBossData().getHealth().ToString() + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-success\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:100%; ;\" aria-valuenow=\"100\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                        }
                                                        else
                                                        {
                                                            double finalPercent = 0;
                                                            if (boss_data.getHealthOverTime().Count > 0)
                                                            {
                                                                finalPercent = 100.0 - boss_data.getHealthOverTime()[boss_data.getHealthOverTime().Count - 1][1] * 0.01;
                                                            }
                                                            string tp = Math.Round(getBossData().getHealth() * finalPercent / 100.0) + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-success\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:" + finalPercent + "%;\" aria-valuenow=\""+ finalPercent+"\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                            tp = Math.Round(getBossData().getHealth() * (100.0-finalPercent) / 100.0) + " Health";
                                                            sw.Write("<div class=\"progress-bar bg-danger\" data-toggle=\"tooltip\" title=\"" + tp + "\" role=\"progressbar\" style=\"width:" + (100.0 - finalPercent) + "%;\" aria-valuenow=\""+ (100.0 - finalPercent )+ "\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>");
                                                            
                                                        }
                                                    }
                                                    sw.Write("</div>");
                                                    sw.Write("<p class=\"small\" style=\"text-align:center; color: #FFF;\">" + getBossData().getHealth().ToString() + " Health</p>");
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
                        //if (p_list.Count() == 1)//Create condensed version of log
                        //{
                        //    CreateSoloHTML(sw,settingsSnap);
                        //    return;
                        //}
                        sw.Write("</div>");
                        sw.Write("<ul class=\"nav nav-tabs\">");
                        {
                            sw.Write("<li class=\"nav-item\">" +
                                        "<a class=\"nav-link active\" data-toggle=\"tab\" href=\"#stats\">Stats</a>" +
                                    "</li>" + 
                                    
                                    "<li class=\"nav-item\">" +
                                        "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#dmgGraph\">Damage Graph</a>" +
                                    "</li>" +
                                     "<li class=\"nav-item\">" +
                                        "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#boons\">Boons</a>" +
                                    "</li>" +
                                   
                                    
                                    "<li class=\"nav-item\">" +
                                        "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#mechTable\">Mechanics</a>" +
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
                                                "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#bossSummary\">Boss</a>" +
                                            "</li>");
                            }
                            if (settingsSnap[8])
                            {
                                sw.Write("<li class=\"nav-item\">" +
                                                "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#eventList\">Event List</a>" +
                                            "</li>");
                            }
                            if (settingsSnap[13])
                            {
                                sw.Write("<li class=\"nav-item\">" +
                                                "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#estimates\">Estimates</a>" +
                                            "</li>");
                            }
                        }
                        sw.Write("</ul>");
                        sw.Write("<div id=\"myTabContent\" class=\"tab-content\">");
                        {
                            sw.Write("<div class=\"tab-pane fade show active\" id=\"stats\">");
                            {
                                //Stats Tab
                                sw.Write("<h3 align=\"center\"> Stats </h3>");

                                sw.Write("<ul class=\"nav nav-tabs\">"+
                                        "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#dpsStats\">DPS</a></li>"+
                                        "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offStats\">Damage Stats</a></li>"+
                                        "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defStats\">Defensive Stats</a></li>"+
                                        "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#healStats\">Heal Stats</a></li>"+
                                    "</ul>");
                                sw.Write("<div id=\"statsSubTab\" class=\"tab-content\">");
                                {
                                    sw.Write("<div class=\"tab-pane fade show active\" id=\"dpsStats\">");
                                    {
                                        // DPS table
                                        CreateDPSTable(sw, fight_duration);
                                    }
                                    sw.Write("</div>");
                                    sw.Write("<div class=\"tab-pane fade \" id=\"offStats\">");
                                    {
                                        // HTML_dmgstats 
                                        CreateDMGStatsTable(sw, fight_duration);
                                    }
                                    sw.Write("</div>");
                                    sw.Write("<div class=\"tab-pane fade \" id=\"defStats\">");
                                    {
                                        // def stats
                                        CreateDefTable(sw, fight_duration);
                                    }
                                    sw.Write("</div>");
                                    sw.Write("<div class=\"tab-pane fade\" id=\"healStats\">");
                                    {
                                        //  HTML_supstats
                                        CreateSupTable(sw, fight_duration);
                                    }
                                    sw.Write("</div>");
                                }
                                sw.Write("</div>");

                            }
                            sw.Write("</div>");
                           
                            sw.Write("<div class=\"tab-pane fade\" id=\"dmgGraph\">");
                            {
                                //Html_dpsGraph
                                CreateDPSGraph(sw);
                            }
                            sw.Write("</div>");
                            //Boon Stats
                            sw.Write("<div class=\"tab-pane fade \" id=\"boons\">");
                            {
                                //Boons Tab
                                sw.Write("<h3 align=\"center\"> Boons </h3>");

                                sw.Write("<ul class=\"nav nav-tabs\">" +
                                        "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#mainBoon\">Boons</a></li>" +
                                        "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offBuff\">Damage Buffs</a></li>" +
                                        "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defBuff\">Defensive Buffs</a></li>" +
                                    "</ul>");
                                sw.Write("<div id=\"boonsSubTab\" class=\"tab-content\">");
                                {
                                    sw.Write("<div class=\"tab-pane fade show active  \" id=\"mainBoon\">");
                                    {
                                        sw.Write("<ul class=\"nav nav-tabs\">" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#boonsUptime\">Uptime</a></li>" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenSelf\">Generation (Self)</a></li>" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenGroup\">Generation (Group)</a></li>" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenOGroup\">Generation (Off-Group)</a></li>" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#boonsGenSquad\">Generation (Squad)</a></li>" +
                                               "</ul>");
                                        sw.Write("<div id=\"mainBoonsSubTab\" class=\"tab-content\">");
                                        {
                                            sw.Write("<div class=\"tab-pane fade show active\" id=\"boonsUptime\">");
                                            {
                                                sw.Write("<p> Boon Uptime</p>");
                                                // Html_boons
                                                CreateUptimeTable(sw, present_boons, "boons_table");
                                            }
                                            sw.Write("</div>");
                                            sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenSelf\">");
                                            {
                                                //Html_boonGenSelf
                                                sw.Write("<p> Boons generated by a character for themselves</p>");
                                                CreateGenSelfTable(sw, present_boons, "boongenself_table");
                                            }
                                            sw.Write("</div>");
                                            sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenGroup\">");
                                            {
                                                sw.Write("<p> Boons generated by a character for their sub group</p>");
                                                // Html_boonGenGroup
                                                CreateGenGroupTable(sw, present_boons, "boongengroup_table");
                                            }
                                            sw.Write("</div>");
                                            sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenOGroup\">");
                                            {
                                                sw.Write("<p> Boons generated by a character for any subgroup that is not their own</p>");
                                                // Html_boonGenOGroup
                                                CreateGenOGroupTable(sw, present_boons, "boongenogroup_table");
                                            }
                                            sw.Write("</div>");
                                            sw.Write("<div class=\"tab-pane fade\" id=\"boonsGenSquad\">");
                                            {
                                                sw.Write("<p> Boons generated by a character for the entire squad</p>");
                                                //  Html_boonGenSquad
                                                CreateGenSquadTable(sw, present_boons, "boongensquad_table");
                                            }
                                            sw.Write("</div>");
                                        }
                                        sw.Write("</div>");
                                    }
                                    sw.Write("</div>");
                                    sw.Write("<div class=\"tab-pane fade  \" id=\"offBuff\">");
                                    {
                                        sw.Write("<ul class=\"nav nav-tabs\">" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#offensiveUptime\">Uptime</a></li>" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenSelf\">Generation (Self)</a></li>" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenGroup\">Generation (Group)</a></li>" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenOGroup\">Generation (Off-Group)</a></li>" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#offensiveGenSquad\">Generation (Squad)</a></li>" +
                                               "</ul>");
                                        sw.Write("<div id=\"offBuffSubTab\" class=\"tab-content\">");
                                        {
                                            //Offensive Buffs stats
                                            sw.Write("<div class=\"tab-pane fade show active\" id=\"offensiveUptime\">");
                                            {
                                                sw.Write("<p> Offensive Buffs Uptime</p>");
                                                CreateUptimeTable(sw, present_offbuffs, "offensive_table");
                                            }
                                            sw.Write("</div>");
                                            sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenSelf\">");
                                            {
                                                sw.Write("<p> Offensive Buffs generated by a character for themselves</p>");
                                                CreateGenSelfTable(sw, present_offbuffs, "offensivegenself_table");
                                            }
                                            sw.Write("</div>");
                                            sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenGroup\">");
                                            {
                                                sw.Write("<p> Offensive Buffs generated by a character for their sub group</p>");
                                                CreateGenGroupTable(sw, present_offbuffs, "offensivegengroup_table");
                                            }
                                            sw.Write("</div>");
                                            sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenOGroup\">");
                                            {
                                                sw.Write("<p> Offensive Buffs generated by a character for any subgroup that is not their own</p>");
                                                CreateGenOGroupTable(sw, present_offbuffs, "offensivegenogroup_table");
                                            }
                                            sw.Write("</div>");
                                            sw.Write("<div class=\"tab-pane fade\" id=\"offensiveGenSquad\">");
                                            {
                                                sw.Write("<p> Offensive Buffs generated by a character for the entire squad</p>");
                                                CreateGenSquadTable(sw, present_offbuffs, "offensivegensquad_table");
                                            }
                                            sw.Write("</div>");
                                        }
                                        sw.Write("</div>");
                                    }
                                    sw.Write("</div>");
                                    sw.Write("<div class=\"tab-pane fade  \" id=\"defBuff\">");
                                    {
                                        sw.Write("<ul class=\"nav nav-tabs\">" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link active\" data-toggle=\"tab\" href=\"#defensiveUptime\">Uptime</a></li>" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenSelf\">Generation (Self)</a></li>" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenGroup\">Generation (Group)</a></li>" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenOGroup\">Generation (Off-Group)</a></li>" +
                                                   "<li class=\"nav-item\"><a class=\"nav-link \" data-toggle=\"tab\" href=\"#defensiveGenSquad\">Generation (Squad)</a></li>" +
                                               "</ul>");
                                        sw.Write("<div id=\"defBuffSubTab\" class=\"tab-content\">");
                                        {
                                            //Defensive Buffs stats
                                            sw.Write("<div class=\"tab-pane fade show active\" id=\"defensiveUptime\">");
                                            {
                                                sw.Write("<p> Defensive Buffs Uptime</p>");
                                                CreateUptimeTable(sw, present_defbuffs, "defensive_table");
                                            }
                                            sw.Write("</div>");
                                            sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenSelf\">");
                                            {
                                                sw.Write("<p> Defensive Buffs generated by a character for themselves</p>");
                                                CreateGenSelfTable(sw, present_defbuffs, "defensivegenself_table");
                                            }
                                            sw.Write("</div>");
                                            sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenGroup\">");
                                            {
                                                sw.Write("<p> Defensive Buffs generated by a character for their sub group</p>");
                                                CreateGenGroupTable(sw, present_defbuffs, "defensivegengroup_table");
                                            }
                                            sw.Write("</div>");
                                            sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenOGroup\">");
                                            {
                                                sw.Write("<p> Defensive Buffs generated by a character for any subgroup that is not their own</p>");
                                                CreateGenOGroupTable(sw, present_defbuffs, "defensivegenogroup_table");
                                            }
                                            sw.Write("</div>");
                                            sw.Write("<div class=\"tab-pane fade\" id=\"defensiveGenSquad\">");
                                            {
                                                sw.Write("<p> Defensive Buffs generated by a character for the entire squad</p>");
                                                CreateGenSquadTable(sw, present_defbuffs, "defensivegensquad_table");
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
                            sw.Write("<div class=\"tab-pane fade\" id=\"mechTable\">");
                            {
                                sw.Write("<p>Mechanics</p>");
                                CreateMechanicTable(sw);
                            }
                            sw.Write("</div>");
                            //boss summary
                            if (settingsSnap[9])
                            {
                                sw.Write("<div class=\"tab-pane fade\" id=\"bossSummary\">");
                                {
                                    CreateBossSummary(sw);
                                }
                                sw.Write("</div>");
                            }
                            //event list
                            if (settingsSnap[8])
                            {
                                sw.Write("<div class=\"tab-pane fade\" id=\"eventList\">");
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
                                sw.Write("<div class=\"tab-pane fade\" id=\"estimates\">");
                                {
                                    CreateEstimateTabs(sw);
                                }
                                sw.Write("</div>");
                            }
                            //Html_playertabs
                            CreatePlayerTab(sw,settingsSnap);
                        }
                        sw.Write("</div>");
                        sw.Write("<p style=\"margin-top:10px;\"> ARC:" + getLogData().getBuildVersion().ToString() + " | Bossid " + getBossData().getID().ToString() + " </p> ");
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
            BossData b_data = getBossData();
            double fight_duration = (b_data.getLastAware() - b_data.getFirstAware()) / 1000.0;
            Player p = p_list[0];

            CombatData c_data = getCombatData();
            
            List<CastLog> casting = p.getCastLogs(b_data, c_data.getCombatList(), getAgentData());
            SkillData s_data = getSkillData();
            List<SkillItem> s_list = s_data.getSkillList();

            CreateDPSTable(sw, fight_duration);
            CreateDMGStatsTable(sw, fight_duration);
            CreateDefTable(sw, fight_duration);
            CreateSupTable(sw, fight_duration);
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

                            string skillName = "";
                            GW2APISkill skill = null;
                            if (s_list.FirstOrDefault(x => x.getID() == cl.getID()) != null)
                            {
                                skill = s_list.FirstOrDefault(x => x.getID() == cl.getID()).GetGW2APISkill();
                            }
                            if (skill == null)
                            {
                                skillName = s_data.getName(cl.getID());
                            }
                            else
                            {
                                skillName = skill.name;
                            }
                            float dur = 0.0f;
                            if (skillName == "Dodge")
                            {
                                dur = 0.5f;
                            }
                            else if (cl.getID() == -2)
                            {//wepswap
                                skillName = "Weapon Swap";
                                dur = 0.1f;
                            }
                            else if (skillName == "Resurrect")
                            {
                                dur = cl.getActDur() / 1000f;
                            }
                            else if (skillName == "Bandage")
                            {
                                dur = cl.getActDur() / 1000f;
                            }
                            else
                            {
                                dur = cl.getActDur() / 1000f;
                            }
                            skillName = skillName.Replace("\"", "");
                            sw.Write("{");
                            {

                                sw.Write("y: ['1.5'],");


                                sw.Write(
                                       "x: ['" + dur + "']," +
                                       "base:'" + cl.getTime() / 1000f + "'," +
                                       "name: \"" + skillName + " " + dur + "s\"," +//get name should be handled by api
                                       "orientation:'h'," +
                                       "mode: 'markers'," +
                                       "type: 'bar',");
                                if (skill != null)
                                {
                                    if (skill.slot == "Weapon_1")
                                    {
                                        sw.Write("width:'0.5',");
                                    }
                                    else
                                    {
                                        sw.Write("width:'1',");
                                    }

                                }
                                else
                                {
                                    sw.Write("width:'1',");
                                }
                                sw.Write("hoverinfo: 'name'," +
                                        "hoverlabel:{namelength:'-1'},");
                                sw.Write("marker: {");
                                {
                                    if (cl.endActivation() != null)
                                    {
                                        if (cl.endActivation().getID() == 3)
                                        {
                                            sw.Write("color: 'rgb(40,40,220)',");
                                        }
                                        else if (cl.endActivation().getID() == 4)
                                        {
                                            sw.Write("color: 'rgb(220,40,40)',");
                                        }
                                        else if (cl.endActivation().getID() == 5)
                                        {
                                            sw.Write("color: 'rgb(40,220,40)',");
                                        }
                                        else
                                        {
                                            sw.Write("color: 'rgb(220,220,0)',");
                                        }
                                    }
                                    else
                                    {
                                        sw.Write("color: 'rgb(220,220,0)',");
                                    }
                                    sw.Write("width: '5',");
                                    sw.Write("line:{");
                                    {
                                        if (cl.startActivation() != null)
                                        {
                                            if (cl.startActivation().getID() == 1)
                                            {
                                                sw.Write("color: 'rgb(20,20,20)',");
                                            }
                                            else if (cl.startActivation().getID() == 2)
                                            {
                                                sw.Write("color: 'rgb(220,40,220)',");
                                            }
                                        }
                                        sw.Write("width: '1'");
                                    }
                                    sw.Write("}");
                                }
                                sw.Write("},");
                                sw.Write("showlegend: false");
                            }
                            sw.Write(" },");
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
                        Dictionary<int,BoonsGraphModel> boonGraphData = p.getBoonGraphs(getBossData(), getSkillData(), getCombatData().getCombatList());
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
                        List<int[]> playerbossdpsgraphdata = getBossDPSGraph(p);
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
                            List<int[]> playertotaldpsgraphdata = getTotalDPSGraph(p);
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
                                string skillIcon = "";
                                GW2APISkill skill = null;
                                if (s_list.FirstOrDefault(x => x.getID() == cl.getID()) != null)
                                {
                                    skill = s_list.FirstOrDefault(x => x.getID() == cl.getID()).GetGW2APISkill();
                                }
                                if (skill != null && cl.getID() != -2)
                                {
                                    if (skill.slot != "Weapon_1")
                                    {
                                        skillIcon = skill.icon;
                                        sw.Write("{" +
                                                     "source: '" + skillIcon + "'," +
                                                     "xref: 'x'," +
                                                     "yref: 'y'," +
                                                     "x: " + (cl.getTime() / 1000f) + "," +
                                                     "y: 0," +
                                                     "sizex: 1.1," +
                                                     "sizey: 1.1," +
                                                     "xanchor: 'left'," +
                                                     "yanchor: 'bottom'" +
                                                "}");
                                    }
                                }
                                else
                                {
                                    string skillName = "";

                                    if (cl.getID() == -2)
                                    { //wepswap
                                        skillName = "Weapon Swap";
                                        // skillIcon = "https://wiki.guildwars2.com/images/archive/c/ce/20140606174035%21Weapon_Swap_Button.png";
                                    }
                                    else
                                    {
                                        skillName = skill_data.getName(cl.getID());
                                    }


                                    if (skillName == "Dodge")
                                    {
                                        // skillIcon = "https://wiki.guildwars2.com/images/c/cc/Dodge_Instructor.png";
                                    }
                                    else if (skillName == "Resurrect")
                                    {
                                        //skillIcon = "https://wiki.guildwars2.com/images/archive/d/dd/20120611120554%21Downed.png";
                                    }
                                    else if (skillName == "Bandage")
                                    {
                                        // skillIcon = "https://wiki.guildwars2.com/images/0/0c/Bandage.png";
                                    }
                                    sw.Write("{" +
                                                  "source: '" + skillIcon + "'," +
                                                  "xref: 'x'," +
                                                  "yref: 'y'," +
                                                  "x: " + cl.getTime() / 1000f + "," +
                                                  "y: 0," +
                                                  "sizex: 1.1," +
                                                  "sizey: 1.1," +
                                                  "xanchor: 'left'," +
                                                  "yanchor: 'bottom'" +
                                              "}");
                                }
                                if (castCount == casting.Count - 1)
                                {
                                }
                                else
                                {
                                    sw.Write(",");
                                }
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
                    CreateDMGDistTable(sw, p, false);
                }
                sw.Write("</div>");
                sw.Write("<div class=\"tab-pane fade\" id=\"distTabBoss" + p.getInstid() + "\">");
                {
                    CreateDMGDistTable(sw, p, true);
                }
                sw.Write("</div>");
            }
            sw.Write("</div>");
        }
        //Creating CSV---------------------------------------------------------------------------------
        public void CreateCSV(StreamWriter sw,String delimiter)
        {
            BossData b_data = getBossData();
            double fight_duration = (b_data.getLastAware() - b_data.getFirstAware()) / 1000.0;
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
                string[] finaldps = getFinalDPS(p).Split('|');
                teamStats[0] += Int32.Parse(finaldps[6]);
                teamStats[1] += Int32.Parse(finaldps[0]);
                teamStats[2] += (Int32.Parse(finaldps[0]) - Int32.Parse(finaldps[6]));
            }

            foreach (Player p in p_list)
            {
                string[] finaldps = getFinalDPS(p).Split('|');
                sw.Write(p.getGroup() + delimiter + // group
                        p.getProf() + delimiter +  // class
                        p.getCharacter() + delimiter + // character
                        p.getAccount().Substring(1) + delimiter + // account
                        finaldps[6] + delimiter + // dps
                        finaldps[8] + delimiter + // physical
                        finaldps[10] + delimiter + // condi
                        finaldps[0] + delimiter); // all dps

                Dictionary<int, string> boonArray = getfinalboons(p);
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
                case "Sword":
                    return "https://wiki.guildwars2.com/images/6/61/Sword_Proficiency.png";
                case "Axe":
                    return "https://wiki.guildwars2.com/images/a/a2/Axe_Proficiency.png";
                case "Dagger":
                    return "https://wiki.guildwars2.com/images/c/c9/Dagger_Proficiency.png";
                case "Mace":
                    return "https://wiki.guildwars2.com/images/3/37/Mace_Smash.png";
                case "Pistol":
                    return "https://wiki.guildwars2.com/images/7/7a/Phantasmal_Duelist.png";
                case "Sceptor":
                    return "https://wiki.guildwars2.com/images/2/22/Water_Trident.png";
                case "Focus":
                    return "https://wiki.guildwars2.com/images/7/79/Focus_Mastery.png";
                case "Shield":
                    return "https://wiki.guildwars2.com/images/c/c6/Shield_Proficiency.png";
                case "Torch":
                    return "https://wiki.guildwars2.com/images/1/11/Torch_Proficiency.png";
                case "Warhorn":
                    return "https://wiki.guildwars2.com/images/b/b8/Warhorn_Proficiency.png";
                case "Greatsword":
                    return "https://wiki.guildwars2.com/images/8/8b/Greatsword_Proficiency.png";
                case "Hammer":
                    return "https://wiki.guildwars2.com/images/5/5f/Hammer_Proficiency.png";
                case "Longbow":
                    return "https://wiki.guildwars2.com/images/5/53/Longbow_Proficiency.png";
                case "Shortbow":
                    return "https://wiki.guildwars2.com/images/e/e7/Short_Bow_Proficiency_%28renegade%29.png";
                case "Rifle":
                    return "https://wiki.guildwars2.com/images/5/5d/Rifle_Proficiency_%28deadeye%29.png";
                case "Staff":
                    return "https://wiki.guildwars2.com/images/7/78/Staff_Proficiency.png";
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
