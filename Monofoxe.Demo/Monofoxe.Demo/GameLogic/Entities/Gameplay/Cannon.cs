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
	

	public class Cannon : Entity
	{
		public new string Tag => "cannon";
		
		public enum ShootingMode
		{
			Auto,
			Trigger,
		}

		public static readonly float Size = 48;

		private float _rotation;
		private float _baseRotation;


		private Vector2 _direction;

		private Alarm _initialDelayAlarm;
		private AutoAlarm _fireAlarm;
		
		private ShootingMode _mode;

		private Button _myButton;


		private bool _shootingAnimationRunning = false;
		private float _shootingAnimation = 0;
		private float _shootingAnimationProgress = 0;
		private float _shootingAnimationSpeed = 9f;
		private Vector2 _shootingAnimationScale = new Vector2(0.05f, 0.05f);
		private float _shootingAnimationOffset = -10;


		public Cannon(Vector2 position, ShootingMode mode, float rotation, float baseRotation, double firePeriod, double initialDelay, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));
			
			_mode = mode;

			var solid = new SolidComponent();
			
			var collider = new RectangleCollider();
			collider.Size = Vector2.One * Size;
			collider.Position = position;

			solid.Collider = collider;

			AddComponent(solid);

			_rotation = rotation;
			_baseRotation = baseRotation;

			var rotationRad = MathHelper.ToRadians(rotation);
			_direction = new Vector2(
				Math.Sign((int)(Math.Cos(rotationRad) * 100)), // Double will be a tiny value instead of zero, so we need this.
				Math.Sign((int)(Math.Sin(rotationRad) * 100))
			);

			_fireAlarm = new AutoAlarm(firePeriod);
			_initialDelayAlarm = new Alarm();
			_initialDelayAlarm.Set(initialDelay);
		}

		public override void Update()
		{
			if (_mode == ShootingMode.Auto)
			{
				_initialDelayAlarm.Update();

				if (!_initialDelayAlarm.Running && _fireAlarm.Update())
				{
					Shoot();
				}
			}

			if (_mode == ShootingMode.Trigger)
			{
				if (_myButton != null)
				{
					if (_myButton.Pressed)
					{
						Shoot();
					}
				}
				else
				{
					if (TryGetComponent(out LinkComponent link))
					{
						if (link.Pair != null)
						{
							_myButton = (Button)link.Pair.Owner;
							RemoveComponent<LinkComponent>();
						}
					}
				}
			}


			if (_shootingAnimationRunning)
			{
				_shootingAnimationProgress += TimeKeeper.GlobalTime(_shootingAnimationSpeed);

				if (_shootingAnimationProgress > 1)
				{
					_shootingAnimationRunning = false;
					_shootingAnimationProgress = 0;
				}
			}

		}

		
		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();

			_shootingAnimation = (float)Math.Sin(_shootingAnimationProgress * _shootingAnimationProgress * Math.PI);
			
			Resources.Sprites.Default.Cannon.Draw( 
				0, 
				position.Position + _shootingAnimationOffset * _direction * _shootingAnimation, 
				Resources.Sprites.Default.Cannon.Origin,
				Vector2.One + _shootingAnimationScale * _shootingAnimation, 
				_rotation, 
				Color.White
			);

			Resources.Sprites.Default.CannonBase.Draw(
				0, 
				position.Position, 
				Resources.Sprites.Default.CannonBase.Origin, 
				Vector2.One, 
				_baseRotation, 
				Color.White
			);
		}

		private void Shoot()
		{
			var position = GetComponent<PositionComponent>();
			new CannonBall(position.Position + _direction * 24, _direction, this, Layer);
			_shootingAnimationRunning = true;
			_shootingAnimationProgress = 0;
			
		}

	}
}
