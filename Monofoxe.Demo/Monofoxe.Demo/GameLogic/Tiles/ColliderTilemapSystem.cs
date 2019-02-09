using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.Utils.Tilemaps;
using Monofoxe.Engine;

namespace Monofoxe.Demo.GameLogic.Tiles
{
	/// <summary>
	/// System for basic tilemap. Based on Monofoxe.ECS.
	/// Draws tilemaps in camera's bounds.
	/// </summary>
	public class ColliderTilemapSystem : BaseSystem
	{
		public override Type ComponentType => typeof(ColliderTilemapComponent);

		public override void Draw(Component component)
		{
			var tilemap = (ColliderTilemapComponent)component;

			var offsetCameraPos = DrawMgr.CurrentCamera.Position
				- tilemap.Offset
				- DrawMgr.CurrentCamera.Offset / DrawMgr.CurrentCamera.Zoom;
			
			var scaledCameraSize = DrawMgr.CurrentCamera.Size / DrawMgr.CurrentCamera.Zoom;
			var startX = (int)(offsetCameraPos.X / tilemap.TileWidth) - tilemap.Padding;
			var startY = (int)(offsetCameraPos.Y / tilemap.TileHeight) - tilemap.Padding;

			var endX = startX + (int)scaledCameraSize.X / tilemap.TileWidth + tilemap.Padding + 2; // One for mama, one for papa.
			var endY = startY + (int)scaledCameraSize.Y / tilemap.TileHeight + tilemap.Padding + 2;

			// It's faster to determine bounds for whole region.

			// Bounding.
			if (startX < 0)
			{
				startX = 0;
			}
			if (startY < 0)
			{
				startY = 0;
			}
			if (endX >= tilemap.Width)
			{
				endX = tilemap.Width - 1;
			}
			if (endY >= tilemap.Height)
			{
				endY = tilemap.Height - 1;
			}
			// Bounding.

			// Telling whatever is waiting to be drawn to draw itself.
			// If pipeline mode is not switched, drawing raw sprite batch may interfere with primitives.
			DrawMgr.SwitchPipelineMode(PipelineMode.Sprites); 
			//Console.WriteLine(tilemap.Offset);
			for(var y = startY; y < endY; y += 1)
			{
				for(var x = startX; x < endX; x += 1)
				{
					// It's fine to use unsafe get, since we know for sure, we are in bounds.
					var tile = tilemap.GetTileUnsafe(x, y);

					if (!tile.IsBlank)
					{
						var tilesetTile = tile.GetTilesetTile();
						
						if (tilesetTile != null)
						{
							var flip = SpriteEffects.None;
							var offset = Vector2.UnitY * (tilesetTile.Frame.Height - tilemap.TileHeight);
							
							// A bunch of Tiled magic.
							if (tile.FlipHor)
							{
								flip ^= SpriteEffects.FlipHorizontally;
							}

							if (tile.FlipVer)
							{	
								flip ^= SpriteEffects.FlipVertically;	
							}
							// A bunch of Tiled magic.

							// Mass-drawing srpites with spritebatch is a bit faster.
							
							DrawMgr.Batch.Draw(
								tilesetTile.Frame.Texture,
								tilemap.Offset + new Vector2(tilemap.TileWidth * x, tilemap.TileHeight * y) - offset + tile.Tileset.Offset,
								tilesetTile.Frame.TexturePosition,
								Color.White,
								0,
								Vector2.Zero,
								Vector2.One,
								flip,
								0
							);
						}
					}

				}
			}

			DrawMgr.CurrentColor = Color.Black * 0.3f;
			for(var y = startY; y < endY; y += 1)
			{
				DrawMgr.DrawLine(tilemap.TileWidth * startX, tilemap.TileHeight * y, tilemap.TileWidth * endX, tilemap.TileHeight * y);
			}
			for(var x = startX; x < endX; x += 1)
			{
				DrawMgr.DrawLine(tilemap.TileWidth * x, tilemap.TileHeight * startY, tilemap.TileWidth * x, tilemap.TileHeight * endY);
			}

		}

	}
}

