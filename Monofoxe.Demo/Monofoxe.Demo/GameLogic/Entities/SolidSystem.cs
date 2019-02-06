﻿using System;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;
using Monofoxe.Demo.GameLogic.Collisions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Monofoxe.Demo.GameLogic.Entities
{
	public class SolidSystem : BaseSystem
	{

		public override Type ComponentType => typeof(SolidComponent);

		public override void Update(List<Component> components)
		{
			foreach(SolidComponent solid in components)
			{
				solid.Speed = Vector2.Zero;

				if (Input.CheckButton(Buttons.Up))
					solid.Speed.Y = -100;
				if (Input.CheckButton(Buttons.Down))
					solid.Speed.Y = 100;
				if (Input.CheckButton(Buttons.Left))
					solid.Speed.X = -100;
				if (Input.CheckButton(Buttons.Right))
					solid.Speed.X = 100;
			}

		}

		public override void Draw(Component component)
		{
			var solid = (SolidComponent)component;
			var position = solid.Owner.GetComponent<PositionComponent>();

			if (solid.Collider is PlatformCollider)
				DrawMgr.CurrentColor = Color.Black * 0.5f;
			else
				DrawMgr.CurrentColor = Color.Black;

			DrawMgr.DrawRectangle(
				position.Position - solid.Collider.Size / 2,
				position.Position + solid.Collider.Size / 2,
				false
			);
			

		}


	}
}
