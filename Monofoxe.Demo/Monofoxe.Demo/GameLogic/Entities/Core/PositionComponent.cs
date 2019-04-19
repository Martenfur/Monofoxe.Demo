using Microsoft.Xna.Framework;
using Monofoxe.Engine.ECS;

namespace Monofoxe.Demo.GameLogic.Entities.Core
{
	/// <summary>
	/// Basic position component. 
	/// </summary>
	public class PositionComponent : Component
	{
		
		/// <summary>
		/// Entity position on the scene.
		/// </summary>
		public Vector2 Position;

		/// <summary>
		/// Entity position on the scene in previous frame.
		/// </summary>
		public Vector2 PreviousPosition;
		
		/// <summary>
		/// Starting entity position on the scene.
		/// </summary>
		public Vector2 StartingPosition;


		public PositionComponent(Vector2 position)
		{
			Position = position;
			StartingPosition = position;
			PreviousPosition = position;
		}
	}
}
