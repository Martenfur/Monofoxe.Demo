using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
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
			var tile = (TiledTileObject)obj;
			System.Console.WriteLine("Spawned player!");
			var entity = EntityMgr.CreateEntityFromTemplate(layer, "Player");
			
			var position = entity.GetComponent<PositionComponent>();
			var actor = entity.GetComponent<StackableActorComponent>();

			position.Position = tile.Position 
				+ new Vector2(actor.MainSprite.Width, -actor.MainSprite.Height) / 2;


			return entity;
		}
	}
}
