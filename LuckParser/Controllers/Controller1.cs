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
//recomend CTRL+M+O to collapse all
namespace LuckParser.Controllers
{
    public class Controller1
    {
        public static byte[] StreamToBytes(Stream input)
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


        // Private Methods
        //for pulling from binary
        private Stream origstream = null;
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
        private int getShort()
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
        public List<Player> p_list = new List<Player>();
        //Main Parse method------------------------------------------------------------------------------------------------------------------------------------------------
        public bool ParseLog(string evtc)
        {
            //used to stream from a database, probably could use better stream now
            WebClient client = new WebClient();
            origstream = client.OpenRead(evtc);
            origstream.CopyTo(stream);
            stream.Position = 0;

            parseBossData();
            parseAgentData();
            parseSkillData();
            parseCombatList();
            fillMissingData();

            origstream.Close();
            stream.Close();
            ////CreateHTML(); is now runnable dont run here
            return (true);
        }
        //sub Parse methods
        private void parseBossData()
        {
            // 12 bytes: arc build version
            String build_version = getString(12);
            this.log_data = new LogData(build_version);

            // 1 byte: skip
            safeSkip(1);

            // 2 bytes: boss instance ID
            int instid = getShort();

            // 1 byte: position
            safeSkip(1);

            //Save
            // TempData["Debug"] = build_version +" "+ instid.ToString() ;
            this.boss_data = new BossData(instid);
        }
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
                    if (a.getProf(this.log_data.getBuildVersion()) == "NPC")
                    {
                        agent_data.addItem(a, new AgentItem(agent, name, a.getName() + ":" + prof.ToString().PadLeft(5, '0')), this.log_data.getBuildVersion());//a.getName() + ":" + String.format("%05d", prof)));
                    }
                    // Gadget
                    else if (a.getProf(this.log_data.getBuildVersion()) == "GDG")
                    {
                        agent_data.addItem(a, new AgentItem(agent, name, a.getName() + ":" + (prof & 0x0000ffff).ToString().PadLeft(5, '0')), this.log_data.getBuildVersion());//a.getName() + ":" + String.format("%05d", prof & 0x0000ffff)));
                    }
                    // Player
                    else
                    {
                        agent_data.addItem(a, new AgentItem(agent, name, a.getProf(this.log_data.getBuildVersion()), toughness, healing, condition), this.log_data.getBuildVersion());
                    }
                }
                // Unknown
                else
                {
                    agent_data.addItem(a, new AgentItem(agent, name, prof.ToString(), toughness, healing, condition), this.log_data.getBuildVersion());
                }
            }
        }
        private void parseSkillData()
        {
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
                String nameTrim = name.Replace("/0", "");
                //Save
                //TempData["Debug"] += "<br/>" + skill_id + " " + name;
                skill_data.addItem(new SkillItem(skill_id, nameTrim));
            }
        }
        private void parseCombatList()
        {
            // 64 bytes: each combat
            while (stream.Length - stream.Position >= 64)
            {
                // 8 bytes: time
                int time = (int)getLong();

                // 8 bytes: src_agent
                long src_agent = getLong();

                // 8 bytes: dst_agent
                long dst_agent = getLong();

                // 4 bytes: value
                int value = getInt();

                // 4 bytes: buff_dmg
                int buff_dmg = getInt();

                // 2 bytes: overstack_value
                int overstack_value = getShort();

                // 2 bytes: skill_id
                int skill_id = getShort();

                // 2 bytes: src_instid
                int src_instid = getShort();

                // 2 bytes: dst_instid
                int dst_instid = getShort();

                // 2 bytes: src_master_instid
                int src_master_instid = getShort();

                // 9 bytes: garbage
                safeSkip(9);

                // 1 byte: iff
                //IFF iff = IFF.getEnum(f.read());
               IFF iff = new IFF(Convert.ToByte(stream.ReadByte())); //Convert.ToByte(stream.ReadByte());

                // 1 byte: buff
                int buff = stream.ReadByte();

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
                int is_ninety = stream.ReadByte();

                // 1 byte: is_fifty
                int is_fifty = stream.ReadByte();

                // 1 byte: is_moving
                int is_moving = stream.ReadByte();

                // 1 byte: is_statechange
                //StateChange is_statechange = StateChange.getEnum(f.read());
                StateChange is_statechange = new StateChange(Convert.ToByte(stream.ReadByte()));

                // 1 byte: is_flanking
                int is_flanking = stream.ReadByte();

                // 3 bytes: garbage
                safeSkip(3);

                //save
                // Add combat
                combat_data.addItem(new CombatItem(time, src_agent, dst_agent, value, buff_dmg, overstack_value, skill_id,
                        src_instid, dst_instid, src_master_instid, iff, buff, result, is_activation, is_buffremoved,
                        is_ninety, is_fifty, is_moving, is_statechange, is_flanking));
            }
        }
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

            // Grab values threw combat data
            foreach (CombatItem c in combat_list)
            {
                if (c.getSrcInstid() == boss_data.getInstid() && c.isStateChange().getID() ==12)//max health update
                {
                    boss_data.setHealth((int)c.getDstAgent());
                   
                }
                 if (c.isStateChange().getID() == 13 && log_data.getPOV() == "N/A")//Point of View
                {
                    int pov_instid = c.getSrcInstid();
                    foreach (AgentItem p in player_list)
                    {
                        if (pov_instid == p.getInstid())
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
                //set boss dead
                if (c.getSrcInstid() == boss_data.getInstid() && c.isStateChange().getID() == 4)//change dead
                {
                    log_data.setBossKill(true);
                   
                }
            }
           
            // Dealing with second half of Xera | ((22611300 * 0.5) + (25560600 *
            // 0.5)
            int xera_2_instid = 0;
            foreach (AgentItem NPC in NPC_list)
            {
                if (NPC.getProf().Contains("16286"))
                {
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
                    }
                    break;
                }
                // Players
                List<AgentItem> playerAgentList = getAgentData().getPlayerAgentList();
                if (p_list.Count == 0)
                {
                    foreach (AgentItem playerAgent in playerAgentList)
                    {
                        p_list.Add(new Player(playerAgent));
                    }
                }
                // Sort

                p_list = p_list.OrderBy(a => Int32.Parse(a.getGroup())).ToList();//p_list.Sort((a, b)=>Int32.Parse(a.getGroup()) - Int32.Parse(b.getGroup()))
                getBossKilled();
            }
        }

        //Statistics--------------------------------------------------------------------------------------------------------------------------------------------------------
        public String getFinalDPS(Player p)
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
            totalAll_dps = (Int32)dps;
            totalAll_damage = (Int32)damage;
            //Allcondi
            damage = p.getDamageLogs(0, b_data, c_data.getCombatList(), getAgentData()).Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalAllcondi_dps = (Int32)dps;
            totalAllcondi_damage = (Int32)damage;
            //All Power
            damage = totalAll_damage - damage;
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalAllphys_dps = (Int32)dps;
            totalAllphys_damage = (Int32)damage;

            // boss DPS
            damage = p.getDamageLogs(b_data.getInstid(), b_data, c_data.getCombatList(), getAgentData()).Sum(x => x.getDamage());//p.getDamageLogs(b_data, c_data.getCombatList()).stream().mapToDouble(DamageLog::getDamage).sum();
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalboss_dps = (Int32)dps;
            totalboss_damage = (Int32)damage;
            //bosscondi
            damage = p.getDamageLogs(b_data.getInstid(), b_data, c_data.getCombatList(), getAgentData()).Where(x => x.isCondi() > 0).Sum(x => x.getDamage());
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalbosscondi_dps = (Int32)dps;
            totalbosscondi_damage = (Int32)damage;
            //boss Power
            damage = totalboss_damage - damage;
            if (fight_duration > 0)
            {
                dps = damage / fight_duration;
            }
            totalbossphys_dps = (Int32)dps;
            totalbossphys_damage = (Int32)damage;
            //Placeholders for further calc
            return totalAll_dps.ToString() + "|" + totalAll_damage.ToString() + "|" + totalAllphys_dps.ToString() + "|" + totalAllphys_damage.ToString() + "|" + totalAllcondi_dps.ToString() + "|" + totalAllcondi_damage.ToString() + "|"
                + totalboss_dps.ToString() + "|" + totalboss_damage.ToString() + "|" + totalbossphys_dps.ToString() + "|" + totalbossphys_damage.ToString() + "|" + totalbosscondi_dps.ToString() + "|" + totalbosscondi_damage.ToString();
        }
        public bool getBossKilled() {
            if (log_data.getBosskill() == false)
            {
                BossData b_data = getBossData();
                CombatData c_data = getCombatData();
                int totaldmg = 0;

                foreach (Player p in p_list)
                {
                    totaldmg += p.getDamageLogs(b_data.getInstid(), b_data, c_data.getCombatList(), getAgentData()).Sum(x => x.getDamage());
                }
                int healthremaining = b_data.getHealth() - totaldmg;
                if (healthremaining > 0)
                {
                    log_data.setBossKill(false);
                    return false;
                }
                else if (healthremaining <= 0)
                {
                    log_data.setBossKill(true);
                    return true;
                }
            }
            return false;
        }
        public String[] getFinalStats(Player p)
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
                    // critical_rate += (log.getResult().equals(Result.CRIT)) ? 1 : 0;
                    scholar_rate += log.isNinety();
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
                if ( cl.endActivation().getID() == 4)
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

            power_loop_count = (power_loop_count == 0) ? 1 : power_loop_count;

            // Counts
            int swap = c_data.getStates(instid, "WEAPON_SWAP").Count();
            int down = c_data.getStates(instid, "CHANGE_DOWN").Count();
            int dodge = c_data.getSkillCount(instid, 65001);//dodge = 65001
            int ress = c_data.getSkillCount(instid, 1066); //Res = 1066

            // R.I.P
            List<Point> dead = c_data.getStates(instid, "CHANGE_DEAD");
            double died = 0.0;
            if (dead.Count() > 0)
            {
                died = dead[0].X - b_data.getFirstAware();
            }

            statsArray = new string[] { power_loop_count.ToString(), critical_rate.ToString(), scholar_rate.ToString(), moving_rate.ToString(),
                flanking_rate.ToString(), swap.ToString(),down.ToString(),dodge.ToString(),ress.ToString(),died.ToString("0.00"),
            glance_rate.ToString(),missed.ToString(),interupts.ToString(),invulned.ToString(),(time_wasted/1000f).ToString(),wasted.ToString(),avgBoons.ToString(),(time_saved/1000f).ToString(),saved.ToString()
            };
            return statsArray;
        }
        string getDamagetaken(Player p)
        {
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            int instid = p.getInstid();
            int damagetaken = p.getDamagetaken(b_data, c_data.getCombatList(), getAgentData()).Sum();
            return damagetaken.ToString();
        }
        string[] getFinalDefenses(Player p)
        {
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            List<DamageLog> damage_logs = p.getDamageTakenLogs(b_data, c_data.getCombatList(), getAgentData());
            int instid = p.getInstid();

            int damagetaken = p.getDamagetaken(b_data, c_data.getCombatList(), getAgentData()).Sum();
            int blocked = 0;
            //int dmgblocked = 0;
            int invulned = 0;
            //int dmginvulned = 0;
            int dodge = c_data.getSkillCount(instid, 65001);//dodge = 65001
            int evades = 0;
            //int dmgevaded = 0;

            foreach (DamageLog log in damage_logs.Where(x => x.getResult().getEnum() == "BLOCK"))
            {
                blocked++;
                //dmgblocked += log.getDamage();
            }
            foreach (DamageLog log in damage_logs.Where(x => x.getResult().getEnum() == "ABSORB"))
            {
                invulned++;
                //dmginvulned += log.getDamage();
            }
            foreach (DamageLog log in damage_logs.Where(x => x.getResult().getEnum() == "EVADE"))
            {
                evades++;
                // dmgevaded += log.getDamage();
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
                blocked.ToString(),"0"/*dmgblocked.ToString()*/,invulned.ToString(),"0"/*dmginvulned.ToString()*/,
                dodge.ToString(),evades.ToString(),"0"/*dmgevaded.ToString()*/,
            down.ToString(),died.ToString("0.00")};
            return statsArray;
        }
        //(currently not correct)
        string[] getFinalSupport(Player p)
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


            String[] statsArray = new string[] { resurrects.ToString(), restime.ToString(), condiCleanse.ToString(), condiCleansetime.ToString() };
            return statsArray;
        }
        public string[] getfinalboons(Player p, List<int> trgetPlayers)
        {
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            SkillData s_data = getSkillData();
            List<BoonMap> boon_logs = new List<BoonMap>();
            if (trgetPlayers.Count() == 0)
            {
                boon_logs = p.getBoonMap(b_data, s_data, c_data.getCombatList());
            }
            else
            {
                boon_logs = p.getboonGen(b_data, s_data, c_data.getCombatList(), agent_data, trgetPlayers);
            }

            List<Boon> boon_list = Boon.getList();
            int n = boon_list.Count();//# of diff boons
            string[] rates = new string[n];
            for (int i = 0; i < n; i++)
            {
                // Boon boon = Boon.getEnum(boon_list[i].ToString());
                Boon boon = boon_list[i];
                AbstractBoon boon_object = BoonFactory.makeBoon(boon);
                List<BoonLog> logs = boon_logs.FirstOrDefault(x => x.getName() == boon.getName()).getBoonLog();//Maybe wrong pretty sure it ok tho
                string rate = "0";
                if (logs.Count() > 0)
                {

                    if (trgetPlayers.Count() == 0)
                    {
                        if (boon.getType().Equals("duration"))
                        {

                            rate = String.Format("{0:0}", Statistics.getBoonDuration(Statistics.getBoonIntervalsList(boon_object, logs, b_data), b_data));
                        }
                        else if (boon.getType().Equals("intensity"))
                        {
                            rate = String.Format("{0:0.0}", Statistics.getAverageStacks(Statistics.getBoonStacksList(boon_object, logs, b_data)));
                        }
                    }
                    else
                    {
                        if (boon.getType().Equals("duration"))
                        {
                            double[] array = Statistics.getBoonUptime(boon_object, logs, b_data, trgetPlayers.Count());
                            rate = "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\"" + String.Format("{0:0} %", array[1] * 100) + "with overstack \">" + String.Format("{0:0}%", array[0] * 100) + "</span>";
                        }
                        else if (boon.getType().Equals("intensity"))
                        {
                            double[] array = Statistics.getBoonUptime(boon_object, logs, b_data, trgetPlayers.Count());
                            rate = "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"\" data-original-title=\"" + String.Format("{0:0.0}", array[1]) + "with overstack \">" + String.Format("{0:0.0}", array[0]) + "</span>";
                        }
                    }

                }
                rates[i] = rate;
            }
            //table.addrow(utility.concatstringarray(new string[] { p.getcharacter(), p.getprof() }, rates));
            return rates;
        }

        //Generate HTML---------------------------------------------------------------------------------------------------------------------------------------------------------
        //Methods that make it easier to create Javascript graphs
        public List<BoonsGraphModel> getBoonGraph(Player p) {
            List<BoonsGraphModel> uptime = new List<BoonsGraphModel>();
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            SkillData s_data = getSkillData();
            List<BoonMap> boon_logs = p.getBoonMap(b_data, s_data, c_data.getCombatList());
            List<Boon> boon_list = Boon.getList();
            int n = boon_list.Count();//# of diff boons

            for (int i = 0; i < n; i++)//foreach boon
            {
                Boon boon = boon_list[i];
                AbstractBoon boon_object = BoonFactory.makeBoon(boon);
                List<BoonLog> logs = boon_logs.FirstOrDefault(x => x.getName() == boon.getName()).getBoonLog();//Maybe wrong pretty sure it ok tho

                List<Point> pointlist = new List<Point>();
                if (logs.Count() > 0)
                {
                    if (boon.getType().Equals("duration"))
                    {
                        int fight_duration = (int)((b_data.getLastAware() - b_data.getFirstAware()) / 1000.0);
                        List<Point> pointswierds = Statistics.getBoonIntervalsList(boon_object, logs, b_data);
                        int pwindex = 0;
                        int enddur = 0;
                        for (int cur_time = 0;cur_time<fight_duration;cur_time++) {
                            if (cur_time == (int)(pointswierds[pwindex].X / 1000f))
                            {
                                pointlist.Add(new Point((int)(pointswierds[pwindex].X / 1000f), 1));
                                enddur = (int)(pointswierds[pwindex].Y/1000f);
                                if(pwindex < pointswierds.Count()-1) {pwindex++; }
                                
                            }
                            else if (cur_time < enddur)
                            {
                                pointlist.Add(new Point(cur_time, 1));
                            }
                            else {
                                pointlist.Add(new Point(cur_time, 0));
                            }
                        }
                        
                    }
                    else if (boon.getType().Equals("intensity"))
                    {
                        List<int> stackslist = Statistics.getBoonStacksList(boon_object, logs, b_data);
                        int time = 0;
                        int timeGraphed = 0;
                        foreach(int stack in stackslist) {
                            if (Math.Floor(time / 1000f) > timeGraphed) {
                                timeGraphed = (int)Math.Floor(time / 1000f);
                                pointlist.Add(new Point(time/1000, stack));
                            }
                            time++;
                        }
                      
                    }
                    BoonsGraphModel bgm = new BoonsGraphModel(boon.getName(),pointlist);
                    uptime.Add(bgm);
                }

            }
            return uptime;
        }
        public List<int[]> getBossDPSGraph(Player p) {
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            List<int[]> bossdmgList = new List<int[]>();
           // List<DamageLog> damage_logs_all = p.getDamageLogs(0, b_data, c_data.getCombatList(), getAgentData());
            List<DamageLog> damage_logs_boss = p.getDamageLogs(b_data.getInstid(), b_data, c_data.getCombatList(), getAgentData());
            int totaldmg = 0;
            int timeGraphed = 0;
            foreach (DamageLog log in damage_logs_boss) {
                totaldmg += log.getDamage();
                int time = log.getTime();
                if (time > 1000 )
                {
                    //to reduce processing time only graph 1 point per sec
                    if (Math.Floor(time / 1000f) > timeGraphed)
                    {
                        if ((Math.Floor(time / 1000f) - timeGraphed) < 2)
                        {
                            timeGraphed = (int)Math.Floor(time / 1000f);
                            bossdmgList.Add(new int[] { time / 1000, (int)(totaldmg / (float)(time / 1000f)) });
                        }
                        else {
                            int gap = (int)Math.Floor(time / 1000f) - timeGraphed;
                            for (int itr = 0; itr < gap-1; itr++) {
                                timeGraphed++;
                                bossdmgList.Add(new int[] {timeGraphed , (int)(totaldmg / (float)timeGraphed ) });
                            }
                        }
                       
                    }
                }
            }
            return bossdmgList;
        }
        public List<int[]> getTotalDPSGraph(Player p)
        {
            BossData b_data = getBossData();
            CombatData c_data = getCombatData();
            List<int[]> totaldmgList = new List<int[]>();
             List<DamageLog> damage_logs_all = p.getDamageLogs(0, b_data, c_data.getCombatList(), getAgentData());
            //List<DamageLog> damage_logs_boss = p.getDamageLogs(b_data.getInstid(), b_data, c_data.getCombatList(), getAgentData());
            int totaldmg = 0;
            int timeGraphed = 0;
            foreach (DamageLog log in damage_logs_all)
            {
                totaldmg += log.getDamage();
                int time = log.getTime();
                if (time > 1000)
                {
                    // to reduce processing time only graph 1 point per sec
                    if (Math.Floor(time / 1000f) > timeGraphed)
                    {
                        if ((Math.Floor(time / 1000f) - timeGraphed) < 2)
                        {
                            timeGraphed = (int)Math.Floor(time / 1000f);
                            totaldmgList.Add(new int[] { time / 1000, (int)(totaldmg / (float)(time / 1000f)) });
                        }
                        else
                        {
                            int gap = (int)Math.Floor(time / 1000f) - timeGraphed;
                            for (int itr = 0; itr < gap - 1; itr++)
                            {
                                timeGraphed++;
                                totaldmgList.Add(new int[] { timeGraphed, (int)(totaldmg / (float)timeGraphed) });
                            }
                        }
                    }
                }
            }
            return totaldmgList;
        }
        public string CreateHTML(bool[] settingArray)
        {
            BossData b_data = getBossData();
            double fight_duration = (b_data.getLastAware() - b_data.getFirstAware()) / 1000.0;
            TimeSpan duration = TimeSpan.FromSeconds(fight_duration);
            String durationString = duration.ToString("mm") + "m " + duration.ToString("ss") + "s";
            string bossname = FilterStringChars(getBossData().getName());
           
            string HTML_CONTENT = "";
            string HTML_Head = "<!DOCTYPE html>\r<html lang=\"en\"><head> " +
                "<meta charset=\"utf-8\">" +
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
              "<script src=\"https://code.jquery.com/jquery-1.12.4.js \"></script>" +
              "<script src=\"https://cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js \"></script>" +
            "<script src=\"https://cdn.datatables.net/plug-ins/1.10.13/sorting/alt-string.js \"></script>" +
             "<script src=\"https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/js/bootstrap.min.js \"></script>" +
            "<style>" +
            "td, th {text-align: center; white-space: nowrap;}" +
            "table.dataTable  td {color: black;}" +
             ".sorting_disabled {padding: 5px !important;}" +
                    "table.dataTable.table-condensed.sorting, table.dataTable.table-condensed.sorting_asc, table.dataTable.table-condensed.sorting_desc " +
                    "{right: 4px !important;}table.dataTable thead.sorting_desc { color: red;}" +
                    "table.dataTable thead.sorting_asc{color: green;}" +
                    ".text-left {text-align: left;}" +
                    "table.dataTable.table-condensed > thead > tr > th.sorting { padding-right: 5px !important; }" +
                    ".rot-table {width: 100%;border-collapse: separate;border-spacing: 5px 0px;}" +
                    ".rot-table > tbody > tr > td {padding: 1px;text-align: left;}" +
                    ".rot-table > thead {vertical-align: bottom;border-bottom: 2px solid #ddd;}" +
                    ".rot-table > thead > tr > th {padding: 10px 1px 9px 1px;line-height: 18px;text-align: left;}" +
                    "div.dataTables_wrapper { width: 1100px; margin: 0 auto; }" +
            "</style>" +
                    "<script>$.extend( $.fn.dataTable.defaults, {searching: false, ordering: true,paging: false,dom:\"t\"} );</script>" +
                 "</head>";
            int groupCount = 0;
            foreach (Player play in p_list)
            {
                if (Int32.Parse(play.getGroup()) > groupCount)
                {
                    groupCount = Int32.Parse(play.getGroup());
                }
            }
            //generate comp table
            string HTML_compTable = "<table class=\table\" style=\"width:auto;position:absolute; top:50%; height:10em; margin-top:-5em\"><tbody>";
            for (int n = 0; n <= groupCount; n++)//NEEDS FIXING FOR WHEN NO PLAYERS IN GROUP 1
            {
                HTML_compTable += "<tr>";
                List<Player> sortedList = p_list.Where(x => Int32.Parse(x.getGroup()) == n).ToList();
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
                        HTML_compTable += "<td style=\"width: 150px; border:1px solid #EE5F5B;\">" +
                             "<img src=\"" + GetLink(gPlay.getProf().ToString()) + " \" alt=\"" + gPlay.getProf().ToString() + "\" height=\"18\" width=\"18\" >" +
                             build +
                            "<br/>" + charName + "</td>";
                    }
                }
                HTML_compTable += "</tr>";
            }
            HTML_compTable += "</tbody></table>";
           
            //generate dps table
            string HTML_dps = " <script> $(function () { $('#dps_table').DataTable({ \"order\": [[4, \"desc\"]]});});</script>" +
       " <table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dps_table\">" +
           " <thead> <tr> <th>Sub</th><th></th><th>Name</th><th>Account</th> <th>Boss DPS</th><th>Power</th><th>Condi</th><th>All DPS</th><th>Power</th><th>Condi</th>" +
           "</th><th><img src=" +GetLink("Downs") + " alt=\"Downs\" title=\"Times downed\" height=\"18\" width=\"18\">" +
           "</th><th><img src=" + GetLink("Dead") + " alt=\"Dead\" title=\"Time died\" height=\"18\" width=\"18\">" + "</th>" +
               " </tr> </thead><tbody>";
            foreach (Player player in p_list)
            {
                HTML_dps += "<tr>";
                HTML_dps += "<td>" + player.getGroup().ToString() + "</td>";
                HTML_dps += "<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>";
                HTML_dps += "<td>" + player.getCharacter().ToString() + "</td>";
                HTML_dps += "<td>" + player.getAccount().TrimStart(':') + "</td>";
                string[] dmg = getFinalDPS(player).Split('|');
                //Boss dps
                HTML_dps += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dmg[7] + " dmg \">" + dmg[6] + "</span>" + "</td>";
                HTML_dps += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dmg[9] + " dmg \">" + dmg[8] + "</span>" + "</td>";
                HTML_dps += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dmg[11] + " dmg \">" + dmg[10] + "</span>" + "</td>";
                //All DPS
                HTML_dps += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dmg[1] + " dmg \">" + dmg[0] + "</span>" + "</td>";
                HTML_dps += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dmg[3] + " dmg \">" + dmg[2] + "</span>" + "</td>";
                HTML_dps += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + dmg[5] + " dmg \">" + dmg[4] + "</span>" + "</td>";

                
               
                string[] stats = getFinalStats(player);
               HTML_dps += "<td>" + stats[6] + "</td>";
                TimeSpan timedead = TimeSpan.FromMilliseconds(Double.Parse(stats[9]));
                if (timedead > TimeSpan.Zero)
                {
                    HTML_dps += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + "(" + (int)((timedead.TotalSeconds / fight_duration) * 100) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</span>" + " </td>";
                }
                else {
                    HTML_dps += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died 100% Alive) \"> 0</span>" + " </td>";

                }
                HTML_dps += "</tr>";
            }

            HTML_dps += "</tbody></table>";

            //generate dmgstats table
            string HTML_dmgstats = " <script> $(function () { $('#dmgstats_table').DataTable({ \"order\": [[3, \"desc\"]]});});</script>" +
       " <table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"dmgstats_table\">" +
           " <thead><tr><th>Sub</th><th></th><th>Name</th>" +
           "<th><img src=" + GetLink("Crit") + " alt=\"Crits\" title=\"Percent time hits critical\" height=\"18\" width=\"18\">" +
           "</th><th><img src=" + GetLink("Scholar") + " alt=\"Scholar\" title=\"Percent time hits while above 90% health\" height=\"18\" width=\"18\">" +
           "</th><th><img src=" + GetLink("SwS") + " alt=\"SwS\" title=\"Percent time hits while moveing\" height=\"18\" width=\"18\">" +
           "</th><th><img src=" + GetLink("Flank") + " alt=\"Flank\" title=\"Percent time hits while flanking\" height=\"18\" width=\"18\">" +
           "</th><th><img src=" + GetLink("Glance") + " alt=\"Glance\" title=\"Percent time hits while glanceing\" height=\"18\" width=\"18\">" +
           "</th><th><img src=" + GetLink("Miss") + " alt=\"Miss\" title=\"Number of hits while blinded\" height=\"18\" width=\"18\">" +
           "</th><th><img src=" + GetLink("Interupts") + " alt=\"Interupts\" title=\"Number of hits interupted?/hits used to interupt\" height=\"18\" width=\"18\">" +
           "</th><th><img src=" + GetLink("Invuln") + " alt=\"Ivuln\" title=\"times the enemy was invulnerable to attacks\" height=\"18\" width=\"18\">" +
           "</th><th><img src=" + GetLink("Wasted") + " alt=\"Wasted\" title=\"Time wasted interupting skill casts\" height=\"18\" width=\"18\">" +
            "</th><th><img src=" + GetLink("Saved") + " alt=\"Saved\" title=\"Time saved interupting skill casts\" height=\"18\" width=\"18\">" +

           "</th><th><img src=" + GetLink("Swap") + " alt=\"Swap\" title=\"Times weapon swapped\" height=\"18\" width=\"18\">" +
           "</th><th><img src=" + GetLink("Downs") + " alt=\"Downs\" title=\"Times downed\" height=\"18\" width=\"18\">" +
           "</th><th><img src=" + GetLink("Dead") + " alt=\"Dead\" title=\"Time died\" height=\"18\" width=\"18\">" + "</th>" +
               " </tr> </thead><tbody>";
            foreach (Player player in p_list)
            {
                HTML_dmgstats += "<tr>";
                HTML_dmgstats += "<td>" + player.getGroup().ToString() + "</td>";
                HTML_dmgstats += "<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>";
                HTML_dmgstats += "<td>" + player.getCharacter().ToString() + "</td>";

                string[] stats = getFinalStats(player);
                HTML_dmgstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[1] + " out of " + stats[0] + "hits \">" + (int)(Double.Parse(stats[1]) / Double.Parse(stats[0]) * 100) + "%</span>" + "</td>";//crit
                HTML_dmgstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[2] + " out of " + stats[0] + "hits \">" + (int)(Double.Parse(stats[2]) / Double.Parse(stats[0]) * 100) + "%</span>" + "</td>";//scholar
                HTML_dmgstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[3] + " out of " + stats[0] + "hits \">" + (int)(Double.Parse(stats[3]) / Double.Parse(stats[0]) * 100) + "%</span>" + "</td>";//sws
                HTML_dmgstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[4] + " out of " + stats[0] + "hits \">" + (int)(Double.Parse(stats[4]) / Double.Parse(stats[0]) * 100) + "%</span>" + "</td>";//flank
                HTML_dmgstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[10] + " out of " + stats[0] + "hits \">" + (int)(Double.Parse(stats[10]) / Double.Parse(stats[0]) * 100) + "%</span>" + "</td>";//glance
                HTML_dmgstats += "<td>" + stats[11] + "</td>";//misses
                HTML_dmgstats += "<td>" + stats[12] + "</td>";//interupts
                HTML_dmgstats += "<td>" + stats[13] + "</td>";//dmg invulned
                HTML_dmgstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[15] + "cancels \">" + stats[14] + "</span>" + " s</td>";//time wasted
                HTML_dmgstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[18] + "cancels \">" + stats[17] + "</span>" + " s</td>";//timesaved
                HTML_dmgstats += "<td>" + stats[5] + "</td>";//w swaps
                HTML_dmgstats += "<td>" + stats[6] + "</td>";//downs
                TimeSpan timedead = TimeSpan.FromMilliseconds(Double.Parse(stats[9]));//dead
                if (timedead > TimeSpan.Zero)
                {
                    HTML_dmgstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + "(" + (int)((timedead.TotalSeconds / fight_duration) * 100) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</span>" + " </td>";
                }
                else
                {
                    HTML_dmgstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died 100% Alive) \"> </span>" + " </td>";

                }
               
                HTML_dmgstats += "</tr>";
            }
            HTML_dmgstats += "</tbody></table>";

            //generate Tankstats table
            string HTML_defstats = " <script> $(function () { $('#defstats_table').DataTable({ \"order\": [[3, \"desc\"]]});});</script>" +
       " <table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"defstats_table\">" +
           " <thead><tr><th>Sub</th><th></th><th>Name</th>" +
           "<th>Dmg Taken" +
           "</th><th>Blocked" +
           "</th><th>Invulned" +
           "</th><th>Evaded" +
           "</th><th>Dodges" +
           "</th><th><img src=" + GetLink("Downs") + " alt=\"Downs\" title=\"Times downed\" height=\"18\" width=\"18\">" +
           "</th><th><img src=" + GetLink("Dead") + " alt=\"Dead\" title=\"Time died\" height=\"18\" width=\"18\">" + "</th>" +
               " </tr> </thead><tbody>";
            foreach (Player player in p_list)
            {
                HTML_defstats += "<tr>";
                HTML_defstats += "<td>" + player.getGroup().ToString() + "</td>";
                HTML_defstats += "<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>";
                HTML_defstats += "<td>" + player.getCharacter().ToString() + "</td>";

                string[] stats = getFinalDefenses(player);
                HTML_defstats += "<td>" + stats[0] + "</td>";//dmg taken
                HTML_defstats += "<td>" + stats[1] + "</td>";//Blocks
                HTML_defstats += "<td>" + stats[3] + "</td>";//invulns
                HTML_defstats += "<td>" + stats[6] + "</td>";//evades
                HTML_defstats += "<td>" + stats[5] + "</td>";//dodges
                HTML_defstats += "<td>" + stats[8] + "</td>";//downs
                TimeSpan timedead = TimeSpan.FromMilliseconds(Double.Parse(stats[9]));//dead
                if (timedead > TimeSpan.Zero)
                {
                    HTML_defstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + timedead + "(" + (int)((timedead.TotalSeconds / fight_duration) * 100) + "% Alive) \">" + timedead.Minutes + " m " + timedead.Seconds + " s</span>" + " </td>";
                }
                else
                {
                    HTML_defstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"Never died 100% Alive) \"> </span>" + " </td>";

                }
                HTML_defstats += "</tr>";
            }
            HTML_defstats += "</tbody></table>";

            //generate suppstats table
            string HTML_supstats = " <script> $(function () { $('#supstats_table').DataTable({ \"order\": [[3, \"desc\"]]});});</script>" +
       " <table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"supstats_table\">" +
           " <thead><tr><th>Sub</th><th></th><th>Name</th>" +
           "<th>Condi Cleanse" +
           "</th><th>Resurrects" +
           "</th>" +
               " </tr> </thead><tbody>";
            foreach (Player player in p_list)
            {
                HTML_supstats += "<tr>";
                HTML_supstats += "<td>" + player.getGroup().ToString() + "</td>";
                HTML_supstats += "<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>";
                HTML_supstats += "<td>" + player.getCharacter().ToString() + "</td>";

                string[] stats = getFinalSupport(player);
                HTML_supstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[3] + " seconds \">" + stats[2] + " condis</span>" + "</td>";//condicleanse                                                                                                                                                                   //HTML_defstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[6] + " Evades \">" + stats[7] + "dmg</span>" + "</td>";//evades
                HTML_supstats += "<td>" + "<span data-toggle=\"tooltip\" data-html=\"true\" data-placement=\"top\" title=\"" + stats[1] + " seconds \">" + stats[0] + "</span>" + "</td>";//res
                HTML_supstats += "</tr>";
            }
            HTML_supstats += "</tbody></table>";
            //Generate DPS graph
            string Html_dpsGraph=  
            "<div id=\"DPSGraph\" style=\"height: 600px;width:1200px; display:inline-block \"></div>" +
 "<script>";

            Html_dpsGraph += "var data = [";
            int maxDPS = 0;
            List<int[]> totalDpsAllPlayers = new List<int[]>();
            foreach (Player p in p_list) {
                //Adding dps axis
                List<int[]> playerbossdpsgraphdata = getBossDPSGraph(p);
                if (totalDpsAllPlayers.Count == 0)
                {
                    totalDpsAllPlayers = playerbossdpsgraphdata;
                }
              
                Html_dpsGraph += "{y: [";
                foreach (int[] dp in playerbossdpsgraphdata)
                {
                    Html_dpsGraph += "'" + dp[1] + "',";
                    if (dp[1] > maxDPS) { maxDPS = dp[1]; }
                    if (totalDpsAllPlayers.Count != 0)
                    {
                        if(totalDpsAllPlayers.FirstOrDefault(x => x[0] == dp[0]) != null)
                         totalDpsAllPlayers.FirstOrDefault(x => x[0] == dp[0])[1] += dp[1];
                    }
                }
                if (playerbossdpsgraphdata.Count == 0) {
                    Html_dpsGraph += "'0',";
                }
                //cuts off extra comma
                Html_dpsGraph = Html_dpsGraph.Substring(0, Html_dpsGraph.Length - 1);
                Html_dpsGraph += "],";
                //add time axis
                Html_dpsGraph += "x: [";
                foreach (int[] dp in playerbossdpsgraphdata)
                {
                    Html_dpsGraph += "'" + dp[0] + "',";
                }
                if (playerbossdpsgraphdata.Count == 0)
                {
                    Html_dpsGraph += "'0',";
                }
                //cuts off extra comma
                Html_dpsGraph = Html_dpsGraph.Substring(0, Html_dpsGraph.Length - 1);
                Html_dpsGraph += "],";
                Html_dpsGraph += " mode: 'lines'," +
                    " line: {shape: 'spline',color:'" + GetLink("Color-" + p.getProf()) + "'}," +


           " name: '" + p.getCharacter() + " DPS'" +
        "},";
                if (settingArray[0]) {//Turns display on or off
                    Html_dpsGraph += "{";
                    //Adding dps axis
                    List<int[]> playertotaldpsgraphdata = getTotalDPSGraph(p);
                    Html_dpsGraph += "y: [";
                    foreach (int[] dp in playertotaldpsgraphdata)
                    {
                        Html_dpsGraph += "'" + dp[1] + "',";
                    }
                    //cuts off extra comma
                    if (playertotaldpsgraphdata.Count == 0)
                    {
                        Html_dpsGraph += "'0',";
                    }
                    Html_dpsGraph = Html_dpsGraph.Substring(0, Html_dpsGraph.Length - 1);
                    Html_dpsGraph += "],";
                    //add time axis
                    Html_dpsGraph += "x: [";
                    foreach (int[] dp in playertotaldpsgraphdata)
                    {
                        Html_dpsGraph += "'" + dp[0] + "',";
                    }
                    if (playertotaldpsgraphdata.Count == 0)
                    {
                        Html_dpsGraph += "'0',";
                    }
                    //cuts off extra comma
                    Html_dpsGraph = Html_dpsGraph.Substring(0, Html_dpsGraph.Length - 1);
                    Html_dpsGraph += "],";
                    Html_dpsGraph += " mode: 'lines'," +
                         " line: {shape: 'spline',color:'" + GetLink("Color-" + p.getProf()) + "'}," +
                        "visible:'legendonly'," +


               " name: '" + p.getCharacter() + "TDPS'" + "},";
                }
           }
            //All Player dps
            Html_dpsGraph += "{";
            //Adding dps axis
            
            Html_dpsGraph += "y: [";
            foreach (int[] dp in totalDpsAllPlayers)
            {
                Html_dpsGraph += "'" + dp[1] + "',";
            }
            //cuts off extra comma
            Html_dpsGraph = Html_dpsGraph.Substring(0, Html_dpsGraph.Length - 1);
            Html_dpsGraph += "],";
            //add time axis
            Html_dpsGraph += "x: [";
            foreach (int[] dp in totalDpsAllPlayers)
            {
                Html_dpsGraph += "'" + dp[0] + "',";
            }
            //cuts off extra comma
            Html_dpsGraph = Html_dpsGraph.Substring(0, Html_dpsGraph.Length - 1);
            Html_dpsGraph += "],";
            Html_dpsGraph += " mode: 'lines'," +
                " line: {shape: 'spline'}," +
                  "visible:'legendonly'," +
       " name: 'All Player Dps'";
            Html_dpsGraph += "},";
            //Boss Health
            Html_dpsGraph += "{";
            //Adding dps axis

            Html_dpsGraph += "y: [";
            int bossDmgDone = 0;
            float scaler = boss_data.getHealth() / maxDPS;
            foreach (int[] dp in totalDpsAllPlayers)
            {
                bossDmgDone = (dp[1] * dp[0]);
                
                Html_dpsGraph += "'" + (boss_data.getHealth()- bossDmgDone)/scaler + "',";
            }
            //cuts off extra comma
            Html_dpsGraph = Html_dpsGraph.Substring(0, Html_dpsGraph.Length - 1);
            Html_dpsGraph += "],";
            //text axis is boss hp in %
            Html_dpsGraph += "text: [";
            int bossDmgDone2 = 0;
            float scaler2 = boss_data.getHealth() / 100;
            foreach (int[] dp in totalDpsAllPlayers)
            {
                bossDmgDone2 = (dp[1] * dp[0]);

                Html_dpsGraph += "'" + (boss_data.getHealth() - bossDmgDone2) / scaler2 + "% HP',";
            }
            //cuts off extra comma
            Html_dpsGraph = Html_dpsGraph.Substring(0, Html_dpsGraph.Length - 1);
            Html_dpsGraph += "],";
            //add time axis
            Html_dpsGraph += "x: [";
            foreach (int[] dp in totalDpsAllPlayers)
            {
                Html_dpsGraph += "'" + dp[0] + "',";
            }
            //cuts off extra comma
            Html_dpsGraph = Html_dpsGraph.Substring(0, Html_dpsGraph.Length - 1);
            Html_dpsGraph += "],";
            Html_dpsGraph += " mode: 'lines'," +
                " line: {shape: 'spline', dash: 'dashdot'}," +
                "hoverinfo: 'text'," +
       " name: 'Boss health'";
            Html_dpsGraph += "}";
            Html_dpsGraph += "];" +
"var layout = {" +

   
    "xaxis:{title:'DPS'},"+
      "yaxis:{title:'Time(sec)'}," +
    //"legend: { traceorder: 'reversed' }," +
    "hovermode: 'compare'," +
    "legend: {orientation: 'h'}," +
   // "yaxis: { title: 'DPS', domain: [0.51, 1] }," +
    

    "font: { color: '#ffffff' }," +
    "paper_bgcolor: 'rgba(0,0,0,0)'," +
    "plot_bgcolor: 'rgba(0,0,0,0)'" +
"};" +
        "Plotly.newPlot('DPSGraph', data, layout);" +
"</script> ";
            //Generate Boon table------------------------------------------------------------------------------------------------
            string Html_boons = " <script> $(function () { $('#boons_table').DataTable({ \"order\": [[3, \"desc\"]], " +
        "\"scrollX\": true," +
            " });});</script>" +
            " <table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"boons_table\">" +
                " <thead> <tr> <th>Sub</th><th></th><th>Name</th>";
            foreach (Boon boon in Boon.getList())
            {
                Html_boons += "<th>" + "<img src=\"" + GetLink(boon.getName()) + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>";
            }
            Html_boons += " </tr> </thead><tbody>";
            foreach (Player player in p_list)
            {
                Html_boons += "<tr>";
                Html_boons += "<td>" + player.getGroup().ToString() + "</td>";
                Html_boons += "<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>";
                Html_boons += "<td>" + player.getCharacter().ToString() + "</td>";
                string[] boonArray = getfinalboons(player, new List<int>());
                int count = 0;
                foreach (Boon boon in Boon.getList())
                {
                    Html_boons += "<td>" + boonArray[count] + "</td>";
                    count++;
                }
                Html_boons += "</tr>";
            }
            Html_boons += "</tbody></table>";

            //Generate BoonGenSelf table
            string Html_boonGenSelf = " <script> $(document).ready(function () { $('#boongenself_table').DataTable({ " +
            "\"scrollX\": true" +

            "});});</script>" +
            " <table class=\"display nowrap compact\" cellspacing=\"0\" width=\"100%\" id=\"boongenself_table\">" +
                " <thead> <tr> <th>Sub</th><th></th><th>Name</th>";
            foreach (Boon boon in Boon.getList())
            {
                Html_boonGenSelf += "<th>" + "<img src=\"" + GetLink(boon.getName()) + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>";
            }
            Html_boonGenSelf += " </tr> </thead><tbody>";

            foreach (Player player in p_list)
            {
                Html_boonGenSelf += "<tr>";
                Html_boonGenSelf += "<td>" + player.getGroup().ToString() + "</td>";
                Html_boonGenSelf += "<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>";
                Html_boonGenSelf += "<td>" + player.getCharacter().ToString() + "</td>";

                List<int> playerID = new List<int>();
                playerID.Add(player.getInstid());
                string[] boonArray = getfinalboons(player, playerID);

                int count = 0;
                foreach (Boon boon in Boon.getList())
                {
                    Html_boonGenSelf += "<td>" + boonArray[count] + "</td>";
                    count++;
                }
                Html_boonGenSelf += "</tr>";
            }
            Html_boonGenSelf += "</tbody></table>";


            //Generate BoonGenGroup table
            string Html_boonGenGroup = " <script> $(function () { $('#boongengroup_table').DataTable({ \"order\": [[3, \"desc\"]], " +
        "\"scrollX\": true," +
            "});});</script>" +
            " <table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"boongengroup_table\">" +
                " <thead> <tr> <th>Sub</th><th></th><th>Name</th>";
            foreach (Boon boon in Boon.getList())
            {
                Html_boonGenGroup += "<th>" + "<img src=\"" + GetLink(boon.getName()) + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>";
            }
            Html_boonGenGroup += " </tr> </thead><tbody>";
            List<int> playerIDS = new List<int>();

            foreach (Player player in p_list)
            {
                Html_boonGenGroup += "<tr>";
                Html_boonGenGroup += "<td>" + player.getGroup().ToString() + "</td>";
                Html_boonGenGroup += "<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>";
                Html_boonGenGroup += "<td>" + player.getCharacter().ToString() + "</td>";

                foreach (Player p in p_list)
                {
                    if (p.getGroup() == player.getGroup())
                        playerIDS.Add(p.getInstid());
                }
                string[] boonArray = getfinalboons(player, playerIDS);
                playerIDS = new List<int>();
                int count = 0;
                foreach (Boon boon in Boon.getList())
                {
                    Html_boonGenGroup += "<td>" + boonArray[count] + "</td>";
                    count++;
                }
                Html_boonGenGroup += "</tr>";
            }
            Html_boonGenGroup += "</tbody></table>";

            //Generate BoonGenOGroup table
            string Html_boonGenOGroup = " <script> $(function () { $('#boongenogroup_table').DataTable({ \"order\": [[3, \"desc\"]], " +
        "\"scrollX\": true," +
            "});});</script>" +
            " <table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"boongenogroup_table\">" +
                " <thead> <tr> <th>Sub</th><th></th><th>Name</th>";
            foreach (Boon boon in Boon.getList())
            {
                Html_boonGenOGroup += "<th>" + "<img src=\"" + GetLink(boon.getName()) + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>";
            }
            Html_boonGenOGroup += " </tr> </thead><tbody>";
            playerIDS = new List<int>();

            foreach (Player player in p_list)
            {
                Html_boonGenOGroup += "<tr>";
                Html_boonGenOGroup += "<td>" + player.getGroup().ToString() + "</td>";
                Html_boonGenOGroup += "<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>";
                Html_boonGenOGroup += "<td>" + player.getCharacter().ToString() + "</td>";

                foreach (Player p in p_list)
                {
                    if (p.getGroup() != player.getGroup())
                        playerIDS.Add(p.getInstid());
                }
                string[] boonArray = getfinalboons(player, playerIDS);
                playerIDS = new List<int>();
                int count = 0;
                foreach (Boon boon in Boon.getList())
                {
                    Html_boonGenOGroup += "<td>" + boonArray[count] + "</td>";
                    count++;
                }
                Html_boonGenOGroup += "</tr>";
            }
            Html_boonGenOGroup += "</tbody></table>";

            //Generate BoonGenSquad table
            string Html_boonGenSquad = " <script> $(function () { $('#boongensquad_table').DataTable({ \"order\": [[5, \"desc\"]], " +
        "\"scrollX\": true," +
            "});});</script>" +
            " <table class=\"display table table-striped table-hover compact\"  cellspacing=\"0\" width=\"100%\" id=\"boongensquad_table\">" +
                " <thead> <tr> <th>Sub</th><th></th><th>Name</th>";
            foreach (Boon boon in Boon.getList())
            {
                Html_boonGenSquad += "<th>" + "<img src=\"" + GetLink(boon.getName()) + " \" alt=\"" + boon.getName() + "\" title =\" " + boon.getName() + "\" height=\"18\" width=\"18\" >" + "</th>";
            }
            Html_boonGenSquad += " </tr> </thead><tbody>";

            playerIDS = new List<int>();
            foreach (Player p in p_list)
            {
                playerIDS.Add(p.getInstid());
            }
            foreach (Player player in p_list)
            {
                Html_boonGenSquad += "<tr>";
                Html_boonGenSquad += "<td>" + player.getGroup().ToString() + "</td>";
                Html_boonGenSquad += "<td>" + "<img src=\"" + GetLink(player.getProf().ToString()) + " \" alt=\"" + player.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</td>";
                Html_boonGenSquad += "<td>" + player.getCharacter().ToString() + "</td>";


                string[] boonArray = getfinalboons(player, playerIDS);

                int count = 0;
                foreach (Boon boon in Boon.getList())
                {
                    Html_boonGenSquad += "<td>" + boonArray[count] + "</td>";
                    count++;
                }
                Html_boonGenSquad += "</tr>";
            }
            Html_boonGenSquad += "</tbody></table>";

            //generate Player list Graphs
            string Html_playerDropdown = "";
            string Html_playertabs = "";
            foreach (Player p in p_list) {
               
                string charname = p.getCharacter();
                Html_playerDropdown += "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#"+ p.getInstid()+"\">"+charname + 
                    "<img src=\"" + GetLink(p.getProf().ToString()) + " \" alt=\"" + p.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</a>";
                Html_playertabs += "<div class=\"tab-pane fade\" id=\"" + p.getInstid() + "\">" +
                    "<h1 align=\"center\"> " + charname + "<img src=\"" + GetLink(p.getProf().ToString()) + " \" alt=\"" + p.getProf().ToString() + "\" height=\"18\" width=\"18\" >" + "</h1>" +

                        "<div id=\"Graph" + p.getInstid() + "\" style=\"height: 800px;width:1000px; display:inline-block \"></div>" +
   "<script>";
                CombatData c_data = getCombatData();
                List<CastLog> casting = p.getCastLogs(b_data, c_data.getCombatList(), getAgentData());
                SkillData s_data = getSkillData();
                Html_playertabs += "var data = [";
                if (settingArray[7])//Display rotation
                {
                    foreach (CastLog cl in casting)
                    {
                        Html_playertabs += "{" +
                            "y: ['1']," +
                            "x: ['" + cl.getActDur() / 1000f + "']," +
                            "base:'" + cl.getTime() / 1000f + "'," +
                            "name: '" + cl.getID() + "'," +//get name should be handled by api
                            "orientation:'h'," +
                            "mode: 'markers'," +
                            "type: 'bar'," +
                            "width:'1'," +
                            "hoverinfo: 'name'," +
                            "hoverlabel:{namelength:'-1'}," +
                            " marker: {";

                        if (cl.endActivation().getID() == 3)
                        {
                            Html_playertabs += "color: 'rgb(40,40,220)',";
                        }
                        else if (cl.endActivation().getID() == 4)
                        {
                            Html_playertabs += "color: 'rgb(220,40,40)',";
                        }
                        else if (cl.endActivation().getID() == 5)
                        {
                            Html_playertabs += "color: 'rgb(40,220,40)',";
                        }

                        Html_playertabs += " width: 5," +
                         "line:" +
                          "{";
                        if (cl.startActivation().getID() == 1)
                        {
                            Html_playertabs += "color: 'rgb(20,20,20)',";
                        }
                        else if (cl.startActivation().getID() == 2)
                        {
                            Html_playertabs += "color: 'rgb(220,40,220)',";
                        }

                        Html_playertabs += "width: 1.5" +
                                "}" +
                            "}," +
                            "showlegend: false" +
                        " },";

                    }
                }

                if (settingArray[3] || settingArray[4]|| settingArray[5]) {
                    List<string> parseBoonsList = new List<string>();
                    if (settingArray[3]) {//Main boons
                        parseBoonsList.AddRange(getMainBoons());

                    }
                    else if (settingArray[4]|| settingArray[5]) {//Important Class specefic boons
                        parseBoonsList.AddRange(getImportantPorfBoons());
                    }
                    else if (settingArray[5]) {//All class specefic boons
                        parseBoonsList.AddRange(getAllProfBoons());
                       
                    } 
                    List<BoonsGraphModel> boonGraphData = getBoonGraph(p);
                    boonGraphData.Reverse();
                    foreach (BoonsGraphModel bgm in boonGraphData) {
                        if (parseBoonsList.Contains(bgm.getBoonName()))
                        {
                            Html_playertabs += "{";
                            Html_playertabs +=
                                "y: [";
                            foreach (Point pnt in bgm.getBoonChart())
                            {
                                Html_playertabs += "'" + pnt.Y + "',";
                            }
                            if (bgm.getBoonChart().Count == 0)
                            {
                                Html_playertabs += "'0',";
                            }
                            //cuts off extra comma
                            Html_playertabs = Html_playertabs.Substring(0, Html_playertabs.Length - 1);

                            Html_playertabs += "]," +
                            "x: [";
                            foreach (Point pnt in bgm.getBoonChart())
                            {
                                Html_playertabs += "'" + pnt.X + "',";
                            }
                            if (bgm.getBoonChart().Count == 0)
                            {
                                Html_playertabs += "'0',";
                            }
                            //cuts off extra comma
                            Html_playertabs = Html_playertabs.Substring(0, Html_playertabs.Length - 1);

                            Html_playertabs += "]," +
                                " yaxis: 'y2'," +
                                " type: 'scatter',";
                            if (bgm.getBoonName() == "Might" || bgm.getBoonName() == "Quickness") { }
                            else
                            {
                                Html_playertabs += " visible: 'legendonly',";
                            }
                            Html_playertabs += " line: {color:'" + GetLink("Color-" + bgm.getBoonName()) + "'},";
                            Html_playertabs += " fill: 'tozeroy'," +
                                 " name: '" + bgm.getBoonName() + "'" +
                                 " },";
                        }

                    }
                }
                if (settingArray[2]) {//show boss dps plot
                    //Adding dps axis
                    List<int[]> playerbossdpsgraphdata = getBossDPSGraph(p);
                    Html_playertabs += "{";
                    Html_playertabs += "y: [";
                    foreach (int[] dp in playerbossdpsgraphdata) {
                        Html_playertabs += "'" + dp[1] + "',";
                    }
                    if (playerbossdpsgraphdata.Count == 0) {
                        Html_playertabs += "'0',";
                    }
                    //cuts off extra comma
                    Html_playertabs = Html_playertabs.Substring(0, Html_playertabs.Length - 1);
                    Html_playertabs += "],";
                    //add time axis
                    Html_playertabs += "x: [";
                    foreach (int[] dp in playerbossdpsgraphdata)
                    {
                        Html_playertabs += "'" + dp[0] + "',";
                    }
                    if (playerbossdpsgraphdata.Count == 0)
                    {
                        Html_playertabs += "'0',";
                    }
                    //cuts off extra comma
                    Html_playertabs = Html_playertabs.Substring(0, Html_playertabs.Length - 1);
                    Html_playertabs += "],";
                    Html_playertabs += " mode: 'lines'," +
                        " line: {shape: 'spline',color:'" + GetLink("Color-" + p.getProf()) + "'}," +

               " yaxis: 'y3'," +

               " name: 'Boss DPS'" +
            "}," ;
                 }
                if (settingArray[1]) {//show total dps plot
                    Html_playertabs += "{";
                    //Adding dps axis
                    List<int[]> playertotaldpsgraphdata = getTotalDPSGraph(p);
                    Html_playertabs += "y: [";
                    foreach (int[] dp in playertotaldpsgraphdata)
                    {
                        Html_playertabs += "'" + dp[1] + "',";
                    }
                    if (playertotaldpsgraphdata.Count == 0)
                    {
                        Html_playertabs += "'0',";
                    }
                    //cuts off extra comma
                    Html_playertabs = Html_playertabs.Substring(0, Html_playertabs.Length - 1);
                    Html_playertabs += "],";
                    //add time axis
                    Html_playertabs += "x: [";
                    foreach (int[] dp in playertotaldpsgraphdata)
                    {
                        Html_playertabs += "'" + dp[0] + "',";
                    }
                    if (playertotaldpsgraphdata.Count == 0)
                    {
                        Html_playertabs += "'0',";
                    }
                    //cuts off extra comma
                    Html_playertabs = Html_playertabs.Substring(0, Html_playertabs.Length - 1);
                    Html_playertabs += "],";
                    Html_playertabs += " mode: 'lines'," +
                        " line: {shape: 'spline',color:'rgb(0,250,0)'}," +
               " yaxis: 'y3'," +

               " name: 'Total DPS'" + "}";
               }
                Html_playertabs += "];" +
    "var layout = {"+

        "yaxis: {"+
            "title: 'Rotation', domain: [0, 0.09], fixedrange: true, showgrid: false,"+
            "range: [0, 2]"+
        "},"+
           
        "legend: { traceorder: 'reversed' },"+
        "hovermode: 'compare',"+
        "yaxis2: { title: 'Boons', domain: [0.11, 0.50], fixedrange: true },"+
        "yaxis3: { title: 'DPS', domain: [0.51, 1] },"+
        "images: ["+
            "{"+
                "source: 'https://render.guildwars2.com/file/660B4E695A6026F9D70CA6B0D7565774805C6B0E/103255.png',"+
                "xref: 'x',"+
                "yref: 'y',"+
                "x: 1,"+
                "y: 0,"+
                "sizex: 0.8,"+
                "sizey: 0.8,"+
                "xanchor: 'left',"+
                "yanchor: 'bottom'"+
            "},{"+
                    "source: 'https://render.guildwars2.com/file/66441BE066120A2590B5AF031D56E221189ADC68/103158.png',"+
                "xref: 'x',"+
                "yref: 'y',"+
                "x: 2,"+
                "y: 0,"+
                "sizex: 0.8,"+
                "sizey: 0.8,"+
                "xanchor: 'left',"+
                "yanchor: 'bottom'"+
            "}],"+
     
        "font: { color: '#ffffff' },"+
        "paper_bgcolor: 'rgba(0,0,0,0)',"+
        "plot_bgcolor: 'rgba(0,0,0,0)'"+
    "};"+
            "Plotly.newPlot('Graph"+p.getInstid()+"', data, layout);"+
"</script> "+
                        "</div>";
            }


           
            string HTML_Body = "<body><div class=\"container\"><p>ARC:" + getLogData().getBuildVersion().ToString() + " | Bossid " + getBossData().getID().ToString() + "</p>";
               
            if (log_data.getBosskill()) {
                HTML_Body += "<p class='text text-success'> Result: Success";
            } else {
                HTML_Body += "<p class='text text-warning'> Result: Fail";
            }

            
                HTML_Body += " | Duration " + durationString + " </p> " +
                 "<p> Time Start: "+log_data.getLogStart() + " | Time End: " + log_data.getLogEnd() + " </p> " +
                    //top 
                    "<div class=\"row\">" +
                        //Boss deets
                        "<div class=\"col-md-4 \"><div class=\"card border-danger\">" +
                        "<h3 class=\"card-header\">" + bossname + "</h3>" +

                         "<div class=\"card-body\"><blockquote class=\"card-blockquote\">" +
                          "<div class=\"row\">" +
                         "<div class=\"col-md-6 \">" +
                            "<center><img src=\"" + GetLink(bossname + "-icon") + " \"alt=\"" + bossname + "-icon" + "\" style=\"height: 100px; width: 100 %; display: block; \" ></center>" +
                           "</div>" + "<div class=\"col-md-6 \">" +
                            "<div class=\"progress\" style=\"width: 100 %; height: 20px;\"><div class=\"progress-bar bg-danger\" role=\"progressbar\" style=\"width:100%; ;display: inline-block;\" aria-valuenow=\"100\" aria-valuemin=\"0\" aria-valuemax=\"100\"><p style=\"text-align:center; color: #FFF;\">" + getBossData().getHealth().ToString() + " Health</p></div></div>" +
                             //"<div class=\"progress\" style=\"width: 100 %; height: 20px; \"><div class=\"progress-bar-striped \" role=\"progressbar\" style=\"width:100%; display: inline-block;\" aria-valuenow=\"100\" aria-valuemin=\"0\" aria-valuemax=\"100\"><p style=\"text-align:center; color: #FFF;\">" + 0 + " Armour(" + getBossData().getTough().ToString() + " Toughness)</p></div></div>" +
                           "</div>" + "</blockquote></div></div> " +
                        "</div>" +
                       
                        //Raid Party
                        "<div class=\"col-md-7\">" +
                        HTML_compTable +
                        "<p>File recorded by:"+log_data.getPOV() +"</p>"+
                        //"<p>Tip: Build Icons appear if player has more than 0 of that stat. 0 = lowest stat in squad, 10 = highest stat in squad, in between is a scaler representation</p>" +
                        "</div>" +
                    "</div>" +
                    "<ul class=\"nav nav-tabs\">" +
                        "<li class=\"nav-item dropdown\">" +
                         "<a class=\"nav-link dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"true\">Stats</a>" +
                             "<div class=\"dropdown-menu \" x-placement=\"bottom-start\" style=\"position:absolute; transform:translate3d(0px, 40px, 0px); top: 0px; left: 0px; will-change: transform;\">" +


                                "<a class=\"dropdown-item \"  data-toggle=\"tab\" href=\"#dpsStats\">DPS</a>" +
                                "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#offStats\">Damage Stats</a>" +
                                "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#defStats\">Defence Stats</a>" +
                                "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#healStats\">Heal Stats</a>" +

                            "</div>" +
                       
                        "</li>" +
                        "<li class=\"nav-item\">" +
                            "<a class=\"nav-link\" data-toggle=\"tab\" href=\"#dmgGraph\">Damage Graph</a>" +
                        "</li>" +
                        "<li class=\"nav-item dropdown\">" +
                        "<a class=\"nav-link dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"true\">Boons</a>" +
                             "<div class=\"dropdown-menu \" x-placement=\"bottom-start\" style=\"position:absolute; transform:translate3d(0px, 40px, 0px); top: 0px; left: 0px; will-change: transform;\">" +


                                "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#boonsUptime\">Boon Uptime</a>" +
                                "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#boonsGenSelf\">Boon Geration(Self)</a>" +
                                "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#boonsGenGroup\">Boon Generation(Group)</a>" +
                                "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#boonsGenOGroup\">Boon Generation(Off Group)</a>" +
                                "<a class=\"dropdown-item\"  data-toggle=\"tab\" href=\"#boonsGenSquad\">Boon Generation(Squad)</a>" +
                            "</div>" +

                        "</li>" +
                         "<li class=\"nav-item dropdown\">" +
                        "<a class=\"nav-link dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"true\">Player</a>" +
                             "<div class=\"dropdown-menu \" x-placement=\"bottom-start\" style=\"position:absolute; transform:translate3d(0px, 40px, 0px); top: 0px; left: 0px; will-change: transform;\">" +
                             //Foreach player loop here
                             Html_playerDropdown +
                            "</div>" +
                             
                        "</li>" +
                    "</ul>" +
                    "<div id=\"myTabContent\" class=\"tab-content\">" +
                         "<div class=\"tab-pane fade show active\" id=\"dpsStats\">" +
                            //table
                            HTML_dps +
                         "</div>" +
                          "<div class=\"tab-pane fade \" id=\"offStats\">" +
                           HTML_dmgstats +

                         "</div>" +
                          "<div class=\"tab-pane fade \" id=\"defStats\">" +
                           HTML_defstats +

                         "</div>" +
                          "<div class=\"tab-pane fade\" id=\"healStats\">" +
                           HTML_supstats +

                         "</div>" +
                         "<div class=\"tab-pane fade\" id=\"dmgGraph\">" +
                         //DMG Graph
                         Html_dpsGraph +
                         "</div>" +
                         "<div class=\"tab-pane fade\" id=\"boonsUptime\">" +
                         "<p> Boon Uptime</p>" +
                         //Boon Stats
                         Html_boons +
                         "</div>" +
                         "<div class=\"tab-pane fade\" id=\"boonsGenSelf\">" +
                          "<p> Boons generated by a character for themselves</p>" +
                          Html_boonGenSelf +
                         "</div>" +
                          "<div class=\"tab-pane fade\" id=\"boonsGenGroup\">" +
                         "<p> Boons generated by a character for their sub group</p>" +
                          Html_boonGenGroup +
                         "</div>" +
                          "<div class=\"tab-pane fade\" id=\"boonsGenOGroup\">" +
                         "<p> Boons generated by a character for any subgroup that is not their own</p>" +
                          Html_boonGenOGroup +
                         "</div>" +
                          "<div class=\"tab-pane fade\" id=\"boonsGenSquad\">" +
                          "<p> Boons generated by a character for the entire squad</p>" +
                          Html_boonGenSquad +
                         "</div>" +
                         Html_playertabs+
                    "</div></div>";// +
                            
            HTML_Body += "</body> <script> $(document).ready(function(){$('[data-toggle=\"tooltip\"]').tooltip(); });</script >";

            string HTML_foot = "</html>";
            HTML_CONTENT = HTML_Head + HTML_Body + HTML_foot;

           

            return HTML_CONTENT;
        }
        //Easy reference to links/color codes
        public string GetLink(string name)
        {
            switch (name)
            {
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
                case"Color-DragonHunter": return "rgb(114,193,217)";
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
                    return "https://wiki.guildwars2.com/images/6/68/Blind.png";
                case "Interupts":
                    return "https://wiki.guildwars2.com/images/thumb/7/79/Daze.png/20px-Daze.png";
                case "Invuln":
                    return "https://wiki.guildwars2.com/images/e/eb/Determined.png";
                case "Wasted":
                    return "https://wiki.guildwars2.com/images/b/b3/Out_Of_Health_Potions.png";
                case "Saved":
                    return "https://wiki.guildwars2.com/images/e/eb/Ready.png";
                case "Swap":
                    return "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png";

                case "743": return "https://wiki.guildwars2.com/images/e/e5/Aegis.png";
                case "Fury": return "https://wiki.guildwars2.com/images/4/46/Fury.png";
                case "Might": return "https://wiki.guildwars2.com/images/7/7c/Might.png";
                case "Protection": return "https://wiki.guildwars2.com/images/6/6c/Protection.png";
                case "Quickness": return "https://wiki.guildwars2.com/images/b/b4/Quickness.png";
                case "Regeneration": return "https://wiki.guildwars2.com/images/5/53/Regeneration.png";
                case "Resistance": return "https://wiki.guildwars2.com/images/thumb/e/e9/Resistance_40px.png/20px-Resistance_40px.png";
                case "Retaliation": return "https://wiki.guildwars2.com/images/5/53/Retaliation.png";
                case "Stability": return "https://wiki.guildwars2.com/images/a/ae/Stability.png";
                case "Swiftness": return "https://wiki.guildwars2.com/images/a/af/Swiftness.png";
                case "Vigor": return "https://wiki.guildwars2.com/images/f/f4/Vigor.png";

                case "Alacrity": return "https://wiki.guildwars2.com/images/thumb/4/4c/Alacrity.png/20px-Alacrity.png";
                case "Glyph of Empowerment": return "https://wiki.guildwars2.com/images/thumb/f/f0/Glyph_of_Empowerment.png/33px-Glyph_of_Empowerment.png";
                case "Grace of the Land": return "https://wiki.guildwars2.com/images/thumb/4/45/Grace_of_the_Land.png/25px-Grace_of_the_Land.png";
                case "Sun Spirit": return "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png";
                case "Spirit of Frost": return "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png";
                case "Banner of Strength": return "https://wiki.guildwars2.com/images/thumb/e/e1/Banner_of_Strength.png/33px-Banner_of_Strength.png";
                case "Banner of Discipline": return "https://wiki.guildwars2.com/images/thumb/5/5f/Banner_of_Discipline.png/33px-Banner_of_Discipline.png";
                case "Spotter": return "https://wiki.guildwars2.com/images/b/b0/Spotter.png";
                case "Stone Spirit": return "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png";
                case "Storm Spirit": return "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png";
                case "Empower Allies": return "https://wiki.guildwars2.com/images/thumb/4/4c/Empower_Allies.png/20px-Empower_Allies.png";

                case "Color-743": return "rgb(,,)";
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
        //This whole way of doing it is gonna change too ineffecient
        public List<string> getMainBoons() {
            List<string> parseBoonsList = new List<string>();
            parseBoonsList.Add("Might");//740
            parseBoonsList.Add("Fury");
            parseBoonsList.Add("Quickness");
            parseBoonsList.Add("Alacrity");
            parseBoonsList.Add("Protection");//717
            parseBoonsList.Add("Regeneration");//718
            parseBoonsList.Add("Vigor");
            parseBoonsList.Add("Stability");
            parseBoonsList.Add("Swiftness");//719
            parseBoonsList.Add("Retaliation");
            parseBoonsList.Add("Resistance");

            return parseBoonsList;
        }

        public List<string> getImportantPorfBoons() {
            List<string> parseBoonsList = new List<string>();

            parseBoonsList.Add("Spotter");
            parseBoonsList.Add("Spirit of Frost");
            parseBoonsList.Add("Sun Spirit");
            parseBoonsList.Add("Stone Spirit");
            parseBoonsList.Add("Empower Allies");
            parseBoonsList.Add("Banner of Strength");
            parseBoonsList.Add("Banner of Discipline");

            return parseBoonsList;
        }

        public List<string> getAllProfBoons() {
            List<string> parseBoonsList = new List<string>();

            parseBoonsList.Add("Stealth");
            parseBoonsList.Add("Superspeed");//5974
            parseBoonsList.Add("Invulnerability");
            //Auras
            parseBoonsList.Add("Chaos Armor");
            parseBoonsList.Add("Fire Shield");//5677
            parseBoonsList.Add("Frost Aura");//5579
            parseBoonsList.Add("Light Aura");
            parseBoonsList.Add("Magnetic Aura");//5684
            parseBoonsList.Add("Shocking Aura");//5577
            //Signets
            parseBoonsList.Add("Signet of Resolve");
            parseBoonsList.Add("Bane Signet");
            parseBoonsList.Add("Signet of Judgment");
            parseBoonsList.Add("Signet of Mercy");
            parseBoonsList.Add("Signet of Wrath");
            parseBoonsList.Add("Signet of Courage");
            parseBoonsList.Add("Healing Signet");
            parseBoonsList.Add("Dolyak Signet");
            parseBoonsList.Add("Signet of Fury");
            parseBoonsList.Add("Signet of Might");
            parseBoonsList.Add("Signet of Stamina");
            parseBoonsList.Add("Signet of Rage");
            parseBoonsList.Add("Signet of Renewal");
            parseBoonsList.Add("Signet of Stone");
            parseBoonsList.Add("Signet of the Hunt");
            parseBoonsList.Add("Signet of the Wild");
            parseBoonsList.Add("Signet of Malice");
            parseBoonsList.Add("Assassin's Signet");
            parseBoonsList.Add("Infiltrator's Signet");
            parseBoonsList.Add("Signet of Agility");
            parseBoonsList.Add("Signet of Shadows");
            parseBoonsList.Add("Signet of Restoration");//739
            parseBoonsList.Add("Signet of Air");//5590
            parseBoonsList.Add("Signet of Earth");//5592
            parseBoonsList.Add("Signet of Fire");//5544
            parseBoonsList.Add("Signet of Water");//5591
            parseBoonsList.Add("Signet of the Ether");
            parseBoonsList.Add("Signet of Domination");
            parseBoonsList.Add("Signet of Illusions");
            parseBoonsList.Add("Signet of Inspiration");
            parseBoonsList.Add("Signet of Midnight");
            parseBoonsList.Add("Signet of Humility");
            parseBoonsList.Add("Signet of Vampirism");
            parseBoonsList.Add("Plague Signet");
            parseBoonsList.Add("Signet of Spite");
            parseBoonsList.Add("Signet of the Locust");
            parseBoonsList.Add("Signet of Undeath");
            //Transforms
            parseBoonsList.Add("Rampage");
            parseBoonsList.Add("Elixir S");
            parseBoonsList.Add("Elixir X");
            parseBoonsList.Add("Tornado");//5534
            parseBoonsList.Add("Whirlpool");
            parseBoonsList.Add("Lich Form");
            parseBoonsList.Add("Become the Bear");
            parseBoonsList.Add("Become the Raven");
            parseBoonsList.Add("Become the Snow Leopard");
            parseBoonsList.Add("Become the Wolf");
            parseBoonsList.Add("Avatar of Melandru");//12368
            //Not really but basically transforms
           
            parseBoonsList.Add("Death Shroud");
            parseBoonsList.Add("Reaper's Shroud");
            parseBoonsList.Add("Celestial Avatar");
            parseBoonsList.Add("Reaper of Grenth");//12366
            //Profession specefic effects
            //ele
                //attunments
            parseBoonsList.Add("Fire Attunement");//5585
            parseBoonsList.Add("Water Attunement");
            parseBoonsList.Add("Air Attunement");//5575
            parseBoonsList.Add("Earth Attunement");//5580
                 //forms
            parseBoonsList.Add("Mist Form");//5543
            parseBoonsList.Add("Ride the Lightning");//5588
            parseBoonsList.Add("Vapor Form");
                //conjures
            parseBoonsList.Add("Conjure Earth Attributes");//15788
            parseBoonsList.Add("Conjure Flame Attributes");//15789
            parseBoonsList.Add("Conjure Frost Attributes");//15790
            parseBoonsList.Add("Conjure Lightning Attributes");//15791
            parseBoonsList.Add("Conjure Fire Attributes");//15792
                //Extras
            parseBoonsList.Add("Arcane Power");//5582
            parseBoonsList.Add("Arcane Shield");//5640
            parseBoonsList.Add("Renewal of Fire");//5764
            parseBoonsList.Add("Glyph of Elemental Power");//5739 5741 5740 5742
            parseBoonsList.Add("Rebound");//31337
            parseBoonsList.Add("Rock Barrier");//34633 750
            parseBoonsList.Add("Magnetic Wave");//15794
            parseBoonsList.Add("Obsidian Flesh");//5667
                //Traits
            parseBoonsList.Add("Harmonious Conduit");//31353
            parseBoonsList.Add("bleh");
            parseBoonsList.Add("bleh");
            parseBoonsList.Add("bleh");
            parseBoonsList.Add("bleh");
            parseBoonsList.Add("bleh");
            return parseBoonsList;

        }
    }
}
