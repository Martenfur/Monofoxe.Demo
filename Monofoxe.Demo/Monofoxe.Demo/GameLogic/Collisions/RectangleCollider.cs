using Microsoft.Xna.Framework;

namespace Monofoxe.Demo.GameLogic.Collisions
{
	/// <summary>
	/// Basic rectangle collider.
	/// </summary>
	public class RectangleCollider : ICollider
	{
		public ColliderType ColliderType => ColliderType.Rectangle;
		public Vector2 Position {get; set;}
		public Vector2 PreviousPosition {get; set;}

		public Vector2 Size {get; set;}

		public bool Enabled {get; set;} = true;
		
		public object Clone()
		{
			var o = new RectangleCollider();
			o.Position = Position;
			o.PreviousPosition = o.PreviousPosition;
			o.Size = Size;
			o.Enabled = Enabled;

			return o;
		}
	}
}
