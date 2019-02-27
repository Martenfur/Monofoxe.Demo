using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using System.Globalization;
using System.Collections.Generic;

namespace Monofoxe.Demo.MapEntityFactories
{
	public class MovingPlatformFactory : ITiledEntityFactory
	{
		public string Tag => "MovingPlatform";

		public Entity Make(TiledObject obj, Layer layer, Map map)
		{
			var poly = (TiledPolygonObject)obj;

			var width = float.Parse(poly.Properties["width"], CultureInfo.InvariantCulture);
			var speed = float.Parse(poly.Properties["speed"], CultureInfo.InvariantCulture);
			
			var entity = new MovingPlatofrm(layer, poly.Position, width, poly.Closed, speed, new List<Vector2>(poly.Points));
			
			return entity;
		}
	}
}
