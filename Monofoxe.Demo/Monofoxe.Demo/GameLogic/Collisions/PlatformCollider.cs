using System;
using Microsoft.Xna.Framework;

namespace Monofoxe.Demo.GameLogic.Collisions
{
	public class PlatformCollider : ICollider
	{
		public ColliderType ColliderType => ColliderType.Platform;
		public Vector2 Position {get; set;}
		public Vector2 Speed {get; set;}

		public Vector2 Size;
	}
}
