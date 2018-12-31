using System;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    // A dummy class that only serve as "stocking" purposes, trying to do anything with it will throw an exception
    public class DummyActor : AbstractMasterActor
    {

        public DummyActor(AgentItem agent) : base(agent)
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

        public override string GetCombatReplayJSON(CombatReplayMap map)
        {
            throw new InvalidOperationException();
        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log)
        {
            throw new InvalidOperationException();
        }

        protected override void SetCastLogs(ParsedLog log)
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
