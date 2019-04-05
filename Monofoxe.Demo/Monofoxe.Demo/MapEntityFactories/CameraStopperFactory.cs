using Monofoxe.Demo.GameLogic;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;


namespace Monofoxe.Demo.MapEntityFactories
{
	public class CameraStopperFactory : ITiledEntityFactory
	{
		public string Tag => "cameraStopper";

		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var point = (TiledPointObject)obj;
			
			return new CameraStopper(point.Position, layer);
		}
	}
}
