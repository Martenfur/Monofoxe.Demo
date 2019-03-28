using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Tiled;
using Monofoxe.Tiled.MapStructure.Objects;


namespace Monofoxe.Demo.MapEntityFactories
{
	public class ButtonFactory : ITiledEntityFactory
	{
		public string Tag => "button";

		public Entity Make(TiledObject obj, Layer layer, MapBuilder map)
		{
			var tile = (TiledTileObject)obj;
			
			// Origin point for tile object is in the bottom left corner,
			// but rotation point is in the center.
			var matrix = Matrix.CreateTranslation(new Vector3(Button.Size.X, 0, 0) / 2) *
				Matrix.CreateRotationZ(MathHelper.ToRadians(tile.Rotation));	
			
			var position = new Vector2(matrix.Translation.X, matrix.Translation.Y) + tile.Position;
			
			var button = new Button(position, tile.Rotation, layer);
			
			if (tile.Properties["link"] != "none")
			{
				button.AddComponent(new LinkComponent(tile.Properties["link"], true));
			}

			return button;
		}
	}
}
