using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class SoullessHorror : BossStrategy
    {
        public SoullessHorror() : base()
        {
            mode = ParseMode.Raid;
            mechanicList = new List<Mechanic>
            {

            new Mechanic(47327, "Vortex Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle',color:'rgb(255,140,0)',", "Donut Inner",0),
            new Mechanic(48432, "Vortex Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle-open',color:'rgb(255,140,0)',", "Donut Outer",0),
            new Mechanic(47430, "Soul Rift", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle',color:'rgb(255,0,0)',", "Golem AOE",0),
            new Mechanic(48363, "Quad Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-square-open',color:'rgb(255,140,0)',", "Slices",0),
            new Mechanic(47915, "Quad Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-diamond-open',color:'rgb(255,140,0)',", "Slices",0),
            new Mechanic(47363, "Spinning Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'octagon',color:'rgb(128,0,0)',", "Scythe",0),
            new Mechanic(48500, "Death Bloom", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'octagon',color:'rgb(255,140,0)',", " 8 Slices",0),
            new Mechanic(47434, "Fixated", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.SoullessHorror, "symbol:'star',color:'rgb(255,0,255)',", "Fixate",0),
            new Mechanic(47414, "Necrosis", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.SoullessHorror, "symbol:'square-open',color:'rgb(200,140,255)',", "Necrosis Debuff",0)
            };
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/A45pVJy.png",
                            Tuple.Create(3657, 3657),
                            Tuple.Create(-12223, -771, -8932, 2420),
                            Tuple.Create(-21504, -12288, 24576, 12288),
                            Tuple.Create(19072, 15484, 20992, 16508));
        }

        public override List<PhaseData> getPhases(Boss boss, ParsedLog log, List<CastLog> cast_logs)
        {
            List<PhaseData> phases = getInitialPhase(log);           
            return phases;
        }

        public override List<ParseEnum.ThrashIDS> getAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: facing information (slashes) and doughnuts for outer circle attack
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Scythe,
                        ParseEnum.ThrashIDS.TormentedDead,
                        ParseEnum.ThrashIDS.SurgingSoul
                    };
            List<CastLog> howling = cls.Where(x => x.getID() == 48662).ToList();
            foreach (CastLog c in howling)
            {
                int start = (int)c.getTime();
                int end = start + c.getActDur();
                replay.addCircleActor(new CircleActor(true, (int)c.getTime() + c.getExpDur(), 180, new Tuple<int, int>(start, end), "rgba(0, 180, 255, 0.3)"));
                replay.addCircleActor(new CircleActor(true, 0, 180, new Tuple<int, int>(start, end), "rgba(0, 180, 255, 0.3)"));
            }
            List<CastLog> vortex = cls.Where(x => x.getID() == 47327).ToList();
            foreach (CastLog c in vortex)
            {
                int start = (int)c.getTime();
                int end = start + 4000;
                Point3D pos = replay.getPositions().FirstOrDefault(x => x.time > start);
                if (pos != null)
                {
                    replay.addCircleActor(new CircleActor(false, 0, 380, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", pos));
                    replay.addCircleActor(new CircleActor(true, end, 380, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", pos));
                    replay.addDoughnutActor(new DoughnutActor(0, 380,760, new Tuple<int, int>(end, end+1000), "rgba(255, 150, 0, 0.5)", pos));
                }
            }
            return ids;
        }

        public override string getReplayIcon()
        {
            return "https://i.imgur.com/jAiRplg.png";
        }
    }
}
