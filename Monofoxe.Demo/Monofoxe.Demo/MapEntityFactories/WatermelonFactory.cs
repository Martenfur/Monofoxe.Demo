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
			return ActorBaseFactory.Make(obj, layer, map, Tag);
		}
	}
}
