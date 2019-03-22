using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;


namespace Monofoxe.Demo.MapEntityFactories
{
	public class CheckpointFactory : ITiledEntityFactory
	{
		public string Tag => "checkpoint";

		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var tile = (TiledTileObject)obj;
			
			return new Checkpoint(tile.Position, layer);
		}
	}
}
