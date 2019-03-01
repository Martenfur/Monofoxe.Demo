using Monofoxe.Engine;
using Monofoxe.Engine.ECS;
using System;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class CatEnemyComponent : Component
	{
		
		public int Direction = 1;
		
		public override object Clone()
		{
			var c = new CatEnemyComponent();

			return c;
		}
	}
}
