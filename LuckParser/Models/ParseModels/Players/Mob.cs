using LuckParser.Models.DataModels;
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
            int start = (int)CombatReplay.GetTimeOffsets().Item1;
            int end = (int)CombatReplay.GetTimeOffsets().Item2;
            Tuple<int, int> lifespan = new Tuple<int, int>(start, end);
            switch (ParseEnum.GetTrashIDS(Agent.ID))
            {
                case ParseEnum.TrashIDS.BlueGuardian:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 0, 255, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.GreenGuardian:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 255, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.RedGuardian:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 1500, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Seekers:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.ChargedSoul:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 220, lifespan, "rgba(255, 150, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Spirit:
                case ParseEnum.TrashIDS.Spirit2:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Olson:
                case ParseEnum.TrashIDS.Engul:
                case ParseEnum.TrashIDS.Faerla:
                case ParseEnum.TrashIDS.Caulle:
                case ParseEnum.TrashIDS.Henley:
                case ParseEnum.TrashIDS.Jessica:
                case ParseEnum.TrashIDS.Galletta:
                case ParseEnum.TrashIDS.Ianim:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 600, lifespan, "rgba(255, 0, 0, 0.5)"));
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 400, lifespan, "rgba(0, 125, 255, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Messenger:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 180, lifespan, "rgba(255, 125, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Scythe:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 80, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Tornado:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 90, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.IcePatch:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 200, lifespan, "rgba(0, 0, 255, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Storm:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 260, lifespan, "rgba(0, 80, 255, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Oil:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 240, lifespan, "rgba(0, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.Echo:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 120, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.TrashIDS.TormentedDead:
                    if (CombatReplay.GetPositions().Count == 0)
                    {
                        break;
                    }
                    CombatReplay.AddCircleActor(new CircleActor(true,0,400,new Tuple<int, int>(end,end+60000), "rgba(255, 0, 0, 0.5)",CombatReplay.GetPositions().Last()));
                    break;
                case ParseEnum.TrashIDS.SurgingSoul:
                    List<Point3D> positions = CombatReplay.GetPositions();
                    if (positions.Count < 2)
                    {
                        break;
                    }
                    if (positions[1].X < -12000 || positions[1].X > -9250)
                    {
                        CombatReplay.AddRectangleActor(new RectangleActor(true, 0, 240, 660, lifespan, "rgba(255,100,0,0.5)"));
                        break;
                    }
                    else if (positions[1].Y < -525 || positions[1].Y > 2275)
                    {
                        CombatReplay.AddRectangleActor(new RectangleActor(true, 0, 645, 238, lifespan, "rgba(255,100,0,0.5)"));
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
                    CombatReplay.SetIcon("https://i.imgur.com/FrPoluz.png");
                    break;
                case ParseEnum.TrashIDS.RedGuardian:
                    CombatReplay.SetIcon("https://i.imgur.com/73Uj4lG.png");
                    break;
                case ParseEnum.TrashIDS.BlueGuardian:
                    CombatReplay.SetIcon("https://i.imgur.com/6CefnkP.png");
                    break;
                case ParseEnum.TrashIDS.GreenGuardian:
                    CombatReplay.SetIcon("https://i.imgur.com/nauDVYP.png");
                    break;
                case ParseEnum.TrashIDS.Spirit:
                case ParseEnum.TrashIDS.Spirit2:
                case ParseEnum.TrashIDS.ChargedSoul:
                    CombatReplay.SetIcon("https://i.imgur.com/sHmksvO.png");
                    break;
                case ParseEnum.TrashIDS.Kernan:
                    CombatReplay.SetIcon("https://i.imgur.com/WABRQya.png");
                    break;
                case ParseEnum.TrashIDS.Knuckles:
                    CombatReplay.SetIcon("https://i.imgur.com/m1y8nJE.png");
                    break;
                case ParseEnum.TrashIDS.Karde:
                    CombatReplay.SetIcon("https://i.imgur.com/3UGyosm.png");
                    break;
                case ParseEnum.TrashIDS.Olson:
                case ParseEnum.TrashIDS.Engul:
                case ParseEnum.TrashIDS.Faerla:
                case ParseEnum.TrashIDS.Caulle:
                case ParseEnum.TrashIDS.Henley:
                case ParseEnum.TrashIDS.Jessica:
                case ParseEnum.TrashIDS.Galletta:
                case ParseEnum.TrashIDS.Ianim:
                    CombatReplay.SetIcon("https://i.imgur.com/qeYT1Bf.png");
                    break;
                case ParseEnum.TrashIDS.Core:
                    CombatReplay.SetIcon("https://i.imgur.com/yI34iqw.png");
                    break;
                case ParseEnum.TrashIDS.Jade:
                    CombatReplay.SetIcon("https://i.imgur.com/ivtzbSP.png");
                    break;
                case ParseEnum.TrashIDS.Guldhem:
                    CombatReplay.SetIcon("https://i.imgur.com/xa7Fefn.png");
                    break;
                case ParseEnum.TrashIDS.Rigom:
                    CombatReplay.SetIcon("https://i.imgur.com/REcGMBe.png");
                    break;
                case ParseEnum.TrashIDS.Saul:
                    CombatReplay.SetIcon("https://i.imgur.com/ck2IsoS.png");
                    break;
                case ParseEnum.TrashIDS.Messenger:
                case ParseEnum.TrashIDS.TormentedDead:
                    CombatReplay.SetIcon("https://i.imgur.com/1J2BTFg.png");
                    break;
                case ParseEnum.TrashIDS.Scythe:
                    CombatReplay.SetIcon("https://i.imgur.com/INCGLIK.png");
                    break;
                case ParseEnum.TrashIDS.Enforcer:
                    CombatReplay.SetIcon("https://i.imgur.com/elHjamF.png");
                    break;
                case ParseEnum.TrashIDS.Tornado:
                    CombatReplay.SetIcon("https://i.imgur.com/e10lZMa.png");
                    break;
                case ParseEnum.TrashIDS.IcePatch:
                    CombatReplay.SetIcon("https://i.imgur.com/yxKJ5Yc.png");
                    break;
                case ParseEnum.TrashIDS.Storm:
                    CombatReplay.SetIcon("https://i.imgur.com/9XtNPdw.png");
                    break;
                case ParseEnum.TrashIDS.UnstableLeyRift:
                    CombatReplay.SetIcon("https://i.imgur.com/YXM3igs.png");
                    break;
                case ParseEnum.TrashIDS.Tear:
                    CombatReplay.SetIcon("https://i.imgur.com/N9seps0.png");
                    break;
                case ParseEnum.TrashIDS.Oil:
                    CombatReplay.SetIcon("https://i.imgur.com/DZIl49i.png");
                    break;
                case ParseEnum.TrashIDS.InsidiousProjection:
                    CombatReplay.SetIcon("https://i.imgur.com/9EdItBS.png");
                    break;
                case ParseEnum.TrashIDS.Pride:
                    CombatReplay.SetIcon("https://i.imgur.com/ePTXx23.png");
                    break;
                case ParseEnum.TrashIDS.SurgingSoul:
                    //List<Point3D> positions = CombatReplay.GetPositions();
                    //if (positions.Count < 2)
                    //{
                        CombatReplay.SetIcon("https://i.imgur.com/k79t7ZA.png");
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
                    CombatReplay.SetIcon("https://i.imgur.com/kcN9ECn.png");
                    break;
                case ParseEnum.TrashIDS.CrimsonPhantasm:
                    CombatReplay.SetIcon("https://i.imgur.com/zP7Bvb4.png");
                    break;
                case ParseEnum.TrashIDS.RadiantPhantasm:
                    CombatReplay.SetIcon("https://i.imgur.com/O5VWLyY.png");
                    break;
                case ParseEnum.TrashIDS.Gambler:
                case ParseEnum.TrashIDS.Thief:
                case ParseEnum.TrashIDS.Drunkard:
                    CombatReplay.SetIcon("https://i.imgur.com/vINeVU6.png");
                    break;
                case ParseEnum.TrashIDS.GamblerClones:
                    CombatReplay.SetIcon("https://i.imgur.com/zMsBWEx.png");
                    break;
                case ParseEnum.TrashIDS.GamblerReal:
                    CombatReplay.SetIcon("https://i.imgur.com/J6oMITN.png");
                    break;
                default:
                    CombatReplay.SetIcon("https://i.imgur.com/xCoypjS.png");
                    break;
            }
        }

        public void AddMechanics(ParsedLog log)
        {
            // nothing to do, trash mob mechanics should be managed by the boss
        }
    }
}
