using Monofoxe.Engine.ECS;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class StackableActorComponent : Component
	{
		
		// Actions.

		public bool LeftAction;
		public bool RightAction;
		public bool JumpAction;
		public bool CrouchAction;

		public bool JumpActionPress;
		public bool JumpActionPrevious;
		
		// Actions.

		public int Height = 32;

		public StateMachine<ActorStates> StateMachine;
		

		// Walking.

		public float WalkMovementSpeed = 600;
		public float MaxMovementSpeed;
		
		public float Acceleration; 
		public float Deceleration; 

		public float GroundAcceleration = 4000;
		public float GroundDeceleration = 8000;
	
		// Walking.
		

		// In air.

		public float AirAcceleration = 3000;
		public float AirDeceleration = 5000;
		
		public float FallGravity = 3500;
		
		public double LandingBufferTime = 0.1;
		public Alarm LandingBufferAlarm;

		// In air.

		
		// Jumping.

		public bool Jumping = true;
		public bool CanJump = true;
		
		public float JumpSpeed = 700;
		public float JumpGravity = 1500;

		public double JumpBufferTime = 0.1;
		public Alarm JumpBufferAlarm;

		// Jumping.


		// Crouching.
		
		public int CrouchingHeight = 8;

		public bool Crouching = false;

		public float CrouchMovementSpeed = 60;

		public float CrouchAcceleration = 4000;
		public float CrouchDeceleration = 400;

		// Crouching.
		
		// Stacking.

		public Entity StackedNext;
		public Entity StackedPrevious;
		public float StackDirectionOffset;
		public float StackDirectionOffsetTarget;
		public float StackDirectionMaxOffset = 3;
		

		public float PendulumMomentum;
		public float PendulumMomentumMax = 15;
		public float PendulumRigidity = 60.08f;

		public float PendulumEnergyLossRate = 16f;
		public float PendulumForceMultiplier = 160f;
		
		public float StackYOffsetDivider = 40f;
		public float StackYOffsetMin = -1f;
		public float StackYOffsetMax = 1f;
		public float StackBaseYOffset = 10;
		// Stacking.


		public override object Clone()
		{
			var c = new StackableActorComponent();
			
			return c;
		}
	}
}
