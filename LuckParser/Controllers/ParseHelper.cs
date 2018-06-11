using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Controllers
{
    class ParseHelper
    {
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
    }

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
                    is_ninety, is_fifty, is_moving, is_statechange, is_flanking, is_shields));
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
            List<AgentItem> playerAgentList = getAgentData().getPlayerAgentList();

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

}
