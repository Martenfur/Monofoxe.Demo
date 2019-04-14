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
			Console.WriteLine(GameMgr.ElapsedTimeTotal);
			foreach(FrogEnemyComponent frog in components)
			{
				var physics = frog.Owner.GetComponent<PhysicsComponent>();
				var actor = frog.Owner.GetComponent<StackableActorComponent>();

				actor.LeftAction = (frog.Direction == -1);
				actor.RightAction = (frog.Direction == 1);
				
				//actor.JumpAction = (actor.LogicStateMachine.CurrentState == ActorStates.OnGround);
				
				if (actor.LogicStateMachine.CurrentState == ActorStates.OnGround)
				{
					var position = frog.Owner.GetComponent<PositionComponent>();
					
					// Checking if there is a wall next to us.
					physics.Collider.Position = position.Position 
						+ Vector2.UnitX * frog.Direction * physics.Collider.Size.X / 2;
					
					var players = SceneMgr.CurrentScene.GetEntityList("player");

					if (players.Count > 0)
					{
						var player = players[0];
						var playerPosition = player.GetComponent<PositionComponent>();
						var playerActor = player.GetComponent<StackableActorComponent>();

						actor.JumpAction = playerActor.JumpAction;

						frog.Direction = Math.Sign(playerPosition.Position.X - position.Position.X);
					}

				}
			}
		}
		
	}
}
