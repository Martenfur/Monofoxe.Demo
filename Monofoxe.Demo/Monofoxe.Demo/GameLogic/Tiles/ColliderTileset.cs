using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Engine.Utils.Tilemaps;
using Microsoft.Xna.Framework;

namespace Monofoxe.Demo.GameLogic.Tiles
{
	public class ColliderTileset : Tileset
	{
		public ColliderTileset(ITilesetTile[] tiles, Vector2 offset, int startingIndex = 1) :
			base(tiles, offset, startingIndex) {}
		
		public ICollider GetCollider(int index)
		{
			if (Tiles == null || index < StartingIndex || index >= StartingIndex + Tiles.Length)
			{
				return null;
			}
			return ((ColliderTilesetTile)Tiles[index - StartingIndex]).Collider;
		}
	}
}
