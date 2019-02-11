using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.JsonConverters;
using Monofoxe.Engine.ECS;
using Newtonsoft.Json;

namespace Monofoxe.Demo.GameLogic.Entities
{
	public class SolidComponent : Component
	{
		[JsonConverter(typeof(ColliderConverter))]
		public ICollider Collider;
		
		public Vector2 Speed;

		public override object Clone()
		{
			var component = new SolidComponent();
			component.Collider = (ICollider)Collider.Clone();
			
			return component;
		}
	}
}
