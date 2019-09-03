using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.Cameras;

namespace Monofoxe.Demo.GameLogic
{
	public static class ScreenController
	{
		public static int WindowWidth = 1000;
		public static int WindowHeight = 600;
		
		static Vector2 _maxResolution = new Vector2(1920, 1080);

		public static Camera MainCamera;

		public static void Init()
		{
			var window = GameMgr.WindowManager;

			window.Window.AllowUserResizing = false;
			window.ApplyChanges();
			window.CanvasMode = CanvasMode.KeepAspectRatio;
			GraphicsMgr.Sampler = SamplerState.PointClamp;

			
			window.CanvasSize = new Vector2(WindowWidth, WindowHeight);
			window.CenterWindow();
			window.ApplyChanges();
			MainCamera = new Camera(window.CanvasWidth, window.CanvasHeight);
			MainCamera.BackgroundColor = new Color(117, 190, 255);
			MainCamera.Offset = MainCamera.Size / 2;

			SetFullscreen(true);
		}
		

		public static void SetFullscreen(bool fullscreen)
		{
			var window = GameMgr.WindowManager;
			
			if (fullscreen == window.IsFullScreen)
			{
				return;
			}
			
			window.SetFullScreen(fullscreen);	
		
			if (fullscreen)
			{
				window.CanvasSize = new Vector2(
					MathHelper.Min(_maxResolution.X, window.ScreenWidth), 
					MathHelper.Min(_maxResolution.Y, window.ScreenHeight)
				);
			}
			else
			{
				window.CanvasSize = new Vector2(WindowWidth, WindowHeight);
				window.CenterWindow();
				window.ApplyChanges();
			}
			MainCamera.Resize(window.CanvasWidth, window.CanvasHeight);
			MainCamera.Offset = MainCamera.Size / 2;
		}

	}
}
