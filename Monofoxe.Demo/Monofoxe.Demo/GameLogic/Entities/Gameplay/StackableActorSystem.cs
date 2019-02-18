using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.ECS;


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
			actor.StateMachine.AddState(ActorStates.InAir, InAir);
			actor.StateMachine.AddState(ActorStates.Stacked, Stacked);

			actor.JumpBufferAlarm = new Alarm();
			
		}

		public override void Update(List<Component> components)
		{
			foreach(StackableActorComponent actor in components)
			{
				actor.StateMachine.Update();
				
				if (actor.MinY > actor.Owner.GetComponent<PositionComponent>().Position.Y)
				{
					actor.MinY = actor.Owner.GetComponent<PositionComponent>().Position.Y;
				}
				if (Input.CheckButton(Buttons.R))
				{
					actor.MinY = 10000;
				}
			}
		}

		void OnGroundEnter(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			actor.CanJump = false;
		}
		

		void OnGround(StateMachine<ActorStates> stateMachine, Entity owner)
		{

			var actor = owner.GetComponent<StackableActorComponent>();
			var physics = owner.GetComponent<PhysicsComponent>();
			var position= owner.GetComponent<PositionComponent>();

			
			if (!actor.JumpAction)
			{
				actor.CanJump = true;
			}

			// Jumping.
			if (actor.CanJump && actor.JumpAction && !physics.InAir)
			{
				physics.Speed.Y = -actor.JumpSpeed;
				actor.CanJump = false; 
				stateMachine.ChangeState(ActorStates.InAir);
				return;
			}
			// Jumping.

			// Falling off.
			if (physics.InAir)
			{
				actor.JumpBufferAlarm.Set(actor.JumpBufferTime);
				stateMachine.ChangeState(ActorStates.InAir);
				return;
			}
			// Falling off.



			// Add ceiling check here.

			if ((!actor.Crouching && actor.CrouchAction) || (actor.Crouching && !actor.CrouchAction))
			{
				var colliderSize = physics.Collider.Size;
				var oldH = colliderSize.Y;
				if (!actor.Crouching)
				{
					colliderSize.Y = actor.CrouchingHeight;
				}
				else
				{
					colliderSize.Y = actor.Height;
				}

				physics.Collider.Size = colliderSize;
				
				position.Position.Y -= (colliderSize.Y - oldH) / 2;
			}


			actor.Crouching = actor.CrouchAction;
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
		
		
		void InAir(StateMachine<ActorStates> stateMachine, Entity owner)
		{
			var actor = owner.GetComponent<StackableActorComponent>();
			var physics = owner.GetComponent<PhysicsComponent>();
			var position = owner.GetComponent<PositionComponent>();


			if (!physics.InAir)
			{
				stateMachine.ChangeState(ActorStates.OnGround);
				return;
			}
			
			
			if (actor.JumpBufferAlarm.Update())
			{
				actor.CanJump = false;
			}
			if (actor.CanJump && actor.JumpAction)
			{
				physics.Speed.Y = -actor.JumpSpeed;
				actor.JumpBufferAlarm.Reset();
				actor.CanJump = false; 
			}

			// REPLACE!
			if (actor.JumpAction && physics.Speed.Y < 0)
			{
				physics.Gravity = actor.JumpGravity;
			}
			else
			{
				physics.Gravity = actor.FallGravity;
			}

			actor.Acceleration = actor.AirAcceleration;
			actor.Deceleration = actor.AirDeceleration;
			UpdateSpeed(actor, physics);
		}

		void Stacked(StateMachine<ActorStates> stateMachine, Entity owner)
		{
		
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
				if (physics.Speed.X != 0)
				{
					var spdSign = Math.Sign(physics.Speed.X);
					physics.Speed.X -= TimeKeeper.GlobalTime(spdSign * actor.Deceleration);
					if (Math.Sign(physics.Speed.X) != Math.Sign(spdSign))
					{
						physics.Speed.X = 0;
					}
				}
			}
			else
			{
				if (Math.Abs(physics.Speed.X) < actor.MaxMovementSpeed || Math.Sign(physics.Speed.X) != Math.Sign(horMovement))
				{
					physics.Speed.X += TimeKeeper.GlobalTime(horMovement * actor.Acceleration);

					if (Math.Abs(physics.Speed.X) > actor.MaxMovementSpeed)
					{
						physics.Speed.X = horMovement * actor.MaxMovementSpeed;
					}
				}
			}
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
			DrawMgr.DrawText(
				actor.StateMachine.CurrentState.ToString() 
				+ " " + actor.JumpBufferAlarm.Counter 
				+ " " + (position.Position.X - position.PreviousPosition.X),
				position.Position.ToPoint().ToVector2() - Vector2.UnitY * 32
			);

			if (physics.Squashed)
			{
				DrawMgr.DrawRectangle(
					position.Position.ToPoint().ToVector2() - physics.Collider.Size / 4,
					position.Position.ToPoint().ToVector2() + physics.Collider.Size / 4,
					true
				);
			}
			DrawMgr.DrawLine(
				position.Position.ToPoint().ToVector2(), 
				position.Position.ToPoint().ToVector2() + new Vector2(physics.CollisionH, physics.CollisionV) * 32
			);
			

			DrawMgr.DrawLine(0, actor.MinY, 1000, actor.MinY);
			

		}


	}
}
