using System;
using System.Collections.Generic;
using Monofoxe.Engine.Utils;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Tiles;

namespace Monofoxe.Demo.GameLogic.Collisions
{
	public static class CollisionDetector
	{
		private static Func<ICollider, ICollider, bool>[,] _collisionMatrix;

		public static void Init()
		{
			/*
			 *   | r  | p 
			 * ------------
			 * r | rr | rp  
			 * ------------
			 * p | xx | xx  
			 */
			 
			_collisionMatrix = new Func<ICollider, ICollider, bool>[3, 3];
			
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

		public static bool CheckCollision(ICollider collider1, ICollider collider2)
		{
			var id1 = (int)collider1.ColliderType;
			var id2 = (int)collider2.ColliderType;

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

			if (
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

			if (
				GameMath.RectangleInRectangleBySize(
					rectangle.Position, 
					rectangle.Size, 
					tilemap.Position, 
					tilemap.Size
				)
			)
			{
				var blockSize = new Vector2(tilemap.Tilemap.TileWidth, tilemap.Tilemap.TileHeight);

				var blockPosition1 = (rectangle.Position - rectangle.Size / 2) / blockSize;
				var blockPosition2 = (rectangle.Position + rectangle.Size / 2) / blockSize;

				for(var x = (int)blockPosition1.X; x <= (int)blockPosition2.X; x += 1)
				{
					for(var y = (int)blockPosition1.Y; y <= (int)blockPosition2.Y; y += 1)
					{
						var tile = tilemap.Tilemap.GetTile(x, y);
						if (tile != null)
						{
							var tilesetTile = (ColliderTilesetTile)((ColliderTile)tile).GetTilesetTile();

							if (tilesetTile == null || tilesetTile.CollisionType == TilesetTileCollisionType.None)
							{
								continue;
							}
							if (tilesetTile.CollisionType == TilesetTileCollisionType.Solid)
							{
								return true;
							}
							if (tilesetTile.CollisionType == TilesetTileCollisionType.Custom)
							{			
								var tileCollider = tile.Value.GetCollider();
								
								tileCollider.Position = new Vector2(x, y) * blockSize 
									+ ((ColliderTilesetTile)tile.Value.GetTilesetTile()).ColliderOffset; 
								tileCollider.PreviousPosition = tileCollider.Position;
								
								if (CheckCollision(collider1, tileCollider))
								{
									return true;
								}
							}
						}
					}
				}	
			}
			
			return false;
		}

	}
}
