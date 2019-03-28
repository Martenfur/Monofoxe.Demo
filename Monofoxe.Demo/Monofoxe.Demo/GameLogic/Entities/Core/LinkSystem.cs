using System;
using System.Collections.Generic;
using Monofoxe.Engine.ECS;

namespace Monofoxe.Demo.GameLogic.Entities.Core
{
	public class LinkSystem : BaseSystem
	{

		public override Type ComponentType => typeof(LinkComponent);

		public override void Update(List<Component> components)
		{
			foreach(LinkComponent link in components)
			{
				if (link.Passive)
				{
					continue;
				}

				foreach(LinkComponent otherLink in components)
				{
					if (otherLink.Passive && link != otherLink && link.Tag == otherLink.Tag)
					{
						link.Pair = otherLink;
						otherLink.Pair = link;
						break;
					}
				}
			}

		}
		


	}
}
