using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Collisions;
using System;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class Spikes : Entity
	{
		public new string Tag => "spikes";
		
		public static readonly float Size = 48;

		private float _rotation;

		public Spikes(Vector2 position, float rotation, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));
			
			var solid = new SolidComponent();
			
			var collider = new RectangleCollider();
			collider.Size = Vector2.One * Size;
			collider.Position = position;

			solid.Collider = collider;

			AddComponent(solid);

			_rotation = rotation;
		}

		public override void Update()
		{
		
		}

		
		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();

			DrawMgr.DrawSprite(Resources.Sprites.Default.Spikes, 0, position.Position, Vector2.One, _rotation, Color.White);
			
			DrawMgr.CurrentColor = Color.White;
			DrawMgr.DrawLine(
				position.Position, 
				position.Position 
				+ new Vector2(
					(float)Math.Cos(MathHelper.ToRadians(_rotation)),
					(float)Math.Sin(MathHelper.ToRadians(_rotation))
				) * 24
			);

		}
	}
}
