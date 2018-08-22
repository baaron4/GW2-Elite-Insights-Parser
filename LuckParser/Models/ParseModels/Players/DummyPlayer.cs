using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    // A dummy class that only serve as "stocking" purposes, trying to do anything with it will throw an exception
    public class DummyPlayer : AbstractMasterPlayer
    {

        public DummyPlayer(AgentItem agent) : base(agent)
        {

        }

        public override void addMechanics(ParsedLog log)
        {
            throw new NotImplementedException();
        }

        protected override void setAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            throw new NotImplementedException();
        }

        protected override void setCastLogs(ParsedLog log)
        {
            throw new NotImplementedException();
        }

        protected override void setCombatReplayIcon(ParsedLog log)
        {
            throw new NotImplementedException();
        }

        protected override void setDamageLogs(ParsedLog log)
        {
            throw new NotImplementedException();
        }

        protected override void setDamagetakenLogs(ParsedLog log)
        {
            throw new NotImplementedException();
        }
    }
}
