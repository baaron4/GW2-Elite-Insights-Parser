using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{

    internal abstract class ActorDto
    {
        public int UniqueID { get;  set; }
        public string Name { get;  set; }
        public uint Tough { get;  set; }
        public List<MinionDto> Minions { get; } = new List<MinionDto>();
        public ActorDetailsDto Details { get;  set; }

        protected ActorDto(AbstractSingleActor actor, ParsedEvtcLog log, ActorDetailsDto details)
        {
            Name = actor.Character;
            Tough = actor.Toughness;
            Details = details;
            UniqueID = actor.UniqueID;
            foreach (KeyValuePair<long, Minions> pair in actor.GetMinions(log))
            {
                Minions.Add(new MinionDto()
                {
                    Id = pair.Key,
                    Name = pair.Value.Character
                });
            }
        }
    }
}
