using System.Collections.Generic;
using ChaiFoxes.FMODAudio;
using Microsoft.Xna.Framework;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine;


namespace Monofoxe.Demo.GameLogic.Audio
{
	public class SoundController : Entity
	{
		public static float SoundVolume = 1f;

		public static float MusicVolume = 1f;

		public static FMOD.ChannelGroup MusicGroup;
		public static FMOD.ChannelGroup SoundGroup;

		public static List<LayeredSound> UpdatingSounds = new List<LayeredSound>();

		private static List<Sound> _playedCurrently = new List<Sound>();

		public SoundController(Layer layer) : base(layer)
		{
			MusicGroup = AudioMgr.CreateChannelGroup("music");
			SoundGroup = AudioMgr.CreateChannelGroup("sound");
			
			MusicGroup.setVolume(MusicVolume);
			SoundGroup.setVolume(SoundVolume);
		}
		
		public override void Update()
		{
			foreach(var sound in UpdatingSounds)
			{
				sound.Update();
			}
			_playedCurrently.Clear();
		}

		public static SoundChannel PlaySound(Sound sound, bool paused = false) =>
			sound.Play(SoundGroup, paused);
		
		public static SoundChannel PlaySoundOnce(Sound sound)
		{
			if (!_playedCurrently.Contains(sound))
			{
				_playedCurrently.Add(sound);
				return sound.Play(SoundGroup);
			}

			return null;
		}
		

		public static SoundChannel PlaySoundAt(Sound sound, Vector2 position)
		{
			var inBounds = false;
			foreach(var camera in SceneMgr.CurrentScene.GetEntityList<GameCamera>())
			{
				if (camera.InBounds(position))
				{
					inBounds = true;
					break;
				}
			}

			if (!inBounds)
			{
				return null;
			}

			sound.Position3D = position.ToVector3();
			var channel = sound.Play(SoundGroup, true);
			channel.Resume();
			return channel;
		}
		

	}
}
