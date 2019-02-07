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
				
				var colliderTileset = new ColliderTileset(
					basicTileset.Tiles, 
					basicTileset.Offset,
					GetColliders(tilesets[i]), 
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
				
				var tilemap = new ColliderTilemapComponent(tileLayer.Width, tileLayer.Height, tileLayer.TileWidth, tileLayer.TileHeight);
				for(var y = 0; y < tilemap.Height; y += 1)	
				{
					for(var x = 0; x < tilemap.Width; x += 1)
					{
						var tileIndex = tileLayer.Tiles[x][y].GID;
						
						tilemap.SetTile(
							x, y, 
							new ColliderTile(
								tileIndex, 
								(ColliderTileset)GetTilesetFromTileIndex(tileIndex, tilesets),
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


		ICollider[] GetColliders(TiledMapTileset tileset)
		{
			var colliders = new List<ICollider>();

			foreach(var tile in tileset.Tiles)
			{
				if (tile.Objects.Length > 0 && tile.Objects[0] is TiledRectangleObject)
				{
					var rectangle = tile.Objects[0];
					var collider = new RectangleCollider();
					collider.Size = rectangle.Size;
					//TODO: Add offset. Somehow.
					colliders.Add(collider);
				}
				else
				{
					colliders.Add(null);
				}
			}
			return colliders.ToArray();
		}

	}
}
