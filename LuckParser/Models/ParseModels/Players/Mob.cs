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
            switch (agent.getID())
            {
                case (ushort)Boss.ThrashIDS.Seekers:
                    break;
                case (ushort)Boss.ThrashIDS.RedGuardian:
                    break;
                case (ushort)Boss.ThrashIDS.BlueGuardian:
                    break;
                case (ushort)Boss.ThrashIDS.GreenGuardian:
                    break;
                case (ushort)Boss.ThrashIDS.ChargedSoul:
                    break;
                case (ushort)Boss.ThrashIDS.Kernan:
                    break;
                case (ushort)Boss.ThrashIDS.Knuckles:
                    break;
                case (ushort)Boss.ThrashIDS.Karde:
                    break;
                case (ushort)Boss.ThrashIDS.Spirit:
                    break;
                case (ushort)Boss.ThrashIDS.BloodStone:
                    break;
                case (ushort)Boss.ThrashIDS.Olson:
                    break;
                case (ushort)Boss.ThrashIDS.Engul:
                    break;
                case (ushort)Boss.ThrashIDS.Faerla:
                    break;
                case (ushort)Boss.ThrashIDS.Caulle:
                    break;
                case (ushort)Boss.ThrashIDS.Henley:
                    break;
                case (ushort)Boss.ThrashIDS.Jessica:
                    break;
                case (ushort)Boss.ThrashIDS.Galletta:
                    break;
                case (ushort)Boss.ThrashIDS.Ianim:
                    break;
                case (ushort)Boss.ThrashIDS.Core:
                    break;
                case (ushort)Boss.ThrashIDS.Jade:
                    break;
                case (ushort)Boss.ThrashIDS.Guldhem:
                    break;
                case (ushort)Boss.ThrashIDS.Rigom:
                    break;
                case (ushort)Boss.ThrashIDS.Saul:
                    break;
                case (ushort)Boss.ThrashIDS.Thief:
                    break;
                case (ushort)Boss.ThrashIDS.Gambler:
                    break;
                case (ushort)Boss.ThrashIDS.GamblerClones:
                    break;
                case (ushort)Boss.ThrashIDS.Drunkard:
                    break;
                case (ushort)Boss.ThrashIDS.TormentedDead:
                    break;
                case (ushort)Boss.ThrashIDS.SurgingSoul:
                    break;
                case (ushort)Boss.ThrashIDS.Scythe:
                    break;
                case (ushort)Boss.ThrashIDS.Messenger:
                    break;
                case (ushort)Boss.ThrashIDS.Echo:
                    break;
                case (ushort)Boss.ThrashIDS.Enforcer:
                    break;
            }
        }
    }
}
