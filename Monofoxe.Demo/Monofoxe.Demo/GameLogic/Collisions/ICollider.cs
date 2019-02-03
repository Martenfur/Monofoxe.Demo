using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monofoxe.Demo.GameLogic.Collisions
{
	public interface ICollider
	{
		ColliderType ColliderType {get;}
		Vector2 Position {get; set;}
		Vector2 Speed {get; set;}
	}
}
