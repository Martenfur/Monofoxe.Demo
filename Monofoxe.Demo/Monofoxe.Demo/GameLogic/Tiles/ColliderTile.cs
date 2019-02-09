using Monofoxe.Engine.Utils.Tilemaps;
using Monofoxe.Demo.GameLogic.Collisions;

namespace Monofoxe.Demo.GameLogic.Tiles
{
	public struct ColliderTile : ITile
	{
		public int Index {get; set;}
		public bool IsBlank => Tileset == null || Index == (Tileset.StartingIndex - 1);

		public bool FlipHor {get; set;}
		public bool FlipVer {get; set;}
		
		public ColliderTileset Tileset;
		
		public ColliderTile(int index, ColliderTileset tileset, bool flipHor = false, bool flipVer = false)
		{
			Index = index;
			Tileset = tileset;
			FlipHor = flipHor;
			FlipVer = flipVer;
		}
		
		public ICollider GetCollider()
		{
			if (Tileset == null)
				return null;

			return Tileset.GetCollider(Index);
		}
		
		public ITilesetTile GetTilesetTile()
		{
			if (Tileset != null)
			{
				return Tileset.GetTilesetTile(Index);
			}
			return null;
		}
		

	}
}
