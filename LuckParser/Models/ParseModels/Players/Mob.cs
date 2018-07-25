using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels.Players
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

        protected override void setAdditionalCombatReplayData(ParsedLog log)
        {
            // nothing to do here, thrash mobs don't have additional data
        }

        protected override void setCombatReplayIcon(ParsedLog log)
        {
            // TODO
            switch (Boss.getThrashIDS(agent.getID()))
            {
                case Boss.ThrashIDS.Seekers:
                    break;
                case Boss.ThrashIDS.RedGuardian:
                    break;
                case Boss.ThrashIDS.BlueGuardian:
                    break;
                case Boss.ThrashIDS.GreenGuardian:
                    break;
                case Boss.ThrashIDS.ChargedSoul:
                    break;
                case Boss.ThrashIDS.Kernan:
                    break;
                case Boss.ThrashIDS.Knuckles:
                    break;
                case Boss.ThrashIDS.Karde:
                    break;
                case Boss.ThrashIDS.Spirit:
                    break;
                case Boss.ThrashIDS.BloodStone:
                    break;
                case Boss.ThrashIDS.Olson:
                    break;
                case Boss.ThrashIDS.Engul:
                    break;
                case Boss.ThrashIDS.Faerla:
                    break;
                case Boss.ThrashIDS.Caulle:
                    break;
                case Boss.ThrashIDS.Henley:
                    break;
                case Boss.ThrashIDS.Jessica:
                    break;
                case Boss.ThrashIDS.Galletta:
                    break;
                case Boss.ThrashIDS.Ianim:
                    break;
                case Boss.ThrashIDS.Core:
                    break;
                case Boss.ThrashIDS.Jade:
                    break;
                case Boss.ThrashIDS.Guldhem:
                    break;
                case Boss.ThrashIDS.Rigom:
                    break;
                case Boss.ThrashIDS.Saul:
                    break;
                case Boss.ThrashIDS.Thief:
                    break;
                case Boss.ThrashIDS.Gambler:
                    break;
                case Boss.ThrashIDS.GamblerClones:
                    break;
                case Boss.ThrashIDS.Drunkard:
                    break;
                case Boss.ThrashIDS.TormentedDead:
                    break;
                case Boss.ThrashIDS.SurgingSoul:
                    break;
                case Boss.ThrashIDS.Scythe:
                    break;
                case Boss.ThrashIDS.Messenger:
                    break;
                case Boss.ThrashIDS.Echo:
                    break;
                case Boss.ThrashIDS.Enforcer:
                    break;
            }
        }
    }
}
