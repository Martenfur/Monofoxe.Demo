using System;
using System.Collections.Generic;
using Monofoxe.Engine.ECS;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Monofoxe.Engine.Converters;
using Monofoxe.Engine;
using Monofoxe.Demo.GameLogic.Collisions;

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
