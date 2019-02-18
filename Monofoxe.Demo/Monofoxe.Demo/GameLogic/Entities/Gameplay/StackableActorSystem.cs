using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.ECS;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Engine.SceneSystem;


namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public enum ActorStates
	{
		OnGround,
		Running,
		InAir,
		Stacked,
	}

	public class StackableActorSystem : BaseSystem
	{
		

		public override Type ComponentType => typeof(StackableActorComponent);

		public override int Priority => 1;


		public override void Create(Component component)
		{
			var actor = (StackableActorComponent)component;
			actor.StateMachine = new StateMachine<ActorStates>(ActorStates.OnGround, actor.Owner);
			actor.StateMachine.AddState(ActorStates.OnGround, OnGround, OnGroundEnter);
			actor.StateMachine.AddState(ActorStates.InAir, InAir, InAirEnter);
			actor.StateMachine.AddState(ActorStates.Stacked, Stacked, StackedEnter, StackedExit);

			actor.JumpBufferAlarm = new Alarm();
			actor.LandingBufferAlarm = new Alarm();

		}

		public override void Update(List<Component> components)
		{
			foreach(StackableActorComponent actor in components)
			{
				actor.StateMachine.Update();
				
				// Maybe all this could be packed into class.
				actor.JumpActionPress = (actor.JumpAction && !actor.JumpActionPrevious);
				actor.JumpActionPrevious = actor.JumpAction; 
				
			}
		}


		void OnGroundEnter(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			actor.CanJump = true;

			if (actor.LandingBufferAlarm.Running)
			{
				Jump(owner.GetComponent<PhysicsComponent>(), actor);
				stateMachine.ChangeState(ActorStates.InAir);
			}
		}
		
		
		void OnGround(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			var physics = owner.GetComponent<PhysicsComponent>();
			var position = owner.GetComponent<PositionComponent>();

			
			// Jumping.
			if (actor.CanJump && actor.JumpActionPress && !actor.Crouching && !physics.InAir)
			{
				Jump(physics, actor);
				stateMachine.ChangeState(ActorStates.InAir);
				return;
			}
			// Jumping.

			// Falling off.
			if (physics.InAir)
			{
				actor.JumpBufferAlarm.Set(actor.JumpBufferTime);
				stateMachine.ChangeState(ActorStates.InAir);
				
				if (actor.Crouching)
				{
					// NOTE: This may pose problems if there is not enough room to uncrouch.
					Uncrouch(position, physics, actor);
				}

				return;
			}
			// Falling off.



			if (!actor.Crouching && actor.CrouchAction)
			{
				Crouch(position, physics, actor);
			} 
			if (actor.Crouching && !actor.CrouchAction)
			{
				// Setting up new collider.
				var collider = (ICollider)physics.Collider.Clone();
				collider.Position = position.Position - Vector2.UnitY * (actor.Height - collider.Size.Y) / 2;
				collider.PreviousPosition = collider.Position;
				collider.Size = new Vector2(collider.Size.X, actor.Height);
				// Setting up new collider.

				// If a solid is above actor - keep crouching.
				if (PhysicsSystem.CheckCollision(actor.Owner, collider) == null)
				{
					Uncrouch(position, physics, actor);
					var stackables = SceneMgr.CurrentScene.GetEntityListByComponent<StackableActorComponent>();
					foreach(var stackable in stackables)
					{
						if (stackable != owner)
						{
							var stackablePosition = stackable.GetComponent<PositionComponent>();
							var stackableActor = stackable.GetComponent<StackableActorComponent>();

							if (
								GameMath.Distance(stackablePosition.Position, position.Position) < 100
								&& stackableActor.StateMachine.CurrentState != ActorStates.Stacked
							)
							{
								
								StackEntity(actor, stackableActor);

								break;
							}
						}
					}
				}
			}
			
			
			if (actor.Crouching)
			{
				actor.MaxMovementSpeed = actor.CrouchMovementSpeed;
				actor.Acceleration = actor.CrouchAcceleration;
				actor.Deceleration = actor.CrouchDeceleration;
			}
			else
			{
				actor.MaxMovementSpeed = actor.WalkMovementSpeed;
				actor.Acceleration = actor.GroundAcceleration;
				actor.Deceleration = actor.GroundDeceleration;	
			}

			physics.Gravity = actor.FallGravity;

			UpdateSpeed(actor, physics);

		}
		

		void InAirEnter(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			actor.LandingBufferAlarm.Reset();
		}
		
		void InAir(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			var physics = owner.GetComponent<PhysicsComponent>();
			//var position = owner.GetComponent<PositionComponent>();

			// Landing.
			if (!physics.InAir)
			{
				stateMachine.ChangeState(ActorStates.OnGround);
				return;
			}
			// Landing.


			// Jump buffering.
			actor.LandingBufferAlarm.Update();
			if (actor.JumpBufferAlarm.Update() || !actor.JumpBufferAlarm.Running)
			{
				actor.CanJump = false;
			}
			
			if (actor.JumpActionPress)
			{
				if (actor.CanJump)
				{
					Jump(physics, actor);
					actor.JumpBufferAlarm.Reset();
				}
				else
				{
					actor.LandingBufferAlarm.Set(actor.LandingBufferTime);	
				}
			}
			// Jump buffering.


			// Variable jump.
			if (actor.Jumping && actor.JumpAction && physics.Speed.Y < 0)
			{
				physics.Gravity = actor.JumpGravity;
			}
			else
			{
				physics.Gravity = actor.FallGravity;
				actor.Jumping = false; // When jump action is not applied anymore, actor is locked at fall gravity. 
			}
			// Variable jump.

			actor.Acceleration = actor.AirAcceleration;
			actor.Deceleration = actor.AirDeceleration;
			UpdateSpeed(actor, physics);
		}

		
		void StackedEnter(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			owner.DisableComponent<PhysicsComponent>();
		}
		void StackedExit(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			owner.EnableComponent<PhysicsComponent>();
		}


		void Stacked(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			var position = owner.GetComponent<PositionComponent>();
			var physics = owner.GetComponent<PhysicsComponent>();

			if (actor.StackedPrevious == null || actor.StackedPrevious.Destroyed)
			{
				stateMachine.ChangeState(ActorStates.InAir);
			}
			else
			{
				var masterPosition = actor.StackedPrevious.GetComponent<PositionComponent>();
				var masterPhysics = actor.StackedPrevious.GetComponent<PhysicsComponent>();
				var masterActor = actor.StackedPrevious.GetComponent<StackableActorComponent>();


				position.Position = masterPosition.PreviousPosition - Vector2.UnitY * masterPhysics.Collider.Size.Y / 2;

				if (masterActor.Crouching && !actor.Crouching)
				{
					Crouch(position, physics, actor);
				}
				
				if (!masterActor.Crouching && actor.Crouching)
				{
					Uncrouch(position, physics, actor);
				}

			}
		}

		void StackEntity(StackableActorComponent master, StackableActorComponent slave)
		{
			if (master.StackedNext != null)
			{
				StackEntity(master.StackedNext.GetComponent<StackableActorComponent>(), slave);
			}
			else
			{
				slave.StateMachine.ChangeState(ActorStates.Stacked);
				slave.StackedPrevious = master.Owner;
				master.StackedNext = slave.Owner;
			}
		}


		void UpdateSpeed(StackableActorComponent actor, PhysicsComponent physics)
		{
			int horMovement = 0;
			if (actor.LeftAction)
			{
				horMovement += -1;
			}
			if (actor.RightAction)
			{
				horMovement += 1;
			}

			
			if (horMovement == 0 || Math.Abs(physics.Speed.X) > actor.MaxMovementSpeed)
			{
				// Slowing down.
				if (physics.Speed.X != 0)
				{
					var spdSign = Math.Sign(physics.Speed.X);
					physics.Speed.X -= TimeKeeper.GlobalTime(spdSign * actor.Deceleration);
					if (Math.Sign(physics.Speed.X) != Math.Sign(spdSign))
					{
						physics.Speed.X = 0;
					}
				}
				// Slowing down.
			}
			else
			{
				// Speeding up.
				if (Math.Abs(physics.Speed.X) < actor.MaxMovementSpeed || Math.Sign(physics.Speed.X) != Math.Sign(horMovement))
				{
					physics.Speed.X += TimeKeeper.GlobalTime(horMovement * actor.Acceleration);

					if (Math.Abs(physics.Speed.X) > actor.MaxMovementSpeed)
					{
						physics.Speed.X = horMovement * actor.MaxMovementSpeed;
					}
				}
				// Speeding up.
			}
		}


		void Jump(PhysicsComponent physics, StackableActorComponent actor)
		{	
			physics.Speed.Y = -actor.JumpSpeed;
			actor.CanJump = false; 
			actor.Jumping = true;
		}

		void Crouch(PositionComponent position, PhysicsComponent physics, StackableActorComponent actor)
		{
			var colliderSize = physics.Collider.Size;
			var oldH = colliderSize.Y;
			colliderSize.Y = actor.CrouchingHeight;
			
			physics.Collider.Size = colliderSize;
				
			position.Position.Y -= (colliderSize.Y - oldH) / 2;

			actor.Crouching = true;
		}

		void Uncrouch(PositionComponent position, PhysicsComponent physics, StackableActorComponent actor)
		{
			var colliderSize = physics.Collider.Size;
			var oldH = colliderSize.Y;
			colliderSize.Y = actor.Height;
			
			physics.Collider.Size = colliderSize;
				
			position.Position.Y -= (colliderSize.Y - oldH) / 2;

			actor.Crouching = false;
		}

		public override void Draw(Component component)
		{
			var physics = component.Owner.GetComponent<PhysicsComponent>();
			var position = component.Owner.GetComponent<PositionComponent>();
			var actor = component.Owner.GetComponent<StackableActorComponent>();

			
			DrawMgr.DrawRectangle(
				position.Position.ToPoint().ToVector2() - physics.Collider.Size / 2,
				position.Position.ToPoint().ToVector2() + physics.Collider.Size / 2,
				true
			);

			if (actor.CanJump)
			{
				DrawMgr.CurrentColor = Color.Red;
			}
			else
			{
				DrawMgr.CurrentColor = Color.Black;
			}
			


			DrawMgr.CurrentFont = Resources.Fonts.Arial;
			DrawMgr.HorAlign = Engine.Drawing.TextAlign.Center;
			if (actor.StateMachine != null)
			{/*
				DrawMgr.DrawText(
					actor.StateMachine.CurrentState.ToString() 
					+ " " + actor.JumpBufferAlarm.Counter 
					+ " " + (position.Position.X - position.PreviousPosition.X),
					position.Position.ToPoint().ToVector2() - Vector2.UnitY * 32
				);*/
			}
			

		}


	}
}
