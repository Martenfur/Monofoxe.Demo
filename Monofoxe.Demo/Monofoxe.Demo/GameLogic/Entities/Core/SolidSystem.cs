﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo.GameLogic.Entities.Core
{
	public class SolidSystem : BaseSystem
	{

		public override Type ComponentType => typeof(SolidComponent);

		public override void Update(List<Component> components)
		{
			
			foreach(SolidComponent solid in components)
			{
				/*solid.Speed = Vector2.Zero;

				if (solid.Collider is TilemapCollider)
					continue;

				if (Input.CheckButton(Buttons.Up))
					solid.Speed.Y = -100;
				if (Input.CheckButton(Buttons.Down))
					solid.Speed.Y = 100;
				if (Input.CheckButton(Buttons.Left))
					solid.Speed.X = -100;
				if (Input.CheckButton(Buttons.Right))
					solid.Speed.X = 100;*/
			}

		}

		public override void Draw(Component component)
		{
			var solid = (SolidComponent)component;
			var position = solid.Owner.GetComponent<PositionComponent>();
			
			GraphicsMgr.CurrentColor = Color.Red;

			RectangleShape.Draw(
				position.Position.Round() - solid.Collider.Size / 2,
				position.Position.Round() + solid.Collider.Size / 2,
				true
			);
			

		}


	}
}
