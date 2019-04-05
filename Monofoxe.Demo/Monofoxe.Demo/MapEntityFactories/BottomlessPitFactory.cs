using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;


namespace Monofoxe.Demo.MapEntityFactories
{
	public class BottomlessPitFactory : ITiledEntityFactory
	{
		public string Tag => "bottomlessPit";

		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var rectangle = (TiledRectangleObject)obj;
			
			return new BottomlessPit(rectangle.Position + rectangle.Size / 2, rectangle.Size, layer);
		}
	}
}
