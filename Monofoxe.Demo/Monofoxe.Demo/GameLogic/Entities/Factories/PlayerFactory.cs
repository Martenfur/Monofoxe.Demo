using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;

namespace Monofoxe.Demo.GameLogic.Entities.Factories
{
	public class PlayerFactory : IEntityFactory
	{
		public string Tag => "Player";

		public Entity Make(Layer layer)
		{
			var entity = new Entity(layer, Tag);

			entity.AddComponent(new PositionComponent(Vector2.Zero));
			entity.AddComponent(
				new PhysicsComponent
				{
					Collider = new RectangleCollider
					{
						Size = new Vector2(32, 32)
					}
				}
			);

			entity.AddComponent(new StackableActorComponent());
			entity.AddComponent(new PlayerComponent());
			
			return entity;
		}
	}
}
