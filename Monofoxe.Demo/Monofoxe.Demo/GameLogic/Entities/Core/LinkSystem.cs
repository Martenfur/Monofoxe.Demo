using System;
using System.Collections.Generic;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine;
using Monofoxe.Engine.SceneSystem;

namespace Monofoxe.Demo.GameLogic.Entities.Core
{
	public class LinkSystem : BaseSystem
	{

		public override Type ComponentType => typeof(LinkComponent);

		public override void Update(List<Component> components)
		{
			var allLinks = SceneMgr.CurrentScene.GetComponentList<LinkComponent>();
			foreach(LinkComponent link in components)
			{
				if (link.Passive)
				{
					continue;
				}

				foreach(LinkComponent otherLink in allLinks)
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
