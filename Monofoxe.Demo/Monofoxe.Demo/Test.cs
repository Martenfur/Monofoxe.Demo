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
	public class Test : Entity
	{
		public static Camera Camera = new Camera(1000, 800);

		public static RandomExt Random = new RandomExt();

		Map _test;
		public Test() : base(SceneMgr.GetScene("default")["default"])
		{
			
			GameMgr.MaxGameSpeed = 60;
			GameMgr.MinGameSpeed = 60;
			Camera.BackgroundColor = new Color(117, 190, 255);
			Camera.Offset = Camera.Size / 2;

			DrawMgr.CurrentFont = Resources.Fonts.Arial;

			GameMgr.WindowManager.CanvasSize = new Vector2(800, 600);
			GameMgr.WindowManager.Window.AllowUserResizing = false;
			GameMgr.WindowManager.ApplyChanges();
			GameMgr.WindowManager.CenterWindow();
			GameMgr.WindowManager.CanvasMode = CanvasMode.Fill;
			
			DrawMgr.Sampler = SamplerState.PointClamp;
			
			_test = new ColliderMap(Resources.Maps.Test);
			_test.Load();
			
			var l = _test.MapScene.CreateLayer("background");
			l.Priority = 10000;

			new Background(l);

			CollisionDetector.Init();
			Scene.Priority = -10000;
		}
		
		public override void Update()
		{

		}

		
		public override void Draw()
		{	
			DrawMgr.CurrentFont = Resources.Fonts.Arial;
			DrawMgr.DrawText("FPS:" + GameMgr.Fps, DrawMgr.CurrentCamera.Position - Vector2.One * 320);
		}

	}
}