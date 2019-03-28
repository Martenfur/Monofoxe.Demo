using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;

namespace Monofoxe.Demo.MapEntityFactories
{
	public class SwitchblockFactory : ITiledEntityFactory
	{
		public string Tag => "switchblock";

		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var tile = (TiledTileObject)obj;
			
			var position = new Vector2(Switchblock.Size, -Switchblock.Size) / 2 + tile.Position;
			
			var switchblock = new Switchblock(position, tile.Properties["active"] == "true", layer);
	
			if (tile.Properties["link_trigger"] != "none")
			{
				switchblock.AddComponent(new LinkComponent(tile.Properties["link_trigger"]));
			}
			
			return switchblock;
		}
	}
}
