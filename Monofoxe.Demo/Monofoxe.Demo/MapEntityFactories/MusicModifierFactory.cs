using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;
using System.Globalization;

namespace Monofoxe.Demo.MapEntityFactories
{
	public class MusicModifierFactory : ITiledEntityFactory
	{
		public string Tag => "musicModifier";

		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var rectangle = (TiledRectangleObject)obj;
			
			return new MusicModifier(
				rectangle.Position + rectangle.Size / 2, 
				rectangle.Size, 
				layer,
				obj.Properties["soundLayer"],
				float.Parse(obj.Properties["transitionSpeed"], CultureInfo.InvariantCulture),
				float.Parse(obj.Properties["transitionValue"], CultureInfo.InvariantCulture)
			);
		}
	}
}
