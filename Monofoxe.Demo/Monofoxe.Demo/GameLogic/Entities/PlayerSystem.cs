using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo.GameLogic.Entities
{
	public class PlayerSystem : BaseSystem
	{

		public override Type ComponentType => typeof(PlayerComponent);

		public override int Priority => 1;

		public override void Create(Component component)
		{
			var player = (PlayerComponent)component;

			var cam = new GameCamera(player.Owner.Layer, Test.Camera);
			cam.Target = player.Owner.GetComponent<PositionComponent>();
		}

		public override void Update(List<Component> components)
		{
			foreach(PlayerComponent player in components)
			{
				var physics = player.Owner.GetComponent<PhysicsComponent>();
				var position = player.Owner.GetComponent<PositionComponent>();
				var actor = player.Owner.GetComponent<StackableActorComponent>();

				actor.LeftAction = Input.CheckButton(player.Left);
				actor.RightAction = Input.CheckButton(player.Right);
				actor.JumpAction = Input.CheckButton(player.Jump);
				actor.CrouchAction = Input.CheckButton(player.Crouch);

				//Test.Camera.Position = player.Owner.GetComponent<PositionComponent>().Position.Round()
				
			}

			if (Input.CheckButtonPress(Buttons.MouseLeft))
			{
				Console.WriteLine(SceneMgr.CurrentLayer.Name);
				var dummy = EntityMgr.CreateEntityFromTemplate(SceneMgr.CurrentLayer, "Dummy");
				var position = dummy.GetComponent<PositionComponent>();
				position.Position = Test.Camera.GetRelativeMousePosition();
				var actor = dummy.GetComponent<StackableActorComponent>();
				ComponentMgr.InitComponent(actor);
			}
		}
		
	}
}
