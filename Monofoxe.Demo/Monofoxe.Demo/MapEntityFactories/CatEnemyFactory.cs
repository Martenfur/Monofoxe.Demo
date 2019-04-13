using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;

namespace Monofoxe.Demo.MapEntityFactories
{
	public class CatEnemyFactory : ITiledEntityFactory
	{
		public string Tag => "CatEnemy";
		
		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var gato = ActorBaseFactory.Make(obj, layer, map, Tag);

			var master = gato;
			for(var i = 0; i < int.Parse(obj.Properties["stack"]); i += 1)
			{
				var slave = Entity.CreateFromTemplate(layer, Tag);
				StackableActorSystem.StackEntity(master.GetComponent<StackableActorComponent>(), slave.GetComponent<StackableActorComponent>());
				master = slave;
			}

			return gato;
		}
	}
}
