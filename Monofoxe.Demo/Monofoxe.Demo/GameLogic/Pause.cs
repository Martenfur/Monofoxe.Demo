using System;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.Utils.Cameras;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine;


namespace Monofoxe.Demo.GameLogic
{
	public class Pause : Entity
	{
		private Scene _targetScene;

		private int _blurMaxValue = 3;

		private double _blurTime = 0.05;

		private Alarm _blurAlarm;

		private int _blur;

		private enum TransitionState
		{
			FadeIn,
			Idle,
			FadeOut,
		}

		private TransitionState _state;

		public Pause(Layer layer, Scene targetScene) : base(layer)
		{
			_targetScene = targetScene;	
			_targetScene.Enabled = false;

			Resources.Effects.Blur.Parameters["radius"].SetValue(0);
			Resources.Effects.Blur.Parameters["width"].SetValue(ScreenController.MainCamera.Size.X);
			Resources.Effects.Blur.Parameters["height"].SetValue(ScreenController.MainCamera.Size.Y);

			ScreenController.MainCamera.PostprocessingMode = PostprocessingMode.Camera;
			ScreenController.MainCamera.PostprocessorEffects.Add(Resources.Effects.Blur);

			_state = TransitionState.FadeIn;
			_blurAlarm = new Alarm();
			_blurAlarm.Set(_blurTime);
		}

		public override void Update()
		{	
			var trigger = _blurAlarm.Update();

			
			if (_state == TransitionState.FadeIn)
			{
				
				_blur = (int)(_blurMaxValue * (1.0 - (_blurAlarm.Counter / _blurTime)));

				if (trigger)
				{
					_blur = _blurMaxValue;
					_state = TransitionState.Idle;
				}
			}

			

			if (_state == TransitionState.FadeOut)
			{
				_blur = (int)(_blurMaxValue * (_blurAlarm.Counter / _blurTime));

				if (trigger)
				{
					_blur = 0;
					_state = TransitionState.Idle;
					
					_targetScene.Enabled = !_targetScene.Enabled;

					ScreenController.MainCamera.PostprocessingMode = PostprocessingMode.None;
					ScreenController.MainCamera.PostprocessorEffects.Remove(Resources.Effects.Blur);

					DestroyEntity();
				}
			}

			
			Resources.Effects.Blur.Parameters["radius"].SetValue(_blur);
			
		}

		public override void Draw()
		{
			GraphicsMgr.CurrentColor = Color.White;
			Resources.Sprites.Default.PlayerMain.Draw(GameMgr.WindowManager.CanvasSize / 2, Vector2.One * 32);
		}


		public void Unpause()
		{
			if (_state == TransitionState.Idle)
			{
				_state = TransitionState.FadeOut;
				_blurAlarm.Set(_blurTime);
			}
		}

	}
}
