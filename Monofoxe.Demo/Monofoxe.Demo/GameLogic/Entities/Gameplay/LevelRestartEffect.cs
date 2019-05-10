using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
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

		private double _fadeDelay = 1f;

		private Alarm _delayAlarm;

		private bool _nextLevel;

		public LevelRestartEffect(Layer layer, bool nextLevel = false) : base(layer)
		{
			_nextLevel = nextLevel;
			_alarm = new AutoAlarm(_fadeTime);
			_delayAlarm = new Alarm();
			_delayAlarm.Set(_fadeDelay);
		}

		public override void Update()
		{
			_delayAlarm.Update();

			if (_delayAlarm.Running)
			{
				return;
			}

			if (_alarm.Update())
			{
				_fadeDirection *= -1;
				if (_fadeDirection == 1)
				{
					DestroyEntity();
				}
				else
				{
					if (_nextLevel)
					{
						MapController.BuildNextMap();
					}
					else
					{
						MapController.RebuildCurrentMap();
					}
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
			GraphicsMgr.CurrentColor = Color.Black * (float)_blackscreenAlpha;
			RectangleShape.Draw(Vector2.Zero, GameMgr.WindowManager.CanvasSize, false);
		}
	}
}
