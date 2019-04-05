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
	
	public class CatEnemySystem : BaseSystem
	{

		public override Type ComponentType => typeof(CatEnemyComponent);

		public override int Priority => 1;

		public override void Create(Component component)
		{
			
		}

		public override void Update(List<Component> components)
		{
			foreach(CatEnemyComponent gato in components)
			{
				var physics = gato.Owner.GetComponent<PhysicsComponent>();
				var actor = gato.Owner.GetComponent<StackableActorComponent>();

				actor.LeftAction = (gato.Direction == -1);
				actor.RightAction = (gato.Direction == 1); 
				
				if (actor.LogicStateMachine.CurrentState == ActorStates.OnGround)
				{
					var position = gato.Owner.GetComponent<PositionComponent>();
					
					// Checking if there is a wall next to us.
					physics.Collider.Position = position.Position
						+ Vector2.UnitX * gato.Direction * (physics.Collider.Size.X / 2 + 1);
					
					if (physics.CollisionH == gato.Direction)
					{
						gato.Direction *= -1;
						actor.Orientation = gato.Direction;
					}
					// Checking if there is a wall next to us.
					else
					{
						// Checking if there is a pit below up.
						var collider = new RectangleCollider();
						collider.Size = Vector2.One;
						collider.Position = position.Position 
							+ (physics.Collider.Size / 2 + Vector2.One) * new Vector2(gato.Direction, 1);

						if (PhysicsSystem.CheckCollision(gato.Owner, collider) == null)
						{
							gato.Direction *= -1;
							actor.Orientation = gato.Direction;
						}
						// Checking if there is a pit below up.
					}
				}

				
				// Damaging the player.
				if (
					actor.LogicStateMachine.CurrentState != ActorStates.Stacked 
					&& actor.LogicStateMachine.CurrentState != ActorStates.Dead
				)
				{
					var players = SceneMgr.CurrentScene.GetEntityListByComponent<PlayerComponent>();
					foreach(var playerEntity in players)
					{
						var player = playerEntity.GetComponent<PlayerComponent>();
						var playerActor = playerEntity.GetComponent<StackableActorComponent>();
						
						if (playerActor.Crouching)
						{
							continue;
						}
						
						var position = gato.Owner.GetComponent<PositionComponent>();
						var playerPosition = playerEntity.GetComponent<PositionComponent>();
						var playerPhysics = playerEntity.GetComponent<PhysicsComponent>();
					
					
						// Setting up colliders.
						physics.Collider.Position = position.Position;
						physics.Collider.PreviousPosition = position.PreviousPosition;
						playerPhysics.Collider.Position = playerPosition.Position;
						playerPhysics.Collider.PreviousPosition = playerPosition.PreviousPosition	;
						// Seting up colliders.

						if (CollisionDetector.CheckCollision(physics.Collider, playerPhysics.Collider))
						{
							StackableActorSystem.Damage(playerEntity.GetComponent<StackableActorComponent>());
						}
					}
				}
				// Damaging the player.

			}
		}
		
	}
}
