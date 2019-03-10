using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Demo.GameLogic;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils.Cameras;
using Monofoxe.Tiled;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo
{
	public class GameplayController : Entity
	{
		
		public static RandomExt Random = new RandomExt();

		MapBuilder _test;
		public GameplayController() : base(SceneMgr.GetScene("default")["default"])
		{
			
			GameMgr.MaxGameSpeed = 60;
			GameMgr.MinGameSpeed = 60;
			
			DrawMgr.CurrentFont = Resources.Fonts.Arial;

			ScreenController.Init();

			
			_test = new ColliderMapBuilder(Resources.Maps.Test);
			_test.Build();
			
			
			CollisionDetector.Init();
			Scene.Priority = -10000;
		}
		
		public override void Update()
		{
			if (Input.CheckButtonPress(Buttons.F))
			{
				ScreenController.SetFullscreen(!GameMgr.WindowManager.IsFullScreen);
			}
			if (Input.CheckButtonPress(Buttons.R))
			{
				_test.Destroy();
				_test.Build();
			}
		}

		
		public override void Draw()
		{	
			DrawMgr.CurrentFont = Resources.Fonts.Arial;
			DrawMgr.DrawText("FPS:" + GameMgr.Fps, DrawMgr.CurrentCamera.Position - Vector2.One * 320);
		}

	}
}