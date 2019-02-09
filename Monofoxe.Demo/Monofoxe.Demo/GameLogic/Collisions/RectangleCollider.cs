using Microsoft.Xna.Framework;

namespace Monofoxe.Demo.GameLogic.Collisions
{
	/// <summary>
	/// Basic rectangle collider.
	/// </summary>
	public struct RectangleCollider : ICollider
	{
		public ColliderType ColliderType => ColliderType.Rectangle;
		public Vector2 Position {get; set;}
		public Vector2 PreviousPosition {get; set;}

		public Vector2 Size {get; set;}

		public object Clone() => this;
	}
}
