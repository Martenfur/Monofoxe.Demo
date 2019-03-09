// Template tags: 
// Default - Name of output class.
// Default - Name of current group.
// <sprite_name> - Name of each sprite.
// <sprite_hash_name> - Hash name of each sprite.

using Microsoft.Xna.Framework.Content;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using System.Collections.Generic;

namespace Resources.Sprites
{
	public static class Default
	{
		#region Sprites.
		public static Sprite Mountains;
		public static Sprite Monofoxe;
		public static Sprite Forest1;
		public static Sprite Forest2;
		public static Sprite Sun;
		public static Sprite WatermelonStem;
		public static Sprite PlayerMain;
		public static Sprite Watermelon;
		public static Sprite Gato;
		public static Sprite Frog;
		public static Sprite Platform;
		#endregion Sprites.
		
		private static string _groupName = "Default";
		private static ContentManager _content = new ContentManager(GameMgr.Game.Services);
		
		public static bool Loaded = false;
		
		public static void Load()
		{
			Loaded = true;
			var graphicsPath = AssetMgr.ContentDir + '/' + AssetMgr.GraphicsDir +  '/' + _groupName;
			var sprites = _content.Load<Dictionary<string, Sprite>>(graphicsPath);
			
			#region Sprite constructors.
			
			Mountains = sprites["Background/mountains"];
			Monofoxe = sprites["monofoxe"];
			Forest1 = sprites["Background/forest1"];
			Forest2 = sprites["Background/forest2"];
			Sun = sprites["Background/sun"];
			WatermelonStem = sprites["watermelon_stem"];
			PlayerMain = sprites["Characters/player_main"];
			Watermelon = sprites["Characters/watermelon"];
			Gato = sprites["Characters/gato"];
			Frog = sprites["Characters/frog"];
			Platform = sprites["Platform"];
			
			#endregion Sprite constructors.
		}
		
		public static void Unload()
		{
			_content.Unload();
			Loaded = false;
		}
	}
}
