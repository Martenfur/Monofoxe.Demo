﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.Utils;
using Monofoxe.Demo.GameLogic.Entities.Core;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class PathSystem : BaseSystem
	{
		public override Type ComponentType => typeof(PathComponent);

		public override void Create(Component component)
		{
			var path = (PathComponent)component;
			
			ResetPath(path);
		}

		public override void Update(List<Component> components)
		{
			foreach(PathComponent path in components)
			{
				// Updating path.
				path.PointProgress += TimeKeeper.GlobalTime(path.Speed);
				
				var l = BoneLength(path, path.PointID);
				if (path.PointProgress > l)
				{
					path.PointID += 1;
					if (!path.Looped)
					{
						// Non-looped paths.
						if (path.PointID >= path.Points.Count - 1)
						{
							path.Speed *= -1;
							path.PointID -= 1;
							path.PointProgress -= (path.PointProgress - l) * 2;
						}
						else
						{
							path.PointProgress -= l;
						}
						// Non-looped paths.
					}
					else
					{
						// Looped paths.
						if (path.PointID >= path.Points.Count)
						{
							path.PointID = 0;
						}
						path.PointProgress -= l;
						// Looped paths.
					}
				}

				if (path.PointProgress < 0)
				{
					path.PointID -= 1;

					if (!path.Looped)
					{
						// Non-looped paths.
						if (path.PointID < 0)
						{
							path.Speed *= -1;
							path.PointID += 1;
							
							path.PointProgress -= path.PointProgress * 2;
						}
						else
						{
							path.PointProgress += BoneLength(path, path.PointID);
						}
						// Non-looped paths.
					}
					else
					{
						// Looped paths.
						if (path.PointID < 0)
						{
							path.PointID = path.Points.Count - 1;
						}
						path.PointProgress += BoneLength(path, path.PointID);
						// Looped paths.
					}
				}
				// Updating path.


				// Updating owner entity position.
				var position = path.Owner.GetComponent<PositionComponent>();
				var solid = path.Owner.GetComponent<SolidComponent>();
			
				var pathPosition = GetCurrentPosition(path);
				if (TimeKeeper.GlobalTime() != 0)
				{
					solid.Speed = (pathPosition - position.Position) / (float)TimeKeeper.GlobalTime();
				}
				// Updating owner entity position.
			}
		}

		public override void Draw(Component component)
		{
			var path = (PathComponent)component;
			
			GraphicsMgr.CurrentColor = Color.White;
			
			var dot = Resources.Sprites.Default.PlatformDot;

			
			
			for(var i = 0; i < path.Points.Count - (!path.Looped).ToInt(); i += 1)
			{
				var pt1 = path.Points[i];
				var pt2 = GetNextPoint(path, i);

				var distance = GameMath.Distance(pt1, pt2);

				var v = (pt2 - pt1).GetSafeNormalize();

				var dotCount = (int)(distance / path.DotSpacing);

				var adjustedSpacing = distance / dotCount;

				for(var k = 0; k < dotCount; k += 1)
				{
					dot.Draw(path.Position + pt1 + v * k * adjustedSpacing, dot.Origin);
				}
			}

			if (!path.Looped)
			{
				dot.Draw(path.Position + path.Points[path.Points.Count - 1], dot.Origin);
			}
		}


		public float BoneLength(PathComponent path, int index) =>
			(path.Points[index] - GetNextPoint(path, index)).Length();
		

		public void ResetPath(PathComponent path)
		{
			path.PointProgress = 0;
			path.PointID = 0;
		}

		
		public static Vector2 GetCurrentPosition(PathComponent path)
		{
			var position = path.Position + path.Points[path.PointID];
			
			var v = (GetNextPoint(path) - path.Points[path.PointID]).GetSafeNormalize();
			position += v * (float)path.PointProgress;	
			return position;
		}


		/// <summary>
		/// Returns next point of the path.
		/// </summary>
		public static Vector2 GetNextPoint(PathComponent path) =>
			GetNextPoint(path, path.PointID);
		
		/// <summary>
		/// Returns next point of the path's given point.
		/// </summary>
		public static Vector2 GetNextPoint(PathComponent path, int i)
		{
			if (path.Looped && i == path.Points.Count - 1)
			{
				return path.Points[0];
			}
			return path.Points[i + 1];
		}
		
		/// <summary>
		/// Returns length of the full path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static double GetPathLength(PathComponent path)
		{
			var length = 0.0;

			// If path is looped - go through aditional point, which is essentially the loop.
			for(var i = 0; i < path.Points.Count - (!path.Looped).ToInt(); i += 1)
			{
				length += (path.Points[i] - GetNextPoint(path, i)).Length();
			}
			return length;
		}

	}
}
