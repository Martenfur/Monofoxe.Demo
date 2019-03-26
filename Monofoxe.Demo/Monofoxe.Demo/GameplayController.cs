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

		public static MapBuilder CurrentMap;

		public static Layer GUILayer;

		public GameplayController() : base(SceneMgr.GetScene("default")["default"])
		{
			
			GameMgr.MaxGameSpeed = 60;
			GameMgr.MinGameSpeed = 60;
			
			DrawMgr.CurrentFont = Resources.Fonts.Arial;

			ScreenController.Init();

			
			CurrentMap = new ColliderMapBuilder(Resources.Maps.Test);
			CurrentMap.Build();
			
			
			CollisionDetector.Init();
			Scene.Priority = -10000;

			GUILayer = Scene.CreateLayer("gui");
			GUILayer.IsGUI = true;
		}
		
		public override void Update()
		{
			if (Input.CheckButtonPress(Buttons.F))
			{
				ScreenController.SetFullscreen(!GameMgr.WindowManager.IsFullScreen);
			}
			if (Input.CheckButtonPress(Buttons.R))
			{
				CurrentMap.Destroy();
				CurrentMap.Build();
			}

			if (Input.CheckButtonPress(Buttons.E))
			{
				if (TimeKeeper.GlobalTimeMultiplier == 1)
				{
					TimeKeeper.GlobalTimeMultiplier = 0.5f;
				}
				else
				{
					TimeKeeper.GlobalTimeMultiplier = 1;
				}
			}
		}

		
		public override void Draw()
		{	
			DrawMgr.CurrentFont = Resources.Fonts.Arial;
			DrawMgr.DrawText("FPS:" + GameMgr.Fps, DrawMgr.CurrentCamera.Position - Vector2.One * 320);
		}

	}
}