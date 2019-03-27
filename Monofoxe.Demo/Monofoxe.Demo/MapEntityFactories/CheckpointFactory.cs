using Microsoft.Xna.Framework;
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
			
			var position = tile.Position 
				+ Vector2.UnitX * Resources.Sprites.Default.CheckpointPedestal.Width / 2;

			return new Checkpoint(position, layer);
		}
	}
}
