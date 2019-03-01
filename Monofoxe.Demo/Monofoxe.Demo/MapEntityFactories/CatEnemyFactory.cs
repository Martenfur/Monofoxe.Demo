using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;

namespace Monofoxe.Demo.MapEntityFactories
{
	public class CatEnemyFactory : ITiledEntityFactory
	{
		public string Tag => "catEnemy";
		public static int counter = 0;

		public Entity Make(TiledObject obj, Layer layer, Map map)
		{
			var point = (TiledPointObject)obj;
			
			var entity = EntityMgr.CreateEntityFromTemplate(layer, Tag);
			entity.GetComponent<PositionComponent>().Position = point.Position;
			counter += 1;
			
			return entity;
		}
	}
}
