using LuckParser.Models.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Mob : AbstractMasterPlayer
    {
        // Constructors
        public Mob(AgentItem agent) : base(agent)
        {        
        }

        //setters
        protected override void SetDamageTakenLogs(ParsedLog log)
        {
        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            // todo
            int start = (int)CombatReplay.TimeOffsets.Item1;
            int end = (int)CombatReplay.TimeOffsets.Item2;
            Tuple<int, int> lifespan = new Tuple<int, int>(start, end);
            switch (ParseEnum.GetTrashIDS(Agent.ID))
            {
                case ParseEnum.TrashIDS.BlueGuardian:
                    CombatReplay.Actors.Add(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 0, 255, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.GreenGuardian:
                    CombatReplay.Actors.Add(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 255, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.RedGuardian:
                    CombatReplay.Actors.Add(new CircleActor(false, 0, 1500, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Seekers:
                    CombatReplay.Actors.Add(new CircleActor(false, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.ChargedSoul:
                    CombatReplay.Actors.Add(new CircleActor(false, 0, 220, lifespan, "rgba(255, 150, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Spirit:
                case ParseEnum.TrashIDS.Spirit2:
                    CombatReplay.Actors.Add(new CircleActor(true, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Olson:
                case ParseEnum.TrashIDS.Engul:
                case ParseEnum.TrashIDS.Faerla:
                case ParseEnum.TrashIDS.Caulle:
                case ParseEnum.TrashIDS.Henley:
                case ParseEnum.TrashIDS.Jessica:
                case ParseEnum.TrashIDS.Galletta:
                case ParseEnum.TrashIDS.Ianim:
                    CombatReplay.Actors.Add(new CircleActor(false, 0, 600, lifespan, "rgba(255, 0, 0, 0.5)"));
                    CombatReplay.Actors.Add(new CircleActor(true, 0, 400, lifespan, "rgba(0, 125, 255, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Messenger:
                    CombatReplay.Actors.Add(new CircleActor(true, 0, 180, lifespan, "rgba(255, 125, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Scythe:
                    CombatReplay.Actors.Add(new CircleActor(true, 0, 80, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Tornado:
                    CombatReplay.Actors.Add(new CircleActor(true, 0, 90, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.IcePatch:
                    CombatReplay.Actors.Add(new CircleActor(true, 0, 200, lifespan, "rgba(0, 0, 255, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Storm:
                    CombatReplay.Actors.Add(new CircleActor(false, 0, 260, lifespan, "rgba(0, 80, 255, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Oil:
<<<<<<< HEAD
                    int delay = 3000;
                    CombatReplay.CircleActors.Add(new CircleActor(true, start+150, 200, new Tuple<int, int>(start,start+delay+1000), "rgba(255,100, 0, 0.5)"));
                    CombatReplay.CircleActors.Add(new CircleActor(true, 0, 200, new Tuple<int, int>(start+delay,end), "rgba(0, 0, 0, 0.5)"));
=======
                    CombatReplay.Actors.Add(new CircleActor(true, 0, 240, lifespan, "rgba(0, 0, 0, 0.5)"));
>>>>>>> 557984c84e3b19946a3310dd98dbaad379ece291
                    break;
                case ParseEnum.TrashIDS.Echo:
                    CombatReplay.Actors.Add(new CircleActor(true, 0, 120, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.TormentedDead:
                    if (CombatReplay.Positions.Count == 0)
                    {
                        break;
                    }
                    CombatReplay.Actors.Add(new CircleActor(true,0,400,new Tuple<int, int>(end,end+60000), "rgba(255, 0, 0, 0.5)",CombatReplay.Positions.Last()));
                    break;
                case ParseEnum.TrashIDS.SurgingSoul:
                    List<Point3D> positions = CombatReplay.Positions;
                    if (positions.Count < 2)
                    {
                        break;
                    }
                    if (positions[1].X < -12000 || positions[1].X > -9250)
                    {
                        CombatReplay.Actors.Add(new RectangleActor(true, 0, 240, 660, lifespan, "rgba(255,100,0,0.5)"));
                        break;
                    }
                    else if (positions[1].Y < -525 || positions[1].Y > 2275)
                    {
                        CombatReplay.Actors.Add(new RectangleActor(true, 0, 645, 238, lifespan, "rgba(255,100,0,0.5)"));
                        break;
                    }
                    break;
            }
        }

        protected override void SetCombatReplayIcon(ParsedLog log)
        {
            switch (ParseEnum.GetTrashIDS(Agent.ID))
            {
                case ParseEnum.TrashIDS.Seekers:
                    CombatReplay.Icon = "https://i.imgur.com/FrPoluz.png";
                    break;
                case ParseEnum.TrashIDS.RedGuardian:
                    CombatReplay.Icon = "https://i.imgur.com/73Uj4lG.png";
                    break;
                case ParseEnum.TrashIDS.BlueGuardian:
                    CombatReplay.Icon = "https://i.imgur.com/6CefnkP.png";
                    break;
                case ParseEnum.TrashIDS.GreenGuardian:
                    CombatReplay.Icon = "https://i.imgur.com/nauDVYP.png";
                    break;
                case ParseEnum.TrashIDS.Spirit:
                case ParseEnum.TrashIDS.Spirit2:
                case ParseEnum.TrashIDS.ChargedSoul:
                    CombatReplay.Icon = "https://i.imgur.com/sHmksvO.png";
                    break;
                case ParseEnum.TrashIDS.Kernan:
                    CombatReplay.Icon = "https://i.imgur.com/WABRQya.png";
                    break;
                case ParseEnum.TrashIDS.Knuckles:
                    CombatReplay.Icon = "https://i.imgur.com/m1y8nJE.png";
                    break;
                case ParseEnum.TrashIDS.Karde:
                    CombatReplay.Icon = "https://i.imgur.com/3UGyosm.png";
                    break;
                case ParseEnum.TrashIDS.Olson:
                case ParseEnum.TrashIDS.Engul:
                case ParseEnum.TrashIDS.Faerla:
                case ParseEnum.TrashIDS.Caulle:
                case ParseEnum.TrashIDS.Henley:
                case ParseEnum.TrashIDS.Jessica:
                case ParseEnum.TrashIDS.Galletta:
                case ParseEnum.TrashIDS.Ianim:
                    CombatReplay.Icon = "https://i.imgur.com/qeYT1Bf.png";
                    break;
                case ParseEnum.TrashIDS.Core:
                    CombatReplay.Icon = "https://i.imgur.com/yI34iqw.png";
                    break;
                case ParseEnum.TrashIDS.Jade:
                    CombatReplay.Icon = "https://i.imgur.com/ivtzbSP.png";
                    break;
                case ParseEnum.TrashIDS.Guldhem:
                    CombatReplay.Icon = "https://i.imgur.com/xa7Fefn.png";
                    break;
                case ParseEnum.TrashIDS.Rigom:
                    CombatReplay.Icon = "https://i.imgur.com/REcGMBe.png";
                    break;
                case ParseEnum.TrashIDS.Saul:
                    CombatReplay.Icon = "https://i.imgur.com/ck2IsoS.png";
                    break;
                case ParseEnum.TrashIDS.Messenger:
                case ParseEnum.TrashIDS.TormentedDead:
                    CombatReplay.Icon = "https://i.imgur.com/1J2BTFg.png";
                    break;
                case ParseEnum.TrashIDS.Scythe:
                    CombatReplay.Icon = "https://i.imgur.com/INCGLIK.png";
                    break;
                case ParseEnum.TrashIDS.Enforcer:
                    CombatReplay.Icon = "https://i.imgur.com/elHjamF.png";
                    break;
                case ParseEnum.TrashIDS.Tornado:
                    CombatReplay.Icon = "https://i.imgur.com/e10lZMa.png";
                    break;
                case ParseEnum.TrashIDS.IcePatch:
                    CombatReplay.Icon = "https://i.imgur.com/yxKJ5Yc.png";
                    break;
                case ParseEnum.TrashIDS.Storm:
                    CombatReplay.Icon = "https://i.imgur.com/9XtNPdw.png";
                    break;
                case ParseEnum.TrashIDS.UnstableLeyRift:
                    CombatReplay.Icon = "https://i.imgur.com/YXM3igs.png";
                    break;
                case ParseEnum.TrashIDS.Tear:
                    CombatReplay.Icon = "https://i.imgur.com/N9seps0.png";
                    break;
                case ParseEnum.TrashIDS.Oil:
                    CombatReplay.Icon = "https://i.imgur.com/R26VgEr.png";
                    break;
                case ParseEnum.TrashIDS.InsidiousProjection:
                    CombatReplay.Icon = "https://i.imgur.com/9EdItBS.png";
                    break;
                case ParseEnum.TrashIDS.Pride:
                    CombatReplay.Icon = "https://i.imgur.com/ePTXx23.png";
                    break;
                case ParseEnum.TrashIDS.SurgingSoul:
                    //List<Point3D> positions = CombatReplay.GetPositions();
                    //if (positions.Count < 2)
                    //{
                        CombatReplay.Icon = "https://i.imgur.com/k79t7ZA.png";
                        break;
                    //}
                    //if (positions[1].X < -12000 || positions[1].X > -9250)
                    //{
                    //    CombatReplay.SetIcon("https://i.imgur.com/9qpuf8c.png");
                    //    break;
                    //}
                    //else if (positions[1].Y < -525 || positions[1].Y > 2275)
                    //{
                    //    CombatReplay.SetIcon("https://i.imgur.com/zNHctbS.png");
                    //    break;
                    //}
                    //CombatReplay.SetIcon("https://i.imgur.com/kcN9ECn.png");
                    //break;
                case ParseEnum.TrashIDS.Echo:
                    CombatReplay.Icon = "https://i.imgur.com/kcN9ECn.png";
                    break;
                case ParseEnum.TrashIDS.CrimsonPhantasm:
                    CombatReplay.Icon = "https://i.imgur.com/zP7Bvb4.png";
                    break;
                case ParseEnum.TrashIDS.RadiantPhantasm:
                    CombatReplay.Icon = "https://i.imgur.com/O5VWLyY.png";
                    break;
                case ParseEnum.TrashIDS.Gambler:
                case ParseEnum.TrashIDS.Thief:
                case ParseEnum.TrashIDS.Drunkard:
                    CombatReplay.Icon = "https://i.imgur.com/vINeVU6.png";
                    break;
                case ParseEnum.TrashIDS.GamblerClones:
                    CombatReplay.Icon = "https://i.imgur.com/zMsBWEx.png";
                    break;
                case ParseEnum.TrashIDS.GamblerReal:
                    CombatReplay.Icon = "https://i.imgur.com/J6oMITN.png";
                    break;
                default:
                    CombatReplay.Icon = "https://i.imgur.com/xCoypjS.png";
                    break;
            }
        }
        //
        private class Serializable
        {
            public string Img { get; set; }
            public string Type { get; set; }
            public int ID { get; set; }
            public int[] Positions { get; set; }
            public long Start { get; set; }
            public long End { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map)
        {
            Serializable aux = new Serializable
            {
                Img = CombatReplay.Icon,
                Type = "Mob",
                Positions = new int[2 * CombatReplay.Positions.Count],
                Start = CombatReplay.TimeOffsets.Item1,
                End = CombatReplay.TimeOffsets.Item2,
                ID = GetCombatReplayID()
            };
            int i = 0;
            foreach (Point3D pos in CombatReplay.Positions)
            {
                Tuple<int, int> coord = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions[i++] = coord.Item1;
                aux.Positions[i++] = coord.Item2;
            }

            return JsonConvert.SerializeObject(aux);
        }


        public override int GetCombatReplayID()
        {
            return (InstID + "_" + CombatReplay.TimeOffsets.Item1 + "_" + CombatReplay.TimeOffsets.Item2).GetHashCode();
        }

        public void AddMechanics(ParsedLog log)
        {
            // nothing to do, trash mob mechanics should be managed by the boss
        }
    }
}
