using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;


namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public enum ActorStates
	{
		OnGround,
		InAir,
		Stacked,
		Dead,
	}

	public enum ActorAnimationStates
	{
		Idle,
		Walking,
		Falling,
		CrouchTransition,
		Crouching,
		Crawling,
		Stacked,
		Dead,
	}

	public class StackableActorSystem : BaseSystem
	{
	

		public override Type ComponentType => typeof(StackableActorComponent);

		
		public override void Create(Component component)
		{
			var actor = (StackableActorComponent)component;
			actor.LogicStateMachine = new StateMachine<ActorStates>(ActorStates.OnGround, actor.Owner);
			actor.LogicStateMachine.AddState(ActorStates.OnGround, OnGround, OnGroundEnter);
			actor.LogicStateMachine.AddState(ActorStates.InAir, InAir, InAirEnter);
			actor.LogicStateMachine.AddState(ActorStates.Stacked, Stacked, StackedEnter, StackedExit);
			actor.LogicStateMachine.AddState(ActorStates.Dead, Dead, DeadEnter);

			actor.AnimationStateMachine = new StateMachine<ActorAnimationStates>(ActorAnimationStates.Idle, actor.Owner);
			actor.AnimationStateMachine.AddState(ActorAnimationStates.Idle, IdleAnimation, IdleAnimationEnter);
			actor.AnimationStateMachine.AddState(ActorAnimationStates.Falling, FallAnimation, FallAnimationEnter);
			actor.AnimationStateMachine.AddState(ActorAnimationStates.Walking, WalkAnimation, WalkAnimationEnter);
			actor.AnimationStateMachine.AddState(ActorAnimationStates.Crawling, CrawlAnimation, CrawlAnimationEnter);
			actor.AnimationStateMachine.AddState(ActorAnimationStates.Crouching, CrouchAnimation, CrouchAnimationEnter);
			actor.AnimationStateMachine.AddState(ActorAnimationStates.CrouchTransition, CrouchTransitionAnimation, CrouchTransitionAnimationEnter);
			actor.AnimationStateMachine.AddState(ActorAnimationStates.Stacked, StackedAnimation, StackedAnimationEnter);

			

			actor.JumpBufferAlarm = new Alarm();
			actor.LandingBufferAlarm = new Alarm();

			actor.CurrentSprite = actor.MainSprite;

			actor.Height = actor.Owner.GetComponent<PhysicsComponent>().Collider.Size.Y;

		}

		public override void Update(List<Component> components)
		{
			foreach(StackableActorComponent actor in components)
			{
				actor.LogicStateMachine.Update();
				actor.AnimationStateMachine.Update();

				if (actor.StackedPrevious == null && actor.StackedNext != null)
				{
					StackedUpdate(actor.StackedNext.GetComponent<StackableActorComponent>(), 90 + (float)Math.Sin(GameMgr.ElapsedTimeTotal) * 3, 1);
				}

				// Maybe all this could be packed into class.
				actor.JumpActionPress = (actor.JumpAction && !actor.JumpActionPrevious);
				actor.JumpActionPrevious = actor.JumpAction; 
				
				if (
					actor.LogicStateMachine.CurrentState != ActorStates.Dead 
					&& actor.Owner.GetComponent<PhysicsComponent>().Squashed
				)
				{
					Kill(actor);
				}

			}
		}


		#region On the ground.

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
						if (other != owner && other.Enabled)
						{
							var otherPosition = other.GetComponent<PositionComponent>();
							var otherPhysics = other.GetComponent<PhysicsComponent>();
							var otherActor = other.GetComponent<StackableActorComponent>();

							otherPhysics.Collider.Position = otherPosition.Position;
							otherPhysics.Collider.PreviousPosition = otherPosition.PreviousPosition;

							if (
								CollisionDetector.CheckCollision(physics.Collider, otherPhysics.Collider)
								&& otherActor.LogicStateMachine.CurrentState == ActorStates.OnGround
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

		void Jump(PhysicsComponent physics, StackableActorComponent actor)
		{	
			physics.Speed.Y = -actor.JumpSpeed;
			if (physics.StandingOn != null)
			{
				physics.Speed += physics.StandingOn.GetComponent<SolidComponent>().Speed;
			}
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

		#endregion On the ground.



		#region In air.

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

		#endregion In air.



		#region Stacked.

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

			if (
				actor.StackedPrevious == null 
				|| actor.StackedPrevious.Destroyed 
				|| actor.StackedPrevious.GetComponent<StackableActorComponent>().LogicStateMachine.CurrentState == ActorStates.Dead
			)
			{
				stateMachine.ChangeState(ActorStates.Dead);
				return;
			}


			var collider = (ICollider)physics.Collider.Clone();
			
			collider.Position = position.Position;
			collider.PreviousPosition = position.Position; 
			// Making collider a bit smaller, so actor will not die to everything.
			collider.Size /= 3f; 

			var colliderEntity = PhysicsSystem.CheckCollision(physics.Owner, collider);

			if (colliderEntity != null && !(colliderEntity.GetComponent<SolidComponent>().Collider is PlatformCollider))
			{
				stateMachine.ChangeState(ActorStates.Dead);
				return;
			}
			
		}
		
		void StackedUpdate(StackableActorComponent actor, float baseDirection, int stackIndex)
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
			actor.StackedTargetPosition = masterPosition.Position 
			+ new Vector2(
				(float)Math.Cos(dir), 
				(float)-Math.Sin(dir)
			) * (masterPhysics.Collider.Size.Y);
			
			
			// TODO: Figure out exact formula.
			position.Position.Y += TimeKeeper.GlobalTime((-position.Position.Y + actor.StackedTargetPosition.Y) / actor.StackPositionDelayDivider);
			//position.Position.Y += (-position.Position.Y + actor.StackedTargetPosition.Y) / actor.StackPositionDelayDivider;
			position.Position.X = actor.StackedTargetPosition.X;
			// Nice y delaying.

			

			if (actor.StackedNext != null)
			{
				StackedUpdate(
					actor.StackedNext.GetComponent<StackableActorComponent>(), 
					baseDirection + actor.StackDirectionOffset, 
					stackIndex + 1
				);
			}

			if (stackIndex > actor.StackLimit)
			{
				actor.LogicStateMachine.ChangeState(ActorStates.Dead);
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
				slave.LogicStateMachine.ChangeState(ActorStates.Stacked);
				slave.StackedPrevious = master.Owner;
				master.StackedNext = slave.Owner;
			}
		}

		#endregion Stacked.



		#region Dead.

		void DeadEnter(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			var physics = owner.GetComponent<PhysicsComponent>();
			var actor = owner.GetComponent<StackableActorComponent>();
			
			if (actor.StackedPrevious != null)
			{
				actor.StackedPrevious.GetComponent<StackableActorComponent>().StackedNext = null;
				actor.StackedPrevious = null;	
			}

			physics.Collider.Enabled = false; // Disabling collisions.

			// Applying some speed for cool flying off the screen.
			physics.Speed = new Vector2(
				(float)GameplayController.Random.NextDouble(actor.DeadMinSpeed.X, actor.DeadMaxSpeed.X) * GameplayController.Random.Choose(-1, 1), 
				(float)GameplayController.Random.NextDouble(actor.DeadMinSpeed.Y, actor.DeadMaxSpeed.Y)
			);
			
			physics.Gravity = actor.DeadGravity;
		}

		
		void Dead(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			var position = owner.GetComponent<PositionComponent>();
			var physics = owner.GetComponent<PhysicsComponent>();
			
			position.Position.Y += 4;

			var camera = SceneMgr.CurrentScene.GetEntityList<GameCamera>()[0];
			
			if (!GameMath.PointInRectangle(position.Position, camera.Camera.Position - camera.Camera.Size, camera.Camera.Position + camera.Camera.Size))
			{
				EntityMgr.DestroyEntity(actor.Owner);
			}
		}
		
		#endregion Dead.
		



		void UpdateSpeed(StackableActorComponent actor, PhysicsComponent physics)
		{
			var horMovement = actor.RightAction.ToInt() - actor.LeftAction.ToInt();
			
			if (
				horMovement == 0 
				|| (
					Math.Abs(physics.Speed.X) > actor.MaxMovementSpeed 
					&& ((!actor.RightAction && !actor.LeftAction) || actor.Crouching)
				)
			)
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



		public static void Kill(StackableActorComponent actor)
		{
			if (actor.LogicStateMachine.CurrentState == ActorStates.Dead)
			{
				return;
			}

			var position = actor.Owner.GetComponent<PositionComponent>();
			
			actor.LogicStateMachine.ChangeState(ActorStates.Dead);

			foreach(var camera in actor.Owner.Scene.GetEntityList<GameCamera>())
			{
				if (camera.Target == position) // If this camera follows player.
				{
					camera.Target = null;
				}
			}
			
		}

		public static void Damage(StackableActorComponent actor)
		{
			// This later can be expanded to take away health.
			Kill(actor);
		}


		#region Animations.

		bool UpdateAnimation(StateMachine<ActorAnimationStates> stateMachine, StackableActorComponent actor, ActorAnimationStates newState)
		{
			actor.Animation += TimeKeeper.GlobalTime(actor.AnimationSpeed);

			if (actor.Animation >= 1 || actor.Animation < 0)
			{
				actor.Animation -= 1;

				if (newState != stateMachine.CurrentState)
				{
					stateMachine.ChangeState(newState);
					return true;
				}
			}
			return false;
		}

		void ResetAnimation(StackableActorComponent actor)
		{
			actor.CurrentSprite = actor.MainSprite;
			actor.Animation = 0;
			actor.SpriteAnimation = 0;
			actor.AnimationSpeed = actor.WalkAnimationSpeed;
			actor.SpriteScale = Vector2.One;
			actor.SpriteOffset = Vector2.Zero;
		}

		void UpdateOrientation(StackableActorComponent actor)
		{
			if (actor.LeftAction != actor.RightAction)
			{
				if (actor.LeftAction)
				{
					actor.Orientation = -1;
				}
				if (actor.RightAction)
				{
					actor.Orientation = 1;
				}
			}
		}


		void IdleAnimationEnter(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			ResetAnimation(actor);
		}

		void IdleAnimation(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			
			// Stacked.
			if (actor.LogicStateMachine.CurrentState == ActorStates.Stacked)
			{
				stateMachine.ChangeState(ActorAnimationStates.Stacked);
				return;
			}
			// Stacked.


			// Jumping and falling.
			if (actor.LogicStateMachine.CurrentState == ActorStates.InAir)
			{
				stateMachine.ChangeState(ActorAnimationStates.Falling);
				return;
			}
			// Jumping and falling.
			
			// Walking.
			if (
				actor.LogicStateMachine.CurrentState == ActorStates.OnGround 
				&& (actor.LeftAction != actor.RightAction)
			)
			{
				stateMachine.ChangeState(ActorAnimationStates.Walking);
				return;
			}
			// Walking.

			// Crouch.
			if (actor.LogicStateMachine.CurrentState == ActorStates.OnGround && actor.Crouching)
			{
				stateMachine.ChangeState(ActorAnimationStates.CrouchTransition);
				return;
			}
			// Crouch.

		}
		


		void WalkAnimationEnter(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			ResetAnimation(actor);
			actor.AnimationSpeed = actor.WalkAnimationSpeed;
		}

		void WalkAnimation(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();

			var newState = stateMachine.CurrentState;

			// Idle.
			if (
				actor.LogicStateMachine.CurrentState == ActorStates.OnGround 
				&& (actor.LeftAction == actor.RightAction)
			)
			{
				newState = ActorAnimationStates.Idle;
			}
			// Idle.

			// Stacked.
			if (actor.LogicStateMachine.CurrentState == ActorStates.Stacked)
			{
				newState = ActorAnimationStates.Stacked;
			}
			// Stacked.

			// Jumping and falling.
			if (actor.LogicStateMachine.CurrentState == ActorStates.InAir)
			{
				stateMachine.ChangeState(ActorAnimationStates.Falling);
				return;
			}
			// Jumping and falling.

			// Crouching.
			if (actor.LogicStateMachine.CurrentState == ActorStates.OnGround && actor.Crouching)
			{
				stateMachine.ChangeState(ActorAnimationStates.CrouchTransition);
				return;
			}
			// Crouching.
			

			// Animation.
			if (UpdateAnimation(stateMachine, actor, newState))
			{
				return;
			}

			var sin = (float)Math.Sin(actor.Animation * Math.PI);
			actor.SpriteScale = Vector2.One + actor.WalkMaxScale * new Vector2(Math.Abs(sin), Math.Abs(sin));
			actor.SpriteOffset = actor.WalkMaxOffset * sin * (Vector2.UnitX * -actor.Orientation);
			// Animation.

			UpdateOrientation(actor);
		}


		
		void CrouchAnimationEnter(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			ResetAnimation(actor);
			
			actor.SpriteAnimation = 0.99f;
		}

		void CrouchAnimation(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			
			// Stacked.
			if (actor.LogicStateMachine.CurrentState == ActorStates.Stacked)
			{
				stateMachine.ChangeState(ActorAnimationStates.Stacked);
				return;
			}
			// Stacked.

			if (!actor.Crouching)
			{
				stateMachine.ChangeState(ActorAnimationStates.CrouchTransition);
				return;
			}

			if (
				actor.LogicStateMachine.CurrentState == ActorStates.OnGround 
				&& (actor.LeftAction != actor.RightAction) 
			)
			{
				var physics = owner.GetComponent<PhysicsComponent>();
				if (Math.Abs(physics.Speed.X) <= actor.MaxMovementSpeed)
				{
					if (actor.Crouching)
					{
						stateMachine.ChangeState(ActorAnimationStates.Crawling);
					}
					else
					{
						stateMachine.ChangeState(ActorAnimationStates.CrouchTransition);
					}
				}
			}

		}



		void CrouchTransitionAnimationEnter(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			ResetAnimation(actor);
			
			actor.AnimationSpeed = actor.CrouchAnimationSpeed;
			if (
				stateMachine.PreviousState == ActorAnimationStates.Crouching
				|| stateMachine.PreviousState == ActorAnimationStates.Crawling
			)
			{
				actor.Animation = 0.99f;
				actor.SpriteAnimation = 0.99f;
				actor.AnimationSpeed = -actor.CrouchAnimationSpeed;
			}
			
		}

		void CrouchTransitionAnimation(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			var newState = ActorAnimationStates.Crouching;

			if (actor.Crouching)
			{
				actor.AnimationSpeed = actor.CrouchAnimationSpeed;
			}
			else
			{
				actor.AnimationSpeed = -actor.CrouchAnimationSpeed;
			}


			if (actor.LogicStateMachine.CurrentState == ActorStates.OnGround && !actor.Crouching)
			{
				newState = ActorAnimationStates.Idle;
			}

			if (actor.LogicStateMachine.CurrentState == ActorStates.InAir)
			{
				newState = ActorAnimationStates.Falling;
			}
			

			actor.SpriteAnimation = actor.Animation;

			if (actor.Animation > 0.5f)
			{
				actor.SpriteScale = Vector2.One;
			}
			else
			{
				actor.SpriteScale = new Vector2(1, 1 - (float)(actor.Animation * actor.Animation));
			}

			UpdateAnimation(stateMachine, actor, newState);

			UpdateOrientation(actor);
		}


		void CrawlAnimationEnter(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			ResetAnimation(actor);
			actor.SpriteAnimation = 0.99f;
			actor.AnimationSpeed = actor.CrawlAnimationSpeed;
		}

		void CrawlAnimation(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();

			var newState = stateMachine.CurrentState;

			if (
				actor.LogicStateMachine.CurrentState == ActorStates.OnGround 
				&& (actor.LeftAction == actor.RightAction)
			)
			{
				if (actor.Crouching)
				{
					newState = ActorAnimationStates.Crouching;
				}
				else
				{
					newState = ActorAnimationStates.CrouchTransition;
				}
			}

			if (actor.LogicStateMachine.CurrentState == ActorStates.InAir)
			{
				stateMachine.ChangeState(ActorAnimationStates.Falling);
				return;
			}

			if (actor.LogicStateMachine.CurrentState == ActorStates.OnGround && !actor.Crouching)
			{	
				newState = ActorAnimationStates.CrouchTransition;
			}

			// Stacked.
			if (actor.LogicStateMachine.CurrentState == ActorStates.Stacked)
			{
				newState = ActorAnimationStates.Stacked;
			}
			// Stacked.


			if (UpdateAnimation(stateMachine, actor, newState))
			{
				return;
			}

			var sin = (float)Math.Sin(actor.Animation * Math.PI);
			actor.SpriteScale = Vector2.One + actor.CrawlMaxScale * new Vector2(1, Math.Abs(sin));
			actor.SpriteOffset = actor.CrawlMaxOffset * sin;

			UpdateOrientation(actor);
		}



		void FallAnimationEnter(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			ResetAnimation(actor);
			actor.AnimationSpeed = actor.CrouchAnimationSpeed;
		}

		void FallAnimation(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			var physics = owner.GetComponent<PhysicsComponent>();
			

			if (actor.LogicStateMachine.CurrentState != ActorStates.InAir)
			{
				stateMachine.ChangeState(ActorAnimationStates.Idle);
				return;
			}

			actor.Animation = -physics.Speed.Y / actor.FallBaseScale;
			if (actor.Animation < 0)
			{
				actor.Animation = 0;
			}


			actor.SpriteScale = Vector2.One + actor.FallMaxScale * (float)actor.Animation;
			

			UpdateOrientation(actor);
		}

		void StackedAnimationEnter(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			ResetAnimation(actor);
		}

		void StackedAnimation(StateMachine<ActorAnimationStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();

			if (actor.StackedPrevious != null)
			{
				var master = actor.StackedPrevious.GetComponent<StackableActorComponent>();
			}

		}

		#endregion Animations.



		public override void Draw(Component component)
		{
			var physics = component.Owner.GetComponent<PhysicsComponent>();
			var position = component.Owner.GetComponent<PositionComponent>();
			var actor = component.Owner.GetComponent<StackableActorComponent>();

			// Drawing the stack.
			if (actor.StackedNext != null)
			{
				// Stack needs to be drawn recursively.
				Draw(actor.StackedNext.GetComponent<StackableActorComponent>());
			}
			// Drawing the stack.

			var ang = 0.0;
			if (actor.StackedPrevious != null)
			{
				ang = 90 + GameMath.Direction(actor.StackedPrevious.GetComponent<PositionComponent>().Position, position.Position);
				var master = actor.StackedPrevious.GetComponent<StackableActorComponent>();

				actor.Orientation = master.Orientation;
				if (master.Crouching)
				{
					actor.SpriteAnimation = 0.99f;
				}
				else
				{
					actor.SpriteAnimation = 0f;
				}
			}
			if (actor.LogicStateMachine != null && actor.LogicStateMachine.CurrentState == ActorStates.Dead)
			{
				ang = -GameMath.Direction(physics.Speed * new Vector2(-1, 1)) - 90;
			}

			var color = Color.White;

			if (physics.Squashed)
			{
				color = Color.Red;
			}
		
			DrawMgr.DrawSprite(
				actor.CurrentSprite, 
				actor.SpriteAnimation,
				position.Position.Round() + physics.Collider.Size * Vector2.UnitY / 2 + actor.SpriteOffset, 
				actor.SpriteScale * new Vector2(actor.Orientation, 1f), 
				(float)ang, 
				color
			);
		}


	}
}
