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
		[JsonConverter(typeof(Vector2Converter))]
		public Vector2 Speed;
		
		public Color Color = Color.Black;

		[JsonConverter(typeof(ColliderConverter))]
		public ICollider Collider;

		public override object Clone()
		{
			var physicsComponent = new PhysicsComponent();
			physicsComponent.Speed = Speed;
			physicsComponent.Collider = (ICollider)Collider.Clone();

			return physicsComponent;
		}
	}
}
