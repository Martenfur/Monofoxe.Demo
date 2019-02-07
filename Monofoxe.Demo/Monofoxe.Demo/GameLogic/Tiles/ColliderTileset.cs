﻿using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.Utils.Tilemaps;

namespace Monofoxe.Demo.GameLogic.Tiles
{
	public class ColliderTileset : Tileset
	{
		private ICollider[] _colliders;

		public ColliderTileset(Sprite tiles, ICollider[] colliders, int startingIndex = 1) : base(tiles, startingIndex)
		{
			_colliders = colliders;
		}

		public ICollider GetCollider(int index)
		{
			if (Tiles == null || index < StartingIndex || index >= StartingIndex + Tiles.Frames.Length)
			{
				return null;
			}
			return _colliders[index - StartingIndex];
		}
	}
}
