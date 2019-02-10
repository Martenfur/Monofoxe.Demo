using System;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Tiles;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.Utils.Tilemaps;

namespace Monofoxe.Demo.GameLogic.Collisions
{
	/// <summary>
	/// Takes two colliders of any type and checks if there was a collision between them.
	/// </summary>
	public static class CollisionDetector
	{
		/// <summary>
		/// Matrix contains all allowed collider combinations.
		/// </summary>
		private static Func<ICollider, ICollider, bool>[,] _collisionMatrix;

		private const int _collisionMatrixSize = 3;

		/// <summary>
		/// Initializes collision matrix.
		/// </summary>
		public static void Init()
		{
			/*
			 *   | r  | p 
			 * ------------
			 * r | rr | rp  
			 * ------------
			 * p | xx | xx  
			 */
			 
			_collisionMatrix = new Func<ICollider, ICollider, bool>[_collisionMatrixSize, _collisionMatrixSize];
			
			_collisionMatrix[
				(int)ColliderType.Rectangle, 
				(int)ColliderType.Rectangle
			] = RectangleRectangle;

			_collisionMatrix[
				(int)ColliderType.Rectangle, 
				(int)ColliderType.Platform
			] = RectanglePlatform;

			_collisionMatrix[
				(int)ColliderType.Rectangle, 
				(int)ColliderType.Tilemap
			] = RectangleTilemap;
		}

		/// <summary>
		/// Checks collision between two colliders of any type.
		/// </summary>
		public static bool CheckCollision(ICollider collider1, ICollider collider2)
		{
			/*
			 * Each collider type has its own unique index.
			 * We are taking them and retrieving appropriate
			 * collision function.
			 */
			var id1 = (int)collider1.ColliderType;
			var id2 = (int)collider2.ColliderType;

			// Maybe add a null check here to prevent crashes, if some function isn't implemented.
			// Though, this probably won't be needed.

			if (id2 < id1) // Only upper half of matrix is being used.
			{
				return _collisionMatrix[id2, id1](collider2, collider1);
			}
			return _collisionMatrix[id1, id2](collider1, collider2);
		}


		static bool RectangleRectangle(ICollider collider1, ICollider collider2)
		{
			var rectangle1 = (RectangleCollider)collider1;
			var rectangle2 = (RectangleCollider)collider2;

			return GameMath.RectangleInRectangleBySize(
				rectangle1.Position, 
				rectangle1.Size, 
				rectangle2.Position, 
				rectangle2.Size
			);
		}

		static bool RectanglePlatform(ICollider collider1, ICollider collider2)
		{
			var rectangle = (RectangleCollider)collider1;
			var platform = (PlatformCollider)collider2;

			if ( // If rectangle enters platform from above.
				rectangle.PreviousPosition.Y + rectangle.Size.Y / 2f 
				< platform.PreviousPosition.Y - platform.Size.Y / 2f
				&&
				rectangle.Position.Y + rectangle.Size.Y / 2f
				> platform.Position.Y - platform.Size.Y / 2f
			)
			{
				return GameMath.RectangleInRectangleBySize(
					rectangle.Position, 
					rectangle.Size, 
					platform.Position, 
					platform.Size
				);
			}

			return false;
		}

		static bool RectangleTilemap(ICollider collider1, ICollider collider2)
		{
			var rectangle = (RectangleCollider)collider1;
			var tilemap = (TilemapCollider)collider2;

			if ( // Checking tilemap bounds.
				GameMath.RectangleInRectangleBySize(
					rectangle.Position, 
					rectangle.Size, 
					tilemap.Position, 
					tilemap.Size
				)
			)
			{
				var blockSize = new Vector2(tilemap.Tilemap.TileWidth, tilemap.Tilemap.TileHeight);

				var corner1 = (rectangle.Position - rectangle.Size / 2) / blockSize;
				var corner2 = (rectangle.Position + rectangle.Size / 2) / blockSize;

				// TODO: Add tilemap offset support.

				// Checking region that tile occupies.
				for(var x = (int)corner1.X; x <= (int)corner2.X; x += 1)
				{
					for(var y = (int)corner1.Y; y <= (int)corner2.Y; y += 1)
					{
						var tilemapTile = tilemap.Tilemap.GetTile(x, y);
						if (tilemapTile != null)
						{
							// Actual info about collisions is stored in tiles from tileset.
							var tilesetTile = (ColliderTilesetTile)((BasicTile)tilemapTile).GetTilesetTile();

							// Empty or non-solid tile.
							if (tilesetTile == null || tilesetTile.CollisionType == TilesetTileCollisionType.None)
							{
								continue;
							}
							// Empty or non-solid tile.

							// Fully solid tile.
							if (tilesetTile.CollisionType == TilesetTileCollisionType.Solid)
							{
								return true;
							}
							// Fully solid tile.

							// Tile with custom collider.
							if (tilesetTile.CollisionType == TilesetTileCollisionType.Custom)
							{	
								var tileCollider = tilesetTile.Collider;//tilemapTile.Value.GetCollider();
								
								var colliderOffset = ((ColliderTilesetTile)tilemapTile.Value.GetTilesetTile()).ColliderOffset;
								tileCollider.Position = new Vector2(x, y) * blockSize + colliderOffset; 
								tileCollider.PreviousPosition = tileCollider.Position;
								
								if (CheckCollision(collider1, tileCollider))
								{
									return true; // We don't want to return false until we didn't checked all other tiles.
								}
							}
							// Tile with custom collider.
						}
					}
				}	
			}
			
			return false;
		}

	}
}
