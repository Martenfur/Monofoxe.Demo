using Monofoxe.Engine.ECS;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class FrogEnemyComponent : Component
	{
		public int Direction = 1;
		
		public FrogTrigger Trigger;

		public StateMachine<FrogEnemyStates> LogicStateMachine;


		public float CrouchRange = 200;

		public float CrouchMargin = 10;
		
		public float UncrouchRange = 10;
		
		public float CrouchAttackMovementSpeed = 1000;

		public float MinSlideSpeed = 10;

		public float AttackStartX;

		public Alarm AttackDelay;
		public double AttackDelayTime = 0.25;

	}
}
