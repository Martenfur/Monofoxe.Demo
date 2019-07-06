using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;


namespace Monofoxe.Demo.MapEntityFactories
{
	public class BoothFactory : ITiledEntityFactory
	{
		public string Tag => "booth";

		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var tile = (TiledTileObject)obj;
			
			var gato = Entity.CreateFromTemplate(layer, "CatEnemy");

			var booth = new Booth(tile.Position, gato, layer);
			
			return booth;
		}
	}
}
