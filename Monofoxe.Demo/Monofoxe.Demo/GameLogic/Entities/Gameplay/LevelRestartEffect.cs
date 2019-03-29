using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class LevelRestartEffect : Entity
	{
		public new string Tag => "levelRestartEffect";
		
		private double _blackscreenAlpha = 0;

		private int _fadeDirection = 1;

		private double _fadeTime = 0.5f;

		private AutoAlarm _alarm;

		public LevelRestartEffect(Layer layer) : base(layer)
		{
			_alarm = new AutoAlarm(_fadeTime);
			
		}

		public override void Update()
		{
			if (_alarm.Update())
			{
				_fadeDirection *= -1;
				if (_fadeDirection == 1)
				{
					EntityMgr.DestroyEntity(this);
				}
				else
				{
					GameplayController.CurrentMap.Destroy();
					GameplayController.CurrentMap.Build();
				}
			}

			
			_blackscreenAlpha = _alarm.Counter / _fadeTime;

			if (_fadeDirection == 1)
			{
				_blackscreenAlpha = 1 - _blackscreenAlpha;
			}

		}

		
		public override void Draw()
		{
			DrawMgr.CurrentColor = Color.Black * (float)_blackscreenAlpha;

			DrawMgr.DrawRectangle(Vector2.Zero, GameMgr.WindowManager.CanvasSize, false);
			
		}
	}
}
