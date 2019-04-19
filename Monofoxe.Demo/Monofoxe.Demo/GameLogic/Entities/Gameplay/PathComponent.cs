using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Engine.ECS;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class PathComponent : Component
	{
		public List<Vector2> Points;	

		public double PointProgress;
		public int PointID;

		public Vector2 Position;

		public double Speed;

		public bool Looped = false;

		public float DotSpacing = 48f;

		public PathComponent()
		{
			Visible = true;
		}
	}
}
