using System.Globalization;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;
using System;

namespace Monofoxe.Demo.MapEntityFactories
{
	public class CannonFactory : ITiledEntityFactory
	{
		public string Tag => "cannon";

		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var tile = (TiledTileObject)obj;
			
			// Origin point for tile object is in the bottom left corner,
			// but rotation point is in the center.
			var matrix = Matrix.CreateTranslation(new Vector3(Spikes.Size, -Spikes.Size, 0) / 2) *
				Matrix.CreateRotationZ(MathHelper.ToRadians(tile.Rotation));	
			
			var position = new Vector2(matrix.Translation.X, matrix.Translation.Y) + tile.Position;
			
			var cannon = new Cannon(
				position, 
				(Cannon.ShootingMode)Enum.Parse(typeof(Cannon.ShootingMode), tile.Properties["shootingMode"]),
				tile.Rotation, 
				float.Parse(tile.Properties["baseRotation"], CultureInfo.InvariantCulture),
				float.Parse(tile.Properties["firePeriod"], CultureInfo.InvariantCulture), 
				float.Parse(tile.Properties["initialDelay"], CultureInfo.InvariantCulture), 
				layer
			);

			
			if (tile.Properties["link_trigger"] != "none")
			{
				cannon.AddComponent(new LinkComponent(tile.Properties["link_trigger"]));
			}

			return cannon;
		}
	}
}
