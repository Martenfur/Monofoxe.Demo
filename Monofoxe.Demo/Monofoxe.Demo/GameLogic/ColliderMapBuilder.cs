using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Tiles;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils.Tilemaps;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure;
using Monofoxe.Tiled.MapStructure.Objects;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;

namespace Monofoxe.Demo.GameLogic
{
	/// <summary>
	/// Custom map builder. Adds tilemap collision functionality to regular tilemap.
	/// </summary>
	public class ColliderMapBuilder : MapBuilder
	{
		private const string _typeProperty = "type";
		private const string _rectangleName = "rectangle";
		private const string _platformName = "platform";
		

		public ColliderMapBuilder(TiledMap tiledMap) : base(tiledMap) {}

		public override void Build()
		{
			base.Build();

			// Putting a background.
			var l = MapScene.CreateLayer("backdrop");
			l.Priority = 10000;

			new Background(l);
			// Putting a background.
		}

		protected override List<Tileset> BuildTilesets(TiledMapTileset[] tilesets)
		{
			// Letting basic tileset builder do its stuff.
			var convertedBasicTilesets = base.BuildTilesets(tilesets);

			var convertedColliderTilesets = new List<Tileset>();

			// Now we got tilesets with basic tiles, which we need to convert into collider tile.

			for(var i = 0; i < convertedBasicTilesets.Count; i += 1)
			{
				var basicTileset = convertedBasicTilesets[i];
				
				// Essentially cloning a tileset with new set of converted tiles.
				// Goal here is to make tilemap work with collision system. 
				// All collider data is stored in tileset tiles, which are later assigned to tilemap tiles.
				// To see collision data, we just need ti take tilemap tile and look at its assigned tileset tile. 

				var colliderTileset = new Tileset(
					ConvertTiles(tilesets[i], convertedBasicTilesets[i]), // All the magic happens here.
					basicTileset.Offset, 
					basicTileset.StartingIndex
				);
				convertedColliderTilesets.Add(colliderTileset);
			}

			return convertedColliderTilesets;
		}


		protected override List<Layer> BuildTileLayers(List<Tileset> tilesets)
		{
			// Letting basic layer builder do its stuff.
			var layers = base.BuildTileLayers(tilesets);

			// Now we need to add position and collider components to entity to make it count as a solid.
			foreach(var layer in layers)
			{
				// Getting list of all tilemaps on this layer.
				var tilemaps = layer.GetEntityListByComponent<BasicTilemapComponent>();
				
				foreach(var tilemap in tilemaps)
				{
					var tilemapComponent = tilemap.GetComponent<BasicTilemapComponent>();
					
					// Making collider.
					var collider = new TilemapCollider();
					collider.Tilemap = tilemapComponent;
					collider.Size = new Vector2(
						tilemapComponent.Width * tilemapComponent.TileWidth, 
						tilemapComponent.Height * tilemapComponent.TileHeight
					);
					var solid = new SolidComponent();
					solid.Collider = collider;
					// Making collider.

					tilemap.AddComponent(solid);
					tilemap.AddComponent(new PositionComponent(tilemapComponent.Offset));
				}
			}

			return layers;
		}



		/// <summary>
		/// Converts basic tilesets into collider tilesets using data from Tiled structures.
		/// </summary>
		ITilesetTile[] ConvertTiles(TiledMapTileset tiledTileset, Tileset tileset)
		{
			var tilesetTiles = new List<ITilesetTile>();

			for(var i = 0; i < tileset.Tiles.Length; i += 1)
			{
				var tiledTile = tiledTileset.Tiles[i];
				
				// Getting collision mode of a tile.
				var mode = TilesetTileCollisionMode.None;
				try
				{
					mode = (TilesetTileCollisionMode)Enum.Parse(typeof(TilesetTileCollisionMode), tiledTile.Properties[_typeProperty]);
				}
				catch(Exception) {}
				// Getting collision mode of a tile.

				ColliderTilesetTile tilesetTile;
				
				if (mode == TilesetTileCollisionMode.Custom)
				{
					// Getting custom collider.
					var colliderOffset = Vector2.Zero;
					var collider = GetCollider(tiledTile, tileset, ref colliderOffset);
					// Getting custom collider.

					tilesetTile = new ColliderTilesetTile(tileset.Tiles[i].Frame, mode, collider, colliderOffset);
				}
				else
				{
					// No need to bother with getting custom colliders here.
					tilesetTile = new ColliderTilesetTile(tileset.Tiles[i].Frame, mode);
				}
				tilesetTiles.Add(tilesetTile);
			}

			return tilesetTiles.ToArray();
		}

		/// <summary>
		/// Returns collider for a given tile and sets an offset.
		/// </summary>
		ICollider GetCollider(TiledMapTilesetTile tiledTile, Tileset tileset, ref Vector2 colliderOffset)
		{
			ICollider collider = null;
			if (tiledTile.Objects.Length > 0 && tiledTile.Objects[0] is TiledRectangleObject)
			{
				var obj = tiledTile.Objects[0];
					
				if (obj.Type.ToLower() == _rectangleName)
				{
					collider = new RectangleCollider();
					collider.Size = obj.Size;
				}
				if (obj.Type.ToLower() == _platformName)
				{
					collider = new PlatformCollider();
					collider.Size = obj.Size;
				}
				// Here we need to flip y in the offset, because tiles are draw with origin in bottom left corner, 
				// but colliders take origin as top left corner. Why? Ask Tiled dev.
				colliderOffset = obj.Position + tileset.Offset * new Vector2(1, -1) + obj.Size / 2;
			}
			return collider;
		}

	}
}
