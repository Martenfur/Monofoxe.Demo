using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;

namespace Monofoxe.Demo.MapEntityFactories
{
	public static class ActorBaseFactory
	{
		public static Entity Make(TiledObject obj, Layer layer, MapBuilder map, string tag)
		{
			var tile = (TiledTileObject)obj;
			
			var entity = EntityMgr.CreateEntityFromTemplate(layer, tag);
			
			var position = entity.GetComponent<PositionComponent>();
			var actor = entity.GetComponent<StackableActorComponent>();

			position.Position = tile.Position 
				+ new Vector2(actor.MainSprite.Width, -actor.MainSprite.Height) / 2;
			
			ComponentMgr.InitComponent(actor);

			return entity;
		}
	}
}
