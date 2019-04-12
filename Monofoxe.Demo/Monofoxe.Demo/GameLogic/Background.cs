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
		Vector2 _mountainsBasePosition = new Vector2(400, -195 - 100);


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
			GraphicsMgr.AddTransformMatrix(
				Matrix.CreateTranslation((GraphicsMgr.CurrentCamera.Position - GraphicsMgr.CurrentCamera.Size / 2).ToVector3())
			);

			GraphicsMgr.CurrentColor = Color.White;

			_sun.Draw(_sunBasePosition, _sun.Origin);
			
			_mountains.Draw( 
				_mountainsBasePosition + GraphicsMgr.CurrentCamera.Size * Vector2.UnitY,
				_mountains.Origin
			);
			

			DrawParallaxForest(_forest1, _forest1Parallax);
			DrawParallaxForest(_forest2, _forest2Parallax);

			GraphicsMgr.ResetTransformMatrix();
		}

		private void DrawParallaxForest(Sprite sprite, float parallax)
		{
			var loops = (int)(GraphicsMgr.CurrentCamera.Size.X / sprite.Width) + 1;
			
			for(var i = -1; i < loops; i += 1)
			{
				var pos = _forestBasePosition 
					+ GraphicsMgr.CurrentCamera.Size * Vector2.UnitY  
					+ GetParallax(sprite, parallax)
					+ Vector2.UnitX * i * sprite.Width
					- Vector2.UnitY * 100;
				
				sprite.Draw(pos, sprite.Origin);

				// Drawing a bottom line of pixels as a filler from the bottom of background sprite to the end of the screen.
				sprite.Draw(
					0, 
					new Rectangle((int)pos.X, (int)pos.Y - 1, sprite.Width, (int)(Math.Abs(GetParallax(sprite, parallax).Y) + (int)GameCamera.OffsetY) + 2 ), 
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
			var parallaxShiftX = -GraphicsMgr.CurrentCamera.Position.X * parallaxValue;
			var parallaxOverflow = (int)(parallaxShiftX / sprite.Width);
			parallaxShiftX -= sprite.Width * parallaxOverflow; 

			var parallaxShiftY = -GraphicsMgr.CurrentCamera.Position.Y * parallaxValue;
			
			
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
