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
	public class Cannon : Entity
	{
		public new string Tag => "cannon";
		
		public static readonly float Size = 48;

		private float _rotation;

		private Vector2 _harmVector;

		public Cannon(Vector2 position, float rotation, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));
			
			var solid = new SolidComponent();
			
			var collider = new RectangleCollider();
			collider.Size = Vector2.One * Size;
			collider.Position = position;

			solid.Collider = collider;

			AddComponent(solid);

			_rotation = rotation;

			var rotationRad = MathHelper.ToRadians(rotation - 90);
			_harmVector = new Vector2(
				Math.Sign((int)(Math.Cos(rotationRad) * 100)), // Double will be a tiny value instead of zero, so we need this.
				Math.Sign((int)(Math.Sin(rotationRad) * 100))
			);
		}

		public override void Update()
		{
			
		}

		
		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();

			DrawMgr.DrawSprite(Resources.Sprites.Default.Cannon, 0, position.Position, Vector2.One, _rotation, Color.White);
		}
	}
}
