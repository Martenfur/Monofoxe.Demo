using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;


namespace Monofoxe.Demo.GameLogic.Entities
{
	public class PlayerSystem : BaseSystem
	{
		public override Type ComponentType => typeof(PlayerComponent);

		public override int Priority => 1;

		
		public override void Update(List<Component> components)
		{
			foreach(PlayerComponent player in components)
			{
				var physics = player.Owner.GetComponent<PhysicsComponent>();
				var position = player.Owner.GetComponent<PositionComponent>();
				var actor = player.Owner.GetComponent<StackableActorComponent>();

				
				actor.LeftAction = Input.CheckButton(player.Left);
				actor.RightAction = Input.CheckButton(player.Right);
				actor.JumpAction = Input.CheckButtonPress(player.Jump);

				Test.Camera.Position = player.Owner.GetComponent<PositionComponent>().Position.ToPoint().ToVector2() 
				- Test.Camera.Size / 2;
			
			}
			
		}
		
	}
}
