using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Parser.ParseEnum.TrashIDS;

namespace GW2EIParser.Logic
{
    public class EaterOfSouls : RaidLogic
    {
        // TODO - add CR icons/indicators (vomit, greens, etc) and some mechanics
        public EaterOfSouls(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(47303, "Hungering Miasma", new MechanicPlotlySetting("triangle-left-open","rgb(100,255,0)"), "Vomit","Hungering Miasma (Vomit Goo)", "Vomit Dmg",0),
            new PlayerBuffApplyMechanic(46950, "Fractured Spirit", new MechanicPlotlySetting("circle","rgb(0,255,0)"), "Orb CD","Applied when taking green", "Green port",0),
            }
            );
            Extension = "souleater";
            Icon = "https://wiki.guildwars2.com/images/thumb/2/24/Eater_of_Souls_%28Hall_of_Chains%29.jpg/194px-Eater_of_Souls_%28Hall_of_Chains%29.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/Owo34RS.png",
                            (710, 709),
                            (1306, -9381, 4720, -5968),
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508));
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                OrbSpider,
                SpiritHorde1,
                SpiritHorde2,
                SpiritHorde3,
                GreenSpirit1,
                GreenSpirit2
            };
        }

        public override void ComputeNPCCombatReplayActors(NPC target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.SoulEater:
                    var breakbar = cls.Where(x => x.SkillId == 48007).ToList();
                    foreach (AbstractCastEvent c in breakbar)
                    {
                        start = (int)c.Time;
                        end = start + c.ActualDuration;
                        replay.Decorations.Add(new CircleDecoration(true, start + c.ExpectedDuration, 180, (start, end), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    var vomit = cls.Where(x => x.SkillId == 47303).ToList();
                    foreach (AbstractCastEvent c in vomit)
                    {
                        start = (int)c.Time + 2100;
                        int cascading = 1500;
                        int duration = 15000 + cascading;
                        end = start + duration;
                        int radius = 900;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start);
                        Point3D position = replay.PolledPositions.LastOrDefault(x => x.Time <= start);
                        if (facing != null && position != null)
                        {
                            replay.Decorations.Add(new PieDecoration(true, start + cascading, radius, facing, 60, (start, end), "rgba(220,255,0,0.5)", new PositionConnector(position)));
                        }
                    }
                    var pseudoDeath = cls.Where(x => x.SkillId == 47440).ToList();
                    foreach (AbstractCastEvent c in pseudoDeath)
                    {
                        start = (int)c.Time;
                        //int duration = 900;
                        end = start + c.ActualDuration; //duration;
                        //replay.Actors.Add(new CircleActor(true, 0, 180, (start, end), "rgba(255, 150, 255, 0.35)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, end, 180, (start, end), "rgba(255, 180, 220, 0.7)", new AgentConnector(target)));
                    }
                    break;
                case (ushort)GreenSpirit1:
                case (ushort)GreenSpirit2:
                    var green = cls.Where(x => x.SkillId == 47153).ToList();
                    foreach (AbstractCastEvent c in green)
                    {
                        int gstart = (int)c.Time + 667;
                        int gend = gstart + 5000;
                        replay.Decorations.Add(new CircleDecoration(true, 0, 240, (gstart, gend), "rgba(0, 255, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, gend, 240, (gstart, gend), "rgba(0, 255, 0, 0.2)", new AgentConnector(target)));
                    }
                    break;
                case (ushort)SpiritHorde1:
                case (ushort)SpiritHorde2:
                case (ushort)SpiritHorde3:
                case (ushort)OrbSpider:
                    break;
                default:
                    break;
            }

        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            var spiritTransform = log.CombatData.GetBuffData(46950).Where(x => x.To == p.AgentItem && x is BuffApplyEvent).ToList();
            foreach (AbstractBuffEvent c in spiritTransform)
            {
                int duration = 30000;
                AbstractBuffEvent removedBuff = log.CombatData.GetBuffRemoveAllData(48583).FirstOrDefault(x => x.To == p.AgentItem && x.Time > c.Time && x.Time < c.Time + duration);
                int start = (int)c.Time;
                int end = start + duration;
                if (removedBuff != null)
                {
                    end = (int)removedBuff.Time;
                }
                replay.Decorations.Add(new CircleDecoration(true, 0, 100, (start, end), "rgba(0, 50, 200, 0.3)", new AgentConnector(p)));
                replay.Decorations.Add(new CircleDecoration(true, start + duration, 100, (start, end), "rgba(0, 50, 200, 0.5)", new AgentConnector(p)));
            }
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            SetSuccessByDeath(combatData, fightData, playerAgents, true, (ushort)ParseEnum.TargetIDS.SoulEater);
        }

        public override string GetFightName()
        {
            return "Statue of Death";
        }
    }
}
