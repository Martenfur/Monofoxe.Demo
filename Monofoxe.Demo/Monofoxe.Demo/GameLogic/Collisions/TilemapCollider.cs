﻿using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Tiles;
using Monofoxe.Engine.Utils.Tilemaps;

namespace Monofoxe.Demo.GameLogic.Collisions
{
	/// <summary>
	/// Collider based on a tilemap.
	/// </summary>
	public class TilemapCollider : ICollider
	{
		public ColliderType ColliderType => ColliderType.Tilemap;
		public Vector2 Position {get; set;}
		public Vector2 PreviousPosition {get; set;}
		public BasicTilemapComponent Tilemap;

		public Vector2 Size {get; set;}

		public object Clone() => this; // Not cloning, but this doesn't really matter.
	}
}
