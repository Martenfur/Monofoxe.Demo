using Monofoxe.Engine;
using Monofoxe.Engine.ECS;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class StackableActorComponent : Component
	{
		public float WalkSpeed = 500;
		public float JumpSpeed = 800;

		public bool LeftAction;
		public bool RightAction;
		public bool JumpAction;


		public override object Clone()
		{
			var c = new StackableActorComponent();
			
			return c;
		}
	}
}
