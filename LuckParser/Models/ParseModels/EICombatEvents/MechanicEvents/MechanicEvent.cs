using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class MechanicEvent : AbstractCombatEvent
    {
        private readonly Mechanic _mechanic;
        public DummyActor Actor { get; }
        public long Skill => _mechanic.SkillId;
        public string ShortName => _mechanic.ShortName;
        public string Description => _mechanic.Description;
        public string InGameName => _mechanic.InGameName;
        public bool Enemy => _mechanic.IsEnemyMechanic;

        public MechanicEvent(long time, Mechanic mech, DummyActor actor) : base(time, 0)
        {
            Actor = actor;
            _mechanic = mech;
        }
    }
}
