using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;

namespace Monofoxe.Demo.GameLogic.Entities
{
	public class PlayerSystem : BaseSystem
	{
		public override Type ComponentType => typeof(PlayerComponent);

		public override int Priority => 1;

		
		public override void Update(List<Component> components)
		{
			foreach(PlayerComponent player in components)
			{
				var physics = player.Owner.GetComponent<PhysicsComponent>();
				var position = player.Owner.GetComponent<PositionComponent>();
			
				if (Input.CheckButton(player.Left))
				{
					physics.Speed.X = -player.WalkSpeed;
				}
				if (Input.CheckButton(player.Right))
				{
					physics.Speed.X = player.WalkSpeed;
				}

				if (!Input.CheckButton(player.Left) && !Input.CheckButton(player.Right))
				{
					physics.Speed.X = 0;
				}

				if (Input.CheckButtonPress(player.Jump))
				{
					physics.Speed.Y = -player.JumpSpeed;
				}

				Test.Camera.Position = player.Owner.GetComponent<PositionComponent>().Position.ToPoint().ToVector2() 
				- Test.Camera.Size / 2;
			
			}
			
		}

		public override void Draw(Component component)
		{
			var physics = component.Owner.GetComponent<PhysicsComponent>();
			var position = component.Owner.GetComponent<PositionComponent>();

			if (physics.InAir)
				DrawMgr.CurrentColor = Color.Azure;
			else
				DrawMgr.CurrentColor = physics.Color;
			

			DrawMgr.DrawRectangle(
				position.Position.ToPoint().ToVector2() - physics.Collider.Size / 2,
				position.Position.ToPoint().ToVector2() + physics.Collider.Size / 2,
				true
			);
			/*
			DrawMgr.DrawCircle(
				position.PreviousPosition.ToPoint().ToVector2(),
				8,
				true
			);*/
			//DrawMgr.CurrentColor = Color.Black;
		}
	}
}
