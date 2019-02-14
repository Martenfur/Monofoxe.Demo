using Monofoxe.Engine.ECS;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class StackableActorComponent : Component
	{
		public float WalkSpeed = 600;
		public float JumpSpeed = 700;


		public float GroundAccelTime = 0.1f; // sec
		public float GroundDecelTime = 0.05f; // sec
		public float AirAccelTime = 0.15f; // sec
		public float AirDecelTime = 1f; // sec
		
		public float JumpGravity = 1500;
		public float FallGravity = 4000;


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
