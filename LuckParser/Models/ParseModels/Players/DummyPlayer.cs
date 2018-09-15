using System;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    // A dummy class that only serve as "stocking" purposes, trying to do anything with it will throw an exception
    public class DummyPlayer : AbstractMasterPlayer
    {

        public DummyPlayer(AgentItem agent) : base(agent)
        {

        }

        public void AddMechanics(ParsedLog log)
        {
            throw new InvalidOperationException();
        }

        public override int GetCombatReplayID()
        {
            throw new InvalidOperationException();
        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            throw new InvalidOperationException();
        }

        protected override void SetCastLogs(ParsedLog log)
        {
            throw new InvalidOperationException();
        }

        protected override void SetCombatReplayIcon(ParsedLog log)
        {
            throw new InvalidOperationException();
        }

        protected override void SetDamageLogs(ParsedLog log)
        {
            throw new InvalidOperationException();
        }

        protected override void SetDamageTakenLogs(ParsedLog log)
        {
            throw new InvalidOperationException();
        }
    }
}
