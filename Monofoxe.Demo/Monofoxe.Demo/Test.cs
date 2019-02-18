using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils.Cameras;
using Resources.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Demo.GameLogic.Entities.Core;
using System;
using Monofoxe.Tiled;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo
{
	public class Test : Entity
	{
		public static Camera Camera = new Camera(800, 600);

		Map _test;

		public Test() : base(SceneMgr.GetScene("default")["default"])
		{
			
			GameMgr.MaxGameSpeed = 60;
			GameMgr.MinGameSpeed = 60;

			Camera.BackgroundColor = Color.Azure;

			GameMgr.WindowManager.CanvasSize = new Vector2(800, 600);
			GameMgr.WindowManager.Window.AllowUserResizing = false;
			GameMgr.WindowManager.ApplyChanges();
			GameMgr.WindowManager.CenterWindow();
			GameMgr.WindowManager.CanvasMode = CanvasMode.Fill;
			
			DrawMgr.Sampler = SamplerState.PointClamp;
			/*
			var entity = new Entity(Layer, "physicsBoi");
			entity.AddComponent(new PositionComponent(Vector2.Zero));
			var phy = new PhysicsObjectComponent();
			phy.Size = Vector2.One * 32;
			entity.AddComponent(phy);
			*/
			_test = new ColliderMap(Resources.Maps.Test);
			_test.Load();

			
			CollisionDetector.Init();
			Scene.Priority = -10000;
		}
		
		public override void Update()
		{

		}

		
		public override void Draw()
		{	
			DrawMgr.CurrentFont = Resources.Fonts.Arial;
			DrawMgr.DrawText("FPS:" + GameMgr.Fps, DrawMgr.CurrentCamera.Position + Vector2.One * 32);
		}

	}
}