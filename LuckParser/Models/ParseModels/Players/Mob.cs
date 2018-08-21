using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class Mob : AbstractMasterPlayer
    {
        // Constructors
        public Mob(AgentItem agent) : base(agent)
        {        
        }

        //setters
        protected override void SetDamagetakenLogs(ParsedLog log)
        {
        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            // todo
            int start = (int)replay.GetTimeOffsets().Item1;
            int end = (int)replay.GetTimeOffsets().Item2;
            Tuple<int, int> lifespan = new Tuple<int, int>(start, end);
            switch (ParseEnum.GetThrashIDS(agent.GetID()))
            {
                case ParseEnum.ThrashIDS.BlueGuardian:
                    replay.AddCircleActor(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 0, 255, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.GreenGuardian:
                    replay.AddCircleActor(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 255, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.RedGuardian:
                    replay.AddCircleActor(new CircleActor(false, 0, 1500, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Seekers:
                    replay.AddCircleActor(new CircleActor(false, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.ChargedSoul:
                    replay.AddCircleActor(new CircleActor(false, 0, 220, lifespan, "rgba(255, 150, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Spirit:
                case ParseEnum.ThrashIDS.Spirit2:
                    replay.AddCircleActor(new CircleActor(true, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Olson:
                case ParseEnum.ThrashIDS.Engul:
                case ParseEnum.ThrashIDS.Faerla:
                case ParseEnum.ThrashIDS.Caulle:
                case ParseEnum.ThrashIDS.Henley:
                case ParseEnum.ThrashIDS.Jessica:
                case ParseEnum.ThrashIDS.Galletta:
                case ParseEnum.ThrashIDS.Ianim:
                    replay.AddCircleActor(new CircleActor(false, 0, 600, lifespan, "rgba(255, 0, 0, 0.5)"));
                    replay.AddCircleActor(new CircleActor(true, 0, 400, lifespan, "rgba(0, 125, 255, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Messenger:
                    replay.AddCircleActor(new CircleActor(true, 0, 180, lifespan, "rgba(255, 125, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Scythe:
                    replay.AddCircleActor(new CircleActor(true, 0, 80, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Tornado:
                    replay.AddCircleActor(new CircleActor(true, 0, 90, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.IcePatch:
                    replay.AddCircleActor(new CircleActor(true, 0, 200, lifespan, "rgba(0, 0, 255, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Storm:
                    replay.AddCircleActor(new CircleActor(false, 0, 260, lifespan, "rgba(0, 80, 255, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Oil:
                    replay.AddCircleActor(new CircleActor(true, 0, 240, lifespan, "rgba(0, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Echo:
                    replay.AddCircleActor(new CircleActor(true, 0, 120, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.TormentedDead:
                    if (replay.GetPositions().Count == 0)
                    {
                        break;
                    }
                    replay.AddCircleActor(new CircleActor(true,0,400,new Tuple<int, int>(end,end+60000), "rgba(255, 0, 0, 0.5)",replay.GetPositions().Last()));
                    break;
            }
        }

        protected override void SetCombatReplayIcon(ParsedLog log)
        {
            switch (ParseEnum.GetThrashIDS(agent.GetID()))
            {
                case ParseEnum.ThrashIDS.Seekers:
                    replay.SetIcon("https://i.imgur.com/FrPoluz.png");
                    break;
                case ParseEnum.ThrashIDS.RedGuardian:
                    replay.SetIcon("https://i.imgur.com/73Uj4lG.png");
                    break;
                case ParseEnum.ThrashIDS.BlueGuardian:
                    replay.SetIcon("https://i.imgur.com/6CefnkP.png");
                    break;
                case ParseEnum.ThrashIDS.GreenGuardian:
                    replay.SetIcon("https://i.imgur.com/nauDVYP.png");
                    break;
                case ParseEnum.ThrashIDS.Spirit:
                case ParseEnum.ThrashIDS.Spirit2:
                case ParseEnum.ThrashIDS.ChargedSoul:
                    replay.SetIcon("https://i.imgur.com/sHmksvO.png");
                    break;
                case ParseEnum.ThrashIDS.Kernan:
                    replay.SetIcon("https://i.imgur.com/WABRQya.png");
                    break;
                case ParseEnum.ThrashIDS.Knuckles:
                    replay.SetIcon("https://i.imgur.com/m1y8nJE.png");
                    break;
                case ParseEnum.ThrashIDS.Karde:
                    replay.SetIcon("https://i.imgur.com/3UGyosm.png");
                    break;
                case ParseEnum.ThrashIDS.Olson:
                case ParseEnum.ThrashIDS.Engul:
                case ParseEnum.ThrashIDS.Faerla:
                case ParseEnum.ThrashIDS.Caulle:
                case ParseEnum.ThrashIDS.Henley:
                case ParseEnum.ThrashIDS.Jessica:
                case ParseEnum.ThrashIDS.Galletta:
                case ParseEnum.ThrashIDS.Ianim:
                    replay.SetIcon("https://i.imgur.com/qeYT1Bf.png");
                    break;
                case ParseEnum.ThrashIDS.Core:
                    replay.SetIcon("https://i.imgur.com/yI34iqw.png");
                    break;
                case ParseEnum.ThrashIDS.Jade:
                    replay.SetIcon("https://i.imgur.com/ivtzbSP.png");
                    break;
                case ParseEnum.ThrashIDS.Guldhem:
                    replay.SetIcon("https://i.imgur.com/xa7Fefn.png");
                    break;
                case ParseEnum.ThrashIDS.Rigom:
                    replay.SetIcon("https://i.imgur.com/REcGMBe.png");
                    break;
                case ParseEnum.ThrashIDS.Saul:
                    replay.SetIcon("https://i.imgur.com/ck2IsoS.png");
                    break;
                case ParseEnum.ThrashIDS.Messenger:
                case ParseEnum.ThrashIDS.TormentedDead:
                    replay.SetIcon("https://i.imgur.com/1J2BTFg.png");
                    break;
                case ParseEnum.ThrashIDS.Scythe:
                    replay.SetIcon("https://i.imgur.com/INCGLIK.png");
                    break;
                case ParseEnum.ThrashIDS.Enforcer:
                    replay.SetIcon("https://i.imgur.com/elHjamF.png");
                    break;
                case ParseEnum.ThrashIDS.Tornado:
                    replay.SetIcon("https://i.imgur.com/e10lZMa.png");
                    break;
                case ParseEnum.ThrashIDS.IcePatch:
                    replay.SetIcon("https://i.imgur.com/yxKJ5Yc.png");
                    break;
                case ParseEnum.ThrashIDS.Storm:
                    replay.SetIcon("https://i.imgur.com/9XtNPdw.png");
                    break;
                case ParseEnum.ThrashIDS.UnstableLeyRift:
                    replay.SetIcon("https://i.imgur.com/YXM3igs.png");
                    break;
                case ParseEnum.ThrashIDS.Tear:
                    replay.SetIcon("https://i.imgur.com/N9seps0.png");
                    break;
                case ParseEnum.ThrashIDS.Oil:
                    replay.SetIcon("https://i.imgur.com/DZIl49i.png");
                    break;
                case ParseEnum.ThrashIDS.InsidiousProjection:
                    replay.SetIcon("https://i.imgur.com/9EdItBS.png");
                    break;
                case ParseEnum.ThrashIDS.Pride:
                    replay.SetIcon("https://i.imgur.com/ePTXx23.png");
                    break;
                case ParseEnum.ThrashIDS.SurgingSoul:
                    replay.SetIcon("https://i.imgur.com/k79t7ZA.png");
                    break;
                case ParseEnum.ThrashIDS.Echo:
                    replay.SetIcon("https://i.imgur.com/kcN9ECn.png");
                    break;
                case ParseEnum.ThrashIDS.CrimsonPhantasm:
                    replay.SetIcon("https://i.imgur.com/zP7Bvb4.png");
                    break;
                case ParseEnum.ThrashIDS.RadiantPhantasm:
                    replay.SetIcon("https://i.imgur.com/O5VWLyY.png");
                    break;
                case ParseEnum.ThrashIDS.Gambler:
                case ParseEnum.ThrashIDS.Thief:
                case ParseEnum.ThrashIDS.Drunkard:
                    replay.SetIcon("https://i.imgur.com/vINeVU6.png");
                    break;
                case ParseEnum.ThrashIDS.GamblerClones:
                    replay.SetIcon("https://i.imgur.com/zMsBWEx.png");
                    break;
                case ParseEnum.ThrashIDS.GamblerReal:
                    replay.SetIcon("https://i.imgur.com/J6oMITN.png");
                    break;
                default:
                    replay.SetIcon("https://i.imgur.com/xCoypjS.png");
                    break;
            }
        }

        public override void AddMechanics(ParsedLog log)
        {
            // nothing to do, thrash mob mechanics should be managed by the boss
        }
    }
}
