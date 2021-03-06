﻿using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Engine.ECS;

namespace Monofoxe.Demo.GameLogic.Entities.Core
{
	/// <summary>
	/// Basic physics component, which can collide with solids and be affected by gravity.
	/// 
	/// Dependent on:
	///		PositionComponent
	/// </summary>
	public class PhysicsComponent : Component
	{
		/// <summary>
		/// Entity will move according to this vector. Measured in px/sec.
		/// </summary>
		public Vector2 Speed;
		
		/// <summary>
		/// Entity will move once per frame according to this vector. Measured in px.
		/// NOTE: This is a variable exclusive for PhysicsSystem usage. 
		/// Trying to set it yourself will give no result.
		/// </summary>
		public Vector2 OneFrameMovement;
		
		/// <summary>
		/// Collider, which collides with solids.
		/// Currently only RectangleCollider is supported,
		/// others will provide unknown results.
		/// </summary>
		public ICollider Collider;

		/// <summary>
		/// True if entity is currently in air, 
		/// false is it stays on something solid.
		/// </summary>
		public bool InAir;

		/// <summary>
		/// Downward acceleration. Measured in px/(sec*sec).
		/// </summary>
		public float Gravity = 1800;

		/// <summary>
		/// If vertical speed is lerger than this value, gravity won't accelerate entity anymore.
		/// Measured in px/sec.
		/// </summary>
		public float MaxFallSpeed = 1000;


		/// <summary>
		/// Tells, if a horizontal collision has occured.
		/// 0 - no collision
		/// -1 - right collision
		/// 1 - left collision
		/// </summary>
		public int CollisionH;

		/// <summary>
		/// Tells, if a vertical collision has occured.
		/// 0 - no collision
		/// -1 - top collision
		/// 1 - bottom collision
		/// </summary>
		public int CollisionV;

		/// <summary>
		/// Tells if entity was squashed and couldn't resolve the collision.
		/// </summary>
		public bool Squashed;

		/// <summary>
		/// Entity that physics object has collided with horizontally.
		/// </summary>
		public Entity CollidedSolidH;

		/// <summary>
		/// Entity that physics object has collided with vertically.
		/// </summary>
		public Entity CollidedSolidV;

		/// <summary>
		/// Entity that physics object has collided with.
		/// </summary>
		public Entity StandingOn;
	}
}
