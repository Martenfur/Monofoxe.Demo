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
			return ActorBaseFactory.Make(obj, layer, map, Tag);
		}
	}
}
