using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;

namespace Monofoxe.Demo.MapEntityFactories
{
	public class PlayerFactory : ITiledEntityFactory
	{
		public string Tag => "player";

		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var tile = (TiledTileObject)obj;
			System.Console.WriteLine("Spawned player!");
			var entity = EntityMgr.CreateEntityFromTemplate(layer, "Player");
			entity.GetComponent<PositionComponent>().Position = tile.Position;
			
			return entity;
		}
	}
}
