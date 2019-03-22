using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;
using Monofoxe.Demo.GameLogic.Entities;
using Monofoxe.Demo.GameLogic.Entities.Core;


namespace Monofoxe.Demo.MapEntityFactories
{
	public class PlayerFactory : ITiledEntityFactory
	{
		public string Tag => "player";

		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var player = ActorBaseFactory.Make(obj, layer, map, Tag);

			// Teleporting player to the checkpoint, if it's set.
			var position = player.GetComponent<PositionComponent>();
			var defaultLayer = SceneMgr.GetScene("default")["default"];

			var checkpointMgr = defaultLayer.FindEntity<CheckpointManager>();
			if (checkpointMgr != null && !checkpointMgr.NoCheckpointSet)
			{
				position.Position = checkpointMgr.CheckpointPosition;
			}

			return player;
		}
	}
}
