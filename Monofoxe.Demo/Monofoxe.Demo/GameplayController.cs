using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Demo.GameLogic;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
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
			
			Text.CurrentFont = Resources.Fonts.Arial;

			ScreenController.Init();

			
			CurrentMap = new ColliderMapBuilder(Resources.Maps.Level1);
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
			
			// TODO: REMOVE!
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
			Text.CurrentFont = Resources.Fonts.Arial;
			Text.Draw("FPS:" + GameMgr.Fps, GraphicsMgr.CurrentCamera.Position - Vector2.One * 320, Vector2.One, Vector2.Zero, 0);
		}

	}
}