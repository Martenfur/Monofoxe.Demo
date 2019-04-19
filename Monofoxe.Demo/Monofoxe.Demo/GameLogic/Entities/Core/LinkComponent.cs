using Monofoxe.Engine.ECS;

namespace Monofoxe.Demo.GameLogic.Entities.Core
{
	public class LinkComponent : Component
	{
		
		public string Tag;

		public LinkComponent Pair;

		public bool Passive;

		public LinkComponent(string link, bool passive = false)
		{
			Tag = link;
			Passive = passive;
		}
	}
}
