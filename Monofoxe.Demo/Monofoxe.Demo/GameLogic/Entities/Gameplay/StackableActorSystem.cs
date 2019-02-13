using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class StackableActorSystem : BaseSystem
	{
		public override Type ComponentType => typeof(StackableActorComponent);

		public override int Priority => 1;

		
		public override void Update(List<Component> components)
		{
			foreach(StackableActorComponent actor in components)
			{
				var physics = actor.Owner.GetComponent<PhysicsComponent>();
				var position = actor.Owner.GetComponent<PositionComponent>();
			
				if (actor.LeftAction)
				{
					physics.Speed.X = -actor.WalkSpeed;
				}
				if (actor.RightAction)
				{
					physics.Speed.X = actor.WalkSpeed;
				}

				if (!actor.LeftAction && !actor.RightAction)
				{
					physics.Speed.X = 0;
				}

				if (actor.JumpAction)
				{
					physics.Speed.Y = -actor.JumpSpeed;
				}

			}
			
		}

		public override void Draw(Component component)
		{
			var physics = component.Owner.GetComponent<PhysicsComponent>();
			var position = component.Owner.GetComponent<PositionComponent>();

			if (physics.InAir)
				DrawMgr.CurrentColor = Color.Azure;
			else
				DrawMgr.CurrentColor = Color.Black;
			

			DrawMgr.DrawRectangle(
				position.Position.ToPoint().ToVector2() - physics.Collider.Size / 2,
				position.Position.ToPoint().ToVector2() + physics.Collider.Size / 2,
				true
			);
		}
	}
}
