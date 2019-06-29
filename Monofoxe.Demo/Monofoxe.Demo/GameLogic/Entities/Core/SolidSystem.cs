using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using System;
using System.Collections.Generic;

namespace Monofoxe.Demo.GameLogic.Entities.Core
{
	public class SolidSystem : BaseSystem
	{

		public override Type ComponentType => typeof(SolidComponent);

		public override void Update(List<Component> components)
		{
			
		}

		public override void Draw(Component component)
		{
			var solid = (SolidComponent)component;
			var position = solid.Owner.GetComponent<PositionComponent>();
			
			GraphicsMgr.CurrentColor = Color.Red;

			RectangleShape.Draw(
				position.Position.RoundV() - solid.Collider.Size / 2,
				position.Position.RoundV() + solid.Collider.Size / 2,
				true
			);
			

		}


	}
}
