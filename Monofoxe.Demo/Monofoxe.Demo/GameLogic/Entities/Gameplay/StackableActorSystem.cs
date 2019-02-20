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

		//public override int Priority => 1;


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
				
				if (actor.StackedPrevious == null && actor.StackedNext != null)
				{
					StackedUpdate(actor.StackedNext.GetComponent<StackableActorComponent>(), 90 + (float)Math.Sin(GameMgr.ElapsedTimeTotal) * 0);
				}

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
				return;
			}
			// Jumping.

			// Falling off.
			if (physics.InAir)
			{
				if (!actor.Jumping)
				{
					actor.JumpBufferAlarm.Set(actor.JumpBufferTime);
					if (actor.Crouching)
					{
						// NOTE: This may pose problems if there is not enough room to uncrouch.
						Uncrouch(position, physics, actor);
					}
				}

				stateMachine.ChangeState(ActorStates.InAir);
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
					physics.Collider.Position = position.Position;
					physics.Collider.PreviousPosition = position.PreviousPosition;
					
					foreach(var other in stackables)
					{
						if (other != owner)
						{
							var otherPosition = other.GetComponent<PositionComponent>();
							var otherPhysics = other.GetComponent<PhysicsComponent>();
							var otherActor = other.GetComponent<StackableActorComponent>();

							otherPhysics.Collider.Position = otherPosition.Position;
							otherPhysics.Collider.PreviousPosition = otherPosition.PreviousPosition;

							if (
								CollisionDetector.CheckCollision(physics.Collider, otherPhysics.Collider)
								&& otherActor.StateMachine.CurrentState != ActorStates.Stacked
							)
							{
								StackEntity(actor, otherActor);
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
			var physics = owner.GetComponent<PhysicsComponent>();
			
			actor.LandingBufferAlarm.Reset();
		}
		
		void InAir(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			var physics = owner.GetComponent<PhysicsComponent>();
			
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
			owner.GetComponent<PhysicsComponent>().Enabled = false;
			owner.GetComponent<StackableActorComponent>().Visible = false;
		}
		void StackedExit(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			owner.GetComponent<PhysicsComponent>().Enabled = true;
			owner.GetComponent<StackableActorComponent>().Visible = true;
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
		}

		void StackedUpdate(StackableActorComponent actor, float baseDirection)
		{
			var position = actor.Owner.GetComponent<PositionComponent>();
			var physics = actor.Owner.GetComponent<PhysicsComponent>();

			var masterPosition = actor.StackedPrevious.GetComponent<PositionComponent>();
			var masterPhysics = actor.StackedPrevious.GetComponent<PhysicsComponent>();
			var masterActor = actor.StackedPrevious.GetComponent<StackableActorComponent>();


			// Pendulum.

			// We are applying force to the pendulum, and it tries to push back.
			// Jiggly stacking stuff happens here. 

			var pendulumForce = (masterPosition.Position.X - masterPosition.PreviousPosition.X) * actor.PendulumForceMultiplier;
			var pendulumSpringForce = (float)Math.Pow(actor.StackDirectionOffset, 3) * actor.PendulumRigidity;

			actor.PendulumMomentum += TimeKeeper.GlobalTime(pendulumForce - pendulumSpringForce);
			actor.PendulumMomentum -= Math.Sign(actor.PendulumMomentum) * TimeKeeper.GlobalTime(actor.PendulumEnergyLossRate);

			if (Math.Abs(actor.PendulumMomentum) > actor.PendulumMomentumMax)
			{
				actor.PendulumMomentum = actor.PendulumMomentumMax * Math.Sign(actor.PendulumMomentum);
			}
			actor.StackDirectionOffset += TimeKeeper.GlobalTime(actor.PendulumMomentum);
			
			if (Math.Abs(actor.StackDirectionOffset) > actor.StackDirectionMaxOffset)
			{
				actor.StackDirectionOffset = actor.StackDirectionMaxOffset * Math.Sign(actor.StackDirectionOffset);
			}
			
			var dir = MathHelper.ToRadians(baseDirection + actor.StackDirectionOffset);
			// Pendulum.


			// Crouch transfer.
			if (masterActor.Crouching && !actor.Crouching)
			{
				Crouch(position, physics, actor);
			}	
			if (!masterActor.Crouching && actor.Crouching)
			{
				Uncrouch(position, physics, actor);
			}
			// Crouch transfer.


			// Nice y delaying.
			var yOffset = (masterPosition.Position.Y - masterPosition.PreviousPosition.Y) / actor.StackYOffsetDivider;
			if (yOffset < actor.StackYOffsetMin)
			{
				yOffset = actor.StackYOffsetMin;
			}
			if (yOffset > actor.StackYOffsetMax)
			{
				yOffset = actor.StackYOffsetMax;
			}
			// Nice y delaying.

			position.Position = masterPosition.Position 
			+ new Vector2(
				(float)Math.Cos(dir), 
				(float)-Math.Sin(dir)
			) * (masterPhysics.Collider.Size.Y * 0.5f + actor.StackBaseYOffset * yOffset);
			
			
			if (actor.StackedNext != null)
			{
				StackedUpdate(actor.StackedNext.GetComponent<StackableActorComponent>(), baseDirection + actor.StackDirectionOffset);
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

			if (actor.StackedNext != null)
			{
				Draw(actor.StackedNext.GetComponent<StackableActorComponent>());
			}
			
			DrawMgr.DrawRectangle(
				position.Position.ToPoint().ToVector2() - physics.Collider.Size / 2,
				position.Position.ToPoint().ToVector2() + physics.Collider.Size / 2,
				false
			);
			

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
