using LuckParser.Models.DataModels;
using System;
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
        protected override void SetDamagetakenLogs(ParsedLog log)
        {
        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            // todo
            int start = (int)Replay.GetTimeOffsets().Item1;
            int end = (int)Replay.GetTimeOffsets().Item2;
            Tuple<int, int> lifespan = new Tuple<int, int>(start, end);
            switch (ParseEnum.GetThrashIDS(Agent.GetID()))
            {
                case ParseEnum.ThrashIDS.BlueGuardian:
                    Replay.AddCircleActor(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 0, 255, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.GreenGuardian:
                    Replay.AddCircleActor(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 255, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.RedGuardian:
                    Replay.AddCircleActor(new CircleActor(false, 0, 1500, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Seekers:
                    Replay.AddCircleActor(new CircleActor(false, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.ChargedSoul:
                    Replay.AddCircleActor(new CircleActor(false, 0, 220, lifespan, "rgba(255, 150, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Spirit:
                case ParseEnum.ThrashIDS.Spirit2:
                    Replay.AddCircleActor(new CircleActor(true, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Olson:
                case ParseEnum.ThrashIDS.Engul:
                case ParseEnum.ThrashIDS.Faerla:
                case ParseEnum.ThrashIDS.Caulle:
                case ParseEnum.ThrashIDS.Henley:
                case ParseEnum.ThrashIDS.Jessica:
                case ParseEnum.ThrashIDS.Galletta:
                case ParseEnum.ThrashIDS.Ianim:
                    Replay.AddCircleActor(new CircleActor(false, 0, 600, lifespan, "rgba(255, 0, 0, 0.5)"));
                    Replay.AddCircleActor(new CircleActor(true, 0, 400, lifespan, "rgba(0, 125, 255, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Messenger:
                    Replay.AddCircleActor(new CircleActor(true, 0, 180, lifespan, "rgba(255, 125, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Scythe:
                    Replay.AddCircleActor(new CircleActor(true, 0, 80, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Tornado:
                    Replay.AddCircleActor(new CircleActor(true, 0, 90, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.IcePatch:
                    Replay.AddCircleActor(new CircleActor(true, 0, 200, lifespan, "rgba(0, 0, 255, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Storm:
                    Replay.AddCircleActor(new CircleActor(false, 0, 260, lifespan, "rgba(0, 80, 255, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Oil:
                    Replay.AddCircleActor(new CircleActor(true, 0, 240, lifespan, "rgba(0, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Echo:
                    Replay.AddCircleActor(new CircleActor(true, 0, 120, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.TormentedDead:
                    if (Replay.GetPositions().Count == 0)
                    {
                        break;
                    }
                    Replay.AddCircleActor(new CircleActor(true,0,400,new Tuple<int, int>(end,end+60000), "rgba(255, 0, 0, 0.5)",Replay.GetPositions().Last()));
                    break;
            }
        }

        protected override void SetCombatReplayIcon(ParsedLog log)
        {
            switch (ParseEnum.GetThrashIDS(Agent.GetID()))
            {
                case ParseEnum.ThrashIDS.Seekers:
                    Replay.SetIcon("https://i.imgur.com/FrPoluz.png");
                    break;
                case ParseEnum.ThrashIDS.RedGuardian:
                    Replay.SetIcon("https://i.imgur.com/73Uj4lG.png");
                    break;
                case ParseEnum.ThrashIDS.BlueGuardian:
                    Replay.SetIcon("https://i.imgur.com/6CefnkP.png");
                    break;
                case ParseEnum.ThrashIDS.GreenGuardian:
                    Replay.SetIcon("https://i.imgur.com/nauDVYP.png");
                    break;
                case ParseEnum.ThrashIDS.Spirit:
                case ParseEnum.ThrashIDS.Spirit2:
                case ParseEnum.ThrashIDS.ChargedSoul:
                    Replay.SetIcon("https://i.imgur.com/sHmksvO.png");
                    break;
                case ParseEnum.ThrashIDS.Kernan:
                    Replay.SetIcon("https://i.imgur.com/WABRQya.png");
                    break;
                case ParseEnum.ThrashIDS.Knuckles:
                    Replay.SetIcon("https://i.imgur.com/m1y8nJE.png");
                    break;
                case ParseEnum.ThrashIDS.Karde:
                    Replay.SetIcon("https://i.imgur.com/3UGyosm.png");
                    break;
                case ParseEnum.ThrashIDS.Olson:
                case ParseEnum.ThrashIDS.Engul:
                case ParseEnum.ThrashIDS.Faerla:
                case ParseEnum.ThrashIDS.Caulle:
                case ParseEnum.ThrashIDS.Henley:
                case ParseEnum.ThrashIDS.Jessica:
                case ParseEnum.ThrashIDS.Galletta:
                case ParseEnum.ThrashIDS.Ianim:
                    Replay.SetIcon("https://i.imgur.com/qeYT1Bf.png");
                    break;
                case ParseEnum.ThrashIDS.Core:
                    Replay.SetIcon("https://i.imgur.com/yI34iqw.png");
                    break;
                case ParseEnum.ThrashIDS.Jade:
                    Replay.SetIcon("https://i.imgur.com/ivtzbSP.png");
                    break;
                case ParseEnum.ThrashIDS.Guldhem:
                    Replay.SetIcon("https://i.imgur.com/xa7Fefn.png");
                    break;
                case ParseEnum.ThrashIDS.Rigom:
                    Replay.SetIcon("https://i.imgur.com/REcGMBe.png");
                    break;
                case ParseEnum.ThrashIDS.Saul:
                    Replay.SetIcon("https://i.imgur.com/ck2IsoS.png");
                    break;
                case ParseEnum.ThrashIDS.Messenger:
                case ParseEnum.ThrashIDS.TormentedDead:
                    Replay.SetIcon("https://i.imgur.com/1J2BTFg.png");
                    break;
                case ParseEnum.ThrashIDS.Scythe:
                    Replay.SetIcon("https://i.imgur.com/INCGLIK.png");
                    break;
                case ParseEnum.ThrashIDS.Enforcer:
                    Replay.SetIcon("https://i.imgur.com/elHjamF.png");
                    break;
                case ParseEnum.ThrashIDS.Tornado:
                    Replay.SetIcon("https://i.imgur.com/e10lZMa.png");
                    break;
                case ParseEnum.ThrashIDS.IcePatch:
                    Replay.SetIcon("https://i.imgur.com/yxKJ5Yc.png");
                    break;
                case ParseEnum.ThrashIDS.Storm:
                    Replay.SetIcon("https://i.imgur.com/9XtNPdw.png");
                    break;
                case ParseEnum.ThrashIDS.UnstableLeyRift:
                    Replay.SetIcon("https://i.imgur.com/YXM3igs.png");
                    break;
                case ParseEnum.ThrashIDS.Tear:
                    Replay.SetIcon("https://i.imgur.com/N9seps0.png");
                    break;
                case ParseEnum.ThrashIDS.Oil:
                    Replay.SetIcon("https://i.imgur.com/DZIl49i.png");
                    break;
                case ParseEnum.ThrashIDS.InsidiousProjection:
                    Replay.SetIcon("https://i.imgur.com/9EdItBS.png");
                    break;
                case ParseEnum.ThrashIDS.Pride:
                    Replay.SetIcon("https://i.imgur.com/ePTXx23.png");
                    break;
                case ParseEnum.ThrashIDS.SurgingSoul:
                    Replay.SetIcon("https://i.imgur.com/k79t7ZA.png");
                    break;
                case ParseEnum.ThrashIDS.Echo:
                    Replay.SetIcon("https://i.imgur.com/kcN9ECn.png");
                    break;
                case ParseEnum.ThrashIDS.CrimsonPhantasm:
                    Replay.SetIcon("https://i.imgur.com/zP7Bvb4.png");
                    break;
                case ParseEnum.ThrashIDS.RadiantPhantasm:
                    Replay.SetIcon("https://i.imgur.com/O5VWLyY.png");
                    break;
                case ParseEnum.ThrashIDS.Gambler:
                case ParseEnum.ThrashIDS.Thief:
                case ParseEnum.ThrashIDS.Drunkard:
                    Replay.SetIcon("https://i.imgur.com/vINeVU6.png");
                    break;
                case ParseEnum.ThrashIDS.GamblerClones:
                    Replay.SetIcon("https://i.imgur.com/zMsBWEx.png");
                    break;
                case ParseEnum.ThrashIDS.GamblerReal:
                    Replay.SetIcon("https://i.imgur.com/J6oMITN.png");
                    break;
                default:
                    Replay.SetIcon("https://i.imgur.com/xCoypjS.png");
                    break;
            }
        }

        public override void AddMechanics(ParsedLog log)
        {
            // nothing to do, thrash mob mechanics should be managed by the boss
        }
    }
}
