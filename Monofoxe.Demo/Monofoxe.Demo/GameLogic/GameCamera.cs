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
		public float MaxDistance = 150;

		
		Vector2 _position;

		public GameCamera(Layer layer, Camera camera) : base(layer)
		{
			Camera = camera;
			_position = camera.Position;
		}

		public override void Update()
		{
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

					distanceToMax *= (float)Math.Pow(1.0/200.0, TimeKeeper.GlobalTime());
					_position = Target.Position + targetVector * (distanceToMax + PullbackDistance);
				}
			}
			Camera.Position = _position.Round();
		}


	}
}
