using Monofoxe.Engine;
using Monofoxe.Engine.ECS;

namespace Monofoxe.Demo.GameLogic.Entities
{
	public class PlayerComponent : Component
	{
		public Buttons Left = Buttons.A;
		public Buttons Right = Buttons.D;
		public Buttons Jump = Buttons.W;

		public float WalkSpeed = 500;
		public float JumpSpeed = 800;

		public override object Clone()
		{
			var playerComponent = new PlayerComponent();
			
			return playerComponent;
		}
	}
}
