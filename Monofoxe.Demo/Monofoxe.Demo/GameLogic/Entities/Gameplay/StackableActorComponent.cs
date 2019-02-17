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

		public float AirAcceleration = 4000;
		public float AirDeceleration = 4000;
		
		public float FallGravity = 3500;
		
		// In air.

		
		// Jumping.
		
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
		
		

		// TODO: Remove!
		public float MinY = 0;



		public override object Clone()
		{
			var c = new StackableActorComponent();
			
			return c;
		}
	}
}
