using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo.GameLogic.Entities
{
	public class TitleScreen : Entity
	{
		private Sprite _titleSprite = Resources.Sprites.Default.Title;
		private Vector2 _titleRelativePositionStart = new Vector2(0.5f, 0.25f);
		private Vector2 _titleRelativePositionEnd = new Vector2(0.5f, -0.25f);


		private Sprite _pressStartTitleSprite = Resources.Sprites.Default.PressStartTitle;
		private Vector2 _pressStartTitleRelativePosition = new Vector2(0.5f, 0.7f);

		private Sprite _creditsSprite = Resources.Sprites.Default.Credits;
		private Vector2 _creditsRelativePositionStart = new Vector2(1f, 1f);
		private Vector2 _creditsRelativePositionEnd = new Vector2(1.3f, 1f);


		private Vector2 _creditsOffset = new Vector2(-16, -16);


		private bool _animationEnabled = false;
		private float _animation = 0;
		private float _animationSpeed = 4f;


		public TitleScreen(Layer layer) : base(layer)
		{
			ToggleControls(false);
		}

		public override void Update()
		{
			if (GameButtons.Select.CheckPress())
			{
				_animationEnabled = true;
				_animation = 0;
			}

			if (_animationEnabled)
			{
				_animation += TimeKeeper.GlobalTime(_animationSpeed);
				if (_animation > 1)
				{
					_animation = 1;
					_animationEnabled = false;

					ToggleControls(true);
					DestroyEntity();
				}
			}

		}
		
		public override void Draw()
		{
			GraphicsMgr.CurrentColor = Color.White;

			_titleSprite.Draw(
				GetRelativePosition(_titleRelativePositionStart, _titleRelativePositionEnd), 
				_titleSprite.Origin
			);

			GraphicsMgr.CurrentColor = Color.White * (1 - _animation);
			_pressStartTitleSprite.Draw(
				GetRelativePosition(_pressStartTitleRelativePosition, _pressStartTitleRelativePosition), 
				_pressStartTitleSprite.Origin
			);
			GraphicsMgr.CurrentColor = Color.White;


			_creditsSprite.Draw(
				GetRelativePosition(_creditsRelativePositionStart, _creditsRelativePositionEnd) + _creditsOffset, 
				_creditsSprite.Origin
			);

		}

		private void ToggleControls(bool toggle)
		{
			var players = MapController.CurrentMap.MapScene.GetComponentList<PlayerComponent>();
			// TODO: Check out why the fuck it returns generic Component list.

			foreach (var player in players)
			{
				var castPlayer = (PlayerComponent)player;
				var actor = player.Owner.GetComponent<StackableActorComponent>();

				actor.Sleeping = !toggle;
				castPlayer.ControlsEnabled = toggle;
				GameplayController.PausingEnabled = toggle;
			}

		}

		private Vector2 GetRelativePosition(Vector2 multiplierStart, Vector2 multiplierEnd)
		{
			var multiplier = Vector2.Lerp(multiplierStart, multiplierEnd, _animation * _animation);

			return (GameMgr.WindowManager.CanvasSize * multiplier).RoundV();
		}
	}
}
