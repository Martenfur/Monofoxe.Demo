using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.JsonConverters;
using Monofoxe.Engine.Converters;
using Monofoxe.Engine.ECS;
using Newtonsoft.Json;

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
		[JsonConverter(typeof(Vector2Converter))]
		public Vector2 Speed;
		
		/// <summary>
		/// Entity will move once per frame according to this vector. Measured in px.
		/// NOTE: This is a variable exclusive for PhysicsSystem usage. 
		/// Trying to set it yourself will give no result.
		/// </summary>
		public Vector2 PosAdd;
		
		/// <summary>
		/// Collider, which collides with solids.
		/// Currently only RectangleCollider is supported,
		/// others will provide unknown results.
		/// </summary>
		[JsonConverter(typeof(ColliderConverter))]
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
		public float MaxFallSpeed = 1800;


		public override object Clone()
		{
			var physicsComponent = new PhysicsComponent();
			physicsComponent.Speed = Speed;
			physicsComponent.Collider = (ICollider)Collider.Clone();
			physicsComponent.Gravity = Gravity;
			physicsComponent.MaxFallSpeed = MaxFallSpeed;

			return physicsComponent;
		}
	}
}
