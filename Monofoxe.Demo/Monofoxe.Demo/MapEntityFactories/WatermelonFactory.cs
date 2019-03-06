using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;

namespace Monofoxe.Demo.MapEntityFactories
{
	public class WatermeonFactory : ITiledEntityFactory
	{
		public string Tag => "watermelon";
	
		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var point = (TiledTileObject)obj;
			
			var entity = EntityMgr.CreateEntityFromTemplate(layer, Tag);
			entity.GetComponent<PositionComponent>().Position = point.Position;
			
			return entity;
		}
	}
}
