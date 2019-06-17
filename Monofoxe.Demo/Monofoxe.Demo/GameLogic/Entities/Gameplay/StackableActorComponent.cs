using Microsoft.Xna.Framework;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.Utils;
using ChaiFoxes.FMODAudio;
using System.Collections.Generic;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class StackableActorComponent : Component
	{
		
		// Actions.

		// This is component's interface.
		// AI components should control actor
		// only through it.

		public bool LeftAction;
		public bool RightAction;
		public bool JumpAction;
		public bool CrouchAction;

		// Actions.

		public bool JumpActionPress;
		public bool JumpActionPrevious;
		

		/// <summary>
		/// Logic state machine. Controls actor's gameplay states.
		/// </summary>
		public StateMachine<ActorStates> LogicStateMachine;
		
		
		
		public float MaxMovementSpeed;
		public float Acceleration; 
		public float Deceleration; 		

		public float Height;

		// In air.

		public Alarm LandingBufferAlarm;

		// In air.

		
		// Jumping.

		public bool Jumping = true;
		public bool CanJump = true;
		
		public Alarm JumpBufferAlarm;

		// Jumping.


		// Crouching.
		
		public bool Crouching = false;

		public SoundChannel SlideSound;

		// Crouching.
		
		// Stacking.

		public Entity StackedNext;
		public Entity StackedPrevious;
		public Entity StackOwner;

		public float StackDirectionOffset;
		public float StackDirectionOffsetTarget;
		public float StackDirectionMaxOffset = 3;
		
		public int StackLimit = 8;

		public float PendulumMomentum;
		public float PendulumMomentumMax = 15;
		public float PendulumRigidity = 60.08f;

		public float PendulumEnergyLossRate = 16f;
		public float PendulumForceMultiplier = 160f;
		

		public float StackPositionDelayDivider = 2 / 60f;

		public Vector2 StackedTargetPosition;

		// Stacking.

		// Dead.

		public float DeadGravity = 1500;
		public Vector2 DeadMinSpeed = new Vector2(300, -800);
		public Vector2 DeadMaxSpeed = new Vector2(400, -700);
		public bool SyncAngleWithSpeedVector = true;

		// Dead.

		#region Customizable properties.

		// Walking.

		/// <summary>
		/// Maximum walking speed.
		/// </summary>
		public float WalkMovementSpeed = 500;
	
		public float GroundAcceleration = 4000;
		public float GroundDeceleration = 8000;
	
		// Walking.
		

		// In air.

		public float AirAcceleration = 3000;
		public float AirDeceleration = 5000;
		
		public float FallGravity = 3500;
		
		public double LandingBufferTime = 0.1;

		// In air.

		
		// Jumping.

		public float JumpSpeed = 750;
		public float JumpGravity = 1500;

		public double JumpBufferTime = 0.11;

		// Jumping.


		// Crouching.
		
		public int CrouchingHeight = 8;

		public float CrouchMovementSpeed = 60;

		public float CrouchAcceleration = 4000;
		public float CrouchDeceleration = 400;

		// Crouching.
		
		public bool Silent = false;
		public List<Sound> PickupSounds = new List<Sound>();
		public bool FellIntoBottomlessPit = false;

		#endregion Customizable properties.


		#region Animations.

		/// <summary>
		/// State machine which controls animations.
		/// </summary>
		public StateMachine<ActorAnimationStates> AnimationStateMachine;
		
		public Sprite MainSprite = Resources.Sprites.Default.Foxe;
		public Sprite SleepingSprite = Resources.Sprites.Default.FoxeSleeping;

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
		public double SleepAnimationSpeed = 0.2f;


		public Vector2 WalkMaxScale = new Vector2(0.05f, -0.1f);
		public Vector2 WalkMaxOffset = new Vector2(-5, 0);
		
		public Vector2 CrawlMaxScale = new Vector2(0.1f, -0.1f);
		public Vector2 CrawlMaxOffset = new Vector2(-5, 0);
		
		public Vector2 FallMaxScale = new Vector2(0f, 0.6f);
		public float FallBaseScale = 1000f;

		public Vector2 SleepMaxScale = new Vector2(0f, -0.1f);

		public bool Sleeping = false;

		public AutoAlarm SleepParticleAlarm;
		public double SleepParticleSpawnTime = 1;

		#endregion Animations.

		public StackableActorComponent()
		{
			Visible = true;
		}
	}
}
