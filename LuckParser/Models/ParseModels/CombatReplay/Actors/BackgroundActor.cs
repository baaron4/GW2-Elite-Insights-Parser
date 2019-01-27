using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
	public abstract class BackgroundActor : GenericActor
	{
		public BackgroundActor((int start, int end) lifespan) : base(lifespan, null)
		{
		}

		protected class BackgroundSerializable : GenericActorSerializable
		{
		}

		public abstract override GenericActorSerializable GetCombatReplayJSON(CombatReplayMap map);
	}
}