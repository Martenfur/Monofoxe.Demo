using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.Utils.Cameras;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Demo.GameLogic.Entities.Core;

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

		public GameCamera(Layer layer, Camera camera) : base(layer)
		{
			Camera = camera;
			_position = camera.Position;


		}

		public override void Update()
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
			}
			



			if (Target != null)
			{
				var targetDistance = GameMath.Distance(Camera.Position, Target.Position);
				var targetVector = (Camera.Position - Target.Position).GetSafeNormalize();

				if (targetDistance > MaxDistance)
				{
					_position = (Target.Position + targetVector * MaxDistance);
					targetDistance = MaxDistance;
				}

				if (targetDistance > PullbackDistance)
				{
					var distanceToMax = targetDistance - PullbackDistance;

					distanceToMax *= (float)Math.Pow(PullbackMultiplier, TimeKeeper.GlobalTime());
					_position = Target.Position + targetVector * (distanceToMax + PullbackDistance);
				}
			}
			
			Camera.Position = _position.Round();
			
			var adjustedTopLeftPoint = _topLeftPoint + Camera.Size / 2;
			var adjustedBottomRightPoint = _bottomRightPoint - Camera.Size / 2;


			if (Camera.Position.X < adjustedTopLeftPoint.X)
			{
				Camera.Position.X = adjustedTopLeftPoint.X;
			}
			if (Camera.Position.Y < adjustedTopLeftPoint.Y)
			{
				Camera.Position.Y = adjustedTopLeftPoint.Y;
			}
			if (Camera.Position.X > adjustedBottomRightPoint.X)
			{
				Camera.Position.X = adjustedBottomRightPoint.X;
			}
			if (Camera.Position.Y > adjustedBottomRightPoint.Y)
			{
				Camera.Position.Y = adjustedBottomRightPoint.Y;
			}
			
			
		}


	}
}
