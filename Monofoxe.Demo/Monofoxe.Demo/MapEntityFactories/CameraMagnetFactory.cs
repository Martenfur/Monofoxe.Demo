using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;

namespace Monofoxe.Demo.MapEntityFactories
{
	public class CameraMagnetFactory : ITiledEntityFactory
	{
		public string Tag => "CameraMagnet";

		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var ellipse = (TiledEllipseObject)obj;
			
			return new CameraMagnet(ellipse.Center, ellipse.Size.X / 2, layer);
		}
	}
}
