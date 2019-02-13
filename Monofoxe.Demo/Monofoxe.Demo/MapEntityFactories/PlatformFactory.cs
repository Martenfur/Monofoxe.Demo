using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;

namespace Monofoxe.Demo.MapEntityFactories
{
	public class PlatformFactory : ITiledEntityFactory
	{
		public string Tag => "platform";

		public Entity Make(TiledObject obj, Layer layer)
		{
			var rectangle = (TiledRectangleObject)obj;

			var entity = EntityMgr.CreateEntityFromTemplate(layer, "Platform");
			entity.GetComponent<PositionComponent>().Position = rectangle.Position + rectangle.Size / 2f;
			entity.GetComponent<SolidComponent>().Collider.Size = rectangle.Size;

			return entity;
		}
	}
}
