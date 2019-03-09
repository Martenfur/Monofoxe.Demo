using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Monofoxe.Demo.GameLogic.Collisions;


namespace Monofoxe.Demo.GameLogic.Entities
{
	public class FrogEnemySystem : BaseSystem
	{

		public override Type ComponentType => typeof(FrogEnemyComponent);

		public override int Priority => 1;

		public override void Create(Component component)
		{
			
		}

		public override void Update(List<Component> components)
		{
			foreach(FrogEnemyComponent gato in components)
			{
				var physics = gato.Owner.GetComponent<PhysicsComponent>();
				var actor = gato.Owner.GetComponent<StackableActorComponent>();

				actor.LeftAction = (gato.Direction == -1);
				actor.RightAction = (gato.Direction == 1);
				
				actor.JumpAction = (actor.LogicStateMachine.CurrentState == ActorStates.OnGround);
				
				if (actor.LogicStateMachine.CurrentState == ActorStates.OnGround)
				{
					var position = gato.Owner.GetComponent<PositionComponent>();
					
					// Checking if there is a wall next to us.
					physics.Collider.Position = position.Position 
						+ Vector2.UnitX * gato.Direction * physics.Collider.Size.X / 2;
					
					var player = SceneMgr.CurrentLayer.GetEntityList("player")[0];
					var playerPosition = player.GetComponent<PositionComponent>();

					gato.Direction = Math.Sign(playerPosition.Position.X - position.Position.X);

				}
			}
		}
		
	}
}
