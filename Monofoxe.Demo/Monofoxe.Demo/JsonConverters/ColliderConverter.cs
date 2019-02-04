using System;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Engine.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Monofoxe.Demo.JsonConverters
{
	/// <summary>
	/// ICollider JSON converter.
	/// JSON format for colliders is: {type: "type", w: 0, h: 0, additional_attributes}.
	/// </summary>
	public class ColliderConverter : BasicConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var o = JObject.Load(reader);
	
			var w = GetFloat(o, "w");
			var h = GetFloat(o, "h");
			
			var type = GetString(o, "type");

			ICollider collider = null;

			if (type == "rectangle")
			{
				var rectangle = new RectangleCollider
				{
					Size = new Vector2(w, h)
				};
				collider = rectangle;
			}
			if (type == "platform")
			{
				var platform = new PlatformCollider
				{
					Size = new Vector2(w, h)
				};
				collider = platform;
			}

			if (collider == null)
			{
				throw new Exception("Unknown collider type!");
			}

			return collider;
		}
	}
}
