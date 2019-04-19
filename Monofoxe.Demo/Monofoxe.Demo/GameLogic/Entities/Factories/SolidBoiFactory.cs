using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;

namespace Monofoxe.Demo.GameLogic.Entities.Factories
{
	public class SolidBoiFactory : IEntityFactory
	{
		public string Tag => "SolidBoid";

		public Entity Make(Layer layer)
		{
			var entity = new Entity(layer, Tag);

			entity.AddComponent(new PositionComponent(Vector2.Zero));
			entity.AddComponent(
				new SolidComponent
				{
					Collider = new RectangleCollider
					{
						Size = new Vector2(64, 64)
					}
				}
			);

			return entity;
		}
	}
}
