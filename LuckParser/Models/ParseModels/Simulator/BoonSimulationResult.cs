using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
	public class BoonSimulationResult
	{
		private readonly BoonSimulationItem[] boonSimulationItems;

		public IEnumerable<BoonSimulationItem> Items => boonSimulationItems;

		public BoonSimulationResult(IEnumerable<BoonSimulationItem> boonSimulationItems)
		{
			this.boonSimulationItems = boonSimulationItems.ToArray();
		}

		public int GetBoonStackCount(int time)
		{
			foreach (var item in boonSimulationItems)
			{
				int start = (int) item.Start;
				int end = (int) item.End;
				if (time >= start && time < end)
				{
					return item.GetStack(time);
				}
			}

			return 0;
		}

		public bool GetEffectPresence(int time)
		{
			foreach (var item in boonSimulationItems)
			{
				int start = (int) item.Start;
				int end = (int) item.End;
				if (time >= start && time < end)
				{
					bool present = item.Duration > 0;
					return present;
				}
			}

			return false;
		}
	}
}