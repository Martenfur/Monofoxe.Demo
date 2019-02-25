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

	public enum ActorAnimationStates
	{
		Idle,
		Walking,
		Falling,
		CrouchTransition,
		Crouching,
		Crawling,
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

			actor.CurrentSprite = actor.Main;

		}

		public override void Update(List<Component> components)
		{
			foreach(StackableActorComponent actor in components)
			{
				actor.StateMachine.Update();
				actor.AnimationStateMachine.Update();

				if (actor.StackedPrevious == null && actor.StackedNext != null)
				{
					StackedUpdate(actor.StackedNext.GetComponent<StackableActorComponent>(), 90 + (float)Math.Sin(GameMgr.ElapsedTimeTotal) * 3);
				}

				// Maybe all this could be packed into class.
				actor.JumpActionPress = (actor.JumpAction && !actor.JumpActionPrevious);
				actor.JumpActionPrevious = actor.JumpAction; 
				
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
			) * (masterPhysics.Collider.Size.Y  + actor.StackBaseYOffset * yOffset);
			
			
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


		#endregion Stacked.



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
			actor.CurrentSprite = actor.Main;
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
			
			// Jumping and falling.
			if (actor.StateMachine.CurrentState == ActorStates.InAir)
			{
				stateMachine.ChangeState(ActorAnimationStates.Falling);
				return;
			}
			// Jumping and falling.
			
			// Walking.
			if (
				actor.StateMachine.CurrentState == ActorStates.OnGround 
				&& (actor.LeftAction != actor.RightAction)
			)
			{
				stateMachine.ChangeState(ActorAnimationStates.Walking);
				return;
			}
			// Walking.

			// Crouch.
			if (actor.StateMachine.CurrentState == ActorStates.OnGround && actor.Crouching)
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
				actor.StateMachine.CurrentState == ActorStates.OnGround 
				&& (actor.LeftAction == actor.RightAction)
			)
			{
				newState = ActorAnimationStates.Idle;
			}
			// Idle.

			// Jumping and falling.
			if (actor.StateMachine.CurrentState == ActorStates.InAir)
			{
				stateMachine.ChangeState(ActorAnimationStates.Falling);
				return;
			}
			// Jumping and falling.

			// Crouching.
			if (actor.StateMachine.CurrentState == ActorStates.OnGround && actor.Crouching)
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
			
			if (!actor.Crouching)
			{
				stateMachine.ChangeState(ActorAnimationStates.CrouchTransition);
				return;
			}

			if (
				actor.StateMachine.CurrentState == ActorStates.OnGround 
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


			if (actor.StateMachine.CurrentState == ActorStates.OnGround && !actor.Crouching)
			{
				newState = ActorAnimationStates.Idle;
			}

			if (actor.StateMachine.CurrentState == ActorStates.InAir)
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
				actor.StateMachine.CurrentState == ActorStates.OnGround 
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

			if (actor.StateMachine.CurrentState == ActorStates.InAir)
			{
				stateMachine.ChangeState(ActorAnimationStates.Falling);
				return;
			}

			if (actor.StateMachine.CurrentState == ActorStates.OnGround && !actor.Crouching)
			{	
				newState = ActorAnimationStates.CrouchTransition;
			}

			

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
			

			if (actor.StateMachine.CurrentState != ActorStates.InAir)
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
			var master = actor.StackedPrevious.GetComponent<StackableActorComponent>();

			//actor.Orientation.
		}

		#endregion Animations.



		public override void Draw(Component component)
		{
			var physics = component.Owner.GetComponent<PhysicsComponent>();
			var position = component.Owner.GetComponent<PositionComponent>();
			var actor = component.Owner.GetComponent<StackableActorComponent>();

			if (actor.StackedNext != null)
			{
				Draw(actor.StackedNext.GetComponent<StackableActorComponent>());
			}
			

			var ang = 0.0;
			if (actor.StackedPrevious != null)
			{
				ang = 90 - GameMath.Direction(actor.StackedPrevious.GetComponent<PositionComponent>().Position, position.Position);
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
		
			DrawMgr.DrawSprite(
				actor.CurrentSprite, 
				actor.SpriteAnimation,
				position.Position.Round() + physics.Collider.Size * Vector2.UnitY / 2 + actor.SpriteOffset, 
				actor.SpriteScale * new Vector2(actor.Orientation, 1f), 
				(float)ang, 
				Color.White
			);
			
			
			DrawMgr.HorAlign = Engine.Drawing.TextAlign.Center;
			
			if (actor.AnimationStateMachine != null)
			{/*
				DrawMgr.DrawText(
					actor.AnimationStateMachine.CurrentState.ToString() + " " + actor.Jumping,
					position.Position.ToPoint().ToVector2() - Vector2.UnitY * 64
				);*/
			}
			
		}


	}
}
