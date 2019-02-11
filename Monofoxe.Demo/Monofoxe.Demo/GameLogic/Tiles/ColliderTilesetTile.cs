using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.Utils.Tilemaps;

namespace Monofoxe.Demo.GameLogic.Tiles
{
	public class ColliderTilesetTile : ITilesetTile
	{
		public Frame Frame {get; private set;}
		public TilesetTileCollisionMode CollisionMode {get; private set;}
		public ICollider Collider {get; private set;}
		public Vector2 ColliderOffset;
		

		public ColliderTilesetTile(Frame frame, TilesetTileCollisionMode collisionMode, ICollider collider, Vector2 colliderOffset)
		{
			Frame = frame;
			CollisionMode = collisionMode;
			Collider = collider;
			ColliderOffset = colliderOffset;
		}

		public ColliderTilesetTile(Frame frame, TilesetTileCollisionMode collisionMode)
		{
			Frame = frame;
			CollisionMode = collisionMode;
			Collider = null;
			ColliderOffset = Vector2.Zero;
		}
	}
}
