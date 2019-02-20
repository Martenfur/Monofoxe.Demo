using Monofoxe.Engine;
using Monofoxe.Engine.ECS;

namespace Monofoxe.Demo.GameLogic.Entities
{
	public class PlayerComponent : Component
	{
		public Buttons Left = Buttons.A;
		public Buttons Right = Buttons.D;
		public Buttons Jump = Buttons.W;
		public Buttons Crouch = Buttons.S;

		public override object Clone()
		{
			var c = new PlayerComponent();
			
			return c;
		}
	}
}
