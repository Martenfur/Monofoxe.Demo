using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.Utils;
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
			
				float accelerationTime, decelerationTime;

				if (physics.InAir)
				{
					accelerationTime = actor.AirAccelTime;
					decelerationTime = actor.AirDecelTime;
				}
				else
				{
					accelerationTime = actor.GroundAccelTime;
					decelerationTime = actor.GroundDecelTime;
				}

				int horMovement = 0;
				if (actor.LeftAction)
				{
					horMovement += -1;
				}
				if (actor.RightAction)
				{
					horMovement += 1;
				}

				if (horMovement != 0)
				{
					if (Math.Abs(physics.Speed.X) < actor.WalkSpeed || Math.Sign(physics.Speed.X) != Math.Sign(horMovement))
					{
						physics.Speed.X += TimeKeeper.GlobalTime(horMovement * actor.WalkSpeed / accelerationTime);
						if (Math.Abs(physics.Speed.X) > actor.WalkSpeed)
						{
							physics.Speed.X = horMovement * actor.WalkSpeed;
						}
					}
				}
				else
				{
					if (physics.Speed.X != 0)
					{
						var spdSign = Math.Sign(physics.Speed.X);
						physics.Speed.X -= TimeKeeper.GlobalTime(spdSign * actor.WalkSpeed / decelerationTime);
						if (Math.Sign(physics.Speed.X) != Math.Sign(spdSign))
						{
							physics.Speed.X = 0;
						}
					}
				}

				if (actor.JumpAction && !physics.InAir)
				{
					physics.Speed.Y = -actor.JumpSpeed;
				}

				if (actor.JumpAction && physics.Speed.Y < 0)
				{
					physics.Gravity = actor.JumpGravity;
				}
				else
				{
					physics.Gravity = actor.FallGravity;
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
