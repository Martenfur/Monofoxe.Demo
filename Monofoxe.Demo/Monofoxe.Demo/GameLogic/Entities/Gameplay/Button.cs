using Microsoft.Xna.Framework;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Collisions;
using System;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class Button : Entity
	{
		public new string Tag => "button";
		
		public static readonly Vector2 Size = new Vector2(48, 8);

		public bool IsDown {get; private set;} = false;
		public bool IsDownPrevious {get; private set;} = false;
		
		public bool Pressed => IsDown && !IsDownPrevious;
		public bool Released => !IsDown && IsDownPrevious;

		private ICollider _collider;

		private float _rotation;

		public Button(Vector2 position, float rotation, Layer layer) : base(layer)
		{
			_rotation = rotation;

			AddComponent(new PositionComponent(position));
			
			_collider = new RectangleCollider();

			var rotationRad = MathHelper.ToRadians(rotation - 90);
			var rotationVector = new Vector2(
				Math.Sign((int)(Math.Cos(rotationRad) * 100)), // Double will be a tiny value instead of zero, so we need this.
				Math.Sign((int)(Math.Sin(rotationRad) * 100))
			);

			if (rotationVector.X != 0)
			{
				_collider.Size = new Vector2(Size.Y, Size.X);
			}
			else
			{
				_collider.Size = Size;	
			}
			_collider.Position = position + rotationVector * Size.Y / 2;
		}

		public override void Update()
		{
			IsDownPrevious = IsDown;
			IsDown = false;
			foreach(var actorEntity in Scene.GetEntityListByComponent<StackableActorComponent>())
			{
				var physics = actorEntity.GetComponent<PhysicsComponent>();
				var position = actorEntity.GetComponent<PositionComponent>();

				physics.Collider.Position = position.Position;
				physics.Collider.PreviousPosition = position.PreviousPosition;

				if (CollisionDetector.CheckCollision(_collider, physics.Collider))
				{
					IsDown = true;
					break;
				}

			}
		}

		
		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();
			
			Resources.Sprites.Default.Button.Draw(
				IsDown.ToInt(), 
				position.Position,
				Resources.Sprites.Default.Button.Origin,
				Vector2.One, 
				_rotation, 
				Color.White
			);
		}
		
	}
}
