using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuckParser.Models.ParseModels;
using System.Drawing;
using System.IO.Compression;

//recomend CTRL+M+O to collapse all
using LuckParser.Models.DataModels;
using System.Globalization;

//recommend CTRL+M+O to collapse all
namespace LuckParser.Controllers
{
    public class Parser
    {
        private GW2APIController APIController = new GW2APIController();

        //Main data storage after binary parse
        private LogData log_data;
        private BossData boss_data;
        private AgentData agent_data = new AgentData();
        private SkillData skill_data = new SkillData();
        private CombatData combat_data = new CombatData();
        private MechanicData mech_data = new MechanicData();
        private List<Player> p_list = new List<Player>();
        private Boss boss;
        private byte revision;

        // Public Methods
        public LogData getLogData()
        {
            return log_data;
        }
        public BossData getBossData()
        {
            return boss_data;
        }

        public ParsedLog GetParsedLog()
        {
            return new ParsedLog(log_data, boss_data, agent_data, skill_data, combat_data, mech_data, p_list, boss);
        }

        //Main Parse method------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Parses the given log
        /// </summary>
        /// <param name="bg">BackgroundWorker handling the log</param>
        /// <param name="row">GridRow object bound to the UI</param>
        /// <param name="evtc">The path to the log to parse</param>
        /// <returns></returns>
        public void ParseLog(GridRow row, string evtc)
        {
            using(var fs = new FileStream(evtc, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if(evtc.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    using(var arch = new ZipArchive(fs, ZipArchiveMode.Read))
                    {
                        if(arch.Entries.Count != 1)
                        {
                            throw new CancellationException(row, new InvalidDataException("Invalid Archive"));
                        }
                        using(var data = arch.Entries[0].Open())
                        {
                            ParseLog(row, data);
                        }
                    }
                }
                else
                {
                    ParseLog(row, fs);
                }
            }
        }

        private void ParseLog(GridRow row, Stream stream)
        {
            try
            {
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "15% - Parsing boss data...", 15);
                parseBossData(stream);
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "20% - Parsing agent data...", 20);
                parseAgentData(stream);
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "25% - Parsing skill data...", 25);
                parseSkillData(stream);
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "30% - Parsing combat list...", 30);
                parseCombatList(stream);                
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "35% - Pairing data...", 35);
                fillMissingData();
                row.BgWorker.ThrowIfCanceled(row);
            }
            catch(Exception ex) when (!(ex is CancellationException))
            {
                throw new CancellationException(row, ex);
            }
        }

        private static BinaryReader CreateReader(Stream stream)
        {
            return new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        }

        private static bool TryRead(Stream stream, byte[] data)
        {
            int offset = 0;
            int count  = data.Length;
            while(count > 0)
            {
                var bytesRead = stream.Read(data, offset, count);
                if(bytesRead == 0)
                {
                    return false;
                }
                offset += bytesRead;
                count  -= bytesRead;
            }
            return true;
        }

        //sub Parse methods
        /// <summary>
        /// Parses boss related data
        /// </summary>
        private void parseBossData(Stream stream)
        {
            using (var reader = CreateReader(stream))
            {
                // 12 bytes: arc build version
                var build_version = ParseHelper.getString(stream, 12);
                this.log_data = new LogData(build_version);

                // 1 byte: skip
                this.revision = reader.ReadByte();

                // 2 bytes: boss instance ID
                ushort id = reader.ReadUInt16();
                // 1 byte: position
                ParseHelper.safeSkip(stream, 1);

                //Save
                // TempData["Debug"] = build_version +" "+ instid.ToString() ;
                this.boss_data = new BossData(id);
            }
        }

        /// <summary>
        /// Parses agent related data
        /// </summary>
        private void parseAgentData(Stream stream)
        {
            using (var reader = CreateReader(stream))
            {
                // 4 bytes: player count
                int player_count = reader.ReadInt32();

                // 96 bytes: each player
                for (int i = 0; i < player_count; i++)
                {
                    // 8 bytes: agent
                    ulong agent = reader.ReadUInt64();

                    // 4 bytes: profession
                    uint prof = reader.ReadUInt32();

                    // 4 bytes: is_elite
                    uint is_elite = reader.ReadUInt32();

                    // 2 bytes: toughness
                    int toughness = reader.ReadInt16();
                    // skip concentration
                    ParseHelper.safeSkip(stream, 2);
                    // 2 bytes: healing
                    int healing = reader.ReadInt16();
                    ParseHelper.safeSkip(stream, 2);
                    // 2 bytes: condition
                    int condition = reader.ReadInt16();
                    ParseHelper.safeSkip(stream, 2);
                    // 68 bytes: name
                    String name = ParseHelper.getString(stream, 68, false);
                    //Save
                    Agent a = new Agent(agent, name, prof, is_elite);
                    var agent_prof = a.getProf(this.log_data.getBuildVersion(), APIController);
                    switch(agent_prof)
                    {
                        case "NPC":
                            // NPC
                            agent_data.addItem(new AgentItem(agent, name, a.getName() + ":" + prof.ToString().PadLeft(5, '0')), agent_prof);
                            break;
                            // Gadget
                        case "GDG":
                            agent_data.addItem(new AgentItem(agent, name, a.getName() + ":" + (prof & 0x0000ffff).ToString().PadLeft(5, '0')), agent_prof);
                            break;
                        default:
                            // Player
                            agent_data.addItem(new AgentItem(agent, name, agent_prof, toughness, healing, condition), agent_prof);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Parses skill related data
        /// </summary>
        private void parseSkillData(Stream stream)
        {
            var apiController = new GW2APIController();
            using (var reader = CreateReader(stream))
            {
                // 4 bytes: player count
                int skill_count = reader.ReadInt32();
                //TempData["Debug"] += "Skill Count:" + skill_count.ToString();
                // 68 bytes: each skill
                for(int i = 0; i < skill_count; i++)
                {
                    // 4 bytes: skill ID
                    int skill_id = reader.ReadInt32();

                    // 64 bytes: name
                    var name = ParseHelper.getString(stream, 64);
                    if(skill_id != 0 && int.TryParse(name, out int n) && n == skill_id)
                    {
                        //was it a known boon?
                        foreach(Boon b in Boon.getBoonList())
                        {
                            if(skill_id == b.getID())
                            {
                                name = b.getName();
                            }
                        }
                    }
                    //Save

                    var skill = new SkillItem(skill_id, name);

                    skill.SetGW2APISkill(apiController);
                    skill_data.addItem(skill);
                }
            }
        }

        private static CombatItem ReadCombatItem(BinaryReader reader)
        {
            // 8 bytes: time
            long time = reader.ReadInt64();

            // 8 bytes: src_agent
            ulong src_agent = reader.ReadUInt64();

            // 8 bytes: dst_agent
            ulong dst_agent = reader.ReadUInt64();

            // 4 bytes: value
            int value = reader.ReadInt32();

            // 4 bytes: buff_dmg
            int buff_dmg = reader.ReadInt32();

            // 2 bytes: overstack_value
            ushort overstack_value = reader.ReadUInt16();

            // 2 bytes: skill_id
            ushort skill_id = reader.ReadUInt16();

            // 2 bytes: src_instid
            ushort src_instid = reader.ReadUInt16();

            // 2 bytes: dst_instid
            ushort dst_instid = reader.ReadUInt16();

            // 2 bytes: src_master_instid
            ushort src_master_instid = reader.ReadUInt16();

            // 9 bytes: garbage
            ParseHelper.safeSkip(reader.BaseStream, 9);

            // 1 byte: iff
            ParseEnum.IFF iff = ParseEnum.getIFF(reader.ReadByte());

            // 1 byte: buff
            ushort buff = (ushort)reader.ReadByte();

            // 1 byte: result
            ParseEnum.Result result = ParseEnum.getResult(reader.ReadByte());

            // 1 byte: is_activation
            ParseEnum.Activation is_activation = ParseEnum.getActivation(reader.ReadByte());

            // 1 byte: is_buffremove
            ParseEnum.BuffRemove is_buffremoved = ParseEnum.getBuffRemove(reader.ReadByte());

            // 1 byte: is_ninety
            ushort is_ninety = (ushort)reader.ReadByte();

            // 1 byte: is_fifty
            ushort is_fifty = (ushort)reader.ReadByte();

            // 1 byte: is_moving
            ushort is_moving = (ushort)reader.ReadByte();

            // 1 byte: is_statechange
            ParseEnum.StateChange is_statechange = ParseEnum.getStateChange(reader.ReadByte());

            // 1 byte: is_flanking
            ushort is_flanking = (ushort)reader.ReadByte();

            // 1 byte: is_flanking
            ushort is_shields = (ushort)reader.ReadByte();
            // 2 bytes: garbage
            ParseHelper.safeSkip(reader.BaseStream, 2);

            //save
            // Add combat
            return new CombatItem(time, src_agent, dst_agent, value, buff_dmg, overstack_value, skill_id,
                src_instid, dst_instid, src_master_instid,0, iff, buff, result, is_activation, is_buffremoved,
                is_ninety, is_fifty, is_moving, is_statechange, is_flanking, is_shields);
        }

        private static CombatItem ReadCombatItemRev1(BinaryReader reader)
        {
            // 8 bytes: time
            long time = reader.ReadInt64();

            // 8 bytes: src_agent
            ulong src_agent = reader.ReadUInt64();

            // 8 bytes: dst_agent
            ulong dst_agent = reader.ReadUInt64();

            // 4 bytes: value
            int value = reader.ReadInt32();

            // 4 bytes: buff_dmg
            int buff_dmg = reader.ReadInt32();

            // 2 bytes: overstack_value
            uint overstack_value = reader.ReadUInt32();

            // 2 bytes: skill_id
            uint skill_id = reader.ReadUInt32();

            // 2 bytes: src_instid
            ushort src_instid = reader.ReadUInt16();

            // 2 bytes: dst_instid
            ushort dst_instid = reader.ReadUInt16();

            // 2 bytes: src_master_instid
            ushort src_master_instid = reader.ReadUInt16();
            // 2 bytes: dst_master_instid
            ushort dst_master_instid = reader.ReadUInt16();

            // 1 byte: iff
            ParseEnum.IFF iff = ParseEnum.getIFF(reader.ReadByte());

            // 1 byte: buff
            ushort buff = (ushort)reader.ReadByte();

            // 1 byte: result
            ParseEnum.Result result = ParseEnum.getResult(reader.ReadByte());

            // 1 byte: is_activation
            ParseEnum.Activation is_activation = ParseEnum.getActivation(reader.ReadByte());

            // 1 byte: is_buffremove
            ParseEnum.BuffRemove is_buffremoved = ParseEnum.getBuffRemove(reader.ReadByte());

            // 1 byte: is_ninety
            ushort is_ninety = (ushort)reader.ReadByte();

            // 1 byte: is_fifty
            ushort is_fifty = (ushort)reader.ReadByte();

            // 1 byte: is_moving
            ushort is_moving = (ushort)reader.ReadByte();

            // 1 byte: is_statechange
            ParseEnum.StateChange is_statechange = ParseEnum.getStateChange(reader.ReadByte());

            // 1 byte: is_flanking
            ushort is_flanking = (ushort)reader.ReadByte();

            // 1 byte: is_flanking
            ushort is_shields = (ushort)reader.ReadByte();
            // 5 bytes: offcycle (?) + garbage
            ParseHelper.safeSkip(reader.BaseStream, 5);

            //save
            // Add combat
            return new CombatItem(time, src_agent, dst_agent, value, buff_dmg, overstack_value, skill_id,
                src_instid, dst_instid, src_master_instid, dst_master_instid, iff, buff, result, is_activation, is_buffremoved,
                is_ninety, is_fifty, is_moving, is_statechange, is_flanking, is_shields);
        }

        /// <summary>
        /// Parses combat related data
        /// </summary>
        private void parseCombatList(Stream stream)
        {
            // 64 bytes: each combat
            var data = new byte[64];
            using(var ms     = new MemoryStream(data, writable: false))
            using(var reader = CreateReader(ms))
            {
                while(true)
                {
                    if(!TryRead(stream, data)) break;
                    ms.Seek(0, SeekOrigin.Begin);
                    combat_data.addItem( revision > 0 ? ReadCombatItemRev1(reader) : ReadCombatItem(reader));
                }
            }
        }
        
        private static bool isGolem(ushort id)
        {
            return id == 16202 || id == 16177 || id == 19676 || id == 19645 || id == 16199;
        }

        private static bool raidBoss(ushort id)
        {
            return id == 15438 || id == 15429 || id == 15375 || id == 16123 || id == 16115 || id == 16235
                || id == 16246 || id == 17194 || id == 17172 || id == 17188 || id == 17154 || id == 19767
                || id == 19450;
        }

        private static bool fractalBoss(ushort id)
        {
            return id == 17021 || id == 17028 || id == 16948 || id == 17632 || id == 17949
                || id == 17759;
        }

        /// <summary>
        /// Parses all the data again and link related stuff to each other
        /// </summary>
        private void fillMissingData()
        {
            var agentsLookup = agent_data.getAllAgentsList().ToDictionary(a => a.getAgent());

            bool golem_mode = isGolem(boss_data.getID());
            bool raid_mode = raidBoss(boss_data.getID());
            bool fractal_mode = fractalBoss(boss_data.getID());
            // Set Agent instid, first_aware and last_aware
            var combat_list = combat_data.getCombatList();
            foreach (CombatItem c in combat_list)
            {
                if(agentsLookup.TryGetValue(c.getSrcAgent(), out var a))
                {
                    if (a.getInstid() == 0 && (c.isStateChange() == ParseEnum.StateChange.Normal ||(((golem_mode && isGolem(a.getID())) || a.getID() == 0x4BFA) && c.isStateChange() == ParseEnum.StateChange.MaxHealthUpdate) ))
                    {
                        a.setInstid(c.getSrcInstid());
                    }
                    if (a.getInstid() != 0)
                    {
                        if (a.getFirstAware() == 0)
                        {
                            a.setFirstAware(c.getTime());
                            a.setLastAware(c.getTime());
                        }
                        else
                        {
                            a.setLastAware(c.getTime());
                        }
                    }
                }
            }

            foreach (CombatItem c in combat_list)
            {
                if (c.getSrcMasterInstid() != 0)
                {
                    var master = agent_data.getAllAgentsList().Find(x => x.getInstid() == c.getSrcMasterInstid() && x.getFirstAware() < c.getTime() && c.getTime() < x.getLastAware());
                    if (master != null)
                    {
                        if(agentsLookup.TryGetValue(c.getSrcAgent(), out var minion) && minion.getFirstAware() < c.getTime() && c.getTime() < minion.getLastAware())
                        {
                            minion.setMasterAgent(master.getAgent());
                        }
                    }
                }
            }

            agent_data.clean();

            // Set Boss data agent, instid, first_aware, last_aware and name
            List<AgentItem> NPC_list = agent_data.getNPCAgentList();
            HashSet<ulong> multiple_boss = new HashSet<ulong>();
            foreach (AgentItem NPC in NPC_list)
            {
                if (NPC.getProf().EndsWith(boss_data.getID().ToString()))
                {
                    if (boss_data.getAgent() == 0)
                    {
                        boss_data.setAgent(NPC.getAgent());
                        boss_data.setInstid(NPC.getInstid());
                        boss_data.setName(NPC.getName());
                        boss_data.setTough(NPC.getToughness());
                    }
                    multiple_boss.Add(NPC.getAgent());
                }
            }
            if (multiple_boss.Count > 1)
            {
                agent_data.cleanInstid(boss_data.getInstid());
            }

            AgentItem bossAgent = agent_data.GetAgent(boss_data.getAgent());
            boss = new Boss(bossAgent);
            List<Point> bossHealthOverTime = new List<Point>();
            // a hack for buggy golem logs
            if (golem_mode)
            {
                AgentItem otherGolem = NPC_list.Find(x => x.getID() == 19603);
                foreach (CombatItem c in combat_list)
                {
                    // redirect all attacks to the main golem
                    if (c.getDstAgent() == 0 && c.getDstInstid() == 0 && c.isStateChange() == ParseEnum.StateChange.Normal && c.getIFF() == ParseEnum.IFF.Foe && c.isActivation() == ParseEnum.Activation.None)
                    {
                        c.setDstAgent(bossAgent.getAgent());
                        c.setDstInstid(bossAgent.getInstid());
                    }
                    // redirect buff initial to main golem
                    if (otherGolem != null && c.isBuff() == 18 && c.getDstInstid() == otherGolem.getInstid())
                    {
                        c.setDstInstid(bossAgent.getInstid());
                    }
                }

            }
            // Grab values threw combat data
            foreach (CombatItem c in combat_list)
            {
                if (c.getSrcInstid() == boss_data.getInstid() && c.isStateChange() == ParseEnum.StateChange.MaxHealthUpdate)//max health update
                {
                    boss_data.setHealth((int)c.getDstAgent());

                }
                switch(c.isStateChange())
                {
                    case ParseEnum.StateChange.PointOfView:
                        if (log_data.getPOV() == "N/A")//Point of View
                        {
                            ulong pov_agent = c.getSrcAgent();
                            if(agentsLookup.TryGetValue(pov_agent, out var p))
                            {
                                log_data.setPOV(p.getName());
                            }
                        }
                        break;
                    case ParseEnum.StateChange.LogStart:
                        log_data.setLogStart(c.getValue());
                        boss_data.setFirstAware(c.getTime());
                        break;
                    case ParseEnum.StateChange.LogEnd:
                        log_data.setLogEnd(c.getValue());
                        boss_data.setLastAware(c.getTime());
                        break;
                    case ParseEnum.StateChange.HealthUpdate:
                        //set health update
                        if (c.getSrcInstid() == boss_data.getInstid())
                        {
                            bossHealthOverTime.Add(new Point ( (int)(c.getTime() - boss_data.getFirstAware()), (int)c.getDstAgent() ));
                        }
                        break;
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
                            if (c.getSrcInstid() == boss_data.getInstid() && c.isStateChange() == ParseEnum.StateChange.HealthUpdate)
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
                foreach (AgentItem NPC in agent_data.getGadgetAgentList())
                {
                    if (NPC.getProf().Contains("08467") || NPC.getProf().Contains("08471"))
                    {
                        deimos_2_instid = NPC.getInstid();
                        long oldAware = bossAgent.getLastAware();
                        if (NPC.getLastAware() < oldAware)
                        {
                            // No split
                            break;
                        }
                        boss.addPhaseData(NPC.getFirstAware() >= oldAware ? NPC.getFirstAware() : oldAware);
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
            // Put non reward stuff in this as we find them
            HashSet<int> notRaidRewardsIds = new HashSet<int>
            {
                13
            };
            if (raid_mode)
            {
                CombatItem reward = combat_list.Find(x => x.isStateChange() == ParseEnum.StateChange.Reward && !notRaidRewardsIds.Contains(x.getValue()));
                if (reward != null)
                {
                    log_data.setBossKill(true);
                    boss_data.setLastAware(reward.getTime());
                }
            } else if (fractal_mode) {
                CombatItem reward = combat_list.Find(x => x.isStateChange() == ParseEnum.StateChange.Reward);
                if (reward != null)
                {
                    log_data.setBossKill(true);
                    boss_data.setLastAware(reward.getTime());
                }
            } else
            {
                CombatItem killed = combat_list.Find(x => x.getSrcInstid() == boss_data.getInstid() && x.isStateChange() == ParseEnum.StateChange.ChangeDead);
                if (killed != null)
                {
                    log_data.setBossKill(true);
                    boss_data.setLastAware(killed.getTime());
                }
            }

            if (golem_mode && bossHealthOverTime.Count > 0)
            {
                log_data.setBossKill(bossHealthOverTime.Last().Y < 200);
            }

            //players
            if (p_list.Count == 0)
            {

                //Fix Disconected players
                var playerAgentList = agent_data.getPlayerAgentList();

                foreach (AgentItem playerAgent in playerAgentList)
                {
                    List<CombatItem> lp = combat_data.getStates(playerAgent.getInstid(), ParseEnum.StateChange.Despawn, boss_data.getFirstAware(), boss_data.getLastAware());
                    Player player = new Player(playerAgent, fractal_mode);
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
                        //make all actions of other instances to original instid
                        foreach (AgentItem extra in NPC_list)
                        {
                            if (extra.getAgent() == playerAgent.getAgent())
                            {
                                var extra_login_Id = extra.getInstid();
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
            p_list = p_list.OrderBy(a => a.getGroup()).ToList();                              
        }
    }
}
