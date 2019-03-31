using System;
using System.Collections.Generic;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;
using Microsoft.Xna.Framework;

namespace Monofoxe.Demo.GameLogic.Entities
{
	public class PlayerSystem : BaseSystem
	{

		public override Type ComponentType => typeof(PlayerComponent);

		public override int Priority => 1;

		public override void Create(Component component)
		{
			var player = (PlayerComponent)component;

			var cam = new GameCamera(player.Owner.Layer, ScreenController.MainCamera);
			cam.Target = player.Owner.GetComponent<PositionComponent>();
		}

		public override void Update(List<Component> components)
		{
			foreach(PlayerComponent player in components)
			{
				var physics = player.Owner.GetComponent<PhysicsComponent>();
				var position = player.Owner.GetComponent<PositionComponent>();
				var actor = player.Owner.GetComponent<StackableActorComponent>();

				
				// If player is crouching or dead, he can't be killed or damaged.
				var playerState = actor.LogicStateMachine.CurrentState;

				actor.LeftAction = Input.CheckButton(player.Left);
				actor.RightAction = Input.CheckButton(player.Right);
				actor.JumpAction = Input.CheckButton(player.Jump);
				actor.CrouchAction = Input.CheckButton(player.Crouch);

				if (
					actor.LogicStateMachine.CurrentState == ActorStates.Dead 
					&& GameplayController.GUILayer.CountEntities<LevelRestartEffect>() == 0
				)
				{ 
					new LevelRestartEffect(GameplayController.GUILayer);
				}
				

			}
		}
		
		
	}
}
