using Monofoxe.Engine.ECS;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.Drawing;
using Microsoft.Xna.Framework;

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

		#region Aminations.

		public StateMachine<ActorAnimationStates> AnimationStateMachine;
		
		public Sprite Main = Resources.Sprites.Default.PlayerMain;
		public Sprite CurrentSprite;
		public double SpriteAnimation = 0;
		public Vector2 SpriteScale = Vector2.One;
		public Vector2 SpriteOffset = Vector2.Zero;

		public int Orientation = 1;

		public double Animation = 0;
		public double AnimationSpeed = 0;


		public double WalkAnimationSpeed = 4f;
		public double CrouchAnimationSpeed = 10f;
		public double CrawlAnimationSpeed = 4;


		public Vector2 WalkMaxScale = new Vector2(0.05f, -0.1f);
		public Vector2 WalkMaxOffset = new Vector2(-5, 0);
		
		public Vector2 CrawlMaxScale = new Vector2(0.1f, -0.1f);
		public Vector2 CrawlMaxOffset = new Vector2(-5, 0);
		
		public Vector2 FallMaxScale = new Vector2(0f, 0.4f);
		public float FallBaseScale = 1000f;
		

		#endregion Animations.


		public override object Clone()
		{
			var c = new StackableActorComponent();
			
			return c;
		}
	}
}
