using System;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.Cameras;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine;
using System.Collections.Generic;

namespace Monofoxe.Demo.GameLogic
{
	public class Pause : Entity
	{
		private Scene _targetScene;

		private int _blurMaxValue = 3;

		private double _blurTime = 0.05;

		private Alarm _blurAlarm;

		private int _blur;

		private SelectionMenu _menu;
		
		private Vector2 _menuOffset = new Vector2(0, 100);

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

			_menu = new SelectionMenu(
				Layer, 
				new List<Sprite>
				{
					Resources.Sprites.Default.TitleContinue,
					Resources.Sprites.Default.TitleExit,
				},
				ScreenController.MainCamera.Size / 2 + _menuOffset
			);
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
					
					_menu.DestroyEntity();

					DestroyEntity();
				}
			}

			if (_menu.Triggered)
			{
				// This is very bad design, but it's only two elements, so whatever.
				// Don't repeat my mistakes, kids. : - )
				var continueOption = (_menu.SelectedItem == 0);
				var exitOption = (_menu.SelectedItem == 1);
			
				if (continueOption)
				{
					Unpause();
				}

				if (exitOption)
				{
					GameMgr.ExitGame();
				}
			}
			
			Resources.Effects.Blur.Parameters["radius"].SetValue(_blur);
			
		}

		public override void Draw()
		{
			GraphicsMgr.CurrentColor = Color.White;
			Resources.Sprites.Default.Pause.Draw(
				_menu.GetComponent<PositionComponent>().Position, 
				Resources.Sprites.Default.Pause.Origin 
					+ Vector2.UnitY * (
						_menu.GetMenuHeight() / 2 
						+ _menu.ButtonSpacing 
						+ _menu.ButtonSize.Y / 2
					)
			);
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
