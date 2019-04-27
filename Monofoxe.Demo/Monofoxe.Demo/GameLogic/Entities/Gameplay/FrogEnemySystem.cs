using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Monofoxe.Demo.GameLogic.Collisions;


namespace Monofoxe.Demo.GameLogic.Entities
{

	public enum FrogEnemyStates
	{
		// Wandering around, looking for player.
		Idle,
		
		// Seeing player and trying to attack him.
		Follow,

		// Actually attacking.
		Attack,
	}
	
	public class FrogEnemySystem : BaseSystem
	{

		public override Type ComponentType => typeof(FrogEnemyComponent);

		public override int Priority => 1;
		
		public override void Create(Component component)
		{
			var frog = (FrogEnemyComponent)component;
			var actor = frog.Owner.GetComponent<StackableActorComponent>();
			
			frog.LogicStateMachine = new StateMachine<FrogEnemyStates>(FrogEnemyStates.Idle, frog.Owner);
			frog.LogicStateMachine.AddState(FrogEnemyStates.Idle, Idle, IdleEnter);
			frog.LogicStateMachine.AddState(FrogEnemyStates.Follow, Follow, FollowEnter);
			frog.LogicStateMachine.AddState(FrogEnemyStates.Attack, Attack, AttackEnter, AttackExit);

			frog.AttackDelay = new Alarm();
		}

		public override void Update(List<Component> components)
		{
			foreach(FrogEnemyComponent frog in components)
			{
				frog.LogicStateMachine.Update();
				frog.AttackDelay.Update();
			}
		}

		#region Idle.
		
		void IdleEnter(StateMachine<FrogEnemyStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			
			actor.LeftAction = false;
			actor.RightAction = false;
			actor.JumpAction = false;
			actor.CrouchAction = false;
		}
		
		void Idle(StateMachine<FrogEnemyStates> stateMachine, Entity owner)
		{
			var frog = owner.GetComponent<FrogEnemyComponent>();

			if (frog.Trigger == null)
			{
				FindTrigger(frog);
			}

			if (frog.Trigger != null && frog.Trigger.HasPlayer)
			{
				stateMachine.ChangeState(FrogEnemyStates.Follow);
			}
		}

		private void FindTrigger(FrogEnemyComponent frog)
		{
			var frogPosition = frog.Owner.GetComponent<PositionComponent>();
				
			foreach(FrogTrigger trigger in SceneMgr.CurrentScene.GetEntityList<FrogTrigger>())
			{
				var triggerPosition = trigger.GetComponent<PositionComponent>();
				
				if (GameMath.PointInRectangleBySize(frogPosition.Position, triggerPosition.Position, trigger.Size))
				{
					frog.Trigger = trigger;
					return;
				}
			}
		}

		#endregion Idle.



		#region Follow.

		void FollowEnter(StateMachine<FrogEnemyStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();

			actor.LeftAction = false;
			actor.RightAction = false;
			actor.JumpAction = false;
			actor.CrouchAction = false;
		}

		void Follow(StateMachine<FrogEnemyStates> stateMachine, Entity owner)
		{
			var frog = owner.GetComponent<FrogEnemyComponent>();

			// If player is not in range - go idle.
			if (!frog.Trigger.HasPlayer)
			{
				stateMachine.ChangeState(FrogEnemyStates.Idle);
				return;
			}

			var physics = frog.Owner.GetComponent<PhysicsComponent>();
			var actor = frog.Owner.GetComponent<StackableActorComponent>();

			// Going in payer's general direction.
			// This code has no pathfinding or anything like that, so put 
			// froggos into open spaces with no obstacles. 
			actor.LeftAction = (frog.Direction == -1);
			actor.RightAction = (frog.Direction == 1);


			var position = frog.Owner.GetComponent<PositionComponent>();

			var players = SceneMgr.CurrentScene.GetEntityList("player");

			if (players.Count > 0)
			{
				var player = players[0];
				var playerPosition = player.GetComponent<PositionComponent>();
				var playerActor = player.GetComponent<StackableActorComponent>();

				// Jumping if player jumps.
				actor.JumpAction = playerActor.JumpAction && actor.LogicStateMachine.CurrentState == ActorStates.OnGround;
				
				if (
					actor.LogicStateMachine.CurrentState == ActorStates.OnGround
					&& !frog.AttackDelay.Running
				)
				{
					frog.Direction = Math.Sign(playerPosition.Position.X - position.Position.X);

					if (Math.Abs(playerPosition.Position.X - position.Position.X) < frog.CrouchRange)
					{
						if (actor.StackedNext != null)
						{
							// If player is already stacked on froggo's head - 
							// finish him off by jumping and killing him with the ceiling.
							actor.JumpAction = true; 
						}
						else
						{
							// If player is too close - initiate the attack.
							frog.AttackStartX = position.Position.X;
							stateMachine.ChangeState(FrogEnemyStates.Attack);
						}
						return;
					}
				}
			}

		}

		#endregion Follow.


		void AttackEnter(StateMachine<FrogEnemyStates> stateMachine, Entity owner)
		{
			var frog = owner.GetComponent<FrogEnemyComponent>();
			var actor = frog.Owner.GetComponent<StackableActorComponent>();
			var physics = frog.Owner.GetComponent<PhysicsComponent>();

			actor.JumpAction = false;
			
			// We need to speed the froggo up a bit before the crouch.
			physics.Speed.X = frog.CrouchAttackMovementSpeed * Math.Sign(physics.Speed.X) / 2;
		}
		
		void Attack(StateMachine<FrogEnemyStates> stateMachine, Entity owner)
		{
			var frog = owner.GetComponent<FrogEnemyComponent>();
			var actor = frog.Owner.GetComponent<StackableActorComponent>();
			var position = frog.Owner.GetComponent<PositionComponent>();
			var physics = frog.Owner.GetComponent<PhysicsComponent>();
			
			if (actor.LogicStateMachine.CurrentState == ActorStates.InAir)
			{
				stateMachine.ChangeState(FrogEnemyStates.Idle);
				return;
			}

			var players = SceneMgr.CurrentScene.GetEntityList("player");

			if (players.Count > 0)
			{
				var player = players[0];
				var playerPosition = player.GetComponent<PositionComponent>();
				var playerActor = player.GetComponent<StackableActorComponent>();
				
				var distance = Math.Abs(playerPosition.Position.X - position.Position.X);

				
				if (Math.Abs(frog.AttackStartX - position.Position.X) > frog.CrouchMargin)
				{	
					actor.CrouchAction = true;
					actor.LeftAction = false;
					actor.RightAction = false;
					actor.JumpAction = false;
				}

				if (
					(
						distance < frog.UncrouchRange 
						&& playerActor.LogicStateMachine.CurrentState == ActorStates.OnGround
					)
					||
					(
						Math.Abs(physics.Speed.X) < frog.MinSlideSpeed
					)
					||
					(
						distance > frog.CrouchRange + frog.CrouchMargin
						&& !playerActor.CrouchAction
					)
				)
				{
					stateMachine.ChangeState(FrogEnemyStates.Idle);
				}
			}

		}

		void AttackExit(StateMachine<FrogEnemyStates> stateMachine, Entity owner)
		{
			var frog = owner.GetComponent<FrogEnemyComponent>();
			var actor = frog.Owner.GetComponent<StackableActorComponent>();
			
			actor.CrouchAction = false;

			frog.AttackDelay.Set(frog.AttackDelayTime);
		}
		



	}
}
