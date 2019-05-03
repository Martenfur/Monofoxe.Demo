using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;

namespace Monofoxe.Demo.GameLogic.Entities.Templates
{
	public class CatEnemyTemplate : IEntityTemplate
	{
		public string Tag => "CatEnemy";

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

			entity.AddComponent(
				new StackableActorComponent
				{
					WalkMovementSpeed = 100,
					MainSprite = Resources.Sprites.Default.Gato
				}
			);

			entity.AddComponent(new CatEnemyComponent());
			
			return entity;
		}
	}
}
