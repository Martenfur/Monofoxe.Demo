using System;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	/// <summary>
	/// Draws nice parallax background.
	/// </summary>
	public class Background : Entity
	{
		public new string Tag => "background";

		Sprite _sun = Resources.Sprites.Default.Sun;
		Vector2 _sunBasePosition = new Vector2(100, 100);

		Sprite _mountains = Resources.Sprites.Default.Mountains;
		Vector2 _mountainsBasePosition = new Vector2(400, -195);


		Vector2 _forestBasePosition = new Vector2(0, 0);

		Sprite _forest1 = Resources.Sprites.Default.Forest1;
		Sprite _forest2 = Resources.Sprites.Default.Forest2;
		
		float _forest1Parallax = 0.01f;
		float _forest2Parallax = 0.02f;

		float _minYParallax = -1000;


		public Background(Layer layer) : base(layer)
		{
			
		}

		public override void Draw()
		{
			DrawMgr.AddTransformMatrix(
				Matrix.CreateTranslation((DrawMgr.CurrentCamera.Position - DrawMgr.CurrentCamera.Size / 2).ToVector3())
			);

			DrawMgr.CurrentColor = Color.White;

			DrawMgr.DrawSprite(_sun, _sunBasePosition);

			DrawMgr.DrawSprite(
				_mountains, 
				_mountainsBasePosition + DrawMgr.CurrentCamera.Size * Vector2.UnitY
			);
			

			DrawParallaxForest(_forest1, _forest1Parallax);
			DrawParallaxForest(_forest2, _forest2Parallax);

			DrawMgr.ResetTransformMatrix();
		}

		private void DrawParallaxForest(Sprite sprite, float parallax)
		{
			var loops = (int)(DrawMgr.CurrentCamera.Size.X / sprite.Width) + 1;
			
			for(var i = -1; i < loops; i += 1)
			{
				var pos = _forestBasePosition 
					+ DrawMgr.CurrentCamera.Size * Vector2.UnitY 
					+ GetParallax(sprite, parallax)
					+ Vector2.UnitX * i * sprite.Width;
				
				DrawMgr.DrawSprite(
					sprite, 
					pos
				);

				// Drawing a bottom line of pixels as a filler from the bottom of background sprite to the end of the screen.
				DrawMgr.DrawSprite(
					sprite, 
					0, 
					new Rectangle((int)pos.X, (int)pos.Y - 1, sprite.Width, (int)(Math.Abs(GetParallax(sprite, parallax).Y)) + 2), 
					new Rectangle(0, sprite.Height - 1, sprite.Width, 1)
				);
			}			
		}

		/// <summary>
		/// Calculates parallax from sprite, parallax value and current camera position.
		/// NOTE: Can be used only in Draw.
		/// </summary>
		private Vector2 GetParallax(Sprite sprite, float parallaxValue)
		{
			var parallaxShiftX = -DrawMgr.CurrentCamera.Position.X * parallaxValue;
			var parallaxOverflow = (int)(parallaxShiftX / sprite.Width);
			parallaxShiftX -= sprite.Width * parallaxOverflow; 

			var parallaxShiftY = -DrawMgr.CurrentCamera.Position.Y * parallaxValue;
			
			
			if (parallaxShiftY < _minYParallax * parallaxValue)
			{
				parallaxShiftY = _minYParallax * parallaxValue;
			}
			if (parallaxShiftY > 0)
			{
				parallaxShiftY = 0;
			}
			

			return new Vector2(parallaxShiftX, parallaxShiftY);
		}

	}
}
