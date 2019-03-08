using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;

namespace Monofoxe.Demo.MapEntityFactories
{
	public class CatEnemyFactory : ITiledEntityFactory
	{
		public string Tag => "CatEnemy";
		
		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var tile = (TiledTileObject)obj;
			
			var entity = EntityMgr.CreateEntityFromTemplate(layer, Tag);
			entity.GetComponent<PositionComponent>().Position = tile.Position;
			
			return entity;
		}
	}
}
