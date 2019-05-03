using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;

namespace Monofoxe.Demo.GameLogic.Entities.Templates
{
	public class PlatformTemplate : IEntityTemplate
	{
		public string Tag => "Platofrm";

		public Entity Make(Layer layer)
		{
			var entity = new Entity(layer, Tag);

			entity.AddComponent(new PositionComponent(Vector2.Zero));
			entity.AddComponent(
				new SolidComponent
				{
					Collider = new PlatformCollider
					{
						Size = new Vector2(32, 32)
					}
				}
			);


			return entity;
		}
	}
}
