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
        private String mobName;

        // Constructors
        public Mob(AgentItem agent) : base(agent)
        {
            String[] name = agent.getName().Split('\0');
            mobName = name[1];
           
        }

        //setters
        protected override void setDamagetakenLogs(ParsedLog log)
        {
        }
        //getters
        public string getMobName()
        {
            return mobName;
        }

        protected override void setAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            // todo
            int start = (int)replay.getTimeOffsets().Item1;
            int end = (int)replay.getTimeOffsets().Item2;
            Tuple<int, int> lifespan = new Tuple<int, int>(start, end);
            switch (Boss.getThrashIDS(agent.getID()))
            {
                case Boss.ThrashIDS.Seekers:
                    replay.addCircleActor(new FollowingCircle(false, false, 180, lifespan, "rgba(255, 0, 0, 0.3)"));
                    break;
                case Boss.ThrashIDS.Spirit:
                case Boss.ThrashIDS.Spirit2:
                    replay.addCircleActor(new FollowingCircle(true, false, 120, lifespan, "rgba(255, 0, 0, 0.3)"));
                    break;
                case Boss.ThrashIDS.Olson:
                case Boss.ThrashIDS.Engul:
                case Boss.ThrashIDS.Faerla:
                case Boss.ThrashIDS.Caulle:
                case Boss.ThrashIDS.Henley:
                case Boss.ThrashIDS.Jessica:
                case Boss.ThrashIDS.Galletta:
                case Boss.ThrashIDS.Ianim:
                    replay.addCircleActor(new FollowingCircle(false, false, 450, lifespan, "rgba(255, 0, 0, 0.3)"));
                    replay.addCircleActor(new FollowingCircle(true, false, 240, lifespan, "rgba(0, 125, 255, 0.3)"));
                    break;
                case Boss.ThrashIDS.Messenger:
                    replay.addCircleActor(new FollowingCircle(true, false, 180, lifespan, "rgba(255, 125, 0, 0.3)"));
                    break;
                case Boss.ThrashIDS.Scythe:
                    replay.addCircleActor(new FollowingCircle(true, false, 80, lifespan, "rgba(255, 0, 0, 0.3)"));
                    break;
                case Boss.ThrashIDS.Tornado:
                    replay.addCircleActor(new FollowingCircle(true, false, 80, lifespan, "rgba(255, 0, 0, 0.3)"));
                    break;
                case Boss.ThrashIDS.IcePatch:
                    replay.addCircleActor(new FollowingCircle(true, false, 180, lifespan, "rgba(0, 0, 255, 0.3)"));
                    break;
                case Boss.ThrashIDS.Storm:
                    replay.addCircleActor(new FollowingCircle(false, false, 240, lifespan, "rgba(0, 80, 255, 0.3)"));
                    break;
                case Boss.ThrashIDS.Oil:
                    replay.addCircleActor(new FollowingCircle(true, false, 240, lifespan, "rgba(0, 0, 0, 0.3)"));
                    break;
                case Boss.ThrashIDS.TormentedDead:
                    replay.addCircleActor(new ImmobileCircle(true,false,300,new Tuple<int, int>(end,end+60000), "rgba(255, 0, 0, 0.3)",replay.getPositions().Last()));
                    break;
            }
        }

        protected override void setCombatReplayIcon(ParsedLog log)
        {

            switch (Boss.getThrashIDS(agent.getID()))
            {
                case Boss.ThrashIDS.Seekers:
                    replay.setIcon("https://i.imgur.com/FrPoluz.png");
                    break;
                case Boss.ThrashIDS.RedGuardian:
                    replay.setIcon("https://i.imgur.com/73Uj4lG.png");
                    break;
                case Boss.ThrashIDS.BlueGuardian:
                    replay.setIcon("https://i.imgur.com/6CefnkP.png");
                    break;
                case Boss.ThrashIDS.GreenGuardian:
                    replay.setIcon("https://i.imgur.com/nauDVYP.png");
                    break;
                case Boss.ThrashIDS.Spirit:
                case Boss.ThrashIDS.Spirit2:
                case Boss.ThrashIDS.ChargedSoul:
                    replay.setIcon("https://i.imgur.com/sHmksvO.png");
                    break;
                case Boss.ThrashIDS.Kernan:
                    replay.setIcon("https://i.imgur.com/WABRQya.png");
                    break;
                case Boss.ThrashIDS.Knuckles:
                    replay.setIcon("https://i.imgur.com/m1y8nJE.png");
                    break;
                case Boss.ThrashIDS.Karde:
                    replay.setIcon("https://i.imgur.com/3UGyosm.png");
                    break;
                case Boss.ThrashIDS.Olson:
                case Boss.ThrashIDS.Engul:
                case Boss.ThrashIDS.Faerla:
                case Boss.ThrashIDS.Caulle:
                case Boss.ThrashIDS.Henley:
                case Boss.ThrashIDS.Jessica:
                case Boss.ThrashIDS.Galletta:
                case Boss.ThrashIDS.Ianim:
                    replay.setIcon("https://i.imgur.com/qeYT1Bf.png");
                    break;
                case Boss.ThrashIDS.Core:
                    replay.setIcon("https://i.imgur.com/yI34iqw.png");
                    break;
                case Boss.ThrashIDS.Jade:
                    replay.setIcon("https://i.imgur.com/ivtzbSP.png");
                    break;
                case Boss.ThrashIDS.Guldhem:
                    replay.setIcon("https://i.imgur.com/xa7Fefn.png");
                    break;
                case Boss.ThrashIDS.Rigom:
                    replay.setIcon("https://i.imgur.com/REcGMBe.png");
                    break;
                case Boss.ThrashIDS.Saul:
                    replay.setIcon("https://i.imgur.com/ck2IsoS.png");
                    break;
                case Boss.ThrashIDS.Messenger:
                case Boss.ThrashIDS.TormentedDead:
                    replay.setIcon("https://i.imgur.com/1J2BTFg.png");
                    break;
                case Boss.ThrashIDS.Scythe:
                    replay.setIcon("https://i.imgur.com/INCGLIK.png");
                    break;
                case Boss.ThrashIDS.Enforcer:
                    replay.setIcon("https://i.imgur.com/elHjamF.png");
                    break;
                case Boss.ThrashIDS.Tornado:
                    replay.setIcon("https://i.imgur.com/e10lZMa.png");
                    break;
                case Boss.ThrashIDS.IcePatch:
                    replay.setIcon("https://i.imgur.com/yxKJ5Yc.png");
                    break;
                case Boss.ThrashIDS.Storm:
                    replay.setIcon("https://i.imgur.com/9XtNPdw.png");
                    break;
                case Boss.ThrashIDS.Tear:
                    replay.setIcon("https://i.imgur.com/N9seps0.png");
                    break;
                case Boss.ThrashIDS.Oil:
                    replay.setIcon("https://i.imgur.com/DZIl49i.png");
                    break;
                default:
                    replay.setIcon("https://i.imgur.com/xCoypjS.png");
                    break;
            }
        }
    }
}
