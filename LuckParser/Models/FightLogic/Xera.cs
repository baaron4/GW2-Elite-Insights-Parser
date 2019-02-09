using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Xera : RaidLogic
    {

        private long _specialSplit = 0;

        public Xera(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new HitOnPlayerMechanic(35128, "Temporal Shred", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Orb","Temporal Shred (Hit by Red Orb)", "Red Orb",0),
            new HitOnPlayerMechanic(34913, "Temporal Shred", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Orb Aoe","Temporal Shred (Stood in Orb Aoe)", "Orb AoE",0),
            new PlayerBoonApplyMechanic(35168, "Bloodstone Protection", new MechanicPlotlySetting("hourglass-open","rgb(128,0,128)"), "In Bubble","Bloodstone Protection (Stood in Bubble)", "Inside Bubble",0),
            new EnemyCastStartMechanic(34887, "Summon Fragment Start", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","Summon Fragment (Xera Breakbar)", "Breakbar",0),
            new EnemyCastEndMechanic(34887, "Summon Fragment End", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC Fail","Summon Fragment (Failed CC)", "CC Fail",0,(condition => condition.Value > 11940)),
            new EnemyCastEndMechanic(34887, "Summon Fragment End", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Summon Fragment (Breakbar broken)", "CCed",0,(condition => condition.Value <= 11940)),
            new PlayerBoonApplyMechanic(34965, "Derangement", new MechanicPlotlySetting("square-open","rgb(200,140,255)"), "Stacks","Derangement (Stacking Debuff)", "Derangement",0), 
            new PlayerBoonApplyMechanic(35084, "Bending Chaos", new MechanicPlotlySetting("triangle-down-open","rgb(255,200,0)"), "Button1","Bending Chaos (Stood on 1st Button)", "Button 1",0),
            new PlayerBoonApplyMechanic(35162, "Shifting Chaos", new MechanicPlotlySetting("triangle-ne-open","rgb(255,200,0)"), "Button2","Bending Chaos (Stood on 2nd Button)", "Button 2",0),
            new PlayerBoonApplyMechanic(35032, "Twisting Chaos", new MechanicPlotlySetting("triangle-nw-open","rgb(255,200,0)"), "Button3","Bending Chaos (Stood on 3rd Button)", "Button 3",0),
            new PlayerBoonApplyMechanic(34956, "Intervention", new MechanicPlotlySetting("square","rgb(0,0,255)"), "Shield","Intervention (got Special Action Key)", "Shield",0),
            new PlayerBoonApplyMechanic(34921, "Gravity Well", new MechanicPlotlySetting("circle-x-open","rgb(255,0,255)"), "Gravity Half","Half-platform Gravity Well", "Gravity Well",4000),
            new PlayerBoonApplyMechanic(34997, "Teleport Out", new MechanicPlotlySetting("circle","rgb(0,128,0)"), "TP Out","Teleport Out (Teleport to Platform)","TP",0),
            new PlayerBoonApplyMechanic(35076, "Hero's Return", new MechanicPlotlySetting("circle","rgb(0,200,0)"), "TP Back","Hero's Return (Teleport back)", "TP back",0),
            /*new Mechanic(35000, "Intervention", ParseEnum.BossIDS.Xera, new MechanicPlotlySetting("hourglass","rgb(128,0,128)"), "Bubble",0),*/
            //new Mechanic(35034, "Disruption", ParseEnum.BossIDS.Xera, new MechanicPlotlySetting("square","rgb(0,128,0)"), "TP",0), 
            //Not sure what this (ID 350342,"Disruption") is. Looks like it is the pulsing "orb removal" from the orange circles on the 40% platform. Would fit the name although it's weird it can hit players. 
            });
            Extension = "xera";
            IconUrl = "https://wiki.guildwars2.com/images/4/4b/Mini_Xera.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/BoHwwY6.png",
                            (7112, 6377),
                            (-5992, -5992, 69, -522),
                            (-12288, -27648, 12288, 27648),
                            (1920, 12160, 2944, 14464));
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Xera);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            long end = 0;
            CombatItem invulXera = log.CombatData.GetBoonData(762).Find(x => x.DstInstid == mainTarget.InstID) ?? log.CombatData.GetBoonData(34113).Find(x => x.DstInstid == mainTarget.InstID);
            if (invulXera != null)
            {
                end = log.FightData.ToFightSpace(invulXera.Time);
                phases.Add(new PhaseData(start, end));
                // split happened
                if (_specialSplit > 0)
                {
                    start = log.FightData.ToFightSpace(_specialSplit);
                    //mainTarget.AddCustomCastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None, log);
                    phases.Add(new PhaseData(start, fightDuration));
                }
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].Targets.Add(mainTarget);

            }
            return phases;
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // find target
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Xera);
            if (target == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            // enter combat
            CombatItem enterCombat = combatData.Find(x => x.SrcInstid == target.InstID && x.IsStateChange == ParseEnum.StateChange.EnterCombat);
            if (enterCombat != null)
            {
                fightData.FightStart = enterCombat.Time;
            }
            // find split
            foreach (AgentItem NPC in agentData.GetAgentByType(AgentItem.AgentType.NPC))
            {
                if (NPC.ID == 16286)
                {
                    target.Health = 24085950;
                    CombatItem move = combatData.FirstOrDefault(x => x.IsStateChange == ParseEnum.StateChange.Position && x.SrcInstid == NPC.InstID && x.Time >= NPC.FirstAware + 500 && x.Time <= NPC.LastAware);
                    if (move != null)
                    {
                        _specialSplit = move.Time;
                    } else
                    {
                        _specialSplit = NPC.FirstAware;
                    }
                    target.AgentItem.LastAware = NPC.LastAware;
                    // get unique id for the fusion
                    ushort instID = 0;
                    Random rnd = new Random();
                    while (agentData.InstIDValues.Contains(instID) || instID == 0)
                    {
                        instID = (ushort)rnd.Next(ushort.MaxValue/2, ushort.MaxValue);
                    }
                    target.AgentItem.InstID = instID;
                    agentData.Refresh();
                    HashSet<ulong> agents = new HashSet<ulong>() { NPC.Agent, target.Agent };
                    // update combat data
                    foreach (CombatItem c in combatData)
                    {
                        if (agents.Contains(c.SrcAgent))
                        {
                            c.SrcInstid = target.InstID;
                            c.SrcAgent = target.Agent;
                        }
                        if (agents.Contains(c.DstAgent))
                        {
                            c.DstInstid = target.InstID;
                            c.DstAgent = target.Agent;
                        }
                    }
                    break;
                }
            }
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                WhiteMantleSeeker1,
                WhiteMantleSeeker2,
                WhiteMantleKnight1,
                WhiteMantleKnight2,
                WhiteMantleBattleMage1,
                WhiteMantleBattleMage2,
                ExquisiteConjunction,
                ChargedBloodstone,
                BloodstoneFragment,
                XerasPhantasm,
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)WhiteMantleSeeker1:
                case (ushort)WhiteMantleSeeker2:
                case (ushort)WhiteMantleKnight1:
                case (ushort)WhiteMantleKnight2:
                case (ushort)WhiteMantleBattleMage1:
                case (ushort)WhiteMantleBattleMage2:
                case (ushort)ExquisiteConjunction:
                case (ushort)ChargedBloodstone:
                case (ushort)BloodstoneFragment:
                case (ushort)XerasPhantasm:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Xera:
                    List<CastLog> summon = cls.Where(x => x.SkillId == 34887).ToList();
                    foreach (CastLog c in summon)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, 180, ((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
        }
    }
}
