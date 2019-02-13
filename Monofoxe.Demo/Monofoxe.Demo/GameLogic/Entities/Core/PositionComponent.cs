using Microsoft.Xna.Framework;
using Monofoxe.Engine.Converters;
using Monofoxe.Engine.ECS;
using Newtonsoft.Json;

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
		[JsonConverter(typeof(Vector2Converter))]
		public Vector2 Position;

		/// <summary>
		/// Entity position on the scene in previous frame.
		/// </summary>
		public Vector2 PreviousPosition;
		
		public PositionComponent(Vector2 position)
		{
			Position = position;
			PreviousPosition = position;
		}

		public override object Clone()
		{
			var c = new PositionComponent(Position);
			c.Position = Position;
			c.PreviousPosition = PreviousPosition;

			return c;
		}
	}
}
