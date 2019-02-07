using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Tiles;

namespace Monofoxe.Demo.GameLogic.Collisions
{
	public class TilemapCollider : ICollider
	{
		public ColliderType ColliderType => ColliderType.Tilemap;
		public Vector2 Position {get; set;}
		public Vector2 PreviousPosition {get; set;}
		public ColliderTilemapComponent Tilemap;

		public Vector2 Size {get; set;}

		public object Clone() => this;
	}
}
