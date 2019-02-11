using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.JsonConverters;
using Monofoxe.Engine.Converters;
using Monofoxe.Engine.ECS;
using Newtonsoft.Json;

namespace Monofoxe.Demo.GameLogic.Entities
{
	public class PhysicsComponent : Component
	{
		/// <summary>
		/// 
		/// </summary>
		[JsonConverter(typeof(Vector2Converter))]
		public Vector2 Speed;
		
		public Vector2 PosAdd;
		
		public Color Color = Color.Black;

		[JsonConverter(typeof(ColliderConverter))]
		public ICollider Collider;

		public bool InAir;


		public float Gravity = 1800;

		public float MaxFallSpeed = 1800; // px/sec

		public override object Clone()
		{
			var physicsComponent = new PhysicsComponent();
			physicsComponent.Speed = Speed;
			physicsComponent.Collider = (ICollider)Collider.Clone();

			return physicsComponent;
		}
	}
}
