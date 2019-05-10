using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;


namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class NextMapTrigger : Entity
	{
		public new string Tag => "nextMapTrigger";
		
		public Vector2 Size;

		private bool _triggered;

		public NextMapTrigger(Vector2 position, Vector2 size, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));

			Size = size;

			Visible = false;

			_triggered = false;
		}

		public override void Update()
		{
			var position = GetComponent<PositionComponent>();

			if (_triggered)
			{
				return;
			}

			foreach(PlayerComponent player in SceneMgr.CurrentScene.GetComponentList<PlayerComponent>())
			{
				var playerPosition = player.Owner.GetComponent<PositionComponent>();
				var playerActor = player.Owner.GetComponent<StackableActorComponent>();
				
				if (
					playerActor.LogicStateMachine.CurrentState != ActorStates.Dead 
					&& GameMath.PointInRectangleBySize(playerPosition.Position, position.Position, Size)
				)
				{
					new LevelRestartEffect(GameplayController.GUILayer, true);
					StackableActorSystem.Kill(playerActor, true);
					_triggered = true;
				}
			}			
		}

		
		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();
			RectangleShape.DrawBySize(position.Position, Size, true);
		}
	}
}
