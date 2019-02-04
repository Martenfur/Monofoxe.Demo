﻿using System;
using Microsoft.Xna.Framework;

namespace Monofoxe.Demo.GameLogic.Collisions
{
	public struct PlatformCollider : ICollider
	{
		public ColliderType ColliderType => ColliderType.Platform;
		public Vector2 Position {get; set;}
		public Vector2 PreviousPosition {get; set;}

		public Vector2 Size {get;set;}

		public object Clone() => this;
		

	}
}