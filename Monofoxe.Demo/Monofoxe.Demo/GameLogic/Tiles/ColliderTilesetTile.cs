using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.Utils.Tilemaps;
using Monofoxe.Demo.GameLogic.Collisions;
using Microsoft.Xna.Framework;

namespace Monofoxe.Demo.GameLogic.Tiles
{
	public class ColliderTilesetTile : ITilesetTile
	{
		public Frame Frame {get; private set;}
		public ICollider Collider {get; private set;}
		public TilesetTileCollisionType CollisionType {get; private set;}
		public Vector2 ColliderOffset;
		

		public ColliderTilesetTile(Frame frame, ICollider collider, TilesetTileCollisionType collisionType, Vector2 colliderOffset)
		{
			Frame = frame;
			Collider = collider;
			CollisionType = collisionType;
			ColliderOffset = colliderOffset;
		}
	}

	// TODO: Rename.
	public enum TilesetTileCollisionType
	{
		None = 0, // Tile isn't solid.
		Solid = 1, // Tile is solid.
		Custom = 2, // Tile uses custom collider.
	}

}
