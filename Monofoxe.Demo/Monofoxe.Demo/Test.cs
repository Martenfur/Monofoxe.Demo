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
			

			Camera.BackgroundColor = new Color(64, 32, 32);

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
		}
		
		public override void Update()
		{
			if (Input.CheckButtonPress(Buttons.L))
			{
				if (GameMgr.MaxGameSpeed == 20)
				{
					GameMgr.MaxGameSpeed = 60;
				}
				else
				{
					GameMgr.MinGameSpeed = 20;
					GameMgr.MaxGameSpeed = 20;
				}
			}
			if (Input.CheckButtonPress(Buttons.K))
			{
				if (TimeKeeper.GlobalTimeMultiplier != 1)
				{
					TimeKeeper.GlobalTimeMultiplier = 1;
				}
				else
				{
					TimeKeeper.GlobalTimeMultiplier = 0.5;
				}
			}

		}

		
		public override void Draw()
		{	
			DrawMgr.DrawSprite(SpritesDefault.Monofoxe, 400, 300);
		}

	}
}