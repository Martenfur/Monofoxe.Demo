using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Monofoxe.Tiled;
using Monofoxe.Demo.GameLogic.Audio;

namespace Monofoxe.Demo
{
	public class GameplayController : Entity
	{
		
		public static RandomExt Random = new RandomExt();

		
		public static Layer GUILayer;

		public static SoundController Audio;

		public static LayeredSound music;

		private Pause _pause;

		public static bool PausingEnabled = true;

		public GameplayController() : base(SceneMgr.GetScene("default")["default"])
		{
			
			GameMgr.MaxGameSpeed = 60;
			GameMgr.MinGameSpeed = 60;
			
			Text.CurrentFont = Resources.Fonts.Arial;

			ScreenController.Init();

			Audio = new SoundController(Layer);
			
			MapController.Init();
			MapController.CurrentMap.Build();
			

			CollisionDetector.Init();
			Scene.Priority = -10000;

			GUILayer = Scene.CreateLayer("gui");
			GUILayer.IsGUI = true;

			
			music = new LayeredSound(SoundController.MusicGroup);
			music.AddLayer("top", Resources.Sounds.MainTopLayer);
			music.AddLayer("base", Resources.Sounds.MainBaseLayer);
			music.Play();
			SoundController.UpdatingSounds.Add(music);
		}
		
		public override void Update()
		{
			if (Input.CheckButtonPress(Buttons.Escape) && PausingEnabled)
			{
				if (_pause == null || _pause.Destroyed)
				{
					_pause = new Pause(GUILayer, MapController.CurrentMap.MapScene);
				}
				else
				{
					_pause.Unpause();
				}
			}

			if (Input.CheckButtonPress(Buttons.F))
			{
				ScreenController.SetFullscreen(!GameMgr.WindowManager.IsFullScreen);
			}
			if (Input.CheckButtonPress(Buttons.R))
			{
				MapController.RebuildCurrentMap();
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
				


				//SoundController.PlaySound(Resources.Sounds.Switchblock);
			}

			if (Input.CheckButtonPress(Buttons.K))
			{
				music.AddVolumeTransition("top", 0, 0.10f);
			}
			if (Input.CheckButtonPress(Buttons.L))
			{
				music.AddVolumeTransition("top", 1, 0.1f);
			}
		}

		
		public override void Draw()
		{	
			Text.CurrentFont = Resources.Fonts.Arial;
			Text.Draw("FPS:" + GameMgr.Fps, GraphicsMgr.CurrentCamera.Position - Vector2.One * 320, Vector2.One, Vector2.Zero, 0);
		}

	}
}