using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils.Tilemaps;
using Monofoxe.Tiled.MapStructure;
using Monofoxe.Tiled;
using Monofoxe.Demo.GameLogic.Tiles;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Tiled.MapStructure.Objects;
using Monofoxe.Demo.GameLogic.Entities;

namespace Monofoxe.Demo.GameLogic
{
	public class ColliderMap : Map
	{
		public ColliderMap(TiledMap tiledMap) : base(tiledMap) {}
		
		protected override List<Tileset> BuildTilesets(TiledMapTileset[] tilesets)
		{
			var convertedBasicTilesets = base.BuildTilesets(tilesets);

			var convertedColliderTilesets = new List<Tileset>();

			for(var i = 0; i < convertedBasicTilesets.Count; i += 1)
			{
				var basicTileset = convertedBasicTilesets[i];
				
				var colliderTileset = new Tileset(
					ConvertTiles(tilesets[i], convertedBasicTilesets[i]), 
					basicTileset.Offset, 
					basicTileset.StartingIndex
				);
				convertedColliderTilesets.Add(colliderTileset);
			}

			return convertedColliderTilesets;
		}


		protected override void BuildTileLayers(List<Tileset> tilesets)
		{
			foreach(var tileLayer in TiledMap.TileLayers)
			{
				var layer = MapScene.CreateLayer(tileLayer.Name);
				layer.Priority = GetLayerPriority(tileLayer);
				
				var tilemap = new BasicTilemapComponent(tileLayer.Width, tileLayer.Height, tileLayer.TileWidth, tileLayer.TileHeight);
				for(var y = 0; y < tilemap.Height; y += 1)	
				{
					for(var x = 0; x < tilemap.Width; x += 1)
					{
						var tileIndex = tileLayer.Tiles[x][y].GID;
						
						tilemap.SetTile(
							x, y, 
							new BasicTile(
								tileIndex, 
								GetTilesetFromTileIndex(tileIndex, tilesets),
								tileLayer.Tiles[x][y].FlipHor,
								tileLayer.Tiles[x][y].FlipVer
							)
						);
					}
				}
				
				var tilemapEntity = new Entity(layer, "ColliderTilemap");
				tilemapEntity.AddComponent(tilemap);
				var collider = new TilemapCollider();
				collider.Tilemap = tilemap;
				collider.Size = new Vector2(tilemap.Width * tilemap.TileWidth, tilemap.Height * tilemap.TileHeight);
				var solid = new SolidComponent();
				solid.Collider = collider;
				tilemapEntity.AddComponent(solid);
				tilemapEntity.AddComponent(new PositionComponent(Vector2.Zero));

			}
		}


		ITilesetTile[] ConvertTiles(TiledMapTileset tiledTileset, Tileset tileset)
		{
			var tilesetTiles = new List<ITilesetTile>();

			for(var i = 0; i < tileset.Tiles.Length; i += 1)
			{
				ICollider collider = null;

				var tiledTile = tiledTileset.Tiles[i];
				var colliderOffset = Vector2.Zero;
				
				if (tiledTile.Objects.Length > 0 && tiledTile.Objects[0] is TiledRectangleObject)
				{
					var rectangle = tiledTile.Objects[0];
					
					if (rectangle.Type == "rectangle")
					{
						collider = new RectangleCollider();
						collider.Size = rectangle.Size;
					}
					if (rectangle.Type == "platform")
					{
						collider = new PlatformCollider();
						collider.Size = rectangle.Size;
					}
					// Here we need to flip y in the offset, because tiles are draw with origin in bottom left corner, 
					// but colliders take origin as top left corner. Why? Ask Tiled dev.
					colliderOffset = rectangle.Position + tileset.Offset * new Vector2(1, -1) + rectangle.Size / 2;
				}
				
				var type = TilesetTileCollisionType.None;
				//TODO: Cleanup.
				try
				{
					Console.WriteLine(tiledTile.Properties["type"]);
					type = (TilesetTileCollisionType)Enum.Parse(typeof(TilesetTileCollisionType), tiledTile.Properties["type"]);
				}
				catch(Exception e) {}

				var tilesetTile = new ColliderTilesetTile(
					tileset.Tiles[i].Frame,
					collider,
					type,
					colliderOffset
				);
				tilesetTiles.Add(tilesetTile);
			}

			return tilesetTiles.ToArray();
		}

	}
}
