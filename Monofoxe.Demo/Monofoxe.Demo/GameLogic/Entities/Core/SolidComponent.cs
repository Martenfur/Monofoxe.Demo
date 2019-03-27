using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.JsonConverters;
using Monofoxe.Engine.ECS;
using Newtonsoft.Json;

namespace Monofoxe.Demo.GameLogic.Entities.Core
{
	/// <summary>
	/// Solid component which stops physics comopnents from passing through it.
	/// 
	/// Dependent on:
	///		PositionComponent
	/// </summary>
	public class SolidComponent : Component
	{
		[JsonConverter(typeof(ColliderConverter))]
		public ICollider Collider;
		
		/// <summary>
		/// Entity will move according to this vector. Measured in px/sec.
		/// </summary>
		public Vector2 Speed;
		

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
		/// Entity that physics object has collided with horizontally.
		/// </summary>
		public Entity CollidedObjectH;

		/// <summary>
		/// Entity that physics object has collided with vertically.
		/// </summary>
		public Entity CollidedObjectV;
		


		public override object Clone()
		{
			var c = new SolidComponent();
			c.Collider = (ICollider)Collider.Clone();
			c.Speed = Speed;

			return c;
		}
	}
}
