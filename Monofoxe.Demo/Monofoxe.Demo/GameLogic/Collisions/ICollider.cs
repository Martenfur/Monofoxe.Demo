using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monofoxe.Demo.GameLogic.Collisions
{
	public interface ICollider : ICloneable
	{
		ColliderType ColliderType {get;}
		Vector2 Position {get; set;}
		Vector2 PreviousPosition {get; set;}
		Vector2 Size {get; set;}
	}
}
