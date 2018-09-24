using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models
{
    public class Xera : RaidLogic
    {
        public Xera(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new Mechanic(35128, "Temporal Shred", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Xera, "symbol:'circle',color:'rgb(255,0,0)',", "Orb","Temporal Shred (Hit by Red Orb)", "Red Orb",0),
            new Mechanic(34913, "Temporal Shred", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Xera, "symbol:'circle-open',color:'rgb(255,0,0)',", "O.Aoe","Temporal Shred (Stood in Orb Aoe)", "Orb AoE",0),
            new Mechanic(35168, "Bloodstone Protection", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'hourglass-open',color:'rgb(128,0,128)',", "InBble","Bloodstone Protection (Stood in Bubble)", "Inside Bubble",0),
            new Mechanic(34887, "Summon Fragment Start", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Xera, "symbol:'diamond-tall',color:'rgb(0,160,150)',", "CC","Summon Fragment (Xera Breakbar)", "Breakbar",0),
            new Mechanic(34887, "Summon Fragment End", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Xera, "symbol:'diamond-tall',color:'rgb(255,0,0)',", "CC.Fail","Summon Fragment (Failed CC)", "CC Fail",0,(condition => condition.CombatItem.Value > 11940)),
            new Mechanic(34887, "Summon Fragment End", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Xera, "symbol:'diamond-tall',color:'rgb(0,160,0)',", "CCed","Summon Fragment (Breakbar broken)", "CCed",0,(condition => condition.CombatItem.Value <= 11940)),
            new Mechanic(34965, "Derangement", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'square-open',color:'rgb(200,140,255)',", "Drgmnt","Derangement (Stacking Debuff)", "Derangement",0), 
            new Mechanic(35084, "Bending Chaos", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'triangle-down-open',color:'rgb(255,200,0)',", "Btn1","Bending Chaos (Stood on 1st Button)", "Button 1",0),
            new Mechanic(35162, "Shifting Chaos", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'triangle-ne-open',color:'rgb(255,200,0)',", "Btn2","Bending Chaos (Stood on 2nd Button)", "Button 2",0),
            new Mechanic(35032, "Twisting Chaos", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'triangle-nw-open',color:'rgb(255,200,0)',", "Btn3","Bending Chaos (Stood on 3rd Button)", "Button 3",0),
            new Mechanic(34956, "Intervention", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'square',color:'rgb(0,0,255)',", "Shld","Intervention (got Special Action Key)", "Shield",0),
            new Mechanic(34921, "Gravity Well", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'circle-x-open',color:'rgb(255,0,255)',", "GrWell","Half-platform Gravity Well", "Gravity Well",4000),
            new Mechanic(34997, "Teleport Out", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'circle',color:'rgb(0,128,0)',", "TP.Out","Teleport Out (Teleport to Platform)","TP",0),
            new Mechanic(35076, "Hero's Return", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'circle',color:'rgb(0,200,0)',", "TP.Back","Hero's Return (Teleport back)", "TP back",0),
            /*new Mechanic(35000, "Intervention", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Xera, "symbol:'hourglass',color:'rgb(128,0,128)',", "Bubble",0),*/
            //new Mechanic(35034, "Disruption", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Xera, "symbol:'square',color:'rgb(0,128,0)',", "TP",0), 
            //Not sure what this (ID 350342,"Disruption") is. Looks like it is the pulsing "orb removal" from the orange circles on the 40% platform. Would fit the name although it's weird it can hit players. 
            });
            Extension = "xera";
            IconUrl = "https://wiki.guildwars2.com/images/4/4b/Mini_Xera.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/BoHwwY6.png",
                            Tuple.Create(7112, 6377),
                            Tuple.Create(-5992, -5992, 69, -522),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(1920, 12160, 2944, 14464));
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            if (!requirePhases)
            {
                return phases;
            }
            // split happened
            if (log.FightData.PhaseData.Count == 1)
            {
                CombatItem invulXera = log.GetBoonData(762).Find(x => x.DstInstid == log.Boss.InstID);
                if (invulXera == null)
                {
                    invulXera = log.GetBoonData(34113).Find(x => x.DstInstid == log.Boss.InstID);
                }
                long end = invulXera.Time - log.FightData.FightStart;
                phases.Add(new PhaseData(start, end));
                start = log.FightData.PhaseData[0] - log.FightData.FightStart;
                log.Boss.AddCustomCastLog(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None), log);
            }
            if (fightDuration - start > 5000 && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].DrawArea = true;
                phases[i].DrawStart = i > 1;
                phases[i].DrawEnd = i < phases.Count - 1;

            }
            return phases;
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData, Boss boss)
        {
            foreach (AgentItem NPC in agentData.GetAgentByType(AgentItem.AgentType.NPC))
            {
                if (NPC.ID == 16286)
                {
                    List<Point> bossHealthOverTime = new List<Point>();//reset boss health over time
                    int xera2Instid = NPC.InstID;
                    boss.Health = 24085950;
                    fightData.PhaseData.Add(NPC.FirstAware);
                    boss.AgentItem.LastAware = NPC.LastAware;
                    foreach (CombatItem c in combatData)
                    {
                        if (c.SrcInstid == xera2Instid)
                        {
                            c.SrcInstid = boss.InstID;
                            c.SrcAgent = boss.Agent;
                        }
                        if (c.DstInstid == xera2Instid)
                        {
                            c.DstInstid = boss.InstID;
                            c.DstAgent = boss.Agent;
                        }
                        //set health update
                        if (c.SrcInstid == boss.InstID && c.IsStateChange == ParseEnum.StateChange.HealthUpdate)
                        {
                            bossHealthOverTime.Add(new Point((int)(c.Time - fightData.FightStart), (int)c.DstAgent));
                        }
                    }
                    boss.HealthOverTime = bossHealthOverTime;
                    break;
                }
            }
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                ParseEnum.TrashIDS.WhiteMantleSeeker1,
                ParseEnum.TrashIDS.WhiteMantleSeeker2,
                ParseEnum.TrashIDS.WhiteMantleKnight,
                ParseEnum.TrashIDS.WhiteMantleBattleMage,
                ParseEnum.TrashIDS.ExquisiteConjunction
            };
        }

        public override void ComputeAdditionalBossData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: needs facing information for hadouken
            List<CastLog> summon = cls.Where(x => x.SkillId == 34887).ToList();
            foreach (CastLog c in summon)
            {
                replay.Actors.Add(new CircleActor(true, 0, 180, new Tuple<int, int>((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)"));
            }
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/lYwJEyV.png";
        }
    }
}
