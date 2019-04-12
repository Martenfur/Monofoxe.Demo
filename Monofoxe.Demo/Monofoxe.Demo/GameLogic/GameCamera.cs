using System;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.Utils.Cameras;

namespace Monofoxe.Demo.GameLogic
{
	public class GameCamera : Entity
	{
		public new string Tag => "gameCamera";

		public Camera Camera;

		/// <summary>
		/// Camera will follow this target.
		/// </summary>
		public PositionComponent Target;

		/// <summary>
		/// Camera will be smoothly pulled, further than this distance.
		/// </summary>
		public float PullbackDistance = 0;

		/// <summary>
		/// Camera can't go further than this from target.
		/// </summary>
		public float MaxDistance = 150 * 2;

		public double PullbackMultiplier = 1.0 / 200.0;

		Vector2 _position;

		private Vector2 _topLeftPoint, _bottomRightPoint;

		private bool _gotStoppers = false;

		public const float OffsetY = 100;

		public GameCamera(Layer layer, Camera camera) : base(layer)
		{
			Camera = camera;
			_position = camera.Position;


		}

		public override void Update()
		{
			RetrieveStopperPoints();

			if (Target != null)
			{
				var targetPosition = Target.Position - Vector2.UnitY * OffsetY;
				var targetDistance = GameMath.Distance(Camera.Position, targetPosition);
				var targetVector = (Camera.Position - targetPosition).GetSafeNormalize();

				if (targetDistance > MaxDistance)
				{
					_position = (targetPosition + targetVector * MaxDistance);
					targetDistance = MaxDistance;
				}

				if (targetDistance > PullbackDistance)
				{
					var distanceToMax = targetDistance - PullbackDistance;

					distanceToMax *= (float)Math.Pow(PullbackMultiplier, TimeKeeper.GlobalTime());
					_position = targetPosition + targetVector * (distanceToMax + PullbackDistance);
				}
			}
			
			
			// Restricting the camera.
			var adjustedTopLeftPoint = _topLeftPoint + Camera.Size / 2;
			var adjustedBottomRightPoint = _bottomRightPoint - Camera.Size / 2;

			if (_position.X < adjustedTopLeftPoint.X)
			{
				_position.X = adjustedTopLeftPoint.X;
			}
			if (_position.Y < adjustedTopLeftPoint.Y)
			{
				_position.Y = adjustedTopLeftPoint.Y;
			}
			if (_position.X > adjustedBottomRightPoint.X)
			{
				_position.X = adjustedBottomRightPoint.X;
			}
			if (_position.Y > adjustedBottomRightPoint.Y)
			{
				_position.Y = adjustedBottomRightPoint.Y;
			}
			// Restricting the camera.
			
			Camera.Position = _position.Round();
			
		}


		/// <summary>
		/// Getting stopper points.
		/// </summary>
		private void RetrieveStopperPoints()
		{
			if (!_gotStoppers)
			{
				var stoppers = SceneMgr.CurrentScene.GetEntityList<CameraStopper>();
			
				_topLeftPoint = stoppers[0].GetComponent<PositionComponent>().Position;
				_bottomRightPoint = _topLeftPoint;
			
				foreach(var stopper in stoppers)
				{
					var stopperPosition = stopper.GetComponent<PositionComponent>().Position;
				
					if (stopperPosition.X < _topLeftPoint.X)
					{
						_topLeftPoint.X = stopperPosition.X;
					}
					if (stopperPosition.Y < _topLeftPoint.Y)
					{
						_topLeftPoint.Y = stopperPosition.Y;
					}
					if (stopperPosition.X > _bottomRightPoint.X)
					{
						_bottomRightPoint.X = stopperPosition.X;
					}
					if (stopperPosition.Y > _bottomRightPoint.Y)
					{
						_bottomRightPoint.Y = stopperPosition.Y;
					}
					stopper.DestroyEntity();
				}
				_gotStoppers = true;
			}
		}


	}
}
