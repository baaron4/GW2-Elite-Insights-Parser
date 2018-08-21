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
using LuckParser.Models;

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
        private List<Player> p_list = new List<Player>();
        private Boss boss;
        private byte revision;

        // Public Methods
        public LogData GetLogData()
        {
            return log_data;
        }
        public BossData GetBossData()
        {
            return boss_data;
        }

        public ParsedLog GetParsedLog()
        {
            return new ParsedLog(log_data, boss_data, agent_data, skill_data, combat_data, p_list, boss);
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
                ParseBossData(stream);
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "20% - Parsing agent data...", 20);
                ParseAgentData(stream);
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "25% - Parsing skill data...", 25);
                ParseSkillData(stream);
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "30% - Parsing combat list...", 30);
                ParseCombatList(stream);                
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "35% - Pairing data...", 35);
                FillMissingData();
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
        private void ParseBossData(Stream stream)
        {
            using (var reader = CreateReader(stream))
            {
                // 12 bytes: arc build version
                var build_version = ParseHelper.GetString(stream, 12);
                this.log_data = new LogData(build_version);

                // 1 byte: skip
                this.revision = reader.ReadByte();

                // 2 bytes: boss instance ID
                ushort id = reader.ReadUInt16();
                // 1 byte: position
                ParseHelper.SafeSkip(stream, 1);

                //Save
                // TempData["Debug"] = build_version +" "+ instid.ToString() ;
                this.boss_data = new BossData(id);
            }
        }

        /// <summary>
        /// Parses agent related data
        /// </summary>
        private void ParseAgentData(Stream stream)
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
                    // 2 bytes: healing
                    int concentration = reader.ReadInt16();
                    // 2 bytes: healing
                    int healing = reader.ReadInt16();
                    ParseHelper.SafeSkip(stream, 2);
                    // 2 bytes: condition
                    int condition = reader.ReadInt16();
                    ParseHelper.SafeSkip(stream, 2);
                    // 68 bytes: name
                    String name = ParseHelper.GetString(stream, 68, false);
                    //Save
                    Agent a = new Agent(agent, name, prof, is_elite);
                    var agent_prof = a.GetProf(this.log_data.GetBuildVersion(), APIController);
                    switch(agent_prof)
                    {
                        case "NPC":
                            // NPC
                            agent_data.AddItem(new AgentItem(agent, name, a.GetName() + ":" + prof.ToString().PadLeft(5, '0'), toughness, healing, condition, concentration), agent_prof);
                            break;
                            // Gadget
                        case "GDG":
                            agent_data.AddItem(new AgentItem(agent, name, a.GetName() + ":" + (prof & 0x0000ffff).ToString().PadLeft(5, '0'),toughness, healing, condition,concentration), agent_prof);
                            break;
                        default:
                            // Player
                            agent_data.AddItem(new AgentItem(agent, name, agent_prof, toughness, healing, condition,concentration), agent_prof);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Parses skill related data
        /// </summary>
        private void ParseSkillData(Stream stream)
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
                    var name = ParseHelper.GetString(stream, 64);
                    if(skill_id != 0 && int.TryParse(name, out int n) && n == skill_id)
                    {
                        //was it a known boon?
                        foreach(Boon b in Boon.GetBoonList())
                        {
                            if(skill_id == b.GetID())
                            {
                                name = b.GetName();
                            }
                        }
                    }
                    //Save

                    var skill = new SkillItem(skill_id, name);

                    skill.SetGW2APISkill(apiController);
                    skill_data.Add(skill);
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
            ParseHelper.SafeSkip(reader.BaseStream, 9);

            // 1 byte: iff
            ParseEnum.IFF iff = ParseEnum.GetIFF(reader.ReadByte());

            // 1 byte: buff
            ushort buff = (ushort)reader.ReadByte();

            // 1 byte: result
            ParseEnum.Result result = ParseEnum.GetResult(reader.ReadByte());

            // 1 byte: is_activation
            ParseEnum.Activation is_activation = ParseEnum.GetActivation(reader.ReadByte());

            // 1 byte: is_buffremove
            ParseEnum.BuffRemove is_buffremoved = ParseEnum.GetBuffRemove(reader.ReadByte());

            // 1 byte: is_ninety
            ushort is_ninety = (ushort)reader.ReadByte();

            // 1 byte: is_fifty
            ushort is_fifty = (ushort)reader.ReadByte();

            // 1 byte: is_moving
            ushort is_moving = (ushort)reader.ReadByte();

            // 1 byte: is_statechange
            ParseEnum.StateChange is_statechange = ParseEnum.GetStateChange(reader.ReadByte());

            // 1 byte: is_flanking
            ushort is_flanking = (ushort)reader.ReadByte();

            // 1 byte: is_flanking
            ushort is_shields = (ushort)reader.ReadByte();
            // 2 bytes: garbage
            ParseHelper.SafeSkip(reader.BaseStream, 2);

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
            ParseEnum.IFF iff = ParseEnum.GetIFF(reader.ReadByte());

            // 1 byte: buff
            ushort buff = (ushort)reader.ReadByte();

            // 1 byte: result
            ParseEnum.Result result = ParseEnum.GetResult(reader.ReadByte());

            // 1 byte: is_activation
            ParseEnum.Activation is_activation = ParseEnum.GetActivation(reader.ReadByte());

            // 1 byte: is_buffremove
            ParseEnum.BuffRemove is_buffremoved = ParseEnum.GetBuffRemove(reader.ReadByte());

            // 1 byte: is_ninety
            ushort is_ninety = (ushort)reader.ReadByte();

            // 1 byte: is_fifty
            ushort is_fifty = (ushort)reader.ReadByte();

            // 1 byte: is_moving
            ushort is_moving = (ushort)reader.ReadByte();

            // 1 byte: is_statechange
            ParseEnum.StateChange is_statechange = ParseEnum.GetStateChange(reader.ReadByte());

            // 1 byte: is_flanking
            ushort is_flanking = (ushort)reader.ReadByte();

            // 1 byte: is_flanking
            ushort is_shields = (ushort)reader.ReadByte();
            // 5 bytes: offcycle (?) + garbage
            ParseHelper.SafeSkip(reader.BaseStream, 5);

            //save
            // Add combat
            return new CombatItem(time, src_agent, dst_agent, value, buff_dmg, overstack_value, skill_id,
                src_instid, dst_instid, src_master_instid, dst_master_instid, iff, buff, result, is_activation, is_buffremoved,
                is_ninety, is_fifty, is_moving, is_statechange, is_flanking, is_shields);
        }

        /// <summary>
        /// Parses combat related data
        /// </summary>
        private void ParseCombatList(Stream stream)
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
                    combat_data.Add( revision > 0 ? ReadCombatItemRev1(reader) : ReadCombatItem(reader));
                }
            }
            combat_data.RemoveAll(x => x.GetSrcInstid() == 0 && x.GetDstAgent() == 0 && x.GetSrcAgent() == 0 && x.GetDstInstid() == 0 && x.GetIFF() == ParseEnum.IFF.Unknown);
        }
        
        private static bool IsGolem(ushort id)
        {
            return id == 16202 || id == 16177 || id == 19676 || id == 19645 || id == 16199;
        }
        
        /// <summary>
        /// Parses all the data again and link related stuff to each other
        /// </summary>
        private void FillMissingData()
        {
            var agentsLookup = agent_data.GetAllAgentsList().ToDictionary(a => a.GetAgent());
            bool golem_mode = boss_data.GetBossBehavior().GetMode() == BossLogic.ParseMode.Golem;
            bool raid_mode = boss_data.GetBossBehavior().GetMode() == BossLogic.ParseMode.Raid;
            bool fractal_mode = boss_data.GetBossBehavior().GetMode() == BossLogic.ParseMode.Fractal;
            // Set Agent instid, first_aware and last_aware
            foreach (CombatItem c in combat_data)
            {
                if(agentsLookup.TryGetValue(c.GetSrcAgent(), out var a))
                {
                    if (a.GetInstid() == 0 && c.IsStateChange().IsSpawn())
                    {
                        a.SetInstid(c.GetSrcInstid());
                    }
                    if (a.GetInstid() != 0)
                    {
                        if (a.GetFirstAware() == 0)
                        {
                            a.SetFirstAware(c.GetTime());
                            a.SetLastAware(c.GetTime());
                        }
                        else
                        {
                            a.SetLastAware(c.GetTime());
                        }
                    }
                }
            }

            foreach (CombatItem c in combat_data)
            {
                if (c.GetSrcMasterInstid() != 0)
                {
                    var master = agent_data.GetAllAgentsList().Find(x => x.GetInstid() == c.GetSrcMasterInstid() && x.GetFirstAware() < c.GetTime() && c.GetTime() < x.GetLastAware());
                    if (master != null)
                    {
                        if(agentsLookup.TryGetValue(c.GetSrcAgent(), out var minion) && minion.GetFirstAware() < c.GetTime() && c.GetTime() < minion.GetLastAware())
                        {
                            minion.SetMasterAgent(master.GetAgent());
                        }
                    }
                }
            }

            agent_data.Clean();

            // Set Boss data agent, instid, first_aware, last_aware and name
            List<AgentItem> NPC_list = agent_data.GetNPCAgentList();
            HashSet<ulong> multiple_boss = new HashSet<ulong>();
            foreach (AgentItem NPC in NPC_list)
            {
                if (NPC.GetID() == boss_data.GetID())
                {
                    if (boss_data.GetAgent() == 0)
                    {
                        boss_data.SetAgent(NPC.GetAgent());
                        boss_data.SetInstid(NPC.GetInstid());
                        boss_data.SetName(NPC.GetName());
                        boss_data.SetTough(NPC.GetToughness());
                    }
                    multiple_boss.Add(NPC.GetAgent());
                }
            }
            if (multiple_boss.Count > 1)
            {
                agent_data.CleanInstid(boss_data.GetInstid());
            }
            AgentItem bossAgent = agent_data.GetAgent(boss_data.GetAgent());
            boss = new Boss(bossAgent);
            List<Point> bossHealthOverTime = new List<Point>();
            // a hack for buggy golem logs
            if (golem_mode)
            {
                AgentItem otherGolem = NPC_list.Find(x => x.GetID() == 19603);
                foreach (CombatItem c in combat_data)
                {
                    // redirect all attacks to the main golem
                    if (c.GetDstAgent() == 0 && c.GetDstInstid() == 0 && c.IsStateChange() == ParseEnum.StateChange.Normal && c.GetIFF() == ParseEnum.IFF.Foe && c.IsActivation() == ParseEnum.Activation.None)
                    {
                        c.SetDstAgent(bossAgent.GetAgent());
                        c.SetDstInstid(bossAgent.GetInstid());
                    }
                    // redirect buff initial to main golem
                    if (otherGolem != null && c.IsBuff() == 18 && c.GetDstInstid() == otherGolem.GetInstid())
                    {
                        c.SetDstInstid(bossAgent.GetInstid());
                    }
                }

            }
            // Grab values threw combat data
            foreach (CombatItem c in combat_data)
            {
                if (c.GetSrcInstid() == boss_data.GetInstid() && c.IsStateChange() == ParseEnum.StateChange.MaxHealthUpdate)//max health update
                {
                    boss_data.SetHealth((int)c.GetDstAgent());

                }
                switch(c.IsStateChange())
                {
                    case ParseEnum.StateChange.PointOfView:
                        if (log_data.GetPOV() == "N/A")//Point of View
                        {
                            ulong pov_agent = c.GetSrcAgent();
                            if(agentsLookup.TryGetValue(pov_agent, out var p))
                            {
                                log_data.SetPOV(p.GetName());
                            }
                        }
                        break;
                    case ParseEnum.StateChange.LogStart:
                        log_data.SetLogStart(c.GetValue());
                        boss_data.SetFirstAware(c.GetTime());
                        break;
                    case ParseEnum.StateChange.LogEnd:
                        log_data.SetLogEnd(c.GetValue());
                        boss_data.SetLastAware(c.GetTime());
                        break;
                    case ParseEnum.StateChange.HealthUpdate:
                        //set health update
                        if (c.GetSrcInstid() == boss_data.GetInstid())
                        {
                            bossHealthOverTime.Add(new Point ( (int)(c.GetTime() - boss_data.GetFirstAware()), (int)c.GetDstAgent() ));
                        }
                        break;
                }
            }

            // Dealing with second half of Xera | ((22611300 * 0.5) + (25560600 * 0.5)
            if (boss_data.GetID() == 16246)
            {
                int xera_2_instid = 0;
                foreach (AgentItem NPC in NPC_list)
                {
                    if (NPC.GetID() == 16286)
                    {
                        bossHealthOverTime = new List<Point>();//reset boss health over time
                        xera_2_instid = NPC.GetInstid();
                        boss_data.SetHealth(24085950);
                        boss.AddPhaseData(NPC.GetFirstAware());
                        boss_data.SetLastAware(NPC.GetLastAware());
                        foreach (CombatItem c in combat_data)
                        {
                            if (c.GetSrcInstid() == xera_2_instid)
                            {
                                c.SetSrcInstid(boss_data.GetInstid());
                                c.SetSrcAgent(boss_data.GetAgent());
                            }
                            if (c.GetDstInstid() == xera_2_instid)
                            {
                                c.SetDstInstid(boss_data.GetInstid());
                                c.SetDstAgent(boss_data.GetAgent());
                            }
                            //set health update
                            if (c.GetSrcInstid() == boss_data.GetInstid() && c.IsStateChange() == ParseEnum.StateChange.HealthUpdate)
                            {
                                bossHealthOverTime.Add(new Point ( (int)(c.GetTime() - boss_data.GetFirstAware()), (int)c.GetDstAgent() ));
                            }
                        }
                        break;
                    }
                }
            }
            //Dealing with Deimos split
            if (boss_data.GetID() == 17154)
            {
                int deimos_2_instid = 0;
                List<AgentItem> deimosGadgets = agent_data.GetGadgetAgentList().Where(x => x.GetFirstAware() > bossAgent.GetLastAware() && x.GetName().Contains("Deimos")).OrderBy(x => x.GetLastAware()).ToList();
                if (deimosGadgets.Count > 0)
                {
                    AgentItem NPC = deimosGadgets.Last();
                    deimos_2_instid = NPC.GetInstid();
                    long oldAware = bossAgent.GetLastAware();
                    boss.AddPhaseData(NPC.GetFirstAware() >= oldAware ? NPC.GetFirstAware() : oldAware);
                    //List<CombatItem> fuckyou = combat_list.Where(x => x.getDstInstid() == deimos_2_instid ).ToList().Sum(x);
                    //int stop = 0;
                    foreach (CombatItem c in combat_data)
                    {
                        if (c.GetTime() > oldAware)
                        {
                            if (c.GetSrcInstid() == deimos_2_instid)
                            {
                                c.SetSrcInstid(boss_data.GetInstid());
                                c.SetSrcAgent(boss_data.GetAgent());

                            }
                            if (c.GetDstInstid() == deimos_2_instid)
                            {
                                c.SetDstInstid(boss_data.GetInstid());
                                c.SetDstAgent(boss_data.GetAgent());
                            }
                        }

                    }
                }
            }
            boss_data.SetHealthOverTime(bossHealthOverTime);//after xera in case of change

            if (raid_mode)
            {
                // Put non reward stuff in this as we find them
                HashSet<int> notRaidRewardsIds = new HashSet<int>
                {
                    13
                };
                CombatItem reward = combat_data.Find(x => x.IsStateChange() == ParseEnum.StateChange.Reward && !notRaidRewardsIds.Contains(x.GetValue()));
                if (reward != null)
                {
                    log_data.SetBossKill(true);
                    boss_data.SetLastAware(reward.GetTime());
                }
            } else if (fractal_mode) {
                CombatItem reward = combat_data.Find(x => x.IsStateChange() == ParseEnum.StateChange.Reward);
                if (reward != null)
                {
                    log_data.SetBossKill(true);
                    boss_data.SetLastAware(reward.GetTime());
                } else
                {
                    // for skorvald, as CM and normal ids are the same
                    CombatItem killed = combat_data.Find(x => x.GetSrcInstid() == boss_data.GetInstid() && x.IsStateChange().IsDead());
                    if (killed != null)
                    {
                        log_data.SetBossKill(true);
                        boss_data.SetLastAware(killed.GetTime());
                    }
                }
            } else
            {
                CombatItem killed = combat_data.Find(x => x.GetSrcInstid() == boss_data.GetInstid() && x.IsStateChange().IsDead());
                if (killed != null)
                {
                    log_data.SetBossKill(true);
                    boss_data.SetLastAware(killed.GetTime());
                }
            }

            if (golem_mode && bossHealthOverTime.Count > 0)
            {
                log_data.SetBossKill(bossHealthOverTime.Last().Y < 200);
                boss_data.SetLastAware(bossHealthOverTime.Last().X + boss_data.GetFirstAware());
            }
            //players
            if (p_list.Count == 0)
            {

                //Fix Disconected players
                var playerAgentList = agent_data.GetPlayerAgentList();

                foreach (AgentItem playerAgent in playerAgentList)
                {
                    if (playerAgent.GetInstid() == 0)
                    {
                        CombatItem tst = combat_data.Find(x => x.GetSrcAgent() == playerAgent.GetAgent());
                        if (tst == null)
                        {
                            tst = combat_data.Find(x => x.GetDstAgent() == playerAgent.GetAgent());
                            if (tst == null)
                            {
                                playerAgent.SetInstid(ushort.MaxValue);
                            }
                            else
                            {
                                playerAgent.SetInstid(tst.GetDstInstid());
                            }
                        }
                        else
                        {
                            playerAgent.SetInstid(tst.GetSrcInstid());
                        }
                    }
                    List<CombatItem> lp = combat_data.GetStates(playerAgent.GetInstid(), ParseEnum.StateChange.Despawn, boss_data.GetFirstAware(), boss_data.GetLastAware());
                    Player player = new Player(playerAgent, fractal_mode);
                    bool skip = false;
                    foreach (Player p in p_list)
                    {
                        if (p.GetAccount() == player.GetAccount())//is this a copy of original?
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
                            if (extra.GetAgent() == playerAgent.GetAgent())
                            {
                                var extra_login_Id = extra.GetInstid();
                                foreach (CombatItem c in combat_data)
                                {
                                    if (c.GetSrcInstid() == extra_login_Id)
                                    {
                                        c.SetSrcInstid(playerAgent.GetInstid());
                                    }
                                    if (c.GetDstInstid() == extra_login_Id)
                                    {
                                        c.SetDstInstid(playerAgent.GetInstid());
                                    }
                                }
                                break;
                            }
                        }

                        player.SetDC(lp[0].GetTime());
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
            if (boss_data.GetFirstAware() == 0)
            {
                boss_data.SetFirstAware(bossAgent.GetFirstAware());
            }
            if (boss_data.GetLastAware() == long.MaxValue)
            {
                boss_data.SetLastAware(bossAgent.GetLastAware());
            }
            // Sort
            p_list = p_list.OrderBy(a => a.GetGroup()).ToList();
            // Check CM
            boss_data.SetCM(combat_data);
            combat_data.Validate(boss_data);
        }
    }
}
