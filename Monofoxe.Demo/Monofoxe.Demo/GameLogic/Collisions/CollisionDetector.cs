using System;
using System.Collections.Generic;
using Monofoxe.Engine.Utils;
using Microsoft.Xna.Framework;

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
			 
			_collisionMatrix = new Func<ICollider, ICollider, bool>[2, 2];
			
			_collisionMatrix[
				(int)ColliderType.Rectangle, 
				(int)ColliderType.Rectangle
			] = RectangleRectangle;

			_collisionMatrix[
				(int)ColliderType.Rectangle, 
				(int)ColliderType.Platform
			] = RectanglePlatform;
		}

		public static bool CheckCollision(ICollider collider1, ICollider collider2)
		{
			var id1 = (int)collider1.ColliderType;
			var id2 = (int)collider2.ColliderType;

			if (id2 > id1) // Only upper half of matrix is being used.
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


			return false;
		}

	}
}
