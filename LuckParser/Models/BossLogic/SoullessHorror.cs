using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class SoullessHorror : RaidLogic
    {
        public SoullessHorror()
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new Mechanic(47327, "Vortex Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle',color:'rgb(255,140,0)',", "D.In","Vortex Slash (Inner Donut hit)", "Inner Donut",0),
            new Mechanic(48432, "Vortex Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle-open',color:'rgb(255,140,0)',", "D.Out","Vortex Slash (Outer Donut hit)", "Outer Donut", 0),
            new Mechanic(47430, "Soul Rift", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle-open',color:'rgb(255,0,0)',", "Golem","Soul Rift (stood in Golem Aoe)", "Golem Aoe",0),
            new Mechanic(48363, "Quad Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-square-open',color:'rgb(255,140,0)',", "Slcs","Quad Slash (4 Slices)", "4 Slices",0),
            new Mechanic(47915, "Quad Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-diamond-open',color:'rgb(255,140,0)',", "Slcs","Quad Slash (4 Slices)", "4 Slices",0),
            new Mechanic(47363, "Spinning Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-triangle-up-open',color:'rgb(128,0,0)',", "Scth","Spinning Slash (hit by Scythe)", "Scythe",0),
            new Mechanic(48500, "Death Bloom", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'octagon',color:'rgb(255,140,0)',", "8Slcs","Death Bloom (8 Slices)", "8 Slices",0),
            new Mechanic(47434, "Fixated", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.SoullessHorror, "symbol:'star',color:'rgb(255,0,255)',", "Fix","Fixated (Special Action Key)", "Fixated",0),
            new Mechanic(47414, "Necrosis", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-open',color:'rgb(255,0,255)',", "Necr","Necrosis (Tanking Debuff)", "Necrosis Debuff",0),
            new Mechanic(48327, "Corrupt the Living", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle',color:'rgb(255,0,0)',", "Spin","Corrupt the Living (Torment+Poisen Spin)", "Torment+Poisen Spin",0),
            new Mechanic(47756, "Wurm Spit", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'diamond-open',color:'rgb(0,128,128)',", "Spit","Wurm Spit", "Wurm Spit",0),
            new Mechanic(48662, "Howling Death", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.SoullessHorror, "symbol:'diamond-tall',color:'rgb(0,160,150)',", "CC","Howling Death (Breakbar)", "Breakbar",0),
            new Mechanic(48662, "Howling Death", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.SoullessHorror, "symbol:'diamond-tall',color:'rgb(0,160,0)',", "CCed","Howling Death (Breakbar) broken", "CCed",0,(condition => condition.CombatItem.Value <=6800)),
            new Mechanic(48662, "Howling Death", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.SoullessHorror, "symbol:'diamond-tall',color:'rgb(255,0,0)',", "CC.Fail","Howling Death (Breakbar failed) ", "CC Fail",0,(condition => condition.CombatItem.Value >6800)),

            });
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/A45pVJy.png",
                            Tuple.Create(3657, 3657),
                            Tuple.Create(-12223, -771, -8932, 2420),
                            Tuple.Create(-21504, -12288, 24576, 12288),
                            Tuple.Create(19072, 15484, 20992, 16508));
        }

        public override List<ParseEnum.TrashIDS> GetAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: facing information (slashes) and doughnuts for outer circle attack
            List<ParseEnum.TrashIDS> ids = new List<ParseEnum.TrashIDS>
                    {
                        ParseEnum.TrashIDS.Scythe,
                        ParseEnum.TrashIDS.TormentedDead,
                        ParseEnum.TrashIDS.SurgingSoul
                    };
            List<CastLog> howling = cls.Where(x => x.SkillId == 48662).ToList();
            foreach (CastLog c in howling)
            {
                int start = (int)c.Time;
                int end = start + c.ActualDuration;
                replay.AddCircleActor(new CircleActor(true, (int)c.Time + c.ExpectedDuration, 180, new Tuple<int, int>(start, end), "rgba(0, 180, 255, 0.3)"));
                replay.AddCircleActor(new CircleActor(true, 0, 180, new Tuple<int, int>(start, end), "rgba(0, 180, 255, 0.3)"));
            }
            List<CastLog> vortex = cls.Where(x => x.SkillId == 47327).ToList();
            foreach (CastLog c in vortex)
            {
                int start = (int)c.Time;
                int end = start + 4000;
                Point3D pos = replay.GetPositions().FirstOrDefault(x => x.Time > start);
                if (pos != null)
                {
                    replay.AddCircleActor(new CircleActor(false, 0, 380, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", pos));
                    replay.AddCircleActor(new CircleActor(true, end, 380, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", pos));
                    replay.AddDoughnutActor(new DoughnutActor(0, 380,760, new Tuple<int, int>(end, end+1000), "rgba(255, 150, 0, 0.5)", pos));
                }
            }
            return ids;
        }

        public override int IsCM(List<CombatItem> clist, int health)
        {
            List<CombatItem> necrosis = clist.Where(x => x.SkillID == 47414 && x.IsBuffRemove == ParseEnum.BuffRemove.None).ToList();
            if (necrosis.Count == 0)
            {
                return 0;
            }
            // split necrosis
            Dictionary<ushort, List<CombatItem>> splitNecrosis = new Dictionary<ushort, List<CombatItem>>();
            foreach (CombatItem c in necrosis)
            {
                ushort inst = c.DstInstid;
                if (!splitNecrosis.ContainsKey(inst))
                {
                    splitNecrosis.Add(inst, new List<CombatItem>());
                }
                splitNecrosis[inst].Add(c);
            }
            List<CombatItem> longestNecrosis = splitNecrosis.Values.First(l => l.Count == splitNecrosis.Values.Max(x => x.Count));
            long minDiff = long.MaxValue;
            for (int i = 0; i < longestNecrosis.Count - 1; i++)
            {
                CombatItem cur = longestNecrosis[i];
                CombatItem next = longestNecrosis[i + 1];
                long timeDiff = next.Time - cur.Time;
                if (timeDiff > 1000 && minDiff > timeDiff)
                {
                    minDiff = timeDiff;
                }
            }
            return (minDiff < 11000) ? 1 : 0;
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/jAiRplg.png";
        }
    }
}
