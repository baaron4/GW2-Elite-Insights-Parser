using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
	public class GenerationSimulationResult
	{
		private readonly BoonSimulationItem[] _generationSimulationItems;

		public IEnumerable<BoonSimulationItem> Items => _generationSimulationItems;

		public GenerationSimulationResult(IEnumerable<BoonSimulationItem> generationSimulationItems)
		{
			_generationSimulationItems = generationSimulationItems.ToArray();
		}

		public int GetStackCount(int time)
		{
			foreach (var item in _generationSimulationItems)
			{
				int start = (int) item.Start;
				int end = (int) item.End;
				if (time >= start && time <= end)
				{
					return item.GetStack();
				}
			}

			return 0;
		}

		public bool GetEffectPresence(int time)
		{
			foreach (var item in _generationSimulationItems)
			{
				int start = (int) item.Start;
				int end = (int) item.End;
				if (time >= start && time <= end)
				{
					return item.Duration > 0;
				}
			}

			return false;
		}
	}
}