using System;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;

namespace Monofoxe.Demo.MapEntityFactories
{
	public class WatermeonSpawnerFactory : ITiledEntityFactory
	{
		public string Tag => "watermelonSpawner";
	
		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var tile = (TiledTileObject)obj;
			

			var entity = new WatermelonSpawner(
				layer, 
				tile.Position, 
				(SpawnMode)Enum.Parse(typeof(SpawnMode), tile.Properties["spawnMode"])
			);
			
			entity.GetComponent<PositionComponent>().Position += new Vector2(entity.StemSprite.Width, 0);

			return entity;
		}
	}
}
