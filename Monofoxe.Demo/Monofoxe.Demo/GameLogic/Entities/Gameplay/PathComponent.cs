using System;
using System.Collections.Generic;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.Drawing;
using Microsoft.Xna.Framework;

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

		public PathComponent()
		{
			Visible = true; // TODO: remove
		}

		public override object Clone()
		{
			throw new NotImplementedException();
		}
	}
}
