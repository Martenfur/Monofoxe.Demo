using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.ECS;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Engine.SceneSystem;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class PathSystem : BaseSystem
	{
		public override Type ComponentType => typeof(PathComponent);

		public override void Create(Component component)
		{
			var path = (PathComponent)component;
			
			ResetPath(path);

			path.Speed = 64;
			path.Looped = false;

			path.Points = new List<Vector2>();
			path.Points.Add(new Vector2(0, 0));
			path.Points.Add(new Vector2(100, 100));
			path.Points.Add(new Vector2(100, 200));
			path.Points.Add(new Vector2(300, 150));


		}

		public override void Update(List<Component> components)
		{
			foreach(PathComponent path in components)
			{
				path.PointProgress += TimeKeeper.GlobalTime(path.Speed);

				var l = BoneLength(path, path.PointID);
				if (path.PointProgress > l)
				{
					path.PointID += 1;
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
				}

				if (path.PointProgress < 0)
				{
					path.PointID -= 1;

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
				}


			}
		}

		public override void Draw(Component component)
		{
			var path = (PathComponent)component;
			
			DrawMgr.CurrentColor = Color.Red;
			
			for(var i = 0; i < path.Points.Count - 1; i += 1)
			{
				DrawMgr.DrawLine(path.Position + path.Points[i], path.Position + path.Points[i + 1]);
				/*if (path.PointID == i)
				{
					var v = (path.Points[i + 1] - path.Points[i]).GetSafeNormalize();
					
					DrawMgr.DrawCircle(path.Position + path.Points[i] + v * (float)path.PointProgress, 4, false);
				}*/
			}
			DrawMgr.DrawCircle(GetCurrentPosition(path), 4, false);

		}


		public float BoneLength(PathComponent path, int index)
		{
			if (path.Looped && index == path.Points.Count - 1)
			{
				return (path.Points[0] - path.Points[path.Points.Count - 1]).Length();
			}
			return (path.Points[index] - path.Points[index + 1]).Length();
		}
		

		public void ResetPath(PathComponent path)
		{
			path.PointProgress = 0;
			path.PointID = 0;

		}

		
		public static Vector2 GetCurrentPosition(PathComponent path)
		{
			var position = path.Position + path.Points[path.PointID];
			
			var v = (path.Points[path.PointID + 1] - path.Points[path.PointID]).GetSafeNormalize();
			position += v * (float)path.PointProgress;	
			return position;
		}

		public static double GetPathLength(PathComponent path)
		{
			var length = 0.0;

			for(var i = 0; i < path.Points.Count - 1; i += 1)
			{
				length += (path.Points[i] - path.Points[i + 1]).Length();
			}

			if (path.Looped)
			{
				length += (path.Points[0] - path.Points[path.Points.Count - 1]).Length();
			}

			return length;
		}

	}
}
