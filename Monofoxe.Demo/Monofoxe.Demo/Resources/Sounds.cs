using ChaiFoxes.FMODAudio;


namespace Resources
{
	public static class Sounds
	{
		
		public static Sound MainBaseLayer;
		public static Sound MainTopLayer;

		public static void Load()
		{
			MainBaseLayer = AudioMgr.LoadStreamedSound("Music/MainBaseLayer.ogg");
			MainTopLayer = AudioMgr.LoadStreamedSound("Music/MainTopLayer.ogg");
			
			var top = MainTopLayer.Play();
			top.Looping = true;
			
			var b = MainBaseLayer.Play();
			b.Looping = true;
			
		}

		public static void Unload()
		{
		}

	}
}
