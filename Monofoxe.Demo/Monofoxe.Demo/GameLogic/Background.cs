using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Microsoft.Xna.Framework;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Demo.GameLogic.Entities;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
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
		
		float _forest1Parallax = 0.01f * 30;
		float _forest2Parallax = 0.02f * 30;


		public Background(Layer layer) : base(layer)
		{
			
		}

		public override void Draw()
		{
			DrawMgr.AddTransformMatrix(
				Matrix.CreateTranslation((DrawMgr.CurrentCamera.Position - DrawMgr.CurrentCamera.Size / 2).ToVector3())
			);

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
				DrawMgr.DrawSprite(
					sprite, 
					_forestBasePosition 
						+ DrawMgr.CurrentCamera.Size * Vector2.UnitY 
						+ GetParallax(sprite, parallax)
						+ Vector2.UnitX * i * sprite.Width
				);
			}
		}

		private Vector2 GetParallax(Sprite sprite, float parallaxValue)
		{
			var parallaxShift = DrawMgr.CurrentCamera.Position.X * parallaxValue;
			var parallaxOverflow = (int)(parallaxShift / sprite.Width);
			parallaxShift -= sprite.Width * parallaxOverflow; 

			return Vector2.UnitX * parallaxShift;
		}

	}
}
