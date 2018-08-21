using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class SoullessHorror : BossLogic
    {
        public SoullessHorror()
        {
            Mode = ParseMode.Raid;
            MechanicList.AddRange(new List<Mechanic>
            {

            new Mechanic(47327, "Vortex Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle',color:'rgb(255,140,0)',", "D.In",0), // Vortex Slash (Inner Donut hit), Inner Donut
            new Mechanic(48432, "Vortex Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle-open',color:'rgb(255,140,0)',", "D.Out",0), // Vortex Slash (Outer Donut hit), Outer Donut
            new Mechanic(47430, "Soul Rift", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle-open',color:'rgb(255,0,0)',", "Golem",0), //Soul Rift (stood in Golem Aoe), Golem Aoe
            new Mechanic(48363, "Quad Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-square-open',color:'rgb(255,140,0)',", "Slcs1",0), //Quad Slash (4 Slices), 4 Slices 
            new Mechanic(47915, "Quad Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-diamond-open',color:'rgb(255,140,0)',", "Slcs2",0), //Quad Slash (4 Slices), 4 Slices
            new Mechanic(47363, "Spinning Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-triangle-up-open',color:'rgb(128,0,0)',", "Scth",0), //Spinning Slash (hit by Scythe), Scythe
            new Mechanic(48500, "Death Bloom", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'octagon',color:'rgb(255,140,0)',", " 8Slcs",0), //Death Bloom (8 Slices), 8 Slices
            new Mechanic(47434, "Fixated", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.SoullessHorror, "symbol:'star',color:'rgb(255,0,255)',", "Fix",0), //Fixated (Special Action Key), Fixated
            new Mechanic(47414, "Necrosis", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-open',color:'rgb(255,0,255)',", "Necr",0) //Necrosis (Tanking Debuff), Necrosis Debuff
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

        public override List<ParseEnum.ThrashIDS> GetAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: facing information (slashes) and doughnuts for outer circle attack
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Scythe,
                        ParseEnum.ThrashIDS.TormentedDead,
                        ParseEnum.ThrashIDS.SurgingSoul
                    };
            List<CastLog> howling = cls.Where(x => x.GetID() == 48662).ToList();
            foreach (CastLog c in howling)
            {
                int start = (int)c.GetTime();
                int end = start + c.GetActDur();
                replay.AddCircleActor(new CircleActor(true, (int)c.GetTime() + c.GetExpDur(), 180, new Tuple<int, int>(start, end), "rgba(0, 180, 255, 0.3)"));
                replay.AddCircleActor(new CircleActor(true, 0, 180, new Tuple<int, int>(start, end), "rgba(0, 180, 255, 0.3)"));
            }
            List<CastLog> vortex = cls.Where(x => x.GetID() == 47327).ToList();
            foreach (CastLog c in vortex)
            {
                int start = (int)c.GetTime();
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
            List<CombatItem> necrosis = clist.Where(x => x.GetSkillID() == 47414 && x.IsBuffremove() == ParseEnum.BuffRemove.None).ToList();
            if (necrosis.Count == 0)
            {
                return 0;
            }
            // split necrosis
            Dictionary<ushort, List<CombatItem>> splitNecrosis = new Dictionary<ushort, List<CombatItem>>();
            foreach (CombatItem c in necrosis)
            {
                ushort inst = c.GetDstInstid();
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
                long timeDiff = next.GetTime() - cur.GetTime();
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
