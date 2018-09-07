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
        protected override void SetDamageTakenLogs(ParsedLog log)
        {
        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            // todo
            int start = (int)CombatReplay.GetTimeOffsets().Item1;
            int end = (int)CombatReplay.GetTimeOffsets().Item2;
            Tuple<int, int> lifespan = new Tuple<int, int>(start, end);
            switch (ParseEnum.GetThrashIDS(Agent.ID))
            {
                case ParseEnum.ThrashIDS.BlueGuardian:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 0, 255, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.GreenGuardian:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 255, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.RedGuardian:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 1500, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Seekers:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.ChargedSoul:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 220, lifespan, "rgba(255, 150, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Spirit:
                case ParseEnum.ThrashIDS.Spirit2:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Olson:
                case ParseEnum.ThrashIDS.Engul:
                case ParseEnum.ThrashIDS.Faerla:
                case ParseEnum.ThrashIDS.Caulle:
                case ParseEnum.ThrashIDS.Henley:
                case ParseEnum.ThrashIDS.Jessica:
                case ParseEnum.ThrashIDS.Galletta:
                case ParseEnum.ThrashIDS.Ianim:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 600, lifespan, "rgba(255, 0, 0, 0.5)"));
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 400, lifespan, "rgba(0, 125, 255, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Messenger:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 180, lifespan, "rgba(255, 125, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Scythe:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 80, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Tornado:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 90, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.IcePatch:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 200, lifespan, "rgba(0, 0, 255, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Storm:
                    CombatReplay.AddCircleActor(new CircleActor(false, 0, 260, lifespan, "rgba(0, 80, 255, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Oil:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 240, lifespan, "rgba(0, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.Echo:
                    CombatReplay.AddCircleActor(new CircleActor(true, 0, 120, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                case ParseEnum.ThrashIDS.TormentedDead:
                    if (CombatReplay.GetPositions().Count == 0)
                    {
                        break;
                    }
                    CombatReplay.AddCircleActor(new CircleActor(true,0,400,new Tuple<int, int>(end,end+60000), "rgba(255, 0, 0, 0.5)",CombatReplay.GetPositions().Last()));
                    break;
            }
        }

        protected override void SetCombatReplayIcon(ParsedLog log)
        {
            switch (ParseEnum.GetThrashIDS(Agent.ID))
            {
                case ParseEnum.ThrashIDS.Seekers:
                    CombatReplay.SetIcon("https://i.imgur.com/FrPoluz.png");
                    break;
                case ParseEnum.ThrashIDS.RedGuardian:
                    CombatReplay.SetIcon("https://i.imgur.com/73Uj4lG.png");
                    break;
                case ParseEnum.ThrashIDS.BlueGuardian:
                    CombatReplay.SetIcon("https://i.imgur.com/6CefnkP.png");
                    break;
                case ParseEnum.ThrashIDS.GreenGuardian:
                    CombatReplay.SetIcon("https://i.imgur.com/nauDVYP.png");
                    break;
                case ParseEnum.ThrashIDS.Spirit:
                case ParseEnum.ThrashIDS.Spirit2:
                case ParseEnum.ThrashIDS.ChargedSoul:
                    CombatReplay.SetIcon("https://i.imgur.com/sHmksvO.png");
                    break;
                case ParseEnum.ThrashIDS.Kernan:
                    CombatReplay.SetIcon("https://i.imgur.com/WABRQya.png");
                    break;
                case ParseEnum.ThrashIDS.Knuckles:
                    CombatReplay.SetIcon("https://i.imgur.com/m1y8nJE.png");
                    break;
                case ParseEnum.ThrashIDS.Karde:
                    CombatReplay.SetIcon("https://i.imgur.com/3UGyosm.png");
                    break;
                case ParseEnum.ThrashIDS.Olson:
                case ParseEnum.ThrashIDS.Engul:
                case ParseEnum.ThrashIDS.Faerla:
                case ParseEnum.ThrashIDS.Caulle:
                case ParseEnum.ThrashIDS.Henley:
                case ParseEnum.ThrashIDS.Jessica:
                case ParseEnum.ThrashIDS.Galletta:
                case ParseEnum.ThrashIDS.Ianim:
                    CombatReplay.SetIcon("https://i.imgur.com/qeYT1Bf.png");
                    break;
                case ParseEnum.ThrashIDS.Core:
                    CombatReplay.SetIcon("https://i.imgur.com/yI34iqw.png");
                    break;
                case ParseEnum.ThrashIDS.Jade:
                    CombatReplay.SetIcon("https://i.imgur.com/ivtzbSP.png");
                    break;
                case ParseEnum.ThrashIDS.Guldhem:
                    CombatReplay.SetIcon("https://i.imgur.com/xa7Fefn.png");
                    break;
                case ParseEnum.ThrashIDS.Rigom:
                    CombatReplay.SetIcon("https://i.imgur.com/REcGMBe.png");
                    break;
                case ParseEnum.ThrashIDS.Saul:
                    CombatReplay.SetIcon("https://i.imgur.com/ck2IsoS.png");
                    break;
                case ParseEnum.ThrashIDS.Messenger:
                case ParseEnum.ThrashIDS.TormentedDead:
                    CombatReplay.SetIcon("https://i.imgur.com/1J2BTFg.png");
                    break;
                case ParseEnum.ThrashIDS.Scythe:
                    CombatReplay.SetIcon("https://i.imgur.com/INCGLIK.png");
                    break;
                case ParseEnum.ThrashIDS.Enforcer:
                    CombatReplay.SetIcon("https://i.imgur.com/elHjamF.png");
                    break;
                case ParseEnum.ThrashIDS.Tornado:
                    CombatReplay.SetIcon("https://i.imgur.com/e10lZMa.png");
                    break;
                case ParseEnum.ThrashIDS.IcePatch:
                    CombatReplay.SetIcon("https://i.imgur.com/yxKJ5Yc.png");
                    break;
                case ParseEnum.ThrashIDS.Storm:
                    CombatReplay.SetIcon("https://i.imgur.com/9XtNPdw.png");
                    break;
                case ParseEnum.ThrashIDS.UnstableLeyRift:
                    CombatReplay.SetIcon("https://i.imgur.com/YXM3igs.png");
                    break;
                case ParseEnum.ThrashIDS.Tear:
                    CombatReplay.SetIcon("https://i.imgur.com/N9seps0.png");
                    break;
                case ParseEnum.ThrashIDS.Oil:
                    CombatReplay.SetIcon("https://i.imgur.com/DZIl49i.png");
                    break;
                case ParseEnum.ThrashIDS.InsidiousProjection:
                    CombatReplay.SetIcon("https://i.imgur.com/9EdItBS.png");
                    break;
                case ParseEnum.ThrashIDS.Pride:
                    CombatReplay.SetIcon("https://i.imgur.com/ePTXx23.png");
                    break;
                case ParseEnum.ThrashIDS.SurgingSoul:
                    CombatReplay.SetIcon("https://i.imgur.com/k79t7ZA.png");
                    break;
                case ParseEnum.ThrashIDS.Echo:
                    CombatReplay.SetIcon("https://i.imgur.com/kcN9ECn.png");
                    break;
                case ParseEnum.ThrashIDS.CrimsonPhantasm:
                    CombatReplay.SetIcon("https://i.imgur.com/zP7Bvb4.png");
                    break;
                case ParseEnum.ThrashIDS.RadiantPhantasm:
                    CombatReplay.SetIcon("https://i.imgur.com/O5VWLyY.png");
                    break;
                case ParseEnum.ThrashIDS.Gambler:
                case ParseEnum.ThrashIDS.Thief:
                case ParseEnum.ThrashIDS.Drunkard:
                    CombatReplay.SetIcon("https://i.imgur.com/vINeVU6.png");
                    break;
                case ParseEnum.ThrashIDS.GamblerClones:
                    CombatReplay.SetIcon("https://i.imgur.com/zMsBWEx.png");
                    break;
                case ParseEnum.ThrashIDS.GamblerReal:
                    CombatReplay.SetIcon("https://i.imgur.com/J6oMITN.png");
                    break;
                default:
                    CombatReplay.SetIcon("https://i.imgur.com/xCoypjS.png");
                    break;
            }
        }

        public void AddMechanics(ParsedLog log)
        {
            // nothing to do, thrash mob mechanics should be managed by the boss
        }
    }
}
