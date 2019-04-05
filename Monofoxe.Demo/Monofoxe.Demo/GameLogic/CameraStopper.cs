using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;


namespace Monofoxe.Demo.GameLogic
{
	public class CameraStopper : Entity
	{
		public new string Tag => "cameraStopper";
		
		public CameraStopper(Vector2 position, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));
			
			Visible = false;
		}
	}
}
