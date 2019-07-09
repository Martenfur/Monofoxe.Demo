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

		private double _fadeInTime = 0.5f;
		private double _fadeOutTime = 0.5f;


		private AutoAlarm _alarm;

		private double _fadeDelay = 1f;

		private Alarm _delayAlarm;

		private bool _nextLevel;

		public double Fade => _blackscreenAlpha;

		public LevelRestartEffect(Layer layer, bool nextLevel = false) : base(layer)
		{
			_nextLevel = nextLevel;
			_alarm = new AutoAlarm(_fadeInTime);
			_delayAlarm = new Alarm();
			_delayAlarm.Set(_fadeDelay);
			Depth = -999;
			GameplayController.PausingEnabled = false;
		}

		public LevelRestartEffect(Layer layer, double fadeTime, bool nextLevel = false) : base(layer)
		{
			_fadeInTime = fadeTime;
			_nextLevel = nextLevel;
			_alarm = new AutoAlarm(_fadeInTime);
			_delayAlarm = new Alarm();
			_delayAlarm.Set(_fadeDelay);
			Depth = -999;
			GameplayController.PausingEnabled = false;
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

			

			if (_fadeDirection == 1)
			{
				_blackscreenAlpha = 1 - _alarm.Counter / _fadeInTime;
			}
			else
			{
				_blackscreenAlpha = _alarm.Counter / _fadeOutTime;
			}

		}

		public override void Destroy()
		{
			GameplayController.PausingEnabled = true;
		}

		public override void Draw()
		{
			GraphicsMgr.CurrentColor = Color.Black * (float)_blackscreenAlpha;
			RectangleShape.Draw(Vector2.Zero, GameMgr.WindowManager.CanvasSize, false);
		}
	}
}
