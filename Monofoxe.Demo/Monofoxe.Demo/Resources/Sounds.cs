using ChaiFoxes.FMODAudio;


namespace Resources
{
	public static class Sounds
	{
		
		public static Sound MainBaseLayer;
		public static Sound MainTopLayer;

		public static Sound ButtonPress;
		public static Sound ButtonRelease;
		public static Sound Cannon;
		public static Sound Slide;
		public static Sound Crouch;
		public static Sound Jump;
		public static Sound Checkpoint;
		public static Sound CatDeath;


		public static void Load()
		{
			MainBaseLayer = AudioMgr.LoadStreamedSound("Music/MainBaseLayer.ogg");
			MainBaseLayer.Looping = true;
			MainTopLayer = AudioMgr.LoadStreamedSound("Music/MainTopLayer.ogg");
			MainTopLayer.Looping = true;
			
			ButtonPress = Load3DSound("Sounds/ButtonPress.wav", 400, 500);
			ButtonRelease = Load3DSound("Sounds/ButtonRelease.wav", 400, 500);
			Cannon = Load3DSound("Sounds/Cannon.wav", 400, 500);
			Cannon.Volume = 0.2f;


			Slide = AudioMgr.LoadSound("Sounds/Slide.wav");
			Slide.Looping = true;
			Slide.LowPass = 0.5f;

			Crouch = AudioMgr.LoadSound("Sounds/Crouch.wav");
			Crouch.LowPass = 0.5f;

			Jump = AudioMgr.LoadSound("Sounds/Jump.wav");
			
			Checkpoint = AudioMgr.LoadSound("Sounds/Checkpoint.wav");
			
			CatDeath = AudioMgr.LoadSound("Sounds/CatDeath.wav");
			
		}

		static Sound Load3DSound(string path, float minDistance, float maxDistance)
		{
			var sound = AudioMgr.LoadSound(path);
			sound.Is3D = true;
			sound.MinDistance3D = minDistance;
			sound.MaxDistance3D = maxDistance;

			return sound;
		}

		public static void Unload()
		{
		}

	}
}
