using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;


namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class CameraMagnet : Entity
	{
		public new string Tag => "cameraMagnet";
		
		public float Raduis;

		public PositionComponent PreviousTarget;

		private bool _gotCamera = false;

		public CameraMagnet(Vector2 position, float raduis, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));

			Raduis = raduis;

			Visible = false;
		}

		public override void Update()
		{
			var position = GetComponent<PositionComponent>();

			var cameras = Scene.GetEntityList<GameCamera>();

			if (cameras.Count > 0)
			{
				var camera = cameras[0];

				if (camera.Target == null)
				{
					return;
				}


				if (_gotCamera)
				{
					var distance = GameMath.Distance(position.Position, PreviousTarget.Position);
					if (distance > Raduis)
					{
						camera.Target = PreviousTarget;
						_gotCamera = false;
						camera.MaxDistanceEnabled = true;
					}
				}
				else
				{
					var distance = GameMath.Distance(position.Position, camera.Target.Position);
					if (distance < Raduis)
					{
						PreviousTarget = camera.Target;
						camera.Target = position;
						_gotCamera = true;
						camera.MaxDistanceEnabled = false; // Makes transition smoother.
					}
				}
			}
			
		}

		
		public override void Draw()
		{
			GraphicsMgr.CurrentColor = Color.Red;
			var position = GetComponent<PositionComponent>();
			CircleShape.Draw(position.Position, Raduis, true);
		}
	}
}
