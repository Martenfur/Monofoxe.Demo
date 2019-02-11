
namespace Monofoxe.Demo.GameLogic.Tiles
{
	/// <summary>
	/// Tells, how a tile behaves during collisions.
	/// </summary>
	public enum TilesetTileCollisionMode
	{
		None = 0, // Tile isn't solid.
		Solid = 1, // Tile is solid.
		Custom = 2, // Tile uses custom collider.
	}
}
