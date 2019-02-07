using System;
using Microsoft.Xna.Framework;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.Utils.Tilemaps;

namespace Monofoxe.Demo.GameLogic.Tiles
{
	/// <summary>
	/// Basic tilemap class. Provides basic functionality,
	/// supports camera zooming.
	/// </summary>
	public class ColliderTilemapComponent : Component, ITilemap<ColliderTile>
	{
		protected ColliderTile[,] _tileGrid;

		public Vector2 Offset {get; set;} = Vector2.Zero;

		public int TileWidth {get; protected set;}
		public int TileHeight {get; protected set;}

		public int Width {get; protected set;}
		public int Height {get; protected set;}

		/// <summary>
		/// Tells how many tile rows and columns will be drawn outside of camera's bounds.
		/// May be useful for tiles larger than the grid. 
		/// </summary>
		public int Padding = 0;

		
		public ColliderTile? GetTile(int x, int y)
		{
			if (InBounds(x, y))
			{
				return _tileGrid[x, y]; 
			}
			return null;
		}

		public void SetTile(int x, int y, ColliderTile tile)
		{
			if (InBounds(x, y))
			{
				_tileGrid[x, y] = tile;
			}
		}

		/// <summary>
		/// Returns tile without out-of-bounds check.
		/// WARNING: This method will throw an exception, if coordinates are out of bounds.
		/// </summary>
		public ColliderTile GetTileUnsafe(int x, int y) =>
			_tileGrid[x, y];
		
		/// <summary>
		/// Sets tile without out-of-bounds check.
		/// WARNING: This method will throw an exception, if coordinates are out of bounds.
		/// </summary>
		public ColliderTile SetTileUnsafe(int x, int y, ColliderTile tile) =>
			_tileGrid[x, y] = tile;
		
		
		public bool InBounds(int x, int y) =>
			x >= 0 && y >= 0 && x < Width && y < Height;
		

		public ColliderTilemapComponent(int width, int height, int tileWidth, int tileHeight)
		{
			Width = width;
			Height = height;
			TileWidth = tileWidth;
			TileHeight = tileHeight;
			_tileGrid = new ColliderTile[Width, Height];
		}


		public override object Clone() =>
			throw new NotImplementedException();

	}
}
