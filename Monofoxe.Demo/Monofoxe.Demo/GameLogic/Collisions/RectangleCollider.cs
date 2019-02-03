using System;
using Microsoft.Xna.Framework;

namespace Monofoxe.Demo.GameLogic.Collisions
{
	public class RectangleCollider : ICollider
	{
		public ColliderType ColliderType => ColliderType.Rectangle;
		public Vector2 Position {get; set;}
		public Vector2 Speed {get; set;}

		public Vector2 Size;
	}
}
