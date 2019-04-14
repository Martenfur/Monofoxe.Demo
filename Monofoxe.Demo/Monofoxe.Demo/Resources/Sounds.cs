using Monofoxe.FMODAudio;


namespace Resources
{
	public static class Sounds
	{
		
		public static Sound MainBaseLayer;
		public static Sound MainTopLayer;

		public static void Load()
		{
			MainBaseLayer = AudioMgr.LoadStreamedSound("Music/MainBaseLayer");
			MainTopLayer = AudioMgr.LoadStreamedSound("Music/MainTopLayer");
			//MainTopLayer.Play();
			//MainTopLayer.Loops = -1;
			MainBaseLayer.Play();
			MainBaseLayer.Loops = -1;
			
		}

		public static void Unload()
		{
		}

	}
}
