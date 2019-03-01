using System;
using Microsoft.Xna.Framework;

namespace Monofoxe.Demo.GameLogic.Collisions
{
	public interface ICollider : ICloneable
	{
		ColliderType ColliderType {get;}
		
		/// <summary>
		/// Center point at current frame.
		/// NOTE: This property has to be set manually 
		/// before checking collision.
		/// </summary>
		Vector2 Position {get; set;}

		/// <summary>
		/// Center point at previous frame.
		/// NOTE: This property has to be set manually 
		/// before checking collision.
		/// </summary>
		Vector2 PreviousPosition {get; set;}

		/// <summary>
		/// AABB size.
		/// </summary>
		Vector2 Size {get; set;}


		bool Enabled {get; set;}
	}
}
